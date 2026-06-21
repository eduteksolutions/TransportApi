namespace TransportApi.Models
{
    public class TransportStationMaster
    {
        public int StCode { get; set; }
        public string StName { get; set; }
        public int SubRtCd { get; set; }
        public string LoginName { get; set; }
        public DateTime lUserDt { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public TimeSpan? ArrivalTime { get; set; }
        public TimeSpan? DepartureTime { get; set; }
        public int UserID { get; set; }
        public double? startLat { get; set; }
        public double? startLng { get; set; }
        public double? endLat { get; set; }
        public double? endLng { get; set; }
    }
}
