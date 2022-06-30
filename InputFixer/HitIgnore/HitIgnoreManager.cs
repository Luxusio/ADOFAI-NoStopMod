using NoStopMod.InputFixer.HitIgnore.KeyLimiter;
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using KeyCode = SharpHook.Native.KeyCode;

namespace NoStopMod.InputFixer.HitIgnore
{
    class HitIgnoreManager
    {
        private static readonly Dictionary<String, HashSet<KeyCode>> _dictionary = new Dictionary<string, HashSet<KeyCode>>();

        public static bool scnCLS_searchMode;
        public static scrController.States scrController_state;

        public static void Init()
        {
            if (GCS.sceneToLoad == null) GCS.sceneToLoad = "scnNewIntro";

            HashSet<KeyCode> ignoreScnNewIntro = new HashSet<KeyCode>();
            _dictionary["scnNewIntro"] = ignoreScnNewIntro;

            ignoreScnNewIntro.Add(KeyCode.VcQuote);
            ignoreScnNewIntro.Add(KeyCode.VcBackquote);
            ignoreScnNewIntro.Add(KeyCode.Vc0);
            ignoreScnNewIntro.Add(KeyCode.Vc1);
            ignoreScnNewIntro.Add(KeyCode.Vc2);
            ignoreScnNewIntro.Add(KeyCode.Vc3);
            ignoreScnNewIntro.Add(KeyCode.Vc4);
            ignoreScnNewIntro.Add(KeyCode.Vc5);
            ignoreScnNewIntro.Add(KeyCode.Vc6);
            ignoreScnNewIntro.Add(KeyCode.Vc7);

            HashSet<KeyCode> ignoreScnCLS_ = new HashSet<KeyCode>();
            _dictionary["scnCLS"] = ignoreScnCLS_;
            ignoreScnCLS_.Add(KeyCode.VcS);
            ignoreScnCLS_.Add(KeyCode.VcDelete);
            ignoreScnCLS_.Add(KeyCode.VcF);
            ignoreScnCLS_.Add(KeyCode.VcO);
            ignoreScnCLS_.Add(KeyCode.Vc7);
            ignoreScnCLS_.Add(KeyCode.VcUp);
            ignoreScnCLS_.Add(KeyCode.VcNumPadUp);
            ignoreScnCLS_.Add(KeyCode.VcDown);
            ignoreScnCLS_.Add(KeyCode.VcNumPadDown);

            HashSet<KeyCode> ignoreScnTaroMenu0 = new HashSet<KeyCode>();
            _dictionary["scnTaroMenu0"] = ignoreScnTaroMenu0;
            ignoreScnTaroMenu0.Add(KeyCode.Vc1);
            ignoreScnTaroMenu0.Add(KeyCode.Vc2);
            ignoreScnTaroMenu0.Add(KeyCode.Vc3);
            ignoreScnTaroMenu0.Add(KeyCode.Vc4);
            ignoreScnTaroMenu0.Add(KeyCode.Vc5);
            ignoreScnTaroMenu0.Add(KeyCode.Vc6);
            
            HashSet<KeyCode> ignoreScnTaroMenu3 = new HashSet<KeyCode>();
            _dictionary["scnTaroMenu3"] = ignoreScnTaroMenu3;
            ignoreScnTaroMenu3.Add(KeyCode.Vc0);
            ignoreScnTaroMenu3.Add(KeyCode.Vc1);
            ignoreScnTaroMenu3.Add(KeyCode.Vc2);
            ignoreScnTaroMenu3.Add(KeyCode.Vc3);
            ignoreScnTaroMenu3.Add(KeyCode.Vc4);
            ignoreScnTaroMenu3.Add(KeyCode.Vc5);

            scnCLS_searchMode = false;

            if (scrController.instance != null)
            {
                scrController_state = (scrController.States) scrController.instance.GetState();
            }

            KeyLimiterManager.Init();
        }

        public static bool ShouldBeIgnored(KeyCode keyCode)
        {
#if DEBUG
            NoStopMod.mod.Logger.Log($"scene : {GCS.sceneToLoad}, keyCode: {keyCode}");
#endif
            
            if (keyCode == KeyCode.VcEscape) return true;
            if (KeyLimiterManager.isChangingLimitedKeys)
            {
                KeyLimiterManager.UpdateKeyLimiter(keyCode);
                return true;
            }

            if (GCS.sceneToLoad == "scnCLS" && scnCLS_searchMode)
            {
                return true;
            }
            
            HashSet<KeyCode> ignoreScnCLS;
            if (_dictionary.TryGetValue(GCS.sceneToLoad, out ignoreScnCLS))
            {
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
