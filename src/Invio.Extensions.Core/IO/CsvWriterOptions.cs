using System;

namespace Invio.Extensions.IO {
    /// <summary>
    /// Formatting options for <see cref="CsvWriter" />.
    /// </summary>
    /// <remarks>
    /// Once a <see cref="CsvWriterOptions" /> instance is passed to the <see cref="CsvWriter" />
    /// subsequent changes to the <see cref="CsvWriterOptions" /> will have no effect.
    /// </remarks>
    public sealed class CsvWriterOptions {
        /// <summary>
        /// The character to use to separate fields.
        /// </summary>
        public Char FieldSeparator { get; set; } = ',';

        /// <summary>
        /// The character to use when quoting fields.
        /// </summary>
        public Char QuoteCharacter { get; set; } = '"';

        /// <summary>
        /// The character to use at the beginning of escape sequences.
        /// </summary>
        public Char EscapeCharacter { get; set; } = '\\';

        /// <summary>
        /// When set to <c>true</c>, allows quoted fields to contain actual newline characters. The
        /// default behavior is to replace newline characters with an escape sequence consisting of
        /// the escape character and the 'n' character.
        /// </summary>
        public Boolean AllowQuotedNewline { get; set; } = false;

        /// <summary>
        /// When set to <c>true</c>, causes all fields to be quoted. The default behavior is to only
        /// quote fields that contain field delimiters, escape characters, or quote characters.
        /// </summary>
        public Boolean QuoteAllFields { get; set; } = false;

        internal Immutable ToImmutable() {
            return new Immutable(
                this.FieldSeparator,
                this.QuoteCharacter,
                this.EscapeCharacter,
                this.AllowQuotedNewline,
                this.QuoteAllFields
            );
        }

        internal class Immutable {
            public Char FieldSeparator { get; }
            public Char QuoteCharacter { get; }
            public Char EscapeCharacter { get; }
            public Boolean AllowQuotedNewline { get; }
            public Boolean QuoteAllFields { get; }

            internal Immutable(
                Char fieldSeparator,
                Char quoteCharacter,
                Char escapeCharacter,
                Boolean allowQuotedNewline,
                Boolean quoteAllFields) {

                this.FieldSeparator = fieldSeparator;
                this.QuoteCharacter = quoteCharacter;
                this.EscapeCharacter = escapeCharacter;
                this.AllowQuotedNewline = allowQuotedNewline;
                this.QuoteAllFields = quoteAllFields;
            }
        }
    }
}
