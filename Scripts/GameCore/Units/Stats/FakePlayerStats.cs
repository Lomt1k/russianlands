using MarkOne.Scripts.GameCore.Buildings;
using MarkOne.Scripts.GameCore.Buildings.Data;
using MarkOne.Scripts.GameCore.Buildings.General;
using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Items.ItemProperties;

namespace MarkOne.Scripts.GameCore.Units.Stats;

public class FakePlayerStats : UnitStats
{
    public byte rageAbilityCounter;

    private readonly FakePlayer _fakePlayer;

    public FakePlayerStats(FakePlayer fakePlayer)
    {
        _fakePlayer = fakePlayer;
        Recalculate();
        SetFullHealth();
        SubscribeEvents();
    }

    public void SubscribeEvents()
    {
        _fakePlayer.equippedItems.onUpdateEquippedItems += Recalculate;
    }

    public override void OnStartBattle()
    {
        base.OnStartBattle();
        SetupArrowsAmount();
        rageAbilityCounter = 0;
    }

    private void SetupArrowsAmount()
    {
        var playerTownhall = _fakePlayer.townhallLevel;
        var resultArrows = 1;
        var tyrBuilding = (TyrBuilding)BuildingId.Tyr.GetBuilding();
        foreach (TyrLevelInfo tyrLevelInfo in tyrBuilding.buildingData.levels)
        {
            if (tyrLevelInfo.requiredTownHall <= playerTownhall)
            {
                resultArrows = tyrLevelInfo.arrowsAmount;
            }
        }
        currentArrows = (byte)resultArrows;
    }

    public override void OnStartMineTurn()
    {
        base.OnStartMineTurn();
        var equippedStick = _fakePlayer.equippedItems[ItemType.Stick];
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
        maxHP = PlayerHealthByLevel.Get(_fakePlayer.level);
        resistance = DamageInfo.Zero;
    }

    private void ApplyItemProperties()
    {
        var equippedItems = _fakePlayer.equippedItems.allEquipped;
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
