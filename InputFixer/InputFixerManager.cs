using NoStopMod.InputFixer.HitIgnore;
using System;
using System.Collections.Generic;
using SharpHook;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityModManagerNet;
using UnityEngine;

namespace NoStopMod.InputFixer
{
    class InputFixerManager
    {
        public static InputFixerSettings settings;

        public static Queue<Tuple<long, ushort, bool>> keyQueue = new Queue<Tuple<long, ushort, bool>>();

        public static long targetSongTick;

        public static bool jumpToOtherClass = false;
        public static bool editInputLimit = false;

        private static object hook;

        public static long currFrameTick;
        public static long prevFrameTick;

        public static double dspTime;
        public static double dspTimeSong;

        public static long offsetTick;

        public static double lastReportedDspTime;

        public static double previousFrameTime;

        public static readonly HashSet<ushort> mask = new HashSet<ushort>();

        public static bool disablingAdofaiTweaksKeyLimiter = false;

        public static void Init()
        {
            hook = new SimpleGlobalHook();
            IGlobalHook mHook = (IGlobalHook) hook;
            mHook.KeyPressed += HookOnKeyPressed;
            mHook.KeyReleased += HookOnKeyReleased;
            mHook.MousePressed += HookOnMousePressed;
            mHook.MouseReleased += HookOnMouseReleased;
            
            mHook.RunAsync();

            NoStopMod.onToggleListener.Add(_ => InitQueue());
            NoStopMod.onGUIListener.Add(OnGUI);

            settings = new InputFixerSettings();
            Settings.settings.Add(settings);

            disablingAdofaiTweaksKeyLimiter = UnityModManager.FindMod("AdofaiTweaks") != null;

            HitIgnoreManager.Init();
        }

        private static void OnGUI(UnityModManager.ModEntry entry)
        {
            if (disablingAdofaiTweaksKeyLimiter)
            {
                GUILayout.Label("AdofaiTweaks mod's key limiter has been disabled by NoStopMod's async input.");
            }
            
            settings.insertKeyOnWindowFocus = GUILayout.Toggle(settings.insertKeyOnWindowFocus, "Insert key on window focus");
        }

        public static void InitQueue()
        {
            currFrameTick = 0;
            prevFrameTick = 0;
            keyQueue.Clear();
            mask.Clear();
        }

        private static void HookOnKeyPressed(object sender, KeyboardHookEventArgs e)
        {
            ushort keyCode = (ushort) e.Data.KeyCode;

            if (!mask.Contains(keyCode))
            {
                mask.Add(keyCode);
                keyQueue.Enqueue(Tuple.Create(DateTime.Now.Ticks, keyCode, true));
#if DEBUG
                NoStopMod.mod.Logger.Log("eq " + keyCode);
#endif
            }
        }

        private static void HookOnKeyReleased(object sender, KeyboardHookEventArgs e)
        {
            ushort keyCode = (ushort) e.Data.KeyCode;
            mask.Remove(keyCode);
            keyQueue.Enqueue(Tuple.Create(DateTime.Now.Ticks, keyCode, false));
        }

        private static void HookOnMousePressed(object sender, MouseHookEventArgs e)
        {
            var keyCode = (ushort) (e.Data.Button + 1000);

            if (!mask.Contains(keyCode))
            {
                mask.Add(keyCode);
                keyQueue.Enqueue(Tuple.Create(DateTime.Now.Ticks, keyCode, true));
#if DEBUG
                NoStopMod.mod.Logger.Log("eq " + keyCode);
#endif
            }
        }
        
        private static void HookOnMouseReleased(object sender, MouseHookEventArgs e)
        {
            var keyCode = (ushort) (e.Data.Button + 1000);
            mask.Remove(keyCode);
            keyQueue.Enqueue(Tuple.Create(DateTime.Now.Ticks, keyCode, false));
        }

        public static double GetSongPosition(scrConductor __instance, long nowTick)
        {
            if (!GCS.d_oldConductor && !GCS.d_webglConductor)
            {
                return (nowTick / 10000000.0 - dspTimeSong - scrConductor.calibration_i) * __instance.song.pitch - __instance.addoffset;
            }
            else
            {
                return __instance.song.time - scrConductor.calibration_i - __instance.addoffset / __instance.song.pitch;
            }
        }

        public static double GetAngle(scrPlanet __instance, double ___snappedLastAngle, long nowTick)
        {
            return ___snappedLastAngle + (GetSongPosition(__instance.conductor, nowTick) - __instance.conductor.lastHit) / __instance.conductor.crotchet
                * 3.141592653598793238 * __instance.controller.speed * (__instance.controller.isCW ? 1 : -1);
        }

