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

<details>
<summary>Example JSON</summary>

<p>

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

</p>
</details>

### GenericParametersInfo

All the information relevant to generic parameters.

**`unlocalizedName` as (string):** The unlocalized name of the generic parameter as it would appear in the IL code.

**`name` as (string):** The name of the generic parameter.

**`constraints` as ([QuickTypeInfo](#attributefieldinfo)[]):** The list of constraints of what type the generic parameter should be.

<details>
<summary>Example JSON</summary>

<p>

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

</p>
</details>

### AttributeFieldInfo

All the information relevant to the attribute's fields.

**`name` as (string):** The name of the attribute field.

**`value` as (string):** The value of the attribute field.

**`typeInfo` as ([QuickTypeInfo](#quicktypeinfo)):** The information of the attribute field's type.

<details>
<summary>Example JSON</summary>

<p>

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

</p>
</details>

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

<details>
<summary>Example JSON</summary>

<p>

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

</p>
</details>

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

<details>
<summary>Example JSON</summary>

<p>

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

</p>
</details>

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

<details>
<summary>Example JSON</summary>

<p>

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

</p>
</details>

### MethodInfo

All the information relevant to methods.

**`name` as (string):** The name of the method.

**`accessor` as (string):** The accessor of the method (such as internal, private, protected, public).

**`modifier` as (string):** Any modifiers of the method (such as static, virtual, override, etc.).

**`isAbstract` as (boolean):** Set to true if the method is abstract.

**`isConstructor` as (boolean):** Set to true if the method is a constructor.

**`isConversionOperator` as (boolean):** Set to true if the method is a conversion operator.

**`isExtension` as (boolean):** Set to true if the method is an extension.

**`isOperator` as (boolean):** Set to true if the method is an operator.

**`isOverriden` as (boolean):** Set to true if the method is overriden.

**`isStatic` as (boolean):** Set to true if the method is static.

**`isVirtual` as (boolean):** Set to true if the method is virtual.

**`implementedType` as ([QuickTypeInfo](#quicktypeinfo)):** The type that the method is implemented in.

**`returnType` as ([QuickTypeInfo](#quicktypeinfo)):** The type that the method returns.

**`attributes` as ([AttributeInfo](#attributeinfo)[]):** The attributes of the methods.

**`parameters` as ([ParameterInfo](#parameterinfo)[]):** The parameters that the methods contains.

**`declaration` as (string):** The partial declaration of the method (without parameters) that can be found in the code.

**`parameterDeclaration` as (string):** The partial declaration of the parameters that can be found in the code.

**`fullDeclaration` as (string):** The full declaration of the method that can be found in the code.

<details>
<summary>Example JSON</summary>

<p>

```json
{
  "name": "Display",
  "accessor": "public",
  "modifier": "",
  "isStatic": false,
  "isVirtual": false,
  "isAbstract": false,
  "isOverriden": true,
  "isOperator": false,
  "isExtension": false,
  "isConversionOperator": false,
  "isConstructor": false,
  "implementedType": {
    "unlocalizedName": "Dummy.DummyStruct",
    "name": "DummyStruct",
    "fullName": "Dummy.DummyStruct",
    "namespaceName": "Dummy",
    "genericParameters": []
  },
  "returnType": {
    "unlocalizedName": "System.Void",
    "name": "void",
    "fullName": "System.Void",
    "namespaceName": "System",
    "genericParameters": []
  },
  "parameters": [
    {
      "name": "dummy",
      "defaultValue": "",
      "attributes": [],
      "modifier": "ref",
      "isOptional": false,
      "typeInfo": {
        "unlocalizedName": "Dummy.DummyClass`1",
        "name": "DummyClass<int>",
        "fullName": "Dummy.DummyClass<System.Int32>",
        "namespaceName": "Dummy",
        "genericParameters": [
          {
            "unlocalizedName": "System.Int32",
            "name": "int",
            "constraints": []
          }
        ]
      },
      "genericParameterDeclarations": [
        "int"
      ],
      "fullDeclaration": "ref DummyClass<int> dummy"
    },
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
  ],
  "attributes": [],
  "declaration": "public void Display",
  "parameterDeclaration": "ref DummyClass<int> dummy, int max = 100",
  "fullDeclaration": "public void Display(ref DummyClass<int> dummy, int max = 100)"
}
```

</p>
</details>

### PropertyInfo

TODO: Add formatting

<details>
<summary>Example JSON</summary>

<p>

```json
{
  "name": "Guuid",
  "isStatic": false,
  "hasGetter": true,
  "hasSetter": false,
  "attributes": [],
  "accessor": "public",
  "modifier": "virtual",
  "typeInfo": {
    "unlocalizedName": "System.String",
    "name": "string",
    "fullName": "System.String",
    "namespaceName": "System",
    "genericParameters": []
  },
  "implementedType": {
    "unlocalizedName": "Dummy.DummyClass`1",
    "name": "DummyClass<T>",
    "fullName": "Dummy.DummyClass<T>",
    "namespaceName": "Dummy",
    "genericParameters": [
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
    ]
  },
  "parameters": [],
  "getter": {
    "name": "get_Guuid",
    "accessor": "public",
    "modifier": "virtual",
    "isAbstract": false,
    "isConstructor": false,
    "isConversionOperator": false,
    "isExtension": false,
    "isOperator": false,
    "isOverriden": false,
    "isStatic": false,
    "isVirtual": true,
    "implementedType": {
      "unlocalizedName": "Dummy.DummyClass`1",
      "name": "DummyClass<T>",
      "fullName": "Dummy.DummyClass<T>",
      "namespaceName": "Dummy",
      "genericParameters": [
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
      ]
    },
    "returnType": {
      "unlocalizedName": "System.String",
      "name": "string",
      "fullName": "System.String",
      "namespaceName": "System",
      "genericParameters": []
    },
    "attributes": [],
    "parameters": [],
    "declaration": "public virtual string get_Guuid",
    "parameterDeclaration": "",
    "fullDeclaration": "public virtual string get_Guuid()"
  },
  "setter": null,
  "declaration": "public virtual string Guuid",
  "parameterDeclaration": "",
  "getSetDeclaration": "get;",
  "fullDeclaration": "public virtual string Guuid { get; }"
}
```

</p>
</details>

### EventInfo

All the information relevant to events.

**`name` as (string):** The name of the event.

**`isStatic` as (boolean):** Set to true if the event is static.

**`accessor` as (string):** The accessor of the event (such as internal, private, protected, public).

**`modifier` as (string):** Any modifiers of the event (such as static, virtual, override, etc.).

**`typeInfo` as ([QuickTypeInfo](#quicktypeinfo)):** The information of the event's type.

**`adder` as ([MethodInfo](#methodinfo)):** The information of the event's adding method.

**`remover` as ([MethodInfo](#methodinfo):** The information of the event's removing method.

**`fullDeclaration` as (string):** The declaration of the event as it would be found in the code.

<details>
<summary>Example JSON</summary>

<p>

```json
{
  "name": "OnLog",
  "isStatic": false,
  "accessor": "public",
  "modifier": "",
  "typeInfo": {
    "unlocalizedName": "System.EventHandler",
    "name": "EventHandler",
    "fullName": "System.EventHandler",
    "namespaceName": "System",
    "genericParameters": []
  },
  "adder": {
    "name": "add_OnLog",
    "accessor": "public",
    "modifier": "",
    "isAbstract": false,
    "isConstructor": false,
    "isConversionOperator": false,
    "isExtension": false,
    "isOperator": false,
    "isOverriden": true,
    "isStatic": false,
    "isVirtual": false,
    "implementedType": {
      "unlocalizedName": "Dummy.DummyClass`1",
      "name": "DummyClass<T>",
      "fullName": "Dummy.DummyClass<T>",
      "namespaceName": "Dummy",
      "genericParameters": [
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
      ]
    },
    "returnType": {
      "unlocalizedName": "System.Void",
      "name": "void",
      "fullName": "System.Void",
      "namespaceName": "System",
      "genericParameters": []
    },
    "attributes": [
      {
        "typeInfo": {
          "unlocalizedName": "System.Runtime.CompilerServices.CompilerGeneratedAttribute",
          "name": "CompilerGeneratedAttribute",
          "fullName": "System.Runtime.CompilerServices.CompilerGeneratedAttribute",
          "namespaceName": "System.Runtime.CompilerServices",
          "genericParameters": []
        },
        "constructorArgs": [],
        "properties": [],
        "parameterDeclaration": "",
        "fullDeclaration": "[System.Runtime.CompilerServices.CompilerGeneratedAttribute]"
      }
    ],
    "parameters": [
      {
        "name": "value",
        "defaultValue": "",
        "attributes": [],
        "modifier": "",
        "isOptional": false,
        "typeInfo": {
          "unlocalizedName": "System.EventHandler",
          "name": "EventHandler",
          "fullName": "System.EventHandler",
          "namespaceName": "System",
          "genericParameters": []
        },
        "genericParameterDeclarations": [],
        "fullDeclaration": "EventHandler value"
      }
    ],
    "declaration": "public void add_OnLog",
    "parameterDeclaration": "EventHandler value",
    "fullDeclaration": "public void add_OnLog(EventHandler value)"
  },
  "remover": {
    "name": "remove_OnLog",
    "accessor": "public",
    "modifier": "",
    "isAbstract": false,
    "isConstructor": false,
    "isConversionOperator": false,
    "isExtension": false,
    "isOperator": false,
    "isOverriden": true,
    "isStatic": false,
    "isVirtual": false,
    "implementedType": {
      "unlocalizedName": "Dummy.DummyClass`1",
      "name": "DummyClass<T>",
      "fullName": "Dummy.DummyClass<T>",
      "namespaceName": "Dummy",
      "genericParameters": [
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
      ]
    },
    "returnType": {
      "unlocalizedName": "System.Void",
      "name": "void",
      "fullName": "System.Void",
      "namespaceName": "System",
      "genericParameters": []
    },
    "attributes": [
      {
        "typeInfo": {
          "unlocalizedName": "System.Runtime.CompilerServices.CompilerGeneratedAttribute",
          "name": "CompilerGeneratedAttribute",
          "fullName": "System.Runtime.CompilerServices.CompilerGeneratedAttribute",
          "namespaceName": "System.Runtime.CompilerServices",
          "genericParameters": []
        },
        "constructorArgs": [],
        "properties": [],
        "parameterDeclaration": "",
        "fullDeclaration": "[System.Runtime.CompilerServices.CompilerGeneratedAttribute]"
      }
    ],
    "parameters": [
      {
        "name": "value",
        "defaultValue": "",
        "attributes": [],
        "modifier": "",
        "isOptional": false,
        "typeInfo": {
          "unlocalizedName": "System.EventHandler",
          "name": "EventHandler",
          "fullName": "System.EventHandler",
          "namespaceName": "System",
          "genericParameters": []
        },
        "genericParameterDeclarations": [],
        "fullDeclaration": "EventHandler value"
      }
    ],
    "declaration": "public void remove_OnLog",
    "parameterDeclaration": "EventHandler value",
    "fullDeclaration": "public void remove_OnLog(EventHandler value)"
  },
  "fullDeclaration": "public EventHandler OnLog"
}
```

</p>
</details>
