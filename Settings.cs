using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NoStopMod.Helper;
using NoStopMod.Helper.Abstraction;
using SimpleJSON;

namespace NoStopMod
{
    public class Settings
    {

        public const String path = "\\Mods\\NoStopMod\\settings.json";

        public static List<SettingsBase> settings = new List<SettingsBase>();
        public static EventListener<bool> settingsLoadListener = new EventListener<bool>("SettingsLoad");

        public static void Init()
        {
            NoStopMod.onToggleListener.Add(_ => Load());
            NoStopMod.onApplicationQuitListener.Add(_ => Save());
        }

        public static void Load()
        {
            try
            {
                string text = File.ReadAllText(Environment.CurrentDirectory + path);

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
                settingsLoadListener.Invoke(true);
            }
            catch
            {
                Save();
                settingsLoadListener.Invoke(false);
            }
        }

        public static void Save()
        {
            JSONNode node = JSONHelper.CreateEmptyNode();
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
