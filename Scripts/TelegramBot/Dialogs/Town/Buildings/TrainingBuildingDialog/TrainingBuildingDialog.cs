using System;
using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Buildings;
using TextGameRPG.Scripts.GameCore.Buildings.Training;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;
using TextGameRPG.Scripts.TelegramBot.Dialogs.Town.Shop;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Town.Buildings.TrainingBuildingDialog
{
    public class TrainingBuildingDialog : DialogBase
    {
        private TrainingBuildingBase _building;
        private ProfileBuildingsData _data;

        public TrainingBuildingDialog(GameSession session, TrainingBuildingBase building, ProfileBuildingsData data) : base(session)
        {
            _building = building;
            _data = data;
        }

        public override async Task Start()
        {
            if (_building is WarriorTrainingBuilding)
            {
                await ShowCurrentUnit(0, fromUnitsList: true);
                return;
            }
            await ShowUnitsList();
        }

        private async Task ShowUnitsList()
        {
            ClearButtons();
            var sb = new StringBuilder();
            sb.AppendLine($"{Emojis.elements[Element.Training]} " + Localization.Get(session, "dialog_training_header_" + _building.buildingType));
            sb.AppendLine();

            var updates = _building.GetUpdates(session, _data, onlyImportant: false);
            if (updates.Count > 0)
            {
                foreach (var update in updates)
                {
                    sb.AppendLine($"{Emojis.elements[Element.SmallWhite]} {update}");
                }
                sb.AppendLine();
            }
            sb.AppendLine(Localization.Get(session, "dialog_training_selection_" + _building.buildingType));

            var units = _building.GetAllUnits(_data);
            foreach (var unit in units)
            {
                var button = $"{_building.GetUnitIcon(_data, unit)} {_building.GetUnitName(session, _data, unit)}";
                RegisterButton(button, () => ShowCurrentUnit(unit, fromUnitsList: true));
            }

            RegisterBackButton(() => new BuildingsDialog(session).StartWithShowBuildingInfo(_building));
            await messageSender.SendTextDialog(session.chatId, sb.ToString(), GetKeyboardWithFixedRowSize(2));
        }

        private async Task ShowCurrentUnit(sbyte unitIndex, bool fromUnitsList = false)
        {
            if (fromUnitsList)
            {
                TrySilentFinishTrainings();
            }

            ClearButtons();
            var unitLevel = _building.GetUnitLevel(_data, unitIndex);
            var maxUnitLevel = _building.GetCurrentMaxUnitLevel(_data);
            var firstTrainingUnit = _building.GetFirstTrainingUnitIndex(_data);
            var secondTrainingUnit = _building.GetSecondTrainingUnitIndex(_data);
            bool hasFreeTrainingSlot = _building.HasFreeTrainingSlot(_data);
            bool currentUnitIsTraining = unitIndex == firstTrainingUnit || unitIndex == secondTrainingUnit;

            var sb = new StringBuilder();
            sb.Append($"<b>{_building.GetUnitIcon(_data, unitIndex)} {_building.GetUnitName(session, _data, unitIndex)}");
            sb.AppendLine(string.Format(Localization.Get(session, "building_level_suffix"), unitLevel) + "</b>");

            if (unitLevel >= maxUnitLevel)
            {
                // Юнит уже достиг максимального уровня
                sb.AppendLine();
                sb.AppendLine(Localization.Get(session, "dialog_training_max_level_reached"));

                sb.AppendLine();
                sb.AppendLine(_building.GetInfoAboutUnitTraining(session, _data, unitIndex));
            }
            else if (currentUnitIsTraining)
            {
                // В процессе тренировки
                var endTime = unitIndex == firstTrainingUnit ? _building.GetFirstTrainingUnitEndTime(_data) : _building.GetSecondTrainingUnitEndTime(_data);
                var timeSpan = endTime - DateTime.UtcNow;

                sb.AppendLine();
                sb.AppendLine(string.Format(Localization.Get(session, "dialog_training_progress"), timeSpan.GetView(session)));

                sb.AppendLine();
                sb.AppendLine(_building.GetInfoAboutUnitTraining(session, _data, unitIndex));

                var diamondsForBoost = ResourceHelper.CalculateTrainingBoostPriceInDiamonds((int)timeSpan.TotalSeconds);
                var priceView = Emojis.resources[ResourceType.Diamond] + diamondsForBoost;
                var boostButton = string.Format(Localization.Get(session, "menu_item_boost_button"), priceView);
                RegisterButton(boostButton, () => TryBoostTraining(unitIndex));
                RegisterButton(Localization.Get(session, "dialog_training_cancel_training_button"), () => CancelTraining(unitIndex));
            }
            else if (hasFreeTrainingSlot)
            {
                // Можно начать тренировку
                var requiredSeconds = _building.GetRequiredTrainingTime(unitLevel);
                var dtNow = DateTime.UtcNow;
                var timeSpan = dtNow.AddSeconds(requiredSeconds) - dtNow;

                sb.AppendLine();
                sb.AppendLine(string.Format(Localization.Get(session, "dialog_training_next_level_available"), unitLevel + 1));

                sb.AppendLine();
                sb.AppendLine(_building.GetInfoAboutUnitTraining(session, _data, unitIndex));

                sb.AppendLine();
                sb.AppendLine(Localization.Get(session, "dialog_training_required_time_header"));
                sb.AppendLine(timeSpan.GetView(session));
                RegisterButton(Localization.Get(session, "dialog_training_start_training_button"), () => StartTraining(unitIndex));
            }
            else
            {
                // Нет доступных слотов для тренировки
                sb.AppendLine();
                sb.AppendLine(Localization.Get(session, "dialog_training_no_slots_available"));

                sb.AppendLine();
                sb.AppendLine(_building.GetInfoAboutUnitTraining(session, _data, unitIndex));
            }

            if (_building is WarriorTrainingBuilding)
            {
                RegisterBackButton(() => new BuildingsDialog(session).StartWithShowBuildingInfo(_building));
            }
            else
            {
                RegisterBackButton(() => ShowUnitsList());
            }
            await messageSender.SendTextDialog(session.chatId, sb.ToString(), GetMultilineKeyboard());
        }

        private void TrySilentFinishTrainings()
        {
            _building.IsTrainingCanBeFinished(_data, out var isFirstTrainingCanBeFinished, out var isSecondTrainingCanBeFinished);
            if (isFirstTrainingCanBeFinished)
            {
                _building.LevelUpFirst(session, _data);
            }
            if (isSecondTrainingCanBeFinished)
            {
                _building.LevelUpSecond(session, _data);
            }
        }

        private async Task StartTraining(sbyte unitIndex)
        {
            if (!_building.HasFirstTrainingUnit(_data))
            {
                _building.SetFirstTrainingUnitIndex(_data, unitIndex);
                _building.SetFirstTrainingUnitStartTime(_data, DateTime.UtcNow.Ticks);
            }
            else if (!_building.HasSecondTrainingUnit(_data))
            {
                _building.SetSecondTrainingUnitIndex(_data, unitIndex);
                _building.SetSecondTrainingUnitStartTime(_data, DateTime.UtcNow.Ticks);
            }
            await ShowCurrentUnit(unitIndex);
        }

        private async Task TryBoostTraining(sbyte unitIndex)
        {
            _building.IsTrainingCanBeFinished(_data, out var isFirstTrainingCanBeFinished, out var isSecondTrainingCanBeFinished);
            var firstTrainingUnit = _building.GetFirstTrainingUnitIndex(_data);
            var secondTrainingUnit = _building.GetSecondTrainingUnitIndex(_data);
            var isFirst = unitIndex == firstTrainingUnit;

            if (isFirst && isFirstTrainingCanBeFinished)
            {
                _building.LevelUpFirst(session, _data);
                var message = $"{Emojis.elements[Element.Training]} {Localization.Get(session, "dialog_training_boost_expired")}";
                await messageSender.SendTextMessage(session.chatId, message);
            }
            else if (isSecondTrainingCanBeFinished)
            {
                _building.LevelUpSecond(session, _data);
                var message = $"{Emojis.elements[Element.Training]} {Localization.Get(session, "dialog_training_boost_expired")}";
                await messageSender.SendTextMessage(session.chatId, message);
            }
            else
            {
                var endTime = isFirst ? _building.GetFirstTrainingUnitEndTime(_data) : _building.GetSecondTrainingUnitEndTime(_data);
                var secondsToEnd = (int)(endTime - DateTime.UtcNow).TotalSeconds;
                var requiredDiamonds = ResourceHelper.CalculateTrainingBoostPriceInDiamonds(secondsToEnd);
                var playerResources = session.player.resources;

                bool successsPurchase = playerResources.TryPurchase(ResourceType.Diamond, requiredDiamonds);
                if (successsPurchase)
                {
                    if (isFirst)
                        _building.LevelUpFirst(session, _data);
                    else
                        _building.LevelUpSecond(session, _data);

                    var sb = new StringBuilder();
                    sb.AppendLine($"{Emojis.elements[Element.Training]} {Localization.Get(session, "dialog_training_boosted")}");
                    sb.AppendLine();
                    sb.AppendLine(Localization.Get(session, "resource_header_spent"));
                    sb.AppendLine(ResourceType.Diamond.GetLocalizedView(session, requiredDiamonds));
                    await messageSender.SendTextMessage(session.chatId, sb.ToString());
                }
                else
                {
                    ClearButtons();
                    var text = string.Format(Localization.Get(session, "resource_not_enough_diamonds"), Emojis.smiles[Smile.Sad]);
                    RegisterButton($"{Emojis.menuItems[MenuItem.Shop]} {Localization.Get(session, "menu_item_shop")}", () => new ShopDialog(session).Start());
                    RegisterBackButton(() => ShowCurrentUnit(unitIndex));
                    await messageSender.SendTextDialog(session.chatId, text, GetMultilineKeyboard());
                    return;
                }
            }

            await ShowCurrentUnit(unitIndex);
        }

        private async Task CancelTraining(sbyte unitIndex)
        {
            _building.IsTrainingCanBeFinished(_data, out var isFirstTrainingCanBeFinished, out var isSecondTrainingCanBeFinished);

            if (unitIndex == _building.GetFirstTrainingUnitIndex(_data))
            {
                if (isFirstTrainingCanBeFinished)
                {
                    _building.LevelUpFirst(session, _data);
                }
                else
                {
                    _building.SetFirstTrainingUnitIndex(_data, -1);
                    _building.SetFirstTrainingUnitStartTime(_data, 0);
                }
            }
            else if (unitIndex == _building.GetSecondTrainingUnitIndex(_data))
            {
                if (isSecondTrainingCanBeFinished)
                {
                    _building.LevelUpSecond(session, _data);
                }
                else
                {
                    _building.SetSecondTrainingUnitIndex(_data, -1);
                    _building.SetSecondTrainingUnitStartTime(_data, 0);
                }
            }

            await ShowCurrentUnit(unitIndex);
        }

    }
}
