using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GameDataEditor.Views.RegularDialogs;

public partial class ItemSelectionDialog : Window
{
    public ItemSelectionDialog()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
