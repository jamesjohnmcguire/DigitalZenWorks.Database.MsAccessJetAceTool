# MsAccessJetAceTool

A command line tool for exporting an MS Access database to a SQL file, and importing a SQL file back into an MS Access database.

## Why

MS Access has always been GUI-first, with little command line support — the opposite of most other databases. That's fine for one-off use, but it makes Access databases hard to work with the way modern development expects: no easy way to diff a schema change, review it in a pull request, or apply it as part of an automated build.

MsAccessJetAceTool exports an Access database's schema to a plain SQL text file, and imports it back — so an Access schema can be treated like any other piece of source code: versioned, diffed, reviewed, and scripted.

## Use Cases

- **Version control** — commit your schema as a `.sql` file and get real diffs on schema changes instead of an opaque binary `.accdb`.
- **Migration** — a scriptable first step when moving a legacy Access database to another format.
- **CI / automation** — build or provision an Access database from a checked-in schema file as part of a scripted pipeline, with no GUI interaction required.

## Installation

### Prerequisites

You'll need the Microsoft Access Database Engine (ACE OLEDB provider) installed, matching your system architecture (x86/x64). This is available as a free redistributable from Microsoft and does not require a full Access/Office installation.

### Download

Download the latest release from the [Releases](../../releases) page. The zip contains a stand-alone executable, published as a ReadyToRun single-file app — drop it into your working directory or somewhere on your `PATH`.

### Usage

```
MsAccessJetAceTool <command> <input> <output>
```

| Command | Usage | Description |
| --- | --- | --- |
| `export` | `export <ACCDB file> <SQL file>` | Export an ACCDB file's schema to a SQL file |
| `import` | `import <SQL file> <ACCDB file>` | Create/populate an ACCDB file from a SQL file |

### Example

```
MsAccessJetAceTool export Northwind.accdb Northwind.sql
git diff Northwind.sql
MsAccessJetAceTool import Northwind.sql Northwind-rebuilt.accdb
```

## Contributing

If you have found a bug or have a suggestion that would make this better, please fork this repository and create a pull request. You can also simply open an issue with the tag "bug" or "enhancement".

### Process

1. Fork the Project
2. Create your Bug / Feature Branch (`git checkout -b feature/amazing-feature`)
3. Commit your Changes (`git commit -m 'Add some amazing feature'`)
4. Push to the Branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Coding style

Please match the existing coding style. Most notably:

1. One operation per line
2. Use complete English words in variable and method names
3. Declare variable and method names in a self-documenting manner
4. Add unit tests for new functionality

## License

Distributed under the MIT License. See `LICENSE` for more information.

## Contact

James John McGuire - jamesjohnmcguire@gmail.com

[LinkedIn](https://www.linkedin.com/in/jamesjohnmcguire) - [GitHub](https://github.com/jamesjohnmcguire)

Project Link: [MsAccessJetAceTool](https://github.com/jamesjohnmcguire/DigitalZenWorks.Database.MsAccessJetAceTool)
