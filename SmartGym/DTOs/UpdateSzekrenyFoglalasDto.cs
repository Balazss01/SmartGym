namespace GymWebApiBackend.DTOs
{
    public class UpdateSzekrenyFoglalasDto
    {
        public int TagId { get; set; }
        public int SzekrenyId { get; set; }
        public bool Zarva { get; set; }

        public DateTime? FoglalvaKezdete { get; set; }
        public DateTime? FoglalvaVege { get; set; }
    }
}