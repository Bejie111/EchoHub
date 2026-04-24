using System.ComponentModel.DataAnnotations;

namespace EchoHub.Models
{
    public class User 
    {
        public int Id { get; set; }

        //Validation: Ensures the user provides a name 
        [Required(ErrorMessage = "Name is Required")] 

        public string Name { get; set; }

        //Validation: Ensures uniqueness and correct format for login credentials
        [Required (ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        //Validation: Ensures the user provide a password
        [Required(ErrorMessage = "Password is Required")]
        public string Password { get; set; }

        //Authorization: Determines system access levels (e.g., "Admin", "Staff", "User")
        [Required]
        public string Role { get; set; }

        //Automatically records when the account was created
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
