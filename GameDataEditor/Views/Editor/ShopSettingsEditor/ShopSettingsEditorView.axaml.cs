using Avalonia.Controls;
using GameDataEditor.ViewModels.Editor.ShopSettingsEditor;

namespace GameDataEditor.Views.Editor.ShopSettingsEditor;
public partial class ShopSettingsEditorView : UserControl
{
    public ShopSettingsEditorView()
    {
        InitializeComponent();
        DataContext = new ShopSettingsEditorViewModel();
    }
}