        public static bool Hit(scrController controller)
        {
            // @see scrController#PlayerControl_Update
            bool flag2 = (bool) (UnityEngine.Object) controller.chosenplanet.currfloor.nextfloor && controller.chosenplanet.currfloor.nextfloor.holdLength > -1;
            double num1 = scrMisc.GetAdjustedAngleBoundaryInDeg(HitMarginGeneral.Counted, (double) (controller.conductor.bpm * (float) controller.speed), (double) controller.conductor.song.pitch) * (Math.PI / 180.0);
            float num2 = (UnityEngine.Object) controller.chosenplanet.currfloor.nextfloor != (UnityEngine.Object) null ? (float) controller.chosenplanet.currfloor.nextfloor.marginScale : 1f;
            double num3 = 1.0 - num1 * (double) num2 / controller.chosenplanet.currfloor.angleLength;

#if DEBUG
            NoStopMod.mod.Logger.Log($"{controller.keyTimes.Count > 0}, {!GCS.d_stationary}, {!GCS.d_freeroam}, {(((controller.chosenplanet.currfloor.holdLength <= -1 ? 0 : (!controller.strictHolds ? 1 : 0)) | (flag2 ? 1 : 0)) != 0 || controller.chosenplanet.currfloor.holdLength == -1 || (double) controller.chosenplanet.currfloor.holdCompletion < num3)}, {(!RDC.auto || !scrController.isGameWorld)}, {(!(bool) (UnityEngine.Object) controller.mobileMenu || (bool) (UnityEngine.Object) controller.mobileMenu && scnMobileMenu.instance.mobileMenuPhase == scnMobileMenu.MobileMenuPhase.Road)}, {(!scrController.isGameWorld || controller.chosenplanet.currfloor.seqID < controller.lm.listFloors.Count - 1)}");
#endif
            
            if (controller.keyTimes.Count > 0 && 
                !GCS.d_stationary && 
                !GCS.d_freeroam && 
                (((controller.chosenplanet.currfloor.holdLength <= -1 ? 0 : (!controller.strictHolds ? 1 : 0)) | (flag2 ? 1 : 0)) != 0 || 
                 controller.chosenplanet.currfloor.holdLength == -1 || 
                 (double) controller.chosenplanet.currfloor.holdCompletion < num3) && 
                (!RDC.auto || !scrController.isGameWorld) && 
                (!(bool) (UnityEngine.Object) controller.mobileMenu || (bool) (UnityEngine.Object) controller.mobileMenu && scnMobileMenu.instance.mobileMenuPhase == scnMobileMenu.MobileMenuPhase.Road) && 
                (!scrController.isGameWorld || controller.chosenplanet.currfloor.seqID < controller.lm.listFloors.Count - 1)
                //&& !flag3 
                //&& !controller.benchmarkMode
                )
            {
                controller.keyTimes.RemoveAt(0);
                if (controller.Hit() && controller.chosenplanet.currfloor.holdLength > -1)
                {
                    controller.holdKeys.Clear();
                    //controller.IterateValidKeysHeld((Action<KeyCode?>) (k => controller.holdKeys.Add(k)), (Action<KeyCode?>) (k => controller.holdKeys.Remove(k)));
                }
                if (controller.midspinInfiniteMargin)
                {
                    controller.keyTimes.RemoveAt(0);
                    if (controller.Hit() && controller.chosenplanet.currfloor.holdLength > -1)
                    {
                        controller.holdKeys.Clear();
                        //controller.IterateValidKeysHeld((Action<KeyCode?>) (k => controller.holdKeys.Add(k)), (Action<KeyCode?>) (k => controller.holdKeys.Remove(k)));
                    }
                }

                return true;
            }
            else
            {
                controller.keyTimes.RemoveAt(0);
            }

            return false;
        }

        public static bool FailAction(scrController controller)
        {
            // @see scrController#PlayerControl_Update
            double num1 = scrMisc.GetAdjustedAngleBoundaryInDeg(HitMarginGeneral.Counted,
                (double) (controller.conductor.bpm * (float) controller.speed), (double) controller.conductor.song.pitch) * (Math.PI / 180.0);
            double num2 = Math.Max(3.14159274101257, num1 * 2.0);

            if (controller.noFail || controller.currFloor.isSafe)
                num2 = num1 * 1.01;

            double num3 = controller.chosenplanet.angle - controller.chosenplanet.targetExitAngle;

            if (!controller.isCW)
                num3 *= -1.0;

            // fail
            if (scrController.isGameWorld && controller.lm.listFloors.Count > controller.currFloor.seqID + 1 &&
                num3 > num2)
            {
                controller.FailAction();
                return true;
            }

            return false;
        }

        public static bool OttoHit(scrController controller)
        {
            bool flag1 = controller.chosenplanet.currfloor.nextfloor != null && controller.chosenplanet.currfloor.nextfloor.auto;
            if ((RDC.auto || flag1 || controller.chosenplanet.currfloor.holdLength > -1 && controller.chosenplanet.currfloor.auto) && scrController.isGameWorld)
            {
                if (controller.chosenplanet.AutoShouldHitNow())
                {
#if DEBUG
                    NoStopMod.mod.Logger.Log($"angle {controller.chosenplanet.angle}, {controller.chosenplanet.targetExitAngle}");           
#endif
                    
                    bool auto = RDC.auto;
                    RDC.auto = true;
                    if (controller.chosenplanet.currfloor.holdLength > -1)
                        controller.chosenplanet.currfloor.holdRenderer.Hit();
                    controller.keyTimes.Clear();
                    controller.Hit();

                    RDC.auto = auto;
                    return true;
                }
            }

            return false;
        }
        

