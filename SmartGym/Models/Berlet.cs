namespace GymWebApiBackend.Models
{
    public class Berlet
    {
        public int BerletId { get; set; }
        public int TagId { get; set; }
        public int BerletTipusId { get; set; }
        public DateTime KezdetDatum { get; set; }
        public DateTime VegeDatum { get; set; }
        public bool Aktiv { get; set; }

        public Tag Tag { get; set; } = null!;
        public BerletTipus BerletTipus { get; set; } = null!;
    }
}