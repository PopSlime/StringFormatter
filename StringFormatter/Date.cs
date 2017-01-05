namespace System.Text.Formatting
{
    internal class Date
    {
        public static unsafe void Format(StringBuffer formatter, DateTime dateTime, StringView format)
        {
            var tempCharsLength = 4;
            char* tempChars = stackalloc char[tempCharsLength];

            AppendNumber(formatter, dateTime.Year, 4, tempChars, tempCharsLength);
            formatter.Append('-');
            AppendNumber(formatter, dateTime.Month, 2, tempChars, tempCharsLength);
            formatter.Append('-');
            AppendNumber(formatter, dateTime.Day, 2, tempChars, tempCharsLength);
            formatter.Append(' ');
            AppendNumber(formatter, dateTime.Hour, 2, tempChars, tempCharsLength);
            formatter.Append(':');
            AppendNumber(formatter, dateTime.Minute, 2, tempChars, tempCharsLength);
            formatter.Append(':');
            AppendNumber(formatter, dateTime.Second, 2, tempChars, tempCharsLength);
            formatter.Append('.');
            AppendNumber(formatter, dateTime.Millisecond, 3, tempChars, tempCharsLength);
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