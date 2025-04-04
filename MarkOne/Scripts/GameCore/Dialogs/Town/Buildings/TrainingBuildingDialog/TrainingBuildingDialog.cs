﻿using System;
using System.Text;
using System.Threading.Tasks;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Buildings;
using MarkOne.Scripts.GameCore.Buildings.Training;
using MarkOne.Scripts.GameCore.Dialogs.Town.Shop;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Dialogs.Town.Buildings.TrainingBuildingDialog;

public class TrainingBuildingDialog : DialogBase
{
    private readonly TrainingBuildingBase _building;
    private readonly ProfileBuildingsData _data;

    public TrainingBuildingDialog(GameSession session, TrainingBuildingBase building, ProfileBuildingsData data) : base(session)
    {
        _building = building;
        _data = data;
    }

    public override async Task Start()
    {
        if (_building is WarriorTrainingBuilding)
        {
            await ShowCurrentUnit(0, fromUnitsList: true).FastAwait();
            return;
        }
        await ShowUnitsList();
    }

    private async Task ShowUnitsList()
    {
        ClearButtons();
        var sb = new StringBuilder();
        sb.AppendLine(Emojis.ElementTraining + Localization.Get(session, "dialog_training_header_" + _building.buildingId));
        sb.AppendLine();

        var updates = _building.GetUpdates(session, _data, onlyImportant: false);
        if (updates.Count > 0)
        {
            foreach (var update in updates)
            {
                sb.AppendLine(Emojis.ElementSmallBlack + update);
            }
            sb.AppendLine();
        }
        sb.AppendLine(Localization.Get(session, "dialog_training_selection_" + _building.buildingId));

        var units = _building.GetAllUnits(_data);
        foreach (var unit in units)
        {
            var button = $"{_building.GetUnitIcon(_data, unit)} {_building.GetUnitName(session, _data, unit)} ({_building.GetUnitLevel(_data, unit)})";
            RegisterButton(button, () => ShowCurrentUnit(unit, fromUnitsList: true));
        }

        RegisterBackButton(() => new BuildingsDialog(session).StartWithShowBuilding(_building));
        RegisterTownButton(isDoubleBack: true);
        await SendDialogMessage(sb, GetKeyboardWithFixedRowSize(2)).FastAwait();
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
        var hasFreeTrainingSlot = _building.HasFreeTrainingSlot(_data);
        var currentUnitIsTraining = unitIndex == firstTrainingUnit || unitIndex == secondTrainingUnit;

        var sb = new StringBuilder();
        sb.Append($"<b>{_building.GetUnitIcon(_data, unitIndex)} {_building.GetUnitName(session, _data, unitIndex)}");
        sb.AppendLine(Localization.Get(session, "level_suffix", unitLevel) + "</b>");

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
            sb.AppendLine(Localization.Get(session, "dialog_training_progress", timeSpan.GetView(session)));

            sb.AppendLine();
            sb.AppendLine(_building.GetInfoAboutUnitTraining(session, _data, unitIndex));

            var diamondsForBoost = ResourceHelper.CalculateTrainingBoostPriceInDiamonds((int)timeSpan.TotalSeconds);
            var priceView = diamondsForBoost.GetCompactView(shortView: false);
            var boostButton = Localization.Get(session, "menu_item_boost_button", priceView);
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
            sb.AppendLine(Localization.Get(session, "dialog_training_next_level_available", unitLevel + 1));

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
            RegisterBackButton(() => new BuildingsDialog(session).StartWithShowBuilding(_building));
        }
        else
        {
            RegisterBackButton(ShowUnitsList);
        }
        RegisterTownButton(isDoubleBack: true);
        await SendDialogMessage(sb, GetMultilineKeyboardWithDoubleBack()).FastAwait();
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
            Program.logger.Info($"User {session.actualUser} started training in {_building.buildingId} (worker: First, unitIndex: {unitIndex})");
            _building.SetFirstTrainingUnitIndex(_data, unitIndex);
            _building.SetFirstTrainingUnitStartTime(_data, DateTime.UtcNow);
        }
        else if (!_building.HasSecondTrainingUnit(_data))
        {
            Program.logger.Info($"User {session.actualUser} started training in {_building.buildingId} (worker: Second, unitIndex: {unitIndex})");
            _building.SetSecondTrainingUnitIndex(_data, unitIndex);
            _building.SetSecondTrainingUnitStartTime(_data, DateTime.UtcNow);
        }
        await ShowCurrentUnit(unitIndex).FastAwait();
    }

    private async Task TryBoostTraining(sbyte unitIndex)
    {
        _building.IsTrainingCanBeFinished(_data, out var isFirstTrainingCanBeFinished, out var isSecondTrainingCanBeFinished);
        var firstTrainingUnit = _building.GetFirstTrainingUnitIndex(_data);
        var secondTrainingUnit = _building.GetSecondTrainingUnitIndex(_data);
        var isFirst = unitIndex == firstTrainingUnit;

        if (isFirstTrainingCanBeFinished || isSecondTrainingCanBeFinished)
        {
            if (isFirst)
                _building.LevelUpFirst(session, _data);
            else
                _building.LevelUpSecond(session, _data);

            var message = Emojis.ElementTraining + Localization.Get(session, "dialog_training_boost_expired");
            ClearButtons();
            RegisterButton(Localization.Get(session, "menu_item_continue_button"), () => ShowCurrentUnit(unitIndex));

            await SendDialogMessage(message, GetOneLineKeyboard()).FastAwait();
            return;
        }

        var endTime = isFirst ? _building.GetFirstTrainingUnitEndTime(_data) : _building.GetSecondTrainingUnitEndTime(_data);
        var secondsToEnd = (int)(endTime - DateTime.UtcNow).TotalSeconds;
        var requiredDiamonds = ResourceHelper.CalculateTrainingBoostPriceInDiamonds(secondsToEnd);
        var playerResources = session.player.resources;

        var successsPurchase = playerResources.TryPurchase(requiredDiamonds);
        if (successsPurchase)
        {
            if (isFirst)
                _building.LevelUpFirst(session, _data);
            else
                _building.LevelUpSecond(session, _data);

            Program.logger.Info($"User {session.actualUser} boosted training in {_building.buildingId} (worker: {(isFirst ? "First" : "Second")})");
            var sb = new StringBuilder();
            sb.AppendLine(Emojis.ElementTraining + Localization.Get(session, "dialog_training_boosted"));
            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "resource_header_spent"));
            sb.AppendLine(requiredDiamonds.GetLocalizedView(session));

            ClearButtons();
            RegisterButton(Localization.Get(session, "menu_item_continue_button"), () => ShowCurrentUnit(unitIndex));

            await SendDialogMessage(sb, GetOneLineKeyboard()).FastAwait();
            return;
        }

        ClearButtons();
        var text = Localization.Get(session, "resource_not_enough_diamonds", Emojis.SmileSad);
        RegisterButton(Emojis.ButtonShop + Localization.Get(session, "menu_item_shop"), () => new ShopDialog(session).Start());
        RegisterBackButton(() => ShowCurrentUnit(unitIndex));
        await SendDialogMessage(text, GetMultilineKeyboard()).FastAwait();
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
                _building.SetFirstTrainingUnitStartTime(_data, DateTime.MinValue);
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
                _building.SetSecondTrainingUnitStartTime(_data, DateTime.MinValue);
            }
        }

        await ShowCurrentUnit(unitIndex).FastAwait();
    }

}
