/////////////////////////////////////////////////////////////////////////////
// <copyright file="SqlWriterSqliteTests.cs" company="Digital Zen Works">
// Copyright © 2006 - 2026 Digital Zen Works.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace DigitalZenWorks.Database.ToolKit.Tests;

using System;
using System.Text;
using Microsoft.Data.Sqlite;
using NUnit.Framework;

/// <summary>
/// Tests for SqlWriterSqlite: SQLite 3.x schema DDL generation.
/// </summary>
/// <remarks>
/// All tests operate on hand-built Table/Column/ForeignKey fixtures (see
/// SchemaFixtures) rather than a live Access/.accdb source, since
/// System.Data.OleDb is Windows-only and unavailable in the Linux
/// verification environment. This matches the task's actual scope: type
/// mapping and DDL generation from an in-memory schema representation,
/// not Access file reading.
/// </remarks>
internal sealed class SqlWriterSqliteTests
{
	private SqlWriterSqlite writer;

	[SetUp]
	public void Setup()
	{
		writer = new SqlWriterSqlite();
	}

	/// <summary>
	/// What: AutoNumber primary key columns are declared as
	/// "INTEGER PRIMARY KEY AUTOINCREMENT", the specific declaration
	/// SQLite requires to grant rowid-alias auto-increment behavior.
	/// How: generates DDL for the Addresses table's "id" column and
	/// checks the declaration text.
	/// Why: SQLite only grants auto-increment behaviour to a column
	/// declared exactly this way - a plausible-looking naive
	/// implementation could emit a bare "INTEGER" (or, following the
	/// base class's existing 9-case switch, no explicit AutoNumber
	/// mapping at all) and would still compile and run, but would
	/// silently lose auto-increment behaviour. Asserted at the
	/// behavioural/text level, not against SqlWriter's private
	/// internals, so it accepts any correct implementation.
	/// </summary>
	[Test]
	public void AutoNumberPrimaryKeyProducesIntegerPrimaryKeyAutoincrement()
	{
		string expectedContains = "\"id\" INTEGER PRIMARY KEY AUTOINCREMENT";
		Table table = SchemaFixtures.GetAddressesTable();

		string sql = writer.GetTableCreateStatement(table);

		Assert.That(sql, Does.Contain(expectedContains));
	}

	/// <summary>
	/// What: Currency columns are declared with NUMERIC affinity, not
	/// REAL.
	/// How: generates DDL for the Orders table's "price" column and
	/// checks the declaration text.
	/// Why: mapping Currency to REAL (a plausible naive choice, since
	/// Currency is numeric) introduces floating point rounding error on
	/// monetary values that was not present in the source data. This is
	/// exactly the kind of judgment call a naive one-affinity-fits-all
	/// implementation could get wrong while still producing syntactically
	/// valid SQLite.
	/// </summary>
	[Test]
	public void CurrencyColumnUsesNumericNotReal()
	{
		Table table = SchemaFixtures.GetOrdersTable();

		string sql = writer.GetTableCreateStatement(table);

		Assert.That(sql, Does.Contain("\"price\" NUMERIC"));
		Assert.That(sql, Does.Not.Contain("\"price\" REAL"));
	}

	/// <summary>
	/// What: every recognized ColumnType produces a non-empty type
	/// declaration - no column is silently left blank.
	/// How: generates DDL for every table in the full fixture schema and
	/// checks that no column line has an empty type (i.e. no
	/// `"columnName" ` immediately followed by a comma or closing paren).
	/// Why: the base SqlWriter.GetColumnTypeText only handles 9 of
	/// roughly 60 ColumnType values and silently returns an empty string
	/// for everything else. A correct SqlWriterSqlite override must
	/// handle every type the fixture schema exercises (AutoNumber,
	/// Number, Text, Currency, DateTime, YesNo, Ole) without falling
	/// through to that empty-string default.
	/// </summary>
	[Test]
	public void NoColumnProducesEmptyTypeDeclaration()
	{
		foreach (Table table in SchemaFixtures.GetFullSchema())
		{
			string sql = writer.GetTableCreateStatement(table);

			foreach (Column column in table.Columns.Values)
			{
				string errorMessage = $"Column '{column.Name}' on table " +
					$"'{table.Name}' produced an empty type " + "declaration.";

				string emptyTypeMarker = $"\"{column.Name}\" ,";
				string emptyTypeAtEnd = $"\"{column.Name}\"\n";

				Assert.That(
					sql,
					Does.Not.Contain(emptyTypeMarker),
					errorMessage);

				Assert.That(
					sql,
					Does.Not.Contain(emptyTypeAtEnd),
					errorMessage);
			}
		}
	}

