using NoStopMod.InputFixer.HitIgnore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using SharpHook;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using MonsterLove.StateMachine;
using NoStopMod.Helper;
using UnityModManagerNet;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NoStopMod.InputFixer
{
    class InputFixerManager
    {
        public static InputFixerSettings settings;

        public static readonly ReflectionField<StateEngine, StateMapping> DestinationStateReflectionField = new("destinationState");
        public static readonly ReflectionField<scrController, bool> BenchmarkModeReflectionField = new("benchmarkMode");
        public static readonly ReflectionField<scrController, bool> ExitingToMainMenuReflectionField = new("exitingToMainMenu");
        public static readonly ReflectionField<scrController, bool> ValidInputWasReleasedThisFrameReflectionField = new("validInputWasReleasedThisFrame");

        public static ConcurrentQueue<KeyEvent> keyQueue = new();

        public static long targetSongTick;

        public static bool jumpToOtherClass = false;
        public static bool editInputLimit = false;

        private static object hook_;

        public static long currFrameTick;
        public static long prevFrameTick;

        public static double dspTime;
        public static double dspTimeSong;

        public static long offsetTick;
        public static bool offsetTickUpdated = false;

        public static double lastReportedDspTime;

        public static double previousFrameTime;


        
        //////////////////////////////////////////////////////////
        // scope : each timing
        public static readonly HashSet<ushort> keyMask = new();
        public static readonly HashSet<ushort> keyDownMask = new();
        //public static bool validKeyWasTriggered = false;
        public static readonly HashSet<ushort> holdKeys = new();
        public static bool validKeyWasReleased = false;
        public static int countValidKeysPressed;
		
        //
        //////////////////////////////////////////////////////////
        
        public static bool disablingAdofaiTweaksKeyLimiter = false;

        private static Thread inputHookThread;

        public static void Init()
        {
            IGlobalHook mHook = new SimpleGlobalHook();
            mHook.KeyPressed += HookOnKeyPressed;
            mHook.KeyReleased += HookOnKeyReleased;
            mHook.MousePressed += HookOnMousePressed;
            mHook.MouseReleased += HookOnMouseReleased;
            hook_ = mHook;

            NoStopMod.onToggleListener.Add(OnToggle);
            NoStopMod.onGUIListener.Add(OnGUI);
            NoStopMod.onApplicationQuitListener.Add(OnApplicationQuit);

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
        
        public static void OnToggle(bool toggle)
        {
            InitQueue();
            if (toggle)
            {
                StartHook();
            }
            else
            {
                StopHook();
            }

        }

        public static void OnApplicationQuit(scrController controller)
        {
            StopHook();
        }

        public static void InitQueue()
        {
            currFrameTick = 0;
            prevFrameTick = 0;
            while (keyQueue.TryDequeue(out _))
            {
                // do nothing
            }
            keyMask.Clear();
        }
        
        public static void StartHook()
        {
            inputHookThread = new Thread(() =>
            {

                bool stopHook = false;
                try
                {
                    IGlobalHook mHook = new SimpleGlobalHook();
                    mHook.KeyPressed += HookOnKeyPressed;
                    mHook.KeyReleased += HookOnKeyReleased;
                    mHook.MousePressed += HookOnMousePressed;
                    mHook.MouseReleased += HookOnMouseReleased;
                    hook_ = mHook;
                    
                    mHook.Run();
                }
                catch (HookException e)
                {
                    if (e.GetBaseException() is ThreadAbortException)
                    {
                        stopHook = true;
                    }
                    else
                    {
                        NoStopMod.mod.Logger.Error($"[{Time.frameCount}] Exception while hook run : {e}");
                    }
                }
                catch (Exception e)
                {
                    NoStopMod.mod.Logger.Error($"[{Time.frameCount}] Exception while hook run : {e}");
                }
                finally
                {
                    if (!stopHook)
                    {
                        StartHook();
                    }
                }
            });
            
            inputHookThread.Start();
        }

        public static void StopHook()
        {
            try
            {
                if (inputHookThread != null && inputHookThread.IsAlive)
                {
                    inputHookThread.Abort();
                }

                ((IGlobalHook) hook_).Dispose();
            }
            catch (Exception e)
            {
                NoStopMod.mod.Logger.Error($"[{Time.frameCount}] Exception while StopHook : {e}");
            }
        }


        private static void HookOnKeyPressed(object sender, KeyboardHookEventArgs e)
        {
            ushort keyCode = (ushort) e.Data.KeyCode;
            keyQueue.Enqueue(new KeyEvent(DateTime.Now.Ticks, keyCode, true));
        }

        private static void HookOnKeyReleased(object sender, KeyboardHookEventArgs e)
        {
            ushort keyCode = (ushort) e.Data.KeyCode;
            keyQueue.Enqueue(new KeyEvent(DateTime.Now.Ticks, keyCode, false));
        }

        private static void HookOnMousePressed(object sender, MouseHookEventArgs e)
        {
            var keyCode = (ushort) (e.Data.Button + 1000);
            keyQueue.Enqueue(new KeyEvent(DateTime.Now.Ticks, keyCode, true));
        }
        
        private static void HookOnMouseReleased(object sender, MouseHookEventArgs e)
        {
            var keyCode = (ushort) (e.Data.Button + 1000);
            keyQueue.Enqueue(new KeyEvent(DateTime.Now.Ticks, keyCode, false));
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

        
        public static void PlayerControl_Update(scrController controller, long targetTick)
        {
           		//printe ($"paused {paused} || chosenplanet.currfloor == null {chosenplanet.currfloor == null} || isCutscene {isCutscene} == {paused || chosenplanet.currfloor == null || isCutscene}");
	        
			if (controller.paused || controller.chosenplanet.currfloor == null || controller.isCutscene || !offsetTickUpdated)
				return;
			// 대부분의 변수 선언, 각도 계산 로직 등은 ExecuteUntilTileNotChange block안으로 들어갔습니다.
			// block 안으로 들어가는건 함수로 빼주길 바람.

			ValidInputWasReleasedThisFrameReflectionField.SetValue(controller, controller.ValidInputWasReleased());
#if DEBUG
	        if (ValidInputWasReleasedThisFrameReflectionField.GetValue(controller))
	        {
		        NoStopMod.mod.Logger.Log($"[{Time.frameCount}] Valid input was released this frame");
	        }
#endif

			//Taro: fail if we let go before 45 degrees from the end.
			//currfloor.holdCompletion and currfloor.angleLength vars are used for this.
			//float marginScale = controller.chosenplanet.currfloor.nextfloor != null ? (float)controller.chosenplanet.currfloor.nextfloor.marginScale : 1;
			//var holdMargin = 1.0f - ((minAngleMargin * marginScale) / controller.chosenplanet.currfloor.angleLength);

			// Fail when past the max of 180 degrees or 2x the latest possible time to hit.
			// At some BPMs, 180 degrees is smaller than the valid hit window.


			ControllerHelper.ExecuteUntilTileNotChange(controller, () =>
			{
				InputFixerManager.AdjustAngle(scrController.instance, targetTick);
				
				double minAngleMargin = scrMisc.GetAdjustedAngleBoundaryInDeg (
					HitMarginGeneral.Counted,
					(float)(controller.conductor.bpm * controller.speed),
					controller.conductor.song.pitch) * Mathf.Deg2Rad;
				
				double extraRadiansBeforeDeath = Math.Max (Mathf.PI, minAngleMargin * 2);
				if (controller.noFail || controller.currFloor.isSafe)
				{
					// "Fail" very soon after hit window passes if no-fail is enabled
					extraRadiansBeforeDeath = minAngleMargin * 1.01;
				}
				
				double angleDiff = controller.chosenplanet.angle - controller.chosenplanet.targetExitAngle;
				if (!controller.isCW)
				{
					angleDiff *= -1;
				}
				
				if (controller.gameworld
				    && controller.lm.listFloors.Count > controller.currFloor.seqID + 1 // i.e. must be at least two higher
				    && angleDiff > extraRadiansBeforeDeath)
				{
					controller.FailAction();
#if  DEBUG
					NoStopMod.mod.Logger.Log($"[{Time.frameCount}] CheckForFail FailAction from update {controller.currFloor.seqID}th tile");
#endif
				}
			});
			//Check for fail

			ControllerHelper.ExecuteUntilTileNotChange(controller, () =>
			{
				InputFixerManager.AdjustAngle(scrController.instance, targetTick);
				
				bool nextTileIsAuto = controller.chosenplanet.currfloor.nextfloor != null && controller.chosenplanet.currfloor.nextfloor.auto;
				
				//check for d_autohit, and TRIGGER it
				if ((RDC.auto || BenchmarkModeReflectionField.GetValue(controller) || nextTileIsAuto ||
				     controller.chosenplanet.currfloor.holdLength > -1 && controller.chosenplanet.currfloor.auto) &&
				    controller.gameworld) // changed if components
				{
					if (controller.chosenplanet.AutoShouldHitNow())
					{

						bool auto = RDC.auto;
						RDC.auto = true;
						
						if (controller.chosenplanet.currfloor.holdLength > -1)
							controller.chosenplanet.currfloor.holdRenderer.Hit();

						controller.keyTimes.Clear(); //important so autohit isnt detected as a multipress
						controller.Hit();
#if  DEBUG
						NoStopMod.mod.Logger.Log($"[{Time.frameCount}] Otto Hit from update {controller.currFloor.seqID}th tile");
#endif

						RDC.auto = auto;

					}
				}
			});


			// if(newPressCount > 0)
			// 	printe_frame("newPressCount: " + newPressCount);


			// var floorTapCount = 1;
			// scrFloor nextFloor = null;
			// if (controller.currFloor != null && controller.currFloor.nextfloor != null)
			// {
			// 	nextFloor = controller.currFloor.nextfloor;
			// 	floorTapCount = nextFloor.tapsNeeded;
			// }

			{
				InputFixerManager.AdjustAngle(scrController.instance, targetTick);
				bool nextTileIsAuto = controller.chosenplanet.currfloor.nextfloor != null &&
				                      controller.chosenplanet.currfloor.nextfloor.auto;

				if (controller.ValidInputWasTriggered() && (!nextTileIsAuto || (nextTileIsAuto && controller.chosenplanet.currfloor.freeroam)))
				{
					int newPressCount = InputFixerManager.countValidKeysPressed;

					for (var i = 0; i < newPressCount; i++)
					{
						var t = Time.timeAsDouble;
						controller.keyTimes.Add(t);
						// print($"new hit {t}");
					}

					if (newPressCount == 1) controller.consecMultipressCounter = 0; //reset the counter!
				}
			}

			// bool alreadyHitHoldEnding = false;

			{ // 홀드는 한 타이밍에 한개씩만 처리
				InputFixerManager.AdjustAngle(scrController.instance, targetTick);
				
				
				bool nextTileIsAuto = controller.chosenplanet.currfloor.nextfloor != null && controller.chosenplanet.currfloor.nextfloor.auto;
				bool nextTileIsHold = controller.chosenplanet.currfloor.nextfloor && controller.chosenplanet.currfloor.nextfloor.holdLength > -1;

				double minAngleMargin = scrMisc.GetAdjustedAngleBoundaryInDeg (
					HitMarginGeneral.Counted,
					(float)(controller.conductor.bpm * controller.speed),
					controller.conductor.song.pitch) * Mathf.Deg2Rad;

				float marginScale = controller.chosenplanet.currfloor.nextfloor != null ? (float)controller.chosenplanet.currfloor.nextfloor.marginScale : 1;
				var holdMargin = 1.0f - ((minAngleMargin * marginScale) / controller.chosenplanet.currfloor.angleLength);
				
				// for scene
				if (!controller.gameworld && controller.chosenplanet.currfloor.holdLength > -1)
				{
					float holdAngleLength = Mathf.PI * (controller.chosenplanet.currfloor.holdLength * 2 + 1);
					holdMargin = 1.0f - ((minAngleMargin * 1f) / holdAngleLength);

					if ((ValidInputWasReleasedThisFrameReflectionField.GetValue(controller)) || (controller.ValidInputWasTriggered() && !controller.strictHolds))
					{
						// a > 0.97 && a < 1.03 (1 + (1-0.97))
						if (controller.chosenplanet.currfloor.holdCompletion > holdMargin)
						{
							controller.chosenplanet.currfloor.holdRenderer.Unfill();
							controller.chosenplanet.currfloor.holdCompletion = 0;

							controller.Hit();
#if  DEBUG
							NoStopMod.mod.Logger.Log($"[{Time.frameCount}] hold Hit from update {controller.currFloor.seqID}th tile");
#endif
						}
						else
						{
							controller.chosenplanet.currfloor.holdRenderer.Unfill();
							controller.chosenplanet.currfloor.holdCompletion = 0;

							controller.camy.Refocus(controller.chosenplanet.currfloor.prevfloor.transform);
							DOTween.To(() => controller.camy.holdOffset, x => controller.camy.holdOffset = x,
								Vector3.zero, 0.3f).SetEase(Ease.OutCubic);
							//DOTween.To(()=> customVector, x=> customVector = x, new Vector3(3,4,2), 2);

							//it's back to the cringe zone with you
							//chosenplanet.transform.position = new Vector3 (chosenplanet.currfloor.prevfloor.transform.position.x, chosenplanet.currfloor.prevfloor.transform.position.y, chosenplanet.transform.position.z);
							controller.chosenplanet.transform
								.DOMove(controller.chosenplanet.currfloor.prevfloor.transform.position, 0.4f)
								.SetEase(Ease.OutCubic);
							controller.chosenplanet.currfloor = controller.chosenplanet.currfloor.prevfloor;

							scrFlash.OnDamage();

							controller.LockInput(0.4f);
						}
					}

					if (controller.chosenplanet.currfloor.holdCompletion > 2 - holdMargin ||
					    controller.chosenplanet.currfloor.holdCompletion <
					    -0.3f) //cringe - held too long or song horribly reset
					{
						controller.chosenplanet.currfloor.holdRenderer.Unfill(false);
						controller.chosenplanet.currfloor.holdCompletion = 0;

						controller.camy.Refocus(controller.chosenplanet.currfloor.prevfloor.transform);
						DOTween.To(() => controller.camy.holdOffset, x => controller.camy.holdOffset = x, Vector3.zero,
							0.3f).SetEase(Ease.OutCubic);

						//it's back to the cringe zone with you
						//chosenplanet.transform.position = new Vector3 (chosenplanet.currfloor.prevfloor.transform.position.x, chosenplanet.currfloor.prevfloor.transform.position.y, chosenplanet.transform.position.z);
						controller.chosenplanet.transform
							.DOMove(controller.chosenplanet.currfloor.prevfloor.transform.position, 0.4f)
							.SetEase(Ease.OutCubic);
						controller.chosenplanet.currfloor = controller.chosenplanet.currfloor.prevfloor;

						scrFlash.OnDamage();

						controller.LockInput(0.4f);
					}
				}
				
				// for in-game
				if (controller.gameworld && ValidInputWasReleasedThisFrameReflectionField.GetValue(controller) && !nextTileIsAuto &&
				    !controller.chosenplanet.currfloor.auto)
				{
					//printe("Input Released");

					//printe("All Inputs Released");
					if (!RDC.auto && !BenchmarkModeReflectionField.GetValue(controller) &&
					    controller.chosenplanet.currfloor.holdLength > -1)
					{
						//printe ("hold completion: "+chosenplanet.currfloor.holdCompletion+"/"+holdMargin);
						if (controller.chosenplanet.currfloor.holdCompletion < holdMargin)
						{
							if (GCS.checkpointNum != controller.chosenplanet.currfloor.seqID &&
							    controller.requireHolding)
							{
								controller.FailAction();
#if  DEBUG
								NoStopMod.mod.Logger.Log($"[{Time.frameCount}] Hold FailAction from update {controller.currFloor.seqID}th tile");
#endif
							}
								
							
							
						}
						else if (!nextTileIsHold)
						{
							controller.chosenplanet.currfloor.holdRenderer.Hit();

							controller.Hit();
							// alreadyHitHoldEnding = true;
							//this would be used for making "release" timing be required for hitting the landing tile
							
#if  DEBUG
							NoStopMod.mod.Logger.Log($"[{Time.frameCount}] nextTileIsHold Hit from update {controller.currFloor.seqID}th tile");
#endif
						}
					}

				}
			}

			// execute only once
			
			//Taro: auto hit the next tile if we started the countdown on a hold...

			ControllerHelper.ExecuteUntilTileNotChange(controller, () =>
			{
				InputFixerManager.AdjustAngle(scrController.instance, targetTick);
				
				if (!RDC.auto && !BenchmarkModeReflectionField.GetValue(controller) &&
				    controller.chosenplanet.AutoShouldHitNow() && controller.chosenplanet.currfloor.holdLength > -1
				    && GCS.checkpointNum == controller.chosenplanet.currfloor.seqID
				    // && !alreadyHitHoldEnding
				    && (scrController.States) controller.GetState() != scrController.States.Fail
				    && (scrController.States) controller.GetState() != scrController.States.Fail2)
				{
					// alreadyHitHoldEnding = true;
					controller.Hit();
#if  DEBUG
					NoStopMod.mod.Logger.Log($"Taro Hit from update {controller.currFloor.seqID}th tile");
#endif
				}
			});

			ControllerHelper.ExecuteUntilTileNotChange(controller, () =>
			{
				InputFixerManager.AdjustAngle(scrController.instance, targetTick);
				
				bool nextTileIsAuto = controller.chosenplanet.currfloor.nextfloor != null && controller.chosenplanet.currfloor.nextfloor.auto;

				double minAngleMargin = scrMisc.GetAdjustedAngleBoundaryInDeg (
					HitMarginGeneral.Counted,
					(float)(controller.conductor.bpm * controller.speed),
					controller.conductor.song.pitch) * Mathf.Deg2Rad;

				//Taro: fail if we let go before 45 degrees from the end.
				//currfloor.holdCompletion and currfloor.angleLength vars are used for this.
				float marginScale = controller.chosenplanet.currfloor.nextfloor != null ? (float)controller.chosenplanet.currfloor.nextfloor.marginScale : 1;
				var holdMargin = 1.0f - ((minAngleMargin * marginScale) / controller.chosenplanet.currfloor.angleLength);

				if (controller.gameworld && !nextTileIsAuto && !controller.chosenplanet.currfloor.auto)
				{
					if (controller.chosenplanet.currfloor.holdCompletion > holdMargin
					    && controller.chosenplanet.currfloor.holdCompletion < 1 - holdMargin
					    && !RDC.auto
					    && (!controller.holding && controller.requireHolding)
					    && GCS.checkpointNum != controller.chosenplanet.currfloor.seqID
					    && !BenchmarkModeReflectionField.GetValue(controller)
					    && controller.chosenplanet.currfloor.holdLength > -1)
					{
						controller.FailAction();
#if  DEBUG
						NoStopMod.mod.Logger.Log($"FailAction from update {controller.currFloor.seqID}th tile");
#endif
					}
				}
			});

			bool consecHold = controller.chosenplanet.currfloor.nextfloor && (controller.strictHolds && controller.chosenplanet.currfloor.nextfloor.holdLength > -1);


			ControllerHelper.ExecuteUntilTileNotChange(controller, () =>
			{
				InputFixerManager.AdjustAngle(scrController.instance, targetTick);
				
				bool nextTileIsHold = controller.chosenplanet.currfloor.nextfloor && controller.chosenplanet.currfloor.nextfloor.holdLength > -1;

				double minAngleMargin = scrMisc.GetAdjustedAngleBoundaryInDeg (
					HitMarginGeneral.Counted,
					(float)(controller.conductor.bpm * controller.speed),
					controller.conductor.song.pitch) * Mathf.Deg2Rad;

				//Taro: fail if we let go before 45 degrees from the end.
				//currfloor.holdCompletion and currfloor.angleLength vars are used for this.
				float marginScale = controller.chosenplanet.currfloor.nextfloor != null ? (float)controller.chosenplanet.currfloor.nextfloor.marginScale : 1;
				var holdMargin = 1.0f - ((minAngleMargin * marginScale) / controller.chosenplanet.currfloor.angleLength);
				
				if (controller.keyTimes.Count > 0
				    && !GCS.d_stationary
				    && !GCS.d_freeroam
				    && (controller.chosenplanet.currfloor.holdLength > -1 && !controller.strictHolds
				        || nextTileIsHold
				        || controller.chosenplanet.currfloor.holdLength == -1
				        || controller.chosenplanet.currfloor.holdCompletion < holdMargin)
				    && (!RDC.auto || !controller.gameworld)
				    && (!controller.mobileMenu || (controller.mobileMenu &&
				                                   scnMobileMenu.instance.mobileMenuPhase ==
				                                   scnMobileMenu.MobileMenuPhase.Road))
				    && (!controller.gameworld ||
				        controller.chosenplanet.currfloor.seqID < controller.lm.listFloors.Count - 1)
				    // && !alreadyHitHoldEnding //don't use this input!
				    && !BenchmarkModeReflectionField.GetValue(controller))
				{

					{
						controller.keyTimes.RemoveAt(0);
						var didSwitch = controller.Hit();
#if  DEBUG
						NoStopMod.mod.Logger.Log($"Hit from update {controller.currFloor.seqID}th tile");
#endif
						if (didSwitch && controller.chosenplanet.currfloor.holdLength > -1)
						{
							// Need to snapshot the currently held keys that must be released
							// to complete the hold. Also need to clear out old keys for when
							// we allow pressing to end a hold.

							//holdKeys.Clear ();
							//IterateValidKeysHeld ((k) => holdKeys.Add (k), (k) => holdKeys.Remove (k));
							holdKeys.Clear();
							IterateValidKeysHeld(controller, (k) => holdKeys.Add(k), (k) => holdKeys.Remove(k));
						}
					}

					// Process another hit if it's a midspin so that regular midspins don't take
					// 2 frames. This helps with judgments on triple-midspin-pseudos on low fps.
					if (controller.midspinInfiniteMargin)
					{
						controller.keyTimes.RemoveAt(0); // Assume tile after midspin is always 1 tap
						var didSwitch = controller.Hit();
#if  DEBUG
						NoStopMod.mod.Logger.Log($"midspinInfiniteMargin Hit from update {controller.currFloor.seqID}th tile");
#endif
						if (didSwitch && controller.chosenplanet.currfloor.holdLength > -1)
						{
							// Need to snapshot the currently held keys that must be released
							// to complete the hold. Also need to clear out old keys for when
							// we allow pressing to end a hold.

							//holdKeys.Clear ();
							//IterateValidKeysHeld ((k) => holdKeys.Add (k), (k) => holdKeys.Remove (k));
							holdKeys.Clear();
							IterateValidKeysHeld(controller, (k) => holdKeys.Add(k), (k) => holdKeys.Remove(k));
						}
					}
				}
			});

			// Update average frame time
			controller.averageFrameTime =
				(1 - GCS.frameTimeSensitivity) * controller.averageFrameTime
				+ GCS.frameTimeSensitivity * Time.deltaTime;

			if (controller.camy.followMode && controller.camy.followMovingPlatforms)
			{
				// camy.frompos = camy.transform.localPosition;
				controller.camy.topos = new Vector3 (
					controller.chosenplanet.transform.position.x,
					controller.chosenplanet.transform.position.y,
					controller.camy.transform.position.z);
				// camy.timer = 0;
			}
        }
        
        public static void IterateValidKeysHeld(scrController controller, Action<ushort> foundHeld, Action<ushort> foundSpecial)
        {
	        // Luxus가 막음 (여기부터)
	        // TOUCH
	        // if (ADOBase.isMobile)
	        // {
	        //     foreach (var t in Input.touches)
	        //     {
	        //         if (!controller.IsScreenPointInsideUIElements (t.position))
	        //         {
	        //             // Treat nulls as touches
	        //             foundHeld (null);
	        //         }
	        //     }
	        // }

	        // MOUSE
	        // else
	        // Luxus가 막음 (여기까지)
	        
	        // 이거 바뀜
	        // {
	        //     for (var i = 323; i <= 329; i++)
	        //     {
	        //         var keyCode = (KeyCode)i;
	        //         if (Input.GetKey (keyCode))
	        //         {
	        //             foundHeld (keyCode);
	        //         }
	        //     }
	        // }

	        foreach (var @ushort in keyMask)
	        {
		        foundHeld(@ushort);
	        }

	        // KEYBOARD
	        // {
	        //     const int KeyCodeJustKeys = 320;
				     //
	        //     for (var i = 0; i < keyMask.Count; i++)
	        //     {
		       //      
	        //     }
	        //     
	        //     for (var i = 0; i < KeyCodeJustKeys; i++)
	        //     {
	        //         var keyCode = (KeyCode)i;
	        //         if (Input.GetKey (keyCode))
	        //         {
	        //             // printe ("keydown on: " + keyCode.ToString ());
	        //
	        //             //Letters have single-character length keycodes ('a','b',..)
	        //             //and we only want to support multiple input in one frame for these
	        //             //we cant just use the whole list of keycodes cause ctrl activates both leftCtrl and Ctrl, for example, so double input
	        //
	        //             //TEMP: test disabling this, see if any problems
	        //
	        //             // if (keyCode.ToString ().Length == 1 || //a clever way to say 'only letters'
	        //             // keyCode.ToString ().Contains ("Arrow"))
	        //             // {
	        //
	        //             //     keyCount++;
	        //             // }
	        //             // else
	        //             // {
	        //             //     //for non-letters, only increase to 1 at max.
	        //             //     if (keyCount == 0) keyCount++;
	        //             // }
	        //
	        //             foundHeld (keyCode);
	        //         }
	        //     }
	        //
	        //     // Normally we would check for special keys here, but since held keys are only
	        //     // used to snapshot for hold notes, it's likely that the special keys will be
	        //     // released for the holds.
	        // }

	        //JOYSTICK
	        //problem: hitting one button triggers both Joystick0Button1 and JoystickButton1
	        //hence just check only these keys

	        // var joystickButtons = GCS.joystickButtons;
	        // for (int i = 0; i < joystickButtons.Length; i++)
	        // {
	        //     if (Input.GetKey (joystickButtons[i]))
	        //         foundHeld (joystickButtons[i]);
	        // }

	        // Use extremely-unlikely-to-be-used buttons for dpad inputs
	        // Luxus가 막음 (여기부터)
	        // if (controller.dpadInputChecker.holdingX)
	        // {
	        //     foundHeld (KeyCode.Joystick8Button0);
	        // }
	        // if (controller.dpadInputChecker.holdingY)
	        // {
	        //     foundHeld (KeyCode.Joystick8Button1);
	        // }
	        // Luxus가 막음 (여기까지)
        }

        public static bool ValidInputWasReleased(scrController controller)
        {
	        // if (ADOBase.isMobile)
	        // {
		       //  if (this.holdKeys.Count != 0 && Input.touchCount == 0 && !Input.anyKey)
		       //  {
			      //   this.holdKeys.Clear();
			      //   return true;
		       //  }
	        // }
	        // else
	        {
		        // bool holding = controller.holding;

		        List<ushort> list = new();
		        foreach (var holdKey in InputFixerManager.holdKeys)
		        {
			        
			        
				        // if (key == KeyCode.Joystick8Button0 && !controller.dpadInputChecker.holdingX || key == KeyCode.Joystick8Button1 && !controller.dpadInputChecker.holdingY || !Input.GetKey(key))
					       //  InputFixerManager.holdKeys.RemoveAt(index);

						       //InputFixerManager.holdKeys.Remove(holdKey);
					       
					if (!InputFixerManager.keyMask.Contains(holdKey))
					{
						list.Add(holdKey);
					}
			        
		        }
		        foreach (var removeKey in list)
		        {
			        InputFixerManager.holdKeys.Remove(removeKey);
		        }
		        
		        // for (int index = InputFixerManager.holdKeys.Count - 1; index >= 0; --index)
		        // {
			       //  ushort? holdKey = InputFixerManager.holdKeys[index];
			       //  if (!holdKey.HasValue)
			       //  {
				      //   InputFixerManager.holdKeys.RemoveAt(index);
			       //  }
			       //  else
			       //  {
				      //   holdKey = InputFixerManager.holdKeys[index];
				      //   KeyCode key = holdKey.Value;
				      //   if (key == KeyCode.Joystick8Button0 && !controller.dpadInputChecker.holdingX || key == KeyCode.Joystick8Button1 && !controller.dpadInputChecker.holdingY || !Input.GetKey(key))
					     //    InputFixerManager.holdKeys.RemoveAt(index);
			       //  }
		        // }
// 		        
// 		        if (holding && InputFixerManager.holdKeys.Count == 0)
// 			        return true;
	        }
	        return InputFixerManager.validKeyWasReleased;
        }
        
        public static bool ValidInputWasTriggered(scrController controller)
        {
	        if (ExitingToMainMenuReflectionField.GetValue(controller))
		        return false;
	        // bool flag1 = false;
	        // if (ADOBase.isMobile)
	        // {
		       //  foreach (Touch touch in Input.touches)
		       //  {
			      //   if (touch.phase == TouchPhase.Began && !controller.IsScreenPointInsideUIElements(touch.position))
			      //   {
				     //    flag1 = true;
				     //    break;
			      //   }
		       //  }
	        // }

	        bool flag2;
	        // if (ADOBase.isMobile)
	        // {
		       //  flag2 = ((!Input.anyKeyDown || Input.GetKeyDown(KeyCode.Mouse0)
			      //   ? 0
			      //   : (!Input.GetKeyDown(KeyCode.Mouse1) ? 1 : 0)) | (flag1 ? 1 : 0)) != 0;
	        // }
	        // else
	        {
		        bool flag3 = false;
		        //if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1))
		        if (keyDownMask.Contains((ushort) (1000 + SharpHook.Native.MouseButton.Button1)) || keyDownMask.Contains((ushort) (1000 + SharpHook.Native.MouseButton.Button2)))
			        flag3 = EventSystem.current.IsPointerOverGameObject();
		        if (controller.isCutscene)
			        flag3 = false;
		        //flag2 = (Input.anyKeyDown || controller.dpadInputChecker.anyDirDown) && !flag3 && !controller.paused;
		        flag2 = keyDownMask.Count > 0 && !flag3 && !controller.paused;
	        }

	        return flag2 && controller.CountValidKeysPressed() > 0;
        }

        public static void AdjustAngle(scrController controller, long targetTick)
        {
#if DEBUG
            var originalAngle = controller.chosenplanet.angle;
#endif
            InputFixerManager.targetSongTick = targetTick - InputFixerManager.offsetTick;
            InputFixerManager.jumpToOtherClass = true;
            controller.chosenplanet.Update_RefreshAngles();
#if DEBUG
            //NoStopMod.mod.Logger.Log($"AdjustAngle SongTime {(targetTick - InputFixerManager.offsetTick) / 1000000.0 - InputFixerManager.dspTimeSong - scrConductor.calibration_i}s, angle {originalAngle}->{controller.chosenplanet.angle}");
#endif
        }

    }

}
