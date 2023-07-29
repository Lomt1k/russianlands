using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FastTelegramBot.DataTypes.InputFiles;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Dialogs;

public class SimpleDialog : DialogBase
{
    private readonly string _text;
    private readonly InputFile? _photo;

    public SimpleDialog(GameSession _session, string text, bool withTownButton, Dictionary<string, Func<Task>> buttons) : base(_session)
    {
        _text = text;
        foreach (var button in buttons)
        {
            RegisterButton(button.Key, button.Value);
        }
        if (withTownButton)
        {
            RegisterTownButton(isDoubleBack: false);
        }
    }

    public SimpleDialog(GameSession _session, InputFile photo, string text, bool withTownButton, Dictionary<string, Func<Task>> buttons) : this(_session, text, withTownButton, buttons)
    {
        _photo = photo;
    }

    public SimpleDialog(GameSession _session, string text) : base(_session)
    {
        _text = text;
        RegisterTownButton(isDoubleBack: false);
    }

    public override async Task Start()
    {
        if (_photo is null)
        {
            await SendDialogMessage(_text, GetMultilineKeyboard()).FastAwait();
        }
        else
        {
            await SendDialogPhotoMessage(_photo, _text, GetMultilineKeyboard()).FastAwait();
        }
    }
}
