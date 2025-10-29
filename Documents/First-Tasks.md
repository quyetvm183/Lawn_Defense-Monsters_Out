# First Tasks: 10 Hands-On Exercises
## Quick Wins to Learn the Codebase

**Document Version**: 2.0 (Updated October 2025)
**Original**: Vietnamese (Version 1.0)
**Difficulty**: Beginner
**Time Required**: 1-2 hours per task
**Prerequisites**: Unity installed, project opened

---

## Introduction

This document provides **10 quick, practical tasks** designed to help you:
- ✅ Familiarize yourself with the codebase
- ✅ See immediate results from your changes
- ✅ Build confidence making modifications
- ✅ Learn the project structure hands-on

**Philosophy**: Learn by doing small, incremental changes with visible results.

### Before You Start

**Unity Basics Required**:
If you're completely new to Unity, read these first:
- **[00_Unity_Fundamentals.md](00_Unity_Fundamentals.md)** - Unity basics
- **[00_START_HERE.md](00_START_HERE.md)** - Project overview

**Git Best Practice**:
For each task, create a new branch:
```bash
git checkout -b task/descriptive-name
# Make your changes
git add .
git commit -m "Task 1: Adjusted enemy speed"
```

**Testing Protocol**:
- ✅ Make ONE small change
- ✅ Test in Unity Play Mode
- ✅ Verify the change works
- ✅ Commit before moving to next task

---

## Task 1: Run Game and Inspect Scene

**Goal**: Understand the game structure by running it

**Time**: 15-20 minutes

**Unity Basics Needed**:
- **Hierarchy**: Shows all GameObjects in scene
- **Inspector**: Shows properties of selected GameObject
- **Console**: Shows debug messages and errors
- **Play Mode**: Press ▶ button to run game

### Steps

**1. Open the Main Menu Scene**
```
File → Open Scene → Assets/_MonstersOut/Scenes/MainMenu.unity
```

**2. Explore the Hierarchy**
```
Hierarchy (left panel):
├── Canvas (UI elements)
├── EventSystem (handles UI input)
├── Main Camera
├── MenuManager (controls menus)
└── SoundManager (plays audio)
```

**3. Select GameManager**
- **Hierarchy** → Click "GameManager"
- **Inspector** → View components:
  ```
  [GameManager Script]
  ├── Game Levels: Array of level prefabs
  ├── Player: Reference to player prefab
  └── Listeners: List (empty at start)
  ```

**4. Press Play (▶ button)**
- Observe the main menu
- Click "PLAY" button
- Watch level load
- Observe Console for any errors

**5. While Playing, Select an Enemy**
- **Hierarchy** → Find "Enemy(Clone)"
- **Inspector** → View:
  ```
  Enemy Component:
  ├── Current Health: (watch it change when hit)
  ├── State: WALK / ATTACK / DEATH
  └── Speed: (controls movement)
  ```

**6. Stop Play Mode (⏹ button)**

### What You Learned

- ✅ Game starts from MainMenu scene
- ✅ GameManager controls game flow
- ✅ Enemies are spawned at runtime (Clone suffix)
- ✅ Inspector shows live values during Play Mode

### Common Issues

**Issue**: Can't find GameManager
**Solution**: Look in Hierarchy, not Project. It's in the scene.

**Issue**: Console shows red errors
**Solution**: This is normal if you haven't assigned all Inspector fields. Continue for now.

### Next Steps

Now you know how to run and inspect the game. Let's make some changes!

---

## Task 2: Modify Enemy Speed

**Goal**: Change enemy movement speed and see the effect

**Time**: 10-15 minutes

**Unity Basics Needed**:
- **Prefab**: Template for creating GameObjects
- **Inspector**: Edit component values
- **Play Mode**: Test changes

### Steps

**1. Find Enemy Prefab**
```
Project → Assets/_MonstersOut/Prefabs/Enemies/
Click: Goblin.prefab
```

**2. View in Inspector**
```
[SmartEnemyGrounded Component]
├── Speed: 3          ← Current value
└── Attack Distance: 1.5
```

**3. Change Speed Value**
```
Speed: 3  →  Speed: 6  (double the speed)
```

**4. Test the Change**
- Press **Play** (▶)
- Load a level
- **Watch**: Enemies now move twice as fast!

**5. Experiment**
Try different values:
- `Speed: 1` → Very slow (easy mode)
- `Speed: 10` → Very fast (hard mode)
- `Speed: 0` → Frozen (great for debugging!)

### Understanding the Code

The speed value is used in `SmartEnemyGrounded.cs`:

```csharp
// Line ~85 in SmartEnemyGrounded.cs
void Update()
{
    if (State == ENEMYSTATE.WALK)
    {
        // Move towards player
        velocity.x = direction * speed; // ← 'speed' from Inspector
        controller.Move(velocity * Time.deltaTime);
    }
}
```

**Explanation**:
- `speed` is multiplied by `direction` (-1 for left, 1 for right)
- `Time.deltaTime` makes it framerate-independent
- Higher speed = more units per second

