using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using TextGameRPG.ViewModels.Editor.MobsEditor;

namespace TextGameRPG.Views.Editor.MobsEditor;

public partial class MobsEditorView : UserControl
{
    public MobsEditorView()
    {
        InitializeComponent();
        DataContext = new MobsEditorViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
