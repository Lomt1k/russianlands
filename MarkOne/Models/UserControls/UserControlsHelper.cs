using System.Collections.Generic;
using System.Collections.ObjectModel;
using MarkOne.ViewModels.UserControls;
using MarkOne.Views.UserControls;

namespace MarkOne.Models.UserControls;

public static class UserControlsHelper
{
    public static ObjectPropertiesEditorView CreateObjectEditorView(object obj)
    {
        var view = new ObjectPropertiesEditorView();
        view.DataContext = new ObjectPropertiesEditorViewModel(obj);
        return view;
    }

    public static void RefillObjectEditorsCollection(ObservableCollection<ObjectPropertiesEditorView> collection, IEnumerable<object> objects)
    {
        collection.Clear();
        foreach (var element in objects)
        {
            var editorView = CreateObjectEditorView(element);
            collection.Add(editorView);
        }
    }

}
