using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timetracker.Models
{
    public class MonatsInfo
    {
        public string MonatJahr { get; set; } = "";
        public TimeSpan MonatlicheSollzeit { get; set; }
        public TimeSpan MonatlichGearbeitet { get; set; }

        public TimeSpan MonatlicheAbweichung { get; set; }

        public TimeSpan KumuliertesGleitzeitkonto { get; set; }
    }
}
