using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Invio.Extensions.Collections {
    /// <summary>
    /// Extension methods on the <see cref="IList{T}" /> type.
    /// </summary>
    public static class IListExtensions {

        /// <summary>
        /// By ensuring each instance of <see cref="Random" /> used is on its own thread,
        /// giving the extension methods that use it thread safety.
        /// </summary>
        private static ThreadLocal<Random> random { get; } =
            new ThreadLocal<Random>(() => new Random());

        /// <summary>
        /// Deconstructs a list into a <see cref="Tuple{T,IEnumerable}" /> containing the first
        /// element and a <see cref="IEnumerable{T}" /> containing any remaining elements.
        /// </summary>
        /// <param name="source">
        /// The list to deconstruct.</param>
        /// <param name="v1">
        /// The first value in the element in <paramref name="source" />.
        /// </param>
        /// <param name="rest">
        /// The remaining elements in <paramref name="source"/>.
        /// </param>
        /// <typeparam name="T">The type contained by the list.</typeparam>
        /// <exception cref="ArgumentNullException">The source list was null.</exception>
        /// <exception cref="IndexOutOfRangeException">
        /// The source list did not contain sufficient elements to deconstruct the list.
        /// </exception>
        public static void Deconstruct<T>(this IList<T> source, out T v1, out IEnumerable<T> rest) {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            v1 = source[0];
            rest = source.Skip(1);
        }

        /// <summary>
        /// Deconstructs a list into a <see cref="Tuple{T,T,IEnumerable}" /> containing the first
        /// two elements and a <see cref="IEnumerable{T}" /> containing any remaining
        /// elements.
        /// </summary>
        /// <param name="source">
        /// The list to deconstruct.</param>
        /// <param name="v1">
        /// The first value in the element in <paramref name="source" />.
        /// </param>
        /// <param name="v2">
        /// The second value in the element in <paramref name="source" />.
        /// </param>
        /// <param name="rest">
        /// The remaining elements in <paramref name="source"/>.
        /// </param>
        /// <typeparam name="T">The type contained by the list.</typeparam>
        /// <exception cref="ArgumentNullException">The source list was null.</exception>
        /// <exception cref="IndexOutOfRangeException">
        /// The source list did not contain sufficient elements to deconstruct the list.
        /// </exception>
        public static void Deconstruct<T>(
            this IList<T> source,
            out T v1,
            out T v2,
            out IEnumerable<T> rest) {

            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            v1 = source[0];
            v2 = source[1];
            rest = source.Skip(2);
        }

        /// <summary>
        /// Deconstructs a list into a <see cref="Tuple{T,T,T,IEnumerable}" /> containing the
        /// first three elements and a <see cref="IEnumerable{T}" /> containing any remaining
        /// elements.
        /// </summary>
        /// <param name="source">
        /// The list to deconstruct.</param>
        /// <param name="v1">
        /// The first value in the element in <paramref name="source" />.
        /// </param>
        /// <param name="v2">
        /// The second value in the element in <paramref name="source" />.
        /// </param>
        /// <param name="v3">
        /// The third value in the element in <paramref name="source" />.
        /// </param>
        /// <param name="rest">
        /// The remaining elements in <paramref name="source"/>.
        /// </param>
        /// <typeparam name="T">The type contained by the list.</typeparam>
        /// <exception cref="ArgumentNullException">The source list was null.</exception>
        /// <exception cref="IndexOutOfRangeException">
        /// The source list did not contain sufficient elements to deconstruct the list.
        /// </exception>
        public static void Deconstruct<T>(
            this IList<T> source,
            out T v1,
            out T v2,
            out T v3,
            out IEnumerable<T> rest) {

            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            v1 = source[0];
            v2 = source[1];
            v3 = source[2];
            rest = source.Skip(3);
        }

        /// <summary>
        /// Deconstructs a list into a <see cref="Tuple{T,T,T,T,IEnumerable}" /> containing the
        /// first four elements and a <see cref="IEnumerable{T}" /> containing any remaining
        /// elements.
        /// </summary>
        /// <param name="source">
        /// The list to deconstruct.</param>
        /// <param name="v1">
        /// The first value in the element in <paramref name="source" />.
        /// </param>
        /// <param name="v2">
        /// The second value in the element in <paramref name="source" />.
        /// </param>
        /// <param name="v3">
        /// The third value in the element in <paramref name="source" />.
        /// </param>
        /// <param name="v4">
        /// The fourth value in the element in <paramref name="source" />.
        /// </param>
        /// <param name="rest">
        /// The remaining elements in <paramref name="source" />.
        /// </param>
        /// <typeparam name="T">The type contained by the list.</typeparam>
        /// <exception cref="ArgumentNullException">The source list was null.</exception>
        /// <exception cref="IndexOutOfRangeException">
        /// The source list did not contain sufficient elements to deconstruct the list.
        /// </exception>
        public static void Deconstruct<T>(
            this IList<T> source,
            out T v1,
            out T v2,
            out T v3,
            out T v4,
            out IEnumerable<T> rest) {

            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            v1 = source[0];
            v2 = source[1];
            v3 = source[2];
            v4 = source[3];
            rest = source.Skip(4);
        }

        /// <summary>
        /// Deconstructs a list into a <see cref="Tuple{T,T,T,T,T,IEnumerable}" /> containing the
        /// first five elements and a <see cref="IEnumerable{T}" /> containing any remaining
        /// elements.
        /// </summary>
        /// <param name="source">
        /// The list to deconstruct.</param>
        /// <param name="v1">
        /// The first value in the element in <paramref name="source" />.
        /// </param>
        /// <param name="v2">
        /// The second value in the element in <paramref name="source" />.
        /// </param>
        /// <param name="v3">
        /// The third value in the element in <paramref name="source" />.
        /// </param>
        /// <param name="v4">
        /// The fourth value in the element in <paramref name="source" />.
        /// </param>
        /// <param name="v5">
        /// The fifth value in the element in <paramref name="source" />.
        /// </param>
        /// <param name="rest">
        /// The remaining elements in <paramref name="source" />.
        /// </param>
        /// <typeparam name="T">The type contained by the list.</typeparam>
        /// <exception cref="ArgumentNullException">The source list was null.</exception>
        /// <exception cref="IndexOutOfRangeException">
        /// The source list did not contain sufficient elements to deconstruct the list.
        /// </exception>
        public static void Deconstruct<T>(
            this IList<T> source,
            out T v1,
            out T v2,
            out T v3,
            out T v4,
            out T v5,
            out IEnumerable<T> rest) {

            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            v1 = source[0];
            v2 = source[1];
            v3 = source[2];
            v4 = source[3];
            v5 = source[4];
            rest = source.Skip(5);
        }

        /// <summary>
        /// Shuffles a list in place by iterating through each item and randomly
        /// picking a target item to swap places with.
        /// </summary>
        /// <remarks>
        /// This implementation does not guarantee that any items will actually
        /// change their current locations in the list.
        /// </remarks>
        /// <param name="source">
        /// The list to shuffle.</param>
        /// <typeparam name="T">The type contained by the list.</typeparam>
        /// <exception cref="ArgumentNullException">The source list was null.</exception>
        public static void Shuffle<T>(this IList<T> source) {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            for (var sourceIndex = 0; sourceIndex < source.Count; sourceIndex++) {
                var targetIndex = random.Value.Next(0, source.Count);

                if (sourceIndex != targetIndex) {
                    var tmp = source[sourceIndex];
                    source[sourceIndex] = source[targetIndex];
                    source[targetIndex] = tmp;
                }
            }
        }

    }

}
