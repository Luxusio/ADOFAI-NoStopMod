using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityModManagerNet;
using UnityEngine.Scripting;

namespace NoStopMod
{
    class NoStopMod
    {

        public static UnityModManager.ModEntry mod;
        public static Harmony harmony;

        public static bool currentEnabled = false;
        public static bool ready = false;
        public static bool gcEnabled = true;

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            modEntry.OnToggle = new Func<UnityModManager.ModEntry, bool, bool>(NoStopMod.OnToggle);
            NoStopMod.mod = modEntry;
            return true;
        }

        public static bool OnToggle(UnityModManager.ModEntry modEntry, bool enabled)
        {
            NoStopMod.mod = modEntry;
            
            if (NoStopMod.currentEnabled != enabled)
            {
                if (enabled)
                {
                    NoStopMod.ready = true;
                    NoStopMod.harmony = new Harmony(modEntry.Info.Id);
                    NoStopMod.harmony.PatchAll(Assembly.GetExecutingAssembly());
                    
                }
                else
                {
                    NoStopMod.ready = false;
                    NoStopMod.harmony.UnpatchAll(NoStopMod.harmony.Id);
                    
                }
                NoStopMod.currentEnabled = enabled;
            }
            NoStopMod.gcEnabled = true;
            return true;
        }

        public static bool GetDisableAutoSave()
        {
            return NoStopMod.ready && !NoStopMod.gcEnabled;
        }

        public static void DisableGC()
        {
            if (NoStopMod.ready)
            {
                try
                {
                    gcEnabled = false;
                    //NoStopMod.mod.Logger.Log("disablegc");
                    GarbageCollector.GCMode = GarbageCollector.Mode.Disabled;
                    System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.SustainedLowLatency;
                }
                catch (NotImplementedException e)
                {
                    NoStopMod.mod.Logger.Log("Exception occur");
                    NoStopMod.mod.Logger.Error(e.ToString());
                }
            }
        }

        public static void EnableGC()
        {
            if (NoStopMod.ready)
            {
                try
                {
                    gcEnabled = true;
                    //NoStopMod.mod.Logger.Log("enablegc");
                    GarbageCollector.GCMode = GarbageCollector.Mode.Enabled;
                    System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.Interactive;
                    GC.Collect();
                } catch (NotImplementedException e)
                {
                    NoStopMod.mod.Logger.Log("Exception occur");
                    NoStopMod.mod.Logger.Error(e.ToString());
                }
            }
        }

    }
}
