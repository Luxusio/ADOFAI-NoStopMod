using HarmonyLib;
using System;

namespace NoStopMod.AsyncInput
{
    public static class AsyncInputPatches
    {




        public static string s(double d, int to = 6)
        {
            try
            {
                return ("" + d).Substring(0, to);
            }
            catch
            {
                return "" + d;
            }
        }

        [HarmonyPatch(typeof(scrConductor), "Update")]
        private static class scrConductor_Update_Patch_Time
        {
            public static void Prefix(scrConductor __instance)
            {
                NoStopMod.asyncInputManager.currTick = DateTime.Now.Ticks;
            }
        }

        [HarmonyPatch(typeof(scrController), "TogglePauseGame")]
        private static class scrController_TogglePauseGame_Patch
        {
            public static void Postfix(scrController __instance)
            {
                if (__instance.paused)
                {
                    NoStopMod.asyncInputManager.pauseStart = NoStopMod.asyncInputManager.currTick;
                }
                else if (NoStopMod.asyncInputManager.pauseStart != 0)
                {
                    NoStopMod.asyncInputManager.offsetTick += NoStopMod.asyncInputManager.currTick - NoStopMod.asyncInputManager.pauseStart;
                    NoStopMod.asyncInputManager.pauseStart = 0;
                }
            }
        }

        [HarmonyPatch(typeof(scrController), "Awake_Rewind")]
        private static class scrController_Awake_Rewind_Patch
        {
            public static void Postfix(scrController __instance)
            {
                NoStopMod.asyncInputManager.jumpToOtherClass = true;
                __instance.conductor.Start();
                //NoStopMod.asyncInputManager.Start();
            }
        }



        [HarmonyPatch(typeof(scrConductor), "Update")]
        private static class scrConductor_Update_Patch
        {
            public static void Postfix(scrConductor __instance, double ___dspTimeSong)
            {
                if (__instance.isGameWorld)
                {
                    long nowTick = NoStopMod.asyncInputManager.currTick - NoStopMod.asyncInputManager.offsetTick;
                    
                    double calculated = NoStopMod.asyncInputManager.getSongPosition(__instance, nowTick);
                    //NoStopMod.mod.Logger.Log("New Update " + s(__instance.songposition_minusi) + ", " + s(calculated) + " : " + s(calculated - __instance.songposition_minusi));
                    __instance.songposition_minusi = calculated;
                }
            }
        }

        [HarmonyPatch(typeof(scrController), "OnMusicScheduled")]
        private static class scrController_Rewind_Patch
        {
            public static void Postfix(scrController __instance)
            {
                NoStopMod.asyncInputManager.jumpToOtherClass = true;
                __instance.conductor.Start();
            }
        }

        [HarmonyPatch(typeof(scrConductor), "Start")]
        private static class scrConductor_Start_Patch
        {
            public static bool Prefix(scrConductor __instance, double ___dspTimeSong)
            {
                if (NoStopMod.asyncInputManager.jumpToOtherClass)
                {
                    NoStopMod.asyncInputManager.jumpToOtherClass = false;
                    NoStopMod.asyncInputManager.adjustOffsetTick(__instance, ___dspTimeSong);
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(scrConductor), "Rewind")]
        private static class scrConductor_Rewind_Patch
        {
            public static void Postfix(scrConductor __instance, double ___dspTimeSong)
            {
                NoStopMod.asyncInputManager.adjustOffsetTick(__instance, ___dspTimeSong);
            }
        }

        [HarmonyPatch(typeof(scrConductor), "StartMusicCo")]
        private static class scrConductor_StartMusicCo_Patch
        {
            public static void Prefix(scrConductor __instance, double ___dspTimeSong)
            {
                NoStopMod.asyncInputManager.adjustOffsetTick(__instance, ___dspTimeSong);
            }
        }

        [HarmonyPatch(typeof(scrConductor), "ScrubMusicToTile")]
        private static class scrConductor_ScrubMusicToTile_Patch
        {
            public static void Postfix(scrConductor __instance, double ___dspTimeSong)
            {
                NoStopMod.asyncInputManager.adjustOffsetTick(__instance, ___dspTimeSong);
            }
        }

        [HarmonyPatch(typeof(scrConductor), "DesyncFix")]
        private static class scrConductor_DesyncFix_Patch
        {
            public static void Postfix(scrConductor __instance, double ___dspTimeSong)
            {
                NoStopMod.asyncInputManager.adjustOffsetTick(__instance, ___dspTimeSong);
            }
        }


    }
}
