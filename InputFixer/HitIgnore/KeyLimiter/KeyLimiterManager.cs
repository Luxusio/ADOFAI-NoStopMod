using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityModManagerNet;

namespace NoStopMod.InputFixer.HitIgnore.KeyLimiter
{
    class KeyLimiterManager
    {

        public static KeyLimiterSettings settings;

        public static void Init()
        {
            NoStopMod.onGUIListeners.Add(OnGUI);

            settings = new KeyLimiterSettings();
            Settings.settings.Add(settings);
            
        }

        private static void OnGUI(UnityModManager.ModEntry entry)
        {

        }

    }
}
