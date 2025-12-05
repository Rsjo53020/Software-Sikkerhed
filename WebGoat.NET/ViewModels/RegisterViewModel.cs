using System.ComponentModel.DataAnnotations;

namespace WebGoatCore.ViewModels
{
    public class RegisterViewModel
    {
        [Display(Name = "User Name")]
        [Required(ErrorMessage = "Please enter your user name")]
        [StringLength(50, MinimumLength = 3,
            ErrorMessage = "User name must be between {2} and {1} characters.")]
        [RegularExpression(
            @"^[a-zA-Z0-9._\-@]+$",
            ErrorMessage = "User name may only contain letters, digits, '.', '-', '_' and '@'.")]
        public string Username { get; set; } = string.Empty;

        [Display(Name = "E-mail")]
        [Required(ErrorMessage = "Please enter your company email")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [RegularExpression(
            @"^[a-zA-Z0-9._%+\-]+@[a-zA-Z0-9.\-]+\.[A-Za-z]{2,}$",
            ErrorMessage = "Indtast en gyldig emailadresse.")]
        [StringLength(100,
            ErrorMessage = "E-mail must be between {2} and {1} characters long.",
            MinimumLength = 5)]
        [DataType(DataType.EmailAddress)]

        public string Email { get; set; } = string.Empty;

        [Display(Name = "Company Name")]
        [Required(ErrorMessage = "Please enter your company name")]
        [StringLength(100,
            ErrorMessage = "Company name must be between {2} and {1} characters long.",
            MinimumLength = 2)]
        public string CompanyName { get; set; } = string.Empty;

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Please enter your password")]
        [StringLength(100,
            ErrorMessage = "Password must be between {2} and {1} characters long.",
            MinimumLength = 8)]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,100}$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter and one digit.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Confirm Password")]
        [Required(ErrorMessage = "Please confirm your password")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password),
            ErrorMessage = "The password and confirmation password do not match")]
        public string ConfirmedPassword { get; set; } = string.Empty;

        [Display(Name = "Address")]
        [StringLength(200, ErrorMessage = "Address must be at most {1} characters long.")]
        public string? Address { get; set; }

        [Display(Name = "City")]
        [StringLength(100, ErrorMessage = "City must be at most {1} characters long.")]
        public string? City { get; set; }

        [Display(Name = "Region/State")]
        [StringLength(100, ErrorMessage = "Region must be at most {1} characters long.")]
        public string? Region { get; set; }

        [Display(Name = "Postal Code")]
        [DataType(DataType.PostalCode)]
        [StringLength(10, ErrorMessage = "Postal code must be at most {1} characters long.")]
        [RegularExpression(
            @"^[0-9A-Za-z\- ]{3,10}$",
            ErrorMessage = "Postal code contains invalid characters.")]
        public string? PostalCode { get; set; }

        [Display(Name = "Country")]
        [StringLength(100, ErrorMessage = "Country must be at most {1} characters long.")]
        public string? Country { get; set; }
    }
}
