using Avalonia.Controls;
using GameDataEditor.ViewModels.Editor.ArenaShopSettingsEditor;

namespace GameDataEditor.Views.Editor.ArenaShopSettingsEditor;
public partial class ArenaShopSettingsEditorView : UserControl
{
    public ArenaShopSettingsEditorView()
    {
        InitializeComponent();
        DataContext = new ArenaShopSettingsEditorViewModel();
    }
}
