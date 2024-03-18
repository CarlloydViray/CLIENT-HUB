using System.Globalization;

namespace BPOI_HUB.modules.core
{ 
    internal class DateTimeOps
    {
        public static bool ValidateDate(string date, string format = "MM/dd/yyyy")
        {
            return DateTime.TryParseExact(date, format,
                                    CultureInfo.InvariantCulture,
                                    DateTimeStyles.None,
                                    out _);

        }

        public static DateTime ConvertExcelDateNumber(string date)
        {
            double excelDate = double.Parse(date);

            return DateTime.FromOADate(excelDate);

        }

        public static int GetRegularDaysCount(int month, int year)
        {
            List<DateTime> dateList = Enumerable.Range(1, DateTime.DaysInMonth(year, month))
                    .Select(day => new DateTime(year, month, day))
                    .Where(dt => dt.DayOfWeek != DayOfWeek.Sunday &&
                                 dt.DayOfWeek != DayOfWeek.Saturday)
                    .ToList();

            return dateList.Count;
        }

        public static decimal GetTimeDiff(string timeIn, string timeOut)
        {
            TimeSpan duration = DateTime.Parse(timeOut).Subtract(DateTime.Parse(timeIn));

            return duration.Hours + (decimal)duration.Minutes / (decimal)60;
        }
        public static decimal GetTimeDiff(DateTime timeIn, DateTime timeOut)
        {
            TimeSpan duration = timeOut.Subtract(timeIn);

            return duration.Hours + (decimal)duration.Minutes / (decimal)60;
        }

        public static string ConvertExcelDecimalTimeToString(decimal value) 
        {
            DateTime dateTime = DateTime.MinValue.AddHours((double)value * 24);
            return dateTime.ToString("hh:mm tt");
        }
    }

    public class TimeRange
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public TimeRange(DateTime startTime, DateTime endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
        }

        public TimeSpan ComputeIntersection(TimeRange otherRange)
        {
            DateTime intersectionStart = StartTime > otherRange.StartTime ? StartTime : otherRange.StartTime;
            DateTime intersectionEnd = EndTime < otherRange.EndTime ? EndTime : otherRange.EndTime;

            if (intersectionStart <= intersectionEnd)
            {
                TimeSpan diff = intersectionEnd - intersectionStart;

                return diff;
            }

            // No intersection, return null or a special value to indicate this.
            return TimeSpan.Zero;
        }
    }
}
