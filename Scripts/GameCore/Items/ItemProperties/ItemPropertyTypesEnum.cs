namespace TextGameRPG.Scripts.GameCore.Items.ItemProperties
{
    public enum ItemPropertyType
    {
        None = 0,
        DealDamage = 1,
        DamageResist = 2,
        IncreaseAttributeStrength = 3,
        IncreaseAttributeVitality = 4,
        IncreaseAttributeSorcery = 5,
        IncreaseAttributeLuck = 6,
        IncreaseMaxHealth = 7,
        IncreaseMaxMana = 8
    }

    public enum DamageType { Physical, Fire, Cold, Lightning }

}
