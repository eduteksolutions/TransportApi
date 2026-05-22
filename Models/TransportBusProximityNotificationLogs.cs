namespace TransportApi.Models
{
    
        public class TransportBusProximityNotificationLogs
        {
            public int Id { get; set; }
            public string AdmCd { get; set; } = "";
            public string UserId { get; set; } = "";
            public string DeviceId { get; set; } = "";
            public double BusLat { get; set; }
            public double BusLng { get; set; }
            public double StudentLat { get; set; }
            public double StudentLng { get; set; }
            public double DistanceMeters { get; set; }
            public string Message { get; set; } = "";
            public bool IsSuccess { get; set; }
            public DateTime SentAt { get; set; }
        }
    
}
