namespace TransportApi.Models
{
    public class LiveLocationDto
    {
        public int SchoolId { get; set; }
        public string? VehicleNo { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Speed { get; set; }
    }

    
}
