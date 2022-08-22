using NoStopMod.Helper;
using NoStopMod.Helper.Abstraction;
using SimpleJSON;
using System.Collections.Generic;
using KeyCode = SharpHook.Native.NativeKeyCode;

namespace NoStopMod.InputFixer.HitIgnore.KeyLimiter
{
    class KeyLimiterSettings : SettingsBase
    {

        public bool enable = false;
        public List<KeyCode> limitKeys = new List<KeyCode>();
        
        public void Load(ref JSONNode json)
        {
            JSONNode node = json["KeyLimiter"];

            enable = node["enable"].AsBool;
            limitKeys = JSONHelper.ReadArray(ref node, "limitKeys", (arrayNode) => { return (KeyCode) arrayNode.AsULong; });

        }

        public void Save(ref JSONNode json)
        {
            JSONNode node = JSONHelper.CreateEmptyNode();
            node["enable"].AsBool = enable;
            node["limitKeys"] = JSONHelper.WriteArray(limitKeys, (element) => { return (int) (ushort) element; });

            json["KeyLimiter"] = node;
        }
    }
}
