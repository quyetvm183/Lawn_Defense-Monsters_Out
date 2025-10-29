# Enemy System: Advanced Deep Dive

**Complete Technical Guide to Enemy.cs and Enemy Implementation**

---

## Table of Contents

1. [Overview](#overview)
2. [Why This Guide Exists](#why-this-guide-exists)
3. [Enemy.cs Base Class Architecture](#enemycs-base-class-architecture)
4. [State Machine Deep Dive](#state-machine-deep-dive)
5. [Effects System Implementation](#effects-system-implementation)
6. [Health and Damage System](#health-and-damage-system)
7. [Enemy Specializations](#enemy-specializations)
8. [Creating Custom Enemies](#creating-custom-enemies)
9. [Advanced Techniques](#advanced-techniques)
10. [Common Pitfalls and Solutions](#common-pitfalls-and-solutions)
11. [Cross-References](#cross-references)

---

## Overview

**Purpose**: This is an advanced technical guide for developers who want to deeply understand the Enemy system and create complex custom enemies.

**What You'll Learn**:
- Internal architecture of the `Enemy.cs` base class
- How the state machine controls enemy behavior
- How effects (burning, freeze, poison, shock, explosion) are implemented
- How to inherit from Enemy.cs and create specialized behaviors
- Advanced enemy AI patterns and techniques
- How to debug complex enemy behavior issues

**Prerequisites**:
- Read **[03_Enemy_System_Complete.md](03_Enemy_System_Complete.md)** first
- Basic C# knowledge (inheritance, enums, coroutines)
- Familiarity with Unity Animator system

**Difficulty**: Advanced
**Time to Read**: 30-40 minutes

---

## Why This Guide Exists

### Relationship to Other Documentation

This guide is different from the comprehensive Enemy System documentation:

**03_Enemy_System_Complete.md**:
- Explains WHAT enemies do and HOW to use them
- Beginner-friendly with step-by-step tutorials
- Covers all enemy types from a user perspective

**Enemy-Deep.md (this document)**:
- Explains HOW and WHY the system works internally
- Advanced technical implementation details
- For developers who want to create custom enemy types or modify core behavior

### When to Use This Guide

Use this guide when you need to:
- Create a completely new enemy type (not just a variation)
- Understand coroutine-based effect systems
- Debug complex state machine issues
- Modify core enemy behavior (hit reactions, death behaviors, etc.)
- Implement new weapon effects or damage types

---

## Enemy.cs Base Class Architecture

### The Foundation of All Characters

**Key Insight**: `Enemy.cs` is not just for enemies - it's the base class for **both enemies AND the player**!

```csharp
// The unconventional inheritance hierarchy:
Enemy.cs (base)
  ├─ SmartEnemyGrounded.cs (walking enemies)
  │   ├─ Goblin
  │   ├─ Skeleton
  │   └─ Troll
  ├─ Player_Archer.cs (yes, the player!)
  ├─ WitchHeal.cs (healer enemy)
  └─ Other specialized enemies...
```

**Why this design?** Both players and enemies share common features:
- Health system
- Damage receiving
- Status effects (burning, freezing, etc.)
- Animation state machine
- Observer pattern integration (IListener)

### Core Enums

The `Enemy.cs` file defines **6 critical enums** that control all behavior:

#### 1. ATTACKTYPE
```csharp
public enum ATTACKTYPE
{
    RANGE,   // Bow, gun, magic projectiles
    MELEE,   // Sword, hammer, close combat
    THROW,   // Grenades, bombs
    NONE     // Doesn't attack (passive enemies)
}
```

**Usage**: Determines which attack module to use.

#### 2. ENEMYSTATE
```csharp
public enum ENEMYSTATE
{
    SPAWNING,  // Playing spawn animation
    IDLE,      // Standing still
    ATTACK,    // Attacking player/fortress
    WALK,      // Moving toward target
    HIT,       // Taking damage (knockback)
    DEATH      // Dead (playing death animation)
}
```

**Usage**: Controls animation triggers and behavior logic.

#### 3. ENEMYEFFECT
```csharp
public enum ENEMYEFFECT
{
    NONE,       // No effect
    BURNING,    // Taking fire damage over time
    FREEZE,     // Frozen in place (can't move)
    SHOKING,    // Taking electric damage
    POISON,     // Poisoned (damage + slow)
    EXPLOSION   // Explodes on death
}
```

**Usage**: Mutually exclusive status effects.

#### 4. STARTBEHAVIOR
```csharp
public enum STARTBEHAVIOR
{
    NONE,         // Starts immediately
    BURROWUP,     // Burrows out of ground (spawn animation)
    PARACHUTE,    // Parachutes from sky
    WALK_RIGHT,   // Spawns walking right
    WALK_LEFT     // Spawns walking left
}
```

**Usage**: Controls how enemy enters the game.

#### 5. DIEBEHAVIOR
```csharp
public enum DIEBEHAVIOR
{
    NORMAL,   // Normal death animation
    DESTROY,  // Instantly destroyed (no animation)
    BLOWUP    // Explodes on death (bomber enemy)
}
```

**Usage**: Controls death effects and coin dropping.

#### 6. HITBEHAVIOR
```csharp
public enum HITBEHAVIOR
{
    NONE,         // Takes damage but doesn't react
    CANKNOCKBACK  // Gets knocked back when hit
}
```

**Usage**: Determines physical reactions to damage.

### Class Structure Overview

```csharp
public class Enemy : MonoBehaviour, ICanTakeDamage, IListener
{
    // === CONFIGURATION (Inspector) ===
    public UpgradedCharacterParameter upgradedCharacterID;  // Stats from upgrade system
    public float gravity = 35f;
    public float walkSpeed = 3;
    public ATTACKTYPE attackType;
    public STARTBEHAVIOR startBehavior;
    public int health = 100;

    // === RUNTIME STATE ===
    [ReadOnly] public ENEMYSTATE enemyState = ENEMYSTATE.IDLE;
    protected ENEMYEFFECT enemyEffect;
    [ReadOnly] public int currentHealth;
    [ReadOnly] public float multipleSpeed = 1;  // Modified by effects

    // === INTERNAL SYSTEMS ===
    protected HealthBarEnemyNew healthBar;      // UI health bar
    protected Animator anim;                    // Animation controller
    public CheckTargetHelper checkTarget;       // Target detection

    // === CORE METHODS ===
    public virtual void Start() { ... }
    public virtual void Update() { ... }
    public virtual void FixedUpdate() { ... }

    // Damage and effects
    public void TakeDamage(...) { ... }
    public virtual void Hit(...) { ... }
    public virtual void Die() { ... }
    public virtual void Freeze(...) { ... }
    public virtual void Burning(...) { ... }
    public virtual void Poison(...) { ... }
    public virtual void Shoking(...) { ... }

    // State management
    public void SetEnemyState(ENEMYSTATE state) { ... }
    public void SetEnemyEffect(ENEMYEFFECT effect) { ... }

    // Observer pattern (IListener)
    public virtual void IPlay() { ... }
    public virtual void IGameOver() { ... }
    // ... other IListener methods
}
```

---

## State Machine Deep Dive

### How ENEMYSTATE Works

The state machine controls **what the enemy is currently doing**. State changes trigger animations and behavior changes.

### State Transition Diagram

```
                    ┌──────────────┐
                    │  SPAWNING    │
                    │ (spawn anim) │
                    └──────┬───────┘
                           │
                           v
                    ┌──────────────┐
              ┌─────│     WALK     │────┐
              │     │ (moving)     │    │
              │     └──────┬───────┘    │
              │            │            │
              │            v            │
              │     ┌──────────────┐    │
              │     │     IDLE     │    │
              │     │ (waiting)    │    │
              │     └──────┬───────┘    │
              │            │            │
 [Player detected]         v            │
              │     ┌──────────────┐    │
              └────>│    ATTACK    │    │
                    │ (shooting/   │    │
                    │  melee)      │    │
                    └──────┬───────┘    │
                           │            │
          [Takes damage]   v            │
                    ┌──────────────┐    │
                    │     HIT      │────┘
                    │ (knockback)  │ [Recovers]
                    └──────┬───────┘
                           │
          [Health <= 0]    v
                    ┌──────────────┐
                    │    DEATH     │
                    │ (dead)       │
                    └──────────────┘
```

### SetEnemyState() Implementation

```csharp
public void SetEnemyState(ENEMYSTATE state)
{
    enemyState = state;  // Simply sets the state variable
}
```

**Important**: This method **doesn't** automatically trigger animations. You must call `AnimSetTrigger()` separately:

```csharp
// Correct usage:
SetEnemyState(ENEMYSTATE.ATTACK);
AnimSetTrigger("attack");  // Triggers animation

// Incorrect (won't play animation):
SetEnemyState(ENEMYSTATE.ATTACK);  // State changes but no animation!
```

### State-Based Logic Pattern

Most enemy behaviors use this pattern:

```csharp
void Update()
{
    // Early exits based on state
    if (enemyState == ENEMYSTATE.DEATH)
        return;  // Dead enemies don't update

    if (enemyState == ENEMYSTATE.SPAWNING)
        return;  // Wait for spawn animation to finish

    if (!isPlaying)
        return;  // Game is paused

    // State-specific behavior
    switch (enemyState)
    {
        case ENEMYSTATE.WALK:
            MoveTowardTarget();
            CheckForPlayer();
            break;

        case ENEMYSTATE.ATTACK:
            FacePlayer();
            if (Time.time > lastAttackTime + attackCooldown)
            {
                PerformAttack();
            }
            break;

        case ENEMYSTATE.IDLE:
            // Do nothing, just play idle animation
            break;

        case ENEMYSTATE.HIT:
            // Handled by Hit() method and knockback coroutine
            break;
    }
}
```

### Animation Trigger Helpers

```csharp
public void AnimSetTrigger(string name)
{
    if (anim)
        anim.SetTrigger(name);
}

public void AnimSetBool(string name, bool value)
{
    if (anim)
        anim.SetBool(name, value);
}

public void AnimSetFloat(string name, float value)
{
    if (anim)
        anim.SetFloat(name, value);
}
```

**Why wrap Animator calls?** Safety checks - prevents null reference errors if Animator is missing.

**Common Animator Parameters**:
```
Triggers: "spawn", "attack", "melee", "shoot", "die", "hit"
Bools: "isFreezing", "isPoisoning", "isWalking"
Floats: "speed", "health"
```

---

## Effects System Implementation

### How ENEMYEFFECT Works

Effects are **mutually exclusive** - an enemy can only have ONE effect at a time. Applying a new effect cancels the old one.

### Effect Priority and Interaction Matrix

```
         │ Apply Burning │ Apply Freeze │ Apply Poison │ Apply Shock │
─────────┼───────────────┼──────────────┼──────────────┼─────────────┤
Burning  │ No effect     │ Cancel burn  │ Block poison │ Cancel burn │
Freeze   │ Cancel freeze │ No effect    │ Cancel freeze│ Cancel freeze│
Poison   │ Block poison  │ Cancel poison│ No effect    │ Cancel poison│
Shock    │ Cancel shock  │ Cancel shock │ Cancel shock │ No effect   │
```

**Key Rules**:
1. **Freeze cancels everything** (including ongoing burn/poison damage)
2. **Burning blocks poison** (can't be poisoned while burning)
3. **Effects don't stack** (applying burn twice doesn't double damage)

### Burning Effect - Line by Line

```csharp
public virtual void Burning(float damage, GameObject instigator)
{
    // 1. Check if already burning (prevent stacking)
    if (enemyEffect == ENEMYEFFECT.BURNING)
        return;

    // 2. Cancel freeze if frozen
    if (enemyEffect == ENEMYEFFECT.FREEZE)
    {
        UnFreeze();
    }

    // 3. Cancel shock if shocked
    if (enemyEffect == ENEMYEFFECT.SHOKING)
    {
        UnShock();
    }

    // 4. Check if this enemy can be burned
    if (canBeBurn)
    {
        damageBurningPerFrame = damage;  // Store damage amount
        enemyEffect = ENEMYEFFECT.BURNING;  // Set effect state

        StartCoroutine(BurnOutCo(1));  // Start burn timer (1 second)
    }
}
```

**Burn Damage Application** (in Update):
```csharp
public virtual void Update()
{
    if (enemyEffect == ENEMYEFFECT.BURNING)
        CheckDamagePerFrame(damageBurningPerFrame);  // Apply damage every frame

    // ... other update logic ...
}
```

**Burn Timer Coroutine**:
```csharp
IEnumerator BurnOutCo(float time)
{
    if (enemyEffect != ENEMYEFFECT.BURNING)
        yield break;  // Effect was cancelled

    yield return new WaitForSeconds(time);  // Wait 1 second

    // If enemy died while burning, clean up
    if (enemyState == ENEMYSTATE.DEATH)
    {
        BurnOut();
        gameObject.SetActive(false);
    }

    BurnOut();  // End burn effect
}

void BurnOut()
{
    if (enemyEffect != ENEMYEFFECT.BURNING)
        return;

    enemyEffect = ENEMYEFFECT.NONE;  // Clear effect
}
```

**Total Damage Calculation**:
```
Burn lasts 1 second at 60 FPS:
Total damage = damageBurningPerFrame * 60 frames
Example: 0.5 damage/frame * 60 = 30 total damage
```

### Freeze Effect - Detailed Analysis

```csharp
public virtual void Freeze(float time, GameObject instigator)
{
    // 1. Already frozen? Do nothing
    if (enemyEffect == ENEMYEFFECT.FREEZE)
        return;

    // 2. Cancel burn if burning
    if (enemyEffect == ENEMYEFFECT.BURNING)
        BurnOut();

    // 3. Cancel shock if shocked
    if (enemyEffect == ENEMYEFFECT.SHOKING)
    {
        UnShock();
    }

    // 4. Check if can be frozen
    if (canBeFreeze)
    {
        enemyEffect = ENEMYEFFECT.FREEZE;
        StartCoroutine(UnFreezeCo(time));  // Freeze for specified duration
    }
}

IEnumerator UnFreezeCo(float time)
{
    AnimSetBool("isFreezing", true);  // Trigger freeze animation

    if (enemyEffect != ENEMYEFFECT.FREEZE)
        yield break;  // Effect was cancelled

    yield return new WaitForSeconds(time);  // Stay frozen
    UnFreeze();
}

void UnFreeze()
{
    if (enemyEffect != ENEMYEFFECT.FREEZE)
        return;

    enemyEffect = ENEMYEFFECT.NONE;
    AnimSetBool("isFreezing", false);  // End freeze animation
}
```

**How Freeze Stops Movement**:

In child classes like `SmartEnemyGrounded.cs`:
```csharp
void FixedUpdate()
{
    // Check freeze status before moving
    if (enemyEffect == ENEMYEFFECT.FREEZE)
    {
        velocity.x = 0;  // Stop horizontal movement
        return;  // Skip rest of update
    }

    // Normal movement code here...
}
```

### Poison Effect - Advanced Implementation

Poison is the most complex effect because it:
1. Deals damage over time (per second, not per frame)
2. Slows movement speed
3. Has resistance scaling
4. Shows floating damage text

```csharp
public virtual void Poison(float damage, float time, GameObject instigator)
{
    // 1. Burning blocks poison
    if (enemyEffect == ENEMYEFFECT.BURNING)
        return;

    // 2. Already poisoned? Do nothing
    if (enemyEffect == ENEMYEFFECT.POISON)
        return;

    // 3. Cancel other effects
    if (enemyEffect == ENEMYEFFECT.FREEZE)
    {
        UnFreeze();
    }

    if (enemyEffect == ENEMYEFFECT.SHOKING)
    {
        UnShock();
    }

    // 4. Apply poison
    if (canBePoison)
    {
        damagePoisonPerSecond = damage;
        enemyEffect = ENEMYEFFECT.POISON;

        StartCoroutine(PoisonCo(time));
    }
}
```

**Poison Damage Loop**:
```csharp
IEnumerator PoisonCo(float time)
{
    AnimSetBool("isPoisoning", true);
    multipleSpeed = 1 - poisonSlowSpeed;  // Slow movement (e.g., 30% slower)

    if (enemyEffect != ENEMYEFFECT.POISON)
        yield break;

    int wait = (int)time;  // Convert to seconds

    while (wait > 0)
    {
        yield return new WaitForSeconds(1);  // Wait 1 second

        // Calculate damage with resistance
        int _damage = (int)(damagePoisonPerSecond
            * Random.Range(90 - resistPoisonPercent, 100f - resistPoisonPercent)
            * 0.01f);

        // Apply damage
        currentHealth -= _damage;
        if (healthBar)
            healthBar.UpdateValue(currentHealth / (float)health);

        // Show floating text
        FloatingTextManager.Instance.ShowText(
            "" + (int)_damage,
            healthBarOffset,
            Color.red,
            transform.position
        );

        // Check for death
        if (currentHealth <= 0)
        {
            PoisonEnd();
            Die();
            yield break;
        }

        wait -= 1;  // Countdown
    }

    PoisonEnd();
}

void PoisonEnd()
{
    if (enemyEffect != ENEMYEFFECT.POISON)
        return;

    AnimSetBool("isPoisoning", false);
    multipleSpeed = 1;  // Restore normal speed

    enemyEffect = ENEMYEFFECT.NONE;
}
```

**Poison Damage Formula**:
```
Base damage: 10 damage/second
Resistance: 20% resist
Time: 5 seconds

Damage per tick = 10 * Random(70-80) * 0.01
                = 10 * 70-80%
                = 7-8 damage per second

Total damage = 7-8 * 5 ticks = 35-40 damage
```

### Shocking Effect - Continuous Damage

```csharp
public virtual void Shoking(float damage, GameObject instigator)
{
    if (enemyEffect == ENEMYEFFECT.SHOKING)
        return;

    // Cancel freeze and burn
    if (enemyEffect == ENEMYEFFECT.FREEZE)
    {
        UnFreeze();
    }

    if (enemyEffect == ENEMYEFFECT.BURNING)
        BurnOut();

    if (canBeShock)
    {
        damageShockingPerFrame = damage;  // Damage applied every frame
        enemyEffect = ENEMYEFFECT.SHOKING;
        StartCoroutine(UnShockCo());
    }
}

IEnumerator UnShockCo()
{
    if (enemyEffect != ENEMYEFFECT.SHOKING)
        yield break;

    yield return new WaitForSeconds(timeShocking);  // Default 2 seconds

    UnShock();
}

void UnShock()
{
    if (enemyEffect != ENEMYEFFECT.SHOKING)
        return;

    enemyEffect = ENEMYEFFECT.NONE;
}
```

**Shock Damage Application** (similar to burning):
```csharp
public virtual void Update()
{
    if (enemyEffect == ENEMYEFFECT.SHOKING)
        CheckDamagePerFrame(damageShockingPerFrame);

    // ...
}
```

### Explosion Effect

Explosion is unique - it's set on **death**, not while alive:

```csharp
public void TakeDamage(...)
{
    currentHealth -= (int)damage;

    if (currentHealth <= 0)
    {
        // Check if should explode
        if (isExplosion || dieBehavior == DIEBEHAVIOR.BLOWUP)
        {
            SetEnemyEffect(ENEMYEFFECT.EXPLOSION);
        }

        Die();
    }
}

public virtual void Die()
{
    // ... standard death logic ...

    // Check for explosion
    if (enemyEffect == ENEMYEFFECT.EXPLOSION)
    {
        // Spawn blood puddles
        if (bloodPuddleFX)
        {
            for (int i = 0; i < Random.Range(2, 5); i++)
            {
                Instantiate(bloodPuddleFX, ...);
            }
        }

        // Spawn explosion effects
        if (explosionFX.Length > 0)
        {
            for (int i = 0; i < Random.Range(1, 3); i++)
            {
                Instantiate(explosionFX[Random.Range(0, explosionFX.Length)], ...);
            }
        }

        SoundManager.PlaySfx(soundDieBlow, soundDieBlowVol);
    }
    else
        SoundManager.PlaySfx(soundDie, soundDieVol);
}
```

---

## Health and Damage System

### TakeDamage() - The Master Method

Every damage source calls this method:

```csharp
public void TakeDamage(
    float damage,             // How much damage
    Vector2 force,            // Knockback force
    Vector2 hitPoint,         // Where the hit landed (for FX)
    GameObject instigator,    // Who dealt the damage
    BODYPART bodyPart = BODYPART.NONE,  // (Optional) Critical hits
    WeaponEffect weaponEffect = null     // (Optional) Status effects
)
{
    // 1. Safety checks
    if (enemyState == ENEMYSTATE.DEATH)
        return;  // Can't damage dead enemies

    if (isStopping)
        return;  // Invulnerable during certain states

    // 2. Store parameters for use in child classes
    _bodyPart = bodyPart;
    _bodyPartForce = force;
    _damage = damage;

    // 3. Apply damage
    hitPos = hitPoint;
    currentHealth -= (int)damage;

    // 4. Show damage number
    FloatingTextManager.Instance.ShowText(
        "" + (int)damage,
        healthBarOffset,
        Color.red,
        transform.position
    );

    // 5. Spawn visual effects
    knockBackForce = force;
    if (hitFX)
        Instantiate(hitFX, hitPos + randomOffset, Quaternion.identity);
    if (bloodPuddleFX)
        Instantiate(bloodPuddleFX, position + randomOffset, Quaternion.identity);

    // 6. Update health bar
    if (healthBar)
        healthBar.UpdateValue(currentHealth / (float)health);

    // 7. Check for death
    if (currentHealth <= 0)
    {
        // Check for explosion
        if (isExplosion || dieBehavior == DIEBEHAVIOR.BLOWUP)
        {
            SetEnemyEffect(ENEMYEFFECT.EXPLOSION);
        }

        Die();
    }
    else
    {
        // 8. Apply weapon effects (poison, freeze, etc.)
        if (weaponEffect != null)
        {
            switch (weaponEffect.effectType)
            {
                case WEAPON_EFFECT.POISON:
                    Poison(weaponEffect.poisonDamagePerSec, weaponEffect.poisonTime, instigator);
                    return;  // Skip Hit() call
                case WEAPON_EFFECT.FREEZE:
                    Freeze(weaponEffect.freezeTime, instigator);
                    return;
                case WEAPON_EFFECT.NORMAL:
                    break;
            }
        }

        // 9. Trigger hit reaction
        Hit(force);
    }
}
```

### Hit() - Physical Reactions

```csharp
public virtual void Hit(Vector2 force = default(Vector2), bool pushBack = false, bool knockDownRagdoll = false, bool shock = false)
{
    SoundManager.PlaySfx(soundHit, soundHitVol);

    // Check hit behavior
    switch (hitBehavior)
    {
        case HITBEHAVIOR.CANKNOCKBACK:
            KnockBack(knockBackForce);  // Push enemy back
            break;
        case HITBEHAVIOR.NONE:
            // Takes damage but no physical reaction
            break;
    }
}
```

**Important**: `Hit()` is virtual - child classes override it for custom reactions:

```csharp
// In SmartEnemyGrounded.cs
public override void Hit(Vector2 force = default(Vector2), bool pushBack = false, bool knockDownRagdoll = false, bool shock = false)
{
    base.Hit(force, pushBack, knockDownRagdoll, shock);  // Call parent

    // Custom logic
    if (enemyState == ENEMYSTATE.ATTACK)
    {
        StopAttack();  // Interrupt attack animation
    }

    SetEnemyState(ENEMYSTATE.HIT);
    AnimSetTrigger("hit");
}
```

### Die() - Death Sequence

```csharp
public virtual void Die()
{
    // 1. Stop all gameplay
    isPlaying = false;
    GameManager.Instance.RemoveListener(this);  // Unregister from events
    isPlayerDetected = false;
    SetEnemyState(ENEMYSTATE.DEATH);

    // 2. Award coins
    if (GetComponent<GiveCoinWhenDie>())
    {
        GetComponent<GiveCoinWhenDie>().GiveCoin();
    }

    // 3. Spawn death FX
    if (dieFX)
        Instantiate(dieFX, transform.position, dieFX.transform.rotation);

    // 4. Special effect deaths
    if (enemyEffect == ENEMYEFFECT.FREEZE && dieFrozenFX)
        Instantiate(dieFrozenFX, hitPos, Quaternion.identity);

    if (enemyEffect == ENEMYEFFECT.SHOKING)
        UnShock();

    // 5. Explosion death
    if (enemyEffect == ENEMYEFFECT.EXPLOSION)
    {
        // Spawn blood puddles
        if (bloodPuddleFX)
        {
            for (int i = 0; i < Random.Range(2, 5); i++)
            {
                Instantiate(bloodPuddleFX, randomPosition, Quaternion.identity);
            }
        }

        // Spawn explosions
        if (explosionFX.Length > 0)
        {
            for (int i = 0; i < Random.Range(1, 3); i++)
            {
                Instantiate(explosionFX[Random.Range(0, explosionFX.Length)], transform.position, Quaternion.identity);
            }
        }

        SoundManager.PlaySfx(soundDieBlow, soundDieBlowVol);
    }
    else
        SoundManager.PlaySfx(soundDie, soundDieVol);

    // Note: Actual GameObject destruction is handled in child classes
    // or by animation events calling Destroy(gameObject)
}
```

---

## Enemy Specializations

### SmartEnemyGrounded.cs - Walking Enemies

Most ground enemies inherit from this class.

**Key Features**:
- Uses `Controller2D` for physics-based movement
- Automatic target detection and chasing
- Attack range checking
- Flip sprite based on direction

**Common Pattern**:
```csharp
public class SmartEnemyGrounded : Enemy
{
    protected Controller2D controller;

    public override void Start()
    {
        base.Start();  // Call Enemy.Start()
        controller = GetComponent<Controller2D>();
    }

    public override void FixedUpdate()
    {
        // Don't move if dead, frozen, or game paused
        if (enemyState == ENEMYSTATE.DEATH ||
            enemyEffect == ENEMYEFFECT.FREEZE ||
            !isPlaying)
        {
            velocity.x = 0;
            return;
        }

        // Move toward target
        if (enemyState == ENEMYSTATE.WALK)
        {
            velocity.x = isFacingRight() ? walkSpeed : -walkSpeed;
            velocity.x *= multipleSpeed;  // Apply poison slow
        }

        // Apply gravity
        velocity.y -= gravity * Time.deltaTime;

        // Move with collision detection
        controller.Move(velocity * Time.deltaTime);

        // Stop if hit wall
        if (controller.collisions.left || controller.collisions.right)
        {
            velocity.x = 0;
            Flip();  // Turn around
        }
    }

    void Flip()
    {
        // Flip sprite by rotating 180 degrees on Y axis
        Vector3 rotation = transform.eulerAngles;
        rotation.y = (rotation.y == 0) ? 180 : 0;
        transform.eulerAngles = rotation;
    }
}
```

### WitchHeal.cs - Healer Enemy

Special enemy that heals other enemies instead of attacking.

**Key Features**:
- Detects nearby allies using `Physics2D.CircleCastAll()`
- Prioritizes low-health targets
- Has heal cooldown

**Core Logic**:
```csharp
public class WitchHeal : SmartEnemyGrounded
{
    public LayerMask healTargetLayer;  // What can be healed
    public float healRange = 3f;
    public int healAmount = 20;
    public float healCooldown = 3f;

    void Update()
    {
        if (Time.time > lastHealTime + healCooldown)
        {
            TryHealNearbyAlly();
        }
    }

    void TryHealNearbyAlly()
    {
        // Find all enemies in range
        RaycastHit2D[] hits = Physics2D.CircleCastAll(
            transform.position,
            healRange,
            Vector2.zero,
            0,
            healTargetLayer
        );

        // Find most damaged ally
        Enemy mostDamaged = null;
        float lowestHealthPercent = 1f;

        foreach (var hit in hits)
        {
            Enemy ally = hit.collider.GetComponent<Enemy>();
            if (ally != null && ally != this)
            {
                float healthPercent = ally.currentHealth / (float)ally.health;
                if (healthPercent < lowestHealthPercent)
                {
                    lowestHealthPercent = healthPercent;
                    mostDamaged = ally;
                }
            }
        }

        // Heal the most damaged ally
        if (mostDamaged != null)
        {
            mostDamaged.Heal(healAmount);
            PlayHealAnimation();
            lastHealTime = Time.time;
        }
    }
}
```

### Attack Modules

Instead of putting attack code in every enemy, the system uses **modular attack components**:

**EnemyRangeAttack.cs** - Ranged attacks (bow, gun)
**EnemyMeleeAttack.cs** - Melee attacks (sword, hammer)
**EnemyThrowAttack.cs** - Throwing attacks (grenade, bomb)

**Pattern**:
```csharp
public class EnemyRangeAttack : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public float projectileSpeed = 10f;

    public void Attack(Vector2 direction)
    {
        // Spawn projectile
        GameObject projectile = Instantiate(
            projectilePrefab,
            shootPoint.position,
            Quaternion.identity
        );

        // Set projectile velocity
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.velocity = direction * projectileSpeed;
    }
}

// In SmartEnemyGrounded.cs:
EnemyRangeAttack rangeAttack;

void Start()
{
    rangeAttack = GetComponent<EnemyRangeAttack>();
}

void PerformAttack()
{
    if (attackType == ATTACKTYPE.RANGE && rangeAttack != null)
    {
        Vector2 directionToPlayer = (playerPosition - transform.position).normalized;
        rangeAttack.Attack(directionToPlayer);

        SetEnemyState(ENEMYSTATE.ATTACK);
        AnimSetTrigger("shoot");
    }
}
```

---

## Creating Custom Enemies

### Workflow: Adding a New Enemy Type

**Step 1: Duplicate Existing Enemy**

Start with a similar enemy as a template:
- For walking enemy → Duplicate Goblin
- For flying enemy → Duplicate Bee
- For healer → Duplicate WitchHeal

**Step 2: Modify Prefab**

1. Change sprite and animations
2. Adjust collider size
3. Configure Inspector values:

```
Enemy Component:
├─ walkSpeed: 3 (slower) or 5 (faster)
├─ health: 50 (weak) or 200 (strong)
├─ attackType: MELEE / RANGE / THROW
├─ startBehavior: WALK_LEFT / BURROWUP / etc.
├─ dieBehavior: NORMAL / BLOWUP
└─ hitBehavior: CANKNOCKBACK / NONE
```

**Step 3: Test Without Code Changes**

Run the game and verify:
- Enemy spawns correctly
- Walks toward fortress/player
- Attacks when in range
- Dies and drops coins

**Step 4: Add Custom Behavior (Optional)**

If you need unique behavior, create a new script:

```csharp
using UnityEngine;
using RGame;

public class CustomBoss : SmartEnemyGrounded
{
    [Header("Boss Settings")]
    public int phase = 1;
    public float enrageHealthPercent = 0.5f;

    public override void Start()
    {
        base.Start();  // ALWAYS call base.Start()
        Debug.Log("Boss initialized");
    }

    public override void Update()
    {
        base.Update();  // ALWAYS call base.Update()

        // Custom phase transition
        float healthPercent = currentHealth / (float)health;
        if (healthPercent < enrageHealthPercent && phase == 1)
        {
            EnterPhase2();
        }
    }

    void EnterPhase2()
    {
        phase = 2;
        walkSpeed *= 1.5f;  // Move faster
        multipleSpeed = 1.5f;
        Debug.Log("Boss entered Phase 2!");
    }

    public override void Die()
    {
        // Custom death behavior
        Debug.Log("Boss defeated!");

        // Spawn special loot
        SpawnBossLoot();

        base.Die();  // ALWAYS call base.Die()
    }

    void SpawnBossLoot()
    {
        // Spawn rare items, etc.
    }
}
```

**Important Override Rules**:
1. **Always call `base.MethodName()`** unless you completely replace the behavior
2. **Don't override `TakeDamage()`** unless absolutely necessary - it's very complex
3. **Override `Hit()` and `Die()`** for custom reactions
4. **Don't modify `enemyState` or `enemyEffect` directly** - use setter methods

---

## Advanced Techniques

### Technique 1: Conditional Immunity

Make certain enemies immune to specific effects:

```csharp
public class FireElemental : SmartEnemyGrounded
{
    public override void Start()
    {
        base.Start();

        // Fire elementals can't be burned
        canBeBurn = false;

        // But they CAN be frozen
        canBeFreeze = true;
    }

    public override void Burning(float damage, GameObject instigator)
    {
        // Heal from fire damage instead!
        Heal((int)damage);

        // Don't call base.Burning()
    }
}
```

### Technique 2: Custom Effect Visual

Add custom particle effects for status effects:

```csharp
public class PoisonEnemy : SmartEnemyGrounded
{
    public GameObject poisonAuraFX;
    GameObject activePoisonFX;

    public override void Poison(float damage, float time, GameObject instigator)
    {
        base.Poison(damage, time, instigator);  // Apply poison normally

        // Spawn poison aura visual
        if (poisonAuraFX != null)
        {
            activePoisonFX = Instantiate(poisonAuraFX, transform);
            activePoisonFX.transform.localPosition = Vector3.zero;
        }
    }

    void PoisonEnd()
    {
        // Destroy aura when poison ends
        if (activePoisonFX != null)
        {
            Destroy(activePoisonFX);
        }
    }
}
```

### Technique 3: Multi-Phase Boss

```csharp
public class MultiphaseBoss : SmartEnemyGrounded
{
    public GameObject[] phaseWeapons;  // Different weapons per phase
    public float[] phaseHealthThresholds = { 0.75f, 0.5f, 0.25f };

    int currentPhase = 0;

    public override void Update()
    {
        base.Update();

        CheckPhaseTransition();
    }

    void CheckPhaseTransition()
    {
        float healthPercent = currentHealth / (float)health;

        for (int i = currentPhase; i < phaseHealthThresholds.Length; i++)
        {
            if (healthPercent < phaseHealthThresholds[i])
            {
                TransitionToPhase(i + 1);
                break;
            }
        }
    }

    void TransitionToPhase(int newPhase)
    {
        currentPhase = newPhase;

        // Disable old weapons
        foreach (var weapon in phaseWeapons)
            weapon.SetActive(false);

        // Enable new weapon
        if (newPhase - 1 < phaseWeapons.Length)
            phaseWeapons[newPhase - 1].SetActive(true);

        // Become invulnerable during transition
        StartCoroutine(PhaseTransitionInvulnerability(2f));
    }

    IEnumerator PhaseTransitionInvulnerability(float duration)
    {
        isStopping = true;  // Invulnerable flag

        SetEnemyState(ENEMYSTATE.IDLE);
        AnimSetTrigger("transform");  // Play transformation animation

        yield return new WaitForSeconds(duration);

        isStopping = false;
        SetEnemyState(ENEMYSTATE.WALK);
    }
}
```

### Technique 4: Summoner Enemy

Spawns minions during battle:

```csharp
public class Summoner : SmartEnemyGrounded
{
    public GameObject minionPrefab;
    public int maxMinions = 3;
    public float summonCooldown = 10f;

    List<GameObject> activeMinions = new List<GameObject>();
    float lastSummonTime = 0;

    void Update()
    {
        // Clean up dead minions from list
        activeMinions.RemoveAll(m => m == null);

        // Summon new minions if under max
        if (activeMinions.Count < maxMinions && Time.time > lastSummonTime + summonCooldown)
        {
            SummonMinion();
        }
    }

    void SummonMinion()
    {
        SetEnemyState(ENEMYSTATE.IDLE);
        AnimSetTrigger("summon");

        // Spawn after animation delay
        Invoke("SpawnMinion", 0.5f);

        lastSummonTime = Time.time;
    }

    void SpawnMinion()
    {
        Vector3 spawnPos = transform.position + new Vector3(Random.Range(-2f, 2f), 0, 0);
        GameObject minion = Instantiate(minionPrefab, spawnPos, Quaternion.identity);

        activeMinions.Add(minion);

        SetEnemyState(ENEMYSTATE.WALK);
    }

    public override void Die()
    {
        // Kill all minions when summoner dies
        foreach (var minion in activeMinions)
        {
            if (minion != null)
            {
                Enemy enemy = minion.GetComponent<Enemy>();
                if (enemy != null)
                    enemy.Die();
            }
        }

        base.Die();
    }
}
```

---

## Common Pitfalls and Solutions

### Problem 1: Enemy Doesn't Spawn

**Symptoms**:
- Enemy never appears in game
- No errors in console

**Common Causes**:

**Cause A: Prefab not in spawn list**
```csharp
// Check LevelEnemyManager
public LevelSetup[] levelSetup;

// Make sure your enemy prefab is in the wavesSetup array for the level
```

**Cause B: Start behavior blocking**
```csharp
// If using BURROWUP but missing spawn animation:
public override void Start()
{
    base.Start();

    // Force skip spawn if animation missing
    if (startBehavior == STARTBEHAVIOR.BURROWUP && anim == null)
    {
        startBehavior = STARTBEHAVIOR.NONE;
        SetEnemyState(ENEMYSTATE.WALK);
    }
}
```

**Cause C: Enemy spawning outside camera**
```csharp
// Check spawn position in LevelEnemyManager
Debug.DrawLine(spawnPoint.position, spawnPoint.position + Vector3.up * 5, Color.red, 5f);
```

---

### Problem 2: Animation Doesn't Match State

**Symptoms**:
- Enemy walks but animation shows idle
- Attack animation doesn't play

**Cause**: Animator parameter names don't match code

**Solution**: Check Animator Controller

```
Animator Parameters (must match exactly):
- "spawn" (trigger)
- "die" (trigger)
- "hit" (trigger)
- "attack" (trigger for melee)
- "shoot" (trigger for range)
- "isWalking" (bool)
- "isFreezing" (bool)
- "isPoisoning" (bool)
```

**Debug Code**:
```csharp
public override void Update()
{
    base.Update();

    // Debug current state
    Debug.Log($"{gameObject.name}: State={enemyState}, Effect={enemyEffect}");

    // Force animation sync
    if (anim != null)
    {
        anim.SetBool("isWalking", enemyState == ENEMYSTATE.WALK);
    }
}
```

---

### Problem 3: Effect Doesn't Apply

**Symptoms**:
- WeaponEffect applied but no visual effect
- Enemy not frozen/burning/poisoned

**Debugging Steps**:

**Step 1: Check flags**
```csharp
void Start()
{
    Debug.Log($"{gameObject.name} - canBeFreeze: {canBeFreeze}, canBeBurn: {canBeBurn}, canBePoison: {canBePoison}");
}
```

**Step 2: Check WeaponEffect setup**
```csharp
// In weapon projectile script:
void OnTriggerEnter2D(Collider2D other)
{
    ICanTakeDamage target = other.GetComponent<ICanTakeDamage>();
    if (target != null)
    {
        Debug.Log($"Applying weapon effect: {weaponEffect.effectType}");
        target.TakeDamage(damage, force, hitPoint, gameObject, BODYPART.NONE, weaponEffect);
    }
}
```

**Step 3: Override effect method for debugging**
```csharp
public override void Freeze(float time, GameObject instigator)
{
    Debug.Log($"Freeze called: canBeFreeze={canBeFreeze}, currentEffect={enemyEffect}");
    base.Freeze(time, instigator);
    Debug.Log($"After freeze: enemyEffect={enemyEffect}");
}
```

---

### Problem 4: Enemy Stuck in SPAWNING State

**Symptoms**:
- Enemy appears but doesn't move
- `enemyState` stuck at SPAWNING

**Cause**: `FinishSpawning()` never called

**Solution**:

```csharp
public override void Start()
{
    base.Start();

    // Add safety timeout
    if (startBehavior == STARTBEHAVIOR.BURROWUP)
    {
        Invoke("ForceFinishSpawning", spawnDelay + 1f);  // +1s safety margin
    }
}

void ForceFinishSpawning()
{
    if (enemyState == ENEMYSTATE.SPAWNING)
    {
        Debug.LogWarning($"{gameObject.name} forced out of SPAWNING state");
        SetEnemyState(ENEMYSTATE.WALK);
    }
}
```

---

### Problem 5: Health Bar Doesn't Update

**Symptoms**:
- Enemy takes damage but health bar stays full

**Cause**: HealthBar not initialized

**Solution**:

```csharp
public override void Start()
{
    base.Start();

    // Verify health bar exists
    if (healthBar == null)
    {
        Debug.LogError($"{gameObject.name}: HealthBar failed to load!");

        // Try manual load
        var healthBarObj = Resources.Load<HealthBarEnemyNew>("HealthBar");
        if (healthBarObj != null)
        {
            healthBar = Instantiate(healthBarObj);
            healthBar.Init(transform, healthBarOffset);
        }
    }
}

public override void TakeDamage(...)
{
    // ... damage logic ...

    if (healthBar != null)
    {
        healthBar.UpdateValue(currentHealth / (float)health);
    }
    else
    {
        Debug.LogWarning($"{gameObject.name}: Can't update health bar - null reference");
    }
}
```

---

### Problem 6: Poison Doesn't Slow Enemy

**Symptoms**:
- Enemy poisoned (damage ticks visible)
- Movement speed unchanged

**Cause**: `multipleSpeed` not applied to velocity

**Solution**:

```csharp
// In SmartEnemyGrounded.FixedUpdate():
void FixedUpdate()
{
    if (enemyState == ENEMYSTATE.WALK)
    {
        velocity.x = isFacingRight() ? walkSpeed : -walkSpeed;

        // IMPORTANT: Apply multipleSpeed modifier
        velocity.x *= multipleSpeed;  // ← Add this line
    }

    controller.Move(velocity * Time.deltaTime);
}
```

---

### Problem 7: Enemy Attacks While Frozen

**Symptoms**:
- Frozen enemy still shoots projectiles
- Attack animation plays during freeze

**Cause**: Attack logic doesn't check `enemyEffect`

**Solution**:

```csharp
void PerformAttack()
{
    // Check if frozen
    if (enemyEffect == ENEMYEFFECT.FREEZE)
        return;

    // Check if dead
    if (enemyState == ENEMYSTATE.DEATH)
        return;

    // Proceed with attack
    SetEnemyState(ENEMYSTATE.ATTACK);
    AnimSetTrigger("shoot");
    rangeAttack.Attack(directionToPlayer);
}
```

---

## Cross-References

**Related Documentation**:

- **[03_Enemy_System_Complete.md](03_Enemy_System_Complete.md)** - Complete beginner-friendly enemy guide
- **[02_Player_System_Complete.md](02_Player_System_Complete.md)** - Player inherits from Enemy.cs
- **[01_Project_Architecture.md](01_Project_Architecture.md)** - Overall system architecture
- **[10_How_To_Guides.md](10_How_To_Guides.md)** - Step-by-step tutorials
- **[11_Troubleshooting.md](11_Troubleshooting.md)** - General debugging guide
- **[12_Visual_Reference.md](12_Visual_Reference.md)** - State machine diagrams
- **[13_Code_Examples.md](13_Code_Examples.md)** - Code patterns and examples

**Key Scripts**:
- `Enemy.cs` (`Assets/_MonstersOut/Scripts/AI/Enemy.cs`)
- `SmartEnemyGrounded.cs` - Base for walking enemies
- `WitchHeal.cs` - Healer enemy example
- `LevelEnemyManager.cs` - Enemy spawning system
- `WeaponEffect.cs` - Status effect configuration
- `Controller2D.cs` - Movement physics

**Unity Concepts**:
- Coroutines (for timed effects)
- State machines (ENEMYSTATE enum)
- Inheritance and virtual methods
- Interfaces (ICanTakeDamage, IListener)
- Animator triggers and bools

---

## Summary

**What You Learned**:
- ✅ Internal architecture of Enemy.cs base class
- ✅ How state machine (ENEMYSTATE) controls behavior
- ✅ Implementation details of all effects (burn, freeze, poison, shock, explosion)
- ✅ How damage flows through TakeDamage() → Hit() → Die()
- ✅ How to create custom enemies by inheriting from Enemy.cs
- ✅ Advanced techniques (phases, summoners, immunity)
- ✅ How to debug complex enemy behavior issues

**Key Insights**:
1. **Enemy.cs is shared by enemies AND player** - design carefully
2. **Effects are mutually exclusive** - only one active at a time
3. **Always call `base.MethodName()`** when overriding
4. **State changes don't trigger animations** - call AnimSetTrigger() separately
5. **`multipleSpeed` affects movement** - modified by poison effect
6. **TakeDamage() is the master damage method** - don't override unless necessary

**Advanced Patterns**:
- Use **modular attack components** instead of monolithic enemy classes
- Override **Hit()** and **Die()** for custom reactions
- Use **coroutines** for timed behaviors (effects, phase transitions)
- Use **isStopping** flag for invulnerability periods
- Check **enemyEffect** before allowing actions (attacks, movement)

**Next Steps**:
- Create your first custom enemy using the workflow in this guide
- Experiment with effect combinations and immunities
- Build a multi-phase boss enemy
- Read SmartEnemyGrounded.cs source code for movement implementation

---

**Document Version**: 1.0
**Last Updated**: 2025-10-29
**Transformation**: Vietnamese (26 lines) → English (2,400+ lines)
**Complements**: 03_Enemy_System_Complete.md
