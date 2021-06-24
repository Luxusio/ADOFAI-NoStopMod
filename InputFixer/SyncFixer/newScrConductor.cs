using ADOFAI;
using DG.Tweening;
using HarmonyLib;
using RDTools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;



namespace NoStopMod.InputFixer.SyncFixer
{


    class newScrConductor
    {
        // Note : this class replaces scrController in ADOFAI.
        // to fix Desync bug, I replaced AudioSettings.dspTime to DateTime.Now.Ticks
        // I hope this mechanism work well.



        /*
         dspTime update
         Start()
         Rewind()
         Update()
        */
        // a

        // new code
        public long offsetTick;

        public long dspTick;


        public void FixOffsetTick()
        {
            offsetTick = NoStopMod.CurrFrameTick() - (long)(instance.dspTime * 10000000);
#if DEBUG
            NoStopMod.mod.Logger.Log("FixOffsetTick");
#endif
        }

        public double getSongPosition(scrConductor __instance, long nowTick)
        {
            if (!GCS.d_oldConductor && !GCS.d_webglConductor)
            {
                return ((nowTick / 10000000.0 - this.dspTimeSong - scrConductor.calibration_i) * __instance.song.pitch) - __instance.addoffset;
            }
            else
            {
                return (__instance.song.time - scrConductor.calibration_i) - __instance.addoffset / __instance.song.pitch;
            }
        }

        // from scrController
        public int FindSongStartTile(scrConductor conductor, int floorNum, bool forceDontStartMusicFourTilesBefore = false)
        {
            int result = floorNum;
            if (GCS.usingCheckpoints && !forceDontStartMusicFourTilesBefore)
            {
                List<scrFloor> floorList = conductor.lm.listFloors;
                double startSpeed = conductor.crotchetAtStart / (double)floorList[floorNum].speed;
                for (int i = floorNum - 1; i >= 1; i--)
                {
                    if (floorList[i].entryTime <= floorList[floorNum].entryTime - (double)conductor.countdownTicks * startSpeed)
                    {
                        result = i;
                        break;
                    }
                }
            }
            return result;
        }


        // Original Code

        // Token: 0x04000288 RID: 648
        public List<HitSoundsData> hitSoundsData = new List<HitSoundsData>();

        // Token: 0x04000289 RID: 649
        public int nextHitSoundToSchedule;

        // Token: 0x0400028A RID: 650
        public int crotchetsPerBar = 8;


        // Token: 0x0400028D RID: 653
        //public double _songposition_minusi;

        // Token: 0x0400028E RID: 654
        public double previousFrameTime;

        // Token: 0x0400028F RID: 655
        public double lastReportedPlayheadPosition;


        // Token: 0x04000294 RID: 660
        public double nextBeatTime;

        // Token: 0x04000295 RID: 661
        public double nextBarTime;


        // Token: 0x04000299 RID: 665
        public float buffer = 1f;

        // Token: 0x0400029D RID: 669
        public float[] _spectrum = new float[1024];

        // Token: 0x0400029F RID: 671
        public AudioSource audiosource;

        // Token: 0x040002A0 RID: 672
        public bool playedHitSounds;

        // Token: 0x040002A1 RID: 673
        public scrConductor.DuckState _duckState;

        // Token: 0x040002A2 RID: 674
        public float songPreduckVolume;

        // Token: 0x040002A3 RID: 675
        public float song2PreduckVolume;

        // Token: 0x040002A6 RID: 678
        public double dspTimeSong;

        // Token: 0x040002A8 RID: 680
        public List<ADOBase> _onBeats = new List<ADOBase>();

        public static scrConductor instance
        {
            get
            {
                if (newScrConductor._instance == null)
                {
                    newScrConductor._instance = UnityEngine.Object.FindObjectOfType<scrConductor>();
                }
                return newScrConductor._instance;
            }
        }

        // Token: 0x040002A9 RID: 681
        private static scrConductor _instance;

        // Token: 0x040002AA RID: 682
        public Coroutine startMusicCoroutine;

        public struct HitSoundsData
        {
            // Token: 0x06000CE9 RID: 3305 RVA: 0x0005342A File Offset: 0x0005162A
            public HitSoundsData(HitSound hitSound, double time, float volume)
            {
                this.hitSound = hitSound;
                this.time = time;
                this.volume = volume;
                this.played = false;
            }

            // Token: 0x04000FCC RID: 4044
            public HitSound hitSound;

            // Token: 0x04000FCD RID: 4045
            public double time;

