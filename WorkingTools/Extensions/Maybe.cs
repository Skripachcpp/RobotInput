using System;

namespace WorkingTools.Extensions
{
    /// <summary>
    /// Выполнить при условии
    /// </summary>
    public static class Maybe
    {
        /// <summary>
        /// Выполнить если объект != null
        /// </summary>
        /// <typeparam name="TObj">тип объекта</typeparam>
        /// <typeparam name="TRes">тип результата</typeparam>
        /// <param name="obj">объект</param>
        /// <param name="func">функция, результат которой возвращает метод</param>
        /// <param name="defaultRes">возвращаемое значение если объект == null</param>
        /// <returns>возвращает результат работы ф-ии, если переданный объект != null</returns>
        public static TRes Get<TObj, TRes>(this TObj obj, Func<TObj, TRes> func, TRes defaultRes)
        {
            if (Equals(obj, null) || func == null)
                return defaultRes;

            return func(obj);
        }

        /// <summary>
        /// Выполнить если объект != null
        /// </summary>
        /// <typeparam name="TObj">тип объекта</typeparam>
        /// <typeparam name="TRes">тип результата</typeparam>
        /// <param name="obj">объект</param>
        /// <param name="func">функция, результат которой возвращает метод</param>
        /// <param name="continuation">продолжение, выполняется над переданным оъектом после получения значения</param>
        /// <returns>возвращает результат работы ф-ии, если переданный объект != null</returns>
        public static TRes Get<TObj, TRes>(this TObj obj, Func<TObj, TRes> func, Action<TObj> continuation = null)
        {
            if (Equals(obj, null) || func == null)
                return default(TRes);

            var result = func(obj);
            if (!Equals(continuation, null))
                continuation(obj);

            return result;
        }

        /// <summary>
        /// Выполнить действие, если объект != null
        /// </summary>
        /// <typeparam name="TObj">тип объекта</typeparam>
        /// <param name="obj">объект</param>
        /// <param name="func">выполняемая функция</param>
        /// <returns>возвращает переданный объект</returns>
        public static TObj Do<TObj>(this TObj obj, Action<TObj> func)
        {
            if (Equals(obj, null) || func == null)
                return obj;

            func(obj);

            return obj;
        }

        public static TRes If<TObj, TRes>(this TObj obj, Func<TObj, bool> when, Func<TObj, TRes> then, Func<TObj, TRes> els = null)
        {
            if (when != null)
            {
                if (when(obj))
                {
                    if (then != null)
                        return then(obj);

                    return default(TRes);
                }
                else
                {
                    if (els != null)
                        return els(obj);

                    return default(TRes);
                }
            }
            else
            {
                throw new ArgumentNullException("when", "условие не можут быть пустым");
            }

            return default(TRes);
        }
    }
}
