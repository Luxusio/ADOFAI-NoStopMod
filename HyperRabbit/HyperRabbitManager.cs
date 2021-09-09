using UnityEngine;
using UnityModManagerNet;

namespace NoStopMod.HyperRabbit
{
    class HyperRabbitManager
    {

        public static HyperRabbitSettings settings;

        public static void Init()
        {
            NoStopMod.onGUIListener.Add(OnGUI);

            settings = new HyperRabbitSettings();
            Settings.settings.Add(settings);
        }

        private static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            GUILayout.BeginHorizontal("HyperRabbit");
            GUILayout.TextArea("Max tile per frame", 20);

            settings.maxTilePerFrame = (int) GUILayout.HorizontalSlider(settings.maxTilePerFrame, 1, 1000);
            
            GUILayout.EndHorizontal();
        }

    }
}
