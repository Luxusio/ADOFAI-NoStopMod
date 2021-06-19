using NoStopMod.Helper;
using NoStopMod.Helper.Abstraction;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NoStopMod.InputFixer.HitIgnore.KeyLimiter
{
    class KeyLimiterSettings : SettingsBase
    {

        public bool enable;
        public List<KeyCode> limitKeys;
        
        public void Load(ref JSONNode json)
        {
            JSONNode node = json["KeyLimiter"];

            enable = node["enable"].AsBool;
            limitKeys = JSONHelper.ReadArray(ref node, "limitKeys", (arrayNode) => { return (KeyCode) arrayNode.AsInt; });
            
            throw new NotImplementedException();
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
