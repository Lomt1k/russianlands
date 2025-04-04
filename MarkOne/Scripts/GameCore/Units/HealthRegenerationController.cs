﻿using System;
using MarkOne.Scripts.GameCore.Buildings;
using MarkOne.Scripts.GameCore.Buildings.General;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;

namespace MarkOne.Scripts.GameCore.Units;

public class HealthRegenerationController
{
    private readonly Player _player;

    private static HospitalBuilding hospital => (HospitalBuilding)BuildingId.Hospital.GetBuilding();
    private ProfileBuildingsData buildingsData => _player.session.profile.buildingsData;

    public HealthRegenerationController(Player player)
    {
        _player = player;
    }

    public void InvokeRegen()
    {
        if (_player.unitStats.isFullHealth)
            return;

        var prevRegenTime = hospital.GetLastRegenTime(buildingsData);
        var timeSpan = DateTime.UtcNow - prevRegenTime;
        if (timeSpan.TotalSeconds < 1)
            return;

        var healthPerMinute = hospital.GetHealthRestorePerMinute(buildingsData);
        var heathAmount = (int)Math.Round(timeSpan.TotalMinutes * healthPerMinute);
        _player.unitStats.RestoreHealth(heathAmount);
        SetLastRegenTimeAsNow();
    }

    public void SetLastRegenTimeAsNow()
    {
        hospital.SetLastRegenTimeAsNow(buildingsData);
    }

}
