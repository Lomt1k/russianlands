using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GameDataEditor.Views.Editor.ItemsEditor;

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
