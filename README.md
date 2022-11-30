# Dusty-Arcade-Project
# All Volunteers Must Read this Document
### Version Control Written by: Kyle [KdotIV]
### Coding Standards Written by: Nathan [NoteMEdown]
### License: 
    Copyright (C) Justin Gonzalez - All Rights Reserved
    Unauthorized copying of this file, via any medium is strictly prohibited
    Proprietary and confidential
 
### Last Update: 06/01/2021

# Table of Contents
- [Dusty-Arcade-Project](#dusty-arcade-project)
    - [License:](#license)
    - [Last Update: 06/01/2021](#last-update-02272020)
- [Table of Contents](#table-of-contents)
- [Dev Requirements](#dev-requirements)
  - [System Requirements](#system-requirements)
  - [Software Requirements](#software-requirements)
    - [Backend Devs:](#backend-devs)
- [Initial Git](#initial-git)
- [Version Control Guide - MUST READ](#version-control-guide---must-read)
  - [Naming Conventions](#naming-conventions)
  - [Daily checks](#daily-checks)
- [Coding Standards](#coding-standards)
- [Why?](#why-do-we-need-to-use-the-same-coding-style?)
- [Curly Braces](#curly-braces)
  - [Curly Braces Exceptions](#curly-braces-exceptions)
- [If Statements](#if-statements)
  - [If Statement Exceptions](#if-statement-exceptions)
- [Private/Public/Static](#private/public/static)
- [Hard Coding Variables](#Hard-Coding-Variables)
- [Global Variable and Method Placement](#Global-Variable-and-Method-Placement)
- [Spaces Between Operators](#Spaces-Between-Operators)
- [Underscores for Local Variables](#Underscores-for-Local-Variables)
- [Spaces Between Operators](#Important-Note)

# Dev Requirements
To assure proper workflow is done on this project all devs must have required versions/software or they will be denied commits/mergers.

## System Requirements
For Unity Editor Runtime
| Minimum  |  Windows |  macOS | Linux  |
|---|---|---|---|
|  Operating system version | Windows 7 and Windows 10, 64-bit versions only.  | Sierra 10.12.6+  | Ubuntu 16.04, Ubuntu 18.04, and CentOS 7  |
| CPU  | X64 architecture  | X64 architecture  | X64 architecture with SSE2  |
| Graphics API  | DX10, DX11, and DX12-capable GPUs  | Metal-capable Intel and AMD GPUs  | OpenGL 3.2+/ Vulkan-capable, Nvidia and AMD GPUs.  |
| Additional Requirements  | Hardware vendor officially supported drivers  | Apple officially supported drivers  | Gnome desktop environment running on top of X11 windowing system, Nvidia official proprietary graphics driver or AMD Mesa graphics driver. Other configuration and user environment as provided stock with the supported distribution (Kernel, Compositor, etc.)  |

## Software Requirements

- [GIT] https://git-scm.com/
- Text Editor/IDE that supports C# & .NET development
  - It is preferable the text editor allows GitHub/Git-like plugins. Make sure your favorite toy works well with Unity/Git Bash
  - Ex.
  - Visual Studio Code
  - Visual Studio Community IDE
  - JetBrains Rider
- Unity Editor & Hub
  - 2019.4.9f1 (https://unity3d.com/unity/qa/lts-releases)
  - Universal Windows Platform Build Support Module

### Backend Devs:
- NuGet support
  - JSON

# Initial Git 

Inside Git Bash/Command Line
```
cd <place folder path for cloning here>
```


```
git clone --branch Dev https://github.com/DustyArcade/Dusty-Arcade-Project.git
```

Confirm branch is given
```
git branch -a
```

You should see with * your current branch
```
* Dev
  Stable
  remotes/origin/Dev
  remotes/origin/HEAD -> origin/Stable
  remotes/origin/Release
  remotes/origin/Stable
```

# Version Control Guide - MUST READ

To begin working on a branch you must *check out* not clone or create a branch

First fetch repo then check out your desired branch
```
git fetch --all
//next line
git checkout branchName
```
Dev/Stable branches are protected and will prevent you from committing unless it is checked by KDot. In order to start implementing your work into the repo, you will need to checkout or create a new feature branch that will be merged.

If you want to create a new branch for a potential feature within the repo inform @KdotIV on discord to gain permission. Branches that are randomly created/nested or branches created off the wrong branch will be deleted.

Core functionality and Code Architecture will be following this design: https://docs.google.com/presentation/d/1ILBFTRs2zExK5Bna19RLdcqZ0gLZOB_th8IYXLYCemY/edit?usp=sharing

## Naming Conventions
We will be using proper naming conventions when committing/branching

For branching we follow this tree pattern ```dev/feature-name/component-name ``` and ```yourname-featurename-id ``` for commitments (can use discord usernames).

Examples:
If I'm in the dev tree and I want to make a feature or prototype branch.
```
git checkout -b feature-name/component-name dev 
```
this will not only create the branch but checks it out for you as well. The point of it is to safely create the branch based off dev's origin

when I make commits I will put summary as ```myname-featurename-id ``` then in description ```<description of what I did> ``` then commit it.

## Daily checks
It is important to follow "pull first then push" philosophy so when you try to push your code to ovewrite, it doesn't disappear when you pull and shit breaks.

As things get pushed/merged to Dev, it is important on keeping up to date with current files that core to the overall project. You can take specific files from Dev's repo with cherry picking commands

Checkout to the branch you want the files to go to.

```
git checkout branchname
```
Then check the Dev's commit history and look for the most recent commit ID. Add that ID to the following command.
```
git cherry-pick <commitId>
```

## When to Pull

When you are working on your current branch and you are "behind" on commits that means someone working on your branch has pushed code. *BEFORE* you push your code to the working branch, run a pull command.

```
git pull branchname
```

It is typically easier when you first start your work. Run a fetch on the repo itself. Which updates all current history that was pointed to that repo.
```
git fetch --all
```

## Pruning

Apply pruning whenever there have been merges and you want to clean up old unused branches.

```
git fetch --prune
```

# Coding Standards

## Why do we need to use the same Coding Style?
Using the same coding style will help people look over your code and easily understand what is happening. It will also help everyone to keep themselves more organized. 

## Curly Braces
- Brace starts { and ends } should always be put on their own line (Unless Stated below)
- After each { all lines below it should have one extra TAB in front of it. 
- After each } all lines below it should have one less TAB in front of it.
```
namespace Example.Namespace
{
  public class ExampleClass : MonoBehaviour
  {
    Here would be Tabbed twice, because two { symbols are before it.
        
    private void Example()
    {
      Here is tabbed 3 times.
    }
    Now this would go back to being tabbed twice
  }
}
```

### Curly Braces Exceptions
- If statement leads to a simple return statement
- Method/Function with simple return
```
if(sampleStatement){ return 5; }
```
```
void GetSpeed(){ return speed; }
```

## If Statements
 - All If Statements should have curly braces that follow them for oganization.
 - While it is syntactically correct to not use curly braces for If Statements that that only have one command, in this project, use curly braces for all if statements. (Unless stated below)
```
if(exampleCondition)
{
  singlecommand();
}
```

 ### If Statement Exceptions
- Empty return statements after If Statements do not need curly braces
```
if(exampleCondition) return;
```

## Private/Public/Static
- Unless a variable/class/method NEEDS to be public, or NEEDS to be static, it should not be labeled as static, and should be private.
```
private float speed;
private void UpdateMovement()
{
  ExampleCode();
}
```
- NOTE: All public variables will show up as a field in the editor for the script you put it in.
  - Do NOT make a variable public just to have it show up in the editor, simply make it a serialized field.
```
[SerializeField]
private float speed = 1000.0f;
```

## Hard Coding Variables
- Limit the amount of numbers inside of the code.
- Any value that has a chance to change in the future should be made a variable.
- If you are cutting speed in half (example below), then using a 2 to have cut a value in half is completely acceptable.
```
reducedSpeed = speed / 2
```
- If you want to reduce speed by a certain amount when walking over mud for example, do not hardcode the amount the speed in reduced, instead make a variable (called mudMovementSlow for example), and use that value to reduce the speed. This will yourself or others quickly look at the variables at the top and change them easily.
```
private float mudMovementSlow = 1.5f;
private void walkingOnMud()
{
  speed = speed / mudMovementSlow;
}
```

## Global Variable and Method Placement
- The ordering for every class should be as followed:
  - Using statements
  - Namespace
  - Class Declaration
  - Global Variables
  - Start Method (If the class is using this function) 
  - Update Method (If the class is using this function) 
  - Custom Methods
- As the program grows larger, regions might need to be more strictly enforced, for now regions can be used for your own orginization purposes.
```
using UnityEngine;
using System;

namespace RPG.Control
{
  public class PlayerController : MonoBehaviour
  {
    private int exampleInt;
    private float exampleLong;

    private void Start()
    {
      // Enter Code here
    }

    private void Update()
    {
      // Enter Code here
    }

    private void OtherMethods()
    {
      // Enter Code here
    }
  }
}
```

## Spaces Between Operators 
- ALL operators should have a space between the variables and the operators.
```
speed = speed + 10;
private float exampleFloat = 25f;
if(speed + 5 == 150)
{
  String exampleString = "Your speed: " + speed + "!";
}
```

## Underscores for Local Variables
- ALL local variables, including parameters, should have an _ symbol in front of them.
```
private void changeSpeed(_newSpeed)
{
  float _exampleVariable = 25f;
  speed = _newSpeed + _exampleVariable;
}
```

# Important Note
When in doubt, look at the code already in the dev Branch and see how it is written, and try to follow that style.
