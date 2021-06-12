using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NoStopMod.AsyncInput.HitDisable
{
    class HitDisableManager
    {
        private Dictionary<String, bool[]> dictionary;

        public bool scnCLS_searchMode;

        public HitDisableManager()
        {
            dictionary = new Dictionary<string, bool[]>();

            bool[] disableScnNewIntro = Enumerable.Repeat(false, 1024).ToArray();
            dictionary["scnNewIntro"] = disableScnNewIntro;
            disableScnNewIntro[(int)KeyCode.BackQuote] = true;
            disableScnNewIntro[(int)KeyCode.Alpha0] = true;
            disableScnNewIntro[(int)KeyCode.Alpha1] = true;
            disableScnNewIntro[(int)KeyCode.Alpha2] = true;
            disableScnNewIntro[(int)KeyCode.Alpha3] = true;
            disableScnNewIntro[(int)KeyCode.Alpha4] = true;
            disableScnNewIntro[(int)KeyCode.Alpha5] = true;
            disableScnNewIntro[(int)KeyCode.Alpha6] = true;
            disableScnNewIntro[(int)KeyCode.Alpha7] = true;

            bool[] disableScnCLS = Enumerable.Repeat(false, 1024).ToArray();
            dictionary["scnCLS"] = disableScnCLS;
            disableScnCLS[(int)KeyCode.S] = true;
            disableScnCLS[(int)KeyCode.Delete] = true;
            disableScnCLS[(int)KeyCode.F] = true;
            disableScnCLS[(int)KeyCode.O] = true;
            disableScnCLS[(int)KeyCode.Alpha7] = true;
            disableScnCLS[(int)KeyCode.UpArrow] = true;
            disableScnCLS[(int)KeyCode.DownArrow] = true;
            
            scnCLS_searchMode = false;
        }

        public bool shouldBeDisabled(KeyCode keyCode)
        {
            if (keyCode == KeyCode.Escape) return true;
            if (GCS.sceneToLoad == null) GCS.sceneToLoad = "scnNewIntro";

            bool[] disableMasks = dictionary[GCS.sceneToLoad];
            if (disableMasks != null)
            {
                if (GCS.sceneToLoad == "scnCLS" && scnCLS_searchMode)
                {
                    return true;
                }
                return disableMasks[(int) keyCode];
            }
            return false;
        }


    }
}
