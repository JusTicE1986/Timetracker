using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Timetracker.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Timetracker.Helper;
using System.Web;
using System.Globalization;

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

        public void BerechneMonatsInfo()
        {
            var daten = LadeAlleTage();
            var monat = Datum.Month;
            var jahr = Datum.Year;

            var tageImMonat = daten
                .Where(t => t.Datum.Month == monat && t.Datum.Year == jahr)
                .ToList();

            var gearbeiteteZeit = tageImMonat.Aggregate(TimeSpan.Zero, (summe, tag) => summe + tag.BerechneteGearbeiteteZeit);

            var sollzeit = BerechneSollzeit(monat, jahr);

            AktuelleMonatsInfo = new MonatsInfo
            {
                MonatJahr = $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monat)} {jahr}",
                MonatlichGearbeitet = gearbeiteteZeit,
                MonatlicheSollzeit = sollzeit,
                KumuliertesGleitzeitkonto = BerechneGleitzeitBis(jahr, monat)
            };
        }

        private TimeSpan BerechneSollzeit(int monat, int jahr)
        {
            var sollzeitProTag = new TimeSpan(7, 36, 0);
            var anzahlTage = DateTime.DaysInMonth(jahr, monat);
            var summe = TimeSpan.Zero;

            for (int tag = 1; tag <= anzahlTage; tag++)
            {
                var datum = new DateTime(jahr, monat, tag);

                if (datum.DayOfWeek == DayOfWeek.Saturday || datum.DayOfWeek == DayOfWeek.Sunday)
                    continue;

                summe += sollzeitProTag;
            }
            return summe;
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


        public MainViewModel()
        {
            var heute = DateTime.Today;
            aktuelleKalenderwoche = KulturHelper.GetKalenderwoche(heute);
            aktuellesJahr = heute.Year;
            LadeWoche();
            LadeAlleTage();
        }


    }
}
