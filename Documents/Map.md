# Map System (Level Selection & Scene Loading)

**Purpose**: Complete technical guide to the level selection UI, map navigation system, and how levels/scenes are loaded and managed in "Lawn Defense: Monsters Out".

**What This Document Covers**:
- Map UI navigation and pagination system
- Level selection and world switching
- Scene loading with async operations
- Level configuration with GameLevelSetup
- Creating and configuring new levels
- Troubleshooting common map/level issues

**Related Documentation**:
- See `05_Managers_Complete.md` for GameManager and scene management
- See `04_UI_System_Complete.md` for UI architecture
- See `Enemy-Deep.md` for enemy wave configuration
- See `10_How_To_Guides.md` for practical tutorials

---

## Table of Contents

1. [Unity Basics You Need](#unity-basics-you-need)
2. [System Overview](#system-overview)
3. [MapControllerUI - Navigation System](#mapcontrollerui---navigation-system)
4. [GameLevelSetup - Level Configuration](#gamelevelsetup---level-configuration)
5. [MenuManager - Scene Loading](#menumanager---scene-loading)
6. [Complete Level Loading Flow](#complete-level-loading-flow)
7. [Creating New Levels](#creating-new-levels)
8. [Advanced Techniques](#advanced-techniques)
9. [Troubleshooting](#troubleshooting)

---

## Unity Basics You Need

Before understanding the map system, you should know these Unity concepts:

### 1. **Scene Management**
```
A Scene is a Unity file that contains game objects and configurations.
Think of it like a "level" or "room" in your game.

Example scenes:
- Menu scene (main menu)
- Game scene (gameplay)
- Map scene (level selection)
```

### 2. **SceneManager.LoadSceneAsync**
```csharp
// Asynchronous = loads in the background while showing progress
AsyncOperation operation = SceneManager.LoadSceneAsync("Menu");

// Synchronous = freezes game until loaded (BAD for large scenes!)
SceneManager.LoadScene("Menu"); // Don't use this for big scenes
```

### 3. **Prefab Instantiation**
```csharp
// Prefabs are templates stored in Project window
// Instantiate = create a copy in the scene
GameObject level = Instantiate(levelPrefab, Vector2.zero, Quaternion.identity);
```

### 4. **RectTransform**
```
RectTransform is used for UI positioning (not regular Transform).
- anchoredPosition: Position relative to anchor point
- sizeDelta: Width and height
```

### 5. **Coroutines**
```csharp
// Coroutines run over multiple frames, allowing delays
IEnumerator MyCoroutine()
{
    yield return new WaitForSeconds(1f); // Wait 1 second
    // Code here runs after 1 second
}

// Start it with:
StartCoroutine(MyCoroutine());
```

---

## System Overview

The map system has **three main components** working together:

```
┌────────────────────────────────────────────────────────────────┐
│                     MAP SYSTEM ARCHITECTURE                     │
└────────────────────────────────────────────────────────────────┘

1. MapControllerUI
   ├─ Handles UI navigation (Previous/Next buttons)
   ├─ Displays world/block indicators (dots)
   └─ Animates map scrolling

2. GameLevelSetup
   ├─ Stores level configurations (waves, mana, fortress HP)
   ├─ Provides level data to LevelEnemyManager
   └─ Auto-fills mana over time

3. MenuManager
   ├─ Loads scenes asynchronously
   ├─ Shows loading screen with progress bar
   └─ Handles scene transitions (restart, next level, main menu)

┌─────────────────────────────────────────────────────────────────┐
│                        COMPLETE FLOW                            │
└─────────────────────────────────────────────────────────────────┘

Player Opens Map Screen
        │
        ├─► MapControllerUI.Start()
        │   └─ Sets dots to show current world position
        │
Player Clicks "Next World" Button
        │
        ├─► MapControllerUI.Next()
        │   ├─ Plays click sound
        │   ├─ Shows black screen effect
        │   ├─ Scrolls map UI to next block
        │   └─ Updates dot indicators
        │
Player Selects Level
        │
        ├─► MenuManager.LoadAsynchronously("Game")
        │   ├─ Shows loading UI
        │   ├─ Loads scene in background
        │   └─ Updates progress bar (0% → 100%)
        │
Scene Loads → GameManager.Awake()
        │
        ├─► Instantiates level prefab from gameLevels array
        │   └─ Uses GlobalValue.levelPlaying to pick level
        │
Level Prefab Contains GameLevelSetup Component
        │
        └─► GameLevelSetup.Awake()
            ├─ Sets itself as singleton Instance
            ├─ Sets GlobalValue.finishGameAtLevel
            └─ Provides wave data to LevelEnemyManager
```

---

## MapControllerUI - Navigation System

**Location**: `Assets/_MonstersOut/Scripts/UI/MapControllerUI.cs`

**Purpose**: Controls the map/world selection UI with horizontal scrolling navigation.

### Key Concepts

Think of the map UI like a **horizontal carousel** of world blocks:

```
┌──────────────────────────────────────────────────────────────┐
│                        MAP CAROUSEL                          │
└──────────────────────────────────────────────────────────────┘

     ┌───────┐   ┌───────┐   ┌───────┐
     │ World │   │ World │   │ World │
     │   1   │   │   2   │   │   3   │  ← Blocks (children)
     └───────┘   └───────┘   └───────┘
         ▲
         │
    Current Position

◉ ○ ○  ← Dots (indicators showing which world is visible)

[◀ Previous]  [Next ▶]  ← Navigation buttons
```

### Class Structure

```csharp
namespace RGame
{
    public class MapControllerUI : MonoBehaviour
    {
        // CONFIGURATION
        public RectTransform BlockLevel;    // Container holding all world blocks
        public int howManyBlocks = 3;       // Total number of worlds
        public float step = 720f;           // Distance to scroll per world (in pixels)
        public Image[] Dots;                // Dot indicators for each world
        public AudioClip music;             // Music to play on map screen

        // STATE
        private float newPosX = 0;          // Current scroll position
        int currentPos = 0;                 // Current world index (0, 1, 2...)
        bool allowPressButton = true;       // Prevent double-clicking
    }
}
```

### How It Works: Initialization

**Step 1: Start() - Set Up Dots**

```csharp
void Start()
{
    SetDots();  // Initialize dot indicators
}

void SetDots()
{
    // Step 1: Make all dots semi-transparent and small
    foreach (var obj in Dots)
    {
        obj.color = new Color(1, 1, 1, 0.5f);  // 50% opacity white
        obj.rectTransform.sizeDelta = new Vector2(28, 28);  // 28x28 pixels
    }

    // Step 2: Highlight the current world's dot
    Dots[currentPos].color = Color.yellow;  // Full yellow
    Dots[currentPos].rectTransform.sizeDelta = new Vector2(38, 38);  // Bigger (38x38)
}
```

**What This Does**:
```
Before SetDots():          After SetDots():
○ ○ ○                      ◉ ○ ○
(all same)                 (first one highlighted)
```

**Step 2: OnEnable() - Play Map Music**

```csharp
void OnEnable()
{
    SoundManager.PlayMusic(music);  // Play map screen music
}

void OnDisable()
{
    SoundManager.PlayMusic(SoundManager.Instance.musicsGame);  // Restore game music
}
```

**Why This Matters**: When player opens map, they hear different music. When they close it, game music resumes.

---

### How It Works: Navigation

**Clicking "Next" Button**

```csharp
public void Next()
{
    if (allowPressButton)  // Prevent double-click
    {
        StartCoroutine(NextCo());
    }
}

IEnumerator NextCo()
{
    // STEP 1: Prevent double-clicking
    allowPressButton = false;
    SoundManager.Click();  // Play click sound

    // STEP 2: Check if we're already at the last world
    if (newPosX != (-step * (howManyBlocks - 1)))
    {
        // We can move forward
        currentPos++;  // Move to next world (0 → 1, 1 → 2, etc.)

        newPosX -= step;  // Move left by one step (e.g., -720 pixels)
        newPosX = Mathf.Clamp(newPosX, -step * (howManyBlocks - 1), 0);
        // Clamp ensures we never scroll past the last world
    }
    else
    {
        // Already at last world, can't go further
        allowPressButton = true;
        yield break;  // Exit coroutine
    }

    // STEP 3: Animate transition
    BlackScreenUI.instance.Show(0.15f);  // Fade to black
    yield return new WaitForSeconds(0.15f);  // Wait for fade

    // STEP 4: Update map position (instant snap while screen is black)
    SetMapPosition();  // Move the BlockLevel container

    // STEP 5: Fade back in
    BlackScreenUI.instance.Hide(0.15f);  // Fade from black

    // STEP 6: Update dot indicators
    SetDots();  // Highlight new current world

    // STEP 7: Re-enable button
    allowPressButton = true;
}

void SetMapPosition()
{
    // Move the entire BlockLevel container horizontally
    BlockLevel.anchoredPosition = new Vector2(newPosX, BlockLevel.anchoredPosition.y);
}
```

**Visual Example of Navigation**:
```
Initial State (World 1):
┌─────────┐
│ [World1]│   World2   World3
└─────────┘
Position: newPosX = 0
Dots: ◉ ○ ○

After Next() (World 2):
   World1   [World2]   World3
            └─────────┐
Position: newPosX = -720
Dots: ○ ◉ ○

After Next() Again (World 3):
   World1   World2   [World3]
                    └─────────┐
Position: newPosX = -1440
Dots: ○ ○ ◉
```

**Math Breakdown**:
```
howManyBlocks = 3
step = 720

Max scroll = -step * (howManyBlocks - 1)
           = -720 * (3 - 1)
           = -720 * 2
           = -1440 pixels

World 1: newPosX = 0
World 2: newPosX = -720
World 3: newPosX = -1440
```

**Clicking "Previous" Button**

```csharp
public void Pre()
{
    if (allowPressButton)
    {
        StartCoroutine(PreCo());
    }
}

IEnumerator PreCo()
{
    allowPressButton = false;
    SoundManager.Click();

    // Check if we're already at the first world
    if (newPosX != 0)
    {
        // We can move backward
        currentPos--;  // 2 → 1, 1 → 0

        newPosX += step;  // Move right by one step (+720 pixels)
        newPosX = Mathf.Clamp(newPosX, -step * (howManyBlocks - 1), 0);
    }
    else
    {
        // Already at first world
        allowPressButton = true;
        yield break;
    }

    // Same animation as Next()
    BlackScreenUI.instance.Show(0.15f);
    yield return new WaitForSeconds(0.15f);
    SetMapPosition();
    BlackScreenUI.instance.Hide(0.15f);
    SetDots();
    allowPressButton = true;
}
```

---

### Utility: Unlock All Levels (Developer Tool)

```csharp
public void UnlockAllLevels()
{
    // Add 1000 to levels passed (unlock everything)
    GlobalValue.LevelPass = (GlobalValue.LevelPass + 1000);

    // Reload the current scene to refresh UI
    UnityEngine.SceneManagement.SceneManager.LoadScene(
        UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
    );

    SoundManager.Click();
}
```

**Usage**: Attach this to a hidden debug button for testing.

---

## GameLevelSetup - Level Configuration

**Location**: `Assets/_MonstersOut/Scripts/Managers/GameLevelSetup.cs`

**Purpose**: Stores and provides level-specific configurations (enemy waves, mana, fortress health).

### Key Concepts

Each **level prefab** has a `GameLevelSetup` component that stores:
- **Enemy waves** for that level
- **Starting mana**
- **Enemy fortress health**
- **Auto-mana refill rate**

```
Level Prefab Hierarchy:
┌──────────────────────────────┐
│ Level_1 (Prefab)             │
├──────────────────────────────┤
│ ├─ GameLevelSetup (Component)│
│ │  ├─ levelWaves: List       │
│ │  │  ├─ Level 1 (LevelWave) │
│ │  │  ├─ Level 2 (LevelWave) │
│ │  │  └─ Level 3 (LevelWave) │
│ │  ├─ amountMana: 2          │
│ │  └─ rate: 2.0              │
│ ├─ LevelEnemyManager         │
│ └─ Other level objects...    │
└──────────────────────────────┘
```

### Class Structure

```csharp
namespace RGame
{
    public class GameLevelSetup : MonoBehaviour
    {
        public static GameLevelSetup Instance;  // Singleton

        [Header("===AUTO FILL MANA===")]
        public int amountMana = 2;      // Mana added per interval
        public float rate = 2;          // Interval in seconds (add 2 mana every 2 seconds)

        [ReadOnly]
        public List<LevelWave> levelWaves = new List<LevelWave>();  // All level configs

        void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Persist across scene loads

            // Set the final level number
            GlobalValue.finishGameAtLevel = levelWaves.Count;
        }
    }
}
```

**LevelWave Structure**:

```csharp
public class LevelWave : MonoBehaviour
{
    public int level = 1;              // Level number (1, 2, 3...)
    public int givenMana = 1000;       // Starting mana for this level

    [Range(1, 5)]
    public int enemyFortrestLevel = 1; // Enemy fortress health tier

    public EnemyWave[] Waves;          // Enemy spawn waves
}
```

**EnemyWave Structure** (from LevelEnemyManager):

```csharp
[System.Serializable]
public class EnemyWave
{
    public EnemySpawn[] enemyInWaves;  // Enemies in this wave
}

[System.Serializable]
public class EnemySpawn
{
    public GameObject enemyPrefab;     // Which enemy to spawn
    public int amount;                 // How many
    public float delay;                // Delay between spawns
}
```

---

### How It Works: Providing Level Data

**Method 1: GetLevelWave() - Get Enemy Waves**

```csharp
public EnemyWave[] GetLevelWave()
{
    // Loop through all level configurations
    foreach (var obj in levelWaves)
    {
        // Find the level that matches current level being played
        if (obj.level == GlobalValue.levelPlaying)
            return obj.Waves;  // Return the enemy waves for this level
    }

    return null;  // Level not found
}
```

**Example Usage**:
```csharp
// In LevelEnemyManager.cs:
EnemyWave[] waves = GameLevelSetup.Instance.GetLevelWave();

// waves[0] = First wave (e.g., 5 goblins)
// waves[1] = Second wave (e.g., 3 skeletons)
// waves[2] = Third wave (e.g., 1 boss)
```

**Method 2: GetEnemyFortrestLevel() - Get Enemy Fortress HP**

```csharp
public int GetEnemyFortrestLevel()
{
    foreach (var obj in levelWaves)
    {
        if (obj.level == GlobalValue.levelPlaying)
            return obj.enemyFortrestLevel;  // Returns 1-5
    }

    return 1;  // Default to level 1
}
```

**What This Means**:
```
enemyFortrestLevel = 1 → Enemy fortress has 500 HP
enemyFortrestLevel = 2 → Enemy fortress has 1000 HP
enemyFortrestLevel = 3 → Enemy fortress has 1500 HP
... etc.
```

**Method 3: GetGivenMana() - Get Starting Mana**

```csharp
public int GetGivenMana()
{
    foreach (var obj in levelWaves)
    {
        if (obj.level == GlobalValue.levelPlaying)
            return obj.givenMana;  // Starting mana for level
    }

    return -1;  // Error: level not found
}
```

**Method 4: isFinalLevel() - Check If This Is The Last Level**

```csharp
public bool isFinalLevel()
{
    return GlobalValue.levelPlaying == levelWaves.Count;
}
```

**Usage**:
```csharp
if (GameLevelSetup.Instance.isFinalLevel())
{
    // Show special victory screen
    // Unlock special reward
}
```

---

### Editor Tool: OnDrawGizmos()

This method **automatically generates level configs** in the Unity Editor:

```csharp
private void OnDrawGizmos()
{
    // Check if the number of child LevelWave objects has changed
    if (levelWaves.Count != transform.childCount)
    {
        // Re-scan all child LevelWave components
        var waves = transform.GetComponentsInChildren<LevelWave>();
        levelWaves = new List<LevelWave>(waves);

        // Auto-number them
        for (int i = 0; i < levelWaves.Count; i++)
        {
            levelWaves[i].level = i + 1;  // Level 1, 2, 3...
            levelWaves[i].gameObject.name = "Level " + levelWaves[i].level;
        }
    }
}
```

**What This Does**:
```
Hierarchy Before:                Hierarchy After OnDrawGizmos():
├─ GameLevelSetup               ├─ GameLevelSetup
│  ├─ LevelWave                 │  ├─ Level 1  (level = 1)
│  ├─ LevelWave                 │  ├─ Level 2  (level = 2)
│  └─ LevelWave                 │  └─ Level 3  (level = 3)
```

**Why This Is Useful**: You don't have to manually set level numbers—Unity does it automatically!

---

## MenuManager - Scene Loading

**Location**: `Assets/_MonstersOut/Scripts/UI/MenuManager.cs`

**Purpose**: Handles asynchronous scene loading with progress bar UI.

### Key Concepts

**Synchronous vs Asynchronous Loading**:

```
Synchronous (BAD):
SceneManager.LoadScene("Game");
└─ Game freezes until scene loads (might take 3-5 seconds!)

Asynchronous (GOOD):
SceneManager.LoadSceneAsync("Game");
├─ Game keeps running
├─ Shows loading screen
└─ Updates progress bar (0% → 100%)
```

### LoadAsynchronously Method

```csharp
[Header("Load scene")]
public Slider slider;         // Progress bar UI
public Text progressText;     // "Loading... 45%" text

IEnumerator LoadAsynchronously(string name)
{
    // STEP 1: Show loading screen
    LoadingUI.SetActive(true);

    // STEP 2: Start async scene load operation
    AsyncOperation operation = SceneManager.LoadSceneAsync(name);

    // STEP 3: Update progress bar while loading
    while (!operation.isDone)  // Loop until scene finishes loading
    {
        // operation.progress goes from 0 to 0.9, so we normalize to 0-1
        float progress = Mathf.Clamp01(operation.progress / 0.9f);

        // Update UI
        slider.value = progress;  // Set progress bar fill (0.0 to 1.0)
        progressText.text = (int)(progress * 100f) + "%";  // "45%", "67%", etc.

        yield return null;  // Wait one frame, then check again
    }

    // STEP 4: Scene is now loaded! Unity automatically switches to it.
}
```

**Visual Flow**:
```
Frame 1:  progress = 0.0   → slider = 0%    → "0%"
Frame 50: progress = 0.25  → slider = 25%   → "25%"
Frame 100: progress = 0.5  → slider = 50%   → "50%"
Frame 150: progress = 0.75 → slider = 75%   → "75%"
Frame 200: progress = 1.0  → slider = 100%  → "100%"
          → isDone = true  → Loop exits
          → New scene activates
```

**Why Divide by 0.9?**

Unity's `operation.progress` goes from **0 to 0.9**, not 0 to 1.0. The last 10% is reserved for scene activation. Dividing by 0.9 normalizes it:

```
operation.progress = 0.0  → 0.0 / 0.9 = 0.00 (0%)
operation.progress = 0.45 → 0.45 / 0.9 = 0.50 (50%)
operation.progress = 0.9  → 0.9 / 0.9 = 1.00 (100%)
```

---

### Scene Loading Helpers

**Load Main Menu**:
```csharp
public void LoadHomeMenuScene()
{
    SoundManager.Click();  // Play click sound
    StartCoroutine(LoadAsynchronously("Menu"));  // Load "Menu" scene
}
```

**Restart Current Level**:
```csharp
public void RestarLevel()
{
    SoundManager.Click();
    StartCoroutine(LoadAsynchronously(
        SceneManager.GetActiveScene().name  // Reload current scene
    ));
}
```

**Load Next Level**:
```csharp
public void LoadNextLevel()
{
    SoundManager.Click();
    GlobalValue.levelPlaying++;  // Increment level number (1 → 2 → 3)
    StartCoroutine(LoadAsynchronously(
        SceneManager.GetActiveScene().name  // Reload game scene with new level
    ));
}
```

**Important Note**: `LoadNextLevel()` doesn't load a different scene—it **reloads the same scene** but with `GlobalValue.levelPlaying` incremented. GameManager then instantiates a different level prefab.

---

## Complete Level Loading Flow

Here's the **complete end-to-end flow** when a player starts a level:

### Flow Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│              COMPLETE LEVEL LOADING SEQUENCE                    │
└─────────────────────────────────────────────────────────────────┘

1. PLAYER ACTION
   └─ Player clicks "Start Level 5" button

2. UI EVENT
   └─ Button.onClick() → LevelButton.OnClick()
      └─ Sets GlobalValue.levelPlaying = 5
      └─ Calls MenuManager.LoadAsynchronously("Game")

3. ASYNC LOADING BEGINS
   ├─ MenuManager shows LoadingUI
   ├─ SceneManager.LoadSceneAsync("Game") starts
   ├─ Progress bar updates (0% → 100%)
   └─ Scene loads in background

4. GAME SCENE ACTIVATES
   └─ GameManager.Awake() runs

5. LEVEL PREFAB INSTANTIATION
   └─ GameManager.Awake():
      if (GameMode.Instance == null)
          Instantiate(gameLevels[1])  // Default: Level 1
      else
          Instantiate(gameLevels[GlobalValue.levelPlaying - 1])
          └─ GlobalValue.levelPlaying = 5
             └─ Instantiates gameLevels[4] (index 4 = Level 5)

6. LEVEL SETUP INITIALIZATION
   └─ Instantiated level prefab contains GameLevelSetup component
      └─ GameLevelSetup.Awake():
         ├─ Instance = this (singleton)
         ├─ DontDestroyOnLoad(gameObject)
         └─ GlobalValue.finishGameAtLevel = levelWaves.Count

7. LEVEL ENEMY MANAGER READS CONFIG
   └─ LevelEnemyManager.Start():
      └─ EnemyWave[] waves = GameLevelSetup.Instance.GetLevelWave()
         └─ Searches levelWaves for level == 5
            └─ Returns waves for Level 5

8. GAME BEGINS
   └─ GameManager.StartGame()
      └─ Broadcasts IPlay() to all listeners
         └─ LevelEnemyManager.IPlay()
            └─ Starts spawning enemies from waves
```

### Code Walkthrough

**Step 1: GameManager Instantiates Level**

In `GameManager.cs`:

```csharp
[Header("LEVELS")]
public GameObject[] gameLevels;  // Assign level prefabs in Inspector

void Awake()
{
    // ... other setup ...

    // Spawn the correct level prefab
    if (GameMode.Instance == null)
    {
        // No level selected, use default (Level 1)
        Instantiate(gameLevels[1], Vector2.zero, Quaternion.identity);
    }
    else
    {
        // Use the level player selected
        // GlobalValue.levelPlaying = 1 → gameLevels[0]
        // GlobalValue.levelPlaying = 2 → gameLevels[1]
        // etc.
        Instantiate(
            gameLevels[GlobalValue.levelPlaying - 1],
            Vector2.zero,
            Quaternion.identity
        );
    }
}
```

**Example**:
```
GlobalValue.levelPlaying = 3

gameLevels array in Inspector:
[0] Level_1_Prefab
[1] Level_2_Prefab
[2] Level_3_Prefab  ← This one gets instantiated!
[3] Level_4_Prefab
```

**Step 2: GameLevelSetup Initializes**

The instantiated level prefab has this component:

```csharp
void Awake()
{
    Instance = this;  // Make it accessible globally
    DontDestroyOnLoad(gameObject);  // Keep it when scenes change

    // Tell the game how many levels exist
    GlobalValue.finishGameAtLevel = levelWaves.Count;
}
```

**Step 3: LevelEnemyManager Requests Wave Data**

```csharp
void Start()
{
    EnemyWave[] waves = GameLevelSetup.Instance.GetLevelWave();
    // waves now contains enemy spawn data for current level
}
```

**Step 4: GameManager Starts Game**

```csharp
public void StartGame()
{
    State = GameState.Playing;

    // Notify all listeners (including LevelEnemyManager)
    foreach (var listener in listeners)
    {
        if (listener != null)
            listener.IPlay();
    }
}
```

**Step 5: LevelEnemyManager Spawns Enemies**

```csharp
public void IPlay()
{
    isPlaying = true;
    StartCoroutine(SpawnEnemyCo());  // Begin spawning waves
}
```

---

## Creating New Levels

Follow this step-by-step guide to add a new level to your game.

### Step 1: Create Level Prefab

**Option A: Duplicate Existing Level**

1. In Project window, navigate to `Assets/_MonstersOut/Prefabs/Levels/`
2. Right-click existing level prefab (e.g., `Level_1`)
3. Click **Duplicate**
4. Rename to `Level_4` (or next number)

**Option B: Create From Scratch**

1. In Hierarchy, right-click → **Create Empty**
2. Name it `Level_4`
3. Add `GameLevelSetup` component
4. Add `LevelEnemyManager` component
5. Create child GameObject named `Level 4`
6. Add `LevelWave` component to child
7. Drag from Hierarchy to Project window to create prefab

### Step 2: Configure GameLevelSetup

Select your new level prefab and configure:

```
GameLevelSetup Component:
├─ Amount Mana: 2          (Mana added per interval)
├─ Rate: 2.0               (Add mana every 2 seconds)
└─ Level Waves: (auto-populated, see Step 3)
```

### Step 3: Configure LevelWave

The `GameLevelSetup` prefab should have child objects with `LevelWave` components.

**To add a new level config**:

1. Select `GameLevelSetup` in Hierarchy
2. Right-click → **Create Empty**
3. Add `LevelWave` component
4. Configure:

```
LevelWave Component:
├─ Level: 4                (Auto-set by OnDrawGizmos)
├─ Given Mana: 1500        (Starting mana for player)
├─ Enemy Fortrest Level: 3 (Fortress HP tier, 1-5)
└─ Waves: (see Step 4)
```

### Step 4: Configure Enemy Waves

Click the **+** button to add waves to the `Waves` array:

```
Wave 0: (First wave)
├─ Size: 2
│  ├─ Element 0:
│  │  ├─ Enemy Prefab: Goblin
│  │  ├─ Amount: 5
│  │  └─ Delay: 1.0
│  └─ Element 1:
│     ├─ Enemy Prefab: Skeleton
│     ├─ Amount: 3
│     └─ Delay: 2.0

Wave 1: (Second wave)
├─ Size: 1
│  └─ Element 0:
│     ├─ Enemy Prefab: TrollBoss
│     ├─ Amount: 1
│     └─ Delay: 0
```

**What This Means**:
```
Wave 0 starts:
  - Spawn 5 Goblins, 1 second apart
  - Then spawn 3 Skeletons, 2 seconds apart

After Wave 0 completes:
  - Wave 1 starts
  - Spawn 1 TrollBoss
```

### Step 5: Add Level to GameManager

1. Open your Game scene
2. Select `GameManager` in Hierarchy
3. In Inspector, find `Game Levels` array
4. Increase **Size** by 1
5. Drag your new level prefab into the new slot

```
GameManager:
Game Levels:
  Size: 4
  Element 0: Level_1
  Element 1: Level_2
  Element 2: Level_3
  Element 3: Level_4  ← Your new level!
```

**CRITICAL**: Array index starts at 0, but level numbers start at 1!

```
GlobalValue.levelPlaying = 1 → gameLevels[0] (Level_1)
GlobalValue.levelPlaying = 2 → gameLevels[1] (Level_2)
GlobalValue.levelPlaying = 3 → gameLevels[2] (Level_3)
GlobalValue.levelPlaying = 4 → gameLevels[3] (Level_4)
```

### Step 6: Test Your Level

**Quick Test (Skip Map UI)**:

```csharp
// In GameManager.cs Awake(), temporarily force your level:
void Awake()
{
    // TEST: Force load Level 4
    GlobalValue.levelPlaying = 4;

    Instantiate(gameLevels[GlobalValue.levelPlaying - 1], Vector2.zero, Quaternion.identity);
}
```

**Proper Test (Through Map UI)**:

1. Run the game
2. Open Map screen
3. Navigate to your new world
4. Click level button
5. Verify:
   - Correct starting mana
   - Correct enemy waves
   - Correct fortress HP

### Step 7: Configure Map UI

If you added a new **world** (not just a level), update `MapControllerUI`:

1. Select `MapControllerUI` in your Map scene
2. Configure:

```
MapControllerUI:
├─ Block Level: (assign the container RectTransform)
├─ How Many Blocks: 4  (increased from 3)
├─ Step: 720
└─ Dots: (add one more Image to array)
```

3. Add a new dot indicator Image:
   - Duplicate existing dot in Hierarchy
   - Add to `Dots` array in Inspector

---

## Advanced Techniques

### 1. Dynamic Difficulty Scaling

Adjust enemy stats based on player performance:

```csharp
public class GameLevelSetup : MonoBehaviour
{
    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        GlobalValue.finishGameAtLevel = levelWaves.Count;

        // NEW: Scale difficulty based on player win streak
        ScaleDifficulty();
    }

    void ScaleDifficulty()
    {
        int winStreak = GlobalValue.ConsecutiveWins;

        if (winStreak >= 3)
        {
            // Player is doing well, increase difficulty
            foreach (var levelWave in levelWaves)
            {
                levelWave.enemyFortrestLevel = Mathf.Min(5, levelWave.enemyFortrestLevel + 1);
                levelWave.givenMana = (int)(levelWave.givenMana * 0.9f);  // 10% less mana
            }
        }
        else if (winStreak == 0 && GlobalValue.ConsecutiveLosses >= 2)
        {
            // Player is struggling, decrease difficulty
            foreach (var levelWave in levelWaves)
            {
                levelWave.enemyFortrestLevel = Mathf.Max(1, levelWave.enemyFortrestLevel - 1);
                levelWave.givenMana = (int)(levelWave.givenMana * 1.2f);  // 20% more mana
            }
        }
    }
}
```

### 2. Random Level Selection

Create variety by randomizing which level loads:

```csharp
public class GameManager : MonoBehaviour
{
    public GameObject[] gameLevels;
    public bool randomizeLevels = false;

    void Awake()
    {
        // ... other setup ...

        if (randomizeLevels)
        {
            // Pick a random level instead of sequential
            int randomIndex = Random.Range(0, gameLevels.Length);
            Instantiate(gameLevels[randomIndex], Vector2.zero, Quaternion.identity);
        }
        else
        {
            // Normal sequential loading
            Instantiate(gameLevels[GlobalValue.levelPlaying - 1], Vector2.zero, Quaternion.identity);
        }
    }
}
```

### 3. Endless Mode

Create an endless mode that cycles through levels with increasing difficulty:

```csharp
public class EndlessModeManager : MonoBehaviour
{
    void Start()
    {
        if (GlobalValue.isEndlessMode)
        {
            StartCoroutine(EndlessModeCo());
        }
    }

    IEnumerator EndlessModeCo()
    {
        int difficultyMultiplier = 1;

        while (true)
        {
            // Wait for level to complete
            yield return new WaitUntil(() => GameManager.Instance.State == GameManager.GameState.Success);

            // Increase difficulty
            difficultyMultiplier++;

            // Cycle through levels
            int nextLevel = (GlobalValue.levelPlaying % GameLevelSetup.Instance.levelWaves.Count) + 1;
            GlobalValue.levelPlaying = nextLevel;

            // Scale enemy stats
            foreach (var enemy in FindObjectsOfType<Enemy>())
            {
                enemy.maxHealth = (int)(enemy.maxHealth * difficultyMultiplier * 1.1f);
            }

            // Wait a moment, then load next level
            yield return new WaitForSeconds(3f);
            MenuManager.Instance.LoadNextLevel();
        }
    }
}
```

### 4. Custom Loading Screen Animation

Enhance the loading screen with animated elements:

```csharp
public class CustomLoadingScreen : MonoBehaviour
{
    public Image loadingIcon;  // Spinning icon
    public Text tipText;       // Gameplay tips
    public string[] tips;      // Array of tips

    IEnumerator LoadAsynchronously(string name)
    {
        LoadingUI.SetActive(true);

        // Show random tip
        tipText.text = tips[Random.Range(0, tips.Length)];

        AsyncOperation operation = SceneManager.LoadSceneAsync(name);

        while (!operation.isDone)
        {
            // Rotate loading icon
            loadingIcon.transform.Rotate(Vector3.forward * -360 * Time.deltaTime);

            // Update progress
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            slider.value = progress;
            progressText.text = (int)(progress * 100f) + "%";

            // Change tip every 3 seconds
            if (Time.frameCount % 180 == 0)
            {
                tipText.text = tips[Random.range(0, tips.Length)];
            }

            yield return null;
        }
    }
}
```

### 5. Map UI Smooth Scrolling

Replace instant snap with smooth animated scrolling:

```csharp
public class MapControllerUI : MonoBehaviour
{
    public float scrollSpeed = 5f;  // Speed of animation
    private float targetPosX;       // Target position

    IEnumerator NextCo()
    {
        allowPressButton = false;
        SoundManager.Click();

        if (newPosX != (-step * (howManyBlocks - 1)))
        {
            currentPos++;
            targetPosX = newPosX - step;  // Set target
            targetPosX = Mathf.Clamp(targetPosX, -step * (howManyBlocks - 1), 0);
        }
        else
        {
            allowPressButton = true;
            yield break;
        }

        // Smooth scroll instead of instant snap
        BlackScreenUI.instance.Show(0.15f);
        yield return new WaitForSeconds(0.15f);

        // Animate from current position to target
        float elapsed = 0f;
        float duration = 0.5f;
        float startPosX = newPosX;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            newPosX = Mathf.Lerp(startPosX, targetPosX, elapsed / duration);
            SetMapPosition();
            yield return null;
        }

        newPosX = targetPosX;  // Ensure exact position
        SetMapPosition();

        BlackScreenUI.instance.Hide(0.15f);
        SetDots();
        allowPressButton = true;
    }
}
```

---

## Troubleshooting

### Problem 1: Level Doesn't Load (Black Screen)

**Symptoms**:
- Click level button
- Loading screen shows
- Progress bar reaches 100%
- Game stays on black screen

**Causes & Solutions**:

**Cause A: Scene name mismatch**
```csharp
// Check MenuManager.cs:
StartCoroutine(LoadAsynchronously("Game"));  // ← Must match scene name exactly!

// Verify scene name:
// Unity → File → Build Settings → Scenes In Build
// Look for "Game" scene
```

**Solution**: Ensure scene name in `LoadAsynchronously` matches Build Settings.

**Cause B: Scene not in Build Settings**
```
File → Build Settings
└─ If "Game" scene is missing, click "Add Open Scenes"
```

**Cause C: GameManager missing from scene**
```
The Game scene must have a GameManager object!
If it's missing:
1. Open Game scene
2. Create empty GameObject
3. Add GameManager component
4. Configure gameLevels array
```

---

### Problem 2: Wrong Level Loads

**Symptoms**:
- Click "Level 3" button
- Level 1 loads instead

**Cause**: `GlobalValue.levelPlaying` not set correctly

**Solution**:
```csharp
// In your level button script:
public void OnLevelButtonClick()
{
    GlobalValue.levelPlaying = 3;  // Set BEFORE loading scene
    MenuManager.Instance.LoadAsynchronously("Game");
}
```

**Debug Check**:
```csharp
// In GameManager.Awake(), add debug log:
void Awake()
{
    Debug.Log("Loading level: " + GlobalValue.levelPlaying);
    Instantiate(gameLevels[GlobalValue.levelPlaying - 1], Vector2.zero, Quaternion.identity);
}
```

---

### Problem 3: IndexOutOfRangeException When Loading Level

**Error Message**:
```
IndexOutOfRangeException: Index was outside the bounds of the array.
GameManager.Awake() (at Assets/.../GameManager.cs:56)
```

**Cause**: `gameLevels` array doesn't have enough elements

**Example**:
```csharp
GlobalValue.levelPlaying = 5

gameLevels array:
  Size: 3
  [0] Level_1
  [1] Level_2
  [2] Level_3

// Trying to access gameLevels[4] → CRASH!
```

**Solution 1: Add missing level prefabs**
```
1. Select GameManager in Hierarchy
2. In Inspector, increase "Game Levels" Size to 5
3. Assign Level_4 and Level_5 prefabs
```

**Solution 2: Clamp level number**
```csharp
void Awake()
{
    // Prevent crash by clamping to valid range
    int levelIndex = Mathf.Clamp(GlobalValue.levelPlaying - 1, 0, gameLevels.Length - 1);
    Instantiate(gameLevels[levelIndex], Vector2.zero, Quaternion.identity);
}
```

---

### Problem 4: Map Navigation Buttons Don't Work

**Symptoms**:
- Click "Next" button
- Nothing happens
- No error in Console

**Debugging Steps**:

**Step 1: Check button is connected**
```
1. Select "Next" button in Hierarchy
2. Check Inspector → Button component → On Click()
3. Verify:
   - Target: MapControllerUI
   - Function: MapControllerUI.Next
```

**Step 2: Check allowPressButton**
```csharp
// Add debug log in Next():
public void Next()
{
    Debug.Log("Next button clicked. allowPressButton = " + allowPressButton);

    if (allowPressButton)
    {
        StartCoroutine(NextCo());
    }
}
```

**Step 3: Check if already at last world**
```csharp
// In NextCo(), add logs:
IEnumerator NextCo()
{
    Debug.Log("newPosX = " + newPosX);
    Debug.Log("Max position = " + (-step * (howManyBlocks - 1)));

    if (newPosX != (-step * (howManyBlocks - 1)))
    {
        // Can move
    }
    else
    {
        Debug.Log("Already at last world!");
        allowPressButton = true;
        yield break;
    }
}
```

---

### Problem 5: No Enemy Waves Spawn

**Symptoms**:
- Level loads successfully
- Player fortress appears
- No enemies spawn

**Debugging Steps**:

**Step 1: Check GameLevelSetup.Instance**
```csharp
// In LevelEnemyManager.Start():
void Start()
{
    if (GameLevelSetup.Instance == null)
    {
        Debug.LogError("GameLevelSetup.Instance is NULL!");
        return;
    }

    EnemyWave[] waves = GameLevelSetup.Instance.GetLevelWave();

    if (waves == null)
    {
        Debug.LogError("GetLevelWave() returned NULL!");
        return;
    }

    Debug.Log("Found " + waves.Length + " waves for level " + GlobalValue.levelPlaying);
}
```

**Step 2: Check level prefab has GameLevelSetup**
```
1. Select your level prefab in Project window
2. Verify it has GameLevelSetup component
3. Check that levelWaves list is populated
```

**Step 3: Check level number matches**
```csharp
// In GameLevelSetup.GetLevelWave(), add logs:
public EnemyWave[] GetLevelWave()
{
    Debug.Log("Looking for level: " + GlobalValue.levelPlaying);

    foreach (var obj in levelWaves)
    {
        Debug.Log("Checking LevelWave with level = " + obj.level);

        if (obj.level == GlobalValue.levelPlaying)
        {
            Debug.Log("Found matching level! Returning " + obj.Waves.Length + " waves");
            return obj.Waves;
        }
    }

    Debug.LogError("No matching level found!");
    return null;
}
```

**Common Fix**: Level number mismatch
```
GlobalValue.levelPlaying = 3

But levelWaves list only has:
- Level 1
- Level 2

Solution: Add Level 3 configuration to GameLevelSetup!
```

---

### Problem 6: Progress Bar Stuck at 90%

**Symptoms**:
- Loading screen shows
- Progress bar reaches 90%
- Never completes

**Cause**: `operation.progress` naturally stops at 0.9

**This is NORMAL Unity behavior!** The scene is still loading in the background.

**Solution**: Add timeout or wait for `isDone`:
```csharp
IEnumerator LoadAsynchronously(string name)
{
    LoadingUI.SetActive(true);
    AsyncOperation operation = SceneManager.LoadSceneAsync(name);

    float timeout = 10f;  // Max 10 seconds
    float elapsed = 0f;

    while (!operation.isDone && elapsed < timeout)
    {
        elapsed += Time.deltaTime;

        float progress = Mathf.Clamp01(operation.progress / 0.9f);
        slider.value = progress;
        progressText.text = (int)(progress * 100f) + "%";

        yield return null;
    }

    if (elapsed >= timeout)
    {
        Debug.LogError("Scene load timeout! Check for missing assets.");
    }
}
```

---

## Summary

**Key Takeaways**:

1. **MapControllerUI**: Handles horizontal scrolling UI navigation with Previous/Next buttons and dot indicators
2. **GameLevelSetup**: Stores level configurations (waves, mana, fortress HP) and provides data to managers
3. **MenuManager**: Loads scenes asynchronously with progress bar, preventing game freeze
4. **Level Flow**: Button click → Set GlobalValue.levelPlaying → LoadAsynchronously → GameManager instantiates level prefab → GameLevelSetup provides config → LevelEnemyManager spawns waves
5. **Creating Levels**: Duplicate prefab → Configure LevelWave → Add to GameManager.gameLevels array → Test

**Essential Code Patterns**:

```csharp
// 1. Load scene asynchronously
AsyncOperation operation = SceneManager.LoadSceneAsync("Game");
while (!operation.isDone)
{
    float progress = Mathf.Clamp01(operation.progress / 0.9f);
    yield return null;
}

// 2. Instantiate level prefab
Instantiate(gameLevels[GlobalValue.levelPlaying - 1], Vector2.zero, Quaternion.identity);

// 3. Get level configuration
EnemyWave[] waves = GameLevelSetup.Instance.GetLevelWave();

// 4. Navigate map UI
newPosX -= step;  // Move left one block
newPosX = Mathf.Clamp(newPosX, -step * (howManyBlocks - 1), 0);
BlockLevel.anchoredPosition = new Vector2(newPosX, BlockLevel.anchoredPosition.y);
```

**Next Steps**:
- See `Enemy-Deep.md` for configuring enemy waves
- See `05_Managers_Complete.md` for GameManager details
- See `10_How_To_Guides.md` for step-by-step tutorials
- See `11_Troubleshooting.md` for general debugging strategies
