using System.Globalization;

namespace System.Text.Formatting
{
    public abstract unsafe partial class BaseStringBuffer
    {
        protected static readonly CachedCulture CachedInvariantCulture = new CachedCulture(CultureInfo.InvariantCulture);
        protected static readonly CachedCulture CachedCurrentCulture = new CachedCulture(CultureInfo.CurrentCulture);

        protected CachedCulture culture;

        protected BaseStringBuffer()
        {
            culture = CachedCurrentCulture;
        }


        /// <summary>
        /// The culture used to format string data.
        /// </summary>
        public CultureInfo Culture
        {
            get { return culture.Culture; }
            set
            {
                if (culture.Culture == value)
                    return;

                if (value == CultureInfo.InvariantCulture)
                    culture = CachedInvariantCulture;
                else if (value == CachedCurrentCulture.Culture)
                    culture = CachedCurrentCulture;
                else
                    culture = new CachedCulture(value);
            }
        }

        public abstract void Append(string value);

        public abstract void Append(char* str, int count);

        /// <summary>
        /// Appends the specified value as a string to the current buffer.
        /// </summary>
        /// <param name="value">The value to append.</param>
        /// <param name="format">A format specifier indicating how to convert <paramref name="value"/> to a string.</param>
        public void Append(sbyte value, StringView format)
        {
            Numeric.FormatSByte(this, value, format, culture);
        }

        /// <summary>
        /// Appends the specified value as a string to the current buffer.
        /// </summary>
        /// <param name="value">The value to append.</param>
        /// <param name="format">A format specifier indicating how to convert <paramref name="value"/> to a string.</param>
        public void Append(byte value, StringView format)
        {
            // widening here is fine
            Numeric.FormatUInt32(this, value, format, culture);
        }

        /// <summary>
        /// Appends the specified value as a string to the current buffer.
        /// </summary>
        /// <param name="value">The value to append.</param>
        /// <param name="format">A format specifier indicating how to convert <paramref name="value"/> to a string.</param>
        public void Append(short value, StringView format)
        {
            Numeric.FormatInt16(this, value, format, culture);
        }

        /// <summary>
        /// Appends the specified value as a string to the current buffer.
        /// </summary>
        /// <param name="value">The value to append.</param>
        /// <param name="format">A format specifier indicating how to convert <paramref name="value"/> to a string.</param>
        public void Append(ushort value, StringView format)
        {
            // widening here is fine
            Numeric.FormatUInt32(this, value, format, culture);
        }

        /// <summary>
        /// Appends the specified value as a string to the current buffer.
        /// </summary>
        /// <param name="value">The value to append.</param>
        /// <param name="format">A format specifier indicating how to convert <paramref name="value"/> to a string.</param>
        public void Append(int value, StringView format)
        {
            Numeric.FormatInt32(this, value, format, culture);
        }

        /// <summary>
        /// Appends the specified value as a string to the current buffer.
        /// </summary>
        /// <param name="value">The value to append.</param>
        /// <param name="format">A format specifier indicating how to convert <paramref name="value"/> to a string.</param>
        public void Append(uint value, StringView format)
        {
            Numeric.FormatUInt32(this, value, format, culture);
        }

        /// <summary>
        /// Appends the specified value as a string to the current buffer.
        /// </summary>
        /// <param name="value">The value to append.</param>
        /// <param name="format">A format specifier indicating how to convert <paramref name="value"/> to a string.</param>
        public void Append(long value, StringView format)
        {
            Numeric.FormatInt64(this, value, format, culture);
        }

        /// <summary>
        /// Appends the specified value as a string to the current buffer.
        /// </summary>
        /// <param name="value">The value to append.</param>
        /// <param name="format">A format specifier indicating how to convert <paramref name="value"/> to a string.</param>
        public void Append(ulong value, StringView format)
        {
            Numeric.FormatUInt64(this, value, format, culture);
        }

        /// <summary>
        /// Appends the specified value as a string to the current buffer.
        /// </summary>
        /// <param name="value">The value to append.</param>
        /// <param name="format">A format specifier indicating how to convert <paramref name="value"/> to a string.</param>
        public void Append(float value, StringView format)
        {
            Numeric.FormatSingle(this, value, format, culture);
        }

        /// <summary>
        /// Appends the specified value as a string to the current buffer.
        /// </summary>
        /// <param name="value">The value to append.</param>
        /// <param name="format">A format specifier indicating how to convert <paramref name="value"/> to a string.</param>
        public void Append(double value, StringView format)
        {
            Numeric.FormatDouble(this, value, format, culture);
        }

        /// <summary>
        /// Appends the specified value as a string to the current buffer.
        /// </summary>
        /// <param name="value">The value to append.</param>
        /// <param name="format">A format specifier indicating how to convert <paramref name="value"/> to a string.</param>
        public void Append(decimal value, StringView format)
        {
            Numeric.FormatDecimal(this, (uint*)&value, format, culture);
        }
    }
}