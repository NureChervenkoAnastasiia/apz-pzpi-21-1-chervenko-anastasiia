using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TastifyAPI.DTOs
{
    public class ProductDto
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public Double? Amount { get; set; }
    }
}
