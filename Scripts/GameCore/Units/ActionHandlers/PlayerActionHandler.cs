using System.Collections.Generic;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Dialogs.Battle;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Items.ItemAbilities;
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

        public async Task<IBattleAction?> GetAttackAction(BattleTurn battleTurn)
        {
            IBattleAction? actionBySelection = null;
            var dialog = new SelectBattleItemDialog(player.session, battleTurn, (item) =>
            {
                player.unitStats.OnUseItemInBattle(item);
                actionBySelection = new PlayerAttackAction(player, item);
            })
            .Start().ConfigureAwait(false);
            while (actionBySelection == null && battleTurn.isWaitingForActions)
            {
                await Task.Delay(500).ConfigureAwait(false);
            }

            return actionBySelection;
        }

        public bool TryAddShieldOnStartEnemyTurn(out DamageInfo damageInfo)
        {
            damageInfo = DamageInfo.Zero;

            var shield = player.inventory.equipped[ItemType.Shield];
            if (shield == null)
                return false;

            var blockAbility = shield.data.ablitityByType[AbilityType.BlockIncomingDamageEveryTurn] as BlockIncomingDamageEveryTurnAbility;
            if (blockAbility == null)
                return false;

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
