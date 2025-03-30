using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Timetracker.Models;

namespace Timetracker.Services
{
    public class EinstellungsService
    {
        private static readonly string pfad = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Timetracker",
            "einstellungen.json");

        public static Benutzereinstellungen Laden()
        {
            try
            {
                if (File.Exists(pfad))
                {
                    var json = File.ReadAllText(pfad);
                    return JsonSerializer.Deserialize<Benutzereinstellungen>(json) ?? new();
                }
            }
            catch (Exception)
            {

                throw;
            }

            return new Benutzereinstellungen();
        }

        public static void Speichern(Benutzereinstellungen einstellungen)
        {
            var verzeichnis = Path.GetDirectoryName(pfad);
            if (!Directory.Exists(verzeichnis))
                Directory.CreateDirectory(verzeichnis);

            var json = JsonSerializer.Serialize(einstellungen, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(pfad, json);
        }
    }
}
