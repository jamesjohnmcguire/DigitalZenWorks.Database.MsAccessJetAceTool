// <copyright file="MsAccessTool.cs" company="James John McGuire">
// Copyright © 2006 - 2026 James John McGuire. All Rights Reserved.
// </copyright>

[assembly: System.CLSCompliant(true)]

namespace MsAccessJetAceTool;

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using Common.Logging;
using DigitalZenWorks.Database.ToolKit;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;

/// <summary>
/// Microsoft Access tool.
/// </summary>
internal static class MsAccessTool
{
	private static readonly ILog Log = LogManager.GetLogger(
		System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

	private static readonly ResourceManager StringTable = new(
		"MsAccessJetAceTool.Resources",
		Assembly.GetExecutingAssembly());

	/// <summary>
	/// The programs main entry point.
	/// </summary>
	/// <param name="args">The array of arguments.</param>
	/// <returns>A status code.</returns>
	public static int Main(string[] args)
	{
		LogInitialization();

		int returnCode = ProcessCommand(args);

		return returnCode;
	}

	/// <summary>
	/// Parses the supplied arguments and dispatches the 'import' or 'export'
	/// command.
	/// </summary>
	/// <remarks>Performs a case-insensitive comparison of the command name.
	/// Calls Usage() when arguments are missing or the command is unknown and
	/// logs a warning for unknown commands.</remarks>
	/// <param name="args">Array of command-line arguments where the first
	/// element specifies the command ('import' or 'export') and remaining
	/// elements are passed to the command. If null or fewer than three
	/// elements, usage information is displayed.</param>
	/// <returns>Exit code returned by the invoked command, or -1 when arguments
	/// are invalid or the command is unrecognized.</returns>
	internal static int ProcessCommand(string[] args)
	{
		int returnCode = -1;

		if (args == null || args.Length < 3)
		{
			Usage();
		}
		else
		{
			string command = args[0];

			if (command.Equals(
				"import", StringComparison.OrdinalIgnoreCase))
			{
				returnCode = Import(args);
			}
			else if (command.Equals(
				"export", StringComparison.OrdinalIgnoreCase))
			{
				returnCode = Export(args);
			}
			else
			{
				Log.Warn("unknown command");
				Usage();
			}
		}

		return returnCode;
	}

	private static int CommandComplete(string command, bool successCode)
	{
		int returnCode = -1;

		if (successCode == true)
		{
			Log.Info($"{command} complete.");

			returnCode = 0;
		}

		return returnCode;
	}

	private static int Export(string[] args)
	{
		int returnCode = -1;
		string databaseFile = args[1];
		string sqlFile = args[2];
		bool toSqlite = args.Contains("--to-sqlite");

		Log.Info("exporting");

		bool successCode =
			DataDefinitionOleDb.ExportSchema(databaseFile, sqlFile, toSqlite);

		returnCode = CommandComplete(args[0], successCode);

		return returnCode;
	}

	private static int Import(string[] args)
	{
		int returnCode = -1;
		string sqlFile = args[1];
		string databaseFile = args[2];

		Log.Info("importing");

		string databaseFilePath = Path.GetDirectoryName(databaseFile);

		if (string.IsNullOrWhiteSpace(databaseFilePath))
		{
			string currentDirectory = Directory.GetCurrentDirectory();

			databaseFile = Path.Combine(currentDirectory, databaseFile);
		}

		bool successCode = OleDbHelper.CreateAccessDatabaseFile(databaseFile);

		if (successCode == true)
		{
			successCode = DataDefinitionOleDb.ImportSchema(
				sqlFile, databaseFile);

			returnCode = CommandComplete(args[0], successCode);
		}

		return returnCode;
	}

	private static void LogInitialization()
	{
		string applicationDataDirectory = @"DigitalZenWorks\BackUpManager";
		string baseDataDirectory = Environment.GetFolderPath(
			Environment.SpecialFolder.ApplicationData,
			Environment.SpecialFolderOption.Create) + @"\" +
			applicationDataDirectory;

		string logFilePath = baseDataDirectory + "\\Backup.log";
		string outputTemplate =
			"[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] " +
			"{Message:lj}{NewLine}{Exception}";

		LoggerConfiguration configuration = new();
		LoggerSinkConfiguration sinkConfiguration = configuration.WriteTo;
		sinkConfiguration.Console(
			LogEventLevel.Verbose,
			outputTemplate,
			CultureInfo.InvariantCulture);
		sinkConfiguration.File(
			logFilePath,
			LogEventLevel.Verbose,
			outputTemplate,
			CultureInfo.InvariantCulture);
		Serilog.Log.Logger = configuration.CreateLogger();

		LogManager.Adapter =
			new Common.Logging.Serilog.SerilogFactoryAdapter();
	}

	private static void Usage()
	{
		string usage1 = StringTable.GetString(
			"USAGE1", CultureInfo.InvariantCulture);
		string usage2 = StringTable.GetString(
			"USAGE2", CultureInfo.InvariantCulture);
		string usage3 = StringTable.GetString(
			"USAGE3", CultureInfo.InvariantCulture);

		Console.WriteLine(usage1);
		Console.WriteLine(usage2);
		Console.WriteLine(usage3);
	}
}