### What You Learned

- ✅ Prefabs store default values for GameObjects
- ✅ Changes to prefabs affect all instances
- ✅ Inspector values directly control code behavior

### Common Issues

**Issue**: Changes don't appear in game
**Solution**:
1. Make sure you edited the **prefab** (blue icon), not a scene instance
2. Stop and restart Play Mode
3. Check if you saved the prefab (Ctrl+S)

**Issue**: Enemy moves too fast and falls through floor
**Solution**: Speed > 15 can cause physics issues. Keep under 10.

### Try This

Modify other enemy values:
- **Attack Distance**: How close before attacking
- **Max Health**: How much damage before death
- **Damage**: How much damage enemy deals

### Related Documentation

- **[03_Enemy_System_Complete.md](03_Enemy_System_Complete.md)** - Complete enemy documentation
- **[12_Visual_Reference.md](12_Visual_Reference.md)** - State machine diagrams

---

## Task 3: Create a New Enemy (Duplicate Method)

**Goal**: Create a custom enemy variant by duplicating an existing prefab

**Time**: 20-30 minutes

**Unity Basics Needed**:
- **Prefab Duplication**: Copy existing templates
- **Sprite**: Visual appearance
- **Animator**: Animation controller

### Steps

**1. Duplicate Goblin Prefab**
```
Project → Prefabs/Enemies/
Right-click: Goblin.prefab → Duplicate
Rename: FastGoblin
```

**2. Modify Stats**
Select `FastGoblin.prefab` → Inspector:
```
[Enemy Component]
Max Health: 50      → 30   (less health)
Damage: 10          → 15   (more damage)

[SmartEnemyGrounded Component]
Speed: 3            → 6    (faster movement)
```

**3. Change Visual Appearance (Optional)**
```
[Sprite Renderer Component]
Sprite: Goblin-Idle_00  → Skeleton-Idle_00
Color: White            → Red (tint)
```

**4. Test Your Custom Enemy**

Option A: **Replace in Wave**
```
Hierarchy → LevelEnemyManager
Inspector → Waves → Wave 0
Enemy Prefab: Goblin  →  FastGoblin
```

Option B: **Spawn Manually**
```
Hierarchy → Right-click → Create Empty
Drag: FastGoblin prefab into Hierarchy
Press Play → Watch your enemy!
```

**5. Compare Behaviors**
- Fast Goblin moves faster
- Dies quicker (less health)
- Deals more damage

### Understanding Prefab Workflow

```
Original Prefab (Goblin)
        │
        ▼
    Duplicate
        │
        ▼
  New Prefab (FastGoblin) ← Modify this
        │
        ▼
   Spawn in Game
```

**Key Concept**: Changes to FastGoblin don't affect Goblin.

### Advanced: Create Completely New Enemy

**If you want different animations**:

1. Create new Animator Controller:
   ```
   Project → Animations → Right-click
   → Create → Animator Controller
   Name: FastGoblin_Controller
   ```

2. Add animation states (copy from Goblin_Controller)

3. Assign to prefab:
   ```
   FastGoblin → Animator Component
   Controller: FastGoblin_Controller
   ```

### What You Learned

- ✅ Duplication is fastest way to create variants
- ✅ You can mix-and-match sprites and stats
- ✅ Prefabs can be tested by dragging to Hierarchy

### Common Issues

**Issue**: Duplicate enemy uses wrong sprite
**Solution**: Check Sprite Renderer component, assign correct sprite

**Issue**: Animations don't play
**Solution**: Make sure Animator Controller is assigned and has states

**Issue**: Enemy doesn't spawn in level
**Solution**: Assign to LevelEnemyManager wave settings

### Try This

Create these enemy variants:
- **Tank**: High health (200), slow (speed: 1.5)
- **Assassin**: Low health (20), very fast (speed: 8)
- **Boss**: Huge health (500), medium speed (4), high damage (30)

### Related Documentation

- **[10_How_To_Guides.md](10_How_To_Guides.md)** § Guide 1 - Creating new enemy types
- **[03_Enemy_System_Complete.md](03_Enemy_System_Complete.md)** - Enemy system details

---

## Task 4: Increase Projectile Damage

**Goal**: Make player arrows deal more damage

**Time**: 10 minutes

**Unity Basics Needed**:
- **Script Editing**: Open and modify C# code
- **Component Reference**: Find scripts on GameObjects

### Steps

**1. Find Arrow Prefab**
```
Project → Prefabs/Projectiles/
Click: Arrow.prefab
```

**2. View Damage Script**
```
Inspector → ArrowProjectile (Script)
Damage: 10  ← Current damage
```

**3. Increase Damage**
```
Damage: 10  →  Damage: 25
```

**4. Test**
- Press Play
- Shoot enemies
- **Notice**: They die faster!

### Understanding the Damage System

