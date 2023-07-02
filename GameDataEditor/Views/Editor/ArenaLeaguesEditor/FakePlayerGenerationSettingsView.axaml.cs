using Avalonia.Controls;
using GameDataEditor.ViewModels.Editor.ArenaLeaguesEdittor;

namespace GameDataEditor.Views.Editor.ArenaLeaguesEditor;
public partial class FakePlayerGenerationSettingsView : UserControl
{
    public FakePlayerGenerationSettingsViewModel vm { get; }

    public FakePlayerGenerationSettingsView()
    {
        InitializeComponent();
        DataContext = vm = new FakePlayerGenerationSettingsViewModel();
    }
}
