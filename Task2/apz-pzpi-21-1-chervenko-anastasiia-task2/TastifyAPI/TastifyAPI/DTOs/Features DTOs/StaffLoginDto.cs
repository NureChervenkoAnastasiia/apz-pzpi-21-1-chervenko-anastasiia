using System.ComponentModel.DataAnnotations;

namespace TastifyAPI.DTOs
{
    public class StaffLoginDto
    {
        [Required]
        public string? Login { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
