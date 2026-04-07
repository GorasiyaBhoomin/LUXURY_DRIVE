using System.ComponentModel.DataAnnotations;

namespace LUXURY_DRIVE.Models.Entities
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }
        
        [MaxLength(50)]
        public string? Username { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Email { get; set; }
        
        [Required]
        public string PasswordHash { get; set; }
        
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }
        
        [MaxLength(20)]
        public string Role { get; set; } = "User";
    }
}
