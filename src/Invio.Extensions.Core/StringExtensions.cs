using System;
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
        /// <returns></returns>
        public static String Quote(
            this String str,
            Char quoteCharacter = '"',
            Char escapeCharacter = '\\') {

            if (str == null) {
                throw new ArgumentNullException(nameof(str));
            }

            var sb = new StringBuilder(str.Length + 2);
            sb.Append(quoteCharacter);
            if (escapeCharacter != quoteCharacter) {
                str = str.Replace($"{escapeCharacter}", $"{escapeCharacter}{escapeCharacter}");
            }
            sb.Append(
                str.Replace($"{quoteCharacter}", $"{escapeCharacter}{quoteCharacter}")
            );
            sb.Append(quoteCharacter);
            return sb.ToString();
        }
    }
}