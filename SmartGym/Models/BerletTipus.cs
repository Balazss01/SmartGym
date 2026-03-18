namespace GymWebApiBackend.Models
{
    public class BerletTipus
    {
        public int BerletTipusId { get; set; }
        public string Megnevezes { get; set; } = null!;
        public int IdotartamNapok { get; set; }
        public decimal Ar { get; set; }

        public ICollection<Berlet> Berletek { get; set; } = new List<Berlet>();
    }
}