﻿using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Locations;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.GlobalMap;

public class MapDialog : DialogWithPanel
{
    private readonly MapDialogPanel _mapPanel;
    public override DialogPanelBase DialogPanel => _mapPanel;


    public MapDialog(GameSession _session) : base(_session)
    {
        _mapPanel = new MapDialogPanel(this);

        RegisterButton(Emojis.ButtonArena + Localization.Get(session, "menu_item_arena"),
            () => messageSender.SendTextMessage(session.chatId, "Арена недоступна в текущей версии игры")); // заглушка
        RegisterButton(Emojis.ButtonCrossRoads + Localization.Get(session, "menu_item_crossroads"),
            () => messageSender.SendTextMessage(session.chatId, "Распутье недоступно в текущей версии игры")); // заглушка
        RegisterTownButton(isDoubleBack: false);
    }



    public override async Task Start()
    {
        var header = Emojis.ButtonMap + Localization.Get(session, "menu_item_map").Bold();
        await SendDialogMessage(header, GetKeyboardWithRowSizes(2, 1)).FastAwait();
        await _mapPanel.Start().FastAwait();
    }

    public async Task StartWithLocation(LocationId locationId)
    {
        var header = Emojis.ButtonMap + Localization.Get(session, "menu_item_map").Bold();
        await SendDialogMessage(header, GetKeyboardWithRowSizes(2, 1)).FastAwait();
        await _mapPanel.ShowLocation(locationId).FastAwait();
    }

}
