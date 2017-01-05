using System;
using System.Text.Formatting;
using NFluent;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Test
{
    [TestFixture]
    public class DateFormatTests
    {
        private readonly IFixture _fixture = new Fixture();

        public DateFormatTests()
        {
            _fixture.Customizations.Add(new RandomNumericSequenceGenerator(3600, 3600 * 24));
        }

        [Test]
        public void should_format_with_standard_date_format()
        {
            var buffer = new StringBuffer();
            buffer.AppendFormat("Date {0}", new DateTime(2017, 01, 15, 11, 01, 55, 843));
            
            Check.That(buffer.ToString()).IsEqualTo("Date 2017-01-15 11:01:55.843");
        }

        [Test]
        [Repeat(50)]
        public void should_multiple_times()
        {
            var buffer = new StringBuffer();
            var dateTime = _fixture.Create<DateTime>();
            buffer.AppendFormat("Date {0}", dateTime);

            Check.That(buffer.ToString()).IsEqualTo($"Date {dateTime:yyyy-MM-dd HH:mm:ss.fff}");
        }

        [Test]
        public void should_format_with_standard_timespan_format()
        {
            var buffer = new StringBuffer();
            buffer.AppendFormat("Timespan {0}", new TimeSpan(11, 01, 55));
            
            Check.That(buffer.ToString()).IsEqualTo("Timespan 11:01:55");
        }

        [Test]
        [Repeat(50)]
        public void should_format_many_with_standard_timespan_format()
        {
            var buffer = new StringBuffer();
            
            var timeSpan = TimeSpan.FromSeconds(_fixture.Create<int>());
            buffer.AppendFormat("Timespan {0}", timeSpan);

            Check.That(buffer.ToString()).IsEqualTo($"Timespan {timeSpan.ToString(@"hh\:mm\:ss")}");
        }
    }
}
