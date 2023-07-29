using System;
using System.Threading.Tasks;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Sessions;
using FastTelegramBot.DataTypes;
using System.Collections.Generic;

namespace MarkOne.Scripts.Bot;
public class TelegramBotUpdateHandler
{
    private static readonly SessionManager sessionManager = ServiceLocator.Get<SessionManager>();

    public void HandleUpdates(List<Update> updates)
    {
        foreach (Update update in updates)
        {
            HandleUpdate(update);
        }
    }

    public void HandleUpdate(Update update)
    {
        //Program.logger.Debug($"Handle Update ID: {update.id} Type: {update.updateType}");
        try
        {
            User? fromUser = null;
            switch (update.UpdateType)
            {
                case UpdateType.Message: fromUser = update.Message?.From; break;
                case UpdateType.CallbackQuery: fromUser = update.CallbackQuery?.From; break;

                default:
                    Program.logger.Warn($"Unhandled Update {update.Id} (Unsupported type: {update.UpdateType})");
                    return;
            }

            if (fromUser == null)
            {
                Program.logger.Warn($"Unhandled Update {update.Id} (Update not from user)");
                return;
            }

            var gameSession = sessionManager.GetOrCreateSession(fromUser);
            Task.Run(() => gameSession.HandleUpdateAsync(fromUser, update));
        }
        catch (Exception ex)
        {
            Program.logger.Error($"Exception on handle update with ID: {update.Id}\n{ex}\n");
        }
    }

}
