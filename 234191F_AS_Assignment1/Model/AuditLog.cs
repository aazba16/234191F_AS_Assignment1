using System;
using System.ComponentModel.DataAnnotations;

namespace _234191F_AS_Assignment1.Model
{
    public class AuditLog
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]  // Ensure UserId is a valid email address
        public string UserId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 5)] // Ensure Action has a minimum length of 5 characters and max length of 100
        public string Action { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        [Required]
        [StringLength(200)]  // Ensure DeviceInfo is not too long (max length of 200)
        public string DeviceInfo { get; set; }

        [Required]
        [StringLength(50)]  // Ensure IP Address is not too long (max length of 50 for simplicity)
        public string IpAddress { get; set; }

        [StringLength(100)]  // SessionId is optional, but if provided, should be max 100 chars
        public string? SessionId { get; set; }  // Unique Session ID for each login

        [Required]
        public bool IsActive { get; set; }  // Ensure IsActive is always set
    }
}
