# UI System - Complete Guide

> **For**: Beginners who finished Unity Fundamentals
> **Read Time**: 30-40 minutes
> **Prerequisites**: 00_Unity_Fundamentals.md, 01_Project_Architecture.md

---

## Table of Contents
1. [System Overview](#system-overview)
2. [UI Architecture](#ui-architecture)
3. [Main Menu System](#main-menu-system)
4. [In-Game HUD](#in-game-hud)
5. [Menu Manager (Pause, Victory, Fail)](#menu-manager)
6. [Health Bar System](#health-bar-system)
7. [Scene Loading System](#scene-loading-system)
8. [Global Values](#global-values)
9. [How to Create Custom UI](#how-to-create-custom-ui)
10. [Common Issues & Solutions](#common-issues--solutions)

---

## System Overview

### What is the UI System?

The **UI System** manages all visual interface elements that players interact with:
- **Main Menu**: Title screen, level select, settings, shop
- **In-Game HUD**: Health bars, coin counter, wave progress
- **Pause Menu**: Pause/resume, sound/music toggles
- **Victory/Fail Screens**: Level completion with star ratings
- **Loading Screens**: Scene transition animations

### Why is this Important?

The UI System provides **player feedback** and **game control**. Understanding UI allows you to:
- Add new menus and screens
- Customize HUD elements
- Implement save/load systems
- Create tutorial overlays
- Debug game state issues

### UI System Architecture Diagram

```
┌─────────────────────────────────────────────────────┐
│                  UI SYSTEM                          │
└───────────┬─────────────────────────────────────────┘
            │
       ┌────┼──────┬──────────┬──────────┐
       │    │      │          │          │
       ▼    ▼      ▼          ▼          ▼
┌──────────┐ ┌──────┐ ┌──────────┐ ┌─────────┐ ┌────────┐
│MainMenu  │ │Menu  │ │  UI_UI   │ │Victory  │ │Health  │
│HomeScene │ │Manager│ │  (HUD)   │ │/Fail    │ │Bars    │
└──────────┘ └──────┘ └──────────┘ └─────────┘ └────────┘
     │           │          │             │           │
     ▼           ▼          ▼             ▼           ▼
Map, Shop   Pause, Load  Sliders,    Star Rating  Follow
Settings    Scene Mgmt   Coin, Mana   Buttons     Enemy
```

### Key Files

| File | Location | Purpose |
|------|----------|---------|
| `MainMenuHomeScene.cs` | `Assets/_MonstersOut/Scripts/UI/` | Main menu controller |
| `MenuManager.cs` | `Assets/_MonstersOut/Scripts/UI/` | In-game menu system |
| `UI_UI.cs` | `Assets/_MonstersOut/Scripts/UI/` | In-game HUD controller |
| `Menu_Victory.cs` | `Assets/_MonstersOut/Scripts/UI/` | Victory screen with stars |
| `HealthBarEnemyNew.cs` | `Assets/_MonstersOut/Scripts/UI/` | Enemy health bar |
| `GlobalValue.cs` | `Assets/_MonstersOut/Scripts/` | Save data & settings |

---

## UI Architecture

### Unity UI System Basics

Unity uses **Canvas** system for UI:

```
Canvas (Screen Space - Overlay)
    │
    ├─ Panel (Background)
    │   └─ Button (Play)
    │       └─ Text (Label)
    │
    ├─ Slider (Health Bar)
    │   ├─ Background
    │   ├─ Fill Area
    │   └─ Handle Slide Area
    │
    └─ Text (Coin Counter)
```

**Canvas Render Modes**:
- **Screen Space - Overlay**: UI rendered on top of everything (most common)
- **Screen Space - Camera**: UI rendered by camera (allows depth)
- **World Space**: UI exists in 3D world (rare)

### This Project's UI Structure

```
Scene: Menu
└─ Canvas
    ├─ MainPanel
    ├─ MapUI (hidden by default)
    ├─ ShopUI (hidden by default)
    ├─ SettingsUI (hidden by default)
    └─ LoadingUI (hidden by default)

Scene: Playing
└─ Canvas
    ├─ StartUI (countdown)
    ├─ UI (HUD - health, coins, wave)
    ├─ PauseUI (hidden)
    ├─ VictoryUI (hidden)
    ├─ FailUI (hidden)
    └─ LoadingUI (hidden)
```

### IListener Pattern for UI

**Problem**: How does UI know when game state changes (GameOver, Victory)?

**Solution**: **Observer Pattern** via IListener interface

```csharp
public interface IListener
{
    void IPlay();       // Called when game starts
    void ISuccess();    // Called when level won
    void IGameOver();   // Called when level lost
    void IPause();      // Called when paused
    void IUnPause();    // Called when unpaused
    // ... other methods
}
```

**MenuManager** implements IListener:
```csharp
public class MenuManager : MonoBehaviour, IListener
{
    public void ISuccess()
    {
        StartCoroutine(VictoryCo());  // Show victory screen
    }

    public void IGameOver()
    {
        StartCoroutine(GameOverCo());  // Show fail screen
    }
}
```

**How it Works**:
1. MenuManager registers with GameManager on start
2. GameManager calls `ISuccess()` when all enemies dead
3. MenuManager shows victory screen automatically

---

## Main Menu System

### MainMenuHomeScene Overview

**File**: `MainMenuHomeScene.cs`

**Purpose**: Controls the title screen and main menu

**Responsibilities**:
- Show/hide UI panels (Map, Shop, Settings)
- Load game scenes
- Display coin amount
- Handle sound/music toggles
- Social media links

### Scene Flow Diagram

```
Game Start
    │
    ▼
┌─────────────┐
│ MAIN MENU   │
└──────┬──────┘
       │
   ┌───┼────┬──────┬─────────┐
   │   │    │      │         │
   ▼   ▼    ▼      ▼         ▼
 Play Map  Shop Settings  Tutorial
   │
   ▼
┌─────────────┐
│ LOADING     │
└──────┬──────┘
       │
       ▼
┌─────────────┐
│ PLAYING     │
└──────┬──────┘
       │
   ┌───┴───┐
   │       │
   ▼       ▼
Victory  GameOver
   │       │
   └───┬───┘
       │
    ┌──┴──┐
    │     │
    ▼     ▼
  Retry Home
```

### Awake() Method

Located in `MainMenuHomeScene.cs:26-38`

```csharp
void Awake()
{
    // Set singleton instance
    Instance = this;

    // Hide all UI panels on start
    if (Loading != null)
        Loading.SetActive(false);
    if (MapUI != null)
        MapUI.SetActive(false);
    if (Settings)
        Settings.SetActive(false);
    if (ShopUI)
        ShopUI.SetActive(false);
}
```

**Why?**
- All panels start hidden
- Only show panels when player clicks buttons
- Prevents visual clutter on startup

### Start() Coroutine

Located in `MainMenuHomeScene.cs:58-74`

```csharp
IEnumerator Start()
{
    // Check and apply sound/music settings
    CheckSoundMusic();

    // If first time opening menu
    if (GlobalValue.isFirstOpenMainMenu)
    {
        GlobalValue.isFirstOpenMainMenu = false;

        // Pause background music
        SoundManager.Instance.PauseMusic(true);

        // Play intro sound
        SoundManager.PlaySfx(SoundManager.Instance.beginSoundInMainMenu);

        // Wait for intro to finish
        yield return new WaitForSeconds(
            SoundManager.Instance.beginSoundInMainMenu.length
        );

        // Resume background music
        SoundManager.Instance.PauseMusic(false);
        SoundManager.PlayMusic(SoundManager.Instance.musicsGame);
    }

    // Hide ads banner (if ads system exists)
    if (AdsManager.Instance)
        AdsManager.Instance.ShowAdmobBanner(false);
}
```

**Flow Explanation**:
```
Frame 1: Start() begins
         └─ CheckSoundMusic()

Frame 2: isFirstOpenMainMenu? YES
         ├─ Set to false (won't play again)
         ├─ Pause music
         └─ Play intro sound (3 seconds)

Frame 180: (3 seconds @ 60fps)
         ├─ Intro finishes
         ├─ Resume music
         └─ Play background music

Frame 181+: Normal menu state
```

### Update() Method

Located in `MainMenuHomeScene.cs:76-85`

```csharp
void Update()
{
    // Constantly check sound/music state
    CheckSoundMusic();

    // Update coin display on all coin text elements
    foreach (var ct in coinTxt)
    {
        ct.text = GlobalValue.SavedCoins + "";
    }
}
```

**Why Every Frame?**
- Sound/music settings might change in other scenes
- Coin amount updates when player earns coins
- Always shows correct values

### Opening Panels

**OpenMap() Method** (`MainMenuHomeScene.cs:87-101`)

```csharp
public void OpenMap(bool open)
{
    // Play click sound
    SoundManager.Click();

    // Call coroutine to handle transition
    StartCoroutine(OpenMapCo(open));
}

IEnumerator OpenMapCo(bool open)
{
    yield return null;  // Wait one frame

    // Fade to black
    BlackScreenUI.instance.Show(0.2f);

    // Toggle map UI
    MapUI.SetActive(open);

    // Fade from black
    BlackScreenUI.instance.Hide(0.2f);
}
```

**Visual Effect**:
```
Frame 100: OpenMap(true) called
           └─ Play click sound

Frame 101: OpenMapCo() starts
           └─ Wait one frame

Frame 102: BlackScreenUI.Show(0.2f)
           └─ Screen fades to black (12 frames @ 60fps)

Frame 114: MapUI.SetActive(true)
           └─ Map panel enabled

Frame 115: BlackScreenUI.Hide(0.2f)
           └─ Screen fades from black (12 frames)

Frame 127: Map fully visible
```

**Why Use BlackScreenUI?**
- Smooth visual transition
- Hides instant panel swap
- Professional polish

### Sound/Music Toggles

**TurnSound() Method** (`MainMenuHomeScene.cs:128-135`)

```csharp
public void TurnSound()
{
    // Toggle sound state
    GlobalValue.isSound = !GlobalValue.isSound;

    // Update button image
    soundImage.sprite = GlobalValue.isSound ? soundImageOn : soundImageOff;

    // Set sound volume (1 = full, 0 = mute)
    SoundManager.SoundVolume = GlobalValue.isSound ? 1 : 0;
}
```

**TurnMusic() Method** (`MainMenuHomeScene.cs:137-144`)

```csharp
public void TurnMusic()
{
    // Toggle music state
    GlobalValue.isMusic = !GlobalValue.isMusic;

    // Update button image
    musicImage.sprite = GlobalValue.isMusic ? musicImageOn : musicImageOff;

    // Set music volume
    SoundManager.MusicVolume = GlobalValue.isMusic
        ? SoundManager.Instance.musicsGameVolume
        : 0;
}
```

**How it Works**:
1. User clicks sound button
2. `TurnSound()` called via Button OnClick event
3. Toggle `GlobalValue.isSound` (true ↔ false)
4. Update button sprite (speaker on/off icon)
5. Set `SoundManager.SoundVolume` (1 or 0)

**Why GlobalValue?**
- Persists across scenes
- Saved to PlayerPrefs
- Shared with all scripts

---

## In-Game HUD

### UI_UI Overview

**File**: `UI_UI.cs`

**Purpose**: Displays in-game stats (health, coins, wave progress)

**Components**:
- Player health bar + text
- Enemy health bar + text (for boss fights)
- Wave progress bar
- Coin counter
- Mana counter
- Level name

### UI_UI Architecture

```
UI_UI GameObject
    │
    ├─ Player Health Section
    │   ├─ Slider (healthSlider)
    │   └─ Text (health)
    │
    ├─ Enemy Health Section
    │   ├─ Slider (enemyHealthSlider)
    │   └─ Text (enemyHealth)
    │
    ├─ Wave Progress Section
    │   └─ Slider (enemyWavePercentSlider)
    │
    └─ Counters
        ├─ Text (coinTxt)
        ├─ Text (manaTxt)
        └─ Text (levelName)
```

### Start() Method

Located in `UI_UI.cs:32-41`

```csharp
private void Start()
{
    // Initialize default values
    healthValue = 1;          // 100% health
    enemyWaveValue = 0;       // 0% wave progress

    // Reset sliders
    healthSlider.value = 1;
    enemyWavePercentSlider.value = 0;

    // Set level name
    levelName.text = "Level " + GlobalValue.levelPlaying;
}
```

**Initial State**:
```
healthSlider:             [████████████████████] 100%
enemyWavePercentSlider:   [                    ] 0%
levelName:                "Level 1"
coinTxt:                  "0"
manaTxt:                  "0"
```

### Update() Method (Smooth Interpolation)

Located in `UI_UI.cs:43-52`

```csharp
private void Update()
{
    // Smoothly lerp health slider to target value
    healthSlider.value = Mathf.Lerp(
        healthSlider.value,     // Current
        healthValue,            // Target
        lerpSpeed * Time.deltaTime  // Speed
    );

    // Smoothly lerp enemy health slider
    enemyHealthSlider.value = Mathf.Lerp(
        enemyHealthSlider.value,
        enemyHealthValue,
        lerpSpeed * Time.deltaTime
    );

    // Smoothly lerp wave progress
    enemyWavePercentSlider.value = Mathf.Lerp(
        enemyWavePercentSlider.value,
        enemyWaveValue,
        lerpSpeed * Time.deltaTime
    );

    // Update text (instant, no lerp needed)
    coinTxt.text = GlobalValue.SavedCoins + "";
    manaTxt.text = LevelManager.Instance.mana + "";
}
```

**Why Mathf.Lerp()?**

**Without Lerp** (instant):
```
healthValue changes: 1.0 → 0.5
healthSlider jumps:  ████████████ → ██████
                     (instant, jarring)
```

**With Lerp** (smooth):
```
Frame 100: healthValue = 0.5
           healthSlider = 1.0

Frame 101: healthSlider = Lerp(1.0, 0.5, 0.1) = 0.95
Frame 102: healthSlider = Lerp(0.95, 0.5, 0.1) = 0.90
Frame 103: healthSlider = Lerp(0.90, 0.5, 0.1) = 0.86
...
Frame 120: healthSlider ≈ 0.5

Result: Smooth animation over ~20 frames
```

**Lerp Speed Calculation**:
```csharp
lerpSpeed = 1;  // Default value
Time.deltaTime = 0.0166f;  // At 60 FPS

lerpSpeed * Time.deltaTime = 1 * 0.0166 = 0.0166

Each frame: Lerp(current, target, 0.0166)
           → Moves 1.66% toward target per frame
           → Reaches target in ~60 frames (1 second)
```

### UpdateHealthbar() Method

Located in `UI_UI.cs:54-67`

```csharp
public void UpdateHealthbar(float currentHealth, float maxHealth,
                           HEALTH_CHARACTER healthBarType)
{
    // Update player health
    if (healthBarType == HEALTH_CHARACTER.PLAYER)
    {
        // Calculate percentage (0.0 to 1.0)
        healthValue = Mathf.Clamp01(currentHealth / maxHealth);

        // Update text (e.g., "50/100")
        health.text = (int)currentHealth + "/" + (int)maxHealth;
    }
    // Update enemy health (for boss battles)
    else if (healthBarType == HEALTH_CHARACTER.ENEMY)
    {
        enemyHealthValue = Mathf.Clamp01(currentHealth / maxHealth);
        enemyHealth.text = (int)currentHealth + "/" + (int)maxHealth;
    }
}
```

**How it's Called**:

```csharp
// In TheFortrest.cs (fortress health)
MenuManager.Instance.UpdateHealthbar(
    currentHealth,    // 75
    maxHealth,        // 100
    HEALTH_CHARACTER.PLAYER
);

// Result:
healthValue = 75 / 100 = 0.75
health.text = "75/100"
healthSlider will lerp to 0.75 (75%)
```

**Mathf.Clamp01() Explanation**:
```csharp
Mathf.Clamp01(value)  // Clamps between 0 and 1

Examples:
Mathf.Clamp01(0.5)  = 0.5   ✓
Mathf.Clamp01(1.2)  = 1.0   (clamped)
Mathf.Clamp01(-0.3) = 0.0   (clamped)

Purpose: Prevent slider going below 0% or above 100%
```

### UpdateEnemyWavePercent() Method

Located in `UI_UI.cs:69-73`

```csharp
public void UpdateEnemyWavePercent(float currentSpawn, float maxValue)
{
    // Calculate wave progress percentage
    enemyWaveValue = Mathf.Clamp01(currentSpawn / maxValue);
}
```

**Usage Example**:
```
Total enemies in level: 50
Enemies spawned so far: 25

UpdateEnemyWavePercent(25, 50)
enemyWaveValue = 25 / 50 = 0.5

Wave progress slider shows 50%
```

---

## Menu Manager

### MenuManager Overview

**File**: `MenuManager.cs`

**Purpose**: Manages in-game UI panels and game state transitions

**Responsibilities**:
- Show/hide panels (Start, Victory, Fail, Pause)
- Handle pause/resume
- Load scenes
- Listen to game events (IListener)

### Singleton Pattern

Located in `MenuManager.cs:26-29`

```csharp
private void Awake()
{
    // Set singleton instance
    Instance = this;

    // Disable all UI panels on start
    StartUI.SetActive(false);
    VictotyUI.SetActive(false);
    FailUI.SetActive(false);
    PauseUI.SetActive(false);
    LoadingUI.SetActive(false);
    CharacterContainer.SetActive(false);

    // Get UI_UI component
    uiControl = gameObject.GetComponentInChildren<UI_UI>(true);
}
```

**Why Singleton?**
```csharp
// Any script can access MenuManager
MenuManager.Instance.UpdateHealthbar(...);
MenuManager.Instance.Pause();

// No need to use FindObjectOfType or references
```

### Start() Coroutine

Located in `MenuManager.cs:39-58`

```csharp
IEnumerator Start()
{
    // Apply sound/music settings
    soundImage.sprite = GlobalValue.isSound ? soundImageOn : soundImageOff;
    musicImage.sprite = GlobalValue.isMusic ? musicImageOn : musicImageOff;

    if (!GlobalValue.isSound)
        SoundManager.SoundVolume = 0;
    if (!GlobalValue.isMusic)
        SoundManager.MusicVolume = 0;

    // Show countdown UI
    StartUI.SetActive(true);

    // Wait 1 second (countdown animation)
    yield return new WaitForSeconds(1);

    // Hide countdown
    StartUI.SetActive(false);

    // Show main game UI
    UI.SetActive(true);
    CharacterContainer.SetActive(true);

    // Start the game
    GameManager.Instance.StartGame();
}
```

**Flow Diagram**:
```
Frame 1:   Start() begins
           └─ Apply settings
           └─ StartUI.SetActive(true)

Frame 1-60: Countdown UI visible ("Ready... GO!")

Frame 60:  yield finishes
           ├─ StartUI.SetActive(false)
           ├─ UI.SetActive(true)
           └─ GameManager.Instance.StartGame()

Frame 61+: Game playing, enemies spawning
```

### Pause() Method

Located in `MenuManager.cs:73-96`

```csharp
public void Pause()
{
    // Play pause sound
    SoundManager.PlaySfx(SoundManager.Instance.soundPause);

    // If game is running (timeScale != 0)
    if (Time.timeScale != 0)
    {
        // Save current time scale
        currentTimeScale = Time.timeScale;

        // Freeze game
        Time.timeScale = 0;

        // Hide game UI
        UI.SetActive(false);

        // Show pause menu
        PauseUI.SetActive(true);

        CharacterContainer.SetActive(false);
    }
    else  // Game is paused, resume
    {
        // Restore time scale
        Time.timeScale = currentTimeScale;

        // Show game UI
        UI.SetActive(true);

        // Hide pause menu
        PauseUI.SetActive(false);

        CharacterContainer.SetActive(true);
    }
}
```

**Time.timeScale Explanation**:

```csharp
Time.timeScale = 1;   // Normal speed
Time.timeScale = 0.5; // Half speed (slow motion)
Time.timeScale = 2;   // Double speed (fast forward)
Time.timeScale = 0;   // Frozen (pause)

When timeScale = 0:
- Update() still runs
- FixedUpdate() does NOT run
- Time.deltaTime = 0
- Animations stop
- Physics stops
```

**Pause/Resume Flow**:
```
[PLAYING]
    │
    │ User presses Pause
    │
    ├─ Time.timeScale = 0
    ├─ UI hidden
    └─ PauseUI shown
    │
    ▼
[PAUSED]
    │
    │ User presses Resume
    │
    ├─ Time.timeScale = 1
    ├─ PauseUI hidden
    └─ UI shown
    │
    ▼
[PLAYING]
```

### ISuccess() Method (Victory)

Located in `MenuManager.cs:103-115`

```csharp
public void ISuccess()
{
    StartCoroutine(VictoryCo());
}

IEnumerator VictoryCo()
{
    // Hide game UI
    UI.SetActive(false);
    CharacterContainer.SetActive(false);

    // Wait 1.5 seconds
    yield return new WaitForSeconds(1.5f);

    // Show victory screen
    VictotyUI.SetActive(true);
}
```

**When is this Called?**

```csharp
// In LevelEnemyManager.cs
if (allEnemiesDead && GameManager.Instance.State == GameManager.GameState.Playing)
{
    GameManager.Instance.Victory();
    // GameManager broadcasts ISuccess() to all listeners
    // MenuManager.ISuccess() is called
}
```

**Victory Flow**:
```
Frame 500:  Last enemy dies
            └─ LevelEnemyManager detects all dead

Frame 501:  GameManager.Victory() called
            └─ Broadcasts ISuccess() to listeners

Frame 502:  MenuManager.ISuccess() called
            ├─ UI hidden
            └─ Start 1.5 second wait

Frame 592:  (1.5 seconds @ 60fps)
            └─ VictotyUI shown (star animation begins)
```

### IGameOver() Method (Fail)

Located in `MenuManager.cs:128-141`

```csharp
public void IGameOver()
{
    StartCoroutine(GameOverCo());
}

IEnumerator GameOverCo()
{
    // Hide game UI
    UI.SetActive(false);
    CharacterContainer.SetActive(false);

    // Wait 1.5 seconds
    yield return new WaitForSeconds(1.5f);

    // Show fail screen
    FailUI.SetActive(true);
}
```

**When is this Called?**

```csharp
// In TheFortrest.cs (fortress health)
if (currentHealth <= 0)
{
    GameManager.Instance.GameOver();
    // GameManager broadcasts IGameOver() to all listeners
}
```

---

## Health Bar System

### HealthBarEnemyNew Overview

**File**: `HealthBarEnemyNew.cs`

**Purpose**: Individual health bar that follows enemies

**Features**:
- Follows enemy position
- Auto-hides after damage
- Smooth fade out
- Scales with health percentage

### Architecture

```
HealthBarEnemyNew GameObject
    │
    ├─ backgroundImage (SpriteRenderer)
    │   └─ Red background bar
    │
    └─ healthBar Transform
        └─ barImage (SpriteRenderer)
            └─ Green health bar (scales)
```

### Init() Method

Located in `HealthBarEnemyNew.cs:35-40`

```csharp
public void Init(Transform _target, Vector3 _offset)
{
    // Set target to follow
    target = _target;

    // Set offset from target position
    offset = _offset;
}
```

**Usage**:
```csharp
// In Enemy.Start()
var healthBarObj = (HealthBarEnemyNew)Resources.Load("HealthBar", typeof(HealthBarEnemyNew));
healthBar = (HealthBarEnemyNew)Instantiate(healthBarObj, healthBarOffset, Quaternion.identity);

healthBar.Init(transform, (Vector3)healthBarOffset);
//             ^^^^^^^^^  ^^^^^^^^^^^^^^^^^^^^^^
//             This enemy  Offset (0, 1.5)
```

### Update() Method (Follow Enemy)

Located in `HealthBarEnemyNew.cs:42-49`

```csharp
private void Update()
{
    // If target exists, follow it
    if (target)
    {
        transform.position = target.position + offset;
    }
}
```

**How it Works**:
```
Frame 1:   Enemy at (5, 0)
           healthBar.offset = (0, 1.5)
           healthBar.position = (5, 0) + (0, 1.5) = (5, 1.5)

Frame 2:   Enemy moves to (5.1, 0)
           healthBar.position = (5.1, 0) + (0, 1.5) = (5.1, 1.5)

Result: Health bar always floats above enemy
```

### UpdateValue() Method

Located in `HealthBarEnemyNew.cs:51-66`

```csharp
public void UpdateValue(float value)
{
    // Stop all running animations
    StopAllCoroutines();
    CancelInvoke();

    // Show health bar (make visible)
    backgroundImage.color = oriBGImage;
    barImage.color = oriBarImage;

    // Clamp value between 0 and 1
    value = Mathf.Max(0, value);

    // Scale health bar (1 = full, 0 = empty)
    healthBar.localScale = new Vector2(value, healthBar.localScale.y);

    // If health > 0, hide after showTime seconds
    if (value > 0)
        Invoke("HideBar", showTime);  // Default 1 second
    else
        gameObject.SetActive(false);  // Dead, disable completely
}
```

**Visual Example**:

```
Health: 100/100 (value = 1.0)
healthBar.localScale = (1.0, 1.0)
[████████████████████] 100%

Health: 75/100 (value = 0.75)
healthBar.localScale = (0.75, 1.0)
[███████████████     ] 75%

Health: 25/100 (value = 0.25)
healthBar.localScale = (0.25, 1.0)
[█████               ] 25%

Health: 0/100 (value = 0.0)
gameObject.SetActive(false)
[                    ] (disabled)
```

### HideBar() Method (Fade Out)

Located in `HealthBarEnemyNew.cs:68-76`

```csharp
private void HideBar()
{
    // Check if GameObject still active
    if (gameObject.activeInHierarchy)
    {
        // Fade background to transparent
        StartCoroutine(RGFade.FadeSpriteRenderer(
            backgroundImage,
            hideSpeed,  // Default 0.5 seconds
            new Color(oriBGImage.r, oriBGImage.g, oriBGImage.b, 0)
            //                                                  ^
            //                                                  Alpha = 0 (transparent)
        ));

        // Fade health bar to transparent
        StartCoroutine(RGFade.FadeSpriteRenderer(
            barImage,
            hideSpeed,
            new Color(oriBarImage.r, oriBarImage.g, oriBarImage.b, 0)
        ));
    }
}
```

**Fade Timeline**:
```
Frame 100:  Enemy takes damage
            ├─ UpdateValue(0.75) called
            ├─ healthBar.localScale = (0.75, 1)
            ├─ backgroundImage.color = (R, G, B, 1) [visible]
            └─ Invoke("HideBar", 1.0f)

Frame 101-159: Health bar visible at 75%

Frame 160:  HideBar() called (1 second later)
            ├─ Start fade coroutine
            └─ Fade from alpha 1 → 0 over 0.5 seconds

Frame 161-190: Fading out (30 frames @ 60fps)
            └─ alpha: 1.0 → 0.9 → 0.8 → ... → 0.1 → 0.0

Frame 191:  Health bar fully transparent (invisible)
```

---

## Scene Loading System

### LoadAsynchronously() Method

Located in `MenuManager.cs:205-220`

```csharp
IEnumerator LoadAsynchronously(string name)
{
    // Show loading UI
    LoadingUI.SetActive(true);

    // Start async scene load
    AsyncOperation operation = SceneManager.LoadSceneAsync(name);

    // Wait until load complete
    while (!operation.isDone)
    {
        // Calculate progress (0.0 to 1.0)
        float progress = Mathf.Clamp01(operation.progress / 0.9f);

        // Update slider
        slider.value = progress;

        // Update percentage text
        progressText.text = (int)progress * 100f + "%";

        yield return null;  // Wait one frame
    }
}
```

**Why divide by 0.9?**

Unity's `operation.progress` goes from 0.0 to 0.9, then jumps to 1.0 when done.

```
Without division:
operation.progress = 0.9
slider shows 90%, but scene still loading

With division:
operation.progress = 0.9
progress = 0.9 / 0.9 = 1.0
slider shows 100%, feels complete
```

**Loading Flow**:
```
Frame 1:    LoadAsynchronously("Menu") called
            ├─ LoadingUI shown
            └─ Start async load

Frame 2:    operation.progress = 0.0
            ├─ slider.value = 0.0
            └─ progressText = "0%"

Frame 30:   operation.progress = 0.3
            ├─ slider.value = 0.33
            └─ progressText = "33%"

Frame 60:   operation.progress = 0.6
            ├─ slider.value = 0.66
            └─ progressText = "66%"

Frame 90:   operation.progress = 0.9
            ├─ slider.value = 1.0
            └─ progressText = "100%"

Frame 91:   operation.isDone = true
            └─ Coroutine ends, scene loads
```

### Scene Transition Methods

**LoadHomeMenuScene()** (`MenuManager.cs:180-185`)

```csharp
public void LoadHomeMenuScene()
{
    SoundManager.Click();  // Play click sound
    StartCoroutine(LoadAsynchronously("Menu"));
}
```

**RestarLevel()** (`MenuManager.cs:187-192`)

```csharp
public void RestarLevel()
{
    SoundManager.Click();
    // Load current scene again
    StartCoroutine(LoadAsynchronously(SceneManager.GetActiveScene().name));
}
```

**LoadNextLevel()** (`MenuManager.cs:194-200`)

```csharp
public void LoadNextLevel()
{
    SoundManager.Click();

    // Increment level number
    GlobalValue.levelPlaying++;

    // Reload current scene (GameManager will load new level)
    StartCoroutine(LoadAsynchronously(SceneManager.GetActiveScene().name));
}
```

**How LoadNextLevel() Works**:
```
Current state:
- GlobalValue.levelPlaying = 1
- Scene: "Playing"

User clicks "Next Level":
1. GlobalValue.levelPlaying = 2
2. Reload "Playing" scene
3. GameManager.Awake() runs
4. GameManager instantiates gameLevels[1] (level 2)
```

---

## Global Values

### GlobalValue Overview

**GlobalValue** is a **static class** that stores game-wide settings and save data.

**Why Static?**
- Persists across scenes
- No GameObject needed
- Accessible from anywhere

### Common GlobalValue Properties

```csharp
// Player Progress
public static int levelPlaying;        // Current level (1, 2, 3...)
public static int SavedCoins;          // Total coins earned
public static int finishGameAtLevel;   // Max level in game

// Audio Settings
public static bool isSound = true;     // Sound effects on/off
public static bool isMusic = true;     // Background music on/off

// First-Time Flags
public static bool isFirstOpenMainMenu = true;  // Play intro?
```

### How GlobalValue is Used

**Saving Progress**:
```csharp
// In some script
GlobalValue.SavedCoins += 10;  // Earned 10 coins

// In UI_UI.Update()
coinTxt.text = GlobalValue.SavedCoins + "";  // Display: "10"
```

**Level Management**:
```csharp
// In GameManager.Awake()
Instantiate(gameLevels[GlobalValue.levelPlaying - 1], Vector2.zero, Quaternion.identity);
//                     ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
//                     0 = level 1, 1 = level 2, etc.
```

**Audio Settings**:
```csharp
// In MainMenuHomeScene.Start()
if (!GlobalValue.isSound)
    SoundManager.SoundVolume = 0;
if (!GlobalValue.isMusic)
    SoundManager.MusicVolume = 0;
```

### PlayerPrefs Integration

GlobalValue likely saves/loads from PlayerPrefs:

```csharp
// Hypothetical save method
public static void Save()
{
    PlayerPrefs.SetInt("Coins", SavedCoins);
    PlayerPrefs.SetInt("Level", levelPlaying);
    PlayerPrefs.SetInt("Sound", isSound ? 1 : 0);
    PlayerPrefs.SetInt("Music", isMusic ? 1 : 0);
    PlayerPrefs.Save();
}

// Hypothetical load method
public static void Load()
{
    SavedCoins = PlayerPrefs.GetInt("Coins", 0);
    levelPlaying = PlayerPrefs.GetInt("Level", 1);
    isSound = PlayerPrefs.GetInt("Sound", 1) == 1;
    isMusic = PlayerPrefs.GetInt("Music", 1) == 1;
}
```

---

## How to Create Custom UI

### Step-by-Step: Add New UI Panel

#### Step 1: Create UI in Scene

1. Right-click Hierarchy → `UI → Panel`
2. Name it: `CustomPanel`
3. Add child elements:
   - `UI → Button` (Close button)
   - `UI → Text` (Title)
   - `UI → Image` (Background)

```
Canvas
└─ CustomPanel
    ├─ Background (Image)
    ├─ Title (Text)
    └─ CloseButton (Button)
        └─ Text ("X")
```

#### Step 2: Create Script

Create `CustomPanel.cs`:

```csharp
using UnityEngine;
using UnityEngine.UI;

namespace RGame
{
    public class CustomPanel : MonoBehaviour
    {
        public Text titleText;

        void Start()
        {
            // Hide on start
            gameObject.SetActive(false);
        }

        public void Show(string title)
        {
            titleText.text = title;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void OnCloseButtonClick()
        {
            SoundManager.Click();
            Hide();
        }
    }
}
```

#### Step 3: Hook Up Button

1. Select `CloseButton` in Hierarchy
2. In Inspector, find `Button` component
3. Click `+` under `OnClick()`
4. Drag `CustomPanel` GameObject to object field
5. Select function: `CustomPanel → OnCloseButtonClick()`

#### Step 4: Access from Other Scripts

```csharp
// In MenuManager.cs, add:
public CustomPanel customPanel;

// To show panel:
public void ShowCustomPanel()
{
    customPanel.Show("Hello!");
}
```

### Example: Add Leaderboard Panel

**LeaderboardPanel.cs**:

```csharp
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace RGame
{
    public class LeaderboardPanel : MonoBehaviour
    {
        public Transform entryContainer;  // Where entries spawn
        public GameObject entryPrefab;     // Entry template

        void Start()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            // Clear old entries
            foreach (Transform child in entryContainer)
                Destroy(child.gameObject);

            // Load scores
            List<int> scores = LoadScores();

            // Create entry for each score
            for (int i = 0; i < scores.Count; i++)
            {
                GameObject entry = Instantiate(entryPrefab, entryContainer);
                entry.GetComponent<Text>().text = $"{i + 1}. {scores[i]} points";
            }

            gameObject.SetActive(true);
        }

        List<int> LoadScores()
        {
            // In real game, load from PlayerPrefs or server
            return new List<int> { 1000, 850, 720, 650, 500 };
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
```

---

## Common Issues & Solutions

### Issue 1: UI Not Visible

**Symptoms**:
- UI GameObject active, but nothing shows on screen

**Possible Causes & Fixes**:

1. **Wrong Canvas Render Mode**
   ```csharp
   // Check Canvas component
   Canvas Render Mode: Screen Space - Overlay  ✓
   Canvas Render Mode: World Space            ✗ (won't show)
   ```

2. **UI Behind Camera**
   - Check sorting order
   - Canvas should be on top
   - Fix: Increase Canvas sorting order to 100

3. **Alpha = 0**
   ```csharp
   // Check Image/Text component
   Color: (R, G, B, 0)  ✗ (transparent)
   Color: (R, G, B, 255) ✓ (visible)
   ```

4. **Raycast Target Disabled**
   - Check Image component
   - "Raycast Target" should be checked for clickable elements

### Issue 2: Button Not Clickable

**Symptoms**:
- Button visible but OnClick() doesn't fire

**Possible Causes & Fixes**:

1. **No EventSystem**
   ```
   Hierarchy must have:
   - EventSystem GameObject
   ```
   Fix: `GameObject → UI → Event System`

2. **Button Blocked by Other UI**
   - Check sibling order in Hierarchy
   - Later siblings render on top
   - Fix: Reorder in Hierarchy

3. **OnClick Not Configured**
   ```
   Button component → OnClick():
   - Must have at least one entry
   - Must reference correct GameObject
   - Must select correct function
   ```

4. **Interactable = False**
   ```csharp
   // Check Button component
   Interactable: ✓  (enabled)
   Interactable: ✗  (disabled, grayed out)
   ```

### Issue 3: Slider Not Updating

**Symptoms**:
- Call slider.value = X, but slider doesn't move

**Possible Causes & Fixes**:

1. **Wrong Value Range**
   ```csharp
   // Check Slider component
   Min Value: 0
   Max Value: 1  ✓ (for 0-100%)

   // If Max = 100:
   slider.value = 0.5;  // Shows 0.5% (wrong)
   slider.value = 50;   // Shows 50% (correct)
   ```

2. **Lerp Never Reaches Target**
   ```csharp
   // In Update():
   slider.value = Mathf.Lerp(slider.value, target, 0.1f);
   //                                               ^^^^
   //                                               Too slow if not * Time.deltaTime

   // Fix:
   slider.value = Mathf.Lerp(slider.value, target, 5 * Time.deltaTime);
   ```

3. **Fill Rect Not Assigned**
   - Check Slider component
   - "Fill Rect" must reference fill image
   - Fix: Drag Fill Image to Fill Rect field

### Issue 4: Text Not Updating

**Symptoms**:
- Change text.text value, but display doesn't change

**Possible Causes & Fixes**:

1. **Wrong Text Reference**
   ```csharp
   public Text coinText;  // Assigned in Inspector

   // Check:
   Debug.Log(coinText);  // Should not be null

   // Fix: Drag Text component to field in Inspector
   ```

2. **Font Size Too Small**
   - Text may exist but invisible
   - Fix: Increase font size to 24+

3. **RectTransform Too Small**
   - Text cut off by container size
   - Fix: Increase width/height of RectTransform

4. **TextMesh vs Text Component**
   ```csharp
   // Unity has two text systems:
   using UnityEngine.UI;
   public Text uiText;  // For Canvas UI ✓

   using TMPro;
   public TextMeshProUGUI tmpText;  // TextMeshPro
   ```

### Issue 5: Panel Won't Hide

**Symptoms**:
- Call SetActive(false), but panel still visible

**Possible Causes & Fixes**:

1. **Multiple Instances**
   ```csharp
   // Check Hierarchy
   - PauseUI (inactive)
   - PauseUI (1) (active) ← Duplicate!

   // Fix: Delete duplicate
   ```

2. **Wrong Reference**
   ```csharp
   public GameObject pauseUI;

   // In Start():
   Debug.Log(pauseUI.name);  // Verify correct panel

   // Fix: Reassign in Inspector
   ```

3. **Child Overriding Parent**
   ```csharp
   // Parent inactive, but child has script setting it active
   // Fix: Remove script or check active state
   ```

### Issue 6: Scene Won't Load

**Symptoms**:
- Call LoadScene(), but nothing happens

**Possible Causes & Fixes**:

1. **Scene Not in Build Settings**
   ```
   File → Build Settings → Scenes In Build
   - Must include all scenes
   - Check order (0, 1, 2...)

   Fix: Drag scene into list
   ```

2. **Wrong Scene Name**
   ```csharp
   SceneManager.LoadScene("menu");  ✗ (case-sensitive)
   SceneManager.LoadScene("Menu");  ✓

   // Fix: Use exact scene name
   ```

3. **SceneManager Not Imported**
   ```csharp
   using UnityEngine.SceneManagement;  // Required

   SceneManager.LoadScene("Menu");
   ```

### Issue 7: Time.timeScale Not Resetting

**Symptoms**:
- After pause, game stays frozen even after resume

**Possible Causes & Fixes**:

1. **OnDisable() Not Implemented**
   ```csharp
   // In MenuManager.cs
   private void OnDisable()
   {
       // Always reset time scale
       Time.timeScale = 1;
   }
   ```

2. **Multiple Scripts Setting timeScale**
   ```csharp
   // Script A:
   Time.timeScale = 0;

   // Script B:
   Time.timeScale = 0;

   // Resume only sets one back to 1
   // Fix: Use single manager for timeScale
   ```

### Issue 8: UI Stretched Incorrectly

**Symptoms**:
- UI looks correct in editor, stretched in game

**Possible Causes & Fixes**:

1. **Canvas Scaler Not Configured**
   ```
   Canvas Scaler component:
   - UI Scale Mode: Scale With Screen Size
   - Reference Resolution: 1920 x 1080
   - Match: 0.5 (balance width/height)
   ```

2. **Anchor Points Wrong**
   ```
   For center-positioned elements:
   - Anchor: Center
   - Position: (0, 0)

   For corner-positioned elements:
   - Anchor: Top Left (for top-left UI)
   - Position: (10, -10)
   ```

---

## Summary

The **UI System** provides visual feedback and user control through:

1. **Main Menu** (MainMenuHomeScene): Title screen, level select, settings
2. **In-Game HUD** (UI_UI): Health bars, coin/mana counters, wave progress
3. **Menu Manager** (MenuManager): Pause, victory, fail screens
4. **Health Bars** (HealthBarEnemyNew): Individual enemy health displays
5. **Loading System**: Async scene transitions with progress bars
6. **Global Values**: Persistent settings and save data

**Key Concepts**:
- **Singleton Pattern**: MenuManager.Instance for global access
- **Observer Pattern**: IListener for game event responses
- **Lerp for Smoothness**: Smooth health bar animations
- **Time.timeScale**: Pause/resume game logic
- **AsyncOperation**: Non-blocking scene loads

**Best Practices**:
- Hide panels by default (SetActive(false) in Awake)
- Use Lerp for smooth UI transitions
- Always reset Time.timeScale in OnDisable()
- Use GlobalValue for persistent data
- Implement IListener for event-driven UI updates

**Next Steps**:
- Read `05_Managers_Complete.md` to understand GameManager, LevelManager
- Read `10_How_To_Guides.md` for practical UI examples
- Read `02_Player_System_Complete.md` to see how player updates health bar

---

**Last Updated**: 2025
**File**: `Documents/04_UI_System_Complete.md`
