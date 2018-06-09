using System;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Invio.Extensions.Threading.Tasks {
    /// <summary>
    /// Extension methods on the <see cref="Task" /> and <see cref="Task{T}" /> types.
    /// </summary>
    public static class TaskExtensions {
        /// <summary>
        /// Creates a new task that casts the result of the original task to a new type upon
        /// completion.
        /// </summary>
        /// <remarks>
        /// No type conversion is performed, so it is only possible to cast the result to the
        /// actual type, a base type, or an implemented interface.
        /// </remarks>
        /// <see cref="Enumerable.Cast{T}"/>
        /// <param name="task">The input task.</param>
        /// <typeparam name="T">Th type to cast the result to.</typeparam>
        /// <returns>
        /// A new task that casts theresult of the original task to the type
        /// <typeparamref name="T" /> upon completion.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// The parameter <paramref name="task" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The parameter <paramref name="task" /> does not return a result.
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// The cast from the original task result type to <typeparamref name="T" /> is invalid.
        /// </exception>
        public static Task<T> Cast<T>(this Task task) {
            if (task == null) {
                throw new ArgumentNullException(nameof(task));
            }

            if (task is Task<T> typedTask) {
                return typedTask;
            }

            var taskType = task.GetType();
            Console.WriteLine("Cast<T> - TaskType: " + taskType.ToString());
            Console.WriteLine("Cast<T> - IsGenericType: " + taskType.IsGenericType);
            if (taskType.IsGenericType) {
                Console.WriteLine("Cast<T> - GetGenericTypeDefinition: " + taskType.GetGenericTypeDefinition().ToString());
            }

            Console.WriteLine("Cast<T> - BaseType: " + taskType.BaseType?.ToString() ?? "null");

            if (!taskType.IsGenericType || taskType.GetGenericTypeDefinition() != typeof(Task<>)) {
                throw new ArgumentException(
                    "Cannot cast Task with no result type.",
                    nameof(task)
                );
            }

            var castTask =
                genericCastMethod
                    .MakeGenericMethod(taskType.GetGenericArguments().Single(), typeof(T))
                    .Invoke(null, new Object[] { task });

            return (Task<T>)castTask;
        }

        /// <summary>
        /// Create a new task that synchronously executes a function on the result of an input task
        /// upon completion of that task.
        /// </summary>
        /// <param name="task">
        /// Ths input task.
        /// </param>
        /// <param name="func">
        /// The function to execute.
        /// </param>
        /// <typeparam name="TResult">
        /// The type of the result returned by the input task.
        /// </typeparam>
        /// <typeparam name="TNewResult">
        /// The type returned by the function.
        /// </typeparam>
        /// <returns>
        /// A task which returns the result of executing <paramref name="func" /> on the result of
        /// <paramref name="task" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// The argument <paramref name="task" /> is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// The argument <paramref name="func" /> is null.
        /// </exception>
        public static Task<TNewResult> ContinueWithResult<TResult, TNewResult>(
            this Task<TResult> task,
            Func<TResult, TNewResult> func) {

            if (task == null) {
                throw new ArgumentNullException(nameof(task));
            }
            if (func == null) {
                throw new ArgumentNullException(nameof(func));
            }

            return task.ContinueWith(
                t => {
                    try {
                        return func(t.Result);
                    } catch(AggregateException ex) {
                        ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                        throw;
                    }
                },
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default
            );
        }

        private static MethodInfo genericCastMethod { get; } =
            ((Func<Task<Object>, Task<Object>>)Cast<Object, Object>)
                .Method
                .GetGenericMethodDefinition();

        private static async Task<TOut> Cast<TIn, TOut>(Task<TIn> task) {
            var result = await task.ConfigureAwait(false);
            return (TOut)(Object)result;
        }
    }
}
