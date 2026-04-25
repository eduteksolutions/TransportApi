using System.Text.Json.Serialization;

namespace TransportApi.Models
{
    public class TransportStudent
    {
        [JsonPropertyName("admCd")]
        public int AdmCd { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("class")]
        public string Class { get; set; } = string.Empty;

        [JsonPropertyName("section")]
        public string Section { get; set; } = string.Empty;

        [JsonPropertyName("fatherName")]
        public string FatherName { get; set; } = string.Empty;

        [JsonPropertyName("motherName")]
        public string MotherName { get; set; } = string.Empty;

        [JsonPropertyName("address")]
        public string Address { get; set; } = string.Empty;

    }
}
