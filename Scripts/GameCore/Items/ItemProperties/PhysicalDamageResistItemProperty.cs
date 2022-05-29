﻿using TextGameRPG.Scripts.TelegramBot;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Items.ItemProperties
{
    internal class PhysicalDamageResistItemProperty : ItemPropertyBase
    {
        public override string debugDescription => "Сопротивление физическому урону";
        public override ItemPropertyType propertyType => ItemPropertyType.PhysicalDamageResist;

        public int value;

        public PhysicalDamageResistItemProperty(int value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return $"{debugDescription}: {value}";
        }

        public override string GetView(GameSession session)
        {
            return $"{Emojis.stats[Stat.PhysicalDamage]} {value}";
        }

    }
}
