/////////////////////////////////////////////////////////////////////////////
// <copyright file="ProgramTests.cs" company="James John McGuire">
// Copyright © 2006 - 2026 James John McGuire. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace DigitalZenWorks.MsAccessJetAceTool.Tests;

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Versioning;
using DigitalZenWorks.Common.Utilities;
using DigitalZenWorks.Database.ToolKit;
using global::MsAccessJetAceTool;
using NUnit.Framework;

/// <summary>
/// Program tests class.
/// </summary>
[TestFixture]
internal sealed class ProgramTests
{
	private string? originalCurrentDirectory;
	private string? testDirectory;
	private string? sourceDatabaseFile;
	private string? sourceSqlFile;

	/// <summary>
	/// Setup method for the test fixture.
	/// </summary>
	[SetUp]
	public void Setup()
	{
		originalCurrentDirectory = Directory.GetCurrentDirectory();

		string uniqueDirectory = "MsAccessJetAceToolTests" + Guid.NewGuid();
		testDirectory = Path.Combine(Path.GetTempPath(), uniqueDirectory);
		Directory.CreateDirectory(testDirectory);

		sourceDatabaseFile = GetTestAccdbFile();
		sourceSqlFile = GetTestSqlFile();
	}

	/// <summary>
	/// Tear down method for the test fixture.
	/// </summary>
	[TearDown]
	public void TearDown()
	{
		Directory.SetCurrentDirectory(originalCurrentDirectory!);

		try
		{
			if (Directory.Exists(testDirectory))
			{
				Directory.Delete(testDirectory, true);
			}
		}
		catch (IOException)
		{
			// File handle possibly still releasing from OLEDB/ACE;
			// not worth failing the test run over leftover temp files.
		}
	}

	/// <summary>
	/// Sanity check test to ensure that the testing framework is working.
	/// </summary>
	[Test]
	public void SanityCheck()
	{
		Assert.Pass();
	}

	/// <summary>
	/// Test that export continues to use Access DDL when no target option is
	/// supplied.
	/// </summary>
	[NonParallelizable]
	[Test]
	public void DefaultExportUsesAccessDdl()
	{
		Func<string, Collection<Table>> originalLoader =
			DataDefinitionOleDb.SchemaLoader;
		string outputSqlFile =
			Path.Combine(testDirectory!, "defaultExport.sql");

		try
		{
			Table table = new("Orders");
			Column id = new(
				"id", ColumnType.AutoNumber, 0, false, false, null, 1);
			id.Primary = true;
			table.AddColumn(id);
			table.AddColumn(new Column(
				"notes", ColumnType.Memo, 0, false, true, null, 2));

			Collection<Table> tables = [table];
			DataDefinitionOleDb.SchemaLoader = _ => tables;

			string[] args = { "export", "ignored.accdb", outputSqlFile };
			int returnCode = MsAccessTool.ProcessCommand(args);
			string ddl = File.ReadAllText(outputSqlFile);

			Assert.That(returnCode, Is.EqualTo(0));
			Assert.That(ddl, Does.Contain("CREATE TABLE [Orders]"));
			Assert.That(ddl, Does.Contain("[notes] MEMO"));
			Assert.That(ddl, Does.Contain(" IDENTITY"));
		}
		finally
		{
			DataDefinitionOleDb.SchemaLoader = originalLoader;
		}
	}

	/// <summary>
	/// Test that the export command creates a non-empty SQL file.
	/// </summary>
	[SupportedOSPlatform("windows")]
	[Test]
	public void ExportCreatesNonEmptySqlFile()
	{
		string outputSqlFile =
			Path.Combine(testDirectory!, "exportOutput.sql");
		File.Delete(outputSqlFile);

		string[] args = { "export", sourceDatabaseFile!, outputSqlFile };

		int returnCode = MsAccessTool.ProcessCommand(args);

		Assert.That(returnCode, Is.EqualTo(0));

		bool exists = File.Exists(outputSqlFile);
		Assert.That(exists, Is.True);

		string contents = File.ReadAllText(outputSqlFile);
		Assert.That(contents, Is.Not.Empty);
	}

	/// <summary>
	/// Test that exporting a database and then importing it back produces a
	/// valid database file.
	/// </summary>
	[SupportedOSPlatform("windows")]
	[Test]
	public void ExportThenImportRoundTripProducesDatabase()
	{
		string exportedSqlFile =
			Path.Combine(testDirectory!, "roundtripExport.sql");
		string reimportedDatabaseFile =
			Path.Combine(testDirectory!, "roundtripImport.accdb");

		File.Delete(exportedSqlFile);
		File.Delete(reimportedDatabaseFile);

		string[] exportArgs =
		{
			"export", sourceDatabaseFile!, exportedSqlFile
		};

		int exportReturnCode = MsAccessTool.ProcessCommand(exportArgs);

		string[] importArgs =
		{
			"import", exportedSqlFile, reimportedDatabaseFile
		};

		int importReturnCode = MsAccessTool.ProcessCommand(importArgs);

		Assert.That(exportReturnCode, Is.EqualTo(0));
		Assert.That(importReturnCode, Is.EqualTo(0));

		bool exists = File.Exists(reimportedDatabaseFile);
		Assert.That(exists, Is.True);
	}

