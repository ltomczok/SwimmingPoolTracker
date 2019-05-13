using Kokosoft.SwimmingPoolTracker.CheckNewSchedule.Helpers;
using System;
using Xunit;

namespace CheckNewScheduleTests
{
    public class MonthHelpersShould
    {
        MonthHelpers mh = new MonthHelpers();

        [Theory]
        [InlineData("test")]
        [InlineData("jan")]
        [InlineData("")]
        [InlineData("lu")]
        public void CheckIfMonth_ReturnFalse(string month)
        {
            Assert.False(mh.CheckIfMonth(month));
        }

        [Theory]
        [InlineData("Maj")]
        [InlineData("maj")]
        [InlineData("MAJ")]
        public void GetMonthNumber_Return_5(string month)
        {
            Assert.True(mh.CheckIfMonth(month));
        }
    }
}
