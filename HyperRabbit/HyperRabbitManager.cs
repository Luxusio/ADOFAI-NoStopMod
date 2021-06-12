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


        public bool isEnabled;


        enum Status
        {
            OFF,
            FIXED_OTTO,
            HYPER_RABBIT,
        }

        public HyperRabbitManager()
        {
            NoStopMod.onToggleListeners.Add(OnToggle);
            NoStopMod.onGUIListeners.Add(OnGUI);
        }

        private void OnToggle(bool enabled)
        {
            this.isEnabled = enabled;
        }

        private void OnGUI(UnityModManager.ModEntry modEntry)
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
