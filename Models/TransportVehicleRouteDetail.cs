namespace TransportApi.Models
{
    public class TransportVehicleRouteDetail
    {
        public int VehicleCode { get; set; }

        public int SerialNo { get; set; }

        public string VehicleNo { get; set; }

        public int RouteCode { get; set; }

        public string DriverCode { get; set; }

        public string CoDriverCode { get; set; }

        public DateTime ValidDate { get; set; }

        public TimeSpan MorningTime { get; set; }

        public TimeSpan EveningTime { get; set; }

        public string LoginName { get; set; }

        public int UserID { get; set; }
    }
}
