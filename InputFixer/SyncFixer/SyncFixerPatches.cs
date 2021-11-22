using HarmonyLib;
using UnityEngine;

namespace NoStopMod.InputFixer.SyncFixer
{
    class SyncFixerPatches
    {

        [HarmonyPatch(typeof(scrConductor), "Update")]
        private static class scrConductor_Update_Patch_Time
        {
            public static void Postfix(scrConductor __instance, double ___dspTimeSong)
            {
                if (!AudioListener.pause && Application.isFocused && Time.unscaledTime - SyncFixerManager.previousFrameTime < 0.1)
                {
                    SyncFixerManager.dspTime += Time.unscaledTime - SyncFixerManager.previousFrameTime;
                }
                SyncFixerManager.previousFrameTime = Time.unscaledTime;

                if (AudioSettings.dspTime != SyncFixerManager.lastReportedDspTime)
                {
                    SyncFixerManager.lastReportedDspTime = AudioSettings.dspTime;
                    SyncFixerManager.dspTime = AudioSettings.dspTime;
                    SyncFixerManager.offsetTick = NoStopMod.CurrFrameTick() - (long)(SyncFixerManager.dspTime * 10000000);
                }

                SyncFixerManager.dspTimeSong = ___dspTimeSong;
            }
        }

    }
}
