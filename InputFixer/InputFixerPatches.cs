using DG.Tweening;
using HarmonyLib;
using NoStopMod.InputFixer.HitIgnore;
using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using KeyCode = SharpHook.Native.KeyCode;

namespace NoStopMod.InputFixer
{
    
    public static class AsyncInputPatches
    {
        
        [HarmonyPatch(typeof(scrController), "Awake")]
        private static class scrController_Awake_Patch
        {
            public static void Postfix(scrController __instance)
            {
                InputFixerManager.InitQueue();
            }
        }

        [HarmonyPatch(typeof(scrConductor), "Update")]
        private static class scrConductor_Update_Patch
        {
            public static void Postfix(scrConductor __instance, double ___dspTimeSong)
            {
                // frameMs set
                InputFixerManager.prevFrameTick = InputFixerManager.currFrameTick;
                InputFixerManager.currFrameTick = DateTime.Now.Ticks;
                
                // dspTime adjust
                if (!AudioListener.pause && Application.isFocused && Time.unscaledTime - InputFixerManager.previousFrameTime < 0.1)
                {
                    InputFixerManager.dspTime += Time.unscaledTime - InputFixerManager.previousFrameTime;
                }
                InputFixerManager.previousFrameTime = Time.unscaledTime;

                if (AudioSettings.dspTime - InputFixerManager.lastReportedDspTime != 0)
                {
                    InputFixerManager.lastReportedDspTime = AudioSettings.dspTime;
                    InputFixerManager.dspTime = AudioSettings.dspTime;
                    InputFixerManager.offsetMs = InputFixerManager.currFrameTick - (long)(InputFixerManager.dspTime * 10000000);
                }

                InputFixerManager.dspTimeSong = ___dspTimeSong;

                // planet hit processing
                long rawKeyCodesTick = 0;
                var keyCodes = new List<KeyCode>();

                while (InputFixerManager.keyQueue.Any())
                {
                    InputFixerManager.keyQueue.Dequeue().Deconstruct(out var ms, out var ushortRawKeyCode);

                    var rawKeyCode = (KeyCode) ushortRawKeyCode;

                    if (ms != rawKeyCodesTick)
                    {
                        ProcessKeyInputs(keyCodes, rawKeyCodesTick);
                        keyCodes.Clear();
                        rawKeyCodesTick = ms;
                    }

                    keyCodes.Add(rawKeyCode);
                }

                ProcessKeyInputs(keyCodes, rawKeyCodesTick);
            }

            private static void ProcessKeyInputs([NotNull] IReadOnlyList<KeyCode> keyCodes, long ms)
            {
                var count = GetValidKeyCount(keyCodes);

                InputFixerManager.currPressTick = ms - InputFixerManager.offsetMs;
                
                var controller = scrController.instance;
                for (var i = 0; i < count; i++)
                {
                    controller.keyTimes.Add(0);
                }
                
                while (controller.keyTimes.Count > 0)
                {
                    AccurateHit(controller);
                    if (controller.midspinInfiniteMargin)
                    {
                        AccurateHit(controller);
                    }
                }
                
            }

            private static int GetValidKeyCount([NotNull] IReadOnlyList<KeyCode> keyCodes)
            {
                var count = 0;
                for (var i = 0; i < keyCodes.Count(); i++)
                {
                    if (HitIgnoreManager.ShouldBeIgnored(keyCodes[i])) continue;

                    if (AudioListener.pause || RDC.auto) continue;
#if DEBUG
                    NoStopMod.mod.Logger.Log("Fetch Input : " + InputFixerManager.offsetMs + ", " + keyCodes[i]);
                    
#endif
                    if (++count > 4) break;
                }

                return count;
            }

            private static void AccurateHit(scrController controller)
            {
                InputFixerManager.jumpToOtherClass = true;
                controller.chosenplanet.Update_RefreshAngles();
                controller.keyTimes.RemoveAt(0);
                controller.Hit();
            }
            
        }

        [HarmonyPatch(typeof(scrController), "CountValidKeysPressed")]
        private static class scrController_CountValidKeysPressed_Patch
        {
            public static bool Prefix(scrController __instance, ref int __result)
            {
                return false;
            }

            public static void Postfix(ref int __result)
            {
                __result = 0;
            }
        }

        [HarmonyPatch(typeof(scrPlanet), "Update_RefreshAngles")]
        private static class scrPlanet_Update_RefreshAngles_Patch
        {
            public static bool Prefix(scrPlanet __instance, ref double ___snappedLastAngle)
            {

                if (InputFixerManager.jumpToOtherClass)
                {
                    InputFixerManager.jumpToOtherClass = false;
                    __instance.angle = InputFixerManager.GetAngle(__instance, ___snappedLastAngle, InputFixerManager.currPressTick);
#if DEBUG
                    {
                        var difference = __instance.angle - __instance.targetExitAngle;
                        NoStopMod.mod.Logger.Log("Diff : " + difference);
                    }
#endif
                    return false;
                }

                if (!__instance.isChosen || __instance.conductor.crotchet == 0.0) return false;

                if (!GCS.d_stationary)
                {
                    var nowMilliseconds = InputFixerManager.currFrameTick - InputFixerManager.offsetMs;
                    __instance.angle = InputFixerManager.GetAngle(__instance, ___snappedLastAngle, nowMilliseconds);

                    if (__instance.shouldPrint)
                    {
                        __instance.shouldPrint = false;
                    }
                }
                else
                {
                    if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.DownArrow))
                    {
                        __instance.angle += 0.1;
                    }
                    if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.UpArrow))
                    {
                        __instance.angle -= 0.1;
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
        
        
    }
}
