using System;
using System.Text.Formatting;
using NFluent;
using NUnit.Framework;

namespace Test
{
    [TestFixture]
    public class NumericTests
    {
        [Test]
        public void ShouldTestIntegers([Values("", "N0", "N2", "P0", "P2", "C0", "C2", "D", "D0", "D2", "G", "G0", "G2", "X", "E")]string format, [Random(int.MinValue, int.MaxValue, 20)]int integer)
        {
            var buffer = new StringBuffer();
            buffer.AppendFormat($"{{0:{format}}}", integer);
            Check.That(buffer.ToString()).IsEqualTo(integer.ToString(format));
        }
        [Test]
        public void ShouldTestUIntegers([Values("", "N0", "N2", "P0", "P2", "C0", "C2", "D", "D0", "D2", "G", "G0", "G2", "X", "E")]string format, [Random(0, int.MaxValue, 20)]int integer)
        {
            var unsigned = (uint) integer;
            var buffer = new StringBuffer();
            buffer.AppendFormat($"{{0:{format}}}", unsigned);
            Check.That(buffer.ToString()).IsEqualTo(unsigned.ToString(format));
        }

        [Test]
        public void ShouldTestShortNegativeHex([Values(-1)]short integer)
        {
            var buffer = new StringBuffer();
            buffer.AppendFormat("{0:X}", integer);
            Check.That(buffer.ToString()).IsEqualTo(integer.ToString("X"));
        }

        [Test]
        public void ShouldTestShorts([Values("", "N0", "N2", "P0", "P2", "C0", "C2", "D", "D0", "D2", "G", "G0", "G2", "X", "E")]string format, [Random(short.MinValue, short.MaxValue, 20)]short integer)
        {
            var buffer = new StringBuffer();
            buffer.AppendFormat($"{{0:{format}}}", integer);
            Check.That(buffer.ToString()).IsEqualTo(integer.ToString(format));
        }

        [Test]
        public void ShouldTestBytes([Values("", "N0", "N2", "P0", "P2", "C0", "C2", "D", "D0", "D2", "G", "G0", "G2", "X", "E")]string format, [Random(byte.MinValue, byte.MaxValue, 20)]byte integer)
        {
            var buffer = new StringBuffer();
            buffer.AppendFormat($"{{0:{format}}}", integer);
            Check.That(buffer.ToString()).IsEqualTo(integer.ToString(format));
        }

        [Test]
        public void ShouldTestSignedBytes([Values("", "N0", "N2", "P0", "P2", "C0", "C2", "D", "D0", "D2", "G", "G0", "G2", "X", "E")]string format, [Random(sbyte.MinValue, sbyte.MaxValue, 20)]sbyte integer)
        {
            var buffer = new StringBuffer();
            buffer.AppendFormat($"{{0:{format}}}", integer);
            Check.That(buffer.ToString()).IsEqualTo(integer.ToString(format));
        }

        [Test]
        public void ShouldTestLongs([Values("", "N0", "N2", "P0", "P2", "C0", "C2", "D", "D0", "D2", "G", "G0", "G2", "X", "E")]string format, [Random(int.MinValue, int.MaxValue, 20)]long integer)
        {
            var buffer = new StringBuffer();
            buffer.AppendFormat($"{{0:{format}}}", integer);
            Check.That(buffer.ToString()).IsEqualTo(integer.ToString(format));
        }

        [Test]
        public void ShouldTestSingles([Values("", "N0", "N2", "P0", "P2", "C0", "C2", "G", "G0", "G2", "E")]string format, [Random(float.MinValue, float.MaxValue, 20)]double floating)
        {
            float single = (float)floating;
            var buffer = new StringBuffer();
            buffer.AppendFormat($"{{0:{format}}}", single);
            Check.That(buffer.ToString()).IsEqualTo(single.ToString(format));
        }

        [Test]
        public void ShouldTestDoubles([Values("", "N0", "N2", "P0", "P2", "C0", "C2", "G", "G0", "G2", "E")]string format, [Random(double.MinValue, double.MaxValue, 20)]double floating)
        {
            var buffer = new StringBuffer();
            buffer.AppendFormat($"{{0:{format}}}", floating);
            Check.That(buffer.ToString()).IsEqualTo(floating.ToString(format));
        }

        [Test]
        public void ShouldTestDecimals([Values("", "N0", "N2", "P0", "P2", "C0", "C2", "G", "G0", "G2", "E")]string format, [Random(-79228162514264337593543950335d, 79228162514264337593543950335d, 20)]decimal floating)
        {
            var buffer = new StringBuffer();
            buffer.AppendFormat($"{{0:{format}}}", floating);
            Check.That(buffer.ToString()).IsEqualTo(floating.ToString(format));
        }
    }
}
