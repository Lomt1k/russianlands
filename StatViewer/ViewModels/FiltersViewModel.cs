using ReactiveUI;
using StatViewer.Scripts;
using StatViewer.Scripts.Metrics;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace StatViewer.ViewModels;
public class FiltersViewModel : ViewModelBase
{
    private MetricType _selectedMetricType = MetricType.DailyActiveUsers;
    private FilterModel? _selectedFilter;
    private bool _showFilters = false;

    public MetricType selectedMetricType
    {
        get => _selectedMetricType;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedMetricType, value);
            showFilters = value.IsSupportFilters();
        }
    }
    public FilterModel? selectedFilter
    {
        get => _selectedFilter;
        set => this.RaiseAndSetIfChanged(ref _selectedFilter, value);
    }
    public bool showFilters
    {
        get => _showFilters;
        set => this.RaiseAndSetIfChanged(ref _showFilters, value);
    }

    public ObservableCollection<MetricType> metricTypes => new(Enum.GetValues<MetricType>());
    public ObservableCollection<FilterModel> filters { get; } = new();
    public ReactiveCommand<Unit, Unit> addFilterCommand { get; }
    public ReactiveCommand<Unit, Unit> removeFilterCommand { get; }
    public ReactiveCommand<Unit, Task> showStatsCommand { get; }

    public FiltersViewModel()
    {
        addFilterCommand = ReactiveCommand.Create(AddNewFilter);
        removeFilterCommand = ReactiveCommand.Create(RemoveNewFilter);
        showStatsCommand = ReactiveCommand.Create(ShowStats);
    }

    private void AddNewFilter()
    {
        var filter = new FilterModel();
        filters.Add(filter);
        selectedFilter = filter;
    }

    private void RemoveNewFilter()
    {
        if (selectedFilter is null)
        {
            return;
        }
        filters.Remove(selectedFilter);
        selectedFilter = null;
    }

    private async Task ShowStats()
    {
        await App.mainViewModel.StartLoading(() => StatDataBase.ShowStats(selectedMetricType, filters.ToArray()), $"Loading {selectedMetricType}");
    }

}
