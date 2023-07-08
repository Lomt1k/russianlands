using StatViewer.Views;

namespace StatViewer.ViewModels;
public class InterfaceViewModel : ViewModelBase
{
    public FiltersView filtersView { get; } = new();
    public ResultsView resultsView { get; } = new();
}
