# Player System - Complete Guide

**Prerequisites:** Read `00_Unity_Fundamentals.md` and `01_Project_Architecture.md`
**Target Audience:** Developers who want to understand or modify the player character
**Estimated Reading Time:** 45 minutes
**Script Location:** `Assets/_MonstersOut/Scripts/Player/Player_Archer.cs`

---

## Table of Contents
1. [System Overview](#1-system-overview)
2. [Player Architecture](#2-player-architecture)
3. [Auto-Targeting System](#3-auto-targeting-system)
4. [Trajectory Calculation](#4-trajectory-calculation)
5. [Shooting Mechanics](#5-shooting-mechanics)
6. [Movement System](#6-movement-system)
7. [Damage & Health System](#7-damage--health-system)
8. [Animation Control](#8-animation-control)
9. [Inspector Configuration](#9-inspector-configuration)
10. [How to Modify](#10-how-to-modify)
11. [Common Issues & Solutions](#11-common-issues--solutions)

---

## 1. System Overview

### 1.1 What is the Player System?

The Player System controls the **Archer character** - the player's defensive unit that automatically shoots arrows at approaching enemies.

**Key Characteristics:**
- 🏹 **Auto-Shooting:** Player automatically aims and fires at enemies
- 🎯 **Trajectory Calculation:** Physics simulation to hit moving targets
- 🚶 **Movable:** Player can walk left/right (unlike traditional tower defense)
- ❤️ **Health System:** Can take damage and die
- ⚡ **Weapon Effects:** Arrows can poison, burn, freeze enemies
- 📈 **Upgradeable:** Stats saved via UpgradedCharacterParameter

### 1.2 Unique Design: Player Inherits from Enemy

**This is unusual but clever!**

```
MonoBehaviour
      │
      └─── Enemy.cs (base class)
           ├─ Health system (currentHealth, maxHealth)
           ├─ Damage handling (TakeDamage, Die, Hit)
           ├─ Effects (Poison, Burn, Freeze, Shock)
           ├─ Animation helpers
           ├─ State machine (SPAWNING, WALK, ATTACK, HIT, DEATH)
           │
           └─── Player_Archer.cs (THE PLAYER)
                ├─ Auto-targeting system
                ├─ Trajectory calculation
                ├─ Arrow shooting
                └─ Movement control
```

**Why does Player inherit from Enemy?**
- ✅ **Code Reuse:** Enemy already has health, damage, effects - player needs all of this
- ✅ **Unified System:** One TakeDamage() system for everything
- ✅ **Consistent Behavior:** Player and enemies work the same way
- ✅ **Less Maintenance:** Fix bugs once in Enemy, affects both

**What Player adds on top of Enemy:**
- Auto-targeting enemies
- Ballistic trajectory calculation
- Arrow spawning and firing
- Reload/cooldown system
- Custom movement (can walk, unlike most enemies)

### 1.3 Player Component Diagram

```
Player GameObject
├─ Transform                        ← Position, rotation, scale
├─ Sprite Renderer                  ← Visual appearance (archer sprite)
├─ Box Collider 2D (x2)            ← Collision detection (body + hitbox)
├─ Animator                         ← Animation controller
├─ Controller2D                     ← Custom physics controller
├─ CheckTargetHelper               ← Enemy detection (raycasts)
└─ Player_Archer (Script)          ← Main player logic
    │
    ├── Inherited from Enemy:
    │   ├─ health, currentHealth
    │   ├─ TakeDamage(), Die(), Hit()
    │   ├─ Freeze(), Poison(), Burning(), Shoking()
    │   ├─ enemyState, enemyEffect
    │   ├─ anim (Animator)
    │   ├─ checkTarget (CheckTargetHelper)
    │   └─ healthBar (HealthBarEnemyNew)
    │
    └── Player-Specific:
        ├─ Auto-targeting coroutine
        ├─ Trajectory calculation
        ├─ Arrow shooting
        ├─ Reload system
        └─ Movement logic
```

---

## 2. Player Architecture

### 2.1 Class Structure

**File:** `Player_Archer.cs` (445 lines)

**Inheritance Chain:**
```csharp
MonoBehaviour  →  Enemy  →  Player_Archer
```

**Interfaces Implemented:**
```csharp
public class Player_Archer : Enemy, ICanTakeDamage, IListener
```
- `ICanTakeDamage` - Inherited from Enemy, allows taking damage
- `IListener` - Receives game state events (IPlay, IPause, etc.)

### 2.2 Key Properties

**Arrow Shooting Configuration:**
```csharp
[Header("ARROW SHOOT")]
public float shootRate = 1;           // Seconds between shots
public float force = 20;              // Arrow launch force
[Range(0.01f, 0.1f)]
public float stepCheck = 0.1f;        // Trajectory precision
public float stepAngle = 1;           // Angle iteration step
public float gravityScale = 3.5f;     // Arrow gravity
public bool onlyShootTargetInFront = true;  // Only shoot forward

[Header("ARROW DAMAGE")]
public ArrowProjectile arrow;         // Arrow prefab
public WeaponEffect weaponEffect;     // Poison, burn, freeze, etc.
public int arrowDamage = 30;          // Base damage
public Transform firePostion;         // Spawn point for arrows
```

**Sound Effects:**
```csharp
[Header("Sound")]
public float soundShootVolume = 0.5f;
public AudioClip[] soundShoot;        // Random shoot sounds
```

**Internal State:**
```csharp
private Vector2 _direction;           // Movement direction
private float velocityXSmoothing = 0; // Smooth movement
private bool isAvailable = true;      // Can shoot? (not reloading)
private bool isLoading = false;       // Currently reloading?
private bool isDead = false;          // Player dead?
private Transform target;             // Current target enemy
private Vector2 autoShootPoint;       // Calculated aim point
```

### 2.3 System Flow Diagram

```
Player Spawns
      │
      ▼
Start() - Initialize
├─ Get Controller2D component
├─ Set facing direction
├─ Get arrow damage from UpgradedCharacterParameter
└─ Start AutoCheckAndShoot() coroutine
      │
      ▼
┌─────────────────────────────────────────────┐
│  AutoCheckAndShoot() Coroutine (Infinite)  │
├─────────────────────────────────────────────┤
│  1. Wait for enemy in range                 │
│  2. Detect all enemies (CircleCast)         │
│  3. Find closest enemy                      │
│  4. Raycast to confirm line-of-sight        │
│  5. Calculate aim point                     │
│  6. Call Shoot()                            │
│  7. Wait 0.2 seconds                        │
│  8. Repeat                                  │
└─────────────────┬───────────────────────────┘
                  │
                  ▼
          Shoot() Called
                  │
                  ▼
    CheckTarget() Coroutine
    ├─ Calculate best trajectory angle
    ├─ Simulate physics for each angle
    ├─ Find angle closest to target
    └─ Spawn arrow with calculated force
                  │
                  ▼
         ReloadingCo() Coroutine
         ├─ Disable shooting (isAvailable = false)
         ├─ Play reload animation
         ├─ Wait for shootRate seconds
         └─ Enable shooting (isAvailable = true)
                  │
                  ▼
         (Loop back to AutoCheckAndShoot)
```

---

## 3. Auto-Targeting System

The player automatically detects and targets enemies without manual input.

### 3.1 Detection Coroutine

**Code Location:** `Player_Archer.cs:276-320`

```csharp
IEnumerator AutoCheckAndShoot()
{
    while (true)  // Infinite loop
    {
        // STEP 1: Reset target
        target = null;
        yield return null;  // Wait one frame

        // STEP 2: Wait until enemy detected
        // checkTargetHelper checks if any enemy is in front
        while (!checkTargetHelper.CheckTarget((isFacingRight() ? 1 : -1)))
        {
            yield return null;  // Keep waiting each frame
        }

        // STEP 3: Enemy detected! Find all enemies in huge radius
        RaycastHit2D[] hits = Physics2D.CircleCastAll(
            transform.position,      // Center point (player position)
            100,                      // Radius (very large to catch all enemies)
            Vector2.zero,             // Direction (not used, just detecting in area)
            0,                        // Distance (0 = just check at center)
            GameManager.Instance.layerEnemy  // Only detect Enemy layer
        );

        // STEP 4: Process all hit enemies
        if (hits.Length > 0)
        {
            float closestDistance = 99999;  // Track closest enemy

            foreach (var obj in hits)
            {
                // Try to get ICanTakeDamage component
                var checkEnemy = (ICanTakeDamage)obj.collider.gameObject
                    .GetComponent(typeof(ICanTakeDamage));

                if (checkEnemy != null)
                {
                    // Calculate horizontal distance to enemy
                    float distance = Mathf.Abs(obj.transform.position.x -
                                               transform.position.x);

                    // Is this closer than current closest?
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        target = obj.transform;

                        // STEP 5: Verify line-of-sight with raycast
                        var hit = Physics2D.Raycast(
                            transform.position,
                            (obj.point - (Vector2)transform.position),
                            100,
                            GameManager.Instance.layerEnemy
                        );

                        // Debug visualization (red line in Scene view)
                        Debug.DrawRay(
                            transform.position,
                            (obj.point - (Vector2)transform.position) * 100,
                            Color.red
                        );

                        // STEP 6: Set aim point
                        autoShootPoint = hit.point;
                        // Ensure aim point isn't too low
                        autoShootPoint.y = Mathf.Max(
                            autoShootPoint.y,
                            firePostion.position.y - 0.1f
                        );
                    }
                }
            }

            // STEP 7: Shoot at closest enemy
            if (target)
            {
                Shoot();
                yield return new WaitForSeconds(0.2f);  // Brief pause
            }
        }
    }
}
```

**How it works:**

1. **Continuous Detection Loop:** Runs forever while player is active
2. **Wait for Enemy:** Pauses until CheckTargetHelper detects something
3. **Find All Enemies:** Uses CircleCast to get all enemies in huge area
4. **Select Closest:** Compares distances, picks nearest enemy
5. **Verify Line-of-Sight:** Raycast to ensure no obstacles blocking
6. **Calculate Aim Point:** Sets where to shoot
7. **Fire Arrow:** Calls Shoot() method
8. **Brief Cooldown:** 0.2 second pause before next iteration

**Visual Diagram:**
```
Player Position
      │
      │ CheckTargetHelper (forward cone detection)
      │        ╱
      │       ╱ Enemy in range?
      │      ╱
      ▼─────▼─────────────────────────────
      │     ╲                    Enemy 1 (far)
      │      ╲
      │       ╲
      │        ╲ Enemy 2 (close) ← Target this!
      │         │
      │         │ Raycast to verify
      │         │
      │ ────────┼─────→ autoShootPoint
      │         │
      │     [Shoot arrow]
```

### 3.2 CheckTargetHelper

**What it does:** Detects if any enemy is in front of player

**Location:** `CheckTargetHelper.cs` (attached component)

**Returns:** `true` if enemy detected, `false` otherwise

**Usage:**
```csharp
// Check right side if facing right, left side if facing left
bool enemyDetected = checkTargetHelper.CheckTarget(isFacingRight() ? 1 : -1);
```

---

## 4. Trajectory Calculation

This is the **most complex part** of the player system. The player calculates a ballistic arc to hit moving targets.

### 4.1 Why Calculate Trajectory?

**Problem:** Enemies are moving. If we shoot straight at them, arrow will miss.

**Solution:** Simulate physics to find the perfect angle that hits the target.

**Analogy:** Like a basketball player calculating the arc needed to sink a shot.

### 4.2 Trajectory Algorithm

**Code Location:** `Player_Archer.cs:336-407`

```csharp
IEnumerator CheckTarget()
{
    // STEP 1: Get target position
    Vector3 mouseTempLook = autoShootPoint;  // Where we want to hit
    mouseTempLook -= transform.position;     // Relative to player
    mouseTempLook.x *= (isFacingRight() ? -1 : 1);  // Flip if needed
    yield return null;

    // STEP 2: Setup trajectory calculation
    Vector2 fromPosition = firePostion.position;  // Arrow spawn point
    Vector2 target = autoShootPoint;              // Target position

    // STEP 3: Calculate initial angle estimate
    float beginAngle = Vector2ToAngle(target - fromPosition);
    Vector2 ballPos = fromPosition;

    // STEP 4: Find best angle iteratively
    float closestAngleDistance = int.MaxValue;  // Track best result
    bool checkingPerAngle = true;

    while (checkingPerAngle)
    {
        // Initialize per-angle check
        int k = 0;
        Vector2 lastPos = fromPosition;
        bool isCheckingAngle = true;
        float closestDistance = int.MaxValue;

        // STEP 5: Simulate trajectory for this angle
        while (isCheckingAngle)
        {
            // Calculate force vector for current angle
            Vector2 shotForce = force * AngleToVector2(beginAngle);

            // PHYSICS SIMULATION (simplified projectile motion)
            // X position: constant velocity
            x1 = ballPos.x + shotForce.x * Time.fixedDeltaTime * (stepCheck * k);

            // Y position: affected by gravity
            // Formula: y = y0 + v0*t - (1/2)*g*t^2
            y1 = ballPos.y + shotForce.y * Time.fixedDeltaTime * (stepCheck * k)
                 - (-(Physics2D.gravity.y * gravityScale) / 2f
                    * Time.fixedDeltaTime * Time.fixedDeltaTime
                    * (stepCheck * k) * (stepCheck * k));

            // STEP 6: Check distance to target at this point
            float distance = Vector2.Distance(target, new Vector2(x1, y1));

            if (distance < closestDistance)
                closestDistance = distance;

            // STEP 7: Stop if trajectory is falling and below target
            if ((y1 < lastPos.y) && (y1 < target.y))
                isCheckingAngle = false;
            else
                k++;

            lastPos = new Vector2(x1, y1);
        }

        // STEP 8: Is this angle better than previous attempts?
        if (closestDistance >= closestAngleDistance)
        {
            // No improvement, stop iteration
            checkingPerAngle = false;
        }
        else
        {
            // Better angle found! Save it and try next angle
            closestAngleDistance = closestDistance;

            // Adjust angle for next iteration
            if (isTargetRight)
                beginAngle += stepAngle;  // Increment
            else
                beginAngle -= stepAngle;  // Decrement
        }
    }

    // STEP 9: Best angle found! Prepare to shoot
    var lookAt = AngleToVector2(beginAngle) * 10;
    lookAt.x *= (isFacingRight() ? -1 : 1);

    yield return null;

    // STEP 10: Trigger shoot animation
    anim.SetTrigger("shoot");

    // STEP 11: Spawn arrow with calculated trajectory
    ArrowProjectile _tempArrow = Instantiate(
        arrow,
        fromPosition,
        Quaternion.identity
    );

    // Initialize arrow with force and gravity
    _tempArrow.Init(
        force * AngleToVector2(beginAngle),  // Launch force
        gravityScale,                         // Gravity
        arrowDamage                           // Damage
    );

    // STEP 12: Play sound effect
    SoundManager.PlaySfx(
        soundShoot[Random.Range(0, soundShoot.Length)],
        soundShootVolume
    );

    // STEP 13: Start reload cooldown
    StartCoroutine(ReloadingCo());
}
```

### 4.3 Trajectory Visualization

**What the algorithm does:**

```
Attempt 1: Angle 45°
   ╭────╮
  ╱      ╲
 │        ╲      ← Arc too high, misses
 │         ╲
 │          ✗ (miss)
Player      Target

Attempt 2: Angle 44°
   ╭───╮
  ╱     ╲
 │       ╲     ← Arc still too high
 │        ╲
 │         ✗ (miss)
Player      Target

Attempt 3: Angle 43°
   ╭──╮
  ╱    ╲
 │      ╲    ← Perfect arc!
 │       ╲
 │        ✓ (hit!)
Player      Target
```

**Step-by-Step Breakdown:**

1. **Start with estimated angle:** Calculate rough angle to target
2. **Simulate trajectory:** For current angle, calculate arrow path point-by-point
3. **Check accuracy:** Measure closest distance to target during flight
4. **Improve angle:** Increment/decrement angle slightly
5. **Repeat:** Keep iterating until angle gets worse (found best)
6. **Shoot:** Use best angle to launch arrow

**Helper Functions:**

```csharp
// Convert angle (degrees) to direction vector
public static Vector2 AngleToVector2(float degree)
{
    // Quaternion.Euler creates rotation
    // Multiply by Vector2.right to get direction
    Vector2 dir = (Vector2)(Quaternion.Euler(0, 0, degree) * Vector2.right);
    return dir;
}

// Convert direction vector to angle (degrees)
public float Vector2ToAngle(Vector2 vec2)
{
    // Atan2 returns radians, convert to degrees
    var angle = Mathf.Atan2(vec2.y, vec2.x) * Mathf.Rad2Deg;
    return angle;
}
```

**Why This Works:**

Physics simulation at each angle predicts where arrow will land. By trying multiple angles and comparing results, we find the one that lands closest to target.

**Performance Note:**

This calculation happens **every shot**, but it's optimized:
- Uses `stepAngle = 1` (only checks every 1 degree)
- Uses `stepCheck = 0.1` (simulates every 0.1 time units)
- Stops early when angle gets worse

---

## 5. Shooting Mechanics

### 5.1 Shoot() Method

**Trigger:** Called by AutoCheckAndShoot when target detected

**Code Location:** `Player_Archer.cs:322-333`

```csharp
public void Shoot()
{
    // VALIDATION: Can't shoot if...
    if (!isAvailable ||                                    // Reloading
        target == null ||                                  // No target
        GameManager.Instance.State != GameManager.GameState.Playing)  // Not playing
        return;

    // DIRECTION CHECK: Is target on right or left?
    isTargetRight = autoShootPoint.x > transform.position.x;

    // OPTIONAL: Only shoot targets in front
    if (onlyShootTargetInFront &&
        ((isTargetRight && !isFacingRight()) ||           // Target right, facing left
         (isFacingRight() && !isTargetRight)))            // Facing right, target left
        return;

    // START TRAJECTORY CALCULATION
    StartCoroutine(CheckTarget());
}
```

**Why the `onlyShootTargetInFront` check?**
- Prevents player from shooting backward
- More realistic (archer can't twist around)
- Encourages player positioning

### 5.2 Arrow Spawning

**What happens when arrow is created:**

```csharp
// Create arrow instance
ArrowProjectile _tempArrow = Instantiate(
    arrow,          // Prefab
    fromPosition,   // Spawn at firePosition (bow)
    Quaternion.identity  // No rotation (arrow script handles rotation)
);

// Initialize arrow
_tempArrow.Init(
    force * AngleToVector2(beginAngle),  // Launch velocity vector
    gravityScale,                         // How fast arrow falls
    arrowDamage                           // Damage on hit
);
```

**Arrow Initialization (in ArrowProjectile.cs):**
```csharp
public void Init(Vector2 velocity, float gravity, int damage)
{
    this.velocity = velocity;      // Set initial velocity
    this.gravityScale = gravity;   // Set gravity
    this.damage = damage;          // Set damage
    // Arrow script takes over from here
}
```

### 5.3 Reload System

**Code Location:** `Player_Archer.cs:410-428`

```csharp
IEnumerator ReloadingCo()
{
    // STEP 1: Disable shooting
    isAvailable = false;
    lastShoot = Time.time;  // Record when we shot
    isLoading = true;

    // STEP 2: Brief delay before reload animation
    yield return new WaitForSeconds(0.1f);

    // STEP 3: Show reload animation
    anim.SetBool("isLoading", true);

    // STEP 4: Wait for reload duration
    while (Time.time < (lastShoot + shootRate))
    {
        yield return null;  // Wait each frame
    }

    // STEP 5: Hide reload animation
    anim.SetBool("isLoading", false);

    // STEP 6: Brief delay
    yield return new WaitForSeconds(0.2f);

    // STEP 7: Ready to shoot again!
    isAvailable = true;
    isLoading = false;
}
```

**Reload Timeline:**
```
Shoot Arrow
    │
    ├─ isAvailable = false (can't shoot)
    │
    ├─ 0.1s delay
    │
    ├─ Show "isLoading" animation
    │
    ├─ Wait shootRate seconds (e.g., 1 second)
    │
    ├─ Hide "isLoading" animation
    │
    ├─ 0.2s delay
    │
    └─ isAvailable = true (can shoot again)
```

**Inspector Configuration:**
- `shootRate = 1.0f` → 1 arrow per second
- `shootRate = 0.5f` → 2 arrows per second (faster)
- `shootRate = 2.0f` → 1 arrow every 2 seconds (slower)

---

## 6. Movement System

Player uses **custom 2D physics** (Controller2D), not Rigidbody2D.

### 6.1 Movement Code

**Code Location:** `Player_Archer.cs:89-127`

```csharp
public virtual void LateUpdate()
{
    // STOP CONDITION 1: Game not playing
    if (GameManager.Instance.State != GameManager.GameState.Playing)
    {
        velocity.x = 0;
        return;
    }

    // STOP CONDITION 2: Various states that prevent movement
    else if (!isPlaying ||           // Not active
             isSocking ||             // Being shocked
             enemyEffect == ENEMYEFFECT.SHOKING ||  // Shock effect
             isLoading ||             // Reloading
             checkTargetHelper.CheckTarget((isFacingRight() ? 1 : -1)))  // Enemy in range
    {
        velocity = Vector2.zero;
        return;
    }

    // CALCULATE TARGET VELOCITY
    float targetVelocityX = _direction.x * moveSpeed;

    // STOP CONDITION 3: Special states
    if (isSocking || enemyEffect == ENEMYEFFECT.SHOKING)
        targetVelocityX = 0;

    if (enemyState != ENEMYSTATE.WALK || enemyEffect == ENEMYEFFECT.FREEZE)
        targetVelocityX = 0;

    if (isStopping || isStunning)
        targetVelocityX = 0;

    // SMOOTH VELOCITY (gradual acceleration/deceleration)
    velocity.x = Mathf.SmoothDamp(
        velocity.x,                  // Current velocity
        targetVelocityX,             // Target velocity
        ref velocityXSmoothing,      // Smoothing variable (passed by ref)
        (controller.collisions.below) ? 0.1f : 0.2f  // Smoothing time
    );

    // APPLY GRAVITY
    velocity.y += -gravity * Time.deltaTime;

    // WALL COLLISION: Stop if hitting wall
    if ((_direction.x > 0 && controller.collisions.right) ||
        (_direction.x < 0 && controller.collisions.left))
        velocity.x = 0;

    // MOVE CHARACTER using Controller2D
    controller.Move(
        velocity * Time.deltaTime * multipleSpeed,  // Movement delta
        false,                                       // Not jumping
        isFacingRight()                             // Facing direction
    );

    // GROUND/CEILING COLLISION: Stop vertical movement
    if (controller.collisions.above || controller.collisions.below)
        velocity.y = 0;
}
```

**Key Points:**

1. **LateUpdate vs Update:**
   - LateUpdate runs after Update
   - Ensures movement happens after all logic updates

2. **Smooth Movement:**
   - Uses `Mathf.SmoothDamp` for gradual acceleration
   - Feels more natural than instant velocity changes

3. **Controller2D:**
   - Custom physics using raycasts
   - No Rigidbody2D required
   - More precise control

4. **Movement Stopping Conditions:**
   - Game not playing
   - Player reloading
   - Player stunned/frozen/shocked
   - Enemy in detection range (stops to shoot)

### 6.2 Direction Control

**Flip Method:**
```csharp
void Flip()
{
    // Reverse direction vector
    _direction = -_direction;

    // Rotate sprite (0° = right, 180° = left)
    transform.rotation = Quaternion.Euler(
        new Vector3(
            transform.rotation.x,
            isFacingRight() ? 0 : 180,  // Y rotation
            transform.rotation.z
        )
    );
}
```

**Check Facing Direction:**
```csharp
public bool isFacingRight()
{
    // Y rotation 180° = facing right (sprite is flipped)
    return transform.rotation.eulerAngles.y == 180 ? true : false;
}
```

**Initial Direction Setup:**
```csharp
void Start()
{
    // Set direction based on initial rotation
    _direction = isFacingRight() ? Vector2.right : Vector2.left;

    // If startBehavior conflicts with facing, flip
    if ((_direction == Vector2.right && startBehavior == STARTBEHAVIOR.WALK_LEFT) ||
        (_direction == Vector2.left && startBehavior == STARTBEHAVIOR.WALK_RIGHT))
    {
        Flip();
    }
}
```

---

## 7. Damage & Health System

Player **inherits** health system from Enemy base class.

### 7.1 Taking Damage

**Inherited from Enemy.cs:**
```csharp
public void TakeDamage(
    float damage,
    Vector2 force,
    Vector2 hitPoint,
    GameObject instigator,
    BODYPART bodyPart = BODYPART.NONE,
    WeaponEffect weaponEffect = null)
{
    if (enemyState == ENEMYSTATE.DEATH)
        return;

    // Reduce health
    currentHealth -= (int)damage;

    // Show damage number
    FloatingTextManager.Instance.ShowText(
        "" + (int)damage,
        healthBarOffset,
        Color.red,
        transform.position
    );

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
            // Handle poison, freeze, burn, shock
        }

        Hit(force);  // Play hit reaction
    }
}
```

### 7.2 Player Death

**Overridden in Player_Archer.cs:**
```csharp
public override void Die()
{
    // Already dead? Stop
    if (isDead)
        return;

    base.Die();  // Call Enemy.Die() first

    // Set dead flag
    isDead = true;

    CancelInvoke();  // Cancel scheduled actions

    // Disable colliders (can't be hit anymore)
    var cols = GetComponents<BoxCollider2D>();
    foreach (var col in cols)
        col.enabled = false;

    // Play death animation
    AnimSetBool("isDead", true);
    if (Random.Range(0, 2) == 1)
        AnimSetTrigger("die2");  // Alternate death animation

    // Special death effects
    if (enemyEffect == ENEMYEFFECT.BURNING)
        return;  // Keep burning

    if (enemyEffect == ENEMYEFFECT.EXPLOSION || dieBehavior == DIEBEHAVIOR.DESTROY)
    {
        gameObject.SetActive(false);
        return;
    }

    // Stop all coroutines
    StopAllCoroutines();

    // Disable after death animation finishes
    StartCoroutine(DisableEnemy(
        AnimationHelper.getAnimationLength(anim, "Die") + 2f
    ));
}
```

**What Happens on Player Death:**
1. Stop all actions (shooting, moving)
2. Disable colliders (can't be hit again)
3. Play death animation
4. Wait for animation to finish
5. Disable GameObject
6. GameManager.GameOver() is called (from Enemy.Die())

### 7.3 Hit Reaction

**Code Location:** `Player_Archer.cs:216-232`

```csharp
public override void Hit(Vector2 force, bool pushBack = false, bool knockDownRagdoll = false, bool shock = false)
{
    // Can't react if not playing or stunned
    if (!isPlaying || isStunning)
        return;

    base.Hit(force, pushBack, knockDownRagdoll, shock);  // Call Enemy.Hit()

    if (isDead)
        return;

    // Play hit animation
    AnimSetTrigger("hit");

    // Apply knockback
    if (pushBack)
        StartCoroutine(PushBack(force));
    else if (shock)
        StartCoroutine(Shock());
}
```

**PushBack Effect:**
```csharp
public IEnumerator PushBack(Vector2 force)
{
    // Apply force to push player back
    SetForce(force.x, force.y);

    if (isDead)
    {
        Die();
        yield break;
    }
}
```

---

## 8. Animation Control

Player uses Unity's Animator with parameters.

### 8.1 Animation Parameters

**Animator Parameters (set in Unity Animator):**
- `speed` (float) - Movement speed for walk animation
- `isRunning` (bool) - Running animation
- `isStunning` (bool) - Stun animation
- `shoot` (trigger) - Shoot animation
- `isLoading` (bool) - Reload animation
- `hit` (trigger) - Hit reaction animation
- `isDead` (bool) - Death animation
- `die2` (trigger) - Alternate death animation
- `stun` (trigger) - Stun trigger

### 8.2 Animation Update

**Code Location:** `Player_Archer.cs:169-175`

```csharp
void HandleAnimation()
{
    // Update movement animation based on velocity
    AnimSetFloat("speed", Mathf.Abs(velocity.x));

    // Running if moving faster than walkSpeed
    AnimSetBool("isRunning", Mathf.Abs(velocity.x) > walkSpeed);

    // Show stun animation
    AnimSetBool("isStunning", isStunning);
}
```

**Inherited Animation Helpers (from Enemy.cs):**
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

**When to call each:**
- **Update():** HandleAnimation() - every frame for smooth animation
- **Shoot():** AnimSetTrigger("shoot") - one-time action
- **Die():** AnimSetBool("isDead", true) - persistent state

---

## 9. Inspector Configuration

### 9.1 Essential Settings

**Arrow Shoot Settings:**
```
Shoot Rate: 1.0        // 1 arrow per second
Force: 20              // Arrow launch power
Step Check: 0.1        // Trajectory precision (lower = more accurate)
Step Angle: 1          // Angle iteration step
Gravity Scale: 3.5     // Arrow fall speed
Only Shoot Target In Front: ✓  // Prevent backward shooting
```

**Arrow Damage:**
```
Arrow: [ArrowProjectile Prefab]     // Drag arrow prefab here
Weapon Effect: [WeaponEffect]       // Poison/Burn/Freeze effect
Arrow Damage: 30                    // Base damage (overridden by upgrade)
Fire Position: [Transform]          // Bow spawn point
```

**Sound:**
```
Sound Shoot Volume: 0.5
Sound Shoot: [Array of AudioClips]  // Random shoot sounds
```

**Inherited from Enemy (also configurable):**
```
Gravity: 35                // Fall speed
Walk Speed: 3              // Movement speed
Health: 100                // Max health
```

### 9.2 Required Components

**Must be on same GameObject:**
- ✅ Animator (with configured controller)
- ✅ Controller2D (custom physics)
- ✅ CheckTargetHelper (enemy detection)
- ✅ Box Collider 2D (at least one, for collision)
- ✅ Sprite Renderer (visual)

**Inspector Checklist:**
```
Player_Archer Component
├─ ✓ Arrow prefab assigned
├─ ✓ Weapon Effect assigned (if using effects)
├─ ✓ Fire Position set (child transform at bow)
├─ ✓ Sound Shoot array filled
├─ ✓ Upgraded Character Parameter assigned
└─ ✓ All inherited Enemy fields configured

Controller2D Component
├─ ✓ Collision Mask set (ground layer)
├─ ✓ Raycast settings configured
└─ ✓ Horizontal/Vertical ray count set

CheckTargetHelper Component
├─ ✓ Detection range set
├─ ✓ Target layer set (Enemy layer)
└─ ✓ Detection angle configured
```

---

## 10. How to Modify

### 10.1 Change Fire Rate

**Make player shoot faster/slower:**

```csharp
// In Inspector or code
public float shootRate = 0.5f;  // 2 arrows per second (faster)
public float shootRate = 2.0f;  // 1 arrow every 2 seconds (slower)
```

### 10.2 Add Manual Aiming

**Allow player to aim with mouse:**

```csharp
// Add to Update()
void Update()
{
    base.Update();
    HandleAnimation();

    // NEW: Manual aim mode
    if (Input.GetMouseButton(0))  // Left click to aim
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        autoShootPoint = mousePos;  // Set aim point to mouse
        Shoot();  // Shoot at mouse position
    }

    // Rest of existing code...
}
```

### 10.3 Add Weapon Upgrades

**Different arrow types:**

```csharp
[Header("Arrow Types")]
public ArrowProjectile normalArrow;
public ArrowProjectile fireArrow;
public ArrowProjectile iceArrow;
private ArrowProjectile currentArrow;

void Start()
{
    base.Start();
    // Set based on upgrade level
    if (GlobalValue.weaponLevel >= 3)
        currentArrow = iceArrow;
    else if (GlobalValue.weaponLevel >= 2)
        currentArrow = fireArrow;
    else
        currentArrow = normalArrow;
}

// In CheckTarget() coroutine, change:
ArrowProjectile _tempArrow = Instantiate(currentArrow, fromPosition, Quaternion.identity);
```

### 10.4 Add Dash Ability

**Quick dodge mechanic:**

```csharp
[Header("Dash Settings")]
public float dashSpeed = 20f;
public float dashDuration = 0.2f;
public KeyCode dashKey = KeyCode.Space;
private bool isDashing = false;

void Update()
{
    base.Update();

    // NEW: Dash input
    if (Input.GetKeyDown(dashKey) && !isDashing)
    {
        StartCoroutine(Dash());
    }
}

IEnumerator Dash()
{
    isDashing = true;
    float dashTime = 0;

    while (dashTime < dashDuration)
    {
        // Move at dash speed
        velocity.x = (isFacingRight() ? 1 : -1) * dashSpeed;
        dashTime += Time.deltaTime;
        yield return null;
    }

    isDashing = false;
}
```

### 10.5 Add Multi-Shot

**Shoot multiple arrows at once:**

```csharp
[Header("Multi-Shot")]
public int arrowCount = 3;        // Number of arrows per shot
public float spreadAngle = 15f;   // Angle between arrows

// Modify CheckTarget() coroutine:
// After calculating beginAngle, instead of spawning one arrow:

for (int i = 0; i < arrowCount; i++)
{
    // Calculate spread
    float angleOffset = spreadAngle * (i - (arrowCount - 1) / 2f);
    float finalAngle = beginAngle + angleOffset;

    // Spawn arrow
    ArrowProjectile _tempArrow = Instantiate(
        arrow,
        fromPosition,
        Quaternion.identity
    );

    _tempArrow.Init(
        force * AngleToVector2(finalAngle),
        gravityScale,
        arrowDamage
    );
}
```

---

## 11. Common Issues & Solutions

### 11.1 Player Not Shooting

**Problem:** Player doesn't shoot at enemies

**Possible Causes & Solutions:**

**1. CheckTargetHelper not detecting enemies**
- **Check:** Is `checkTargetHelper` assigned?
- **Solution:** Ensure CheckTargetHelper component exists and is configured
- **Verify:** Look for red debug ray in Scene view when enemy approaches

**2. Wrong layer configuration**
- **Check:** Are enemies on correct layer?
- **Solution:** Verify `GameManager.layerEnemy` matches enemy GameObject layer

**3. Arrow prefab not assigned**
- **Check:** Inspector → Arrow field
- **Solution:** Drag ArrowProjectile prefab into field

**4. Fire position missing**
- **Check:** Inspector → Fire Position
- **Solution:** Create empty child GameObject at bow, assign to Fire Position

**5. Player in wrong state**
- **Check:** `isAvailable`, `isLoading`, `isDead`
- **Debug:** Add `Debug.Log("Can shoot: " + isAvailable);` in Shoot()

### 11.2 Arrows Miss Target

**Problem:** Arrows fly past or under enemies

**Causes & Solutions:**

**1. Trajectory precision too low**
- **Solution:** Decrease `stepCheck` to 0.05 or lower (more precise)
- **Trade-off:** Lower value = more CPU usage

**2. Step angle too large**
- **Solution:** Decrease `stepAngle` to 0.5 (finer angle adjustments)

**3. Gravity mismatch**
- **Solution:** Ensure `gravityScale` matches arrow's gravity setting

**4. Moving targets**
- **Note:** Current system aims at current position, doesn't predict movement
- **Advanced Solution:** Implement predictive aiming (calculate where enemy will be)

### 11.3 Player Won't Move

**Problem:** Player stuck in place

**Causes:**

**1. Controller2D not configured**
- **Check:** Controller2D component exists
- **Solution:** Add Controller2D, configure collision layers

**2. Always in reload state**
- **Check:** `isLoading` always true
- **Debug:** Add `Debug.Log("Loading: " + isLoading);`
- **Solution:** Check if ReloadingCo() is completing correctly

**3. Enemy always detected**
- **Issue:** CheckTargetHelper detection range too large
- **Solution:** Reduce detection range in CheckTargetHelper settings

**4. Frozen by effect**
- **Check:** `enemyEffect == ENEMYEFFECT.FREEZE`
- **Solution:** Check what's applying freeze effect

### 11.4 Performance Issues

**Problem:** Game lags when player shoots

**Solutions:**

**1. Optimize trajectory calculation**
```csharp
// Reduce precision (faster but less accurate)
public float stepCheck = 0.15f;  // Instead of 0.1
public float stepAngle = 2f;     // Instead of 1
```

**2. Limit target search frequency**
```csharp
// In AutoCheckAndShoot, add delay
if (target)
{
    Shoot();
    yield return new WaitForSeconds(0.5f);  // Longer delay
}
```

**3. Use object pooling for arrows**
- Create arrow pool instead of Instantiate/Destroy
- Reuse arrow GameObjects

### 11.5 Health Not Updating

**Problem:** Player takes damage but health bar doesn't change

**Causes:**

**1. Health bar not assigned**
- **Check:** Inherited `healthBar` variable
- **Solution:** Health bar auto-created in Enemy.Start()
- **Verify:** `healthBar != null` in TakeDamage()

**2. UpgradedCharacterParameter not set**
- **Check:** Inspector → Upgraded Character ID field
- **Solution:** Assign ScriptableObject with character stats

**3. TakeDamage not being called**
- **Debug:** Add `Debug.Log("Took damage: " + damage);` in TakeDamage()
- **Check:** Ensure arrows/enemies call TakeDamage() correctly

---

## 12. Related Systems

**Player_Archer depends on:**

| System | Purpose | Location |
|--------|---------|----------|
| Enemy (base class) | Health, damage, effects | AI/Enemy.cs |
| Controller2D | Movement physics | Controllers/Controller2D.cs |
| CheckTargetHelper | Enemy detection | Helpers/CheckTargetHelper.cs |
| ArrowProjectile | Arrow behavior | Controllers/ArrowProjectile.cs |
| UpgradedCharacterParameter | Stats storage | Player/UpgradedCharacterParameter.cs |
| GameManager | Game state | Managers/GameManager.cs |
| SoundManager | Audio | Managers/SoundManager.cs |
| FloatingTextManager | Damage numbers | UI/FloatingTextManager.cs |

**See Also:**
- `03_Enemy_System_Complete.md` - Enemy base class details
- `05_Managers_Complete.md` - GameManager, SoundManager
- `10_How_To_Guides.md` - Practical modification tutorials

---

**You now have a complete understanding of the Player System!**

**Next Document:** → `03_Enemy_System_Complete.md`
