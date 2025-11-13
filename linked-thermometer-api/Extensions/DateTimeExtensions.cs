namespace linked_thermometer_api.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime RoundDownToTimeSpan(this DateTime dateTime, TimeSpan interval)
        {
            return dateTime.AddTicks(-(dateTime.Ticks % interval.Ticks));
        }
    }
}
