using NoStopMod.Abstraction;
using SimpleJSON;
using System;

namespace NoStopMod.AsyncInput
{
    class AsyncInputSettings : SettingsBase
    {
        public bool enableAsync = false;

        public void Load(ref JSONNode json)
        {
            JSONNode node = json["AsyncInput"];

            enableAsync = node["enableAsync"].AsBool;
        }

        public void Save(ref JSONNode json)
        {
            JSONNode node = JSON.Parse("{}");
            node["enableAsync"].AsBool = enableAsync;

            json["AsyncInput"] = node;
        }
    }
}
