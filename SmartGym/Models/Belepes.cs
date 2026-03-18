namespace GymWebApiBackend.Models
{
    public class Belepes
    {
        public int BelepesId { get; set; }
        public int TagId { get; set; }
        public DateTime BelepesIdopont { get; set; }

        public Tag Tag { get; set; }
    }
}