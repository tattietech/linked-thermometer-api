namespace linked_thermometer_api.Models
{
    using Newtonsoft.Json;

    public class Reading
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("deviceId")]
        public string DeviceId { get; set; } = string.Empty;

        [JsonProperty("deviceName")]
        public string DeviceName { get; set; } = string.Empty;

        [JsonProperty("temperature")]
        public float Temperature {  get; set; }

        [JsonProperty("humidity")]
        public float Humidity { get; set; }

        [JsonProperty("timeStamp")]
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;

        [JsonProperty("partitionKey")]
        public string PartitionKey { get; set; } = string.Empty;
    }
}
