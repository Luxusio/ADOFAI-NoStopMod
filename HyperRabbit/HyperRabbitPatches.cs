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
                    int lastTileNum = -1;
                    while (__instance.chosenplanet.AutoShouldHitNow() && lastTileNum != __instance.currFloor.seqID)
                    {
                        lastTileNum = __instance.currFloor.seqID;
                        __instance.keyTimes.Clear();
                        __instance.Hit();
                    }
                }
            }
        }

    }

}
