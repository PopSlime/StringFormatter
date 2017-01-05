using System;
using System.Text.Formatting;
using NFluent;
using NUnit.Framework;

namespace Test
{
    [TestFixture]
    public class DateFormatTests
    {
        [Test]
        public void should_format_with_standard_date_format()
        {
            var buffer = new StringBuffer();
            buffer.AppendFormat("Date {0}", new DateTime(2017, 01, 15, 11, 01, 55, 843));
            
            Check.That(buffer.ToString()).IsEqualTo("Date 2017-01-15 11:01:55.843");
        }

        [Test]
        public void should_format_with_standard_timespan_format()
        {
            var buffer = new StringBuffer();
            buffer.AppendFormat("Timespan {0}", new TimeSpan(11, 01, 55));
            
            Check.That(buffer.ToString()).IsEqualTo("Timespan 11:01:55");
        }
    }
}
