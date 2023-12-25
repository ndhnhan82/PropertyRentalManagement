namespace PropertyRentalManagement.Models
{
    public class UserRolesViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? UserName { get; set; }
        public string Address { get; set; }
        public byte[]? ProfilePicture { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public IEnumerable<string>? Roles { get; set; }
    }
}
