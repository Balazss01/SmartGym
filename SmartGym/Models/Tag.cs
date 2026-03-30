namespace GymWebApiBackend.Models
{
    public class Tag
    {
        public int TagId { get; set; }
        public Guid IdentityUserId { get; set; } 
        public string Vezeteknev { get; set; } = null!;
        public string Keresztnev { get; set; } = null!;
        public DateTime SzuletesiDatum { get; set; }
        public bool Aktiv { get; set; }

        public ICollection<Berlet> Berletek { get; set; } = new List<Berlet>();
        public ICollection<Belepes> Belepesek { get; set; } = new List<Belepes>();
        public ICollection<SzekrenyFoglalas> SzekrenyFoglalasok { get; set; } = new List<SzekrenyFoglalas>();
    }
}