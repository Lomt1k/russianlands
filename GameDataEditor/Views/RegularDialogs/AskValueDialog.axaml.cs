using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GameDataEditor.Views.RegularDialogs;

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
