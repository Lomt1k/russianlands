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
        private static Dictionary<QuestType, QuestBase> quests = new Dictionary<QuestType, QuestBase>();

        public static void LoadAll(GameDataLoaderViewModel loaderVM, string gamedataPath)
        {
            quests.Clear();
            string questsFolder = Path.Combine(gamedataPath, "Quests");
            if (!Directory.Exists(questsFolder))
            {
                Directory.CreateDirectory(questsFolder);
            }

            var allQuests = Enum.GetValues(typeof(QuestType));
            foreach (QuestType questType in allQuests)
            {
                if (questType == QuestType.None)
                    continue;

                LoadQuest(loaderVM, questsFolder, questType);
            }
        }

        public static void LoadQuest(GameDataLoaderViewModel loaderVM, string questsFolderPath, QuestType questType)
        {
            var fileName = $"{questType}.json";
            loaderVM.AddNextState($"Loading {fileName}...");
            var filePath = Path.Combine(questsFolderPath, fileName);

            if (!File.Exists(filePath))
            {
                CreateEmptyQuestFile(filePath, questType);
                loaderVM.AddNextState($"Created new quest file with name '{fileName}'");
            }

            using (StreamReader reader = new StreamReader(filePath, Encoding.UTF8))
            {
                var jsonStr = reader.ReadToEnd();
                var quest = JsonConvert.DeserializeObject<QuestBase>(jsonStr);
                quests.Add(quest.questType, quest);
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
