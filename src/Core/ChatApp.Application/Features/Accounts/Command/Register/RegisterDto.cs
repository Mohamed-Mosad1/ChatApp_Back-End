namespace ChatApp.Application.Features.Accounts.Command.Register
{
    public class RegisterDto
    {
        public string? UserName { get; set; }
        public string? KnownAs { get; set; }
        public string? Gender { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
