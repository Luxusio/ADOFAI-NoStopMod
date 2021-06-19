using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoStopMod.Helper
{
    class EventListener<T>
    {

        public readonly List<Action<T>> listeners = new List<Action<T>>();

        private readonly string errorMessage;

        public EventListener(string errorMessage)
        {
            this.errorMessage = errorMessage;
        }

        public void Add(Action<T> listener)
        {
            listeners.Add(listener);
        }

        public void Invoke(T value)
        {
            for (int i = 0; i < listeners.Count; i++)
            {
                try
                {
                    listeners[i].Invoke(value);
                }
                catch (Exception e)
                {
                    NoStopMod.mod.Logger.Error("Error on " + errorMessage + " : " + e.Message + ", " + e.StackTrace);
                }
            }
        }

    }
}
