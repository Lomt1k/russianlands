using Avalonia.Controls;
using MarkOne.ViewModels.Editor.ArenaShopSettingsEditor;

namespace MarkOne.Views.Editor.ArenaShopSettingsEditor;
public partial class ArenaShopSettingsEditorView : UserControl
{
    public ArenaShopSettingsEditorView()
    {
        InitializeComponent();
        DataContext = new ArenaShopSettingsEditorViewModel();
    }
}