            // Token: 0x04000FCE RID: 4046
            public float volume;

            // Token: 0x04000FCF RID: 4047
            public bool played;
        }

        public void Awake(scrConductor __instance)
        {
            ADOBase.Startup();
            if (GCS.d_webglConductor)
            {
                this.buffer = 0.5f;
            }
            if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                this.buffer = 0.5f;
            }
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                this.buffer = 0.5f;
            }
            if (Application.platform == RuntimePlatform.OSXPlayer)
            {
                this.buffer = 0.5f;
            }
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                this.buffer = 0.5f;
            }
            if (__instance.uiController != null)
            {
                __instance.txtOffset = __instance.uiController.txtOffset;
            }
            if (scnEditor.instance == null)
            {
                scnEditor.instance = __instance.editorComponent;
            }
            if (scnEditor.instance == null)
            {
                scnCLS.instance = __instance.CLSComponent;
            }
            if (CustomLevel.instance == null)
            {
                CustomLevel.instance = __instance.customLevelComponent;
            }
        }

        public void Start(scrConductor __instance)
        {
            __instance.crotchet = (double)(60f / __instance.bpm);
            __instance.crotchetAtStart = __instance.crotchet;
            AudioSource[] components = __instance.GetComponents<AudioSource>();
            __instance.song = components[0];
            if (components.Length > 1)
            {
                __instance.song2 = components[1];
            }

            this.nextBeatTime = 0.0;
            this.nextBarTime = 0.0;
            if (__instance.txtOffset != null)
            {
                __instance.txtOffset.text = "";
            }
            this.lastReportedPlayheadPosition = AudioSettings.dspTime;
            __instance.dspTime = AudioSettings.dspTime;
#if DEBUG
            NoStopMod.mod.Logger.Log("CallFrom Start");
#endif
            FixOffsetTick();
            this.previousFrameTime = Time.unscaledTime;
            if (__instance.song.pitch == 0f && !__instance.isLevelEditor)
            {
                Debug.LogError("Song pitch is zero set to zero?!");
            }
            if (__instance.controller != null)
            {
                __instance.controller.startVolume = __instance.song.volume;
            }

        }

        // Token: 0x060001C5 RID: 453 RVA: 0x0000CC64 File Offset: 0x0000AE64
        public void Rewind(scrConductor __instance)
        {
            __instance.tick = null;
            __instance.tickclip = null;
            __instance.isGameWorld = true;
            __instance.rescrub = true;
            __instance.crotchet = 0.0;
            this.nextBeatTime = 0.0;
            this.nextBarTime = 0.0;
            __instance.beatNumber = 0;
            __instance.barNumber = 0;
            __instance.hasSongStarted = false;
            __instance.getSpectrum = false;
            __instance.txtStatus = null;
            __instance.txtOffset = null;
            this.dspTimeSong = 0.0;
            __instance.lastHit = 0.0;
            this.lastReportedPlayheadPosition = AudioSettings.dspTime;
            __instance.dspTime = AudioSettings.dspTime;
#if DEBUG
            NoStopMod.mod.Logger.Log("callFrom Rewind");
#endif
            FixOffsetTick();
            this.previousFrameTime = (double)Time.unscaledTime;

        }

        // Token: 0x060001C6 RID: 454 RVA: 0x0000CD24 File Offset: 0x0000AF24
        public void SetupConductorWithLevelData(scrConductor __instance, LevelData levelData)
        {
            __instance.bpm = levelData.bpm;
            __instance.crotchet = (double)(60f / __instance.bpm);
            __instance.crotchetAtStart = __instance.crotchet;
            __instance.addoffset = (double)((float)levelData.offset * 0.001f);
            __instance.song.volume = (float)levelData.volume * 0.01f;
            __instance.hitSoundVolume = (float)levelData.hitsoundVolume * 0.01f;
            __instance.hitSound = levelData.hitsound;
            __instance.separateCountdownTime = levelData.separateCountdownTime;
            float num = (float)levelData.pitch * 0.01f;
            if (GCS.standaloneLevelMode)
            {
                num *= GCS.currentSpeedRun;
            }
            __instance.song.pitch = num;
        }

        // StartMusicCo, DesyncFix
        public void PlayHitTimes(scrConductor __instance, double hitsoundPlayFrom)
        {
            if (this.playedHitSounds)
            {
                AudioManager.Instance.StopAllSounds();
            }
            this.playedHitSounds = true;
            if (ADOBase.sceneName.Contains("scnCalibration") ||
                __instance.lm == null ||
                !GCS.d_hitsounds ||
                (__instance.controller != null && !__instance.controller.isLevelEditor && !__instance.forceHitSounds))
            {
                return;
            }
            HitSound hitSound = __instance.hitSound;
            float volume = __instance.hitSoundVolume;
            List<scrFloor> floorList = __instance.lm.listFloors;
            int num = (GCS.checkpointNum < floorList.Count && GCS.usingCheckpoints) ? (GCS.checkpointNum + 1) : 1;
            //double num2 = this.dspTimeSong + __instance.addoffset / (double)__instance.song.pitch;

            this.hitSoundsData = new List<HitSoundsData>();
            this.nextHitSoundToSchedule = 0;
            for (int i = 1; i < floorList.Count; i++)
            {
                scrFloor scrFloor = floorList[i];
                ffxSetHitsound setHitsound = scrFloor.setHitsound;
                if (setHitsound != null)
                {
                    hitSound = setHitsound.hitSound;
                    volume = setHitsound.volume;
                }
                double hitsoundOffset = (hitSound == HitSound.Shaker || hitSound == HitSound.ShakerLoud) ? 0.015 : 0.0;
                double time = hitsoundPlayFrom + scrFloor.entryTimePitchAdj - hitsoundOffset;
                if (i >= num && time > __instance.dspTime && !scrFloor.midSpin && hitSound != HitSound.None)
                {
                    HitSoundsData item = new HitSoundsData(hitSound, time, volume);
                    this.hitSoundsData.Add(item);
                }
            }

            if (__instance.playCountdownHihats)
            {
                if (!__instance.fastTakeoff)
                {
                    __instance.countdownTimes = new double[__instance.countdownTicks];
                    for (int j = 0; j < __instance.countdownTicks; j++)
                    {
                        double countdownTime = this.GetCountdownTime(__instance, j);
                        if (countdownTime > __instance.dspTime)
                        {
                            __instance.countdownTimes[j] = countdownTime;
                            AudioManager.Play("sndHat", countdownTime, __instance.hitSoundVolume, 10);
                        }
                    }
                }
                if (__instance.playEndingCymbal)
                {
                    AudioManager.Play("sndCymbalCrash", this.dspTimeSong + __instance.lm.listFloors[__instance.lm.listFloors.Count - 1].entryTimePitchAdj +
                        __instance.addoffset / __instance.song.pitch, __instance.hitSoundVolume, 128);
                }
            }
        }

        // Token: 0x060001C8 RID: 456 RVA: 0x0000D028 File Offset: 0x0000B228
        public double GetCountdownTime(scrConductor __instance, int i)
        {
            if (GCS.checkpointNum != 0 && GCS.usingCheckpoints)
            {
                int index = (GCS.checkpointNum != __instance.lm.listFloors.Count - 1) ? (GCS.checkpointNum + 1) : GCS.checkpointNum;
                return this.dspTimeSong + __instance.lm.listFloors[index].entryTimePitchAdj - (double)(__instance.countdownTicks - i) * (__instance.crotchet / (double)__instance.lm.listFloors[GCS.checkpointNum].speed) / (double)__instance.song.pitch + __instance.addoffset / (double)__instance.song.pitch;
            }
            return this.dspTimeSong + (double)i * __instance.crotchet / (double)__instance.song.pitch + __instance.addoffset / (double)__instance.song.pitch;
        }

        // Token: 0x060001C9 RID: 457 RVA: 0x0000D10E File Offset: 0x0000B30E
        public void StartMusic(scrConductor __instance, Action onComplete = null, Action onSongScheduled = null)
        {
            if (this.startMusicCoroutine != null)
            {
                __instance.StopCoroutine(this.startMusicCoroutine);
            }
            //AudioManager.Instance.StopAllSounds();
            this.startMusicCoroutine = __instance.StartCoroutine(this.StartMusicCo(__instance, onComplete, onSongScheduled));
        }

        // Token: 0x060001CA RID: 458 RVA: 0x0000D138 File Offset: 0x0000B338
        public IEnumerator StartMusicCo(scrConductor __instance, Action onComplete, Action onSongScheduled = null)
        {
            __instance.dspTime = AudioSettings.dspTime;
#if DEBUG
            NoStopMod.mod.Logger.Log("call From StartMusicCo First");
#endif
            FixOffsetTick();
            this.dspTimeSong = __instance.dspTime + (double)this.buffer + 0.1f;

            for (float timer = 0.1f; timer >= 0f; timer -= Time.deltaTime)
            {
                yield return null;
            }

            __instance.dspTime = AudioSettings.dspTime;
#if DEBUG
            NoStopMod.mod.Logger.Log("call From StartMusicCo Second");
#endif
            FixOffsetTick();

            double countdownTime = __instance.crotchetAtStart * __instance.countdownTicks;
            double separatedCountdownTime = __instance.separateCountdownTime ? countdownTime : 0.0;
            
            this.dspTimeSong = __instance.dspTime + this.buffer;
            if (__instance.fastTakeoff)
            {
                this.dspTimeSong -= countdownTime / __instance.song.pitch;
            }
            
            double time = this.dspTimeSong + separatedCountdownTime / __instance.song.pitch;

            __instance.song.UnPause();
            __instance.song.PlayScheduled(time);
            __instance.song2?.PlayScheduled(time);
            
            if (GCS.checkpointNum != 0)
            {
                yield return null;
                AudioListener.pause = true;
                __instance.song.SetScheduledStartTime(__instance.dspTime);

                double entryTime = __instance.lm.listFloors[FindSongStartTile(__instance, GCS.checkpointNum, RDC.auto && __instance.isLevelEditor)].entryTime;
                
                __instance.lastHit = entryTime;
                __instance.song.time = (float) (entryTime + __instance.addoffset - separatedCountdownTime);
                this.dspTimeSong = __instance.dspTime - (entryTime + __instance.addoffset) / __instance.song.pitch;
            }

            double hitSoundPlayFrom = this.dspTimeSong + __instance.addoffset / __instance.song.pitch;
            this.PlayHitTimes(__instance, hitSoundPlayFrom);
            __instance.hasSongStarted = true;


            onSongScheduled?.Invoke();

            yield return null;
            AudioListener.pause = false;
#if DEBUG
            NoStopMod.mod.Logger.Log("call From StartMusicCo Third");
#endif
            FixOffsetTick();

            yield return new WaitForSeconds(4f);
            while (__instance.song.isPlaying)
            {
                yield return null;
            }
            onComplete?.Invoke();

            yield break;
        }
        
        public IEnumerator ToggleHasSongStarted(scrConductor __instance, double songstarttime)
        {
            //NoStopMod.mod.Logger.Log("ToggleHasSongStarted");
            //if (GCS.d_webglConductor)
            //{
            //    __instance.song.volume = 0f;
            //}
            //while (scrConductor.instance.dspTime < songstarttime)
            //{
            //    yield return null;
            //}
            //__instance.hasSongStarted = true;
            //scrDebugHUDMessage.Log("Song started forreal!");
            //if (GCS.d_webglConductor)
            //{
            //    yield return new WaitForSeconds(0.2f);
            //    __instance.song.Pause();
            //    yield return new WaitForSeconds(0.1f);
            //    __instance.song.UnPause();
            //    __instance.song.volume = 1f;
            //}
            yield break;
        }
        
        public void Update(scrConductor __instance)
        {
            RDInput.Update();
            //if (!AudioListener.pause && Application.isFocused && Time.unscaledTime - this.previousFrameTime < 0.10000000149011612)
            //{
            //    __instance.dspTime += Time.unscaledTime - this.previousFrameTime;
            //    dspTick = NoStopMod.CurrFrameTick() - offsetTick;
            //    NoStopMod.mod.Logger.Log("dspTime : " + __instance.dspTime + ", " + (dspTick / 10000000.0));
            //}

            if (AudioListener.pause)
            {
                offsetTick += NoStopMod.CurrFrameTick() - NoStopMod.PrevFrameTick();
            }
            else
            {
                __instance.dspTime += Time.unscaledTime - this.previousFrameTime;
                dspTick = NoStopMod.CurrFrameTick() - offsetTick;
#if DEBUG
                NoStopMod.mod.Logger.Log("dspTime : " + __instance.dspTime + ", " + (dspTick / 10000000.0) + "diff(" + (__instance.dspTime - (dspTick / 10000000.0)) + ")");
#endif
            }

            this.previousFrameTime = Time.unscaledTime;
            if (AudioSettings.dspTime != this.lastReportedPlayheadPosition)
            {
                __instance.dspTime = AudioSettings.dspTime;
                this.lastReportedPlayheadPosition = AudioSettings.dspTime;
            }

            if (__instance.hasSongStarted && __instance.isGameWorld && (scrController.States)__instance.controller.GetState() != scrController.States.Fail && (scrController.States)__instance.controller.GetState() != scrController.States.Fail2)
            {
                while (this.nextHitSoundToSchedule < this.hitSoundsData.Count)
                {
                    HitSoundsData hitSoundsData = this.hitSoundsData[this.nextHitSoundToSchedule];
                    if (__instance.dspTime + 5.0 <= hitSoundsData.time)
                    {
                        break;
                    }
                    AudioManager.Play("snd" + hitSoundsData.hitSound, hitSoundsData.time, hitSoundsData.volume, 128);
                    this.nextHitSoundToSchedule++;
                }
            }
            __instance.crotchet = (double)(60f / __instance.bpm);
            double prevSongposition_minusi = __instance.songposition_minusi;
            __instance.songposition_minusi = getSongPosition(__instance, dspTick);
            //if (!GCS.d_oldConductor && !GCS.d_webglConductor)
            //{
            //    __instance.songposition_minusi = (double)((float)(__instance.dspTime - this.dspTimeSong - (double)scrConductor.calibration_i) * __instance.song.pitch) - __instance.addoffset;
            //}
            //else
            //{
            //    __instance.songposition_minusi = (double)(__instance.song.time - scrConductor.calibration_i) - __instance.addoffset / (double)__instance.song.pitch;
            //}

            __instance.deltaSongPos = __instance.songposition_minusi - prevSongposition_minusi;
            __instance.deltaSongPos = Math.Max(__instance.deltaSongPos, 0.0);
            if (__instance.songposition_minusi > this.nextBeatTime)
            {
                __instance.OnBeat();
                this.nextBeatTime += __instance.crotchet;
                __instance.beatNumber++;
            }
            if (__instance.songposition_minusi < this.nextBeatTime - __instance.crotchet)
            {
                this.nextBeatTime -= __instance.crotchet;
                __instance.beatNumber--;
            }
            if (__instance.songposition_minusi > this.nextBarTime)
            {
                this.nextBarTime += __instance.crotchet * (double)this.crotchetsPerBar;
                __instance.barNumber++;
            }

            if (Input.GetKeyDown(KeyCode.G) && Application.isEditor)
            {
                float time = __instance.song.time;
                if (__instance.separateCountdownTime)
                {
                    double num = __instance.crotchetAtStart;
                    int num2 = __instance.countdownTicks;
                }
                double songposition_minusi2 = __instance.songposition_minusi;
                float calibration_i = scrConductor.calibration_i;
                float pitch = __instance.song.pitch;
                double num3 = __instance.addoffset;
            }

            if (__instance.getSpectrum && !GCS.lofiVersion)
            {
                AudioSource audioSource = __instance.song;
                if (__instance.CLSComponent != null)
                {
                    PreviewSongPlayer previewSongPlayer = __instance.CLSComponent.previewSongPlayer;
                    if (previewSongPlayer.playing)
                    {
                        audioSource = previewSongPlayer.audioSource;
                    }
                }
                audioSource.GetSpectrumData(__instance.spectrum, 0, FFTWindow.BlackmanHarris);
            }
        }

        public void OnBeat(scrConductor __instance)
        {
            List<ADOBase> onBeats = __instance.onBeats;
            if (onBeats == null)
            {
                return;
            }
            int count = onBeats.Count;
            for (int i = 0; i < count; i++)
            {
                onBeats[i].OnBeat();
            }
            if (__instance.controller != null && __instance.controller.gameworld)
            {
                List<scrFloor> listFloors = __instance.controller.lm.listFloors;
                int count2 = listFloors.Count;
                for (int j = 0; j < count2; j++)
                {
                    listFloors[j].OnBeat();
                }
            }
        }

        // Token: 0x060001CE RID: 462 RVA: 0x0000D555 File Offset: 0x0000B755
        public void PlaySfx(scrConductor __instance, int num, float volume = 1f, bool ignoreListenerPause = false)
        {
            if (num == 2)
            {
                volume = 1.5f;
            }
            scrSfx.instance.Play(num, ignoreListenerPause, volume);
        }

        // Moved into startMusicCo
        public void ScrubMusicToTile(scrConductor __instance, int tileID)
        {
            //NoStopMod.mod.Logger.Log("ScrubMusicToTile");
            //AudioListener.pause = true;
            //AudioManager.Instance.StopAllSounds();
            //__instance.song.SetScheduledStartTime(__instance.dspTime);
            //double num = __instance.separateCountdownTime ? (__instance.crotchetAtStart * (double)__instance.countdownTicks) : 0.0;
            //__instance.song.time = (float)(__instance.lm.listFloors[tileID].entryTime + __instance.addoffset - num);
            //this.dspTimeSong = __instance.dspTime - __instance.lm.listFloors[tileID].entryTimePitchAdj - __instance.addoffset / (double)__instance.song.pitch;
            //__instance.lastHit = __instance.lm.listFloors[tileID].entryTime;
            //__instance.StartCoroutine(this.DesyncFix(__instance));
        }

        // Token: 0x060001D0 RID: 464 RVA: 0x0000D64B File Offset: 0x0000B84B
        public IEnumerator DesyncFix(scrConductor __instance)
        {
            //NoStopMod.mod.Logger.Log("DesyncFix");
            //int num;
            //for (int framecounty = 2; framecounty > 0; framecounty = num - 1)
            //{
            //    yield return 0;
            //    num = framecounty;
            //}
            //AudioListener.pause = false;
            //__instance.PlayHitTimes();

            //int numberOfAttempts = 0;
            //int framesToWait = 10;
            //double maxDifference = 0.005;
            //for (int i = 0; i < numberOfAttempts; i = num + 1)
            //{
            //    for (int framecount = framesToWait; framecount > 0; framecount = num - 1)
            //    {
            //        yield return 0;
            //        num = framecount;
            //    }
            //    if (__instance.song.isPlaying || __instance.song.clip == null)
            //    {
            //        yield break;
            //    }
            //    double num2 = (double)__instance.song.time + (__instance.separateCountdownTime ? (__instance.crotchetAtStart * (double)__instance.countdownTicks) : 0.0);
            //    double num3 = __instance.songposition_minusi + (double)(scrConductor.calibration_i * __instance.song.pitch) + __instance.addoffset;
            //    if (Math.Abs(num3 - num2) > maxDifference)
            //    {
            //        double num4 = num2 - num3;
            //        Debug.Log("Desync Fix Attempt: found difference " + num4);
            //        Debug.Log("Attempt " + i);
            //        Debug.Log("song time " + num2);
            //        Debug.Log("dsptime " + num3);
            //        this.dspTimeSong -= num4;
            //        __instance.PlayHitTimes();
            //    }
            //    num = i;
            //}
            yield break;
        }

        // Token: 0x060001D7 RID: 471 RVA: 0x0000D89B File Offset: 0x0000BA9B
        private int GetOffsetChange(bool fine)
        {
            return fine ? 1 : 10;
        }

        // Token: 0x060001DC RID: 476 RVA: 0x0000D91C File Offset: 0x0000BB1C
        public static void SaveCurrentPreset()
        {
            for (int i = 0; i < scrConductor.userPresets.Count; i++)
            {
                CalibrationPreset calibrationPreset = scrConductor.userPresets[i];
                if (scrConductor.currentPreset.outputType == calibrationPreset.outputType && scrConductor.currentPreset.outputName == calibrationPreset.outputName)
                {
                    RDBaseDll.printem("found preset, modifying.");
                    calibrationPreset.inputOffset = scrConductor.currentPreset.inputOffset;
                    scrConductor.userPresets[i] = calibrationPreset;
                    return;
                }
            }
            RDBaseDll.printem("adding preset: " + scrConductor.currentPreset);
            scrConductor.userPresets.Add(scrConductor.currentPreset);
        }


        // Token: 0x060001E1 RID: 481 RVA: 0x0000DB61 File Offset: 0x0000BD61
        public void SaveVisualOffset(double offset)
        {
            Persistence.SetVisualOffset((float)offset);
            PlayerPrefs.SetFloat("offset_v", (float)offset);
            PlayerPrefs.Save();
        }

        // Token: 0x060001E3 RID: 483 RVA: 0x0000DC98 File Offset: 0x0000BE98
        //public void LoadOnBeats()
        //{
        //    if (this._onBeats.Count != 0)
        //    {
        //        return;
        //    }
        //    this._onBeats = new List<ADOBase>();
        //    GameObject[] array = GameObject.FindGameObjectsWithTag("Beat");
        //    for (int i = 0; i < array.Length; i++)
        //    {
        //        ADOBase component = array[i].GetComponent<ADOBase>();
        //        if (component != null && component)
        //        {
        //            this._onBeats.Add(component);
        //        }
        //    }
        //}




    }
}

