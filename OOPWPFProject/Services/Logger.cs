using System.Globalization;
using System.IO;
using System.Text;

namespace OOPWPFProject.Services;

public enum LogLevel
{
	Debug,
	Info,
	Error,
}

public sealed class Logger : IDisposable
{
	private static Logger? _instance;
	private static readonly object _initLock = new();

	private readonly StreamWriter _writer;
	private readonly object _lock = new();
	private bool _disposed;

	public string LogFilePath { get; }

	private Logger(string logFilePath)
	{
		LogFilePath = logFilePath;

		FileStream fs = new FileStream(
			logFilePath,
			FileMode.Append,
			FileAccess.Write,
			FileShare.Read
		);

		_writer = new StreamWriter(fs, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false))
		{
			AutoFlush = true,
		};
	}

	public static void Initialize(string logFilePath)
	{
		if (_instance == null)
		{
			lock (_initLock)
			{
				_instance ??= new Logger(logFilePath);
			}
		}
	}

	public static void Log(LogLevel level, string message, Exception? exception = null)
	{
		_instance?.Write(level, message, exception);
	}

	private void Write(LogLevel level, string message, Exception? exception)
	{
		if (message is null)
		{
			return;
		}

		lock (_lock)
		{
			if (_disposed)
			{
				return;
			}

			_writer.WriteLine(
				$"[{DateTime.Now:HH:mm:ss}] | {level.ToString().ToUpperInvariant(), -5} | {message}"
			);

			if (exception is not null)
			{
				_writer.WriteLine(exception);
			}
		}
	}

	public static string WorkingTime()
	{
		var workingTime = (DateTime.Now - App.StartTime).ToString(
			@"hh\:mm\:ss",
			CultureInfo.CurrentCulture
		);
		return workingTime;
	}

	public static void Close()
	{
		_instance?.Dispose();
	}

	public void Dispose()
	{
		lock (_lock)
		{
			if (_disposed)
				return;
			_disposed = true;
			_writer.Dispose();
		}
	}
}
