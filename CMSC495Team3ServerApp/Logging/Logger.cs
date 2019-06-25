using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CMSC495Team3ServerApp.UtilityClasses;

namespace CMSC495Team3ServerApp.Logging
{
    public class Logger : ILogger
    {
        private readonly CancellationTokenSource cancellationTokenSource;

        private static readonly string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");

        private static readonly string filePath = Path.Combine(logDirectory,
            $"ServerApp_{DateTime.UtcNow:yyyy-MM-dd}.log");

        private readonly FileStream fileStream;


        private readonly QueueProcessor<string> queueProcessor;

        public Logger()
        {
            cancellationTokenSource = new CancellationTokenSource();

            queueProcessor = new QueueProcessor<string>(Write, cancellationTokenSource.Token);

            if (!File.Exists(filePath))
            {
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);

                    Task.Delay(500).Wait();
                }

                fileStream =
                    File.Create(filePath);

                fileStream.Close();

                Task.Delay(1000).Wait();
            }

            fileStream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
        }

        private void Write(string content)
        {
            Task.Run(() => Console.Out.WriteAsync(content), cancellationTokenSource.Token).ConfigureAwait(false);

            Task.Run(() =>
            {
                var byteArrayContent = new UTF8Encoding(true).GetBytes(content);

                fileStream.Position = fileStream.Length;

                fileStream.WriteAsync(byteArrayContent, 0, byteArrayContent.Length).Wait();

                fileStream.Flush();
            }).ConfigureAwait(true);
        }

        public void Debug(string content, [CallerFilePath] string callerName = "",
            [CallerMemberName] string memberName = "")
        {
            var logStatement =
                $"{DateTime.UtcNow:O}[DEBUG]{Path.GetFileNameWithoutExtension(callerName)}.{memberName}|{content}\n";

            queueProcessor.Add(logStatement);
        }

        public void Info(string content, [CallerFilePath] string callerName = "",
            [CallerMemberName] string memberName = "")
        {
            var logStatement =
                $"{DateTime.UtcNow:O}[INFO]{Path.GetFileNameWithoutExtension(callerName)}.{memberName}|{content}\n";

            queueProcessor.Add(logStatement);
        }

        public void Error(string content, [CallerFilePath] string callerName = "",
            [CallerMemberName] string memberName = "")
        {
            var logStatement =
                $"{DateTime.UtcNow:O}[ERROR]{Path.GetFileNameWithoutExtension(callerName)}.{memberName}|{content}\n";

            queueProcessor.Add(logStatement);
        }

        public void Error(string content, Exception exception, [CallerFilePath] string callerName = "",
            [CallerMemberName] string memberName = "")
        {
            var logStatement =
                $"{DateTime.UtcNow:O}[ERROR]{Path.GetFileNameWithoutExtension(callerName)}.{memberName}|{content}{exception}\n";

            queueProcessor.Add(logStatement);
        }

        public void Fatal(string content, [CallerFilePath] string callerName = "",
            [CallerMemberName] string memberName = "")
        {
            var logStatement =
                $"{DateTime.UtcNow:O}[FATAL]{Path.GetFileNameWithoutExtension(callerName)}.{memberName}|{content}\n";

            queueProcessor.Add(logStatement);
        }

        public void Fatal(string content, Exception exception, [CallerFilePath] string callerName = "",
            [CallerMemberName] string memberName = "")
        {
            var logStatement =
                $"{DateTime.UtcNow:O}[FATAL]{Path.GetFileNameWithoutExtension(callerName)}.{memberName}|{content}{exception}\n";

            queueProcessor.Add(logStatement);
        }
    }

}