using System;
using System.Collections.Generic;

namespace Rechazos.Models;

public partial class RjDefects
{
    public int Id { get; set; }

    public string Name { get; set; }

    public virtual ICollection<Rejections> Rejections { get; set; } = new List<Rejections>();

    public virtual ICollection<RjCondition> RjCondition { get; set; } = new List<RjCondition>();
}