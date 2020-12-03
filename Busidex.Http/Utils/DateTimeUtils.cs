using System;

namespace Busidex.Http.Utils
{
    public class DateTimeUtils
    {
        public static double DateDiffDays(DateTime start, DateTime end)
        {
            return start.Subtract(end).TotalDays;
        }
    }
}
