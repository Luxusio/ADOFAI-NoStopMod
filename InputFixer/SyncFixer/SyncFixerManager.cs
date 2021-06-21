using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoStopMod.InputFixer.SyncFixer
{
    class SyncFixerManager
    {

        public static newScrConductor newScrConductor;
        
        public static void Init()
        {
            newScrConductor = new newScrConductor();
            //if (newScrConductor.instance != null) newScrConductor.FixOffsetTick();
        }
        
    }
}
