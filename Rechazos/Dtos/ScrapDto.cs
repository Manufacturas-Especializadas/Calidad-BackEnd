namespace Calidad.Dtos
{
    public class ScrapDto
    {
        public int? ShiftId { get; set; }

        public int? LineId { get; set; }

        public int? ProcessId { get; set; }

        public int? PayRollNumber { get; set; }

        public int? MaterialId { get; set; }

        public string Alloy { get; set; }

        public string Diameter { get; set; }

        public string Wall { get; set; }

        public int? TypeScrapId { get; set; }

        public int? DefectId { get; set; }

        public int? MachineId { get; set; }

        public string Rdm { get; set; }

        public string Weight { get; set; }
    }
}