using DG.Tweening;
using HarmonyLib;
using NoStopMod.InputFixer.HitIgnore;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NoStopMod.InputFixer
{
    public static class AsyncInputPatches
    {

        //public static string s(double d, int to = 6)
        //{
        //    try
        //    {
        //        return ("" + d).Substring(0, to);
        //    }
        //    catch
        //    {
        //        return "" + d;
        //    }
        //}
        
        [HarmonyPatch(typeof(scrController), "Awake")]
        private static class scrController_Awake_Patch
        {
            public static void Postfix(scrController __instance)
            {
                InputFixerManager.jumpToOtherClass = true;
                __instance.conductor.Start();
                InputFixerManager.Start();
            }
        }

        [HarmonyPatch(typeof(scrConductor), "Update")]
        private static class scrConductor_Update_Patch_Time
        {
            public static void Prefix(scrConductor __instance)
            {
                InputFixerManager.prevTick = InputFixerManager.currTick;
                InputFixerManager.currTick = DateTime.Now.Ticks;
            }

            public static void Postfix(scrConductor __instance, double ___dspTimeSong)
            {   
                if (AudioListener.pause)
                {
                    InputFixerManager.adjustOffsetTick(__instance, ___dspTimeSong);
                }
                
                if (!InputFixerManager.settings.enableAsync)
                {
                    InputFixerManager.UpdateKeyQueue(InputFixerManager.currTick);
                }

                while (InputFixerManager.keyQueue.Any())
                {
                    long tick;
                    List<KeyCode> keyCodes;
                    InputFixerManager.keyQueue.Dequeue().Deconstruct(out tick, out keyCodes);

                    if (AudioListener.pause || RDC.auto) continue;
                    
                    scrController controller = __instance.controller;
                    int count = 0;
                    for (int i = 0; i < keyCodes.Count(); i++)
                    {
                        if (HitIgnoreManager.shouldBeIgnored(keyCodes[i])) continue;
                        if (++count > 4) break;
                    }

                    InputFixerManager.currPressTick = tick - InputFixerManager.offsetTick;
                    controller.keyBufferCount += count;
                    while (controller.keyBufferCount > 0)
                    {
                        controller.keyBufferCount--;
                        InputFixerManager.jumpToOtherClass = true;
                        controller.chosenplanet.Update_RefreshAngles();
                        controller.Hit();
                    }
                }

            }
        }

        [HarmonyPatch(typeof(scrController), "CountValidKeysPressed")]
        private static class scrController_CountValidKeysPressed_Patch
        {
            public static bool Prefix(scrController __instance, ref int __result)
            {
                __result = 0;
                return false;
            }

            //public static void Postfix(scrController __instance, ref int __result)
            //{
            //    if (InputFixerManager.settings.enableAsync)
            //    {
            //        __result = 0;
            //    }
            //}
        }

        [HarmonyPatch(typeof(scrPlanet), "Update_RefreshAngles")]
        private static class scrPlanet_Update_RefreshAngles_Patch
        {
            public static bool Prefix(scrPlanet __instance, ref double ___snappedLastAngle)
            {

                if (InputFixerManager.jumpToOtherClass)
                {
                    InputFixerManager.jumpToOtherClass = false;
                    __instance.angle = InputFixerManager.getAngle(__instance, ___snappedLastAngle, InputFixerManager.currPressTick);

                    return false;
                }

                if (!__instance.isChosen || __instance.conductor.crotchet == 0.0) return false;

                if (!GCS.d_stationary)
                {
                    long nowTick = InputFixerManager.currTick - InputFixerManager.offsetTick;
                    __instance.angle = InputFixerManager.getAngle(__instance, ___snappedLastAngle, nowTick);

                    if (__instance.shouldPrint)
                    {
                        __instance.shouldPrint = false;
                    }
                }
                else
                {
                    if (Input.GetKey(KeyCode.DownArrow))
                    {
                        __instance.angle += 0.10000000149011612;
                    }
                    if (Input.GetKey(KeyCode.UpArrow))
                    {
                        __instance.angle -= 0.10000000149011612;
                    }
                }
                float num = (float)__instance.angle;
                if (__instance.currfloor != null)
                {
                    if (__instance.controller.rotationEase != Ease.Linear)
                    {
                        float num2 = scrMisc.EasedAngle((float)___snappedLastAngle, (float)__instance.targetExitAngle, num, __instance.controller.rotationEase, __instance.controller.rotationEaseParts);
                        if (!float.IsNaN(num2) && !float.IsInfinity(num2))
                        {
                            num = num2;
                        }
                    }
                    if (__instance.controller.stickToFloor)
                    {
                        num -= (__instance.currfloor.transform.rotation.eulerAngles - __instance.currfloor.startRot).z * 0.017453292f;
                    }
                }
                Vector3 position = __instance.transform.position;
                __instance.other.transform.position = new Vector3(position.x + Mathf.Sin(num) * __instance.cosmeticRadius, position.y + Mathf.Cos(num) * __instance.cosmeticRadius, position.z);
                if (__instance.is3D)
                {
                    __instance.other.transform.position = new Vector3(position.x + Mathf.Sin((float)__instance.angle) * __instance.cosmeticRadius, position.y, position.z + Mathf.Cos((float)__instance.angle) * __instance.cosmeticRadius);
                }

                return false;
            }
        }

        [HarmonyPatch(typeof(scrController), "OnMusicScheduled")]
        private static class scrController_Rewind_Patch
        {
            public static void Postfix(scrController __instance)
            {
                InputFixerManager.jumpToOtherClass = true;
                __instance.conductor.Start();
            }
        }

        [HarmonyPatch(typeof(scrConductor), "Start")]
        private static class scrConductor_Start_Patch
        {
            public static bool Prefix(scrConductor __instance, double ___dspTimeSong)
            {
                if (InputFixerManager.jumpToOtherClass)
                {
                    InputFixerManager.jumpToOtherClass = false;
                    InputFixerManager.adjustOffsetTick(__instance, ___dspTimeSong);
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
                InputFixerManager.adjustOffsetTick(__instance, ___dspTimeSong);
                //NoStopMod.mod.Logger.Log("Rewind");
            }
        }

        [HarmonyPatch(typeof(scrConductor), "StartMusicCo")]
        private static class scrConductor_StartMusicCo_Patch
        {
            public static void Prefix(scrConductor __instance, double ___dspTimeSong)
            {
                InputFixerManager.adjustOffsetTick(__instance, ___dspTimeSong);
                //NoStopMod.mod.Logger.Log("StartMusicCo");
            }
        }

        [HarmonyPatch(typeof(scrConductor), "ScrubMusicToTile")]
        private static class scrConductor_ScrubMusicToTile_Patch
        {
            public static void Postfix(scrConductor __instance, double ___dspTimeSong)
            {
                InputFixerManager.adjustOffsetTick(__instance, ___dspTimeSong);
                //NoStopMod.mod.Logger.Log("ScrubMusicToTile");
            }
        }

        [HarmonyPatch(typeof(scrConductor), "DesyncFix")]
        private static class scrConductor_DesyncFix_Patch
        {
            public static void Postfix(scrConductor __instance, double ___dspTimeSong)
            {
                InputFixerManager.adjustOffsetTick(__instance, ___dspTimeSong);
                //NoStopMod.mod.Logger.Log("DesyncFix");
            }
        }


    }
}
