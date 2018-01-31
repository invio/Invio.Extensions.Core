using System;
using System.Linq;
using System.Reflection;
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