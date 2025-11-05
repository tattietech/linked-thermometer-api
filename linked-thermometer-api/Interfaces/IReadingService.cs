namespace linked_thermometer_api.Interfaces
{
    using linked_thermometer_api.Models;

    public interface IReadingService
    {
        public Task<bool> SendReading(Reading reading);
        public Task<List<Reading>> GetLatestReadings();
    }
}
