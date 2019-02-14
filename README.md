# CodeFormatter

[![Build status](https://img.shields.io/appveyor/ci/hanabi1224/codeformatter/nuget.svg)](https://ci.appveyor.com/project/hanabi1224/codeformatter)
[![NuGet](https://buildstats.info/nuget/codeformatter)](https://www.nuget.org/packages/codeformatter)

CodeFormatter is a tool that uses Roslyn to automatically rewrite the source to
follow our coding styles, which are [documented here][corefx-coding-style].

## Nuget package Usage:
```xml
<ItemGroup>
  <PackageReference Include="Dotnet.CodeFormatter.BuildTask.Fork" Version="0.0.1">
   <PrivateAssets>all</PrivateAssets>
  </PackageReference>
</ItemGroup>
```

[corefx-coding-style]: https://github.com/dotnet/corefx/blob/master/Documentation/coding-guidelines/coding-style.md

## Prerequisites

In order to build or run this tool you will need to have Microsoft Build Tools
2015 installed.  This comes as a part of [Visual Studio 2015](https://www.visualstudio.com/downloads/download-visual-studio-vs).

## Installation

Download binaries from [GitHub Releases](https://github.com/dotnet/codeformatter/releases)

## Usage

In order get the usage, simply invoke the tool with no arguments:

```
$ .\CodeFormatter.exe
CodeFormatter <project or solution> [<rule types>] [/file:<filename>] [/nocopyright] [/c:<config1,config2> [/copyright:file]
    <rule types> - Rule types to use in addition to the default ones.
                   Use ConvertTests to convert MSTest tests to xUnit.
    <filename>   - Only apply changes to files with specified name.
    <configs>    - Additional preprocessor configurations the formatter
                   should run under.
    <copyright>  - Specifies file containing copyright header.
```

## Contributing

We follow the same contribution process that 
[corefx is using][corefx-contributing].

[corefx-contributing]: https://github.com/dotnet/corefx/wiki/Contributing
