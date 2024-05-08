namespace TastifyAPI.DTOs
{
    public class BookingDto
    {
        public string? Id { get; set; }
        public string? TableId { get; set; }
        public string? GuestId { get; set; }
        public DateTime DateTime { get; set; }
        public Int32? PersonsCount { get; set; }
        public string? Comment { get; set; }
    }
}
