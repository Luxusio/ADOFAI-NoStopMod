using HarmonyLib;

namespace NoStopMod.AsyncInput.HitDisable
{
    class HitDisablePatches
    {
        
        [HarmonyPatch(typeof(scnCLS), "Refresh")]
        private static class scnCLS_Refresh_Patch
        {
            public static void Postfix(scnCLS __instance, ref bool ___searchMode)
            {
                NoStopMod.asyncInputManager.hitDisableManager.scnCLS_searchMode = ___searchMode;
            }
        }

        [HarmonyPatch(typeof(scnCLS), "ToggleSearchMode")]
        private static class scnCLS_ToggleSearchMode_Patch
        {
            public static void Postfix(scnCLS __instance, ref bool ___searchMode)
            {
                NoStopMod.asyncInputManager.hitDisableManager.scnCLS_searchMode = ___searchMode;
            }
        }


    }
}
