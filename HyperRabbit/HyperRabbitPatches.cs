using HarmonyLib;

namespace NoStopMod.HyperRabbit
{
    class HyperRabbitPatches
    {

        [HarmonyPatch(typeof(scrController), "PlayerControl_Update")]
        private static class scrController_PlayerControl_Update_Patch
        {
            public static void Postfix(scrController __instance)
            {
                if (RDC.auto && scrController.isGameWorld)
                {
                    int num = HyperRabbitManager.settings.maxTilePerFrame;
                    while (num > 0 && __instance.chosenplanet.AutoShouldHitNow())
                    {
                        __instance.keyBufferCount = 0;
                        __instance.Hit();
                        num--;
                    }
                }
            }
        }

    }

}
