using System;

namespace NoStopMod.Helper
{
    public class ControllerHelper
    {

        public static void ExecuteUntilTileNotChange(scrController controller, Action action)
        {
            int lastTileNum = -1;
            while (lastTileNum != controller.currFloor.seqID)
            {
                lastTileNum = controller.currFloor.seqID;
                action();
            }
        }
        
        
        
    }
}