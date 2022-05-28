namespace TextGameRPG.Scripts.GameCore.Items.ItemProperties
{
    internal class IncreaseMaxManaProperty : ItemPropertyBase
    {
        public override string debugDescription => "Увеличивает максимальный запас маны";
        public override ItemPropertyType propertyType => ItemPropertyType.IncreaseMaxMana;

        public int value;

        public IncreaseMaxManaProperty(int value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            var prefix = value >= 0 ? "+" : string.Empty;
            return $"{prefix}{value} к максимальному запасу маны";
        }
    }
}
