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
        private static Dictionary<QuestId, Quest> quests = new Dictionary<QuestId, Quest>();

        public static void LoadAll(GameDataLoader? loader, string gamedataPath)
        {
            loader?.AddNextState("Loading quests...");
            quests.Clear();
            questFolderPath = Path.Combine(gamedataPath, "Quests");
            if (!Directory.Exists(questFolderPath))
            {
                Directory.CreateDirectory(questFolderPath);
            }

            var allQuests = Enum.GetValues(typeof(QuestId));
            foreach (QuestId questId in allQuests)
            {
                if (questId == QuestId.None)
                    continue;

                LoadQuest(questId, loader);
            }
            loader?.AddInfoToCurrentState(quests.Count.ToString());
        }

        public static void LoadQuest(QuestId questId, GameDataLoader? loader = null)
        {
            var fileName = $"{questId}.json";
            var filePath = Path.Combine(questFolderPath, fileName);

            if (!File.Exists(filePath))
            {
                var quest = new Quest() 
                {
                    questId = questId 
                };
                quests.Add(quest.questId, quest);
                SaveQuest(questId);
                loader?.AddInfoToCurrentState($"\n[!] Created new quest file with name '{fileName}'");
                return;
            }

            using (StreamReader reader = new StreamReader(filePath, Encoding.UTF8))
            {
                var jsonStr = reader.ReadToEnd();
                var quest = JsonConvert.DeserializeObject<Quest>(jsonStr);
                quests[questId] = quest;
            }
        }

        public static void SaveQuests()
        {
            foreach (var questId in quests.Keys)
            {
                SaveQuest(questId);
            }
        }

        public static void SaveQuest(QuestId questId)
        {
            var quest = quests[questId];
            var filePath = Path.Combine(questFolderPath, questId + ".json");
            var jsonStr = JsonConvert.SerializeObject(quest, Formatting.Indented);
            using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                writer.Write(jsonStr);
            }
        }

        public static Quest GetQuest(QuestId questId)
        {
            return quests[questId];
        }

        public static IEnumerable<Quest> GetAllQuests()
        {
            return quests.Values;
        }

    }
}
