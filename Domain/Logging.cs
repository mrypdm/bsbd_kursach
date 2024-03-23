using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using ILogger = Serilog.ILogger;

namespace Domain;

[SuppressMessage("Naming", "CA1724:Type names should not match namespaces")]
public static class Logging
{
    public static bool IsInit { get; private set; }

    public static ILogger Logger { get; private set; }

    public static ILoggerFactory LoggerFactory { get; private set; }

    public static void Init()
    {
        const string format = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}";
        const string filePath = "./logs/log.txt";

        var loggerConfiguration = new LoggerConfiguration()
            .WriteTo.Async(p => p.Console(LogEventLevel.Information, format, CultureInfo.InvariantCulture))
            .WriteTo.Async(p => p.File(filePath, LogEventLevel.Debug, format, CultureInfo.InvariantCulture));
        Log.Logger = loggerConfiguration.CreateLogger();

        IsInit = true;

        Logger = Log.Logger;
        LoggerFactory = new SerilogLoggerFactory();
    }

    public static void Close()
    {
        Log.CloseAndFlush();
        IsInit = false;
        Logger = null;
        LoggerFactory = null;
    }
}