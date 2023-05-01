using JsonKnownTypes;
using Newtonsoft.Json;
using TextGameRPG.Scripts.Bot;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Services.GameData;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Services.Battles;
using TextGameRPG.Scripts.GameCore.Units;
using TextGameRPG.ViewModels.UserControls;

namespace TextGameRPG.Scripts.GameCore.Potions
{
    [JsonObject] [JsonConverter(typeof(JsonKnownTypesConverter<PotionData>))]    
    public abstract class PotionData : IGameDataWithId<int>
    {
        public string debugName { get; set; } = "New Potion";
        [IgnoreInEditor]
        public int id { get; set; }
        public byte workshopLevel { get; set; }
        public string localizationKey { get; set; } = string.Empty;
        public byte potionLevel { get; set; }

        public PotionData(int _id)
        {
            id = _id;
        }

        public void OnSetupAppMode(AppMode appMode)
        {
            // ignored
        }

        public string GetName(GameSession session)
        {
            return Emojis.ButtonPotions + Localization.Get(session, localizationKey)
                +  $" {Localization.Get(session, "level_suffix", potionLevel)}";
        }

        public string GetNameWithoutIcon(GameSession session)
        {
            return Localization.Get(session, localizationKey)
                + $" {Localization.Get(session, "level_suffix", potionLevel)}";
        }

        public abstract string GetDescription(GameSession sessionForValues, GameSession sessionForView);
        public abstract void Apply(BattleTurn battleTurn, IBattleUnit unit);

    }
}
