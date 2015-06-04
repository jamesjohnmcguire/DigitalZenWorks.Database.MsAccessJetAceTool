/////////////////////////////////////////////////////////////////////////////
// $Id:  $
//
// Copyright (c) 2008-2015 by James John McGuire
// All rights reserved.
/////////////////////////////////////////////////////////////////////////////
using System;
using System.IO;
using System.Text;
using DigitalZenWorks.Common.DatabaseLibrary;

namespace MsAccessTool
{
	class MsAccessTool
	{
		static int Main(string[] args)
		{
			int returnCode = -1;

			if (3 > args.Length)
			{
				Console.WriteLine(
					"usage: MsAccessTool import <Sql File> <MDB File>");
				Console.WriteLine(
					"usage: MsAccessTool export <MDB File> <Sql File>");
			}
			else
			{
				if (args[0].ToLower().Equals("import"))
				{
					string sqlFile = args[1];
					string databaseFile = args[2];

					DatabaseUtils.CreateMdbFile(databaseFile);
					bool successCode = DataDefinition.ImportSchema(sqlFile,
						databaseFile);

					returnCode = Convert.ToInt32(successCode);
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
					Console.WriteLine("unknown command");
					Console.WriteLine(
						"usage: MsAccessTool import <Sql File> <MDB File>");
					Console.WriteLine(
						"usage: MsAccessTool export <MDB File> <Sql File>");
				}

			}

			return returnCode;
		}
	}
}
