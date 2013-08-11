/////////////////////////////////////////////////////////////////////////////
// JetUtil
//
// Console application to export a Jet (MDB, MS Access) database to a Sql
// definition file, which can be used to create a new database.
// 
// 2008-07-26:	Created
//
// Copyright © James John McGuire
// Portions Copyright © 2003 Michael Perlini
/////////////////////////////////////////////////////////////////////////////
using System;
using System.IO;
using System.Text;

namespace JetUtil
{
	class JetUtil
	{
		static int Main(string[] args)
		{
			JetUtil This = new JetUtil();

			int ReturnCode = This.Run(args);

			return ReturnCode;
		}

		private int Run(string[] args)
		{
			int ReturnCode = -1;

			if (2 > args.Length)
			{
				Console.WriteLine("usage: JetUtil <MDB File> <Sql Export File>");
			}
			else
			{
				string MdbFile = args[0];
				string SqlFile = args[1];
				if (!File.Exists(MdbFile))
				{
					Console.WriteLine("Mdb File does not exist");
				}
				else
				{
					JetExport ExportEngine = new JetExport(MdbFile);
					string SqlStatements = ExportEngine.GetSqlStatements();
					StreamWriter FileStream = new StreamWriter(SqlFile);

					FileStream.Write(SqlStatements);
					FileStream.Close();

					Console.WriteLine("Finished.");
					ReturnCode = 0;
				}
			}

			return ReturnCode;
		}

	}
}
