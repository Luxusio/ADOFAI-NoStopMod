using NoStopMod.Abstraction;
using SimpleJSON;
using System;

namespace NoStopMod.AsyncInput
{
    class AsyncInputSettings : SettingsBase
    {
        public bool enableAsync;

        public void Load(ref JSONNode json)
        {
            JSONNode node = json["AsyncInput"];

            enableAsync = node["enableAsync"];
            throw new NotImplementedException();
        }

        public void Save(ref JSONNode json)
        {
            JSONNode node = JSON.Parse("{}");
            node["enableAsync"].AsBool = enableAsync;

            json["AsyncInput"] = node;
        }
    }
}
