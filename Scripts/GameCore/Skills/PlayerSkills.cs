using System.Collections.Generic;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Buildings;
using TextGameRPG.Scripts.GameCore.Buildings.General;
using TextGameRPG.Scripts.GameCore.Items;

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
            return SkillsDictionary.GetAllSkillTypes();
        }

    }
}
