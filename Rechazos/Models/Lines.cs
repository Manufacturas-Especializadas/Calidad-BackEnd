using System;
using System.Collections.Generic;

namespace Rechazos.Models;

public partial class Lines
{
    public int Id { get; set; }

    public string Name { get; set; }

    public virtual ICollection<MesaProcess> MesaProcess { get; set; } = new List<MesaProcess>();

    public virtual ICollection<Rejections> Rejections { get; set; } = new List<Rejections>();

    public virtual ICollection<Scrap> Scrap { get; set; } = new List<Scrap>();
}