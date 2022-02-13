using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BradyTask.Models.Home
{
    public class Weather
    {
        [JsonPropertyName("main")]
        public string Main { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }

    public class Rain
    {
        [JsonPropertyName("_1h")]
        public double Hour { get; set; }
    }

    public class Current
    {
        [JsonPropertyName("temp")]
        public double Temp { get; set; }

        [JsonPropertyName("feels_like")]
        public double FeelsLike { get; set; }

        [JsonPropertyName("weather")]
        public List<Weather> Weather { get; set; }

        [JsonPropertyName("rain")]
        public Rain Rain { get; set; }
    }

    public class Temp
    {
        [JsonPropertyName("min")]
        public double Min { get; set; }

        [JsonPropertyName("max")]
        public double Max { get; set; }
    }

    public class Daily
    {
        [JsonPropertyName("temp")]
        public Temp Temp { get; set; }

        [JsonPropertyName("weather")]
        public List<WeatherData> Weather { get; set; }
    }

    public class WeatherData
    {
        [JsonPropertyName("current")]
        public Current Current { get; set; }

        [JsonPropertyName("daily")]
        public List<Daily> Daily { get; set; }

        public string ErrorMessage { get; set; }
    }
}
