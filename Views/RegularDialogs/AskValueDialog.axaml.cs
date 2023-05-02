using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TextGameRPG.Views.RegularDialogs;

public partial class AskValueDialog : Window
{
    public AskValueDialog()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
