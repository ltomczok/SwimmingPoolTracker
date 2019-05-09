using Kokosoft.SwimmingPoolTracker.ImportSchedule;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ImportScheduleTests
{
    public class ImportPoolScheduleShould
    {
       
        private readonly Kokosoft.SwimmingPoolTracker.ImportSchedule.ImportPoolSchedule i = new Kokosoft.SwimmingPoolTracker.ImportSchedule.ImportPoolSchedule(null, null, null);

        [Theory]
        [InlineData("06:00")]
        [InlineData("13:00")]
        [InlineData("11:23")]
        [InlineData("23:59")]
        [InlineData("00:00")]
        public void AcceptValidTimeFormat(string time)
        {
            Assert.True(i.IsValidTimeFormat(time));
        }

        [Theory]
        [InlineData("")]
        [InlineData("test")]
        [InlineData("6:00")]
        [InlineData("25:00")]
        [InlineData("25100")]
        public void RejectInvalidTimeFormat(string time)
        {
            Assert.False(i.IsValidTimeFormat(time));
        }

        [Fact]
        public async Task ShouldDownloadFile()
        {
            Assert.True(await i.DownloadFile("https://mzuk.gliwice.pl/wp-content/uploads/2019/05/niecka-od-6-do-12-maja-2019.pdf", @"c:\Temp\test.pdf"));
        }
    }
}
