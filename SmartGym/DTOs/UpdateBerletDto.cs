namespace GymWebApiBackend.DTOs
{
    public class UpdateBerletDto
    {
        public int TagId { get; set; }
        public int BerletTipusId { get; set; }
        public DateTime KezdetDatum { get; set; }
        public DateTime VegeDatum { get; set; }
        public bool Aktiv { get; set; }
    }
}