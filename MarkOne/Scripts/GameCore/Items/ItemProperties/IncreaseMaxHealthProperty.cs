﻿using MarkOne.Scripts.GameCore.Sessions;
using System.Collections.Generic;

namespace MarkOne.Scripts.GameCore.Items.ItemProperties;

public class IncreaseMaxHealthProperty : ItemPropertyBase
{
    public override string debugDescription => "Увеличивает максимальный запас здоровья";
    public override PropertyType propertyType => PropertyType.IncreaseMaxHealth;

    public int value;

    public override IEnumerable<ItemStatIcon> GetIcons(ItemType itemType)
    {
        yield return ItemStatIcon.IncreaseHealth;
    }

    public override string ToString()
    {
        return $"{value} к максимальному запасу здоровья";
    }

    public override string GetView(GameSession session)
    {
        return Localizations.Localization.Get(session, "property_view_increase_max_health", value);
    }

}
