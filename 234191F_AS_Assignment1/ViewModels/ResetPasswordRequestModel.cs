using System.ComponentModel.DataAnnotations;

namespace _234191F_AS_Assignment1.ViewModels
{
    public class ResetPasswordRequestModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
