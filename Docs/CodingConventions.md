# Coding Conventions

To ensure readability and consistence of the code, here is a sick style guide.
It is partly based on [Stan's Asset Unity C# Coding guidelines](https://unionassets.com/blog/stan-s-assets-unity-c-coding-guidelines-465), coding conventions of other games, the [official C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions)  and some personal habits.
If there is something not mentioned in this document, refer to the [official C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions).

THIS IS A WORK IN PROGRESS!

(If you wanna add something to this Coding Convention document and don't wanna look up the Markdown syntax, here is the [Markdown Cheat Sheet](https://github.com/adam-p/markdown-here/wiki/Markdown-Cheatsheet))

## Naming Conventions

Everything should be named in englisch, no german-englisch language mixing. Comments should also be in english to keep everything consistent.

### Classes

Public, Private and Abstract classes in **PascalCase**. For example `Button`.
Interfaces in **PascalCase** + **I**prefix For example `IButton`.

### Methods

Methods are written in **PascalCase**. For example `DoSomething()`.

### Fields

- const -> **UPPER_CASE**
- public -> **PascalCase**
- private, protected, internal -> _ prefix + **camelCase**


Sort fields from const -> static -> serialized fields -> public -> private

```c#
public class ExampleClass : MonoBehaviour
{
    public  const int MY_CONST_VALUE = 0;
    private const int MY_CONST_VALUE_2 = 0;

    public static int MyStaticFiled;
    private static int _myPrivate;

    public int PublicField;

    private int _myPrivate;
    protected int _myProtected;
}
```

### Public access
Don't make fields public. Use `[SerializeField]` and public properties. Make values constants when finished fine tuning them in the inspector.

#### Inspector access
When setting something in the inspector, use `[SerializeField]` to make private fields accessible in the inspector, but keep the field itself private. Use `Tooltip` to give an explanation in the inspector.

```c#
[Tooltip("This is a reference to a specific UI element")]
[SerializeField] private Text _uiText;
```

#### Properties

Wrap private fields in public properties to acces them from other scripts but not display them in the inspector. Use auto-properties when possible for readability and auto suggestion clutter. Mark set as private unless it needs to be public.

```c#
public int AutoProperty { get; private set; }

private void SomeMethod(int value)
{
    AutoProperty = value;
} 
```

or

```c#
private int _someField;
public int SomeProperty 
{
    get => _someField;
    private set => _someField = value;
}

private void SomeMethod(int value)
{
    _someField = value;
} 
```

### Expression body definitions
Use [expression-bodied members](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/expression-bodied-members) to improve readability. Here is an example for the previous property:

DO:

```c#
private int _someField;
public int SomeProperty 
{
    get => _someField;
    private set => _someField = value;
}
```

DON'T:

```c#
private int _someField;
public int SomeProperty 
{
    get 
    { 
        return _someField; 
    }

    private set 
    { 
        _someField = value; 
    }
}
```


### Parameters

Parameters are written in camelCase.

```c#
private void SetPosition(Vector3 position) 
{
    var newPosition = position;
}
```

### Names

Prefix event methods with the prefix On.

```c#
public event Action OnClose;
```

If a method is used to handle delegate callback add the suffix **Handler**

```c#
void CloseHandler();
void ResetButtonClickHandler();
void PlayerDisconnectedHandler();
```

## Layout Conventions

### General

- Write only one statement per line.
- Write only one declaration per line.
- Add at least one blank line between method definitions and property definitions.
- Use parentheses to make clauses in an expression apparent.

```c#
if ((val1 > val2) && (val1 > val3)) 
{
    // do something
}
````

### Brace Style

Add a new line before opening a brace.

```c#
public class ExampleClass : MonoBehaviour
{
    private void ExampleMethod() 
    {
        // do something
    }
}
```

## Commenting
TODO: [xml](https://docs.microsoft.com/en-gb/dotnet/csharp/language-reference/language-specification/documentation-comments) and use of regions.

- Place the comment on a separate line, not at the end of a line of code.
- Insert one space between the comment delimiter `//` and the comment text.
- When commenting out lagre code sections use an individual comment delimiter `//` for each line instead of asterisks `/* */` around the code block.

## Misc

### String Concatenation

Use string interpolation to concatenate strings instead of adding them.

```c#
string str = $"Hello {userName}. Today is {date}.";
```


### Clean up your code

Remove all comented out **dead code** and all `Debug.Log()` calls that are not loger needed.

### No magic numbers

Or strings, floats etc. Make them constants. Also make values constants when finished fine tuning them in the inspector.

#### DON'T:

```c#
private void SomeMethod()
{
    float circle = _diameter * 3.14;
}
```

#### DO:

```c#
private const float PI = 3.14f;

private void SomeMethod()
{
    float circle = _diameter * PI;
}
```

### Null-Checks

use [null-conditionals](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/member-access-operators#null-conditional-operators--and-) where applicable to check if a value has been set.

```c#
[SerializeField] private Text _uiText;

public void SomeMethod()
{
    _uiText?.text = "Hello World";
}
```
