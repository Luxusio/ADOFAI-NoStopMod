using NoStopMod.InputFixer.HitIgnore;
using NoStopMod.Helper;
using System;
using System.Collections.Generic;
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

        public static long currTick;
        public static long prevTick;

        public static long offsetTick;
        public static long currPressTick;

        public static bool jumpToOtherClass = false;
        public static bool editInputLimit = false;

        private static bool[] mask;
        
        public static void Init()
        {
            NoStopMod.onToggleListeners.Add(OnToggle);
            NoStopMod.onGUIListeners.Add(OnGUI);
            NoStopMod.onApplicationQuitListeners.Add(OnApplicationQuit);

            settings = new InputFixerSettings();
            Settings.settings.Add(settings);

            prevTick = DateTime.Now.Ticks;
            currTick = prevTick;
            
            mask = Enumerable.Repeat(false, 1024).ToArray();

            HitIgnoreManager.Init();
        }

        private static void OnToggle(bool enabled)
        {
            UpdateEnableAsync(enabled);
        }

        private static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            //GUILayout.BeginVertical("Input");
            SimpleGUI.Toggle(ref settings.enableAsync, "Toggle Input Asynchronously", UpdateEnableAsync);
            //GUILayout.EndVertical(); 
        }

        private static void UpdateEnableAsync(bool value)
        {
            if (value)
            {
                Start();
            }
            else
            {
                Stop();
            }
        }

        private static void OnApplicationQuit(scrController __instance)
        {
            Stop();
        }

        public static void Start()
        {
            Stop();
            adjustOffsetTick();
            if (settings.enableAsync) {
                thread = new Thread(Run);
                thread.Start();
            }
        }

        public static void Stop()
        {
            if (thread != null)
            {
                thread.Abort();
                thread = null;
            }
            keyQueue.Clear();
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

        private static void Run()
        {
            long prevTick = DateTime.Now.Ticks;
            while (true)
            {
                long currTick = DateTime.Now.Ticks;
                if (currTick > prevTick)
                {
                    prevTick = currTick;
                    UpdateKeyQueue(currTick);
                }
            }
        }

        public static void UpdateKeyQueue(long currTick)
        {
            List<KeyCode> keyCodes = getPressedKeys();
            if (keyCodes.Any())
            {
                keyQueue.Enqueue(new Tuple<long, List<KeyCode>>(currTick, keyCodes));
            }
        }

        private static List<KeyCode> getPressedKeys()
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

        public static double getAngle(scrPlanet __instance, double ___snappedLastAngle, long nowTick)
        {
            return ___snappedLastAngle + (getSongPosition(__instance.conductor, nowTick) - __instance.conductor.lastHit) / __instance.conductor.crotchet
                * 3.141592653598793238 * __instance.controller.speed * (double)(__instance.controller.isCW ? 1 : -1);
        }

        public static double getSongPosition(scrConductor __instance, long nowTick)
        {
            if (!GCS.d_oldConductor && !GCS.d_webglConductor)
            {
                return ((nowTick / 10000000.0 - scrConductor.calibration_i) * __instance.song.pitch) - __instance.addoffset;
            }
            else
            {
                return (__instance.song.time - scrConductor.calibration_i) - __instance.addoffset / __instance.song.pitch;
            }
        }

        public static void adjustOffsetTick()
        {
            if (scrConductor.instance != null)
            {
                InputFixerManager.jumpToOtherClass = true;
                scrConductor.instance.Start();
            }
        }

        public static void adjustOffsetTick(scrConductor __instance, double ___dspTimeSong)
        {
            offsetTick = currTick - (long)((__instance.dspTime - ___dspTimeSong) * 10000000);
        }

    }
}
