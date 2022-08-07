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
                CalculateFistsDamage(attacker);
                return;
            }
            _item = item;
            _damageInfo = dealDamageAbility.GetRandomValues();
        }

        private void CalculateFistsDamage(Player attacker)
        {
            var strength = ((PlayerStats)attacker.unitStats).attributeStrength;
            _damageInfo = new DamageInfo(strength * 3, 0, 0, 0); //По 3 очка физ. урона за единицу силы (сделал от балды, можно менять это значение)
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
            var itemName = _item != null ? _item.GetFullName(session) 
                : $"{Emojis.stats[Stat.PhysicalDamage]} {Localization.Get(session, "battle_attack_fists")}";

            sb.AppendLine($"<b>{itemName}:</b>");
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
