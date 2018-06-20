using System;

namespace FinData.Infrastructure
{
    public class TimeProvider
    {
        internal static ITimeProvider Instance { get; set; } = new DateTimeProvider();

        public static DateTime Now => Instance.Now;
        public static DateTime UtcNow => Instance.UtcNow;
    }

    internal interface ITimeProvider
    {
        DateTime Now { get; }
        DateTime UtcNow { get; }
    }

    internal class DateTimeProvider : ITimeProvider
    {
        public DateTime Now => DateTime.Now;

        public DateTime UtcNow => DateTime.UtcNow;
    }

    internal class StaticTimeProvider : ITimeProvider
    {
        private DateTime value;
        public StaticTimeProvider(DateTime staticTime)
        {
            value = staticTime;
        }

        public DateTime Now => value;

        public DateTime UtcNow => value;
    }
}
