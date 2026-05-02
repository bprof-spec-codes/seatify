using Logic.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;
using System.IO;

namespace Logic.Services
{
    public class PdfService : IPdfService
    {
        public PdfService()
        {
            // QuestPDF License - Community
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public byte[] GenerateTicketPdf(
            string eventName,
            string venueName,
            string auditoriumName,
            DateTime eventTime,
            IEnumerable<PdfTicketItem> tickets,
            string currency)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text(eventName).FontSize(24).SemiBold().FontColor(Colors.Blue.Medium);
                            col.Item().Text($"{venueName} - {auditoriumName}").FontSize(14).Medium();
                            col.Item().Text(eventTime.ToString("f")).FontSize(12).Italic();
                        });
                    });

                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        foreach (var ticket in tickets)
                        {
                            col.Item().PaddingBottom(10).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Row(row =>
                            {
                                row.RelativeItem().Column(ticketCol =>
                                {
                                    // Event info on each ticket
                                    ticketCol.Item().Text(eventName).FontSize(14).SemiBold().FontColor(Colors.Blue.Medium);
                                    ticketCol.Item().Text($"{venueName} - {auditoriumName}").FontSize(10).Medium();
                                    ticketCol.Item().Text(eventTime.ToString("f")).FontSize(9).Italic().FontColor(Colors.Grey.Medium);
                                    
                                    ticketCol.Item().PaddingTop(5).Row(detailsRow => {
                                        detailsRow.RelativeItem().Column(c => {
                                            c.Item().Text("Sor:").FontSize(8).SemiBold().FontColor(Colors.Grey.Medium);
                                            c.Item().Text(ticket.Row).FontSize(14).Bold();
                                        });
                                        detailsRow.RelativeItem().Column(c => {
                                            c.Item().Text("Szék:").FontSize(8).SemiBold().FontColor(Colors.Grey.Medium);
                                            c.Item().Text(ticket.SeatNumber).FontSize(14).Bold();
                                        });
                                    });

                                    ticketCol.Item().AlignBottom().Text($"{ticket.Price:N0} {currency}").FontSize(10);
                                });

                                row.ConstantItem(100).Column(qrCol =>
                                {
                                    var qrBytes = Convert.FromBase64String(ticket.QrCodeBase64);
                                    qrCol.Item().Image(qrBytes);
                                    qrCol.Item().PaddingTop(2).Text(ticket.ManualCode).FontSize(6).AlignCenter().FontColor(Colors.Grey.Medium);
                                });
                            });
                        }
                    });
                });
            });

            using var stream = new MemoryStream();
            document.GeneratePdf(stream);
            return stream.ToArray();
        }
    }
}
