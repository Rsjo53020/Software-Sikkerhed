using System.ComponentModel.DataAnnotations;

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
namespace WebGoatCore.ViewModels
{
    public class AddUserTempViewModel
    {
        [Display(Name = "New user's name:")]
        [Required(ErrorMessage = "Please enter a user name.")]
        [StringLength(50, MinimumLength = 3,
            ErrorMessage = "User name must be between {2} and {1} characters.")]
        [RegularExpression(@"^[a-zA-Z0-9._\-@]+$",
            ErrorMessage = "User name may only contain letters, digits, '.', '-', '_' and '@'.")]
        public string NewUsername { get; set; } = string.Empty;


        [Display(Name = "Password:")]
        [Required(ErrorMessage = "Please enter a password.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8,
            ErrorMessage = "Password must be between {2} and {1} characters.")]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,100}$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter and one digit.")]
        public string NewPassword { get; set; } = string.Empty;

        [Display(Name = "Email address:")]
        [Required(ErrorMessage = "Please enter an email address.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(100,
        ErrorMessage = "Email must be between {2} and {1} characters.",
        MinimumLength = 5)]
        public string NewEmail { get; set; } = string.Empty;

        [Display(Name = "Make this user an administrator.")]
        public bool MakeNewUserAdmin { get; set; }

        public bool CreatedUser { get; set; }
    }
}
