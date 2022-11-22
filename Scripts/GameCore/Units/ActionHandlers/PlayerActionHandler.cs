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

        public List<IBattleAction> GetEveryTurnActions(BattleTurn battleTurn)
        {
            //TODO
            return new List<IBattleAction>();
        }

    }
}
