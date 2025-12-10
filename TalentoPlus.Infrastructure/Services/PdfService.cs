using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TalentoPlus.Core.Entities;

using TalentoPlus.Infrastructure.Services.Interfaces;

namespace TalentoPlus.Infrastructure.Services
{
    public class PdfService : IPdfService
    {
        public PdfService()
        {
            // License configuration for QuestPDF (Community License)
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public byte[] GenerateEmployeeCv(Employee employee)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .Text($"Curriculum Vitae - {employee.FirstName} {employee.LastName}")
                        .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(x =>
                        {
                            x.Spacing(20);

                            x.Item().Text($"Position: {employee.Position}");
                            x.Item().Text($"Department: {employee.Department?.Name ?? "N/A"}");
                            x.Item().Text($"Email: {employee.Email}");
                            x.Item().Text($"Phone: {employee.ContactPhone}");
                            x.Item().Text($"Education: {employee.EducationLevel}");
                            x.Item().Text($"Profile: {employee.ProfessionalProfile}");
                            x.Item().Text($"Join Date: {employee.JoinDate:yyyy-MM-dd}");
                            x.Item().Text($"Status: {employee.Status}");
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Page ");
                            x.CurrentPageNumber();
                        });
                });
            });

            return document.GeneratePdf();
        }
    }
}