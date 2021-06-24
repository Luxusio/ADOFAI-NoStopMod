using DG.Tweening;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NoStopMod.InputFixer.SyncFixer
{
    class SyncFixerPatchesScrController
    {

        [HarmonyPatch(typeof(scrController), "OnMusicScheduled")]
        private static class scrController_OnMusicScheduled_Patch
        {
            public static bool Prefix(scrController __instance)
            {
                if (GCS.checkpointNum != 0)
                {
                    //__instance.conductor.hasSongStarted = true;
                    __instance.Scrub(GCS.checkpointNum, RDC.auto && __instance.isLevelEditor);
                    __instance.ChangeState(scrController.States.Checkpoint);
                }
                else if (!GCS.d_oldConductor)
                {
                    __instance.conductor.song.time = 0f;
                    scrController.States states = (__instance.gameworld && !__instance.forceNoCountdown) ? scrController.States.Countdown : scrController.States.PlayerControl;
                    __instance.printe("changing to state: " + states);
                    __instance.ChangeState(states);
                }
                __instance.uiController.MinimizeDifficultyContainer();
                if (GCS.checkpointNum != 0)
                {
                    scrDebugHUDMessage.Log("OnMusicStart");
                    if (__instance.isLevelEditor)
                    {
                        scrUIController.instance.FadeFromBlack(1f);
                    }
                }
                if (!__instance.gameworld)
                {
                    scrUIController.instance.FadeFromBlack(0.3f);
                }
                float duration = Mathf.Min(50f / (__instance.conductor.bpm * __instance.conductor.song.pitch), 0.5f);
                DOTween.To(() => __instance.chosenplanet.cosmeticRadius, delegate (float x)
                {
                    __instance.chosenplanet.cosmeticRadius = x;
                }, __instance.startRadius, duration);

                return false;
            }
        }


        [HarmonyPatch(typeof(scrController), "Scrub")]
        private static class scrController_Scrub_Patch
        {
            public static bool Prefix(scrController __instance, int floorNum, bool forceDontStartMusicFourTilesBefore = false)
            {
                if (floorNum > scrLevelMaker.instance.listFloors.Count - 1 || floorNum < 0)
                {
                    scrDebugHUDMessage.Log("Past the limit");
                    return false;
                }
                int num = __instance.FindScrubStart(floorNum, forceDontStartMusicFourTilesBefore);
                int windbackNum = (num == floorNum) ? -1 : num;
                __instance.chosenplanet.ScrubToFloorNumber(floorNum, windbackNum, __instance.isLevelEditor || RDC.debug);
                if (RDC.debug)
                {
                    __instance.camy.ViewObjectInstant(__instance.chosenplanet.transform, false);
                }
                //base.conductor.ScrubMusicToTile(num);
                if (__instance.isLevelEditor)
                {
                    GameObject.Find("Vfx")
                        ?.GetComponent<scrVfxPlus>()
                        ?.ScrubToTime((float) __instance.lm.listFloors[num].entryTime);
                }
                return false;
            }
        }

    }
}
