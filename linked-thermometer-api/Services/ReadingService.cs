namespace linked_thermometer_api.Services
{
    using linked_thermometer_api.Interfaces;
    using linked_thermometer_api.Models;
    using Microsoft.Azure.Cosmos;

    public class ReadingService(Container container, IConfiguration configuration) : IReadingService
    {
        private readonly Container _container = container;
        private readonly IConfiguration _configuration = configuration;

        public async Task<bool> SendReading(Reading reading)
        {
            // store all readings for historical data and analytics
            reading.Id = Guid.NewGuid().ToString();
            reading.PartitionKey = reading.DeviceId;

            await _container.CreateItemAsync(reading);

            // store one reading per device in one partition for easier
            // querying of latest/current readings
            reading.Id = reading.DeviceId;
            reading.PartitionKey = "latest";

            await _container.UpsertItemAsync(reading);

            return true;
        }

        public async Task<List<Reading>> GetLatestReadings()
        {
            var query = new QueryDefinition("SELECT * FROM readings r WHERE r.partitionKey = 'latest'");

            using FeedIterator<Reading> feed = _container.GetItemQueryIterator<Reading>(
                queryDefinition: query
            );

            var readings = new List<Reading>();
            while(feed.HasMoreResults)
            {
                FeedResponse<Reading> response = await feed.ReadNextAsync();
                foreach (Reading item in response)
                {
                    item.DeviceName = _configuration[item.DeviceId] ?? string.Empty;
                    readings.Add(item);
                }
            }

            return readings;
        }
    }
}
