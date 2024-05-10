namespace TastifyAPI.DTOs
{
    public class StaffDto
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Position { get; set; }
        public double? HourlySalary { get; set; }
        public Int64? Telephone { get; set; }
        public Int32? AttendanceCard { get; set; }
        public string? Login { get; set; }
        public string? PasswordHash { get; set; }
    }
}
