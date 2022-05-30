using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Items.ItemProperties
{
    internal class IncreaseAttributeLuckItemProperty : ItemPropertyBase
    {
        public override string debugDescription => "Повышает удачу";
        public override ItemPropertyType propertyType => ItemPropertyType.IncreaseAttributeLuck;
        public override bool isSupportLevelUp => false;

        public int value;

        public IncreaseAttributeLuckItemProperty(int value)
        {
            this.value = value;
        }

        public override void ApplyItemLevel(byte level) { }

        public override string ToString()
        {
            return $"{value} к Удаче";
        }

        public override string GetView(GameSession session)
        {
            return string.Format(Localization.Localization.Get(session, "property_view_increase_luck"), value);
        }

    }
}
