using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TextGameRPG.ViewModels;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Quests
{
    internal static class QuestsHolder
    {
        private static string questFolderPath = string.Empty;
        private static Dictionary<QuestType, QuestBase> quests = new Dictionary<QuestType, QuestBase>();

        public static void LoadAll(GameDataLoaderViewModel loaderVM, string gamedataPath)
        {
            loaderVM.AddNextState("Loading quests...");
            quests.Clear();
            questFolderPath = Path.Combine(gamedataPath, "Quests");
            if (!Directory.Exists(questFolderPath))
            {
                Directory.CreateDirectory(questFolderPath);
            }

            var allQuests = Enum.GetValues(typeof(QuestType));
            foreach (QuestType questType in allQuests)
            {
                if (questType == QuestType.None)
                    continue;

                LoadQuest(loaderVM, questType);
            }
        }

        public static void LoadQuest(GameDataLoaderViewModel loaderVM, QuestType questType)
        {
            var fileName = $"{questType}.json";
            loaderVM.AddInfoToCurrentState($"\n* {fileName}");
            var filePath = Path.Combine(questFolderPath, fileName);

            bool needSave = false;
            if (!File.Exists(filePath))
            {
                CreateEmptyQuestFile(filePath, questType);
                loaderVM.AddInfoToCurrentState($"\n[!] Created new quest file with name '{fileName}'");
                needSave = true;
            }

            using (StreamReader reader = new StreamReader(filePath, Encoding.UTF8))
            {
                var jsonStr = reader.ReadToEnd();
                var quest = JsonConvert.DeserializeObject<QuestBase>(jsonStr);
                quests.Add(quest.questType, quest);
            }

            if (needSave)
            {
                SaveQuest(questType);
            }
        }

        private static void CreateEmptyQuestFile(string filePath, QuestType questType)
        {
            using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                var sb = new StringBuilder();
                sb.Append("{$type : \"");
                sb.Append(questType);
                sb.Append("\",}");
                writer.WriteLine(sb.ToString());
            }
        }

        public static void SaveQuests()
        {
            foreach (var questType in quests.Keys)
            {
                SaveQuest(questType);
            }
        }

        private static void SaveQuest(QuestType questType)
        {
            var quest = quests[questType];
            var filePath = Path.Combine(questFolderPath, questType + ".json");
            var jsonStr = JsonConvert.SerializeObject(quest, Formatting.Indented);
            using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                writer.Write(jsonStr);
            }
        }

        public static QuestBase GetQuest(QuestType questType)
        {
            return quests[questType];
        }

        public static IEnumerable<QuestBase> GetAllQuests()
        {
            return quests.Values;
        }

    }
}
