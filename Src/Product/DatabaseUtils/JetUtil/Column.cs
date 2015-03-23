using System;
using System.Collections.Generic;
using System.Text;

namespace JetUtil
{
	/////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Represents a database column.
	/// </summary>
	/////////////////////////////////////////////////////////////////////////
	public class Column
	{
		/////////////////////////////////////////////////////////////////////
		/// <summary>
		/// The name of the column
		/// </summary>
		/////////////////////////////////////////////////////////////////////
		public string Name;

		/////////////////////////////////////////////////////////////////////
		/// <summary>
		/// The type of the column
		/// </summary>
		/////////////////////////////////////////////////////////////////////
		public int ColumnType;

		/////////////////////////////////////////////////////////////////////
		/// <summary>
		/// The length of the column
		/// </summary>
		/////////////////////////////////////////////////////////////////////
		public int Length;

		/////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Represents whether the column requires unique values
		/// </summary>
		/////////////////////////////////////////////////////////////////////
		public bool Unique;

		/////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Represents whether the column can have a null value
		/// </summary>
		/////////////////////////////////////////////////////////////////////
		public bool Nullable;

		/////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Represents the defaul value of the column
		/// </summary>
		/////////////////////////////////////////////////////////////////////
		public string DefaultValue;

		/////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Represents the position of the column
		/// </summary>
		/////////////////////////////////////////////////////////////////////
		public int Position;

		/////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Default Constructor
		/// </summary>
		/////////////////////////////////////////////////////////////////////
		public Column()
		{
			Name = "Untitled Column";
			ColumnType = (int)ColumnTypes.Number;
			Length = 255;
			Unique = false;
			Nullable = false;
			DefaultValue = "";
			Position = 1;
		}

		/////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Constructor
		/// </summary>
		/////////////////////////////////////////////////////////////////////
		public Column(string name, int type, int length, bool unique,
			bool nullable, string defaultvalue, int position)
		{
			Name = name;
			ColumnType = type;
			Unique = unique;
			Nullable = nullable;
			DefaultValue = defaultvalue;
			Position = position;
		}
	}
}
