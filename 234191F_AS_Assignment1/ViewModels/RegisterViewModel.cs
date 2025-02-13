using System.ComponentModel.DataAnnotations;
using _234191F_AS_Assignment1.Model;
using Microsoft.AspNetCore.Http;

namespace _234191F_AS_Assignment1.ViewModels
{
    public class RegisterViewModel
    {

        [Required]
        [StringLength(100, ErrorMessage = "Full Name cannot exceed 100 characters.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Full Name can only contain letters.")]
        public string FullName { get; set; }

        [Required]
        [RegularExpression(@"^\d{4}-\d{4}-\d{4}-\d{4}$", ErrorMessage = "Credit Card number must be in the format: 1234-5678-1234-5678.")]
        public string CreditCardNo { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [Phone(ErrorMessage = "Invalid phone number.")]
        [RegularExpression(@"^\+?\d{1,4}?[-.\s]?\(?\d{1,3}?\)?[-.\s]?\d{1,4}[-.\s]?\d{1,4}[-.\s]?\d{1,9}$", ErrorMessage = "Phone number format is incorrect.")]
        public string MobileNumber { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Delivery address cannot exceed 500 characters.")]
        public string DeliveryAddress { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+]).{8,}$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Photo is required.")]
        [DataType(DataType.Upload)]
        public IFormFile Photo { get; set; }


        [StringLength(1000, ErrorMessage = "About Me text cannot exceed 1000 characters.")]
        [Required(ErrorMessage = "About Me is required.")]
        public string AboutMe { get; set; }
    }
}
