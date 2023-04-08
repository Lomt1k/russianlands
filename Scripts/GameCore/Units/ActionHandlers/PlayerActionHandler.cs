using System.Collections.Generic;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Dialogs.Battle;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Items.ItemAbilities;
using TextGameRPG.Scripts.GameCore.Items.ItemAbilities.Keywords;
using TextGameRPG.Scripts.GameCore.Managers.Battles;
using TextGameRPG.Scripts.GameCore.Managers.Battles.Actions;
using TextGameRPG.Scripts.Utils;

namespace TextGameRPG.Scripts.GameCore.Units.ActionHandlers
{
    public class PlayerActionHandler : IBattleActionHandler
    {
        public Player player { get; }

        public PlayerActionHandler(Player player)
        {
            this.player = player;
        }

        public async Task<List<IBattleAction>> GetActionsBySelectedItem(BattleTurn battleTurn)
        {
            var result = new List<IBattleAction>();

            bool isActionsReady = false;
            var dialog = new SelectBattleItemDialog(player.session, battleTurn, (item) =>
            {
                player.unitStats.OnUseItemInBattle(item);
                var generalAttackAction = new PlayerAttackAction(player, item);
                result.Add(generalAttackAction);
                ItemKeywordActionsHandler.HandleKeywords(battleTurn, item, ref generalAttackAction, ref result);
                generalAttackAction.damageInfo += player.unitStats.statEffects.GetExtraDamageAndRemoveEffects();
                isActionsReady = true;
            })
            .Start().FastAwait();

            while (battleTurn.isWaitingForActions && !isActionsReady)
            {
                await Task.Delay(500).FastAwait();
            }

            return result;
        }

        public bool TryAddShieldOnEnemyTurn(out DamageInfo damageInfo)
        {
            damageInfo = DamageInfo.Zero;

            var shield = player.inventory.equipped[ItemType.Shield];
            if (shield == null)
                return false;

            if (!shield.data.ablitityByType.TryGetValue(AbilityType.BlockIncomingDamageEveryTurn, out var shieldBlockAbility))
                return false;

            var blockAbility = (BlockIncomingDamageEveryTurnAbility)shieldBlockAbility;
            var success = Randomizer.TryPercentage(blockAbility.chanceToSuccessPercentage);
            if (!success)
                return false;

            damageInfo = new DamageInfo(
                physicalDamage: blockAbility.physicalDamage,
                fireDamage: blockAbility.fireDamage,
                coldDamage: blockAbility.coldDamage,
                lightningDamage: blockAbility.lightningDamage);
            return true;
        }

        public bool TryAddSwordBlockOnEnemyTurn(out DamageInfo damageInfo)
        {
            damageInfo = DamageInfo.Zero;

            var sword = player.inventory.equipped[ItemType.Sword];
            if (sword == null)
                return false;

            if (!sword.data.ablitityByType.TryGetValue(AbilityType.SwordBlockEveryTurnKeyword, out var swordBlockAbility))
                return false;

            var blockAbility = (SwordBlockKeywordAbility)swordBlockAbility;
            var success = Randomizer.TryPercentage(blockAbility.chanceToSuccessPercentage);
            if (!success)
                return false;

            damageInfo = new DamageInfo(
                physicalDamage: blockAbility.physicalDamage,
                fireDamage: blockAbility.fireDamage,
                coldDamage: blockAbility.coldDamage,
                lightningDamage: blockAbility.lightningDamage);
            return true;
        }

        public IEnumerable<IBattleAction> GetEveryTurnActions(BattleTurn battleTurn)
        {
            var allEquipped = player.inventory.equipped.allEquipped;

            // --- Restore Health
            var restoreHealthAction = new RestoreHealthAction();
            foreach (var item in allEquipped)
            {
                if (item.data.ablitityByType.TryGetValue(AbilityType.RestoreHealthEveryTurn, out var ability))
                {
                    var restoreHealthAbility = (RestoreHealthEveryTurnAbility)ability;
                    if (restoreHealthAbility.TryChance())
                    {
                        restoreHealthAction.Add(item, restoreHealthAbility.healthValue);
                    }
                }
            }
            if (restoreHealthAction.healthAmount > 0)
            {
                yield return restoreHealthAction;
            }

            // --- Add Mana
            var addManaAction = new AddManaAction();
            foreach (var item in allEquipped)
            {
                if (item.data.ablitityByType.TryGetValue(AbilityType.AddManaEveryTurn, out var ability))
                {
                    var addManaAbility = (AddManaEveryTurnAbility)ability;
                    if (addManaAbility.TryChance())
                    {
                        addManaAction.Add(item, addManaAbility.manaValue);
                    }
                }
            }
            if (addManaAction.manaAmount > 0)
            {
                yield return addManaAction;
            }

            // ...
        }

    }
}
