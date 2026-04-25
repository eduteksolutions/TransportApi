using System.Text.Json.Serialization;

namespace TransportApi.Models
{
    public class RouteModel
    {
        [JsonPropertyName("routeCode")]
        public int RouteCode { get; set; }

        [JsonPropertyName("routeName")]
        public string RouteName { get; set; } = string.Empty;

        [JsonPropertyName("vehicleNo")]
        public string VehicleNo { get; set; } = string.Empty;

        [JsonPropertyName("morningTime")]
        public string MorningTime { get; set; } = string.Empty;

        [JsonPropertyName("eveningTime")]
        public string EveningTime { get; set; } = string.Empty;

        [JsonPropertyName("validDate")]
        public string ValidDate { get; set; } = string.Empty;

        [JsonPropertyName("students")]
        public List<TransportStudent> Students { get; set; } = new();
    }
}
