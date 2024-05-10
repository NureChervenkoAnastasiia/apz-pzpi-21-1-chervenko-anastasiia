using System.ComponentModel.DataAnnotations;

namespace TastifyAPI.DTOs.Features_DTOs
{
    public class StaffRegistrationDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Position { get; set; }

        [Required]
        public double HourlySalary { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]{12}$", ErrorMessage = "Mobile number must contain exactly 10 digits")]
        public string Phone { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]{3,20}$", ErrorMessage = "Login must be at least 3 characters long and contain only alphanumeric characters")]
        public string Login { get; set; }

        [Required]
        [MinLength(8)]
        [RegularExpression(@"^(?=.*\d).*$", ErrorMessage = "Password must be at least 8 characters long and contain at least one numeric digit")]
        public string Password { get; set; }
    }
}
