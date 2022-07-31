using HarmonyLib;
using MonsterLove.StateMachine;
using System;
using JetBrains.Annotations;

namespace NoStopMod.InputFixer.HitIgnore
{
    public class HitIgnorePatches
    {
        
        // scnCLS_serachMode ////////////////////////////////////
        [HarmonyPatch(typeof(scnCLS), "Refresh")]
        private static class scnCLS_Refresh_Patch
        {
            public static void Postfix(scnCLS __instance)
            {
                HitIgnoreManager.scnCLS_searchMode = __instance.searchMode;
            }
        }

        [HarmonyPatch(typeof(scnCLS), "ToggleSearchMode")]
        private static class scnCLS_ToggleSearchMode_Patch
        {
            public static void Postfix(scnCLS __instance)
            {
                HitIgnoreManager.scnCLS_searchMode = __instance.searchMode;
            }
        }

        // scrController_state //////////////////////////
        [HarmonyPatch(typeof(StateEngine), "ChangeState")]
        private static class StateEngine_ChangeState_Patch
        {
            public static void Postfix([NotNull] StateEngine __instance, Enum newState, StateTransition transition)
            {
                HitIgnoreManager.scrController_state = (scrController.States) newState;
            }
        }

        [HarmonyPatch(typeof(scrController), "OnLandOnPortal")]
        private static class scrController_OnLandOnPortal_Patch
        {
            public static void Postfix(scrController __instance)
            {
                HitIgnoreManager.scrController_state = scrController.States.Won;
            }
        }

    }
}
