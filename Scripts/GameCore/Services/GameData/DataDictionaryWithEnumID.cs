using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TextGameRPG.Scripts.GameCore.Services.GameData
{
    public class DataDictionaryWithEnumID<TEnum, TData> where TEnum : Enum where TData : IDataWithEnumID<TEnum>
    {
        public readonly string dataPath;

        public Action onDataChanged;

        private Dictionary<TEnum, TData> _dictionary;

        public int count => _dictionary.Count;

        public TData this[TEnum id] => _dictionary[id];

        public DataDictionaryWithEnumID(Dictionary<TEnum, TData> dictionary, string path)
        {
            _dictionary = dictionary;
            dataPath = path;

            Program.onSetupAppMode += OnSetupAppMode;
        }

        public static DataDictionaryWithEnumID<TEnum, TData> LoadFromJSON<TEnum, TData>(string path) where TEnum : Enum where TData : IDataWithEnumID<TEnum>
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
                var dictionary = JsonConvert.DeserializeObject<IEnumerable<TData>>(jsonStr).ToDictionary(x => x.id);
                return new DataDictionaryWithEnumID<TEnum, TData>(dictionary, path);
            }
        }

        public bool ContainsKey(TEnum id)
        {
            return _dictionary.ContainsKey(id);
        }

        public IEnumerable<TData> GetAllData()
        {
            return _dictionary.Values;
        }

        public void AddData(TEnum id, TData data)
        {
            _dictionary.Add(id, data);
            OnDataChanged();
        }

        public void ChangeData(TEnum id, TData data)
        {
            _dictionary[id] = data;
            OnDataChanged();
        }

        public void RemoveData(TEnum id)
        {
            _dictionary.Remove(id);
            OnDataChanged();
        }

        private void OnDataChanged()
        {
            onDataChanged?.Invoke();
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
