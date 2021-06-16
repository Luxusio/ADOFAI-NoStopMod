using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NoStopMod.AsyncInput.HitIgnore
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
            ignoreScnNewIntro[(int)KeyCode.BackQuote] = true;
            ignoreScnNewIntro[(int)KeyCode.Alpha0] = true;
            ignoreScnNewIntro[(int)KeyCode.Alpha1] = true;
            ignoreScnNewIntro[(int)KeyCode.Alpha2] = true;
            ignoreScnNewIntro[(int)KeyCode.Alpha3] = true;
            ignoreScnNewIntro[(int)KeyCode.Alpha4] = true;
            ignoreScnNewIntro[(int)KeyCode.Alpha5] = true;
            ignoreScnNewIntro[(int)KeyCode.Alpha6] = true;
            ignoreScnNewIntro[(int)KeyCode.Alpha7] = true;

            bool[] ignoreScnCLS = Enumerable.Repeat(false, 1024).ToArray();
            dictionary["scnCLS"] = ignoreScnCLS;
            ignoreScnCLS[(int)KeyCode.S] = true;
            ignoreScnCLS[(int)KeyCode.Delete] = true;
            ignoreScnCLS[(int)KeyCode.F] = true;
            ignoreScnCLS[(int)KeyCode.O] = true;
            ignoreScnCLS[(int)KeyCode.Alpha7] = true;
            ignoreScnCLS[(int)KeyCode.UpArrow] = true;
            ignoreScnCLS[(int)KeyCode.DownArrow] = true;
            
            scnCLS_searchMode = false;
        }

        public static bool shouldBeIgnored(KeyCode keyCode)
        {
            if (keyCode == KeyCode.Escape) return true;
            if (scrController_state != scrController.States.PlayerControl) return true;

            bool[] ignoreScnCLS;
            if (dictionary.TryGetValue(GCS.sceneToLoad, out ignoreScnCLS))
            {
                if (GCS.sceneToLoad == "scnCLS" && scnCLS_searchMode)
                {
                    return true;
                }
                return ignoreScnCLS[(int) keyCode];
            }
            return false;
        }


    }
}
