using HarmonyLib;
using NoStopMod.Helper;

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
                    ControllerHelper.ExecuteUntilTileNotChange(__instance, () =>
                    {
                        if (__instance.chosenplanet.AutoShouldHitNow())
                        {
                            __instance.keyTimes.Clear();
                            __instance.Hit();
#if DEBUG
                            NoStopMod.mod.Logger.Log($"otto Hit {__instance.currFloor.seqID}th tile");
#endif
                        }
                    });
                }
            }
        }

    }

}
