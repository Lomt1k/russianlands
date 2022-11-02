using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TextGameRPG.Scripts.TelegramBot.Sessions;
using TextGameRPG.ViewModels;

namespace TextGameRPG.Scripts.GameCore.Localizations
{
    public static class Localization
    {
        private static Dictionary<LanguageCode, Dictionary<string, string>> data = new Dictionary<LanguageCode, Dictionary<string, string>>();
        private static HashSet<string> allKeys = new HashSet<string>();

        public static void LoadAll(IGameDataLoader loaderVM, string gamedataPath)
        {
            allKeys.Clear();
            loaderVM.AddNextState("Loading localization...");
            string localizationFolder = Path.Combine(gamedataPath, "Localization");
            if (!Directory.Exists(localizationFolder))
            {
                Directory.CreateDirectory(localizationFolder);
            }

            foreach (var element in Enum.GetValues(typeof(LanguageCode)))
            {
                var code = (LanguageCode)element;
                loaderVM.AddInfoToCurrentState(code.ToString());
                var filePath = Path.Combine(localizationFolder, $"localization_{code}.json");
                data[code] = LoadLocalization(filePath);
            }
            // Убрал AlertMissingKeys так как пока разрабатываем только для русского языка и не следим за другими локализациями
            //AlertMissingKeys(loaderVM);
        }

        public static string Get(GameSession session, string key)
        {
            if (data.TryGetValue(session.language, out var localization))
            {
                if (localization.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }
            return key;
        }

        private static Dictionary<string,string> LoadLocalization(string filePath)
        {
            if (!File.Exists(filePath))
            {
                CreateEmptyLocalizationFile(filePath);
            }

            using (StreamReader reader = new StreamReader(filePath, Encoding.UTF8))
            {
                var jsonStr = reader.ReadToEnd();
                var localization = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonStr);

                foreach (var key in localization.Keys)
                {
                    allKeys.Add(key);
                }

                return localization;
            }
        }

        private static void CreateEmptyLocalizationFile(string filePath)
        {
            var newLocalization = new Dictionary<string, string>();
            var jsonStr = JsonConvert.SerializeObject(newLocalization, Formatting.Indented);
            using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                writer.Write(jsonStr);
            }
        }

        private static void AlertMissingKeys(GameDataLoaderViewModel loaderVM)
        {
            foreach (var localization in data)
            {
                foreach (var key in allKeys)
                {
                    if (!localization.Value.ContainsKey(key))
                    {
                        string message = $"Localiation {localization.Key} not contains key: {key}";
                        loaderVM.AddNextState(message);
                    }
                }
            }
        }

    }
}
