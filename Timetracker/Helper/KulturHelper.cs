﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timetracker.Helper
{
    public class KulturHelper
    {
        public static int GetKalenderwoche(DateTime datum)
        {
            return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                datum,
                CalendarWeekRule.FirstFourDayWeek,
                DayOfWeek.Monday
                );
        }

        public static DateTime GetStartDerKalenderwoche(int jahr, int kalenderwoche)
        {
            DateTime jan1 = new(jahr, 1, 1);
            int daysOffset = DayOfWeek.Monday - jan1.DayOfWeek;
            DateTime ersterMontag = jan1.AddDays(daysOffset);

            var kalender = CultureInfo.CurrentCulture.Calendar;
            return ersterMontag.AddDays((kalenderwoche - 1) * 7);
        }

        public static string? IstFeiertagOderWochenende(DateTime datum)
        {
            if (datum.DayOfWeek == DayOfWeek.Saturday || datum.DayOfWeek == DayOfWeek.Sunday) return "Wochenende";

            // Dummy-Feiertage:
            if (datum.Month == 12 && datum.Day == 25)
                return "Weihnachten";
            if (datum.Month == 1 && datum.Day == 1)
                return "Neujahr";

            return null;
        }

    }
}
