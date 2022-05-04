namespace TextGameRPG.Scripts.GameCore.Items.ItemProperties
{
    class PhysicalDamageItemProperty : ItemPropertyBase
    {
        public override string debugDescription => "Физический Урон";
        public override ItemPropertyType propertyType => ItemPropertyType.PhysicalDamage;

        public int physicalDamage;

        public PhysicalDamageItemProperty(int damage)
        {
            physicalDamage = damage;
        }

        public override string ToString()
        {
            return $"{debugDescription}: {physicalDamage}";
        }
    }
}
