using NoStopMod.Helper;
using System.Reflection;

namespace NoStopMod.EditorSongFixer
{
    class EditorSongFixerManager
    {

        public static ReflectionField<string> scnEditorCurrentSongKey = new ReflectionField<string>("currentSongKey");

    }
}
