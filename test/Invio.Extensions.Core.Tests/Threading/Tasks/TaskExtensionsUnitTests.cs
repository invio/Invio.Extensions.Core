using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Invio.Extensions.Collections;
using Invio.Extensions.Threading.Tasks;
using Xunit;

namespace Invio.Extensions.Core.Tests.Threading.Tasks {
    public class TaskExtensionsUnitTests {
        private const Int32 IterationCount = 20;

        [Fact]
        public async Task Cast_ToObject() {
            var task = GenerateRandomData(IterationCount);

            var result = await task.Cast<Object>();

            var list = Assert.IsType<List<Int32>>(result);
            Assert.Equal(IterationCount, list.Count);
        }

        [Fact]
        public async Task Cast_CompareToContinueWith() {
            var task = GenerateRandomData(IterationCount);

            var continueWithTask = task.ContinueWith(t => (List<Int32>)t.Result);

            var castTask = task.Cast<List<Int32>>();

            var (expected, actual, _) = await Task.WhenAll(continueWithTask, castTask);

            Assert.Same(expected, actual);
        }

        [Fact]
        public async Task Cast_Implicit() {
            var task = GenerateRandomData(IterationCount);

            var result = await task.Cast<IEnumerable<Int32>>();

            Assert.NotNull(result);
            Assert.Equal(IterationCount, result.Count());
        }

        [Fact]
        public async Task Cast_Unchanged() {
            Task task = GenerateRandomData(IterationCount);

            var result = await task.Cast<IList<Int32>>();

            Assert.NotNull(result);
            Assert.Equal(IterationCount, result.Count);
        }

        [Fact]
        public async Task Cast_Actual() {
            var task = GenerateRandomData(IterationCount);

            var result = await task.Cast<List<Int32>>();

            Assert.NotNull(result);
            Assert.Equal(IterationCount, result.Count);
        }

        [Fact]
        public void Cast_Synchronous() {
            var task = GenerateRandomData(IterationCount);

            var result = task.Cast<List<Int32>>().Result;

            Assert.NotNull(result);
            Assert.Equal(IterationCount, result.Count);
            Assert.True(task.IsCompletedSuccessfully);
        }

        [Fact]
        public async Task Cast_AfterContinuation() {
            async Task<IList<Double>> DivByTwo(Task<IList<Int32>> list) {
                return (await list).Select(v => v / 2.0).ToList();
            }

            var task = DivByTwo(GenerateRandomData(IterationCount));

            var result = await task.Cast<List<Double>>();

            Assert.NotNull(result);
            Assert.Equal(IterationCount, result.Count);
        }

        [Fact]
        public async Task Cast_Complete() {
            var task = GenerateRandomData(IterationCount);
            await task;

            var result = await task.Cast<List<Int32>>();

            Assert.NotNull(result);
            Assert.Equal(IterationCount, result.Count);
        }

        [Fact]
        public async Task Cast_Error() {
            async Task<IList<Int32>> Bork(Task<IList<Int32>> t) {
                var v = await t;
                if (v.Any()) {
                    throw new InvalidOperationException("totally senseless");
                }

                return v;
            }

            var task = Bork(GenerateRandomData(IterationCount));

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                task.Cast<Int32>()
            );

            Assert.Equal("totally senseless", exception.Message);
        }

        [Fact]
        public async Task Cast_Invalid() {
            var task = GenerateRandomData(IterationCount);

            await Assert.ThrowsAsync<InvalidCastException>(() =>
                task.Cast<Dictionary<Int32, Int32>>()
            );
        }

        [Fact]
        public void Cast_ArgumentNull() {
            // Should throw synchronously
            var exception = Assert.Throws<ArgumentNullException>(() =>
                (Object)((Task)null).Cast<Object>()
            );

            Assert.Equal("task", exception.ParamName);
        }

        [Fact]
        public void Cast_VoidResult_ArgumentException() {
            var task = new Task(() => Console.WriteLine("test"));

            // Should throw synchronously
            var exception = Assert.Throws<ArgumentException>(() => (Object)task.Cast<Object>());

            Assert.Equal("task", exception.ParamName);
        }

        private static Task<IList<Int32>> GenerateRandomData(Int32 r) {
            var task = new Task<IList<Int32>>(() => {
                var rand = new Random();
                var list = new List<Int32>(r);
                for (var i = 0; i < r; i++) {
                    list.Add(rand.Next());
                }

                list.Sort();
                return list;
            });
            task.Start();
            return task;
        }
    }
}