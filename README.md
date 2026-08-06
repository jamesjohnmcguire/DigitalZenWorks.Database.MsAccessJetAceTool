# MsAccessJetAceTool

A command line tool to export to an SQL file from an MS Access database and vica-versa.

This tools exists for the purpose of making importing and exporting from MS Access databases from the command line easy and scriptable.

MS Access has been traditionally very GUI oriented with little command line support.  This has made a bit of an outsider compared to other databases, which have almost no GUI support.  Both have their advantages.  But for continual development and maintenance of databases, command line tools are much more helpful as they can be automated and included in build scripts.

## Possible Uses

no way to export schema
no way to import from schema file

## Possible Uses
Migration Support - First step of converting to another database format.
Version-control - Keep your schema in version control
Use in Automated or CI environments - Use this in build or automation scripts.

## Installation

### Prerequisites

MS Access (Part of Office) needs to be installed on the same computer as this program.  You will also need to install a driver matching your version of Office Links change often but searching for the latest should be easy.

### Download

You can download the latest release from the Releases page.  The zip file, while containing a few support files, mainly contains a stand-alone exe, published as a ReadyToRun/single-file app for easy drop-in in your working directory or in your PATH.

### Usage:

MsAccessJetAceTool \<command\> \<input\> \<output\>

| Commands:                          |                                     |
| ---------------------------------- | ----------------------------------- |
| export \<ACCDB file\> \<SQL file\> | Export a ACCDB file to a SQL file   |
| import \<SQL file\> \<ACCDB file\> | Import a ACCDB file from a SQL file |

## Contributing

If you have found a bug or have a suggestion that would make this better, please fork this repository and create a pull request. You can also simply open an issue with the tag "bug" or "enhancement".

### Process:

1. Fork the Project
2. Create your Bug / Feature Branch (`git checkout -b feature/amazing-feature`)
3. Commit your Changes (`git commit -m 'Add some amazing feature'`)
4. Push to the Branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Coding style
Please match the current coding style.  Most notably:
1. One operation per line
2. Use complete English words in variable and method names
3. Attempt to declare variable and method names in a self-documenting manner
4. Add unit tests


## License

Distributed under the MIT License. See `LICENSE` for more information.

## Contact

James John McGuire - jamesjohnmcguire@gmail.com [LinkedIn](https://twitter.com/https://www.linkedin.com/in/jamesjohnmcguire) -  [GitHub](https://github.com/jamesjohnmcguire)

Project Link: [Email.ToolKit](https://github.com/jamesjohnmcguire/DigitalZenWorks.Database.MsAccessJetAceTool)
