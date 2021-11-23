using NoStopMod.InputFixer.HitIgnore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityModManagerNet;

namespace NoStopMod.InputFixer
{
    class InputFixerManager
    {
        public static InputFixerSettings settings;
        
        private static Thread thread;
        public static Queue<Tuple<long, List<KeyCode>>> keyQueue = new Queue<Tuple<long, List<KeyCode>>>();
        
        public static long currPressMs;

        public static bool jumpToOtherClass = false;
        public static bool editInputLimit = false;

        private static bool[] mask;


        public static Stopwatch stopwatch;

        public static long currFrameMs;
        public static long prevFrameMs;

        public static double dspTime;
        public static double dspTimeSong;

        public static long offsetMs;
         

        public static double lastReportedDspTime;

        public static double previousFrameTime; 


        public static void Init()
        {
            NoStopMod.onToggleListener.Add(ToggleThread);
            NoStopMod.onGUIListener.Add(OnGUI);
            NoStopMod.onApplicationQuitListener.Add(_ => ToggleThread(false));

            settings = new InputFixerSettings();
            Settings.settings.Add(settings);
            
            mask = Enumerable.Repeat(false, 1024).ToArray();

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
            keyQueue.Clear();
            stopwatch?.Stop();
            currFrameMs = 0;
            prevFrameMs = 0;
            if (toggle)
            {
                thread = new Thread(Run);
                thread.Start();
            }
        }

        private static void Run()
        {
            stopwatch = new Stopwatch();
            stopwatch.Start();

            long prevMs = 0, currMs = 0;

            while (true)
            {
                currMs = stopwatch.ElapsedMilliseconds;
                if (currMs > prevMs)
                {
                    prevMs = currMs;
                    UpdateKeyQueue(currMs);
                }
            }
        }

        private static bool GetKeyDown(int idx)
        {
            if (mask[idx])
            {
                if (!Input.GetKey((KeyCode)idx))
                {
                    mask[idx] = false;
                }
            }
            else
            {
                if (Input.GetKey((KeyCode)idx))
                {
                    mask[idx] = true;
                    return true;
                }
            }
            return false;
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

        public static void UpdateKeyQueue(long currMs)
        {
            List<KeyCode> keyCodes = GetPressedKeys();
            if (keyCodes.Any())
            {
                keyQueue.Enqueue(new Tuple<long, List<KeyCode>>(currMs, keyCodes));
            }
        }

        private static List<KeyCode> GetPressedKeys()
        {
            List<KeyCode> keyCodes = new List<KeyCode>();

            for (int i = 0; i < 320; i++)
            {
                if (GetKeyDown(i))
                {
                    keyCodes.Add((KeyCode)i);
                }
            }

            for (int i = 323; i <= 329; i++)
            {
                if (GetKeyDown(i))
                {
                    keyCodes.Add((KeyCode)i);
                }
            }

            return keyCodes;
        }

        public static double GetAngle(scrPlanet __instance, double ___snappedLastAngle, long nowTick)
        {
            return ___snappedLastAngle + (GetSongPosition(__instance.conductor, nowTick) - __instance.conductor.lastHit) / __instance.conductor.crotchet
                * 3.141592653598793238 * __instance.controller.speed * (double)(__instance.controller.isCW ? 1 : -1);
        }

        
        
    }
}
