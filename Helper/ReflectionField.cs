using System;
using System.Linq;
using System.Reflection;

namespace NoStopMod.Helper
{
    class ReflectionField
    {
        private static FieldInfo fi;

        private string[] fieldNames;

        public ReflectionField(params string[] fieldNames)
        {
            this.fieldNames = fieldNames;
        }

        public FieldInfo getFieldInfo(Type type)
        {
            if (fi == null)
            {
                for (int i=0;i < fieldNames.Count();i++)
                {
                    fi = type.GetField(fieldNames[i], BindingFlags.NonPublic | BindingFlags.Instance);
                    if (fi != null) break;
                }
            }
            return fi;
        }

    }
}
