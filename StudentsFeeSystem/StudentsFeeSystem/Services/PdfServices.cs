using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using StudentsFeeSystem.Models;
using System;

namespace StudentsFeeSystem.Services
{
    public static class PdfService
    {
        public static byte[] GenerateStudentReceipt(Student student)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(14f, 21f, Unit.Centimetre);
                    page.Margin(1.5f, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header().Column(col =>
                    {
                        col.Spacing(5);
                        col.Item().Text("Government of the People's Republic of Bangladesh")
                                  .FontSize(14).Bold().AlignCenter();
                        col.Item().Text("Kaligonj Government Technical School and College")
                                  .FontSize(12).Bold().AlignCenter();
                        col.Item().Text("Kaligonj, Satkhira")
                                  .FontSize(11).Italic().AlignCenter();
                        col.Item().PaddingVertical(6).LineHorizontal(1);
                        col.Item().Text("Fee Receipt")
                                  .FontSize(16).Bold().AlignCenter().ParagraphSpacing(10);
                    });

                    page.Content().Column(col =>
                    {
                        col.Spacing(10);

                       
                        col.Item().LineHorizontal(2);

                        col.Item().Text($"Name: {student.Name}").SemiBold();
                        col.Item().Text($"Father's Name: {student.FathersName}");
                        col.Item().Text($"Roll Number: {student.Roll}");
                        col.Item().Text($"Class: {student.Class}");
                        col.Item().Text($"Department: {GetDepartmentName(student.Department)}");
                        col.Item()
     .Text($"Fee Paid: {(student.HasPaid ? "Yes" : "No")}")
     .Bold();

                        col.Item()
                            .Text($"Paid: {student.Fee} TK")
                            .Bold();

                        col.Item().Text($"Receipt Date: {student.Date.ToShortDateString()}");
                        col.Item().LineHorizontal(1);

                        col.Item().Text("Thank you for your payment!")
                                 .AlignCenter()
                                 .FontSize(13)
                                 ;
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
