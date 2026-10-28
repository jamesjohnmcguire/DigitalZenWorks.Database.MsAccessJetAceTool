# Repository Guidelines

## Project Structure & Module Organization

`SourceCode/MsAccessJetAceTool/` contains the Windows command line application. `SourceCode/MsAccessJetAceTool.Tests/` contains NUnit tests and the `test.accdb` and `test.sql` fixtures. `SourceCode/Database.ToolKit/` is a Git submodule that supplies database code; initialize it before building. The solution is `SourceCode/MsAccessJetAceTool.slnx`. `DevelopmentTools/Build.cmd` publishes the release executable, while `Support/` holds project notes.

## Build, Test, and Development Commands

Run these from the repository root on Windows with the .NET 10 SDK:

```powershell
git submodule update --init --recursive
dotnet build SourceCode/MsAccessJetAceTool.slnx
dotnet test SourceCode/MsAccessJetAceTool.slnx
dotnet run --project SourceCode/MsAccessJetAceTool -- export input.accdb output.sql
```

The first command fetches the database dependency; the next two build and test the solution. The final command runs an export locally. Import uses `import input.sql output.accdb`. Database integration tests require a matching architecture Microsoft ACE OLEDB provider. `DevelopmentTools/Build.cmd` publishes a self-contained Windows x64 release; its `release` argument also packages and publishes a GitHub release, so use that mode only for an intended release.

## Coding Style & Naming Conventions

Follow `.editorconfig`: tabs with width four, UTF-8, final newlines, and an 80-column guide. C# and project files use LF; `.cmd` files use CRLF. Use descriptive English names, PascalCase for types and methods, and one operation per line. Preserve the existing XML documentation style. .NET analyzers, Roslynator, and StyleCop run during builds; address new warnings before submitting.

## Testing Guidelines

Add NUnit `[Test]` methods in `*Tests.cs` for behavior changes, using names that describe the expected result (for example, `NoArgumentsReturnsUsageErrorCode`). Keep fixture work isolated in temporary directories and restore process-wide state in teardown or `finally`. Run `dotnet test SourceCode/MsAccessJetAceTool.slnx` before a pull request. No coverage threshold is configured.

## Commits & Pull Requests

Recent commits use short, imperative, title-case subjects such as `Update Packages` and `Remove Warnings`. Keep each commit focused. For pull requests, describe the behavior changed, list test results, link the relevant issue when one exists, and include sample command output for CLI changes. Do not include generated release files or incidental submodule updates.
