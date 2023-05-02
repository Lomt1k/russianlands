namespace MarkOne.Scripts.GameCore.Items.ItemProperties;

public enum PropertyType : byte
{
    None = 0,
    DamageResist = 1,
    IncreaseMaxHealth = 2,
}

public static class PropertyTypeExtensions
{
    public static ViewPriority GetPriority(this PropertyType type)
    {
        return type switch
        {
            PropertyType.DamageResist => ViewPriority.GeneralInfo,
            _ => ViewPriority.Passive
        };
    }
}
