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
            GUILayout.BeginHorizontal();
            GUILayout.Label("Max otto tile per frame (5 + " + settings.maxTilePerFrame + ")");

            settings.maxTilePerFrame = (int) GUILayout.HorizontalSlider(settings.maxTilePerFrame, 0, 100);

            GUILayout.EndHorizontal();
        }

    }
}
