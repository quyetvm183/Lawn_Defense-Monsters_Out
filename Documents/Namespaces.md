# Namespaces in C# and Unity (RGame Namespace)

**Purpose**: Complete guide to understanding namespaces in C#, why this project uses the `RGame` namespace, and how to work with namespaces effectively in Unity.

**What This Document Covers**:
- C# namespace fundamentals
- Why this project uses `namespace RGame`
- How to add scripts to the namespace
- Best practices for namespace organization
- Troubleshooting namespace-related errors

**Related Documentation**:
- See `00_Unity_Fundamentals.md` for C# basics
- See `01_Project_Architecture.md` for code organization
- See `13_Code_Examples.md` for practical coding patterns
- See `11_Troubleshooting.md` for compiler error solutions

---

## Table of Contents

1. [What Are Namespaces?](#what-are-namespaces)
2. [Why This Project Uses RGame](#why-this-project-uses-rgame)
3. [How Namespaces Work](#how-namespaces-work)
4. [Adding Scripts to RGame Namespace](#adding-scripts-to-rgame-namespace)
5. [Using Classes from Different Namespaces](#using-classes-from-different-namespaces)
6. [Advanced Namespace Techniques](#advanced-namespace-techniques)
7. [Best Practices](#best-practices)
8. [Troubleshooting](#troubleshooting)

---

## What Are Namespaces?

### Simple Explanation

Think of a namespace like **folders for your code**:

```
Without Namespaces (messy):
┌───────────────────────────┐
│ All Classes in One Place  │
├───────────────────────────┤
│ GameManager               │
│ MenuManager               │
│ Enemy                     │
│ Player                    │
│ ThirdPartyGameManager     │  ← Name collision!
│ ThirdPartyEnemy           │  ← Name collision!
└───────────────────────────┘

With Namespaces (organized):
┌───────────────────────────┐
│ RGame Namespace           │
├───────────────────────────┤
│ RGame.GameManager         │
│ RGame.MenuManager         │
│ RGame.Enemy               │
│ RGame.Player              │
└───────────────────────────┘
┌───────────────────────────┐
│ ThirdParty Namespace      │
├───────────────────────────┤
│ ThirdParty.GameManager    │  ← No collision!
│ ThirdParty.Enemy          │  ← No collision!
└───────────────────────────┘
```

**Real-World Analogy**:

Imagine a library with books:

```
Without namespaces:
- "Animals" by John Smith
- "Animals" by Jane Doe     ← Which one do you want?
- "Animals" by Bob Johnson

With namespaces:
- Science.Animals by John Smith
- Fiction.Animals by Jane Doe
- Textbook.Animals by Bob Johnson  ← Clear which one!
```

---

### Technical Definition

A **namespace** is a C# feature that groups related classes, interfaces, structs, and enums together.

**Basic Syntax**:

```csharp
namespace RGame
{
    public class GameManager : MonoBehaviour
    {
        // Class code here
    }
}
```

**Without namespace** (NOT recommended):

```csharp
public class GameManager : MonoBehaviour
{
    // This GameManager is in the "global namespace"
    // Can conflict with other GameManager classes!
}
```

---

### Why Namespaces Exist

**Problem**: Without namespaces, these two files cause a conflict:

**File 1: YourGameManager.cs**
```csharp
public class GameManager : MonoBehaviour
{
    public void StartGame() { /* your code */ }
}
```

**File 2: ThirdPartyPlugin.cs** (from an asset you imported)
```csharp
public class GameManager : MonoBehaviour
{
    public void Initialize() { /* their code */ }
}
```

**Result**: Compiler error!
```
error CS0101: The namespace 'global' already contains a definition for 'GameManager'
```

**Solution**: Use namespaces!

**File 1: YourGameManager.cs**
```csharp
namespace RGame
{
    public class GameManager : MonoBehaviour
    {
        public void StartGame() { /* your code */ }
    }
}
```

**File 2: ThirdPartyPlugin.cs**
```csharp
namespace ThirdPartyPlugin
{
    public class GameManager : MonoBehaviour
    {
        public void Initialize() { /* their code */ }
    }
}
```

**Result**: No conflict! You have `RGame.GameManager` and `ThirdPartyPlugin.GameManager`.

---

## Why This Project Uses RGame

This project wraps all game code in `namespace RGame` for three main reasons:

### 1. **Organization**

All project code is clearly identified:

```csharp
namespace RGame
{
    public class GameManager : MonoBehaviour { }
    public class Enemy : MonoBehaviour { }
    public class Player_Archer : Enemy { }
    // ... all game classes
}
```

**Benefits**:
- Easy to identify which classes belong to this project
- Clear separation from Unity classes (`UnityEngine.UI`, etc.)
- Clear separation from third-party assets

### 2. **Avoid Name Collisions**

Many Unity assets use common names:

```
Common class names that might conflict:
- GameManager
- MenuManager
- SoundManager
- Enemy
- Player
- Weapon
- Item
```

With `namespace RGame`, you can safely import assets without conflicts:

```csharp
// Your code:
namespace RGame
{
    public class GameManager : MonoBehaviour { }
}

// Third-party asset (e.g., "Super Game Framework"):
namespace SuperGameFramework
{
    public class GameManager : MonoBehaviour { }
}

// Both can coexist! No conflict.
```

### 3. **Encapsulation**

Namespace provides logical grouping:

```
RGame namespace contains:
├─ Core Systems (GameManager, MenuManager, SoundManager)
├─ Character Systems (Player, Enemy, Controller2D)
├─ UI Systems (UI_UI, MapControllerUI, MenuManager)
├─ Helper Classes (AnimationHelper, CheckTargetHelper)
└─ Data Classes (GlobalValue, UpgradedCharacterParameter)
```

**Like a folder structure**, but for code organization.

---

## How Namespaces Work

### Basic Declaration

Every script in this project starts with:

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGame
{
    public class Enemy : MonoBehaviour
    {
        // Class code here
    }
}
```

**Structure Breakdown**:

```csharp
// 1. Using directives (import other namespaces)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 2. Declare your namespace
namespace RGame
{
    // 3. Everything inside this block is part of RGame namespace
    public class Enemy : MonoBehaviour
    {
        public int health = 100;

        public void TakeDamage(int damage)
        {
            health -= damage;
        }
    }
}
// 4. Closing brace ends the namespace
```

---

### Accessing Classes in the Same Namespace

**Classes in the same namespace can reference each other directly**:

```csharp
// File: Enemy.cs
namespace RGame
{
    public class Enemy : MonoBehaviour
    {
        public int health = 100;
    }
}

// File: Player.cs
namespace RGame
{
    public class Player : MonoBehaviour
    {
        void AttackEnemy()
        {
            Enemy enemy = FindObjectOfType<Enemy>();  // ← Can use directly!
            // No need for "RGame.Enemy" or "using RGame;"
        }
    }
}
```

**Why**: Both `Enemy` and `Player` are in the `RGame` namespace, so they can see each other.

---

### Accessing Classes from Different Namespaces

**Option 1: Fully Qualified Name** (use full path)

```csharp
namespace RGame
{
    public class GameManager : MonoBehaviour
    {
        void Start()
        {
            // Access Unity UI classes with full path
            UnityEngine.UI.Text myText = GetComponent<UnityEngine.UI.Text>();
        }
    }
}
```

**Option 2: Using Directive** (import namespace)

```csharp
using UnityEngine.UI;  // ← Import the UI namespace

namespace RGame
{
    public class GameManager : MonoBehaviour
    {
        void Start()
        {
            Text myText = GetComponent<Text>();  // ← Can use short name!
        }
    }
}
```

**Option 2 is preferred** because it's cleaner and easier to read.

---

### Visual Flow of Using Directives

```csharp
// These are like "import" statements in other languages
using System.Collections;         // Import System.Collections namespace
using System.Collections.Generic; // Import System.Collections.Generic namespace
using UnityEngine;                // Import UnityEngine namespace
using UnityEngine.UI;             // Import UnityEngine.UI namespace

namespace RGame
{
    public class MenuManager : MonoBehaviour
    {
        public Text scoreText;  // ← From UnityEngine.UI (imported above)

        void Start()
        {
            // Can use IEnumerator from System.Collections (imported above)
            StartCoroutine(CountdownCo());
        }

        IEnumerator CountdownCo()
        {
            // Can use List<T> from System.Collections.Generic (imported above)
            List<int> numbers = new List<int> { 3, 2, 1 };

            foreach (int num in numbers)
            {
                scoreText.text = num.ToString();
                yield return new WaitForSeconds(1f);  // ← From UnityEngine (imported above)
            }
        }
    }
}
```

---

## Adding Scripts to RGame Namespace

### When to Add Scripts to RGame

**Add to RGame namespace** if:
- ✅ Script is specific to this game
- ✅ Script is a core system (manager, controller, helper)
- ✅ Script is a game object behavior (player, enemy, item)
- ✅ Script will be used across multiple scenes

**Don't add to RGame namespace** if:
- ❌ Script is a generic tool (could be reused in other projects)
- ❌ Script is an editor tool (Unity Editor customization)
- ❌ You're prototyping and script might be deleted

---

### Step-by-Step: Adding a New Script

**Step 1: Create Script**

In Unity Editor:
1. Right-click in Project window → Create → C# Script
2. Name it `MyNewScript`
3. Double-click to open in your code editor

**Step 2: Add Namespace**

Unity generates this:

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyNewScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
```

**Change it to this**:

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGame  // ← Add this
{              // ← Add opening brace
    public class MyNewScript : MonoBehaviour
    {
        void Start()
        {

        }

        void Update()
        {

        }
    }
}              // ← Add closing brace
```

**Step 3: Save and Compile**

Unity will automatically recompile the script.

---

### Template for New Scripts

**Copy this template** for all new scripts:

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGame
{
    /// <summary>
    /// Brief description of what this class does.
    /// </summary>
    public class MyClassName : MonoBehaviour
    {
        [Header("CONFIGURATION")]
        public int myVariable = 10;

        [Header("STATE")]
        private bool isActive = false;

        void Start()
        {
            // Initialization code
        }

        void Update()
        {
            // Per-frame code
        }

        // Your custom methods here
        public void MyMethod()
        {

        }
    }
}
```

---

## Using Classes from Different Namespaces

### Scenario 1: Accessing RGame Classes from Another Script

**Your script** (in `RGame` namespace):

```csharp
namespace RGame
{
    public class GameManager : MonoBehaviour
    {
        public void StartGame()
        {
            Debug.Log("Game started!");
        }
    }
}
```

**Third-party script** (in different namespace):

```csharp
using RGame;  // ← Import RGame namespace

namespace ThirdPartyTool
{
    public class ToolScript : MonoBehaviour
    {
        void Start()
        {
            GameManager gm = FindObjectOfType<GameManager>();  // ← Can access!
            gm.StartGame();
        }
    }
}
```

---

### Scenario 2: Accessing Third-Party Classes from RGame

**Third-party asset** (e.g., "Dialogue System"):

```csharp
namespace DialogueSystem
{
    public class DialogueManager : MonoBehaviour
    {
        public void ShowDialogue(string text)
        {
            Debug.Log(text);
        }
    }
}
```

**Your script**:

```csharp
using DialogueSystem;  // ← Import their namespace

namespace RGame
{
    public class NPC : MonoBehaviour
    {
        void OnTriggerEnter2D(Collider2D other)
        {
            DialogueManager dm = FindObjectOfType<DialogueManager>();  // ← Can access!
            dm.ShowDialogue("Hello, traveler!");
        }
    }
}
```

---

### Scenario 3: Name Collision - Same Class Name

**Problem**: Both `RGame` and a third-party asset have a class named `GameManager`.

**Bad Solution** (causes error):

```csharp
using RGame;
using ThirdPartyAsset;  // Both have GameManager class!

namespace MyCode
{
    public class MyScript : MonoBehaviour
    {
        void Start()
        {
            GameManager gm = new GameManager();  // ← ERROR: Ambiguous reference!
        }
    }
}
```

**Good Solution 1: Fully Qualified Names**

```csharp
namespace MyCode
{
    public class MyScript : MonoBehaviour
    {
        void Start()
        {
            RGame.GameManager rgm = FindObjectOfType<RGame.GameManager>();
            ThirdPartyAsset.GameManager tpm = FindObjectOfType<ThirdPartyAsset.GameManager>();
        }
    }
}
```

**Good Solution 2: Alias**

```csharp
using RGM = RGame.GameManager;  // ← Create alias
using TPM = ThirdPartyAsset.GameManager;  // ← Create alias

namespace MyCode
{
    public class MyScript : MonoBehaviour
    {
        void Start()
        {
            RGM rgm = FindObjectOfType<RGM>();  // ← Clear which one!
            TPM tpm = FindObjectOfType<TPM>();
        }
    }
}
```

---

## Advanced Namespace Techniques

### 1. Nested Namespaces

You can create **sub-namespaces** for better organization:

```csharp
namespace RGame.Enemies
{
    public class Goblin : Enemy { }
    public class Skeleton : Enemy { }
}

namespace RGame.Players
{
    public class Warrior : Player { }
    public class Archer : Player { }
}

namespace RGame.UI
{
    public class MenuManager : MonoBehaviour { }
    public class MapControllerUI : MonoBehaviour { }
}
```

**Directory structure visualization**:

```
RGame (namespace)
├─ Enemies (sub-namespace)
│  ├─ Goblin
│  └─ Skeleton
├─ Players (sub-namespace)
│  ├─ Warrior
│  └─ Archer
└─ UI (sub-namespace)
   ├─ MenuManager
   └─ MapControllerUI
```

**Accessing nested namespace classes**:

```csharp
using RGame.Enemies;  // Import sub-namespace

namespace RGame
{
    public class GameManager : MonoBehaviour
    {
        void Start()
        {
            Goblin goblin = Instantiate(goblinPrefab);  // Can access directly
        }
    }
}
```

---

### 2. Global Using Directives (C# 10+)

**If you use the same namespace in EVERY file**, add a global using:

**Create file: GlobalUsings.cs**

```csharp
global using System.Collections;
global using System.Collections.Generic;
global using UnityEngine;
```

**Now all files automatically have these imports**:

```csharp
// No need for "using UnityEngine;" anymore!

namespace RGame
{
    public class MyScript : MonoBehaviour  // ← Works without "using UnityEngine;"
    {
        void Start()
        {
            Debug.Log("Hello!");  // ← Works!
        }
    }
}
```

**Note**: This feature requires C# 9.0+ and Unity 2021.2+.

---

### 3. Namespace Aliases

**Use aliases** to shorten long namespace names:

```csharp
using UI = UnityEngine.UI;  // ← Alias for UnityEngine.UI

namespace RGame
{
    public class MenuManager : MonoBehaviour
    {
        public UI.Text scoreText;  // ← Shorter than "UnityEngine.UI.Text"
        public UI.Button playButton;
        public UI.Slider healthBar;
    }
}
```

---

### 4. File-Scoped Namespaces (C# 10+)

**Old syntax** (verbose):

```csharp
namespace RGame
{
    public class GameManager : MonoBehaviour
    {
        // Code here
    }
}
```

**New syntax** (cleaner):

```csharp
namespace RGame;  // ← File-scoped namespace (one line!)

public class GameManager : MonoBehaviour
{
    // Code here - automatically in RGame namespace
}
```

**Benefits**: Less indentation, cleaner code.

**Requirement**: C# 10+ (Unity 2021.2+)

---

## Best Practices

### 1. Consistent Namespace Usage

**✅ DO**: Use `namespace RGame` in all game-specific scripts

```csharp
// ✅ Good
namespace RGame
{
    public class GameManager : MonoBehaviour { }
}
```

**❌ DON'T**: Mix namespaced and non-namespaced scripts

```csharp
// ❌ Bad
public class GameManager : MonoBehaviour { }  // No namespace
```

---

### 2. Match Namespace to Folder Structure

**Folder structure**:
```
Assets/
└─ _MonstersOut/
   └─ Scripts/
      ├─ Managers/
      │  └─ GameManager.cs
      ├─ Player/
      │  └─ Player_Archer.cs
      └─ Enemy/
         └─ Enemy.cs
```

**Option 1: Single namespace** (current project approach)

```csharp
// All scripts use: namespace RGame
```

**Option 2: Nested namespaces** (more organized for large projects)

```csharp
// GameManager.cs:
namespace RGame.Managers
{
    public class GameManager : MonoBehaviour { }
}

// Player_Archer.cs:
namespace RGame.Players
{
    public class Player_Archer : Enemy { }
}

// Enemy.cs:
namespace RGame.Enemies
{
    public class Enemy : MonoBehaviour { }
}
```

**Recommendation**: Use **Option 1** (single namespace) unless your project has 200+ scripts.

---

### 3. Use Clear Namespace Names

**✅ DO**: Use descriptive, unique names

```csharp
namespace LawnDefenseMonsters { }  // Unique, descriptive
namespace RGame { }                // Short, unique
```

**❌ DON'T**: Use generic names

```csharp
namespace Game { }      // Too generic, conflicts likely
namespace MyGame { }    // Too generic
namespace Project { }   // Too generic
```

---

### 4. Group Related Classes

**✅ DO**: Keep related classes in the same namespace

```csharp
namespace RGame
{
    public class Enemy : MonoBehaviour { }
    public class SmartEnemyGrounded : Enemy { }
    public class EnemyHealth : MonoBehaviour { }
}
```

**❌ DON'T**: Split tightly coupled classes into different namespaces

```csharp
// ❌ Bad: Enemy and SmartEnemyGrounded are closely related!
namespace RGame.Core
{
    public class Enemy : MonoBehaviour { }
}

namespace RGame.Advanced
{
    public class SmartEnemyGrounded : Enemy { }
}
```

---

### 5. Document Namespace Purpose

Add a comment at the top of your namespace:

```csharp
using UnityEngine;

/// <summary>
/// RGame namespace contains all core game systems for "Lawn Defense: Monsters Out".
/// Includes managers, player/enemy systems, UI, helpers, and data classes.
/// </summary>
namespace RGame
{
    public class GameManager : MonoBehaviour
    {
        // ...
    }
}
```

---

## Troubleshooting

### Problem 1: "The type or namespace name could not be found"

**Error Message**:
```
error CS0246: The type or namespace name 'Enemy' could not be found
(are you missing a using directive or an assembly reference?)
```

**Cause**: Script is trying to use a class from a different namespace without importing it.

**Example**:

```csharp
// Enemy.cs
namespace RGame
{
    public class Enemy : MonoBehaviour { }
}

// ThirdPartyScript.cs
namespace ThirdParty
{
    public class ThirdPartyScript : MonoBehaviour
    {
        void Start()
        {
            Enemy enemy = FindObjectOfType<Enemy>();  // ← ERROR!
        }
    }
}
```

**Solution**: Add `using RGame;`

```csharp
using RGame;  // ← Import the namespace

namespace ThirdParty
{
    public class ThirdPartyScript : MonoBehaviour
    {
        void Start()
        {
            Enemy enemy = FindObjectOfType<Enemy>();  // ← Works!
        }
    }
}
```

---

### Problem 2: "The namespace 'XXX' already contains a definition for 'YYY'"

**Error Message**:
```
error CS0101: The namespace 'RGame' already contains a definition for 'GameManager'
```

**Cause**: Two classes with the same name in the same namespace.

**Example**:

```csharp
// File: GameManager.cs
namespace RGame
{
    public class GameManager : MonoBehaviour { }
}

// File: GameManagerNew.cs
namespace RGame
{
    public class GameManager : MonoBehaviour { }  // ← ERROR: Duplicate!
}
```

**Solution 1**: Rename one of the classes

```csharp
// File: GameManagerNew.cs
namespace RGame
{
    public class GameManagerV2 : MonoBehaviour { }  // ← Different name
}
```

**Solution 2**: Move to different namespace

```csharp
// File: GameManagerNew.cs
namespace RGame.Experimental  // ← Different namespace
{
    public class GameManager : MonoBehaviour { }
}
```

---

### Problem 3: "Ambiguous reference between 'RGame.X' and 'ThirdParty.X'"

**Error Message**:
```
error CS0104: 'GameManager' is an ambiguous reference between
'RGame.GameManager' and 'ThirdPartyAsset.GameManager'
```

**Cause**: Both namespaces imported, both have same class name.

**Example**:

```csharp
using RGame;
using ThirdPartyAsset;

public class MyScript : MonoBehaviour
{
    void Start()
    {
        GameManager gm = new GameManager();  // ← Which one?!
    }
}
```

**Solution 1**: Remove one `using` statement and use fully qualified name

```csharp
using RGame;  // Only import RGame

public class MyScript : MonoBehaviour
{
    void Start()
    {
        GameManager rgm = new GameManager();  // RGame.GameManager
        ThirdPartyAsset.GameManager tpm = new ThirdPartyAsset.GameManager();  // Full path
    }
}
```

**Solution 2**: Use aliases

```csharp
using RGM = RGame.GameManager;
using TPM = ThirdPartyAsset.GameManager;

public class MyScript : MonoBehaviour
{
    void Start()
    {
        RGM rgm = new RGM();  // Clear!
        TPM tpm = new TPM();  // Clear!
    }
}
```

---

### Problem 4: MonoBehaviour Script Not Attaching in Inspector

**Symptom**: Can't drag script onto GameObject in Inspector.

**Cause**: Script has namespace, but you're using old Unity version that doesn't support namespaced MonoBehaviour in Inspector.

**Check Unity Version**: Unity 5.3+ supports namespaced MonoBehaviour.

**Solution 1**: Update Unity to 5.3 or later.

**Solution 2**: Remove namespace (NOT recommended):

```csharp
// Remove namespace temporarily
public class MyScript : MonoBehaviour
{
    // ...
}
```

**Solution 3**: Use `AddComponent<>()` in code instead:

```csharp
namespace RGame
{
    public class GameManager : MonoBehaviour
    {
        void Start()
        {
            gameObject.AddComponent<MyScript>();
        }
    }
}
```

---

### Problem 5: Serialization Issues with Namespaced Classes

**Symptom**: Custom class data not saving in Inspector.

**Example**:

```csharp
namespace RGame
{
    [System.Serializable]
    public class CharacterData
    {
        public string name;
        public int health;
    }

    public class GameManager : MonoBehaviour
    {
        public CharacterData player;  // ← Might not serialize correctly in old Unity
    }
}
```

**Cause**: Very old Unity versions (<  5.0) had serialization issues with namespaced classes.

**Solution 1**: Update Unity (5.0+).

**Solution 2**: Use fully qualified name in attribute:

```csharp
namespace RGame
{
    [System.Serializable]
    [UnityEngine.SerializeField]  // ← Fully qualified
    public class CharacterData
    {
        public string name;
        public int health;
    }
}
```

---

## Summary

**Key Takeaways**:

1. **Namespaces** are like folders for code—they organize classes and prevent name collisions
2. **RGame namespace** wraps all game-specific code in this project
3. **Using directives** import classes from other namespaces (`using UnityEngine;`)
4. **Classes in the same namespace** can reference each other directly
5. **Classes in different namespaces** require `using` directive or fully qualified name
6. **Best practice**: Always add new game scripts to `namespace RGame`

**Essential Patterns**:

```csharp
// 1. Standard script template
using UnityEngine;

namespace RGame
{
    public class MyScript : MonoBehaviour
    {
        // Your code
    }
}

// 2. Importing other namespaces
using RGame;  // Import RGame namespace

namespace MyNamespace
{
    public class MyScript : MonoBehaviour
    {
        void Start()
        {
            GameManager gm = FindObjectOfType<GameManager>();  // Can access RGame classes
        }
    }
}

// 3. Handling name collisions with aliases
using RGM = RGame.GameManager;
using TPM = ThirdParty.GameManager;

public class MyScript : MonoBehaviour
{
    void Start()
    {
        RGM myGM = new RGM();
        TPM theirGM = new TPM();
    }
}

// 4. Nested namespaces
namespace RGame.Managers
{
    public class GameManager : MonoBehaviour { }
}

namespace RGame.UI
{
    public class MenuManager : MonoBehaviour { }
}
```

**Quick Reference**:

```csharp
// Access class in same namespace
Enemy enemy = new Enemy();  // Direct access

// Access class in different namespace (Option 1: using directive)
using ThirdParty;
ThirdPartyClass obj = new ThirdPartyClass();

// Access class in different namespace (Option 2: fully qualified name)
ThirdParty.ThirdPartyClass obj = new ThirdParty.ThirdPartyClass();

// Handle name collision with alias
using MyEnemy = RGame.Enemy;
using TheirEnemy = ThirdParty.Enemy;
```

**Next Steps**:
- See `13_Code_Examples.md` for practical coding patterns
- See `01_Project_Architecture.md` for overall code organization
- See `11_Troubleshooting.md` for compiler error solutions
