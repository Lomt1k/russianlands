using System;
using System.Collections.Generic;
using System.Text;
using TextGameRPG.Scripts.Bot;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Buildings;
using TextGameRPG.Scripts.GameCore.Buildings.General;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.GameCore.Units.Stats;

namespace TextGameRPG.Scripts.GameCore.Skills
{
    public class PlayerSkills
    {
        private static SkillsDictionary skillsDictionary = new SkillsDictionary();

        private GameSession _session;
        private ProfileData _profileData;

        public PlayerSkills(GameSession session)
        {
            _session = session;
            _profileData = session.profile.data;
            ApplySkillsOnInit();
        }

        private void ApplySkillsOnInit()
        {
            foreach (var itemType in GetAllSkillTypes())
            {
                var value = GetValue(itemType);
                if (value > 0)
                {
                    _session.player.inventory.ApplyPlayerSkills(itemType, value);
                }
            }
        }

        /// <returns>Уровень прокачки всех навыков в компактном отображении</returns>
        public string GetShortView()
        {
            var sb = new StringBuilder();
            sb.AppendLine(Localization.Get(_session, "dialog_skills_your_skills_header"));
            int i = 0;
            foreach (var itemType in GetAllSkillTypes())
            {
                if (i > 0)
                {
                    sb.Append(i % 4 == 0 ? Environment.NewLine : Emojis.middleSpace);
                }
                sb.Append(itemType.GetEmoji() + GetValue(itemType).ToString());
                i++;
            }
            return sb.ToString();
        }

        /// <returns>Достигнут ли максимальный уровнь всех навыков</returns>
        public bool IsAllSkillsMax()
        {
            foreach (var itemType in GetAllSkillTypes())
            {
                if (!IsMaxLevel(itemType))
                {
                    return false;
                }
            }
            return true;
        }

        /// <returns>Достигнут ли максимальный уровнь навыка, доступный на данный момент</returns>
        public bool IsMaxLevel(ItemType itemType)
        {
            return GetValue(itemType) >= GetSkillLimit();
        }

        /// <returns>Уровень навыка для соответствующего типа предметов</returns>
        public byte GetValue(ItemType itemType)
        {
            return skillsDictionary[itemType].GetValue(_profileData);
        }

        /// <summary>
        /// Устанавливает конкретное значение навыку
        /// </summary>
        public void SetValue(ItemType itemType, byte value)
        {
            skillsDictionary[itemType].SetValue(_profileData, value);
            _session.player.inventory.ApplyPlayerSkills(itemType, value);
            RecalculateStatsAfterSkillChange();
        }

        /// <summary>
        /// Добавляет значение к навыку
        /// </summary>
        public void AddValue(ItemType itemType, byte value)
        {
            var skill = skillsDictionary[itemType];
            var currentValue = skill.GetValue(_profileData);

            var canBeAdded = byte.MaxValue - currentValue;
            var reallyAdded = value > canBeAdded ? (byte)canBeAdded : value;
            skill.AddValue(_profileData, reallyAdded);

            _session.player.inventory.ApplyPlayerSkills(itemType, skill.GetValue(_profileData));
            RecalculateStatsAfterSkillChange();
        }

        /// <returns>Ресурсы, требуемые для прокачки навыка</returns>
        public ResourceType[] GetRequiredFruits(ItemType itemType)
        {
            return skillsDictionary[itemType].requiredFruits;
        }

        /// <returns>Максимальный доступный уровень для всех навыков</returns>
        public byte GetSkillLimit()
        {
            var elixirWorkshop = (ElixirWorkshopBuilding)BuildingType.ElixirWorkshop.GetBuilding();
            var maxSkillLevel = elixirWorkshop.GetCurrentMaxSkillLevel(_session.profile.buildingsData);
            return (byte)maxSkillLevel;
        }

        /// <returns>Все виды навыков</returns>
        public static IEnumerable<ItemType> GetAllSkillTypes()
        {
            return skillsDictionary.GetAllSkillTypes();
        }

        private void RecalculateStatsAfterSkillChange()
        {
            var playerStats = (PlayerStats)_session.player.unitStats;
            playerStats.Recalculate();
        }

    }
}
