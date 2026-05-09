namespace TransportApi.Models
{
    using System;

    namespace TransportApi.Models
    {
        public class VehicleDeviceLiveLocation
        {
            public int Id { get; set; }

            // 🏫 Multi-school / tenant support
            public int SchoolId { get; set; }

            // 📡 DEVICE ID (GPS Unique ID)
            public string DeviceId { get; set; } = string.Empty;

            // 📍 LOCATION
            public double Latitude { get; set; }
            public double Longitude { get; set; }

            // 🚗 TELEMETRY
            public double? Speed { get; set; }
            public double? Altitude { get; set; }
            public double? Course { get; set; }

            // 🔋 DEVICE STATUS
            public double? BatteryLevel { get; set; }
            public bool? Ignition { get; set; }
            public bool? Motion { get; set; }

            // ⏱ LAST UPDATE
            public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

            // 📦 RAW GPS JSON (debug / future use)
            public string? RawData { get; set; }
        }
    }
}
