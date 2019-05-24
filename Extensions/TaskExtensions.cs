using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vitevic.Shared.Extensions
{
    public static class TaskExtensions
    {
        /// <summary>
        /// Implemented using ContinueWith.
        /// https://github.com/dotnet/roslyn/issues/13897#issuecomment-318171853
        /// Note: ContinueWith uses <c>TaskScheduler.Current</c>, not <c>TaskScheduler.Default</c>!
        /// </summary>
        public static void FireAndForget(this Task task, Action<Exception> handler = null)
        {
            if (task != null)
            {
                task.ContinueWith(t => {
                    try
                    {
                        t.GetAwaiter().GetResult();
                    }
                    catch(Exception ex)
                    {
                        handler?.Invoke(ex);
                    }
                }, TaskContinuationOptions.NotOnRanToCompletion);
            }
        }
    }
}
