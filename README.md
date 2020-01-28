# CodeStyleAnalyzer
CodeStyleAnalyzer is a Roslyn analyzer and code fixer to check for coding style violations. The coding styles are [documented here][corefx-coding-style].

Large part of the code is based on the [.NET CodeFormatter tool][codeformatter]

# Maintainance status
Since the creation of this project, Visual Studio got support more advance [.editorconfig][editorconfig] support, which allows naming guidelines to be enforced on project level. You will need Visual Studio 2017 for this functionality.
Because of this I didn't work on this project anymore and didn't use it myself and no issues were reported. The latest NuGet package will be available and if necesary, I am open for PR's to fix issues.

# Usage

In order to use the CodeStyleAnalyzer, simply add the following NuGet package to your project:

```
Install-Package CodeStyleAnalyzer
```

During compilation of your project, this analyzer will be run by the compiler and will give warning, errors or information about codestyle.

[corefx-coding-style]: http://github.com/dotnet/corefx/blob/master/Documentation/coding-guidelines/coding-style.md
[codeformatter]: https://github.com/dotnet/codeformatter
[editorconfig]: https://docs.microsoft.com/en-us/visualstudio/ide/editorconfig-naming-conventions
