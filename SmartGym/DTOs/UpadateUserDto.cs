namespace GymWebApiBackend.DTOs
{
    public class UpdateUserDto
    {
        public string Email { get; set; } = "";
        public string Vezeteknev { get; set; } = "";
        public string Keresztnev { get; set; } = "";
        public DateTime SzuletesiDatum { get; set; }
    }
}
