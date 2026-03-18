namespace GymWebApiBackend.Models
{
    public class SzekrenyFoglalas
    {
        public int FoglalasId { get; set; }
        public int TagId { get; set; }
        public int SzekrenyId { get; set; }
        public bool Zarva { get; set; }
        public DateTime FoglalvaKezdete { get; set; }
        public DateTime FoglalvaVege { get; set; }

        public Tag Tag { get; set; }
        public Szekreny Szekreny { get; set; }
    }
}