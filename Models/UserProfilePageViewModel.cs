namespace LUXURY_DRIVE.Models
{
    public class UserProfilePageViewModel
    {
        public UserProfileViewModel Profile { get; set; } = new();
        public UserChangePasswordViewModel Password { get; set; } = new();
    }
}