**Arrow.cs** (simplified):
```csharp
public class ArrowProjectile : MonoBehaviour
{
    public float damage = 10f;  // ← Inspector value

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if hit an enemy
        ICanTakeDamage target = other.GetComponent<ICanTakeDamage>();

        if (target != null)
        {
            // Deal damage
            target.TakeDamage(damage, Vector2.zero, gameObject);

            // Destroy arrow
            Destroy(gameObject);
        }
    }
}
```

**Flow**:
```
Arrow spawns → Flies through air → Hits enemy →
OnTriggerEnter2D() called → TakeDamage(25) → Enemy loses 25 HP
```

### Code Explanation (Line by Line)

```csharp
public float damage = 10f;
```
- `public` = visible in Inspector
- `float` = decimal number (10.5 allowed)
- `damage` = variable name
- `= 10f` = default value (10.0)

```csharp
void OnTriggerEnter2D(Collider2D other)
```
- Called automatically when arrow's collider touches another collider
- `other` = the thing we hit

```csharp
ICanTakeDamage target = other.GetComponent<ICanTakeDamage>();
```
- Look for damage interface on hit object
- Returns null if object can't take damage (like walls)

```csharp
if (target != null)
```
- Only proceed if we hit something damageable

```csharp
target.TakeDamage(damage, Vector2.zero, gameObject);
```
- Call TakeDamage method
- `damage` = how much (25 in our case)
- `Vector2.zero` = no knockback force
- `gameObject` = who dealt damage (the arrow)

### What You Learned

- ✅ Inspector values are code variables
- ✅ Damage is dealt via `ICanTakeDamage` interface
- ✅ Collision triggers code execution

### Common Issues

**Issue**: Damage doesn't change
**Solution**: Make sure you edited the **Arrow prefab**, not an instance in scene

**Issue**: Arrows pass through enemies
**Solution**:
1. Check Arrow has **Collider2D** with **Is Trigger** checked
2. Check Enemy has **Collider2D**
3. Verify **Layer Collision Matrix** (Edit → Project Settings → Physics 2D)

### Try This

Modify other projectile properties:
- **Speed**: How fast arrow flies
- **Lifetime**: How long before arrow despawns
- **Pierce**: Can arrow hit multiple enemies?

### Related Documentation

- **[13_Code_Examples.md](13_Code_Examples.md)** § Example 7 - Damage Dealer
- **[11_Troubleshooting.md](11_Troubleshooting.md)** § Problem 8 - Damage not applying

---

## Task 5: Add Character Upgrade Level

**Goal**: Add a new upgrade tier to player character

**Time**: 15-20 minutes

**Unity Basics Needed**:
- **Serializable Classes**: Data structures shown in Inspector
- **Arrays**: Lists of values

### Steps

**1. Find Player Prefab**
```
Project → Prefabs/Player/
Click: Player.prefab
```

**2. Locate Upgrade Component**
```
Inspector → Find: UpgradedCharacterParameter
```

**3. View Current Upgrades**
```
[Upgrade Steps]
Size: 3  ← Currently 3 levels

Element 0: (Level 1)
├── Cost: 100
├── Health: 100
└── Damage: 10

Element 1: (Level 2)
├── Cost: 250
├── Health: 150
└── Damage: 15

Element 2: (Level 3)
├── Cost: 500
├── Health: 200
└── Damage: 20
```

**4. Add New Upgrade Level**
```
Size: 3  →  Size: 4  (adds Element 3)
```

**5. Configure Level 4**
```
Element 3: (Level 4)
├── Cost: 1000       ← Expensive!
├── Health: 300      ← Max health
└── Damage: 30       ← Strong attacks
```

**6. Test in Shop**
- Play game
- Open shop
- Buy upgrades
- Reach Level 4!

### Understanding Upgrade System

**UpgradeStep** class structure:
```csharp
[System.Serializable]
public class UpgradeStep
{
    public int cost;       // ← How much to buy
    public float health;   // ← Max HP at this level
    public float damage;   // ← Attack damage
    // ... other stats
}
```

**Upgrade process**:
```
Player at Level 1 (100 HP, 10 Damage)
        │
        ├─ Spend 250 coins in shop
        │
        ▼
Player at Level 2 (150 HP, 15 Damage)
        │
        ├─ Spend 500 coins
        │
        ▼
Player at Level 3 (200 HP, 20 Damage)
        │
        ├─ Spend 1000 coins
        │
        ▼
Player at Level 4 (300 HP, 30 Damage)  ← Your new level!
```

### Code Behind Upgrades

**When player buys upgrade** (simplified):
```csharp
public void UpgradeCharacter()
{
    // Get current upgrade level
    int currentLevel = GlobalValue.CharacterLevel;

    // Get next upgrade stats
    UpgradeStep nextStep = upgradeSteps[currentLevel];

    // Check if enough coins
    if (GlobalValue.SavedCoins >= nextStep.cost)
    {
        // Deduct coins
        GlobalValue.SavedCoins -= nextStep.cost;

        // Apply new stats
        maxHealth = nextStep.health;
        damage = nextStep.damage;

        // Increase level
        GlobalValue.CharacterLevel++;

        // Save
        PlayerPrefs.Save();
    }
}
```

