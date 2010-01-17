using System;
using System.Collections;
using System.Data;
using System.Data.OleDb;

namespace JetUtil
{
	enum ColumnTypes
	{
		Autonumber,
		Currency,
		DateTime,
		Memo,
		Number,
		Ole,
		String,
		YesNo
	}
	
	// A struct to hold foreign key constraints temporarily
	struct Relationship
	{
		public string m_Name;
		public string m_ParentTable;
		public string m_ParentTableCol;
		public string m_ChildTable;
		public string m_ChildTableCol;
		public bool m_OnUpdateCascade;
		public bool m_OnDeleteCascade;
			
		public Relationship(
			string Name,
			string ParentTable,
			string ParentTableColumn,
			string child_table,
			string child_table_column)
		{
			m_Name = Name;
			m_ParentTable = ParentTable;
			m_ParentTableCol = ParentTableColumn;
			m_ChildTable = child_table;
			m_ChildTableCol = child_table_column;
			m_OnUpdateCascade = false;
			m_OnDeleteCascade = false;
		}
	}

	/////////////////////////////////////////////////////////////////////////
	/// Class <c>JetExport</c>
	/// <summary>
	/// Represents an engine to export a jet (MDB, used by MS Access)
	/// database to a Sql definition file.
	/// </summary>
	/////////////////////////////////////////////////////////////////////////
	public class JetExport
	{
		/////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Represents an OleDb open connection to a data source.
		/// </summary>
		/////////////////////////////////////////////////////////////////////
		OleDbConnection m_Connection;

		/////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="MdbFile"></param>
		/////////////////////////////////////////////////////////////////////
		public JetExport(
			string MdbFile)
		{
			m_Connection = new OleDbConnection();

			m_Connection.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Password="""";User ID=Admin;"
					+ "Data Source=" + MdbFile + @";Mode=Share Deny None;Extended Properties="""";Jet OLEDB:System database="""";"
					+ @"Jet OLEDB:Registry Path="""";Jet OLEDB:Database Password="""";Jet OLEDB:Engine Type=5;"
					+ @"Jet OLEDB:Database Locking Mode=1;Jet OLEDB:Global Partial Bulk Ops=2;Jet OLEDB:Global Bulk Transactions=1;"
					+ @"Jet OLEDB:New Database Password="""";Jet OLEDB:Create System Database=False;Jet OLEDB:Encrypt Database=False;"
					+ @"Jet OLEDB:Don't Copy Locale on Compact=False;Jet OLEDB:Compact Without Replica Repair=False;Jet OLEDB:SFP=False";
		}

		/////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Gets a Sql string
		/// </summary>
		/// <returns></returns>
		/////////////////////////////////////////////////////////////////////
		public string GetSqlStatements()
		{
			System.Collections.Hashtable tables = ParseJET();

			string sqlString = "";

			// Get Sorted List
			ArrayList list = OrderTable(tables);

			foreach (string Name in list)
			{
				sqlString += WriteSQL((Table)tables[Name]);
				sqlString += Environment.NewLine;
			}

			return sqlString;
		}
		private DataTable GetTableNames()
		{
			m_Connection.Open();

			DataTable schemaTable = m_Connection.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, new Object[] { null, null, null, "TABLE" });

			m_Connection.Close();

			return schemaTable;
		}

		private DataTable GetPrimaryKeys(string tablename)
		{
			m_Connection.Open();

			DataTable schemaTable = m_Connection.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Primary_Keys, new Object[] { null, null, tablename });

			m_Connection.Close();

