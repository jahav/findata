using FinData.Infrastructure;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JusticeRA
{
    /// <summary>
    /// A class that delays actions, until the limits are passed so we don't get banned 
    /// </summary>
    public class Limiter
    {
        private int limitPerMinute;
        private int limitPerDay;

        /// <summary>
        /// Sliding list of UTC times when the action was called. The first elements are oldest.
        /// </summary>
        private readonly List<DateTime> actions = new List<DateTime>();

        private readonly Func<TimeSpan, Task> delay = Task.Delay;

        /// <summary>
        /// Constructor for testin.
        /// </summary>
        internal Limiter(int limitPerMinute, int limitPerDay, Func<TimeSpan, Task> delay) 
            : this(limitPerMinute, limitPerDay)
        {
            this.delay = delay;
        }

        public Limiter(int limitPerMinute, int limitPerDay)
        {
            if (limitPerMinute < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(limitPerMinute));
            }

            this.limitPerMinute = limitPerMinute;
            if (limitPerDay < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(limitPerDay));
            }

            this.limitPerDay = limitPerDay;
        }

        public async Task<T> DoAsync<T>(Func<Task<T>> function)
        {
            var now = TimeProvider.UtcNow;
            var dayAgo = now.AddDays(-1);
            var dayCount = CountActionsSince(dayAgo);
            if (dayCount >= limitPerDay)
            {
                var allowedLimitTime = actions[actions.Count - limitPerMinute];
                var waitTime = now - allowedLimitTime;
                await Task.Delay(waitTime);
            }
            var minuteAgo = now.AddMinutes(-1);
            var minuteCount = CountActionsSince(minuteAgo);
            if (minuteCount >= limitPerMinute)
            {
                var allowedLimitTime = actions[actions.Count - limitPerMinute];
                var elapsedTime = now - allowedLimitTime;
                var waitTime = (TimeSpan.FromMinutes(1) - elapsedTime);
                await delay(waitTime);
            }
            actions.Add(now);
            CleanUpActions(now);
            return await function();
        }

        private int CountActionsSince(DateTime reference)
        {
            return actions.Count(x => x > reference);
        }

        private void CleanUpActions(DateTime now)
        {
            var dayAgo = now.AddDays(-1);
            while (actions.Count > 0 && actions[0] <= dayAgo)
            {
                actions.RemoveAt(0);
            }
        }
    }
}
