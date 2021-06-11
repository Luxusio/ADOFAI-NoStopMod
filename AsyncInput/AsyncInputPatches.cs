using HarmonyLib;
using System;
using UnityEngine;

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
                NoStopMod.asyncInputManager.prevTick = NoStopMod.asyncInputManager.currTick;
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
                if (AudioListener.pause)
                {
                    NoStopMod.asyncInputManager.adjustOffsetTick(__instance, ___dspTimeSong);
                }

                {

                    //long nowTick = NoStopMod.asyncInputManager.currTick - NoStopMod.asyncInputManager.offsetTick;
                    //double calculated = NoStopMod.asyncInputManager.getSongPosition(__instance, nowTick);
                    //NoStopMod.mod.Logger.Log("New Update " + s(__instance.dspTime) + ", " + s(___dspTimeSong) + "," + s(__instance.songposition_minusi) + ", " + s(calculated) + " : " + s(calculated - __instance.songposition_minusi));
                    //__instance.songposition_minusi = calculated;
                }
            }
        }

        [HarmonyPatch(typeof(scrPlanet), "MoveToNextFloor")]
        private static class scrPlanet_Rewind_Patch
        {
            public static void Postfix(scrPlanet __instance)
            {
                long nowTick = (NoStopMod.asyncInputManager.currTick - NoStopMod.asyncInputManager.offsetTick);
                NoStopMod.asyncInputManager.lastHitSec = NoStopMod.asyncInputManager.getSongPosition(__instance.conductor, nowTick);

                // diffTime (from last angle to current)
                // ___snappedLastAngle + (diffTime) 
                //  ;

                // ((nowTick / 10000000.0 - scrConductor.calibration_i) * __instance.song.pitch) - __instance.addoffset;

                //this.conductor.lastHit = this.conductor.lastHit + (exitAngle - (exitAngle % 2)) 
                // (x % m + m) % m;
                //     ;


                // diffTime (from last angle to current)
                // ___snappedLastAngle + (diffTime) / __instance.conductor.crotchet
                // *3.141592653598793238 * __instance.controller.speed * (double)(__instance.controller.isCW ? 1 : -1);

                // ((nowTick / 10000000.0 - scrConductor.calibration_i) * __instance.song.pitch) - __instance.addoffset;

                //this.conductor.lastHit += ((double)exitAngle - this.scrMisc.mod((double)(exitAngle + 3.1415927f), 6.2831854820251465)) * (double)(this.controller.isCW ? 1 : -1) 
                //    / 3.1415927410125732 * this.conductor.crotchet / this.controller.speed;

                /*
                
                if (scrController.isGameWorld)

                num2 = this.targetExitAngle;


                else





                 */

                //NoStopMod.asyncInputManager.lastHitTick = (NoStopMod.asyncInputManager.currTick - NoStopMod.asyncInputManager.offsetTick);

                //double lastHitSec = NoStopMod.asyncInputManager.lastHitTick / 10000000;
                //NoStopMod.mod.Logger.Log("MoveToNextFloor " + __instance.conductor.lastHit + ", " + lastHitSec + " , err: " + (lastHitSec - __instance.conductor.lastHit));

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
                NoStopMod.asyncInputManager.lastHitTick = 0;
                NoStopMod.mod.Logger.Log("Rewind");

            }
        }

        [HarmonyPatch(typeof(scrConductor), "StartMusicCo")]
        private static class scrConductor_StartMusicCo_Patch
        {
            public static void Prefix(scrConductor __instance, double ___dspTimeSong)
            {
                NoStopMod.asyncInputManager.adjustOffsetTick(__instance, ___dspTimeSong);
                NoStopMod.mod.Logger.Log("StartMusicCo");
            }
        }

        [HarmonyPatch(typeof(scrConductor), "ScrubMusicToTile")]
        private static class scrConductor_ScrubMusicToTile_Patch
        {
            public static void Postfix(scrConductor __instance, double ___dspTimeSong)
            {
                NoStopMod.asyncInputManager.adjustOffsetTick(__instance, ___dspTimeSong);
                NoStopMod.asyncInputManager.lastHitTick = (long) (__instance.lastHit * 10000000.0);
                NoStopMod.mod.Logger.Log("ScrubMusicToTile");
            }
        }

        [HarmonyPatch(typeof(scrConductor), "DesyncFix")]
        private static class scrConductor_DesyncFix_Patch
        {
            public static void Postfix(scrConductor __instance, double ___dspTimeSong)
            {
                NoStopMod.asyncInputManager.adjustOffsetTick(__instance, ___dspTimeSong);
                NoStopMod.mod.Logger.Log("DesyncFix");
            }
        }


    }
}
