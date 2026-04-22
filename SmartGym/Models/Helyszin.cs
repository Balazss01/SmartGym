namespace SmartGym.Models
{
    public class Helyszin
    {
        public int Id { get; set; }
        public string Nev { get; set; } = string.Empty;
        public string Varos { get; set; } = string.Empty;
        public string Cim { get; set; } = string.Empty;
        public string Leiras { get; set; } = string.Empty;
        public string Telefon { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string NyitvatartasHetfo { get; set; } = string.Empty;
        public string NyitvatartasHeto { get; set; } = string.Empty;
        public string NyitvatartasVasarnap { get; set; } = string.Empty;
        public double Szelesseg { get; set; }   // latitude
        public double Hosszusag { get; set; }   // longitude
        public bool Aktiv { get; set; } = true;
        public List<string> Szolgaltatasok { get; set; } = new();
        public string Kep { get; set; } = string.Empty;   // emoji vagy URL
    }
}
