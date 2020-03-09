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

### TypeInfo

TODO: Add formatting

### QuickTypeInfo

A quick look into the information of the type.

**`unlocalizedName` as (string):** The name of the type as found within the library's IL code.

**`name` as (string):** The name of the type as found when looking at the code.

**`fullName` as (string):** The full name of the type as found when looking at the code. Includes the namespace and the name within this variable.

**`namespaceName` as (string):** The name of the namespace where the type is located in.

**`genericParameters` as ([GenericParametersInfo](#genericparametersinfo)[]):** The list of generic parameters that the type contains.

Example JSON:

```json
{
  "unlocalizedName": "Dummy.DummyClass`1",
  "name": "DummyClass<T>",
  "fullName": "Dummy.DummyClass<T>",
  "namespaceName": "Dummy",
  "genericParameters": [
    {
      "unlocalizedName": "T",
      "name": "T",
      "constraints": []
    }
  ]
}
```

### AttributeInfo

All the information relevant to an attribute.

**`typeInfo` as ([QuickTypeInfo](#quicktypeinfo)):** The information of the type
that the attribute is.

**`constructorArgs` as ([AttributeFieldInfo](#attributefieldinfo)[]):** The list of
constructor arguments that the attribute is declaring.

**`properties` as ([AttributeFieldInfo](#attributefieldinfo)[]):** The list of fields and properties that the attribute is declaring.

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

### FieldInfo

All the information relevant to fields.

**`name` as (string):** The name of the field.

**`value` as (string):** The value of the field (if it's a constant).

**`isConstant` as (boolean):** Set to true if the field is constant.

**`isStatic` as (boolean):** Set to true if the field is static.

**`isReadonly` as (boolean):** Set to true if the field is readonly.

**`attributes` as ([AttributeInfo](#attributeinfo)[]):** The list of attributes that the field contains.

**`accessor` as (string):** The accessor of the field (such as internal, private, protected, public).

**`modifier` as (string):** Any modifiers to the field (such as static, const, static readonly, etc).

**`typeInfo` as ([QuickTypeInfo](#quicktypeinfo)):** The type information of the field's type.

**`declaration` as (string):** The declaration of the field as it is found witihn the code.

Example JSON:

```json
{
  "name": "grid",
  "value": "",
  "isConstant": false,
  "isStatic": false,
  "isReadonly": false,
  "attributes": [
    {
      "typeInfo": {
        "unlocalizedName": "System.ObsoleteAttribute",
        "name": "ObsoleteAttribute",
        "fullName": "System.ObsoleteAttribute",
        "namespaceName": "System",
        "genericParameters": []
      },
      "constructorArgs": [],
      "properties": [],
      "parameterDeclaration": "",
      "fullDeclaration": "[System.ObsoleteAttribute]"
    }
  ],
  "accessor": "public",
  "modifier": "",
  "typeInfo": {
    "unlocalizedName": "System.Object[][]",
    "name": "object[][]",
    "fullName": "System.Object[][]",
    "namespaceName": "System",
    "genericParameters": []
  },
  "declaration": "public object[][] grid"
}
```

### MethodInfo

TODO: Add formatting

### ParameterInfo

All the information relevant to parameters.

**`name` as (string):** The name of the parameter.

**`defaultValue` as (string):** The default value of the parameter (if it exists).

**`attributes` as ([AttributeInfo](#attributeinfo)[]):** The list of attributes that the parameter contains.

**`modifier` as (string):** Any modifiers to the parameter (such as ref, in, out, params, etc.).

**`isOptional` as (boolean):** Set to true if the parameter is optional and can be left out when calling the method.

**`typeInfo` as ([QuickTypeInfo](#quicktypeinfo)):** The information of the parameter's type.

**`genericParameterDeclarations` as (string[]):** The list of types used for the generic parameters.

**`fullDeclaration` as (string):** The full declaration of the parameter as it would be found within the code.

Example JSON:

```json
{
  "name": "max",
  "defaultValue": "100",
  "attributes": [],
  "modifier": "",
  "isOptional": true,
  "typeInfo": {
    "unlocalizedName": "System.Int32",
    "name": "int",
    "fullName": "System.Int32",
    "namespaceName": "System",
    "genericParameters": []
  },
  "genericParameterDeclarations": [],
  "fullDeclaration": "int max = 100"
}
```

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

### GenericParametersInfo

All the information relevant to generic parameters.

**`unlocalizedName` as (string):** The unlocalized name of the generic parameter as it would appear in the IL code.

**`name` as (string):** The name of the generic parameter.

**`constraints` as ([QuickTypeInfo](#attributefieldinfo)[]):** The list of constraints of what type the generic parameter should be.

Example JSON:

```json
{
  "unlocalizedName": "T",
  "name": "T",
  "constraints": [
    {
      "unlocalizedName": "System.ValueType",
      "name": "struct",
      "fullName": "System.ValueType",
      "namespaceName": "System",
      "genericParameters": []
    }
  ]
}
```

### Example JSON Output

```json

```
