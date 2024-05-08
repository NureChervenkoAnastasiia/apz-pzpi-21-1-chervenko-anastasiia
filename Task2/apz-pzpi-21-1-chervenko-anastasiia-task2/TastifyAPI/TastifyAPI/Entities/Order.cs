using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TastifyAPI.Entities
{
    public class Order
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        [BsonElement("name"), BsonRepresentation(BsonType.String)]
        public string? Name { get; set; }

        [BsonElement("table_id"), BsonRepresentation(BsonType.String)]
        public string? TableId { get; set; }

        [BsonElement("date_time"), BsonRepresentation(BsonType.DateTime)]
        public DateTime? DateTime { get; set; }

        [BsonElement("comment"), BsonRepresentation(BsonType.String)]
        public string? Comment { get; set; }

        [BsonElement("status"), BsonRepresentation(BsonType.String)]
        public string? Status { get; set; }

    }
}
