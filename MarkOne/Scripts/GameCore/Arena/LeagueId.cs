using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MarkOne.Scripts.GameCore.Arena;

[SQLite.StoreAsText]
[JsonConverter(typeof(StringEnumConverter))]
public enum LeagueId : byte
{
    HALL_3 = 0,
    HALL_3_MID = 1,
    HALL_3_END = 2,
    HALL_4 = 40,
    HALL_4_MID = 41,
    HALL_4_END = 42,
    HALL_5_START = 50,
    HALL_5_MID = 51,
    HALL_5_END = 52,
    HALL_6_START = 60,
    HALL_6_MID = 61,
    HALL_6_END = 62,
    HALL_7_START = 70,
    HALL_7_MID = 71,
    HALL_7_END = 72,
    HALL_8_START = 80,
    HALL_8_MID = 81,
    HALL_8_END = 82,
}