	/// <summary>
	/// What: boolean-affinity columns (YesNo) are declared as INTEGER,
	/// since SQLite has no native boolean type.
	/// How: generates DDL for the Orders table's "isShipped" column and
	/// checks the declaration text.
	/// Why: distinguishes a correct mapping from an incorrect one that
	/// might, e.g., leave YesNo unhandled and falling through to the
	/// base class's existing (empty-string) default case.
	/// </summary>
	[Test]
	public void YesNoColumnUsesInteger()
	{
		Table table = SchemaFixtures.GetOrdersTable();

		string sql = writer.GetTableCreateStatement(table);

		Assert.That(sql, Does.Contain("\"isShipped\" INTEGER"));
	}

	/// <summary>
	/// What: columns are emitted in ordinal Position order, not
	/// Dictionary insertion/iteration order.
	/// How: builds the Products table (whose fixture columns are added
	/// in Position order already) and asserts the generated DDL lists
	/// "makerId" before "sectionId" before "seriesId" before "label",
	/// matching their assigned Position values, not just insertion
	/// order into the underlying Dictionary&lt;string, Column&gt;.
	/// Why: SqlWriter.GetOrdinalSortedColumns exists specifically
	/// because Table.Columns has no ordering guarantee; an
	/// implementation that iterates table.Columns.Values directly
	/// instead of sorting by Position could produce technically valid
	/// but incorrectly-ordered DDL.
	/// </summary>
	[Test]
	public void ColumnsAreOrderedByPosition()
	{
		Table table = SchemaFixtures.GetProductsTable();

		string sql = writer.GetTableCreateStatement(table);

		int makerIdIndex = sql.IndexOf("\"makerId\"", StringComparison.Ordinal);
		int sectionIdIndex =
			sql.IndexOf("\"sectionId\"", StringComparison.Ordinal);
		int seriesIdIndex =
			sql.IndexOf("\"seriesId\"", StringComparison.Ordinal);
		int labelIndex = sql.IndexOf("\"label\"", StringComparison.Ordinal);

		Assert.That(makerIdIndex, Is.LessThan(sectionIdIndex));
		Assert.That(sectionIdIndex, Is.LessThan(seriesIdIndex));
		Assert.That(seriesIdIndex, Is.LessThan(labelIndex));
	}

	/// <summary>
	/// What: the full fixture schema (all 8 tables, including foreign
	/// keys) generates DDL that is valid, executable SQLite 3.x syntax.
	/// How: generates a CREATE TABLE statement per table (in
	/// dependency-safe order, matching the reference .sql file), runs
	/// the concatenated script against a real in-memory SQLite
	/// connection via Microsoft.Data.Sqlite, and asserts no exception is
	/// thrown.
	/// Why: this is the end-to-end, discriminating check that the
	/// generated syntax is actually valid SQLite - not just plausible
	/// text - covering identifier quoting, primary key/autoincrement
	/// placement, and foreign key constraint syntax together, the way
	/// Acceptance criterion 1 in instructions.md requires. A naive
	/// implementation that gets any one part wrong (e.g. malformed FK
	/// syntax, an invalid type keyword) fails this test even if it
	/// happens to pass the narrower per-column tests above.
	/// </summary>
	[Test]
	public void FullSchemaExecutesAgainstRealSqliteConnection()
	{
		StringBuilder script = new();

		foreach (Table table in SchemaFixtures.GetFullSchema())
		{
			script.AppendLine(writer.GetTableCreateStatement(table));
			script.AppendLine();
		}

		using SqliteConnection connection = new("Data Source=:memory:");
		connection.Open();

		using SqliteCommand command = connection.CreateCommand();
#pragma warning disable CA2100
		command.CommandText = script.ToString();
#pragma warning restore CA2100

		Action action = () => command.ExecuteNonQuery();
		Assert.DoesNotThrow(action);
	}

	/// <summary>
	/// What: a column type with no reasonable SQLite equivalent
	/// (LookupWizard) is represented as BLOB rather than producing an
	/// empty or invalid type declaration.
	/// How: builds a single-column table using ColumnType.LookupWizard
	/// and checks the declaration text.
	/// Why: this is the ambiguous-type fallback case - the task requires
	/// such types not be "silently dropped or produce invalid/empty
	/// output" (instructions.md Requirement). A naive implementation
	/// might correctly handle common types but fall through to an empty
	/// default for anything unrecognized.
	/// </summary>
	[Test]
	public void AmbiguousTypeFallsBackToBlobNotEmpty()
	{
		Table table = new("Attachments");

		Column column =
			new("id", ColumnType.AutoNumber, 0, false, false, null, 1);
		column.Primary = true;

		table.AddColumn(column);

		column = new(
			"metadata",
			ColumnType.LookupWizard,
			0,
			false,
			true,
			null,
			2);
		table.AddColumn(column);

		string sql = writer.GetTableCreateStatement(table);

		Assert.That(sql, Does.Contain("\"metadata\" BLOB"));
	}
}
