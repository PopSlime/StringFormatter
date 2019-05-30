using System.Runtime.CompilerServices;
using InlineIL;

namespace System.Text.Formatting
{
    internal class Date
    {
        private const char _standardFormatShort = 'd';
        private const string _standardFormat = "yyyy-MM-dd";

        public static unsafe void Format(StringBuffer formatter, DateTime dateTime, StringView format)
        {
            IL.DeclareLocals(false);

            const int tempCharsLength = 4;
            var tempChars = stackalloc char[tempCharsLength];

            if (IsStandardShortDateFormat(format))
            {
                var (year, month, day) = dateTime;
                AppendNumber(formatter, year, 4, tempChars, tempCharsLength);
                formatter.Append('-');
                AppendNumber(formatter, month, 2, tempChars, tempCharsLength);
                formatter.Append('-');
                AppendNumber(formatter, day, 2, tempChars, tempCharsLength);
            }
            else
            {
                var(year, month, day, hour, minute, second, millisecond) = dateTime;
                AppendNumber(formatter, year, 4, tempChars, tempCharsLength);
                formatter.Append('-');
                AppendNumber(formatter, month, 2, tempChars, tempCharsLength);
                formatter.Append('-');
                AppendNumber(formatter, day, 2, tempChars, tempCharsLength);
                formatter.Append(' ');
                AppendNumber(formatter, hour, 2, tempChars, tempCharsLength);
                formatter.Append(':');
                AppendNumber(formatter, minute, 2, tempChars, tempCharsLength);
                formatter.Append(':');
                AppendNumber(formatter, second, 2, tempChars, tempCharsLength);
                formatter.Append('.');
                AppendNumber(formatter, millisecond, 3, tempChars, tempCharsLength);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe bool IsStandardShortDateFormat(StringView format)
        {
            if (format.Length == 1 && format.Data[0] == _standardFormatShort)
                return true;

            return format == _standardFormat;
        }

        public static unsafe void Format(StringBuffer formatter, TimeSpan timeSpan, StringView format)
        {
            IL.DeclareLocals(false);

            const int tempCharsLength = 7;
            var tempChars = stackalloc char[tempCharsLength];

            var fmt = ParseTimeSpanFormat(format);

            if (timeSpan.Ticks < 0)
            {
                formatter.Append('-');
                timeSpan = new TimeSpan(-timeSpan.Ticks);
            }

            var (days, hours, minutes, seconds, ticks) = timeSpan;

            if (days > 0 || fmt == TimeSpanFormat.GeneralLong)
            {
                formatter.Append(days, StringView.Empty);
                formatter.Append(fmt == TimeSpanFormat.Constant ? '.' : ':');
            }

            if (fmt == TimeSpanFormat.GeneralShort)
                formatter.Append(hours, StringView.Empty);
            else
                AppendNumber(formatter, hours, 2, tempChars, tempCharsLength);

            formatter.Append(':');
            AppendNumber(formatter, minutes, 2, tempChars, tempCharsLength);
            formatter.Append(':');
            AppendNumber(formatter, seconds, 2, tempChars, tempCharsLength);

            if (ticks != 0 || fmt == TimeSpanFormat.GeneralLong)
            {
                formatter.Append('.');
                AppendNumber(formatter, ticks, 7, tempChars, tempCharsLength);

                if (fmt == TimeSpanFormat.GeneralShort)
                    formatter.TrimEnd('0');
            }
        }

        private static unsafe void AppendNumber(StringBuffer formatter, int value, int maxLength, char* tempChars, int tempCharsLength)
        {
            var startOffset = tempCharsLength - maxLength;
            for (var i = startOffset; i < tempCharsLength; i++)
                tempChars[i] = '0';

            Numeric.Int32ToDecChars(tempChars + tempCharsLength, (uint)value, 0);
            formatter.Append(tempChars + startOffset, maxLength);
        }

        private static unsafe TimeSpanFormat ParseTimeSpanFormat(StringView format)
        {
            if (format.Length != 1)
                return TimeSpanFormat.Constant;

            switch (format.Data[0])
            {
                case 'g':
                    return TimeSpanFormat.GeneralShort;

                case 'G':
                    return TimeSpanFormat.GeneralLong;

                default:
                    return TimeSpanFormat.Constant;
            }
        }

        private enum TimeSpanFormat
        {
            Constant,
            GeneralShort,
            GeneralLong
        }
    }
}
