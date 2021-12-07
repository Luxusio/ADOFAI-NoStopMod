using NoStopMod.InputFixer.HitIgnore.KeyLimiter;
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using KeyCode = SharpHook.Native.KeyCode;

namespace NoStopMod.InputFixer.HitIgnore
{
    class HitIgnoreManager
    {
        [CanBeNull] private static Dictionary<String, HashSet<KeyCode>> dictionary;

        public static bool scnCLS_searchMode;
        public static scrController.States scrController_state;
        
        public static void Init()
        {
            if (GCS.sceneToLoad == null) GCS.sceneToLoad = "scnNewIntro";

            dictionary = new Dictionary<string, HashSet<KeyCode>>();

            HashSet<KeyCode> ignoreScnNewIntro = new HashSet<KeyCode>();
            dictionary["scnNewIntro"] = ignoreScnNewIntro;
           
            ignoreScnNewIntro.Add(KeyCode.VcQuote);
            ignoreScnNewIntro.Add(KeyCode.Vc0);
            ignoreScnNewIntro.Add(KeyCode.Vc1);
            ignoreScnNewIntro.Add(KeyCode.Vc2);
            ignoreScnNewIntro.Add(KeyCode.Vc3);
            ignoreScnNewIntro.Add(KeyCode.Vc4);
            ignoreScnNewIntro.Add(KeyCode.Vc5);
            ignoreScnNewIntro.Add(KeyCode.Vc6);
            ignoreScnNewIntro.Add(KeyCode.Vc7);

            HashSet<KeyCode> ignoreScnCLS = new HashSet<KeyCode>();
            dictionary["scnCLS"] = ignoreScnCLS;
            ignoreScnCLS.Add(KeyCode.VcS);
            ignoreScnCLS.Add(KeyCode.VcDelete);
            ignoreScnCLS.Add(KeyCode.VcF);
            ignoreScnCLS.Add(KeyCode.VcO);
            ignoreScnCLS.Add(KeyCode.Vc7);
            ignoreScnCLS.Add(KeyCode.VcUp);
            ignoreScnCLS.Add(KeyCode.VcDown);

            scnCLS_searchMode = false;

            if (scrController.instance != null)
            {
                scrController_state = (scrController.States) scrController.instance.GetState();
            }

            KeyLimiterManager.Init();
        }

        public static bool ShouldBeIgnored(KeyCode keyCode)
        {
            if (keyCode == KeyCode.VcEscape) return true;
            if (KeyLimiterManager.isChangingLimitedKeys)
            {
                KeyLimiterManager.UpdateKeyLimiter(keyCode);
                return true;
            }

            if (scrController_state != scrController.States.PlayerControl)
            {
                return true;
            }

            HashSet<KeyCode> ignoreScnCLS;
            if (dictionary.TryGetValue(GCS.sceneToLoad, out ignoreScnCLS))
            {
                if (GCS.sceneToLoad == "scnCLS" && scnCLS_searchMode)
                {
                    return true;
                }
                if (ignoreScnCLS.Contains(keyCode))
                {
                    return true;
                }
            }
            
            if (KeyLimiterManager.settings.enable)
            {
                return !KeyLimiterManager.IsKeyEnabled(keyCode);
            }
            
            return false;
        }


    }
}
