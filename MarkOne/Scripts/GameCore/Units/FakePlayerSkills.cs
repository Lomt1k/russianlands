using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Sessions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarkOne.Scripts.GameCore.Units;

public class FakePlayerSkills
{
    private Dictionary<ItemType, byte> _skillsByType = new();

    public FakePlayerSkills(FakePlayer fakePlayer, Dictionary<ItemType, byte> skillsByType)
    {
        _skillsByType = skillsByType;
        ApplySkillsOnInit(fakePlayer);
    }

    private void ApplySkillsOnInit(FakePlayer fakePlayer)
    {
        foreach (var item in fakePlayer.equippedItems.allEquipped)
        {
            var skillLevel = GetValue(item.data.itemType);
            item.RecalculateDataWithPlayerSkills(skillLevel);
        }
    }

    /// <returns>Уровень прокачки всех навыков в компактном отображении</returns>
    public string GetShortView(GameSession sessionToSend)
    {
        var sb = new StringBuilder();
        sb.AppendLine(Localization.Get(sessionToSend, "unit_view_skills_header"));
        var i = 0;
        foreach (ItemType itemType in Enum.GetValues<ItemType>())
        {
            if (!_skillsByType.ContainsKey(itemType))
            {
                continue;
            }

            if (i > 0)
            {
                sb.Append(i % 4 == 0 ? Environment.NewLine : Emojis.middleSpace);
            }
            sb.Append(itemType.GetEmoji() + GetValue(itemType).ToString());
            i++;
        }
        return sb.ToString();
    }

    /// <returns>Уровень навыка для соответствующего типа предметов</returns>
    public byte GetValue(ItemType itemType)
    {
        return _skillsByType.ContainsKey(itemType) ? _skillsByType[itemType] : (byte)0;
    }

}
