using System;
using System.Data;
using System.Data.OleDb;

namespace JetUtil
{
	/////////////////////////////////////////////////////////////////////////
	/// Class <c>Connection</c>
	/// <summary>
	/// Represents an OleDbConnection
	/// </summary>
	/////////////////////////////////////////////////////////////////////////
	public class Connection
	{
		/////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Represents an OleDb open connection to a data source.
		/// </summary>
		/////////////////////////////////////////////////////////////////////
		private OleDbConnection oleDbConnection;

		/////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Represents a provider type for a connection string
		/// </summary>
		/////////////////////////////////////////////////////////////////////
		private string provider = "Microsoft.Jet.OLEDB.4.0";

		/////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="databaseFile"></param>
		/////////////////////////////////////////////////////////////////////
		public Connection(string databaseFile)
		{
			if (Environment.Is64BitProcess)
			{
				provider = "Microsoft.ACE.OLEDB.12.0";
			}

			string connectionString = @"Provider=" + provider + ";Password=\"\";User ID=Admin;"
					+ "Data Source=" + databaseFile + @";Mode=Share Deny None;Extended Properties="""";Jet OLEDB:System database="""";"
					+ @"Jet OLEDB:Registry Path="""";Jet OLEDB:Database Password="""";Jet OLEDB:Engine Type=5;"
					+ @"Jet OLEDB:Database Locking Mode=1;Jet OLEDB:Global Partial Bulk Ops=2;Jet OLEDB:Global Bulk Transactions=1;"
					+ @"Jet OLEDB:New Database Password="""";Jet OLEDB:Create System Database=False;Jet OLEDB:Encrypt Database=False;"
					+ @"Jet OLEDB:Don't Copy Locale on Compact=False;Jet OLEDB:Compact Without Replica Repair=False;Jet OLEDB:SFP=False";

			oleDbConnection = new OleDbConnection(connectionString);
		}

		/////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Gets the constraints from the given table
		/// </summary>
		/// <returns>DataTable</returns>
		/////////////////////////////////////////////////////////////////////
		private DataTable GetConstraints(string tablename)
		{
			oleDbConnection.Open();

			DataTable schemaTable = oleDbConnection.GetOleDbSchemaTable(
				System.Data.OleDb.OleDbSchemaGuid.Constraint_Column_Usage,
				new Object[] { null, null, tablename, null, null, null });

			oleDbConnection.Close();

			return schemaTable;
		}

		/////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Gets the foreign keys from the given table
		/// </summary>
		/// <returns>DataTable</returns>
		/////////////////////////////////////////////////////////////////////
		public DataTable GetForeignKeys(string tablename)
		{
			oleDbConnection.Open();

			DataTable schemaTable = oleDbConnection.GetOleDbSchemaTable(
				System.Data.OleDb.OleDbSchemaGuid.Foreign_Keys,
				new Object[] { null, null, tablename });

			oleDbConnection.Close();

			return schemaTable;
		}

		/////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Gets the primary keys from the given table
		/// </summary>
		/// <returns>DataTable</returns>
		/////////////////////////////////////////////////////////////////////
		public DataTable GetPrimaryKeys(string tablename)
		{
			oleDbConnection.Open();

			DataTable schemaTable = oleDbConnection.GetOleDbSchemaTable(
				System.Data.OleDb.OleDbSchemaGuid.Primary_Keys,
				new Object[] { null, null, tablename });

			oleDbConnection.Close();

			return schemaTable;
		}

		/////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Gets the column names from the given table
		/// </summary>
		/// <returns>DataTable</returns>
		/////////////////////////////////////////////////////////////////////
		public DataTable GetTableColumns(string tablename)
		{
			oleDbConnection.Open();

			DataTable schemaTable = oleDbConnection.GetOleDbSchemaTable(
				System.Data.OleDb.OleDbSchemaGuid.Columns,
				new Object[] { null, null, tablename });

			oleDbConnection.Close();

			return schemaTable;
		}

		/////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Gets the constraints from the given table
		/// </summary>
		/// <returns>DataTable</returns>
		/////////////////////////////////////////////////////////////////////
		public DataTable GetTableConstraints(string tablename)
		{
			oleDbConnection.Open();

			DataTable schemaTable = oleDbConnection.GetOleDbSchemaTable(
				System.Data.OleDb.OleDbSchemaGuid.Table_Constraints,
				new Object[] { null, null, null, null, null, tablename });

			oleDbConnection.Close();

			return schemaTable;
		}

		/////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Gets all table names from the connected database
		/// </summary>
		/// <returns>DataTable</returns>
		/////////////////////////////////////////////////////////////////////
		public DataTable GetTableNames()
		{
			oleDbConnection.Open();

			DataTable schemaTable = oleDbConnection.GetOleDbSchemaTable(
				System.Data.OleDb.OleDbSchemaGuid.Tables,
				new Object[] { null, null, null, "TABLE" });

			oleDbConnection.Close();

			return schemaTable;
		}
	}
}