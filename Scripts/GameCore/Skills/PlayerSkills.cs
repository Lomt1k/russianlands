using System;
using System.Collections.Generic;
using System.Text;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Buildings;
using MarkOne.Scripts.GameCore.Buildings.General;
using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Sessions;
using MarkOne.Scripts.GameCore.Units;
using MarkOne.Scripts.GameCore.Units.Stats;

namespace MarkOne.Scripts.GameCore.Skills;

public class PlayerSkills
{
    private static readonly SkillsDictionary skillsDictionary = new SkillsDictionary();

    private readonly Player _player;
    private readonly ProfileData _profileData;

    public PlayerSkills(Player player)
    {
        _player = player;
        _profileData = player.session.profile.data;
        ApplySkillsOnInit();
    }

    private void ApplySkillsOnInit()
    {
        foreach (var itemType in GetAllSkillTypes())
        {
            var value = GetValue(itemType);
            if (value > 0)
            {
                _player.inventory.ApplyPlayerSkills(itemType, value);
            }
        }
    }

    /// <returns>Уровень прокачки всех навыков в компактном отображении</returns>
    public string GetShortView(GameSession sessionToSend)
    {
        var sb = new StringBuilder();
        sb.AppendLine(Localization.Get(sessionToSend, "unit_view_skills_header"));
        var i = 0;
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
        return skillsDictionary.ContainsKey(itemType) ? skillsDictionary[itemType].GetValue(_profileData) : (byte)0;
    }

    /// <summary>
    /// Устанавливает конкретное значение навыку
    /// </summary>
    public void SetValue(ItemType itemType, byte value)
    {
        skillsDictionary[itemType].SetValue(_profileData, value);
        _player.inventory.ApplyPlayerSkills(itemType, value);
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

        _player.inventory.ApplyPlayerSkills(itemType, skill.GetValue(_profileData));
        RecalculateStatsAfterSkillChange();
    }

    /// <returns>Ресурсы, требуемые для прокачки навыка</returns>
    public ResourceId[] GetRequiredFruits(ItemType itemType)
    {
        return skillsDictionary[itemType].requiredFruits;
    }

    /// <returns>Максимальный доступный уровень для всех навыков</returns>
    public byte GetSkillLimit()
    {
        var elixirWorkshop = (ElixirWorkshopBuilding)BuildingId.ElixirWorkshop.GetBuilding();
        var maxSkillLevel = elixirWorkshop.GetCurrentMaxSkillLevel(_player.session.profile.buildingsData);
        return (byte)maxSkillLevel;
    }

    /// <returns>Все виды навыков</returns>
    public static IEnumerable<ItemType> GetAllSkillTypes()
    {
        return skillsDictionary.GetAllSkillTypes();
    }

    /// <returns>Средний уровень навыков</returns>
    public byte GetAverageSkillLevel()
    {
        var sum = 0;
        var count = 0;
        foreach (var skill in GetAllSkillTypes())
        {
            sum += GetValue(skill);
            count++;
        }
        var result = Math.Round((float)sum / count);
        return (byte)result;
    }

    public Dictionary<ItemType,byte> GetAllSkills()
    {
        var result = new Dictionary<ItemType,byte>();
        foreach (var itemType in GetAllSkillTypes())
        {
            result[itemType] = GetValue(itemType);
        }
        return result;
    }

    private void RecalculateStatsAfterSkillChange()
    {
        var playerStats = (PlayerStats)_player.unitStats;
        playerStats.Recalculate();
    }

}
