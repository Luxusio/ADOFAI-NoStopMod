using System.Reflection;
using HarmonyLib;
using UnityModManagerNet;
using NoStopMod.InputFixer;
using NoStopMod.HyperRabbit;
using NoStopMod.Helper;
using System;

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

        private static long currFrameTick;
        private static long prevFrameTick;

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            modEntry.OnToggle = NoStopMod.OnToggle;
            modEntry.OnGUI = onGUIListener.Invoke;
            modEntry.OnHideGUI = onHideGUIListener.Invoke;

            NoStopMod.harmony = new Harmony(modEntry.Info.Id);
            NoStopMod.mod = modEntry;
            
            NoStopMod.prevFrameTick = DateTime.Now.Ticks;
            NoStopMod.currFrameTick = prevFrameTick;
            
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

        public static long CurrFrameTick()
        {
            return NoStopMod.currFrameTick;
        }

        public static long PrevFrameTick()
        {
            return NoStopMod.prevFrameTick;
        }

        [HarmonyPatch(typeof(scrConductor), "Update")]
        private static class scrConductor_Update_Patch_Time
        {
            public static void Prefix(scrConductor __instance)
            {
                NoStopMod.prevFrameTick = NoStopMod.currFrameTick;
                NoStopMod.currFrameTick = DateTime.Now.Ticks;
            }
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
