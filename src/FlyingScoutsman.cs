using System.Text.Json;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Timers;

namespace FlyingScoutsman;
public class FlyingScoutsman : BasePlugin
{
    public override string ModuleName => "Flying Scoutsman";
    public override string ModuleVersion => "1.0.1";
    public override string ModuleAuthor => "devPoland";

    public FlyingScoutsmanConfig Config { get; set; } = new();
    private readonly HashSet<string> _allowedWeapons = new() { "weapon_ssg08", "weapon_knife", "weapon_c4" };

    public bool PreventSpam = false;

    public void OnConfigParsed(FlyingScoutsmanConfig config)
    {
        Config = config;
    }

    public override void Load(bool hotReload)
    {
        Config = LoadConfig();
        SetConvars();
        
        RegisterEventHandler<EventPlayerSpawn>(OnPlayerSpawn);
        RegisterEventHandler<EventRoundStart>(OnRoundStart);

        AddTimer(0.1f, () => 
        {
            foreach (var player in Utilities.GetPlayers().Where(p => p.IsValid && p.PawnIsAlive))
            {
                CheckPlayerWeapons(player);
            }
        }, TimerFlags.REPEAT);
    }

    private FlyingScoutsmanConfig LoadConfig()
    {
        var configPath = Path.Combine(ModuleDirectory, "flying_scoutsman.json");

        if (!File.Exists(configPath))
        {
            SaveConfig(configPath, Config);
            return Config;
        }
        
        var json = File.ReadAllText(configPath);
        return JsonSerializer.Deserialize<FlyingScoutsmanConfig>(json) ?? new FlyingScoutsmanConfig();
    }

    private FlyingScoutsmanConfig SaveConfig(string configPath, FlyingScoutsmanConfig config)
    {
        var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(configPath, json);
        return config;
    }

    private void SetConvars()
    {
        Server.ExecuteCommand($"mp_maxrounds {Config.MpMaxRounds}");
        Server.ExecuteCommand($"sv_gravity {Config.SvGravity}");
        Server.ExecuteCommand($"mp_cash_awards {Config.MpCashAwards}");
        Server.ExecuteCommand($"mp_team_cash_awards {Config.MpTeamCashAwards}");
        Server.ExecuteCommand($"mp_free_armor {Config.MpFreeArmor}");
        Server.ExecuteCommand($"mp_buy_allow_guns {Config.MpBuyAllowGuns}");
        Server.ExecuteCommand($"weapon_accuracy_nospread {Config.WeaponAccuracyNospread}");
        Server.ExecuteCommand($"sv_full_alltalk {Config.SvFullAlltalk}");
        Server.ExecuteCommand($"mp_roundtime {Config.MpRoundtime}");
        Server.ExecuteCommand($"mp_roundtime_defuse {Config.MpRoundtimeDefuse}");
        Server.ExecuteCommand($"mp_halftime {Config.MpHalftime}");
        Server.ExecuteCommand($"mp_solid_teammates {Config.MpSolidTeammates}");
        Server.ExecuteCommand($"sv_autobunnyhopping {Config.SvAutoBunnyhopping}");
    }

    private HookResult OnPlayerSpawn(EventPlayerSpawn @event, GameEventInfo info)
    {
        var player = @event.Userid;

        if (!player.IsValid || player.PlayerPawn?.Value == null)
            return HookResult.Continue;

        var pawn = player.PlayerPawn.Value;
        if (!pawn.IsValid) return HookResult.Continue;

        player.GiveNamedItem("item_kevlar");
        player.GiveNamedItem("weapon_ssg08");
        player.GiveNamedItem("weapon_knife");

        if (player.InGameMoneyServices != null)
            player.InGameMoneyServices.Account = 1;

        return HookResult.Continue;
    }

    private void CheckPlayerWeapons(CCSPlayerController player)
    {
        var pawn = player.PlayerPawn.Value;
        if (pawn == null || !pawn.IsValid) return;

        var activeWeapon = pawn.WeaponServices?.ActiveWeapon?.Value;
        if (activeWeapon == null || !activeWeapon.IsValid) return;

        string weaponName = activeWeapon.DesignerName?.ToLowerInvariant().Trim();

        if (string.IsNullOrEmpty(weaponName) || !_allowedWeapons.Contains(weaponName))
        {
            if (!PreventSpam){
                player.PrintToChat($" {ChatColors.Purple}[Flying Scoutsman]{ChatColors.LightRed} this weapon is not allowed!");
                PreventSpam = true;
                AddTimer(2f, () => {PreventSpam = false;});
            }
            
            player.ExecuteClientCommand("slot3");
        }
    }
        
    private HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        SetConvars();
        foreach (var player in Utilities.GetPlayers())
        {
            if (player.IsValid && player.PawnIsAlive)
            {
                if (player.InGameMoneyServices != null)
                {
                    player.InGameMoneyServices.Account = 1;
                }
            }
        }
        return HookResult.Continue;
    }
}