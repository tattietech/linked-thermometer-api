namespace linked_thermometer_api.Models
{
    using Newtonsoft.Json;

    public class GraphReading
    {
        [JsonProperty("deviceName")]
        public string DeviceName { get; set; } = string.Empty;

        [JsonProperty("avgTemp")]
        public float AvgTemp {  get; set; }

        [JsonProperty("minTemp")]
        public float MinTemp { get; set; }

        [JsonProperty("maxTemp")]
        public float MaxTemp { get; set; }

        [JsonProperty("avgHum")]
        public float AvgHum { get; set; }

        [JsonProperty("minHum")]
        public float MinHum { get; set; }

        [JsonProperty("maxHum")]
        public float MaxHum { get; set; }
    }
}
