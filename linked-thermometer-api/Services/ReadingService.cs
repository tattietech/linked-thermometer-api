namespace linked_thermometer_api.Services
{
    using linked_thermometer_api.Extensions;
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
            reading.DayGroup = reading.TimeStamp.RoundDownToTimeSpan(TimeSpan.FromDays(1));
            reading.HourGroup = reading.TimeStamp.RoundDownToTimeSpan(TimeSpan.FromHours(1));
            reading.QuarterHourGroup = reading.TimeStamp.RoundDownToTimeSpan(TimeSpan.FromMinutes(15));

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

        public async Task<List<GraphReading>> GetGraphReadings(string deviceId, DateTime from, DateTime to, string granularity)
        {
            string queryString =
                "SELECT c.quarterHourGroup," +
                "AVG(c.temperature) AS avgTemp," +
                "MIN(c.temperature) AS minTemp," +
                "MAX(c.temperature) AS maxTemp," +
                "AVG(c.humidity) AS avgHum," +
                "MIN(c.humidity) AS minHum," +
                "MAX(c.humidity) AS maxHum " +
                "FROM c " +
                $"WHERE c.deviceId = '{deviceId}' AND " +
                $"c.quarterHourGroup >= '{from}' AND " +
                $"c.quarterHourGroup < '{to}'";

            switch (granularity)
            {
                case "quarterHourly":
                    queryString += "GROUP BY c.quarterHourGroup ORDER BY c.quarterHourGroup";
                    break;
                case "hourly":
                    queryString += "GROUP BY c.hourGroup ORDER BY c.hourGroup";
                    break;
                case "daily":
                    queryString += "GROUP BY c.dayGroup ORDER BY c.dayGroup";
                    break;
                default:
                    queryString += "GROUP BY c.hourGroup ORDER BY c.hourGroup";
                    break;
            }

            var query = new QueryDefinition(queryString);

            using FeedIterator<GraphReading> feed = _container.GetItemQueryIterator<GraphReading>(
                queryDefinition: query
            );

            var readings = new List<GraphReading>();
            while (feed.HasMoreResults)
            {
                FeedResponse<GraphReading> response = await feed.ReadNextAsync();
                foreach (GraphReading item in response)
                {
                    item.DeviceName = _configuration[deviceId] ?? string.Empty;
                    readings.Add(item);
                }
            }

            return readings;
        }
    }
}
