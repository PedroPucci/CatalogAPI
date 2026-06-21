using Serilog;
using ILogger = Serilog.ILogger;

namespace CatalogAPI.Extensions.ExtensionsLogs
{
    public class LogExtension
    {
        private static readonly string LogDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "Downloads",
            "FCG-CatalogAPI-logs"
        );

        static LogExtension()
        {
            InitializeLogger();
        }

        public static void InitializeLogger()
        {
            try
            {
                Directory.CreateDirectory(LogDirectory);
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.File(Path.Combine(LogDirectory, "log-.txt"), rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true, fileSizeLimitBytes: 1000000)
                    .CreateLogger();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to start the logger: {ex.Message}");
            }
        }

        public static ILogger GetLogger()
        {
            return Log.Logger;
        }
    }
}