using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using Timetracker.Helper;
using Timetracker.Models;
using Timetracker.Services;
using static Timetracker.App;

namespace Timetracker.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private DateTime datum = DateTime.Today;

        [ObservableProperty]
        private TimeSpan start = new(6, 0, 0);

        [ObservableProperty]
        private TimeSpan ende = new(14, 6, 0);

        [ObservableProperty]
        private TimeSpan pause = new(0, 30, 0);

        [ObservableProperty]
        private string notiz = string.Empty;
        [ObservableProperty]
        private ObservableCollection<ArbeitszeitTag> wochenDaten = new();

        [ObservableProperty]
        private TimeSpan wochenSumme;

        [ObservableProperty]
        private ObservableCollection<ArbeitszeitTag> gespeicherteTage = new();

        [ObservableProperty]
        private ArbeitszeitTag? ausgewaehlterTag;

        [ObservableProperty]
        private int aktuelleKalenderwoche;

        [ObservableProperty]
        private int aktuellesJahr;

        [ObservableProperty]
        private ObservableCollection<WochenTagEintrag> wochenTage = new();

        [ObservableProperty]
        private MonatsInfo aktuelleMonatsInfo = new();

        [ObservableProperty]
        private Arbeitszeitmodell ausgewaehltesModell;

        [ObservableProperty]
        private DateTime startdatum = DateTime.Today.AddDays(-7);

        [ObservableProperty]
        private DateTime enddatum = DateTime.Today;

        [ObservableProperty]
        private bool isDarkTheme;

        [ObservableProperty]
        private string themeButtonText = "🌙 Dark Mode";

        [ObservableProperty]
        private bool isZeitraumPopupOpen;

        public IEnumerable<Arbeitszeitmodell> Arbeitszeitmodelle => Enum.GetValues(typeof(Arbeitszeitmodell)).Cast<Arbeitszeitmodell>();

        private Benutzereinstellungen einstellugnen;

        public string KalenderwochenAnzeige => $"Kalenderwoche {AktuelleKalenderwoche} / {AktuellesJahr}";

        partial void OnAktuelleKalenderwocheChanged(int value)
        {

            OnPropertyChanged(nameof(KalenderwochenAnzeige));
        }

        partial void OnAktuellesJahrChanged(int value)
        {

            OnPropertyChanged(nameof(KalenderwochenAnzeige));
        }

        partial void OnDatumChanged(DateTime value)
        {
            LadeWoche();
            BerechneMonatsInfo();
        }

        partial void OnAusgewaehlterTagChanged(ArbeitszeitTag? value)
        {
            if (value is null) return;
            Debug.WriteLine($"Tag ausgewählt: {value.Datum:dd.MM.yyyy}");
            Datum = value.Datum;
            Start = value.Start;
            Ende = value.Ende;
            Pause = value.Pause;
            Notiz = value.Notiz;
        }

        partial void OnStartChanged(TimeSpan value)
        {
            BerechnePause();
        }

        partial void OnEndeChanged(TimeSpan value)
        {
            BerechnePause();
        }

        partial void OnAusgewaehltesModellChanged(Arbeitszeitmodell value)
        {
            einstellugnen.Arbeitszeitmodell = value;
            EinstellungsService.Speichern(einstellugnen);
            BerechneMonatsInfo();
        }

        private readonly string dateipfad = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Timetracker", "daten.json");

        [RelayCommand]
        private void Speichern()
        {
            var tag = new ArbeitszeitTag
            {
                Datum = Datum,
                Start = Start,
                Ende = Ende,
                Pause = Pause,
                Notiz = Notiz
            };

            var verzeichnis = Path.GetDirectoryName(dateipfad);
            if (!Directory.Exists(verzeichnis))
                Directory.CreateDirectory(verzeichnis);
            List<ArbeitszeitTag> daten = new();
            if (File.Exists(dateipfad))
            {
                var json = File.ReadAllText(dateipfad);
                daten = JsonSerializer.Deserialize<List<ArbeitszeitTag>>(json) ?? new();
            }

            // Wenn der Tag schon existiert, ersetzen
            daten.RemoveAll(t => t.Datum.Date == tag.Datum.Date);
            daten.Add(tag);

            //Backup anlegen, bevor gespeichert wird
            if (File.Exists(dateipfad))
            {
                var backupVerzeichnis = Path.Combine(Path.GetDirectoryName(dateipfad)!, "backups");
                Directory.CreateDirectory(backupVerzeichnis);

                var backupDateiname = $"arbeitszeiten_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                var backupPfad = Path.Combine(backupVerzeichnis, backupDateiname);

                File.Copy(dateipfad, backupPfad, overwrite: true);

            }

            File.WriteAllText(dateipfad, JsonSerializer.Serialize(daten, new JsonSerializerOptions { WriteIndented = true }));
            LadeWoche();
            BerechneMonatsInfo();

        }

        [RelayCommand]
        private void Laden()
        {
            if (!File.Exists(dateipfad)) return;

            var json = File.ReadAllText(dateipfad);
            var daten = JsonSerializer.Deserialize<List<ArbeitszeitTag>>(json);
            var heute = daten?.FirstOrDefault(t => t.Datum.Date == DateTime.Today);
            if (heute != null)
            {
                Datum = heute.Datum;
                Start = heute.Start;
                Ende = heute.Ende;
                Pause = heute.Pause;
                Notiz = heute.Notiz;
            }
        }

        [RelayCommand]
        private void WocheVor()
        {
            var start = KulturHelper.GetStartDerKalenderwoche(AktuellesJahr, AktuelleKalenderwoche).AddDays(7);
            AktuelleKalenderwoche = KulturHelper.GetKalenderwoche(start);
            AktuellesJahr = start.Year;
            LadeWoche();
        }

        [RelayCommand]
        private void WocheZurueck()
        {
            var start = KulturHelper.GetStartDerKalenderwoche(AktuellesJahr, AktuelleKalenderwoche).AddDays(-7);
            AktuelleKalenderwoche = KulturHelper.GetKalenderwoche(start);
            AktuellesJahr = start.Year;
            LadeWoche();
        }

        [RelayCommand]
        private void Zuruecksetzen()
        {
            Start = new TimeSpan(6, 0, 0);
            Ende = new TimeSpan(14, 6, 0);
            Notiz = string.Empty;
        }

        [RelayCommand]
        private void ExportiereWoche()
        { 
            if (WochenDaten is null || WochenDaten.Count == 0) return;
            PdfExportService.ExportiereWoche(WochenDaten, WochenSumme);
        }

        [RelayCommand]
        private void ExportiereMonat()
        {
            var monat = Datum.Month;
            var jahr = Datum.Year;

            // 1. Lade alle gespeicherten Tage aus der Datei
            var alleTage = LadeAlleTage();

            // 2. Filter auf die Tage des gewünschten Monats (nicht zwingend nötig im Service, aber nice to check)
            var tageDesMonats = alleTage
                .Where(t => t.Datum.Month == monat && t.Datum.Year == jahr)
                .ToList();

            if (tageDesMonats.Count == 0)
            {
                MessageBox.Show("Keine erfassten Tage im ausgewählten Monat.", "Hinweis",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // 3. Übergabe an den PDF-Export
            PdfExportService.ExportiereMonat(tageDesMonats, monat, jahr);
        }

        [RelayCommand]
        private void ExportiereZeitraum()
        {
            if (Startdatum > Enddatum)
            {
                MessageBox.Show("Das Startdatum darf nicht nach dem Enddatum liegen.", "Ungültiger Datumsbereich", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var alleTage = LadeAlleTage();
            var gefiltert = alleTage
                .Where(t => t.Datum.Date >= Startdatum.Date && t.Datum.Date <= Enddatum.Date)
                .ToList();

            if(gefiltert.Count == 0)
            {
                MessageBox.Show("Für den gewählten Zeitraum wurden keine Einträge gefunden.", "Keine Daten", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            PdfExportService.ExportiereZeitraum(gefiltert, Startdatum, Enddatum);


        }

        [RelayCommand]
        private void ToggleTheme()
        {
            if (isDarkTheme)
            {
                ThemeManager.SetTheme("LightTheme");
                ThemeButtonText = "🌙 Dark Mode";
            }
            {
                ThemeManager.SetTheme("DarkTheme");
                ThemeButtonText = "☀️ Light Mode";
            }

            IsDarkTheme = !IsDarkTheme;
        }

        private void LadeWoche()
        {
            WochenDaten.Clear();
            WochenSumme = TimeSpan.Zero;

            if (!File.Exists(dateipfad)) return;

            var json = File.ReadAllText(dateipfad);
            var daten = JsonSerializer.Deserialize<List<ArbeitszeitTag>>(json);

            if (daten is null) return;

            var startDerWoche = KulturHelper.GetStartDerKalenderwoche(AktuellesJahr, AktuelleKalenderwoche); // Montag

            for (int i = 0; i < 7; i++)
            {
                var datum = startDerWoche.AddDays(i);
                var eintrag = daten.FirstOrDefault(t => t.Datum.Date == datum.Date);

                if (eintrag == null)
                {
                    eintrag = new ArbeitszeitTag
                    {
                        Datum = datum,
                        Start = TimeSpan.Zero,
                        Ende = TimeSpan.Zero,
                        Pause = TimeSpan.Zero,
                        Notiz = ""
                    };
                }

                WochenDaten.Add(eintrag);
                WochenSumme += eintrag.GearbeiteteZeit;
            }

        }

        private List<ArbeitszeitTag> LadeAlleTage()
        {
            if (!File.Exists(dateipfad))
                return new List<ArbeitszeitTag>();

            var json = File.ReadAllText(dateipfad);
            var daten = JsonSerializer.Deserialize<List<ArbeitszeitTag>>(json) ?? new();

            return daten.OrderBy(t => t.Datum).ToList();
        }

        public void BerechnePause()
        {
            var arbeitszeitBrutto = Ende - Start;

            if (arbeitszeitBrutto <= TimeSpan.FromHours(6))
            {
                Pause = TimeSpan.Zero;
            }
            else if (arbeitszeitBrutto > TimeSpan.FromHours(6) && arbeitszeitBrutto <= TimeSpan.FromHours(6.5))
            {
                Pause = arbeitszeitBrutto - TimeSpan.FromHours(6);
            }
            else if (arbeitszeitBrutto > TimeSpan.FromHours(6.5) && arbeitszeitBrutto <= TimeSpan.FromHours(9.25))
            {
                Pause = TimeSpan.FromMinutes(30);
            }
            else if (arbeitszeitBrutto > TimeSpan.FromHours(9.25) && arbeitszeitBrutto <= TimeSpan.FromHours(9.5))
            {
                Pause = arbeitszeitBrutto - TimeSpan.FromHours(9);
            }
            else
            {
                Pause = TimeSpan.FromMinutes(45);
            }

        }

        private void BerechneMonatsInfo()
        {
            Debug.WriteLine("[MONATSINFO] Berechnung wurde gestartet");

            var monat = Datum.Month;
            var jahr = Datum.Year;
            var sollzeitProTag = new TimeSpan(7, 36, 0);

            var daten = LadeAlleTage(); // oder direkt auf gespeicherte Collection zugreifen
            var tageImMonat = daten
                .Where(t => t.Datum.Month == monat && t.Datum.Year == jahr)
                .ToList();

            // Gearbeitete Zeit summieren
            var gearbeiteteZeit = tageImMonat
                .Select(t => t.BerechneteGearbeiteteZeit)
                .Aggregate(TimeSpan.Zero, (summe, zeit) => summe + zeit);

            // Sollzeit berechnen (nur Mo–Fr, Feiertage werden NICHT ausgeschlossen!)
            var anzahlSolltage = Enumerable.Range(1, DateTime.DaysInMonth(jahr, monat))
                .Select(day => new DateTime(jahr, monat, day))
                .Count(d => d.DayOfWeek != DayOfWeek.Saturday && d.DayOfWeek != DayOfWeek.Sunday);

            var sollzeit = TimeSpan.FromTicks(sollzeitProTag.Ticks * anzahlSolltage);

            // Monatsabweichung
            var abweichung = gearbeiteteZeit - sollzeit;

            // Gleitzeitkonto (hier weiterhin separat berechnet)
            var gleitzeit = BerechneGleitzeitBis(jahr, monat);

            Debug.WriteLine($"[MONATSINFO] Gearbeitet: {gearbeiteteZeit}");
            Debug.WriteLine($"[MONATSINFO] Sollzeit:   {sollzeit}");
            Debug.WriteLine($"[MONATSINFO] Abweichung: {abweichung}");
            Debug.WriteLine($"[MONATSINFO] Gleitzeit:  {gleitzeit}");

            AktuelleMonatsInfo = new MonatsInfo
            {
                MonatJahr = $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monat)} {jahr}",
                MonatlichGearbeitet = gearbeiteteZeit,
                MonatlicheSollzeit = sollzeit,
                MonatlicheAbweichung = abweichung,
                KumuliertesGleitzeitkonto = abweichung
            };
        }

        private TimeSpan BerechneSollzeit(int monat, int jahr)
        {
            var sollzeitProTag = HoleTagesSollzeit();
            var anzahlTage = DateTime.DaysInMonth(jahr, monat);
            var summe = TimeSpan.Zero;

            for (int tag = 1; tag <= anzahlTage; tag++)
            {
                var datum = new DateTime(jahr, monat, tag);

                // Nur Samstag und Sonntag ausschließen
                if (datum.DayOfWeek == DayOfWeek.Saturday || datum.DayOfWeek == DayOfWeek.Sunday)
                    continue;

                // Feiertage NICHT ausschließen → zählen mit
                summe += sollzeitProTag;
            }

            return summe;
        }

        private TimeSpan HoleTagesSollzeit()
        {
            return AusgewaehltesModell switch
            {
                Arbeitszeitmodell.Stunden35 => new TimeSpan(7, 0, 0),
                Arbeitszeitmodell.Stunden38 => new TimeSpan(7, 36, 0),
                Arbeitszeitmodell.Stunden40 => new TimeSpan(8, 0, 0),
                _ => new TimeSpan(7, 36, 0)
            };
        }

        private TimeSpan BerechneGleitzeitBis(int jahr, int monat)
        {
            if (!File.Exists(dateipfad))
                return TimeSpan.Zero;

            var json = File.ReadAllText(dateipfad);
            var daten = JsonSerializer.Deserialize<List<ArbeitszeitTag>>(json) ?? new();

            var start = new DateTime(jahr, 1, 1);
            var ende = new DateTime(jahr, monat, DateTime.DaysInMonth(jahr, monat));

            var betroffeneTage = daten
                .Where(t => t.Datum.Date >= start && t.Datum.Date <= ende)
                .OrderBy(t => t.Datum)
                .ToList();

            TimeSpan soll = TimeSpan.Zero;
            TimeSpan ist = TimeSpan.Zero;

            foreach (var tag in betroffeneTage)
            {
                if (tag.IstWochenende) continue;

                soll += new TimeSpan(7, 36, 0);
                ist += tag.BerechneteGearbeiteteZeit;

            }

            return ist - soll;
        }

        public static class EnumHelper
        {
            public static string GetDescription(Enum value)
            {
                var field = value.GetType().GetField(value.ToString());
                var attr = field?.GetCustomAttribute<DescriptionAttribute>();
                return attr?.Description ?? value.ToString();
            }
        }

        public MainViewModel()
        {
            var heute = DateTime.Today;
            aktuelleKalenderwoche = KulturHelper.GetKalenderwoche(heute);
            aktuellesJahr = heute.Year;
            einstellugnen = EinstellungsService.Laden();
            AusgewaehltesModell = einstellugnen.Arbeitszeitmodell;
            LadeWoche();
            LadeAlleTage();
            BerechneMonatsInfo();
        }


    }
}
