using Avalonia.Controls;
using StatViewer.ViewModels;

namespace StatViewer.Views;
public partial class LoadingView : UserControl
{
    public readonly LoadingViewModel vm  = new LoadingViewModel();

    public LoadingView()
    {
        InitializeComponent();
        DataContext = vm;
    }
}
