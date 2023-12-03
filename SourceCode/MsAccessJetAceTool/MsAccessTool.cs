// <copyright file="MsAccessTool.cs" company="James John McGuire">
// Copyright © 2006 - 2023 James John McGuire. All Rights Reserved.
// </copyright>

using Common.Logging;
using DigitalZenWorks.Database.ToolKit;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using System;
using System.Globalization;

[assembly: CLSCompliant(true)]

namespace MsAccessJetAceTool
{
	/// <summary>
	/// Microsoft Access tool.
	/// </summary>
	public static class MsAccessTool
	{
		private static readonly ILog Log = LogManager.GetLogger(
			System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// The programs main entry point.
		/// </summary>
		/// <param name="args">The array of arguments.</param>
		/// <returns>A status code.</returns>
		public static int Main(string[] args)
		{
			int returnCode = -1;

			LogInitialization();

			if (args == null || 3 > args.Length)
			{
				Usage();
			}
			else
			{
				string command = args[0];

				if (command.Equals(
					"import", StringComparison.OrdinalIgnoreCase))
				{
					Log.Info(
						CultureInfo.InvariantCulture,
						m => m("importing"));

					string sqlFile = args[1];
					string databaseFile = args[2];

					bool successCode = DatabaseUtilities.
						CreateAccessDatabaseFile(databaseFile);
					if (true == successCode)
					{
						successCode = DataDefinition.ImportSchema(
							sqlFile, databaseFile);
						returnCode = Convert.ToInt32(successCode);
					}
				}
				else if (command.Equals(
					"export", StringComparison.OrdinalIgnoreCase))
				{
					string databaseFile = args[1];
					string sqlFile = args[2];

					bool successCode = DataDefinition.ExportSchema(
						databaseFile, sqlFile);

					returnCode = Convert.ToInt32(successCode);
				}
				else
				{
					Log.Warn(
						CultureInfo.InvariantCulture,
						m => m("unknown command"));
					Usage();
				}
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

			LoggerConfiguration configuration = new ();
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
			Console.WriteLine(
				"usage: MsAccessTool import <Sql File> <MDB File>");
			Console.WriteLine(
				"usage: MsAccessTool export <MDB File> <Sql File>");
		}
	}
}
