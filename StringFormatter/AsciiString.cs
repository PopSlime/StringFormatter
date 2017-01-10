namespace System.Text.Formatting
{
    /// <summary>
    /// A byte array (as a pointer) that contains only ascii values
    /// </summary>
    public unsafe struct AsciiString
    {
        private readonly byte* _bytes;
        private readonly int _length;

        /// <summary>
        /// Create an ascii string
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="length"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public AsciiString(byte * bytes, int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            _bytes = bytes;
            _length = length;
        }

        /// <summary>
        /// The length of the string
        /// </summary>
        public int Length => _length;

        /// <summary>
        /// Copy the ascii string to the given char pointer
        /// </summary>
        /// <param name="chars"></param>
        public void CopyTo(char* chars)
        {
            for (var i = 0; i < _length; i++)
            {
                chars[i] = (char)_bytes[i];
            }
        }
    }
}