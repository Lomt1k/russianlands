﻿using Obisoft.HSharp.Models;

namespace MarkOne.Scripts.GameCore.Http.AdminService;
public static class StylesHelper
{
    public static HProp CenterScreenParent()
    {
        return new HProp("style",
            "width: 100%;" +
            "height: 100%;" +
            "position: absolute;" +
            "top: 0;" +
            "left: 0;" +
            "overflow: auto;");
    }

    public static HProp CenterScreenBlock(int width, int height)
    {
        return new HProp("style",
            $"width: {width}px;" +
            $"height: {height}px;" +
            "position: absolute;" +
            "top: 0;" +
            "right: 0;" +
            "bottom: 0;" +
            "left: 0;" +
            "margin: auto;");
    }

    public static HProp Button(string color = "#4CAF50", int size = 16)
    {
        return new HProp("style",
            $"background-color: {color};" +
            "border: none;" +
            "color: white;" +
            "padding: 15px 32px;" +
            "text-align: center;" +
            "text-decoration: none;" +
            "display: inline-block;" +
            $"font-size: {size}px;" +
            "border-radius: 12px;");
    }
}