			return schemaTable;
		}

		private DataTable GetForeignKeys(string tablename)
		{
			m_Connection.Open();

			DataTable schemaTable = m_Connection.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Foreign_Keys, new Object[] { null, null, tablename });

			m_Connection.Close();

			return schemaTable;
		}

		private DataTable GetConstraints(string tablename)
		{
			m_Connection.Open();

			DataTable schemaTable = m_Connection.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Constraint_Column_Usage, new Object[] { null, null, tablename, null, null, null });

			m_Connection.Close();

			return schemaTable;
		}

		private DataTable GetTableConstraints(string tablename)
		{
			m_Connection.Open();

			DataTable schemaTable = m_Connection.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Table_Constraints, new Object[] { null, null, null, null, null, tablename });

			m_Connection.Close();

			return schemaTable;
		}

		private DataTable GetTableColumns(string tablename)
		{
			m_Connection.Open();

			DataTable schemaTable = m_Connection.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Columns, new Object[] { null, null, tablename });

			m_Connection.Close();

			return schemaTable;
		}
		// Parse a jet database, returning a hashtable of Table structures
		// Key is name of table.
		private System.Collections.Hashtable ParseJET()
		{
			System.Collections.Hashtable tables = new System.Collections.Hashtable();

			System.Collections.ArrayList relationships = new System.Collections.ArrayList();

			DataTable t = GetTableNames();

			foreach (DataRow row in t.Rows)
			{
				string table_name = row["TABLE_NAME"].ToString();

				Table table = new Table(table_name);

				Console.WriteLine("Getting Columns for " + table_name);
				DataTable cols = GetTableColumns(table_name);

				foreach (DataRow r in cols.Rows)
				{
					Column col = new Column();
					col.Name = r["COLUMN_NAME"].ToString();

					switch ((int)r["DATA_TYPE"])
					{
						case 3:	// Number
							col.ColumnType = (int)ColumnTypes.Number;
							break;
						case 130:  // String
							if (Int32.Parse(r["COLUMN_FLAGS"].ToString()) > 127)
							{
								col.ColumnType = (int)ColumnTypes.Memo;
							}
							else
							{
								col.ColumnType = (int)ColumnTypes.String;
							}
							break;
						case 7:  // Date
							col.ColumnType = (int)ColumnTypes.DateTime;
							break;
						case 6:  // Currency
							col.ColumnType = (int)ColumnTypes.Currency;
							break;
						case 11:  // Yes/No
							col.ColumnType = (int)ColumnTypes.YesNo;
							break;
						case 128:  // OLE
							col.ColumnType = (int)ColumnTypes.Ole;
							break;
					}

					if (!r.IsNull("CHARACTER_MAXIMUM_LENGTH"))
						col.Length = Int32.Parse(r["CHARACTER_MAXIMUM_LENGTH"].ToString());

					if (r["IS_NULLABLE"].ToString() == "True") col.Nullable = true;

					if (r["COLUMN_HASDEFAULT"].ToString() == "True")
						col.DefaultValue = r["COLUMN_DEFAULT"].ToString();

					col.Position = Int32.Parse(r["ORDINAL_POSITION"].ToString());

					table.AddColumn(col);
				}

				// Get primary key
				DataTable primary_key_table = GetPrimaryKeys(table_name);

				foreach (DataRow pkrow in primary_key_table.Rows)
				{
					table.m_PrimaryKey = pkrow["COLUMN_NAME"].ToString();
				}

				// If PK is an integer change type to Autonumber
				if (table.m_PrimaryKey != "")
				{
					if (((Column)table.m_Columns[table.m_PrimaryKey]).ColumnType == (int)ColumnTypes.Number)
					{
						((Column)table.m_Columns[table.m_PrimaryKey]).ColumnType = (int)ColumnTypes.Autonumber;
					}
				}

				DataTable foreign_key_table = GetForeignKeys(table_name);

				foreach (DataRow fkrow in foreign_key_table.Rows)
				{
					Relationship rel = new Relationship(
						fkrow["FK_NAME"].ToString(),
						fkrow["PK_TABLE_NAME"].ToString(),
						fkrow["PK_COLUMN_NAME"].ToString(),
						fkrow["FK_TABLE_NAME"].ToString(),
						fkrow["FK_COLUMN_NAME"].ToString()
					);

					if (fkrow["UPDATE_RULE"].ToString() != "NO ACTION")
						rel.m_OnUpdateCascade = true;

					if (fkrow["DELETE_RULE"].ToString() != "NO ACTION")
						rel.m_OnDeleteCascade = true;

					relationships.Add(rel);
				}
				tables.Add(table.m_Name, table);
			}

			// Add foreign keys to table, using relationships
			foreach (Relationship rel in relationships)
			{
				string tname = rel.m_ChildTable;

				ForeignKey fk = new ForeignKey(rel.m_Name, rel.m_ChildTableCol, rel.m_ParentTable,
					rel.m_ParentTableCol, rel.m_OnDeleteCascade, rel.m_OnUpdateCascade);

				((Table)tables[tname]).AddForeignKey(fk);
			}

			return tables;
		}

		// Return an array list in the order that the tables need to be added
		// to take dependencies into account
		private ArrayList OrderTable(Hashtable table)
		{
			Hashtable dependencies = new Hashtable();

			Hashtable list = new Hashtable();
			ArrayList deps = new ArrayList();

			foreach (DictionaryEntry ent in table)
			{
				string Name = (string)ent.Key;
				Table t = (Table)ent.Value;
				foreach (ForeignKey fk in t.m_ForeignKeys)
				{
					deps.Add(fk.m_ParentTable);
				}
				list.Add(Name, new ArrayList(deps));
				deps.Clear();
			}

			return TopologicalSort(list);
		}

		// Take a Table structure and output the SQL commands
		private string WriteSQL(Table t)
		{
			string SQL = "";

			SQL += String.Format("CREATE TABLE [{0}] ({1}", t.m_Name, Environment.NewLine);

			// Sort Columns into ordinal positions
			System.Collections.SortedList columns = new System.Collections.SortedList();

			foreach (DictionaryEntry d in t.m_Columns)
			{
				Column col = (Column)d.Value;
				columns.Add(col.Position, col);
			}

			foreach (DictionaryEntry d in columns)
			{
				SQL += "\t" + WriteColumnSQL((Column)d.Value) + "," + Environment.NewLine;
			}

			if (!(t.m_PrimaryKey == ""))
				SQL += String.Format("\tCONSTRAINT PrimaryKey PRIMARY KEY ([{0}]),{1}", t.m_PrimaryKey, Environment.NewLine);

			foreach (ForeignKey fk in t.m_ForeignKeys)
			{
				SQL += "\t" + WriteFKSQL(fk) + "," + Environment.NewLine;
			}

			// Remove trailing ','
			SQL = SQL.Remove(SQL.Length - 3, 3);

			SQL += String.Format("{0});{0}", Environment.NewLine);

			return SQL;
		}

		// Write the SQL for a column
		private string WriteColumnSQL(Column c)
		{
			string colSQL = "";

			colSQL += "[" + c.Name + "]";

			switch (c.ColumnType)
			{
				case (int)ColumnTypes.Number:
				case (int)ColumnTypes.Autonumber:
					colSQL += " INTEGER";
					break;
				case (int)ColumnTypes.String:
					colSQL += String.Format(" CHAR({0})", c.Length);
					break;
				case (int)ColumnTypes.Memo:
					colSQL += " MEMO";
					break;
				case (int)ColumnTypes.DateTime:
					colSQL += " DATETIME";
					break;
				case (int)ColumnTypes.Currency:
					colSQL += " CURRENCY";
					break;
				case (int)ColumnTypes.Ole:
					colSQL += " OLEOBJECT";
					break;
				case (int)ColumnTypes.YesNo:
					colSQL += " YESNO";
					break;
			}

			if (c.Unique)
				colSQL += " UNIQUE";

			if (!c.Nullable)
				colSQL += " NOT NULL";

			if (c.ColumnType == (int)ColumnTypes.Autonumber)
				colSQL += " IDENTITY";

			if (!(c.DefaultValue == ""))
				colSQL += " DEFAULT " + c.DefaultValue;

			return colSQL;
		}

		// Write the SQL for a Foreign Key constraint
		private string WriteFKSQL(ForeignKey fk)
		{
			string FKeySQL = "";

			if (fk.m_ColumnName == fk.m_ParentTableColumn)
			{
				FKeySQL = String.Format("CONSTRAINT [{0}] FOREIGN KEY ([{1}]) REFERENCES [{2}]", fk.m_Name, fk.m_ColumnName, fk.m_ParentTable);
			}
			else
			{
				FKeySQL = String.Format("CONSTRAINT [{0}] FOREIGN KEY ([{1}]) REFERENCES [{2}] ([{3}])", fk.m_Name, fk.m_ColumnName, fk.m_ParentTable, fk.m_ParentTableColumn);
			}

			if (fk.m_CascadeOnDelete)
			{
				FKeySQL += " ON DELETE CASCADE";
			}

			if (fk.m_CascadeOnUpdate)
			{
				FKeySQL += " ON UPDATE CASCADE";
			}

			return FKeySQL;
		}

		/////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Outputs the schema.
		/// </summary>
		/////////////////////////////////////////////////////////////////////
		public void OutputSchema()
		{
			DataTable tables = GetTableNames();
			DumpTable(tables);
			
			foreach (DataRow row in tables.Rows) {
				string table_name = row["TABLE_NAME"].ToString();
				
				Console.WriteLine("For table - " + table_name);
				
				Console.WriteLine("Columns:");
				DataTable cols = GetTableColumns(table_name);
				DumpTable(cols);
				
				Console.WriteLine("TableConstraints:");
				DataTable refcons = GetTableConstraints(table_name);
				DumpTable(refcons);
				
				Console.WriteLine("Primary Key:");
				DataTable pk = GetPrimaryKeys(table_name);
				DumpTable(pk);
			
				Console.WriteLine("Foreign Key:");
				DataTable fk = GetForeignKeys(table_name);
				DumpTable(fk);
			}
		}
		
		private void DumpTable(DataTable table) {
			
			foreach (DataRow row in table.Rows) {
				foreach (DataColumn col in table.Columns) {
					Console.WriteLine(col.ColumnName + " = " + row[col].ToString());
				}
				Console.WriteLine();
			}
		}

		/// <summary>
		/// Performs a topological sort on a list with dependencies
		/// </summary>
		/// <param name="table">A table to be sorted with the structure
		/// Object name, ArrayList dependencies.</param>
		/// <returns>A sorted arraylist.</returns>
		ArrayList TopologicalSort(Hashtable table)
		{
			ArrayList sortedList = new ArrayList();
			object key;
			ArrayList dependencies;

			while (sortedList.Count < table.Count)
			{
				foreach (DictionaryEntry entry in table)
				{
					key = entry.Key;
					dependencies = (ArrayList)entry.Value;

					// No dependencies, add to start of table.
					if (dependencies.Count == 0)
					{
						if (!sortedList.Contains(key))
						{
							Console.WriteLine("Adding: (ND) " + key.ToString());
							sortedList.Insert(0, key);
						}
						continue;
					}

					bool all_dependencies_exist = false;
					int last_dependency = 0;

					foreach (object dependency in dependencies)
					{
						if (sortedList.Contains(dependency))
						{
							all_dependencies_exist = true;
							if (sortedList.IndexOf(dependency) > last_dependency)
							{
								last_dependency = sortedList.IndexOf(dependency);
							}
						}
						else
						{
							all_dependencies_exist = false;
							break;
						}
					}

					// All dependencies have been added, add object at location of last
					// dependency.
					if (all_dependencies_exist)
					{
						if (!sortedList.Contains(key))
						{
							Console.WriteLine("Adding: (D) " + key.ToString());
							sortedList.Add(key);
						}
					}
				}
			}

			return sortedList;
		}
	}
}
