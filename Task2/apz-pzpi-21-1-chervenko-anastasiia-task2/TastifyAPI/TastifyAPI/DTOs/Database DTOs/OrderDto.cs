using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TastifyAPI.DTOs
{
    public class OrderDto
    {
        public string? Id { get; set; }
        public int? Number { get; set; }
        public string? TableId { get; set; }
        public DateTime? DateTime { get; set; }
        public string? Comment { get; set; }
        public string? Status { get; set; }
    }
}
