using Avalonia.Controls;
using GameDataEditor.ViewModels.Editor.LocationMobsEditor;

namespace GameDataEditor.Views.Editor.LocationMobsEditor;

public partial class LocationMobsEditorView : UserControl
{
    public LocationMobsEditorView()
    {
        InitializeComponent();
        DataContext = new LocationMobsEditorViewModel();
    }
}