### What You Learned

- ✅ Arrays store multiple values (upgrade tiers)
- ✅ `[Serializable]` makes classes visible in Inspector
- ✅ Upgrades are permanent (saved in PlayerPrefs)

### Common Issues

**Issue**: Can't see Level 4 in shop
**Solution**: You need enough coins. Use PlayerPrefs to add coins:
```
Unity Menu → Window → PlayerPrefs Editor (if installed)
Or manually: PlayerPrefs.SetInt("coins", 10000);
```

**Issue**: Stats don't change after upgrade
**Solution**: Make sure Player script reads from upgradeSteps array

### Try This

Create a progression system:
```
Level 1: Beginner (100 HP, 10 DMG) - Cost: FREE
Level 2: Apprentice (150 HP, 15 DMG) - Cost: 250
Level 3: Warrior (200 HP, 20 DMG) - Cost: 500
Level 4: Knight (300 HP, 30 DMG) - Cost: 1000
Level 5: Legend (500 HP, 50 DMG) - Cost: 5000
```

### Related Documentation

- **[13_Code_Examples.md](13_Code_Examples.md)** § Example 17 - Save/Load Systems
- **[02_Player_System_Complete.md](02_Player_System_Complete.md)** § Upgrade System

---

## Task 6: Add Animation Event

**Goal**: Trigger code from animation timeline

**Time**: 20-25 minutes

**Unity Basics Needed**:
- **Animation Window**: Edit animation clips
- **Animation Events**: Call functions at specific frames

### Steps

**1. Open Enemy Animator**
```
Project → Animations/Enemies/
Double-click: Goblin_Controller
```

**2. Open Animation Window**
```
Window → Animation → Animation
```

**3. Select Attack Animation**
```
Animation Window → Dropdown → "Goblin-Attack"
```

**4. Add Animation Event**
```
Timeline → Click frame 10 (mid-swing)
Button: Add Event (or white marker line)
```

**5. Assign Function**
```
Function: DealDamage()  ← Method from Enemy script
```

**6. Add Debug Log**

Open `Assets/_MonstersOut/Scripts/Enemy/EnemyMeleeAttack.cs`:

```csharp
public void DealDamage()
{
    // Add this line at the top
    Debug.Log(name + " dealing damage at frame 10!");

    // Existing damage code...
    if (AttackTargetPlayer && player != null)
    {
        player.TakeDamage(damage, Vector2.zero, gameObject);
    }
}
```

**7. Test**
- Play game
- Wait for enemy to attack
- **Console** shows: "Goblin(Clone) dealing damage at frame 10!"

### Understanding Animation Events

**Animation Timeline**:
```
Attack Animation (30 frames, 1 second)

Frame:  0     5     10    15    20    25    30
        │     │     │     │     │     │     │
        ▼     ▼     ▼     ▼     ▼     ▼     ▼
Sprite: idle  windup SWING  hit  follow  follow idle
                      ▲
                      │
                 Event: DealDamage()
                 (damage applied here!)
```

**Why use events?**
- Sync code with animation
- Damage at correct visual moment
- Sounds play when foot hits ground
- Particles spawn when sword swings

### Code Explanation

```csharp
// This method is called by Animation Event
public void DealDamage()
{
    Debug.Log(name + " dealing damage!");

    if (AttackTargetPlayer && player != null)
    {
        player.TakeDamage(damage, Vector2.zero, gameObject);
    }
}
```

**Flow**:
```
Enemy starts attack animation →
Frame 10 reached →
Unity calls DealDamage() automatically →
Code runs →
Player takes damage
```

### What You Learned

- ✅ Animation can trigger code at specific frames
- ✅ Events sync visuals with gameplay
- ✅ Methods must be public to be called by events

### Common Issues

**Issue**: Function not appearing in dropdown
**Solution**:
1. Method must be `public`
2. Method must have no parameters (or only specific types)
3. Script must be attached to same GameObject as Animator

**Issue**: Event fires too early/late
**Solution**: Drag event marker to different frame

**Issue**: Event doesn't fire
**Solution**:
1. Check Animator is playing the animation
2. Verify script is attached to GameObject
3. Make sure method name matches exactly

### Try This

Add more animation events:
- **Footstep Sound**: Play sound when foot touches ground
- **Spawn Effect**: Create particles at attack moment
- **Shake Camera**: Shake on heavy attack land

### Related Documentation

- **[12_Visual_Reference.md](12_Visual_Reference.md)** § Animation System
- **[03_Enemy_System_Complete.md](03_Enemy_System_Complete.md)** § Attack System

---

## Task 7: Test Shop System

**Goal**: Understand shop mechanics by testing purchases

**Time**: 15-20 minutes

**Unity Basics Needed**:
- **UI Navigation**: Find and interact with UI
- **PlayerPrefs**: Save data system

### Steps

**1. Open Shop Scene**
```
File → Open Scene → Scenes/Shop.unity
(Or play game and navigate to shop)
```

**2. Give Yourself Coins**

