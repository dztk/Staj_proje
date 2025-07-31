using Microsoft.EntityFrameworkCore;
using Proje.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace Proje.Services
{
    public class ToPdfService   
    {
        private readonly AppDbContext _context;  

        public ToPdfService(AppDbContext context)       
        {
            _context = context;
        }

        public byte[] CreateKapanisRaporuPdf(int isEmriId)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "iett.png");
            var logoBytes = System.IO.File.ReadAllBytes(logoPath);

            var isEmri = _context.IsEmri 
                .Include (i => i.Arac)
                .Include(i => i.Durak)
                 .FirstOrDefault(i => i.Id == isEmriId);



            if (isEmri == null)
                throw new Exception("İş emri bulunamadı.");

            var teknisyen = _context.Personel.FirstOrDefault(p => p.Id == isEmri.PersonelId);
            var sef = _context.Personel.FirstOrDefault(p => p.Unvan == "Şef");

            var yapilanIslem = _context.BakimHareket.FirstOrDefault(b => b.IsEmriId == isEmriId);
            var parcalar = _context.ParcaIsEmri
                .Where(p => p.IsEmriId == isEmriId)
                .Select(p => new
                {
                    Ad = p.Parca.Ad,
                    p.Miktar,
                    p.Parca.BirimFiyat,
                    Tutar = p.Miktar * p.Parca.BirimFiyat
                }).ToList();

            var belgeTarihi = DateTime.Now.ToShortDateString();
            var dokumanNo = $"D-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Content().Column(col =>
                    {
                        col.Item().Row(row =>
                        {
                            row.ConstantItem(100).Height(100).Image(logoBytes);

                            row.RelativeItem().Column(header =>
                            {
                                header.Item().Element(x => x.AlignCenter().PaddingBottom(10)).Text("Bakım&Arıza Takip Yönetimi Kapanış Raporu").FontSize(20).Bold();
                                //col.Item().PaddingTop(20);
                                header.Item().Element(x => x.AlignCenter().PaddingBottom(15)).Text("İstanbul Elektrik Tramvay ve Tünel İşletmeleri").FontSize(14);
                            });
                        });

                        col.Item().Element(x => x.PaddingTop(5)).Text($"Doküman No: {dokumanNo}");
                        col.Item().Element(x => x.PaddingBottom(10)).Text($"Belge Oluşturulma Tarihi: {belgeTarihi}");

                        col.Item().Element(x => x.PaddingBottom(5)).Text("Genel Bilgiler").Bold().FontSize(14);
                        col.Item().PaddingTop(20);

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Cell().Element(CellStyle).Text("İş Emri No");
                            table.Cell().Element(CellStyle).Text(isEmri.Id.ToString());

                            table.Cell().Element(CellStyle).Text("Kayıt Tarihi");
                            table.Cell().Element(CellStyle).Text(isEmri.AcilisTarihi.ToShortDateString());

                            table.Cell().Element(CellStyle).Text("Kapanış Tarihi");
                            table.Cell().Element(CellStyle).Text(isEmri.KapanisTarihi?.ToShortDateString() ?? " ");

                            //table.Cell().Element(CellStyle).Text("Araç/Durak Bilgisi");
                            //table.Cell().Element(CellStyle).Text(isEmri.Tip == Enums.IsEmriTipi.Arac ? isEmri.Arac?.Plaka : isEmri.Durak?.Ad);

                            table.Cell().Element(CellStyle).Text("Araç/Durak Bilgisi");

                            string aracDurakBilgisi = "";

                            if (isEmri.Tip == Enums.IsEmriTipi.Arac && isEmri.Arac != null)
                                aracDurakBilgisi = $"Araç - {isEmri.Arac.Plaka} / {isEmri.Arac.KapiNo}";
                                //"Araç - " + isEmri.Arac.Plaka;
                            else if (isEmri.Tip == Enums.IsEmriTipi.Durak && isEmri.Durak != null)
                                aracDurakBilgisi = $"Durak - {isEmri.Durak.Ad} / {isEmri.Durak.Kod}";
                            //"Durak - " + isEmri.Durak.Ad;

                            table.Cell().Element(CellStyle).Text(aracDurakBilgisi);


                            table.Cell().Element(CellStyle).Text("Teknisyen");
                            table.Cell().Element(CellStyle).Text(teknisyen?.AdSoyad ?? " ");

                            table.Cell().Element(CellStyle).Text("Şef");
                            table.Cell().Element(CellStyle).Text(sef?.AdSoyad ?? " ");
                        });

                        col.Item().Element(x => x.PaddingTop(20).PaddingBottom(5)).Text("Yapılan İşlemler").Bold().FontSize(14);
                        col.Item().PaddingTop(10);

                        col.Item().Text(string.IsNullOrWhiteSpace(yapilanIslem?.YapilanIslem) ? " " : yapilanIslem.YapilanIslem);

                        col.Item().PaddingTop(30);

                        col.Item().Element(x => x.PaddingTop(20).PaddingBottom(5)).Text("Kullanılan Parçalar").Bold().FontSize(14);
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.ConstantColumn(50);
                                columns.ConstantColumn(80);
                                columns.ConstantColumn(80);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Parça Adı").Bold();
                                header.Cell().Element(CellStyle).Text("Miktar").Bold();
                                header.Cell().Element(CellStyle).Text("Birim Fiyat (TL)").Bold();
                                header.Cell().Element(CellStyle).Text("Tutar (TL)").Bold();
                            });

                            foreach (var parca in parcalar)
                            {
                                table.Cell().Element(CellStyle).Text(parca.Ad);
                                table.Cell().Element(CellStyle).Text(parca.Miktar.ToString());
                                table.Cell().Element(CellStyle).Text(parca.BirimFiyat.ToString("N2"));
                                table.Cell().Element(CellStyle).Text(parca.Tutar.ToString("N2"));
                            }

                            for (int i = parcalar.Count; i < 3; i++)
                            {
                                table.Cell().Element(CellStyle).Text(" ");
                                table.Cell().Element(CellStyle).Text(" ");
                                table.Cell().Element(CellStyle).Text(" ");
                                table.Cell().Element(CellStyle).Text(" ");
                            }
                        });

                        //col.Item().PaddingTop(40).AlignRight().Text("Ad Soyad: ___________");
                        //col.Item().AlignRight().Text("İmza: ___________");
                        col.Item().PaddingTop(40);

                        col.Item().PaddingLeft(300).Text("Ad Soyad: ");
                        col.Item().PaddingTop(20);

                        col.Item().PaddingLeft(300).Text("İmza: ");
                    });
                });
            });

            try
            {
                return pdf.GeneratePdf();
            }
            catch (Exception ex)
            {
                throw new Exception("PDF oluşturulurken hata oluştu: " + ex.Message, ex);
            }
        }

        private IContainer CellStyle(IContainer container)
        {
            return container
                .PaddingVertical(5)
                .PaddingHorizontal(5);
               // .BorderBottom(1)
               // .BorderColor("#E0E0E0");
        }
    }
}