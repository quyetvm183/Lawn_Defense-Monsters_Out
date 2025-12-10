# How-To Guides - Practical Tutorials

> **For**: Developers ready to modify the game
> **Read Time**: 60+ minutes (reference guide)
> **Prerequisites**: All core system documentation (02-05)

---

## Table of Contents
1. [How to Add a New Enemy Type](#how-to-add-a-new-enemy-type)
2. [How to Create a Custom UI Panel](#how-to-create-a-custom-ui-panel)
3. [How to Add a New Weapon Effect](#how-to-add-a-new-weapon-effect)
4. [How to Modify Player Stats](#how-to-modify-player-stats)
5. [How to Add a New Level](#how-to-add-a-new-level)
6. [How to Add Power-Up Items](#how-to-add-power-up-items)
7. [How to Create Custom Health Bars](#how-to-create-custom-health-bars)
8. [How to Add Sound Effects](#how-to-add-sound-effects)
9. [How to Implement Save/Load System](#how-to-implement-saveload-system)
10. [How to Change Game Difficulty](#how-to-change-game-difficulty)

---

## How to Add a New Enemy Type

### Goal
Create a new enemy called "Ghost" with flying behavior and phase-through ability.

### Prerequisites
- Read `03_Enemy_System_Complete.md`
- Understand Enemy base class and SmartEnemyGrounded

### Step 1: Create Enemy Sprite

1. Import sprite: `Assets/Resources/Sprite/Enemy/10. Ghost/`
2. Create animations:
   - `Ghost_Idle` (loop)
   - `Ghost_Fly` (loop)
   - `Ghost_Attack` (once)
   - `Ghost_Die` (once)

3. Create Animation Controller: `Ghost_AnimController`
   ```
   Parameters:
   - speed (float)
   - attack (trigger)
   - isDead (bool)

   Transitions:
   - Idle → Fly: speed > 0.1
   - Fly → Idle: speed < 0.1
   - Any State → Attack: attack trigger
   - Any State → Die: isDead = true
   ```

### Step 2: Create Ghost Script

Create `Enemy_Ghost.cs` in `Assets/_MonstersOut/Scripts/AI/`:

```csharp
using UnityEngine;
using System.Collections;

namespace RGame
{
    [AddComponentMenu("ADDP/Enemy AI/Ghost Enemy")]
    public class Enemy_Ghost : Enemy, ICanTakeDamage
    {
        [Header("Flying Settings")]
        public float flySpeed = 3f;
        public float flyHeight = 2f;
        public float floatSpeed = 1f;
        public float floatAmount = 0.5f;

        private Vector3 targetPosition;
        private float floatTimer;
        private EnemyRangeAttack rangeAttack;

        public override void Start()
        {
            base.Start();

            // Get attack component
            rangeAttack = GetComponent<EnemyRangeAttack>();

            // Find fortress to fly toward
            var fortress = FindObjectOfType<TheFortrest>();
            if (fortress)
            {
                targetPosition = new Vector3(
                    fortress.transform.position.x,
                    flyHeight,
                    0
                );
            }
        }

        public override void Update()
        {
            base.Update();

            // Only move if playing and in WALK state
            if (isPlaying && enemyState == ENEMYSTATE.WALK)
            {
                // Move toward fortress
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPosition,
                    flySpeed * Time.deltaTime
                );

                // Floating motion (up and down)
                floatTimer += Time.deltaTime * floatSpeed;
                float newY = flyHeight + Mathf.Sin(floatTimer) * floatAmount;
                transform.position = new Vector3(
                    transform.position.x,
                    newY,
                    transform.position.z
                );

                // Update animation
                AnimSetFloat("speed", flySpeed);

                // Face movement direction
                if (targetPosition.x < transform.position.x)
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                else
                    transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else
            {
                AnimSetFloat("speed", 0);
            }
        }

        public override void DetectPlayer(float delayChase = 0)
        {
            base.DetectPlayer(delayChase);

            // Check if in attack range
            if (rangeAttack && rangeAttack.CheckPlayer(isFacingRight()))
            {
                // Stop moving
                SetEnemyState(ENEMYSTATE.ATTACK);

                // Attack
                if (rangeAttack.AllowAction())
                {
                    rangeAttack.Action();
                    AnimSetTrigger("attack");
                }
            }
        }

        public override void Die()
        {
            base.Die();

            // Fall down animation
            StartCoroutine(FallDown());
        }

        IEnumerator FallDown()
        {
            float timer = 0;
            float fallDuration = 1f;
            Vector3 startPos = transform.position;

            while (timer < fallDuration)
            {
                timer += Time.deltaTime;
                float newY = Mathf.Lerp(startPos.y, -2f, timer / fallDuration);
                transform.position = new Vector3(
                    transform.position.x,
                    newY,
                    transform.position.z
                );
                yield return null;
            }

            gameObject.SetActive(false);
        }
    }
}
```

### Step 3: Create Ghost Prefab

1. Create empty GameObject: `Enemy_Ghost`
2. Add components:
   - `SpriteRenderer` → assign Ghost sprite
   - `Animator` → assign Ghost_AnimController
   - `BoxCollider2D` → adjust to sprite size
   - `Enemy_Ghost` script
   - `EnemyRangeAttack` script
   - `CheckTargetHelper` script
   - `GiveCoinWhenDie` script (optional)

3. Configure `Enemy_Ghost` settings:
   ```
   Health: 80
   Walk Speed: 3
   Gravity: 0 (flying enemy)
   Attack Type: RANGE
   Start Behavior: WALK_LEFT

   Can Be Freeze: true
   Can Be Burn: true
   Can Be Poison: true
   Can Be Shock: true
   ```

4. Configure `EnemyRangeAttack`:
   ```
   Enemy Layer: Player
   Check Point: Create child "CheckPoint"
   Fire Point: Create child "FirePoint"
   Shooting Point: Create child "ShootingPoint"
   Damage: 25
   Detect Distance: 8
   Bullet: Your projectile prefab
   Shooting Rate: 2
   Aim Target: true
   ```

5. Save as prefab: `Assets/Resources/Prefabs/Enemies/Enemy_Ghost.prefab`

### Step 4: Add to Enemy Wave

In your level configuration:

```csharp
EnemyWave wave3 = new EnemyWave
{
    wait = 15f,  // Wait 15 seconds
    enemySpawns = new EnemySpawn[]
    {
        new EnemySpawn
        {
            enemy = ghostPrefab,  // Reference to Ghost prefab
            numberEnemy = 3,       // Spawn 3 ghosts
            wait = 0f,            // Start immediately
            rate = 2f             // 2 seconds between spawns
        }
    }
};
```

### Step 5: Test

1. Play scene
2. Wait for Ghost wave (15 seconds)
3. Verify:
   - ✓ Ghost flies toward fortress
   - ✓ Ghost floats up and down
   - ✓ Ghost attacks when in range
   - ✓ Ghost falls down when killed
   - ✓ Effects work (freeze, burn, etc.)

### Expected Result

```
Timeline:
0s:    Game starts
15s:   Ghost #1 spawns, flies toward fortress
17s:   Ghost #2 spawns
19s:   Ghost #3 spawns
22s:   Ghost #1 in range, shoots projectile
25s:   Ghost #1 killed, falls down
```

---

## How to Create a Custom UI Panel

### Goal
Create a "Statistics Panel" showing kills, damage dealt, and time played.

### Prerequisites
- Read `04_UI_System_Complete.md`
- Understand Canvas and UI components

### Step 1: Create UI Elements

1. In Hierarchy, find Canvas
2. Right-click Canvas → `UI → Panel`
3. Name it: `StatisticsPanel`
4. Add child elements:

```
StatisticsPanel (Panel)
├─ Background (Image)
├─ Title (Text)
│   └─ Text: "Statistics"
├─ CloseButton (Button)
│   └─ Text: "X"
├─ KillsText (Text)
│   └─ Text: "Kills: 0"
├─ DamageText (Text)
│   └─ Text: "Damage: 0"
└─ TimeText (Text)
    └─ Text: "Time: 00:00"
```

### Step 2: Position UI Elements

```
StatisticsPanel:
- Anchor: Center
- Position: (0, 0)
- Width: 400
- Height: 300

Title:
- Anchor: Top Center
- Position: (0, -30)
- Font Size: 32

CloseButton:
- Anchor: Top Right
- Position: (-20, -20)
- Width: 40, Height: 40

KillsText, DamageText, TimeText:
- Anchor: Center
- Position: (0, 50), (0, 0), (0, -50)
- Font Size: 24
```

### Step 3: Create StatisticsPanel Script

Create `StatisticsPanel.cs` in `Assets/_MonstersOut/Scripts/UI/`:

```csharp
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace RGame
{
    public class StatisticsPanel : MonoBehaviour
    {
        [Header("UI References")]
        public Text killsText;
        public Text damageText;
        public Text timeText;

        [Header("Data")]
        private int totalKills = 0;
        private int totalDamage = 0;
        private float playTime = 0f;

        void Start()
        {
            // Hide on start
            gameObject.SetActive(false);
        }

        void Update()
        {
            // Only count time when panel is visible
            if (gameObject.activeInHierarchy)
            {
                playTime += Time.deltaTime;
                UpdateDisplay();
            }
        }

        public void Show()
        {
            // Reset stats
            totalKills = 0;
            totalDamage = 0;
            playTime = 0f;

            // Load from StatisticsTracker if exists
            if (StatisticsTracker.Instance)
            {
                totalKills = StatisticsTracker.Instance.GetKills();
                totalDamage = StatisticsTracker.Instance.GetDamage();
                playTime = StatisticsTracker.Instance.GetPlayTime();
            }

            // Show panel
            gameObject.SetActive(true);
            UpdateDisplay();
        }

        public void Hide()
        {
            SoundManager.Click();
            gameObject.SetActive(false);
        }

        void UpdateDisplay()
        {
            killsText.text = $"Kills: {totalKills}";
            damageText.text = $"Damage: {totalDamage}";

            // Format time as MM:SS
            int minutes = Mathf.FloorToInt(playTime / 60);
            int seconds = Mathf.FloorToInt(playTime % 60);
            timeText.text = $"Time: {minutes:00}:{seconds:00}";
        }
    }
}
```

### Step 4: Create StatisticsTracker

Create `StatisticsTracker.cs` in `Assets/_MonstersOut/Scripts/Managers/`:

```csharp
using UnityEngine;

namespace RGame
{
    public class StatisticsTracker : MonoBehaviour, IListener
    {
        public static StatisticsTracker Instance { get; private set; }

        private int totalKills = 0;
        private int totalDamage = 0;
        private float playTime = 0f;
        private bool isPlaying = false;

        void Awake()
        {
            Instance = this;
        }

        void Update()
        {
            // Count play time
            if (isPlaying)
                playTime += Time.deltaTime;
        }

        public void AddKill()
        {
            totalKills++;
        }

        public void AddDamage(int damage)
        {
            totalDamage += damage;
        }

        public int GetKills() => totalKills;
        public int GetDamage() => totalDamage;
        public float GetPlayTime() => playTime;

        // IListener implementation
        public void IPlay()
        {
            isPlaying = true;
            totalKills = 0;
            totalDamage = 0;
            playTime = 0f;
        }

        public void ISuccess()
        {
            isPlaying = false;
        }

        public void IGameOver()
        {
            isPlaying = false;
        }

        public void IPause() { }
        public void IUnPause() { }
        public void IOnRespawn() { }
        public void IOnStopMovingOn() { }
        public void IOnStopMovingOff() { }
    }
}
```

### Step 5: Hook Up Statistics Tracking

In `Enemy.cs` Die() method, add:

```csharp
public virtual void Die()
{
    // Existing code...

    // Track kill
    if (StatisticsTracker.Instance)
        StatisticsTracker.Instance.AddKill();

    // Existing code...
}
```

In `Enemy.cs` TakeDamage() method, add:

```csharp
public void TakeDamage(float damage, ...)
{
    // Existing code...

    currentHealth -= (int)damage;

    // Track damage
    if (StatisticsTracker.Instance)
        StatisticsTracker.Instance.AddDamage((int)damage);

    // Existing code...
}
```

### Step 6: Add Button to Show Statistics

In Victory UI, add button:

```csharp
// In Menu_Victory.cs
public StatisticsPanel statisticsPanel;

public void OnStatisticsButtonClick()
{
    statisticsPanel.Show();
}
```

### Step 7: Configure Button

1. Select Statistics button in Victory UI
2. Button component → OnClick()
3. Add entry: `Menu_Victory → OnStatisticsButtonClick()`

### Step 8: Test

1. Play game
2. Kill enemies
3. Complete level
4. Click Statistics button
5. Verify:
   - ✓ Shows correct kill count
   - ✓ Shows total damage dealt
   - ✓ Shows play time
   - ✓ Close button works

### Expected Result

```
Victory Screen:
┌─────────────────────────┐
│      VICTORY!           │
│   ★ ★ ★                 │
│                         │
│  [Statistics] [Menu]    │
└─────────────────────────┘

Click Statistics:
┌─────────────────────────┐
│    Statistics      [X]  │
│                         │
│  Kills: 47              │
│  Damage: 3,842          │
│  Time: 03:45            │
│                         │
└─────────────────────────┘
```

---

## How to Add a New Weapon Effect

### Goal
Add a "Lightning" weapon effect that chains to nearby enemies.

### Prerequisites
- Read `03_Enemy_System_Complete.md` (Effect System section)
- Understand WeaponEffect and ENEMYEFFECT enum

### Step 1: Add Lightning to ENEMYEFFECT Enum

In `Enemy.cs`, modify enum:

```csharp
public enum ENEMYEFFECT
{
    NONE,
    BURNING,
    FREEZE,
    SHOKING,
    POISON,
    EXPLOSION,
    LIGHTNING  // ← Add this
}
```

### Step 2: Add Lightning Effect Method to Enemy

In `Enemy.cs`, add:

```csharp
#region ICanLightning implementation

[Header("Lightning Option")]
[HideInInspector] public bool canBeLightning = true;
[HideInInspector] public int lightningChainCount = 3;
[HideInInspector] public float lightningChainRadius = 5f;

public virtual void Lightning(float damage, GameObject instigator)
{
    // Can't lightning if already lightning
    if (enemyEffect == ENEMYEFFECT.LIGHTNING)
        return;

    if (canBeLightning)
    {
        // Apply damage
        currentHealth -= (int)damage;

        // Show damage
        FloatingTextManager.Instance.ShowText(
            "" + (int)damage,
            healthBarOffset,
            Color.yellow,
            transform.position
        );

        // Update health bar
        if (healthBar)
            healthBar.UpdateValue(currentHealth / (float)health);

        // Check if dead
        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        // Chain to nearby enemies
        StartCoroutine(LightningChain(damage, instigator));
    }
}

IEnumerator LightningChain(float damage, GameObject instigator)
{
    enemyEffect = ENEMYEFFECT.LIGHTNING;

    // Find nearby enemies
    RaycastHit2D[] hits = Physics2D.CircleCastAll(
        transform.position,
        lightningChainRadius,
        Vector2.zero,
        0,
        GameManager.Instance.layerEnemy
    );

    int chained = 0;

    foreach (var hit in hits)
    {
        // Skip self
        if (hit.collider.gameObject == gameObject)
            continue;

        // Check if can take damage
        var enemy = hit.collider.GetComponent<Enemy>();
        if (enemy != null && chained < lightningChainCount)
        {
            // Draw lightning effect (visual)
            DrawLightning(transform.position, enemy.transform.position);

            // Chain lightning
            enemy.Lightning(damage * 0.7f, instigator);  // 70% damage

            chained++;

            yield return new WaitForSeconds(0.1f);  // Small delay
        }
    }

    // Clear effect
    yield return new WaitForSeconds(0.5f);
    enemyEffect = ENEMYEFFECT.NONE;
}

void DrawLightning(Vector3 start, Vector3 end)
{
    // Create lightning line renderer
    GameObject lightningObj = new GameObject("Lightning");
    LineRenderer line = lightningObj.AddComponent<LineRenderer>();

    line.startWidth = 0.1f;
    line.endWidth = 0.1f;
    line.positionCount = 2;
    line.SetPosition(0, start);
    line.SetPosition(1, end);

    // Set color (yellow)
    line.material = new Material(Shader.Find("Sprites/Default"));
    line.startColor = Color.yellow;
    line.endColor = Color.yellow;

    // Destroy after 0.2 seconds
    Destroy(lightningObj, 0.2f);
}

#endregion
```

### Step 3: Add LIGHTNING to WEAPON_EFFECT Enum

In `WeaponEffect.cs` (or create if doesn't exist):

```csharp
public enum WEAPON_EFFECT
{
    NORMAL,
    FREEZE,
    POISON,
    LIGHTNING  // ← Add this
}
```

### Step 4: Modify WeaponEffect ScriptableObject

In `WeaponEffect.cs`:

```csharp
[System.Serializable]
public class WeaponEffect : ScriptableObject
{
    public WEAPON_EFFECT effectType = WEAPON_EFFECT.NORMAL;

    // ... existing freeze/poison fields ...

    [Header("Lightning")]
    public float lightningDamage = 30f;
}
```

### Step 5: Update TakeDamage to Handle Lightning

In `Enemy.cs` TakeDamage() method:

```csharp
public void TakeDamage(float damage, Vector2 force, Vector2 hitPoint,
                       GameObject instigator, BODYPART bodyPart = BODYPART.NONE,
                       WeaponEffect weaponEffect = null)
{
    // ... existing code ...

    if (currentHealth <= 0)
    {
        Die();
    }
    else
    {
        if (weaponEffect != null)
        {
            switch (weaponEffect.effectType)
            {
                case WEAPON_EFFECT.POISON:
                    Poison(weaponEffect.poisonDamagePerSec,
                          weaponEffect.poisonTime,
                          instigator);
                    return;

                case WEAPON_EFFECT.FREEZE:
                    Freeze(weaponEffect.freezeTime, instigator);
                    return;

                case WEAPON_EFFECT.LIGHTNING:  // ← Add this
                    Lightning(weaponEffect.lightningDamage, instigator);
                    return;

                case WEAPON_EFFECT.NORMAL:
                    break;
            }
        }

        Hit(force);
    }
}
```

### Step 6: Create Lightning Weapon Effect Asset

1. In Project window: `Create → Weapon Effect → Lightning Effect`
2. Name: `LightningEffect`
3. Configure:
   ```
   Effect Type: LIGHTNING
   Lightning Damage: 30
   ```

### Step 7: Assign to Weapon

In your arrow/bullet prefab:

```csharp
public class Projectile : MonoBehaviour
{
    public WeaponEffect weaponEffect;  // Assign LightningEffect here

    void OnTriggerEnter2D(Collider2D other)
    {
        var takeDamage = other.GetComponent<ICanTakeDamage>();
        if (takeDamage != null)
        {
            takeDamage.TakeDamage(
                damage,
                force,
                transform.position,
                owner,
                BODYPART.NONE,
                weaponEffect  // Pass weapon effect
            );
        }
    }
}
```

### Step 8: Test

1. Play game
2. Shoot enemy with lightning weapon
3. Verify:
   - ✓ Enemy takes lightning damage
   - ✓ Lightning chains to 3 nearby enemies
   - ✓ Chain damage is 70% of original
   - ✓ Visual lightning effect appears
   - ✓ Chained enemies can die

### Expected Result

```
Player shoots enemy A with lightning arrow:

Enemy A: Takes 30 damage
         └─ Chains to Enemy B (5 units away)
                Takes 21 damage (70%)
                └─ Chains to Enemy C
                       Takes 14.7 damage (70% of 21)
                       └─ Chains to Enemy D
                              Takes 10.3 damage (70% of 14.7)

Visual: Yellow lines connect A→B→C→D
```

---

## How to Modify Player Stats

### Goal
Increase player health and arrow damage.

### Prerequisites
- Read `02_Player_System_Complete.md`
- Understand Player_Archer component

### Method 1: Direct Inspector Modification

**Easiest** but requires manual changes per instance.

1. Select Player_Archer GameObject in scene
2. In Inspector, find `Enemy` component (base class)
3. Modify:
   ```
   Health: 100 → 150
   ```

4. Find `Player_Archer` component
5. Modify:
   ```
   Arrow Damage: 20 → 30
   ```

6. Save scene

**Pros**: Quick, no code
**Cons**: Doesn't persist across levels, must change each level

### Method 2: Prefab Modification

**Better** - changes apply to all instances.

1. Find Player prefab: `Assets/Resources/Prefabs/Player_Archer.prefab`
2. Double-click to edit prefab
3. Modify settings (same as Method 1)
4. Save prefab (Ctrl+S)

**Pros**: Applies to all levels
**Cons**: Still manual

### Method 3: ScriptableObject Upgrade System

**Best** - dynamic upgrades with save system.

#### Step 1: Create UpgradeData ScriptableObject

Create `PlayerUpgradeData.cs`:

```csharp
using UnityEngine;

namespace RGame
{
    [CreateAssetMenu(fileName = "PlayerUpgrade", menuName = "RGame/Player Upgrade Data")]
    public class PlayerUpgradeData : ScriptableObject
    {
        [Header("Health")]
        public int baseHealth = 100;
        public int healthUpgradePerLevel = 10;

        [Header("Damage")]
        public float baseDamage = 20f;
        public float damageUpgradePerLevel = 2f;

        [Header("Attack Speed")]
        public float baseReloadTime = 0.5f;
        public float reloadTimeReduction = 0.05f;  // Faster per level

        public int GetHealth(int upgradeLevel)
        {
            return baseHealth + (healthUpgradePerLevel * upgradeLevel);
        }

        public float GetDamage(int upgradeLevel)
        {
            return baseDamage + (damageUpgradePerLevel * upgradeLevel);
        }

        public float GetReloadTime(int upgradeLevel)
        {
            return Mathf.Max(0.1f, baseReloadTime - (reloadTimeReduction * upgradeLevel));
        }
    }
}
```

#### Step 2: Create Upgrade Asset

1. Right-click in Project: `Create → RGame → Player Upgrade Data`
2. Name: `PlayerUpgradeData`
3. Configure base stats

#### Step 3: Modify Player_Archer to Use Upgrades

In `Player_Archer.cs`:

```csharp
public class Player_Archer : Enemy
{
    [Header("Upgrade Data")]
    public PlayerUpgradeData upgradeData;

    public override void Start()
    {
        base.Start();

        // Apply upgrades if data exists
        if (upgradeData != null)
        {
            int healthLevel = GlobalValue.GetPlayerHealthLevel();  // Get saved level
            int damageLevel = GlobalValue.GetPlayerDamageLevel();
            int speedLevel = GlobalValue.GetPlayerSpeedLevel();

            // Override health
            health = upgradeData.GetHealth(healthLevel);
            currentHealth = health;

            // Override damage
            arrowDamage = upgradeData.GetDamage(damageLevel);

            // Override reload time
            timeReload = upgradeData.GetReloadTime(speedLevel);

            // Update health bar
            if (healthBar)
                healthBar.UpdateValue(currentHealth / (float)health);
        }
    }
}
```

#### Step 4: Add Save/Load to GlobalValue

In `GlobalValue.cs`:

```csharp
public static class GlobalValue
{
    // Upgrade levels
    public static int playerHealthLevel = 0;
    public static int playerDamageLevel = 0;
    public static int playerSpeedLevel = 0;

    public static void UpgradePlayerHealth()
    {
        playerHealthLevel++;
        PlayerPrefs.SetInt("PlayerHealthLevel", playerHealthLevel);
    }

    public static void UpgradePlayerDamage()
    {
        playerDamageLevel++;
        PlayerPrefs.SetInt("PlayerDamageLevel", playerDamageLevel);
    }

    public static void UpgradePlayerSpeed()
    {
        playerSpeedLevel++;
        PlayerPrefs.SetInt("PlayerSpeedLevel", playerSpeedLevel);
    }

    public static int GetPlayerHealthLevel()
    {
        return PlayerPrefs.GetInt("PlayerHealthLevel", 0);
    }

    public static int GetPlayerDamageLevel()
    {
        return PlayerPrefs.GetInt("PlayerDamageLevel", 0);
    }

    public static int GetPlayerSpeedLevel()
    {
        return PlayerPrefs.GetInt("PlayerSpeedLevel", 0);
    }

    public static void ResetUpgrades()
    {
        playerHealthLevel = 0;
        playerDamageLevel = 0;
        playerSpeedLevel = 0;
        PlayerPrefs.DeleteAll();
    }
}
```

#### Step 5: Create Upgrade Shop UI

In shop UI, add upgrade buttons:

```csharp
public class ShopManager : MonoBehaviour
{
    public int healthUpgradeCost = 100;
    public int damageUpgradeCost = 150;
    public int speedUpgradeCost = 200;

    public void BuyHealthUpgrade()
    {
        if (GlobalValue.SavedCoins >= healthUpgradeCost)
        {
            GlobalValue.SavedCoins -= healthUpgradeCost;
            GlobalValue.UpgradePlayerHealth();

            SoundManager.PlaySfx(SoundManager.Instance.soundUpgrade);
            UpdateUI();
        }
        else
        {
            SoundManager.PlaySfx(SoundManager.Instance.soundNotEnoughCoin);
        }
    }

    // Similar for damage and speed
}
```

#### Step 6: Test

1. Open shop
2. Buy health upgrade (costs 100 coins)
3. Play level
4. Check player health is 110 (was 100, +10)
5. Buy damage upgrade
6. Check arrows deal more damage

### Expected Result

```
Initial Stats:
Health: 100
Damage: 20
Reload: 0.5s

After 1 Health Upgrade:
Health: 110
Damage: 20
Reload: 0.5s
Coins: -100

After 2 Damage Upgrades:
Health: 110
Damage: 24
Reload: 0.5s
Coins: -400 (100 + 150 + 150)

Stats persist across levels!
```

---

## How to Add a New Level

### Goal
Create Level 6 with custom enemy waves and mana.

### Prerequisites
- Read `05_Managers_Complete.md` (LevelEnemyManager section)
- Understand GameLevelSetup and LevelManager

### Step 1: Duplicate Existing Level Prefab

1. Find existing level: `Assets/Resources/Prefabs/Levels/Level_5.prefab`
2. Duplicate (Ctrl+D)
3. Rename: `Level_6.prefab`

### Step 2: Configure Level Settings

Double-click Level_6 prefab to edit:

```
GameLevelSetup component:
- Given Mana: 1500 (was 1000)
```

### Step 3: Design Enemy Waves

In LevelEnemyManager component:

```csharp
Wave 1: (Easy start)
  wait: 3
  EnemySpawns:
    - Goblin x5 (wait: 0, rate: 0.5)

Wave 2: (Medium difficulty)
  wait: 10
  EnemySpawns:
    - Skeleton x4 (wait: 0, rate: 1)
    - Goblin x3 (wait: 2, rate: 0.5)

Wave 3: (Hard)
  wait: 15
  EnemySpawns:
    - Troll x2 (wait: 0, rate: 2)
    - Bomber x3 (wait: 5, rate: 1.5)

Wave 4: (Boss wave)
  wait: 20
  EnemySpawns:
    - TrollBoss x1 (wait: 0, rate: 0)
    - Skeleton x5 (wait: 3, rate: 0.5)
```

### Step 4: Visually Configure Waves in Inspector

1. Select LevelEnemyManager in Level_6 prefab
2. Expand "Enemy Waves" array
3. Set Size: 4
4. Configure each wave:

```
Element 0 (Wave 1):
  Wait: 3
  Enemy Spawns (Size: 1):
    Element 0:
      Enemy: Goblin prefab
      Number Enemy: 5
      Wait: 0
      Rate: 0.5

Element 1 (Wave 2):
  Wait: 10
  Enemy Spawns (Size: 2):
    Element 0:
      Enemy: Skeleton prefab
      Number Enemy: 4
      Wait: 0
      Rate: 1
    Element 1:
      Enemy: Goblin prefab
      Number Enemy: 3
      Wait: 2
      Rate: 0.5

(Continue for waves 3 and 4)
```

### Step 5: Add Level to GameManager

1. Open Playing scene
2. Find GameManager GameObject
3. In Inspector, find "Game Levels" array
4. Increase Size to 6
5. Assign Level_6 prefab to Element 5

### Step 6: Update GlobalValue

In `GlobalValue.cs`:

```csharp
public static int finishGameAtLevel = 6;  // Was 5
```

### Step 7: Test Level Directly

To test without playing all previous levels:

```csharp
// Temporary: In GameManager.Awake()
GlobalValue.levelPlaying = 6;  // Force level 6
```

Or create test scene:

1. `File → New Scene`
2. Save as: `TestLevel6`
3. Add GameManager, MenuManager, Canvas
4. Set GameManager → Game Levels[0] = Level_6
5. Play

### Step 8: Calculate Total Enemies for Balance

```
Wave 1: 5 enemies
Wave 2: 4 + 3 = 7 enemies
Wave 3: 2 + 3 = 5 enemies
Wave 4: 1 + 5 = 6 enemies

Total: 23 enemies

Mana given: 1500
Average mana per enemy: 1500 / 23 ≈ 65 mana
(Good if units cost 50-100 mana each)
```

### Expected Result

```
Level 6 Timeline:

0:00  - Game starts, given 1500 mana
0:03  - Wave 1: Goblin x5 spawn (over 2.5s)
0:13  - Wave 2: Skeleton x4 spawn, then Goblin x3
0:28  - Wave 3: Troll x2, then Bomber x3
0:48  - Wave 4: TrollBoss spawns, then Skeleton x5
1:15  - All enemies defeated → Victory!

Difficulty: Medium-Hard (Boss at end)
```

---

## How to Add Power-Up Items

### Goal
Create health pack and speed boost power-ups that drop from enemies.

### Prerequisites
- Read `03_Enemy_System_Complete.md`
- Understand collision and pickup systems

### Step 1: Create PowerUp Base Script

Create `PowerUp.cs` in `Assets/_MonstersOut/Scripts/`:

```csharp
using UnityEngine;

namespace RGame
{
    public enum POWERUP_TYPE
    {
        HEALTH,
        SPEED,
        DAMAGE,
        INVINCIBILITY
    }

    [RequireComponent(typeof(CircleCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class PowerUp : MonoBehaviour
    {
        [Header("Settings")]
        public POWERUP_TYPE powerUpType;
        public float value = 25f;          // Health amount or boost %
        public float duration = 5f;         // For temporary buffs
        public AudioClip pickupSound;

        [Header("Visual")]
        public GameObject pickupEffect;
        public float rotateSpeed = 100f;
        public float bobSpeed = 2f;
        public float bobAmount = 0.3f;

        private float bobTimer;
        private Vector3 startPos;

        void Start()
        {
            startPos = transform.position;

            // Setup physics
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0;  // Float in air
            rb.isKinematic = true;

            // Setup collider as trigger
            CircleCollider2D col = GetComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.5f;

            // Auto-destroy after 10 seconds
            Destroy(gameObject, 10f);
        }

        void Update()
        {
            // Rotate
            transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);

            // Bob up and down
            bobTimer += Time.deltaTime * bobSpeed;
            float newY = startPos.y + Mathf.Sin(bobTimer) * bobAmount;
            transform.position = new Vector3(
                transform.position.x,
                newY,
                transform.position.z
            );
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            // Check if player picked up
            if (other.CompareTag("Player"))
            {
                var player = other.GetComponent<Player_Archer>();
                if (player != null)
                {
                    ApplyPowerUp(player);
                    Pickup();
                }
            }
        }

        void ApplyPowerUp(Player_Archer player)
        {
            switch (powerUpType)
            {
                case POWERUP_TYPE.HEALTH:
                    player.Heal((int)value);
                    FloatingTextManager.Instance.ShowText(
                        "+" + (int)value + " HP",
                        Vector3.up,
                        Color.green,
                        player.transform.position
                    );
                    break;

                case POWERUP_TYPE.SPEED:
                    player.StartCoroutine(player.SpeedBoost(value / 100f, duration));
                    FloatingTextManager.Instance.ShowText(
                        "SPEED UP!",
                        Vector3.up,
                        Color.cyan,
                        player.transform.position
                    );
                    break;

                case POWERUP_TYPE.DAMAGE:
                    player.StartCoroutine(player.DamageBoost(value / 100f, duration));
                    FloatingTextManager.Instance.ShowText(
                        "POWER UP!",
                        Vector3.up,
                        Color.red,
                        player.transform.position
                    );
                    break;

                case POWERUP_TYPE.INVINCIBILITY:
                    player.StartCoroutine(player.Invincibility(duration));
                    FloatingTextManager.Instance.ShowText(
                        "INVINCIBLE!",
                        Vector3.up,
                        Color.yellow,
                        player.transform.position
                    );
                    break;
            }
        }

        void Pickup()
        {
            // Play sound
            if (pickupSound)
                SoundManager.PlaySfx(pickupSound);

            // Spawn effect
            if (pickupEffect)
                Instantiate(pickupEffect, transform.position, Quaternion.identity);

            // Destroy pickup
            Destroy(gameObject);
        }
    }
}
```

### Step 2: Add Boost Methods to Player_Archer

In `Player_Archer.cs`, add:

```csharp
public class Player_Archer : Enemy
{
    private float speedMultiplier = 1f;
    private float damageMultiplier = 1f;
    private bool isInvincible = false;

    public IEnumerator SpeedBoost(float multiplier, float duration)
    {
        speedMultiplier = 1f + multiplier;  // e.g., 50% = 1.5x
        yield return new WaitForSeconds(duration);
        speedMultiplier = 1f;
    }

    public IEnumerator DamageBoost(float multiplier, float duration)
    {
        damageMultiplier = 1f + multiplier;
        yield return new WaitForSeconds(duration);
        damageMultiplier = 1f;
    }

    public IEnumerator Invincibility(float duration)
    {
        isInvincible = true;

        // Blink effect
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        float timer = 0;
        while (timer < duration)
        {
            sprite.color = new Color(1, 1, 1, 0.5f);
            yield return new WaitForSeconds(0.1f);
            sprite.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            timer += 0.2f;
        }

        isInvincible = false;
    }

    // Modify existing methods to use multipliers:
    public override void FixedUpdate()
    {
        // Apply speed multiplier
        velocity.x = direction.x * moveSpeed * speedMultiplier;
        // ... rest of movement
    }

    void Shoot()
    {
        // Apply damage multiplier
        float finalDamage = arrowDamage * damageMultiplier;
        // ... spawn arrow with finalDamage
    }

    public override void TakeDamage(...)
    {
        // Check invincibility
        if (isInvincible)
            return;

        // Normal damage
        base.TakeDamage(...);
    }
}
```

### Step 3: Create PowerUp Prefabs

**Health Pack**:
1. Create sprite: Heart icon
2. Create GameObject: `PowerUp_Health`
3. Add components:
   - SpriteRenderer (heart sprite)
   - CircleCollider2D (trigger)
   - Rigidbody2D (kinematic)
   - PowerUp script
4. Configure PowerUp:
   ```
   PowerUp Type: HEALTH
   Value: 25
   Pickup Sound: Heal sound
   ```
5. Save as prefab

**Speed Boost**:
- Similar to Health Pack
- Use lightning bolt sprite
- PowerUp Type: SPEED
- Value: 50 (50% speed boost)
- Duration: 5

### Step 4: Add Drop System to Enemy

In `Enemy.cs`, add:

```csharp
[Header("Drops")]
public GameObject[] possibleDrops;
[Range(0, 100)]
public float dropChance = 20f;  // 20% chance

public virtual void Die()
{
    // ... existing code ...

    // Check if should drop item
    if (possibleDrops.Length > 0 && Random.Range(0, 100f) < dropChance)
    {
        // Random drop
        GameObject drop = possibleDrops[Random.Range(0, possibleDrops.Length)];
        Instantiate(drop, transform.position + Vector3.up, Quaternion.identity);
    }

    // ... existing code ...
}
```

### Step 5: Configure Enemy Drops

1. Select enemy prefab (e.g., Goblin)
2. Configure drops:
   ```
   Possible Drops (Size: 2):
     Element 0: PowerUp_Health
     Element 1: PowerUp_Speed
   Drop Chance: 20
   ```

### Step 6: Test

1. Play game
2. Kill enemies
3. About 20% should drop power-ups
4. Walk player over power-up
5. Verify:
   - ✓ Health pack heals player
   - ✓ Speed boost increases movement
   - ✓ Effects show floating text
   - ✓ Power-ups rotate and bob
   - ✓ Power-ups auto-destroy after 10s

### Expected Result

```
Enemy dies:
  20% chance → spawns power-up
  Power-up: Rotates, bobs up/down

Player touches power-up:
  Health Pack: Player heals +25 HP
               Green "+25 HP" text appears

  Speed Boost: Player moves 50% faster for 5 seconds
               Cyan "SPEED UP!" text appears
               Normal speed returns after 5s
```

---

## How to Create Custom Health Bars

### Goal
Create a fancy health bar with background, fill, damage overlay, and smooth animations.

### Prerequisites
- Read `04_UI_System_Complete.md` (Health Bar section)
- Understand Canvas and UI components

### Step 1: Create Health Bar Prefab

1. Create empty GameObject: `FancyHealthBar`
2. Add Canvas component:
   ```
   Render Mode: World Space
   Width: 100
   Height: 20
   Scale: 0.01, 0.01, 0.01
   ```

3. Add child UI elements:

```
FancyHealthBar (Canvas)
├─ Background (Image)
│   └─ Color: Dark gray (0.2, 0.2, 0.2)
├─ DamageOverlay (Image)
│   └─ Color: Red (1, 0, 0, 0.5)
├─ HealthFill (Image)
│   └─ Color: Green (0, 1, 0)
└─ Border (Image)
    └─ Color: White outline
```

### Step 2: Create FancyHealthBar Script

Create `FancyHealthBar.cs`:

```csharp
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace RGame
{
    public class FancyHealthBar : MonoBehaviour
    {
        [Header("References")]
        public Image healthFill;
        public Image damageOverlay;
        public Image background;

        [Header("Settings")]
        public float updateSpeed = 5f;
        public float damageDelay = 0.5f;
        public float hideDelay = 2f;
        public float fadeSpeed = 2f;

        private float targetFill = 1f;
        private float damageFill = 1f;
        private Transform target;
        private Vector3 offset;
        private CanvasGroup canvasGroup;

        void Start()
        {
            // Add canvas group for fading
            canvasGroup = GetComponent<CanvasGroup>();
            if (!canvasGroup)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();

            // Start hidden
            canvasGroup.alpha = 0;
        }

        public void Initialize(Transform _target, Vector3 _offset)
        {
            target = _target;
            offset = _offset;
        }

        void Update()
        {
            // Follow target
            if (target)
            {
                transform.position = target.position + offset;

                // Face camera
                transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                                Camera.main.transform.rotation * Vector3.up);
            }

            // Smooth health fill
            if (healthFill.fillAmount != targetFill)
            {
                healthFill.fillAmount = Mathf.Lerp(
                    healthFill.fillAmount,
                    targetFill,
                    updateSpeed * Time.deltaTime
                );
            }

            // Smooth damage overlay (delayed)
            if (damageOverlay.fillAmount != targetFill)
            {
                damageOverlay.fillAmount = Mathf.Lerp(
                    damageOverlay.fillAmount,
                    targetFill,
                    updateSpeed * 0.5f * Time.deltaTime  // Slower than health
                );
            }
        }

        public void UpdateHealth(float currentHealth, float maxHealth)
        {
            // Show bar
            StopAllCoroutines();
            canvasGroup.alpha = 1;

            // Calculate fill amount
            targetFill = Mathf.Clamp01(currentHealth / maxHealth);

            // Start damage overlay animation
            StartCoroutine(DamageOverlayCo());

            // Auto-hide after delay
            if (targetFill > 0)
                StartCoroutine(HideBarCo());
            else
                gameObject.SetActive(false);  // Dead
        }

        IEnumerator DamageOverlayCo()
        {
            // Wait before damage overlay catches up
            yield return new WaitForSeconds(damageDelay);

            // Smoothly reduce damage overlay
            float timer = 0;
            float startFill = damageOverlay.fillAmount;

            while (timer < 1f)
            {
                timer += Time.deltaTime * updateSpeed;
                damageOverlay.fillAmount = Mathf.Lerp(startFill, targetFill, timer);
                yield return null;
            }
        }

        IEnumerator HideBarCo()
        {
            yield return new WaitForSeconds(hideDelay);

            // Fade out
            float timer = 0;
            while (timer < 1f)
            {
                timer += Time.deltaTime * fadeSpeed;
                canvasGroup.alpha = Mathf.Lerp(1, 0, timer);
                yield return null;
            }
        }
    }
}
```

### Step 3: Set Up UI Elements

Configure RectTransforms:

```
Background:
- Anchor: Stretch (fills parent)
- Offset: 0, 0, 0, 0
- Image Type: Sliced (optional border)

HealthFill:
- Anchor: Left
- Image Type: Filled
- Fill Method: Horizontal
- Fill Origin: Left
- Fill Amount: 1

DamageOverlay:
- Same as HealthFill
- Color: Semi-transparent red

Border:
- Stretch to fill
- Sprite: Border sprite
```

### Step 4: Use in Enemy

In `Enemy.cs` Start() method, replace health bar spawning:

```csharp
public virtual void Start()
{
    // ... existing code ...

    // Spawn fancy health bar
    var healthBarPrefab = (FancyHealthBar)Resources.Load("FancyHealthBar", typeof(FancyHealthBar));
    var fancyBar = Instantiate(healthBarPrefab, transform.position + (Vector3)healthBarOffset, Quaternion.identity);
    fancyBar.Initialize(transform, healthBarOffset);

    // Store reference (need to modify Enemy class)
    fancyHealthBar = fancyBar;

    // ... existing code ...
}
```

### Step 5: Update Health Bar Calls

Modify `TakeDamage()` to use fancy health bar:

```csharp
public void TakeDamage(...)
{
    // ... existing code ...

    currentHealth -= (int)damage;

    // Update fancy health bar
    if (fancyHealthBar)
        fancyHealthBar.UpdateHealth(currentHealth, health);

    // ... existing code ...
}
```

### Expected Result

```
Enemy takes damage:
  Green bar shrinks immediately → 70%
  Red overlay stays at 100% for 0.5s
  Red overlay smoothly shrinks → 70%
  Health bar visible for 2 seconds
  Health bar fades out

Visual:
Before damage:  [████████████████████] 100%
After damage:   [██████████████      ] 70% (green)
                [████████████████    ] ~85% (red overlay)
                (red overlay catches up over time)
```

---

## How to Add Sound Effects

### Goal
Add footstep sounds to player movement and hurt sounds to enemy damage.

### Prerequisites
- Read `05_Managers_Complete.md` (SoundManager section)
- Have audio files ready

### Step 1: Import Audio Files

1. Import audio clips:
   ```
   Assets/Audio/Sound/Player/
   ├─ footstep1.wav
   ├─ footstep2.wav
   ├─ footstep3.wav
   ├─ hurt1.wav
   └─ hurt2.wav
   ```

2. Select all audio files in Project window
3. Inspector settings:
   ```
   Force To Mono: true (for sound effects)
   Load Type: Decompress On Load (small files)
   Compression Format: PCM (best quality for short sounds)
   ```

### Step 2: Add Sound Fields to Player

In `Player_Archer.cs`:

```csharp
public class Player_Archer : Enemy
{
    [Header("Sounds")]
    public AudioClip[] footstepSounds;
    [Range(0, 1)]
    public float footstepVolume = 0.3f;
    public float footstepInterval = 0.4f;

    private float footstepTimer = 0f;

    // ... existing code ...
}
```

### Step 3: Play Footstep Sounds

In `Player_Archer.cs` FixedUpdate():

```csharp
public override void FixedUpdate()
{
    base.FixedUpdate();

    // ... existing movement code ...

    // Play footsteps when moving
    if (Mathf.Abs(velocity.x) > 0.1f && controller.collisions.below)
    {
        footstepTimer += Time.fixedDeltaTime;

        if (footstepTimer >= footstepInterval)
        {
            // Play random footstep
            if (footstepSounds.Length > 0)
            {
                SoundManager.PlaySfx(footstepSounds, footstepVolume);
            }

            footstepTimer = 0f;
        }
    }
    else
    {
        footstepTimer = 0f;
    }

    // ... rest of movement
}
```

### Step 4: Add Randomized Pitch

For variety, add pitch randomization:

```csharp
public static void PlaySfxWithPitch(AudioClip clip, float volume, float pitchVariation = 0.1f)
{
    if (Instance != null && clip != null)
    {
        // Randomize pitch
        Instance.soundFx.pitch = Random.Range(1f - pitchVariation, 1f + pitchVariation);

        // Play sound
        Instance.soundFx.PlayOneShot(clip, volume);

        // Reset pitch
        Instance.soundFx.pitch = 1f;
    }
}
```

Use it:

```csharp
SoundManager.PlaySfxWithPitch(
    footstepSounds[Random.Range(0, footstepSounds.Length)],
    footstepVolume,
    0.15f  // ±15% pitch variation
);
```

### Step 5: Add to SoundManager

In `SoundManager.cs`, add convenience method:

```csharp
public static void PlayFootstep(AudioClip[] clips, float volume = 0.3f)
{
    if (clips == null || clips.Length == 0)
        return;

    // Random clip
    AudioClip clip = clips[Random.Range(0, clips.Length)];

    // Random pitch for variety
    Instance.soundFx.pitch = Random.Range(0.9f, 1.1f);
    Instance.soundFx.PlayOneShot(clip, volume * SoundVolume);
    Instance.soundFx.pitch = 1f;
}
```

Simplified usage:

```csharp
SoundManager.PlayFootstep(footstepSounds, footstepVolume);
```

### Step 6: Assign in Inspector

1. Select Player_Archer in scene
2. Find "Sounds" header
3. Set Footstep Sounds array size: 3
4. Drag footstep1/2/3 to array slots
5. Set Footstep Volume: 0.3
6. Set Footstep Interval: 0.4

### Step 7: Test

1. Play game
2. Move player left/right
3. Verify:
   - ✓ Footsteps play every 0.4 seconds while moving
   - ✓ Random footstep clips play
   - ✓ No footsteps when standing still
   - ✓ No footsteps when in air
   - ✓ Pitch varies slightly

---

## How to Implement Save/Load System

### Goal
Save player progress, unlocked levels, and coins using PlayerPrefs.

### Prerequisites
- Understand GlobalValue static class
- Basic knowledge of serialization

### Step 1: Create SaveData Class

Create `SaveData.cs`:

```csharp
using UnityEngine;
using System;

namespace RGame
{
    [Serializable]
    public class SaveData
    {
        // Player Progress
        public int currentLevel = 1;
        public int highestLevelUnlocked = 1;
        public int totalCoins = 0;

        // Player Upgrades
        public int healthLevel = 0;
        public int damageLevel = 0;
        public int speedLevel = 0;

        // Level Stars (3 stars per level)
        public int[] levelStars = new int[10];  // 10 levels

        // Settings
        public bool soundEnabled = true;
        public bool musicEnabled = true;

        // Statistics
        public int totalKills = 0;
        public int totalDeaths = 0;
        public float totalPlayTime = 0f;

        // Constructor with defaults
        public SaveData()
        {
            // Initialize level stars array
            for (int i = 0; i < levelStars.Length; i++)
                levelStars[i] = 0;
        }
    }
}
```

### Step 2: Create SaveSystem

Create `SaveSystem.cs`:

```csharp
using UnityEngine;

namespace RGame
{
    public static class SaveSystem
    {
        private const string SAVE_KEY = "GameSaveData";

        public static void Save(SaveData data)
        {
            // Convert to JSON
            string json = JsonUtility.ToJson(data);

            // Save to PlayerPrefs
            PlayerPrefs.SetString(SAVE_KEY, json);
            PlayerPrefs.Save();

            Debug.Log("Game Saved!");
        }

        public static SaveData Load()
        {
            // Check if save exists
            if (PlayerPrefs.HasKey(SAVE_KEY))
            {
                // Load JSON
                string json = PlayerPrefs.GetString(SAVE_KEY);

                // Convert to object
                SaveData data = JsonUtility.FromJson<SaveData>(json);

                Debug.Log("Game Loaded!");
                return data;
            }
            else
            {
                // No save file, return new save
                Debug.Log("No save file found. Creating new save.");
                return new SaveData();
            }
        }

        public static void DeleteSave()
        {
            PlayerPrefs.DeleteKey(SAVE_KEY);
            PlayerPrefs.Save();
            Debug.Log("Save deleted!");
        }

        public static bool HasSave()
        {
            return PlayerPrefs.HasKey(SAVE_KEY);
        }
    }
}
```

### Step 3: Integrate with GlobalValue

In `GlobalValue.cs`, modify to use SaveSystem:

```csharp
public static class GlobalValue
{
    // Current save data
    private static SaveData currentSave;

    // Properties that read/write to save data
    public static int levelPlaying
    {
        get => currentSave.currentLevel;
        set
        {
            currentSave.currentLevel = value;
            SaveGame();
        }
    }

    public static int SavedCoins
    {
        get => currentSave.totalCoins;
        set
        {
            currentSave.totalCoins = value;
            SaveGame();
        }
    }

    public static bool isSound
    {
        get => currentSave.soundEnabled;
        set
        {
            currentSave.soundEnabled = value;
            SaveGame();
        }
    }

    public static bool isMusic
    {
        get => currentSave.musicEnabled;
        set
        {
            currentSave.musicEnabled = value;
            SaveGame();
        }
    }

    // Initialize (call on game start)
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        currentSave = SaveSystem.Load();
    }

    // Save game
    public static void SaveGame()
    {
        SaveSystem.Save(currentSave);
    }

    // Get level stars
    public static int GetLevelStars(int level)
    {
        if (level >= 1 && level <= currentSave.levelStars.Length)
            return currentSave.levelStars[level - 1];
        return 0;
    }

    // Set level stars
    public static void SetLevelStars(int level, int stars)
    {
        if (level >= 1 && level <= currentSave.levelStars.Length)
        {
            // Only save if better than previous
            if (stars > currentSave.levelStars[level - 1])
            {
                currentSave.levelStars[level - 1] = stars;
                SaveGame();
            }
        }
    }

    // Reset all data
    public static void ResetAllData()
    {
        SaveSystem.DeleteSave();
        currentSave = new SaveData();
    }
}
```

### Step 4: Auto-Save on Key Events

In `GameManager.cs` Victory():

```csharp
public void Victory()
{
    // ... existing code ...

    // Save level progress
    if (GlobalValue.levelPlaying > GlobalValue.currentSave.highestLevelUnlocked)
    {
        GlobalValue.currentSave.highestLevelUnlocked = GlobalValue.levelPlaying + 1;
    }

    // Save stars
    GlobalValue.SetLevelStars(GlobalValue.levelPlaying, levelStarGot);

    // Save game
    GlobalValue.SaveGame();

    // ... existing code ...
}
```

In shop when buying upgrades:

```csharp
public void BuyHealthUpgrade()
{
    if (GlobalValue.SavedCoins >= healthUpgradeCost)
    {
        GlobalValue.SavedCoins -= healthUpgradeCost;  // Auto-saves
        GlobalValue.UpgradePlayerHealth();
    }
}
```

### Step 5: Add Manual Save/Load Buttons

In main menu:

```csharp
public class MainMenuHomeScene : MonoBehaviour
{
    public void OnSaveButtonClick()
    {
        GlobalValue.SaveGame();
        ShowMessage("Game Saved!");
    }

    public void OnLoadButtonClick()
    {
        // Reload scene to apply loaded data
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        ShowMessage("Game Loaded!");
    }

    public void OnResetDataButtonClick()
    {
        if (ConfirmDialog("Reset all progress?"))
        {
            GlobalValue.ResetAllData();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
```

### Step 6: Test Save System

1. Play game, complete level 1 with 3 stars
2. Collect 100 coins
3. Quit game (close Unity or build)
4. Restart game
5. Verify:
   - ✓ Level 2 is unlocked
   - ✓ 100 coins still there
   - ✓ Level 1 shows 3 stars
   - ✓ Settings (sound/music) preserved

### Expected Behavior

```
PlayerPrefs data (stored in registry/plist):
{
  "currentLevel": 2,
  "highestLevelUnlocked": 2,
  "totalCoins": 100,
  "healthLevel": 1,
  "damageLevel": 0,
  "speedLevel": 0,
  "levelStars": [3, 0, 0, 0, 0, 0, 0, 0, 0, 0],
  "soundEnabled": true,
  "musicEnabled": false,
  "totalKills": 47,
  "totalDeaths": 2,
  "totalPlayTime": 125.5
}

Saved to: PlayerPrefs["GameSaveData"]
Location (Windows): Registry HKCU\Software\[CompanyName]\[ProductName]
```

---

## How to Change Game Difficulty

### Goal
Add Easy/Normal/Hard difficulty modes with different enemy health and damage.

### Prerequisites
- Understand UpgradedCharacterParameter
- Read Enemy System documentation

### Step 1: Create Difficulty Enum

Create `GameDifficulty.cs`:

```csharp
namespace RGame
{
    public enum GameDifficulty
    {
        Easy,
        Normal,
        Hard,
        Insane
    }

    public static class DifficultySettings
    {
        // Current difficulty
        public static GameDifficulty CurrentDifficulty = GameDifficulty.Normal;

        // Enemy health multipliers
        public static float GetHealthMultiplier()
        {
            switch (CurrentDifficulty)
            {
                case GameDifficulty.Easy:
                    return 0.7f;  // 70% health
                case GameDifficulty.Normal:
                    return 1.0f;  // 100% health
                case GameDifficulty.Hard:
                    return 1.5f;  // 150% health
                case GameDifficulty.Insane:
                    return 2.0f;  // 200% health
                default:
                    return 1.0f;
            }
        }

        // Enemy damage multipliers
        public static float GetDamageMultiplier()
        {
            switch (CurrentDifficulty)
            {
                case GameDifficulty.Easy:
                    return 0.5f;  // 50% damage
                case GameDifficulty.Normal:
                    return 1.0f;  // 100% damage
                case GameDifficulty.Hard:
                    return 1.25f; // 125% damage
                case GameDifficulty.Insane:
                    return 1.5f;  // 150% damage
                default:
                    return 1.0f;
            }
        }

        // Enemy speed multipliers
        public static float GetSpeedMultiplier()
        {
            switch (CurrentDifficulty)
            {
                case GameDifficulty.Easy:
                    return 0.8f;  // 80% speed
                case GameDifficulty.Normal:
                    return 1.0f;  // 100% speed
                case GameDifficulty.Hard:
                    return 1.1f;  // 110% speed
                case GameDifficulty.Insane:
                    return 1.3f;  // 130% speed
                default:
                    return 1.0f;
            }
        }

        // Coin rewards
        public static float GetCoinMultiplier()
        {
            switch (CurrentDifficulty)
            {
                case GameDifficulty.Easy:
                    return 0.8f;  // 80% coins
                case GameDifficulty.Normal:
                    return 1.0f;  // 100% coins
                case GameDifficulty.Hard:
                    return 1.5f;  // 150% coins
                case GameDifficulty.Insane:
                    return 2.0f;  // 200% coins
                default:
                    return 1.0f;
            }
        }
    }
}
```

### Step 2: Apply Difficulty to Enemies

In `Enemy.cs` Start() method:

```csharp
public virtual void Start()
{
    // ... existing code ...

    // Apply difficulty multiplier
    health = (int)(health * DifficultySettings.GetHealthMultiplier());
    currentHealth = health;

    walkSpeed *= DifficultySettings.GetSpeedMultiplier();

    // ... rest of Start()
}
```

### Step 3: Apply to Enemy Attacks

In `EnemyMeleeAttack.cs`:

```csharp
void Start()
{
    // ... existing code ...

    // Apply difficulty to damage
    dealDamage *= DifficultySettings.GetDamageMultiplier();
}
```

In `EnemyRangeAttack.cs`:

```csharp
void Start()
{
    // ... existing code ...

    damage *= DifficultySettings.GetDamageMultiplier();
}
```

### Step 4: Apply to Coin Rewards

In `GiveCoinWhenDie.cs`:

```csharp
public void GiveCoin()
{
    // Calculate coins with difficulty multiplier
    int coinAmount = Random.Range(coinGiveMin, coinGiveMax + 1);
    coinAmount = (int)(coinAmount * DifficultySettings.GetCoinMultiplier());

    GlobalValue.SavedCoins += coinAmount;

    // ... spawn coin effect
}
```

### Step 5: Create Difficulty Selection UI

In main menu, add difficulty buttons:

```csharp
public class DifficultySelector : MonoBehaviour
{
    public Text currentDifficultyText;

    void Start()
    {
        UpdateDifficultyText();
    }

    public void SetEasy()
    {
        DifficultySettings.CurrentDifficulty = GameDifficulty.Easy;
        UpdateDifficultyText();
        SoundManager.Click();
    }

    public void SetNormal()
    {
        DifficultySettings.CurrentDifficulty = GameDifficulty.Normal;
        UpdateDifficultyText();
        SoundManager.Click();
    }

    public void SetHard()
    {
        DifficultySettings.CurrentDifficulty = GameDifficulty.Hard;
        UpdateDifficultyText();
        SoundManager.Click();
    }

    public void SetInsane()
    {
        DifficultySettings.CurrentDifficulty = GameDifficulty.Insane;
        UpdateDifficultyText();
        SoundManager.Click();
    }

    void UpdateDifficultyText()
    {
        currentDifficultyText.text = "Difficulty: " + DifficultySettings.CurrentDifficulty.ToString();

        // Color coding
        switch (DifficultySettings.CurrentDifficulty)
        {
            case GameDifficulty.Easy:
                currentDifficultyText.color = Color.green;
                break;
            case GameDifficulty.Normal:
                currentDifficultyText.color = Color.white;
                break;
            case GameDifficulty.Hard:
                currentDifficultyText.color = new Color(1f, 0.5f, 0f);  // Orange
                break;
            case GameDifficulty.Insane:
                currentDifficultyText.color = Color.red;
                break;
        }
    }
}
```

### Step 6: Save Difficulty Setting

Add to `SaveData.cs`:

```csharp
public class SaveData
{
    // ... existing fields ...

    public int difficultyLevel = 1;  // 0=Easy, 1=Normal, 2=Hard, 3=Insane

    // ... rest of class
}
```

Add to `GlobalValue.cs`:

```csharp
public static GameDifficulty Difficulty
{
    get => (GameDifficulty)currentSave.difficultyLevel;
    set
    {
        currentSave.difficultyLevel = (int)value;
        DifficultySettings.CurrentDifficulty = value;
        SaveGame();
    }
}
```

### Step 7: Test Difficulties

**Easy Mode** (Level 1):
- Goblin health: 70 (was 100)
- Goblin damage: 10 (was 20)
- Goblin speed: 2.4 (was 3)
- Coins: 8 (was 10)

**Hard Mode** (Level 1):
- Goblin health: 150 (was 100)
- Goblin damage: 25 (was 20)
- Goblin speed: 3.3 (was 3)
- Coins: 15 (was 10)

**Insane Mode** (Level 1):
- Goblin health: 200 (was 100)
- Goblin damage: 30 (was 20)
- Goblin speed: 3.9 (was 3)
- Coins: 20 (was 10)

### Expected Result

```
Main Menu:
┌──────────────────────────┐
│   Select Difficulty:     │
│                          │
│  [Easy]   (Green)        │
│  [Normal] (White) ✓      │
│  [Hard]   (Orange)       │
│  [Insane] (Red)          │
│                          │
│  Current: Normal         │
└──────────────────────────┘

In-Game (Normal → Hard):
Before: Goblin has 100 HP
After:  Goblin has 150 HP (+50%)
```

---

## Summary

This How-To Guides document covered:

1. **Adding New Enemy Type** - Created flying Ghost enemy
2. **Custom UI Panel** - Statistics panel with kill tracking
3. **New Weapon Effect** - Lightning chains between enemies
4. **Modifying Player Stats** - 3 methods (Inspector, Prefab, ScriptableObject)
5. **Adding New Level** - Custom wave configuration
6. **Power-Up Items** - Health packs and speed boosts with drop system
7. **Custom Health Bars** - Fancy health bars with damage overlay
8. **Sound Effects** - Footsteps and hurt sounds
9. **Save/Load System** - JSON-based PlayerPrefs save system
10. **Difficulty Modes** - Easy/Normal/Hard/Insane with multipliers

All guides include:
- Clear goals and prerequisites
- Step-by-step instructions
- Complete code examples
- Expected results and testing steps

**Next Steps**:
- Use these guides as templates for your own modifications
- Combine multiple guides for complex features
- Read troubleshooting guide for common issues

---

**Last Updated**: 2025
**File**: `Documents/10_How_To_Guides.md`