**Method A**: Via Inspector (during Play Mode)
```
Play Mode → Hierarchy → Find object with GlobalValue
Inspector → Saved Coins: 0  →  10000
```

**Method B**: Via PlayerPrefs (before Play Mode)

Create this temporary script:
```csharp
using UnityEngine;

public class CheatCoins : MonoBehaviour
{
    void Start()
    {
        PlayerPrefs.SetInt("coins", 10000);
        PlayerPrefs.Save();
        Debug.Log("Added 10000 coins!");
    }
}
```

Attach to any GameObject, play once, then remove script.

**3. Buy a Character**
```
Shop UI → Click character slot
Button: "Buy" (cost shown)
Observe: Coins deduct, character unlocked
```

**4. Observe Floating Text**
```
When buying: "Purchased!" text floats up
When insufficient: "Not enough coins!" shows
```

**5. Test Character in Game**
```
Return to main menu
Select unlocked character
Play level
Character loads with purchased stats!
```

### Understanding Shop System

**Shop UI Hierarchy**:
```
Canvas
└── ShopPanel
    ├── CharacterSlot1
    │   ├── Character Image
    │   ├── Price Text
    │   └── Buy Button
    │       └── OnClick: ShopManager.BuyCharacter(0)
    │
    ├── CharacterSlot2
    └── CoinDisplay
        └── Text: Shows GlobalValue.SavedCoins
```

**Purchase Flow**:
```
Player clicks "Buy" →
ShopManager.BuyCharacter(characterID) called →
Check: coins >= price? →
  YES → Deduct coins
     → Set character as owned
     → Save PlayerPrefs
     → Show "Purchased!" text
  NO  → Show "Not enough coins!" text
```

### Code Behind Shop (Simplified)

```csharp
public class ShopManager : MonoBehaviour
{
    public void BuyCharacter(int characterID)
    {
        // Get character data
        CharacterData character = characters[characterID];

        // Check if enough coins
        if (GlobalValue.SavedCoins >= character.price)
        {
            // Deduct coins
            GlobalValue.SavedCoins -= character.price;

            // Mark as owned
            PlayerPrefs.SetInt("Character_" + characterID + "_Owned", 1);

            // Save
            PlayerPrefs.Save();

            // Show feedback
            FloatingTextManager.Instance.Show("Purchased!", Color.green);
        }
        else
        {
            // Not enough
            FloatingTextManager.Instance.Show("Not enough coins!", Color.red);
        }
    }
}
```

### What You Learned

- ✅ Shop uses PlayerPrefs for save data
- ✅ Floating text provides user feedback
- ✅ UI buttons call script methods via OnClick events

### Common Issues

**Issue**: Coins don't persist
**Solution**: Make sure `PlayerPrefs.Save()` is called

**Issue**: Purchased character not showing in game
**Solution**: Check character selection logic loads owned characters

**Issue**: Buy button doesn't work
**Solution**: Verify OnClick event is assigned in Inspector

### Try This

Modify shop behavior:
- **Discounts**: Reduce prices by 50%
- **Double Coins**: Give 2x coins for completing levels
- **New Currency**: Add gems as premium currency

### Related Documentation

- **[04_UI_System_Complete.md](04_UI_System_Complete.md)** § Shop System
- **[13_Code_Examples.md](13_Code_Examples.md)** § Example 17 - Save/Load

---

## Task 8: Add Debug Logs to Wave Spawner

**Goal**: Understand wave system by adding logging

**Time**: 10 minutes

**Unity Basics Needed**:
- **Debug.Log**: Print messages to Console
- **Coroutines**: Time-based code execution

### Steps

**1. Open LevelEnemyManager Script**
```
Assets/_MonstersOut/Scripts/Managers/LevelEnemyManager.cs
```

**2. Find SpawnEnemyCo() Method**

Around line 60-80, find this method.

**3. Add Debug Logs**

```csharp
IEnumerator SpawnEnemyCo()
{
    Debug.Log("=== WAVE SPAWNING STARTED ===");

    int totalSpawned = 0;

    foreach (var wave in waves)
    {
        Debug.Log("Starting wave: " + wave.waveName +
                 " with " + wave.enemyCount + " enemies");

        for (int i = 0; i < wave.enemyCount; i++)
        {
            // Spawn enemy
            SpawnEnemy(wave.enemyPrefab);
            totalSpawned++;

            Debug.Log("Spawned enemy #" + totalSpawned +
                     " (Wave: " + wave.waveName + ")");

            yield return new WaitForSeconds(wave.spawnInterval);
        }

        Debug.Log("Wave " + wave.waveName + " complete!");
        yield return new WaitForSeconds(wave.delayToNextWave);
    }

    Debug.Log("=== ALL WAVES COMPLETE! Total spawned: " + totalSpawned + " ===");
}
```

**4. Test**
- Play game
- Load level
- **Watch Console**: Shows spawn progress!

