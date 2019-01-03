using System;
using System.Runtime.CompilerServices;
using System.Text.Formatting;
using NFluent;
using NUnit.Framework;

namespace Test
{
    [TestFixture]
    public class StringBufferTests
    {
        [Test]
        public void lowering_count_shortens_string()
        {
            var buffer = new StringBuffer(128);

            buffer.Append("XXXXXXXXXzzzzzzzzz");

            Check.That(buffer.ToString()).IsEqualTo("XXXXXXXXXzzzzzzzzz");

            buffer.Count = 9;

            Check.That(buffer.ToString()).IsEqualTo("XXXXXXXXX");

            buffer.Append('w', 9);

            Check.That(buffer.ToString()).IsEqualTo("XXXXXXXXXwwwwwwwww");
        }

        [Test]
        public void increasing_count_pads_with_nulls_and_increases_capacity()
        {
            var buffer = new StringBuffer(9);

            buffer.Append("XXXXXXXXX");

            Check.That(buffer.ToString()).IsEqualTo("XXXXXXXXX");

            buffer.Count = buffer.Count * 2;

            Check.That(buffer.Count).IsEqualTo(18);

            char[] dest = new char[18];

            buffer.CopyTo(0, dest, 0, 18);

            for (var i = 0; i < 9; i++)
                Check.That((int) dest[i]).IsEqualTo('X');

            for (var i = 9; i < 18; i++)
                Check.That((int) dest[i]).IsEqualTo(0);
        }

        [Test]
        public unsafe void copy_to_span()
        {
            var buffer = new StringBuffer();
            buffer.Append("some_string");

            Span<char> chars = stackalloc char[100];
            ref var r0 = ref chars.GetPinnableReference();

            buffer.CopyTo(chars, 0, buffer.Count);
#if NETCOREAPP2_1
            Check.That(new string(chars.Slice(0, buffer.Count))).IsEqualTo("some_string");
#else
            fixed (char* cp = &Unsafe.As<char, char>(ref r0))
                Check.That(new string(cp, 0, buffer.Count)).IsEqualTo("some_string");
#endif

            chars.Clear();
            buffer.CopyTo(chars, 5, buffer.Count - 5);

#if NETCOREAPP2_1
            Check.That(new string(chars.Slice(0, buffer.Count - 5))).IsEqualTo("string");
#else
            fixed (char* cp = &Unsafe.As<char, char>(ref r0))
                Check.That(new string(cp, 0, buffer.Count - 5)).IsEqualTo("string");
#endif

        }
    }
}
