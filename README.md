# CodeStyleAnalyzer
CodeStyleAnalyzer is a Roslyn analyzer and code fixer to check for coding style violations. The coding styles are [documented here][corefx-coding-style].

> Since the creation of this project, Visual Studio got support more advance [.editorconfig][editorconfig] support, which allows naming guidelines to be enforced on project level. You will need Visual Studio 2017 for this functionality.

Large part of the code is based on the [.NET CodeFormatter tool][codeformatter]

# Usage

In order to use the CodeStyleAnalyzer, simply add the following NuGet package to your project:

```
Install-Package CodeStyleAnalyzer
```

During compilation of your project, this analyzer will be run by the compiler and will give warning, errors or information about codestyle.

[corefx-coding-style]: http://github.com/dotnet/corefx/blob/master/Documentation/coding-guidelines/coding-style.md
[codeformatter]: https://github.com/dotnet/codeformatter
[editorconfig]: https://docs.microsoft.com/en-us/visualstudio/ide/editorconfig-naming-conventions
