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

        public HitIgnoreManager hitIgnoreManager;

        private Thread thread;
        public Queue<Tuple<long, List<KeyCode>>> keyQueue = new Queue<Tuple<long, List<KeyCode>>>();

        public long currTick;
        public long prevTick;

        public long offsetTick;
        public long currPressTick;
        
        public bool jumpToOtherClass = false;

        private bool[] mask;

        //[DllImport("USER32.dll")]
        //static extern short GetKeyState(VirtualKeyStates nVirtKey);

        public AsyncInputManager()
        {
            NoStopMod.onToggleListeners.Add(OnToggle);

            prevTick = DateTime.Now.Ticks;
            currTick = prevTick;
            
            mask = Enumerable.Repeat(false, 1024).ToArray();

            hitIgnoreManager = new HitIgnoreManager();
        }

        private void OnToggle(bool enabled)
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

        public void Start()
        {
            Stop();
            thread = new Thread(Run);
            thread.Start();
        }

        public void Stop()
        {
            if (thread != null)
            {
                thread.Abort();
                thread = null;
            }
        }

        private bool GetKeyDown(int idx)
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

        private void Run()
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

        public double getAngle(scrPlanet __instance, double ___snappedLastAngle, long nowTick)
        {
            return ___snappedLastAngle + (this.getSongPosition(__instance.conductor, nowTick) - __instance.conductor.lastHit) / __instance.conductor.crotchet
                * 3.141592653598793238 * __instance.controller.speed * (double)(__instance.controller.isCW ? 1 : -1);
        }

        public double getSongPosition(scrConductor __instance, long nowTick)
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

        public void adjustOffsetTick(scrConductor __instance, double ___dspTimeSong)
        {
            NoStopMod.asyncInputManager.offsetTick = NoStopMod.asyncInputManager.currTick - (long)((__instance.dspTime - ___dspTimeSong) * 10000000);
        }
        
    }
}