**Output Example**:
```
=== WAVE SPAWNING STARTED ===
Starting wave: Wave 1 with 5 enemies
Spawned enemy #1 (Wave: Wave 1)
Spawned enemy #2 (Wave: Wave 1)
Spawned enemy #3 (Wave: Wave 1)
Spawned enemy #4 (Wave: Wave 1)
Spawned enemy #5 (Wave: Wave 1)
Wave Wave 1 complete!
Starting wave: Wave 2 with 8 enemies
...
```

### Understanding Wave System

**Wave Configuration**:
```csharp
[System.Serializable]
public class Wave
{
    public string waveName;           // Display name
    public GameObject enemyPrefab;    // What to spawn
    public int enemyCount;            // How many
    public float spawnInterval;       // Time between each (1-3 sec)
    public float delayToNextWave;     // Break between waves (5-10 sec)
}
```

**Timeline Visualization**:
```
Time: 0s ─────5s ─────10s ────15s ────20s ────25s ────30s ───→

Wave 1: Enemy Enemy Enemy Enemy Enemy ──(wait 5s)──→
                  ▲
                  spawnInterval (1s between each)

Wave 2: Enemy Enemy Enemy ──(wait 5s)──→
                  ▲
                  spawnInterval (2s)
```

### Code Explanation

```csharp
foreach (var wave in waves)
```
- Loop through all waves in order

```csharp
for (int i = 0; i < wave.enemyCount; i++)
```
- Spawn `enemyCount` enemies per wave

```csharp
yield return new WaitForSeconds(wave.spawnInterval);
```
- Wait before spawning next enemy
- `yield return` pauses coroutine

```csharp
Debug.Log("Message: " + variable);
```
- Print to Console
- Use `+` to combine strings and numbers

### What You Learned

- ✅ Debug.Log helps understand code flow
- ✅ Waves spawn enemies sequentially with delays
- ✅ Coroutines handle time-based spawning

### Common Issues

**Issue**: Console flooded with messages
**Solution**: Remove Debug.Log() calls after understanding system

**Issue**: Logs don't appear
**Solution**:
1. Check Console is visible (Window → General → Console)
2. Make sure code is actually running (add breakpoint)

### Try This

Log more information:
- **Enemy Health**: Log when enemy takes damage
- **Player Actions**: Log when player shoots
- **Victory/Defeat**: Log game end conditions

### Related Documentation

- **[05_Managers_Complete.md](05_Managers_Complete.md)** § LevelEnemyManager
- **[11_Troubleshooting.md](11_Troubleshooting.md)** § Debug Techniques

---

## Task 9: Modify Camera Movement Limits

**Goal**: Change camera boundaries

**Time**: 10 minutes

**Unity Basics Needed**:
- **Camera**: What player sees
- **Transform**: Position in world

### Steps

**1. Find Main Camera**
```
Hierarchy → Main Camera
```

**2. View Camera Controller**
```
Inspector → CameraController (Script)
├── Limit Left: -5     ← Left boundary
├── Limit Right: 5     ← Right boundary
└── Smooth Speed: 3    ← Follow smoothness
```

**3. Expand Camera Range**
```
Limit Left: -5   →  -10   (can see further left)
Limit Right: 5   →  15    (can see further right)
```

**4. Test**
- Play game
- Drag screen left/right (mouse drag or touch)
- Camera moves within new limits!

### Understanding Camera System

**Camera Clamping**:
```
                Limit Left          Limit Right
                    │                   │
                    ▼                   ▼
World: ────────────[-10]═══════════[15]────────────
                    ◄─────────────►
                    Camera can move
                    within this range
```

**Code Behind Camera Movement**:
```csharp
void Update()
{
    // Get desired position (from player or drag)
    float desiredX = GetDesiredCameraX();

    // Clamp to limits
    float clampedX = Mathf.Clamp(
        desiredX,
        limitLeft,     // Can't go further left
        limitRight     // Can't go further right
    );

    // Smooth movement
    Vector3 targetPos = new Vector3(clampedX, transform.position.y, -10);
    transform.position = Vector3.Lerp(
        transform.position,
        targetPos,
        smoothSpeed * Time.deltaTime
    );
}
```

### Code Explanation

```csharp
Mathf.Clamp(value, min, max)
```
- Restricts value between min and max
- Example: `Clamp(12, -10, 15)` → 12 (within range)
- Example: `Clamp(-20, -10, 15)` → -10 (clamped to min)

```csharp
Vector3.Lerp(current, target, speed * Time.deltaTime)
```
- Smoothly move from current to target
- `speed * Time.deltaTime` = gradual movement
- Higher speed = faster following

### What You Learned

- ✅ Camera limits prevent seeing outside level
- ✅ Mathf.Clamp restricts values to range
- ✅ Lerp creates smooth camera movement

### Common Issues

**Issue**: Camera shows empty space
**Solution**: Set limits to match level size

**Issue**: Camera too jerky
**Solution**: Increase `smoothSpeed` (try 5-10)

**Issue**: Can't drag camera
**Solution**: Verify input handling code is active

### Try This

Advanced camera behaviors:
- **Follow Player**: Make camera track player position
- **Zoom Control**: Change Camera.orthographicSize
- **Shake Effect**: Add random offset on damage

