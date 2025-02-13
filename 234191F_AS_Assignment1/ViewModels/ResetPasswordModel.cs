using System.ComponentModel.DataAnnotations;

namespace _234191F_AS_Assignment1.ViewModels
{
    public class ResetPasswordModel
    {
        [Required]
        public string Token { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(ConfirmPassword), ErrorMessage = "The passwords do not match.")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
