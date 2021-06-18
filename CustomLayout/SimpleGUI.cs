using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NoStopMod.CustomLayout
{
    class SimpleGUI
    {

        public static void Toggle(ref bool previous, string message, Action<bool> callback)
        {
            bool current = GUILayout.Toggle(previous, message);
            if (current != previous)
            {
                previous = current;
                callback.Invoke(current);
            }
        }
        

    }
}
