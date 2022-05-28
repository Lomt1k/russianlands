namespace TextGameRPG.Scripts.GameCore.Items.ItemProperties
{
    internal class IncreaseMaxHealthItemProperty : ItemPropertyBase
    {
        public override string debugDescription => "Увеличивает максимальный запас здоровья";
        public override ItemPropertyType propertyType => ItemPropertyType.IncreaseMaxHealth;

        public int value;

        public IncreaseMaxHealthItemProperty(int value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            var prefix = value >= 0 ? "+" : string.Empty;
            return $"{prefix}{value} к максимальному запасу здоровью";
        }
    }
}
