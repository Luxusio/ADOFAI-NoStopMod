using System;
using System.Reflection;
using HarmonyLib;
using UnityModManagerNet;
using NoStopMod.GarbageCollection;
using System.Collections.Generic;
using NoStopMod.InputFixer;
using NoStopMod.HyperRabbit;
using SimpleJSON;
using NoStopMod.Helper;

namespace NoStopMod
{
    class NoStopMod
    {

        public static UnityModManager.ModEntry mod;
        public static Harmony harmony;
        public static bool isEnabled = false;

        public static EventListener<bool> onToggleListener = new EventListener<bool>("OnToggle");
        public static EventListener<UnityModManager.ModEntry> onGUIListener = new EventListener<UnityModManager.ModEntry>("OnGUI");
        public static EventListener<UnityModManager.ModEntry> onHideGUIListener = new EventListener<UnityModManager.ModEntry>("OnHideGUI");
        public static EventListener<scrController> onApplicationQuitListener = new EventListener<scrController>("OnApplicationQuit");

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            modEntry.OnToggle = NoStopMod.OnToggle;
            modEntry.OnGUI = onGUIListener.Invoke;
            modEntry.OnHideGUI = onHideGUIListener.Invoke;

            NoStopMod.harmony = new Harmony(modEntry.Info.Id);
            NoStopMod.mod = modEntry;
            
            GCManager.Init();
            InputFixerManager.Init();
            HyperRabbitManager.Init();

            Settings.Init();
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
            onToggleListener.Invoke(enabled);
            return true;
        }
        
        [HarmonyPatch(typeof(scrController), "OnApplicationQuit")]
        private static class scrController_OnApplicationQuit_Patch
        {
            public static void Prefix(scrController __instance)
            {
                onApplicationQuitListener.Invoke(__instance);
            }
        }
        
    }
}
