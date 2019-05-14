using Kokosoft.SwimmingPoolTracker.CheckNewSchedule;
using Kokosoft.SwimmingPoolTracker.CheckNewSchedule.Helpers;
using Kokosoft.SwimmingPoolTracker.CheckNewSchedule.Model;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace CheckNewScheduleTests
{
    public class GetValuesFromDateStringShould
    {
        [Fact]
        public void SetCorrectFromAndToValues()
        {
            string fromDate = "28 grudnia 2019";
            string toDate = "2 stycznia 2020";

            PoolSchedule poolSchedule = new PoolSchedule();
            CheckNewScheduleBackgroundService check = new CheckNewScheduleBackgroundService(new MonthHelpers(), null, null, null, null);
            check.GetValuesFromDateString(fromDate, poolSchedule, DateType.From);
            check.GetValuesFromDateString(toDate, poolSchedule, DateType.To);

            Assert.Equal(28, poolSchedule.DayFrom);
            Assert.Equal(12, poolSchedule.MonthFrom);
            Assert.Equal(2019, poolSchedule.YearFrom);

            Assert.Equal(2, poolSchedule.DayTo);
            Assert.Equal(1, poolSchedule.MonthTo);
            Assert.Equal(2020, poolSchedule.YearTo);
        }

        [Theory]
        [InlineData("6", DateType.From)]
        public void SetCorrectFromValues(string date, DateType dateType)
        {
            PoolSchedule poolSchedule = new PoolSchedule();
            CheckNewScheduleBackgroundService check = new CheckNewScheduleBackgroundService(new MonthHelpers(), null, null, null, null);
            check.GetValuesFromDateString(date, poolSchedule, dateType);

            Assert.Equal(6, poolSchedule.DayFrom);
        }

        [Theory]
        [InlineData("12 maja 2019", DateType.To)]
        public void SetCorrectToValues(string date, DateType dateType)
        {
            PoolSchedule poolSchedule = new PoolSchedule();
            CheckNewScheduleBackgroundService check = new CheckNewScheduleBackgroundService(new MonthHelpers(), null, null, null, null);
            check.GetValuesFromDateString(date, poolSchedule, dateType);

            Assert.Equal(12, poolSchedule.DayTo);
            Assert.Equal(5, poolSchedule.MonthTo);
            Assert.Equal(2019, poolSchedule.YearTo);
        }
    }
}
