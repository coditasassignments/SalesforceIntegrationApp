using System;
using System.IO;

namespace SalesforceIntegrationApp.Logging
{
    public static class Logger
    {
        private static readonly string logFilePath = "Logs/log.txt";
        static Logger()
        {
            var directory = Path.GetDirectoryName(logFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);
        }
        public static void LogInfo(string message)
        {
            var logMessage = $"[INFO]  {DateTime.Now} - {message}";
            Console.WriteLine(logMessage);
            File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
        }
        public static void LogError(string message, Exception ex)
        {
            var logMessage = $"[ERROR] {DateTime.Now} - {message}\nException: {ex.Message}\nStackTrace: {ex.StackTrace}";
            Console.WriteLine(logMessage);
            File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
        }
    }
}

