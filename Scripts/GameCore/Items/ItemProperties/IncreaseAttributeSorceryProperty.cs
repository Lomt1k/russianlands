using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Items.ItemProperties
{
    internal class IncreaseAttributeSorceryProperty : ItemPropertyBase
    {
        public override string debugDescription => "Повышает колдовство";
        public override PropertyType propertyType => PropertyType.IncreaseAttributeSorcery;
        public override bool isSupportLevelUp => false;

        public int value;

        public override void ApplyItemLevel(byte level) { }

        public override string ToString()
        {
            return $"{value} к Колдовству";
        }

        public override string GetView(GameSession session)
        {
            return string.Format(Localization.Localization.Get(session, "property_view_increase_sorcery"), value);
        }

    }
}
