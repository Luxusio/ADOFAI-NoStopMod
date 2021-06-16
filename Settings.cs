using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoStopMod.Abstraction;
using SimpleJSON;
using UnityEngine;

namespace NoStopMod
{
    class Settings
    {

        public const String path = "\\Mods\\NoStopMod\\settings.json";

        public static List<SettingsBase> settings;

        public static void Load()
        {
            TextAsset json = Resources.Load<TextAsset>(Environment.CurrentDirectory + path);
            if (json == null)
            {
                Save();
                return;
            }

            JSONNode jsonNode = JSON.Parse(json.text);
            for (int i = 0; i < settings.Count(); i++)
            {
                try
                {
                    settings[i].Load(jsonNode);
                }
                catch (Exception e)
                {
                    NoStopMod.mod.Logger.Error("While Loading : " + e.Message + ", " + e.StackTrace);
                }
            }
        }

        public static void Save()
        {
            JSONNode node = JSON.Parse("{}");
            for (int i = 0;i < settings.Count();i++)
            {
                try
                {
                    settings[i].Save(node);
                }
                catch (Exception e)
                {
                    NoStopMod.mod.Logger.Error("While Saving : " + e.Message + ", " + e.StackTrace);
                }
            }
            
            File.WriteAllText(Environment.CurrentDirectory + path, node.ToString());
        }

    }
}
