# Unity Fundamentals - From Zero to Hero

**Target Audience:** Complete beginners with no Unity experience
**Prerequisites:** Basic understanding of programming concepts (variables, functions)
**Estimated Reading Time:** 2-3 hours
**Related Documents:** → `01_Project_Architecture.md`, `99_Glossary.md`

---

## Table of Contents
1. [What is Unity?](#1-what-is-unity)
2. [Unity Editor Interface](#2-unity-editor-interface)
3. [GameObject & Components](#3-gameobject--components)
4. [Prefabs (Template Objects)](#4-prefabs-template-objects)
5. [Scenes](#5-scenes)
6. [Scripting Basics](#6-scripting-basics)
7. [Input System](#7-input-system)
8. [Physics System](#8-physics-system)
9. [UI System (Canvas)](#9-ui-system-canvas)
10. [Resources & Asset Management](#10-resources--asset-management)
11. [Best Practices](#11-best-practices)

---

## 1. What is Unity?

### 1.1 Game Engine Overview

**Unity** is a game development platform (called a "game engine") that provides tools and systems to create interactive 2D and 3D applications, primarily games. Think of it as a **comprehensive toolkit** where you can:

- **Design game worlds** visually (drag-and-drop interface)
- **Write game logic** using C# programming language
- **Add physics** (gravity, collisions, forces)
- **Create animations** (character movement, UI transitions)
- **Manage assets** (images, sounds, 3D models)
- **Build for multiple platforms** (PC, Mobile, Console, Web)

**Analogy:** Unity is like **Microsoft Word for games** - Word helps you create documents, Unity helps you create games.

### 1.2 Unity Workflow

The typical Unity development workflow:

```
1. CREATE
   ↓
Design scene → Add GameObjects → Attach Components
   ↓
2. SCRIPT
   ↓
Write C# code → Attach scripts to GameObjects → Define behavior
   ↓
3. TEST
   ↓
Press Play button → Test in Game view → Debug issues
   ↓
4. ITERATE
   ↓
Fix bugs → Add features → Improve visuals
   ↓
5. BUILD
   ↓
Export to target platform (Android, iOS, PC, etc.)
```

### 1.3 Core Terminology

Before we dive deeper, understand these fundamental terms:

| Term | Simple Explanation |
|------|-------------------|
| **Scene** | A level or screen in your game (like "Main Menu" or "Level 1") |
| **GameObject** | Any object in your game (player, enemy, camera, button) |
| **Component** | A piece of functionality attached to a GameObject |
| **Prefab** | A reusable template for GameObjects |
| **Asset** | Any file in your project (image, sound, script) |
| **Inspector** | Panel showing properties of selected GameObject |
| **Hierarchy** | List of all GameObjects in current scene |

---

## 2. Unity Editor Interface

When you open Unity, you'll see several panels. Let's break down each one:

### 2.1 Editor Layout Diagram

```
┌───────────────────────────────────────────────────────────────────────┐
│  Menu Bar: File  Edit  Assets  GameObject  Component  Window  Help    │
├─────────────────┬─────────────────────────────────────┬───────────────┤
│                 │                                     │               │
│   HIERARCHY     │         SCENE VIEW                  │   INSPECTOR   │
│                 │                                     │               │
│  Canvas         │   ┌─────────────────────────────┐   │  ┌─────────┐ │
│  ├─ Player      │   │                             │   │  │Transform│ │
│  ├─ Enemy       │   │    [Visual game world]      │   │  │         │ │
│  │  └─ Health   │   │                             │   │  │Position │ │
│  ├─ Camera      │   │    Drag/select objects here │   │  │ X: 0    │ │
│  └─ Managers    │   │                             │   │  │ Y: 0    │ │
│                 │   └─────────────────────────────┘   │  │ Z: 0    │ │
│                 │                                     │  └─────────┘ │
├─────────────────┼─────────────────────────────────────┤               │
│                 │                                     │  Components: │
│   PROJECT       │         GAME VIEW                   │  - Rigidbody │
│                 │                                     │  - Collider  │
│  Assets/        │   ┌─────────────────────────────┐   │  - Script    │
│  ├─ Scenes      │   │                             │   │              │
│  ├─ Scripts     │   │  [What player sees]         │   │              │
│  ├─ Prefabs     │   │                             │   │              │
│  ├─ Sprites     │   │  (Press Play to test)       │   │              │
│  └─ Audio       │   │                             │   │              │
│                 │   └─────────────────────────────┘   │              │
├─────────────────┴─────────────────────────────────────┴───────────────┤
│  CONSOLE - Debug messages, errors, warnings appear here               │
│  ▶ "Player took damage: 10"                                           │
│  ⚠ "Warning: No AudioSource found"                                    │
│  ❌ "Error: NullReferenceException on line 42"                        │
└───────────────────────────────────────────────────────────────────────┘
```

### 2.2 Panel Descriptions

#### **Hierarchy Panel** (Top-Left)
**Purpose:** Shows all GameObjects in the current scene as a tree structure

**What you'll do here:**
- View all objects in your scene
- Organize objects in parent-child relationships
- Create new objects (Right-click → Create)
- Delete objects (Select → Delete key)

**Example:**
```
Player (parent)
├─ PlayerSprite (child - the visual)
├─ WeaponHolder (child - holds weapon)
└─ HealthBar (child - shows HP)
```

#### **Scene View** (Center-Top)
**Purpose:** Visual editor where you design your game world

**What you'll do here:**
- Drag and position GameObjects
- Scale and rotate objects
- Navigate the 3D/2D space
- Select objects to edit

**Controls:**
- **Mouse Wheel:** Zoom in/out
- **Middle Mouse Drag:** Pan camera
- **Right Click Drag:** Rotate view (3D)
- **Q/W/E/R/T:** Tool selection (Move, Rotate, Scale, etc.)

#### **Game View** (Center-Bottom)
**Purpose:** Shows what the player will see when playing

**What you'll do here:**
- Press **Play button** to test your game
- See the actual game as it runs
- Test gameplay and mechanics
- **WARNING:** Changes made during Play mode are lost when you stop!

#### **Inspector Panel** (Right)
**Purpose:** Shows and edits properties of the selected GameObject

**What you'll do here:**
- Modify GameObject properties (position, scale, rotation)
- Add/remove Components
- Adjust Component settings
- Assign references between objects

**Example Inspector for a Player GameObject:**
```
┌─────────────────────────────────────┐
│ GameObject: Player            [✓]   │ ← Active checkbox
├─────────────────────────────────────┤
│ Tag: Player        Layer: Default   │ ← Identification
├─────────────────────────────────────┤
│ ▼ Transform                         │ ← Position/Rotation/Scale
│   Position  X: 0    Y: 0    Z: 0    │
│   Rotation  X: 0    Y: 0    Z: 0    │
│   Scale     X: 1    Y: 1    Z: 1    │
├─────────────────────────────────────┤
│ ▼ Sprite Renderer                   │ ← Visual appearance
│   Sprite: [PlayerImage]             │
│   Color: [White]                    │
├─────────────────────────────────────┤
│ ▼ Rigidbody 2D                      │ ← Physics
│   Mass: 1                           │
│   Gravity Scale: 0                  │
├─────────────────────────────────────┤
│ ▼ Box Collider 2D                   │ ← Collision
│   Size: X: 1    Y: 1                │
├─────────────────────────────────────┤
│ ▼ Player Controller (Script)        │ ← Custom behavior
│   Move Speed: 5                     │
│   Jump Force: 10                    │
└─────────────────────────────────────┘
```

#### **Project Panel** (Bottom-Left)
**Purpose:** File browser showing all assets in your project

**What you'll do here:**
- Browse folders and files
- Import new assets (drag files into panel)
- Create new assets (Right-click → Create)
- Organize project files

**Typical Structure:**
```
Assets/
├─ Scenes/          (.unity files - levels/menus)
├─ Scripts/         (.cs files - C# code)
├─ Prefabs/         (.prefab files - templates)
├─ Sprites/         (.png, .jpg - images)
├─ Audio/           (.wav, .mp3 - sounds)
├─ Animations/      (.anim - animation clips)
└─ Resources/       (loadable at runtime)
```

#### **Console Panel** (Bottom)
**Purpose:** Displays messages, warnings, and errors from your code

**What you'll do here:**
- Read error messages when something breaks
- See Debug.Log() messages from your scripts
- Track warnings about potential issues
- Double-click errors to jump to code line

**Message Types:**
- ℹ️ **Log:** Information messages (white)
- ⚠️ **Warning:** Potential issues (yellow)
- ❌ **Error:** Code errors that prevent running (red)

---

## 3. GameObject & Components

This is the **most important concept** in Unity. Everything in your game is built from GameObjects and Components.

### 3.1 What is a GameObject?

**Definition:** A GameObject is a **container** or **box** that holds Components. By itself, a GameObject does nothing - it's just an empty container with a position in the game world.

**Analogy:** Think of a GameObject like a **phone case**:
- The case itself is just a shell (GameObject)
- You add functionality by inserting components (phone battery, camera, screen)
- Different combinations create different devices

**Every GameObject has:**
1. **Name:** Identifier shown in Hierarchy ("Player", "Enemy", "Camera")
2. **Transform:** Position, Rotation, and Scale (ALWAYS present)
3. **Tag:** Label for identification ("Player", "Enemy", "Ground")
4. **Layer:** Used for collision filtering and rendering

### 3.2 GameObject Hierarchy

GameObjects can be **parents** and **children**, forming a tree structure:

```
Player (Parent GameObject)
├─ Transform         (X:0, Y:0, Z:0)  ← Parent position
│
├─ Sprite            (Child GameObject)
│  └─ Transform      (X:0, Y:0, Z:0)  ← Relative to parent!
│
├─ WeaponHolder      (Child GameObject)
│  └─ Transform      (X:1, Y:0, Z:0)  ← 1 unit right of parent
│     │
│     └─ Sword       (Grandchild GameObject)
│        └─ Transform (X:0, Y:0, Z:0) ← Relative to WeaponHolder!
│
└─ HealthBar         (Child GameObject)
   └─ Transform      (X:0, Y:1, Z:0)  ← 1 unit above parent
```

**Important Rules:**
- **Child positions are relative to parent** - If parent moves, children move with it
- **If parent is destroyed, children are destroyed** too
- **If parent is disabled, children are disabled** too
- This is useful for organizing complex objects (like a character with weapons and UI)

### 3.3 What is a Component?

**Definition:** A Component is a **piece of functionality** that you attach to a GameObject to make it do something.

**Analogy:** Components are like **apps on a phone**:
- Phone (GameObject) + Camera app (Component) = Takes photos
- Phone (GameObject) + GPS app (Component) = Navigation
- Phone (GameObject) + Music app (Component) = Plays music

**Every Component:**
- Must be attached to a GameObject
- Can access other components on the same GameObject
- Can be enabled/disabled independently
- Has properties you can adjust in the Inspector

### 3.4 Built-in Components

Unity provides many built-in components:

#### **Transform Component**
**Purpose:** Defines position, rotation, and scale
**Present on:** Every GameObject (cannot be removed)

```
Transform
├─ Position: Where the object is (X, Y, Z coordinates)
├─ Rotation: How the object is rotated (X, Y, Z degrees)
└─ Scale: How big the object is (X, Y, Z multipliers)
```

**Code Access:**
```csharp
// Get position
Vector3 pos = transform.position;

// Move object
transform.position = new Vector3(5, 0, 0);  // Move to (5, 0, 0)
transform.position += new Vector3(1, 0, 0); // Move right by 1 unit

// Rotate object
transform.Rotate(0, 0, 90);  // Rotate 90 degrees on Z-axis

// Scale object
transform.localScale = new Vector3(2, 2, 1);  // Make 2x bigger
```

#### **Renderer Components** (Make things visible)

**SpriteRenderer** - For 2D images
```csharp
SpriteRenderer sr = GetComponent<SpriteRenderer>();
sr.sprite = mySprite;       // Change image
sr.color = Color.red;       // Tint red
sr.flipX = true;            // Flip horizontally
sr.enabled = false;         // Make invisible
```

**MeshRenderer** - For 3D models (not used in this 2D project)

#### **Collider Components** (Detect collisions)

**Purpose:** Define the collision shape of an object

**BoxCollider2D** - Rectangular collision area (2D)
```csharp
BoxCollider2D collider = GetComponent<BoxCollider2D>();
collider.size = new Vector2(1, 2);      // Width: 1, Height: 2
collider.offset = new Vector2(0, 0.5);  // Shift collision box
collider.isTrigger = true;              // Trigger instead of solid collision
```

**CircleCollider2D** - Circular collision area (2D)
```csharp
CircleCollider2D collider = GetComponent<CircleCollider2D>();
collider.radius = 0.5f;     // Radius of circle
collider.isTrigger = false; // Solid collision
```

**Trigger vs. Collider:**
- **Collider (isTrigger = false):** Physical, blocks movement (walls, ground)
- **Trigger (isTrigger = true):** Non-physical, detects overlap (pickup items, detection zones)

#### **Rigidbody2D Component** (Adds physics)

**Purpose:** Makes GameObject respond to physics (gravity, forces, velocity)

```csharp
Rigidbody2D rb = GetComponent<Rigidbody2D>();

// Movement
rb.velocity = new Vector2(5, 0);        // Move right at 5 units/second
rb.AddForce(new Vector2(10, 0));        // Apply force (gradual acceleration)

// Configuration
rb.gravityScale = 1;    // How much gravity affects this (0 = no gravity)
rb.mass = 1;            // Weight of object
rb.drag = 0;            // Air resistance (higher = slower)
rb.bodyType = RigidbodyType2D.Dynamic;  // Dynamic, Kinematic, or Static
rb.constraints = RigidbodyConstraints2D.FreezeRotation;  // Prevent rotation
```

**Body Types:**
- **Dynamic:** Affected by physics (falls, collides, moves)
- **Kinematic:** Not affected by physics but can move via code
- **Static:** Immovable (walls, ground)

#### **Animator Component** (Controls animations)

**Purpose:** Plays animations and transitions between them

```csharp
Animator anim = GetComponent<Animator>();

// Trigger animations
anim.SetTrigger("Jump");        // Play "Jump" animation
anim.SetBool("IsWalking", true); // Set boolean parameter
anim.SetFloat("Speed", 5.0f);   // Set float parameter
anim.SetInteger("Health", 100);  // Set integer parameter
```

#### **AudioSource Component** (Plays sounds)

**Purpose:** Plays audio clips

```csharp
AudioSource audio = GetComponent<AudioSource>();

audio.clip = jumpSound;     // Assign sound clip
audio.Play();               // Play sound
audio.Stop();               // Stop sound
audio.volume = 0.5f;        // 50% volume
audio.loop = true;          // Loop sound
```

### 3.5 Script Components (Custom Components)

**Scripts** are custom components you create using C#. They inherit from **MonoBehaviour**.

**Creating a script component:**
1. Right-click in Project → Create → C# Script → Name it "PlayerController"
2. Double-click to open in code editor
3. Write your logic in the script
4. Drag script onto GameObject in Hierarchy or add via Inspector

**Basic Script Structure:**
```csharp
using UnityEngine;  // Import Unity functionality

public class PlayerController : MonoBehaviour  // Inherit from MonoBehaviour
{
    // VARIABLES (Properties shown in Inspector)
    public float speed = 5f;      // Can be edited in Inspector (public)
    private Rigidbody2D rb;       // Hidden from Inspector (private)

    // AWAKE - Called when GameObject is created (before Start)
    void Awake()
    {
        // Initialize references
        rb = GetComponent<Rigidbody2D>();
    }

    // START - Called before first frame (after all Awakes)
    void Start()
    {
        // Initialize game state
        Debug.Log("Player spawned!");
    }

    // UPDATE - Called every frame (~60 times per second)
    void Update()
    {
        // Handle input and logic that needs to happen every frame
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space pressed!");
        }
    }

    // FIXEDUPDATE - Called at fixed time intervals (for physics)
    void FixedUpdate()
    {
        // Handle physics calculations
        float moveX = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveX * speed, rb.velocity.y);
    }
}
```

### 3.6 GameObject Lifecycle

Understanding the order Unity calls functions is crucial:

```
GameObject Created
      ↓
  Awake()           ← Initialize this object (cache components)
      ↓
  OnEnable()        ← Called when object is enabled
      ↓
  Start()           ← Initialize after all Awakes (access other objects)
      ↓
┌─────────────────┐
│  Update()       │ ← Every frame (~60 FPS)
│  FixedUpdate()  │ ← Fixed timestep (50 FPS, for physics)
│  LateUpdate()   │ ← After all Updates (camera follow)
└────────┬────────┘
         │ (Loops continuously)
         ↓
  OnDisable()       ← Called when object is disabled
      ↓
  OnDestroy()       ← Called when object is destroyed
      ↓
GameObject Destroyed
```

**When to use each:**

| Function | Use Case | Example |
|----------|----------|---------|
| `Awake()` | Initialize this object, cache components | `rb = GetComponent<Rigidbody2D>();` |
| `Start()` | Initialize after other objects ready | `player = GameObject.Find("Player");` |
| `Update()` | Per-frame logic (input, AI, timers) | Check if spacebar pressed |
| `FixedUpdate()` | Physics calculations (consistent timestep) | Apply forces to Rigidbody |
| `LateUpdate()` | Logic that depends on all Updates finishing | Camera following player |
| `OnDestroy()` | Cleanup when object destroyed | Save data, unsubscribe events |

### 3.7 Component Communication Example

**Scenario:** Player collects a coin

```csharp
// Coin.cs - Script on Coin GameObject
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int coinValue = 10;  // How much this coin is worth

    // Called when another collider enters this trigger
    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object that touched us is the Player
        if (other.gameObject.tag == "Player")
        {
            // Get the PlayerInventory component from Player
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();

            if (inventory != null)
            {
                // Add coins to player
                inventory.AddCoins(coinValue);

                // Destroy this coin
                Destroy(gameObject);
            }
        }
    }
}

// PlayerInventory.cs - Script on Player GameObject
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int coins = 0;  // Current coin count

    public void AddCoins(int amount)
    {
        coins += amount;
        Debug.Log("Coins: " + coins);
    }
}
```

**What happens:**
1. Player GameObject touches Coin GameObject (both have Collider2D)
2. `OnTriggerEnter2D` is called on Coin
3. Coin checks if `other` is tagged "Player"
4. Coin gets `PlayerInventory` component from Player
5. Coin calls `AddCoins()` method
6. Coin destroys itself

---

## 4. Prefabs (Template Objects)

### 4.1 What is a Prefab?

**Definition:** A Prefab is a **reusable template** for GameObjects. Think of it as a blueprint or cookie cutter.

**Analogy:** Prefabs are like **blueprints for houses**:
- Blueprint (Prefab) → Build house (Instantiate)
- Change blueprint → All future houses use new design
- Existing houses can be updated to match blueprint

**Why use Prefabs?**
- ✅ **Reusability:** Create once, spawn many times
- ✅ **Consistency:** All instances have same setup
- ✅ **Easy Updates:** Change prefab → all instances update
- ✅ **Organization:** Clean Hierarchy (spawn/destroy at runtime)

### 4.2 Creating a Prefab

**Method 1: From Scene GameObject**
1. Create GameObject in scene (e.g., "Enemy")
2. Add components (SpriteRenderer, Collider, Scripts)
3. Drag from Hierarchy into Project panel's "Prefabs" folder
4. Now you have a reusable template!

**Method 2: Directly Create in Project**
1. Right-click in Project panel → Create → Prefab
2. Double-click to enter Prefab Mode
3. Add/configure GameObjects
4. Save and exit Prefab Mode

### 4.3 Using Prefabs in Code

**Spawning (Instantiating) Prefabs:**
```csharp
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // Assign prefab in Inspector (drag prefab here)
    public GameObject enemyPrefab;

    void Start()
    {
        // Spawn enemy at position (5, 0, 0) with no rotation
        Vector3 spawnPos = new Vector3(5, 0, 0);
        Quaternion noRotation = Quaternion.identity;
        Instantiate(enemyPrefab, spawnPos, noRotation);

        // Spawn enemy with parent
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPos, noRotation);
        newEnemy.transform.parent = this.transform;  // Make child of this object

        // Spawn and get reference to modify
        GameObject enemy = Instantiate(enemyPrefab, spawnPos, noRotation);
        enemy.name = "SpawnedEnemy";  // Rename
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        enemyScript.health = 200;  // Modify after spawning
    }
}
```

**Destroying Instantiated Objects:**
```csharp
// Destroy immediately
Destroy(gameObject);

// Destroy after 2 seconds
Destroy(gameObject, 2.0f);

// Destroy specific component
Destroy(GetComponent<Rigidbody2D>());
```

### 4.4 Prefab Best Practices

**DO:**
- ✅ Use prefabs for anything that spawns multiple times (enemies, bullets, pickups)
- ✅ Keep prefabs in organized folders (Prefabs/Enemies/, Prefabs/UI/)
- ✅ Use descriptive names (Enemy_Goblin, Projectile_Arrow)
- ✅ Test prefabs by dragging into scene temporarily

**DON'T:**
- ❌ Create prefabs for unique objects (main camera, game manager)
- ❌ Spawn thousands of prefabs per frame (use Object Pooling instead)
- ❌ Forget to assign prefab references in Inspector

---

## 5. Scenes

### 5.1 What is a Scene?

**Definition:** A Scene is a **container for GameObjects** that represents one level, menu, or area of your game.

**Analogy:** Scenes are like **chapters in a book** or **rooms in a house**:
- Each scene is self-contained
- You can switch between scenes
- Different scenes can have different content

**Common Scene Types:**
- Main Menu scene (title screen, buttons)
- Gameplay scenes (Level 1, Level 2, Level 3)
- Game Over scene (results, retry button)
- Loading scene (progress bar between levels)

### 5.2 Scene Management in Code

**Loading Scenes:**
```csharp
using UnityEngine;
using UnityEngine.SceneManagement;  // Required for scene management

public class SceneLoader : MonoBehaviour
{
    // Load scene by name
    public void LoadLevel1()
    {
        SceneManager.LoadScene("Level1");
    }

    // Load scene by index (build settings)
    public void LoadNextLevel()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene + 1);
    }

    // Reload current scene (restart level)
    public void RestartLevel()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    // Load scene asynchronously (with loading screen)
    public void LoadLevelAsync(string sceneName)
    {
        StartCoroutine(LoadAsync(sceneName));
    }

    IEnumerator LoadAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            float progress = operation.progress;  // 0 to 1
            Debug.Log("Loading: " + (progress * 100) + "%");
            yield return null;  // Wait one frame
        }
    }
}
```

### 5.3 Build Settings

For scenes to be loadable, they must be added to **Build Settings**:
1. File → Build Settings
2. Drag scenes from Project into "Scenes in Build" list
3. Scenes are assigned index numbers (0, 1, 2...)

### 5.4 DontDestroyOnLoad

By default, all GameObjects are destroyed when loading a new scene. To keep an object:

```csharp
using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Awake()
    {
        // This object survives scene loads
        DontDestroyOnLoad(gameObject);
    }
}
```

**Use case:** Managers that should persist (AudioManager, GameManager, PlayerData)

---

## 6. Scripting Basics

### 6.1 MonoBehaviour Class

All Unity scripts inherit from **MonoBehaviour**, which provides Unity-specific functionality:

```csharp
using UnityEngine;  // Import Unity's core functionality

// Must match filename! (PlayerController.cs)
public class PlayerController : MonoBehaviour
{
    // Your code here
}
```

**What MonoBehaviour gives you:**
- Lifecycle methods (Awake, Start, Update, etc.)
- Access to `gameObject`, `transform`, `name`
- Coroutine support
- Component system access
- Unity event functions

### 6.2 Variables & Serialization

```csharp
public class ExampleScript : MonoBehaviour
{
    // PUBLIC - Visible in Inspector, other scripts can access
    public int health = 100;
    public float speed = 5.0f;
    public string playerName = "Hero";
    public GameObject target;

    // PRIVATE - Hidden from Inspector, only this script can access
    private int secretValue = 42;
    private bool isAlive = true;

    // SERIALIZE PRIVATE - Hidden from other scripts, shown in Inspector
    [SerializeField] private int coins = 0;

    // HIDE PUBLIC - Public for scripts, hidden from Inspector
    [HideInInspector] public float internalTimer = 0;

    // HEADER - Organizes Inspector
    [Header("Movement Settings")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;

    [Header("Combat Settings")]
    public int damage = 10;
    public float attackRange = 2f;

    // TOOLTIP - Adds hover description in Inspector
    [Tooltip("How many seconds between attacks")]
    public float attackCooldown = 1.5f;

    // RANGE - Creates slider in Inspector
    [Range(0, 100)]
    public int volume = 50;
}
```

### 6.3 Common Patterns

#### **Getting Components**

```csharp
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;

    void Awake()
    {
        // Get component on same GameObject
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        // Null check (important!)
        if (rb == null) {
            Debug.LogError("Rigidbody2D not found on " + gameObject.name);
        }
    }
}
```

**Why cache in Awake?**
- GetComponent() is slow, don't call every frame
- Cache once, reuse many times

#### **Finding GameObjects**

```csharp
public class Enemy : MonoBehaviour
{
    private GameObject player;
    private Transform playerTransform;

    void Start()
    {
        // Find by name (slow, use sparingly)
        player = GameObject.Find("Player");

        // Find by tag (faster, preferred method)
        player = GameObject.FindGameObjectWithTag("Player");

        // Find with type (finds first instance)
        GameManager gm = FindObjectOfType<GameManager>();

        // Direct access to transform
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }
}
```

**Best Practice:** Assign references in Inspector when possible instead of finding at runtime.

#### **Instantiate (Spawn Objects)**

```csharp
public class Spawner : MonoBehaviour
{
    public GameObject enemyPrefab;

    void SpawnEnemy()
    {
        // Basic spawn
        Instantiate(enemyPrefab);

        // Spawn at position
        Vector3 spawnPos = new Vector3(5, 0, 0);
        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        // Spawn and keep reference
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        newEnemy.name = "SpawnedEnemy";

        // Spawn as child
        GameObject enemy = Instantiate(enemyPrefab, transform);
    }
}
```

#### **Destroy Objects**

```csharp
public class Coin : MonoBehaviour
{
    void OnCollect()
    {
        // Destroy immediately
        Destroy(gameObject);

        // Destroy after delay (useful for particles/sounds)
        Destroy(gameObject, 2.0f);

        // Destroy specific component
        Destroy(GetComponent<Collider2D>());

        // Disable instead of destroy (reusable)
        gameObject.SetActive(false);
    }
}
```

#### **Coroutines (Delayed Execution)**

Coroutines allow you to execute code over time or with delays:

```csharp
public class Timer : MonoBehaviour
{
    void Start()
    {
        // Start coroutine
        StartCoroutine(CountdownTimer());
        StartCoroutine(DelayedAction(3.0f));
    }

    // Coroutine must return IEnumerator
    IEnumerator CountdownTimer()
    {
        for (int i = 3; i > 0; i--)
        {
            Debug.Log(i);
            yield return new WaitForSeconds(1.0f);  // Wait 1 second
        }
        Debug.Log("Go!");
    }

    IEnumerator DelayedAction(float delay)
    {
        Debug.Log("Starting...");
        yield return new WaitForSeconds(delay);
        Debug.Log("Finished after " + delay + " seconds!");
    }

    IEnumerator FadeOut()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color color = sr.color;

        // Fade over 2 seconds
        for (float t = 0; t < 2.0f; t += Time.deltaTime)
        {
            color.a = 1 - (t / 2.0f);  // Alpha from 1 to 0
            sr.color = color;
            yield return null;  // Wait one frame
        }
    }

    // Stop coroutine
    void StopCountdown()
    {
        StopCoroutine("CountdownTimer");
        // or
        Coroutine c = StartCoroutine(CountdownTimer());
        StopCoroutine(c);
    }
}
```

**Yield Options:**
- `yield return null;` - Wait one frame
- `yield return new WaitForSeconds(2.0f);` - Wait 2 seconds
- `yield return new WaitForFixedUpdate();` - Wait for physics update
- `yield return new WaitUntil(() => condition);` - Wait until condition is true

---

## 7. Input System

### 7.1 Keyboard Input

```csharp
public class InputExample : MonoBehaviour
{
    void Update()
    {
        // CHECK IF KEY IS DOWN (returns true every frame while held)
        if (Input.GetKey(KeyCode.W)) {
            Debug.Log("W is held down");
        }

        // CHECK IF KEY WAS JUST PRESSED (returns true only on first frame)
        if (Input.GetKeyDown(KeyCode.Space)) {
            Debug.Log("Space was just pressed");
        }

        // CHECK IF KEY WAS JUST RELEASED
        if (Input.GetKeyUp(KeyCode.Space)) {
            Debug.Log("Space was just released");
        }

        // AXIS INPUT (returns -1 to 1, smooth)
        float horizontal = Input.GetAxis("Horizontal");  // A/D or Left/Right arrows
        float vertical = Input.GetAxis("Vertical");      // W/S or Up/Down arrows

        // RAW AXIS INPUT (returns -1, 0, or 1, no smoothing)
        float horizontalRaw = Input.GetAxisRaw("Horizontal");

        // ANY KEY PRESSED
        if (Input.anyKeyDown) {
            Debug.Log("Any key was pressed");
        }
    }
}
```

**Common KeyCodes:**
- Arrow keys: `KeyCode.LeftArrow`, `KeyCode.RightArrow`, `KeyCode.UpArrow`, `KeyCode.DownArrow`
- WASD: `KeyCode.W`, `KeyCode.A`, `KeyCode.S`, `KeyCode.D`
- Numbers: `KeyCode.Alpha1`, `KeyCode.Alpha2`, etc.
- Function: `KeyCode.F1`, `KeyCode.F2`, etc.
- Modifiers: `KeyCode.LeftShift`, `KeyCode.LeftControl`, `KeyCode.LeftAlt`
- Others: `KeyCode.Space`, `KeyCode.Return` (Enter), `KeyCode.Escape`

### 7.2 Mouse Input

```csharp
public class MouseExample : MonoBehaviour
{
    void Update()
    {
        // MOUSE BUTTONS (0 = left, 1 = right, 2 = middle)
        if (Input.GetMouseButton(0)) {
            Debug.Log("Left mouse held down");
        }

        if (Input.GetMouseButtonDown(0)) {
            Debug.Log("Left mouse clicked");
        }

        if (Input.GetMouseButtonUp(1)) {
            Debug.Log("Right mouse released");
        }

        // MOUSE POSITION (screen coordinates)
        Vector3 mousePos = Input.mousePosition;
        Debug.Log("Mouse: " + mousePos);

        // CONVERT TO WORLD POSITION
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        worldPos.z = 0;  // For 2D games, set z to 0

        // MOUSE SCROLL WHEEL
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0) {
            Debug.Log("Scrolled up");
        } else if (scroll < 0) {
            Debug.Log("Scrolled down");
        }
    }
}
```

### 7.3 Touch Input (Mobile)

```csharp
public class TouchExample : MonoBehaviour
{
    void Update()
    {
        // NUMBER OF TOUCHES
        int touchCount = Input.touchCount;

        if (touchCount > 0)
        {
            // Get first touch
            Touch touch = Input.GetTouch(0);

            // Touch position
            Vector2 touchPos = touch.position;

            // Touch phase
            if (touch.phase == TouchPhase.Began) {
                Debug.Log("Touch started");
            }
            else if (touch.phase == TouchPhase.Moved) {
                Debug.Log("Touch moved");
            }
            else if (touch.phase == TouchPhase.Ended) {
                Debug.Log("Touch ended");
            }
        }

        // MULTI-TOUCH
        if (Input.touchCount == 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            // Pinch-to-zoom detection
            float prevDistance = (touch1.position - touch1.deltaPosition -
                                  touch2.position - touch2.deltaPosition).magnitude;
            float currentDistance = (touch1.position - touch2.position).magnitude;

            if (currentDistance > prevDistance) {
                Debug.Log("Pinch out (zoom in)");
            } else if (currentDistance < prevDistance) {
                Debug.Log("Pinch in (zoom out)");
            }
        }
    }
}
```

---

## 8. Physics System

Unity's physics system handles collisions, gravity, and forces.

### 8.1 Colliders (Collision Shapes)

Colliders define the collision boundaries of GameObjects.

**Types of Colliders (2D):**
- **BoxCollider2D** - Rectangle/square shape
- **CircleCollider2D** - Circle shape
- **PolygonCollider2D** - Custom polygon shape
- **EdgeCollider2D** - Line/edge shape
- **CapsuleCollider2D** - Capsule (rounded rectangle)

**Configuration:**
```csharp
BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
boxCollider.size = new Vector2(1, 2);       // Width x Height
boxCollider.offset = new Vector2(0, 0.5);   // Shift from center
boxCollider.isTrigger = false;              // Solid collision
```

### 8.2 Rigidbody2D (Physics Simulation)

Rigidbody2D adds physics behavior to GameObjects.

**Configuration:**
```csharp
Rigidbody2D rb = GetComponent<Rigidbody2D>();

// Body Type
rb.bodyType = RigidbodyType2D.Dynamic;     // Affected by physics
rb.bodyType = RigidbodyType2D.Kinematic;   // Move via code, not physics
rb.bodyType = RigidbodyType2D.Static;      // Immovable

// Properties
rb.mass = 1.0f;                // Weight
rb.gravityScale = 1.0f;        // 0 = no gravity, 1 = normal gravity
rb.drag = 0;                   // Linear damping (air resistance)
rb.angularDrag = 0.05f;        // Rotation damping

// Constraints (freeze axes)
rb.constraints = RigidbodyConstraints2D.FreezeRotation;              // No rotation
rb.constraints = RigidbodyConstraints2D.FreezePositionX;             // No X movement
rb.constraints = RigidbodyConstraints2D.FreezePosition;              // No movement
rb.constraints = RigidbodyConstraints2D.FreezeRotation |
                 RigidbodyConstraints2D.FreezePositionY;             // Combine constraints

// Collision Detection
rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;  // Precise (for fast objects)
rb.collisionDetectionMode = CollisionDetectionMode2D.Discrete;    // Fast (default)
```

### 8.3 Movement with Physics

```csharp
public class PhysicsMovement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 10f;
    private Rigidbody2D rb;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()  // Use FixedUpdate for physics!
    {
        // METHOD 1: Set velocity directly (instant speed change)
        float moveX = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveX * speed, rb.velocity.y);  // Keep Y velocity

        // METHOD 2: AddForce (gradual acceleration)
        Vector2 force = new Vector2(moveX * speed, 0);
        rb.AddForce(force);

        // METHOD 3: MovePosition (for kinematic bodies)
        Vector2 movement = new Vector2(moveX, 0) * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        // JUMP
        if (Input.GetKeyDown(KeyCode.Space)) {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
}
```

### 8.4 Collision Detection

**Two types of collisions:**

1. **Collision** (solid, blocks movement)
```csharp
// Called when collision starts
void OnCollisionEnter2D(Collision2D collision)
{
    Debug.Log("Hit: " + collision.gameObject.name);

    // Access collision point
    Vector2 contactPoint = collision.contacts[0].point;

    // Access collision normal (direction)
    Vector2 normal = collision.contacts[0].normal;

    // Get other object's component
    Enemy enemy = collision.gameObject.GetComponent<Enemy>();
    if (enemy != null) {
        enemy.TakeDamage(10);
    }
}

// Called every frame while colliding
void OnCollisionStay2D(Collision2D collision)
{
    Debug.Log("Still colliding with: " + collision.gameObject.name);
}

// Called when collision ends
void OnCollisionExit2D(Collision2D collision)
{
    Debug.Log("Stopped colliding with: " + collision.gameObject.name);
}
```

2. **Trigger** (non-solid, overlap detection)
```csharp
// Called when entering trigger
void OnTriggerEnter2D(Collider2D other)
{
    Debug.Log("Entered trigger: " + other.gameObject.name);

    if (other.gameObject.tag == "Coin") {
        Destroy(other.gameObject);  // Collect coin
    }
}

// Called every frame while inside trigger
void OnTriggerStay2D(Collider2D other)
{
    Debug.Log("Inside trigger: " + other.gameObject.name);
}

// Called when leaving trigger
void OnTriggerExit2D(Collider2D other)
{
    Debug.Log("Left trigger: " + other.gameObject.name);
}
```

**Collision Matrix (what can collide):**
- Requires **at least one Rigidbody2D** (on either object)
- Both objects must have **Colliders**
- Colliders must not be on ignored layers

### 8.5 Raycasting (Line-of-Sight Detection)

Raycasting shoots an invisible ray to detect objects.

```csharp
public class RaycastExample : MonoBehaviour
{
    public float rayDistance = 10f;
    public LayerMask targetLayer;

    void Update()
    {
        // BASIC RAYCAST
        Vector2 origin = transform.position;
        Vector2 direction = Vector2.right;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, rayDistance);

        if (hit.collider != null)
        {
            Debug.Log("Hit: " + hit.collider.gameObject.name);
            Debug.Log("Distance: " + hit.distance);
            Debug.Log("Point: " + hit.point);
        }

        // LAYERMASK RAYCAST (only hit specific layers)
        hit = Physics2D.Raycast(origin, direction, rayDistance, targetLayer);

        // RAYCAST WITH TAG CHECK
        if (hit.collider != null && hit.collider.gameObject.tag == "Enemy")
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            enemy.TakeDamage(10);
        }

        // VISUALIZE RAYCAST (in Scene view)
        Debug.DrawRay(origin, direction * rayDistance, Color.red);
    }

    // CIRCLECAST (raycast with width)
    void CircleCastExample()
    {
        Vector2 origin = transform.position;
        Vector2 direction = Vector2.right;
        float radius = 0.5f;

        RaycastHit2D hit = Physics2D.CircleCast(origin, radius, direction, rayDistance);

        if (hit.collider != null) {
            Debug.Log("CircleCast hit: " + hit.collider.gameObject.name);
        }
    }

    // OVERLAP DETECTION (check area)
    void OverlapCircleExample()
    {
        Vector2 center = transform.position;
        float radius = 2.0f;

        // Get all colliders in radius
        Collider2D[] colliders = Physics2D.OverlapCircleAll(center, radius);

        foreach (Collider2D col in colliders)
        {
            Debug.Log("In range: " + col.gameObject.name);
        }

        // Visualize
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, radius);
    }
}
```

---

## 9. UI System (Canvas)

Unity's UI system uses a **Canvas** to display 2D interface elements.

### 9.1 Canvas Setup

**Canvas** is the root container for all UI elements.

**Canvas Render Modes:**
1. **Screen Space - Overlay:** UI drawn on top of everything (most common)
2. **Screen Space - Camera:** UI rendered by specific camera (for 3D effects)
3. **World Space:** UI exists in 3D world (health bars above characters)

**Creating UI:**
1. Right-click Hierarchy → UI → Canvas (creates Canvas + EventSystem)
2. Add UI elements as children of Canvas
3. Use **RectTransform** instead of Transform for positioning

### 9.2 Common UI Components

#### **Text (TextMeshPro recommended)**
```csharp
using TMPro;  // TextMeshPro namespace

public class UIExample : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    void UpdateScore(int score)
    {
        scoreText.text = "Score: " + score;
        scoreText.color = Color.yellow;
        scoreText.fontSize = 24;
    }
}
```

#### **Button**
```csharp
using UnityEngine;
using UnityEngine.UI;

public class ButtonExample : MonoBehaviour
{
    public Button playButton;

    void Start()
    {
        // Add listener via code
        playButton.onClick.AddListener(OnPlayClicked);
    }

    void OnPlayClicked()
    {
        Debug.Log("Play button clicked!");
    }

    // Or assign in Inspector:
    // 1. Select Button in Hierarchy
    // 2. In Inspector, find OnClick() list
    // 3. Click +, drag object with script
    // 4. Select function from dropdown
}
```

#### **Slider**
```csharp
using UnityEngine.UI;

public class SliderExample : MonoBehaviour
{
    public Slider healthSlider;

    void SetHealth(int current, int max)
    {
        healthSlider.maxValue = max;
        healthSlider.value = current;

        // Percentage (0-1)
        healthSlider.value = (float)current / max;
    }

    void Start()
    {
        // Listen to slider changes
        healthSlider.onValueChanged.AddListener(OnHealthChanged);
    }

    void OnHealthChanged(float value)
    {
        Debug.Log("Health slider: " + value);
    }
}
```

#### **Image**
```csharp
using UnityEngine.UI;

public class ImageExample : MonoBehaviour
{
    public Image characterPortrait;
    public Sprite newSprite;

    void ChangeImage()
    {
        characterPortrait.sprite = newSprite;
        characterPortrait.color = Color.red;       // Tint
        characterPortrait.fillAmount = 0.5f;       // Fill (for radial/filled images)
        characterPortrait.enabled = false;         // Hide
    }
}
```

### 9.3 Anchors & RectTransform

**Anchors** control how UI elements scale and position relative to screen size.

```
┌──────────────────────────────────┐
│  Canvas (Screen)                 │
│                                  │
│  ┌────────┐    ← Anchor: Top-Left
│  │ Button │      Stays in corner
│  └────────┘                      │
│                                  │
│             [Button]             │ ← Anchor: Center
│         (stays centered)         │   Stays in center
│                                  │
│                        ┌────────┐│ ← Anchor: Bottom-Right
│                        │ Button ││   Stays in corner
│                        └────────┘│
└──────────────────────────────────┘
```

**Common Anchor Presets:**
- Top-Left, Top-Center, Top-Right
- Middle-Left, Middle-Center, Middle-Right
- Bottom-Left, Bottom-Center, Bottom-Right
- Stretch (expands with screen)

**Setting Anchors in Code:**
```csharp
RectTransform rectTransform = GetComponent<RectTransform>();

// Set anchored position
rectTransform.anchoredPosition = new Vector2(100, 50);

// Set size
rectTransform.sizeDelta = new Vector2(200, 100);

// Set anchors (0-1 range)
rectTransform.anchorMin = new Vector2(0.5f, 0.5f);  // Center
rectTransform.anchorMax = new Vector2(0.5f, 0.5f);  // Center
```

---

## 10. Resources & Asset Management

### 10.1 Resources Folder

The **Resources** folder allows loading assets at runtime.

**Structure:**
```
Assets/
└── Resources/          ← Special folder name
    ├── Prefabs/
    │   └── Enemy.prefab
    ├── Sprites/
    │   └── Icon.png
    └── Audio/
        └── Music.mp3
```

**Loading Resources:**
```csharp
public class ResourceLoader : MonoBehaviour
{
    void Start()
    {
        // Load prefab
        GameObject enemyPrefab = Resources.Load<GameObject>("Prefabs/Enemy");
        Instantiate(enemyPrefab);

        // Load sprite
        Sprite icon = Resources.Load<Sprite>("Sprites/Icon");

        // Load audio
        AudioClip music = Resources.Load<AudioClip>("Audio/Music");

        // Load all assets of type
        GameObject[] allPrefabs = Resources.LoadAll<GameObject>("Prefabs");
    }
}
```

**⚠️ Warning:** Don't put everything in Resources - it increases build size and memory. Use for dynamically loaded assets only.

### 10.2 Asset References

**Best Practice:** Assign references in Inspector when possible.

```csharp
public class AssetExample : MonoBehaviour
{
    // METHOD 1: Inspector Reference (BEST)
    public GameObject enemyPrefab;      // Drag prefab here in Inspector
    public Sprite playerSprite;         // Drag sprite here
    public AudioClip jumpSound;         // Drag audio clip here

    void Start()
    {
        // Assets are already loaded, ready to use
        Instantiate(enemyPrefab);
    }
}
```

---

## 11. Best Practices

### 11.1 Performance Optimization

**DO:**
- ✅ **Cache GetComponent calls** in Awake/Start
```csharp
// GOOD
private Rigidbody2D rb;
void Awake() { rb = GetComponent<Rigidbody2D>(); }
void Update() { rb.velocity = ...; }

// BAD (calls GetComponent every frame)
void Update() { GetComponent<Rigidbody2D>().velocity = ...; }
```

- ✅ **Use Object Pooling** for frequently spawned objects
```csharp
// Instead of Instantiate/Destroy every frame
// Reuse objects from a pool
```

- ✅ **Avoid Update() for non-frame-dependent logic**
```csharp
// Use events, coroutines, or InvokeRepeating instead
InvokeRepeating("CheckEnemies", 0, 0.5f);  // Every 0.5 seconds
```

- ✅ **Use FixedUpdate() for physics**
```csharp
void FixedUpdate() { rb.AddForce(...); }  // Consistent physics
```

- ✅ **Minimize garbage collection**
```csharp
// GOOD - reuse Vector3
private Vector3 movement = Vector3.zero;
void Update() {
    movement.x = Input.GetAxis("Horizontal");
    transform.Translate(movement);
}

// BAD - creates new Vector3 every frame
void Update() {
    transform.Translate(new Vector3(Input.GetAxis("Horizontal"), 0, 0));
}
```

**DON'T:**
- ❌ **Don't use Find/FindObjectOfType in Update()**
```csharp
// BAD - very slow
void Update() {
    GameObject player = GameObject.Find("Player");  // DON'T DO THIS
}

// GOOD - find once
private GameObject player;
void Start() {
    player = GameObject.Find("Player");
}
```

- ❌ **Don't spam Instantiate/Destroy**
- ❌ **Don't use Camera.main in loops** (cache it)
- ❌ **Don't ignore compiler warnings**

### 11.2 Code Organization

**Naming Conventions:**
```csharp
public class PlayerController : MonoBehaviour  // PascalCase for classes
{
    public int MaxHealth = 100;        // PascalCase for public fields
    private float moveSpeed = 5f;      // camelCase for private fields

    const int MAX_ENEMIES = 50;        // UPPER_CASE for constants

    public void TakeDamage(int amount) // PascalCase for methods
    {
        // ...
    }

    private void UpdateHealth()        // PascalCase for methods
    {
        // ...
    }
}
```

**Folder Organization:**
```
Assets/
├── _YourGameName/         ← Main game folder (underscore sorts to top)
│   ├── Scenes/
│   ├── Scripts/
│   │   ├── Player/
│   │   ├── Enemies/
│   │   ├── Managers/
│   │   └── UI/
│   ├── Prefabs/
│   ├── Sprites/
│   └── Audio/
└── Plugins/               ← Third-party assets
```

### 11.3 Debugging Tips

**Debug.Log Variations:**
```csharp
Debug.Log("Normal message");         // White
Debug.LogWarning("Warning!");        // Yellow
Debug.LogError("Error occurred!");   // Red

// Log with context (click to highlight object)
Debug.Log("Message", gameObject);

// Conditional logging
if (debugMode) Debug.Log("Debug info");
```

**Debug.DrawRay for Visualizing:**
```csharp
void Update()
{
    // Draw line in Scene view (not Game view)
    Debug.DrawRay(transform.position, Vector3.forward * 10, Color.red);
    Debug.DrawLine(transform.position, targetPosition, Color.green);
}
```

**Gizmos for Editor Visualization:**
```csharp
void OnDrawGizmos()
{
    // Always visible
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(transform.position, 2f);
}

void OnDrawGizmosSelected()
{
    // Only when object is selected
    Gizmos.color = Color.red;
    Gizmos.DrawSphere(transform.position, 0.5f);
}
```

**Break Points:**
- Set breakpoints in your IDE (Visual Studio/Rider)
- Attach Unity debugger to inspect variables during Play mode

---

## 12. What's Next?

Congratulations! You now understand Unity fundamentals. Here's your next steps:

**Immediate Next Steps:**
1. ✅ Read **01_Project_Architecture.md** - Understand this specific project
2. ✅ Read **02_Player_System_Complete.md** - Deep dive into player mechanics
3. ✅ Open Unity Editor and explore the project
4. ✅ Run the game in Play mode and observe behavior

**Practice Tasks:**
1. Create a simple GameObject with a script that moves when pressing arrow keys
2. Make an object spawn when pressing spacebar
3. Add a UI button that changes text when clicked
4. Create a trigger that destroys objects entering it

**Advanced Learning:**
1. Read system-specific documentation (Enemy, UI, Managers)
2. Follow **10_How_To_Guides.md** to make modifications
3. Implement your own feature from scratch

---

## 13. Quick Reference Cheat Sheet

**Lifecycle Order:**
```
Awake() → OnEnable() → Start() → Update()/FixedUpdate()/LateUpdate() → OnDisable() → OnDestroy()
```

**Common Code Snippets:**
```csharp
// Get component
Rigidbody2D rb = GetComponent<Rigidbody2D>();

// Find GameObject
GameObject player = GameObject.FindGameObjectWithTag("Player");

// Instantiate
Instantiate(prefab, position, Quaternion.identity);

// Destroy
Destroy(gameObject);
Destroy(gameObject, 2f);  // After 2 seconds

// Coroutine delay
IEnumerator Example() {
    yield return new WaitForSeconds(2f);
    // Code here runs after 2 seconds
}

// Input
if (Input.GetKeyDown(KeyCode.Space)) { }
float h = Input.GetAxis("Horizontal");

// Physics movement
rb.velocity = new Vector2(speed, rb.velocity.y);
rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

// UI
text.text = "Score: " + score;
button.onClick.AddListener(OnClick);
```

---

**Congratulations! You're now ready to dive into the project documentation.**

**Next Document:** → `01_Project_Architecture.md`

