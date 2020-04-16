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

        [Theory]
        [InlineData("", "")]
        [InlineData("plain", "plain")]
        [InlineData("\"quoted\"", @"\""quoted\""")]
        [InlineData("foo\nbar", @"foo\nbar")]
        [InlineData("foo\0bar", @"foo\0bar")]
        [InlineData("foo\bbar", @"foo\BELLbar")]
        [InlineData("\\foo", @"\\foo")]
        public void Escape_Basic(String original, String expected) {
            var escaped =
                original.Escape(
                    '\\',
                    new[] { '\r', '\n', '\0', '\b', '"' },
                    new[] { "r", "n", "0", "BELL", "\"" }
                );

            Assert.Equal(expected, escaped);
        }

        [Fact]
        public void Escape_EscapesByDefault() {
            Assert.Equal(@"foo\\bar", @"foo\bar".Escape('\\', new Char[0], new String[0]));
        }

        [Theory]
        [InlineData("")]
        [InlineData("plain")]
        [InlineData("\"quoted\"")]
        [InlineData("foo\nbar")]
        public void Escape_NoOps(String original) {
            Assert.Equal(original, original.Escape('\\', new Char[0], new String[0]));
        }

        [Fact]
        public void Escape_NullString() {
            var ex = Assert.Throws<ArgumentNullException>(
                () => StringExtensions.Escape(null, '\\', new Char[0], new String[0])
            );

            Assert.Equal("str", ex.ParamName);
        }

        [Fact]
        public void Escape_NullSpecialCharacters() {
            var ex = Assert.Throws<ArgumentNullException>(
                () => "foo".Escape('\\', null, new String[0])
            );

            Assert.Equal("specialCharacters", ex.ParamName);
        }

        [Fact]
        public void Escape_NullEscapeSequences() {
            var ex = Assert.Throws<ArgumentNullException>(
                () => "foo".Escape('\\', new Char[0], null)
            );

            Assert.Equal("escapeSequences", ex.ParamName);
        }

        [Fact]
        public void Escape_ArrayLengthMismatch() {
            var ex = Assert.Throws<ArgumentException>(
                () => "foo".Escape('\\', new Char[0], new[] { "derp" })
            );

            Assert.Equal("escapeSequences", ex.ParamName);
        }
    }
}
