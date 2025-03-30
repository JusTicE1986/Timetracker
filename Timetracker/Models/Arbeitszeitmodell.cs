using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timetracker.Models
{
    public enum Arbeitszeitmodell
    {
        [Description("Verkürzte Vollzeit")]
        Stunden35,
        [Description("Regelarbeitszeit")]
        Stunden38,
        [Description("40-Stunden-Woche")]
        Stunden40

    }
}
