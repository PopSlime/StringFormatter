namespace System.Text.Formatting {
    // TODO: clean this up
#pragma warning disable 660,661
    public unsafe struct StringView {
#pragma warning restore 660,661
        public static readonly StringView Empty = new StringView();

        public readonly char* Data;
        public readonly int Length;

        public bool IsEmpty {
            get { return Length == 0; }
        }

        public StringView (char* data, int length) {
            Data = data;
            Length = length;
        }

        public static bool operator ==(StringView lhs, string rhs) {
            var count = lhs.Length;
            if (count != rhs.Length)
                return false;

            fixed (char* r = rhs)
            {
                var lhsPtr = lhs.Data;
                var rhsPtr = r;
                for (int i = 0; i < count; i++) {
                    if (*lhsPtr++ != *rhsPtr++)
                        return false;
                }
            }

            return true;
        }

        public static bool operator !=(StringView lhs, string rhs) {
            return !(lhs == rhs);
        }

        public override string ToString()
        {
            return new string(Data, 0, Length);
        }
    }
}
