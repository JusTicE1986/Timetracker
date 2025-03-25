using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timetracker.Models
{
    public class ArbeitszeitTag
    {
        public DateTime Datum { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan Ende { get; set; }
        public TimeSpan Pause { get; set; }
        public string Notiz { get; set; }
        public TimeSpan GearbeiteteZeit => (Ende - Start - Pause);
    }
}
