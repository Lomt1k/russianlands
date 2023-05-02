using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Services;

public abstract class Service
{
    public virtual Task OnBotStarted()
    {
        return Task.CompletedTask;
    }
    public virtual Task OnBotStopped()
    {
        return Task.CompletedTask;
    }
}
