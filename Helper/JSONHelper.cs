using SimpleJSON;
using System;
using System.Collections.Generic;

namespace NoStopMod.Helper
{
    public class JSONHelper
    {

        public static JSONNode CreateEmptyNode()
        {
            return JSON.Parse("{}");
        }

        public static long ConvertLong(JSONNode node)
        {
            return node.AsLong;
        }

        public static int ConvertInt(JSONNode node)
        {
            return node.AsInt;
        }

        public static List<T> ReadArray<T>(ref JSONNode node, string name, Func<JSONNode, T> converter)
        {
            JSONArray array = node[name].AsArray;

            List<T> results = new List<T>(array.Count);
            for (int i = 0; i < array.Count; i++)
            {
                results.Add(converter.Invoke(array[i]));
            }
            return results;
        }
        
        public static JSONArray WriteArray<T>(List<T> list, Func<T, JSONNode> converter)
        {
            JSONArray array = new JSONArray();
            for (int i = 0; i < list.Count; i++)
            {
                array.Add(converter.Invoke(list[i]));
            }
            return array;
        }

    }
}
