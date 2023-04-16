using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Items;
using Telegram.Bot.Types.ReplyMarkups;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Services.Battles;
using System.Text;
using TextGameRPG.Scripts.GameCore.Items.ItemAbilities;
using TextGameRPG.Scripts.GameCore.Units.Stats;
using TextGameRPG.Scripts.GameCore.Inventory;
using System.Linq;
using TextGameRPG.Scripts.GameCore.Potions;
using TextGameRPG.Scripts.GameCore.Units;

namespace TextGameRPG.Scripts.Bot.Dialogs.Battle
{
    public class SelectBattleItemDialog : DialogBase
    {
        private Action<InventoryItem?> _selectedAttackItemCallback;
        private BattleTurn _battleTurn;
        private bool _isActionAlreadySelected;
        private bool _isPotionAlreadySelected;

        public PlayerStats playerStats => (PlayerStats)session.player.unitStats;
        public EquippedItems equipped => session.player.inventory.equipped;

        public SelectBattleItemDialog(GameSession _session, BattleTurn battleTurn, Action<InventoryItem?> callback) : base(_session)
        {
            _battleTurn = battleTurn;
            _selectedAttackItemCallback = callback;
        }

        public override async Task Start()
        {
            ClearButtons();
            var keyboardRows = new List<List<KeyboardButton>>();
            AppendSingleSlotItems(ref keyboardRows);
            AppendMultiSlotItems(ref keyboardRows);
            var keyboard = new ReplyKeyboardMarkup(keyboardRows);

            var sb = new StringBuilder();
            sb.Append(Emojis.ButtonBattle + Localization.Get(session, "battle_mine_turn_start"));
            if (equipped.HasItem(ItemType.Bow) && playerStats.currentArrows > 0)
            {
                sb.Append(' ' + Emojis.StatArrows.ToString() + playerStats.currentArrows.ToString());
            }
            if (playerStats.availablePotions > 0)
            {
                sb.Append(' ' + Emojis.ButtonPotions.ToString() + playerStats.availablePotions.ToString());
            }
            if (equipped.HasItem(ItemType.Scroll))
            {
                sb.Append(' ' + Emojis.StatMana.ToString() + playerStats.currentMana.ToString());
            }
            sb.AppendLine();

            if (_battleTurn.isLastChance)
            {
                sb.AppendLine();
                sb.AppendLine(Emojis.ElementBrokenHeart + Localization.Get(session, "battle_mine_turn_start_last_chance"));
            }

            bool additionalLineUsed = false;
            var equippedStick = session.player.inventory.equipped[ItemType.Stick];
            if (equippedStick != null)
            {
                var currentCharge = playerStats.currentStickCharge;
                var requiredCharge = InventoryItem.requiredStickCharge;
                if (currentCharge < requiredCharge)
                {
                    TryAppendAdditionalLine();
                    var localization = Localization.Get(session, "battle_mine_turn_stick_charge", currentCharge, requiredCharge);
                    sb.AppendLine(Emojis.ItemStick + localization);
                }
            }

            if (playerStats.rageAbilityCounter > 0)
            {
                TryAppendAdditionalLine();
                sb.AppendLine(Emojis.StatKeywordRage + Localization.Get(session, "battle_action_rage_header") +
                    $" {playerStats.rageAbilityCounter} / 3");
            }

            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "battle_mine_turn_start_select_item"));

            await SendDialogMessage(sb, keyboard).FastAwait();

