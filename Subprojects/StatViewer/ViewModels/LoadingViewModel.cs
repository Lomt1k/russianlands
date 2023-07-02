using ReactiveUI;
using StatViewer.Views;
using System.Threading.Tasks;

namespace StatViewer.ViewModels;
internal class LoadingViewModel : ViewModelBase
{
    private string? _loadingText = "Loading";
    private readonly LoadingView _view;

    public string? LoadingText
    {
        get => _loadingText;
        set => this.RaiseAndSetIfChanged(ref _loadingText, value);
    }

    public LoadingViewModel(LoadingView view)
    {
        Task.Run(HandleAnimation);
        _view = view;
    }

    private async Task HandleAnimation()
    {
        await Task.Delay(500);
        while (_view is not null)
        {
            LoadingText = "Loading.";
            await Task.Delay(500);
            LoadingText = "Loading..";
            await Task.Delay(500);
            LoadingText = "Loading...";
            await Task.Delay(500);
        }
    }

}
