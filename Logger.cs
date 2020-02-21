using System.IO;

namespace NKMCore
{
    public class Logger : ILogger
    {
        private readonly string _logFilePath;
        public Logger(string logFilePath)
        {
            _logFilePath = logFilePath;

            //Make sure target directory exists
            string directoryName = Path.GetDirectoryName(_logFilePath);
            if(directoryName != null) Directory.CreateDirectory(directoryName);

            //TODO: handle case when two loggers get instantiated at the same second
        }

        public void Log(string msg)
        {
            if (_logFilePath == null) return;

            File.AppendAllText(_logFilePath, $"{msg}\n");
        }
    }
}