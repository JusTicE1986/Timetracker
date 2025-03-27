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

namespace Timetracker.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private DateTime datum = DateTime.Today;

        [ObservableProperty]
        private TimeSpan start = new(8, 0, 0);

        [ObservableProperty]
        private TimeSpan ende = new(16, 30, 0);

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

        public string KalenderwochenAnzeige => $"Kalenderwoche {AktuelleKalenderwoche} / {AktuellesJahr}";

        partial void OnAktuelleKalenderwocheChanged(int value)
        {
            OnPropertyChanged(nameof(KalenderwochenAnzeige));
        }

        partial void OnAktuellesJahrChanged(int value)
        {
            OnPropertyChanged(nameof(KalenderwochenAnzeige));
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
            var start = KulturHelper.GetStartDerKalenderwoche(aktuellesJahr, aktuelleKalenderwoche).AddDays(7);
            aktuelleKalenderwoche = KulturHelper.GetKalenderwoche(start);
            aktuellesJahr = start.Year;
            LadeWoche();
        }

        [RelayCommand]
        private void WocheZurueck()
        {
            var start = KulturHelper.GetStartDerKalenderwoche(aktuellesJahr, aktuelleKalenderwoche).AddDays(-7);
            aktuelleKalenderwoche = KulturHelper.GetKalenderwoche(start);
            aktuellesJahr = start.Year;
            LadeWoche();
        }

        private void LadeWoche()
        {
            WochenDaten.Clear();
            WochenSumme = TimeSpan.Zero;

            if (!File.Exists(dateipfad)) return;

            var json = File.ReadAllText(dateipfad);
            var daten = JsonSerializer.Deserialize<List<ArbeitszeitTag>>(json);

            if (daten is null) return;

            var startDerWoche = KulturHelper.GetStartDerKalenderwoche(aktuellesJahr, aktuelleKalenderwoche); // Montag
                                                                                                             //var endeDerWoche = startDerWoche.Add(TimeSpan.FromDays(6));

            //var eintraege = daten
            //    .Where(t => t.Datum >= startDerWoche && t.Datum <= endeDerWoche)
            //    .OrderBy(t => t.Datum)
            //    .ToList();

            //foreach (var tag in eintraege)
            //{
            //    WochenDaten.Add(tag);
            //    WochenSumme += tag.GearbeiteteZeit;
            //
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
                wochenSumme += eintrag.GearbeiteteZeit;
            }

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
        public void LadeAlleTage()
        {
            if (!File.Exists(dateipfad)) return;

            var json = File.ReadAllText(dateipfad);
            var daten = JsonSerializer.Deserialize<List<ArbeitszeitTag>>(json) ?? new();

            gespeicherteTage.Clear();
            foreach (var tag in daten.OrderBy(t => t.Datum))
            {
                gespeicherteTage.Add(tag);
            }
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
