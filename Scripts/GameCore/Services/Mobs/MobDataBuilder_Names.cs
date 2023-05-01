using System;
using System.Collections.Generic;
using System.Linq;
using TextGameRPG.Scripts.GameCore.Locations;

namespace TextGameRPG.Scripts.GameCore.Services.Mobs
{
    public partial class MobDataBuilder
    {
        private static readonly string[] location_1_names =
        {
            "mob_name_young_wolf",
            "mob_name_frail_robber",
            "mob_name_greedy_robber",
            "mob_name_wild_boar",
            "mob_name_insidious_robber",
            "mob_name_ferosious_wolf",
            "mob_name_brown_bear",
            "mob_name_hungry_lynx",
            "mob_name_severe_robber",
            "mob_name_experienced_robber",
        };

        private static readonly string[] location_2_names =
        {
            "mob_name_nasty_spider",
            "mob_name_bloodthirsty_zombie",
            "mob_name_dashing_zombie",
            "mob_name_slimy_toad",
            "mob_name_slug",
            "mob_name_mad_elk",
            "mob_name_damn_zombie",
            "mob_name_poisonous_spider",
            "mob_name_mermaid",
            "mob_name_hungry_zombie",
        };

        private static readonly string[] location_3_names =
        {
            "mob_name_snow_wolf",
            "mob_name_mountain_troll",
            "mob_name_polar_bear",
            "mob_name_feral_hermit",
            "mob_name_giant",
            "mob_name_wild_goat",
            "mob_name_harpy",
            "mob_name_armored_troll",
            "mob_name_ferosious_harpy",
        };

        private static readonly string[] location_4_names =
        {
            "mob_name_sly_rogue",
            "mob_name_impudent_thief",
            "mob_name_dire_wolf",
            "mob_name_daring_thief",
            "mob_name_steppe_viper",
            "mob_name_notorious_rogue",
            "mob_name_street_robber",
            "mob_name_mad_lynx",
            "mob_name_village_thief",
        };

        private static readonly string[] location_5_names =
        {
            "mob_name_wild_elf",
            "mob_name_hyena",
            "mob_name_evil_elf",
            "mob_name_angry_bull",
            "mob_name_brave_dwarf",
            "mob_name_fearless_dwarf",
            "mob_name_idiot_dwarf",
            "mob_name_mighty_dwarf",
            "mob_name_treacherous_elf",
            "mob_name_light_elf",
            "mob_name_dark_elf",
        };

        private static readonly string[] location_6_names =
        {
            "mob_name_wild_elf",
            "mob_name_evil_elf",
            "mob_name_angry_bull",
            "mob_name_treacherous_elf",
            "mob_name_light_elf",
            "mob_name_dark_elf",
            "mob_name_young_ent",
            "mob_name_giant_spider",
            "mob_name_elder_ent",
            "mob_name_agressive_elf",
        };

        private static readonly string[] location_7_names =
        {
            "mob_name_greedy_goblin",
            "mob_name_alligator",
            "mob_name_gilatooth",
            "mob_name_evil_rat",
            "mob_name_petty_goblin",
            "mob_name_fat_goblin",
            "mob_name_black_eagle",
            "mob_name_caribou",
        };

        public MobDataBuilder SetRandomName(LocationType locationType = LocationType.None, List<string>? excludeNames = null)
        {
            var random = new Random();
            if (locationType == LocationType.None)
            {
                var min = (byte)LocationType.Loc_01;
                var max = (byte)LocationType.Loc_07;
                locationType = (LocationType)random.Next(min, max + 1);
            }

            var array = locationType switch
            {
                LocationType.Loc_01 => location_1_names,
                LocationType.Loc_02 => location_2_names,
                LocationType.Loc_03 => location_3_names,
                LocationType.Loc_04 => location_4_names,
                LocationType.Loc_05 => location_5_names,
                LocationType.Loc_06 => location_6_names,
                LocationType.Loc_07 => location_7_names,
                _ => throw new NotImplementedException()
            };

            do
            {
                var index = random.Next(array.Length);
                _mobData.localizationKey = array[index];
            } while (excludeNames != null && excludeNames.Contains(_mobData.localizationKey));
            
            return this;
        }

    }
}
