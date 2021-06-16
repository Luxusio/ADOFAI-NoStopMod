using NoStopMod.Abstraction;
using NoStopMod.AsyncInput.HitIgnore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

namespace NoStopMod.AsyncInput
{
    class AsyncInputManager
    {
        public static AsyncInputSettings settings;
        
        private static Thread thread;
        public static Queue<Tuple<long, List<KeyCode>>> keyQueue = new Queue<Tuple<long, List<KeyCode>>>();

        public static long currTick;
        public static long prevTick;

        public static long offsetTick;
        public static long currPressTick;
        
        public static bool jumpToOtherClass = false;

        private static bool[] mask;

        //[DllImport("USER32.dll")]
        //static extern short GetKeyState(VirtualKeyStates nVirtKey);

        public static void Init()
        {
            NoStopMod.onToggleListeners.Add(OnToggle);
            settings = new AsyncInputSettings();

            prevTick = DateTime.Now.Ticks;
            currTick = prevTick;
            
            mask = Enumerable.Repeat(false, 1024).ToArray();

            HitIgnoreManager.Init();
        }

        private static void OnToggle(bool enabled)
        {
            if (enabled)
            {
                Start();
            }
            else
            {
                Stop();
            }
        }

        public static void Start()
        {
            Stop();
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
                    
                    if (keyCodes.Any())
                    {
                        keyQueue.Enqueue(new Tuple<long, List<KeyCode>>(currTick, keyCodes));
                    }
                }
            }
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

        public static void adjustOffsetTick(scrConductor __instance, double ___dspTimeSong)
        {
            offsetTick = currTick - (long)((__instance.dspTime - ___dspTimeSong) * 10000000);
        }

    }
}
