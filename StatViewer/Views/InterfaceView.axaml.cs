using Avalonia.Controls;
using StatViewer.ViewModels;

namespace StatViewer.Views;
public partial class InterfaceView : UserControl
{
    public readonly InterfaceViewModel vm = new InterfaceViewModel();

    public InterfaceView()
    {
        InitializeComponent();
        DataContext = vm;
    }
}
