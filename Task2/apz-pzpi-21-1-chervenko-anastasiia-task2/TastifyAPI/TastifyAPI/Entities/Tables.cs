using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TastifyAPI.Entities
{
    public class Tables
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("number"), BsonRepresentation(BsonType.String)]
        public string? Number { get; set; }

        [BsonElement("status"), BsonRepresentation(BsonType.String)]
        public string? Status { get; set; }

    }
}
