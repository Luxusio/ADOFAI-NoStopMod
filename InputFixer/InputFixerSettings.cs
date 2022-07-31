using NoStopMod.Helper;
using NoStopMod.Helper.Abstraction;
using SimpleJSON;

namespace NoStopMod.InputFixer
{
    public class InputFixerSettings : SettingsBase
    {
        public bool insertKeyOnWindowFocus = true;
        //public bool enableAsync = false;
        //public bool enableKeyLimit = false;

        public void Load(ref JSONNode json)
        {
            JSONNode node = json["InputFixer"];

            insertKeyOnWindowFocus = node["insertKeyOnWindowFocus"].AsBool;
        }

        public void Save(ref JSONNode json)
        {
            JSONNode node = JSONHelper.CreateEmptyNode();
            node["insertKeyOnWindowFocus"].AsBool = insertKeyOnWindowFocus;

            json["InputFixer"] = node;
        }
    }
}
