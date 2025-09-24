namespace Rechazos.Dtos
{
    public class RejectionDto
    {
        public int? IdDefect { get; set; }

        public int? IdCondition { get; set; }

        public int? IdLine { get; set; }

        public int? IdClient { get; set; }

        public int? IdContainmentaction { get; set; }

        public string Insepector { get; set; } = string.Empty;

        public string PartNumber { get; set; } = string.Empty;

        public int? NumberOfPieces { get; set; }

        public string Description { get; set; } = string.Empty;

        public List<IFormFile>? Photos { get; set; }

        public string InformedSignature { get; set; } = string.Empty;

        public int? OperatorPayroll { get; set; }

        public int Folio {  get; set; }
    }
}