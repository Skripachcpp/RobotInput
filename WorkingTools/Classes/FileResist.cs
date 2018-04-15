using System;
using System.IO;
using System.Threading;

namespace WorkingTools
{
    public static class FileResist
    {
        private const int RetryTimeout = 250;

        public static FileStream OpenRead(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
            }

            if (!File.Exists(path))
            {
            }

            FileStream fileStream = null;
            while (fileStream == null)
            {
                try
                {
                    fileStream = File.OpenRead(path);
                }
                catch (Exception)
                {
                    Thread.Sleep(RetryTimeout);
                }
            }

            return fileStream;
        }
    }
}
