using System;

namespace Busidex3.Services.Utils
{
    public class DateTimeUtils
    {
        public static double DateDiffDays(DateTime start, DateTime end)
        {
            return start.Subtract(end).TotalDays;
        }
    }
}
