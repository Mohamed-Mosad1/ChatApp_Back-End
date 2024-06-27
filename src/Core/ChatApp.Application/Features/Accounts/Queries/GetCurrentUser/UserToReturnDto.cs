namespace ChatApp.Application.Features.Accounts.Queries.GetCurrentUser
{
    public class UserToReturnDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