### Related Documentation

- **[13_Code_Examples.md](13_Code_Examples.md)** § Camera Systems
- **[12_Visual_Reference.md](12_Visual_Reference.md)** § Scene Structure

---

## Task 10: Implement Double-Shot Power-Up

**Goal**: Create a feature from scratch - player shoots two arrows

**Time**: 30-40 minutes

**Unity Basics Needed**:
- **Script Modification**: Edit existing C# code
- **Quaternion**: Rotation system
- **Boolean Flags**: True/false variables

### Steps

**1. Open Player Script**
```
Assets/_MonstersOut/Scripts/Player/Player_Archer.cs
```

**2. Add Double Shot Variable**

Around line 20-30, add:
```csharp
[Header("Power-Ups")]
public bool hasDoubleShot = false;  // ← Enable in Inspector for testing
public float doubleShotAngle = 15f; // ← Spread angle
```

**3. Find Shoot() Method**

Around line 100-150, locate the shooting code.

**4. Modify Shoot() Method**

```csharp
void Shoot()
{
    if (!allowMoveByPlayer)
        return;

    if (Time.time - lastShootTime < shootRate)
        return; // Cooldown not ready

    lastShootTime = Time.time;

    if (hasDoubleShot)
    {
        // Shoot two arrows with angle spread
        ShootArrowWithAngle(-doubleShotAngle); // Left arrow
        ShootArrowWithAngle(doubleShotAngle);  // Right arrow
    }
    else
    {
        // Normal single shot
        ShootArrowWithAngle(0); // Straight
    }

    // Play sound
    SoundManager.PlaySfx(SoundManager.Instance.soundShoot);

    // Play animation
    animator.SetTrigger("Shoot");
}

// New helper method
void ShootArrowWithAngle(float angleOffset)
{
    GameObject arrow = Instantiate(
        arrowPrefab,
        shootPoint.position,
        Quaternion.identity
    );

    // Calculate direction with angle
    Vector2 baseDirection = transform.localScale.x > 0
        ? Vector2.right
        : Vector2.left;

    // Apply angle offset
    float angleRad = angleOffset * Mathf.Deg2Rad;
    float cos = Mathf.Cos(angleRad);
    float sin = Mathf.Sin(angleRad);

    Vector2 rotatedDirection = new Vector2(
        baseDirection.x * cos - baseDirection.y * sin,
        baseDirection.x * sin + baseDirection.y * cos
    );

    // Set arrow direction
    ArrowScript arrowScript = arrow.GetComponent<ArrowScript>();
    if (arrowScript != null)
    {
        arrowScript.Initialize(rotatedDirection);
    }
}
```

**5. Test**

```
Inspector → Player GameObject
→ Player_Archer Component
→ Has Double Shot: ☑ (check this!)
```

Play game → Shoot → Two arrows spread out!

### Visual Result

**Single Shot**:
```
        →
Player ───→ Enemy
```

**Double Shot**:
```
       ↗
Player ─→  Enemy
       ↘
```

### Understanding the Math

**Angle Rotation Formula**:
```csharp
// Rotate vector by angle
rotatedX = x * cos(angle) - y * sin(angle)
rotatedY = x * sin(angle) + y * cos(angle)
```

**Example**: Rotate (1, 0) by 15°:
```
cos(15°) = 0.966
sin(15°) = 0.259

rotatedX = 1 * 0.966 - 0 * 0.259 = 0.966
rotatedY = 1 * 0.259 + 0 * 0.966 = 0.259

Result: (0.966, 0.259) → Angled 15° upward
```

### What You Learned

- ✅ Created new feature from existing code
- ✅ Used trigonometry for angle calculations
- ✅ Boolean flags enable/disable features

### Common Issues

**Issue**: Arrows don't spread
**Solution**: Increase `doubleShotAngle` (try 30°)

**Issue**: Arrows shoot wrong direction
**Solution**: Check `baseDirection` calculation based on player facing

**Issue**: Only one arrow spawns
**Solution**: Make sure `hasDoubleShot` is checked in Inspector

### Try This

Extend this feature:
- **Triple Shot**: Shoot 3 arrows (left, center, right)
- **Powerup Duration**: Enable for 10 seconds, then disable
- **Rapid Fire**: Reduce `shootRate` when power-up active

### Advanced: Make it Collectible

**1. Create Powerup Prefab**:
```
GameObject → 3D Object → Cube
Add: BoxCollider2D (Is Trigger: ✓)
Add: Script: DoubleShotPowerup.cs
```

**2. Powerup Script**:
```csharp
public class DoubleShotPowerup : MonoBehaviour
{
    public float duration = 10f;

    void OnTriggerEnter2D(Collider2D other)
    {
        Player_Archer player = other.GetComponent<Player_Archer>();

        if (player != null)
        {
            player.ActivateDoubleShot(duration);
            Destroy(gameObject);
        }
    }
}
```

