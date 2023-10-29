using FastTelegramBot.DataTypes.InputFiles;
using MarkOne.Scripts.GameCore.Services.GameData;
using MarkOne.Scripts.Utils;
using System.Collections.Generic;
using System.IO;

namespace MarkOne.Scripts.GameCore.Input;
public static class InputFiles
{
    private static Dictionary<string, byte[]> filesCache = new();

    public static InputFile Get(string filename)
    {
        lock (filename)
        {
            if (!filesCache.TryGetValue(filename, out var bytesArray)) 
            {
                var filePath = Path.Combine(GameDataHolder.gameDataPath, "InputFiles", filename);
                using var fileStream = File.Open(filePath, FileMode.Open);
                bytesArray = fileStream.ReadAllBytes();
                filesCache[filename] = bytesArray;
            }

            return InputFile.FromStream(new MemoryStream(bytesArray), filename);
        }
    }

    public static InputFile Photo_Arena => Get("arena.webp");
    public static InputFile Photo_Crossroads => Get("crossroads.webp");
    public static InputFile Photo_Loc01 => Get("loc_01.webp");
    public static InputFile Photo_Loc02 => Get("loc_02.webp");
    public static InputFile Photo_Loc03 => Get("loc_03.webp");
    public static InputFile Photo_Loc04 => Get("loc_04.webp");
    public static InputFile Photo_Loc05 => Get("loc_05.webp");
    public static InputFile Photo_Loc06 => Get("loc_06.webp");
    public static InputFile Photo_Loc07 => Get("loc_07.webp");
}
