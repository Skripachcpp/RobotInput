using System.IO;

namespace WorkingTools.Extensions
{
    public static class MemoryStreamExtension
    {
        public static void Save(this MemoryStream ms, string path)
        {
            using (var fileStream = File.Create(path))
            {
                ms.WriteTo(fileStream);
                fileStream.Close();
            }
        }
    }
}
