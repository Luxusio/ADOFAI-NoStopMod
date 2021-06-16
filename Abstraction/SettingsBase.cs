using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleJSON;

namespace NoStopMod.Abstraction
{
    interface SettingsBase
    {

        void Load(JSONNode json);

        void Save(JSONNode json);
        
    }
}
