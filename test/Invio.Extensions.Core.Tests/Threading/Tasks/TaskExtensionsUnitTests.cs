using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Invio.Extensions.Collections;
using Invio.Extensions.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Invio.Extensions.Core.Tests.Threading.Tasks {
    public class TaskExtensionsUnitTests {
        // private readonly ITestOutputHelper outputHelper;
        private const Int32 IterationCount = 20;

        // public TaskExtensionsUnitTests(ITestOutputHelper outputHelper) {
        //     this.outputHelper = outputHelper;
        // }

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

        [Fact]
        public async Task ContinueWithResult_FunctionExecuted() {
            var task = GenerateRandomData(IterationCount);

            var result = await task.ContinueWithResult(list => list.Max());

            Assert.True(task.IsCompletedSuccessfully);
            Assert.Equal(task.Result.Max(), result);
        }

        [Fact]
        public async Task ContinueWithResult_ExecutedSynchronously() {
            var threadId = -1;
            var continuationThreadId = -2;

            var task = new Task<Int32>(() => {
                threadId = Thread.CurrentThread.ManagedThreadId;
                Thread.Sleep(50);
                return 21;
            });
            task.Start();

            var result = await task.ContinueWithResult(v => {
                continuationThreadId = Thread.CurrentThread.ManagedThreadId;
                return v * 2;
            });

            Assert.True(task.IsCompletedSuccessfully);
            Assert.Equal(42, result);
            Assert.Equal(threadId, continuationThreadId);
        }

        [Fact]
        public async Task ContinueWithResult_NotExecutedOnError() {
            var task = new Task<Int32>(() => {
                Thread.Sleep(50);
                throw new InvalidOperationException("test exception");
            });
            task.Start();

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                task.ContinueWithResult<Int32, Int32>(
                    v => throw new NotImplementedException()
                )
            );

            Assert.Equal("test exception", exception.Message);
        }

        [Fact]
        public void ContinueWithResult_NullTask() {
            var exception = Assert.Throws<ArgumentNullException>(() => { ((Task<Int32>)null).ContinueWithResult(v => v); });

            Assert.Equal("task", exception.ParamName);
        }

        [Fact]
        public void ContinueWithResult_NullFunc() {
            var task = new Task<Int32>(() => 0);

            var exception = Assert.Throws<ArgumentNullException>(() => { task.ContinueWithResult<Int32, Int32>(null); });

            Assert.Equal("func", exception.ParamName);
        }

        [Fact]
        public void SingleThreadedTaskScheduler_WaitDeadlocks() {
            var output = new ConcurrentQueue<String>();
            var executing = new ManualResetEventSlim();
            var doneExecuting = new ManualResetEventSlim();
            var testThread = new Thread(() => {
                var taskFactory = new TaskFactory(new SingleThreadedTaskScheduler());

                taskFactory.StartNew(() => {
                    // This is a bad idea
                    output.Enqueue($"Result: {SomeAsyncFunction(output, executing).Result}");
                    doneExecuting.Set();
                });
            });

            testThread.Start();

            Assert.True(executing.Wait(5000), "The task did not start executing");
            Assert.False(doneExecuting.Wait(5000), "The task finished when it should have deadlocked.");
        }

        [Fact]
        public void SingleThreadedTaskScheduler_OverlyProtectiveAsyncFunctionDoesNotDeadlocks() {
            var output = new ConcurrentQueue<String>();
            var executing = new ManualResetEventSlim();
            var doneExecuting = new ManualResetEventSlim();
            var testThread = new Thread(() => {
                var taskFactory = new TaskFactory(new SingleThreadedTaskScheduler());

                taskFactory.StartNew(() => {
                    output.Enqueue($"Result: {OverlyProtectiveAsyncFunction(output, executing).Result}");
                    doneExecuting.Set();
                });
            });

            testThread.Start();

            Assert.True(executing.Wait(5000), "The task did not start executing");
            Assert.True(doneExecuting.Wait(5000), "The task deadlocked when it should have finished.");

            Assert.Equal(3, output.Count);
            // foreach (var line in output) {
            //     this.outputHelper.WriteLine(line);
            // }
        }

        [Fact]
        public void SingleThreadedTaskScheduler_SynchronouslyAwaitDoesNotDeadlocks() {
            var output = new ConcurrentQueue<String>();
            var executing = new ManualResetEventSlim();
            var doneExecuting = new ManualResetEventSlim();
            var testThread = new Thread(() => {
                var taskFactory = new TaskFactory(new SingleThreadedTaskScheduler());

                taskFactory.StartNew(() => {
                    output.Enqueue($"Result: {Synchronously.Await(() => SomeAsyncFunction(output, executing))}");
                    doneExecuting.Set();
                });
            });

            testThread.Start();

            Assert.True(executing.Wait(5000), "The task did not start executing");
            Assert.True(doneExecuting.Wait(5000), "The task deadlocked when it should have finished.");

            Assert.Equal(3, output.Count);
            // foreach (var line in output) {
            //     this.outputHelper.WriteLine(line);
            // }
        }


        private async Task<Int32> SomeAsyncFunction(ConcurrentQueue<String> output, ManualResetEventSlim signal) {
            // this.outputHelper.WriteLine("Async Function Started");
            output.Enqueue("Task Started");
            signal.Set();
            await Task.Delay(100);
            // this.outputHelper.WriteLine("First Delay Completed");
            // If this task is awaited synchronously on the main thread than there will be a deadlock and this part of the function will never execute.
            await Task.Delay(100);
            // this.outputHelper.WriteLine("Second Delay Completed");
            output.Enqueue("Task Complete");
            return output.Count;
        }

        private async Task<Int32> OverlyProtectiveAsyncFunction(ConcurrentQueue<String> output, ManualResetEventSlim signal) {
            // this.outputHelper.WriteLine("Async Function Started");
            output.Enqueue("Task Started");
            signal.Set();
            await Task.Delay(100).ConfigureAwait(false);
            // this.outputHelper.WriteLine("First Delay Completed");
            // If this task is awaited synchronously on the main thread than there will be a deadlock and this part of the function will never execute.
            await Task.Delay(100).ConfigureAwait(false);
            // this.outputHelper.WriteLine("Second Delay Completed");
            output.Enqueue("Task Complete");
            return output.Count;
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

        private class SingleThreadedTaskScheduler : TaskScheduler {
            // Indicates whether the current thread is processing work items.
            [ThreadStatic] private static Boolean currentThreadIsProcessingItems;

            private readonly ConcurrentQueue<TaskWrapper> taskQueue = new ConcurrentQueue<TaskWrapper>();
            private readonly SemaphoreSlim taskStartSync = new SemaphoreSlim(1);
            private readonly Object taskQueueSync = new Object();

            // private readonly ITestOutputHelper outputHelper;

            // public SingleThreadedTaskScheduler(ITestOutputHelper outputHelper) {
            //     this.outputHelper = outputHelper;
            // }

            protected override IEnumerable<Task> GetScheduledTasks() {
                return this.taskQueue.Select(w => w.Task).ToList();
            }

            protected override void QueueTask(Task task) {
                // this.outputHelper.WriteLine($"Queuing task: {task.GetHashCode()}");
                // Atomically acquire enqueue our new task and then check the semaphore
                Monitor.Enter(this.taskQueueSync);
                this.taskQueue.Enqueue(task);
                if (this.taskStartSync.Wait(TimeSpan.Zero)) {
                    Monitor.Exit(this.taskQueueSync);
                    // No task was running, we can proceed
                    // this.outputHelper.WriteLine($"Initiating ThreadPool execution for task: {task.GetHashCode()}");
                    ThreadPool.UnsafeQueueUserWorkItem(
                        _ => {
                            // this.outputHelper.WriteLine($"Beginning task execution with task: {task.GetHashCode()}");
                            currentThreadIsProcessingItems = true;
                            try {
                                while (true) {
                                    TaskWrapper nextTask;

                                    // Atomically check the task queue length and release the semaphore if it is empty
                                    Monitor.Enter(this.taskQueueSync);
                                    try {
                                        // this.outputHelper.WriteLine($"Attempting to dequeue the next task");
                                        if (!this.taskQueue.TryDequeue(out nextTask)) {
                                            // this.outputHelper.WriteLine($"No Tasks Pending");
                                            this.taskStartSync.Release();
                                            break;
                                        }
                                    } finally {
                                        Monitor.Exit(this.taskQueueSync);
                                    }

                                    if (!nextTask.Skip) {
                                        // this.outputHelper.WriteLine($"Executing Task: {nextTask.Task.GetHashCode()}");
                                        this.TryExecuteTask(nextTask.Task);
                                    } else {
                                        // this.outputHelper.WriteLine($"Skipping Previously Executed Task: {nextTask.Task.GetHashCode()}");
                                    }
                                }
                            } finally {
                                currentThreadIsProcessingItems = false;
                            }
                        },
                        null
                    );
                } else {
                    // else some other task is currently running.
                    Monitor.Exit(this.taskQueueSync);
                }
            }

            protected override Boolean TryExecuteTaskInline(Task task, Boolean taskWasPreviouslyQueued) {
                if (!currentThreadIsProcessingItems) {
                    // this.outputHelper.WriteLine($"Declining to inline task {task.GetHashCode()} because this is not a task execution thread.");
                    return false;
                }

                if (taskWasPreviouslyQueued) {
                    var wrapper = this.taskQueue.SingleOrDefault(w => w.Task == task);
                    if (wrapper != null) {
                        wrapper.Skip = true;
                    }
                }

                // this.outputHelper.WriteLine($"Inlining Task {task.GetHashCode()}");
                return base.TryExecuteTask(task);
            }

            private class TaskWrapper {
                public Task Task { get; }

                public Boolean Skip { get; set; }

                private TaskWrapper(Task task) {
                    this.Task = task;
                }

                public static implicit operator TaskWrapper(Task task) {
                    return new TaskWrapper(task);
                }
            }
        }
    }
}
