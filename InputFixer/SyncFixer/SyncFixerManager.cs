namespace NoStopMod.InputFixer.SyncFixer
{
    class SyncFixerManager
    {

        public static newScrConductor newScrConductor;
        
        public static void Init()
        {
            newScrConductor = new newScrConductor();
            if (newScrConductor.instance != null) newScrConductor.FixOffsetTick();
        }
        
    }
}
