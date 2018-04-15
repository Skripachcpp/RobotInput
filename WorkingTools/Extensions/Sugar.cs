using System;
using System.Collections.Generic;
using System.Text;

namespace WorkingTools.Extensions
{
    public static class Sugar
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> func, Action<T, Exception> _catch = null)
        {
            if (func == null || enumerable == null) return;
            foreach (T item in enumerable)
                try
                {
                    func(item);
                }
                catch (Exception ex)
                {
                    if (_catch == null) throw;
                    _catch(item, ex);
                }
        }

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> func)
        {
            if (func == null || enumerable == null) return;
            foreach (T item in enumerable)
                func(item);
        }

        public static T[] Union<T>(this T[] array, params T[] items)
        {
            if (items == null || items.Length <= 0) return array;

            var newArray = array;
            Array.Resize(ref newArray, array.Length + items.Length);

            int insertPosition = array.Length;
            int insertionElement = 0;
            while (insertionElement < newArray.Length)
            {
                newArray[insertPosition] = items[insertionElement];

                insertPosition++;
                insertionElement++;
            }

            return newArray;
        }

        /// <summary>
        /// Возвращает строку в указаном формате или null, если выполнить преобразование не удалось
        /// </summary>
        /// <param name="value"></param>
        /// <param name="encoding">по умолчанию UTF8</param>
        /// <returns></returns>
        public static string AsString(this byte[] value, Encoding encoding)
        {
            if (value == null)
                return null;

            try
            {
                return (encoding ?? Encoding.UTF8).GetString(value);
            }
            catch (Exception) { return null; }
        }


        public static string AsString(this object obj)
        {
            if (obj is byte[]) return ((byte[])obj).AsString(null);

            return obj == null ? null : obj.ToString();
        }

        public static TObj ThrowIfNull<TObj>(this TObj obj, string paramName, string errorMessage) where TObj : class
        {
            if (obj == null) throw new ArgumentNullException(paramName, errorMessage);
            return obj;
        }

        public static TObj ThrowIfNull<TObj>(this TObj obj, string errorMessage) where TObj : class
        {
            if (obj == null) throw new NullReferenceException(errorMessage);
            return obj;
        }

        public static TRes As<TRes>(this object obj) where TRes : class
        { return obj as TRes; }
    }
}