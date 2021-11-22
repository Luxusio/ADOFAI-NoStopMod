namespace NoStopMod.InputFixer.SyncFixer
{
    class SyncFixerManager
    {

        public static double dspTime;
        public static double dspTimeSong;

        public static long offsetTick;

        public static double lastReportedDspTime;

        public static double previousFrameTime;

        public static void Init()
        {
        }
        
        public static double GetSongPosition(scrConductor __instance, long nowTick)
        {
            if (!GCS.d_oldConductor && !GCS.d_webglConductor)
            {
                return ((nowTick / 10000000.0 - SyncFixerManager.dspTimeSong - scrConductor.calibration_i) * __instance.song.pitch) - __instance.addoffset;
            }
            else
            {
                return (__instance.song.time - scrConductor.calibration_i) - __instance.addoffset / __instance.song.pitch;
            }
        }

    }
}
