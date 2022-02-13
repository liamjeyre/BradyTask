using System.Text.Json.Serialization;

namespace BradyTask.Models.Home
{
    public class LocationData
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("lat")]
        public double Lat { get; set; }

        [JsonPropertyName("lon")]
        public double Lon { get; set; }

        public string ErrorMessage { get; set; }
    }
}
