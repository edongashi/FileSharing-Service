using System;
using System.IO;
using FileSharing.Serveri.Infrastruktura.Abstrakt;
using FileSharing.Serveri.Sherbimet.Abstrakt;

namespace FileSharing.Serveri.Infrastruktura
{
    public class StandardPaths : IPathResolver
    {
        private string rootFolder;
        private string dataFolder;
        private string tempFolder;

        public string GetRootFolder()
        {
            if (rootFolder != null)
            {
                return rootFolder;
            }

            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "FileSharing Serveri");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return rootFolder = path;
        }

        public string GetDataFolder()
        {
            if (dataFolder != null)
            {
                return dataFolder;
            }

            var path = Path.Combine(GetRootFolder(), "data");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return dataFolder = path;
        }

        public string GetTempFolder()
        {
            if (tempFolder != null)
            {
                return tempFolder;
            }

            var path = Path.Combine(GetRootFolder(), "temp");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return tempFolder = path;
        }

        public string GetFileInRootPath(string file)
        {
            return Path.Combine(GetRootFolder(), file);
        }

        public string GetFileInDataPath(string file)
        {
            return Path.Combine(GetDataFolder(), file);
        }

        public string GetTempFile()
        {
            var folder = GetTempFolder();
            string tempFile;
            do
            {
                tempFile = Path.Combine(folder, Path.GetRandomFileName());
            } while (File.Exists(tempFile));
            return tempFile;
        }
    }
}
