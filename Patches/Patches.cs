using System;
using HarmonyLib;
using UnityEngine;

namespace NoStopMod.Patches
{
    internal static class Patches
    {
        [HarmonyPatch(typeof(CustomLevel), "Play")]
        public static class CustomLevel_Play_Patch
        {
            private static void Prefix(CustomLevel __instance)
            {
                //NoStopMod.mod.Logger.Log("Play");
                NoStopMod.DisableGC();
            }
        }

        [HarmonyPatch(typeof(scnEditor), "ResetScene")]
        public static class scnEditor_ResetScene_Patch
        {
            private static void Postfix(scnEditor __instance)
            {
                //NoStopMod.mod.Logger.Log("ResetScene");
                NoStopMod.EnableGC();
            }
        }

        [HarmonyPatch(typeof(scnEditor), "SaveBackup")]
        public static class scnEditor_SaveBackup_Patch
        {
            private static bool Prefix(scnEditor __instance)
            {
                //NoStopMod.mod.Logger.Log("SaveBackup");
                if (NoStopMod.GetDisableAutoSave())
                {
                    //NoStopMod.mod.Logger.Log("Cancel AutoSave");
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(scrController), "Awake")]
        public static class scrController_Awake_Patch
        {
            public static void Postfix(scrController __instance)
            {
                //NoStopMod.mod.Logger.Log("Awake");
                NoStopMod.EnableGC();
            }
        }

        [HarmonyPatch(typeof(scrController), "FailAction")]
        public static class scrController_FailAction_Patch
        {
            private static void Prefix(scrController __instance)
            {
                NoStopMod.EnableGC();
            }
        }

        [HarmonyPatch(typeof(scrController), "QuitToMainMenu")]
        internal static class scrController_QuitToMainMenu_Patch
        {
            private static void Postfix(scrController __instance)
            {
                //NoStopMod.mod.Logger.Log("StartLoadingScene");
                NoStopMod.EnableGC();
            }
        }

        [HarmonyPatch(typeof(scrController), "ResetCustomLevel")]
        internal static class scrController_ResetCustomLevel_Patch
        {
            private static void Postfix(scrController __instance)
            {
                //NoStopMod.mod.Logger.Log("ResetCustomLevel");
                NoStopMod.EnableGC();
            }
        }

        [HarmonyPatch(typeof(scrController), "Restart")]
        public static class scrController_Restart_Patch
        {
            private static void Prefix(scrController __instance)
            {
                //NoStopMod.mod.Logger.Log("Restart");
                NoStopMod.EnableGC();
            }
        }

        [HarmonyPatch(typeof(scrController), "StartLoadingScene")]
        internal static class scrController_StartLoadingScene_Patch
        {
            private static void Postfix(scrController __instance)
            {
                //NoStopMod.mod.Logger.Log("StartLoadingScene");
                NoStopMod.EnableGC();
            }
        }


        

        [HarmonyPatch(typeof(scrUIController), "WipeToBlack")]
        public static class scrUIController_WipeToBlack_Patch
        {
            private static void Postfix(scrUIController __instance)
            {
                //NoStopMod.mod.Logger.Log("WipeToBlack");
                NoStopMod.EnableGC();
            }
        }

    }
    
    
}
