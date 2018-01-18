namespace System.Text.Formatting
{
    internal class Date
    {
        private static readonly string _standardFormat = "yyyy-MM-dd";

        public static unsafe void Format(StringBuffer formatter, DateTime dateTime, StringView format)
        {
            var tempCharsLength = 4;
            char* tempChars = stackalloc char[tempCharsLength];

            if (IsStandardShortFormat(format))
            {
                var(year, month, day) = dateTime;
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

        private static unsafe bool IsStandardShortFormat(StringView format)
        {
            if (_standardFormat.Length != format.Length)
                return false;

            fixed (char* standardFormat = _standardFormat)
            {
                for (var i = 0; i < format.Length; i++)
                {
                    if (*(format.Data + i) != *(standardFormat + i))
                        return false;
                }
            }

            return true;
        }

        public static unsafe void Format(StringBuffer formatter, TimeSpan timeSpan, StringView format)
        {
            var tempCharsLength = 3;
            char* tempChars = stackalloc char[tempCharsLength];

            AppendNumber(formatter, timeSpan.Hours, 2, tempChars, tempCharsLength);
            formatter.Append(':');
            AppendNumber(formatter, timeSpan.Minutes, 2, tempChars, tempCharsLength);
            formatter.Append(':');
            AppendNumber(formatter, timeSpan.Seconds, 2, tempChars, tempCharsLength);
            formatter.Append('.');
            AppendNumber(formatter, timeSpan.Milliseconds, 3, tempChars, tempCharsLength);
        }

        private static unsafe void AppendNumber(StringBuffer formatter, int value, int maxLength, char* tempChars, int tempCharsLength)
        {
            var startOffset = tempCharsLength - maxLength;
            for (var i = startOffset; i < tempCharsLength; i++)
            {
                *(tempChars + i) = '0';
            }

            Numeric.Int32ToDecChars(tempChars + tempCharsLength, (uint)value, 0);
            formatter.Append(tempChars + startOffset, maxLength);
        }
    }
}
