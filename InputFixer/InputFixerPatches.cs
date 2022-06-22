using DG.Tweening;
using HarmonyLib;
using NoStopMod.InputFixer.HitIgnore;
using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NoStopMod.Helper;
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
        
        [HarmonyPatch(typeof(scrController), "PlayerControl_Update")]
        private static class scrController_PlayerControl_Update_Patch
        {
            public static void Postfix(scrController __instance)
            {
                ControllerHelper.ExecuteUntilTileNotChange(__instance, () =>
                {
                    InputFixerManager.AdjustAngle(__instance, DateTime.Now.Ticks);
                    InputFixerManager.OttoHit(__instance);
#if DEBUG
                    NoStopMod.mod.Logger.Log($"OttoHit before hit {__instance.currFloor.seqID}th tile");
#endif
                });
                ControllerHelper.ExecuteUntilTileNotChange(__instance, () =>
                {
                    InputFixerManager.AdjustAngle(__instance, DateTime.Now.Ticks);
                    InputFixerManager.FailAction(__instance);
#if DEBUG
                    NoStopMod.mod.Logger.Log($"FailAction from update {__instance.currFloor.seqID}th tile");
#endif
                });
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
                    InputFixerManager.offsetTick = InputFixerManager.currFrameTick - (long)(InputFixerManager.dspTime * 10000000);
                }

                InputFixerManager.dspTimeSong = ___dspTimeSong;

                // planet hit processing
                long rawKeyCodesTick = 0;
                var pressKeyCodes = new List<KeyCode>();

                while (InputFixerManager.keyQueue.Any())
                {
                    InputFixerManager.keyQueue.Dequeue().Deconstruct(out var eventTick, out var ushortRawKeyCode);

                    var rawKeyCode = (KeyCode) ushortRawKeyCode;
                    
                    if (eventTick != rawKeyCodesTick)
                    {
                        ProcessKeyInputs(pressKeyCodes, rawKeyCodesTick);
                        pressKeyCodes.Clear();
                        rawKeyCodesTick = eventTick;
                    }

                    pressKeyCodes.Add(rawKeyCode);

                }

                ProcessKeyInputs(pressKeyCodes, rawKeyCodesTick);
            }



            private static void ProcessKeyInputs([NotNull] IReadOnlyList<KeyCode> keyCodes, long eventTick)
            {
                var count = GetValidKeyCount(keyCodes);
                var controller = scrController.instance;
                if (count == 1)
                {
                    controller.consecMultipressCounter = 0;
                }
                ControllerHelper.ExecuteUntilTileNotChange(controller, () =>
                {
                    InputFixerManager.AdjustAngle(controller, eventTick);
                    InputFixerManager.OttoHit(controller);
#if DEBUG
                    NoStopMod.mod.Logger.Log($"OttoHit before hit {controller.currFloor.seqID}th tile");
#endif
                });
                ControllerHelper.ExecuteUntilTileNotChange(controller, () =>
                {
                    InputFixerManager.AdjustAngle(controller, eventTick);
                    InputFixerManager.FailAction(controller);
#if DEBUG
                    NoStopMod.mod.Logger.Log($"FailAction before hit {controller.currFloor.seqID}th tile");
#endif
                });
                
                for (var i = 0; i < count; i++)
                {
                    controller.keyTimes.Add(0);
                }
                
                while (controller.keyTimes.Count > 0)
                {
                    InputFixerManager.AdjustAngle(controller, eventTick);
                    InputFixerManager.Hit(controller);
                    if (controller.midspinInfiniteMargin)
                    {
                        InputFixerManager.AdjustAngle(controller, eventTick);
                        InputFixerManager.Hit(controller);
                    }
                }
                
            }

            private static int GetValidKeyCount([NotNull] IReadOnlyList<KeyCode> keyCodes)
            {
                var count = 0;
                for (var i = 0; i < keyCodes.Count; i++)
                {
                    if (HitIgnoreManager.ShouldBeIgnored(keyCodes[i])) continue;

                    if (AudioListener.pause || RDC.auto) continue;
#if DEBUG
                    NoStopMod.mod.Logger.Log("Fetch Input : " + InputFixerManager.offsetTick + ", " + keyCodes[i]);
                    
#endif
                    if (++count > 4) break;
                }

                return count;
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
                    __instance.angle = InputFixerManager.GetAngle(__instance, ___snappedLastAngle, InputFixerManager.targetSongTick);
#if DEBUG
                    {
                        var difference = __instance.angle - __instance.targetExitAngle;
                        NoStopMod.mod.Logger.Log("Diff : " + difference);
                    }
#endif
                    return false;
                }

                return true;
            }
        }
        
        
    }
}
