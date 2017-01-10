using System;
using System.Text;
using System.Text.Formatting;
using NFluent;
using NUnit.Framework;

namespace Test
{
    [TestFixture]
    public class AsciiStringTests
    {
        [Test]
        public unsafe void should_append_ascii_string()
        {
            var stringBuffer = new StringBuffer();
            var bytes = Encoding.ASCII.GetBytes("Hello");
            fixed (byte* b = bytes)
            {
                stringBuffer.Append(new AsciiString(b, 5));
            }

            Check.That(stringBuffer.ToString()).IsEqualTo("Hello");
        }
    }
}
