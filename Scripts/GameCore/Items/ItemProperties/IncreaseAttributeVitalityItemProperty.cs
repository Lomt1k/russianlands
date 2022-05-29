﻿using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Items.ItemProperties
{
    internal class IncreaseAttributeVitalityItemProperty : ItemPropertyBase
    {
        public override string debugDescription => "Повышает стойкость";
        public override ItemPropertyType propertyType => ItemPropertyType.IncreaseAttributeVitality;

        public int value;

        public IncreaseAttributeVitalityItemProperty(int value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return $"{value} к Стойкости";
        }

        public override string GetView(GameSession session)
        {
            return string.Format(Localization.Localization.Get(session, "property_view_increase_vitality"), value);
        }

    }
}
