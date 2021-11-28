using NoStopMod.Helper.RawInputManager;
using NoStopMod.InputFixer.HitIgnore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityModManagerNet;
using Application = System.Windows.Forms.Application;

namespace NoStopMod.InputFixer
{
    class InputFixerManager
    {
        public static InputFixerSettings settings;
        
        private static Thread thread;
        public static Queue<Tuple<long, RawKeyCode>> keyQueue = new Queue<Tuple<long, RawKeyCode>>();
        
        public static long currPressMs;

        public static bool jumpToOtherClass = false;
        public static bool editInputLimit = false;

        public static HHook hhook;
        public static Stopwatch stopwatch;

        public static long currFrameMs;
        public static long prevFrameMs;

        public static double dspTime;
        public static double dspTimeSong;

        public static long offsetMs;

        public static double lastReportedDspTime;

        public static double previousFrameTime;

        private static HashSet<RawKeyCode> mask = new HashSet<RawKeyCode>();

        private static bool _threadRunning;

        public static void Init()
        {
            NoStopMod.onToggleListener.Add(ToggleThread);
            NoStopMod.onGUIListener.Add(OnGUI);
            NoStopMod.onApplicationQuitListener.Add(_ => ToggleThread(false));

            settings = new InputFixerSettings();
            Settings.settings.Add(settings);

            HitIgnoreManager.Init();
        }

        private static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            //GUILayout.BeginVertical("Input");
            //GUILayout.EndVertical(); 
        }

        public static void ToggleThread(bool toggle)
        {
            if (thread != null)
            {
                try
                {
#if DEBUG
                    NoStopMod.mod.Logger.Log("abort thread");
#endif
                    _threadRunning = false;
                    thread.Abort();
                }
                catch (ThreadAbortException ex)
                {
                    NoStopMod.mod.Logger.Error("Error while aborting input thread : " + ex);
                }

                thread = null;
            }
            currFrameMs = 0;
            prevFrameMs = 0;
            if (toggle)
            {
                thread = new Thread(() =>
                {
                    try
                    {
                        _threadRunning = true;

                        stopwatch?.Stop();
                        stopwatch = new Stopwatch();
                        stopwatch.Start();
                        mask.Clear();
                        keyQueue.Clear();

                        hhook?.UnHook();
                        hhook = KBDHooker.Hook((nCode, wParam, lParam) =>
                        {
                            RawKeyCode code = lParam.GetKeyCode();
                            if (code.IsKeyDown(nCode, wParam, lParam))
                            {
                                if (!mask.Contains(code))
                                {
                                    mask.Add(code);
                                    keyQueue.Enqueue(new Tuple<long, RawKeyCode>(stopwatch.ElapsedMilliseconds, code));
                                }
                            }
                            else if (code.IsKeyUp(nCode, wParam, lParam))
                            {
                                mask.Remove(code);
                            }

                            return hhook.CallNextHookEx(nCode, wParam, lParam);
                        });

                        Application.Run();

                        while (_threadRunning)
                        {
                        }
                    }
                    finally
                    {
#if DEBUG
                        NoStopMod.mod.Logger.Log("exit thread");
#endif
                        hhook?.UnHook();
                        Application.Exit();
                        Application.ExitThread();
#if DEBUG
                        NoStopMod.mod.Logger.Log("exit success");
#endif
                    }
                });

                thread.Start();
            }
        }
        
        public static double GetSongPosition(scrConductor __instance, long nowTick)
        {
            if (!GCS.d_oldConductor && !GCS.d_webglConductor)
            {
                return ((nowTick / 1000.0 - dspTimeSong - scrConductor.calibration_i) * __instance.song.pitch) - __instance.addoffset;
            }
            else
            {
                return (__instance.song.time - scrConductor.calibration_i) - __instance.addoffset / __instance.song.pitch;
            }
        }

        public static double GetAngle(scrPlanet __instance, double ___snappedLastAngle, long nowTick)
        {
            return ___snappedLastAngle + (GetSongPosition(__instance.conductor, nowTick) - __instance.conductor.lastHit) / __instance.conductor.crotchet
                * 3.141592653598793238 * __instance.controller.speed * (double)(__instance.controller.isCW ? 1 : -1);
        }
        
        
    }
}
