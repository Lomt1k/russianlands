using System.Collections.ObjectModel;
using System.Linq;
using TextGameRPG.Models.Editor.ItemsEditor;

namespace TextGameRPG.ViewModels.UserControls
{
    public class ObjectFieldsEditorViewModel : ViewModelBase
    {
        private ObservableCollection<FieldModel> _fields = new ObservableCollection<FieldModel>();

        public object editableObject {get; private set; }
        public string objectName  => editableObject.GetType().Name;
        public ObservableCollection<FieldModel> fields => _fields;

        public ObjectFieldsEditorViewModel(object obj)
        {
            FieldModel.FillObservableCollection(ref _fields, obj);
            editableObject = obj;
        }

        public T GetEditableObject<T>()
        {
            return (T)editableObject;
        }

        public void SaveObjectChanges()
        {
            var fieldInfos = editableObject.GetType().GetFields();
            foreach (var fieldInfo in fieldInfos)
            {
                var fieldModel = _fields.Where(x => x.name.Equals(fieldInfo.Name)).First();
                if (fieldModel.isValidValue)
                {
                    fieldInfo.SetValue(editableObject, fieldModel.value);
                }
            }
        }

    }
}
