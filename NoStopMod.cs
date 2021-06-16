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
        public static List<Action<scrController>> onApplicationQuitListeners;

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            modEntry.OnToggle = NoStopMod.OnToggle;
            modEntry.OnGUI = NoStopMod.OnGUI;
            NoStopMod.harmony = new Harmony(modEntry.Info.Id);
            NoStopMod.mod = modEntry;

            NoStopMod.onToggleListeners = new List<Action<bool>>();
            NoStopMod.onGUIListeners = new List<Action<UnityModManager.ModEntry>>();
            NoStopMod.onApplicationQuitListeners = new List<Action<scrController>>();

            GCManager.Init();
            AsyncInputManager.Init();
            HyperRabbitManager.Init();

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
            executeListeners<bool>(onToggleListeners, enabled, "OnToggle");

            return true;
        }

        public static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            NoStopMod.mod = modEntry;
            executeListeners<UnityModManager.ModEntry>(onGUIListeners, modEntry, "OnGUI");
        }


        [HarmonyPatch(typeof(scrController), "OnApplicationQuit")]
        private static class scrController_OnApplicationQuit_Patch
        {
            public static void Prefix(scrController __instance)
            {
                executeListeners<scrController>(onApplicationQuitListeners, __instance, "OnApplicationQuit");
            }
        }

        private static void executeListeners<T>(List<Action<T>> listeners, T obj, String guiMessage)
        {
            for (int i=0;i< listeners.Count;i++)
            {
                try
                {
                    listeners[i].Invoke(obj);
                }
                catch (Exception e)
                {
                    mod.Logger.Error("Error on " + guiMessage + " : " + e.Message + ", " + e.StackTrace);
                }
            }
        }


    }
}
