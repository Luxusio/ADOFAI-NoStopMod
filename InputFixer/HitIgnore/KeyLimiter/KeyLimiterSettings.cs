using NoStopMod.Helper;
using NoStopMod.Helper.Abstraction;
using NoStopMod.Helper.RawInputManager;
using SimpleJSON;
using System.Collections.Generic;
using UnityEngine;

namespace NoStopMod.InputFixer.HitIgnore.KeyLimiter
{
    class KeyLimiterSettings : SettingsBase
    {

        public bool enable = false;
        public List<RawKeyCode> limitKeys = new List<RawKeyCode>();
        
        public void Load(ref JSONNode json)
        {
            JSONNode node = json["KeyLimiter"];

            enable = node["enable"].AsBool;
            limitKeys = JSONHelper.ReadArray(ref node, "limitKeys", (arrayNode) => { return (RawKeyCode) arrayNode.AsInt; });

        }

        public void Save(ref JSONNode json)
        {
            JSONNode node = JSONHelper.CreateEmptyNode();
            node["enable"].AsBool = enable;
            node["limitKeys"] = JSONHelper.WriteArray(limitKeys, (element) => { return (int) element; });

            json["KeyLimiter"] = node;
        }
    }
}
