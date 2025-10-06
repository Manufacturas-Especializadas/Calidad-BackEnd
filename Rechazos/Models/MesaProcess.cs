using System;
using System.Collections.Generic;

namespace Rechazos.Models;

public partial class MesaProcess
{
    public int Id { get; set; }

    public int? LineId { get; set; }

    public string Name { get; set; }

    public virtual Lines Line { get; set; }

    public virtual ICollection<MachineCodes> MachineCodes { get; set; } = new List<MachineCodes>();

    public virtual ICollection<Scrap> Scrap { get; set; } = new List<Scrap>();
}