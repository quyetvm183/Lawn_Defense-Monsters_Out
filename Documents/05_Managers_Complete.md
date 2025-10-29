# Managers System - Complete Guide

> **For**: Beginners who finished Unity Fundamentals
> **Read Time**: 35-45 minutes
> **Prerequisites**: 00_Unity_Fundamentals.md, 01_Project_Architecture.md

---

## Table of Contents
1. [System Overview](#system-overview)
2. [GameManager - Core Controller](#gamemanager---core-controller)
3. [Observer Pattern (IListener)](#observer-pattern-ilistener)
4. [LevelEnemyManager - Wave System](#levelenemymanager---wave-system)
5. [LevelManager - Resource Management](#levelmanager---resource-management)
6. [SoundManager - Audio System](#soundmanager---audio-system)
7. [Manager Communication Flow](#manager-communication-flow)
8. [How to Create Custom Manager](#how-to-create-custom-manager)
9. [Common Issues & Solutions](#common-issues--solutions)

---

## System Overview

### What are Managers?

**Managers** are **singleton** classes that control global game systems. They:
- **Coordinate** between different systems (Player, Enemy, UI)
- **Maintain** game state (Playing, Paused, Victory, GameOver)
- **Broadcast** events to listeners
- **Persist** across scenes (some managers)

### Why Managers?

Without managers, every script would need references to every other script:

```
❌ WITHOUT MANAGERS:
Enemy → needs Player reference
Enemy → needs UI reference
Enemy → needs SoundManager reference
Enemy → needs LevelManager reference
(messy, tightly coupled)

✓ WITH MANAGERS:
Enemy → calls GameManager.Victory()
GameManager → broadcasts to all listeners
(clean, loosely coupled)
```

### Manager Architecture Diagram

```
┌─────────────────────────────────────────────┐
│            GAME MANAGER                     │
│       (Central State Controller)            │
└────────────┬────────────────────────────────┘
             │
       Broadcasts Events
             │
    ┌────────┼────────┬────────┬────────┐
    │        │        │        │        │
    ▼        ▼        ▼        ▼        ▼
┌────────┐ ┌──────┐ ┌──────┐ ┌─────┐ ┌──────┐
│ Level  │ │Menu  │ │Enemy │ │Sound│ │Level │
│ Enemy  │ │ Mgr  │ │  AI  │ │ Mgr │ │ Mgr  │
│Manager │ │      │ │      │ │     │ │      │
└────────┘ └──────┘ └──────┘ └─────┘ └──────┘
```

### Key Files

| File | Location | Purpose |
|------|----------|---------|
| `GameManager.cs` | `Assets/_MonstersOut/Scripts/Managers/` | Core game state & events |
| `LevelEnemyManager.cs` | `Assets/_MonstersOut/Scripts/Managers/` | Enemy wave spawning |
| `LevelManager.cs` | `Assets/_MonstersOut/Scripts/Managers/` | Mana & resources |
| `SoundManager.cs` | `Assets/_MonstersOut/Scripts/Managers/` | Music & sound effects |

---

## GameManager - Core Controller

### GameManager Overview

**File**: `GameManager.cs`

**Purpose**: Central game state controller

**Responsibilities**:
- Manage game state (Menu, Playing, Pause, Success, GameOver)
- Broadcast events to all listeners
- Track alive enemies
- Load level prefabs
- Handle Victory/GameOver logic

### Singleton Pattern

Located in `GameManager.cs:14` and `:44-48`

```csharp
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        Application.targetFrameRate = 60;  // Lock to 60 FPS
        Instance = this;                    // Set singleton

        State = GameState.Menu;             // Initial state
        listeners = new List<IListener>();  // Init listener list

        // Load level prefab based on progress
        if (GameMode.Instance == null)
            Instantiate(gameLevels[1], Vector2.zero, Quaternion.identity);
        else
            Instantiate(gameLevels[GlobalValue.levelPlaying - 1], Vector2.zero, Quaternion.identity);
    }
}
```

**Why Singleton?**

```csharp
// Any script can access GameManager
GameManager.Instance.Victory();
GameManager.Instance.State;
GameManager.Instance.AddListener(this);

// No FindObjectOfType needed
// No need to pass references
```

### Game States (Enum)

Located in `GameManager.cs:18`

```csharp
public enum GameState
{
    Menu,      // Title screen, not playing yet
    Playing,   // Game actively running
    GameOver,  // Player lost (fortress destroyed)
    Success,   // Player won (all enemies dead)
    Pause      // Game paused by player
}

public GameState State { get; set; }
```

### State Flow Diagram

```
[MENU]
   │
   │ MenuManager calls StartGame()
   │
   ▼
[PLAYING]
   │
   ├──────────┬──────────┐
   │          │          │
   │          │          ▼
   │          │      [PAUSE]
   │          │          │
   │          │ UnPause()│
   │          │          │
   │          │          ▼
   │          │      [PLAYING]
   │          │
   │    Fortress   All enemies
   │      HP=0        dead
   │          │          │
   ▼          ▼          ▼
[GAMEOVER] [SUCCESS]
```

### StartGame() Method

Located in `GameManager.cs:68-83`

```csharp
//called by MenuManager
public void StartGame()
{
    // Change state to Playing
    State = GameState.Playing;

    // Find all objects with IListener interface
    var listener_ = FindObjectsOfType<MonoBehaviour>().OfType<IListener>();

    // Add all listeners to list
    foreach (var _listener in listener_)
    {
        listeners.Add(_listener);
    }

    // Broadcast IPlay() to all listeners
    foreach (var item in listeners)
    {
        item.IPlay();
    }
}
```

**How it Works**:

```
Frame 1: MenuManager.Start() finishes countdown
         └─ GameManager.Instance.StartGame() called

Frame 2: GameManager.StartGame() executes
         ├─ State = Playing
         ├─ Find all IListeners in scene:
         │   ├─ MenuManager (implements IListener)
         │   ├─ LevelEnemyManager (implements IListener)
         │   ├─ All Enemy GameObjects (implement IListener)
         │   └─ Player_Archer (implements IListener)
         │
         └─ Call IPlay() on each listener:
             ├─ MenuManager.IPlay() → (empty)
             ├─ LevelEnemyManager.IPlay() → StartCoroutine(SpawnEnemyCo())
             ├─ Enemy.IPlay() → isPlaying = true
             └─ Player_Archer.IPlay() → (empty, inherited from Enemy)

Frame 3: Game starts
         └─ Enemies begin spawning
```

**FindObjectsOfType() Explanation**:

```csharp
// Find ALL MonoBehaviours
var allScripts = FindObjectsOfType<MonoBehaviour>();

// Filter only those implementing IListener
var listeners = allScripts.OfType<IListener>();

// Equivalent to:
List<IListener> listeners = new List<IListener>();
foreach (MonoBehaviour script in FindObjectsOfType<MonoBehaviour>())
{
    if (script is IListener)
        listeners.Add(script as IListener);
}
```

### Victory() Method

Located in `GameManager.cs:101-126`

```csharp
public void Victory()
{
    // Prevent double-trigger
    if (State == GameState.Success)
        return;

    Time.timeScale = 1;  // Reset time scale (in case paused)

    // Pause music
    SoundManager.Instance.PauseMusic(true);

    // Play victory sound
    SoundManager.PlaySfx(SoundManager.Instance.soundVictory, 0.6f);

    // Change state to Success
    State = GameState.Success;

    // Show ads (if ads manager exists)
    if (AdsManager.Instance)
    {
        AdsManager.Instance.ShowAdmobBanner(true);
        AdsManager.Instance.ShowNormalAd(State);
    }

    // Broadcast ISuccess() to all listeners
    foreach (var item in listeners)
    {
        if (item != null)
            item.ISuccess();
    }

    // Save level progress (unlock next level)
    if (GlobalValue.levelPlaying > GlobalValue.LevelPass)
        GlobalValue.LevelPass = GlobalValue.levelPlaying;
}
```

**When is Victory() Called?**

```csharp
// In LevelEnemyManager.SpawnEnemyCo()
while (isEnemyAlive()) { yield return new WaitForSeconds(0.1f); }
// After loop exits (all enemies dead)
GameManager.Instance.Victory();
```

**Victory Flow**:

```
Frame 500:  Last enemy dies
            └─ LevelEnemyManager checks isEnemyAlive()

Frame 501:  isEnemyAlive() returns false
            └─ GameManager.Victory() called

Frame 502:  Victory() executes
            ├─ State = Success
            ├─ Pause music
            ├─ Play victory sound
            └─ Broadcast ISuccess():
                ├─ MenuManager.ISuccess() → Show victory UI
                ├─ LevelEnemyManager.ISuccess() → StopAllCoroutines
                └─ All enemies → (removed from listeners, already dead)

Frame 503:  Victory UI appears
            └─ Star animation begins
```

### GameOver() Method

Located in `GameManager.cs:134-152`

```csharp
public void GameOver()
{
    Time.timeScale = 1;  // Reset time scale

    // Stop music
    SoundManager.Instance.PauseMusic(true);

    // Prevent double-trigger
    if (State == GameState.GameOver)
        return;

    // Set GameOver state
    State = GameState.GameOver;

    // Show ads
    if (AdsManager.Instance)
    {
        AdsManager.Instance.ShowAdmobBanner(true);
        AdsManager.Instance.ShowNormalAd(State);
    }

    // Broadcast IGameOver() to all listeners
    foreach (var item in listeners)
        item.IGameOver();
}
```

**When is GameOver() Called?**

```csharp
// In TheFortrest.cs (fortress health system)
public override void TakeDamage(...)
{
    currentHealth -= (int)damage;

    if (currentHealth <= 0)
    {
        GameManager.Instance.GameOver();
        Die();
    }
}
```

### Pause/Resume Methods

Located in `GameManager.cs:85-99`

```csharp
public void Gamepause()
{
    // Set Pause state
    State = GameState.Pause;

    // Broadcast IPause() to all listeners
    foreach (var item in listeners)
        item.IPause();
}

public void UnPause()
{
    // Set back to Playing state
    State = GameState.Playing;

    // Broadcast IUnPause() to all listeners
    foreach (var item in listeners)
        item.IUnPause();
}
```

**How Pause Works**:

```
User clicks Pause button
    │
    ▼
MenuManager.Pause() called
    ├─ Time.timeScale = 0 (freeze physics)
    ├─ GameManager.Instance.Gamepause()
    │   └─ Broadcast IPause() to listeners
    └─ Show pause UI

During Pause:
    - Update() still runs
    - FixedUpdate() does NOT run
    - Animations stop
    - Enemy movement stops
    - Player can't shoot

User clicks Resume button
    │
    ▼
MenuManager.Pause() called again
    ├─ Time.timeScale = 1 (resume physics)
    ├─ GameManager.Instance.UnPause()
    │   └─ Broadcast IUnPause() to listeners
    └─ Hide pause UI
```

### Enemy Tracking

Located in `GameManager.cs:154-175`

```csharp
[HideInInspector]
public List<GameObject> enemyAlives;
[HideInInspector]
public List<GameObject> listEnemyChasingPlayer;

public void RigisterEnemy(GameObject obj)
{
    // Add enemy to alive list
    enemyAlives.Add(obj);
}

public void RemoveEnemy(GameObject obj)
{
    // Remove enemy from alive list
    enemyAlives.Remove(obj);
}

public int EnemyAlive()
{
    // Return count of alive enemies
    return enemyAlives.Count;
}
```

**Note**: This code exists but **is NOT actually used** in the current project. Instead, `LevelEnemyManager` tracks enemies via `listEnemySpawned`.

---

## Observer Pattern (IListener)

### What is the Observer Pattern?

**Observer Pattern** allows objects to **subscribe** to events and get **notified** when events happen.

**Problem Without Observer**:
```csharp
// ❌ Tightly coupled
public class GameManager
{
    public MenuManager menuManager;
    public Enemy[] enemies;

    void Victory()
    {
        menuManager.ShowVictoryScreen();
        foreach (var enemy in enemies)
            enemy.Stop();
    }
}
```

**Solution With Observer**:
```csharp
// ✓ Loosely coupled
public class GameManager
{
    List<IListener> listeners;

    void Victory()
    {
        foreach (var listener in listeners)
            listener.ISuccess();
    }
}
```

### IListener Interface

```csharp
public interface IListener
{
    void IPlay();         // Game started
    void ISuccess();      // Level won
    void IGameOver();     // Level lost
    void IPause();        // Game paused
    void IUnPause();      // Game resumed
    void IOnRespawn();    // Player respawned (not used)
    void IOnStopMovingOn();  // Movement locked (not used)
    void IOnStopMovingOff(); // Movement unlocked (not used)
}
```

### AddListener() / RemoveListener()

Located in `GameManager.cs:27-39`

```csharp
public void AddListener(IListener _listener)
{
    // Check if not already added
    if (!listeners.Contains(_listener))
        listeners.Add(_listener);
}

public void RemoveListener(IListener _listener)
{
    // Check if exists, then remove
    if (listeners.Contains(_listener))
        listeners.Remove(_listener);
}
```

**When to Add/Remove**:

```csharp
// In Enemy.cs
protected virtual void OnEnable()
{
    if (GameManager.Instance)
        GameManager.Instance.AddListener(this);
    isPlaying = true;
}

public virtual void Die()
{
    isPlaying = false;
    GameManager.Instance.RemoveListener(this);  // Stop receiving events
    // ... death logic
}
```

**Why Remove on Death?**
- Dead enemies shouldn't receive events
- Prevents null reference errors
- Improves performance (fewer listeners to iterate)

### IListener Implementation Example

**Enemy.cs** (implements IListener):

```csharp
public class Enemy : MonoBehaviour, ICanTakeDamage, IListener
{
    #region IListener implementation

    public virtual void IPlay()
    {
        // Game started - do nothing (already handled in Start)
    }

    public virtual void ISuccess()
    {
        // Level won - do nothing
    }

    public virtual void IPause()
    {
        // Game paused - do nothing
    }

    public virtual void IUnPause()
    {
        // Game resumed - do nothing
    }

    public virtual void IGameOver()
    {
        if (!isPlaying)
            return;

        isPlaying = false;           // Stop enemy actions
        SetEnemyState(ENEMYSTATE.IDLE);  // Freeze in place
    }

    // ... other IListener methods
    #endregion
}
```

**MenuManager.cs** (implements IListener):

```csharp
public class MenuManager : MonoBehaviour, IListener
{
    public void ISuccess()
    {
        StartCoroutine(VictoryCo());  // Show victory UI
    }

    IEnumerator VictoryCo()
    {
        UI.SetActive(false);
        CharacterContainer.SetActive(false);
        yield return new WaitForSeconds(1.5f);
        VictotyUI.SetActive(true);
    }

    public void IGameOver()
    {
        StartCoroutine(GameOverCo());  // Show game over UI
    }

    // ... other IListener methods
}
```

### Event Broadcast Flow

```
GameManager.Victory() called
    │
    ├─ foreach (var item in listeners)
    │   │
    │   ├─ MenuManager.ISuccess()
    │   │   └─ Show victory UI
    │   │
    │   ├─ LevelEnemyManager.ISuccess()
    │   │   └─ StopAllCoroutines()
    │   │
    │   └─ Enemy instances.ISuccess()
    │       └─ (empty, do nothing)
    │
    └─ All listeners notified simultaneously
```

---

## LevelEnemyManager - Wave System

### LevelEnemyManager Overview

**File**: `LevelEnemyManager.cs`

**Purpose**: Spawn enemies in waves

**Responsibilities**:
- Load enemy wave configuration
- Spawn enemies over time
- Track spawned enemies
- Detect when all enemies dead → trigger Victory

### Enemy Wave Data Structure

Located in `LevelEnemyManager.cs:137-144`

```csharp
[System.Serializable]
public class EnemyWave
{
    public float wait = 3;  // Delay before this wave starts
    public EnemySpawn[] enemySpawns;  // List of enemy groups
}

// Note: EnemySpawn is defined elsewhere
public class EnemySpawn
{
    public GameObject enemy;     // Enemy prefab
    public int numberEnemy;      // How many to spawn
    public float wait;           // Delay before this group
    public float rate;           // Delay between each enemy
}
```

**Wave Configuration Example**:

```
Wave 1:
  wait: 2 seconds
  EnemySpawns:
    - Goblin x5 (rate: 0.5s)
    - Skeleton x3 (rate: 1s)

Wave 2:
  wait: 10 seconds
  EnemySpawns:
    - Troll x2 (rate: 2s)
    - Bomber x4 (rate: 1.5s)
```

### Start() Method (Count Total Enemies)

Located in `LevelEnemyManager.cs:25-47`

```csharp
void Start()
{
    // Load wave config from level prefab
    if (GameLevelSetup.Instance)
        EnemyWaves = GameLevelSetup.Instance.GetLevelWave();

    // Count total enemies in level
    totalEnemy = 0;

    // Loop through each wave
    for (int i = 0; i < EnemyWaves.Length; i++)
    {
        // Loop through each enemy spawn group
        for (int j = 0; j < EnemyWaves[i].enemySpawns.Length; j++)
        {
            var enemySpawn = EnemyWaves[i].enemySpawns[j];

            // Count each individual enemy
            for (int k = 0; k < enemySpawn.numberEnemy; k++)
            {
                totalEnemy++;
            }
        }
    }

    currentSpawn = 0;
}
```

**Example Calculation**:

```
Wave 1:
  Goblin x5 = 5 enemies
  Skeleton x3 = 3 enemies

Wave 2:
  Troll x2 = 2 enemies
  Bomber x4 = 4 enemies

totalEnemy = 5 + 3 + 2 + 4 = 14
```

### SpawnEnemyCo() Coroutine

Located in `LevelEnemyManager.cs:49-87`

```csharp
IEnumerator SpawnEnemyCo()
{
    // Loop through each wave
    for (int i = 0; i < EnemyWaves.Length; i++)
    {
        // Wait before wave starts
        yield return new WaitForSeconds(EnemyWaves[i].wait);

        // Loop through each enemy spawn group in wave
        for (int j = 0; j < EnemyWaves[i].enemySpawns.Length; j++)
        {
            var enemySpawn = EnemyWaves[i].enemySpawns[j];

            // Wait before group starts
            yield return new WaitForSeconds(enemySpawn.wait);

            // Spawn each enemy in group
            for (int k = 0; k < enemySpawn.numberEnemy; k++)
            {
                // Random Y position within spawn zone
                spawnPosition = transform.position
                                + Vector3.up * Random.Range(-spawnHeightZone, spawnHeightZone);

                // Instantiate enemy
                GameObject _temp = Instantiate(
                    enemySpawn.enemy,
                    spawnPosition + Vector2.up * 0.1f,
                    Quaternion.identity
                ) as GameObject;

                _temp.SetActive(false);           // Disable first
                _temp.transform.parent = transform;  // Set parent

                // Wait 0.1s before activating
                yield return new WaitForSeconds(0.1f);

                _temp.SetActive(true);  // Activate enemy

                // Add to spawned list
                listEnemySpawned.Add(_temp);

                // Increment spawn counter
                currentSpawn++;

                // Update wave progress bar
                MenuManager.Instance.UpdateEnemyWavePercent(currentSpawn, totalEnemy);

                // Wait before next enemy
                yield return new WaitForSeconds(enemySpawn.rate);
            }
        }
    }

    // All enemies spawned, wait until all dead
    while (isEnemyAlive()) { yield return new WaitForSeconds(0.1f); }
}
```

**Timeline Example**:

```
Frame 1:    IPlay() called
            └─ StartCoroutine(SpawnEnemyCo())

Frame 2:    Wait Wave 1 delay (2 seconds)
Frame 120:  Wave 1 starts

Frame 120:  Wait Goblin group delay (0 seconds)
            └─ Spawn Goblin #1

Frame 150:  Wait rate (0.5 seconds)
            └─ Spawn Goblin #2

Frame 180:  Wait rate (0.5 seconds)
            └─ Spawn Goblin #3

... (continue spawning)

Frame 500:  All enemies spawned
            └─ while (isEnemyAlive()) loop begins

Frame 1000: Last enemy dies
            └─ isEnemyAlive() returns false

Frame 1001: SpawnEnemyCo() exits
            └─ (No Victory call here - handled elsewhere)
```

**Why SetActive(false) then SetActive(true)?**

```csharp
GameObject _temp = Instantiate(...);
_temp.SetActive(false);   // Disable
yield return new WaitForSeconds(0.1f);
_temp.SetActive(true);    // Enable

// Reason:
// 1. OnEnable() calls AddListener() - we want slight delay
// 2. Prevents enemy acting before fully positioned
// 3. Gives time for all components to initialize
```

### isEnemyAlive() Method

Located in `LevelEnemyManager.cs:90-100`

```csharp
bool isEnemyAlive()
{
    // Check all spawned enemies
    for (int i = 0; i < listEnemySpawned.Count; i++)
    {
        // If any enemy is active, return true
        if (listEnemySpawned[i].activeInHierarchy)
            return true;
    }

    // No active enemies, return false
    return false;
}
```

**How it Works**:

```
listEnemySpawned = [Goblin1, Goblin2, Goblin3]

Goblin1.activeInHierarchy = true
Goblin2.activeInHierarchy = true
Goblin3.activeInHierarchy = true
isEnemyAlive() = true ✓

Goblin1 dies → SetActive(false)
Goblin1.activeInHierarchy = false
Goblin2.activeInHierarchy = true
Goblin3.activeInHierarchy = true
isEnemyAlive() = true ✓

Goblin2 dies → SetActive(false)
Goblin3 dies → SetActive(false)
All .activeInHierarchy = false
isEnemyAlive() = false → Victory!
```

### IPlay() Method

Located in `LevelEnemyManager.cs:122-125`

```csharp
public void IPlay()
{
    StartCoroutine(SpawnEnemyCo());
}
```

**How it's Called**:

```
GameManager.StartGame()
    └─ foreach (var item in listeners)
        item.IPlay()
            └─ LevelEnemyManager.IPlay()
                └─ StartCoroutine(SpawnEnemyCo())
```

### ISuccess() Method

Located in `LevelEnemyManager.cs:127-130`

```csharp
public void ISuccess()
{
    StopAllCoroutines();  // Stop spawning enemies
}
```

**Why Stop Coroutines?**
- Player won, no need to spawn more enemies
- Prevents enemies spawning during victory screen
- Clean up

---

## LevelManager - Resource Management

### LevelManager Overview

**File**: `LevelManager.cs`

**Purpose**: Manage level resources (primarily mana)

**Responsibilities**:
- Initialize mana from level config
- Provide global access to mana value

### Awake() Method

Located in `LevelManager.cs:10-19`

```csharp
private void Awake()
{
    Instance = this;  // Singleton

    // Load mana from level configuration
    if (GameLevelSetup.Instance)
    {
        mana = GameLevelSetup.Instance.GetGivenMana();
    }
}
```

**How Mana is Used**:

```csharp
// In UI_UI.Update()
manaTxt.text = LevelManager.Instance.mana + "";

// Hypothetical usage (not in current code):
public void SpawnUnit(int manaCost)
{
    if (LevelManager.Instance.mana >= manaCost)
    {
        LevelManager.Instance.mana -= manaCost;
        // Spawn unit
    }
}
```

**Note**: Current implementation is very simple. Mana system appears incomplete - there's no code that actually **uses** or **reduces** mana.

---

## SoundManager - Audio System

### SoundManager Overview

**File**: `SoundManager.cs`

**Purpose**: Centralized audio management

**Responsibilities**:
- Play music (background loop)
- Play sound effects (one-shot)
- Control volume
- Mute/unmute

### Singleton + Audio Sources

Located in `SoundManager.cs:64-72`

```csharp
void Awake()
{
    Instance = this;

    // Create AudioSource for music
    musicAudio = gameObject.AddComponent<AudioSource>();
    musicAudio.loop = true;      // Music loops forever
    musicAudio.volume = 0.5f;    // Default 50% volume

    // Create AudioSource for sound effects
    soundFx = gameObject.AddComponent<AudioSource>();
}
```

**Why Two AudioSources?**

```
musicAudio:
- Plays one clip at a time
- Loops continuously
- Can be paused/resumed
- Volume controlled separately

soundFx:
- Plays multiple clips simultaneously (PlayOneShot)
- Does NOT loop
- Used for short effects
- Volume controlled separately
```

### PlaySfx() Methods

Located in `SoundManager.cs:88-112`

```csharp
// Play single sound effect
public static void PlaySfx(AudioClip clip)
{
    if (Instance != null)
    {
        Instance.PlaySound(clip, Instance.soundFx);
    }
}

// Play sound with custom volume
public static void PlaySfx(AudioClip clip, float volume)
{
    if (Instance != null)
        Instance.PlaySound(clip, Instance.soundFx, volume);
}

// Play random sound from array
public static void PlaySfx(AudioClip[] clips)
{
    if (Instance != null && clips.Length > 0)
        Instance.PlaySound(clips[Random.Range(0, clips.Length)], Instance.soundFx);
}

// Play random sound with custom volume
public static void PlaySfx(AudioClip[] clips, float volume)
{
    if (Instance != null && clips.Length > 0)
        Instance.PlaySound(clips[Random.Range(0, clips.Length)], Instance.soundFx, volume);
}
```

**Usage Examples**:

```csharp
// Single sound
SoundManager.PlaySfx(soundVictory);

// Sound with volume
SoundManager.PlaySfx(soundClick, 0.8f);

// Random sound from array
AudioClip[] hurtSounds = {hurt1, hurt2, hurt3};
SoundManager.PlaySfx(hurtSounds);  // Plays random hurt sound
```

### PlayMusic() Methods

Located in `SoundManager.cs:114-122`

```csharp
public static void PlayMusic(AudioClip clip)
{
    Instance.PlaySound(clip, Instance.musicAudio);
}

public static void PlayMusic(AudioClip clip, float volume)
{
    Instance.PlaySound(clip, Instance.musicAudio, volume);
}
```

**Usage**:

```csharp
// In MainMenuHomeScene.Start()
SoundManager.PlayMusic(SoundManager.Instance.musicsGame);
```

### PlaySound() Private Methods

Located in `SoundManager.cs:124-161`

```csharp
private void PlaySound(AudioClip clip, AudioSource audioOut)
{
    if (clip == null)
        return;

    if (Instance == null)
        return;

    // If music audio
    if (audioOut == musicAudio)
    {
        audioOut.clip = clip;  // Set clip
        audioOut.Play();       // Play from start
    }
    else  // Sound effect
        audioOut.PlayOneShot(clip, SoundVolume);
}

private void PlaySound(AudioClip clip, AudioSource audioOut, float volume)
{
    if (clip == null)
        return;

    // If music audio
    if (audioOut == musicAudio)
    {
        // Check GlobalValue setting
        audioOut.volume = GlobalValue.isMusic ? volume : 0;
        audioOut.clip = clip;
        audioOut.Play();
    }
    else  // Sound effect
    {
        // Check GlobalValue setting
        if (!GlobalValue.isSound) return;
        audioOut.PlayOneShot(clip, SoundVolume * volume);
    }
}
```

**Play() vs PlayOneShot()**:

```
Play():
- Stops current clip and plays new one
- Only one clip at a time
- Used for music

PlayOneShot(clip):
- Plays clip without stopping others
- Multiple clips can play simultaneously
- Used for sound effects

Example:
musicAudio.Play(music1);
musicAudio.Play(music2);  // Stops music1, plays music2

soundFx.PlayOneShot(gunshot);
soundFx.PlayOneShot(explosion);  // Both play together
```

### Volume Properties

Located in `SoundManager.cs:51-62`

```csharp
public static float MusicVolume
{
    set { Instance.musicAudio.volume = value; }
    get { return Instance.musicAudio.volume; }
}

public static float SoundVolume
{
    set { Instance.soundFx.volume = value; }
    get { return Instance.soundFx.volume; }
}
```

**Usage**:

```csharp
// Set music volume
SoundManager.MusicVolume = 0.7f;  // 70%

// Mute sound effects
SoundManager.SoundVolume = 0;

// Get current volume
float currentVolume = SoundManager.MusicVolume;
```

### PauseMusic() Method

Located in `SoundManager.cs:43-49`

```csharp
public void PauseMusic(bool isPause)
{
    if (isPause)
        Instance.musicAudio.mute = true;   // Mute
    else
        Instance.musicAudio.mute = false;  // Unmute
}
```

**Usage**:

```csharp
// In GameManager.Victory()
SoundManager.Instance.PauseMusic(true);  // Mute music
SoundManager.PlaySfx(soundVictory);      // Play victory sound

// Later
SoundManager.Instance.PauseMusic(false);  // Unmute music
```

**Note**: `mute` doesn't actually pause - it just silences. The music keeps playing.

### Click() Helper

Located in `SoundManager.cs:78-81`

```csharp
public static void Click()
{
    PlaySfx(Instance.soundClick);
}
```

**Usage**:

```csharp
// In buttons throughout UI
public void OnButtonClick()
{
    SoundManager.Click();  // Shorthand for PlaySfx(soundClick)
    // Do button action
}
```

---

## Manager Communication Flow

### Complete Game Flow Diagram

```
GAME START
    │
    ▼
GameManager.Awake()
    ├─ Set targetFrameRate = 60
    ├─ Create Instance
    ├─ State = Menu
    └─ Instantiate level prefab
        └─ LevelEnemyManager spawns
        └─ LevelManager spawns
        └─ SoundManager spawns

MenuManager.Start()
    ├─ Show countdown UI
    ├─ Wait 1 second
    └─ Call GameManager.StartGame()

GameManager.StartGame()
    ├─ State = Playing
    ├─ Find all IListeners
    └─ Broadcast IPlay()
        ├─ LevelEnemyManager.IPlay()
        │   └─ Start spawning enemies
        └─ Enemy.IPlay()
            └─ isPlaying = true

LevelEnemyManager.SpawnEnemyCo()
    ├─ Spawn wave 1
    ├─ Spawn wave 2
    ├─ ...
    └─ Wait until all dead
        └─ (Victory detected elsewhere)

Enemy takes damage
    ├─ currentHealth -= damage
    └─ If health <= 0:
        └─ Enemy.Die()
            ├─ SetActive(false)
            └─ RemoveListener()

LevelEnemyManager checks
    └─ isEnemyAlive() returns false
        └─ GameManager.Victory() called

GameManager.Victory()
    ├─ State = Success
    ├─ Stop music
    ├─ Play victory sound
    └─ Broadcast ISuccess()
        ├─ MenuManager.ISuccess()
        │   └─ Show victory UI
        └─ LevelEnemyManager.ISuccess()
            └─ StopAllCoroutines()

Menu_Victory.Start()
    ├─ Check fortress health %
    ├─ Award stars (1-3)
    └─ Show buttons (Menu, Restart, Next)

GAME END
```

---

## How to Create Custom Manager

### Step-by-Step: Create ItemManager

#### Step 1: Create Script

Create `ItemManager.cs`:

```csharp
using UnityEngine;
using System.Collections.Generic;

namespace RGame
{
    public class ItemManager : MonoBehaviour
    {
        // Singleton
        public static ItemManager Instance { get; private set; }

        // Item data
        public List<GameObject> activeItems = new List<GameObject>();
        public GameObject[] itemPrefabs;

        // Settings
        public float spawnInterval = 5f;
        public Transform spawnArea;

        void Awake()
        {
            // Set singleton
            Instance = this;
        }

        void Start()
        {
            // Start spawning items
            InvokeRepeating("SpawnRandomItem", spawnInterval, spawnInterval);
        }

        void SpawnRandomItem()
        {
            // Check if game is playing
            if (GameManager.Instance.State != GameManager.GameState.Playing)
                return;

            // Random position
            Vector2 spawnPos = new Vector2(
                Random.Range(spawnArea.position.x - 5, spawnArea.position.x + 5),
                Random.Range(spawnArea.position.y - 2, spawnArea.position.y + 2)
            );

            // Random item
            GameObject itemPrefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];

            // Spawn
            GameObject item = Instantiate(itemPrefab, spawnPos, Quaternion.identity);
            activeItems.Add(item);
        }

        public void RemoveItem(GameObject item)
        {
            activeItems.Remove(item);
            Destroy(item);
        }

        public int GetActiveItemCount()
        {
            return activeItems.Count;
        }
    }
}
```

#### Step 2: Create GameObject

1. Create empty GameObject: `GameObject → Create Empty`
2. Name it: `ItemManager`
3. Add script: `Add Component → ItemManager`
4. Assign prefabs in Inspector

#### Step 3: Use in Other Scripts

```csharp
// In Player.cs
void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("Item"))
    {
        // Collect item
        Heal(10);

        // Remove from manager
        ItemManager.Instance.RemoveItem(other.gameObject);
    }
}
```

### Best Practices for Managers

1. **Use Singleton Pattern**
   ```csharp
   public static MyManager Instance { get; private set; }

   void Awake()
   {
       Instance = this;
   }
   ```

2. **DontDestroyOnLoad for Persistent Managers**
   ```csharp
   void Awake()
   {
       if (Instance == null)
       {
           Instance = this;
           DontDestroyOnLoad(gameObject);
       }
       else
       {
           Destroy(gameObject);
       }
   }
   ```

3. **Clear Data on Scene Load**
   ```csharp
   void OnEnable()
   {
       SceneManager.sceneLoaded += OnSceneLoaded;
   }

   void OnDisable()
   {
       SceneManager.sceneLoaded -= OnSceneLoaded;
   }

   void OnSceneLoaded(Scene scene, LoadSceneMode mode)
   {
       // Clear lists
       activeItems.Clear();
   }
   ```

4. **Null Checks**
   ```csharp
   if (GameManager.Instance != null)
       GameManager.Instance.Victory();
   ```

---

## Common Issues & Solutions

### Issue 1: NullReferenceException on Manager.Instance

**Symptoms**:
- `NullReferenceException: Object reference not set to an instance`
- Happens when accessing `Manager.Instance`

**Possible Causes & Fixes**:

1. **Manager GameObject Not in Scene**
   ```
   Check Hierarchy:
   - GameManager GameObject ✓
   - SoundManager GameObject ✓

   Fix: Add manager GameObjects to scene
   ```

2. **Accessing Before Awake()**
   ```csharp
   // ❌ Wrong order
   void Awake()
   {
       GameManager.Instance.AddListener(this);  // Instance not set yet!
       Instance = this;
   }

   // ✓ Correct order
   void Awake()
   {
       Instance = this;  // Set first
   }

   void OnEnable()
   {
       if (GameManager.Instance)
           GameManager.Instance.AddListener(this);  // Now safe
   }
   ```

3. **Script Execution Order**
   ```
   Edit → Project Settings → Script Execution Order
   - GameManager: -100 (runs first)
   - Other managers: 0
   - Game objects: 100
   ```

### Issue 2: Listeners Not Receiving Events

**Symptoms**:
- GameManager broadcasts event, but listeners don't respond

**Possible Causes & Fixes**:

1. **Not Implementing IListener**
   ```csharp
   // ❌ Missing interface
   public class MyScript : MonoBehaviour
   {
       public void IPlay() { }  // Won't work!
   }

   // ✓ Correct
   public class MyScript : MonoBehaviour, IListener
   {
       public void IPlay() { }
   }
   ```

2. **Not Added to Listener List**
   ```csharp
   // Check in Start():
   Debug.Log("Listeners count: " + GameManager.Instance.listeners.Count);

   // Fix: Manually add
   void OnEnable()
   {
       GameManager.Instance.AddListener(this);
   }
   ```

3. **Removed Too Early**
   ```csharp
   // ❌ Removed before event
   void Die()
   {
       GameManager.Instance.RemoveListener(this);
       // Victory event fires now → won't receive it
   }

   // ✓ Remove after handling
   public void ISuccess()
   {
       // Handle event
       GameManager.Instance.RemoveListener(this);
   }
   ```

### Issue 3: Sound Not Playing

**Symptoms**:
- Call `SoundManager.PlaySfx()`, no sound plays

**Possible Causes & Fixes**:

1. **Audio Clip Not Assigned**
   ```csharp
   // Check in Inspector
   SoundManager → soundClick: None (AudioClip) ✗

   // Fix: Drag audio file to field
   ```

2. **GlobalValue.isSound = false**
   ```csharp
   // Check setting
   Debug.Log("Sound enabled: " + GlobalValue.isSound);

   // Fix: Enable sound
   GlobalValue.isSound = true;
   SoundManager.SoundVolume = 1;
   ```

3. **No AudioListener in Scene**
   ```
   Check Main Camera:
   - Audio Listener component ✓

   Fix: Add AudioListener to Camera
   ```

4. **Volume = 0**
   ```csharp
   // Check volume
   Debug.Log("Sound volume: " + SoundManager.SoundVolume);

   // Fix: Increase volume
   SoundManager.SoundVolume = 1;
   ```

### Issue 4: Victory/GameOver Not Triggering

**Symptoms**:
- All enemies dead, but no victory screen
- Fortress destroyed, but no game over screen

**Possible Causes & Fixes**:

1. **Victory() Not Called**
   ```csharp
   // In LevelEnemyManager, add debug:
   while (isEnemyAlive()) { yield return new WaitForSeconds(0.1f); }
   Debug.Log("All enemies dead!");
   GameManager.Instance.Victory();  // Make sure this is called
   ```

2. **State Already Set**
   ```csharp
   // In GameManager.Victory()
   if (State == GameState.Success)
       return;  // Already won, exits early

   // Fix: Only call Victory() once
   ```

3. **Listener Not Responding**
   ```csharp
   // In MenuManager.ISuccess(), add debug:
   public void ISuccess()
   {
       Debug.Log("MenuManager received ISuccess!");
       StartCoroutine(VictoryCo());
   }
   ```

### Issue 5: Enemies Spawn All at Once

**Symptoms**:
- All enemies appear instantly instead of over time

**Possible Causes & Fixes**:

1. **Wrong Wait Values**
   ```csharp
   // Check wave config
   EnemyWave[0].wait = 0;  // Should be > 0
   EnemySpawn.rate = 0;    // Should be > 0

   // Fix: Set proper delays
   EnemyWave[0].wait = 2;
   EnemySpawn.rate = 0.5f;
   ```

2. **Coroutine Not Starting**
   ```csharp
   // In LevelEnemyManager.IPlay(), add debug:
   public void IPlay()
   {
       Debug.Log("Starting spawn coroutine");
       StartCoroutine(SpawnEnemyCo());
   }
   ```

3. **Time.timeScale = 0**
   ```csharp
   // Check time scale
   Debug.Log("Time scale: " + Time.timeScale);

   // Fix: Ensure time scale = 1
   Time.timeScale = 1;
   ```

### Issue 6: Music Won't Stop

**Symptoms**:
- Call `PauseMusic(true)`, music continues

**Possible Causes & Fixes**:

1. **Wrong Method**
   ```csharp
   // ❌ Mutes but doesn't stop
   SoundManager.Instance.PauseMusic(true);

   // ✓ Actually stops
   SoundManager.Instance.musicAudio.Stop();
   ```

2. **Multiple Audio Sources**
   ```
   Check scene for duplicate:
   - SoundManager (1)
   - SoundManager (1) ← Extra!

   Fix: Delete duplicate
   ```

---

## Summary

The **Managers System** coordinates all game systems through:

1. **GameManager** - Central state controller
   - Manages game state (Menu, Playing, Pause, Success, GameOver)
   - Observer pattern via IListener interface
   - Broadcasts events to all listeners
   - Handles Victory/GameOver logic

2. **LevelEnemyManager** - Enemy wave spawning
   - Loads wave configuration
   - Spawns enemies over time with delays
   - Tracks spawned enemies
   - Detects victory condition

3. **LevelManager** - Resource management
   - Initializes mana from level config
   - Provides global mana access

4. **SoundManager** - Audio system
   - Singleton pattern for global access
   - Two AudioSources (music + sound FX)
   - Volume control
   - Mute/unmute functionality

**Key Concepts**:
- **Singleton Pattern**: Global access via `.Instance`
- **Observer Pattern**: Loosely-coupled event system via IListener
- **Coroutines**: Time-based enemy spawning
- **DRY Principle**: Centralized managers reduce code duplication

**Best Practices**:
- Always null-check `Manager.Instance`
- Add/remove listeners in OnEnable/OnDisable
- Use singleton for global systems
- Broadcast events instead of direct references

**Next Steps**:
- Read `10_How_To_Guides.md` for practical examples
- Read `02_Player_System_Complete.md` to see how player uses managers
- Read `03_Enemy_System_Complete.md` to see IListener implementation

---

**Last Updated**: 2025
**File**: `Documents/05_Managers_Complete.md`
