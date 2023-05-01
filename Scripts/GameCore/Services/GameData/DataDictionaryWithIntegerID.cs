using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace TextGameRPG.Scripts.GameCore.Services.GameData
{
    public class DataDictionaryWithIntegerID<T> where T : IDataWithIntegerID
    {
        public readonly string dataPath;

        public System.Action onDataChanged;

        private Dictionary<int, T> _dictionary;

        public int count => _dictionary.Count;

        public T this[int id] => _dictionary[id];

        public DataDictionaryWithIntegerID(Dictionary<int, T> dictionary, string path)
        {
            _dictionary = dictionary;
            dataPath = path;

            Program.onSetupAppMode += OnSetupAppMode;
        }

        public static DataDictionaryWithIntegerID<Type> LoadFromJSON<Type>(string path) where Type : IDataWithIntegerID
        {
            if (!File.Exists(path))
            {
                using (StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8))
                {
                    writer.Write("[]");
                }
            }
            using (StreamReader reader = new StreamReader(path, Encoding.UTF8))
            {
                var jsonStr = reader.ReadToEnd();
                var dictionary = JsonConvert.DeserializeObject<IEnumerable<Type>>(jsonStr).ToDictionary(x => x.id);
                return new DataDictionaryWithIntegerID<Type>(dictionary, path);
            }
        }

        public bool ContainsKey(int id)
        {
            return _dictionary.ContainsKey(id);
        }

        public IEnumerable<T> GetAllData()
        {
            return _dictionary.Values;
        }

        public void AddData(int id, T data)
        {
            _dictionary.Add(id, data);
            OnDataChanged();
        }

        public void ChangeData(int id, T data)
        {
            _dictionary[id] = data;
            OnDataChanged();
        }

        public void RemoveData(int id)
        {
            _dictionary.Remove(id);
            OnDataChanged();
        }

        private void OnDataChanged()
        {
            onDataChanged?.Invoke();
        }

        public void ReloadAllData()
        {
            _dictionary.Clear();
            using (StreamReader reader = new StreamReader(dataPath, Encoding.UTF8))
            {
                var jsonStr = reader.ReadToEnd();
                _dictionary = JsonConvert.DeserializeObject<IEnumerable<T>>(jsonStr).ToDictionary(x => x.id);
            }
        }

        public void Save()
        {
            if (Program.appMode != AppMode.Editor)
                return;

            var jsonStr = JsonConvert.SerializeObject(_dictionary.Values, Formatting.Indented);
            using (StreamWriter writer = new StreamWriter(dataPath, false, Encoding.UTF8))
            {
                writer.Write(jsonStr);
            }
        }

        private void OnSetupAppMode(AppMode appMode)
        {
            if (appMode != AppMode.PlayMode)
                return;

            foreach (var item in _dictionary.Values)
            {
                item.OnSetupAppMode(appMode);
            }
        }

    }
}
