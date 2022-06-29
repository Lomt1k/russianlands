using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Items.ItemProperties
{
    public class IncreaseAttributeStrengthProperty : ItemPropertyBase
    {
        public override string debugDescription => "Повышает силу";
        public override PropertyType propertyType => PropertyType.IncreaseAttributeStrength;
        public override bool isSupportLevelUp => false;

        public int value;

        public override void ApplyItemLevel(byte level) { }

        public override string ToString()
        {
            return $"{value} к Силе";
        }

        public override string GetView(GameSession session)
        {
            return string.Format(Localizations.Localization.Get(session, "property_view_increase_strength"), value);
        }

    }
}
