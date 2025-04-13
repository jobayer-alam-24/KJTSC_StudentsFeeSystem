using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using StudentsFeeSystem.Models;
using System;
using System.IO;
using System.Linq;

namespace StudentsFeeSystem.Services
{
    public static class PdfService
    {
        private static string ConvertToBengaliNumber(string input)
        {
            var englishDigits = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            var bengaliDigits = new[] { '০', '১', '২', '৩', '৪', '৫', '৬', '৭', '৮', '৯' };

            var result = input.Select(c =>
            {
                var index = Array.IndexOf(englishDigits, c);
                return index >= 0 ? bengaliDigits[index] : c;
            });

            return new string(result.ToArray());
        }

        public static byte[] GenerateReceipts(Student student)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(14f, 21f, Unit.Centimetre);
                    page.Margin(1.5f, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    var watermarkPath = "wwwroot/assets/img/bdlogo.png";
                    var modifiedWatermarkPath = "wwwroot/assets/img/bdlogo_semi_transparent.png";
                    var modifiedWatermarkPath2 = "wwwroot/assets/img/bdlogo_semi_transparent_2.png";

                   
                    if (!File.Exists(modifiedWatermarkPath))
                    {
                        ImageHelper.CreateSemiTransparentImage(watermarkPath, modifiedWatermarkPath, 0.01f);
                    }

                    
                    if (!File.Exists(modifiedWatermarkPath2))
                    {
                        ImageHelper.CreateSemiTransparentImage(watermarkPath, modifiedWatermarkPath2, 0.2f);
                    }

                    byte[] imageBytes = File.Exists(modifiedWatermarkPath) ? File.ReadAllBytes(modifiedWatermarkPath) : null;
                    byte[] imageBytes2 = File.Exists(modifiedWatermarkPath2) ? File.ReadAllBytes(modifiedWatermarkPath2) : null;

                    page.Content().Layers(layers =>
                    {
                        
                        if (imageBytes != null)
                        {
                            layers.Layer()
                                .AlignCenter()
                                .AlignMiddle()
                                .Image(imageBytes, ImageScaling.FitArea);
                        }

                        // Main content layer
                        layers.PrimaryLayer().Column(col =>
                        {
                            col.Item().Element(header =>
                            {
                                header.Row(row =>
                                {
                                    row.ConstantColumn(50).Element(logo =>
                                    {
                                        var logoPath = "wwwroot/assets/img/bteblog.png";
                                        if (File.Exists(logoPath))
                                        {
                                            var logoBytes = File.ReadAllBytes(logoPath);
                                            logo.Element(x => x
                                                .AlignCenter()
                                                .Padding(2)
                                                .Width(40)
                                                .Height(40)
                                                .Image(logoBytes, ImageScaling.FitArea)
                                            );
                                        }
                                    });

                                    row.RelativeColumn().Column(innerCol =>
                                    {
                                        innerCol.Spacing(3);
                                        innerCol.Item().Text("গণপ্রজাতন্ত্রী বাংলাদেশ").FontSize(12).Bold().AlignCenter();
                                        innerCol.Item().Text("কালিগঞ্জ সরকারি টেকনিক্যাল স্কুল ও কলেজ").FontSize(11).Bold().AlignCenter();
                                        innerCol.Item().Text("কালিগঞ্জ, সাতক্ষীরা").FontSize(10).Italic().AlignCenter();
                                        innerCol.Item().PaddingVertical(4).LineHorizontal(1);
                                        innerCol.Item().Text("শিক্ষার্থীর ফি রসিদ").FontSize(14).Bold().AlignCenter().ParagraphSpacing(8);
                                    });

                                    row.ConstantColumn(50).Element(logo =>
                                    {
                                        var logoPath = "wwwroot/assets/img/bdlogo.png";
                                        if (File.Exists(logoPath))
                                        {
                                            var logoBytes = File.ReadAllBytes(logoPath);
                                            logo.Element(x => x
                                                .AlignCenter()
                                                .Padding(2)
                                                .Width(40)
                                                .Height(40)
                                                .Image(logoBytes, ImageScaling.FitArea)
                                            );
                                        }
                                    });
                                });
                            });

                            col.Item().LineHorizontal(1);
                            col.Item().Text($"নাম: {student.Name}").SemiBold();
                            col.Item().Text($"পিতার নাম: {student.FathersName}");
                            col.Item().Text($"রোল নম্বর: {ConvertToBengaliNumber(student.Roll.ToString())}");
                            col.Item().Text($"শ্রেণী: {ConvertToBengaliNumber(student.Class.ToString())}");
                            col.Item().Text($"বিভাগ: {GetDepartmentName(student.Department)}");
                            col.Item().Text($"ফি পরিশোধ: {(student.HasPaid ? "হ্যাঁ" : "না")}").Bold();
                            col.Item().Text($"পরিশোধিত: {ConvertToBengaliNumber(student.Fee.ToString())} টাকা").Bold();
                            col.Item().Text($"তারিখ: {ConvertToBengaliNumber(student.Date.ToString("dd-MM-yyyy"))}");
                            col.Item().LineHorizontal(1);
                            col.Item().Text("আপনার পরিশোধের জন্য ধন্যবাদ!").AlignCenter().FontSize(12);
                            col.Item().PaddingVertical(2);
                            col.Item().Text("হিসাবরক্ষকের স্বাক্ষর: ____________________").AlignRight();

                            col.Item().Text("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -").AlignCenter().FontSize(10);
                            col.Item().PaddingVertical(2);

                            // Add second watermark in the background layer for the second receipt
                            if (imageBytes2 != null)
                            {
                                layers.Layer()
                                    .AlignCenter()
                                    .AlignMiddle()
                                    .Image(imageBytes2, ImageScaling.FitArea);
                            }

                            col.Item().Element(header =>
                            {
                                header.Row(row =>
                                {
                                    row.ConstantColumn(50).Element(logo =>
                                    {
                                        var logoPath = "wwwroot/assets/img/bteblog.png";
                                        if (File.Exists(logoPath))
                                        {
                                            var logoBytes = File.ReadAllBytes(logoPath);
                                            logo.Element(x => x
                                                .AlignCenter()
                                                .Padding(2)
                                                .Width(40)
                                                .Height(40)
                                                .Image(logoBytes, ImageScaling.FitArea)
                                            );
                                        }
                                    });

                                    row.RelativeColumn().Column(innerCol =>
                                    {
                                        innerCol.Spacing(3);
                                        innerCol.Item().Text("গণপ্রজাতন্ত্রী বাংলাদেশ").FontSize(12).Bold().AlignCenter();
                                        innerCol.Item().Text("কালিগঞ্জ সরকারি টেকনিক্যাল স্কুল ও কলেজ").FontSize(11).Bold().AlignCenter();
                                        innerCol.Item().Text("কালিগঞ্জ, সাতক্ষীরা").FontSize(10).Italic().AlignCenter();
                                        innerCol.Item().PaddingVertical(4).LineHorizontal(1);
                                        innerCol.Item().Text("অফিস ফি রসিদ").FontSize(14).Bold().AlignCenter().ParagraphSpacing(8);
                                    });

                                    row.ConstantColumn(50).Element(logo =>
                                    {
                                        var logoPath = "wwwroot/assets/img/bdlogo.png";
                                        if (File.Exists(logoPath))
                                        {
                                            var logoBytes = File.ReadAllBytes(logoPath);
                                            logo.Element(x => x
                                                .AlignCenter()
                                                .Padding(2)
                                                .Width(40)
                                                .Height(40)
                                                .Image(logoBytes, ImageScaling.FitArea)
                                            );
                                        }
                                    });
                                });
                            });

                            col.Item().LineHorizontal(1);
                            col.Item().Text($"নাম: {student.Name}").SemiBold();
                            col.Item().Text($"পিতার নাম: {student.FathersName}");
                            col.Item().Text($"রোল নম্বর: {ConvertToBengaliNumber(student.Roll.ToString())}");
                            col.Item().Text($"শ্রেণী: {ConvertToBengaliNumber(student.Class.ToString())}");
                            col.Item().Text($"বিভাগ: {GetDepartmentName(student.Department)}");
                            col.Item().Text($"ফি পরিশোধ: {(student.HasPaid ? "হ্যাঁ" : "না")}").Bold();
                            col.Item().Text($"পরিশোধিত: {ConvertToBengaliNumber(student.Fee.ToString())} টাকা").Bold();
                            col.Item().Text($"তারিখ: {ConvertToBengaliNumber(student.Date.ToString("dd-MM-yyyy"))}");
                            col.Item().LineHorizontal(1);
                            col.Item().Text("আপনার পরিশোধের জন্য ধন্যবাদ!").AlignCenter().FontSize(12);
                            col.Item().PaddingVertical(5);
                            col.Item().Text("হিসাবরক্ষকের স্বাক্ষর: ____________________").AlignRight();
                        });
                    });
                });
            });

            return document.GeneratePdf();
        }

        private static string GetDepartmentName(Department department)
        {
            var type = typeof(Department);
            var memInfo = type.GetMember(department.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);

            return attributes.Length > 0
                ? ((System.ComponentModel.DescriptionAttribute)attributes[0]).Description
                : department.ToString();
        }
    }
}
