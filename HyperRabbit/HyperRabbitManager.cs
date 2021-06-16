using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityModManagerNet;

namespace NoStopMod.HyperRabbit
{
    class HyperRabbitManager
    {

        public static bool isEnabled;
        
        enum Status
        {
            OFF,
            FIXED_OTTO,
            HYPER_RABBIT,
        }

        public static void Init()
        {
            NoStopMod.onToggleListeners.Add(OnToggle);
            NoStopMod.onGUIListeners.Add(OnGUI);
        }

        private static void OnToggle(bool enabled)
        {
            isEnabled = enabled;
        }

        private static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            GUILayout.BeginHorizontal("HyperRabbit");
            GUILayout.TextArea("HyperRabbit Status", 20);
            //if (GUILayout.Button(tex))
            //{

            //}
            //this.isEnabled = GUILayout.Toggle(isEnabled, "HyperRabbit");
            GUILayout.EndHorizontal();
        }

    }
}
