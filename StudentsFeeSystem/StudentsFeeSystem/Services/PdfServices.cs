using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using StudentsFeeSystem.Models;
using System;
using System.IO;

namespace StudentsFeeSystem.Services
{
    public static class PdfService
    {
        public static byte[] GenerateReceipts(Student student)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(14f, 21f, Unit.Centimetre);
                    page.Margin(1.5f, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Content().Column(col =>
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
                                        var imageBytes = File.ReadAllBytes(logoPath);
                                        logo.Element(x => x
                                            .AlignCenter()
                                            .Padding(2)
                                            .Width(40)
                                            .Height(40)
                                            .Image(imageBytes, ImageScaling.FitArea)
                                        );
                                    }
                                });

                                row.RelativeColumn().Column(innerCol =>
                                {
                                    innerCol.Spacing(3);
                                    innerCol.Item().Text("Government of the People's Republic of Bangladesh")
                                                  .FontSize(12).Bold().AlignCenter();
                                    innerCol.Item().Text("Kaligonj Government Technical School and College")
                                                  .FontSize(11).Bold().AlignCenter();
                                    innerCol.Item().Text("Kaligonj, Satkhira")
                                                  .FontSize(10).Italic().AlignCenter();
                                    innerCol.Item().PaddingVertical(4).LineHorizontal(1);
                                    innerCol.Item().Text("Student's Fee Receipt")
                                                  .FontSize(14).Bold().AlignCenter().ParagraphSpacing(8);
                                });

                                row.ConstantColumn(50).Element(logo =>
                                {
                                    var logoPath = "wwwroot/assets/img/bdlogo.png";
                                    if (File.Exists(logoPath))
                                    {
                                        var imageBytes = File.ReadAllBytes(logoPath);
                                        logo.Element(x => x
                                            .AlignCenter()
                                            .Padding(2)
                                            .Width(40)
                                            .Height(40)
                                            .Image(imageBytes, ImageScaling.FitArea)
                                        );
                                    }
                                });
                            });
                        });

                        col.Item().LineHorizontal(1);
                        col.Item().Text($"Name: {student.Name}").SemiBold();
                        col.Item().Text($"Father's Name: {student.FathersName}");
                        col.Item().Text($"Roll Number: {student.Roll}");
                        col.Item().Text($"Class: {student.Class}");
                        col.Item().Text($"Department: {GetDepartmentName(student.Department)}");
                        col.Item().Text($"Fee Paid: {(student.HasPaid ? "Yes" : "No")}").Bold();
                        col.Item().Text($"Paid: {student.Fee} TK").Bold();
                        col.Item().Text($"Receipt Date: {student.Date.ToShortDateString()}");
                        col.Item().LineHorizontal(1);
                        col.Item().Text("Thank you for your payment!").AlignCenter().FontSize(12);

                        col.Item().PaddingVertical(6);
                        col.Item().Text("Accountant's Signature: ____________________").AlignRight();

                        // Adding separator line here
                        col.Item().Text("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -")
         .AlignCenter()
         .FontSize(12);

                        col.Item().PaddingVertical(6);

                        col.Item().Element(header =>
                        {
                            header.Row(row =>
                            {
                                row.ConstantColumn(50).Element(logo =>
                                {
                                    var logoPath = "wwwroot/assets/img/bteblog.png";
                                    if (File.Exists(logoPath))
                                    {
                                        var imageBytes = File.ReadAllBytes(logoPath);
                                        logo.Element(x => x
                                            .AlignCenter()
                                            .Padding(2)
                                            .Width(40)
                                            .Height(40)
                                            .Image(imageBytes, ImageScaling.FitArea)
                                        );
                                    }
                                });

                                row.RelativeColumn().Column(innerCol =>
                                {
                                    innerCol.Spacing(3);
                                    innerCol.Item().Text("Government of the People's Republic of Bangladesh")
                                                  .FontSize(12).Bold().AlignCenter();
                                    innerCol.Item().Text("Kaligonj Government Technical School and College")
                                                  .FontSize(11).Bold().AlignCenter();
                                    innerCol.Item().Text("Kaligonj, Satkhira")
                                                  .FontSize(10).Italic().AlignCenter();
                                    innerCol.Item().PaddingVertical(4).LineHorizontal(1);
                                    innerCol.Item().Text("Teacher's Fee Receipt")
                                                  .FontSize(14).Bold().AlignCenter().ParagraphSpacing(8);
                                });

                                row.ConstantColumn(50).Element(logo =>
                                {
                                    var logoPath = "wwwroot/assets/img/bdlogo.png";
                                    if (File.Exists(logoPath))
                                    {
                                        var imageBytes = File.ReadAllBytes(logoPath);
                                        logo.Element(x => x
                                            .AlignCenter()
                                            .Padding(2)
                                            .Width(40)
                                            .Height(40)
                                            .Image(imageBytes, ImageScaling.FitArea)
                                        );
                                    }
                                });
                            });
                        });

                        col.Item().LineHorizontal(1);
                        col.Item().Text($"Name: {student.Name}").SemiBold();
                        col.Item().Text($"Father's Name: {student.FathersName}");
                        col.Item().Text($"Roll Number: {student.Roll}");
                        col.Item().Text($"Class: {student.Class}");
                        col.Item().Text($"Department: {GetDepartmentName(student.Department)}");
                        col.Item().Text($"Fee Paid: {(student.HasPaid ? "Yes" : "No")}").Bold();
                        col.Item().Text($"Paid: {student.Fee} TK").Bold();
                        col.Item().Text($"Receipt Date: {student.Date.ToShortDateString()}");
                        col.Item().LineHorizontal(1);
                        col.Item().Text("Thank you for your payment!").AlignCenter().FontSize(12);

                        col.Item().PaddingVertical(5);
                        col.Item().Text("Accountant's Signature: ____________________").AlignRight();
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
