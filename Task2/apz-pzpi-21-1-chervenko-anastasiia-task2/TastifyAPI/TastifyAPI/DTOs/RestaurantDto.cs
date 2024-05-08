namespace TastifyAPI.DTOs
{
    public class RestaurantDto
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Telephone { get; set; }
        public string? Email { get; set; }
        public string? Info { get; set; }
        public List<string>? Cuisine { get; set; }
    }
}
