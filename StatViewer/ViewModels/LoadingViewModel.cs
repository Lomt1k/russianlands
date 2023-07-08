using ReactiveUI;
using System.Threading;
using System.Threading.Tasks;

namespace StatViewer.ViewModels;
public class LoadingViewModel : ViewModelBase
{
    private string _defaultText = "Loading";
    private string? _loadingText = "Loading";
    private CancellationTokenSource _cts = new();

    public string? LoadingText
    {
        get => _loadingText;
        set => this.RaiseAndSetIfChanged(ref _loadingText, value);
    }

    public LoadingViewModel()
    {
        Task.Run(() => HandleAnimation(_cts.Token), _cts.Token);
    }

    public void SetText(string text)
    {
        _defaultText = text;
        LoadingText = text;

        _cts.Cancel();
        _cts = new();
        Task.Run(() => HandleAnimation(_cts.Token), _cts.Token);
    }

    private async Task HandleAnimation(CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(500);
            while (!cancellationToken.IsCancellationRequested)
            {
                LoadingText = $"{_defaultText}.";
                await Task.Delay(500, cancellationToken);
                LoadingText = $"{_defaultText}..";
                await Task.Delay(500, cancellationToken);
                LoadingText = $"{_defaultText}...";
                await Task.Delay(500, cancellationToken);
            }
        }
        catch (TaskCanceledException)
        {
            // ignored
        }
    }

}
