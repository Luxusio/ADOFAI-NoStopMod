using System.Reflection;

namespace NoStopMod.EditorSongFixer
{
    class EditorSongFixerManager
    {

        private static FieldInfo scnEditorCurrentSongKeyInfo_;

        public static FieldInfo scnEditorCurrentSongKeyInfo
        {
            get
            {
                if (scnEditorCurrentSongKeyInfo_ == null)
                {
                    scnEditorCurrentSongKeyInfo_ = CustomLevel.instance?.GetType()
                            .GetField("currentSongKey", BindingFlags.NonPublic | BindingFlags.Instance);
                }
                return scnEditorCurrentSongKeyInfo_;
            }
        }

    }
}
