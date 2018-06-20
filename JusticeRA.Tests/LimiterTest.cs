using System;
using System.Threading.Tasks;
using FinData.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace JusticeRA.Tests
{
    [TestClass]
    public class LimiterTest
    {
        [TestMethod]
        public async Task FirstCallIsPerformed()
        {
            TimeProvider.Instance = new StaticTimeProvider(new DateTime(2018, 6, 20, 1, 32, 14));
            var limiter = new Limiter(1, 10);
            var value = limiter.DoAsync(Do);
            Assert.IsTrue(value.IsCompleted);
        }

        [TestMethod]
        public async Task SecondCallWithinMinuteWaitsUntilMinutePasses()
        {
            TimeProvider.Instance = new StaticTimeProvider(new DateTime(2018, 6, 20, 1, 32, 15));
            var delay = new Mock<Func<TimeSpan, Task>>();
            var expectedTimeSpan = new TimeSpan(0, 0, 10);
            Func<TimeSpan, Task> q = Wait;
            delay.Setup(x => Wait(TimeSpan.FromSeconds(1)));
            var limiter = new Limiter(1, 10, delay.Object);
            var value = limiter.DoAsync(Do);
            delay.VerifyNoOtherCalls();

            TimeProvider.Instance = new StaticTimeProvider(new DateTime(2018, 6, 20, 1, 33, 05));
            value = limiter.DoAsync(Do);
            Assert.IsFalse(value.IsCompleted);
            await value;
            delay.Verify(x => x(It.Is<TimeSpan>(y => y == expectedTimeSpan)));
        }

        public async Task<int> Do()
        {
            return 1;
        }

        public async Task Wait(TimeSpan a)
        {
            await Task.Delay(100);
        }
    }
}
