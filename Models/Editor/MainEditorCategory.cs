﻿using Avalonia.Controls;

namespace TextGameRPG.Models.Editor;

public class MainEditorCategory
{
    public string name { get; }
    public UserControl view { get; }

    public MainEditorCategory(string _name, UserControl _view)
    {
        name = _name;
        view = _view;
    }

}
