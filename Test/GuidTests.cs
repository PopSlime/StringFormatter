using System;
using System.Text.Formatting;
using NFluent;
using NUnit.Framework;

namespace Test
{
    [TestFixture]
    public class GuidTests
    {
        [Test]
        public void should_format_guid()
        {
            var buffer = new StringBuffer(128);

            buffer.AppendFormat("Guid {0}", Guid.Parse("de7bc307-699f-47a2-a0da-79c504edd68f"));

            Check.That(buffer.ToString()).IsEqualTo("Guid de7bc307-699f-47a2-a0da-79c504edd68f");
        }

        [TestCase("D")]
        [TestCase("d")]
        [TestCase("N")]
        [TestCase("n")]
        [TestCase("B")]
        [TestCase("b")]
        [TestCase("P")]
        [TestCase("p")]
        [TestCase("X")]
        [TestCase("x")]
        public void should_format_guid_with_specifier(string format)
        {
            var guid = Guid.Parse("de7bc307-699f-47a2-a0da-79c504edd68f");
            var buffer = new StringBuffer(128);

            buffer.AppendFormat($"{{0:{format}}}", guid);

            Check.That(buffer.ToString()).IsEqualTo(guid.ToString(format));
        }

        [Test]
        public void should_throw_if_specifier_is_unrecognized()
        {
            var buffer = new StringBuffer(128);

            Check.ThatCode(() => buffer.AppendFormat("{0:q}", Guid.NewGuid()))
                 .Throws<FormatException>();
        }

        [Test]
        [Repeat(50)]
        public void should_format_many_guids()
        {
            var buffer = new StringBuffer();

            var newGuid = Guid.NewGuid();
            buffer.AppendFormat("Guid {0}", newGuid);

            Check.That(buffer.ToString()).IsEqualTo($"Guid {newGuid}");
        }
    }
}
