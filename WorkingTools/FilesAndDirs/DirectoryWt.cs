using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WorkingTools.FilesAndDirs
{
    //смысл класса быть более устойчивым к исключениям возникающим в System.IO.Directory
    public static class DirectoryWt
    {
        public static string[] GetFiles(params string[] dirPath)
        {
            var path = Path.Combine(dirPath);

            if (!Directory.Exists(path))
                return new string[] {};

            return Directory.GetFiles(path);
        }


        public static DirectoryInfo Create(params string[] dirPath)
        {
            var path = Path.Combine(dirPath);

            return Directory.CreateDirectory(path);
        }

        /// <summary>
        /// Проверяет каталог на наличие вложенных элементов
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fast">true - метод вернет true если каталог не содержит ни одного элемента; 
        /// false - метод вернет true если каталог не содержит ни одного файла</param>
        /// <returns>true если каталог пуст</returns>
        public static bool IsEmpty(string path, bool fast = true)
        {
            if (!Directory.Exists(path)) return true;

            if (fast)
            {
                if (Directory.EnumerateFileSystemEntries(path).Any())
                    return false;

                return true;
            }
            else
            {
                if (Directory.EnumerateFiles(path).Any()) return false;

                var existsNotEmptyFolder = Directory.EnumerateDirectories(path).Any(p => !IsEmpty(p));
                if (existsNotEmptyFolder) return false;

                return true;
            }
        }

        public static bool IsEmpty(params string[] dirPath)
        {
            var path = Path.Combine(dirPath);
            return IsEmpty(path);
        }

        /// <summary>
        /// Удалить директорию
        /// </summary>
        /// <param name="path"></param>
        /// <param name="recursive"></param>
        /// <returns>true если удалось удалить директорию</returns>
        public static bool Delete(string path, bool recursive)
        {
            if (!recursive)
            {
                if (Directory.Exists(path) && IsEmpty(path))
                {
                    Directory.Delete(path);
                    return true;
                }

                return false;
            }
            else
            {
                Directory.Delete(path, recursive);
                return true;
            }
        }

        public static void Delete(params string[] dirPath)
        {
            var path = Path.Combine(dirPath);
            Delete(path, false);
        }

        public static IEnumerable<String> EnumerateDirectories(params string[] dirPath)
        {
            var path = Path.Combine(dirPath);

            if (!Directory.Exists(path))
                return Enumerable.Empty<String>();

            return Directory.EnumerateDirectories(path);
        }

        public static void Clear(params string[] dirPath)
        {
            var path = Path.Combine(dirPath);
            if (Directory.Exists(path))
            {
                foreach (var directory in Directory.GetDirectories(path))
                    Directory.Delete(directory, true);

                foreach (var file in Directory.GetFiles(path))
                    File.Delete(file);
            }
        }
    }
}