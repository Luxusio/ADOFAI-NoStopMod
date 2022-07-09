using System;
using System.Linq;
using System.Reflection;

namespace NoStopMod.Helper
{
    public class ReflectionField<T, TV>
    {
        private FieldInfo _fieldInfo;

        private readonly string[] _fieldNames;

        public ReflectionField(params string[] fieldNames)
        {
            this._fieldNames = fieldNames;
        }

        public FieldInfo GetFieldInfo(Type type)
        {
            if (_fieldInfo == null)
            {
                for (int i = 0; i < _fieldNames.Count(); i++)
                {
                    _fieldInfo = type.GetField(_fieldNames[i], 
                        BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | 
                        BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty);
                    if (_fieldInfo != null) break;
                }
                if (_fieldInfo == null)
                {
                    NoStopMod.mod.Logger.Error("Cannot find fieldInfo : (type=" + type + ", name" + _fieldNames + ")");
                }
            }
            return _fieldInfo;
        }

        public void SetValue(T obj, TV value)
        {
            GetFieldInfo(obj.GetType())?.SetValue(obj, value);
        }
        
        public TV GetValue(T obj)
        {
            return (TV) GetFieldInfo(obj.GetType())?.GetValue(obj);
        }

    }
}
