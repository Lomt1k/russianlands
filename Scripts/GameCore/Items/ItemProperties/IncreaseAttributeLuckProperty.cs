using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Items.ItemProperties
{
    public class IncreaseAttributeLuckProperty : ItemPropertyBase
    {
        public override string debugDescription => "Повышает удачу";
        public override PropertyType propertyType => PropertyType.IncreaseAttributeLuck;
        public override bool isSupportLevelUp => false;

        public int value;

        public override void ApplyItemLevel(byte level) { }

        public override string ToString()
        {
            return $"{value} к Удаче";
        }

        public override string GetView(GameSession session)
        {
            return string.Format(Localizations.Localization.Get(session, "property_view_increase_luck"), value);
        }

    }
}
