namespace TextGameRPG.Scripts.GameCore.Items.ItemProperties
{
    internal class IncreaseAttributeVitalityItemProperty : ItemPropertyBase
    {
        public override string debugDescription => "Повышает стойкость";
        public override ItemPropertyType propertyType => ItemPropertyType.IncreaseAttributeVitality;

        public int value;

        public IncreaseAttributeVitalityItemProperty(int value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            var prefix = value >= 0 ? "+" : string.Empty;
            return $"{prefix}{value} к Стойкости";
        }
    }
}
