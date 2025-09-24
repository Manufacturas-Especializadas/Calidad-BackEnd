using System;
using System.Collections.Generic;

namespace Rechazos.Models;

public partial class Rejections
{
    public int Id { get; set; }

    public int? IdDefect { get; set; }

    public int? IdCondition { get; set; }

    public int? IdLine { get; set; }

    public int? IdClient { get; set; }

    public int? IdContainmentaction { get; set; }

    public string Insepector { get; set; }

    public string PartNumber { get; set; }

    public int? NumberOfPieces { get; set; }

    public int? OperatorPayroll { get; set; }

    public string Description { get; set; }

    public string Image { get; set; }

    public string InformedSignature { get; set; }

    public DateTime? RegistrationDate { get; set; }

    public int? Folio { get; set; }

    public virtual Clients IdClientNavigation { get; set; }

    public virtual RjCondition IdConditionNavigation { get; set; }

    public virtual RjContainmentaction IdContainmentactionNavigation { get; set; }

    public virtual RjDefects IdDefectNavigation { get; set; }

    public virtual Lines IdLineNavigation { get; set; }
}