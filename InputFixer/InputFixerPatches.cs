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

                while (InputFixerManager.keyQueue.TryDequeue(out var keyEvent))
                {
                    if (keyEvent.tick != rawKeyCodesTick)
                    {
                        ProcessKeyInputs(pressKeyCodes, rawKeyCodesTick);
                        rawKeyCodesTick = keyEvent.tick;
                        pressKeyCodes.Clear();
                    }
                    
                    if (keyEvent.press)
                    {
                        if (!InputFixerManager.keyMask.Contains(keyEvent.keyCode))
                        {
                            InputFixerManager.keyMask.Add(keyEvent.keyCode);
                            pressKeyCodes.Add((KeyCode) keyEvent.keyCode);
                        }
                    }
                    else
                    {
                        InputFixerManager.keyMask.Remove(keyEvent.keyCode);
                    }
                }

                ProcessKeyInputs(pressKeyCodes, rawKeyCodesTick);
            }



            private static void ProcessKeyInputs([NotNull] IReadOnlyList<KeyCode> keyCodes, long eventTick)
            {
                if (InputFixerManager.settings.insertKeyOnWindowFocus && !Application.isFocused)
                {
                    InputFixerManager.countValidKeysPressed = 0;
                }
                else
                {
                    InputFixerManager.countValidKeysPressed = GetValidKeyCount(keyCodes);
                }


                var controller = scrController.instance;
                var targetTick = eventTick != 0 ? eventTick : InputFixerManager.currFrameTick;

                if ((scrController.States) controller.GetState() == scrController.States.PlayerControl &&
                    (scrController.States) InputFixerManager.DestinationStateReflectionField
                        .GetValue(controller.stateMachine).state == scrController.States.PlayerControl)
                {
                    InputFixerManager.PlayerControl_Update(controller, targetTick);
                }
            }

            private static int GetValidKeyCount([NotNull] IReadOnlyList<KeyCode> keyCodes)
            {
                var count = 0;
                for (var i = 0; i < keyCodes.Count; i++)
                {
                    if (HitIgnoreManager.ShouldBeIgnored(keyCodes[i])) continue;

                    // if (AudioListener.pause || RDC.auto) continue;
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
                __result = InputFixerManager.countValidKeysPressed;
            }
        }
        
        [HarmonyPatch(typeof(scrController), "PlayerControl_Update")]
        private static class scrController_PlayerControl_Update_Patch
        {
            public static bool Prefix(scrController __instance)
            {
                return false;
            }
        }

        [HarmonyPatch(typeof(scrController), "holding", MethodType.Getter)]
        private static class scrController_holding_Patch
        {
            public static bool Prefix(scrController __instance, ref bool __result)
            {
                __result = InputFixerManager.holdKeys.Count != 0;
                return false;
            }
        }
        
        [HarmonyPatch(typeof(scrController), "ValidInputWasTriggered")]
        private static class scrController_ValidInputWasTriggered_Patch
        {
            public static bool Prefix(scrController __instance, ref bool __result)
            {
                // __result = InputFixerManager.validKeyWasTriggered;
                __result = InputFixerManager.ValidInputWasTriggered(__instance);
                return false;
            }
        }
        
        [HarmonyPatch(typeof(scrController), "ValidInputWasReleased")]
        private static class scrController_ValidInputWasReleased_Patch
        {
            public static bool Prefix(scrController __instance, ref bool __result)
            {
                __result = InputFixerManager.ValidInputWasReleased(scrController.instance);
                return false;
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
                        //NoStopMod.mod.Logger.Log($"angle diff={difference}, songTick={InputFixerManager.targetSongTick}, ___snappedLastAngle={___snappedLastAngle}, offsetTick={InputFixerManager.offsetTick}, targetTick={InputFixerManager.targetSongTick + InputFixerManager.offsetTick}");
                    }
#endif
                    return false;
                }

                return true;
            }
        }
        
    }
    
}
