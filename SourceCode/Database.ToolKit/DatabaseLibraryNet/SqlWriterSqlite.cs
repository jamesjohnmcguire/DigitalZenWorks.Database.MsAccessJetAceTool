/////////////////////////////////////////////////////////////////////////////
// <copyright file="SqlWriterSqlite.cs" company="Digital Zen Works">
// Copyright © 2006 - 2026 Digital Zen Works.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace DigitalZenWorks.Database.ToolKit;

using System;

/// <summary>
/// SQL writer helper for SQLite 3.x class.
/// </summary>
/// <remarks>
/// SQLite uses a small set of type affinities (TEXT, NUMERIC, INTEGER,
/// REAL, BLOB) rather than the wide, precise type systems of Access,
/// MySQL, or SQL Server. This class maps every <see cref="ColumnType"/>
/// value onto the closest matching affinity. The base <see cref="SqlWriter"/>
/// class already produces SQLite-compatible structural output (double
/// quoted identifiers, inline PRIMARY KEY AUTOINCREMENT), so this class
/// only needs to override the type-mapping step.
/// </remarks>
public class SqlWriterSqlite : SqlWriter
{
	/// <summary>
	/// Returns the SQLite type affinity string corresponding to the
	/// specified column's <see cref="ColumnType"/>.
	/// </summary>
	/// <remarks>
	/// Two mappings here are deliberate judgment calls rather than
	/// obvious translations:
	/// <list type="bullet">
	/// <item><description><see cref="ColumnType.Currency"/>,
	/// <see cref="ColumnType.Money"/>, <see cref="ColumnType.SmallMoney"/>,
	/// <see cref="ColumnType.Decimal"/>, and <see cref="ColumnType.Numeric"/>
	/// map to NUMERIC rather than REAL, to avoid floating point rounding
	/// on fixed-point/money values.</description></item>
	/// <item><description><see cref="ColumnType.AutoNumber"/> and
	/// <see cref="ColumnType.Identity"/> map to INTEGER; combined with the
	/// base class's existing "PRIMARY KEY AUTOINCREMENT" clause (added
	/// separately, based on <c>column.Primary</c>), this produces SQLite's
	/// rowid-alias auto-increment behavior.</description></item>
	/// </list>
	/// <see cref="ColumnType.SqlVariant"/> and
	/// <see cref="ColumnType.LookupWizard"/> have no reasonable SQLite
	/// equivalent and are stored opaquely as BLOB rather than guessed at.
	/// </remarks>
	/// <param name="column">The column for which to generate the SQL type
	/// declaration.</param>
	/// <returns>A string representing the SQLite type affinity for the
	/// column.</returns>
	protected override string GetColumnTypeText(Column column)
	{
#if NET6_0_OR_GREATER
		System.ArgumentNullException.ThrowIfNull(column);
#else
		if (column == null)
		{
			string name = nameof(column);
			throw new System.ArgumentNullException(name);
		}
#endif

		string columnType = column.ColumnType switch
		{
			// Text affinity - character/string types.
			ColumnType.Char => " TEXT",
			ColumnType.NChar => " TEXT",
			ColumnType.VarChar => " TEXT",
			ColumnType.NVarChar => " TEXT",
			ColumnType.LongVarChar => " TEXT",
			ColumnType.String => " TEXT",
			ColumnType.Text => " TEXT",
			ColumnType.Memo => " TEXT",
			ColumnType.LongText => " TEXT",
			ColumnType.MediumText => " TEXT",
			ColumnType.TinyText => " TEXT",
			ColumnType.Hyperlink => " TEXT",
			ColumnType.Xml => " TEXT",
			ColumnType.Enum => " TEXT",
			ColumnType.Set => " TEXT",
			ColumnType.UniqueIdentifier => " TEXT",

			// Text affinity - date/time types, stored as ISO-8601 text
			// per SQLite's own recommended date/time convention.
			ColumnType.DateTime => " TEXT",
			ColumnType.Date => " TEXT",
			ColumnType.Time => " TEXT",
			ColumnType.Timestamp => " TEXT",
			ColumnType.SmallDateTime => " TEXT",
			ColumnType.DateTime2 => " TEXT",
			ColumnType.DateTimeOffset => " TEXT",
			ColumnType.Year => " TEXT",

			// Integer affinity - whole-number types.
			ColumnType.AutoNumber => " INTEGER",
			ColumnType.Identity => " INTEGER",
			ColumnType.BigInt => " INTEGER",
			ColumnType.Int => " INTEGER",
			ColumnType.Integer => " INTEGER",
			ColumnType.SmallInt => " INTEGER",
			ColumnType.TinyInt => " INTEGER",
			ColumnType.MediumInt => " INTEGER",
			ColumnType.Long => " INTEGER",
			ColumnType.Byte => " INTEGER",
			ColumnType.Number => " INTEGER",

			// Integer affinity - boolean types (SQLite has no native
			// boolean; stored as 0/1).
			ColumnType.Boolean => " INTEGER",
			ColumnType.YesNo => " INTEGER",
			ColumnType.Bit => " INTEGER",

			// Real affinity - floating point types.
			ColumnType.Float => " REAL",
			ColumnType.Double => " REAL",
			ColumnType.Single => " REAL",
			ColumnType.Real => " REAL",

			// Numeric affinity - fixed point/money types, kept off REAL
			// to avoid floating point rounding on precise values.
			ColumnType.Decimal => " NUMERIC",
			ColumnType.Numeric => " NUMERIC",
			ColumnType.Currency => " NUMERIC",
			ColumnType.Money => " NUMERIC",
			ColumnType.SmallMoney => " NUMERIC",

			// Blob affinity - binary/opaque types.
			ColumnType.Binary => " BLOB",
			ColumnType.VarBinary => " BLOB",
			ColumnType.LongVarBinary => " BLOB",
			ColumnType.Blob => " BLOB",
			ColumnType.LongBlob => " BLOB",
			ColumnType.MediumBlob => " BLOB",
			ColumnType.Image => " BLOB",
			ColumnType.Ole => " BLOB",
			ColumnType.OleObject => " BLOB",
			ColumnType.JavaObject => " BLOB",

			// No reasonable SQLite equivalent - stored opaquely rather
			// than guessed at.
			ColumnType.SqlVariant => " BLOB",
			ColumnType.LookupWizard => " BLOB",

			// Not real persisted column types in practice; safe non-empty
			// fallback so this switch never silently degrades to blank
			// output the way the base implementation does today.
			ColumnType.Table => " TEXT",
			ColumnType.Cursor => " TEXT",
			ColumnType.Unknown => " TEXT",
			ColumnType.Other => " TEXT",

			_ => " TEXT",
		};

		return columnType;
	}
}
