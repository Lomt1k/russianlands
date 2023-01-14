using JsonKnownTypes;
using Newtonsoft.Json;
using TextGameRPG.Scripts.Bot;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.GameDataBase;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Managers.Battles;
using TextGameRPG.Scripts.GameCore.Units;

namespace TextGameRPG.Scripts.GameCore.Potions
{
    [JsonConverter(typeof(JsonKnownTypesConverter<PotionData>))]
    public abstract class PotionData : IDataWithIntegerID
    {
        public string debugName { get; set; } = "New Potion";
        public int id { get; set; }
        public byte workshopLevel;
        public string localizationKey = string.Empty;
        public byte potionLevel;

        // костыль для отображения в редакторе
        public byte workshopLevelProperty => workshopLevel;

        public PotionData(int _id)
        {
            id = _id;
        }

        public PotionData Clone()
        {
            return (PotionData)MemberwiseClone();
        }

        public void OnSetupAppMode(AppMode appMode)
        {
            //ignored
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
