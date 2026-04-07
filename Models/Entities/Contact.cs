using System;
using System.ComponentModel.DataAnnotations;

namespace LUXURY_DRIVE.Models.Entities
{
    public class Contact
    {
        [Key]
        public int ContactId { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Email { get; set; }
        
        [Required]
        public string Message { get; set; }
        
        public DateTime SubmittedAt { get; set; } = DateTime.Now;
    }
}
