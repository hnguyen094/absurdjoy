// https://www.notion.so/File-Utilities-b13e98389ed9469da90960e660c73ca0
using System.IO;

namespace absurdjoy {
    public static class FileUtils {
        /// <summary>
        /// Separates the filename from the given path and creates the path.
        /// </summary>
        public static void CreateDirectoryFromFilePathIfNeeded(string filePath) {
            FileInfo fi = new FileInfo(filePath);
            CreateDirectoryIfNeeded(fi.DirectoryName);
        }

        /// <summary>
        /// Creates a folder if required. Note: Does not work if the path contains a filename; for that, use CreateDirectoryFromFilePathIfNeeded.
        /// </summary>
        public static void CreateDirectoryIfNeeded(string path) {
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }
        }
    }
}