using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timetracker.Models
{
    public class WochenTagEintrag
    {
        public DateTime Datum { get; set; }
        public string Wochentag { get; set; }
        public bool Erfasst { get; set; }
        public string? Besonderheit { get; set; }
    }
}
