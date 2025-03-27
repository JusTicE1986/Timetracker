using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timetracker.Helper;

namespace Timetracker.Models
{
    public class ArbeitszeitTag
    {
        public DateTime Datum { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan Ende { get; set; }
        public TimeSpan Pause { get; set; }
        public string Notiz { get; set; }
        public TimeSpan GearbeiteteZeit => (Ende - Start - Pause) < TimeSpan.Zero ? TimeSpan.Zero : Ende - Start - Pause;

        public string Wochentag => Datum.ToString("dddd", CultureInfo.CurrentCulture);
        public string? Besonderheit => KulturHelper.IstFeiertagOderWochenende(Datum);
        public bool IstWochenende => Datum.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
        public bool IstFeiertag => Besonderheit != null && Besonderheit != "Wochenende";
        public bool IstErfasst => Start > TimeSpan.Zero || Ende > TimeSpan.Zero || !string.IsNullOrWhiteSpace(Notiz);
    }
}