	/// <summary>
	/// Test that exporting a database and then importing it back using bare
	/// file names produces a valid database file in the current directory.
	/// </summary>
	[SupportedOSPlatform("windows")]
	[Test]
	public void ExportThenImportRoundTripWithBareFileNames()
	{
		Directory.SetCurrentDirectory(testDirectory!);

		const string exportedSqlFile = "roundtripExport.sql";
		const string reimportedDatabaseFile = "roundtripImport.accdb";

		string[] exportArgs = { "export", "test.accdb", exportedSqlFile };
		int exportReturnCode = MsAccessTool.ProcessCommand(exportArgs);

		string[] importArgs =
		{
			"import", exportedSqlFile, reimportedDatabaseFile
		};

		int importReturnCode = MsAccessTool.ProcessCommand(importArgs);

		Assert.That(exportReturnCode, Is.EqualTo(0));
		Assert.That(importReturnCode, Is.EqualTo(0));

		string expectedFile =
			Path.Combine(testDirectory!, reimportedDatabaseFile);
		bool exists = File.Exists(expectedFile);
		Assert.That(exists, Is.True);
	}

	/// <summary>
	/// Test that exporting a database with bare file names uses the current
	/// directory for the output SQL file.
	/// </summary>
	[SupportedOSPlatform("windows")]
	[Test]
	public void ExportWithBareFileNamesUsesCurrentDirectory()
	{
		Directory.SetCurrentDirectory(testDirectory!);

		const string outputSqlFile = "exportOutput.sql";

		string[] args = { "export", "test.accdb", outputSqlFile };

		int returnCode = MsAccessTool.ProcessCommand(args);

		Assert.That(returnCode, Is.EqualTo(0));

		string expectedFile = Path.Combine(testDirectory!, outputSqlFile);
		bool exists = File.Exists(expectedFile);
		Assert.That(exists, Is.True);
	}

	/// <summary>
	/// Test that importing a SQL file creates a new database file.
	/// </summary>
	[SupportedOSPlatform("windows")]
	[Test]
	public void ImportCreatesDatabaseFile()
	{
		string outputDatabaseFile =
			Path.Combine(testDirectory!, "importOutput.accdb");
		File.Delete(outputDatabaseFile);

		string[] args = { "import", sourceSqlFile!, outputDatabaseFile };

		int returnCode = MsAccessTool.ProcessCommand(args);

		Assert.That(returnCode, Is.EqualTo(0));

		bool exists = File.Exists(outputDatabaseFile);
		Assert.That(exists, Is.True);
	}

	/// <summary>
	/// Test that importing a SQL file with bare file names uses the current
	/// directory for the output database file.
	/// </summary>
	[SupportedOSPlatform("windows")]
	[Test]
	public void ImportWithBareFileNamesUsesCurrentDirectory()
	{
		Directory.SetCurrentDirectory(testDirectory!);

		const string outputDatabaseFile = "importOutput.accdb";

		string[] args = { "import", "test.sql", outputDatabaseFile };

		int returnCode = MsAccessTool.ProcessCommand(args);
		Assert.That(returnCode, Is.EqualTo(0));

		string expectedFile = Path.Combine(testDirectory!, outputDatabaseFile);
		bool exists = File.Exists(expectedFile);
		Assert.That(exists, Is.True);
	}

	/// <summary>
	/// Test that no arguments returns a usage error code.
	/// </summary>
	[Test]
	public void NoArgumentsReturnsUsageErrorCode()
	{
		string[] args = Array.Empty<string>();

		int returnCode = MsAccessTool.ProcessCommand(args);

		Assert.That(returnCode, Is.EqualTo(-1));
	}

	/// <summary>
	/// Test that too few arguments returns a usage error code.
	/// </summary>
	[Test]
	public void TooFewArgumentsReturnsUsageErrorCode()
	{
		string[] args = { "export", sourceDatabaseFile! };

		int returnCode = MsAccessTool.ProcessCommand(args);

		Assert.That(returnCode, Is.EqualTo(-1));
	}

	/// <summary>
	/// Test that an unknown command returns a usage error code.
	/// </summary>
	[Test]
	public void UnknownCommandReturnsUsageErrorCode()
	{
		string[] args = { "frobnicate", "a", "b" };

		int returnCode = MsAccessTool.ProcessCommand(args);

		Assert.That(returnCode, Is.EqualTo(-1));
	}

	private static string GetEmbeddedResourceFile(
		string resource, string filePath)
	{
		bool result =
			FileUtils.CreateFileFromEmbeddedResource(resource, filePath);

		Assert.That(result, Is.True);

		result = File.Exists(filePath);
		Assert.That(result, Is.True);

		return filePath;
	}

	private string GetTestAccdbFile()
	{
		string databaseFile = Path.Combine(testDirectory!, "test.accdb");

		const string resource = "MsAccessJetAceTool.Tests.test.accdb";

		databaseFile = GetEmbeddedResourceFile(resource, databaseFile);

		return databaseFile;
	}

	private string GetTestSqlFile()
	{
		string sqlFile = Path.Combine(testDirectory!, "test.sql");

		const string resource = "MsAccessJetAceTool.Tests.test.sql";

		sqlFile = GetEmbeddedResourceFile(resource, sqlFile);

		return sqlFile;
	}
}
