namespace TextGameRPG.Scripts.GameCore.Items.ItemProperties
{
    internal class IncreaseAttributeStrengthItemProperty : ItemPropertyBase
    {
        public override string debugDescription => "Повышает силу";
        public override ItemPropertyType propertyType => ItemPropertyType.IncreaseAttributeStrength;

        public int value;

        public IncreaseAttributeStrengthItemProperty(int value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            var prefix = value >= 0 ? "+" : string.Empty;
            return $"{prefix}{value} к Силе";
        }
    }
}
