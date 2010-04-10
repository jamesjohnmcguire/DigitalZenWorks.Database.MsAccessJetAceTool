using System;
using System.Collections;

namespace JetUtil
{
	/////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Represents a database table.
	/// </summary>
	/////////////////////////////////////////////////////////////////////////
	public class Table
	{
		/////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Represents a table name
		/// </summary>
		/////////////////////////////////////////////////////////////////////
		public string m_Name;

		/////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Represents the columns
		/// </summary>
		/////////////////////////////////////////////////////////////////////
		public Hashtable m_Columns;

		/////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Represents the foreign keys
		/// </summary>
		/////////////////////////////////////////////////////////////////////
		public ArrayList m_ForeignKeys;

		/////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Represents the primary key
		/// </summary>
		/////////////////////////////////////////////////////////////////////
		public string m_PrimaryKey;

		/////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Default constructor
		/// </summary>
		/////////////////////////////////////////////////////////////////////
		public Table()
		{
			m_Name = "Untitled";
			m_PrimaryKey = "";
			m_ForeignKeys = new System.Collections.ArrayList();
			m_Columns = new System.Collections.Hashtable();
		}

		/////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Constructor - with name
		/// </summary>
		/// <param name="name"></param>
		/////////////////////////////////////////////////////////////////////
		public Table(string name)
			: this()
		{
			m_Name = name;
		}

		/////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Add a column
		/// </summary>
		/// <param name="col"></param>
		/////////////////////////////////////////////////////////////////////
		public void AddColumn(Column col)
		{
			m_Columns.Add(col.Name, col);
		}

		/////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Adds a foreign key
		/// </summary>
		/// <param name="fk"></param>
		/////////////////////////////////////////////////////////////////////
		public void AddForeignKey(ForeignKey fk)
		{
			m_ForeignKeys.Add(fk);
		}

		public void DumpTable()
		{
			Console.WriteLine("Table: " + m_Name);
			foreach (DictionaryEntry col in m_Columns)
			{
				Console.WriteLine("   -" + ((Column)col.Value).Name);
			}

			Console.WriteLine("  PK = " + m_PrimaryKey);
			foreach (ForeignKey fk in m_ForeignKeys)
			{
				Console.WriteLine("    -FK:{0} {1} {2}", fk.m_Name, fk.m_ColumnName, fk.m_ParentTable);
			}
		}
	}
}
