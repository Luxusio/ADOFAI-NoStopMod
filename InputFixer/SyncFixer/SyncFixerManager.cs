namespace NoStopMod.InputFixer.SyncFixer
{
    class SyncFixerManager
    {

        public static newScrConductor newScrConductor;
        
        public static void Init()
        {
            newScrConductor = new newScrConductor();
            if (scrConductor.instance != null)
            {
                newScrConductor.dspTime = newScrConductor.dspTimeField.GetValue(scrConductor.instance);
                newScrConductor.FixOffsetTick();
            }
        }
        
    }
}
