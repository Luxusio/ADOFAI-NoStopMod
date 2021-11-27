using NoStopMod.Helper.RawInputManager;
using NoStopMod.InputFixer.HitIgnore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityModManagerNet;

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

        private static readonly Process adofaiProcess = Process.GetCurrentProcess();
        private static Process noopProcess;

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
                thread.Abort();
                thread = null;
            }
            currFrameMs = 0;
            prevFrameMs = 0;
            if (toggle)
            {
                thread = new Thread(() => {
                    stopwatch?.Stop();
                    stopwatch = new Stopwatch();
                    stopwatch.Start();
                    mask.Clear();
                    keyQueue.Clear();

                    while (true)
                    {
                        Thread.Sleep(60000);
                    }
                });

                thread.Start();
                SetHooker();
            }
        }

        private static void SetHooker()
        {
            hhook?.UnHook();

            noopProcess?.Kill();
            noopProcess = new Process();
            noopProcess.StartInfo.FileName = "notepad.exe";
            noopProcess.StartInfo.Arguments = "";
            noopProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            noopProcess.Start();

            hhook = KBDHooker.HookProcess((nCode, wParam, lParam) =>
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

                return KBDHooker.CallNextHookEx(hhook, nCode, wParam, lParam);
            }, noopProcess);
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
