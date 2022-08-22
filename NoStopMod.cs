﻿using System.Reflection;
using HarmonyLib;
using UnityModManagerNet;
using NoStopMod.InputFixer;
using NoStopMod.Helper;
using System;
using UnityEngine;
using System.IO;

namespace NoStopMod
{
    public class NoStopMod
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

            //LoadSharpHookLib();
            LoadDll("Mods/NoStopMod/SharpHook.dll");

            InputFixerManager.Init();

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


        private static void LoadSharpHookLib()
        {
            string osInfo = SystemInfo.operatingSystem.ToLower();

            bool is64bit = osInfo.Contains("64");
            bool isArm = osInfo.Contains("arm");

            string basePath = "Mods/NoStopMod/runtimes/SharpHook-";

            string dllPath = null;

            if (osInfo.Contains("windows"))
            {
                if (isArm)
                {
                    if (is64bit)
                        dllPath = basePath + "win-arm.dll";
                    else
                        dllPath = basePath + "win-arm.dll";
                }
                else
                {
                    if (is64bit)
                        dllPath = basePath + "win-x64.dll";
                    else
                        dllPath = basePath + "win-x86.dll";
                }
            }
            else if (osInfo.Contains("mac"))
            {
                if (isArm)
                    dllPath = basePath + "osx-arm64.dll";
                else
                    dllPath = basePath + "osx-x64.dll";
            }
            else if (osInfo.Contains("linux"))
            {
                if (isArm)
                {
                    if (is64bit)
                        dllPath = basePath + "linux-arm64.dll";
                    else
                        dllPath = basePath + "linux-arm.dll";
                }
                else
                {
                    if (is64bit)
                        dllPath = basePath + "linux-x64.dll";
                    else
                        dllPath = null;
                }
            }

            if (dllPath == null)
            {
                NoStopMod.mod.Logger.Error("Failed to find correct dll (osInfo=" + osInfo + ")");
                return;
            }

            LoadDll(dllPath);
        }

        private static void LoadDll(String path)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            byte[] buffer = new byte[(int)fs.Length];
            fs.Read(buffer, 0, buffer.Length);
            fs.Close();

            AppDomain.CurrentDomain.Load(buffer);
            NoStopMod.mod.Logger.Log("Dll load : " + path);
        }

    }
}
