﻿using TextGameRPG.Scripts.TelegramBot;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Items.ItemProperties
{
    internal class LightningDamageResistItemProperty : ItemPropertyBase
    {
        public override string debugDescription => "Сопротивление урону от молнии";
        public override ItemPropertyType propertyType => ItemPropertyType.LightningDamageResist;

        public int value;

        public LightningDamageResistItemProperty(int value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return $"{debugDescription}: {value}";
        }

        public override string GetView(GameSession session)
        {
            return $"{Emojis.stats[Stat.LightningDamage]} {value}";
        }

    }
}
