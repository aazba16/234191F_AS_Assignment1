using System.ComponentModel.DataAnnotations;

namespace _234191F_AS_Assignment1.Model
{
    public class reCAPTCHASettings
    {
        [Required(ErrorMessage = "SiteKey is required.")]
        [StringLength(100, MinimumLength = 20, ErrorMessage = "SiteKey must be between 20 and 100 characters.")]
        public string SiteKey { get; set; }

        [Required(ErrorMessage = "SecretKey is required.")]
        [StringLength(100, MinimumLength = 20, ErrorMessage = "SecretKey must be between 20 and 100 characters.")]
        public string SecretKey { get; set; }
    }
}