**3. Add to Player**:
```csharp
public void ActivateDoubleShot(float duration)
{
    StartCoroutine(DoubleShotCo(duration));
}

IEnumerator DoubleShotCo(float duration)
{
    hasDoubleShot = true;
    Debug.Log("Double shot activated!");

    yield return new WaitForSeconds(duration);

    hasDoubleShot = false;
    Debug.Log("Double shot expired!");
}
```

### Related Documentation

- **[10_How_To_Guides.md](10_How_To_Guides.md)** § Guide 6 - Power-Up Items
- **[02_Player_System_Complete.md](02_Player_System_Complete.md)** § Shooting System
- **[13_Code_Examples.md](13_Code_Examples.md)** § Math Utilities

---

## Summary & Next Steps

### What You Accomplished

✅ **Task 1**: Explored game structure and hierarchy
✅ **Task 2**: Modified enemy speed (prefab values)
✅ **Task 3**: Created custom enemy variant
✅ **Task 4**: Increased projectile damage
✅ **Task 5**: Added character upgrade tier
✅ **Task 6**: Triggered code from animations
✅ **Task 7**: Tested shop and save system
✅ **Task 8**: Added debug logging to spawner
✅ **Task 9**: Modified camera boundaries
✅ **Task 10**: Implemented double-shot feature

### Skills Gained

**Unity Editor**:
- Navigate Hierarchy, Inspector, Project
- Modify prefab values
- Test in Play Mode
- Use Console for debugging

**Code Understanding**:
- Read and understand C# scripts
- Add debug logs
- Modify method behavior
- Create new features

**Game Systems**:
- Enemy AI and spawning
- Player shooting mechanics
- Shop and save system
- Animation events
- Camera control

### Recommended Next Steps

**Week 1**: Review what you learned
1. Go through tasks again without looking at instructions
2. Modify values experimentally
3. Break something intentionally, then fix it

**Week 2**: Expand features
1. Combine multiple tasks (fast enemy with high damage)
2. Create your own task ideas
3. Share with team/community

**Week 3**: Study systems in depth
1. Read **[02_Player_System_Complete.md](02_Player_System_Complete.md)**
2. Read **[03_Enemy_System_Complete.md](03_Enemy_System_Complete.md)**
3. Read **[05_Managers_Complete.md](05_Managers_Complete.md)**

**Week 4**: Build something new
1. Follow **[10_How_To_Guides.md](10_How_To_Guides.md)**
2. Implement a guide from scratch
3. Document your process

### Git Best Practices Review

**After completing these tasks**:

```bash
# Review your changes
git status
git diff

# Create logical commits
git add Assets/Prefabs/Enemies/FastGoblin.prefab
git commit -m "Task 3: Created FastGoblin enemy variant

- Duplicated Goblin prefab
- Increased speed to 6
- Reduced health to 30
- Increased damage to 15"

# Push your work
git push origin task/fast-goblin

# Merge when ready
git checkout main
git merge task/fast-goblin
```

### Troubleshooting Tips

**If you get stuck**:
1. **Read error messages** carefully in Console
2. **Check this guide** for common issues
3. **Consult documentation**:
   - [11_Troubleshooting.md](11_Troubleshooting.md) - Common problems
   - [99_Glossary.md](99_Glossary.md) - Term definitions
4. **Ask for help** with specific error messages

**Before asking for help**:
- ✅ What were you trying to do?
- ✅ What did you expect to happen?
- ✅ What actually happened?
- ✅ What have you tried already?

### Celebrate Your Progress!

You've completed 10 hands-on tasks and learned:
- Unity Editor basics
- Code modification
- Game system understanding
- Debugging techniques
- Git workflow

**You're ready for more advanced topics!**

---

## Additional Resources

### Unity Learning
- **[00_Unity_Fundamentals.md](00_Unity_Fundamentals.md)** - Complete Unity basics
- Unity Learn: https://learn.unity.com/
- Unity Manual: https://docs.unity3d.com/Manual/

### Project Documentation
- **[00_START_HERE.md](00_START_HERE.md)** - Main entry point
- **[01_Project_Architecture.md](01_Project_Architecture.md)** - Project structure
- **[99_Glossary.md](99_Glossary.md)** - Term definitions

### Practical Guides
- **[10_How_To_Guides.md](10_How_To_Guides.md)** - Step-by-step tutorials
- **[11_Troubleshooting.md](11_Troubleshooting.md)** - Fix problems
- **[13_Code_Examples.md](13_Code_Examples.md)** - Copy-paste code

### Reference
- **[12_Visual_Reference.md](12_Visual_Reference.md)** - Diagrams & visuals
- **[project-analysis.md](project-analysis.md)** - Technical deep dive

---

**Ready for more?** → [10_How_To_Guides.md](10_How_To_Guides.md)

**Need help?** → [11_Troubleshooting.md](11_Troubleshooting.md)

**Good luck with your game development journey! 🎮**

---

<p align="center">
<strong>Lawn Defense: Monsters Out</strong><br>
First Tasks - Hands-On Learning<br>
Version 2.0 • October 2025
</p>