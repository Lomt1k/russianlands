namespace TextGameRPG.Scripts.GameCore.Items.ItemProperties
{
    internal class IncreaseAttributeLuckItemProperty : ItemPropertyBase
    {
        public override string debugDescription => "Повышает удачу";
        public override ItemPropertyType propertyType => ItemPropertyType.IncreaseAttributeLuck;

        public int value;

        public IncreaseAttributeLuckItemProperty(int value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            var prefix = value >= 0 ? "+" : string.Empty;
            return $"{prefix}{value} к Удаче";
        }
    }
}
