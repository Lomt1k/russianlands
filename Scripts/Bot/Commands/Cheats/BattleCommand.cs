using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Dialogs;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Items.ItemAbilities;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Managers;
using TextGameRPG.Scripts.GameCore.Units;
using TextGameRPG.Scripts.GameCore.Units.Mobs;

namespace TextGameRPG.Scripts.Bot.Commands.Cheats
{
    public class BattleCommand : CommandBase
    {
        public override CommandGroup commandGroup => CommandGroup.Cheat;

        public override async Task Execute(GameSession session, string[] args)
        {
            var currentBattle = GlobalManagers.battleManager?.GetCurrentBattle(session.player);
            if (currentBattle != null)
                return;

            await new SimpleDialog(session, "Select an enemy", true, new Dictionary<string, Func<Task>>()
            {
                { "Dummy", () => StartBattleWithDummy(session.player) },
                { "Shadow Copy", () => StartBattleWithShadowCopy(session.player) },
            })
            .Start().FastAwait();
        }

        private Task StartBattleWithDummy(Player player)
        {
            var mobData = new MobData();
            mobData.localizationKey = "Dummy";
            mobData.statsSettings.health = 100_000;
            mobData.mobAttacks.Add(new MobAttack());
            GlobalManagers.battleManager?.StartBattleWithMob(player, mobData);
            return Task.CompletedTask;
        }

        private Task StartBattleWithShadowCopy(Player player)
        {
            var playerStats = player.unitStats;
            var equipped = player.inventory.equipped;

            var mobData = new MobData();
            mobData.localizationKey = "Shadow";
            mobData.statsSettings = new MobStatsSettings()
            {
                level = player.level,
                health = playerStats.maxHP,
                physicalResist = playerStats.resistance[DamageType.Physical],
                fireResist = playerStats.resistance[DamageType.Fire],
                coldResist = playerStats.resistance[DamageType.Cold],
                lightningResist = playerStats.resistance[DamageType.Lightning],
            };

            var sword = equipped[ItemType.Sword];
            if (sword != null)
            {
                mobData.mobAttacks.Add(CreateMobAttackFromItem(player, sword));
            }
            else
            {
                var fistsAttack = new MobAttack()
                {
                    localizationKey = Emojis.StatPhysicalDamage + Localization.Get(player.session, "battle_attack_fists"),
                    minPhysicalDamage = 10,
                    maxPhysicalDamage = 10,
                };
                mobData.mobAttacks.Add(fistsAttack);
            }

            var bow = equipped[ItemType.Bow];
            if (bow != null)
            {
                mobData.mobAttacks.Add(CreateMobAttackFromItem(player, bow));
            }

            var stick = equipped[ItemType.Stick];
            if (stick != null)
            {
                mobData.mobAttacks.Add(CreateMobAttackFromItem(player, stick));
            }

            for (int i = 0; i < ItemType.Scroll.GetSlotsCount(); i++)
            {
                var scroll = equipped[ItemType.Scroll, i];
                if (scroll != null)
                {
                    mobData.mobAttacks.Add(CreateMobAttackFromItem(player, scroll));
                }
            }

            GlobalManagers.battleManager?.StartBattleWithMob(player, mobData);
            return Task.CompletedTask;
        }



        private MobAttack CreateMobAttackFromItem(Player player, InventoryItem item)
        {
            var dealDamage = (DealDamageAbility)item.data.ablitityByType[AbilityType.DealDamage];
            if (dealDamage == null)
            {
                return new MobAttack();
            }

            var attack = new MobAttack()
            {
                localizationKey = item.GetFullName(player.session).Bold(),
                minPhysicalDamage = dealDamage.minPhysicalDamage,
                maxPhysicalDamage = dealDamage.maxPhysicalDamage,
                minFireDamage = dealDamage.minFireDamage,
                maxFireDamage = dealDamage.maxFireDamage,
                minColdDamage = dealDamage.minColdDamage,
                maxColdDamage = dealDamage.maxColdDamage,
                minLightningDamage = dealDamage.minLightningDamage,
                maxLightningDamage = dealDamage.maxLightningDamage,
                manaCost = item.data.itemType == ItemType.Stick ? (sbyte)InventoryItem.requiredStickCharge : item.manaCost,
            };

            return attack;
        }

    }
}
