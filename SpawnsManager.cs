using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fougerite;
using Fougerite.Events;
using UnityEngine;
using RustProto;

namespace SpawnsManager
{
    public class SpawnsManager : Fougerite.Module
    {
        public override string Name { get { return "SpawnsManager"; } }
        public override string Author { get { return "ice cold"; } }
        public override string Description { get { return "Recreate the spawns of rust"; } }
        public override Version Version { get { return new Version("1.0"); } }

        public string green = "[color #82FA58]";
        public string red = "[color #B40404]";
        public static IniParser ini;
        public static IniParser Spawns;
        public static string Path;
        public static string Combine;


        public override void Initialize()
        {
            if (!File.Exists(Path.Combine(ModuleFolder, "Spawns.ini")))
            {
                File.Create(Path.Combine(ModuleFolder, "Spawns.ini")).Dispose();
                Spawns = new IniParser(Path.Combine(ModuleFolder, "Spawns.ini"));
                ini.AddSetting("Spawns", "Spawn1", "6600, 356, -4400");
                Spawns.Save();
            }
            ReloadSpawns();
            Fougerite.Hooks.OnCommand += OnCommand;
            Fougerite.Hooks.OnPlayerSpawned += OnPlayerSpawned;
            Fougerite.Hooks.OnPlayerTeleport += OnPlayerTeleport;
            rnd = new System.Random();
            ini = new IniParser(Path.Combine(ModuleFolder, "Spawns.ini"));
        }
        public override void DeInitialize()
        {
            Fougerite.Hooks.OnCommand += OnCommand;
            Fougerite.Hooks.OnPlayerSpawned += OnPlayerSpawned;
            Fougerite.Hooks.OnPlayerTeleport += OnPlayerTeleport;
        }
        public void OnCommand(Fougerite.Player Player, string cmd, string[] args)
        {
            if (cmd == "spawnshelp")
            {
                if (!Player.Admin)
                {
                    Player.MessageFrom("SpawnsManager", red + "You are not an administrator to use this command.");
                    return;
                }
                else
                {
                    Player.MessageFrom("SpawnsManager", green + "/spawnadd - adds a spawn to the random spawns.");
                    Player.MessageFrom("SpawnsManager", green + "/spawndel - deletes a spawn.");
                    Player.MessageFrom("SpawnsManager", green + "/spawnsreload - reloads the spawns list.");
                }
                if (cmd == "spawnadd")
                {
                    if (!Player.Admin)
                    {
                        Player.MessageFrom("SpawnsManager", red + "You are not an administrator to use this command.");
                        return;
                    }
                    else
                    {
                        if (args.Length > 0)
                        {
                            if (Name.Contains(args[0]))
                            {
                                Player.MessageFrom("SpawnsManager", "This spawn is already added.");
                                return;
                            }
                            else
                            {
                                float x = Player.X;
                                float y = Player.Y;
                                float z = Player.Z;
                                ini.AddSetting(args[0], (Player.X) + ", " + (Player.Y) + ", " + (Player.Z));
                                Player.MessageFrom("SpawnsManager", green + "Spawn added");
                                Spawns.Save();
                            }
                        }
                        else
                        {
                            Player.MessageFrom("SpawnsManager", "You need to add a name of the spawn");
                            return;
                        }
                    }
                }
            }
        }
        public void OnPlayerSpawned(Fougerite.Player player, SpawnEvent se)
        {
            var location = player.Location;
            {
                int r = rnd.Next(1, 10);
                string l = ini.GetSetting("Spawns", r.ToString());
                Vector3 v = Util.GetUtil().ConvertStringToVector3(l);
                Player.TeleportTo(Spawns, false);
            }

