using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Items.ItemProperties
{
    internal class IncreaseMaxManaProperty : ItemPropertyBase
    {
        public override string debugDescription => "Увеличивает максимальный запас маны";
        public override PropertyType propertyType => PropertyType.IncreaseMaxMana;
        public override bool isSupportLevelUp => false;

        public int value;

        public override void ApplyItemLevel(byte level) { }

        public override string ToString()
        {
            return $"{value} к максимальному запасу маны";
        }

        public override string GetView(GameSession session)
        {
            return string.Format(Localizations.Localization.Get(session, "property_view_increase_max_mana"), value);
        }

    }
}
