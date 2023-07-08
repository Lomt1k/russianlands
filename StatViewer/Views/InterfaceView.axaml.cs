using Avalonia.Controls;
using StatViewer.ViewModels;

namespace StatViewer.Views;
public partial class InterfaceView : UserControl
{
    public readonly InterfaceViewModel viewModel = new();

    public InterfaceView()
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
