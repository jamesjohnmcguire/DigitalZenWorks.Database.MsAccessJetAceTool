using System;
using System.Data.OleDb;
using System.IO;
using System.Reflection;

namespace JetImportSchema
{
	class Program
	{
		/////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Represents a provider type for a connection string
		/// </summary>
		/////////////////////////////////////////////////////////////////////
		private string provider = "Microsoft.Jet.OLEDB.4.0";

		static int Main(string[] args)
		{
			int ReturnCode = -1;
			if (args.Length < 2)
			{
				Console.WriteLine("usage: JetImportSchema <schema file> <new .mdb file>");
			}
			else
			{
				if (Environment.Is64BitOperatingSystem)
				{
					provider = "Microsoft.ACE.OLEDB.12.0";
				}

				CreateMdbFile(args[1]);
				ImportSchema(args[0], args[1]);

				ReturnCode = 0;
			}

			return ReturnCode;
		}

		static void CreateMdbFile(
			string NewFilePath)
		{
			Stream TemplateObjectStream = null;
			FileStream NewFileStream = null;

			try
			{
				byte[] EmbeddedResource;
				Assembly ThisAssembly = Assembly.GetExecutingAssembly(); 

				TemplateObjectStream = ThisAssembly.GetManifestResourceStream("JetImportSchema.template.mdb");
				EmbeddedResource = new Byte[TemplateObjectStream.Length];
				TemplateObjectStream.Read(EmbeddedResource, 0, (int)TemplateObjectStream.Length);
				NewFileStream = new FileStream(NewFilePath, FileMode.Create);
				NewFileStream.Write(EmbeddedResource, 0, (int)TemplateObjectStream.Length);
				NewFileStream.Close();
			}
			catch (Exception Ex)
			{
				Console.WriteLine("Exception: " + Ex.ToString());
			}
		}

		static void ImportSchema(
			string SchemaFile,
			string MdbFile)
		{
			try
			{
				if (File.Exists(SchemaFile))
				{
					string FileContents = GetFileContents(SchemaFile);

					string[] StringSeparators = new string[] { "\r\n\r\n" };
					string[] Queries = FileContents.Split(StringSeparators,
															32000,
															StringSplitOptions.RemoveEmptyEntries);

					string ConnectionString = "Provider=" + provider + "; Data Source=" + MdbFile;
					OleDbConnection Connection = new OleDbConnection(ConnectionString);
					OleDbCommand ThisCommand = new OleDbCommand();

					Connection.Open();

					ThisCommand.Connection = Connection;
					ThisCommand.CommandTimeout = 30;

					foreach (string SqlQuery in Queries)
					{
						ThisCommand.CommandText = SqlQuery;
						ThisCommand.ExecuteNonQuery();
					}
				}
			}
			catch (Exception Ex)
			{
				Console.WriteLine("Exception: " + Ex.ToString());
			}
		}

		static string GetFileContents(
			string FilePath)
		{
			string FileContents = null;

			if (File.Exists(FilePath))
			{
				StreamReader StreamReaderObject = new StreamReader(FilePath);

				if (null != StreamReaderObject)
				{
					FileContents = StreamReaderObject.ReadToEnd();
					StreamReaderObject.Close();
				}
			}

			return FileContents;
		}

	}
}
