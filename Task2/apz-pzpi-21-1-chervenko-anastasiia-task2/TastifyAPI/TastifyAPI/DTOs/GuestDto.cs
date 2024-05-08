using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TastifyAPI.DTOs
{
    public class GuestDto
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? MobileNumber { get; set; }
        public Int32 Bonus { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
    }
}

