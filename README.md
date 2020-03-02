# SharpChecker

SharpChecker is a CLI program that looks into .NET libraries (dll) and
applications (exe) to check on information about specific types as the
user listed. It will also save all the information as a JSON file, normally
saved as `type.json` from where it was ran. Information on how to use can
be found under the [Usage section](#usage).

**NOTE:** Since it's a CLI and types that use generic parameters, types that
use generic parameters are found using "`". Instead you can replace that
character with -, since the original character can mess up running on the terminal.

## Build Status

Builds and tests are done as
[GitHub actions](https://github.com/FuLagann/sharp-checker/actions).

| Status | Description |
|:-------|-------------|
| ![.NET Test](https://github.com/FuLagann/sharp-checker/workflows/.NET%20Test/badge.svg) | .NET build and test for all platforms in .NET Core 3.0 in the master branch |

## Usage

To use the application, the basics usage is to run it with the full path of
the type and a following list of libraries to look into.

Basic outline:

```
SharpChecker <type-path> <list-of-assemblies>
```

Example usage:

```bat
SharpChecker Dummy.DummyClass-1 DummyLibrary.dll SecondDummyLibrary.dll
```

```bat
SharpChecker Newtonsoft.Json.JsonConvert Newtonsoft.Json.dll
```

The returning `type.json` returns a json of the following formats:

`TypeInfo`: TODO: Add formatting

`InterfaceInfo`: TODO: Add formatting

`MethodInfo`: TODO: Add formatting

`ParameterInfo`: TODO: Add formatting

Example:

```json
{
	"unlocalizeName": "Dummy.DummyClass`1",
	"name": "DummyClass<T>",
	"fullName": "Dummy.DummyClass<T>",
	"namespaceName": "Dummy",
	"assemblyName": "DummyLibrary",
	"accessor": "public",
	"modifier": "abstract",
	"objectType": "class",
	"declaration": "public class DummyClass<T>",
	"fullDeclaration": "public class DummyClass<T> : IDummy",
	"baseType": "",
	"genericParameters": ["T"],
	"interfaces": [],
	"methods": []
}
```
