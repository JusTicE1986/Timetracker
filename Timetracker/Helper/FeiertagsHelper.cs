using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timetracker.Helper
{
    public static class FeiertagsHelper
    {
        public static Dictionary<DateTime, string> GetFeiertage(int jahr)
        {
            var feiertage = new Dictionary<DateTime, string>();

            //feste Feiertage
            feiertage.Add(new DateTime(jahr, 1, 1), "Neujahr");
            feiertage.Add(new DateTime(jahr, 5, 1), "Tag der Arbeit");
            feiertage.Add(new DateTime(jahr, 10, 3), "Tag der deutschen Einheit");
            feiertage.Add(new DateTime(jahr, 12, 25), "1. Weihnachtstag");
            feiertage.Add(new DateTime(jahr, 12, 26), "2. Weihnachtstag");

            //bewegliche Feiertage (basierend auf Ostersonntag)
            var ostern = BerechneOstersonntag(jahr);
            feiertage.Add(ostern.AddDays(-2), "Karfreitag");
            feiertage.Add(ostern.AddDays(1), "Ostermontag");
            feiertage.Add(ostern.AddDays(39), "Christi Himmelfahrt");
            feiertage.Add(ostern.AddDays(49), "Pfingstsonntag");
            feiertage.Add(ostern.AddDays(50), "Pfingstmontag");
            feiertage.Add(ostern.AddDays(60), "Fronleichnam");

            return feiertage;
        }

        private static DateTime BerechneOstersonntag(int jahr)
        {
            //Gaußsche Osterformel
            int a = jahr % 19;
            int b = jahr / 100;
            int c = jahr % 100;
            int d = b / 4;
            int e = b % 4;
            int f = (b + 8) / 25;
            int g = (b - f + 1) / 3;
            int h = (19 * a + b - d - g + 15) % 30;
            int i = c / 4;
            int k = c % 4;
            int l = (32 + 2 * e + 2 * i - h - k) % 7;
            int m = (a + 11 * h + 22 * 1) / 451;
            int n = (h + l - 7 * m + 114) / 31;
            int p = (h + l - 7 * m + 114) % 31;

            return new DateTime(jahr, n, p + 1);
        }

    }
}
