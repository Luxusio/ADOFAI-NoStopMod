using System;
using System.Reflection;
using HarmonyLib;
using UnityModManagerNet;
using NoStopMod.GarbageCollection;
using System.Collections.Generic;
using NoStopMod.AsyncInput;
using NoStopMod.HyperRabbit;
using SimpleJSON;

namespace NoStopMod
{
    class NoStopMod
    {

        public static UnityModManager.ModEntry mod;
        public static Harmony harmony;
        public static bool isEnabled = false;

        public static List<Action<bool>> onToggleListeners;
        public static List<Action<UnityModManager.ModEntry>> onGUIListeners;

        public static GCManager gcManager;
        public static AsyncInputManager asyncInputManager;
        public static HyperRabbitManager hyperRabbitManager;

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            modEntry.OnToggle = NoStopMod.OnToggle;
            //modEntry.OnGUI = NoStopMod.OnGUI;
            NoStopMod.harmony = new Harmony(modEntry.Info.Id);
            NoStopMod.mod = modEntry;
            NoStopMod.onToggleListeners = new List<Action<bool>>();
            NoStopMod.onGUIListeners = new List<Action<UnityModManager.ModEntry>>();

            gcManager = new GCManager();
            asyncInputManager = new AsyncInputManager();
            hyperRabbitManager = new HyperRabbitManager();

            

            return true;
        }

        public static bool OnToggle(UnityModManager.ModEntry modEntry, bool enabled)
        {
            NoStopMod.mod = modEntry;

            if (enabled)
            {
                NoStopMod.harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            else
            {
                NoStopMod.harmony.UnpatchAll(NoStopMod.harmony.Id);
            }

            isEnabled = enabled;
            foreach (Action<bool> listener in onToggleListeners)
            {
                try
                {
                    listener.Invoke(enabled);
                }
                catch (Exception e)
                {
                    mod.Logger.Error("Error on OnToggle : " + e.Message);
                }
            }

            return true;
        }

        public static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            NoStopMod.mod = modEntry;
            foreach (Action<UnityModManager.ModEntry> listener in onGUIListeners)
            {
                try
                { 
                    listener.Invoke(modEntry);
                }
                catch (Exception e)
                {
                    mod.Logger.Error("Error on ONGUI : " + e.Message);
                }
            }
        }

    }
}
