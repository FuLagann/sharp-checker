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

`TypeInfo`:

```json
{
	// The unlocalized name that is used to find the type within the .NET library / application
	"unlocalizeName": "Dummy.DummyClass`1",
	// The name of the type in a more readable way
	"name": "DummyClass<T>",
	// The full name of the type (namespace + name)
	"fullName": "Dummy.DummyClass<T>",
	// The namespace of the type
	"namespaceName": "Dummy",
	// The name of the library / application
	"assemblyName": "DummyLibrary",
	// The access of the type (public or private)
	"accessor": "public",
	// The modifiers of the type (static, abstract, sealed, etc)
	"modifier": "abstract",
	// The object type of the type (class, struct, enum, or interface)
	"objectType": "class",
	// The declaration of the type as if found in the code
	"declaration": "public class DummyClass<T>",
	// The full declaration of the type as if found in the code
	"fullDeclaration": "public class DummyClass<T> : IDummy",
	// The base type of the type
	"baseType": "",
	// The list of names of the generic parameters
	"genericParameters": ["T"],
	// The list of interfaces found from the type
	"interfaces": [/* InterfaceInfo */],
	// The list of methods found from the type
	"methods": [/* MethodInfo */]
}
```
