using System;
using System.Collections.Generic;
using Xunit;
using Invio.Extensions.Collections;

namespace Invio.Extensions.Core.Tests.Collections {
    public class IListExtensionsUnitTests {
        [Theory]
        [InlineData(new[] { 0 }, 0, new Int32[0])]
        [InlineData(new[] { 0, 1 }, 0, new[] { 1 })]
        [InlineData(new[] { 0, 1, 2 }, 0, new[] { 1, 2 })]
        public void Destructure_One(IList<Int32> source, Int32 first, IEnumerable<Int32> rest) {
            var (car, cdr) = source;

            Assert.Equal(first, car);
            Assert.Equal(rest, cdr);
        }

        [Theory]
        [InlineData(new[] { 0, 1 }, 0, 1, new Int32[0])]
        [InlineData(new[] { 0, 1, 2 }, 0, 1, new[] { 2 })]
        [InlineData(new[] { 0, 1, 2, 3 }, 0, 1, new[] { 2, 3 })]
        public void Destructure_Two(
            IList<Int32> source,
            Int32 first,
            Int32 second,
            IEnumerable<Int32> rest) {

            var (n1, n2, cdr) = source;

            Assert.Equal(first, n1);
            Assert.Equal(second, n2);
            Assert.Equal(rest, cdr);
        }

        [Theory]
        [InlineData(new[] { 0, 1, 2 }, 0, 1, 2, new Int32[0])]
        [InlineData(new[] { 0, 1, 2, 3 }, 0, 1, 2, new[] { 3 })]
        [InlineData(new[] { 0, 1, 2, 3, 4  }, 0, 1, 2, new[] { 3, 4 })]
        public void Destructure_Three(
            IList<Int32> source,
            Int32 first,
            Int32 second,
            Int32 third,
            IEnumerable<Int32> rest) {

            var (n1, n2, n3, cdr) = source;

            Assert.Equal(first, n1);
            Assert.Equal(second, n2);
            Assert.Equal(third, n3);
            Assert.Equal(rest, cdr);
        }

        [Theory]
        [InlineData(new[] { 0, 1, 2, 3 }, 0, 1, 2, 3, new Int32[0])]
        [InlineData(new[] { 0, 1, 2, 3, 4 }, 0, 1, 2, 3, new[] { 4 })]
        [InlineData(new[] { 0, 1, 2, 3, 4, 5 }, 0, 1, 2, 3, new[] { 4, 5 })]
        public void Destructure_Four(
            IList<Int32> source,
            Int32 first,
            Int32 second,
            Int32 third,
            Int32 fourth,
            IEnumerable<Int32> rest) {

            var (n1, n2, n3, n4, cdr) = source;

            Assert.Equal(first, n1);
            Assert.Equal(second, n2);
            Assert.Equal(third, n3);
            Assert.Equal(fourth, n4);
            Assert.Equal(rest, cdr);
        }

        [Theory]
        [InlineData(new[] { 0, 1, 2, 3, 4 }, 0, 1, 2, 3, 4, new Int32[0])]
        [InlineData(new[] { 0, 1, 2, 3, 4, 5 }, 0, 1, 2, 3, 4, new[] { 5 })]
        [InlineData(new[] { 0, 1, 2, 3, 4, 5, 6 }, 0, 1, 2, 3, 4, new[] { 5, 6 })]
        public void Destructure_Five(
            IList<Int32> source,
            Int32 first,
            Int32 second,
            Int32 third,
            Int32 fourth,
            Int32 fifth,
            IEnumerable<Int32> rest) {

            var (n1, n2, n3, n4, n5, cdr) = source;

            Assert.Equal(first, n1);
            Assert.Equal(second, n2);
            Assert.Equal(third, n3);
            Assert.Equal(fourth, n4);
            Assert.Equal(fifth, n5);
            Assert.Equal(rest, cdr);
        }

        [Fact]
        public void Destructure_One_ArgumentOutOfRange() {
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                var (first, _) = new Int32[0];
            });
        }

        [Theory]
        [InlineData(new Int32[0])]
        [InlineData(new[] { 1 })]
        public void Destructure_Two_ArgumentOutOfRange(IList<Int32> source) {
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                var (first, second, _) = source;
            });
        }

        [Theory]
        [InlineData(new Int32[0])]
        [InlineData(new[] { 1, 2 })]
        public void Destructure_Three_ArgumentOutOfRange(IList<Int32> source) {
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                var (first, second, third, _) = source;
            });
        }

        [Theory]
        [InlineData(new Int32[0])]
        [InlineData(new[] { 1, 2, 3 })]
        public void Destructure_Four_ArgumentOutOfRange(IList<Int32> source) {
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                var (first, second, third, fourth, _) = source;
            });
        }

        [Theory]
        [InlineData(new Int32[0])]
        [InlineData(new[] { 1, 2, 3, 4 })]
        public void Destructure_Five_ArgumentOutOfRange(IList<Int32> source) {
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                var (first, second, third, fourth, fifth, _) = source;
            });
        }

        [Fact]
        public void Destructure_One_ArgumentNull() {
            Assert.Throws<ArgumentNullException>(() => {
                var (first, _) = (Int32[])null;
            });
        }

        [Fact]
        public void Destructure_Two_ArgumentNull() {
            Assert.Throws<ArgumentNullException>(() => {
                var (first, second, _) = (Int32[])null;
            });
        }

        [Fact]
        public void Destructure_Three_ArgumentNull() {
            Assert.Throws<ArgumentNullException>(() => {
                var (first, second, third, _) = (Int32[])null;
            });
        }

        [Fact]
        public void Destructure_Four_ArgumentNull() {
            Assert.Throws<ArgumentNullException>(() => {
                var (first, second, third, fourth, _) = (Int32[])null;
            });
        }

        [Fact]
        public void Destructure_Five_ArgumentNull() {
            Assert.Throws<ArgumentNullException>(() => {
                var (first, second, third, fourth, fifth, _) = (Int32[])null;
            });
        }
    }
}
