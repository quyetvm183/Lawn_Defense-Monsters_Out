# Project Architecture - Lawn Defense: Monsters Out

**Prerequisites:** Read `00_Unity_Fundamentals.md` first
**Target Audience:** Developers who want to understand the project structure
**Estimated Reading Time:** 30-40 minutes
**Related Documents:** → `02_Player_System_Complete.md`, `03_Enemy_System_Complete.md`

---

## Table of Contents
1. [High-Level Overview](#1-high-level-overview)
2. [Project Type & Technical Details](#2-project-type--technical-details)
3. [Folder Structure](#3-folder-structure)
4. [Core Systems Diagram](#4-core-systems-diagram)
5. [Design Patterns Used](#5-design-patterns-used)
6. [Data Flow & Game Loop](#6-data-flow--game-loop)
7. [Scene Flow](#7-scene-flow)
8. [System Dependencies Map](#8-system-dependencies-map)
9. [Key Classes Reference](#9-key-classes-reference)
10. [Namespace Organization](#10-namespace-organization)

---

## 1. High-Level Overview

### 1.1 Project Description

**Name:** Lawn Defense: Monsters Out
**Genre:** 2D Tower Defense / Action Defense
**Platform:** Mobile (Android/iOS)
**Unity Version:** 2021.x

**Core Gameplay:**
- Player controls an **Archer character** that automatically shoots arrows at enemies
- **Defend a fortress** from waves of monsters approaching from the right
- **Upgrade system** for characters, weapons, and stats
- **Level-based progression** with increasing difficulty
- **Monetization** through IAP (In-App Purchases) and Unity Ads

**Key Features:**
- ⚔️ **Automatic Shooting:** Player archer auto-aims and shoots using trajectory calculation
- 🎯 **Wave-Based Spawning:** Enemies spawn in configured waves
- 💀 **Multiple Enemy Types:** Melee, ranged, flying, tank enemies
- 🔥 **Weapon Effects:** Poison, Burn, Freeze, Shock effects
- 🛒 **Shop System:** Upgrade characters, buy items with coins or IAP
- 📊 **Progression:** Unlock levels, earn stars based on performance

### 1.2 Technical Stack

```
Unity Engine 2021.x
    ├─ C# Scripting Language
    ├─ Unity Input System
    ├─ Unity 2D Tools (Sprite Renderer, Tilemap)
    ├─ Custom 2D Physics (Raycast-based, not Rigidbody2D)
    ├─ Unity IAP (In-App Purchases)
    ├─ Unity Ads + AdMob Integration
    ├─ PlayerPrefs for local data storage
    └─ Namespace: RGame (all scripts)
```

**Why Custom Physics?**
- More control over character movement
- Precise collision detection using raycasts
- Better performance for 2D side-scrolling
- Avoids Rigidbody2D quirks (unexpected bouncing, rotation)

---

## 2. Project Type & Technical Details

### 2.1 Game Type Analysis

**Genre Breakdown:**
```
Tower Defense (50%)
├─ Static player position (archer)
├─ Enemy waves attacking
├─ Defend target (fortress)
└─ Upgrade progression

Action Game (30%)
├─ Player-controlled shooting
├─ Manual aiming (auto-calculated)
├─ Real-time combat
└─ Dodge mechanics (player can move)

Idle Game Elements (20%)
├─ Auto-shooting (no manual fire)
├─ Incremental upgrades
├─ Coin collection
└─ Prestige-like progression
```

### 2.2 Core Game Loop

```
1. SELECT LEVEL
   └─ Choose from unlocked levels in Map UI
       ↓
2. LOAD GAME SCENE
   └─ GameManager instantiates level prefab
   └─ MenuManager initializes UI
       ↓
3. PRESS "PLAY" BUTTON
   └─ GameManager.StartGame() called
   └─ Game state changes: Menu → Playing
   └─ All IListeners receive IPlay() event
       ↓
4. GAMEPLAY LOOP (Playing State)
   ├─ [Player System]
   │   └─ Player_Archer auto-detects enemies
   │   └─ Calculates trajectory
   │   └─ Shoots arrows with effects
   ├─ [Enemy System]
   │   └─ LevelEnemyManager spawns waves
   │   └─ Enemies walk toward fortress
   │   └─ Enemies attack when in range
   ├─ [Combat System]
   │   └─ Arrows hit enemies (TakeDamage)
   │   └─ Apply weapon effects (poison, burn, etc.)
   │   └─ Enemies die, give coins
   ├─ [UI System]
   │   └─ Update health bars
   │   └─ Show floating text (damage numbers)
   │   └─ Display wave progress
   └─ [Win/Loss Conditions]
       ├─ WIN: All waves defeated
       └─ LOSS: Fortress health reaches 0
           ↓
5. END GAME
   ├─ Victory → Menu_Victory screen
   │   └─ Award stars (1-3 based on performance)
   │   └─ Unlock next level
   │   └─ Give coins
   └─ Defeat → Game Over screen
       └─ Retry or return to map
```

---

## 3. Folder Structure

### 3.1 Project Organization

```
Lawn_Defense-Monsters_Out/
├── Assets/
│   ├── _MonstersOut/                 ← Main game folder (underscore = top priority)
│   │   ├── AdController/             ← Ad integration scripts
│   │   │   ├── AdmobController.cs
│   │   │   ├── AdsManager.cs
│   │   │   └── UnityAds.cs
│   │   │
│   │   ├── Editor/                   ← Custom Unity Editor scripts
│   │   │   ├── GameModeEditor.cs     ← Inspector customization for GameMode
│   │   │   └── ReadOnlyEditor.cs     ← [ReadOnly] attribute editor
│   │   │
│   │   ├── Scenes/                   ← Game scenes
│   │   │   └── SampleScene.unity     ← Main game scene
│   │   │
│   │   ├── Prefabs/                  ← Reusable game objects (not visible in file list)
│   │   │   ├── Enemies/
│   │   │   ├── Players/
│   │   │   ├── Projectiles/
│   │   │   └── UI/
│   │   │
│   │   └── Scripts/                  ← **ALL GAME CODE (60+ scripts)**
│   │       ├── AI/                   ← Enemy behavior (11 scripts)
│   │       ├── Controllers/          ← Physics & projectiles (7 scripts)
│   │       ├── Helpers/              ← Utilities (9 scripts)
│   │       ├── Managers/             ← Game managers (14 scripts)
│   │       ├── Player/               ← Player scripts (3 scripts)
│   │       └── UI/                   ← UI scripts (16 scripts)
│   │
│   ├── Audio/                        ← Sound effects & music
│   │   ├── Music/                    ← Background music tracks
│   │   └── Sound/                    ← SFX (shooting, hitting, dying)
│   │
│   ├── Resources/                    ← Runtime-loadable assets
│   │   └── Sprite/                   ← All visual assets
│   │       ├── Background/           ← Backgrounds, battlefield
│   │       ├── Enemy/                ← 9 enemy sprite sets (animations)
│   │       ├── Player/               ← Player character sprites
│   │       ├── Fortress/             ← Fortress/base sprites
│   │       └── GUI/                  ← UI elements (buttons, icons)
│   │
│   ├── Scenes/                       ← Additional scenes
│   │   └── SampleScene.unity
│   │
│   ├── Settings/                     ← Unity project settings
│   │   ├── InputSystem_Actions      ← Input configuration
│   │   └── ... (project settings)
│   │
│   └── TutorialInfo/                 ← Unity tutorial assets (can ignore)
│
├── Documents/                        ← **THIS DOCUMENTATION**
│   ├── 00_Unity_Fundamentals.md
│   ├── 01_Project_Architecture.md   ← YOU ARE HERE
│   ├── scripts/                      ← Legacy Vietnamese docs
│   └── ... (other docs)
│
├── ProjectSettings/                  ← Unity configuration
├── Packages/                         ← Unity packages
└── Logs/                             ← Unity logs
```

### 3.2 Scripts Folder Deep Dive

**AI/ - Enemy Behavior (11 scripts):**
```
AI/
├── Enemy.cs                          ← **BASE CLASS** for all enemies
│   │ State Machine: SPAWNING, IDLE, WALK, ATTACK, HIT, DEATH
│   │ Effect System: BURN, POISON, FREEZE, SHOCK
│   │ Health management, damage handling
│   │ IListener implementation (game events)
│   │ ICanTakeDamage implementation (damage interface)
│   └── Used by: All enemy types AND Player (inheritance!)
│
├── SmartEnemyGrounded.cs             ← **MAIN IMPLEMENTATION** for ground enemies
│   │ Inherits from Enemy
│   │ Implements movement with Controller2D
│   │ Handles attack logic (calls attack modules)
│   │ Detects target, chases player
│   └── Used by: Most enemy prefabs
│
├── EnemyMeleeAttack.cs               ← Melee attack module
├── EnemyRangeAttack.cs               ← Ranged attack module
├── EnemyThrowAttack.cs               ← Throwing attack module
├── EnemySpawn.cs                     ← Spawn configuration data class
├── GiveCoinWhenDie.cs                ← Drops coins on death
├── ICanTakeDamage.cs                 ← Damage interface
├── ICanTakeDamageBodyPart.cs         ← Body part damage interface
├── TheFortrest.cs                    ← Fortress (player's base)
└── WitchHeal.cs                      ← Support enemy with healing ability
```

**Controllers/ - Physics & Projectiles (7 scripts):**
```
Controllers/
├── Controller2D.cs                   ← **CORE** custom 2D physics controller
│   │ Raycasting-based collision detection
│   │ Movement without Rigidbody2D
│   └── Used by: Player, Enemies
│
├── RaycastController.cs              ← Base class for raycast collision
│   │ Manages raycasts for detecting ground/walls
│   └── Inherited by: Controller2D
│
├── Projectile.cs                     ← Base projectile class
├── SimpleProjectile.cs               ← Simple straight-line projectile
├── ArrowProjectile.cs                ← Arrow with gravity and trajectory
├── CameraController.cs               ← Smooth camera follow
└── FixedCamera.cs                    ← Static camera
```

**Helpers/ - Utilities (9 scripts):**
```
Helpers/
├── GlobalValue.cs                    ← **CENTRAL DATA STORE**
│   │ PlayerPrefs wrapper
│   │ Coins, level progress, unlocks
│   │ Save/load player data
│   └── Static class, accessed globally
│
├── AnimationHelper.cs                ← Animation utilities
├── CheckTargetHelper.cs              ← Target detection (raycasts)
├── SpawnItemHelper.cs                ← Item spawning utility
├── WeaponEffect.cs                   ← Weapon effect data (poison, burn, etc.)
├── AutoDestroy.cs                    ← Auto-destroy objects after time
├── RotateAround.cs                   ← Rotate object around point
├── SortingLayerHelper.cs             ← Sprite layer sorting
├── IListener.cs                      ← **LISTENER PATTERN INTERFACE**
│   └── Methods: IPlay(), IPause(), IGameOver(), ISuccess(), IUnPause()
└── ReadOnlyAttribute.cs              ← [ReadOnly] attribute for Inspector
```

**Managers/ - Game Management (14 scripts):**
```
Managers/
├── GameManager.cs                    ← **SINGLETON, CORE CONTROLLER**
│   │ Manages game state (Menu, Playing, GameOver, Success, Pause)
│   │ Listener pattern coordinator
│   │ Spawns level prefabs
│   │ Calls Victory()/GameOver()
│   └── Accessed via: GameManager.Instance
│
├── LevelEnemyManager.cs              ← **WAVE SPAWNING CONTROLLER**
│   │ Spawns enemies in waves
│   │ IListener implementation
│   │ Manages enemy timing and counts
│   └── Configured by level prefab
│
├── LevelManager.cs                   ← Level progression logic
├── MenuManager.cs                    ← UI initialization and control
├── SoundManager.cs                   ← Audio management (Singleton)
├── ShopManager.cs                    ← Shop system logic
├── ShopCharacterUpgrade.cs           ← Character upgrade UI
├── ShopItemUpgrade.cs                ← Item upgrade UI
├── GameLevelSetup.cs                 ← Level prefab configuration
├── GameMode.cs                       ← Game mode data
├── Level.cs                          ← Level data class
├── LevelWave.cs                      ← Wave configuration data
├── IncreaseGameSpeed.cs              ← Game speed control
├── Purchaser.cs                      ← IAP handler (Unity IAP)
├── IAPItem.cs                        ← IAP item data
└── UnityAdsitem.cs                   ← Unity Ads item data
```

**Player/ - Player Character (3 scripts):**
```
Player/
├── Player_Archer.cs                  ← **MAIN PLAYER CLASS**
│   │ Inherits from Enemy! (reuses health/damage system)
│   │ Auto-targeting system
│   │ Trajectory calculation for arrows
│   │ Arrow shooting with reload time
│   │ Movement with Controller2D
│   └── One of a kind (player is special enemy!)
│
├── CharacterManager.cs               ← Character spawning logic
└── UpgradedCharacterParameter.cs     ← Character upgrade stats
    │ Stores: health, damage, weapon effects
    │ Saved via PlayerPrefs
    └── ScriptableObject (asset-based data)
```

**UI/ - User Interface (16 scripts):**
```
UI/
├── MenuManager.cs                    ← Main menu controller
├── Menu_Victory.cs                   ← Victory screen UI
├── MainMenuHomeScene.cs              ← Home screen UI
├── MapControllerUI.cs                ← Level select map UI
├── BuyCharacterBtn.cs                ← Character purchase button
├── NotEnoughCoins.cs                 ← Insufficient funds popup
├── GiftVideoAd.cs                    ← Rewarded video ad UI
├── AutoAddManaUI.cs                  ← Auto-mana UI element
├── HealthBarEnemyNew.cs              ← Enemy health bar (follows enemy)
├── FloatingText.cs                   ← Damage number popup
├── FloatingTextManager.cs            ← Floating text object pool
├── BlackScreenUI.cs                  ← Screen fade effect
├── FlashScene.cs                     ← Scene transition flash
├── RGFade.cs                         ← Color fade utility
├── Tutorial.cs                       ← In-game tutorial system
├── UI_UI.cs                          ← General UI utilities
└── Helper_Swipe.cs                   ← Swipe gesture detection
```

---

## 4. Core Systems Diagram

### 4.1 System Overview

```
┌─────────────────────────────────────────────────────────────────────┐
│                         GAME MANAGER                                │
│                      (Central Controller)                           │
│                                                                     │
│  - Singleton Instance                                               │
│  - Game State Machine (Menu, Playing, Pause, GameOver, Success)    │
│  - Listener Pattern Coordinator                                     │
│  - Level Prefab Spawning                                            │
│  - LayerMask Configuration                                          │
└────────────┬────────────────────────────────────────────────────────┘
             │
             │ (Broadcasts events to all IListeners)
             │
    ┌────────┼────────┬────────┬────────┬────────┬────────┐
    │        │        │        │        │        │        │
    ▼        ▼        ▼        ▼        ▼        ▼        ▼
┌────────┐ ┌──────┐ ┌──────┐ ┌─────┐ ┌──────┐ ┌──────┐ ┌──────┐
│ PLAYER │ │ENEMY │ │ MENU │ │ UI  │ │SOUND │ │LEVEL │ │ ADS  │
│ SYSTEM │ │SYSTEM│ │  MGR │ │ELEMS│ │ MGR  │ │ENEMY │ │ MGR  │
│        │ │      │ │      │ │     │ │      │ │ MGR  │ │      │
└────────┘ └──────┘ └──────┘ └─────┘ └──────┘ └──────┘ └──────┘
    │          │         │       │        │        │        │
    └──────────┴─────────┴───────┴────────┴────────┴────────┘
                         │
                  (All implement IListener)
```

### 4.2 Detailed System Interaction

```
┌─────────────────────────┐
│   GameManager.Start()   │
│   Load Level Prefab     │
└────────────┬────────────┘
             │
             ▼
┌─────────────────────────┐        ┌──────────────────────┐
│ MenuManager.ShowUI()    │───────→│  Player clicks PLAY  │
└────────────┬────────────┘        └───────────┬──────────┘
             │                                  │
             │                                  │
             ▼                                  ▼
┌─────────────────────────────────────────────────────────┐
│         GameManager.StartGame()                         │
│         State = Playing                                 │
│         Find all IListeners                             │
│         Call IPlay() on each listener                   │
└───────────┬─────────────────────────────────────────────┘
            │
            └─────┬───────────┬─────────────┬──────────────┐
                  │           │             │              │
                  ▼           ▼             ▼              ▼
        ┌──────────────┐ ┌─────────┐ ┌──────────┐ ┌──────────────┐
        │ Player.IPlay │ │Enemy    │ │UI.IPlay  │ │LevelEnemy    │
        │ - Enable     │ │.IPlay   │ │- Show    │ │Manager.IPlay │
        │ - Start auto │ │- Start  │ │  game UI │ │- Start wave  │
        │   shooting   │ │  moving │ └──────────┘ │  spawning    │
        └──────────────┘ └─────────┘              └──────┬───────┘
                                                          │
                ┌─────────────────────────────────────────┘
                │
                ▼
┌───────────────────────────────────────────────────────────────┐
│                    GAME LOOP (Playing)                        │
├───────────────────────────────────────────────────────────────┤
│                                                               │
│  ┌─────────────┐         ┌──────────────┐                   │
│  │   PLAYER    │         │   ENEMIES    │                   │
│  ├─────────────┤         ├──────────────┤                   │
│  │ Auto-detect │───────→ │ Walk toward  │                   │
│  │ enemies     │         │ fortress     │                   │
│  │             │         │              │                   │
│  │ Calculate   │         │ Attack when  │                   │
│  │ trajectory  │         │ in range     │                   │
│  │             │         │              │                   │
│  │ Shoot arrow │───────→ │ TakeDamage() │                   │
│  │             │         │              │                   │
│  │             │         │ Apply effect │                   │
│  │             │         │ (poison/burn)│                   │
│  │             │         │              │                   │
│  │             │         │ Die() →      │                   │
│  │             │         │ GiveCoin()   │                   │
│  └─────────────┘         └──────────────┘                   │
│         │                        │                          │
│         │                        │                          │
│         └────────┬───────────────┘                          │
│                  │                                           │
│                  ▼                                           │
│          ┌──────────────┐                                   │
│          │  UI UPDATES  │                                   │
│          ├──────────────┤                                   │
│          │ Health bars  │                                   │
│          │ Damage text  │                                   │
│          │ Coin count   │                                   │
│          │ Wave progress│                                   │
│          └──────────────┘                                   │
│                                                               │
└───────────────────────────────────────────────────────────────┘
                            │
                  ┌─────────┴──────────┐
                  │                    │
                  ▼                    ▼
        ┌──────────────────┐  ┌────────────────┐
        │ All waves cleared│  │Fortress HP = 0 │
        │ GameManager.     │  │GameManager.    │
        │ Victory()        │  │ GameOver()     │
        └────────┬─────────┘  └────────┬───────┘
                 │                     │
                 │                     │
                 ▼                     ▼
        ┌──────────────────┐  ┌────────────────┐
        │ Call ISuccess()  │  │Call IGameOver()│
        │ All listeners    │  │All listeners   │
        │ Show victory UI  │  │Show game over  │
        │ Award stars      │  │UI              │
        │ Unlock next level│  │                │
        └──────────────────┘  └────────────────┘
```

---

## 5. Design Patterns Used

This project demonstrates several professional design patterns.

### 5.1 Singleton Pattern

**Purpose:** Ensure only one instance exists and provide global access

**Implementation in GameManager.cs:**
```csharp
public class GameManager : MonoBehaviour
{
    // Static property for global access
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        // Assign this instance
        Instance = this;

        // Additional singleton setup
        // (Note: This project doesn't use DontDestroyOnLoad for GameManager
        //  because it's created per scene)
    }
}
```

**Usage:**
```csharp
// Access from any script
if (GameManager.Instance.State == GameManager.GameState.Playing)
{
    // Do something
}

// Check layer
int enemyLayer = GameManager.Instance.layerEnemy;
```

**Singletons in This Project:**
- `GameManager` - Game state controller
- `SoundManager` - Audio management
- `FloatingTextManager` - Damage number pool
- `AdsManager` - Ad system

**Why Singleton?**
- ✅ Global access without FindObjectOfType (faster)
- ✅ Prevents multiple instances causing conflicts
- ✅ Clean API (GameManager.Instance.Victory())

### 5.2 Observer Pattern (Listener System)

**Purpose:** Decouple systems - GameManager doesn't need to know about every system directly

**IListener Interface:**
```csharp
// Defined in Helpers/IListener.cs
public interface IListener
{
    void IPlay();        // Game started
    void IPause();       // Game paused
    void IUnPause();     // Game resumed
    void IGameOver();    // Game over (loss)
    void ISuccess();     // Victory
}
```

**GameManager Implementation:**
```csharp
public class GameManager : MonoBehaviour
{
    // List of all objects listening for game events
    public List<IListener> listeners;

    public void StartGame()
    {
        State = GameState.Playing;

        // Find all objects implementing IListener
        var listener_ = FindObjectsOfType<MonoBehaviour>().OfType<IListener>();
        foreach (var _listener in listener_)
        {
            listeners.Add(_listener);
        }

        // Broadcast IPlay to all listeners
        foreach (var item in listeners)
        {
            item.IPlay();
        }
    }

    public void Victory()
    {
        State = GameState.Success;

        // Broadcast ISuccess to all listeners
        foreach (var item in listeners)
        {
            if (item != null)
                item.ISuccess();
        }
    }

    // Similar for Gamepause(), UnPause(), GameOver()
}
```

**Listener Example (LevelEnemyManager):**
```csharp
public class LevelEnemyManager : MonoBehaviour, IListener
{
    // IListener implementation
    public void IPlay()
    {
        // Start spawning waves when game starts
        StartCoroutine(SpawnEnemyWaves());
    }

    public void IPause()
    {
        // Stop spawning when paused
        StopAllCoroutines();
    }

    public void IUnPause()
    {
        // Resume spawning
        StartCoroutine(SpawnEnemyWaves());
    }

    public void IGameOver()
    {
        // Stop all activity
        StopAllCoroutines();
    }

    public void ISuccess()
    {
        // Victory - stop spawning
    }
}
```

**Who Implements IListener:**
- `Enemy` (base class - all enemies)
- `Player_Archer`
- `LevelEnemyManager`
- UI components (menus, health bars)
- Other managers

**Why Observer Pattern?**
- ✅ **Decoupling:** GameManager doesn't know about specific classes
- ✅ **Scalability:** Add new listeners without changing GameManager
- ✅ **Synchronization:** All systems react to state changes simultaneously

**Sequence Diagram:**
```
GameManager          Enemy1          Enemy2      LevelEnemyManager      MenuManager
     │                 │               │                 │                   │
     │  StartGame()    │               │                 │                   │
     ├────────────────→│               │                 │                   │
     │                 │               │                 │                   │
     │  listeners.IPlay()               │                │                   │
     ├─────────────────┼───────────────┼─────────────────┼───────────────────┤
     │                 │               │                 │                   │
     │                 │  IPlay()      │                 │                   │
     │                 ├──────────────→│   IPlay()       │                   │
     │                 │  (start move) │   (start move)  │    IPlay()        │
     │                 │               │                 ├──────────────────→│
     │                 │               │   (spawn waves) │   (hide menu, show game UI)
```

### 5.3 State Machine Pattern

**Purpose:** Manage object behavior based on current state

**Enemy State Machine:**
```csharp
// Defined in AI/Enemy.cs
public enum ENEMYSTATE
{
    SPAWNING,    // Enemy is appearing (animation)
    IDLE,        // Enemy is standing still
    WALK,        // Enemy is moving
    ATTACK,      // Enemy is attacking
    HIT,         // Enemy was just hit
    DEATH        // Enemy is dead
}

public class Enemy : MonoBehaviour
{
    public ENEMYSTATE enemyState = ENEMYSTATE.IDLE;

    public void SetEnemyState(ENEMYSTATE state)
    {
        enemyState = state;
        // Transition logic
    }

    public virtual void Update()
    {
        // Behavior based on state
        switch (enemyState)
        {
            case ENEMYSTATE.SPAWNING:
                // Play spawn animation, can't move
                break;
            case ENEMYSTATE.WALK:
                // Move toward target
                MoveToTarget();
                break;
            case ENEMYSTATE.ATTACK:
                // Attack target
                AttackTarget();
                break;
            case ENEMYSTATE.HIT:
                // Play hit animation
                break;
            case ENEMYSTATE.DEATH:
                // Die, give coins, destroy
                break;
        }
    }
}
```

**State Transition Diagram:**
```
        START
          │
          ▼
     SPAWNING ──────────────┐
          │                 │
          │ (spawn done)    │
          ▼                 │
        WALK ◄──────────────┘
          │ ▲               │
          │ │               │ (detect target)
          │ │               │
          │ │               ▼
          │ │            ATTACK
          │ │               │
          │ └───────────────┘
          │
          │ (take damage)
          ▼
         HIT ────┐
          │      │ (damage < health)
          │      │
          │ ◄────┘
          │
          │ (health <= 0)
          ▼
        DEATH
          │
          ▼
       DESTROY
```

**Game State Machine (GameManager):**
```csharp
public enum GameState
{
    Menu,       // Main menu, level select
    Playing,    // Gameplay active
    GameOver,   // Defeat
    Success,    // Victory
    Pause       // Game paused
}

public GameState State { get; set; }
```

**Why State Machine?**
- ✅ **Clear behavior:** Each state has defined actions
- ✅ **Easy debugging:** Log current state
- ✅ **Prevention:** Can't do invalid actions (can't attack while spawning)

### 5.4 Interface Pattern (ICanTakeDamage)

**Purpose:** Polymorphic damage system - anything can take damage

**Interface Definition:**
```csharp
// Defined in AI/ICanTakeDamage.cs
public enum BODYPART
{
    NONE, HEAD, BODY, ARM, LEG
}

public interface ICanTakeDamage
{
    void TakeDamage(
        float damage,             // Amount of damage
        Vector2 force,            // Knockback force
        Vector2 hitPoint,         // Where hit occurred
        GameObject instigator,    // Who caused damage
        BODYPART bodyPart,        // Body part hit (for critical hits)
        WeaponEffect weaponEffect // Poison, burn, freeze, etc.
    );
}
```

**Implementation in Enemy.cs:**
```csharp
public class Enemy : MonoBehaviour, ICanTakeDamage
{
    public void TakeDamage(
        float damage,
        Vector2 force,
        Vector2 hitPoint,
        GameObject instigator,
        BODYPART bodyPart = BODYPART.NONE,
        WeaponEffect weaponEffect = null)
    {
        // Already dead? Ignore
        if (enemyState == ENEMYSTATE.DEATH)
            return;

        // Reduce health
        currentHealth -= (int)damage;

        // Show floating damage text
        FloatingTextManager.Instance.ShowText(
            "" + (int)damage,
            healthBarOffset,
            Color.red,
            transform.position
        );

        // Spawn hit effect
        if (hitFX)
            Instantiate(hitFX, hitPoint, Quaternion.identity);

        // Update health bar
        if (healthBar)
            healthBar.UpdateValue(currentHealth / (float)health);

        // Check death
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Apply weapon effects (poison, freeze, etc.)
            if (weaponEffect != null)
            {
                switch (weaponEffect.effectType)
                {
                    case WEAPON_EFFECT.POISON:
                        Poison(weaponEffect.poisonDamagePerSec,
                               weaponEffect.poisonTime,
                               instigator);
                        break;
                    case WEAPON_EFFECT.FREEZE:
                        Freeze(weaponEffect.freezeTime, instigator);
                        break;
                }
            }

            Hit(force);  // Play hit reaction
        }
    }
}
```

**Usage in ArrowProjectile.cs:**
```csharp
void OnTriggerEnter2D(Collider2D other)
{
    // Try to get ICanTakeDamage component
    var takeDamage = (ICanTakeDamage)other.gameObject.GetComponent(typeof(ICanTakeDamage));

    if (takeDamage != null)
    {
        // Deal damage polymorphically
        takeDamage.TakeDamage(
            damage,
            Vector2.zero,
            transform.position,
            gameObject,
            BODYPART.NONE,
            weaponEffect
        );

        // Destroy arrow
        Destroy(gameObject);
    }
}
```

**Who Implements ICanTakeDamage:**
- `Enemy` (base class - all enemies)
- `Player_Archer` (inherits from Enemy, so gets it automatically)
- `TheFortrest` (player's base)

**Why Interface Pattern?**
- ✅ **Polymorphism:** Don't care what object is, just call TakeDamage()
- ✅ **Extensibility:** New damag able objects just implement interface
- ✅ **Unified System:** One damage calculation for everything

### 5.5 Inheritance Hierarchy (Unconventional but Clever)

**Player Inherits from Enemy!**

This is unusual but pragmatic:

```
MonoBehaviour
      │
      └─── Enemy.cs (base class)
           ├─ Health system
           ├─ Damage handling (ICanTakeDamage)
           ├─ Animation system
           ├─ Effects (burn, poison, freeze, shock)
           ├─ IListener implementation
           │
           ├─── SmartEnemyGrounded.cs (most enemies)
           │    └─ Movement AI
           │    └─ Attack logic
           │
           ├─── WitchHeal.cs (special enemy)
           │    └─ Healing ability
           │
           └─── Player_Archer.cs (THE PLAYER!)
                └─ Auto-targeting
                └─ Trajectory shooting
                └─ Movement (can move like enemy)
```

**Why Does Player Inherit from Enemy?**

**Benefits:**
- ✅ **Code Reuse:** Player needs health, damage, effects - Enemy has it all
- ✅ **Unified System:** One damage system for everything
- ✅ **Consistent Behavior:** Player and enemies work the same way
- ✅ **Less Code:** Don't reimplement health/damage/effects

**Considerations:**
- ⚠️ **Unconventional:** Most games separate Player and Enemy hierarchies
- ⚠️ **Potentially Confusing:** New developers might be surprised
- ⚠️ **Tight Coupling:** Player changes might affect enemies

**Player_Archer Specific Additions:**
```csharp
public class Player_Archer : Enemy, ICanTakeDamage, IListener
{
    // UNIQUE TO PLAYER (not in Enemy base class)

    [Header("ARROW SHOOT")]
    public float shootRate = 1;       // Fire rate
    public float force = 20;          // Arrow force
    public ArrowProjectile arrow;     // Arrow prefab
    public int arrowDamage = 30;      // Arrow damage
    public Transform firePostion;     // Spawn point for arrows

    // AUTO-TARGETING SYSTEM (player-only)
    IEnumerator AutoCheckAndShoot()
    {
        while (true)
        {
            // Detect enemies
            // Calculate trajectory
            // Shoot arrow
            yield return new WaitForSeconds(shootRate);
        }
    }

    // TRAJECTORY CALCULATION (player-only)
    IEnumerator CheckTarget()
    {
        // Physics simulation to calculate perfect angle
        // Iterates through angles to find best shot
        // Spawns arrow with calculated force
    }

    // INHERITED FROM ENEMY (reused!)
    // - health, currentHealth
    // - TakeDamage()
    // - Die()
    // - Hit()
    // - Freeze(), Poison(), Burning(), Shoking()
    // - AnimSetTrigger(), AnimSetBool(), AnimSetFloat()
    // - checkTarget (target detection)
    // - enemyState, enemyEffect
}
```

**Comparison:**
```
       Enemy (Goblin)               Player_Archer
       ─────────────               ─────────────
Health: ✓ (from Enemy base)   Health: ✓ (inherited)
Damage: ✓ (from Enemy base)   Damage: ✓ (inherited)
Effects: ✓ (burn, poison...)   Effects: ✓ (inherited)
AI: ✓ (walk toward target)     AI: ❌ (manual/auto-aim)
Attack: ✓ (melee/range)        Attack: ✓ (custom trajectory)
```

### 5.6 Object Pooling (Partial Implementation)

**Purpose:** Reuse objects instead of Instantiate/Destroy every frame

**FloatingTextManager Implementation:**
```csharp
public class FloatingTextManager : MonoBehaviour
{
    public static FloatingTextManager Instance;

    public List<FloatingText> pool;  // Pool of text objects

    public void ShowText(string text, Vector2 offset, Color color, Vector3 position)
    {
        // Get available text from pool
        FloatingText floatingText = GetAvailableText();

        if (floatingText != null)
        {
            // Reuse existing object
            floatingText.gameObject.SetActive(true);
            floatingText.Show(text, offset, color, position);
        }
        else
        {
            // Pool empty, create new
            FloatingText newText = Instantiate(floatingTextPrefab);
            pool.Add(newText);
            newText.Show(text, offset, color, position);
        }
    }

    FloatingText GetAvailableText()
    {
        // Find inactive text in pool
        foreach (var text in pool)
        {
            if (!text.gameObject.activeInHierarchy)
                return text;
        }
        return null;  // Pool full
    }
}
```

**Why Object Pooling?**
- ✅ **Performance:** Instantiate/Destroy are slow
- ✅ **No GC Spikes:** Less garbage collection
- ✅ **Smooth Gameplay:** No frame drops from spawning

**Where Used:**
- `FloatingTextManager` - Damage numbers (pools text objects)
- Projectiles - Partially (arrows are instantiated, could be pooled)

---

## 6. Data Flow & Game Loop

### 6.1 Complete Data Flow

```
   USER INPUT                  GAME LOGIC                   OUTPUT
┌──────────────┐           ┌──────────────┐           ┌──────────────┐
│              │           │              │           │              │
│ Touch/Click  │──────────→│ MenuManager  │──────────→│ UI Updates   │
│ "Play Button"│           │ .StartGame() │           │              │
│              │           │              │           │              │
└──────────────┘           └──────┬───────┘           └──────────────┘
                                  │
                                  ▼
                          ┌────────────────┐
                          │  GameManager   │
                          │  .StartGame()  │
                          │  State=Playing │
                          └────────┬───────┘
                                   │
                      ┌────────────┼────────────┐
                      │                         │
                      ▼                         ▼
         ┌──────────────────────┐   ┌──────────────────────┐
         │  LevelEnemyManager   │   │   Player_Archer      │
         │  .IPlay()            │   │   .IPlay()           │
         │  Start wave spawning │   │   Enable auto-shoot  │
         └──────────┬───────────┘   └──────────┬───────────┘
                    │                           │
                    │                           │
          ┌─────────▼────────┐                  │
          │ Spawn Enemy      │                  │
          │ (Instantiate)    │                  │
          └─────────┬────────┘                  │
                    │                           │
                    ▼                           │
          ┌──────────────────┐                  │
          │ SmartEnemy       │                  │
          │ Grounded.Start() │                  │
          │ - Init health    │                  │
          │ - State = WALK   │                  │
          │ - Move toward    │                  │
          │   fortress       │◄─────────────────┘
          └─────────┬────────┘     (detects enemy)
                    │                           │
                    │                           │
                    │         ┌─────────────────▼──────────┐
                    │         │ Player shoots arrow        │
                    │         │ ArrowProjectile.Init()     │
                    │         │ - Calculate trajectory     │
                    │         │ - Fly toward target        │
                    │         └─────────────┬──────────────┘
                    │                       │
                    │                       │ (collision)
                    │                       │
                    ▼◄──────────────────────┘
          ┌──────────────────────────────────────┐
          │ Enemy.TakeDamage()                   │
          │ - Reduce health                      │
          │ - Apply weapon effects               │
          │ - Update health bar                  │
          │ - Show floating damage text          │
          │ - Check if health <= 0               │
          └─────────────────┬────────────────────┘
                            │
                   ┌────────┴────────┐
                   │                 │
                   │ (alive)         │ (dead)
                   ▼                 ▼
          ┌────────────────┐  ┌─────────────────┐
          │ Enemy.Hit()    │  │ Enemy.Die()     │
          │ - Hit reaction │  │ - State = DEATH │
          │ - Continue     │  │ - Play animation│
          └────────────────┘  │ - Give coins    │
                              │ - Remove from   │
                              │   active list   │
                              │ - Destroy       │
                              └────────┬────────┘
                                       │
                                       ▼
                              ┌─────────────────┐
                              │ GiveCoinWhenDie │
                              │ .GiveCoin()     │
                              └────────┬────────┘
                                       │
                                       ▼
                              ┌─────────────────┐
                              │ GlobalValue     │
                              │ .Coin += amount │
                              └────────┬────────┘
                                       │
                                       ▼
                              ┌─────────────────┐
                              │ UI Update       │
                              │ CoinText.text   │
                              └─────────────────┘
```

### 6.2 Frame-by-Frame Execution

**Every Frame (60 FPS):**
```
1. Unity calls Update() on all active scripts
   ├─ GameManager.Update() - Check game state
   ├─ Player_Archer.Update() - Handle animation
   ├─ Enemy.Update() - Check effects (burn, shock damage)
   ├─ MenuManager.Update() - UI interactions
   └─ ... (60+ Update methods called)

2. Unity calls FixedUpdate() on physics scripts (50 FPS)
   ├─ Player_Archer.LateUpdate() - Move character (custom physics)
   ├─ SmartEnemyGrounded.FixedUpdate() - Move enemies
   └─ Controller2D.Move() - Raycast collision detection

3. Unity calls LateUpdate() for things that depend on Update
   ├─ CameraController.LateUpdate() - Follow player (after movement)
   ├─ HealthBar.LateUpdate() - Follow enemy (after movement)
   └─ ...

4. Unity renders the frame
   ├─ Sprite Renderers draw characters
   ├─ UI Canvas draws interface
   ├─ Particle systems draw effects
   └─ Camera captures and displays

5. Unity handles input (touch, mouse, keyboard)
   └─ Input.GetKey(), Input.GetTouch(), etc.

⟲ REPEAT (every ~16ms for 60 FPS)
```

---

## 7. Scene Flow

### 7.1 Scene Structure

This project uses **ONE main scene** with dynamic level loading:

```
SampleScene.unity (Only Scene)
├─ Persistent Objects (DontDestroyOnLoad or always present)
│   ├─ GameManager (spawns level)
│   ├─ MenuManager (UI controller)
│   ├─ SoundManager (audio)
│   ├─ EventSystem (UI input)
│   └─ Canvas (UI root)
│
├─ Level Prefab (Spawned at Runtime)
│   ├─ GameLevelSetup (configuration)
│   ├─ LevelEnemyManager (wave spawning)
│   ├─ Fortress (defense target)
│   ├─ Background (visual)
│   ├─ Ground (collision)
│   └─ Spawn Points (enemy entry)
│
├─ Player (Spawned at Runtime)
│   └─ Player_Archer
│
└─ UI Elements
    ├─ MainMenu (hidden during gameplay)
    ├─ GameUI (visible during gameplay)
    ├─ PauseMenu (shown on pause)
    ├─ VictoryScreen (shown on win)
    └─ GameOverScreen (shown on lose)
```

### 7.2 Level Loading Flow

```
Game Start
    │
    ▼
┌──────────────────────────────────────┐
│ GameManager.Awake()                  │
│ - Set target frame rate to 60        │
│ - Create Singleton instance           │
│ - State = Menu                        │
│ - Initialize listeners list           │
│ - Get current level index             │
│   from GlobalValue.levelPlaying      │
│ - Instantiate level prefab:           │
│   gameLevels[levelPlaying - 1]       │
└────────────────┬─────────────────────┘
                 │
                 ▼
┌──────────────────────────────────────┐
│ Level Prefab Instantiated            │
│ - GameLevelSetup.Awake()             │
│ - LevelEnemyManager.Awake()          │
│ - Fortress spawned                   │
│ - Background loaded                  │
│ - Player spawned (CharacterManager)  │
└────────────────┬─────────────────────┘
                 │
                 ▼
┌──────────────────────────────────────┐
│ MenuManager.Start()                  │
│ - Show main menu UI                  │
│ - Display level info                 │
│ - Wait for user to press "Play"      │
└────────────────┬─────────────────────┘
                 │
         ┌───────┴───────┐
         │ User clicks   │
         │ "PLAY" button │
         └───────┬───────┘
                 │
                 ▼
┌──────────────────────────────────────┐
│ MenuManager.OnPlayButtonClick()      │
│ - Call GameManager.StartGame()       │
└────────────────┬─────────────────────┘
                 │
                 ▼
┌──────────────────────────────────────┐
│ GameManager.StartGame()              │
│ - State = Playing                    │
│ - Find all IListeners                │
│ - Call IPlay() on each               │
└────────────────┬─────────────────────┘
                 │
                 ▼
      [GAMEPLAY ACTIVE]
```

### 7.3 UI State Transitions

```
┌──────────────┐
│  MAIN MENU   │ ← Game starts here
│              │
│ - Title      │
│ - Play Btn   │
│ - Shop Btn   │
│ - Settings   │
└──────┬───────┘
       │
       │ (click Map)
       ▼
┌──────────────┐
│  MAP SELECT  │
│              │
│ - Level icons│
│ - Stars      │
│ - Locked     │
└──────┬───────┘
       │
       │ (select level)
       ▼
┌──────────────┐
│ LEVEL INTRO  │
│              │
│ - Level info │
│ - Play button│
└──────┬───────┘
       │
       │ (click Play)
       ▼
┌──────────────┐
│  GAMEPLAY UI │
│              │
│ - Health bar │
│ - Coin count │
│ - Wave info  │
│ - Pause btn  │
└──────┬───────┘
       │
       ├─────────────────┐
       │                 │
       │ (during game)   │ (Pause button)
       │                 ▼
       │          ┌──────────────┐
       │          │  PAUSE MENU  │
       │          │              │
       │          │ - Resume     │
       │          │ - Restart    │
       │          │ - Quit       │
       │          └──────┬───────┘
       │                 │
       │                 │ (Resume)
       │◄────────────────┘
       │
       ├─────────────────┬─────────────────┐
       │                 │                 │
       │ (all waves      │ (fortress HP    │
       │  cleared)       │  reaches 0)     │
       ▼                 ▼                 ▼
┌──────────────┐  ┌──────────────┐  ┌──────────────┐
│VICTORY SCREEN│  │GAMEOVER      │  │   (playing)  │
│              │  │              │  │              │
│ - Stars (1-3)│  │ - Defeat msg │  │   continues  │
│ - Coins earned│  │ - Retry btn │  │              │
│ - Next level │  │ - Menu btn   │  │              │
│ - Rewards    │  └──────────────┘  └──────────────┘
└──────┬───────┘
       │
       │ (Next/Menu)
       ▼
┌──────────────┐
│  MAP SELECT  │ (cycle back)
└──────────────┘
```

---

## 8. System Dependencies Map

### 8.1 Complete Dependency Graph

```
┌─────────────────────────────────────────────────────────────────┐
│                         GAMEMANAGER                             │
│                    (Core Coordinator)                           │
└───────────────────────────┬─────────────────────────────────────┘
                            │
        ┌───────────────────┼───────────────────┬────────────────┐
        │                   │                   │                │
        ▼                   ▼                   ▼                ▼
┌───────────────┐  ┌────────────────┐ ┌────────────────┐ ┌───────────┐
│ GLOBALVALUE   │  │ SOUNDMANAGER   │ │ FLOATINGTEXT   │ │ ADSMANAGER│
│ (Data Storage)│  │ (Audio)        │ │ MANAGER (UI)   │ │ (Ads)     │
└───────┬───────┘  └────────┬───────┘ └────────┬───────┘ └───────────┘
        │                   │                   │
        │                   │                   │
        ▼                   ▼                   ▼
   PlayerPrefs          AudioSource        ObjectPool

──────────────────────────────────────────────────────────────────────

         PLAYER SYSTEM                    ENEMY SYSTEM
┌───────────────────────────┐    ┌───────────────────────────┐
│ Player_Archer             │    │ SmartEnemyGrounded        │
│ (inherits Enemy)          │    │ (inherits Enemy)          │
└───────────┬───────────────┘    └───────────┬───────────────┘
            │                                 │
            │ depends on                      │ depends on
            │                                 │
     ┌──────┴──────────┬──────────┬──────────┴──────────┬─────────┐
     │                 │          │                     │         │
     ▼                 ▼          ▼                     ▼         ▼
┌─────────┐  ┌──────────────┐ ┌─────────┐  ┌─────────────────┐ ┌──────┐
│Controller│ │CheckTarget   │ │ Arrow   │  │ EnemyMelee     │ │Enemy │
│2D        │ │Helper        │ │Projectile│ │ Attack         │ │Range │
└─────────┘  └──────────────┘ └─────────┘  └─────────────────┘ │Attack│
     │                            │                │              └──────┘
     │                            │                │
     ▼                            ▼                ▼
RaycastController          ICanTakeDamage    CheckTargetHelper

──────────────────────────────────────────────────────────────────────

            UI SYSTEM                     MANAGERS
┌───────────────────────────┐    ┌───────────────────────────┐
│ MenuManager               │    │ LevelEnemyManager         │
└───────────┬───────────────┘    └───────────┬───────────────┘
            │                                 │
            │ manages                         │ manages
            │                                 │
     ┌──────┴──────────┬──────────┬──────────┴──────────┬─────────┐
     │                 │          │                     │         │
     ▼                 ▼          ▼                     ▼         ▼
┌─────────┐  ┌──────────────┐ ┌─────────┐  ┌─────────────────┐ ┌──────┐
│ Victory │  │ GameOver UI │ │ Pause   │  │ EnemyWave       │ │Enemy │
│ Screen  │  │             │ │ Menu    │  │ (config)        │ │Spawn │
└─────────┘  └──────────────┘ └─────────┘  └─────────────────┘ │(data)│
                                                                 └──────┘

──────────────────────────────────────────────────────────────────────

         HELPER SYSTEMS
┌─────────────────────┐
│ AnimationHelper     │ ← Used by Enemy, Player for animation lengths
├─────────────────────┤
│ SpawnItemHelper     │ ← Used for spawning pickups
├─────────────────────┤
│ WeaponEffect        │ ← Data for poison, burn, freeze, shock
├─────────────────────┤
│ AutoDestroy         │ ← Auto-destroy objects after time
└─────────────────────┘
```

### 8.2 Critical Dependencies

**Player_Archer depends on:**
```
Player_Archer.cs
├─ Enemy.cs (base class - INHERITANCE)
│   ├─ ICanTakeDamage (interface)
│   ├─ IListener (interface)
│   ├─ CheckTargetHelper (target detection)
│   ├─ Animator (animations)
│   ├─ HealthBarEnemyNew (health display)
│   └─ FloatingTextManager (damage numbers)
│
├─ Controller2D.cs (movement)
│   └─ RaycastController.cs (collision detection)
│
├─ ArrowProjectile.cs (shooting)
│   ├─ Projectile.cs (base class)
│   └─ WeaponEffect.cs (arrow effects)
│
├─ CheckTargetHelper.cs (enemy detection)
├─ UpgradedCharacterParameter.cs (stats)
│   └─ PlayerPrefs (data storage)
│
├─ GameManager.cs (game state, listeners)
├─ SoundManager.cs (sound effects)
└─ GlobalValue.cs (player data)
```

**Enemy depends on:**
```
Enemy.cs (base class)
├─ ICanTakeDamage (interface - must implement)
├─ IListener (interface - must implement)
│
├─ CheckTargetHelper.cs (target detection)
├─ Animator (animations)
├─ HealthBarEnemyNew (health display)
│
├─ FloatingTextManager.cs (damage numbers)
├─ SoundManager.cs (sound effects)
├─ GameManager.cs (game state, register/remove)
│
├─ GiveCoinWhenDie.cs (optional - rewards)
├─ WeaponEffect.cs (effect data)
└─ UpgradedCharacterParameter.cs (enemy stats)
```

**GameManager depends on:**
```
GameManager.cs
├─ IListener (interface - broadcasts to)
│   ├─ Player_Archer (listener)
│   ├─ Enemy (listener)
│   ├─ LevelEnemyManager (listener)
│   ├─ MenuManager (listener)
│   └─ UI components (listeners)
│
├─ GameMode.cs (game mode data)
├─ GlobalValue.cs (player progress)
├─ AdsManager.cs (ad system)
└─ SoundManager.cs (audio control)
```

---

## 9. Key Classes Reference

### 9.1 Critical Classes Quick Reference

| Class | Type | Purpose | Access Pattern |
|-------|------|---------|----------------|
| `GameManager` | Singleton Manager | Game state control, listener coordinator | `GameManager.Instance` |
| `Enemy` | Base Class | All enemy/player health, damage, effects | Inherited by enemies/player |
| `Player_Archer` | Player Controller | Player character, auto-shooting | Direct reference or FindObjectOfType |
| `Controller2D` | Physics Controller | Custom 2D movement with raycasts | GetComponent on character |
| `LevelEnemyManager` | Manager | Wave spawning system | Direct reference or FindObjectOfType |
| `MenuManager` | UI Controller | Menu navigation, UI state | Direct reference |
| `SoundManager` | Singleton Manager | Audio playback | `SoundManager.Instance` |
| `GlobalValue` | Static Data Store | Player progress, coins, unlocks | `GlobalValue.Coin`, `GlobalValue.LevelPass` |
| `FloatingTextManager` | Singleton Manager | Damage number pooling | `FloatingTextManager.Instance` |
| `IListener` | Interface | Game event listener | Implement in classes needing game events |
| `ICanTakeDamage` | Interface | Damage receiver | Implement in damageable objects |

### 9.2 Enums Reference

**Game States:**
```csharp
// GameManager.GameState
Menu       // Main menu, not playing
Playing    // Gameplay active
GameOver   // Defeat
Success    // Victory
Pause      // Game paused
```

**Enemy States:**
```csharp
// ENEMYSTATE
SPAWNING   // Appearing (spawn animation)
IDLE       // Standing still
WALK       // Moving toward target
ATTACK     // Attacking
HIT        // Just took damage
DEATH      // Dead (death animation)
```

**Enemy Attack Types:**
```csharp
// ATTACKTYPE
RANGE      // Ranged attack (shoot projectile)
MELEE      // Melee attack (close range)
THROW      // Throwing attack
NONE       // No attack (passive enemy)
```

**Weapon Effects:**
```csharp
// ENEMYEFFECT
NONE       // No effect
BURNING    // Damage over time (fire)
FREEZE     // Slow/immobilize
SHOKING    // Stun (can't move)
POISON     // Damage over time (poison)
EXPLOSION  // Explodes on death
```

**Body Parts (for critical hits):**
```csharp
// BODYPART
NONE       // Normal hit
HEAD       // Headshot (could do more damage)
BODY       // Body hit
ARM        // Arm hit
LEG        // Leg hit
```

---

## 10. Namespace Organization

### 10.1 RGame Namespace

All game scripts are wrapped in the `RGame` namespace:

```csharp
using UnityEngine;

namespace RGame  // All game code uses this namespace
{
    public class GameManager : MonoBehaviour
    {
        // ...
    }
}
```

**Why Use a Namespace?**
- ✅ **Prevent Name Conflicts:** If you import an asset with a class named "Enemy", it won't conflict with RGame.Enemy
- ✅ **Organization:** Groups all game code together
- ✅ **Professional Practice:** Industry standard for larger projects
- ✅ **Import Clarity:** Other scripts know where classes come from

**Using RGame Classes:**
```csharp
// Option 1: Import the namespace
using RGame;

public class MyScript : MonoBehaviour
{
    void Start()
    {
        // Can use RGame classes directly
        GameManager gm = GameManager.Instance;
        Enemy enemy = GetComponent<Enemy>();
    }
}

// Option 2: Fully qualify (no import)
public class MyScript : MonoBehaviour
{
    void Start()
    {
        // Specify namespace each time
        RGame.GameManager gm = RGame.GameManager.Instance;
        RGame.Enemy enemy = GetComponent<RGame.Enemy>();
    }
}
```

**RGame Namespace Includes:**
- All Managers (GameManager, LevelEnemyManager, etc.)
- All AI (Enemy, SmartEnemyGrounded, etc.)
- All Controllers (Controller2D, Projectile, etc.)
- All Helpers
- Player classes
- UI classes

**External Classes (not in RGame):**
- Unity built-in classes (MonoBehaviour, GameObject, etc.)
- C# standard library (List, Dictionary, etc.)
- Plugins (AdMob, Unity IAP, etc.)

---

## 11. Performance Considerations

### 11.1 Optimization Strategies Used

**Custom Physics (Not Rigidbody2D):**
- ✅ More control over performance
- ✅ Precise collision detection
- ✅ No unexpected physics interactions

**Object Pooling (Floating Text):**
- ✅ Reduces Instantiate/Destroy calls
- ✅ Prevents garbage collection spikes

**Coroutines for Delays:**
- ✅ Non-blocking delays
- ✅ Better than Update() checks

**Caching Component References:**
```csharp
// GOOD - Cache in Awake/Start
private Rigidbody2D rb;
void Awake() { rb = GetComponent<Rigidbody2D>(); }
void Update() { rb.velocity = ...; }

// BAD - GetComponent every frame
void Update() { GetComponent<Rigidbody2D>().velocity = ...; }
```

**Target Frame Rate:**
```csharp
void Awake()
{
    Application.targetFrameRate = 60;  // Mobile optimization
}
```

---

## 12. What's Next?

Now that you understand the architecture, dive into specific systems:

**Recommended Reading Order:**
1. ✅ **`02_Player_System_Complete.md`** - Understand Player_Archer in detail
2. ✅ **`03_Enemy_System_Complete.md`** - Deep dive into Enemy AI
3. ✅ **`04_UI_System_Complete.md`** - UI implementation
4. ✅ **`05_Managers_Complete.md`** - Manager classes explained
5. ✅ **`10_How_To_Guides.md`** - Practical modification guides

**Key Takeaways:**
- Game uses **Singleton + Listener pattern** for coordination
- **Player inherits from Enemy** (unconventional but practical)
- **Custom 2D physics** using raycasts (not Rigidbody2D)
- **State machines** control enemy and game flow
- **One scene** with dynamic level loading
- **Namespace RGame** wraps all game code

---

**You now have a comprehensive understanding of the project architecture!**

**Next Document:** → `02_Player_System_Complete.md`
