namespace TextGameRPG.Scripts.GameCore.Items.ItemProperties
{
    internal class ColdDamageResistItemProperty : ItemPropertyBase
    {
        public override string debugDescription => "Сопротивление урону от холода";
        public override ItemPropertyType propertyType => ItemPropertyType.ColdDamageResist;

        public int value;

        public ColdDamageResistItemProperty(int value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return $"{debugDescription}: {value}";
        }
    }
}
