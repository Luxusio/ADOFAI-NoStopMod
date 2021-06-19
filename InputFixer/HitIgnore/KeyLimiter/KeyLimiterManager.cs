using System;
using System.Linq;
using UnityEngine;
using UnityModManagerNet;

namespace NoStopMod.InputFixer.HitIgnore.KeyLimiter
{
    class KeyLimiterManager
    {

        public static KeyLimiterSettings settings;

        public static bool isChangingLimitedKeys = false;

        private static bool[] enableKey = Enumerable.Repeat(false, 1024).ToArray();

        public static void Init()
        {
            NoStopMod.onGUIListener.Add(OnGUI);
            NoStopMod.onHideGUIListener.Add(OnHideGUI);
            Settings.settingsLoadListener.Add(_ => InitEnableKey());

            settings = new KeyLimiterSettings();
            Settings.settings.Add(settings);
        }

        private static void InitEnableKey()
        {
            enableKey = Enumerable.Repeat(false, 1024).ToArray();
            for (int i = 0; i < settings.limitKeys.Count; i++)
            {
                enableKey[(int)settings.limitKeys[i]] = true;
            }
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

        public static bool IsKeyEnabled(KeyCode keyCode)
        {
            return enableKey[(int) keyCode];
        }

        public static void UpdateKeyLimiter(KeyCode keyCode)
        {
            int idx = settings.limitKeys.IndexOf(keyCode);
            if (idx == -1)
            {
                settings.limitKeys.Add(keyCode);
                enableKey[(int)keyCode] = true;
            }
            else
            {
                settings.limitKeys.RemoveAt(idx);
                enableKey[(int)keyCode] = false;
            }

        }

    }
}
