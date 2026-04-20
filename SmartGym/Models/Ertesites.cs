using System;

namespace GymWebApiBackend.Models
{
    public class Ertesites
    {
        public int ErtesitesId { get; set; }  
        public int TagId { get; set; }
        public string Uzenet { get; set; }
        public bool Olvasott { get; set; }
        public DateTime Datum { get; set; }
    }
}