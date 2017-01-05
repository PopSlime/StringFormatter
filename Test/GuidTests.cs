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
            var buffer = new StringBuffer();

            buffer.AppendFormat("Guid {0}", Guid.Parse("de7bc307-699f-47a2-a0da-79c504edd68f"));

            Check.That(buffer.ToString()).IsEqualTo("Guid de7bc307-699f-47a2-a0da-79c504edd68f");
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
