using HarmonyLib;

namespace NoStopMod.HyperRabbit
{
    class HyperRabbitPatches
    {
        
        [HarmonyPatch(typeof(scrPlanet), "SwitchChosen")]
        private static class PlanetSwitchChosenPatch
        {
            public static void Prefix(scrPlanet __instance)
            {
                if (RDC.auto && scrController.isGameWorld)
                {
                    __instance.angle = __instance.targetExitAngle;
                }
            }
        }

        [HarmonyPatch(typeof(scrController), "PlayerControl_Update")]
        private static class scrController_PlayerControl_Update_Patch2
        {
            public static void Postfix(scrController __instance)
            {
                if (!RDC.auto || !scrController.isGameWorld)
                {
                    return;
                }

                __instance.chosenplanet.Update_RefreshAngles();
                while (__instance.chosenplanet.AutoShouldHitNow())
                {
                    __instance.Hit();
                    __instance.chosenplanet.Update_RefreshAngles();
                }
            }
        }

        [HarmonyPatch(typeof(scrController), "FailAction")]
        private static class ControllerFailActionPatch
        {
            public static bool Prefix()
            {
                return !RDC.auto;
            }
        }

    }

}
