using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Items.ItemProperties
{
    internal class IncreaseMaxHealthItemProperty : ItemPropertyBase
    {
        public override string debugDescription => "Увеличивает максимальный запас здоровья";
        public override PropertyType propertyType => PropertyType.IncreaseMaxHealth;
        public override bool isSupportLevelUp => false;

        public int value;

        public override void ApplyItemLevel(byte level) { }

        public override string ToString()
        {
            return $"{value} к максимальному запасу здоровья";
        }

        public override string GetView(GameSession session)
        {
            return string.Format(Localization.Localization.Get(session, "property_view_increase_max_health"), value);
        }

    }
}
