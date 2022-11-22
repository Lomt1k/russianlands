using System.Text;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Units;
using TextGameRPG.Scripts.GameCore.Units.Stats;
using TextGameRPG.Scripts.GameCore.Items.ItemAbilities;
using TextGameRPG.Scripts.TelegramBot.Sessions;
using TextGameRPG.Scripts.GameCore.Localizations;

namespace TextGameRPG.Scripts.TelegramBot.Managers.Battles.Actions
{
    public class PlayerAttackAction : IBattleAction
    {
        public BattleActionPriority priority => BattleActionPriority.OnAttack;
        public ActivationType activationType => ActivationType.OnSelectItem;

        private readonly InventoryItem? _item;
        private DamageInfo _damageInfo;
        private DamageInfo _resultDamage;

        /// <summary>
        /// Конструктор для атаки мечом / луком / посохом / свитком
        /// если item == null, то это атака кулаком
        /// </summary>
        public PlayerAttackAction(Player attacker, InventoryItem? item)
        {
            var dealDamageAbility = item?.data.ablitityByType[AbilityType.DealDamage] as DealDamageAbility;
            if (dealDamageAbility == null)
            {
                _damageInfo = new DamageInfo(10, 0, 0, 0); // Урон кулаком: 10 единиц (меньше любого оружия)
                return;
            }
            _item = item;
            _damageInfo = dealDamageAbility.GetRandomValues();
        }

        public void ApplyActionWithMineStats(UnitStats stats)
        {
        }

        public void ApplyActionWithEnemyStats(UnitStats stats)
        {
            _resultDamage = stats.TryDealDamage(_damageInfo);
        }

        public string GetLocalization(GameSession session)
        {
            var sb = new StringBuilder();
            var totalDamage = _resultDamage.GetTotalValue();
            var itemName = _item != null
                ? _item.GetFullName(session) 
                : $"{Emojis.stats[Stat.PhysicalDamage]} {Localization.Get(session, "battle_attack_fists")}";

            sb.AppendLine($"<b>{itemName}</b>");
            sb.Append(string.Format(Localization.Get(session, "battle_action_attack_description"), totalDamage));
            var resultDamageView = _resultDamage.GetCompactView();
            if (resultDamageView != null)
            {
                sb.AppendLine();
                sb.Append(resultDamageView);
            }
            return sb.ToString();
        }
    }
}
