using System.ComponentModel.DataAnnotations;

namespace TastifyAPI.DTOs.Features_DTOs
{
    public class GuestRegistrationDto
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }

        [RegularExpression(@"^[0-9]{12}$", ErrorMessage = "Mobile number must contain exactly 10 digits")]
        public string Phone { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*\d).*$")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string Login {  get; set; }
    }
}
