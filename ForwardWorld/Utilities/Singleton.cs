using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Crystal.WorldServer.Utilities
{
    public class Singleton<T>
    {
        private static Dictionary<Type, T> m_instances = new Dictionary<Type, T>();

        public static T GetInstance()
        {
            if (!HaveInstance(typeof(T)))
            {
                m_instances[typeof(T)] = (T)Activator.CreateInstance(typeof(T));
            }
            return m_instances[typeof(T)];
        }

        public static bool HaveInstance(Type type)
        {
            return m_instances.ContainsKey(type);
        }
    }
}
