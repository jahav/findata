using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FinData.Infrastructure;
using Moq;
using Xunit;

namespace JusticeRA.Tests
{
    public class LimiterTest : IDisposable
    {
        private Limiter limiter;
        private Mock<Func<TimeSpan, Task>> delay;

        public LimiterTest()
        {
            delay = new Mock<Func<TimeSpan, Task>>();
            limiter = new Limiter(2, 5, delay.Object);
        }

        [Fact]
        public async Task FirstCallIsPerformed()
        {
            TimeProvider.Instance = new StaticTimeProvider(new DateTime(2018, 6, 20, 1, 32, 14));
            var value = limiter.DoAsync(Do);
            Assert.True(value.IsCompleted);
        }

        [Fact]
        public void MinuteLimitWillRequireNoMoreThanSpecifiedCallsInLastMinute()
        {
            Call(limiter, new DateTime(2018, 6, 20, 1, 32, 15), null);
            Call(limiter, new DateTime(2018, 6, 20, 1, 32, 30), null);
            Call(limiter, new DateTime(2018, 6, 20, 1, 32, 50), TimeSpan.FromSeconds(25)); // Action at 33:15
            Call(limiter, new DateTime(2018, 6, 20, 1, 33, 16), TimeSpan.FromSeconds(14)); // Action at 33:30
            Call(limiter, new DateTime(2018, 6, 20, 1, 34, 15), null);                     // Action at 34:15
        }

        private void Call(Limiter limiter, DateTime time, TimeSpan? expectedWaitTime)
        {
            TimeProvider.Instance = new StaticTimeProvider(time);
            delay.ResetCalls();
            if (expectedWaitTime == null)
            {
                delay.Setup(delay => delay(It.IsAny<TimeSpan>()));
            }
            else
            {
                delay.Setup(delay => delay(It.Is<TimeSpan>(waitTime => waitTime == expectedWaitTime.Value)))
                    .Returns<TimeSpan>(x =>
                    {
                        var b = new StaticTimeProvider(TimeProvider.UtcNow + x);
                        TimeProvider.Instance = b;
                        return Task.Delay(0);
                    });
            }
            limiter.DoAsync(Do).Wait();
            delay.Verify();
            if (expectedWaitTime == null)
            {
                Assert.Equal(time, TimeProvider.UtcNow);
            }
            else
            {
                Assert.Equal(time + expectedWaitTime, TimeProvider.UtcNow);
            }
        }

        public async Task<int> Do()
        {
            return 1;
        }

        public void Dispose()
        {
            TimeProvider.Instance = new DateTimeProvider();
        }
    }
}
