using System;
using System.Linq;
using System.Reflection;

namespace NoStopMod.Helper
{
    public class ReflectionField<T>
    {
        private static FieldInfo fi;

        private readonly string[] fieldNames;

        public ReflectionField(params string[] fieldNames)
        {
            this.fieldNames = fieldNames;
        }

        public FieldInfo GetFieldInfo(Type type)
        {
            if (fi == null)
            {
                for (int i=0;i < fieldNames.Count();i++)
                {
                    fi = type.GetField(fieldNames[i], 
                        BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | 
                        BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty);
                    if (fi != null) break;
                }
                if (fi == null)
                {
                    NoStopMod.mod.Logger.Error("Cannot find fieldInfo : (type=" + type + ", name" + fieldNames + ")");
                }
            }
            return fi;
        }

        public void SetValue(object obj, T value)
        {
            GetFieldInfo(obj.GetType())?.SetValue(obj, value);
        }
        
        public T GetValue(object obj)
        {
            return (T) GetFieldInfo(obj.GetType())?.GetValue(obj);
        }

    }
}
