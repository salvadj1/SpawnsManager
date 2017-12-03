using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Fougerite;
using Fougerite.Events;
using UnityEngine;

namespace SpawnsManager
{
    public class SpawnsManager : Fougerite.Module
    {
        public override string Name { get { return "SpawnsManager"; } }
        public override string Author { get { return "ice cold & salva/juli"; } }
        public override string Description { get { return "Recreate the spawns of rust"; } }
        public override Version Version { get { return new Version("1.0"); } }

        public string green = "[color #82FA58]";
        public string red = "[color #B40404]";
        public static IniParser ini;
        public List<Vector3> Spawns = new List<Vector3>();

        public override void Initialize()
        {
            if (!File.Exists(Path.Combine(ModuleFolder, "Spawns.ini")))
            {
                File.Create(Path.Combine(ModuleFolder, "Spawns.ini")).Dispose();
                ini = new IniParser(Path.Combine(ModuleFolder, "Spawns.ini"));
                ini.AddSetting("Spawns", "1", "6600, 356, -4400");
                ini.Save();
            }
            Fougerite.Hooks.OnCommand += OnCommand;
            Fougerite.Hooks.OnPlayerSpawned += OnPlayerSpawned;

            LoadSpawns();
        }
        public override void DeInitialize()
        {
            Fougerite.Hooks.OnCommand -= OnCommand;
            Fougerite.Hooks.OnPlayerSpawned -= OnPlayerSpawned;
        }
        public void OnCommand(Fougerite.Player pl, string cmd, string[] args)
        {
            if (cmd == "spawns")
            {
                if (!pl.Admin)
                {
                    pl.MessageFrom("SpawnsManager", red + "You are not an administrator to use this command.");
                    return;
                }

                else
                {
                    pl.MessageFrom("SpawnsManager", green + "/spawnadd - adds a spawn to the random spawns.");
                    pl.MessageFrom("SpawnsManager", green + "/spawndel - MAKE SURE YOU TYPE THE FULL NAME OF THE SPAWN TO DELETE IT.");
                    pl.MessageFrom("SpawnsManager", green + "/spawnsreload - reloads the spawns list.");
                }
            }
            else if (cmd == "spawnadd")// IT IS NOT NECESSARY TO CREATE A NAME FOR EACH SPAWN, WE WILL MAKE THEM GENERATED AUTOMATICALLY AND SUCCESSIVE example: 0 1 2 3 4
            {
                if (!pl.Admin)
                {
                    pl.MessageFrom("SpawnsManager", red + "You are not an administrator to use this command.");
                    return;
                }

                AddSpawn(pl);
            }
            else if (cmd == "spawndel")
            {
                if (!pl.Admin)
                {
                    pl.MessageFrom("SpawnsManager", red + "You dont have acces to use this command.");
                    return;
                }
                
                if (args.Length > 0)
                {
                    DelSpawn(pl, args[0]);
                }
                else
                {
                    pl.MessageFrom("SpawnsManager", green + "Makes sure you choose a number");
                    return;
                }
            }
            else if (cmd == "spawnsreload")
            {
                if (!pl.Admin)
                {
                    pl.MessageFrom("SpawnsManager", red + "You dont have acces to use this command.");
                    return;
                }

                RefreshSpawns(pl);
            }
        }  
        public void OnPlayerSpawned(Fougerite.Player pl, SpawnEvent se)
        {
            RandomSpawn(pl);
        }
        // /////////////////////////////////methods/////////////////////////////////
        public void AddSpawn(Fougerite.Player pl)
        {
            pl.MessageFrom("SpawnsManager", green + "Spawn added");
        }
        public void DelSpawn(Fougerite.Player pl, string number)
        {
            //CHECK IF NUMBER OF SPAWN EXIST
            pl.MessageFrom("SpawnsManager", green + "Spawn deleted");
        }
        public void RandomSpawn(Fougerite.Player pl)
        {
            //Randomize the spawn of all of the available list

            //make sure there are no houses near the spawn

            //Teleport player
           
        }
        public void RefreshSpawns(Fougerite.Player pl)
        {
            Spawns.Clear();
            ini = new IniParser(Path.Combine(ModuleFolder, "Spawns.ini"));
            int total = 0;
            foreach (var x in ini.EnumSection("Spawns"))
            {
                string loc;
                loc = ini.GetSetting("Spawns", total.ToString());
                Spawns.Add(Util.GetUtil().ConvertStringToVector3(loc));
                total += 1;
            }
            pl.MessageFrom("SpawnsManager", green + "Spawn List Reloaded (Found " + total.ToString() + " locations)");
        }
        public void LoadSpawns()
        {
            Spawns.Clear();
            ini = new IniParser(Path.Combine(ModuleFolder, "Spawns.ini"));
            int total = 0;
            foreach (var x in ini.EnumSection("Spawns"))
            {
                string loc;
                loc = ini.GetSetting("Spawns", total.ToString());
                Spawns.Add(Util.GetUtil().ConvertStringToVector3(loc));
                total += 1;
            }
            Logger.Log("Spawn List Reloaded (Found " + total.ToString() + " locations)");
            return;
        }
    }
}

