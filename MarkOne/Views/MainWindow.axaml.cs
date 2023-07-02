using Avalonia.Controls;

namespace MarkOne.Views;

public partial class MainWindow : Window
{
    public static MainWindow instance;

    public MainWindow()
    {
        instance = this;
        InitializeComponent();
    }
}
