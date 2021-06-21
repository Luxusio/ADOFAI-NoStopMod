using ADOFAI;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NoStopMod.InputFixer.SyncFixer
{
    class SyncFixerPatches
    {
        //[HarmonyPatch(typeof(scrConductor), "StartMusicCo")]
        //private static class scrConductor_StartMusicCo_Patch
        //{
        //    public static IEnumerator Transpiler(scrConductor __instance, Action onComplete, Action onSongScheduled = null)
        //    {
        
        [HarmonyPatch(typeof(scrConductor), "Awake")]
        private static class scrConductor_Awake_Patch
        {
            public static bool Prefix(scrConductor __instance)
            {
                SyncFixerManager.newScrConductor.Awake(__instance);
                return false;
            }
        }

        [HarmonyPatch(typeof(scrConductor), "Start")]
        private static class scrConductor_Start_Patch
        {
            public static bool Prefix(scrConductor __instance)
            {
                SyncFixerManager.newScrConductor.Start(__instance);
                return false;
            }
        }

        [HarmonyPatch(typeof(scrConductor), "Rewind")]
        private static class scrConductor_Rewind_Patch
        {
            public static bool Prefix(scrConductor __instance)
            {
                SyncFixerManager.newScrConductor.Rewind(__instance);
                return false;
            }
        }

        [HarmonyPatch(typeof(scrConductor), "SetupConductorWithLevelData")]
        private static class scrConductor_SetupConductorWithLevelData_Patch
        {
            public static bool Prefix(scrConductor __instance, LevelData levelData)
            {
                SyncFixerManager.newScrConductor.SetupConductorWithLevelData(__instance, levelData);
                return false;
            }
        }

        [HarmonyPatch(typeof(scrConductor), "PlayHitTimes")]
        private static class scrConductor_PlayHitTimes_Patch
        {
            public static bool Prefix(scrConductor __instance)
            {
                SyncFixerManager.newScrConductor.PlayHitTimes(__instance);
                return false;
            }
        }

        [HarmonyPatch(typeof(scrConductor), "GetCountdownTime")]
        private static class scrConductor_GetCountdownTime_Patch
        {
            public static bool Prefix(scrConductor __instance, ref double __result, int i)
            {
                __result = SyncFixerManager.newScrConductor.GetCountdownTime(__instance, i);
                return false;
            }
        }

        [HarmonyPatch(typeof(scrConductor), "StartMusic")]
        private static class scrConductor_StartMusic_Patch
        {
            public static bool Prefix(scrConductor __instance, Action onComplete = null, Action onSongScheduled = null)
            {
                SyncFixerManager.newScrConductor.StartMusic(__instance, onComplete, onSongScheduled);
                return false;
            }
        }

        [HarmonyPatch(typeof(scrConductor), "StartMusicCo")]
        private static class scrConductor_StartMusicCo_Patch
        {
            public static bool Prefix(IEnumerator __result, scrConductor __instance, Action onComplete, Action onSongScheduled = null)
            {
                __result = SyncFixerManager.newScrConductor.StartMusicCo(__instance, onComplete, onSongScheduled);
                return false;
            }
        }

        [HarmonyPatch(typeof(scrConductor), "ToggleHasSongStarted")]
        private static class scrConductor_ToggleHasSongStarted_Patch
        {
            public static bool Prefix(IEnumerator __result, scrConductor __instance, double songstarttime)
            {
                __result = SyncFixerManager.newScrConductor.ToggleHasSongStarted(__instance, songstarttime);
                return false;
            }
        }

        [HarmonyPatch(typeof(scrConductor), "Update")]
        private static class scrConductor_Update_Patch
        {
            public static bool Prefix(scrConductor __instance)
            {
                SyncFixerManager.newScrConductor.Update(__instance);
                return false;
            }
        }

        [HarmonyPatch(typeof(scrConductor), "OnBeat")]
        private static class scrConductor_OnBeat_Patch
        {
            public static bool Prefix(scrConductor __instance)
            {
                SyncFixerManager.newScrConductor.OnBeat(__instance);
                return false;
            }
        }

        [HarmonyPatch(typeof(scrConductor), "PlaySfx")]
        private static class scrConductor_PlaySfx_Patch
        {
            public static bool Prefix(scrConductor __instance, int num, float volume = 1f, bool ignoreListenerPause = false)
            {
                SyncFixerManager.newScrConductor.PlaySfx(__instance, num, volume, ignoreListenerPause);
                return false;
            }
        }

        [HarmonyPatch(typeof(scrConductor), "ScrubMusicToTile")]
        private static class scrConductor_ScrubMusicToTile_Patch
        {
            public static bool Prefix(scrConductor __instance, int tileID)
            {
                SyncFixerManager.newScrConductor.ScrubMusicToTile(__instance, tileID);
                return false;
            }
        }

        [HarmonyPatch(typeof(scrConductor), "DesyncFix")]
        private static class scrConductor_DesyncFix_Patch
        {
            public static bool Prefix(IEnumerator __result, scrConductor __instance)
            {
                __result = SyncFixerManager.newScrConductor.DesyncFix(__instance);
                return false;
            }
        }

        [HarmonyPatch(typeof(scrConductor), "SaveVisualOffset")]
        private static class scrConductor_SaveVisualOffset_Patch
        {
            public static bool Prefix(scrConductor __instance, double offset)
            {
                SyncFixerManager.newScrConductor.SaveVisualOffset(offset);
                return false;
            }
        }
        

    }
}
