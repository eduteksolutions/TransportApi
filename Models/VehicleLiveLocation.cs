namespace TransportApi.Models
{
    public class VehicleLiveLocation
    {
        public int Id { get; set; }            // DB Primary Key
        public int SchoolId { get; set; }
        public string? VehicleNo { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Speed { get; set; }
        public DateTime UpdatedAt { get; set; } // Server time
    }
}
