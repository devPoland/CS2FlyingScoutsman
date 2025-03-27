using CounterStrikeSharp.API.Core;
using System.Text.Json.Serialization;

namespace FlyingScoutsman;

public class FlyingScoutsmanConfig : BasePluginConfig
{
    [JsonPropertyName("mp_maxrounds")] public int MpMaxRounds { get; set; } = 15;
    [JsonPropertyName("sv_gravity")] public int SvGravity { get; set; } = 230;
    [JsonPropertyName("mp_cash_awards")] public int MpCashAwards { get; set; } = 0;
    [JsonPropertyName("mp_team_cash_awards")] public int MpTeamCashAwards { get; set; } = 0;
    [JsonPropertyName("mp_free_armor")] public int MpFreeArmor { get; set; } = 0;
    [JsonPropertyName("mp_buy_allow_guns")] public int MpBuyAllowGuns { get; set; } = 0;
    [JsonPropertyName("weapon_accuracy_nospread")] public int WeaponAccuracyNospread { get; set; } = 1;
    [JsonPropertyName("sv_full_alltalk")] public int SvFullAlltalk { get; set; } = 1;
    [JsonPropertyName("mp_roundtime")] public float MpRoundtime { get; set; } = 3f;
    [JsonPropertyName("mp_roundtime_defuse")] public float MpRoundtimeDefuse { get; set; } = 2.25f;
    [JsonPropertyName("mp_halftime")] public int MpHalftime { get; set; } = 0;
    [JsonPropertyName("mp_solid_teammates")] public int MpSolidTeammates { get; set; } = 0;
    [JsonPropertyName("sv_autobunnyhopping")] public int SvAutoBunnyhopping { get; set; } = 0;

    public FlyingScoutsmanConfig()
    {
        Version = 1;
    }
}