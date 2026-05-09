namespace TransportApi.Models
{
    public class DeviceLocationDto
    {
        // 🏫 Multi-tenant support
        public int SchoolId { get; set; }

        // 📡 GPS DEVICE ID (deviceUniqueId)
        public string DeviceId { get; set; }

        // 📍 LOCATION
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        // 🚗 TELEMETRY (optional)
        public double? Speed { get; set; }
        public double? Altitude { get; set; }
        public double? Course { get; set; }

        // 🔋 DEVICE STATUS
        public double? BatteryLevel { get; set; }
        public bool? Ignition { get; set; }
        public bool? Motion { get; set; }

        // 📦 RAW GPS DATA (optional debugging)
        public string? RawData { get; set; }
    }
}
