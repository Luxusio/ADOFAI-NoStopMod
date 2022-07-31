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
                    InputFixerManager.offsetTickUpdated = true;
                }

                InputFixerManager.dspTimeSong = ___dspTimeSong;

                // planet hit processing
                long rawKeyCodesTick = 0;
                InputFixerManager.keyDownMask.Clear();
                var pressKeyCodes = new List<KeyCode>();

                while (InputFixerManager.keyQueue.TryDequeue(out var keyEvent))
                {
                    if (keyEvent.tick != rawKeyCodesTick)
                    {
                        if (rawKeyCodesTick != 0)
                        {
                            ProcessKeyInputs(pressKeyCodes, rawKeyCodesTick);
                        }

                        rawKeyCodesTick = keyEvent.tick;
                        pressKeyCodes.Clear();
                        InputFixerManager.keyDownMask.Clear();
                    }

                    ushort keyCode = KeyCodeHelper.ConvertNativeKeyCode(keyEvent.keyCode);
                    
                    if (keyEvent.press)
                    {
                        if (!InputFixerManager.keyMask.Contains(keyCode))
                        {
#if DEBUG
                            NoStopMod.mod.Logger.Log($"[{Time.frameCount}] press {(KeyCode) keyCode}");
#endif
                            InputFixerManager.keyMask.Add(keyCode);
                            InputFixerManager.keyDownMask.Add(keyCode);
                            pressKeyCodes.Add((KeyCode) keyCode);
                        }
#if DEBUG
                        else
                        {
                            NoStopMod.mod.Logger.Log($"[{Time.frameCount}] press fail {(KeyCode) keyCode}");
                        }
#endif
                    }
                    else
                    {
                        if (InputFixerManager.keyMask.Remove(keyCode))
                        {
#if DEBUG
                            NoStopMod.mod.Logger.Log($"[{Time.frameCount}] release {(KeyCode) keyCode}");
#endif
                        }
#if DEBUG
                        else
                        {
                            NoStopMod.mod.Logger.Log($"[{Time.frameCount}] release fail {(KeyCode) keyCode}");
                        }
#endif 
                    }
                }

                // 키 씹힘 안정화
                LinkedList<ushort> ignoredPressKeys = new(); // 씹힌 키 목록
                LinkedList<ushort> ignoredReleaseKeys = new(); // 씹힌 키 목록

                for (int i = 0; i < KeyCodeHelper.UnityNativeKeyCodeList.Count; i++)
                {
                    var tuple = KeyCodeHelper.UnityNativeKeyCodeList[i];
                    if (Input.GetKeyUp(tuple.Item1))
                    {
                        if (InputFixerManager.keyMask.Contains(tuple.Item2))
                        {
#if DEBUG
                            NoStopMod.mod.Logger.Log($"[{Time.frameCount}] Fix troll release key {(KeyCode) tuple.Item2}, {tuple.Item1}");
#endif
                            ignoredReleaseKeys.AddLast(tuple.Item2);
                        }
#if DEBUG
                        else
                        {
                            NoStopMod.mod.Logger.Log($"[{Time.frameCount}] Just release key {(KeyCode) tuple.Item2}, {tuple.Item1}");
                        }
#endif
                    }
                    else if (Input.GetKeyDown(tuple.Item1))
                    {
                        if (!InputFixerManager.keyMask.Contains(tuple.Item2))
                        {
#if DEBUG
                            NoStopMod.mod.Logger.Log($"[{Time.frameCount}] Fix troll press key {(KeyCode) tuple.Item2}, {tuple.Item1}");
#endif
                            ignoredPressKeys.AddLast(tuple.Item2);
                        }
#if DEBUG
                        else
                        {
                            NoStopMod.mod.Logger.Log($"[{Time.frameCount}] Just press key {(KeyCode) tuple.Item2}, {tuple.Item1}");
                        }
#endif
                    }
                }

                if (ignoredPressKeys.Count > 0 || ignoredReleaseKeys.Count > 0)
                {
                    if (rawKeyCodesTick != InputFixerManager.currFrameTick)
                    {
                        ProcessKeyInputs(pressKeyCodes, rawKeyCodesTick);
                        rawKeyCodesTick = InputFixerManager.currFrameTick;
                        pressKeyCodes.Clear();
                        InputFixerManager.keyDownMask.Clear();
                    }

                    foreach (var ignoredPressKey in ignoredPressKeys)
                    {
                        pressKeyCodes.Add((KeyCode) ignoredPressKey);
                        InputFixerManager.keyMask.Add(ignoredPressKey);
                        InputFixerManager.keyDownMask.Add(ignoredPressKey);
                    }
                    
                    foreach (var ignoredReleaseKey in ignoredReleaseKeys)
                    {
                        InputFixerManager.keyMask.Remove(ignoredReleaseKey);
                    }
                }
                
                ProcessKeyInputs(pressKeyCodes, rawKeyCodesTick);
            }



            private static void ProcessKeyInputs([NotNull] IReadOnlyList<KeyCode> keyCodes, long eventTick)
            {
#if  DEBUG
                {
                    //NoStopMod.mod.Logger.Log("-");
                }
#endif
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

                
                scrController.States state = (scrController.States) controller.GetState();
                scrController.States targetState = (scrController.States) InputFixerManager.DestinationStateReflectionField.GetValue(controller.stateMachine).state;
                
#if  DEBUG
                // if (InputFixerManager.countValidKeysPressed > 0 || InputFixerManager.validKeyWasReleased)
                // {
                //     NoStopMod.mod.Logger.Log($"PlayerControl before ({state}, {targetState}, {InputFixerManager.validKeyWasReleased}, {InputFixerManager.countValidKeysPressed}, {targetTick}), {controller.currFloor.seqID}th tile");
                // }
#endif
                if (state == scrController.States.PlayerControl &&
                    targetState == scrController.States.PlayerControl)
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
                    if (++count > 4) break;
                }

                return count;
            }

        }
        
        [HarmonyPatch(typeof(scrCamera), "Update")]
        private static class scrCamera_Update_Patch
        {
            public static void Postfix(scrCamera __instance)
            {
                if (InputFixerManager.camyToPosChanged)
                {
                    __instance.topos.x = InputFixerManager.overrideCamyToPos[0];
                    __instance.topos.y = InputFixerManager.overrideCamyToPos[1];
                    __instance.topos.z = InputFixerManager.overrideCamyToPos[2];
                }
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
                
#if DEBUG
                if (__result)
                {
                    NoStopMod.mod.Logger.Log($"[{Time.frameCount}] ValidInputWasTriggered!!!!");
                }
#endif
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
        
        [HarmonyPatch(typeof(scrCreditsText), "Start")]
        private static class scrCreditsText_Start_Patch
        {
            public static void Postfix(scrCreditsText __instance)
            {
                if (scnLevelSelectTaro.instance != null && scnLevelSelectTaro.instance.scene.creditsContentCopy == null)
                {
                    scnLevelSelectTaro.instance.scene.creditsContentCopy = __instance.contentCopy;
                }
            }
        }

        [HarmonyPatch(typeof(scrController), "TogglePauseGame")]
        private static class scrController_TogglePauseGame_Patch
        {
            public static void Postfix()
            {
                InputFixerManager.offsetTickUpdated = false;
            }
        }
        
    }
    
}
