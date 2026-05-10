using System.ComponentModel.DataAnnotations;

namespace EchoHub.Models
{
    public class User 
    {
        public int Id { get; set; }

        // NAME
        [Required(ErrorMessage = "Name is Required")]
        [RegularExpression(@"^[a-zA-Z\s]+$",
            ErrorMessage = "Only letters are allowed.")]
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
        public string Name { get; set; }

        // EMAIL
        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [RegularExpression(@"^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}$",
            ErrorMessage = "Email must be lowercase only.")]
        public string Email { get; set; }

        // PHONE NUMBER
        [Required(ErrorMessage = "Phone Number is Required")]
        [RegularExpression(@"^[0-9]+$",
            ErrorMessage = "Phone number must contain numbers only.")]
        [StringLength(11, MinimumLength = 11,
            ErrorMessage = "Phone number must be exactly 11 digits.")]
        public string PhoneNumber { get; set; }

        // PASSWORD
        [Required(ErrorMessage = "Password is Required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "Password must contain uppercase, lowercase, number, and special character.")]
        public string Password { get; set; }

        //Authorization: Determines system access levels (e.g., "Admin", "Staff", "User")
        public string Role { get; set; } = "User";

        //Automatically records when the account was created
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