            void TryAppendAdditionalLine()
            {
                if (!additionalLineUsed)
                {
                    additionalLineUsed = true;
                    sb.AppendLine();
                }
            }
        }

        public void AppendSingleSlotItems(ref List<List<KeyboardButton>> keyboardRows)
        {
            var swordItem = equipped[ItemType.Sword];
            var swordAttackText = swordItem != null
                ? swordItem.GetFullName(session) 
                : Emojis.StatPhysicalDamage + Localization.Get(session, "battle_attack_fists");
            RegisterButton(swordAttackText, () => OnCategorySelected(ItemType.Sword));
            keyboardRows.Add(new List<KeyboardButton> { swordAttackText });

            var bowItem = equipped[ItemType.Bow];
            if (bowItem != null && playerStats.currentArrows > 0)
            {
                var bowAttakText = bowItem.GetFullName(session);
                RegisterButton(bowAttakText, () => OnCategorySelected(ItemType.Bow));
                keyboardRows.Add(new List<KeyboardButton> { bowAttakText });
            }

            var stickItem = equipped[ItemType.Stick];
            if (stickItem != null && playerStats.currentStickCharge >= InventoryItem.requiredStickCharge)
            {
                var stickAttackText = stickItem.GetFullName(session);
                RegisterButton(stickAttackText, () => OnCategorySelected(ItemType.Stick));
                keyboardRows.Add(new List<KeyboardButton> { stickAttackText });
            }
        }

        public void AppendMultiSlotItems(ref List<List<KeyboardButton>> keyboardRows)
        {
            var multiRow = new List<KeyboardButton>();
            if (playerStats.availablePotions > 0)
            {
                var potionsButtonText = Emojis.ButtonPotions + Localization.Get(session, "menu_item_potions");
                RegisterButton(potionsButtonText, () => ShowPotionsSelection());
                multiRow.Add(potionsButtonText);
            }
            if (equipped.HasItem(ItemType.Scroll))
            {
                var scrollsButtonText = Emojis.ItemScroll + Localization.Get(session, "menu_item_scrolls");
                RegisterButton(scrollsButtonText, () => OnCategorySelected(ItemType.Scroll));
                multiRow.Add(scrollsButtonText);
            }

            if (multiRow.Count > 0)
            {
                keyboardRows.Add(multiRow);
            }
        }


        public async Task OnCategorySelected(ItemType category)
        {
            if (_isActionAlreadySelected)
                return;

            switch (category)
            {
                case ItemType.Sword:
                case ItemType.Bow:
                case ItemType.Stick:
                    var item = equipped[category];
                    TryInvokeItemSelection(item);
                    break;
                case ItemType.Scroll:
                    await ShowScrollsCategory().FastAwait();
                    break;
            }
        }

        private async Task ShowScrollsCategory()
        {
            ClearButtons();
            var sb = new StringBuilder();
            sb.Append(Emojis.ItemScroll + Localization.Get(session, "menu_item_scrolls").Bold());
            sb.AppendLine(' ' + Emojis.StatMana.ToString() + playerStats.currentMana.ToString());
            sb.AppendLine();

            foreach (var scrollItem in GetAllEquippedScrolls())
            {
                if (scrollItem == null)
                    continue;

                sb.AppendLine(scrollItem.GetFullName(session).Bold());
                if (scrollItem.data.ablitityByType.TryGetValue(AbilityType.DealDamage, out var dealDamage))
                {
                    var simpleDamageView = ((DealDamageAbility)dealDamage).GetSimpleView(session);
                    sb.AppendLine(simpleDamageView.RemoveHtmlTags());
                }
                sb.AppendLine();

                if (playerStats.currentMana >= scrollItem.manaCost)
                {
                    RegisterButton(scrollItem.GetFullName(session).RemoveHtmlTags(),
                        () => SelectScrollItem(scrollItem));
                }
            }

            if (buttonsCount == 0)
            {
                sb.AppendLine(Emojis.ElementWarningGrey + Localization.Get(session, "battle_not_enough_mana"));
            }

            RegisterBackButton(() => Start());
            await SendDialogMessage(sb, GetMultilineKeyboard()).FastAwait();
        }

        private Task SelectScrollItem(InventoryItem scrollItem)
        {
            TryInvokeItemSelection(scrollItem);
            return Task.CompletedTask;
        }

        private IEnumerable<InventoryItem?> GetAllEquippedScrolls()
        {
            for (int i = 0; i < ItemType.Scroll.GetSlotsCount(); i++)
            {
                yield return equipped[ItemType.Scroll, i];
            }
        }

        public override Task TryResendDialog()
        {
            if (_battleTurn.isWaitingForActions)
            {
                return base.TryResendDialog();
            }
            return Task.CompletedTask;
        }

        private void TryInvokeItemSelection(InventoryItem? item)
        {
            if (_isActionAlreadySelected || !_battleTurn.isWaitingForActions)
                return;

            _selectedAttackItemCallback(item);
            _isActionAlreadySelected = true;
        }

        private async Task ShowPotionsSelection()
        {
            if (_isPotionAlreadySelected)
            {
                await messageSender.SendTextMessage(session.chatId, Localization.Get(session, "battle_potion_already_used"),
                    cancellationToken: session.cancellationToken).FastAwait();
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine(Emojis.ButtonPotions + Localization.Get(session, "menu_item_potions").Bold());
            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "battle_potion_selection"));

            ClearButtons();
            RegisterBackButton(() => Start());

            var startBattleTime = _battleTurn.battle.startTime.Ticks;
            var potions = session.player.potions.Where(x => x.preparationTime < startBattleTime);
            foreach (var potionItem in potions)
            {
                RegisterButton(potionItem.GetName(session), () => SelectPotionItem(potionItem));
            }

            await SendDialogMessage(sb, GetMultilineKeyboard()).FastAwait();
        }

        private async Task SelectPotionItem(PotionItem potionItem)
        {
            if (_isPotionAlreadySelected || !_battleTurn.isWaitingForActions)
                return;

            _isPotionAlreadySelected = true;
            playerStats.availablePotions--;
            session.player.potions.Remove(potionItem);

            var potionData = potionItem.GetData();
            potionData.Apply(_battleTurn, session.player);

            SendPotionMessageForEnemy(potionData);
            await SendPotionMessage(potionData).FastAwait();
            await Start().FastAwait();
        }

        private async Task SendPotionMessage(PotionData potionData)
        {
            var sb = new StringBuilder();
            sb.AppendLine(Localization.Get(session, "battle_potion_mine_usage"));
            sb.AppendLine(potionData.GetName(session).Bold());
            sb.AppendLine();
            sb.AppendLine(potionData.GetDescription(session, session));

            await messageSender.SendTextMessage(session.chatId, sb.ToString(),
                cancellationToken: session.cancellationToken).FastAwait();
        }

        private async void SendPotionMessageForEnemy(PotionData potionData)
        {
            var enemy = _battleTurn.enemy;
            var isPlayer = enemy is Player;
            if (!isPlayer)
                return;

            var enemySession = enemy.session;
            var sb = new StringBuilder();
            sb.AppendLine(Localization.Get(enemySession, "battle_potion_enemy_usage", session.player.nickname));
            sb.AppendLine(potionData.GetName(enemySession).Bold());
            sb.AppendLine();
            sb.AppendLine(potionData.GetDescription(session, enemySession));

            await messageSender.SendTextMessage(enemySession.chatId, sb.ToString(),
                cancellationToken: enemySession.cancellationToken).FastAwait();
        }

    }
}
