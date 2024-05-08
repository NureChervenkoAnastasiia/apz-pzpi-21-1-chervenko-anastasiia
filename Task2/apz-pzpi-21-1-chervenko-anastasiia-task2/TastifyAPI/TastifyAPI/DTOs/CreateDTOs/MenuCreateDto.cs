namespace TastifyAPI.DTOs.CreateDTOs
{
    public class MenuCreateDto
    {
        public string? RestaurantId { get; set; }

        public string? Name { get; set; }

        public Int32 Size { get; set; }

        public Int32? Price { get; set; }

        public string? Info { get; set; }

        public string? Type { get; set; }
    }
}
