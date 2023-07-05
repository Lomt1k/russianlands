using Avalonia.Controls;
using StatViewer.ViewModels;

namespace StatViewer.Views;
public partial class ResultsView : UserControl
{
    public ResultsViewModel viewModel { get; } = new();

    public ResultsView()
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
