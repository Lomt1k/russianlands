using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MarkOne.Views.RegularDialogs;

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
