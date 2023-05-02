using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MarkOne.Scripts.Bot;
using MarkOne.ViewModels;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Localizations;

public static class Localization
{
    private static readonly Dictionary<LanguageCode, Dictionary<string, string>> data = new Dictionary<LanguageCode, Dictionary<string, string>>();
    private static readonly HashSet<string> allKeys = new HashSet<string>();

    public static void LoadAll(GameDataLoader? loader, string gamedataPath)
    {
        allKeys.Clear();
        loader?.AddNextState("Loading localization...");
        var localizationFolder = Path.Combine(gamedataPath, "Localization");
        if (!Directory.Exists(localizationFolder))
        {
            Directory.CreateDirectory(localizationFolder);
        }

        foreach (var element in Enum.GetValues(typeof(LanguageCode)))
        {
            var code = (LanguageCode)element;
            loader?.AddInfoToCurrentState(code.ToString());
            var filePath = Path.Combine(localizationFolder, $"localization_{code.ToString().ToLower()}.json");
            data[code] = LoadLocalization(filePath);
        }
        // Убрал AlertMissingKeys так как пока разрабатываем только для русского языка и не следим за другими локализациями
        //AlertMissingKeys(loaderVM);
    }

    public static string GetDefault(string key, params object[] args)
    {
        var defaultLanguage = BotController.config?.defaultLanguageCode ?? LanguageCode.EN;
        return Get(defaultLanguage, key, args);
    }

    public static string Get(GameSession session, string key, params object[] args)
    {
        return Get(session.language, key, args);
    }

    public static string Get(LanguageCode languageCode, string key, params object[] args)
    {
        if (data.TryGetValue(languageCode, out var localization))
        {
            if (localization.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value))
            {
                return args.Length > 0 ? string.Format(value, args) : value;
            }
        }
        return key;
    }

    private static Dictionary<string, string> LoadLocalization(string filePath)
    {
        if (!File.Exists(filePath))
        {
            CreateEmptyLocalizationFile(filePath);
        }

        using (var reader = new StreamReader(filePath, Encoding.UTF8))
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
        using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
        {
            writer.Write(jsonStr);
        }
    }

    private static void AlertMissingKeys(GameDataLoader? loader)
    {
        foreach (var localization in data)
        {
            foreach (var key in allKeys)
            {
                if (!localization.Value.ContainsKey(key))
                {
                    var message = $"Localiation {localization.Key} not contains key: {key}";
                    loader?.AddNextState(message);
                }
            }
        }
    }

}
