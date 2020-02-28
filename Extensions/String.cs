using System.IO;

namespace NKMCore.Extensions
{
    public static class String
    {
        public static bool IsValidFilename(this string filename) => !string.IsNullOrEmpty(filename) && filename.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;
    }
}