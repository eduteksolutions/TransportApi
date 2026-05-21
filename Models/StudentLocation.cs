namespace TransportApi.Models
{
    public class StudentLocation
    {
        public int Id { get; set; }

        public string AdmCd { get; set; } = "";

        public string UserId { get; set; } = "";

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
