# SharpChecker

SharpChecker is a CLI program that looks into .NET libraries (dll) and executables (exe) to check on information about specific types as the user has queried. SharpChecker will generate a JSON file that records everything it can about the specified type. Alternatively, the program can also list all the types existing in the list of .NET assemblies that the user has specified, recording a JSON file of all the types found within their respective assembly. To see more information on how to use SharpChecker, see the [Usage section](#usage).

**NOTE:** When looking up types with generic parameters, instead of looking for something like `System.Collections.Generic.Dictionary<TValue, TKey>`, you must look up the type with a grave character ( &grave; ); which means you must look up the type as such: ``System.Collections.Generic.Dictionary`2``. Since the grave character ( &grave; ) does not work nicely with terminals, you can replace the grave character ( &grave; ) with a dash ( - ). Thus looking for types with generic parameters should be as such: `System.Collections.Generic.Dictionary-2`.

## Build Status

Builds and tests are done as
[GitHub actions](https://github.com/FuLagann/sharp-checker/actions).

| Status | Description |
|:-------|-------------|
| ![.NET Test](https://github.com/FuLagann/sharp-checker/workflows/.NET%20Test/badge.svg) | .NET build and test for all platforms in .NET Core 3.0 in the master branch |

## Usage

To use SharpChecker, input the type path and a list of assemblies with any options. The usage format is as follows:

```html
SharpChecker [options] <type-path> <list-of-assemblies>
```

### Options

The following are options that can be optionally inputted into the program to change the way it generates documents:

**`-h` or `--help`:** Displays the help menu.

**`-l` or `--list`:** Lists all the types of each assembly.

**`-p` or `--include-private`:** Include private members.

**`-o <output-file>` or `--out <output-file>`:** The file to be outputted, must have a space between the option declaration and the filename. Output will be relative to where the program is ran. By default, the output file will be `type.json`. Alternatively, when generating a type list (by using `--list`) the output file will be `listTypes.json`.

<details>
<summary>Example Usage</summary>

<p>

Using the following command will generate a [TypeList](#typelist) JSON.

```bat
SharpChecker --list TestingLibrary.dll SecondLibrary.dll
```

Using the following command will generate a [TypeList](#typelist) JSON where all the private and internal types are recorded.

```bat
SharpChecker --list --include-private TestingLibrary.dll SecondLibrary.dll
```

Using the following command will generate a [TypeInfo](#typeinfo) JSON.

```bat
SharpChecker Dummy.DummyClass TestingLibrary.dll SecondLibrary.dll
```

Using the following command will generate a [TypeInfo](#typeinfo) JSON where all the private and internal members are recorded.

```bat
SharpChecker --include-private Dummy.DummyClass TestingLibrary.dll SecondLibrary.dll
```

**NOTE:** When looking up types with generic parameters, instead of looking for something like `System.Collections.Generic.Dictionary<TValue, TKey>`, you must look up the type with a grave character ( &grave; ); which means you must look up the type as such: ``System.Collections.Generic.Dictionary`2``. Since the grave character ( &grave; ) does not work nicely with terminals, you can replace the grave character ( &grave; ) with a dash ( - ). The following is an example of looking up a type with a generic parameter.

```bat
SharpChecker Dummy.DummyList-1 TestingLibrary.dll SecondLibrary.dll
```

</p>
</details>

## Format

The following subsections are all the data structures that the generated JSON can take form of. When generating a list of all the types within the assemblies inputted, the returning data structure of the JSON will be a [TypeList](#typelist). When generating an in depth view of a type, the returning data structure of the JSON will be a [TypeInfo](#typeinfo).

### TypeList

All the information of types with it's associated library or executable.

**`types` as (Map&lt;string, string[]&gt;):** A hashmap of a library or executable mapping to a list of types it contains.

<details>
<summary>Example JSON</summary>

<p>

```json
{
  "types": {
    "Dummy.Library1.dll": [
      "SchoolSys.BaseMember",
      "SchoolSys.FacultyMember",
      "SchoolSys.HiddenMember",
      "SchoolSys.IMember",
      "SchoolSys.ISchedule",
      "SchoolSys.StaffMember",
      "SchoolSys.StudentMember",
      "SchoolSys.TimeBlock",
      "SchoolSys.Guests.GuestMember`1"
    ],
    "Dummy.Library2.dll": [
      "DataStructures.Action`3",
      "DataStructures.DataTree`1",
      "DataStructures.DataTree`1/GatherHash`1",
      "DataStructures.OpenGrid",
      "DataStructures.OpenList`1",
      "DataStructures.OpenMap`2"
    ],
    "Dummy.Library3.dll": [
      "Dummy.AbstractDummyClass",
      "Dummy.DummyClass",
      "Dummy.DummyEnum",
      "Dummy.IDummy",
      "Dummy.SealedDummyClass"
    ]
  }
}
```

</p>
</details>

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

### TypeInfo

All the information relevant to types.

**`typeInfo` as ([QuickTypeInfo](#quicktypeinfo)):** The quick look at the information of the type (including name, namespace, generic parameters).

**`assemblyName` as (string):** The name of the assembly where the type is found in.

**`accessor` as (string):** The accessor of the type (such as internal, private, protected, public).

**`modifier` as (string):** Any modifiers that the type contains (such as static, sealed, abstract, etc.).

**`objectType` as (string):** The object type of the type (such as class, struct, enum, or interface).

**`declaration` as (string):** The partial declaration of the class within the inheritance declaration that can be found within the code.

**`fullDeclaration` as (string):** The full declaration of the type as it would be found within the code.

**`baseType` as ([QuickTypeInfo](#quicktypeinfo)):** The information of the base type that the type inherits.

**`attributes` as ([AttributeInfo](#attributeinfo)[]):** The array of attributes that the type contains.

**`interfaces` as ([QuickTypeInfo](#quicktypeinfo)[]):** The array of type information of interfaces that the type implements.

**`constructors` as ([MethodInfo](#methodinfo)[]):** The array of constructors that the type contains.

**`fields` as ([FieldInfo](#fieldinfo)[]):** The array of fields that the type contains.

**`staticFields` as ([FieldInfo](#fieldinfo)[]):** The array of static fields that the type contains.

**`properties` as ([PropertyInfo](#propertyinfo)[]):** The array of properties that the type contains.

**`staticProperties` as ([PropertyInfo](#propertyinfo)[]):** The array of static properties that the type contains.

**`events` as ([EventInfo](#eventinfo)[]):** The array of events that the type contains.

**`staticEvents` as ([EventInfo](#eventinfo)[]):** The array of static events that the type contains.

**`methods` as ([MethodInfo](#methodinfo)[]):** The array of methods that the type contains.

**`staticMethods` as ([MethodInfo](#methodinfo)[]):** The array of static methods that the type contains.

**`operators` as ([MethodInfo](#methodinfo)[]):** The array of operators that the type contains.

<details>
<summary>Example JSON</summary>

<p>

```json
{
  "typeInfo": {
    "unlocalizedName": "SchoolSys.StudentMember",
    "name": "StudentMember",
    "fullName": "SchoolSys.StudentMember",
    "namespaceName": "SchoolSys",
    "genericParameters": []
  },
  "assemblyName": "Dummy.Library1",
  "accessor": "public",
  "modifier": "sealed",
  "objectType": "class",
  "declaration": "public sealed class StudentMember",
  "fullDeclaration": "public sealed class StudentMember : BaseMember, ICloneable",
  "baseType": {
    "unlocalizedName": "SchoolSys.BaseMember",
    "name": "BaseMember",
    "fullName": "SchoolSys.BaseMember",
    "namespaceName": "SchoolSys",
    "genericParameters": []
  },
  "attributes": [],
  "interfaces": [
    {
      "unlocalizedName": "System.ICloneable",
      "name": "ICloneable",
      "fullName": "System.ICloneable",
      "namespaceName": "System",
      "genericParameters": []
    }
  ],
  "constructors": [
    {
      "name": "StudentMember",
      "accessor": "public",
      "modifier": "",
      "isAbstract": false,
      "isConstructor": true,
      "isConversionOperator": false,
      "isExtension": false,
      "isOperator": false,
      "isOverriden": true,
      "isStatic": false,
      "isVirtual": false,
      "implementedType": {
        "unlocalizedName": "SchoolSys.StudentMember",
        "name": "StudentMember",
        "fullName": "SchoolSys.StudentMember",
        "namespaceName": "SchoolSys",
        "genericParameters": []
      },
      "returnType": {
        "unlocalizedName": "System.Void",
        "name": "void",
        "fullName": "System.Void",
        "namespaceName": "System",
        "genericParameters": []
      },
      "attributes": [],
      "parameters": [
        {
          "name": "id",
          "defaultValue": "",
          "attributes": [],
          "modifier": "",
          "isOptional": false,
          "typeInfo": {
            "unlocalizedName": "System.String",
            "name": "string",
            "fullName": "System.String",
            "namespaceName": "System",
            "genericParameters": []
          },
          "genericParameterDeclarations": [],
          "fullDeclaration": "string id"
        }
      ],
      "declaration": "public StudentMember",
      "parameterDeclaration": "string id",
      "fullDeclaration": "public StudentMember(string id)"
    }
  ],
  "fields": [
    {
      "name": "id",
      "value": "",
      "isConstant": false,
      "isStatic": false,
      "isReadonly": false,
      "attributes": [],
      "accessor": "private",
      "modifier": "",
      "typeInfo": {
        "unlocalizedName": "System.String",
        "name": "string",
        "fullName": "System.String",
        "namespaceName": "System",
        "genericParameters": []
      },
      "implementedType": {
        "unlocalizedName": "SchoolSys.StudentMember",
        "name": "StudentMember",
        "fullName": "SchoolSys.StudentMember",
        "namespaceName": "SchoolSys",
        "genericParameters": []
      },
      "declaration": "private string id"
    }
  ],
  "staticFields": [],
  "properties": [
    {
      "name": "Id",
      "isStatic": false,
      "hasGetter": true,
      "hasSetter": false,
      "attributes": [],
      "accessor": "public",
      "modifier": "override",
      "typeInfo": {
        "unlocalizedName": "System.String",
        "name": "string",
        "fullName": "System.String",
        "namespaceName": "System",
        "genericParameters": []
      },
      "implementedType": {
        "unlocalizedName": "SchoolSys.StudentMember",
        "name": "StudentMember",
        "fullName": "SchoolSys.StudentMember",
        "namespaceName": "SchoolSys",
        "genericParameters": []
      },
      "parameters": [],
      "getter": {
        "name": "get_Id",
        "accessor": "public",
        "modifier": "override",
        "isAbstract": false,
        "isConstructor": false,
        "isConversionOperator": false,
        "isExtension": false,
        "isOperator": false,
        "isOverriden": true,
        "isStatic": false,
        "isVirtual": true,
        "implementedType": {
          "unlocalizedName": "SchoolSys.StudentMember",
          "name": "StudentMember",
          "fullName": "SchoolSys.StudentMember",
          "namespaceName": "SchoolSys",
          "genericParameters": []
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
        "declaration": "public override string get_Id",
        "parameterDeclaration": "",
        "fullDeclaration": "public override string get_Id()"
      },
      "setter": null,
      "declaration": "public override string Id",
      "parameterDeclaration": "",
      "getSetDeclaration": "get;",
      "fullDeclaration": "public override string Id { get; }"
    }
  ],
  "staticProperties": [],
  "events": [
    {
      "name": "OnMesseged",
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
      "implementedType": {
        "unlocalizedName": "SchoolSys.BaseMember",
        "name": "BaseMember",
        "fullName": "SchoolSys.BaseMember",
        "namespaceName": "SchoolSys",
        "genericParameters": []
      },
      "adder": {
        "name": "add_OnMesseged",
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
          "unlocalizedName": "SchoolSys.BaseMember",
          "name": "BaseMember",
          "fullName": "SchoolSys.BaseMember",
          "namespaceName": "SchoolSys",
          "genericParameters": []
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
        "declaration": "public void add_OnMesseged",
        "parameterDeclaration": "EventHandler value",
        "fullDeclaration": "public void add_OnMesseged(EventHandler value)"
      },
      "remover": {
        "name": "remove_OnMesseged",
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
          "unlocalizedName": "SchoolSys.BaseMember",
          "name": "BaseMember",
          "fullName": "SchoolSys.BaseMember",
          "namespaceName": "SchoolSys",
          "genericParameters": []
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
        "declaration": "public void remove_OnMesseged",
        "parameterDeclaration": "EventHandler value",
        "fullDeclaration": "public void remove_OnMesseged(EventHandler value)"
      },
      "fullDeclaration": "public EventHandler OnMesseged"
    },
    {
      "name": "OnMessege",
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
      "implementedType": {
        "unlocalizedName": "SchoolSys.BaseMember",
        "name": "BaseMember",
        "fullName": "SchoolSys.BaseMember",
        "namespaceName": "SchoolSys",
        "genericParameters": []
      },
      "adder": {
        "name": "add_OnMessege",
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
          "unlocalizedName": "SchoolSys.BaseMember",
          "name": "BaseMember",
          "fullName": "SchoolSys.BaseMember",
          "namespaceName": "SchoolSys",
          "genericParameters": []
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
        "declaration": "public void add_OnMessege",
        "parameterDeclaration": "EventHandler value",
        "fullDeclaration": "public void add_OnMessege(EventHandler value)"
      },
      "remover": {
        "name": "remove_OnMessege",
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
          "unlocalizedName": "SchoolSys.BaseMember",
          "name": "BaseMember",
          "fullName": "SchoolSys.BaseMember",
          "namespaceName": "SchoolSys",
          "genericParameters": []
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
        "declaration": "public void remove_OnMessege",
        "parameterDeclaration": "EventHandler value",
        "fullDeclaration": "public void remove_OnMessege(EventHandler value)"
      },
      "fullDeclaration": "public EventHandler OnMessege"
    }
  ],
  "staticEvents": [],
  "methods": [
    {
      "name": "Clone",
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
        "unlocalizedName": "SchoolSys.StudentMember",
        "name": "StudentMember",
        "fullName": "SchoolSys.StudentMember",
        "namespaceName": "SchoolSys",
        "genericParameters": []
      },
      "returnType": {
        "unlocalizedName": "System.Object",
        "name": "object",
        "fullName": "System.Object",
        "namespaceName": "System",
        "genericParameters": []
      },
      "attributes": [],
      "parameters": [],
      "declaration": "public virtual object Clone",
      "parameterDeclaration": "",
      "fullDeclaration": "public virtual object Clone()"
    },
    {
      "name": "SignIn",
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
        "unlocalizedName": "SchoolSys.BaseMember",
        "name": "BaseMember",
        "fullName": "SchoolSys.BaseMember",
        "namespaceName": "SchoolSys",
        "genericParameters": []
      },
      "returnType": {
        "unlocalizedName": "System.Boolean",
        "name": "bool",
        "fullName": "System.Boolean",
        "namespaceName": "System",
        "genericParameters": []
      },
      "attributes": [],
      "parameters": [],
      "declaration": "public virtual bool SignIn",
      "parameterDeclaration": "",
      "fullDeclaration": "public virtual bool SignIn()"
    },
    {
      "name": "SignOut",
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
        "unlocalizedName": "SchoolSys.BaseMember",
        "name": "BaseMember",
        "fullName": "SchoolSys.BaseMember",
        "namespaceName": "SchoolSys",
        "genericParameters": []
      },
      "returnType": {
        "unlocalizedName": "System.Boolean",
        "name": "bool",
        "fullName": "System.Boolean",
        "namespaceName": "System",
        "genericParameters": []
      },
      "attributes": [],
      "parameters": [],
      "declaration": "public virtual bool SignOut",
      "parameterDeclaration": "",
      "fullDeclaration": "public virtual bool SignOut()"
    },
    {
      "name": "GetSchedule",
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
        "unlocalizedName": "SchoolSys.BaseMember",
        "name": "BaseMember",
        "fullName": "SchoolSys.BaseMember",
        "namespaceName": "SchoolSys",
        "genericParameters": []
      },
      "returnType": {
        "unlocalizedName": "SchoolSys.ISchedule[]",
        "name": "ISchedule[]",
        "fullName": "SchoolSys.ISchedule[]",
        "namespaceName": "SchoolSys",
        "genericParameters": []
      },
      "attributes": [],
      "parameters": [],
      "declaration": "public virtual ISchedule[] GetSchedule",
      "parameterDeclaration": "",
      "fullDeclaration": "public virtual ISchedule[] GetSchedule()"
    },
    {
      "name": "Talk",
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
        "unlocalizedName": "SchoolSys.BaseMember",
        "name": "BaseMember",
        "fullName": "SchoolSys.BaseMember",
        "namespaceName": "SchoolSys",
        "genericParameters": []
      },
      "returnType": {
        "unlocalizedName": "System.Void",
        "name": "void",
        "fullName": "System.Void",
        "namespaceName": "System",
        "genericParameters": []
      },
      "attributes": [],
      "parameters": [
        {
          "name": "message",
          "defaultValue": "",
          "attributes": [],
          "modifier": "",
          "isOptional": false,
          "typeInfo": {
            "unlocalizedName": "System.String",
            "name": "string",
            "fullName": "System.String",
            "namespaceName": "System",
            "genericParameters": []
          },
          "genericParameterDeclarations": [],
          "fullDeclaration": "string message"
        }
      ],
      "declaration": "public void Talk",
      "parameterDeclaration": "string message",
      "fullDeclaration": "public void Talk(string message)"
    },
    {
      "name": "GetType",
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
        "unlocalizedName": "System.Object",
        "name": "object",
        "fullName": "System.Object",
        "namespaceName": "System",
        "genericParameters": []
      },
      "returnType": {
        "unlocalizedName": "System.Type",
        "name": "Type",
        "fullName": "System.Type",
        "namespaceName": "System",
        "genericParameters": []
      },
      "attributes": [
        {
          "typeInfo": {
            "unlocalizedName": "System.Runtime.CompilerServices.NullableContextAttribute",
            "name": "NullableContextAttribute",
            "fullName": "System.Runtime.CompilerServices.NullableContextAttribute",
            "namespaceName": "System.Runtime.CompilerServices",
            "genericParameters": []
          },
          "constructorArgs": [
            {
              "name": "",
              "value": "1",
              "typeInfo": {
                "unlocalizedName": "System.Byte",
                "name": "byte",
                "fullName": "System.Byte",
                "namespaceName": "System",
                "genericParameters": []
              }
            }
          ],
          "properties": [],
          "parameterDeclaration": "1",
          "fullDeclaration": "[System.Runtime.CompilerServices.NullableContextAttribute(1)]"
        }
      ],
      "parameters": [],
      "declaration": "public Type GetType",
      "parameterDeclaration": "",
      "fullDeclaration": "public Type GetType()"
    },
    {
      "name": "MemberwiseClone",
      "accessor": "protected",
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
        "unlocalizedName": "System.Object",
        "name": "object",
        "fullName": "System.Object",
        "namespaceName": "System",
        "genericParameters": []
      },
      "returnType": {
        "unlocalizedName": "System.Object",
        "name": "object",
        "fullName": "System.Object",
        "namespaceName": "System",
        "genericParameters": []
      },
      "attributes": [
        {
          "typeInfo": {
            "unlocalizedName": "System.Runtime.CompilerServices.NullableContextAttribute",
            "name": "NullableContextAttribute",
            "fullName": "System.Runtime.CompilerServices.NullableContextAttribute",
            "namespaceName": "System.Runtime.CompilerServices",
            "genericParameters": []
          },
          "constructorArgs": [
            {
              "name": "",
              "value": "1",
              "typeInfo": {
                "unlocalizedName": "System.Byte",
                "name": "byte",
                "fullName": "System.Byte",
                "namespaceName": "System",
                "genericParameters": []
              }
            }
          ],
          "properties": [],
          "parameterDeclaration": "1",
          "fullDeclaration": "[System.Runtime.CompilerServices.NullableContextAttribute(1)]"
        }
      ],
      "parameters": [],
      "declaration": "protected object MemberwiseClone",
      "parameterDeclaration": "",
      "fullDeclaration": "protected object MemberwiseClone()"
    },
    {
      "name": "Finalize",
      "accessor": "protected",
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
        "unlocalizedName": "System.Object",
        "name": "object",
        "fullName": "System.Object",
        "namespaceName": "System",
        "genericParameters": []
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
            "unlocalizedName": "System.Runtime.Versioning.NonVersionableAttribute",
            "name": "NonVersionableAttribute",
            "fullName": "System.Runtime.Versioning.NonVersionableAttribute",
            "namespaceName": "System.Runtime.Versioning",
            "genericParameters": []
          },
          "constructorArgs": [],
          "properties": [],
          "parameterDeclaration": "",
          "fullDeclaration": "[System.Runtime.Versioning.NonVersionableAttribute]"
        }
      ],
      "parameters": [],
      "declaration": "protected virtual void Finalize",
      "parameterDeclaration": "",
      "fullDeclaration": "protected virtual void Finalize()"
    },
    {
      "name": "ToString",
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
        "unlocalizedName": "System.Object",
        "name": "object",
        "fullName": "System.Object",
        "namespaceName": "System",
        "genericParameters": []
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
      "declaration": "public virtual string ToString",
      "parameterDeclaration": "",
      "fullDeclaration": "public virtual string ToString()"
    },
    {
      "name": "Equals",
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
        "unlocalizedName": "System.Object",
        "name": "object",
        "fullName": "System.Object",
        "namespaceName": "System",
        "genericParameters": []
      },
      "returnType": {
        "unlocalizedName": "System.Boolean",
        "name": "bool",
        "fullName": "System.Boolean",
        "namespaceName": "System",
        "genericParameters": []
      },
      "attributes": [],
      "parameters": [
        {
          "name": "obj",
          "defaultValue": "",
          "attributes": [],
          "modifier": "",
          "isOptional": false,
          "typeInfo": {
            "unlocalizedName": "System.Object",
            "name": "object",
            "fullName": "System.Object",
            "namespaceName": "System",
            "genericParameters": []
          },
          "genericParameterDeclarations": [],
          "fullDeclaration": "object obj"
        }
      ],
      "declaration": "public virtual bool Equals",
      "parameterDeclaration": "object obj",
      "fullDeclaration": "public virtual bool Equals(object obj)"
    },
    {
      "name": "GetHashCode",
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
        "unlocalizedName": "System.Object",
        "name": "object",
        "fullName": "System.Object",
        "namespaceName": "System",
        "genericParameters": []
      },
      "returnType": {
        "unlocalizedName": "System.Int32",
        "name": "int",
        "fullName": "System.Int32",
        "namespaceName": "System",
        "genericParameters": []
      },
      "attributes": [],
      "parameters": [],
      "declaration": "public virtual int GetHashCode",
      "parameterDeclaration": "",
      "fullDeclaration": "public virtual int GetHashCode()"
    }
  ],
  "staticMethods": [
    {
      "name": "GenerateId",
      "accessor": "public",
      "modifier": "static",
      "isAbstract": false,
      "isConstructor": false,
      "isConversionOperator": false,
      "isExtension": false,
      "isOperator": false,
      "isOverriden": true,
      "isStatic": true,
      "isVirtual": false,
      "implementedType": {
        "unlocalizedName": "SchoolSys.StudentMember",
        "name": "StudentMember",
        "fullName": "SchoolSys.StudentMember",
        "namespaceName": "SchoolSys",
        "genericParameters": []
      },
      "returnType": {
        "unlocalizedName": "System.String",
        "name": "string",
        "fullName": "System.String",
        "namespaceName": "System",
        "genericParameters": []
      },
      "attributes": [],
      "parameters": [
        {
          "name": "seed",
          "defaultValue": "10203040",
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
          "fullDeclaration": "int seed = 10203040"
        }
      ],
      "declaration": "public static string GenerateId",
      "parameterDeclaration": "int seed = 10203040",
      "fullDeclaration": "public static string GenerateId(int seed = 10203040)"
    }
  ],
  "operators": [
    {
      "name": "op_Implicit__StaffMember",
      "accessor": "public",
      "modifier": "static implicit operator",
      "isAbstract": false,
      "isConstructor": false,
      "isConversionOperator": true,
      "isExtension": false,
      "isOperator": true,
      "isOverriden": true,
      "isStatic": true,
      "isVirtual": false,
      "implementedType": {
        "unlocalizedName": "SchoolSys.StudentMember",
        "name": "StudentMember",
        "fullName": "SchoolSys.StudentMember",
        "namespaceName": "SchoolSys",
        "genericParameters": []
      },
      "returnType": {
        "unlocalizedName": "SchoolSys.StaffMember",
        "name": "StaffMember",
        "fullName": "SchoolSys.StaffMember",
        "namespaceName": "SchoolSys",
        "genericParameters": []
      },
      "attributes": [],
      "parameters": [
        {
          "name": "obj",
          "defaultValue": "",
          "attributes": [],
          "modifier": "",
          "isOptional": false,
          "typeInfo": {
            "unlocalizedName": "SchoolSys.StudentMember",
            "name": "StudentMember",
            "fullName": "SchoolSys.StudentMember",
            "namespaceName": "SchoolSys",
            "genericParameters": []
          },
          "genericParameterDeclarations": [],
          "fullDeclaration": "StudentMember obj"
        }
      ],
      "declaration": "public static implicit operator StaffMember",
      "parameterDeclaration": "StudentMember obj",
      "fullDeclaration": "public static implicit operator StaffMember(StudentMember obj)"
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

**`implementedType` as ([QuickTypeInfo](#quicktypeinfo)):** The type the field is implemented in.

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

### PropertyInfo

All the information relevant to the property.

**`name` as (string):** The name of the property.

**`isStatic` as (boolean):** Set to true if the property is static.

**`hasGetter` as (boolean):** Set to true if the property has a getter method.

**`hasSetter` as (boolean):** Set to true if the property has a setter method.

**`attributes` as ([AttributeInfo](#attributeinfo)[]):** The list of attributes associated with the property.

**`accessor` as (string):** The accessor of the property (such as internal, private, protected, public).

**`modifier` as (string):** Any modifiers to the property (such as static, virtual, override, etc.).

**`typeInfo` as ([QuickTypeInfo](#quicktypeinfo)):** The information of the property's type.

**`implementedType` as ([QuickTypeInfo](#quicktypeinfo)):** The information of where the property was implemented.

**`parameters` as ([ParameterInfo](#parameterinfo)[]):** The parameters the property has (if any).

**`getter` as ([MethodInfo](#methodinfo)):** The getter method of the property (this can be null, you must check the hasGetter variable).

**`setter` as ([MethodInfo](#methodinfo)):** The setter method of the property (this can be null, you must check the hasSetter variable).

**`declaration` as (string):** The partial declaration of the property as can be found in the code.

**`parameterDeclaration` as (string):** The partial declaration of the property's parameters (if any) as can be found in the code.

**`getSetDeclaration` as (string):** The partial declaration of the property that determines the accessibility of the get and set methods as can be found in the code.

**`fullDeclaration` as (string):** The full declaration of the property as can be found in the code.

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

**`implementedType` as ([QuickTypeInfo](#quicktypeinfo)):** The type the event is implemented in.

**`adder` as ([MethodInfo](#methodinfo)):** The information of the event's adding method.

**`remover` as ([MethodInfo](#methodinfo)):** The information of the event's removing method.

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
