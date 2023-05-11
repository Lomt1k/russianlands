using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MarkOne.Scripts.GameCore.Arena;

[SQLite.StoreAsText]
[JsonConverter(typeof(StringEnumConverter))]
public enum LeagueId
{
    HALL_3,
    HALL_4,
    HALL_5_START,
    HALL_5_MID,
    HALL_5_END,
    HALL_6_START,
    HALL_6_MID,
    HALL_6_END,
    HALL_7_START,
    HALL_7_MID,
    HALL_7_END,
    HALL_8_START,
    HALL_8_MID,
    HALL_8_END,
}
