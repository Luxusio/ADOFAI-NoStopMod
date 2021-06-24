using HarmonyLib;
using NoStopMod.EditorSongFixer;

namespace NoStopMod.InputFixer.SyncFixer
{
    class SyncFixerPatchesScnEditor
    {

        /**
         * Author: PatricKR
         * 
         */
        [HarmonyPatch(typeof(scnEditor), "SwitchToEditMode")]
        private static class scnEditor_SwitchToEditMode_Patch
        {
            private static void Prefix(scnEditor __instance)
            {
                __instance.conductor.song.Stop();
                AudioManager.Instance.StopAllSounds();
            }
        }
        
        /**
         * Author: PERIOT
         * 
         */
        [HarmonyPatch(typeof(scnEditor), "OpenLevelCo")]
        private static class scnEditor_OpenLevelCo_Patch
        {
            private static void Prefix(scnEditor __instance)
            {
                EditorSongFixerManager.scnEditorCurrentSongKey
                    .SetValue(CustomLevel.instance, null);
            }
        }

    }
}
