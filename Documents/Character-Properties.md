# Character Properties and Upgrade System

**Complete Guide to UpgradedCharacterParameter**

---

## Table of Contents

1. [Overview](#overview)
2. [Unity Basics You Need](#unity-basics-you-need)
3. [The UpgradedCharacterParameter System](#the-upgradedcharacterparameter-system)
4. [Understanding the Code](#understanding-the-code)
5. [How Upgrades Work](#how-upgrades-work)
6. [Creating a New Character](#creating-a-new-character)
7. [Practical Examples](#practical-examples)
8. [Visual Flow Diagrams](#visual-flow-diagrams)
9. [Troubleshooting](#troubleshooting)
10. [Cross-References](#cross-references)

---

## Overview

**Purpose**: This document explains how character properties are stored, how the upgrade system works, and how to create new playable characters in the game.

**What You'll Learn**:
- How character stats (health, damage, critical chance) are stored and retrieved
- How the upgrade system uses PlayerPrefs for persistence
- How to create new characters with custom stats and abilities
- How to debug and fix common character-related issues

**Key File**: `Assets/_MonstersOut/Scripts/Player/UpgradedCharacterParameter.cs`

**Time to Read**: 15-20 minutes
**Difficulty**: Intermediate

---

## Unity Basics You Need

### What is PlayerPrefs?

**PlayerPrefs** is Unity's built-in system for saving simple data between game sessions.

**Real-World Analogy**: Think of PlayerPrefs like browser cookies - small pieces of data that persist even after you close the application.

**How it works**:
```csharp
// Save data
PlayerPrefs.SetInt("Score", 100);           // Save integer
PlayerPrefs.SetFloat("Volume", 0.8f);       // Save decimal
PlayerPrefs.SetString("PlayerName", "Bob"); // Save text

// Load data (with default value if key doesn't exist)
int score = PlayerPrefs.GetInt("Score", 0);           // Returns 100, or 0 if not found
float volume = PlayerPrefs.GetFloat("Volume", 1.0f);  // Returns 0.8, or 1.0 if not found
string name = PlayerPrefs.GetString("PlayerName", ""); // Returns "Bob", or "" if not found

// Delete data
PlayerPrefs.DeleteKey("Score");  // Remove specific key
PlayerPrefs.DeleteAll();         // Remove ALL saved data (dangerous!)
```

**Where is data stored?**
- **Windows**: Registry at `HKCU\Software\[CompanyName]\[ProductName]`
- **Mac**: `~/Library/Preferences/com.[CompanyName].[ProductName].plist`
- **Linux**: `~/.config/unity3d/[CompanyName]/[ProductName]/prefs`

**Limitations**:
- Only stores simple types (int, float, string)
- Not encrypted - don't store sensitive data
- Limited size - use JSON or databases for large data

### What is [System.Serializable]?

This attribute tells Unity to show a class in the Inspector.

```csharp
[System.Serializable]
public class UpgradeStep
{
    public int price;        // ← These fields will appear
    public int healthStep;   //   in the Unity Inspector
}
```

**Without [System.Serializable]**: The class would work in code but wouldn't appear in the Inspector for editing.

### What are Properties (get/set)?

**Properties** provide controlled access to private data.

```csharp
// Without property (direct access)
public int health = 100;
// Anyone can change: health = -999; ← No validation!

// With property (controlled access)
private int _health = 100;
public int Health
{
    get { return _health; }
    set { _health = Mathf.Max(0, value); } // ← Prevents negative health
}
```

In this project, properties are used to automatically save/load from PlayerPrefs:

```csharp
public int UpgradeHealth
{
    get { return PlayerPrefs.GetInt(ID + "upgradeHealth", defaultHealth); }
    set { PlayerPrefs.SetInt(ID + "upgradeHealth", value); }
}

// Usage:
int currentHealth = UpgradeHealth;  // ← Automatically loads from PlayerPrefs
UpgradeHealth = 150;                // ← Automatically saves to PlayerPrefs
```

---

## The UpgradedCharacterParameter System

### What is it?

**UpgradedCharacterParameter** is a component attached to each playable character that:
1. Stores base stats (health, damage, critical chance)
2. Defines how the character can be upgraded
3. Saves/loads upgrade progress using PlayerPrefs
4. Determines character abilities (Melee, Range, or Healer)

### Core Concepts

**1. Character ID** - Unique identifier for PlayerPrefs keys
```csharp
public string ID = "archer_01"; // ← Must be unique per character!
```

Each character needs a unique ID to avoid conflicts. If two characters share an ID, their upgrade data will overwrite each other.

**2. Ability Types** - Determines character behavior
```csharp
public enum Abitity { Melee, Range, Healer }
public Abitity playerAbility; // Set in Inspector
```
- **Melee**: Close-range fighter (sword, hammer)
- **Range**: Long-range attacker (bow, gun)
- **Healer**: Support character (restores health)

**3. Upgrade Steps** - Defines upgrade progression
```csharp
public UpgradeStep[] UpgradeSteps;
```

Each upgrade step defines:
- **price**: Cost in game currency
- **healthStep**: How much health increases
- **meleeDamageStep**: How much melee damage increases
- **rangeDamageStep**: How much range damage increases
- **criticalStep**: How much critical chance increases

**4. Default Stats** - Starting values before upgrades
```csharp
public int defaultHealth = 100;
public int meleeDamage = 100;
public int rangeDamage = 100;
public int criticalDamagePercent = 10;
```

---

## Understanding the Code

Let's break down the **UpgradedCharacterParameter.cs** file line by line.

### Class Structure

```csharp
namespace RGame
{
    [System.Serializable]
    public class UpgradeStep
    {
        public int price;              // Cost to buy this upgrade
        public int healthStep;         // Health increase
        public int meleeDamageStep;    // Melee damage increase
        public int rangeDamageStep;    // Range damage increase
        public int criticalStep;       // Critical chance increase
    }
```

**Why separate class?** This allows you to create an array of upgrades in the Inspector:

```
UpgradeSteps:
  [0] price: 100, healthStep: 10, meleeDamageStep: 5
  [1] price: 200, healthStep: 15, meleeDamageStep: 8
  [2] price: 400, healthStep: 20, meleeDamageStep: 12
```

### Character Identification

```csharp
public class UpgradedCharacterParameter : MonoBehaviour
{
    public string ID = "unique ID";        // ← MUST be unique per character
    public int unlockAtLevel = 1;          // Level requirement to unlock
    public Abitity playerAbility;          // Melee, Range, or Healer
```

**Example IDs**:
- `"archer_01"` - First archer character
- `"knight_heavy"` - Heavy knight character
- `"mage_fire"` - Fire mage character

### Weapon Effects

```csharp
[Header("EFFECT")]
public WeaponEffect weaponEffect;          // Optional effect (fire, ice, poison, etc.)
```

The `WeaponEffect` component defines special effects that happen when this character attacks (burning, freezing, etc.). See `03_Enemy_System_Complete.md` for details on effects.

### Upgrade Steps Array

```csharp
[Space]
public UpgradeStep[] UpgradeSteps;         // Upgrade progression
```

**Inspector Example**:
```
UpgradeSteps (Size: 5)
  Element 0
    ├─ price: 100
    ├─ healthStep: 20
    ├─ meleeDamageStep: 10
    ├─ rangeDamageStep: 0
    └─ criticalStep: 2
  Element 1
    ├─ price: 250
    ├─ healthStep: 30
    └─ ...
```

### Melee Attack Properties

```csharp
[Header("MELEE ATTACK")]
public int maxTargetPerHit = 1;   // How many enemies can be hit at once
```

**Example Values**:
- `1` = Hits one enemy (single-target)
- `3` = Hits three enemies (cleave attack)
- `10` = Hits all nearby enemies (area attack)

### Default Stats

```csharp
[Header("Default")]
public int defaultHealth = 100;
public int meleeDamage = 100;
public int rangeDamage = 100;
[Range(1, 100)]
public int criticalDamagePercent = 10;  // 10% chance for critical hit
```

These are the **base stats** before any upgrades. When the player upgrades the character, these values are **added to**, not replaced.

### Current Upgrade Property

```csharp
public int CurrentUpgrade
{
    get
    {
        // Load current upgrade level from PlayerPrefs
        int current = PlayerPrefs.GetInt(ID + "upgradeHealth" + "Current", 0);

        // If upgrade level exceeds available steps, mark as maxed out
        if (current >= UpgradeSteps.Length)
            current = -1;  // -1 means "max level reached"

        return current;
    }
    set
    {
        // Save current upgrade level to PlayerPrefs
        PlayerPrefs.SetInt(ID + "upgradeHealth" + "Current", value);
    }
}
```

**How it works**:
```csharp
// First time playing:
int level = CurrentUpgrade; // Returns 0 (no upgrades yet)

// After buying 2 upgrades:
CurrentUpgrade = 2;         // Saves 2 to PlayerPrefs
int level = CurrentUpgrade; // Returns 2

// After buying all 5 upgrades:
CurrentUpgrade = 5;         // Saves 5 to PlayerPrefs
int level = CurrentUpgrade; // Returns -1 (maxed out)
```

**Why -1 for max level?** It's a flag that tells the UI to show "MAX" instead of the next upgrade cost.

### Upgrade Character Method

```csharp
public void UpgradeCharacter(bool health, bool melee, bool range, bool crit)
{
    // Can't upgrade if already at max level
    if (CurrentUpgrade == -1)
        return;

    // Get the current upgrade step
    UpgradeStep step = UpgradeSteps[CurrentUpgrade];

    // Upgrade each stat if requested
    if (health)
        UpgradeHealth += step.healthStep;

    if (melee)
        UpgradeMeleeDamage += step.meleeDamageStep;

    if (range)
        UpgradeRangeDamage += step.rangeDamageStep;

    if (crit)
        UpgradeCriticalDamage += step.criticalStep;

    // Move to next upgrade level
    CurrentUpgrade++;
}
```

**Usage Example**:
```csharp
UpgradedCharacterParameter archer = GetComponent<UpgradedCharacterParameter>();

// Player buys "Upgrade All" button
archer.UpgradeCharacter(
    health: true,   // Increase health
    melee: false,   // Don't increase melee (archer doesn't use melee)
    range: true,    // Increase ranged damage
    crit: true      // Increase critical chance
);
```

### Upgrade Properties (PlayerPrefs Wrappers)

```csharp
public int UpgradeHealth
{
    get { return PlayerPrefs.GetInt(ID + "upgradeHealth", defaultHealth); }
    set { PlayerPrefs.SetInt(ID + "upgradeHealth", value); }
}

public int UpgradeMeleeDamage
{
    get { return PlayerPrefs.GetInt(ID + "UpgradedMeleeDamage", meleeDamage); }
    set { PlayerPrefs.SetInt(ID + "UpgradedMeleeDamage", value); }
}

public int UpgradeRangeDamage
{
    get { return PlayerPrefs.GetInt(ID + "UpgradeRangeDamage", rangeDamage); }
    set { PlayerPrefs.SetInt(ID + "UpgradeRangeDamage", value); }
}

public int UpgradeCriticalDamage
{
    get { return PlayerPrefs.GetInt(ID + "UpgradeCriticalDamage", criticalDamagePercent); }
    set { PlayerPrefs.SetInt(ID + "UpgradeCriticalDamage", value); }
}
```

**How they work together**:

```csharp
// Initial state (no upgrades)
archer.ID = "archer_01";
archer.defaultHealth = 100;
int health = archer.UpgradeHealth; // Returns 100 (default value)

// After first upgrade (+20 health)
archer.UpgradeHealth += 20;        // Saves 120 to PlayerPrefs
int health = archer.UpgradeHealth; // Returns 120

// Next game session (loads from PlayerPrefs)
int health = archer.UpgradeHealth; // Returns 120 (persisted!)
```

**PlayerPrefs Keys Generated** (for ID = "archer_01"):
- `"archer_01upgradeHealthCurrent"` → Current upgrade level
- `"archer_01upgradeHealth"` → Total health after upgrades
- `"archer_01UpgradedMeleeDamage"` → Total melee damage
- `"archer_01UpgradeRangeDamage"` → Total range damage
- `"archer_01UpgradeCriticalDamage"` → Total critical chance

---

## How Upgrades Work

### Upgrade Flow Diagram

```
Player Clicks "Upgrade" Button
         |
         v
[Check if max level?]
    |            |
   Yes           No
    |            |
    v            v
[Return]    [Get current UpgradeStep]
                |
                v
         [Apply stat increases]
                |
                v
         [Save to PlayerPrefs]
                |
                v
         [Increment CurrentUpgrade]
                |
                v
         [Update UI to show new stats]
```

### Example Upgrade Progression

**Character Setup**:
```
ID: "knight_01"
defaultHealth: 100
meleeDamage: 50
rangeDamage: 0

UpgradeSteps:
  [0] price: 100, healthStep: 20, meleeDamageStep: 10
  [1] price: 200, healthStep: 30, meleeDamageStep: 15
  [2] price: 400, healthStep: 40, meleeDamageStep: 20
```

**Upgrade Timeline**:

**Initial State** (No upgrades):
```
CurrentUpgrade: 0
UpgradeHealth: 100 (default)
UpgradeMeleeDamage: 50 (default)
```

**After Upgrade #1** (Cost: 100 coins):
```
CurrentUpgrade: 1
UpgradeHealth: 120 (100 + 20)
UpgradeMeleeDamage: 60 (50 + 10)
```

**After Upgrade #2** (Cost: 200 coins):
```
CurrentUpgrade: 2
UpgradeHealth: 150 (120 + 30)
UpgradeMeleeDamage: 75 (60 + 15)
```

**After Upgrade #3** (Cost: 400 coins):
```
CurrentUpgrade: -1 (MAX LEVEL)
UpgradeHealth: 190 (150 + 40)
UpgradeMeleeDamage: 95 (75 + 20)
```

### Code Example: Checking Upgrade Cost

```csharp
void DisplayUpgradeCost()
{
    UpgradedCharacterParameter character = GetComponent<UpgradedCharacterParameter>();

    if (character.CurrentUpgrade == -1)
    {
        Debug.Log("Character is at MAX level!");
        return;
    }

    int nextUpgradeIndex = character.CurrentUpgrade;
    UpgradeStep nextStep = character.UpgradeSteps[nextUpgradeIndex];

    Debug.Log($"Next upgrade costs: {nextStep.price} coins");
    Debug.Log($"Health will increase by: {nextStep.healthStep}");
    Debug.Log($"Melee damage will increase by: {nextStep.meleeDamageStep}");
}
```

---

## Creating a New Character

Follow these steps to create a completely new playable character.

### Step 1: Create Character GameObject

1. **Create Empty GameObject**:
   - Right-click in Hierarchy → Create Empty
   - Name it `Player_Mage` (or your character name)

2. **Add Visual Components**:
   ```
   Player_Mage
   ├─ SpriteRenderer (add the character sprite)
   ├─ Animator (add animation controller)
   ├─ CircleCollider2D (for collision detection)
   └─ Rigidbody2D (if using physics)
   ```

3. **Set Layer and Tag**:
   - Layer: `Player`
   - Tag: `Player`

### Step 2: Add UpgradedCharacterParameter Component

1. Select `Player_Mage` in Hierarchy
2. Click **Add Component** in Inspector
3. Search for `UpgradedCharacterParameter`
4. Click to add it

### Step 3: Configure Character Properties

**Set Unique ID**:
```
ID: "mage_fire_01"  ← MUST be unique!
```

**⚠️ Warning**: If you copy-paste a character, make sure to change the ID! Otherwise, both characters will share upgrade data.

**Set Unlock Level**:
```
unlockAtLevel: 5  ← Player must complete level 5 to unlock this character
```

**Set Ability Type**:
```
playerAbility: Range  ← This is a ranged character
```

**Configure Default Stats**:
```
defaultHealth: 80        ← Lower health (mage is fragile)
meleeDamage: 10          ← Weak melee attack
rangeDamage: 150         ← Strong magic attack
criticalDamagePercent: 15 ← 15% crit chance
maxTargetPerHit: 1       ← Single-target spells
```

**Add Weapon Effect** (optional):
- Drag a `WeaponEffect` prefab into the `weaponEffect` slot
- Or leave empty for no special effects

### Step 4: Define Upgrade Steps

Click the **+** button under `UpgradeSteps` to add upgrades.

**Example Configuration**:
```
UpgradeSteps (Size: 4)

  Element 0 (Upgrade Level 1)
    price: 150
    healthStep: 15
    meleeDamageStep: 5
    rangeDamageStep: 25
    criticalStep: 3

  Element 1 (Upgrade Level 2)
    price: 300
    healthStep: 20
    meleeDamageStep: 5
    rangeDamageStep: 35
    criticalStep: 4

  Element 2 (Upgrade Level 3)
    price: 600
    healthStep: 25
    meleeDamageStep: 10
    rangeDamageStep: 50
    criticalStep: 5

  Element 3 (Upgrade Level 4 - Final)
    price: 1200
    healthStep: 30
    meleeDamageStep: 10
    rangeDamageStep: 75
    criticalStep: 8
```

**Final Stats After All Upgrades**:
```
Total Health: 80 + 15 + 20 + 25 + 30 = 170
Total Melee: 10 + 5 + 5 + 10 + 10 = 40
Total Range: 150 + 25 + 35 + 50 + 75 = 335
Total Crit: 15 + 3 + 4 + 5 + 8 = 35%
```

### Step 5: Create Prefab

1. Create a folder: `Assets/Resources/Characters/` (if it doesn't exist)
2. Drag `Player_Mage` from Hierarchy to this folder
3. You now have a reusable prefab
4. Delete the instance from Hierarchy (the prefab is saved)

### Step 6: Integrate with Character Selection

**Option A: Add to Character Select Scene**

If you have a character selection menu:
```csharp
public class CharacterSelectionUI : MonoBehaviour
{
    public GameObject[] characterPrefabs;  // Add Player_Mage to this array

    public void SelectCharacter(int index)
    {
        GameObject selectedCharacter = characterPrefabs[index];
        CharacterManager.Instance.SpawnCharacter(selectedCharacter);
    }
}
```

**Option B: Spawn Directly in Game**

```csharp
public class GameController : MonoBehaviour
{
    public GameObject magePrefab;

    void Start()
    {
        // Spawn the mage at the character spawn point
        CharacterManager.Instance.SpawnCharacter(magePrefab);
    }
}
```

### Step 7: Test the Character

1. Enter Play Mode
2. Check the character appears correctly
3. Test basic movement and attacks
4. Debug.Log the stats:

```csharp
void Start()
{
    UpgradedCharacterParameter param = GetComponent<UpgradedCharacterParameter>();
    Debug.Log($"Character: {param.ID}");
    Debug.Log($"Health: {param.UpgradeHealth}");
    Debug.Log($"Range Damage: {param.UpgradeRangeDamage}");
    Debug.Log($"Critical: {param.UpgradeCriticalDamage}%");
    Debug.Log($"Current Upgrade: {param.CurrentUpgrade}");
}
```

**Expected Output** (first time):
```
Character: mage_fire_01
Health: 80
Range Damage: 150
Critical: 15%
Current Upgrade: 0
```

---

## Practical Examples

### Example 1: Display Character Stats in UI

```csharp
using UnityEngine;
using UnityEngine.UI;
using RGame;

public class CharacterStatsUI : MonoBehaviour
{
    [Header("References")]
    public UpgradedCharacterParameter character;

    [Header("UI Elements")]
    public Text healthText;
    public Text damageText;
    public Text critText;
    public Text upgradeText;

    void Start()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        // Display current stats
        healthText.text = $"Health: {character.UpgradeHealth}";

        // Show appropriate damage type
        if (character.playerAbility == Abitity.Melee)
        {
            damageText.text = $"Melee: {character.UpgradeMeleeDamage}";
        }
        else if (character.playerAbility == Abitity.Range)
        {
            damageText.text = $"Range: {character.UpgradeRangeDamage}";
        }

        critText.text = $"Critical: {character.UpgradeCriticalDamage}%";

        // Show upgrade level
        if (character.CurrentUpgrade == -1)
        {
            upgradeText.text = "Level: MAX";
        }
        else
        {
            upgradeText.text = $"Level: {character.CurrentUpgrade + 1}/{character.UpgradeSteps.Length}";
        }
    }
}
```

### Example 2: Upgrade Shop System

```csharp
using UnityEngine;
using UnityEngine.UI;
using RGame;

public class UpgradeShop : MonoBehaviour
{
    [Header("References")]
    public UpgradedCharacterParameter character;

    [Header("UI")]
    public Button upgradeButton;
    public Text costText;
    public Text statsPreviewText;

    private int playerCoins = 1000; // Example currency

    void Start()
    {
        UpdateShopUI();
    }

    void UpdateShopUI()
    {
        // Check if max level
        if (character.CurrentUpgrade == -1)
        {
            upgradeButton.interactable = false;
            costText.text = "MAX LEVEL";
            statsPreviewText.text = "Character fully upgraded!";
            return;
        }

        // Get next upgrade step
        UpgradeStep nextUpgrade = character.UpgradeSteps[character.CurrentUpgrade];

        // Display cost
        costText.text = $"Cost: {nextUpgrade.price} coins";

        // Show what stats will increase
        statsPreviewText.text = $"Next Upgrade:\n" +
            $"+{nextUpgrade.healthStep} Health\n" +
            $"+{nextUpgrade.meleeDamageStep} Melee\n" +
            $"+{nextUpgrade.rangeDamageStep} Range\n" +
            $"+{nextUpgrade.criticalStep}% Critical";

        // Enable/disable button based on affordability
        upgradeButton.interactable = (playerCoins >= nextUpgrade.price);
    }

    public void OnUpgradeButtonClicked()
    {
        if (character.CurrentUpgrade == -1)
            return; // Already max level

        UpgradeStep nextUpgrade = character.UpgradeSteps[character.CurrentUpgrade];

        // Check if player can afford it
        if (playerCoins < nextUpgrade.price)
        {
            Debug.Log("Not enough coins!");
            return;
        }

        // Deduct cost
        playerCoins -= nextUpgrade.price;

        // Apply upgrade (upgrade all stats for this example)
        character.UpgradeCharacter(
            health: true,
            melee: true,
            range: true,
            crit: true
        );

        Debug.Log($"Upgraded {character.ID} to level {character.CurrentUpgrade}!");

        // Refresh UI
        UpdateShopUI();
    }
}
```

### Example 3: Reset Character Progress (Debug Tool)

```csharp
using UnityEngine;
using RGame;

public class CharacterDebugTools : MonoBehaviour
{
    public UpgradedCharacterParameter character;

    [ContextMenu("Reset Character Progress")]
    void ResetProgress()
    {
        // Delete all PlayerPrefs keys for this character
        PlayerPrefs.DeleteKey(character.ID + "upgradeHealthCurrent");
        PlayerPrefs.DeleteKey(character.ID + "upgradeHealth");
        PlayerPrefs.DeleteKey(character.ID + "UpgradedMeleeDamage");
        PlayerPrefs.DeleteKey(character.ID + "UpgradeRangeDamage");
        PlayerPrefs.DeleteKey(character.ID + "UpgradeCriticalDamage");

        PlayerPrefs.Save(); // Force save

        Debug.Log($"Reset all upgrades for {character.ID}");
    }

    [ContextMenu("Unlock All Upgrades")]
    void UnlockAll()
    {
        // Max out all stats
        foreach (UpgradeStep step in character.UpgradeSteps)
        {
            character.UpgradeCharacter(true, true, true, true);
        }

        Debug.Log($"Maxed out {character.ID}!");
    }

    [ContextMenu("Give Free Upgrade")]
    void GiveFreeUpgrade()
    {
        if (character.CurrentUpgrade == -1)
        {
            Debug.Log("Already at max level!");
            return;
        }

        character.UpgradeCharacter(true, true, true, true);
        Debug.Log($"Gave free upgrade to {character.ID}");
    }

    [ContextMenu("Print Current Stats")]
    void PrintStats()
    {
        Debug.Log($"===== {character.ID} Stats =====");
        Debug.Log($"Upgrade Level: {character.CurrentUpgrade}");
        Debug.Log($"Health: {character.UpgradeHealth}");
        Debug.Log($"Melee: {character.UpgradeMeleeDamage}");
        Debug.Log($"Range: {character.UpgradeRangeDamage}");
        Debug.Log($"Critical: {character.UpgradeCriticalDamage}%");
    }
}
```

**How to use**: Attach this script to any GameObject, assign the character, then right-click the component header in Inspector and select the menu option.

### Example 4: Selective Stat Upgrades

Instead of upgrading all stats at once, let players choose:

```csharp
public class SelectiveUpgradeShop : MonoBehaviour
{
    public UpgradedCharacterParameter character;

    public void UpgradeHealthOnly()
    {
        if (character.CurrentUpgrade == -1) return;

        // Only upgrade health, not other stats
        character.UpgradeCharacter(
            health: true,
            melee: false,
            range: false,
            crit: false
        );

        // Note: CurrentUpgrade still increments!
        // You might want to modify the system for independent stat upgrades
    }

    public void UpgradeDamageOnly()
    {
        if (character.CurrentUpgrade == -1) return;

        // Choose melee or range based on character type
        bool isMelee = (character.playerAbility == Abitity.Melee);

        character.UpgradeCharacter(
            health: false,
            melee: isMelee,
            range: !isMelee,
            crit: false
        );
    }
}
```

**⚠️ Important**: The current system increments `CurrentUpgrade` regardless of which stats you upgrade. If you want independent stat upgrades, you'll need to modify the system.

---

## Visual Flow Diagrams

### Character Upgrade Data Flow

```
┌─────────────────────────────────────────────────────────────┐
│                    GAME STARTS                              │
└────────────────┬────────────────────────────────────────────┘
                 │
                 v
┌─────────────────────────────────────────────────────────────┐
│  UpgradedCharacterParameter.Start()                         │
│  ├─ Read PlayerPrefs for character ID                       │
│  └─ If no saved data, use default stats                     │
└────────────────┬────────────────────────────────────────────┘
                 │
                 v
┌─────────────────────────────────────────────────────────────┐
│  DISPLAY STATS IN UI                                        │
│  ├─ Health: UpgradeHealth (loads from PlayerPrefs)          │
│  ├─ Damage: UpgradeMeleeDamage or UpgradeRangeDamage        │
│  └─ Crit: UpgradeCriticalDamage                             │
└────────────────┬────────────────────────────────────────────┘
                 │
                 v
        [Player Clicks Upgrade]
                 │
                 v
┌─────────────────────────────────────────────────────────────┐
│  UpgradeCharacter()                                         │
│  ├─ Check CurrentUpgrade != -1 (not max level)              │
│  ├─ Get UpgradeSteps[CurrentUpgrade]                        │
│  ├─ Add healthStep to UpgradeHealth                         │
│  ├─ Add meleeDamageStep to UpgradeMeleeDamage               │
│  ├─ Add rangeDamageStep to UpgradeRangeDamage               │
│  ├─ Add criticalStep to UpgradeCriticalDamage               │
│  └─ Increment CurrentUpgrade                                │
└────────────────┬────────────────────────────────────────────┘
                 │
                 v
┌─────────────────────────────────────────────────────────────┐
│  AUTOMATIC SAVE (via property setters)                      │
│  ├─ PlayerPrefs.SetInt("archer_01upgradeHealth", 120)       │
│  ├─ PlayerPrefs.SetInt("archer_01UpgradedMeleeDamage", 60)  │
│  └─ PlayerPrefs.SetInt("archer_01upgradeHealthCurrent", 1)  │
└────────────────┬────────────────────────────────────────────┘
                 │
                 v
┌─────────────────────────────────────────────────────────────┐
│  GAME CLOSED                                                │
│  └─ PlayerPrefs data persists on disk                       │
└────────────────┬────────────────────────────────────────────┘
                 │
                 v
┌─────────────────────────────────────────────────────────────┐
│  NEXT GAME SESSION                                          │
│  └─ UpgradeHealth loads 120 from PlayerPrefs (not 100!)     │
└─────────────────────────────────────────────────────────────┘
```

### PlayerPrefs Key Structure

```
For character with ID = "archer_01":

PlayerPrefs Storage:
├─ "archer_01upgradeHealthCurrent" → 2 (current upgrade level)
├─ "archer_01upgradeHealth" → 140 (total health)
├─ "archer_01UpgradedMeleeDamage" → 50 (total melee)
├─ "archer_01UpgradeRangeDamage" → 180 (total range)
└─ "archer_01UpgradeCriticalDamage" → 18 (total crit %)

For character with ID = "knight_01":

PlayerPrefs Storage:
├─ "knight_01upgradeHealthCurrent" → 0 (no upgrades yet)
├─ "knight_01upgradeHealth" → 200 (default health)
├─ "knight_01UpgradedMeleeDamage" → 150 (default melee)
├─ "knight_01UpgradeRangeDamage" → 0 (default range)
└─ "knight_01UpgradeCriticalDamage" → 5 (default crit)
```

### Character Creation Workflow

```
START: I want to create a new character
  |
  v
[Create GameObject]
  ├─ Add SpriteRenderer
  ├─ Add Animator
  ├─ Add Collider
  └─ Set Layer/Tag
  |
  v
[Add UpgradedCharacterParameter Component]
  |
  v
[Configure in Inspector]
  ├─ Set UNIQUE ID (e.g., "mage_01")
  ├─ Set unlockAtLevel
  ├─ Choose playerAbility (Melee/Range/Healer)
  ├─ Set defaultHealth, defaultDamage, defaultCrit
  └─ Fill UpgradeSteps array
  |
  v
[Create Prefab]
  └─ Drag to Assets/Resources/Characters/
  |
  v
[Integrate with Game]
  ├─ Add to Character Selection UI
  └─ OR spawn directly in code
  |
  v
[Test in Play Mode]
  ├─ Verify stats display correctly
  ├─ Test upgrades save/load
  └─ Test ability works as expected
  |
  v
DONE: New character ready!
```

---

## Troubleshooting

### Problem 1: Upgrades Not Saving Between Sessions

**Symptoms**:
- Character stats reset to default every time you restart the game
- Upgrades work during a session but don't persist

**Causes**:
1. PlayerPrefs not being saved to disk
2. Character ID changed between sessions
3. Platform doesn't support PlayerPrefs

**Solutions**:

**Solution A: Force Save PlayerPrefs**
```csharp
public void UpgradeCharacter(bool health, bool melee, bool range, bool crit)
{
    // ... existing upgrade code ...

    PlayerPrefs.Save(); // ← Add this line to force write to disk
}
```

**Solution B: Verify ID Consistency**
```csharp
void Start()
{
    UpgradedCharacterParameter param = GetComponent<UpgradedCharacterParameter>();
    Debug.Log($"Character ID: {param.ID}"); // Should be same every time!
}
```

**Solution C: Check Platform Support**

Some platforms have restrictions on PlayerPrefs. For WebGL builds, browser cookies must be enabled.

---

### Problem 2: Two Characters Share Upgrade Data

**Symptoms**:
- Upgrading one character also upgrades another
- Different characters have identical stats

**Cause**: Both characters have the same ID

**Solution**:

Check IDs in Inspector:
```
Player_Archer
  └─ UpgradedCharacterParameter
      └─ ID: "archer_01" ✓ Good

Player_Mage
  └─ UpgradedCharacterParameter
      └─ ID: "archer_01" ✗ BAD! Should be "mage_01"
```

**Fix**: Ensure every character has a unique ID.

**Bulk ID Checker Script**:
```csharp
using UnityEngine;
using RGame;
using System.Collections.Generic;

public class IDValidator : MonoBehaviour
{
    [ContextMenu("Check for Duplicate IDs")]
    void CheckDuplicateIDs()
    {
        UpgradedCharacterParameter[] allCharacters = FindObjectsOfType<UpgradedCharacterParameter>();

        Dictionary<string, string> idMap = new Dictionary<string, string>();

        foreach (var character in allCharacters)
        {
            if (idMap.ContainsKey(character.ID))
            {
                Debug.LogError($"DUPLICATE ID FOUND: '{character.ID}' used by both " +
                    $"'{idMap[character.ID]}' and '{character.gameObject.name}'");
            }
            else
            {
                idMap[character.ID] = character.gameObject.name;
                Debug.Log($"✓ {character.gameObject.name} has unique ID: {character.ID}");
            }
        }
    }
}
```

---

### Problem 3: Upgrade Costs Don't Increase

**Symptoms**:
- Every upgrade costs the same amount
- Expected: 100, 200, 400... Actual: 100, 100, 100...

**Cause**: Using wrong UpgradeStep index

**Incorrect Code**:
```csharp
// WRONG: Always uses first upgrade step
UpgradeStep step = character.UpgradeSteps[0];
int cost = step.price; // Always returns UpgradeSteps[0].price
```

**Correct Code**:
```csharp
// CORRECT: Uses current upgrade level
int currentLevel = character.CurrentUpgrade;
UpgradeStep step = character.UpgradeSteps[currentLevel];
int cost = step.price; // Returns correct price for current level
```

---

### Problem 4: Can't Upgrade Past First Level

**Symptoms**:
- First upgrade works
- All subsequent upgrades fail or do nothing
- `CurrentUpgrade` stays at 0

**Cause**: Forgetting to increment `CurrentUpgrade`

**Check Your Code**:
```csharp
// If you wrote your own upgrade method:
void CustomUpgrade()
{
    // Add stats...
    character.UpgradeHealth += 20;

    // ← Missing: character.CurrentUpgrade++;
}
```

**Solution**: Use the built-in `UpgradeCharacter()` method, which automatically increments the counter.

---

### Problem 5: NullReferenceException on Upgrade

**Error Message**:
```
NullReferenceException: Object reference not set to an instance of an object
UpgradedCharacterParameter.UpgradeCharacter()
```

**Cause**: `UpgradeSteps` array is empty or null

**Check Inspector**:
```
UpgradedCharacterParameter
  ├─ ID: "archer_01"
  └─ UpgradeSteps (Size: 0) ← Problem! Should be > 0
```

**Solution**: Add at least one upgrade step in the Inspector.

---

### Problem 6: Character Unlocks Too Early/Late

**Symptoms**:
- Character available before specified level
- Or never unlocks even after completing required level

**Check**:
```csharp
public int unlockAtLevel = 5; // Should unlock after level 5
```

**Verify Your Unlock Logic**:
```csharp
void CheckUnlock()
{
    UpgradedCharacterParameter character = GetComponent<UpgradedCharacterParameter>();
    int completedLevels = GameManager.Instance.GetCompletedLevels(); // Example

    if (completedLevels >= character.unlockAtLevel)
    {
        // Character is unlocked
        Debug.Log($"{character.ID} is now unlocked!");
    }
    else
    {
        Debug.Log($"{character.ID} requires level {character.unlockAtLevel}");
    }
}
```

**Note**: This project doesn't automatically check `unlockAtLevel`. You need to implement this in your character selection UI.

---

### Problem 7: Reset Progress Doesn't Work

**Symptoms**:
- Called `PlayerPrefs.DeleteAll()` but character still has upgrades

**Cause**: Properties reload from cache instead of PlayerPrefs

**Solution**: Restart the scene after deleting PlayerPrefs

```csharp
void ResetAndRestart()
{
    PlayerPrefs.DeleteAll();
    PlayerPrefs.Save();

    // Reload the current scene
    UnityEngine.SceneManagement.SceneManager.LoadScene(
        UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
    );
}
```

---

## Cross-References

**Related Documentation**:

- **[02_Player_System_Complete.md](02_Player_System_Complete.md)** - How player characters work, movement, shooting
- **[03_Enemy_System_Complete.md](03_Enemy_System_Complete.md)** - Understanding WeaponEffect and damage system
- **[04_UI_System_Complete.md](04_UI_System_Complete.md)** - Creating UI to display character stats
- **[10_How_To_Guides.md](10_How_To_Guides.md)** - Tutorial: "How to Create Custom Upgrades"
- **[13_Code_Examples.md](13_Code_Examples.md)** - Save/Load system examples
- **[99_Glossary.md](99_Glossary.md)** - Definitions: PlayerPrefs, Property, Serializable

**Key Classes**:
- `UpgradedCharacterParameter.cs` - Main class documented here
- `CharacterManager.cs` - Handles character spawning
- `GameManager.cs` - Manages game state and level progression
- `ShopUI.cs` - Shop interface (if implemented)

**Unity Documentation**:
- [PlayerPrefs](https://docs.unity3d.com/ScriptReference/PlayerPrefs.html)
- [Properties (C#)](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/properties)
- [Serialization](https://docs.unity3d.com/Manual/script-Serialization.html)

---

## Summary

**What You Learned**:
- ✅ How `UpgradedCharacterParameter` stores and retrieves character stats
- ✅ How PlayerPrefs provides data persistence between sessions
- ✅ How the upgrade system progressively increases stats
- ✅ How to create new characters with custom abilities and stats
- ✅ How to debug common character-related issues

**Key Takeaways**:
1. **Every character MUST have a unique ID** to avoid data conflicts
2. **PlayerPrefs automatically saves** when you use the upgrade properties
3. **CurrentUpgrade = -1 means max level** reached
4. **UpgradeSteps array defines** the entire progression system
5. **Default stats are starting values**, upgrades add to them

**Next Steps**:
- Try creating your own character following the step-by-step guide
- Experiment with different stat progressions and costs
- Build a UI shop system to let players purchase upgrades
- Read `04_UI_System_Complete.md` to learn how to display stats

---

**Document Version**: 1.0
**Last Updated**: 2025-10-29
**Transformation**: Vietnamese (22 lines) → English (1,700+ lines)
