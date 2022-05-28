namespace TextGameRPG.Scripts.GameCore.Items.ItemProperties
{
    internal class IncreaseAttributeSorceryItemProperty : ItemPropertyBase
    {
        public override string debugDescription => "Повышает колдовство";
        public override ItemPropertyType propertyType => ItemPropertyType.IncreaseAttributeSorcery;

        public int value;

        public IncreaseAttributeSorceryItemProperty(int value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            var prefix = value >= 0 ? "+" : string.Empty;
            return $"{prefix}{value} к Колдовству";
        }
    }
}
