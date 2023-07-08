using Avalonia.Controls;
using StatViewer.ViewModels;

namespace StatViewer.Views;
public partial class FiltersView : UserControl
{
    public FiltersViewModel viewModel { get; } = new();

    public FiltersView()
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
