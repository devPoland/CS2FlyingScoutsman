using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Events;

namespace FlyingScoutsman;
public class FlyingScoutsman : BasePlugin
{
    public override string ModuleName => "Flying Scoutsman";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "devPoland";

    public override void Load(bool hotReload)
    {
        SetConvars();
        
        RegisterEventHandler<EventPlayerSpawn>(OnPlayerSpawn);
        RegisterEventHandler<EventItemPickup>(OnItemPickup);
        RegisterEventHandler<EventRoundStart>(OnRoundStart);
    }

    private void SetConvars()
    {
        Server.ExecuteCommand("mp_maxrounds 15");
        Server.ExecuteCommand("sv_gravity 230");
        Server.ExecuteCommand("mp_cash_awards 0");
        Server.ExecuteCommand("mp_team_cash_awards 0");
        Server.ExecuteCommand("mp_free_armor 0");
        Server.ExecuteCommand("mp_buy_allow_guns 0");
        Server.ExecuteCommand("weapon_accuracy_nospread 1");
        Server.ExecuteCommand("sv_full_alltalk 1");
        Server.ExecuteCommand("mp_roundtime 3");
        Server.ExecuteCommand("mp_roundtime_defuse 2.25");
        Server.ExecuteCommand("mp_halftime 0");
        Server.ExecuteCommand("mp_solid_teammates 0");
    }

    private HookResult OnPlayerSpawn(EventPlayerSpawn @event, GameEventInfo info)
    {
        var player = @event.Userid;

        if (!player.IsValid || player.PlayerPawn.Value == null)
            return HookResult.Continue;

            var pawn = player.PlayerPawn.Value;
            if (pawn == null || !pawn.IsValid) return HookResult.Continue;


            player.GiveNamedItem("item_kevlar");
            player.GiveNamedItem("weapon_ssg08");
            player.GiveNamedItem("weapon_knife");

            if (player.InGameMoneyServices != null)
                player.InGameMoneyServices.Account = 1;

        return HookResult.Continue;
    }

    private HookResult OnItemPickup(EventItemPickup @event, GameEventInfo info)
    {
        var player = @event.Userid;
        string weaponName = @event.Item.ToLower();

        if (!player.IsValid || player.PlayerPawn.Value == null)
            return HookResult.Continue;

        if (weaponName == "c4") 
            return HookResult.Continue;

        if (weaponName != "ssg08" && weaponName != "knife")
        {
            var pawn = player.PlayerPawn.Value;
            if (pawn == null || !pawn.IsValid) return HookResult.Continue;

            var weapons = pawn.WeaponServices?.MyWeapons;
            if (weapons != null)
            {
                foreach (var weaponHandle in weapons)
                {
                    var weapon = weaponHandle.Value;
                    if (weapon != null && weapon.IsValid)
                    {
                        if (weapon.DesignerName.ToLower() == "weapon_"+weaponName)
                        {
                            weapon.Remove();
                        }
                    }
                }
            }
        }

        return HookResult.Continue;
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