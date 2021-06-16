using HarmonyLib;
using NoStopMod.GarbageCollection;

namespace NoStopMod.Patches
{
    public static class Patches
    {
        [HarmonyPatch(typeof(CustomLevel), "Play")]
        private static class CustomLevel_Play_Patch
        {
            private static void Prefix(CustomLevel __instance)
            {
                //NoStopMod.mod.Logger.Log("Play");
                GCManager.DisableGC();
            }
        }

        [HarmonyPatch(typeof(scnEditor), "ResetScene")]
        private static class scnEditor_ResetScene_Patch
        {
            private static void Postfix(scnEditor __instance)
            {
                //NoStopMod.mod.Logger.Log("ResetScene");
                GCManager.EnableGC();
            }
        }

        [HarmonyPatch(typeof(scnEditor), "SaveBackup")]
        private static class scnEditor_SaveBackup_Patch
        {
            private static bool Prefix(scnEditor __instance)
            {
                //NoStopMod.mod.Logger.Log("SaveBackup");
                if (GCManager.GetDisableAutoSave())
                {
                    //NoStopMod.mod.Logger.Log("Cancel AutoSave");
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(scrController), "Awake")]
        private static class scrController_Awake_Patch
        {
            public static void Postfix(scrController __instance)
            {
                //NoStopMod.mod.Logger.Log("Awake");
                GCManager.EnableGC();
            }
        }

        [HarmonyPatch(typeof(scrController), "FailAction")]
        private static class scrController_FailAction_Patch
        {
            private static void Prefix(scrController __instance)
            {
                GCManager.EnableGC();
            }
        }

        [HarmonyPatch(typeof(scrController), "QuitToMainMenu")]
        private static class scrController_QuitToMainMenu_Patch
        {
            private static void Postfix(scrController __instance)
            {
                //NoStopMod.mod.Logger.Log("StartLoadingScene");
                GCManager.EnableGC();
            }
        }

        [HarmonyPatch(typeof(scrController), "ResetCustomLevel")]
        private static class scrController_ResetCustomLevel_Patch
        {
            private static void Postfix(scrController __instance)
            {
                //NoStopMod.mod.Logger.Log("ResetCustomLevel");
                GCManager.EnableGC();
            }
        }

        [HarmonyPatch(typeof(scrController), "Restart")]
        private static class scrController_Restart_Patch
        {
            private static void Prefix(scrController __instance)
            {
                //NoStopMod.mod.Logger.Log("Restart");
                GCManager.EnableGC();
            }
        }

        [HarmonyPatch(typeof(scrController), "StartLoadingScene")]
        private static class scrController_StartLoadingScene_Patch
        {
            private static void Postfix(scrController __instance)
            {
                //NoStopMod.mod.Logger.Log("StartLoadingScene");
                GCManager.EnableGC();
            }
        }
        

        [HarmonyPatch(typeof(scrUIController), "WipeToBlack")]
        private static class scrUIController_WipeToBlack_Patch
        {
            private static void Postfix(scrUIController __instance)
            {
                //NoStopMod.mod.Logger.Log("WipeToBlack");
                GCManager.EnableGC();
            }
        }

    }
    
    
}
