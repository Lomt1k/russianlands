﻿using System;
using System.Text;
using TextGameRPG.Scripts.GameCore.Buildings;
using TextGameRPG.Scripts.GameCore.Buildings.General;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Items.ItemProperties;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.TelegramBot;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Units.Stats
{
    public class PlayerStats : UnitStats
    {
        public const int DEFAULT_HEALTH = 100;
        public const int HEALTH_PER_LEVEL = 20;

        private Player _player;

        // Attributes
        public int attributeStrength { get; protected set; }
        public int attributeVitality { get; protected set; }
        public int attributeSorcery { get; protected set; }
        public int attributeLuck { get; protected set; }

        public PlayerStats(Player player)
        {
            _player = player;
            SubscribeEvents();
            Recalculate();

            SetFullHealth();
        }

        public void SubscribeEvents()
        {
            _player.inventory.equipped.onUpdateEquippedItems += Recalculate;
        }

        public override void OnStartBattle()
        {
            base.OnStartBattle();
            SetupArrowsAmount();
        }

        private void SetupArrowsAmount()
        {
            var buildingsData = _player.session.profile.buildingsData;
            var tyrBuilding = (TyrBuilding)BuildingType.Tyr.GetBuilding();
            currentArrows = tyrBuilding.GetArrowsAmount(_player.session, buildingsData);
        }

        public override void OnStartMineTurn()
        {
            base.OnStartMineTurn();
            var equippedStick = _player.inventory.equipped[Items.ItemType.Stick];
            if (equippedStick != null && currentStickCharge < equippedStick.data.requiredCharge)
            {
                currentStickCharge++;
            }
        }

        public void Recalculate()
        {
            CalculateBaseValues();
            ApplyItemProperties();
            ApplyAttributes();

            RecalculateCurrentHP();
        }

        private void CalculateBaseValues()
        {
            var profileData = _player.session.profile.data;

            var defaultHealth = DEFAULT_HEALTH + HEALTH_PER_LEVEL * (profileData.level - 1);
            maxHP = defaultHealth;

            attributeStrength = 1;
            attributeVitality = 1;
            attributeSorcery = 1;
            attributeLuck = 1;
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
                    var tempResist = resistance;
                    tempResist[DamageType.Physical] += resistProperty.physicalDamage;
                    tempResist[DamageType.Fire] += resistProperty.fireDamage;
                    tempResist[DamageType.Cold] += resistProperty.coldDamage;
                    tempResist[DamageType.Lightning] += resistProperty.lightningDamage;
                    resistance = tempResist;
                    break;
                case IncreaseAttributeStrengthProperty increaseStrength:
                    this.attributeStrength += increaseStrength.value;
                    break;
                case IncreaseAttributeVitalityProperty increaseVitality:
                    this.attributeVitality += increaseVitality.value;
                    break;
                case IncreaseAttributeSorceryProperty increaseSorcery:
                    this.attributeSorcery += increaseSorcery.value;
                    break;
                case IncreaseAttributeLuckProperty increaseLuck:
                    this.attributeLuck += increaseLuck.value;
                    break;
                case IncreaseMaxHealthProperty increaseMaxHealth:
                    this.maxHP += increaseMaxHealth.value;
                    break;
            }
        }

        private void ApplyAttributes()
        {
            // vitality
            var tempResist = resistance;
            foreach (DamageType damageType in Enum.GetValues(typeof(DamageType)))
            {
                var bonusPerVitalityPoint = (float)tempResist[damageType] / 500; // +0.02% за очко стойкости
                tempResist[damageType] += (int)(bonusPerVitalityPoint * attributeVitality);
            }
            resistance = tempResist;
        }

    }
}
