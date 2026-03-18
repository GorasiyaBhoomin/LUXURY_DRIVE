namespace LUXURY_DRIVE.Models
{
    public class AdminProfilePageViewModel
    {
        public AdminProfileViewModel Profile { get; set; } = new();
        public AdminChangePasswordViewModel Password { get; set; } = new();
    }
}
