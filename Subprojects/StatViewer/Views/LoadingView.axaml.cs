using Avalonia.Controls;
using StatViewer.ViewModels;

namespace StatViewer.Views;
public partial class LoadingView : UserControl
{
    public LoadingView()
    {
        InitializeComponent();
        DataContext = new LoadingViewModel(this);
    }
}
