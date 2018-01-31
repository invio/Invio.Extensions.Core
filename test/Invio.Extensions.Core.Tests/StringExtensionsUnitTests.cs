using System;
using Xunit;

namespace Invio.Extensions.Core.Tests {
    public class StringExtensionsUnitTests {
        [Theory]
        [InlineData("", '\'', '\\', "''")]
        [InlineData("foo", '\'', '\\', "'foo'")]
        [InlineData("foo's bar", '\'', '\\', @"'foo\'s bar'")]
        [InlineData(@"\foo's \bar\", '\'', '\\', @"'\\foo\'s \\bar\\'")]
        [InlineData("foo's bar", '\'', '`', @"'foo`'s bar'")]
        [InlineData(@"`foo's `bar`", '\'', '`', @"'``foo`'s ``bar``'")]
        [InlineData("foo", '\'', '\'', "'foo'")]
        [InlineData("foo's bar", '\'', '\'', @"'foo''s bar'")]
        public void Quote_AlternateQuotesAndEscapes(
            String original,
            Char quoteChar,
            Char escapeChar,
            String expected) {

            var quoted = original.Quote(quoteChar, escapeChar);

            Assert.Equal(expected, quoted);
        }

        [Theory]
        [InlineData("", "\"\"")]
        [InlineData("foo", "\"foo\"")]
        [InlineData("\"foo\"", "\"\\\"foo\\\"\"")]
        [InlineData(@"\foo\bar\", @"""\\foo\\bar\\""")]
        public void Quote_Defaults(String original, String expected) {
            var quoted = original.Quote();

            Assert.Equal(expected, quoted);
        }

        [Fact]
        public void Quote_ArgumentNull() {
            var exception = Assert.Throws<ArgumentNullException>(() => ((String)null).Quote());

            Assert.Equal("str", exception.ParamName);
        }
    }
}