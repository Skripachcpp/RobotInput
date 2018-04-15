namespace WorkingTools.Extensions
{
    public static class ArrayExtension
    {
        public static bool Identity<TElement>(this TElement[] obj, TElement[] compare)
        {
            if (obj == compare)
                return true;

            //если оба массива не пусты и их размеры одинаковы
            if ((obj != null && compare != null) && (obj.Length == compare.Length))
            {
                int lenght = obj.Length;
                //перебор элементов масивов
                for (int index = 0; index < lenght; index++)
                    //если хоть один элемент массива 1 не равен элементу массива 2 имеющего тот же индекс
                    if (!Equals(obj[index], compare[index]))
                        //то считать массивы различными
                        return false;

                return true;
            }

            return false;
        }
    }
}
