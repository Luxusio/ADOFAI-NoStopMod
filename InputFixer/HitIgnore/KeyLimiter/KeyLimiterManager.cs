using System;
using UnityEngine;
using UnityModManagerNet;

namespace NoStopMod.InputFixer.HitIgnore.KeyLimiter
{
    class KeyLimiterManager
    {

        public static KeyLimiterSettings settings;

        public static bool isChangingLimitedKeys = false;

        public static void Init()
        {
            NoStopMod.onGUIListener.Add(OnGUI);
            NoStopMod.onHideGUIListener.Add(OnHideGUI);

            settings = new KeyLimiterSettings();
            Settings.settings.Add(settings);
            
        }

        private static void OnGUI(UnityModManager.ModEntry entry)
        {
            settings.enable = GUILayout.Toggle(settings.enable, "Enable key limiter");
            if (settings.enable)
            {
                //GUILayout.Label("", Array.Empty<GUILayoutOption>());
                GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
                GUILayout.Space(20f);

                GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
                GUILayout.Space(8f);
                GUILayout.EndVertical();

                for (int i = 0; i < settings.limitKeys.Count; i++)
                {
                    GUILayout.Label(settings.limitKeys[i].ToString(), Array.Empty<GUILayoutOption>());
                    GUILayout.Space(8f);
                }
                
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
                if (GUILayout.Button(isChangingLimitedKeys ? "Complete" : "Change Limited Keys", Array.Empty<GUILayoutOption>()))
                {
                    isChangingLimitedKeys = !isChangingLimitedKeys;
                }
                if (isChangingLimitedKeys)
                {
                    GUILayout.Label("Press keys to add / remove limited key", Array.Empty<GUILayoutOption>());
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                
            }
        }

        private static void OnHideGUI(UnityModManager.ModEntry entry)
        {
            isChangingLimitedKeys = false;
        }

    }
}
