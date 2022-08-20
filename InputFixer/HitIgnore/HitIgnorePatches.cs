using HarmonyLib;
using MonsterLove.StateMachine;
using System;
using JetBrains.Annotations;

namespace NoStopMod.InputFixer.HitIgnore
{
    public class HitIgnorePatches
    {

        // scrController_state //////////////////////////
        [HarmonyPatch(typeof(StateEngine), "ChangeState")]
        private static class StateEngine_ChangeState_Patch
        {
            public static void Postfix([NotNull] StateEngine __instance, Enum newState, StateTransition transition)
            {
                HitIgnoreManager.scrController_state = (States) newState;
            }
        }

        [HarmonyPatch(typeof(scrController), "OnLandOnPortal")]
        private static class scrController_OnLandOnPortal_Patch
        {
            public static void Postfix(scrController __instance)
            {
                HitIgnoreManager.scrController_state = States.Won;
            }
        }

    }
}
