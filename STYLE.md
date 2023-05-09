# Coding Style Guide

These are some style conventions to follow when contributing to the project.

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
