using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using ReactiveUI;
using System.Collections.Generic;

namespace StatViewer.ViewModels;
public class ResultsViewModel : ViewModelBase
{
    public static ResultsViewModel instance { get; private set; }

    private string _title;
    private StackPanel? _stackTable;

    public string title
    {
        get => _title;
        set => this.RaiseAndSetIfChanged(ref _title, value);
    }
    public StackPanel? stackTable
    {
        get => _stackTable;
        set => this.RaiseAndSetIfChanged(ref _stackTable, value);
    }

    public ResultsViewModel()
    {
        instance = this;
    }

    public void SetData(string _title, List<string> _headers, List<List<string>> _table)
    {
        title = _title;
        var table = new StackPanel();
        AddRow(table, _headers, isHeader: true);
        foreach (var row in _table)
        {
            AddRow(table, row, isHeader: false);
        }
        stackTable = table;
    }

    private static void AddRow(StackPanel table, List<string> columns, bool isHeader = false)
    {
        var rowStack = new StackPanel();
        rowStack.Orientation = Orientation.Horizontal;
        foreach (var column in columns)
        {
            rowStack.Children.Add(new TextBlock()
            {
                Text = column,
                Width = 200,
                FontWeight = isHeader ? FontWeight.Bold : FontWeight.Normal
            });
        }
        table.Children.Add(rowStack);
    }

}
