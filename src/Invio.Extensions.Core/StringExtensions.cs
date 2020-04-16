using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Invio.Extensions {
    /// <summary>
    /// Extension methods on the <see cref="String" /> type.
    /// </summary>
    public static class StringExtensions {
        /// <summary>
        /// Wrap a string in the specified <paramref name="quoteCharacter" />, escaping an existing
        /// instances of that character using the specified <paramref name="escapeCharacter" />.
        /// </summary>
        /// <param name="str">
        /// The string to wrap in quotes.
        /// </param>
        /// <param name="quoteCharacter">
        /// The quote character to wrap the string with.
        /// </param>
        /// <param name="escapeCharacter">
        /// The character to use when escaping existing quote characters.
        /// </param>
        /// <param name="specialCharacters">
        /// Additional special characters to escape besides the quote character.
        /// </param>
        /// <param name="escapeSequences">
        /// Escape sequence characters to use in place of the specified special characters. Note:
        /// this array must have the same length as <paramref name="specialCharacters" />
        /// </param>
        /// <returns></returns>
        public static String Quote(
            this String str,
            Char quoteCharacter = '"',
            Char escapeCharacter = '\\',
            Char[] specialCharacters = null,
            String[] escapeSequences = null) {
            if (str == null) {
                throw new ArgumentNullException(nameof(str));
            }

            if (specialCharacters != null) {
                if (escapeSequences == null) {
                    throw new ArgumentNullException(nameof(escapeSequences));
                }
            }

            var sb = new StringBuilder(str.Length + 2);
            sb.Append(quoteCharacter);
            if (escapeCharacter != quoteCharacter) {
                if (specialCharacters != null) {
                    EscapeToBuilder(
                        sb,
                        str,
                        escapeCharacter,
                        specialCharacters.Append(quoteCharacter).ToArray(),
                        escapeSequences.Append(quoteCharacter.ToString()).ToArray()
                    );
                } else {
                    EscapeToBuilder(
                        sb,
                        str,
                        escapeCharacter,
                        new[] { quoteCharacter },
                        new[] { quoteCharacter.ToString() }
                    );
                }
            } else {
                // Using a quote character as an escape character means only quotes can be escaped.
                sb.Append(
                    str.Replace($"{quoteCharacter}", $"{quoteCharacter}{quoteCharacter}")
                );
            }

            sb.Append(quoteCharacter);
            return sb.ToString();
        }

        /// <summary>
        /// Escapes special characters in a string by replacing them with an escape sequence.
        /// </summary>
        /// <param name="str">The string to escape.</param>
        /// <param name="escape">The character that starts each escape sequence.</param>
        /// <param name="specialCharacters">
        /// A list of special characters that need to be escaped. Note: the escape character itself
        /// will be implicitly added to this list.
        /// </param>
        /// <param name="escapeSequences">
        /// A list of characters that should be used in place of the listed special characters,
        /// preceded by the escape character.
        /// </param>
        /// <returns>
        /// An escaped copy of the specified string.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If any of the parameters are null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="specialCharacters" /> array is not the same length as the
        /// <paramref name="escapeSequences" /> array.
        /// </exception>
        public static String Escape(
            this String str,
            Char escape,
            Char[] specialCharacters,
            String[] escapeSequences) {

            if (str == null) {
                throw new ArgumentNullException(nameof(str));
            }
            if (specialCharacters == null) {
                throw new ArgumentNullException(nameof(specialCharacters));
            }
            if (escapeSequences == null) {
                throw new ArgumentNullException(nameof(escapeSequences));
            }

            if (escapeSequences.Length != specialCharacters.Length) {
                throw new ArgumentException(
                    "The list of escape sequences must be the same length as the list of special characters.",
                    nameof(escapeSequences)
                );
            }

            var sb = new StringBuilder(str.Length);

            EscapeToBuilder(sb, str, escape, specialCharacters, escapeSequences);

            return sb.ToString();
        }

        private static void EscapeToBuilder(
            StringBuilder sb,
            String str,
            Char escape,
            Char[] specialCharacters,
            IList<String> escapeSequences) {
            int start = 0, pos = 0;
            for (; pos < str.Length; pos++) {
                var escapeIx = Array.IndexOf(specialCharacters, str[pos]);
                if (escapeIx >= 0) {
                    if (pos - start > 0) {
                        sb.Append(str.Substring(start, pos - start));
                    }

                    start = pos + 1;
                    sb.Append($"{escape}{escapeSequences[escapeIx]}");
                } else if (str[pos] == escape) {
                    if (pos - start > 0) {
                        sb.Append(str.Substring(start, pos - start));
                    }

                    start = pos + 1;
                    sb.Append($"{escape}{escape}");
                }
            }

            if (pos - start > 0) {
                sb.Append(str.Substring(start, pos - start));
            }
        }
    }
}
