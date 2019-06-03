using System.Runtime.InteropServices;

namespace System.Text.Formatting
{
    internal class GuidFormatting
    {
        public static unsafe void Format(StringBuffer formatter, Guid value, StringView format)
        {
            if (format.Length > 1)
                throw new FormatException(string.Format(SR.UnknownFormatSpecifier, format));

            var guidProxy = *(GuidProxy*)&value;
            var formatChar = format.IsEmpty ? 'D' : format.Data[0];
            var guidChars = stackalloc char[68];
            var offset = 0;

            switch (formatChar)
            {
                case 'D':
                case 'd':
                    offset = GuidProxyToChars(guidChars, offset, guidProxy, true, false);
                    break;
                case 'N':
                case 'n':
                    offset = GuidProxyToChars(guidChars, offset, guidProxy, false, false);
                    break;
                case 'B':
                case 'b':
                    guidChars[offset++] = '{';
                    offset = GuidProxyToChars(guidChars, offset, guidProxy, true, false);
                    guidChars[offset++] = '}';
                    break;
                case 'P':
                case 'p':
                    guidChars[offset++] = '(';
                    offset = GuidProxyToChars(guidChars, offset, guidProxy, true, false);
                    guidChars[offset++] = ')';
                    break;
                case 'X':
                case 'x':
                    guidChars[offset++] = '{';
                    offset = GuidProxyToChars(guidChars, offset, guidProxy, false, true);
                    guidChars[offset++] = '}';
                    break;
                default:
                    throw new FormatException(string.Format(SR.UnknownFormatSpecifier, format));
            }
            formatter.Append(guidChars, offset);
        }

        private static unsafe int GuidProxyToChars(char* chars, int offset, GuidProxy guidProxy, bool dash, bool hex)
        {
            if (hex)
            {
                // {0xdddddddd,0xdddd,0xdddd,{0xdd,0xdd,0xdd,0xdd,0xdd,0xdd,0xdd,0xdd}}
                chars[offset++] = '0';
                chars[offset++] = 'x';
                offset = HexsToChars(chars, offset, guidProxy.a >> 24, guidProxy.a >> 16);
                offset = HexsToChars(chars, offset, guidProxy.a >> 8, guidProxy.a);
                chars[offset++] = ',';
                chars[offset++] = '0';
                chars[offset++] = 'x';
                offset = HexsToChars(chars, offset, guidProxy.b >> 8, guidProxy.b);
                chars[offset++] = ',';
                chars[offset++] = '0';
                chars[offset++] = 'x';
                offset = HexsToChars(chars, offset, guidProxy.c >> 8, guidProxy.c);
                chars[offset++] = ',';
                chars[offset++] = '{';
                offset = HexsToChars(chars, offset, guidProxy.d, guidProxy.e, true);
                chars[offset++] = ',';
                offset = HexsToChars(chars, offset, guidProxy.f, guidProxy.g, true);
                chars[offset++] = ',';
                offset = HexsToChars(chars, offset, guidProxy.h, guidProxy.i, true);
                chars[offset++] = ',';
                offset = HexsToChars(chars, offset, guidProxy.j, guidProxy.k, true);
                chars[offset++] = '}';
            }
            else
            {
                // [{|(]dddddddd[-]dddd[-]dddd[-]dddd[-]dddddddddddd[}|)]
                offset = HexsToChars(chars, offset, guidProxy.a >> 24, guidProxy.a >> 16);
                offset = HexsToChars(chars, offset, guidProxy.a >> 8, guidProxy.a);
                if (dash) chars[offset++] = '-';
                offset = HexsToChars(chars, offset, guidProxy.b >> 8, guidProxy.b);
                if (dash) chars[offset++] = '-';
                offset = HexsToChars(chars, offset, guidProxy.c >> 8, guidProxy.c);
                if (dash) chars[offset++] = '-';
                offset = HexsToChars(chars, offset, guidProxy.d, guidProxy.e);
                if (dash) chars[offset++] = '-';
                offset = HexsToChars(chars, offset, guidProxy.f, guidProxy.g);
                offset = HexsToChars(chars, offset, guidProxy.h, guidProxy.i);
                offset = HexsToChars(chars, offset, guidProxy.j, guidProxy.k);
            }
            return offset;
        }

        private static char HexToChar(int a)
        {
            a &= 15;
            return a > 9 ? (char)(a - 10 + 97) : (char)(a + 48);
        }

        private static unsafe int HexsToChars(char* guidChars, int offset, int a, int b)
        {
            return HexsToChars(guidChars, offset, a, b, false);
        }

        private static unsafe int HexsToChars(char* guidChars, int offset, int a, int b, bool hex)
        {
            if (hex)
            {
                guidChars[offset++] = '0';
                guidChars[offset++] = 'x';
            }
            guidChars[offset++] = HexToChar(a >> 4);
            guidChars[offset++] = HexToChar(a);
            if (hex)
            {
                guidChars[offset++] = ',';
                guidChars[offset++] = '0';
                guidChars[offset++] = 'x';
            }
            guidChars[offset++] = HexToChar(b >> 4);
            guidChars[offset++] = HexToChar(b);
            return offset;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct GuidProxy
        {
            public int a;
            public short b;
            public short c;
            public byte d;
            public byte e;
            public byte f;
            public byte g;
            public byte h;
            public byte i;
            public byte j;
            public byte k;
        }
    }
}