        public static void HoldHit(scrController controller, bool inputReleased)
        {
            bool flag1 = (UnityEngine.Object) controller.chosenplanet.currfloor.nextfloor != (UnityEngine.Object) null && controller.chosenplanet.currfloor.nextfloor.auto;
            bool flag2 = (bool) (UnityEngine.Object) controller.chosenplanet.currfloor.nextfloor && controller.chosenplanet.currfloor.nextfloor.holdLength > -1;
            double num1 = scrMisc.GetAdjustedAngleBoundaryInDeg(HitMarginGeneral.Counted, (double) (controller.conductor.bpm * (float) controller.speed), (double) controller.conductor.song.pitch) * (Math.PI / 180.0);
            float num2 = (UnityEngine.Object) controller.chosenplanet.currfloor.nextfloor != (UnityEngine.Object) null ? (float) controller.chosenplanet.currfloor.nextfloor.marginScale : 1f;
            double num3 = 1.0 - num1 * (double) num2 / controller.chosenplanet.currfloor.angleLength;
            
            
            if (!controller.gameworld && controller.chosenplanet.currfloor.holdLength > -1)
            {
                float num7 = 3.141593f * (float) (controller.chosenplanet.currfloor.holdLength * 2 + 1);
                num3 = 1.0 - num1 * 1.0 / (double) num7;
                if (inputReleased)
                {
                    if ((double) controller.chosenplanet.currfloor.holdCompletion > num3)
                    {
                        controller.chosenplanet.currfloor.holdRenderer.Unfill();
                        controller.chosenplanet.currfloor.holdCompletion = 0.0f;
                        controller.Hit();
                    }
                    else
                    {
                        controller.chosenplanet.currfloor.holdRenderer.Unfill();
                        controller.chosenplanet.currfloor.holdCompletion = 0.0f;
                        controller.camy.Refocus(controller.chosenplanet.currfloor.prevfloor.transform);
                        DOTween.To((DOGetter<Vector3>) (() => controller.camy.holdOffset),
                                (DOSetter<Vector3>) (x => controller.camy.holdOffset = x), Vector3.zero, 0.3f)
                            .SetEase<TweenerCore<Vector3, Vector3, VectorOptions>>(Ease.OutCubic);
                        controller.chosenplanet.transform
                            .DOMove(controller.chosenplanet.currfloor.prevfloor.transform.position, 0.4f)
                            .SetEase<TweenerCore<Vector3, Vector3, VectorOptions>>(Ease.OutCubic);
                        controller.chosenplanet.currfloor = controller.chosenplanet.currfloor.prevfloor;
                        scrFlash.OnDamage();
                        controller.LockInput(0.4f);
                    }
                }

                if ((double) controller.chosenplanet.currfloor.holdCompletion > 2.0 - num3 ||
                    (double) controller.chosenplanet.currfloor.holdCompletion < -0.3)
                {
                    controller.chosenplanet.currfloor.holdRenderer.Unfill(false);
                    controller.chosenplanet.currfloor.holdCompletion = 0.0f;
                    controller.camy.Refocus(controller.chosenplanet.currfloor.prevfloor.transform);
                    DOTween.To((DOGetter<Vector3>) (() => controller.camy.holdOffset),
                            (DOSetter<Vector3>) (x => controller.camy.holdOffset = x), Vector3.zero, 0.3f)
                        .SetEase<TweenerCore<Vector3, Vector3, VectorOptions>>(Ease.OutCubic);
                    controller.chosenplanet.transform.DOMove(controller.chosenplanet.currfloor.prevfloor.transform.position, 0.4f)
                        .SetEase<TweenerCore<Vector3, Vector3, VectorOptions>>(Ease.OutCubic);
                    controller.chosenplanet.currfloor = controller.chosenplanet.currfloor.prevfloor;
                    scrFlash.OnDamage();
                    controller.LockInput(0.4f);
                }
            }

            if (controller.gameworld && inputReleased && !flag1 && !controller.chosenplanet.currfloor.auto &&
                !RDC.auto && controller.chosenplanet.currfloor.holdLength > -1)
            {
                if ((double) controller.chosenplanet.currfloor.holdCompletion < num3)
                {
                    if (GCS.checkpointNum != controller.chosenplanet.currfloor.seqID && controller.requireHolding)
                        controller.FailAction();
                }
                else if (!flag2)
                {
                    controller.chosenplanet.currfloor.holdRenderer.Hit();
                    controller.Hit();
                    //flag3 = true;
                }
            }
        }

        public static void AdjustAngle(scrController controller, long targetTick)
        {
            InputFixerManager.targetSongTick = targetTick - InputFixerManager.offsetTick;
            InputFixerManager.jumpToOtherClass = true;
            controller.chosenplanet.Update_RefreshAngles();
        }

    }

}
