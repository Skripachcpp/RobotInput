using System.IO;

namespace WorkingTools.FilesAndDirs
{
    public static class FileWt
    {
        public static void Delete(params string[] fileOrDir)
        {
            File.Delete(Path.Combine(fileOrDir));
        }

        public static Stream Create(params string[] filePath)
        {
            var fullPath = Path.Combine(filePath);

            var dirPath = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(dirPath) && !Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            return File.Create(fullPath);
        }

        public static Stream OpenRead(params string[] filePath)
        {
            string fileP = Path.Combine(filePath);

            if (!File.Exists(fileP))
                return Stream.Null;

            return File.OpenRead(fileP);
        }

        public static void Move(string sourceFileName, string destFileName)
        {
            var destDir = Path.GetDirectoryName(destFileName);
            if (destDir != null && !Directory.Exists(destDir))
                Directory.CreateDirectory(destDir);

            File.Move(sourceFileName, destFileName);
        }
    }
}