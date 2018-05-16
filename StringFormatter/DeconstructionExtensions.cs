using System.Runtime.CompilerServices;

namespace System.Text.Formatting
{
    public static class DeconstructionExtensions
    {
        private const long TicksPerMicroSeconds = 10;
        private const long TicksPerMillisecond = 10_000;
        private const long TicksPerSecond = TicksPerMillisecond * 1_000;
        private const long TicksPerMinute = TicksPerSecond * 60;
        private const long TicksPerHour = TicksPerMinute * 60;
        private const long TicksPerDay = TicksPerHour * 24;
        private const int DaysPerYear = 365;
        private const int DaysPer4Years = DaysPerYear * 4 + 1;       // 1461
        private const int DaysPer100Years = DaysPer4Years * 25 - 1;  // 36524
        private const int DaysPer400Years = DaysPer100Years * 4 + 1; // 146097
        private static readonly int[] DaysToMonth365 = {0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334, 365};
        private static readonly int[] DaysToMonth366 = {0, 31, 60, 91, 121, 152, 182, 213, 244, 274, 305, 335, 366};

        public static void Deconstruct(this TimeSpan timeSpan, out int days, out int hours, out int minutes, out int seconds, out int ticks)
        {
            var t = timeSpan.Ticks;
            days         = (int)(t / (TicksPerHour * 24));
            hours        = (int)((t / TicksPerHour) % 24);
            minutes      = (int)((t / TicksPerMinute) % 60);
            seconds      = (int)((t / TicksPerSecond) % 60);
            ticks = (int)(t % 10_000_000);
        }

        public static void Deconstruct(this DateTime date, out int year, out int month, out int day, out int hour, out int minute, out int second, out int millisecond)
        {
            (year, month, day) = date;

            var ticks = date.Ticks;
            hour = (int)((ticks / TicksPerHour) % 24);
            minute = (int)((ticks / TicksPerMinute) % 60);
            second = (int)((ticks / TicksPerSecond) % 60);
            millisecond = (int)((ticks / TicksPerMillisecond) % 1_000);
        }

        public static void Deconstruct(this DateTime date, out int year, out int month, out int day)
        {
            GetDatePart(date.Ticks, out year, out month, out day);
        }

        // Returns a given date part of this DateTime. This method is used
        // to compute the year, day-of-year, month, or day part.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void GetDatePart(long ticks, out int year, out int month, out int day) {
            // n = number of days since 1/1/0001
            var n = (int)(ticks / TicksPerDay);
            // y400 = number of whole 400-year periods since 1/1/0001
            var y400 = n / DaysPer400Years;
            // n = day number within 400-year period
            n -= y400 * DaysPer400Years;
            // y100 = number of whole 100-year periods within 400-year period
            var y100 = n / DaysPer100Years;
            // Last 100-year period has an extra day, so decrement result if 4
            if (y100 == 4) y100 = 3;
            // n = day number within 100-year period
            n -= y100 * DaysPer100Years;
            // y4 = number of whole 4-year periods within 100-year period
            var y4 = n / DaysPer4Years;
            // n = day number within 4-year period
            n -= y4 * DaysPer4Years;
            // y1 = number of whole years within 4-year period
            var y1 = n / DaysPerYear;
            // Last year has an extra day, so decrement result if 4
            if (y1 == 4) y1 = 3;
            // If year was requested, compute and return it
            year = y400 * 400 + y100 * 100 + y4 * 4 + y1 + 1;
            // n = day number within year
            n -= y1 * DaysPerYear;
            // Leap year calculation looks different from IsLeapYear since y1, y4,
            // and y100 are relative to year 1, not year 0
            var leapYear = y1 == 3 && (y4 != 24 || y100 == 3);
            var days = leapYear? DaysToMonth366: DaysToMonth365;
            // All months have less than 32 days, so n >> 5 is a good conservative
            // estimate for the month
            var m = n >> 5 + 1;
            // m = 1-based month number
            while (n >= days[m]) m++;
            // If month was requested, return it
            month =  m;
            // Return 1-based day-of-month
            day = n - days[m - 1] + 1;
        }
    }
}
