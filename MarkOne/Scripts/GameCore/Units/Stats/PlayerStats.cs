using System;
using System.Linq;
using MarkOne.Scripts.GameCore.Buildings;
using MarkOne.Scripts.GameCore.Buildings.General;
using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Items.ItemProperties;

namespace MarkOne.Scripts.GameCore.Units.Stats;

public class PlayerStats : UnitStats, IStatsForUnitWithItems
{
    private readonly Player _player;

    public byte rageAbilityCounter { get; set; }
    public byte availablePotions { get; set; }

    public PlayerStats(Player player)
    {
        _player = player;
        Recalculate();
        SetFullHealth();
        SubscribeEvents();
    }

    public void SubscribeEvents()
    {
        _player.inventory.equipped.onUpdateEquippedItems += Recalculate;
    }

    public override void OnStartBattle()
    {
        base.OnStartBattle();
        SetupArrowsAmount();
        SetupPotionsAmount();
        rageAbilityCounter = 0;
    }

    private void SetupArrowsAmount()
    {
        var buildingsData = _player.session.profile.buildingsData;
        var tyrBuilding = (TyrBuilding)BuildingId.Tyr.GetBuilding();
        currentArrows = tyrBuilding.GetArrowsAmount(_player.session, buildingsData);
    }

    public void SetupPotionsAmount()
    {
        var alchemyLab = (AlchemyLabBuilding)BuildingId.AlchemyLab.GetBuilding();
        var maxPotionsInBattle = alchemyLab.GetCurrentPotionsInBattle(_player.session.profile.buildingsData);
        var dtNow = DateTime.UtcNow.Ticks;
        var totalPotions = _player.potions.Where(x => x.preparationTime < dtNow).Count();
        availablePotions = (byte)Math.Min(maxPotionsInBattle, totalPotions);
    }

    public override void OnStartMineTurn()
    {
        base.OnStartMineTurn();
        var equippedStick = _player.inventory.equipped[Items.ItemType.Stick];
        if (equippedStick != null && currentStickCharge < InventoryItem.requiredStickCharge)
        {
            currentStickCharge++;
        }
    }

    public void Recalculate()
    {
        CalculateBaseValues();
        ApplyItemProperties();

        RecalculateCurrentHP();
    }

    private void CalculateBaseValues()
    {
        var profileData = _player.session.profile.data;

        maxHP = PlayerHealthByLevel.Get(profileData.level);
        resistance = DamageInfo.Zero;
    }

    private void ApplyItemProperties()
    {
        var equippedItems = _player.inventory.equipped.allEquipped;
        foreach (var item in equippedItems)
        {
            foreach (var property in item.data.properties)
            {
                ApplyProperty(property);
            }
        }
    }

    private void ApplyProperty(ItemPropertyBase property)
    {
        switch (property)
        {
            case DamageResistProperty resistProperty:
                resistance += resistProperty.GetValues();
                break;
            case IncreaseMaxHealthProperty increaseMaxHealth:
                maxHP += increaseMaxHealth.value;
                break;
        }
    }

}
