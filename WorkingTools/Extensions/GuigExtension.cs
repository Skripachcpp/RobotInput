using System;

namespace WorkingTools.Extensions
{
    public static class GuigExtension
    {
        /// <summary>
        /// Привести к string, возвращает null если выполнить преобразование не удалось
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string AsString(this Guid? value)
        {
            return value.Get(v => v.ToString());
        }
    }
}
