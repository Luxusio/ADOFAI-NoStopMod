using NoStopMod.Helper;
using NoStopMod.Helper.Abstraction;
using SimpleJSON;
using System;

namespace NoStopMod.InputFixer
{
    class InputFixerSettings : SettingsBase
    {
        public bool enableAsync = false;
        public bool enableKeyLimit = false;

        public void Load(ref JSONNode json)
        {
            JSONNode node = json["InputFixer"];

            enableAsync = node["enableAsync"].AsBool;
            enableKeyLimit = node["enableKeyLimit"].AsBool;
            JSONArray array = node["LimitKeys"].AsArray;
            
        }

        public void Save(ref JSONNode json)
        {
            JSONNode node = JSONHelper.CreateEmptyNode();
            node["enableAsync"].AsBool = enableAsync;
            node["enableKeyLimit"].AsBool = enableKeyLimit;

            json["InputFixer"] = node;
        }
    }
}
