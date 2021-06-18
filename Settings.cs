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

        public static List<SettingsBase> settings = new List<SettingsBase>();

        public static void Init()
        {
            NoStopMod.onToggleListeners.Add(onToggle);
            NoStopMod.onApplicationQuitListeners.Add(onApplicationQuit);
        }

        private static void onToggle(bool enabled)
        {
            Load();
        }

        private static void onApplicationQuit(scrController __instance)
        {
            Save();
        }

        public static void Load()
        {
            string text = File.ReadAllText(Environment.CurrentDirectory + path);
            if (text == null)
            {
                Save();
                return;
            }

            JSONNode jsonNode = JSON.Parse(text);
            for (int i = 0; i < settings.Count(); i++)
            {
                try
                {
                    settings[i].Load(ref jsonNode);
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
            for (int i = 0; i < settings.Count();i++)
            {
                try
                {
                    settings[i].Save(ref node);
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
