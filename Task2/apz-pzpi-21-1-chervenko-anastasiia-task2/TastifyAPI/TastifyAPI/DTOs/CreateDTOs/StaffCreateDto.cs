using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TastifyAPI.DTOs.CreateDTOs
{
    public class StaffCreateDto
    {
        public string? Name { get; set; }
        public string? Position { get; set; }
        public double? HourlySalary { get; set; }
        public Int64? Telephone { get; set; }
        public Int32? AttendanceCard { get; set; }
        public string? Login { get; set; }
        public string? PasswordHash { get; set; }
    }
}
