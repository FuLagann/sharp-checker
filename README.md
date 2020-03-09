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

## Format

The returning json will be of the of the following formats (with `TypeInfo` being the main type):

### QuickTypeInfo

TODO: Add formatting

### TypeInfo

TODO: Add formatting

### AttributeInfo

All the information relevant to an attribute.

**`typeInfo` as ([QuickTypeInfo](#quicktypeinfo)):** The information of the type
that the attribute is.

**`constructorArgs` as ([AttributeFieldInfo](#attributefieldinfo)):** The list of
constructor arguments that the attribute is declaring.

**`properties` as ([AttributeFieldInfo](#attributefieldinfo)):** The list of fields and properties that the attribute is declaring.

**`parameterDeclaration` as (string):** The declaration of parameters as seen if
looking at the code.

**`fullDeclaration` as (string):** The declaration of the attribute as a whole, with name and parameters as seen if looking at the code.

Example JSON:

```json
{
	"typeInfo": {
		"unlocalizedName": "Dummy.DummyAttribute",
		"name": "DummyAttribute",
		"fullName": "Dummy.DummyAttribute",
		"namespaceName": "Dummy",
		"genericParameters": []
	},
	"constructorArgs": [
		{
			"name": "hash1",
			"value": "Hello",
			"typeInfo": {
				"unlocalizedName": "System.String",
				"name": "string",
				"fullName": "System.String",
				"namespaceName": "System",
				"genericParameters": []
			}
		},
		{
			"name": "hash2",
			"value": "World",
			"typeInfo": {
				"unlocalizedName": "System.String",
				"name": "string",
				"fullName": "System.String",
				"namespaceName": "System",
				"genericParameters": []
			}
		}
	],
	"properties": [
		{
			"name": "val",
			"value": "Testing",
			"typeInfo": {
				"unlocalizedName": "System.String",
				"name": "string",
				"fullName": "System.String",
				"namespaceName": "System",
				"genericParameters": []
			}
		},
		{
			"name": "HasValue",
			"value": "true",
			"typeInfo": {
				"unlocalizedName": "System.Boolean",
				"name": "bool",
				"fullName": "System.Boolean",
				"namespaceName": "System",
				"genericParameters": []
			}
		}
	],
	"parameterDeclaration": "\"Hello\", \"World\", val = \"Testing\", HasValue = true",
	"fullDeclaration": "[Dummy.DummyAttribute(\"Hello\", \"World\", val = \"Testing\", HasValue = true)]"
}
```

### InterfaceInfo

TODO: Add formatting

### FieldInfo

TODO: Add formatting

### MethodInfo

TODO: Add formatting

### ParameterInfo

TODO: Add formatting

### AttributeFieldInfo

All the information relevant to the attribute's fields.

**`name` as (string):** The name of the attribute field.

**`value` as (string):** The value of the attribute field.

**`typeInfo` as ([QuickTypeInfo](#quicktypeinfo)):** The information of the attribute field's type.

Example JSON:

```json
{
	"name": "isMod",
	"value": "false",
	"typeInfo": {
		"unlocalizedName": "System.Boolean",
		"name": "bool",
		"fullName": "System.Boolean",
		"namespaceName": "System",
		"genericParameters": []
	}
}
```

### Example JSON Output

```json

```
