using Xunit;
using AntiFraudService;

namespace AntiFraudService.Tests
{
    public class AntiFraudLogicTests
    {
        [Theory]
        [InlineData(100, 100, "Approved")]
        [InlineData(2500, 100, "Rejected")]
        [InlineData(100, 21000, "Rejected")]
        [InlineData(2000, 19999, "Approved")]
        [InlineData(2001, 19999, "Rejected")]
        [InlineData(2000, 20000, "Approved")]
        [InlineData(2000.01, 20000, "Rejected")]
        [InlineData(2000, 20000.01, "Rejected")]
        public void GetStatus_ShouldReturnCorrectStatus(decimal value, decimal dailyTotal, string expectedStatus)
        {
            var result = AntiFraudLogic.GetStatus(value, dailyTotal);
            Assert.Equal(expectedStatus, result);
        }

    }
}