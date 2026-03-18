namespace GymWebApiBackend.DTOs
{
    public class RegisterDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Vezeteknev { get; set; } = null!;
        public string Keresztnev { get; set; } = null!;
        public DateTime SzuletesiDatum { get; set; }
    }
}