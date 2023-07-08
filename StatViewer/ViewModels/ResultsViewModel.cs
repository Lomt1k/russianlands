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
    private Border? _stackTable;

    public string title
    {
        get => _title;
        set => this.RaiseAndSetIfChanged(ref _title, value);
    }
    public Border? stackTable
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
        var border = new Border();
        var scrollViewer = new ScrollViewer();
        var table = new StackPanel();
        AddRow(table, _headers, isHeader: true);
        foreach (var row in _table)
        {
            AddRow(table, row, isHeader: false);
        }

        border.Child = scrollViewer;
        scrollViewer.Content = table;
        scrollViewer.MaxHeight = 800;
        scrollViewer.VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Visible;
        stackTable = border;
    }

    private static void AddRow(StackPanel table, List<string> columns, bool isHeader = false)
    {
        var rowStack = new StackPanel();
        rowStack.Orientation = Orientation.Horizontal;
        bool isFirstColumn = true;
        foreach (var column in columns)
        {
            rowStack.Children.Add(new TextBlock()
            {
                Text = column,
                Width = isFirstColumn ? 400 : 200,
                FontWeight = isHeader ? FontWeight.Bold : FontWeight.Normal
            });
            isFirstColumn = false;
        }
        table.Children.Add(rowStack);
    }

}
