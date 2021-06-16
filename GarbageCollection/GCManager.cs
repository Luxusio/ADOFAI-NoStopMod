using System;
using UnityEngine.Scripting;

namespace NoStopMod.GarbageCollection
{
    public class GCManager
    {

        private static bool gcEnabled = true;
        
        public static void Init()
        {
            NoStopMod.onToggleListeners.Add(OnToggle);
        }

        private static void OnToggle(bool enabled)
        {
            EnableGC();
        }

        public static bool GetDisableAutoSave()
        {
            return !gcEnabled;
        }

        public static void DisableGC()
        {
            try
            {
                gcEnabled = false;
                //NoStopMod.mod.Logger.Log("disablegc");
                GarbageCollector.GCMode = GarbageCollector.Mode.Disabled;
                System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.SustainedLowLatency;
            }
            catch (NotImplementedException e)
            {
                NoStopMod.mod.Logger.Log("Exception occur");
                NoStopMod.mod.Logger.Error(e.ToString());
            }
        }

        public static void EnableGC()
        {
            try
            {
                gcEnabled = true;
                //NoStopMod.mod.Logger.Log("enablegc");
                GarbageCollector.GCMode = GarbageCollector.Mode.Enabled;
                System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.Interactive;
                GC.Collect();
            }
            catch (NotImplementedException e)
            {
                NoStopMod.mod.Logger.Log("Exception occur");
                NoStopMod.mod.Logger.Error(e.ToString());
            }
        }

    }
}
