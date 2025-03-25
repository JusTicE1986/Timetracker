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

        private readonly string dateipfad = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Arbeitszeitrechner", "daten.json");

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
    }
}
