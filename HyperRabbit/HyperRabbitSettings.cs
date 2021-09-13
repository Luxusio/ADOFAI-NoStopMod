using NoStopMod.Helper;
using NoStopMod.Helper.Abstraction;
using SimpleJSON;

namespace NoStopMod.HyperRabbit
{
    class HyperRabbitSettings : SettingsBase
    {
        public int maxTilePerFrame = 0;

        public void Load(ref JSONNode json)
        {
            JSONNode node = json["HyperRabbit"];

            maxTilePerFrame = node["maxTilePerFrame"].AsInt;
        }

        public void Save(ref JSONNode json)
        {
            JSONNode node = JSONHelper.CreateEmptyNode();
            node["maxTilePerFrame"].AsInt = maxTilePerFrame;

            json["HyperRabbit"] = node;
        }
    }
}
