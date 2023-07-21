# Coding Style Guide

These are some style conventions to follow when contributing to the project.

## Easy Code Style

Use "CSharpier" for easy code formatting.  
Available in VSCode and VS.  
Auto format on save for optimal use.

## File and Folder Conventions

Folders should be lower case and with dashes if it's more than one word (/like-this/)
Files should be Pascal case (LikeThis.cs)

## Namespaces

All files should have their own `using` declarations.  
Do not use `global using`

## Variable Naming Conventions

Class or struct variables should be Pascal case (`LikeThis`)

Local and parameter variables should be Camel case (`likeThis`)

Don't use underscores.

## Control Statements

Always err on the side of early returns/exits, only use else when necessary.

Good:

```cs
public int DoThing(int a) {
    if (a < 0) return -1;

    return a + 10;
}
```

Bad:

```cs
public void DoThing(int a) {
    if (a >= 0) {
        return a + 10;
    } else {
        return -1;
    }
}
```

## UI Files

When creating UI files, if you are writing any meaningful code, it shoud be inside of a `.cs` file and not contained in the `.razor` file.  
Exceptions to this include member variable declarations and simple hash code calculation.

You should also try to make the partial class in the corresponding `.cs` file explicitly state that it inherits from `Panel/WorldPanel` so that intellisense plays nicer with that file.

## UI Data Flow Best Practices

Player pawn code should never write to a UI element unless it is telling the UI element to open or close, and this should be done using the `IMenuScreen` interface.

UI code should avoid writing data directly to the player pawn.  
If a mutation of player pawn state needs to happen, it should be done through a function.

Interactable UI elements shouldn't open on their own (ie. they shouldn't be watching for input on `Tick` to know when to open.)

Generally a UI should read data from the player pawn or another source and display that data.  
A UI element shouldn't have some external entity writing data to it, telling it what to display.
To illustrate this point, look at the below example.

### Bad Example

`HealthDisplay.razor/cs`

```
<root>
  @CurrentHealth
</root>

public partial class HealthDisplay : Panel {
  public int CurrentHealth {get; set;}
}
```

`Player.cs`

```
public partial class Player : AnimatedEntity {
  public int Health {get; set;}
  public UI.HealthDisplay HealthDisplay {get; set;}

  private void OnTick() {
    HealthDisplay.CurrentHealth = Health;
  }
}
```

The greatest reason this is bad is that it closely couples the UI code to the player pawn code and forces the player pawn to be responsible for updating the UI element.

If we were to change the UI element, or swap it out for another one, it requires touching multiple places in our codebase.

We want to strive to make code that requires us to touch as few places as neccessary when we change it later, without abstracting this too greatly and making them difficult to use.

### Good Example

`HealthDisplay.razor/cs`

```
<root>
  @CurrentHealth
</root>

public partial class HealthDisplay : Panel {
  private int CurrentHealth => (Game.LocalPawn as Player)?.Health ?? 0;
}
```

`Player.cs`

```
public partial class Player : AnimatedEntity {
  public int Health {get; set;}
}
```

While this example couples the UI element to the `Player` class, it allows for the player pawn code to not care about UI elements.

If we decide to change the UI element we don't need to do anything to the `Player` class.
