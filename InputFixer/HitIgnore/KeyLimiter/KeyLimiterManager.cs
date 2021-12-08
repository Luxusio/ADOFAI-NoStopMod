using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityModManagerNet;
using KeyCode = SharpHook.Native.KeyCode;

namespace NoStopMod.InputFixer.HitIgnore.KeyLimiter
{
    class KeyLimiterManager
    {

        public static KeyLimiterSettings settings;

        public static bool isChangingLimitedKeys = false;

        private static HashSet<KeyCode> enableKey = new HashSet<KeyCode>();

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
            enableKey.Clear();
            for (int i = 0; i < settings.limitKeys.Count; i++)
            {
                enableKey.Add(settings.limitKeys[i]);
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
                    Settings.Save();
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
            Settings.Save();
        }

        public static bool IsKeyEnabled(KeyCode keyCode)
        {
            return enableKey.Contains(keyCode);
        }

        public static void UpdateKeyLimiter(KeyCode keyCode)
        {
            int idx = settings.limitKeys.IndexOf(keyCode);
            if (idx == -1)
            {
                settings.limitKeys.Add(keyCode);
                enableKey.Add(keyCode);
            }
            else
            {
                settings.limitKeys.RemoveAt(idx);
                enableKey.Remove(keyCode);
            }

        }

    }
}
