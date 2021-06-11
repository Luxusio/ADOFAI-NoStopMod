using System;
using UnityEngine.Scripting;

namespace NoStopMod.GarbageCollection
{
    public class GCManager
    {

        private bool gcEnabled = true;
        
        public GCManager()
        {
            NoStopMod.onToggleListeners.Add(OnToggle);
        }

        private void OnToggle(bool enabled)
        {
            EnableGC();
        }

        public bool GetDisableAutoSave()
        {
            return !gcEnabled;
        }

        public void DisableGC()
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

        public void EnableGC()
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
