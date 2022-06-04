using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Items.ItemProperties
{
    internal class IncreaseAttributeVitalityItemProperty : ItemPropertyBase
    {
        public override string debugDescription => "Повышает стойкость";
        public override ItemPropertyType propertyType => ItemPropertyType.IncreaseAttributeVitality;
        public override bool isSupportLevelUp => false;

        public int value;

        public override void ApplyItemLevel(byte level) { }

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
