using NoStopMod.InputFixer.HitIgnore.KeyLimiter;
using System;
using System.Collections.Generic;
using KeyCode = SharpHook.Native.NativeKeyCode;

namespace NoStopMod.InputFixer.HitIgnore
{
    public class HitIgnoreManager
    {
        private static readonly Dictionary<String, HashSet<KeyCode>> _dictionary = new Dictionary<string, HashSet<KeyCode>>();

        public static States scrController_state;

        public static void Init()
        {
            if (GCS.sceneToLoad == null) GCS.sceneToLoad = "scnNewIntro";

            HashSet<KeyCode> ignoreScnNewIntro = new HashSet<KeyCode>();
            _dictionary["scnNewIntro"] = ignoreScnNewIntro;

            ignoreScnNewIntro.Add(KeyCode.Backquote);
            ignoreScnNewIntro.Add(KeyCode.Alpha0);
            ignoreScnNewIntro.Add(KeyCode.Alpha1);
            ignoreScnNewIntro.Add(KeyCode.Alpha2);
            ignoreScnNewIntro.Add(KeyCode.Alpha3);
            ignoreScnNewIntro.Add(KeyCode.Alpha4);
            ignoreScnNewIntro.Add(KeyCode.Alpha5);
            ignoreScnNewIntro.Add(KeyCode.Alpha6);
            ignoreScnNewIntro.Add(KeyCode.Alpha7);

            HashSet<KeyCode> ignoreScnCLS_ = new HashSet<KeyCode>();
            _dictionary["scnCLS"] = ignoreScnCLS_;
            ignoreScnCLS_.Add(KeyCode.Up);
            ignoreScnCLS_.Add(KeyCode.NumPadUp);
            ignoreScnCLS_.Add(KeyCode.Down);
            ignoreScnCLS_.Add(KeyCode.NumPadDown);
            ignoreScnCLS_.Add(KeyCode.R);
            ignoreScnCLS_.Add(KeyCode.S);
            ignoreScnCLS_.Add(KeyCode.Delete);
            ignoreScnCLS_.Add(KeyCode.I);
            ignoreScnCLS_.Add(KeyCode.F);
            ignoreScnCLS_.Add(KeyCode.O);
            ignoreScnCLS_.Add(KeyCode.N);

            HashSet<KeyCode> ignoreScnTaroMenu0 = new HashSet<KeyCode>();
            _dictionary["scnTaroMenu0"] = ignoreScnTaroMenu0;
            ignoreScnTaroMenu0.Add(KeyCode.Alpha1);
            ignoreScnTaroMenu0.Add(KeyCode.Alpha2);
            ignoreScnTaroMenu0.Add(KeyCode.Alpha3);
            ignoreScnTaroMenu0.Add(KeyCode.Alpha4);
            ignoreScnTaroMenu0.Add(KeyCode.Alpha5);
            ignoreScnTaroMenu0.Add(KeyCode.Alpha6);
            
            HashSet<KeyCode> ignoreScnTaroMenu3 = new HashSet<KeyCode>();
            _dictionary["scnTaroMenu3"] = ignoreScnTaroMenu3;
            ignoreScnTaroMenu3.Add(KeyCode.Alpha0);
            ignoreScnTaroMenu3.Add(KeyCode.Alpha1);
            ignoreScnTaroMenu3.Add(KeyCode.Alpha2);
            ignoreScnTaroMenu3.Add(KeyCode.Alpha3);
            ignoreScnTaroMenu3.Add(KeyCode.Alpha4);
            ignoreScnTaroMenu3.Add(KeyCode.Alpha5);

            if (scrController.instance != null)
            {
                scrController_state = (States) scrController.instance.currentState;
            }

            KeyLimiterManager.Init();
        }

        public static bool ShouldBeIgnored(KeyCode keyCode)
        {
#if DEBUG
            //NoStopMod.mod.Logger.Log($"scene : {GCS.sceneToLoad}, keyCode: {keyCode}");
#endif
            
            if (keyCode == KeyCode.Escape) return true;
            if (KeyLimiterManager.isChangingLimitedKeys)
            {
                KeyLimiterManager.UpdateKeyLimiter(keyCode);
                return true;
            }
            
            if (scrController_state != States.PlayerControl && !ADOBase.isEditingLevel && ADOBase.uiController.difficultyUIMode != DifficultyUIMode.DontShow)
            {
                if (keyCode is KeyCode.NumPadLeft 
                            or KeyCode.Left)
                {
                    return true;
                }
                if (keyCode is KeyCode.NumPadRight 
                    or KeyCode.Right)
                {
                    return true;
                }
            }

            if (GCS.sceneToLoad == "scnCLS" && ADOBase.cls.optionsPanels.searchMode)
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
