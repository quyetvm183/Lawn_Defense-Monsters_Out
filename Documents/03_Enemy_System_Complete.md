# Enemy System - Complete Guide

> **For**: Beginners who finished Unity Fundamentals
> **Read Time**: 40-50 minutes
> **Prerequisites**: 00_Unity_Fundamentals.md, 01_Project_Architecture.md

---

## Table of Contents
1. [System Overview](#system-overview)
2. [Enemy Architecture](#enemy-architecture)
3. [State Machine System](#state-machine-system)
4. [Effect System (Burn, Freeze, Poison, Shock)](#effect-system)
5. [Attack Modules (Melee, Range, Throw)](#attack-modules)
6. [Movement & AI](#movement--ai)
7. [Health & Damage System](#health--damage-system)
8. [Animation Control](#animation-control)
9. [Inspector Configuration](#inspector-configuration)
10. [How to Create Custom Enemy](#how-to-create-custom-enemy)
11. [Common Issues & Solutions](#common-issues--solutions)

---

## System Overview

### What is the Enemy System?

The Enemy System is the **AI-controlled opponent system** in this game. Enemies:
- **Walk** toward the player's fortress
- **Detect** the player when in range
- **Attack** using Melee, Range, or Throw attacks
- **Take damage** and apply visual/sound effects
- **Respond** to weapon effects (Freeze, Burn, Poison, Shock)
- **Die** with animations and drop coins

### Why is this Important?

The Enemy System is the **core challenge** of the game. Understanding how enemies work allows you to:
- Create new enemy types
- Adjust difficulty by changing enemy stats
- Add new attack patterns
- Implement new weapon effects
- Debug AI behavior issues

### System Architecture Diagram

```
┌─────────────────────────────────────────────────────────┐
│                  ENEMY BASE CLASS                       │
│  (Health, Effects, State Machine, IListener)            │
└───────────────────┬─────────────────────────────────────┘
                    │
          ┌─────────┴─────────┐
          │                   │
          ▼                   ▼
┌──────────────────┐   ┌─────────────────┐
│ SmartEnemy       │   │ Player_Archer   │ (unusual!)
│ Grounded         │   │ (inherits Enemy)│
└────────┬─────────┘   └─────────────────┘
         │
         │ Uses these Attack Modules:
         │
    ┌────┼─────┬─────────┬─────────┐
    │    │     │         │         │
    ▼    ▼     ▼         ▼         ▼
┌──────┐┌────┐┌───────┐┌───────┐┌──────┐
│Melee ││Range│Throw  ││Check  ││Spawn │
│Attack││Attack│Attack ││Target ││Item  │
└──────┘└────┘└───────┘└───────┘└──────┘
```

### Key Files

| File | Location | Purpose |
|------|----------|---------|
| `Enemy.cs` | `Assets/_MonstersOut/Scripts/AI/` | Base class for ALL enemies |
| `SmartEnemyGrounded.cs` | `Assets/_MonstersOut/Scripts/AI/` | Main enemy implementation |
| `EnemyMeleeAttack.cs` | `Assets/_MonstersOut/Scripts/AI/` | Melee attack module |
| `EnemyRangeAttack.cs` | `Assets/_MonstersOut/Scripts/AI/` | Range attack module |
| `EnemyThrowAttack.cs` | `Assets/_MonstersOut/Scripts/AI/` | Throw attack module |
| `CheckTargetHelper.cs` | `Assets/_MonstersOut/Scripts/Helpers/` | Target detection helper |

---

## Enemy Architecture

### Inheritance Hierarchy

```
MonoBehaviour
    │
    ├─ ICanTakeDamage (interface)
    │       │
    │       └─ Allows any object to receive damage
    │
    ├─ IListener (interface)
    │       │
    │       └─ Receives game state events (Pause, GameOver, etc.)
    │
    └─ Enemy (base class)
            │
            ├─ Implements ICanTakeDamage
            ├─ Implements IListener
            ├─ Has health system
            ├─ Has effect system
            ├─ Has state machine
            │
            └─ SmartEnemyGrounded (main implementation)
                    │
                    ├─ Adds movement logic
                    ├─ Adds attack logic
                    └─ Adds AI behavior
```

### Why This Design?

**Unusual Design Choice**: The `Enemy` base class is used by BOTH enemies AND the player!

**Reason**: Code reuse. Both enemies and player need:
- Health system
- Damage system
- Effect system (freeze, burn, poison, shock)
- Animation system

**Trade-off**:
- **Pro**: Less duplicate code, easier maintenance
- **Con**: Confusing naming (player inherits from "Enemy" class)

---

## State Machine System

### What is a State Machine?

A **State Machine** is a system where an object can be in ONE state at a time, and transitions between states based on events.

**Example**: Enemy states
- IDLE → Player detected → WALK
- WALK → Player in range → ATTACK
- ATTACK → Attack finished → WALK
- Any state → Health = 0 → DEATH

### Enemy States (ENEMYSTATE Enum)

Located in `Enemy.cs:18-26`

```csharp
public enum ENEMYSTATE
{
    SPAWNING,  // Enemy is spawning (burrow up animation)
    IDLE,      // Enemy is standing still
    ATTACK,    // Enemy is attacking
    WALK,      // Enemy is walking
    HIT,       // Enemy was hit (not actively used in code)
    DEATH      // Enemy is dead
}
```

### State Flow Diagram

```
┌─────────┐
│ SPAWNING│ (burrow up animation plays)
└────┬────┘
     │ spawnDelay (1 second)
     ▼
┌─────────┐
│  IDLE   │ (standing still, waiting)
└────┬────┘
     │ Player detected
     ▼
┌─────────┐
│  WALK   │───────────┐ (walking toward fortress)
└────┬────┘           │
     │                │
     │ Player in      │ Attack finished
     │ attack range   │
     │                │
     ▼                │
┌─────────┐           │
│ ATTACK  │───────────┘ (attacking player)
└────┬────┘
     │ Health = 0
     ▼
┌─────────┐
│  DEATH  │ (play death animation, give coins)
└─────────┘
```

### How States are Set

**SetEnemyState() Method** (`Enemy.cs:234-237`)

```csharp
public void SetEnemyState(ENEMYSTATE state)
{
    enemyState = state;  // Simply update the state variable
}
```

**Usage Example**: Start behavior in `Enemy.cs:189-207`

```csharp
switch (startBehavior)
{
    case STARTBEHAVIOR.BURROWUP:
        SoundManager.PlaySfx(soundSpawn, soundSpawnVol);
        SetEnemyState(ENEMYSTATE.SPAWNING);  // Set state to SPAWNING
        AnimSetTrigger("spawn");              // Trigger spawn animation
        Invoke("FinishSpawning", spawnDelay); // Call FinishSpawning after delay
        break;

    case STARTBEHAVIOR.NONE:
    case STARTBEHAVIOR.WALK_LEFT:
    case STARTBEHAVIOR.WALK_RIGHT:
        SetEnemyState(ENEMYSTATE.WALK);  // Set state to WALK immediately
        break;
}
```

**How it Works**:
1. Enemy spawns
2. `Start()` checks `startBehavior` setting
3. If `BURROWUP`, set state to `SPAWNING` and play spawn animation
4. After `spawnDelay` seconds, `FinishSpawning()` is called
5. `FinishSpawning()` sets state to `WALK`

**FinishSpawning() Method** (`Enemy.cs:210-214`)

```csharp
void FinishSpawning()
{
    // Only transition if still in SPAWNING state and game is playing
    if (enemyState == ENEMYSTATE.SPAWNING && isPlaying)
        SetEnemyState(ENEMYSTATE.WALK);
}
```

### State Usage in Update Loop

**SmartEnemyGrounded.Update()** (`SmartEnemyGrounded.cs:83-97`)

```csharp
public override void Update()
{
    base.Update();  // Call parent Update (handles effects)

    HandleAnimation();  // Update animation based on velocity

    // If NOT in WALK state, stop moving
    if (enemyState != ENEMYSTATE.WALK || GameManager.Instance.State != GameManager.GameState.Playing)
    {
        velocity.x = 0;  // Set horizontal velocity to zero
        return;           // Exit early
    }

    // If in WALK state and player detected, start chasing
    if (checkTarget.CheckTarget(isFacingRight() ? 1 : -1))
        DetectPlayer(delayChasePlayerWhenDetect);
}
```

**How it Works**:
- Every frame, check current state
- If state is NOT `WALK`, set velocity to 0 (stop moving)
- If state IS `WALK`, allow movement and check for player

---

## Effect System

### What are Effects?

**Effects** are temporary status conditions applied to enemies by weapon attacks. They change enemy behavior and deal damage over time.

### Effect Types (ENEMYEFFECT Enum)

Located in `Enemy.cs:28-36`

```csharp
public enum ENEMYEFFECT
{
    NONE,        // No effect active
    BURNING,     // Damage over time (DoT) per frame
    FREEZE,      // Enemy can't move, animation plays
    SHOKING,     // Damage over time + stun
    POISON,      // Damage over time per second + slow movement
    EXPLOSION    // Visual explosion on death
}
```

### Effect System Diagram

```
WEAPON ATTACK
     │
     ├─ Has WeaponEffect component?
     │       │
     │       NO──→ Normal damage only
     │       │
     │       YES
     │       │
     │       └─ Check effectType:
     │           │
     │           ├─ FREEZE ──→ Freeze(time)
     │           ├─ POISON ──→ Poison(damage, time)
     │           ├─ BURN ───→ Burning(damage)
     │           └─ SHOCK ──→ Shoking(damage)
     │
     ▼
EFFECT ACTIVE
     │
     ├─ Update() checks enemyEffect every frame
     ├─ Apply damage/behavior changes
     └─ After duration, remove effect
```

### Effect Priority System

**Only ONE effect can be active at a time**. Effects can override each other:

**Freeze Effect** (`Enemy.cs:416-434`)

```csharp
public virtual void Freeze(float time, GameObject instigator)
{
    // Can't freeze if already frozen
    if (enemyEffect == ENEMYEFFECT.FREEZE)
        return;

    // If burning, stop burning first
    if (enemyEffect == ENEMYEFFECT.BURNING)
        BurnOut();

    // If shocking, stop shocking first
    if (enemyEffect == ENEMYEFFECT.SHOKING)
    {
        UnShock();
    }

    // Apply freeze if enemy can be frozen
    if (canBeFreeze)
    {
        enemyEffect = ENEMYEFFECT.FREEZE;  // Set current effect
        StartCoroutine(UnFreezeCo(time));   // Start timer to unfreeze
    }
}
```

**How it Works**:
1. Check if already frozen → return early
2. If currently burning/shocking → cancel those effects
3. Set `enemyEffect = FREEZE`
4. Start coroutine to remove effect after `time` seconds

**Effect Interaction Rules**:
- **Freeze** cancels Burn and Shock
- **Burn** cancels Freeze and Shock
- **Poison** cancels Freeze and Shock
- **Shock** cancels Freeze and Burn
- **Explosion** is only applied on death

### Freeze Effect (Detailed)

**Freeze() Method** (`Enemy.cs:416-434`)

```csharp
public virtual void Freeze(float time, GameObject instigator)
{
    // Early return if already frozen
    if (enemyEffect == ENEMYEFFECT.FREEZE)
        return;

    // Cancel conflicting effects
    if (enemyEffect == ENEMYEFFECT.BURNING)
        BurnOut();

    if (enemyEffect == ENEMYEFFECT.SHOKING)
    {
        UnShock();
    }

    // Apply freeze if allowed
    if (canBeFreeze)
    {
        enemyEffect = ENEMYEFFECT.FREEZE;
        StartCoroutine(UnFreezeCo(time));
    }
}
```

**UnFreezeCo() Coroutine** (`Enemy.cs:436-445`)

```csharp
IEnumerator UnFreezeCo(float time)
{
    AnimSetBool("isFreezing", true);  // Enable freeze animation

    // Safety check (in case effect was cancelled)
    if (enemyEffect != ENEMYEFFECT.FREEZE)
        yield break;

    yield return new WaitForSeconds(time);  // Wait for duration
    UnFreeze();  // Remove freeze effect
}
```

**UnFreeze() Method** (`Enemy.cs:447-454`)

```csharp
void UnFreeze()
{
    // Safety check
    if (enemyEffect != ENEMYEFFECT.FREEZE)
        return;

    enemyEffect = ENEMYEFFECT.NONE;        // Clear effect
    AnimSetBool("isFreezing", false);       // Disable freeze animation
}
```

**Movement Prevention** (`SmartEnemyGrounded.cs:118-119`)

```csharp
// In LateUpdate, prevent movement if frozen
if (enemyState != ENEMYSTATE.WALK || enemyEffect == ENEMYEFFECT.FREEZE)
    targetVelocityX = 0;
```

**Timeline Diagram**:
```
Frame 100: Freeze(3.0f) called
           ├─ enemyEffect = FREEZE
           ├─ AnimSetBool("isFreezing", true)
           └─ Start coroutine with 3 second wait

Frame 101-280: (3 seconds = 180 frames at 60fps)
           ├─ Every frame: Update() checks enemyEffect
           ├─ LateUpdate() sets targetVelocityX = 0
           └─ Freeze animation plays

Frame 281: Coroutine finishes
           ├─ UnFreeze() called
           ├─ enemyEffect = NONE
           ├─ AnimSetBool("isFreezing", false)
           └─ Enemy can move again
```

### Burn Effect (Detailed)

**Burning() Method** (`Enemy.cs:459-481`)

```csharp
public virtual void Burning(float damage, GameObject instigator)
{
    // Can't burn if already burning
    if (enemyEffect == ENEMYEFFECT.BURNING)
        return;

    // Cancel conflicting effects
    if (enemyEffect == ENEMYEFFECT.FREEZE)
    {
        UnFreeze();
    }

    if (enemyEffect == ENEMYEFFECT.SHOKING)
    {
        UnShock();
    }

    // Apply burn if allowed
    if (canBeBurn)
    {
        damageBurningPerFrame = damage;      // Store damage amount
        enemyEffect = ENEMYEFFECT.BURNING;   // Set effect

        StartCoroutine(BurnOutCo(1));        // Burn lasts 1 second
    }
}
```

**Damage Application** (`Enemy.cs:244-247`)

```csharp
public virtual void Update()
{
    // Apply burn damage EVERY FRAME
    if (enemyEffect == ENEMYEFFECT.BURNING)
        CheckDamagePerFrame(damageBurningPerFrame);

    // Apply shock damage EVERY FRAME
    if (enemyEffect == ENEMYEFFECT.SHOKING)
        CheckDamagePerFrame(damageShockingPerFrame);
}
```

**CheckDamagePerFrame() Method** (`Enemy.cs:361-372`)

```csharp
private void CheckDamagePerFrame(float _damage)
{
    // Don't apply damage if already dead
    if (enemyState == ENEMYSTATE.DEATH)
        return;

    currentHealth -= (int)_damage;  // Reduce health

    // Update health bar
    if (healthBar)
        healthBar.UpdateValue(currentHealth / (float)health);

    // Check if dead
    if (currentHealth <= 0)
        Die();
}
```

**BurnOutCo() Coroutine** (`Enemy.cs:483-499`)

```csharp
IEnumerator BurnOutCo(float time)
{
    // Safety check
    if (enemyEffect != ENEMYEFFECT.BURNING)
        yield break;

    yield return new WaitForSeconds(time);  // Wait 1 second

    // If enemy died while burning, disable GameObject
    if (enemyState == ENEMYSTATE.DEATH)
    {
        BurnOut();
        gameObject.SetActive(false);
    }

    BurnOut();  // Remove burn effect
}
```

**Burn Damage Calculation Example**:
```
Enemy Health: 100
Burn Damage Per Frame: 0.5
Frame Rate: 60 FPS
Burn Duration: 1 second

Total Frames: 60 frames
Total Damage: 0.5 × 60 = 30 damage
Final Health: 100 - 30 = 70
```

### Poison Effect (Detailed)

**Poison() Method** (`Enemy.cs:511-536`)

```csharp
public virtual void Poison(float damage, float time, GameObject instigator)
{
    // Can't poison if already poisoned or burning
    if (enemyEffect == ENEMYEFFECT.BURNING)
        return;

    if (enemyEffect == ENEMYEFFECT.POISON)
        return;

    // Cancel conflicting effects
    if (enemyEffect == ENEMYEFFECT.FREEZE)
    {
        UnFreeze();
    }

    if (enemyEffect == ENEMYEFFECT.SHOKING)
    {
        UnShock();
    }

    // Apply poison if allowed
    if (canBePoison)
    {
        damagePoisonPerSecond = damage;      // Store damage per second
        enemyEffect = ENEMYEFFECT.POISON;    // Set effect

        StartCoroutine(PoisonCo(time));      // Start poison timer
    }
}
```

**PoisonCo() Coroutine** (`Enemy.cs:538-575`)

```csharp
IEnumerator PoisonCo(float time)
{
    AnimSetBool("isPoisoning", true);        // Enable poison animation
    multipleSpeed = 1 - poisonSlowSpeed;     // Slow movement (default 30%)

    // Safety check
    if (enemyEffect != ENEMYEFFECT.POISON)
        yield break;

    int wait = (int)time;  // Convert to integer seconds

    // Apply damage every second
    while (wait > 0)
    {
        yield return new WaitForSeconds(1);  // Wait 1 second

        // Calculate damage with resistance
        int _damage = (int)(damagePoisonPerSecond
                      * Random.Range(90 - resistPoisonPercent, 100f - resistPoisonPercent)
                      * 0.01f);

        currentHealth -= _damage;  // Apply damage

        // Update health bar
        if (healthBar)
            healthBar.UpdateValue(currentHealth / (float)health);

        // Show damage number
        FloatingTextManager.Instance.ShowText("" + (int)_damage,
                                               healthBarOffset,
                                               Color.red,
                                               transform.position);

        // Check if dead
        if (currentHealth <= 0)
        {
            PoisonEnd();
            Die();
            yield break;
        }

        wait -= 1;  // Decrement timer
    }

    // Poison duration finished
    if (enemyState == ENEMYSTATE.DEATH)
    {
        BurnOut();  // (This seems like a bug - should be PoisonEnd)
        gameObject.SetActive(false);
    }

    PoisonEnd();  // Remove poison effect
}
```

**Poison Damage Calculation Example**:
```
Base Poison Damage Per Second: 10
Poison Duration: 5 seconds
Resist Poison Percent: 10%

Damage Range Per Second:
  Min: 10 × (90 - 10) × 0.01 = 10 × 0.80 = 8
  Max: 10 × (100 - 10) × 0.01 = 10 × 0.90 = 9

Total Damage Over 5 Seconds: 8-9 damage × 5 seconds = 40-45 damage

Movement Speed:
  poisonSlowSpeed = 0.3 (30% slow)
  multipleSpeed = 1 - 0.3 = 0.7 (70% of normal speed)
```

**Movement Slow** (`SmartEnemyGrounded.cs:131`)

```csharp
// Movement is multiplied by multipleSpeed
controller.Move(velocity * Time.deltaTime * multipleSpeed, false, isFacingRight());
```

### Shock Effect (Detailed)

**Shoking() Method** (`Enemy.cs:591-610`)

```csharp
public virtual void Shoking(float damage, GameObject instigator)
{
    // Can't shock if already shocking
    if (enemyEffect == ENEMYEFFECT.SHOKING)
        return;

    // Cancel conflicting effects
    if (enemyEffect == ENEMYEFFECT.FREEZE)
    {
        UnFreeze();
    }

    if (enemyEffect == ENEMYEFFECT.BURNING)
        BurnOut();

    // Apply shock if allowed
    if (canBeShock)
    {
        damageShockingPerFrame = damage;      // Store damage per frame
        enemyEffect = ENEMYEFFECT.SHOKING;    // Set effect
        StartCoroutine(UnShockCo());          // Start shock timer
    }
}
```

**Damage Application** (`Enemy.cs:249-250`)

```csharp
// In Update(), apply shock damage every frame (same as burn)
if (enemyEffect == ENEMYEFFECT.SHOKING)
    CheckDamagePerFrame(damageShockingPerFrame);
```

**Movement Prevention** (`SmartEnemyGrounded.cs:105-109`)

```csharp
// In LateUpdate, prevent movement if shocking
else if (!isPlaying || isSocking || enemyEffect == ENEMYEFFECT.SHOKING)
{
    velocity = Vector2.zero;  // Stop completely
    return;
}
```

**UnShockCo() Coroutine** (`Enemy.cs:612-620`)

```csharp
IEnumerator UnShockCo()
{
    // Safety check
    if (enemyEffect != ENEMYEFFECT.SHOKING)
        yield break;

    yield return new WaitForSeconds(timeShocking);  // Default 2 seconds

    UnShock();  // Remove shock effect
}
```

**Shock Damage Calculation Example**:
```
Shock Damage Per Frame: 0.3
Shock Duration: 2 seconds
Frame Rate: 60 FPS

Total Frames: 60 × 2 = 120 frames
Total Damage: 0.3 × 120 = 36 damage

Behavior:
- Enemy CANNOT move (velocity = 0)
- Enemy CANNOT attack
- Takes 36 damage over 2 seconds
```

### Explosion Effect

**Explosion** is a special effect that only triggers on death.

**TakeDamage() Method** (`Enemy.cs:690-698`)

```csharp
if (currentHealth <= 0)
{
    // Check if enemy should explode on death
    if (isExplosion || dieBehavior == DIEBEHAVIOR.BLOWUP)
    {
        SetEnemyEffect(ENEMYEFFECT.EXPLOSION);
    }

    Die();
}
```

**Die() Method with Explosion** (`Enemy.cs:337-355`)

```csharp
// If explosion effect is active
if (enemyEffect == ENEMYEFFECT.EXPLOSION)
{
    // Spawn blood puddles
    if (bloodPuddleFX)
    {
        for (int i = 0; i < Random.Range(2, 5); i++)
        {
            Instantiate(bloodPuddleFX,
                       (Vector2)transform.position + new Vector2(
                           Random.Range(-(randomBloodPuddlePoint.x * 2), randomBloodPuddlePoint.x * 2),
                           Random.Range(-(2 * randomBloodPuddlePoint.y), 2 * randomBloodPuddlePoint.y)
                       ),
                       Quaternion.identity);
        }
    }

    // Spawn explosion effects
    if (explosionFX.Length > 0)
    {
        for (int i = 0; i < Random.Range(1, 3); i++)
        {
            Instantiate(explosionFX[Random.Range(0, explosionFX.Length)],
                       transform.position,
                       Quaternion.identity);
        }
    }

    // Play explosion sound
    SoundManager.PlaySfx(soundDieBlow, soundDieBlowVol);
}
else
    SoundManager.PlaySfx(soundDie, soundDieVol);  // Normal death sound
```

---

## Attack Modules

### Attack System Overview

Enemies use **modular attack components** that attach to the enemy GameObject. This allows mixing and matching attack types without code duplication.

### Attack Types (ATTACKTYPE Enum)

Located in `Enemy.cs:10-16`

```csharp
public enum ATTACKTYPE
{
    RANGE,   // Shoot projectiles (guns)
    MELEE,   // Close-range attacks (swords, claws)
    THROW,   // Throw grenades/bombs
    NONE     // No attack (passive enemy)
}
```

### Attack Module Architecture

```
SmartEnemyGrounded
    │
    ├─ attackType = RANGE/MELEE/THROW
    │
    ├─ CheckAttack() method
    │       │
    │       └─ switch(attackType) {
    │           ├─ RANGE  → rangeAttack.Action()
    │           ├─ MELEE  → meleeAttack.Action()
    │           └─ THROW  → throwAttack.Action()
    │           }
    │
    └─ Attack Modules (GetComponent)
            │
            ├─ EnemyRangeAttack
            ├─ EnemyMeleeAttack
            └─ EnemyThrowAttack
```

### Melee Attack Module

**File**: `EnemyMeleeAttack.cs`

**Purpose**: Close-range attacks using CircleCast detection

**How it Works**:

1. **Detection** (`CheckPlayer()` method)
```csharp
public bool CheckPlayer(bool _isFacingRight)
{
    isFacingRight = _isFacingRight;

    // Cast circle at checkPoint to detect player
    RaycastHit2D hit = Physics2D.CircleCast(
        checkPoint.position,   // Center
        radiusCheck,           // Radius (default 1)
        Vector2.zero,          // Direction (none, just check area)
        0,                     // Distance (0 = only check at center)
        targetLayer            // LayerMask (usually player layer)
    );

    if (hit)
        return true;  // Player in range
    else
        return false; // No player detected
}
```

2. **Attack Execution** (`Check4Hit()` method)

```csharp
public void Check4Hit()
{
    // Find ALL targets in range (slightly larger radius)
    RaycastHit2D[] hits = Physics2D.CircleCastAll(
        checkPoint.position,
        radiusCheck * 1.2f,   // 20% larger for better hit detection
        Vector2.zero,
        0,
        targetLayer
    );

    int counterHit = 0;  // Track how many targets hit

    if (hits.Length > 0)
    {
        foreach (var hit in hits)
        {
            // Only hit up to maxTargetPerHit targets
            if (counterHit < maxTargetPerHit)
            {
                // Check if target can take damage
                var takeDamage = (ICanTakeDamage)hit.collider.gameObject
                                 .GetComponent(typeof(ICanTakeDamage));

                if (takeDamage != null)
                {
                    // Calculate damage with random variance
                    float _damage = dealDamage + (int)(Random.Range(-0.1f, 0.1f) * dealDamage);

                    // Critical hit check (default 10% chance)
                    if (Random.Range(0, 100) < criticalPercent)
                    {
                        _damage *= 2;  // Double damage
                        FloatingTextManager.Instance.ShowText(
                            "CRIT!",
                            Vector3.up,
                            Color.red,
                            hit.collider.gameObject.transform.position,
                            30
                        );
                    }

                    // Apply damage with weapon effect
                    if (hasWeaponEffect != null)
                    {
                        takeDamage.TakeDamage(_damage, Vector2.zero, hit.point,
                                              gameObject, BODYPART.NONE, hasWeaponEffect);
                    }
                    else
                        takeDamage.TakeDamage(_damage, Vector2.zero, hit.point, gameObject);

                    counterHit++;  // Increment hit counter
                }

                // Play attack sound
                if (soundAttacks.Length > 0)
                    SoundManager.PlaySfx(soundAttacks[Random.Range(0, soundAttacks.Length)],
                                        soundAttacksVol);
            }
        }
    }
}
```

**Attack Timeline**:
```
Frame 100: CheckPlayer() returns true (player in range)
           ├─ CheckAttack() called
           └─ Action() sets lastShoot = Time.time

Frame 101: AnimSetTrigger("melee") triggers melee animation
           └─ Animation plays

Frame 115: Animation Event calls AnimMeleeAttackStart()
           ├─ Check4Hit() called
           ├─ CircleCast detects player
           ├─ Calculate damage (18-22, or 36-44 if crit)
           ├─ Call player.TakeDamage()
           └─ Play attack sound

Frame 130: Animation Event calls AnimMeleeAttackEnd()
           ├─ EndCheck4Hit() called
           └─ Invoke("EndAttack", 1) schedules EndAttack in 1 second

Frame 190: (1 second later)
           ├─ EndAttack() called
           ├─ isAttacking = false
           └─ Enemy can attack again
```

**Inspector Settings**:
- `targetLayer`: What can be attacked (usually Player layer)
- `checkPoint`: Transform where circle is cast (usually in front of enemy)
- `radiusCheck`: How far melee reaches (default 1 meter)
- `dealDamage`: Base damage (default 20)
- `criticalPercent`: Chance to deal 2x damage (default 10%)
- `meleeRate`: Cooldown between attacks (default 1 second)

### Range Attack Module

**File**: `EnemyRangeAttack.cs`

**Purpose**: Shoot projectiles at player from distance

**How it Works**:

1. **Target Detection** (`CheckPlayer()` method)

```csharp
public bool CheckPlayer(bool isFacingRight)
{
    dir = isFacingRight ? Vector2.right : Vector2.left;
    bool isHit = false;

    // Find ALL enemies in huge circle radius
    RaycastHit2D[] hits = Physics2D.CircleCastAll(
        checkPoint.position,
        detectDistance,      // Default 5 meters
        Vector2.zero,
        0,
        enemyLayer           // Actually targets player layer (naming confusion)
    );

    if (hits.Length > 0)
    {
        float closestDistance = 99999;

        // Find closest target
        foreach (var obj in hits)
        {
            var checkEnemy = (ICanTakeDamage)obj.collider.gameObject
                            .GetComponent(typeof(ICanTakeDamage));

            if (checkEnemy != null)
            {
                // Calculate horizontal distance
                if (Mathf.Abs(obj.transform.position.x - checkPoint.position.x) < closestDistance)
                {
                    closestDistance = Mathf.Abs(obj.transform.position.x - checkPoint.position.x);

                    // Verify line of sight with raycast
                    var hit = Physics2D.Raycast(
                        checkPoint.position,
                        (obj.point - (Vector2)checkPoint.position),
                        detectDistance,
                        enemyLayer
                    );

                    // Draw debug ray (visible in Scene view)
                    Debug.DrawRay(
                        checkPoint.position,
                        (obj.point - (Vector2)checkPoint.position) * 100,
                        Color.red
                    );

                    // Save target position
                    _target = obj.collider.gameObject.transform.position;
                    isHit = true;
                }
            }
        }
    }

    return isHit;
}
```

2. **Projectile Spawning** (`Shoot()` method)

```csharp
public void Shoot(bool isFacingRight)
{
    // Start coroutine to handle multi-shot
    StartCoroutine(ShootCo(isFacingRight));
}

IEnumerator ShootCo(bool isFacingRight)
{
    // Loop for multi-shot (default 1)
    for (int i = 0; i < multiShoot; i++)
    {
        SoundManager.PlaySfx(soundShoot, soundShootVolume);

        // Calculate shoot angle (0 = right, 180 = left)
        float shootAngle = 0;
        shootAngle = isFacingRight ? 0 : 180;

        // Spawn bullet at shooting point
        var obj = Instantiate(
            bullet.gameObject,
            shootingPoint != null ? shootingPoint.position : firePosition(),
            Quaternion.Euler(0, 0, shootAngle)
        );

        var projectile = obj.GetComponent<Projectile>();

        // Calculate shooting direction
        Vector3 _dir;
        if (aimTarget)
        {
            // Aim at target position
            _dir = _target - shootingPoint.position;
            _dir += (Vector3)aimTargetOffset;  // Add offset (usually up)
            _dir.Normalize();                   // Make unit vector
        }
        else
        {
            // Shoot straight
            _dir = Vector2.right * (isFacingRight ? 1 : -1);
        }

        // Initialize projectile with weapon effect
        if (hasWeaponEffect != null)
        {
            projectile.Initialize(
                gameObject,           // Owner
                _dir,                 // Direction
                Vector2.zero,         // Force (not used)
                false,                // Is crit
                damage * 0.9f,        // Min damage
                damage * 1.1f,        // Max damage
                0,                    // Crit percent (handled separately)
                Vector2.zero,         // Knockback
                hasWeaponEffect       // Weapon effect
            );
        }
        else
            projectile.Initialize(gameObject, _dir, Vector2.zero, false,
                                 damage * 0.9f, damage * 1.1f, 0);

        projectile.gameObject.SetActive(true);

        // Wait before next shot (for multi-shot)
        yield return new WaitForSeconds(multiShootRate);
    }

    CancelInvoke();
    Invoke("EndAttack", 1);  // Mark attack as finished after 1 second
}
```

**Attack Flow**:
```
1. CheckPlayer() detects player in 5-meter radius
   └─ Verifies line of sight with raycast

2. Action() sets lastShoot time and isAttacking = true

3. AnimSetTrigger("shoot") plays shooting animation

4. Animation Event calls AnimShoot()

5. Shoot(isFacingRight) called
   └─ ShootCo() coroutine starts

6. For each bullet in multiShoot:
   ├─ Play shoot sound
   ├─ Spawn bullet prefab
   ├─ Calculate direction (aim or straight)
   ├─ Initialize projectile with damage/effects
   └─ Wait multiShootRate seconds

7. After all bullets, wait 1 second
   └─ EndAttack() sets isAttacking = false
```

**Inspector Settings**:
- `enemyLayer`: What to target (usually Player layer)
- `checkPoint`: Detection center point
- `firePoint`: Where bullet visually spawns (gun barrel)
- `shootingPoint`: Actual spawn position (can be different)
- `damage`: Base damage (default 30)
- `detectDistance`: How far can detect player (default 5)
- `bullet`: Projectile prefab to spawn
- `shootingRate`: Cooldown between attacks (default 1 second)
- `aimTarget`: If true, aims at player. If false, shoots straight
- `aimTargetOffset`: Offset for aiming (usually up to hit torso)

### Throw Attack Module

**File**: `EnemyThrowAttack.cs`

**Purpose**: Throw grenades/bombs using physics

**How it Works**:

1. **Target Detection** (`CheckPlayer()` method)

```csharp
public bool CheckPlayer()
{
    // Check for player in radius
    RaycastHit2D[] hits = Physics2D.CircleCastAll(
        checkPoint.position,
        radiusDetectPlayer,   // Default 5 meters
        Vector2.zero,
        0,
        targetPlayer
    );

    if (hits.Length > 0)
    {
        foreach (var hit in hits)
        {
            // If onlyAttackTheFortrest = true, only throw at fortress
            if (onlyAttackTheFortrest)
            {
                if (hit.collider.gameObject.GetComponent<TheFortrest>())
                    return true;
            }
            else
                return true;  // Attack any detected target
        }
    }
    return false;
}
```

2. **Throw Execution** (`Throw()` method)

```csharp
public void Throw(bool isFacingRight)
{
    // Get throw position
    Vector3 throwPos = throwPosition.position;

    // Spawn grenade prefab
    GameObject obj = Instantiate(_Grenade, throwPos, Quaternion.identity) as GameObject;

    // Calculate throw angle
    float angle;
    angle = isFacingRight ? angleThrow : 135;  // Default 60° or 135°

    // Rotate grenade to angle
    obj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

    // Apply force to Rigidbody2D
    obj.GetComponent<Rigidbody2D>().AddRelativeForce(
        obj.transform.right * Random.Range(throwForceMin, throwForceMax)
    );

    // Apply torque (rotation while flying)
    obj.GetComponent<Rigidbody2D>().AddTorque(
        obj.transform.right.x * addTorque
    );
}
```

**Physics Explanation**:

```
Grenade Trajectory:

                    ╱╲
                  ╱    ╲
                ╱        ╲
              ╱            ╲
Enemy ───────              ─────── Ground
        60°                Impact Point

Force: 290-320
Angle: 60° (if facing right)
Gravity: -35 (set in Rigidbody2D)

Formula:
  Horizontal Distance = (Force² × sin(2×Angle)) / Gravity
  Example: (300² × sin(120°)) / 35 ≈ 2.2 meters
```

**Attack Flow**:
```
1. CheckPlayer() detects fortress/player in range

2. Action() sets lastShoot time

3. AnimSetTrigger("throw") plays throw animation

4. Animation Event calls AnimThrow()

5. Throw(isFacingRight) called
   ├─ Spawn grenade at throwPosition
   ├─ Rotate to angle (60° or 135°)
   ├─ AddRelativeForce (moves along rotated direction)
   └─ AddTorque (spins grenade)

6. Grenade flies in arc (physics simulation)

7. Grenade lands and explodes (handled by grenade script)
```

**Inspector Settings**:
- `angleThrow`: Throw angle when facing right (default 60°)
- `throwForceMin/Max`: Force range (default 290-320)
- `addTorque`: Rotation speed (default 100)
- `throwRate`: Cooldown between throws (default 0.5 seconds)
- `throwPosition`: Where grenade spawns
- `_Grenade`: Grenade prefab to throw
- `targetPlayer`: What to detect
- `onlyAttackTheFortrest`: Only throw at fortress (not player)
- `radiusDetectPlayer`: Detection radius (default 5)

### Attack Checking System

**CheckAttack() Method** (`SmartEnemyGrounded.cs:188-258`)

```csharp
void CheckAttack()
{
    // Switch based on attackType enum
    switch (attackType)
    {
        case ATTACKTYPE.RANGE:
            // Check if cooldown finished
            if (rangeAttack.AllowAction())
            {
                // Set state to attacking
                SetEnemyState(ENEMYSTATE.ATTACK);

                // Check if player is in range
                if (rangeAttack.CheckPlayer(isFacingRight()))
                {
                    rangeAttack.Action();           // Start attack cooldown
                    AnimSetTrigger("shoot");        // Play animation
                    DetectPlayer();                 // Make sure player is marked as detected
                }
                else if (!rangeAttack.isAttacking && enemyState == ENEMYSTATE.ATTACK)
                {
                    // Player moved out of range, keep walking
                    SetEnemyState(ENEMYSTATE.WALK);
                }
            }
            break;

        case ATTACKTYPE.MELEE:
            if (meleeAttack.AllowAction())
            {
                if (meleeAttack.CheckPlayer(isFacingRight()))
                {
                    SetEnemyState(ENEMYSTATE.ATTACK);
                    meleeAttack.Action();
                    AnimSetTrigger("melee");
                }
                else if (!meleeAttack.isAttacking && enemyState == ENEMYSTATE.ATTACK)
                {
                    SetEnemyState(ENEMYSTATE.WALK);
                }
            }
            break;

        case ATTACKTYPE.THROW:
            if (throwAttack.AllowAction())
            {
                SetEnemyState(ENEMYSTATE.ATTACK);

                if (throwAttack.CheckPlayer())
                {
                    throwAttack.Action();
                    AnimSetTrigger("throw");
                }
                else if (!throwAttack.isAttacking && enemyState == ENEMYSTATE.ATTACK)
                {
                    SetEnemyState(ENEMYSTATE.WALK);
                }
            }
            break;

        default:
            break;
    }
}
```

**When is CheckAttack() Called?**

```csharp
// In LateUpdate() of SmartEnemyGrounded
if (isPlaying && isPlayerDetected && allowCheckAttack && enemyEffect != ENEMYEFFECT.FREEZE)
{
    CheckAttack();
}
```

**Conditions**:
- `isPlaying` = true (game is playing)
- `isPlayerDetected` = true (player was detected)
- `allowCheckAttack` = true (not currently performing special action)
- `enemyEffect != FREEZE` (not frozen)

---

## Movement & AI

### Movement System Overview

Enemies use **Controller2D** for custom physics-based movement (same as player).

### Movement Flow

```
LateUpdate() called every physics frame
    │
    ├─ Check if game is playing
    ├─ Check if enemy can move
    │   ├─ isPlaying?
    │   ├─ isSocking?
    │   └─ enemyEffect == FREEZE?
    │
    ├─ Calculate target velocity
    │   └─ targetVelocityX = direction.x × moveSpeed
    │
    ├─ Apply gravity
    │   └─ velocity.y += -gravity × Time.deltaTime
    │
    ├─ Smooth velocity
    │   └─ SmoothDamp for smooth acceleration
    │
    ├─ Check collisions
    │   └─ If hit wall, stop
    │
    └─ Move character
        └─ controller.Move(velocity × deltaTime × multipleSpeed)
```

### LateUpdate() Method (Complete)

Located in `SmartEnemyGrounded.cs:99-140`

```csharp
public virtual void LateUpdate()
{
    // If game is not playing, stop
    if (GameManager.Instance.State != GameManager.GameState.Playing)
        return;

    // If enemy is stopping or shocking, set velocity to zero
    else if (!isPlaying || isSocking || enemyEffect == ENEMYEFFECT.SHOKING)
    {
        velocity = Vector2.zero;
        return;
    }

    // Calculate target horizontal velocity
    float targetVelocityX = _direction.x * moveSpeed;

    // If shocking, stop
    if (isSocking || enemyEffect == ENEMYEFFECT.SHOKING)
    {
        targetVelocityX = 0;
    }

    // If not walking or frozen, stop
    if (enemyState != ENEMYSTATE.WALK || enemyEffect == ENEMYEFFECT.FREEZE)
        targetVelocityX = 0;

    // If manually stopped or stunned, stop
    if (isStopping || isStunning)
        targetVelocityX = 0;

    // Smooth velocity change (prevents instant acceleration)
    velocity.x = Mathf.SmoothDamp(
        velocity.x,              // Current velocity
        targetVelocityX,         // Target velocity
        ref velocityXSmoothing,  // Smoothing variable
        (controller.collisions.below) ? 0.1f : 0.2f  // Smoothing time
    );

    // Apply gravity
    velocity.y += -gravity * Time.deltaTime;

    // If hit wall, stop horizontal movement
    if ((_direction.x > 0 && controller.collisions.right) ||
        (_direction.x < 0 && controller.collisions.left))
        velocity.x = 0;

    // Move the character controller
    controller.Move(
        velocity * Time.deltaTime * multipleSpeed,  // Movement amount
        false,                                       // Not jumping
        isFacingRight()                              // Facing direction
    );

    // If hit ceiling or ground, stop vertical movement
    if (controller.collisions.above || controller.collisions.below)
        velocity.y = 0;

    // Check if can attack
    if (isPlaying && isPlayerDetected && allowCheckAttack && enemyEffect != ENEMYEFFECT.FREEZE)
    {
        CheckAttack();
    }
}
```

### Smooth Velocity Explanation

**Mathf.SmoothDamp()** creates smooth acceleration/deceleration.

```csharp
velocity.x = Mathf.SmoothDamp(
    velocity.x,              // Current: 0
    targetVelocityX,         // Target: 3
    ref velocityXSmoothing,  // Reference variable (stores state)
    0.1f                     // Time to reach target
);
```

**How it Works**:

```
Frame 1:  velocity = 0,    target = 3  →  velocity = 1.5
Frame 2:  velocity = 1.5,  target = 3  →  velocity = 2.4
Frame 3:  velocity = 2.4,  target = 3  →  velocity = 2.8
Frame 4:  velocity = 2.8,  target = 3  →  velocity = 2.95
Frame 5:  velocity = 2.95, target = 3  →  velocity = 3.0

Result: Smooth acceleration curve instead of instant jump
```

**Why Different Smoothing Times?**

```csharp
(controller.collisions.below) ? 0.1f : 0.2f
```

- **0.1f** (below = true): On ground → faster acceleration
- **0.2f** (below = false): In air → slower acceleration (more realistic)

### Direction & Facing

**Initial Direction** (`SmartEnemyGrounded.Start()` lines 43-49)

```csharp
// Get direction based on rotation
_direction = isFacingRight() ? Vector2.right : Vector2.left;

// If direction doesn't match start behavior, flip
if ((_direction == Vector2.right && startBehavior == STARTBEHAVIOR.WALK_LEFT) ||
    (_direction == Vector2.left && startBehavior == STARTBEHAVIOR.WALK_RIGHT))
{
    Flip();
}
```

**isFacingRight() Method** (`Enemy.cs:155-159`)

```csharp
public bool isFacingRight()
{
    // Check Y rotation: 180 = facing right, 0 = facing left
    return transform.rotation.eulerAngles.y == 180 ? true : false;
}
```

**Flip() Method** (`SmartEnemyGrounded.cs:142-147`)

```csharp
void Flip()
{
    // Reverse direction
    _direction = -_direction;

    // Update rotation
    transform.rotation = Quaternion.Euler(
        new Vector3(
            transform.rotation.x,
            isFacingRight() ? 0 : 180,  // Toggle between 0 and 180
            transform.rotation.z
        )
    );
}
```

**Rotation Diagram**:
```
Facing LEFT:                 Facing RIGHT:
Rotation.y = 0               Rotation.y = 180
     ◄───                         ───►
    Enemy                        Enemy

Sprite faces left            Sprite faces right
```

### Player Detection

**DetectPlayer() Method** (`Enemy.cs:257-263`)

```csharp
public virtual void DetectPlayer(float delayChase = 0)
{
    // If already detected, do nothing
    if (isPlayerDetected)
        return;

    // Chase player after delay
    StartCoroutine(DelayBeforeChasePlayer(delayChase));
}
```

**DelayBeforeChasePlayer() Coroutine** (`Enemy.cs:266-286`)

```csharp
protected IEnumerator DelayBeforeChasePlayer(float delay)
{
    yield return null;  // Wait one frame

    // Wait until not stopping or stunned
    while (isStopping || isStunning) { yield return null; }

    isPlayerDetected = true;  // Mark player as detected

    if (delay > 0)
    {
        // Stop moving during delay
        SetEnemyState(ENEMYSTATE.IDLE);

        // Wait delay time
        yield return new WaitForSeconds(delay);
    }

    // If already attacking, don't change state
    if (enemyState == ENEMYSTATE.ATTACK)
    {
        yield break;
    }

    // Start walking toward player
    SetEnemyState(ENEMYSTATE.WALK);
}
```

**Detection Flow**:
```
Frame 100: checkTarget.CheckTarget() returns true
           └─ DetectPlayer(1.0f) called

Frame 101: DelayBeforeChasePlayer coroutine starts
           ├─ isPlayerDetected = true
           ├─ SetEnemyState(IDLE)
           └─ Start 1 second wait

Frame 160: (1 second later)
           ├─ Check if attacking
           └─ SetEnemyState(WALK)

Frame 161+: Enemy walks toward player
            └─ CheckAttack() called every frame
```

---

## Health & Damage System

### Health Initialization

**Start() Method** (`Enemy.cs:170-187`)

```csharp
public virtual void Start()
{
    // If upgraded character ID exists, use upgraded health
    if (upgradedCharacterID != null)
    {
        health = upgradedCharacterID.UpgradeHealth;
    }

    currentHealth = health;  // Set current health to max

    moveSpeed = walkSpeed;   // Initialize move speed

    // Spawn health bar from Resources folder
    var healthBarObj = (HealthBarEnemyNew)Resources.Load("HealthBar", typeof(HealthBarEnemyNew));
    healthBar = (HealthBarEnemyNew)Instantiate(healthBarObj, healthBarOffset, Quaternion.identity);

    healthBar.Init(transform, (Vector3)healthBarOffset);  // Attach to enemy

    // Get components
    anim = GetComponent<Animator>();
    checkTarget = GetComponent<CheckTargetHelper>();

    // Handle start behavior (spawn animation, etc.)
    // ... (see State Machine section)
}
```

### TakeDamage() Method (Complete)

Located in `Enemy.cs:662-723`

```csharp
public void TakeDamage(float damage, Vector2 force, Vector2 hitPoint,
                       GameObject instigator, BODYPART bodyPart = BODYPART.NONE,
                       WeaponEffect weaponEffect = null)
{
    // No action if already dead
    if (enemyState == ENEMYSTATE.DEATH)
        return;

    // No action if manually stopped
    if (isStopping)
        return;

    // Store parameters
    _bodyPart = bodyPart;
    _bodyPartForce = force;
    _damage = damage;

    // Get hit point for effects
    hitPos = hitPoint;
    bool isExplosion = false;

    // Reduce health
    currentHealth -= (int)damage;

    // Show damage number
    FloatingTextManager.Instance.ShowText(
        "" + (int)damage,
        healthBarOffset,
        Color.red,
        transform.position
    );

    knockBackForce = force;

    // Spawn hit effect at random position near hit point
    if (hitFX)
        Instantiate(hitFX,
                   hitPos + new Vector2(
                       Random.Range(-randomHitPoint.x, randomHitPoint.x),
                       Random.Range(-randomHitPoint.y, randomHitPoint.y)
                   ),
                   Quaternion.identity);

    // Spawn blood puddle
    if (bloodPuddleFX)
        Instantiate(bloodPuddleFX,
                   (Vector2)transform.position + new Vector2(
                       Random.Range(-randomBloodPuddlePoint.x, randomBloodPuddlePoint.x),
                       Random.Range(-randomBloodPuddlePoint.y, randomBloodPuddlePoint.y)
                   ),
                   Quaternion.identity);

    // Update health bar
    if (healthBar)
        healthBar.UpdateValue(currentHealth / (float)health);

    // Check if dead
    if (currentHealth <= 0)
    {
        // Check if should explode on death
        if (isExplosion || dieBehavior == DIEBEHAVIOR.BLOWUP)
        {
            SetEnemyEffect(ENEMYEFFECT.EXPLOSION);
        }

        Die();
    }
    else
    {
        // If alive, check for weapon effects
        if (weaponEffect != null)
        {
            switch (weaponEffect.effectType)
            {
                case WEAPON_EFFECT.POISON:
                    // Apply poison
                    Poison(weaponEffect.poisonDamagePerSec,
                          weaponEffect.poisonTime,
                          instigator);
                    return;

                case WEAPON_EFFECT.FREEZE:
                    Freeze(weaponEffect.freezeTime, instigator);
                    return;

                case WEAPON_EFFECT.NORMAL:
                    break;

                default:
                    break;
            }
        }

        Hit(force);  // Call Hit method for non-lethal damage
    }
}
```

### Damage Flow Diagram

```
Weapon hits enemy
    │
    ├─ TakeDamage() called
    │
    ├─ Check if dead/stopping → return
    │
    ├─ currentHealth -= damage
    │
    ├─ Show damage number (FloatingText)
    │
    ├─ Spawn hit effect (blood splash)
    │
    ├─ Spawn blood puddle
    │
    ├─ Update health bar
    │
    ├─ Check health:
    │   │
    │   ├─ currentHealth <= 0?
    │   │   │
    │   │   YES─→ Die()
    │   │   │
    │   │   NO──→ Check weaponEffect:
    │   │         │
    │   │         ├─ POISON → Poison()
    │   │         ├─ FREEZE → Freeze()
    │   │         └─ NORMAL → Hit()
    │   │
    │   └─ End
    │
    └─ End
```

### Die() Method (Complete)

Located in `Enemy.cs:316-359`

```csharp
public virtual void Die()
{
    // Stop the game
    isPlaying = false;

    // Remove from listener list
    GameManager.Instance.RemoveListener(this);

    isPlayerDetected = false;

    SetEnemyState(ENEMYSTATE.DEATH);

    // Give coins (if has GiveCoinWhenDie component)
    if (GetComponent<GiveCoinWhenDie>())
    {
        GetComponent<GiveCoinWhenDie>().GiveCoin();
    }

    // Spawn death effect
    if (dieFX)
        Instantiate(dieFX, transform.position, dieFX.transform.rotation);

    // If died while frozen, spawn frozen death effect
    if (enemyEffect == ENEMYEFFECT.FREEZE && dieFrozenFX)
        Instantiate(dieFrozenFX, hitPos, Quaternion.identity);

    // If shocking, remove shock effect
    if (enemyEffect == ENEMYEFFECT.SHOKING)
        UnShock();

    // If explosion effect, spawn blood and explosions
    if (enemyEffect == ENEMYEFFECT.EXPLOSION)
    {
        // Spawn 2-5 blood puddles
        if (bloodPuddleFX)
        {
            for (int i = 0; i < Random.Range(2, 5); i++)
            {
                Instantiate(bloodPuddleFX,
                           (Vector2)transform.position + new Vector2(
                               Random.Range(-(randomBloodPuddlePoint.x * 2), randomBloodPuddlePoint.x * 2),
                               Random.Range(-(2 * randomBloodPuddlePoint.y), 2 * randomBloodPuddlePoint.y)
                           ),
                           Quaternion.identity);
            }
        }

        // Spawn 1-3 explosion effects
        if (explosionFX.Length > 0)
        {
            for (int i = 0; i < Random.Range(1, 3); i++)
            {
                Instantiate(explosionFX[Random.Range(0, explosionFX.Length)],
                           transform.position,
                           Quaternion.identity);
            }
        }

        // Play explosion sound
        SoundManager.PlaySfx(soundDieBlow, soundDieBlowVol);
    }
    else
        SoundManager.PlaySfx(soundDie, soundDieVol);  // Normal death sound
}
```

**SmartEnemyGrounded.Die() Override** (`SmartEnemyGrounded.cs:296-330`)

```csharp
public override void Die()
{
    // Stop if already dead (prevent double death)
    if (isDead)
        return;

    base.Die();  // Call parent Die()

    isDead = true;  // Mark as dead

    CancelInvoke();  // Cancel all scheduled calls

    // Disable all colliders
    var cols = GetComponents<BoxCollider2D>();
    foreach (var col in cols)
        col.enabled = false;

    // Spawn item drop (if has SpawnItemHelper)
    if (spawnItem && spawnItem.spawnWhenDie)
        spawnItem.Spawn();

    // Set death animation
    AnimSetBool("isDead", true);

    // 50% chance to use alternate death animation
    if (Random.Range(0, 2) == 1)
        AnimSetTrigger("die2");

    // If burning, return early (handled by burn effect)
    if (enemyEffect == ENEMYEFFECT.BURNING)
        return;

    // If explosion or destroy behavior, disable immediately
    if (enemyEffect == ENEMYEFFECT.EXPLOSION || dieBehavior == DIEBEHAVIOR.DESTROY)
    {
        gameObject.SetActive(false);
        return;
    }

    // Otherwise, wait for animation to finish
    StopAllCoroutines();
    StartCoroutine(DisableEnemy(AnimationHelper.getAnimationLength(anim, "Die") + 2f));
}
```

**DisableEnemy() Coroutine** (`SmartEnemyGrounded.cs:382-390`)

```csharp
IEnumerator DisableEnemy(float delay)
{
    // Wait for death animation to finish
    yield return new WaitForSeconds(delay);

    // Spawn disable effect (corpse disappear effect)
    if (disableFX)
        Instantiate(disableFX,
                   spawnDisableFX != null ? spawnDisableFX.position : transform.position,
                   Quaternion.identity);

    // Disable GameObject (return to pool or destroy)
    gameObject.SetActive(false);
}
```

**Death Timeline**:
```
Frame 100: currentHealth = 0
           ├─ Die() called
           ├─ isPlaying = false
           ├─ Remove from GameManager listeners
           ├─ SetEnemyState(DEATH)
           ├─ GiveCoin()
           ├─ Spawn death FX
           └─ Play death sound

Frame 101: SmartEnemyGrounded.Die() called
           ├─ isDead = true
           ├─ Disable all colliders
           ├─ AnimSetBool("isDead", true)
           ├─ Trigger death animation
           └─ Start DisableEnemy coroutine

Frame 102-220: (2 seconds @ 60fps)
           └─ Death animation plays

Frame 221: DisableEnemy coroutine finishes
           ├─ Spawn disableFX (corpse disappear)
           └─ gameObject.SetActive(false)
```

---

## Animation Control

### Animation Methods

**AnimSetTrigger()** (`Enemy.cs:216-220`)

```csharp
public void AnimSetTrigger(string name)
{
    if (anim)
        anim.SetTrigger(name);  // Trigger one-shot animation
}
```

**AnimSetBool()** (`Enemy.cs:222-226`)

```csharp
public void AnimSetBool(string name, bool value)
{
    if (anim)
        anim.SetBool(name, value);  // Set persistent bool parameter
}
```

**AnimSetFloat()** (`Enemy.cs:228-232`)

```csharp
public void AnimSetFloat(string name, float value)
{
    if (anim)
        anim.SetFloat(name, value);  // Set float parameter
}
```

### Animation Parameters

**Common Animation Parameters**:
- `speed` (float): Horizontal velocity magnitude → controls walk animation speed
- `spawn` (trigger): Play spawn/burrow up animation
- `shoot` (trigger): Play shooting animation
- `melee` (trigger): Play melee attack animation
- `throw` (trigger): Play throw animation
- `hit` (trigger): Play hit/hurt animation
- `stun` (trigger): Play stun animation
- `die2` (trigger): Alternate death animation
- `isDead` (bool): Persistent death state
- `isFreezing` (bool): Freeze effect animation
- `isPoisoning` (bool): Poison effect animation

### HandleAnimation() Method

Located in `SmartEnemyGrounded.cs:265-269`

```csharp
void HandleAnimation()
{
    // Update speed parameter based on velocity
    AnimSetFloat("speed", Mathf.Abs(velocity.x));
}
```

**How it Works**:
```
velocity.x = 0    → speed = 0   → Idle animation plays
velocity.x = 1.5  → speed = 1.5 → Walk animation plays at 50% speed
velocity.x = 3.0  → speed = 3.0 → Walk animation plays at 100% speed
velocity.x = -3.0 → speed = 3.0 → Walk animation plays (Abs removes negative)
```

### Animation Events

**Animation Events** are markers in Unity animations that call script methods at specific frames.

**Example**: Melee Attack Animation

```
Melee Animation (1 second duration)
│
├─ Frame 0:     Animation starts
├─ Frame 15:    Swing begins
├─ Frame 30:    Event: AnimMeleeAttackStart() ← Check for hit
├─ Frame 40:    Swing ends
├─ Frame 45:    Event: AnimMeleeAttackEnd() ← End attack
└─ Frame 60:    Animation ends
```

**AnimMeleeAttackStart()** (`SmartEnemyGrounded.cs:276-279`)

```csharp
// Called by Animation Event
public void AnimMeleeAttackStart()
{
    meleeAttack.Check4Hit();  // Check if hit player
}
```

**AnimMeleeAttackEnd()** (`SmartEnemyGrounded.cs:281-284`)

```csharp
// Called by Animation Event
public void AnimMeleeAttackEnd()
{
    meleeAttack.EndCheck4Hit();  // End attack cooldown
}
```

**Other Animation Events**:

```csharp
// Called by throw animation
public void AnimThrow()
{
    throwAttack.Throw(isFacingRight());
}

// Called by shoot animation
public void AnimShoot()
{
    rangeAttack.Shoot(isFacingRight());
}
```

---

## Inspector Configuration

### Enemy Base Settings

**Health Section**:
- `health`: Maximum health (default 100)
- `healthBarOffset`: Position of health bar above enemy (default 0, 1.5)

**Setup Section**:
- `gravity`: Falling acceleration (default 35)
- `walkSpeed`: Horizontal movement speed (default 3)

**Behavior Section**:
- `attackType`: RANGE, MELEE, THROW, or NONE
- `startBehavior`: BURROWUP, WALK_LEFT, WALK_RIGHT, or NONE
- `spawnDelay`: Time before spawning finishes (default 1 second)

**Effect Options**:
- `canBeFreeze`: Can be frozen? (default true)
- `canBeBurn`: Can be burned? (default true)
- `canBePoison`: Can be poisoned? (default true)
- `canBeShock`: Can be shocked? (default true)
- `resistPoisonPercent`: Reduce poison damage by % (default 10%)
- `poisonSlowSpeed`: Movement slow during poison (default 0.3 = 30%)
- `timeShocking`: Shock duration (default 2 seconds)

**Sound**:
- `soundHit`: Array of hit sound effects
- `soundHitVol`: Hit sound volume (0-1)
- `soundDie`: Array of death sound effects
- `soundDieVol`: Death sound volume (0-1)

### SmartEnemyGrounded Settings

Inherits all Enemy settings, plus:

**Attack Modules** (assigned via GetComponent):
- `EnemyRangeAttack`: Range attack module
- `EnemyMeleeAttack`: Melee attack module
- `EnemyThrowAttack`: Throw attack module

**Visual Objects**:
- GunObj: Visual gun object (enabled if RANGE attack)
- MeleeObj: Visual melee weapon (enabled if MELEE attack)

### UpgradedCharacterParameter

**What is it?**

A **ScriptableObject** that stores enemy stat upgrades for difficulty progression.

**Fields**:
- `UpgradeHealth`: Upgraded max health
- `UpgradeMeleeDamage`: Upgraded melee damage
- `UpgradeCriticalDamage`: Upgraded critical hit chance
- `UpgradeRangeDamage`: Upgraded range damage
- `weaponEffect`: Weapon effect to apply
- `maxTargetPerHit`: How many targets can be hit at once

**How it's Used** (`SmartEnemyGrounded.Start()` lines 68-80):

```csharp
// Override stats with upgraded values
if (upgradedCharacterID != null)
{
    if (meleeAttack)
    {
        meleeAttack.dealDamage = upgradedCharacterID.UpgradeMeleeDamage;
        meleeAttack.criticalPercent = upgradedCharacterID.UpgradeCriticalDamage;
    }
    if (rangeAttack)
    {
        rangeAttack.damage = upgradedCharacterID.UpgradeRangeDamage;
    }
}
```

**Why Use This?**

Instead of manually adjusting each enemy prefab, you can:
1. Create difficulty ScriptableObjects (Easy, Normal, Hard)
2. Assign to `upgradedCharacterID` field
3. Same enemy prefab becomes stronger automatically

---

## How to Create Custom Enemy

### Step-by-Step Guide

#### Step 1: Create Enemy Prefab

1. Create empty GameObject: `GameObject → Create Empty`
2. Name it: `Enemy_MyNewEnemy`
3. Add Sprite Renderer:
   - `Add Component → Sprite Renderer`
   - Assign your enemy sprite
4. Add Animator:
   - `Add Component → Animator`
   - Create Animation Controller
5. Add Collider:
   - `Add Component → Box Collider 2D`
   - Adjust size to fit sprite
   - Set as trigger if needed

#### Step 2: Add Enemy Scripts

1. Add `SmartEnemyGrounded` component:
   - `Add Component → SmartEnemyGrounded`
2. Add `Controller2D` component (required):
   - `Add Component → Controller2D`
   - Create collision raycasts (see Controller2D docs)
3. Add `CheckTargetHelper` component (required):
   - `Add Component → CheckTargetHelper`
   - Set detection range

#### Step 3: Configure Enemy Settings

**Health Settings**:
```
health: 150
gravity: 35
walkSpeed: 2.5
```

**Behavior Settings**:
```
attackType: MELEE
startBehavior: WALK_LEFT
spawnDelay: 1
```

**Effect Settings**:
```
canBeFreeze: true
canBeBurn: true
canBePoison: true
canBeShock: true
resistPoisonPercent: 10
poisonSlowSpeed: 0.3
timeShocking: 2
```

#### Step 4: Add Attack Module

**For Melee Attack**:

1. Add `EnemyMeleeAttack` component
2. Create child GameObject: `MeleeCheckPoint`
   - Position in front of enemy
3. Configure settings:
   ```
   targetLayer: Player
   checkPoint: MeleeCheckPoint transform
   radiusCheck: 1.5
   dealDamage: 25
   criticalPercent: 15
   meleeRate: 1.2
   ```

**For Range Attack**:

1. Add `EnemyRangeAttack` component
2. Create child objects:
   - `RangeCheckPoint`: Detection center
   - `FirePoint`: Visual spawn point
   - `ShootingPoint`: Actual projectile spawn
3. Configure settings:
   ```
   enemyLayer: Player
   checkPoint: RangeCheckPoint transform
   firePoint: FirePoint transform
   shootingPoint: ShootingPoint transform
   damage: 30
   detectDistance: 8
   bullet: YourProjectilePrefab
   shootingRate: 2
   aimTarget: true
   aimTargetOffset: (0, 0.5)
   ```

**For Throw Attack**:

1. Add `EnemyThrowAttack` component
2. Create child objects:
   - `ThrowCheckPoint`: Detection center
   - `ThrowPosition`: Spawn position
3. Configure settings:
   ```
   angleThrow: 60
   throwForceMin: 290
   throwForceMax: 320
   addTorque: 100
   throwRate: 3
   throwPosition: ThrowPosition transform
   _Grenade: YourGrenadePrefab
   targetPlayer: Player layer
   onlyAttackTheFortrest: true
   radiusDetectPlayer: 10
   ```

#### Step 5: Create Animations

**Required Animations**:
- Idle
- Walk
- Attack (melee/shoot/throw)
- Hit
- Die

**Animation Controller Setup**:

```
States:
├─ Idle (default)
├─ Walk
├─ Attack
├─ Hit
├─ Die

Parameters:
├─ speed (float)
├─ melee/shoot/throw (trigger)
├─ hit (trigger)
├─ isDead (bool)
├─ isFreezing (bool)
├─ isPoisoning (bool)

Transitions:
├─ Idle → Walk: speed > 0.1
├─ Walk → Idle: speed < 0.1
├─ Any State → Attack: melee/shoot/throw trigger
├─ Any State → Hit: hit trigger
├─ Any State → Die: isDead = true
```

**Add Animation Events**:

For **Melee** attack animation:
- Frame 30: Event: `AnimMeleeAttackStart`
- Frame 45: Event: `AnimMeleeAttackEnd`

For **Range** attack animation:
- Frame 20: Event: `AnimShoot`

For **Throw** attack animation:
- Frame 25: Event: `AnimThrow`

#### Step 6: Add Health Bar

Health bar is spawned automatically in `Enemy.Start()`:

```csharp
var healthBarObj = (HealthBarEnemyNew)Resources.Load("HealthBar", typeof(HealthBarEnemyNew));
healthBar = (HealthBarEnemyNew)Instantiate(healthBarObj, healthBarOffset, Quaternion.identity);
healthBar.Init(transform, (Vector3)healthBarOffset);
```

**Requirements**:
- Health bar prefab must be in `Resources/HealthBar`
- Adjust `healthBarOffset` in Inspector (default 0, 1.5)

#### Step 7: Add Coin Drop

1. Add `GiveCoinWhenDie` component
2. Configure:
   ```
   coinAmount: 10
   coinPrefab: CoinPrefab
   ```

Coins are given in `Enemy.Die()`:
```csharp
if (GetComponent<GiveCoinWhenDie>())
{
    GetComponent<GiveCoinWhenDie>().GiveCoin();
}
```

#### Step 8: Test Enemy

1. Add enemy to scene
2. Play game
3. Check:
   - ✓ Walks toward fortress
   - ✓ Detects player
   - ✓ Attacks when in range
   - ✓ Takes damage
   - ✓ Effects work (freeze, burn, poison, shock)
   - ✓ Dies correctly
   - ✓ Drops coins

### Example: Creating Flying Enemy

**Problem**: SmartEnemyGrounded only works on ground.

**Solution**: Create custom flying enemy inheriting from Enemy base class.

```csharp
using UnityEngine;
namespace RGame
{
    public class FlyingEnemy : Enemy
    {
        public float flyHeight = 3f;
        public float flySpeed = 2f;
        private Vector3 targetPosition;

        public override void Start()
        {
            base.Start();

            // Fly toward fortress
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

            // Move toward target if playing
            if (isPlaying && enemyState == ENEMYSTATE.WALK)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPosition,
                    flySpeed * Time.deltaTime
                );

                // Face movement direction
                if (targetPosition.x < transform.position.x)
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                else
                    transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        }

        public override void Die()
        {
            base.Die();

            // Add fall animation
            StartCoroutine(FallDown());
        }

        IEnumerator FallDown()
        {
            float timer = 0;
            Vector3 startPos = transform.position;

            while (timer < 1f)
            {
                timer += Time.deltaTime;
                transform.position = new Vector3(
                    startPos.x,
                    Mathf.Lerp(startPos.y, 0, timer),
                    startPos.z
                );
                yield return null;
            }

            gameObject.SetActive(false);
        }
    }
}
```

---

## Common Issues & Solutions

### Issue 1: Enemy Not Moving

**Symptoms**:
- Enemy spawns but stands still
- `velocity.x` is always 0

**Possible Causes & Fixes**:

1. **Wrong State**
   ```csharp
   // Check current state
   Debug.Log("Enemy State: " + enemyState);

   // Fix: Ensure state is WALK
   SetEnemyState(ENEMYSTATE.WALK);
   ```

2. **isPlaying = false**
   ```csharp
   // Check in Update()
   Debug.Log("isPlaying: " + isPlaying);

   // Fix: Set in OnEnable()
   isPlaying = true;
   ```

3. **Frozen Effect**
   ```csharp
   // Check effect
   Debug.Log("Effect: " + enemyEffect);

   // Fix: Clear effect
   enemyEffect = ENEMYEFFECT.NONE;
   ```

4. **Controller2D Not Setup**
   - Check if Controller2D has raycasts configured
   - Fix: Add horizontal and vertical raycasts

### Issue 2: Enemy Not Attacking

**Symptoms**:
- Enemy walks to player but doesn't attack
- CheckAttack() never triggers

**Possible Causes & Fixes**:

1. **Not Detected**
   ```csharp
   // Check detection
   Debug.Log("isPlayerDetected: " + isPlayerDetected);

   // Fix: Call manually
   DetectPlayer(0);
   ```

2. **Attack Module Missing**
   ```csharp
   // Check in Start()
   Debug.Log("Range Attack: " + (rangeAttack != null));
   Debug.Log("Melee Attack: " + (meleeAttack != null));

   // Fix: Add appropriate attack component
   ```

3. **allowCheckAttack = false**
   ```csharp
   // Check flag
   Debug.Log("allowCheckAttack: " + allowCheckAttack);

   // Fix: Ensure not performing special action
   allowCheckAttack = true;
   ```

4. **Wrong attackType**
   ```csharp
   // Check type
   Debug.Log("Attack Type: " + attackType);

   // Fix: Set correct type in Inspector
   attackType = ATTACKTYPE.MELEE;
   ```

### Issue 3: Health Bar Not Showing

**Symptoms**:
- Enemy spawns but no health bar appears

**Possible Causes & Fixes**:

1. **Missing Prefab**
   ```csharp
   // Check Resources folder
   var healthBarObj = Resources.Load("HealthBar", typeof(HealthBarEnemyNew));
   Debug.Log("Health Bar Prefab: " + (healthBarObj != null));

   // Fix: Create prefab in Resources/HealthBar
   ```

2. **Wrong Offset**
   ```csharp
   // Check offset
   Debug.Log("Health Bar Offset: " + healthBarOffset);

   // Fix: Adjust in Inspector
   healthBarOffset = new Vector2(0, 1.5f);
   ```

3. **Canvas Sorting**
   - Health bar may be behind enemy sprite
   - Fix: Increase sorting layer of health bar

### Issue 4: Effect Not Working

**Symptoms**:
- Freeze/Burn/Poison doesn't apply
- Enemy ignores weapon effects

**Possible Causes & Fixes**:

1. **Effect Disabled**
   ```csharp
   // Check flags
   Debug.Log("Can Freeze: " + canBeFreeze);
   Debug.Log("Can Burn: " + canBeBurn);
   Debug.Log("Can Poison: " + canBePoison);

   // Fix: Enable in Inspector
   canBeFreeze = true;
   ```

2. **Conflicting Effect**
   ```csharp
   // Check current effect
   Debug.Log("Current Effect: " + enemyEffect);

   // Some effects cancel others
   // Fix: Clear effect first
   enemyEffect = ENEMYEFFECT.NONE;
   ```

3. **WeaponEffect Not Assigned**
   ```csharp
   // In weapon script, ensure WeaponEffect is passed
   takeDamage.TakeDamage(damage, force, hitPoint, gameObject, BODYPART.NONE, weaponEffect);
   //                                                                        ^^^^^^^^^^^^^
   ```

### Issue 5: Enemy Dies Immediately

**Symptoms**:
- Enemy spawns and instantly dies
- Health is 0 on Start()

**Possible Causes & Fixes**:

1. **Health Not Set**
   ```csharp
   // Check in Start()
   Debug.Log("Health: " + health);
   Debug.Log("Current Health: " + currentHealth);

   // Fix: Set health in Inspector
   health = 100;
   ```

2. **Upgrade Parameter Wrong**
   ```csharp
   // Check upgrade
   if (upgradedCharacterID != null)
       Debug.Log("Upgraded Health: " + upgradedCharacterID.UpgradeHealth);

   // Fix: Set correct values in ScriptableObject
   ```

3. **Taking Damage on Spawn**
   - Check if spawning inside damaging area
   - Fix: Move spawn point

### Issue 6: Enemy Walks Through Walls

**Symptoms**:
- Enemy ignores terrain collisions
- Walks through solid objects

**Possible Causes & Fixes**:

1. **Controller2D Not Working**
   ```csharp
   // Check collisions
   Debug.Log("Below: " + controller.collisions.below);
   Debug.Log("Left: " + controller.collisions.left);
   Debug.Log("Right: " + controller.collisions.right);

   // Fix: Configure raycasts in Controller2D
   ```

2. **LayerMask Wrong**
   - Controller2D has collision mask setting
   - Fix: Set to terrain layer

3. **Collider Disabled**
   ```csharp
   // Check collider
   Debug.Log("Collider Enabled: " + GetComponent<BoxCollider2D>().enabled);

   // Fix: Enable collider
   ```

### Issue 7: Animation Not Playing

**Symptoms**:
- Enemy moves but animation doesn't play
- Stuck in idle animation

**Possible Causes & Fixes**:

1. **speed Parameter Not Set**
   ```csharp
   // Check in HandleAnimation()
   Debug.Log("Velocity: " + velocity.x);

   // Fix: Ensure HandleAnimation() is called
   AnimSetFloat("speed", Mathf.Abs(velocity.x));
   ```

2. **Animator Missing**
   ```csharp
   // Check component
   Debug.Log("Animator: " + (anim != null));

   // Fix: Add Animator component
   ```

3. **Wrong Transition Conditions**
   - Check Animation Controller transitions
   - Fix: Set correct parameter values

### Issue 8: Damage Numbers Not Showing

**Symptoms**:
- Enemy takes damage but no numbers appear

**Possible Causes & Fixes**:

1. **FloatingTextManager Missing**
   ```csharp
   // Check singleton
   Debug.Log("FloatingTextManager: " + (FloatingTextManager.Instance != null));

   // Fix: Ensure FloatingTextManager exists in scene
   ```

2. **Canvas Sorting**
   - Floating text may be behind camera
   - Fix: Set canvas to "Screen Space - Overlay"

---

## Summary

The **Enemy System** is a sophisticated AI system with:

1. **State Machine**: Controls enemy behavior (SPAWNING → IDLE → WALK → ATTACK → DEATH)
2. **Effect System**: 5 weapon effects (Freeze, Burn, Poison, Shock, Explosion)
3. **Modular Attacks**: 3 attack types (Melee, Range, Throw) as separate components
4. **Custom Physics**: Controller2D for precise 2D movement
5. **Health System**: Damage, health bars, death effects
6. **Observer Pattern**: Listens to game state changes

**Key Takeaways**:
- Enemy base class is reusable (even player uses it!)
- Effects can override each other (only one active)
- Attack modules are independent components
- State machine controls all behavior
- Custom movement system (not Rigidbody2D)

**Next Steps**:
- Read `04_UI_System_Complete.md` to understand menus and HUD
- Read `05_Managers_Complete.md` to understand game flow control
- Read `10_How_To_Guides.md` for practical examples

---

**Last Updated**: 2025
**File**: `Documents/03_Enemy_System_Complete.md`
