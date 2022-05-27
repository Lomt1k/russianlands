namespace TextGameRPG.Scripts.GameCore.Items.ItemProperties
{
    internal class FireDamageResistItemProperty : ItemPropertyBase
    {
        public override string debugDescription => "Сопротивление урону от огня";
        public override ItemPropertyType propertyType => ItemPropertyType.FireDamageResist;

        public int value;

        public FireDamageResistItemProperty(int value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return $"{debugDescription}: {value}";
        }
    }
}
