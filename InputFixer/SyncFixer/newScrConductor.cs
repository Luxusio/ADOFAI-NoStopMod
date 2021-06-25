using NoStopMod.Helper;
using RDTools;
using System;
using System.Collections;
using System.Collections.Generic;
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

        // new code
        public long offsetTick;

        public long dspTick;

        public double dspTime;

        public static ReflectionField<double> dspTimeField = new ReflectionField<double>("dspTime", "_dspTime");

        public void FixOffsetTick()
        {
            offsetTick = NoStopMod.CurrFrameTick() - (long)(this.dspTime * 10000000);
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
        public List<HitSoundsData> hitSoundsData = new List<HitSoundsData>();
        
        public int nextHitSoundToSchedule;
        
        public int crotchetsPerBar = 8;
        
        //public double _songposition_minusi;
        
        public double previousFrameTime;
        
        public double lastReportedPlayheadPosition;
        
        public double nextBeatTime;
        
        public double nextBarTime;
        
        public float buffer = 1f;
        
        public float[] _spectrum = new float[1024];
        
        //public AudioSource audiosource;
        
        //public bool playedHitSounds;
        
        //public scrConductor.DuckState _duckState;
        
        //public float songPreduckVolume;
        
        //public float song2PreduckVolume;
        
        public double dspTimeSong;
        
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
        
        private static scrConductor _instance;
        
        public Coroutine startMusicCoroutine;

        public struct HitSoundsData
        {
            public HitSoundsData(HitSound hitSound, double time, float volume)
            {
                this.hitSound = hitSound;
                this.time = time;
                this.volume = volume;
                this.played = false;
            }
            
            public HitSound hitSound;
            public double time;
            public float volume;
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
            this.dspTime = AudioSettings.dspTime;
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
            this.dspTime = AudioSettings.dspTime;
#if DEBUG
            NoStopMod.mod.Logger.Log("callFrom Rewind");
#endif
            FixOffsetTick();
            this.previousFrameTime = (double)Time.unscaledTime;

        }
        
        //public void SetupConductorWithLevelData(scrConductor __instance, LevelData levelData)
        //{
        //    __instance.bpm = levelData.bpm;
        //    __instance.crotchet = (double)(60f / __instance.bpm);
        //    __instance.crotchetAtStart = __instance.crotchet;
        //    __instance.addoffset = (double)((float)levelData.offset * 0.001f);
        //    __instance.song.volume = (float)levelData.volume * 0.01f;
        //    __instance.hitSoundVolume = (float)levelData.hitsoundVolume * 0.01f;
        //    __instance.hitSound = levelData.hitsound;
        //    __instance.separateCountdownTime = levelData.separateCountdownTime;
        //    float num = (float)levelData.pitch * 0.01f;
        //    if (GCS.standaloneLevelMode)
        //    {
        //        num *= GCS.currentSpeedRun;
        //    }
        //    __instance.song.pitch = num;
        //}

        // StartMusicCo, DesyncFix
        public void PlayHitTimes(scrConductor __instance, double hitsoundPlayFrom)
        {
            this.hitSoundsData = new List<HitSoundsData>();
            //if (this.playedHitSounds)
            //{
            //    AudioManager.Instance.StopAllSounds();
            //}
            //this.playedHitSounds = true;
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

            this.nextHitSoundToSchedule = 0;
            for (int i = 1; i < floorList.Count; i++)
            {
                scrFloor scrFloor = floorList[i];
                ffxSetHitsound setHitsound = scrFloor.GetComponent<ffxSetHitsound>();
                if (setHitsound != null)
                {
                    hitSound = setHitsound.hitSound;
                    volume = setHitsound.volume;
                }
                double hitsoundOffset = (hitSound == HitSound.Shaker || hitSound == HitSound.ShakerLoud) ? 0.015 : 0.0;
                double time = hitsoundPlayFrom + scrFloor.entryTimePitchAdj - hitsoundOffset;
                if (i >= num && time > this.dspTime && !scrFloor.midSpin && hitSound != HitSound.None)
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
                        if (countdownTime > this.dspTime)
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
        
        public double GetCountdownTime(scrConductor __instance, int i)
        {
            if (GCS.checkpointNum != 0 && GCS.usingCheckpoints)
            {
                int index = (GCS.checkpointNum != __instance.lm.listFloors.Count - 1) ? (GCS.checkpointNum + 1) : GCS.checkpointNum;
                return this.dspTimeSong + __instance.lm.listFloors[index].entryTimePitchAdj - (double)(__instance.countdownTicks - i) * (__instance.crotchet / (double)__instance.lm.listFloors[GCS.checkpointNum].speed) / (double)__instance.song.pitch + __instance.addoffset / (double)__instance.song.pitch;
            }
            return this.dspTimeSong + (double)i * __instance.crotchet / (double)__instance.song.pitch + __instance.addoffset / (double)__instance.song.pitch;
        }
        
        public void StartMusic(scrConductor __instance, Action onComplete = null, Action onSongScheduled = null)
        {
            if (this.startMusicCoroutine != null)
            {
                __instance.StopCoroutine(this.startMusicCoroutine);
            }
            this.startMusicCoroutine = __instance.StartCoroutine(this.StartMusicCo(__instance, onComplete, onSongScheduled));
        }
        
        public IEnumerator StartMusicCo(scrConductor __instance, Action onComplete, Action onSongScheduled = null)
        {
            this.dspTime = AudioSettings.dspTime;
#if DEBUG
            NoStopMod.mod.Logger.Log("call From StartMusicCo First");
#endif
            FixOffsetTick();
            this.dspTimeSong = this.dspTime + (double)this.buffer + 0.1f;

            for (float timer = 0.1f; timer >= 0f; timer -= Time.deltaTime)
            {
                yield return null;
            }

            this.dspTime = AudioSettings.dspTime;
#if DEBUG
            NoStopMod.mod.Logger.Log("call From StartMusicCo Second");
#endif
            FixOffsetTick();

            double countdownTime = __instance.crotchetAtStart * __instance.countdownTicks;
            double separatedCountdownTime = __instance.separateCountdownTime ? countdownTime : 0.0;

            this.dspTimeSong = this.dspTime + this.buffer;
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
                __instance.song.SetScheduledStartTime(this.dspTime);

                double entryTime = __instance.lm.listFloors[FindSongStartTile(__instance, GCS.checkpointNum, RDC.auto && __instance.isLevelEditor)].entryTime;

                __instance.lastHit = entryTime;
                __instance.song.time = (float)(entryTime + __instance.addoffset - separatedCountdownTime);
                this.dspTimeSong = this.dspTime - (entryTime + __instance.addoffset) / __instance.song.pitch;
            }
            onSongScheduled?.Invoke();

            double hitSoundPlayFrom = this.dspTimeSong + __instance.addoffset / __instance.song.pitch;
            this.PlayHitTimes(__instance, hitSoundPlayFrom);
            
            __instance.hasSongStarted = true;

            yield return null;
            AudioListener.pause = false;

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
            yield break;
        }

        public void Update(scrConductor __instance)
        {
            RDInput.Update();

            if (AudioListener.pause)
            {
                offsetTick += NoStopMod.CurrFrameTick() - NoStopMod.PrevFrameTick();
            }
            else
            {
                this.dspTime += Time.unscaledTime - this.previousFrameTime;
                dspTick = NoStopMod.CurrFrameTick() - offsetTick;
#if DEBUG
                NoStopMod.mod.Logger.Log("dspTime : " + __instance.dspTime + ", " + (dspTick / 10000000.0) + "diff(" + (__instance.dspTime - (dspTick / 10000000.0)) + ")");
#endif
            }

            this.previousFrameTime = Time.unscaledTime;
            if (AudioSettings.dspTime != this.lastReportedPlayheadPosition)
            {
                this.dspTime = AudioSettings.dspTime;
                this.lastReportedPlayheadPosition = AudioSettings.dspTime;
                FixOffsetTick();
            }

            if (__instance.hasSongStarted && __instance.isGameWorld && (scrController.States)__instance.controller.GetState() != scrController.States.Fail && (scrController.States)__instance.controller.GetState() != scrController.States.Fail2)
            {
                while (this.nextHitSoundToSchedule < this.hitSoundsData.Count)
                {
                    HitSoundsData hitSoundsData = this.hitSoundsData[this.nextHitSoundToSchedule];
                    if (this.dspTime + 5.0 <= hitSoundsData.time)
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

        //public void OnBeat(scrConductor __instance)
        //{
        //    List<ADOBase> onBeats = __instance.onBeats;
        //    if (onBeats == null)
        //    {
        //        return;
        //    }
        //    int count = onBeats.Count;
        //    for (int i = 0; i < count; i++)
        //    {
        //        onBeats[i].OnBeat();
        //    }
        //    if (__instance.controller != null && __instance.controller.gameworld)
        //    {
        //        List<scrFloor> listFloors = __instance.controller.lm.listFloors;
        //        int count2 = listFloors.Count;
        //        for (int j = 0; j < count2; j++)
        //        {
        //            listFloors[j].OnBeat();
        //        }
        //    }
        //}
        
        public void PlaySfx(scrConductor __instance, int num, float volume = 1f, bool ignoreListenerPause = false)
        {
            if (num == 2)
            {
                volume = 1.5f;
            }
            scrSfx.instance.Play(num, ignoreListenerPause, volume);
        }
        
        public void ScrubMusicToTile(scrConductor __instance, int tileID)
        {
        }
        
        public IEnumerator DesyncFix(scrConductor __instance)
        {
            yield break;
        }
        
        private int GetOffsetChange(bool fine)
        {
            return fine ? 1 : 10;
        }
        
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

        
        public void SaveVisualOffset(double offset)
        {
            Persistence.SetVisualOffset((float)offset);
            PlayerPrefs.SetFloat("offset_v", (float)offset);
            PlayerPrefs.Save();
        }
        
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

