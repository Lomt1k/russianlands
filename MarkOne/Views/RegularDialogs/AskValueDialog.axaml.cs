using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MarkOne.Views.RegularDialogs;

public partial class AskValueDialog : Window
{
    public AskValueDialog()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
