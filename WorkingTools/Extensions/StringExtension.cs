using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace WorkingTools.Extensions
{
    public static class StringExtension
    {
        //public static bool? AsBool(this string value)
        //{
        //    bool result;
        //    if (bool.TryParse(value, out result))
        //        return result;
        //
        //    return null;
        //}

        /// <summary>
        /// Привести строку к Guid?, возвращает null если выполнить преобразование не удалось
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Guid? AsGuid(this string value)
        {
            if (value == null)
                return null;

            try
            {
                return new Guid(value);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Привести строку к int?, возвращает null если выполнить преобразование не удалось
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToInt(this string value)
        {
            if (value == null) throw new ArgumentNullException("value");

            int res;
            if (!Int32.TryParse(value, out res))
                throw new ArgumentOutOfRangeException("value", "не удалось привести строку к числу");

            return res;
        }

        /// <summary>
        /// Привести строку к int?, возвращает null если выполнить преобразование не удалось
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int? AsInt(this string value)
        {
            if (value == null)
                return null;

            int res;
            if (!Int32.TryParse(value, out res))
                return null;

            return res;
        }

        /// <summary>
        /// Привести строку к int, возвращает defaultValue если выполнить преобразование не удалось
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int AsInt(this string value, int defaultValue)
        {
            if (value == null)
                return defaultValue;

            int res;
            if (!Int32.TryParse(value, out res))
                return defaultValue;

            return res;
        }

        /// <summary>
        /// Привести строку к decimal?, возвращает null если выполнить преобразование не удалось
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal? AsDecimal(this string value)
        {
            if (value == null)
                return null;

            try
            {
                return Decimal.Parse(value, NumberFormatInfo.InvariantInfo);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Привести строку к DateTime?, возвращает null если выполнить преобразование не удалось
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime? AsDateTime(this string value)
        {
            if (String.IsNullOrEmpty(value))
                return null;

            try
            {
                return Convert.ToDateTime(value);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Привести строку к XDocument, возвращает null если выполнить преобразование не удалось
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static XDocument AsXDocument(this string value)
        {
            if (String.IsNullOrEmpty(value))
                return null;

            try
            {
                return XDocument.Parse(value);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Привести строку к XElement, возвращает null если выполнить преобразование не удалось
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static XElement AsXElement(this string value)
        {
            if (String.IsNullOrEmpty(value))
                return null;

            try
            {
                return XElement.Parse(value);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Кодирует строку как массив байтов, возврящает null выполнить преобразование не удалось
        /// </summary>
        /// <param name="value"></param>
        /// <param name="encoding">по умолчанию UTF8</param>
        /// <returns></returns>
        public static byte[] AsBytes(this string value, Encoding encoding = null)
        {
            if (value == null)
                return null;

            try
            {
                return (encoding ?? Encoding.UTF8).GetBytes(value);
            }
            catch (Exception) { return null; }
        }

        #region Comparer
        private class EqualityStringComparer : IEqualityComparer<string>
        {
            private readonly bool _ignoreCase;

            public EqualityStringComparer(bool ignoreCase)
            {
                _ignoreCase = ignoreCase;
            }

            public bool Equals(string x, string y)
            {
                return string.Compare(x, y, _ignoreCase) == 0;
            }

            public int GetHashCode(string s)
            {
                return s.ToLower().GetHashCode();
            }
        }
        #endregion

        public static IEnumerable<string> Distinct(this IEnumerable<string> enumerable, bool ignoreCase)
        {
            return enumerable.Distinct(new EqualityStringComparer(ignoreCase));
        }
    }
}
