using System.Collections.ObjectModel;
using System.Linq;
using TextGameRPG.Models.Editor.ItemsEditor;

namespace TextGameRPG.ViewModels.UserControls
{
    public class ObjectFieldsEditorViewModel : ViewModelBase
    {
        private object _object;
        private ObservableCollection<FieldModel> _fields = new ObservableCollection<FieldModel>();

        public string objectName { get; }
        public ObservableCollection<FieldModel> fields => _fields;

        public ObjectFieldsEditorViewModel(object obj)
        {
            _object = obj;

            FieldModel.FillObservableCollection(ref _fields, obj);
            objectName = obj.GetType().Name;
        }

        public void SaveObjectChanges()
        {
            var fieldInfos = _object.GetType().GetFields();
            foreach (var fieldInfo in fieldInfos)
            {
                var fieldModel = _fields.Where(x => x.name.Equals(fieldInfo.Name)).First();
                if (fieldModel.isValidValue)
                {
                    fieldInfo.SetValue(_object, fieldModel.value);
                }
            }
        }

    }
}
