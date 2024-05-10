using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TastifyAPI.DTOs
{
    public class TableDto
    {
        public string? Id { get; set; }
        public int? Number { get; set; }
        public string? Status { get; set; }
    }
}
