using System;
using System.Reflection;
using HarmonyLib;
using UnityModManagerNet;
using NoStopMod.GarbageCollection;
using System.Collections.Generic;
using NoStopMod.AsyncInput;

namespace NoStopMod
{
    class NoStopMod
    {

        public static UnityModManager.ModEntry mod;
        public static Harmony harmony;
        public static GCManager gcManager;
        public static AsyncInputManager asyncInputManager;
        public static List<Action<bool>> onToggleListeners;

        public static bool isEnabled = false;
        //public static bool ready = false;

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            modEntry.OnToggle = new Func<UnityModManager.ModEntry, bool, bool>(NoStopMod.OnToggle);
            NoStopMod.harmony = new Harmony(modEntry.Info.Id);
            NoStopMod.mod = modEntry;
            NoStopMod.onToggleListeners = new List<Action<bool>>();

            gcManager = new GCManager();
            asyncInputManager = new AsyncInputManager();

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
                listener.Invoke(enabled);
            }

            return true;
        }

    }
}
