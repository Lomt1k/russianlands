﻿using System;
using System.Threading.Tasks;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Commands;

public class LanguageCommand : CommandBase
{
    public override CommandGroup commandGroup => CommandGroup.Cheat;

    public override async Task Execute(GameSession session, string[] args)
    {
        if (args.Length != 1)
            return;

        if (!Enum.TryParse(args[0], ignoreCase: true, out LanguageCode language))
            return;

        session.profile.data.language = language;
        await messageSender.SendTextMessage(session.chatId, $"Language changed to {language}");
    }
}
