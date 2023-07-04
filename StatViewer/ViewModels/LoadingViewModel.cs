using ReactiveUI;
using System.Threading.Tasks;

namespace StatViewer.ViewModels;
public class LoadingViewModel : ViewModelBase
{
    private string _defaultText = "Loading";
    private string? _loadingText = "Loading";

    public string? LoadingText
    {
        get => _loadingText;
        set => this.RaiseAndSetIfChanged(ref _loadingText, value);
    }

    public LoadingViewModel()
    {
        Task.Run(HandleAnimation);
    }

    public void SetText(string text)
    {
        _defaultText = text;
    }

    private async Task HandleAnimation()
    {
        await Task.Delay(500);
        while (true)
        {
            LoadingText = $"{_defaultText}.";
            await Task.Delay(500);
            LoadingText = $"{_defaultText}..";
            await Task.Delay(500);
            LoadingText = $"{_defaultText}...";
            await Task.Delay(500);
        }
    }

}
