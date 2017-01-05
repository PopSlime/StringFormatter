using System.Runtime.InteropServices;
using System.Security;

namespace System.Text.Formatting
{
    internal class GuidFormatting
    {
        public static unsafe void Format(StringBuffer formatter, Guid value, StringView format)
        {
            var guidProxy = *((GuidProxy*)(&value));
            char* guidChars = stackalloc char[36];

            // [{|(]dddddddd[-]dddd[-]dddd[-]dddd[-]dddddddddddd[}|)]
            int offset = 0;
            offset = HexsToChars(guidChars, offset, guidProxy.a >> 24, guidProxy.a >> 16);
            offset = HexsToChars(guidChars, offset, guidProxy.a >> 8, guidProxy.a);
            guidChars[offset++] = '-';
            offset = HexsToChars(guidChars, offset, guidProxy.b >> 8, guidProxy.b);
            guidChars[offset++] = '-';
            offset = HexsToChars(guidChars, offset, guidProxy.c >> 8, guidProxy.c);
            guidChars[offset++] = '-';
            offset = HexsToChars(guidChars, offset, guidProxy.d, guidProxy.e);
            guidChars[offset++] = '-';
            offset = HexsToChars(guidChars, offset, guidProxy.f, guidProxy.g);
            offset = HexsToChars(guidChars, offset, guidProxy.h, guidProxy.i);
            offset = HexsToChars(guidChars, offset, guidProxy.j, guidProxy.k);

            formatter.Append(guidChars, 36);
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