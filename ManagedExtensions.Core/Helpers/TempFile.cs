using System;
using System.IO;

namespace ManagedExtensions.Core.Helpers
{
    public static class TempFile
    {
        public static string NewPath(string extension)
        {
            return Path.Combine(Path.GetTempPath(), string.Format("{0}.{1}", Guid.NewGuid(), extension));
        }

        public static string Create(string content, string extension)
        {
            var filePath = NewPath(extension);
            File.WriteAllText(filePath, content);
            return filePath;
        }
    }
}
