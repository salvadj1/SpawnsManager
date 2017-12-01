using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Fougerite;
using Fougerite.Events;
using UnityEngine;
//using RustProto;
//using Facepunch.ID;

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
        public static IniParser Settings;


        public override void Initialize()
        {
            if (!File.Exists(Path.Combine(ModuleFolder, "Spawns.ini")))
            {
                File.Create(Path.Combine(ModuleFolder, "Spawns.ini")).Dispose();
                Settings = new IniParser(Path.Combine(ModuleFolder, "Spawns.ini"));
                Settings.AddSetting("Spawns", "Spawn1", "6600, 356, -4400");
                Settings.Save();
            }
            Fougerite.Hooks.OnCommand += OnCommand;
            Fougerite.Hooks.OnPlayerSpawned += OnPlayerSpawned;
            Settings = new IniParser(Path.Combine(ModuleFolder, "Spawns.ini"));
        }
        public override void DeInitialize()
        {
            Fougerite.Hooks.OnCommand += OnCommand;
            Fougerite.Hooks.OnPlayerSpawned += OnPlayerSpawned;
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
            }
            else if (cmd == "spawnadd")
            {
                if (!Player.Admin)
                {
                    Player.MessageFrom("SpawnsManager", red + "You are not an administrator to use this command.");
                }
                else if (args.Length.Equals(0))
                {
                    Player.MessageFrom("SpawnsManager", green + "Makes sure you shoose a name");
                }
                else
                {
                    if (!Player.Equals(null))
                    {
                        float x = Player.X;
                        float y = Player.Y;
                        float z = Player.Z;
                        Settings.AddSetting(args[0], (Player.X) + ", " + (Player.Y) + ", " + (Player.Z));
                        Settings.Save();
                        Player.MessageFrom("SpawnsManager", green + "Spawn added");
                    }
                }
            }
        }
        public void OnPlayerSpawned(Fougerite.Player player, SpawnEvent se)
        {
            var location = player.Location;
            float x = player.X;
            float y = player.Y;
            float z = player.Z;
            //player.TeleportTo(;
        }   //////  still under development
    }
}

