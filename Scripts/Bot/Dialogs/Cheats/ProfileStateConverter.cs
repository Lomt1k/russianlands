using Newtonsoft.Json;
using MarkOne.Scripts.Utils;

namespace MarkOne.Scripts.Bot.Dialogs.Cheats;

internal static class ProfileStateConverter
{
    private const string encryptionPass = "Do not edit this string! This is a password :)";

    public static string Serialize(ProfileState state)
    {
        var json = JsonConvert.SerializeObject(state, Formatting.Indented);
        return Encryption.EncryptStringAES(json, encryptionPass);
    }

    public static ProfileState? Deserialize(string encryptedStr)
    {
        try
        {
            var json = Encryption.DecryptStringAES(encryptedStr, encryptionPass);
            return JsonConvert.DeserializeObject<ProfileState>(json);
        }
        catch (System.Exception ex)
        {
            Program.logger.Error(ex);
            return null;
        }
    }

}
