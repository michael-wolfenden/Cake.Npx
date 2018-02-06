# Cake.Npx

[![MIT License](https://img.shields.io/npm/l/starwars-names.svg?style=flat-square)](http://opensource.org/licenses/MIT)
[![semantic-release](https://img.shields.io/badge/%20%20%F0%9F%93%A6%F0%9F%9A%80-semantic--release-e10079.svg?style=flat-square)](https://github.com/semantic-release/semantic-release)

Cake alias for executing [npx](https://medium.com/@maybekatz/introducing-npx-an-npm-package-runner-55f7d4bd282b) commands.

## Installation

In your cake script add the following line (replacing version with the version you want to pin to)

```
#addin "nuget:https://api.nuget.org/v3/index.json?package=Cake.Npx&version=1.0.0"
```

## Usage

```csharp
// Executes npx with the specified command.
Npx("semantic-release");

// Executes npx with the specified command and captures the standard output.
string[] redirectedStandardOutput = null;

Npx("semantic-release", out redirectedStandardOutput);

// Executes npx with the specified command and additional arguments.
Npx("semantic-release", "--dry-run");

Npx("semantic-release", args => args.Append("--dry-run"));

// Executes npx with the specified command and additional arguments and captures the standard output.
string[] redirectedStandardOutput = null;

Npx("semantic-release",
    "--dry-run",
    out redirectedStandardOutput);

Npx("semantic-release",
    args => args.Append("--dry-run"),
    out redirectedStandardOutput);

// Executes npx with the specified command and additional packages.
Npx("semantic-release",
    settings => settings.AddPackage("@semantic-release/git"));

// Executes npx with the specified command and additional packages and captures the standard output.
string[] redirectedStandardOutput = null;

Npx("semantic-release",
    settings => settings.AddPackage("@semantic-release/git"),
    out redirectedStandardOutput);

// Executes npx with the specified command, additional arguments and packages.
Npx("semantic-release",
    "--dry-run",
    settings => settings.AddPackage("@semantic-release/git"));

Npx("semantic-release",
    args => args.Append("--dry-run"),
    settings => settings.AddPackage("@semantic-release/git"));

// Executes npx with the specified command, additional arguments and packages and captures the standard output.
string[] redirectedStandardOutput = null;

Npx("semantic-release",
    "--dry-run",
    settings => settings.AddPackage("@semantic-release/git"),
    out redirectedStandardOutput);

Npx("semantic-release",
    args => args.Append("--dry-run"),
    settings => settings.AddPackage("@semantic-release/git"),
    out redirectedStandardOutput);
```

