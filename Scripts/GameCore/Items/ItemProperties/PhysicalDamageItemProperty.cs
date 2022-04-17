namespace TextGameRPG.Scripts.GameCore.Items.ItemProperties
{
    class PhysicalDamageItemProperty : ItemPropertyBase
    {
        public override string debugDescription => "Физический Урон";
        public override ItemPropertyType propertyType => ItemPropertyType.PhysicalDamage;

        public int physicalDamage { get; }

        public PhysicalDamageItemProperty(int damage)
        {
            physicalDamage = damage;
        }
    }
}
