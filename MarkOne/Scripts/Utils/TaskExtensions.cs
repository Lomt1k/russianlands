using System.Runtime.CompilerServices;
using System.Threading.Tasks;

public static class TaskExtensions
{
    public static ConfiguredTaskAwaitable<TResult> FastAwait<TResult>(this Task<TResult> task)
    {
        return task.ConfigureAwait(continueOnCapturedContext: false);
    }

    public static ConfiguredTaskAwaitable FastAwait(this Task task)
    {
        return task.ConfigureAwait(continueOnCapturedContext: false);
    }
}
