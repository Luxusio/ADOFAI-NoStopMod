using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace NoStopMod.AsyncInput
{
    class AsyncInputManager
    {

        private Thread thread;
        public Queue<Tuple<long, List<KeyCode>>> keyQueue = new Queue<Tuple<long, List<KeyCode>>>();
        //public double dspTimeSong;

        public long offsetTick;
        public long currTick;
        public long prevTick;

        public long lastHitTick;
        public double lastHitSec;
        public long currHitTick;

        //public double sum = 0;
        //public int count = 0;

        public long pauseStart = 0;
        public bool jumpToOtherClass = false;
        
        private bool[] mask;

        public AsyncInputManager()
        {
            NoStopMod.onToggleListeners.Add(OnToggle);
            mask = Enumerable.Repeat(false, 1024).ToArray();

            prevTick = DateTime.Now.Ticks;
            currTick = prevTick;

        }

        private void OnToggle(bool enabled)
        {
            if (enabled)
            {
                //Start();
            }
            else
            {
                Stop();
            }
        }

        public void Start()
        {
            Stop();
            //thread = new Thread(Run);
            //thread.Start();
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
                        keyQueue.Enqueue(new Tuple<long, List<KeyCode>>(DateTime.Now.Ticks, keyCodes));
                        //string str = DateTime.Now.Ticks + " press ";
                        //foreach (KeyCode keyCode in keyCodes)
                        //{
                        //    str += keyCode.ToString() + ", ";
                        //}
                        //str += keyQueue.Count() + "개 " + keyQueue.Any();
                        //NoStopMod.mod.Logger.Log(str);
                    }
                }
            }
        }

        
        // Update()

        

        

        [HarmonyPatch(typeof(scrController), "CountValidKeysPressed")]
        private static class scrController_CountValidKeysPressed_Patch
        {
            public static void Postfix(scrController __instance, ref int __result)
            {
                if (__result != 0)
                {
                    long diff = NoStopMod.asyncInputManager.currTick - NoStopMod.asyncInputManager.offsetTick;
                    //NoStopMod.asyncInputManager.currHitTick = diff;
                    NoStopMod.mod.Logger.Log(diff + " press " + __result + " keys");
                    NoStopMod.mod.Logger.Log(__instance.chosenplanet.other.angle + " angle");
                }
            }
        }



        //PlayerControl_Update
        [HarmonyPatch(typeof(scrController), "PlayerControl_Update")]
        private static class scrController_PlayerControl_Update_Patch
        { 
            public static void Postfix(scrController __instance)
            {
                while (NoStopMod.asyncInputManager.keyQueue.Any())
                {
                    long tick;
                    List<KeyCode> keyCodes;
                    NoStopMod.asyncInputManager.keyQueue.Dequeue().Deconstruct(out tick, out keyCodes);
                    
                    if (!scrController.isGameWorld) continue;
                    //scrConductor conductor = __instance.conductor;


                    //string str = "";
                    //if (!GCS.d_oldConductor && !GCS.d_webglConductor)
                    //{
                    //    str = "new conductor " + tick + ", " + conductor.dspTime + ", " + NoStopMod.asyncInputManager.dspTimeSong + ":" + conductor.songposition_minusi;
                    //    conductor.songposition_minusi = (double)((float)(conductor.dspTime - NoStopMod.asyncInputManager.dspTimeSong - (double)scrConductor.calibration_i) * conductor.song.pitch) - conductor.addoffset;
                    //}
                    //else
                    //{
                    //    str = "old conductor " + tick + ", " + conductor.dspTime + ", " + NoStopMod.asyncInputManager.dspTimeSong + ":" + conductor.songposition_minusi;
                    //    conductor.songposition_minusi = (double)(conductor.song.time - scrConductor.calibration_i) - conductor.addoffset / (double)conductor.song.pitch;
                    //}
                    //str += " -> " + conductor.songposition_minusi;
                    //NoStopMod.mod.Logger.Log(str);

                    //NoStopMod.asyncInputManager.currTick = tick;

                    scrPlanet planet = __instance.chosenplanet;
                    //planet.angle = planet.snappedLastAngle + (planet.conductor.songposition_minusi - planet.conductor.lastHit) / planet.conductor.crotchet * 3.1415927410125732 * __instance.speed * (double)(__instance.isCW ? 1 : -1);


                    for (int i=0;i< keyCodes.Count(); i++)
                    {
                        //__instance.chosenplanet.Update_RefreshAngles();
                        //__instance.Hit();
                    }
                    

                    // this.snappedLastAngle + (this.conductor.songposition_minusi - this.conductor.lastHit) /
                    // this.conductor.crotchet * 3.1415927410125732 * this.controller.speed * (double) (this.controller.isCW ? 1 : -1);

                    //__instance.chosenplanet.angle;


                }


            }
        }

        // Update_RefreshAngles()
        [HarmonyPatch(typeof(scrPlanet), "Update_RefreshAngles")]
        private static class scrPlanet_Update_RefreshAngles_Patch
        {
            public static bool Prefix(scrPlanet __instance, double ___snappedLastAngle)
            {
                if (NoStopMod.asyncInputManager.currHitTick == 0)
                {
                    return true;
                }


                return true;
                //long diff = 
                
                


                //NoStopMod.asyncInputManager.currTick = 0;
                //return false;
            }


            public static void Postfix(scrPlanet __instance, double ___snappedLastAngle)
            {
                
                if (__instance.isChosen)
                {
                    long nowTick = NoStopMod.asyncInputManager.currTick - NoStopMod.asyncInputManager.offsetTick;
                    __instance.angle = NoStopMod.asyncInputManager.getAngle(__instance, ___snappedLastAngle, nowTick);

                    //NoStopMod.mod.Logger.Error("aa" + ___snappedLastAngle + ", " + NoStopMod.asyncInputManager.getSongPosition(__instance.conductor, nowTick) + ", " + nowTick + ", " + __instance.conductor.crotchet);

                    //NoStopMod.mod.Logger.Log(__instance.conductor.lastHit + ", " + (NoStopMod.asyncInputManager.lastHitSec)  + ":" + (__instance.conductor.lastHit - NoStopMod.asyncInputManager.lastHitSec));
                    //string str = ___snappedLastAngle + ", " + (NoStopMod.asyncInputManager.getSongPosition(__instance.conductor, nowTick)) + ", " + (__instance.conductor.lastHit) + ", " + __instance.conductor.crotchet +
                    //    ", " + __instance.controller.speed + ", " +  (double)(__instance.controller.isCW ? 1 : -1);
                    //double r = ___snappedLastAngle + (NoStopMod.asyncInputManager.getSongPosition(__instance.conductor, nowTick) - NoStopMod.asyncInputManager.lastHitSec) / __instance.conductor.crotchet
                    //    * 3.141592653598793238 * __instance.controller.speed * (double)(__instance.controller.isCW ? 1 : -1);
                    //str += ", result:" + r + ", real:" + __instance.angle + ", " + AudioListener.pause;
                    //NoStopMod.mod.Logger.Log(str);
                    //__instance.angle = r;

                }
            }
        }


        

        // Update LastHitTick
        // scrConductor.Rewind
        // scrConductor.ScrubMusicToTile
        // scrPlanet.MoveToNextFloor

        public void adjustLastHitTick(scrConductor __instance, long tick)
        {

            // scrConductor.Rewind
            // conductor.lastHit = 0.0f

            // scrConductor.ScrubMusicToTile
            // this.lastHit = base.lm.listFloors[tileID].entryTime;

            // scrPlanet.MoveToNextFloor
            // this.conductor.lastHit += ((double)exitAngle - this.snappedLastAngle) * (double)(this.controller.isCW ? 1 : -1) / 3.1415927410125732 * this.conductor.crotchet / this.controller.speed;
            
        }

        public void adjustOffsetTick(scrConductor __instance, double ___dspTimeSong)
        {
            NoStopMod.asyncInputManager.offsetTick = NoStopMod.asyncInputManager.currTick - (long)((__instance.dspTime - ___dspTimeSong) * 10000000);
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

        public double getAngle(scrPlanet __instance, double ___snappedLastAngle, long nowTick)
        {
            return ___snappedLastAngle + (NoStopMod.asyncInputManager.getSongPosition(__instance.conductor, nowTick) - __instance.conductor.lastHit) / __instance.conductor.crotchet
                * 3.141592653598793238 * __instance.controller.speed * (double)(__instance.controller.isCW ? 1 : -1);
        }

        // Update()
        


    }
}
