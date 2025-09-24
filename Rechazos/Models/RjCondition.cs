using System;
using System.Collections.Generic;

namespace Rechazos.Models;

public partial class RjCondition
{
    public int Id { get; set; }

    public int? IdDefects { get; set; }

    public string Name { get; set; }

    public virtual RjDefects IdDefectsNavigation { get; set; }

    public virtual ICollection<Rejections> Rejections { get; set; } = new List<Rejections>();
}