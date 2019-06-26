/////////////////////////////////////////////////////////////////////////////
// Copyright © 2008-2016 by James John McGuire
// All rights reserved.
/////////////////////////////////////////////////////////////////////////////
using System;
using System.IO;
using System.Text;
using DigitalZenWorks.Common.DatabaseLibrary;
using Common.Logging;
using System.Globalization;

namespace MsAccessTool
{
	class MsAccessTool
	{
		private static readonly ILog log = LogManager.GetLogger
			(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		static int Main(string[] args)
		{
			int returnCode = -1;

			try
			{
				if (3 > args.Length)
				{
					Usage();
				}
				else
				{
					if (args[0].ToLower().Equals("import"))
					{
						log.Info(CultureInfo.InvariantCulture,
							m => m("importing"));

						string sqlFile = args[1];
						string databaseFile = args[2];

						bool successCode = DatabaseUtilities.
							CreateAccessDatabaseFile(databaseFile);
						if (true == successCode)
						{
							successCode = DataDefinition.ImportSchema(sqlFile,
								databaseFile);
							returnCode = Convert.ToInt32(successCode);
						}
					}
					else if (args[0].ToLower().Equals("export"))
					{
						string databaseFile = args[1];
						string sqlFile = args[2];

						bool successCode = DataDefinition.ExportSchema(
							databaseFile, sqlFile);

						returnCode = Convert.ToInt32(successCode);
					}
					else
					{
						log.Warn(CultureInfo.InvariantCulture,
							m => m("unknown command"));
						Usage();
					}

				}
			}
			catch
			{
			}

			return returnCode;
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
