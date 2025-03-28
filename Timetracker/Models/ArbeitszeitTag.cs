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
        public TimeSpan BerechnetePause
        {
            get
            {
                var brutto = Ende - Start;

                if (brutto <= TimeSpan.FromHours(6))
                    return TimeSpan.Zero;

                if (brutto <= TimeSpan.FromHours(6.5))
                    return TimeSpan.Zero; // bleibt bei 6h

                if (brutto > TimeSpan.FromHours(6.5) && brutto <= TimeSpan.FromHours(9.25))
                    return TimeSpan.FromMinutes(30); // feste Pause

                if (brutto > TimeSpan.FromHours(9.25) && brutto <= TimeSpan.FromHours(9.5))
                    return TimeSpan.FromMinutes(30); // auch hier, nicht 0

                return TimeSpan.FromMinutes(45); // ab 9:26
            }
        }

        public string Notiz { get; set; }
        public TimeSpan GearbeiteteZeit => (Ende - Start - Pause) < TimeSpan.Zero ? TimeSpan.Zero : Ende - Start - Pause;
        public TimeSpan BerechneteGearbeiteteZeit
        {
            get
            {
                var brutto = Ende - Start;

                if (brutto <= TimeSpan.FromHours(6))
                    return brutto;

                if (brutto <= TimeSpan.FromHours(6.5))
                    return TimeSpan.FromHours(6); // Deckelung auf 6h

                if (brutto <= TimeSpan.FromHours(9.25))
                    return brutto - TimeSpan.FromMinutes(30);

                if (brutto <= TimeSpan.FromHours(9.5))
                    return TimeSpan.FromHours(9); // Deckelung auf 9h

                return brutto - TimeSpan.FromMinutes(45);
            }
        }
        public string Wochentag => Datum.ToString("dddd", CultureInfo.CurrentCulture);
        public string? Besonderheit => KulturHelper.IstFeiertagOderWochenende(Datum);
        public bool IstWochenende => Datum.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
        public bool IstFeiertag => Besonderheit != null && Besonderheit != "Wochenende";
        public bool IstErfasst => Start > TimeSpan.Zero || Ende > TimeSpan.Zero || !string.IsNullOrWhiteSpace(Notiz);
    }
}
