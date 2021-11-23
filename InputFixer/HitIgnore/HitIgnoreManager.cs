using NoStopMod.Helper.RawInputManager;
using NoStopMod.InputFixer.HitIgnore.KeyLimiter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NoStopMod.InputFixer.HitIgnore
{
    class HitIgnoreManager
    {
        private static Dictionary<String, bool[]> dictionary;

        public static bool scnCLS_searchMode;
        public static scrController.States scrController_state;
        
        public static void Init()
        {
            if (GCS.sceneToLoad == null) GCS.sceneToLoad = "scnNewIntro";

            dictionary = new Dictionary<string, bool[]>();

            bool[] ignoreScnNewIntro = Enumerable.Repeat(false, 1024).ToArray();
            dictionary["scnNewIntro"] = ignoreScnNewIntro;
           
            ignoreScnNewIntro[(int)RawKeyCode.OEM_7] = true;
            ignoreScnNewIntro[(int)RawKeyCode.KEY_0] = true;
            ignoreScnNewIntro[(int)RawKeyCode.KEY_1] = true;
            ignoreScnNewIntro[(int)RawKeyCode.KEY_2] = true;
            ignoreScnNewIntro[(int)RawKeyCode.KEY_3] = true;
            ignoreScnNewIntro[(int)RawKeyCode.KEY_4] = true;
            ignoreScnNewIntro[(int)RawKeyCode.KEY_5] = true;
            ignoreScnNewIntro[(int)RawKeyCode.KEY_6] = true;
            ignoreScnNewIntro[(int)RawKeyCode.KEY_7] = true;

            bool[] ignoreScnCLS = Enumerable.Repeat(false, 1024).ToArray();
            dictionary["scnCLS"] = ignoreScnCLS;
            ignoreScnCLS[(int)RawKeyCode.KEY_S] = true;
            ignoreScnCLS[(int)RawKeyCode.DELETE] = true;
            ignoreScnCLS[(int)RawKeyCode.KEY_F] = true;
            ignoreScnCLS[(int)RawKeyCode.KEY_O] = true;
            ignoreScnCLS[(int)RawKeyCode.KEY_7] = true;
            ignoreScnCLS[(int)RawKeyCode.UP] = true;
            ignoreScnCLS[(int)RawKeyCode.DOWN] = true;
            
            scnCLS_searchMode = false;

            if (scrController.instance != null)
            {
                scrController_state = (scrController.States) scrController.instance.GetState();
            }

            KeyLimiterManager.Init();
        }

        public static bool ShouldBeIgnored(RawKeyCode keyCode)
        {
            if (keyCode == RawKeyCode.ESC) return true;
            if (KeyLimiterManager.isChangingLimitedKeys)
            {
                KeyLimiterManager.UpdateKeyLimiter(keyCode);
                return true;
            }

            if (scrController_state != scrController.States.PlayerControl)
            {
                return true;
            }

            bool[] ignoreScnCLS;
            if (dictionary.TryGetValue(GCS.sceneToLoad, out ignoreScnCLS))
            {
                if (GCS.sceneToLoad == "scnCLS" && scnCLS_searchMode)
                {
                    return true;
                }
                if (ignoreScnCLS[(int)keyCode])
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
