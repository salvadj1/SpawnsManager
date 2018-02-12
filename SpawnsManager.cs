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
        #region VARS
        public override string Name { get { return "SpawnsManager"; } }
        public override string Author { get { return "salva/juli & ice cold"; } }
        public override string Description { get { return "Recreate the spawns of rust"; } }
        public override Version Version { get { return new Version("1.0"); } }

        public string green = "[color #82FA58]";
        public string red = "[color #B40404]";

        public List<string> spawns;
        public System.Random rnd;
        public System.IO.StreamWriter spawnsfile;
        #endregion VARS

        public override void Initialize()
        {
            spawns = new List<string>();
            rnd = new System.Random();

            if (!File.Exists(Path.Combine(ModuleFolder, "Spawns.txt")))
            {
                File.Create(Path.Combine(ModuleFolder, "Spawns.txt")).Dispose();
                spawnsfile = new System.IO.StreamWriter(Path.Combine(ModuleFolder, "Spawns.txt"), true);
                spawnsfile.WriteLine("(4668.0, 445.0, -3908.0)"); //add by default at least one spawn point in next valley north
                spawnsfile.Close();
            }
            Fougerite.Hooks.OnCommand += OnCommand;
            Fougerite.Hooks.OnPlayerSpawned += OnPlayerSpawned;
            Hooks.OnPlayerDisconnected += OnPlayerDisconnected;
            ReloadSpawns();
        }
        public override void DeInitialize()
        {
            Fougerite.Hooks.OnCommand -= OnCommand;
            Fougerite.Hooks.OnPlayerSpawned -= OnPlayerSpawned;
            Hooks.OnPlayerDisconnected -= OnPlayerDisconnected;
        }
        #region HOOKS
        public void OnCommand(Fougerite.Player pl, string cmd, string[] args)
        {
            if (!pl.Admin) { return; }
            if (cmd == "spawn")
            {
                if (args.Length == 0)
                {
                    pl.MessageFrom(Name, green + "/spawn - adds a spawn to the random spawns.");
                    pl.MessageFrom(Name, green + "/spawn add - adds a spawn to the random spawns.");
                    //pl.MessageFrom("SpawnsManager", green + "/spawn del - MAKE SURE YOU TYPE THE FULL NAME OF THE SPAWN TO DELETE IT.");
                    pl.MessageFrom(Name, green + "/spawn reload - reloads the spawns list.");
                }
                else
                {
                    if (args[0] == "add")
                    {
                        AddSpawn(pl);
                    }
                    else if (args[0] == "reload")
                    {
                        ReloadSpawns();
                        pl.MessageFrom(Name, green + "Spawn list Reloaded ,found (" + spawns.Count + ") points.");
                    }
                    else if (args[0] == "test")
                    {
                        tryagain:
                        Vector3 location = GivemeRandomSpawn();
                        if (IsCloseOfStructure(location))
                        {
                            pl.Message("TEST: cant spawn on structure, serching another location");
                            goto tryagain;
                        }
                        else
                        {
                            pl.SafeTeleportTo(location);
                        }
                    }
                }
            }
        }
        public void OnPlayerSpawned(Fougerite.Player pl, SpawnEvent se)
        {
            if (!se.CampUsed)
            {
                if (DataStore.GetInstance().ContainsKey("SpawnManager", pl.UID))
                {
                    if (pl.Location != (Vector3)DataStore.GetInstance().Get("SpawnManager", pl.UID))
                    {
                        TryTeleportPlayer(pl);
                    }
                }
                else
                {
                    TryTeleportPlayer(pl);
                }
            }
        }
        public void OnPlayerDisconnected(Fougerite.Player pl)
        {
            if (DataStore.GetInstance().ContainsKey("SpawnManager", pl.UID))
            {
                DataStore.GetInstance().Remove("SpawnManager", pl.UID);
                DataStore.GetInstance().Add("SpawnManager", pl.UID, pl.DisconnectLocation);
            }
            else
            {
                DataStore.GetInstance().Add("SpawnManager", pl.UID, pl.DisconnectLocation);
            }
        }
        #endregion HOOKS
        #region METHODS
        public void ReloadSpawns()
        {
            spawns.Clear();
            foreach (var xx in File.ReadAllLines(Path.Combine(ModuleFolder, "Spawns.txt")))
            {
                if (!xx.Contains("(") || xx =="")
                {
                    continue;
                }
                spawns.Add(xx);
            }
            return;
        }
        public void AddSpawn(Fougerite.Player pl)
        {
            spawnsfile = new System.IO.StreamWriter(Path.Combine(ModuleFolder, "Spawns.txt"), true);
            spawnsfile.WriteLine(pl.Location.ToString());
            spawnsfile.Close();
            pl.MessageFrom(Name, green + "Spawn added");
        }
        public void TryTeleportPlayer(Fougerite.Player pl)
        {
        tryagain:
            Vector3 location = GivemeRandomSpawn();
            if (IsCloseOfStructure(location))
            {
                //pl.Message("CANT TP ON STRUCTURE");
                goto tryagain; //try found another spawn point
            }
            else
            {
                pl.SafeTeleportTo(location);
            }
        }
        public Vector3 GivemeRandomSpawn()
        {
            int TotalSpawnPoints = spawns.Count;
            int RandomPoint = rnd.Next(0, TotalSpawnPoints);
            Vector3 location = Util.GetUtil().ConvertStringToVector3(spawns[RandomPoint]);
            int RandomDistanceX = rnd.Next(-10, 10);
            int RandomDistanceZ = rnd.Next(-10, 10);
            Vector3 FinalLocation = new Vector3(location.x + RandomDistanceX, location.y, location.z + RandomDistanceZ);
            return FinalLocation;
        }
        public bool IsCloseOfStructure(Vector3 location)
        {
            var objects = Physics.OverlapSphere(location, 3f);
            var names = new List<string>();
            foreach (var x in objects.Where(x => !names.Contains(x.name.ToLower())))
            {
                names.Add(x.name.ToLower());
            }
            string ncollected = string.Join(" ", names.ToArray());
            if (ncollected.Contains("meshbatch") || ncollected.Contains("shelter"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion METHODS
    }
}
