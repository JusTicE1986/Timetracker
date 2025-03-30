using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Timetracker.Helper;
using Timetracker.Models;

namespace Timetracker.Services
{
    public class PdfExportService
    {
        public static void ExportiereWoche(IEnumerable<ArbeitszeitTag> tage, TimeSpan wochensumme, string? pfad = null)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var dokument = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header().Text($"Wochenübersicht ({tage.Min(t => t.Datum):dd.MM.yyyy} – {tage.Max(t => t.Datum):dd.MM.yyyy})")
                                  .SemiBold().FontSize(16).FontColor(Colors.Blue.Medium);

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(1); // Tag
                            columns.RelativeColumn(1); // Datum
                            columns.RelativeColumn();  // Start
                            columns.RelativeColumn();  // Ende
                            columns.RelativeColumn();  // Pause
                            columns.RelativeColumn();  // Gearbeitet
                            columns.RelativeColumn(2); // Besonderheit
                        });

                        // Header
                        table.Header(header =>
                        {
                            header.Cell().Text("Tag").SemiBold();
                            header.Cell().Text("Datum").SemiBold();
                            header.Cell().Text("Start").SemiBold();
                            header.Cell().Text("Ende").SemiBold();
                            header.Cell().Text("Pause").SemiBold();
                            header.Cell().Text("Gearbeitet").SemiBold();
                            header.Cell().Text("Besonderheit").SemiBold();
                        });

                        foreach (var tag in tage.OrderBy(t => t.Datum))
                        {
                            table.Cell().Text(tag.Datum.ToString("dddd"));
                            table.Cell().Text(tag.Datum.ToString("dd.MM.yyyy"));
                            table.Cell().Text(tag.Start.ToString(@"hh\:mm"));
                            table.Cell().Text(tag.Ende.ToString(@"hh\:mm"));
                            table.Cell().Text(tag.BerechnetePause.ToString(@"hh\:mm"));
                            table.Cell().Text(tag.BerechneteGearbeiteteZeit.ToString(@"hh\:mm"));
                            table.Cell().Text(tag.Besonderheit ?? "-");
                        }
                    });

                    page.Footer().AlignRight().Text($"Wochensumme: {wochenSummeToText(wochensumme)}")
                                  .FontSize(14).Bold().FontColor(Colors.Green.Darken2);
                });
            });

            var dateiname = pfad ?? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                $"Wochenübersicht_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
            dokument.GeneratePdf(dateiname);

            MessageBox.Show(
    $"Die Wochenübersicht wurde erfolgreich gespeichert:\n\n{dateiname}",
    "PDF Export erfolgreich",
    MessageBoxButton.OK,
    MessageBoxImage.Information
);

        }
        public static void ExportiereMonat(IEnumerable<ArbeitszeitTag> tage, int monat, int jahr)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            Debug.WriteLine($"Monatsexport gestartet.");
            var gruppen = tage
                .Where(t => t.Datum.Month == monat && t.Datum.Year == jahr)
                .OrderBy(t => t.Datum)
                .GroupBy(t => KulturHelper.GetKalenderwoche(t.Datum))
                .OrderBy(g => g.Key)
                .ToList();

            var gesamtsumme = gruppen
                .SelectMany(g => g)
                .Aggregate(TimeSpan.Zero, (sum, tag) => sum + tag.BerechneteGearbeiteteZeit);
            var dokument = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header().Text($"Monatsübersicht – {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monat)} {jahr}")
                                 .SemiBold().FontSize(16).FontColor(Colors.Blue.Medium);

                    page.Content().Column(col =>
                    {
                        foreach (var gruppe in gruppen)
                        {
                            var wochensumme = gruppe.Aggregate(TimeSpan.Zero, (sum, tag) => sum + tag.BerechneteGearbeiteteZeit);
                            var ersteDatum = gruppe.Min(t => t.Datum);
                            var letzteDatum = gruppe.Max(t => t.Datum);

                            col.Item().PaddingBottom(10).Text($"KW {KulturHelper.GetKalenderwoche(ersteDatum)} / {ersteDatum.Year} ({ersteDatum:dd.MM.} – {letzteDatum:dd.MM.})")
                                      .Bold().FontColor(Colors.Grey.Darken2);

                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(cols =>
                                {
                                    cols.RelativeColumn(1); // Tag
                                    cols.RelativeColumn(1); // Datum
                                    cols.RelativeColumn();  // Start
                                    cols.RelativeColumn();  // Ende
                                    cols.RelativeColumn();  // Pause
                                    cols.RelativeColumn();  // Gearbeitet
                                    cols.RelativeColumn(2); // Besonderheit
                                });

                                table.Header(h =>
                                {
                                    h.Cell().Text("Tag").SemiBold();
                                    h.Cell().Text("Datum").SemiBold();
                                    h.Cell().Text("Start").SemiBold();
                                    h.Cell().Text("Ende").SemiBold();
                                    h.Cell().Text("Pause").SemiBold();
                                    h.Cell().Text("Gearbeitet").SemiBold();
                                    h.Cell().Text("Besonderheit").SemiBold();
                                });

                                foreach (var tag in gruppe)
                                {
                                    table.Cell().Text(tag.Datum.ToString("dddd"));
                                    table.Cell().Text(tag.Datum.ToString("dd.MM.yyyy"));
                                    table.Cell().Text(tag.Start.ToString(@"hh\:mm"));
                                    table.Cell().Text(tag.Ende.ToString(@"hh\:mm"));
                                    table.Cell().Text(tag.BerechnetePause.ToString(@"hh\:mm"));
                                    table.Cell().Text(tag.BerechneteGearbeiteteZeit.ToString(@"hh\:mm"));
                                    table.Cell().Text(tag.Besonderheit ?? "-");
                                }
                            });

                            col.Item().AlignRight().Text($"Wochensumme: {(int)wochensumme.TotalHours:D2}:{wochensumme.Minutes:D2} h")
                                      .FontSize(12).Bold().FontColor(Colors.Green.Medium);

                            col.Item().PaddingBottom(15).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                        }

                        col.Item().PaddingTop(20).AlignRight()
                                  .Text($"Monatssumme: {(int)gesamtsumme.TotalHours:D2}:{gesamtsumme.Minutes:D2} h")
                                  .FontSize(14).Bold().FontColor(Colors.Green.Darken2);
                    });
                });
            });

            var pfad = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                $"Monatsübersicht_{jahr}_{monat:00}.pdf");

            dokument.GeneratePdf(pfad);

            MessageBox.Show(
                $"Die Monatsübersicht wurde erfolgreich gespeichert:\n\n{pfad}",
                "PDF Export erfolgreich",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }
        public static void ExportiereZeitraum(List<ArbeitszeitTag> tage, DateTime start, DateTime ende)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            var gruppen = tage
                .OrderBy(t => t.Datum)
                .GroupBy(t => KulturHelper.GetKalenderwoche(t.Datum))
                .OrderBy(g => g.Key)
                .ToList();

            var gesamtsumme = tage.Aggregate(TimeSpan.Zero, (sum, t) => sum + t.BerechneteGearbeiteteZeit);

            var dokument = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header().Text($"Zeitraum: {start:dd.MM.yyyy} – {ende:dd.MM.yyyy}")
                                 .SemiBold().FontSize(16).FontColor(Colors.Blue.Medium);

                    page.Content().Column(col =>
                    {
                        foreach (var gruppe in gruppen)
                        {
                            var erste = gruppe.Min(t => t.Datum);
                            var letzte = gruppe.Max(t => t.Datum);
                            var wochensumme = gruppe.Aggregate(TimeSpan.Zero, (sum, t) => sum + t.BerechneteGearbeiteteZeit);

                            col.Item().PaddingBottom(10).Text($"KW {KulturHelper.GetKalenderwoche(erste)} ({erste:dd.MM.} – {letzte:dd.MM.})")
                                      .Bold().FontColor(Colors.Grey.Darken2);

                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(cols =>
                                {
                                    cols.RelativeColumn(1); // Tag
                                    cols.RelativeColumn(1); // Datum
                                    cols.RelativeColumn();  // Start
                                    cols.RelativeColumn();  // Ende
                                    cols.RelativeColumn();  // Pause
                                    cols.RelativeColumn();  // Gearbeitet
                                    cols.RelativeColumn(2); // Besonderheit
                                });

                                table.Header(h =>
                                {
                                    h.Cell().Text("Tag").SemiBold();
                                    h.Cell().Text("Datum").SemiBold();
                                    h.Cell().Text("Start").SemiBold();
                                    h.Cell().Text("Ende").SemiBold();
                                    h.Cell().Text("Pause").SemiBold();
                                    h.Cell().Text("Gearbeitet").SemiBold();
                                    h.Cell().Text("Besonderheit").SemiBold();
                                });

                                foreach (var tag in gruppe)
                                {
                                    table.Cell().Text(tag.Datum.ToString("dddd"));
                                    table.Cell().Text(tag.Datum.ToString("dd.MM.yyyy"));
                                    table.Cell().Text(tag.Start.ToString(@"hh\:mm"));
                                    table.Cell().Text(tag.Ende.ToString(@"hh\:mm"));
                                    table.Cell().Text(tag.BerechnetePause.ToString(@"hh\:mm"));
                                    table.Cell().Text(tag.BerechneteGearbeiteteZeit.ToString(@"hh\:mm"));
                                    table.Cell().Text(tag.Besonderheit ?? "-");
                                }
                            });

                            col.Item().AlignRight().Text($"Wochensumme: {(int)wochensumme.TotalHours:D2}:{wochensumme.Minutes:D2} h")
                                      .FontSize(12).Bold().FontColor(Colors.Green.Medium);

                            col.Item().PaddingBottom(15).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                        }

                        col.Item().PaddingTop(20).AlignRight()
                                  .Text($"Gesamtsumme: {(int)gesamtsumme.TotalHours:D2}:{gesamtsumme.Minutes:D2} h")
                                  .FontSize(14).Bold().FontColor(Colors.Green.Darken2);
                    });
                });
            });

            var pfad = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                $"Zeitraum_{start:yyyyMMdd}_{ende:yyyyMMdd}.pdf");

            dokument.GeneratePdf(pfad);

            MessageBox.Show(
                $"Die Übersicht wurde erfolgreich gespeichert:\n\n{pfad}",
                "PDF Export erfolgreich",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private static string wochenSummeToText(TimeSpan summe) => $"{(int)summe.TotalHours:D2}:{summe.Minutes:D2} h";
    }
}
