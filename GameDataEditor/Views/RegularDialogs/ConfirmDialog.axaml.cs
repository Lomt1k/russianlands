using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GameDataEditor.Views.RegularDialogs;

public partial class ConfirmDialog : Window
{
    public ConfirmDialog()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
