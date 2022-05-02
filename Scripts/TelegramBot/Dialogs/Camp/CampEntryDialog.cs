using System;
using System.Collections.Generic;
using System.Text;
using TextGameRPG.Scripts.GameCore.Localization;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Camp
{
    public enum CampEntryReason
    {
        StartNewSession,
        EndTutorial
    }

    public class CampEntryDialog : DialogBase
    {
        private CampEntryReason _reason;

        public CampEntryDialog(GameSession _session, CampEntryReason reason) : base(_session)
        {
            _reason = reason;
        }

        public async override void Start()
        {
            string text;
            switch (_reason)
            {
                case CampEntryReason.EndTutorial:
                    text = Localization.Get(session, "dialog_tutorial_camp_entry_text_endTutorial");
                    break;

                case CampEntryReason.StartNewSession:
                default:
                    text = Localization.Get(session, "dialog_tutorial_camp_entry_text_newSession");
                    break;
            }

            RegisterButton("Test Button", null);
            await messageSender.SendTextDialog(session.chatId, text, GetKeyboard());
        }

    }
}
