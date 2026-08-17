using LMS.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace LMS.Infrastructure.Services;

public class CertificateGenerator : ICertificateGenerator
{
    public CertificateGenerator()
    {
        // Set QuestPDF license (Community license for non-commercial or use Commercial license)
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<byte[]> GenerateCertificatePdfAsync(Certificate certificate, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() =>
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(0);
                    page.PageColor(Colors.White);

                    page.Content().PaddingVertical(30).PaddingHorizontal(80).Column(column =>
                    {
                        column.Spacing(10);

                        // Decorative top border
                        column.Item().BorderBottom(4).BorderColor(Colors.Blue.Darken3).PaddingBottom(10);

                        // Header - Certificate of Completion
                        column.Item().AlignCenter().Text("CERTIFICATE OF COMPLETION")
                            .FontSize(36)
                            .Bold()
                            .FontColor(Colors.Blue.Darken3);

                        column.Item().PaddingTop(6).AlignCenter().Text("This certifies that")
                            .FontSize(14)
                            .FontColor(Colors.Grey.Darken2);

                        // Employee/Learner name (larger)
                        column.Item().PaddingTop(3).AlignCenter().Text(GetEmployeeName(certificate.UserId))
                            .FontSize(30)
                            .Bold()
                            .FontColor(Colors.Blue.Darken4);

                        column.Item().PaddingTop(6).AlignCenter().Text("has successfully completed")
                            .FontSize(14)
                            .FontColor(Colors.Grey.Darken2);

                        // Course title
                        column.Item().PaddingTop(3).AlignCenter().Text(certificate.Course?.Title ?? "Course")
                            .FontSize(22)
                            .SemiBold()
                            .FontColor(Colors.Blue.Darken3);

                        // Score section
                        if (certificate.Score.HasValue)
                        {
                            column.Item().PaddingTop(12).AlignCenter().Row(row =>
                            {
                                row.AutoItem().Text("Final Score: ")
                                    .FontSize(13)
                                    .FontColor(Colors.Grey.Darken2);
                                row.AutoItem().Text($"{certificate.Score.Value:F1}%")
                                    .FontSize(13)
                                    .Bold()
                                    .FontColor(Colors.Green.Darken2);
                            });
                        }

                        // Date and certificate number section
                        column.Item().PaddingTop(15).Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text("Date Issued")
                                    .FontSize(11)
                                    .FontColor(Colors.Grey.Darken1);
                                col.Item().PaddingTop(3).Text(certificate.IssuedDate.ToString("MMMM dd, yyyy"))
                                    .FontSize(13)
                                    .SemiBold()
                                    .FontColor(Colors.Blue.Darken3);
                            });

                            row.RelativeItem().AlignCenter().Column(col =>
                            {
                                col.Item().AlignCenter().Text("Certificate Number")
                                    .FontSize(11)
                                    .FontColor(Colors.Grey.Darken1);
                                col.Item().AlignCenter().PaddingTop(3).Text(certificate.CertificateNumber)
                                    .FontSize(13)
                                    .SemiBold()
                                    .FontColor(Colors.Blue.Darken3);
                            });

                            row.RelativeItem().AlignRight().Column(col =>
                            {
                                if (certificate.ExpirationDate.HasValue)
                                {
                                    col.Item().AlignRight().Text("Valid Until")
                                        .FontSize(11)
                                        .FontColor(Colors.Grey.Darken1);
                                    col.Item().AlignRight().PaddingTop(3).Text(certificate.ExpirationDate.Value.ToString("MMMM dd, yyyy"))
                                        .FontSize(13)
                                        .SemiBold()
                                        .FontColor(Colors.Blue.Darken3);
                                }
                                else
                                {
                                    col.Item().AlignRight().Text("No Expiration")
                                        .FontSize(11)
                                        .FontColor(Colors.Green.Darken2);
                                }
                            });
                        });

                        // Signature section
                        column.Item().PaddingTop(18).Row(row =>
                        {
                            row.RelativeItem(2).Column(col =>
                            {
                                col.Item().BorderTop(1).BorderColor(Colors.Grey.Medium);
                                col.Item().PaddingTop(5).Text(certificate.IssuedBy)
                                    .FontSize(11)
                                    .SemiBold()
                                    .FontColor(Colors.Blue.Darken3);
                                col.Item().Text("Authorized Signature")
                                    .FontSize(9)
                                    .FontColor(Colors.Grey.Darken1);
                            });

                            row.RelativeItem(1);

                            row.RelativeItem(2).Column(col =>
                            {
                                col.Item().BorderTop(1).BorderColor(Colors.Grey.Medium);
                                col.Item().PaddingTop(5).Text("Business As Usual")
                                    .FontSize(11)
                                    .SemiBold()
                                    .FontColor(Colors.Blue.Darken3);
                                col.Item().Text("Learning Management System")
                                    .FontSize(9)
                                    .FontColor(Colors.Grey.Darken1);
                            });
                        });

                        // Footer with seal/badge
                        column.Item().PaddingTop(10).AlignCenter().Text("🏆")
                            .FontSize(22);

                        column.Item().PaddingTop(2).AlignCenter().Text("Verify at: www.businessasusual.com/verify")
                            .FontSize(8)
                            .FontColor(Colors.Grey.Medium);

                        // Decorative bottom border
                        column.Item().PaddingTop(4).BorderTop(4).BorderColor(Colors.Blue.Darken3);
                    });
                });
            });

            return document.GeneratePdf();
        }, cancellationToken);
    }

    public async Task<string> SaveCertificatePdfAsync(Certificate certificate, string outputPath, CancellationToken cancellationToken = default)
    {
        var pdfBytes = await GenerateCertificatePdfAsync(certificate, cancellationToken);

        var directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await File.WriteAllBytesAsync(outputPath, pdfBytes, cancellationToken);
        return outputPath;
    }

    private string GetEmployeeName(string employeeId)
    {
        // TODO: Fetch actual employee name from HR system
        // For now, format the employee ID nicely
        return employeeId.Replace(".", " ").Replace("_", " ");
    }
}
