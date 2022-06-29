using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TextGameRPG.ViewModels;

namespace TextGameRPG.Scripts.GameCore.Quests
{
    public static class QuestsHolder
    {
        private static string questFolderPath = string.Empty;
        private static Dictionary<QuestType, Quest> quests = new Dictionary<QuestType, Quest>();

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

                LoadQuest(questType, loaderVM);
            }
        }

        public static void LoadQuest(QuestType questType, GameDataLoaderViewModel? loaderVM = null)
        {
            var fileName = $"{questType}.json";
            loaderVM?.AddInfoToCurrentState($"\n* {fileName}");
            var filePath = Path.Combine(questFolderPath, fileName);

            if (!File.Exists(filePath))
            {
                var quest = new Quest() 
                {
                    questType = questType 
                };
                quests.Add(quest.questType, quest);
                SaveQuest(questType);
                loaderVM?.AddInfoToCurrentState($"\n[!] Created new quest file with name '{fileName}'");
                return;
            }

            using (StreamReader reader = new StreamReader(filePath, Encoding.UTF8))
            {
                var jsonStr = reader.ReadToEnd();
                var quest = JsonConvert.DeserializeObject<Quest>(jsonStr);
                quests[questType] = quest;
            }
        }

        public static void SaveQuests()
        {
            foreach (var questType in quests.Keys)
            {
                SaveQuest(questType);
            }
        }

        public static void SaveQuest(QuestType questType)
        {
            var quest = quests[questType];
            var filePath = Path.Combine(questFolderPath, questType + ".json");
            var jsonStr = JsonConvert.SerializeObject(quest, Formatting.Indented);
            using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                writer.Write(jsonStr);
            }
        }

        public static Quest GetQuest(QuestType questType)
        {
            return quests[questType];
        }

        public static IEnumerable<Quest> GetAllQuests()
        {
            return quests.Values;
        }

    }
}
