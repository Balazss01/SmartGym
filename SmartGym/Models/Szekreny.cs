namespace GymWebApiBackend.Models
{
    public class Szekreny
    {
        public int SzekrenyId { get; set; }
        public int SzekrenySzam { get; set; }
        public bool Aktiv { get; set; }

        public ICollection<SzekrenyFoglalas> SzekrenyFoglalasok { get; set; } = new List<SzekrenyFoglalas>();
    }
}