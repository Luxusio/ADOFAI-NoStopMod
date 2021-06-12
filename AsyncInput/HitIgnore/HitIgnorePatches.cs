using HarmonyLib;

namespace NoStopMod.AsyncInput.HitIgnore
{
    class HitIgnorePatches
    {
        
        // scnCLS_serachMode ////////////////////////////////////
        [HarmonyPatch(typeof(scnCLS), "Refresh")]
        private static class scnCLS_Refresh_Patch
        {
            public static void Postfix(scnCLS __instance, ref bool ___searchMode)
            {
                NoStopMod.asyncInputManager.hitIgnoreManager.scnCLS_searchMode = ___searchMode;
            }
        }

        [HarmonyPatch(typeof(scnCLS), "ToggleSearchMode")]
        private static class scnCLS_ToggleSearchMode_Patch
        {
            public static void Postfix(scnCLS __instance, ref bool ___searchMode)
            {
                NoStopMod.asyncInputManager.hitIgnoreManager.scnCLS_searchMode = ___searchMode;
            }
        }

        // scrController_endLevelType //////////////////////////
        [HarmonyPatch(typeof(scrController), "Fail2Action")]
        private static class scrController_Fail2Action_Patch
        {
            public static void Postfix(scrController __instance, ref bool ___searchMode)
            {
                NoStopMod.asyncInputManager.hitIgnoreManager.scrController_endLevelType = __instance.endLevelType;
            }
        }

        [HarmonyPatch(typeof(scrController), "OnLandOnPortal")]
        private static class scrController_OnLandOnPortal_Patch
        {
            public static void Postfix(scrController __instance, ref bool ___searchMode)
            {
                NoStopMod.asyncInputManager.hitIgnoreManager.scrController_endLevelType = __instance.endLevelType;
            }
        }

    }
}
