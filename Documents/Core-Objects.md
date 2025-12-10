# Core Game Objects: Architecture Backbone
## Understanding GameManager, MenuManager, SoundManager, and CameraController

**Document Version**: 2.0 (Updated October 2025)
**Original**: Vietnamese (Version 1.0)
**Difficulty**: Intermediate
**Time to Read**: 40-50 minutes
**Prerequisites**: Basic Unity knowledge, understanding of classes and objects

---

## Table of Contents

1. [Introduction](#introduction)
2. [Overview: The Big Four](#overview-the-big-four)
3. [GameManager - Game State Controller](#gamemanager---game-state-controller)
4. [MenuManager - UI Controller](#menumanager---ui-controller)
5. [SoundManager - Audio System](#soundmanager---audio-system)
6. [CameraController - View Management](#cameracontroller---view-management)
7. [How They Work Together](#how-they-work-together)
8. [Observer Pattern Explained](#observer-pattern-explained)
9. [Practical Examples](#practical-examples)
10. [Troubleshooting](#troubleshooting)
11. [Summary](#summary)

---

## Introduction

This document explains the **four core objects** that form the backbone of "Lawn Defense: Monsters Out":

1. **GameManager** - Controls game state (menu, playing, paused, victory, defeat)
2. **MenuManager** - Manages UI panels and scene transitions
3. **SoundManager** - Handles all audio (music and sound effects)
4. **CameraController** - Controls camera movement and boundaries

**Why These Are Important**:
- They coordinate **all other systems** in the game
- Understanding them is **essential for modifications**
- They use **design patterns** you'll see in professional games
- They're **singleton objects** (only one instance exists)

**What You'll Learn**:
- How game state is managed
- How UI panels are shown/hidden
- How audio is played throughout the game
- How camera follows action
- How systems communicate via Observer pattern

---

## Overview: The Big Four

### Architectural Diagram

```
                    ┌─────────────────┐
                    │  GAME MANAGER   │ ◄─── Master Controller
                    │  (Singleton)    │      Controls game state
                    └────────┬────────┘
                             │
                ┌────────────┼────────────┐
                │            │            │
                ▼            ▼            ▼
        ┌──────────┐  ┌──────────┐  ┌──────────┐
        │  MENU    │  │  SOUND   │  │  CAMERA  │
        │ MANAGER  │  │ MANAGER  │  │CONTROLLER│
        └────┬─────┘  └────┬─────┘  └────┬─────┘
             │             │             │
             │             │             │
        ┌────┴─────┐  ┌────┴─────┐  ┌────┴─────┐
        │ UI Panel │  │  Music   │  │ Position │
        │ Control  │  │  & SFX   │  │& Limits  │
        └──────────┘  └──────────┘  └──────────┘

        All other game objects register as listeners
        and respond to GameManager state changes
```

### Responsibility Matrix

| Manager | Primary Role | Key Methods | Used By |
|---------|-------------|-------------|---------|
| **GameManager** | Game state control | `StartGame()`, `Victory()`, `GameOver()` | Everything |
| **MenuManager** | UI management | `ShowPanel()`, `LoadScene()` | UI elements |
| **SoundManager** | Audio playback | `PlaySFX()`, `PlayMusic()` | All scripts |
| **CameraController** | View control | `SetLimits()`, `FollowTarget()` | Player, Level |

### Singleton Pattern

All four managers use the **Singleton pattern**:

```csharp
public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // ← One global instance

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist between scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicates
        }
    }
}

// Usage from anywhere:
GameManager.Instance.Victory();
```

**Benefits**:
- Access from any script without `GetComponent` or `Find`
- Guaranteed single instance
- Persists across scene loads

---

## GameManager - Game State Controller

**File**: `Assets/_MonstersOut/Scripts/Managers/GameManager.cs`

**Purpose**: Master controller for game state and event broadcasting

### Core Responsibilities

```
┌─────────────────────────────────────────┐
│           GAME MANAGER                  │
├─────────────────────────────────────────┤
│                                         │
│ 1. State Management                     │
│    ├─ Menu (not playing)                │
│    ├─ Playing (active gameplay)         │
│    ├─ Pause (game frozen)               │
│    ├─ Success (level won)               │
│    └─ GameOver (level lost)             │
│                                         │
│ 2. Listener Management                  │
│    ├─ Add listeners (IListener)         │
│    ├─ Remove listeners                  │
│    └─ Broadcast events to all           │
│                                         │
│ 3. Level Management                     │
│    ├─ Load level prefabs                │
│    ├─ Track current level               │
│    └─ Manage player reference           │
│                                         │
└─────────────────────────────────────────┘
```

### Game State Enum

```csharp
public enum GameState
{
    Prepare,   // Loading/initializing
    Playing,   // Active gameplay
    Pause,     // Paused (Time.timeScale = 0)
    Success,   // Level completed
    Fail       // Level failed
}

public GameState State = GameState.Prepare;
```

### State Flow Diagram

```
        START
          │
          ▼
    ┌──────────┐
    │ PREPARE  │ ◄─── Scene loading
    └────┬─────┘
         │
         │ StartGame()
         │
         ▼
    ┌──────────┐
    │ PLAYING  │ ◄─── Active gameplay
    └────┬─────┘
         │
    ┌────┴─────────────────┬─────────────┐
    │                      │             │
    │ Pause()         Victory()     GameOver()
    │                      │             │
    ▼                      ▼             ▼
┌─────────┐          ┌─────────┐   ┌─────────┐
│ PAUSE   │          │ SUCCESS │   │  FAIL   │
└────┬────┘          └─────────┘   └─────────┘
     │
     │ Resume()
     │
     └───────────────────┘
```

### Key Methods Explained

#### StartGame()

```csharp
public void StartGame()
{
    Debug.Log("Game starting!");

    // Change state
    State = GameState.Playing;

    // Notify all listeners
    foreach (var listener in listeners)
    {
        if (listener != null)
            listener.IPlay(); // ← Calls IPlay() on each listener
    }
}
```

**What happens**:
1. State changes to `Playing`
2. Every registered listener receives `IPlay()` call
3. Listeners respond (e.g., enemy AI starts, UI updates)

**Visual representation**:
```
StartGame() called
      │
      ├─→ State = Playing
      │
      └─→ Broadcast to listeners:
          ├─ Enemy.IPlay()     → Starts AI
          ├─ UI.IPlay()        → Shows HUD
          ├─ Spawner.IPlay()   → Begins waves
          └─ Timer.IPlay()     → Starts countdown
```

#### Victory()

```csharp
public void Victory()
{
    Debug.Log("Victory!");

    // Change state
    State = GameState.Success;

    // Notify all listeners
    foreach (var listener in listeners)
    {
        if (listener != null)
            listener.ISuccess(); // ← Victory event
    }
}
```

**What happens**:
1. State changes to `Success`
2. Broadcast `ISuccess()` to all listeners
3. Listeners respond:
   - MenuManager shows victory screen
   - Enemies stop attacking
   - Score is calculated
   - Coins are awarded

#### GameOver()

```csharp
public void GameOver()
{
    Debug.Log("Game Over!");

    // Change state
    State = GameState.Fail;

    // Notify all listeners
    foreach (var listener in listeners)
    {
        if (listener != null)
            listener.IGameOver(); // ← Failure event
    }
}
```

#### Pause/Resume

```csharp
public void Pause()
{
    State = GameState.Pause;
    Time.timeScale = 0; // ← Freezes game time

    foreach (var listener in listeners)
    {
        if (listener != null)
            listener.IPause();
    }
}

public void Resume()
{
    State = GameState.Playing;
    Time.timeScale = 1; // ← Resumes normal time

    foreach (var listener in listeners)
    {
        if (listener != null)
            listener.IPlay();
    }
}
```

**Time.timeScale explained**:
```
timeScale = 1.0    Normal speed (60 FPS = 60 updates/sec)
timeScale = 0.5    Half speed   (60 FPS = 30 updates/sec)
timeScale = 0.0    Frozen       (no Update() calls)
timeScale = 2.0    Double speed (60 FPS = 120 updates/sec)
```

### Listener Management

```csharp
public List<IListener> listeners = new List<IListener>();

public void AddListener(IListener listener)
{
    if (!listeners.Contains(listener))
    {
        listeners.Add(listener);
        Debug.Log("Listener added: " + listener);
    }
}

public void RemoveListener(IListener listener)
{
    if (listeners.Contains(listener))
    {
        listeners.Remove(listener);
        Debug.Log("Listener removed: " + listener);
    }
}
```

**Objects register themselves**:
```csharp
// In Enemy.cs, MenuManager.cs, UI.cs, etc.
void Start()
{
    GameManager.Instance.AddListener(this);
}

void OnDestroy()
{
    if (GameManager.Instance != null)
        GameManager.Instance.RemoveListener(this);
}
```

### Level Management

```csharp
[Header("Levels")]
public GameObject[] gameLevels;     // Array of level prefabs
public int currentLevel = 0;        // Current level index

void Awake()
{
    // Instantiate current level
    if (currentLevel < gameLevels.Length)
    {
        GameObject level = Instantiate(
            gameLevels[currentLevel],
            Vector3.zero,
            Quaternion.identity
        );

        Debug.Log("Loaded level: " + level.name);
    }
}
```

**Level loading flow**:
```
GameManager.Awake()
      │
      ├─→ Read currentLevel (e.g., 0)
      │
      ├─→ Get gameLevels[0] (Level 1 prefab)
      │
      ├─→ Instantiate level in scene
      │
      └─→ Level spawns:
          ├─ Ground
          ├─ Fortresses
          ├─ Spawn points
          └─ LevelEnemyManager
```

### Practical Usage Examples

**Example 1: Check if game is playing**
```csharp
void Update()
{
    // Only move if game is playing
    if (GameManager.Instance.State == GameState.Playing)
    {
        transform.position += Vector3.right * speed * Time.deltaTime;
    }
}
```

**Example 2: Trigger victory from script**
```csharp
void OnAllEnemiesDefeated()
{
    Debug.Log("All enemies defeated!");
    GameManager.Instance.Victory();
}
```

**Example 3: Listen to game events**
```csharp
public class MyScript : MonoBehaviour, IListener
{
    void Start()
    {
        GameManager.Instance.AddListener(this);
    }

    // Implement IListener interface
    public void IPlay()
    {
        Debug.Log("Game started!");
    }

    public void ISuccess()
    {
        Debug.Log("Level won!");
    }

    public void IGameOver()
    {
        Debug.Log("Level lost!");
    }

    public void IPause()
    {
        Debug.Log("Game paused!");
    }

    void OnDestroy()
    {
        GameManager.Instance?.RemoveListener(this);
    }
}
```

---

## MenuManager - UI Controller

**File**: `Assets/_MonstersOut/Scripts/UI/MenuManager.cs`

**Purpose**: Manages UI panels and scene transitions

### Core Responsibilities

```
┌─────────────────────────────────────────┐
│          MENU MANAGER                   │
├─────────────────────────────────────────┤
│                                         │
│ 1. Panel Management                     │
│    ├─ StartUI (main menu)               │
│    ├─ UI (in-game HUD)                  │
│    ├─ VictoryUI (win screen)            │
│    ├─ FailUI (lose screen)              │
│    └─ PauseUI (pause menu)              │
│                                         │
│ 2. Scene Loading                        │
│    ├─ Async scene transitions           │
│    ├─ Loading screen                    │
│    └─ Progress tracking                 │
│                                         │
│ 3. Audio Control                        │
│    ├─ Sound toggle                      │
│    ├─ Music toggle                      │
│    └─ Volume sliders                    │
│                                         │
└─────────────────────────────────────────┘
```

### Panel System

**UI Hierarchy**:
```
Canvas
├── StartUI (Main Menu)
│   ├── PlayButton
│   ├── SettingsButton
│   └── QuitButton
│
├── UI (In-Game HUD)
│   ├── HealthBar
│   ├── CoinDisplay
│   ├── WaveCounter
│   └── PauseButton
│
├── VictoryUI (Win Screen)
│   ├── Stars (1-3 based on performance)
│   ├── Coins Earned
│   ├── NextButton
│   └── MenuButton
│
├── FailUI (Lose Screen)
│   ├── "Defeat" Message
│   ├── RetryButton
│   └── MenuButton
│
└── PauseUI (Pause Menu)
    ├── ResumeButton
    ├── RestartButton
    └── QuitButton
```

### Key Methods

#### ShowPanel()

```csharp
public void ShowStartUI()
{
    StartUI.SetActive(true);
    UI.SetActive(false);
    VictoryUI.SetActive(false);
    FailUI.SetActive(false);
    PauseUI.SetActive(false);

    Debug.Log("Showing start UI");
}

public void ShowGameUI()
{
    StartUI.SetActive(false);
    UI.SetActive(true);           // ← Show in-game HUD
    VictoryUI.SetActive(false);
    FailUI.SetActive(false);
    PauseUI.SetActive(false);

    // Start the game
    GameManager.Instance.StartGame();
}
```

**Panel visibility pattern**:
```
Only ONE panel active at a time:

[StartUI] ─────┐
               ├──→ Only one ✓
[UI]       ────┤
               │
[VictoryUI]────┤
               │
[FailUI]   ────┤
               │
[PauseUI]  ────┘
```

#### Scene Loading (Async)

```csharp
using UnityEngine.SceneManagement;

public void LoadSceneWithName(string sceneName)
{
    StartCoroutine(LoadAsynchronously(sceneName));
}

IEnumerator LoadAsynchronously(string sceneName)
{
    // Start loading scene in background
    AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

    // Show loading panel
    loadingPanel.SetActive(true);

    // Wait until scene is loaded
    while (!operation.isDone)
    {
        // Update loading bar (0.0 to 1.0)
        float progress = Mathf.Clamp01(operation.progress / 0.9f);
        loadingSlider.value = progress;
        loadingText.text = (progress * 100f) + "%";

        yield return null; // Wait one frame
    }

    // Scene loaded!
    Debug.Log("Scene loaded: " + sceneName);
}
```

**Async loading benefits**:
- Game doesn't freeze during loading
- Can show loading bar
- Better user experience

**Loading timeline**:
```
Time: 0s ────────────1s────────────2s────────────3s
      │             │             │             │
      ▼             ▼             ▼             ▼
   Start load    30% loaded    70% loaded   Complete
      │             │             │             │
      └─────────────┴─────────────┴─────────────┘
              Loading bar fills →
```

#### Audio Toggle

```csharp
public void SoundVolume()
{
    // Toggle sound on/off
    if (SoundManager.Instance.soundVolume == 0)
    {
        SoundManager.Instance.soundVolume = 1;
        soundButton.sprite = soundOnSprite;
    }
    else
    {
        SoundManager.Instance.soundVolume = 0;
        soundButton.sprite = soundOffSprite;
    }
}

public void MusicVolume()
{
    // Toggle music on/off
    if (SoundManager.Instance.musicVolume == 0)
    {
        SoundManager.Instance.musicVolume = 0.5f;
        musicButton.sprite = musicOnSprite;
        SoundManager.Instance.musicsGame.UnPause();
    }
    else
    {
        SoundManager.Instance.musicVolume = 0;
        musicButton.sprite = musicOffSprite;
        SoundManager.Instance.musicsGame.Pause();
    }
}
```

### IListener Implementation

MenuManager listens to GameManager events:

```csharp
public class MenuManager : MonoBehaviour, IListener
{
    void Start()
    {
        GameManager.Instance.AddListener(this);
    }

    // Called when level completed
    public void ISuccess()
    {
        Debug.Log("Victory UI triggered");
        ShowVictoryUI();
    }

    // Called when level failed
    public void IGameOver()
    {
        Debug.Log("Fail UI triggered");
        ShowFailUI();
    }

    // Other IListener methods...
    public void IPlay() { }
    public void IPause() { }

    void OnDestroy()
    {
        GameManager.Instance?.RemoveListener(this);
    }
}
```

**Event flow**:
```
GameManager.Victory() called
        │
        └─→ Broadcasts ISuccess() to listeners
            │
            ├─→ MenuManager.ISuccess()
            │   └─→ Shows VictoryUI
            │
            ├─→ Enemy.ISuccess()
            │   └─→ Stops attacking
            │
            └─→ UI.ISuccess()
                └─→ Updates final score
```

### Practical Examples

**Example 1: Start game from button**
```csharp
// Attached to "Play" button
public void OnPlayButtonClicked()
{
    ShowGameUI();              // Switch to game HUD
    // GameManager.StartGame() called automatically in ShowGameUI()
}
```

**Example 2: Pause game**
```csharp
// Attached to pause button
public void OnPauseButtonClicked()
{
    GameManager.Instance.Pause();  // Freeze game
    ShowPauseUI();                  // Show pause menu
}
```

**Example 3: Load next level**
```csharp
// Attached to "Next Level" button
public void OnNextLevelClicked()
{
    GlobalValue.levelPlaying++;            // Increment level
    LoadSceneWithName("GameplayScene");    // Reload gameplay scene
}
```

---

## SoundManager - Audio System

**File**: `Assets/_MonstersOut/Scripts/Managers/SoundManager.cs`

**Purpose**: Centralized audio playback for music and sound effects

### Core Responsibilities

```
┌─────────────────────────────────────────┐
│          SOUND MANAGER                  │
├─────────────────────────────────────────┤
│                                         │
│ 1. Music Playback                       │
│    ├─ Background music looping          │
│    ├─ Music volume control              │
│    └─ Music transitions                 │
│                                         │
│ 2. Sound Effects (SFX)                  │
│    ├─ One-shot sounds                   │
│    ├─ Positional audio                  │
│    └─ SFX volume control                │
│                                         │
│ 3. Audio Sources                        │
│    ├─ Music AudioSource (looping)       │
│    └─ SFX AudioSource (one-shot)        │
│                                         │
└─────────────────────────────────────────┘
```

### Audio System Architecture

```
┌──────────────────────────────────┐
│      SOUND MANAGER               │
│                                  │
│  ┌────────────┐  ┌────────────┐ │
│  │ Music      │  │ SFX        │ │
│  │ Source     │  │ Source     │ │
│  └─────┬──────┘  └─────┬──────┘ │
└────────┼───────────────┼────────┘
         │               │
         ▼               ▼
    ┌─────────┐     ┌─────────┐
    │Background│    │Shoot    │
    │ Music    │    │Hit      │
    │(Looping) │    │Jump     │
    │          │    │Die      │
    └─────────┘     │(One-shot)
                    └─────────┘
```

### Key Components

#### Audio Sources

```csharp
[Header("Audio Sources")]
public AudioSource musicsGame;     // For background music (looping)
public AudioSource[] sounds;       // For sound effects (one-shot)

[Header("Volume")]
[Range(0f, 1f)]
public float musicVolume = 0.5f;   // Music volume (0 = mute, 1 = full)
[Range(0f, 1f)]
public float soundVolume = 1f;     // SFX volume
```

**AudioSource setup**:
```
Music AudioSource:
├─ Loop: ✓ (enabled)
├─ Play On Awake: ✓
├─ Volume: 0.5
└─ Clip: background_music.mp3

SFX AudioSource:
├─ Loop: ✗ (disabled)
├─ Play On Awake: ✗
├─ Volume: 1.0
└─ Clip: (none - assigned at runtime)
```

#### Sound Clips Library

```csharp
[Header("Sound Clips")]
public AudioClip soundClick;
public AudioClip soundShoot;
public AudioClip soundHit;
public AudioClip soundDie;
public AudioClip soundVictory;
public AudioClip soundFail;
public AudioClip soundCoin;
public AudioClip soundUpgrade;
// ... many more
```

### Key Methods

#### PlaySfx() - Play Sound Effect

```csharp
public static void PlaySfx(AudioClip sound)
{
    if (Instance.soundVolume == 0)
        return; // Muted

    if (sound != null)
    {
        // Play one-shot (doesn't interrupt current sound)
        Instance.sounds[0].PlayOneShot(sound, Instance.soundVolume);
    }
}
```

**Usage examples**:
```csharp
// When player shoots
SoundManager.PlaySfx(SoundManager.Instance.soundShoot);

// When enemy dies
SoundManager.PlaySfx(SoundManager.Instance.soundDie);

// When coin collected
SoundManager.PlaySfx(SoundManager.Instance.soundCoin);
```

**PlayOneShot explained**:
```
Multiple sounds can play simultaneously:

Timeline:
0s ─────1s──────2s──────3s──────4s
│        │       │       │       │
Shoot ───┘       │       │       │  (0.5s duration)
         Hit ────┘       │       │  (0.3s duration)
                 Coin ───┘       │  (0.4s duration)
                         Die ────┘  (1.0s duration)

All sounds overlap naturally!
```

#### PlayMusic() - Play Background Music

```csharp
public static void PlayMusic(AudioClip music)
{
    if (Instance.musicsGame.isPlaying)
    {
        Instance.musicsGame.Stop();
    }

    Instance.musicsGame.clip = music;
    Instance.musicsGame.volume = Instance.musicVolume;
    Instance.musicsGame.loop = true;  // ← Loop continuously
    Instance.musicsGame.Play();

    Debug.Log("Playing music: " + music.name);
}
```

**Usage**:
```csharp
// In menu
SoundManager.PlayMusic(SoundManager.Instance.musicMenu);

// In game
SoundManager.PlayMusic(SoundManager.Instance.musicGame);

// Boss fight
SoundManager.PlayMusic(SoundManager.Instance.musicBoss);
```

#### Volume Control

```csharp
public void SetMusicVolume(float volume)
{
    musicVolume = Mathf.Clamp01(volume); // Ensure 0-1
    musicsGame.volume = musicVolume;

    // Save to PlayerPrefs
    PlayerPrefs.SetFloat("MusicVolume", musicVolume);
    PlayerPrefs.Save();
}

public void SetSoundVolume(float volume)
{
    soundVolume = Mathf.Clamp01(volume);

    // Save to PlayerPrefs
    PlayerPrefs.SetFloat("SoundVolume", soundVolume);
    PlayerPrefs.Save();
}
```

**Connected to UI sliders**:
```
[Settings Panel]
┌──────────────────────────────┐
│ Music Volume:                │
│ [─────────●──] 0.5           │ ← Slider
│  OnValueChanged:             │
│  → SoundManager.SetMusicVolume(value)
│                              │
│ SFX Volume:                  │
│ [───────────●] 1.0           │ ← Slider
│  OnValueChanged:             │
│  → SoundManager.SetSoundVolume(value)
└──────────────────────────────┘
```

### Audio Integration Examples

**Example 1: Enemy takes damage**
```csharp
// In Enemy.cs
public void TakeDamage(float damage)
{
    currentHealth -= damage;

    // Play hurt sound
    SoundManager.PlaySfx(SoundManager.Instance.soundHurt);

    if (currentHealth <= 0)
    {
        Die();
    }
}
```

**Example 2: Player shoots arrow**
```csharp
// In Player_Archer.cs
void Shoot()
{
    // Spawn arrow
    GameObject arrow = Instantiate(arrowPrefab, shootPoint.position, Quaternion.identity);

    // Play shoot sound
    SoundManager.PlaySfx(SoundManager.Instance.soundShoot);

    // Play animation
    animator.SetTrigger("Shoot");
}
```

**Example 3: Victory celebration**
```csharp
// In GameManager.cs
public void Victory()
{
    State = GameState.Success;

    // Play victory sound
    SoundManager.PlaySfx(SoundManager.Instance.soundVictory);

    // Notify listeners
    foreach (var listener in listeners)
    {
        listener?.ISuccess();
    }
}
```

### Best Practices

**DO**:
✅ Use `PlaySfx()` for short sounds (< 2 seconds)
✅ Use `PlayMusic()` for background music
✅ Check volume before playing (SoundManager does this)
✅ Assign clips in Inspector before playing
✅ Save volume preferences to PlayerPrefs

**DON'T**:
❌ Play music with `PlaySfx()` (won't loop)
❌ Play SFX with multiple AudioSources (use one)
❌ Forget to check if clip is assigned (null check)
❌ Play sounds every frame (causes audio spam)

---

## CameraController - View Management

**File**: `Assets/_MonstersOut/Scripts/Controllers/CameraController.cs`

**Purpose**: Controls camera position, boundaries, and smooth movement

### Core Responsibilities

```
┌─────────────────────────────────────────┐
│       CAMERA CONTROLLER                 │
├─────────────────────────────────────────┤
│                                         │
│ 1. Position Clamping                    │
│    ├─ Left boundary limit               │
│    ├─ Right boundary limit              │
│    └─ Keep camera in bounds             │
│                                         │
│ 2. Smooth Movement                      │
│    ├─ Lerp to target position           │
│    ├─ Smooth speed control              │
│    └─ No jarring movements              │
│                                         │
│ 3. Input Handling                       │
│    ├─ Mouse drag (PC)                   │
│    ├─ Touch drag (Mobile)               │
│    └─ Follow target (optional)          │
│                                         │
└─────────────────────────────────────────┘
```

### Camera Boundaries

**Level Layout**:
```
                 Camera View (visible area)
                 ┌─────────────┐
                 │             │
  Limit Left     │   Player    │     Limit Right
      │          │             │          │
      ▼          └─────────────┘          ▼
──────┼──────────────────────────────────┼──────
     -10                                  15
      ◄────────────────────────────────►
          Camera movement range (25 units)

Outside bounds: Empty space, no enemies
```

### Key Variables

```csharp
[Header("Camera Limits")]
public float limitLeft = -5f;      // Leftmost position
public float limitRight = 5f;      // Rightmost position

[Header("Movement")]
public float smoothSpeed = 3f;     // How fast camera follows
public bool followTarget = false;  // Auto-follow player?
public Transform target;           // What to follow (player)
```

### Movement System

#### Clamping Position

```csharp
void Update()
{
    // Get desired camera position
    Vector3 targetPos = GetTargetPosition();

    // Clamp X position to limits
    float clampedX = Mathf.Clamp(
        targetPos.x,
        limitLeft,
        limitRight
    );

    // Create final position
    Vector3 finalPos = new Vector3(
        clampedX,
        transform.position.y,  // Keep Y unchanged
        transform.position.z   // Keep Z unchanged (-10 for 2D)
    );

    // Smooth movement
    transform.position = Vector3.Lerp(
        transform.position,
        finalPos,
        smoothSpeed * Time.deltaTime
    );
}
```

**Mathf.Clamp visualization**:
```
Desired X: -12    →  Clamped: -10  (at left limit)
Desired X: -5     →  Clamped: -5   (within range)
Desired X: 0      →  Clamped: 0    (within range)
Desired X: 20     →  Clamped: 15   (at right limit)

Formula: Mathf.Clamp(value, limitLeft, limitRight)
```

#### Smooth Following (Lerp)

```csharp
// Linear Interpolation (Lerp)
newPos = Lerp(currentPos, targetPos, t)

// t = speed * Time.deltaTime (0.0 to 1.0)
// t = 0.0 → Stay at current (no movement)
// t = 0.5 → Move halfway
// t = 1.0 → Jump to target (instant)

// Example with smoothSpeed = 5:
t = 5 * 0.016 = 0.08 per frame
→ Moves 8% closer each frame
→ Smooth, gradual movement
```

**Visual example**:
```
Frame 1: Current (0)  ─────────→  Target (10)
         Move 8% → New position: 0.8

Frame 2: Current (0.8) ────────→  Target (10)
         Move 8% → New position: 1.54

Frame 3: Current (1.54) ───────→  Target (10)
         Move 8% → New position: 2.21

... gradually approaches target
```

#### Drag Input

```csharp
private bool isDragging = false;
private Vector3 dragStartPos;

void Update()
{
    // Mouse button down
    if (Input.GetMouseButtonDown(0))
    {
        isDragging = true;
        dragStartPos = Input.mousePosition;
    }

    // Mouse button held
    if (Input.GetMouseButton(0) && isDragging)
    {
        Vector3 delta = Input.mousePosition - dragStartPos;

        // Move camera opposite to drag (like scrolling)
        float moveAmount = -delta.x * dragSensitivity;
        Vector3 newPos = transform.position + new Vector3(moveAmount, 0, 0);

        // Apply clamping
        newPos.x = Mathf.Clamp(newPos.x, limitLeft, limitRight);
        transform.position = newPos;

        dragStartPos = Input.mousePosition;
    }

    // Mouse button released
    if (Input.GetMouseButtonUp(0))
    {
        isDragging = false;
    }
}
```

**Drag behavior**:
```
Mouse drag LEFT (←):
    Camera moves RIGHT (→)
    (Reveals content on left)

Mouse drag RIGHT (→):
    Camera moves LEFT (←)
    (Reveals content on right)

Like scrolling a web page!
```

### Camera Helper Methods

#### Get Camera Width

```csharp
public float GetCameraHalfWidth()
{
    // Camera width in world units
    float height = Camera.main.orthographicSize * 2f;
    float width = height * Camera.main.aspect;

    return width / 2f;
}
```

**Usage**:
```csharp
// Ensure level is wider than camera view
float cameraWidth = GetCameraHalfWidth();
limitLeft = -levelWidth / 2 + cameraWidth;
limitRight = levelWidth / 2 - cameraWidth;
```

**Orthographic size explained**:
```
orthographicSize = 5  (height in world units)

┌───────────────────┐
│                   │  ▲
│                   │  │ 5 units
│     Camera        │  │ (top to center)
│     View          │  ▼
│                   │  ─── Center
│                   │  ▲
│                   │  │ 5 units
└───────────────────┘  ▼ (center to bottom)

Total height = 10 units

Width = height * aspect ratio
      = 10 * (16/9)
      = 17.78 units (for 16:9 screen)
```

### Practical Examples

**Example 1: Set camera limits from level**
```csharp
// In LevelManager
void Start()
{
    CameraController cam = Camera.main.GetComponent<CameraController>();

    // Set limits based on level size
    cam.limitLeft = -levelWidth / 2f;
    cam.limitRight = levelWidth / 2f;

    Debug.Log("Camera limits set: " + cam.limitLeft + " to " + cam.limitRight);
}
```

**Example 2: Follow player smoothly**
```csharp
// Enable in CameraController
public bool followTarget = true;
public Transform target; // Assign player

void GetTargetPosition()
{
    if (followTarget && target != null)
    {
        return target.position;
    }

    return transform.position; // Stay where we are
}
```

**Example 3: Camera shake effect**
```csharp
public void Shake(float duration, float magnitude)
{
    StartCoroutine(ShakeCo(duration, magnitude));
}

IEnumerator ShakeCo(float duration, float magnitude)
{
    Vector3 originalPos = transform.localPosition;
    float elapsed = 0f;

    while (elapsed < duration)
    {
        float x = Random.Range(-1f, 1f) * magnitude;
        float y = Random.Range(-1f, 1f) * magnitude;

        transform.localPosition = originalPos + new Vector3(x, y, 0);

        elapsed += Time.deltaTime;
        yield return null;
    }

    transform.localPosition = originalPos; // Reset
}
```

---

## How They Work Together

### System Interaction Diagram

```
                    USER INPUT
                        │
                        ▼
              ┌─────────────────┐
              │  MENU MANAGER   │
              │  Shows UI       │
              └────────┬────────┘
                       │ Calls
                       ▼
              ┌─────────────────┐
              │  GAME MANAGER   │ ◄─── Master Controller
              │  Changes State  │
              └────────┬────────┘
                       │ Broadcasts
           ┌───────────┼───────────┐
           │           │           │
           ▼           ▼           ▼
    ┌──────────┐ ┌──────────┐ ┌──────────┐
    │  ENEMY   │ │   UI     │ │  SPAWNER │
    │Listeners │ │Listeners │ │Listeners │
    └──────────┘ └──────────┘ └──────────┘

    All play appropriate sounds via:
              │
              ▼
    ┌──────────────────┐
    │  SOUND MANAGER   │
    │  Plays Audio     │
    └──────────────────┘

    While player visible in:
              │
              ▼
    ┌──────────────────┐
    │ CAMERA CONTROLLER│
    │  Shows Action    │
    └──────────────────┘
```

### Typical Game Flow

**1. Game Start Sequence**:
```
User clicks "Play" button
        │
        ▼
MenuManager.ShowGameUI()
        │
        ├─→ Hides StartUI
        ├─→ Shows UI (HUD)
        └─→ GameManager.StartGame()
                │
                ├─→ State = Playing
                ├─→ Time.timeScale = 1
                └─→ Broadcasts IPlay() to all listeners
                        │
                        ├─→ Enemy.IPlay() → Starts AI
                        ├─→ Spawner.IPlay() → Begins waves
                        ├─→ UI.IPlay() → Updates HUD
                        └─→ SoundManager.PlayMusic(gameMusic)
```

**2. Victory Sequence**:
```
All enemies defeated
        │
        ▼
LevelEnemyManager detects victory
        │
        ▼
GameManager.Victory()
        │
        ├─→ State = Success
        ├─→ SoundManager.PlaySfx(victorySound)
        └─→ Broadcasts ISuccess() to all listeners
                │
                ├─→ MenuManager.ISuccess()
                │   └─→ Shows VictoryUI
                │       ├─→ Calculates stars
                │       ├─→ Awards coins
                │       └─→ Unlocks next level
                │
                ├─→ Enemy.ISuccess()
                │   └─→ Stops attacking
                │
                └─→ UI.ISuccess()
                    └─→ Shows final stats
```

**3. Pause Sequence**:
```
User clicks pause button
        │
        ▼
MenuManager.ShowPauseUI()
        │
        ├─→ Shows PauseUI
        └─→ GameManager.Pause()
                │
                ├─→ State = Pause
                ├─→ Time.timeScale = 0  ← FREEZES GAME
                ├─→ SoundManager.musicsGame.Pause()
                └─→ Broadcasts IPause() to all listeners
                        │
                        └─→ All Update() loops frozen
                            (no movement, no spawning, no AI)

User clicks resume
        │
        ▼
MenuManager.Resume()
        │
        ├─→ Hides PauseUI
        └─→ GameManager.Resume()
                │
                ├─→ State = Playing
                ├─→ Time.timeScale = 1  ← UNFREEZES
                ├─→ SoundManager.musicsGame.UnPause()
                └─→ Game continues from exact state
```

### Communication Patterns

**Pattern 1: Direct Call**
```csharp
// MenuManager directly calls GameManager
GameManager.Instance.StartGame();
```

**Pattern 2: Event Broadcasting**
```csharp
// GameManager broadcasts to all listeners
foreach (var listener in listeners)
{
    listener.ISuccess();
}
```

**Pattern 3: Singleton Access**
```csharp
// Any script can access managers
SoundManager.Instance.PlaySfx(sound);
CameraController cam = Camera.main.GetComponent<CameraController>();
```

---

## Observer Pattern Explained

### What is the Observer Pattern?

The **Observer Pattern** is a design pattern where:
- **Subject** (GameManager) maintains a list of **Observers** (listeners)
- When Subject's state changes, it **notifies** all Observers
- Observers **react** to the notification

**Real-world analogy**:
```
Newsletter (Subject):
    Has list of subscribers (Observers)

When new article published:
    → Send email to all subscribers
    → Each subscriber decides how to respond

In our game:
    GameManager = Newsletter
    Listeners = Subscribers
    State change = New article
    ISuccess()/IPlay() = Email notification
```

### IListener Interface

```csharp
public interface IListener
{
    void IPlay();      // Called when game starts
    void ISuccess();   // Called when level won
    void IGameOver();  // Called when level lost
    void IPause();     // Called when game paused
}
```

**Why use an interface?**
- Ensures all listeners have required methods
- GameManager can call methods without knowing specific class
- Polymorphism: Different classes respond differently to same event

### Implementation Example

**Step 1: Implement Interface**
```csharp
public class Enemy : MonoBehaviour, IListener
{
    private bool isPlaying = false;

    void Start()
    {
        // Register as listener
        GameManager.Instance.AddListener(this);
    }

    // Implement interface methods
    public void IPlay()
    {
        isPlaying = true;
        Debug.Log("Enemy AI started");
    }

    public void ISuccess()
    {
        isPlaying = false;
        StopAttacking();
        Debug.Log("Enemy celebrating player victory");
    }

    public void IGameOver()
    {
        isPlaying = false;
        Celebrate();
        Debug.Log("Enemy celebrating player defeat");
    }

    public void IPause()
    {
        isPlaying = false;
        Debug.Log("Enemy AI paused");
    }

    void Update()
    {
        if (!isPlaying)
            return; // Don't do anything when not playing

        // AI behavior...
    }

    void OnDestroy()
    {
        // Unregister
        GameManager.Instance?.RemoveListener(this);
    }
}
```

**Step 2: GameManager Notifies**
```csharp
public void Victory()
{
    State = GameState.Success;

    // Notify ALL listeners
    foreach (var listener in listeners)
    {
        if (listener != null)
            listener.ISuccess(); // ← Calls Enemy.ISuccess(), UI.ISuccess(), etc.
    }
}
```

### Benefits of Observer Pattern

✅ **Loose Coupling**: GameManager doesn't need to know about specific classes
✅ **Flexibility**: Easy to add new listeners without changing GameManager
✅ **Reusability**: Same pattern works for UI, enemies, effects, etc.
✅ **Maintainability**: Clear separation of concerns

**Without Observer Pattern** (bad):
```csharp
// GameManager would need to know about every class:
public void Victory()
{
    enemy1.StopAttacking();
    enemy2.StopAttacking();
    ui.ShowVictoryScreen();
    spawner.StopSpawning();
    timer.Stop();
    // ... must update this for every new object!
}
```

**With Observer Pattern** (good):
```csharp
// GameManager just broadcasts:
public void Victory()
{
    foreach (var listener in listeners)
    {
        listener.ISuccess(); // Each object handles itself!
    }
}
```

---

## Practical Examples

### Example 1: Create Custom Listener

```csharp
using UnityEngine;

public class ParticleController : MonoBehaviour, IListener
{
    public ParticleSystem victoryParticles;
    public ParticleSystem defeatParticles;

    void Start()
    {
        GameManager.Instance.AddListener(this);

        // Stop particles initially
        victoryParticles.Stop();
        defeatParticles.Stop();
    }

    public void IPlay()
    {
        // Stop any playing particles
        victoryParticles.Stop();
        defeatParticles.Stop();
    }

    public void ISuccess()
    {
        // Play victory particles
        victoryParticles.Play();
        SoundManager.PlaySfx(SoundManager.Instance.soundVictory);
    }

    public void IGameOver()
    {
        // Play defeat particles
        defeatParticles.Play();
        SoundManager.PlaySfx(SoundManager.Instance.soundFail);
    }

    public void IPause()
    {
        // Pause particles
        victoryParticles.Pause();
        defeatParticles.Pause();
    }

    void OnDestroy()
    {
        GameManager.Instance?.RemoveListener(this);
    }
}
```

### Example 2: Custom Menu Panel

```csharp
using UnityEngine;
using UnityEngine.UI;

public class CustomPanel : MonoBehaviour
{
    public GameObject panel;
    public Text messageText;

    public void ShowPanel(string message)
    {
        panel.SetActive(true);
        messageText.text = message;

        // Play sound
        SoundManager.PlaySfx(SoundManager.Instance.soundClick);
    }

    public void HidePanel()
    {
        panel.SetActive(false);
    }

    // Call from button
    public void OnCloseButtonClicked()
    {
        HidePanel();
        SoundManager.PlaySfx(SoundManager.Instance.soundClick);
    }
}
```

### Example 3: Music Transition

```csharp
using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public AudioClip menuMusic;
    public AudioClip gameMusic;
    public AudioClip bossMusic;

    void Start()
    {
        // Play menu music initially
        SoundManager.PlayMusic(menuMusic);
    }

    public void OnGameStart()
    {
        SoundManager.PlayMusic(gameMusic);
    }

    public void OnBossAppear()
    {
        StartCoroutine(CrossfadeMusic(bossMusic, 1f));
    }

    IEnumerator CrossfadeMusic(AudioClip newMusic, float duration)
    {
        // Fade out current
        float startVolume = SoundManager.Instance.musicVolume;

        for (float t = 0; t < duration / 2; t += Time.deltaTime)
        {
            SoundManager.Instance.musicsGame.volume = Mathf.Lerp(startVolume, 0, t / (duration / 2));
            yield return null;
        }

        // Switch music
        SoundManager.PlayMusic(newMusic);

        // Fade in new
        for (float t = 0; t < duration / 2; t += Time.deltaTime)
        {
            SoundManager.Instance.musicsGame.volume = Mathf.Lerp(0, startVolume, t / (duration / 2));
            yield return null;
        }

        SoundManager.Instance.musicsGame.volume = startVolume;
    }
}
```

---

## Troubleshooting

### Problem 1: Music Not Playing

**Symptoms**:
- No background music
- SoundManager exists but silent

**Diagnosis**:
```csharp
void Start()
{
    Debug.Log("SoundManager exists: " + (SoundManager.Instance != null));
    Debug.Log("Music volume: " + SoundManager.Instance.musicVolume);
    Debug.Log("Music source playing: " + SoundManager.Instance.musicsGame.isPlaying);
    Debug.Log("Music clip assigned: " + (SoundManager.Instance.musicsGame.clip != null));
}
```

**Common Causes**:
1. ❌ Music volume set to 0
   - **Fix**: `SoundManager.Instance.musicVolume = 0.5f;`

2. ❌ No AudioListener in scene
   - **Fix**: Add AudioListener to Main Camera

3. ❌ Music clip not assigned
   - **Fix**: Assign clip in SoundManager Inspector

4. ❌ AudioSource not playing
   - **Fix**: Call `SoundManager.PlayMusic(clip);`

### Problem 2: GameManager State Not Changing

**Symptoms**:
- Game stuck in one state
- Listeners not receiving events

**Diagnosis**:
```csharp
void Update()
{
    if (Input.GetKeyDown(KeyCode.Space))
    {
        Debug.Log("Current state: " + GameManager.Instance.State);
        Debug.Log("Listener count: " + GameManager.Instance.listeners.Count);
    }
}
```

**Common Causes**:
1. ❌ State change not called
   - **Fix**: Call `GameManager.Instance.StartGame()` when ready

2. ❌ Listeners not registered
   - **Fix**: Ensure `AddListener(this)` called in Start()

3. ❌ GameManager destroyed
   - **Fix**: Add `DontDestroyOnLoad(gameObject)` in Awake()

### Problem 3: UI Panel Not Showing

**Symptoms**:
- Panel exists but invisible
- SetActive(true) doesn't work

**Diagnosis**:
```csharp
void ShowPanel()
{
    Debug.Log("Panel exists: " + (panel != null));
    Debug.Log("Panel active: " + panel.activeSelf);
    Debug.Log("Canvas exists: " + (GetComponentInParent<Canvas>() != null));

    panel.SetActive(true);
}
```

**Common Causes**:
1. ❌ Panel reference null
   - **Fix**: Assign panel in Inspector

2. ❌ Parent Canvas disabled
   - **Fix**: Enable parent Canvas first

3. ❌ Panel scale set to 0
   - **Fix**: Check RectTransform scale

4. ❌ Panel behind other UI
   - **Fix**: Increase Canvas sort order

### Problem 4: Camera Not Moving

**Symptoms**:
- Camera stuck at one position
- Drag input not working

**Diagnosis**:
```csharp
void Update()
{
    Debug.Log("Camera position: " + transform.position);
    Debug.Log("Limits: " + limitLeft + " to " + limitRight);
    Debug.Log("Mouse position: " + Input.mousePosition);
}
```

**Common Causes**:
1. ❌ Limits too restrictive
   - **Fix**: Increase range (limitLeft = -10, limitRight = 10)

2. ❌ smoothSpeed too low
   - **Fix**: Increase to 5-10

3. ❌ Input not detected
   - **Fix**: Check EventSystem exists

4. ❌ Camera position clamped
   - **Fix**: Verify clamp logic

---

## Summary

### Key Takeaways

**GameManager**:
- ✅ Controls game state (Menu, Playing, Pause, Success, Fail)
- ✅ Broadcasts events to all listeners via Observer pattern
- ✅ Manages level loading and player reference
- ✅ Singleton - access via `GameManager.Instance`

**MenuManager**:
- ✅ Shows/hides UI panels
- ✅ Handles scene transitions with async loading
- ✅ Implements IListener to respond to game events
- ✅ Controls audio toggle buttons

**SoundManager**:
- ✅ Plays music (looping background)
- ✅ Plays SFX (one-shot sounds)
- ✅ Manages volume for both
- ✅ Singleton - call `SoundManager.PlaySfx(clip)`

**CameraController**:
- ✅ Clamps camera position to level boundaries
- ✅ Smooth movement via Lerp
- ✅ Handles drag input for panning
- ✅ Optional target following

### Design Patterns Used

**Singleton**:
- One instance accessible globally
- Used by all four core managers
- Pattern: `ClassName.Instance.Method()`

**Observer**:
- Subject (GameManager) notifies Observers (listeners)
- Loose coupling between systems
- Pattern: `AddListener()`, broadcast events

**Component**:
- Unity's GameObject-Component architecture
- Each manager is a component on GameObject
- Pattern: `GetComponent<ManagerType>()`

### Integration Checklist

When adding new features, remember to:

- [ ] Register as listener if need game events
  ```csharp
  GameManager.Instance.AddListener(this);
  ```

- [ ] Play sounds for feedback
  ```csharp
  SoundManager.PlaySfx(soundClip);
  ```

- [ ] Show/hide UI via MenuManager
  ```csharp
  menuManager.ShowPanel(panelName);
  ```

- [ ] Check game state before acting
  ```csharp
  if (GameManager.Instance.State == GameState.Playing)
  ```

- [ ] Respect camera boundaries
  ```csharp
  float x = Mathf.Clamp(pos.x, cam.limitLeft, cam.limitRight);
  ```

### Next Steps

**For Beginners**:
1. Read **[00_Unity_Fundamentals.md](00_Unity_Fundamentals.md)** - Unity basics
2. Complete **[First-Tasks.md](First-Tasks.md)** - Hands-on exercises
3. Study **[01_Project_Architecture.md](01_Project_Architecture.md)** - Project structure

**For Intermediate**:
1. Read **[05_Managers_Complete.md](05_Managers_Complete.md)** - Deep dive into managers
2. Study **[12_Visual_Reference.md](12_Visual_Reference.md)** - Visual diagrams
3. Try **[10_How_To_Guides.md](10_How_To_Guides.md)** - Practical tutorials

**For Advanced**:
1. Read **[13_Code_Examples.md](13_Code_Examples.md)** - Reusable code
2. Study **[project-analysis.md](project-analysis.md)** - Technical analysis
3. Extend managers with custom features

---

## Related Documentation

### Core Systems
- **[05_Managers_Complete.md](05_Managers_Complete.md)** - Complete manager documentation
- **[01_Project_Architecture.md](01_Project_Architecture.md)** - Overall architecture

### Practical Guides
- **[First-Tasks.md](First-Tasks.md)** - Hands-on exercises
- **[10_How_To_Guides.md](10_How_To_Guides.md)** - Step-by-step tutorials
- **[11_Troubleshooting.md](11_Troubleshooting.md)** - Problem solving

### Reference
- **[12_Visual_Reference.md](12_Visual_Reference.md)** - Visual diagrams
- **[13_Code_Examples.md](13_Code_Examples.md)** - Code snippets
- **[99_Glossary.md](99_Glossary.md)** - Term definitions

---

**Understanding these four core objects is essential for working with "Lawn Defense: Monsters Out"!**

They form the backbone that coordinates all other systems in the game.

---

<p align="center">
<strong>Lawn Defense: Monsters Out</strong><br>
Core Game Objects Documentation<br>
Version 2.0 • October 2025
</p>