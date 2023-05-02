using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MarkOne.Views.Editor.ItemsEditor;

public partial class EditItemWindow : Window
{
    public EditItemWindow()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
