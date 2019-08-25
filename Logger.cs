using System.IO;

namespace NKMCore
{
    public class Logger
    {
        private readonly string _logFilePath;
        public Logger(string logFilePath)
        {
            _logFilePath = logFilePath;
        }

        public void Log(string msg)
        {
            if (_logFilePath == null) return;

            //Make sure target directory exists
            string directoryName = Path.GetDirectoryName(_logFilePath);
            if(directoryName != null) Directory.CreateDirectory(directoryName);

            File.AppendAllText(_logFilePath, $"{msg}\n");

        }


    }
}