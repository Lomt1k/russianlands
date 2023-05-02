using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MarkOne.Views.Editor.ItemsEditor;

public partial class EditItemAbilityWindow : Window
{
    public EditItemAbilityWindow()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
