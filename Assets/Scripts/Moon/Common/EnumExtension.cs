using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Moon.Common
{
    public static class EnumExtension
    {
        public static int GetLength<T>() where T : Enum
        {
            return Enum.GetNames(typeof(T)).Length;
        }

        public static List<T> GetList<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }

        public static T GetRandomValue<T>(int start, int lastMinus) where T : Enum
        {
            var list = GetList<T>();
            return list[Random.Range(start, list.Count - lastMinus)];
        }
    }
}