/////////////////////////////////////////////////////////////////////////////
// <copyright file="ProgramTests.cs" company="James John McGuire">
// Copyright © 2006 - 2026 James John McGuire. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace DigitalZenWorks.MsAccessJetAceTool.Tests;

using DigitalZenWorks.Common.Utilities;
using global::MsAccessJetAceTool;
using NUnit.Framework;
using System;
using System.IO;

[TestFixture]
internal class ProgramTests
{
	private string originalCurrentDirectory;
	private string testDirectory;
	private string sourceDatabaseFile;
	private string sourceSqlFile;

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

	[TearDown]
	public void TearDown()
	{
		Directory.SetCurrentDirectory(originalCurrentDirectory);

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

	[Test]
	public void SanityCheck()
	{
		Assert.Pass();
	}

	[Test]
	public void ExportCreatesNonEmptySqlFile()
	{
		string outputSqlFile =
			Path.Combine(testDirectory, "exportOutput.sql");
		File.Delete(outputSqlFile);

		string[] args = { "export", sourceDatabaseFile, outputSqlFile };

		int returnCode = MsAccessTool.Main(args);

		Assert.That(returnCode, Is.EqualTo(0));

		bool exists = File.Exists(outputSqlFile);
		Assert.That(exists, Is.True);

		string contents = File.ReadAllText(outputSqlFile);
		Assert.That(contents, Is.Not.Empty);
	}

	[Test]
	public void ExportThenImportRoundTripProducesDatabase()
	{
		string exportedSqlFile =
			Path.Combine(testDirectory, "roundtripExport.sql");
		string reimportedDatabaseFile =
			Path.Combine(testDirectory, "roundtripImport.accdb");

		File.Delete(exportedSqlFile);
		File.Delete(reimportedDatabaseFile);

		string[] exportArgs = { "export", sourceDatabaseFile, exportedSqlFile };
		int exportReturnCode = MsAccessTool.Main(exportArgs);

		string[] importArgs =
			{ "import", exportedSqlFile, reimportedDatabaseFile };
		int importReturnCode = MsAccessTool.Main(importArgs);

		Assert.That(exportReturnCode, Is.EqualTo(0));
		Assert.That(importReturnCode, Is.EqualTo(0));

		bool exists = File.Exists(reimportedDatabaseFile);
		Assert.That(exists, Is.True);
	}

	[Test]
	public void ExportThenImportRoundTripWithBareFileNames()
	{
		Directory.SetCurrentDirectory(testDirectory);

		const string exportedSqlFile = "roundtripExport.sql";
		const string reimportedDatabaseFile = "roundtripImport.accdb";

		string[] exportArgs = { "export", "test.accdb", exportedSqlFile };
		int exportReturnCode = MsAccessTool.Main(exportArgs);

		string[] importArgs =
			{ "import", exportedSqlFile, reimportedDatabaseFile };
		int importReturnCode = MsAccessTool.Main(importArgs);

		Assert.That(exportReturnCode, Is.EqualTo(0));
		Assert.That(importReturnCode, Is.EqualTo(0));

		string expectedFile =
			Path.Combine(testDirectory, reimportedDatabaseFile);
		bool exists = File.Exists(expectedFile);
		Assert.That(exists, Is.True);
	}

	[Test]
	public void ExportWithBareFileNamesUsesCurrentDirectory()
	{
		Directory.SetCurrentDirectory(testDirectory);

		const string outputSqlFile = "exportOutput.sql";

		string[] args = { "export", "test.accdb", outputSqlFile };

		int returnCode = MsAccessTool.Main(args);

		Assert.That(returnCode, Is.EqualTo(0));

		string expectedFile = Path.Combine(testDirectory, outputSqlFile);
		bool exists = File.Exists(expectedFile);
		Assert.That(exists, Is.True);
	}

	[Test]
	public void ImportCreatesDatabaseFile()
	{
		string outputDatabaseFile =
			Path.Combine(testDirectory, "importOutput.accdb");
		File.Delete(outputDatabaseFile);

		string[] args = { "import", sourceSqlFile, outputDatabaseFile };

		int returnCode = MsAccessTool.Main(args);

		Assert.That(returnCode, Is.EqualTo(0));

		bool exists = File.Exists(outputDatabaseFile);
		Assert.That(exists, Is.True);
	}

	[Test]
	public void ImportWithBareFileNamesUsesCurrentDirectory()
	{
		Directory.SetCurrentDirectory(testDirectory);

		const string outputDatabaseFile = "importOutput.accdb";

		string[] args = { "import", "test.sql", outputDatabaseFile };

		int returnCode = MsAccessTool.Main(args);
		Assert.That(returnCode, Is.EqualTo(0));

		string expectedFile = Path.Combine(testDirectory, outputDatabaseFile);
		bool exists = File.Exists(expectedFile);
		Assert.That(exists, Is.True);
	}

	[Test]
	public void NoArgumentsReturnsUsageErrorCode()
	{
		string[] args = Array.Empty<string>();

		int returnCode = MsAccessTool.Main(args);

		Assert.That(returnCode, Is.EqualTo(-1));
	}

	[Test]
	public void TooFewArgumentsReturnsUsageErrorCode()
	{
		string[] args = { "export", sourceDatabaseFile };

		int returnCode = MsAccessTool.Main(args);

		Assert.That(returnCode, Is.EqualTo(-1));
	}

	[Test]
	public void UnknownCommandReturnsUsageErrorCode()
	{
		string[] args = { "frobnicate", "a", "b" };

		int returnCode = MsAccessTool.Main(args);

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
		string databaseFile = Path.Combine(testDirectory, "test.accdb");

		const string resource = "MsAccessJetAceTool.Tests.test.accdb";

		databaseFile = GetEmbeddedResourceFile(resource, databaseFile);

		return databaseFile;
	}

	private string GetTestSqlFile()
	{
		string sqlFile = Path.Combine(testDirectory, "test.sql");

		const string resource = "MsAccessJetAceTool.Tests.test.sql";

		sqlFile = GetEmbeddedResourceFile(resource, sqlFile);

		return sqlFile;
	}
}
