# Troubleshooting Guide
## Common Problems and Solutions for "Lawn Defense: Monsters Out"

**Document Version**: 1.0
**Last Updated**: 2025-10-29
**Difficulty Level**: Beginner to Intermediate

---

## Table of Contents

1. [Introduction](#introduction)
2. [Quick Diagnostic Checklist](#quick-diagnostic-checklist)
3. [Compilation Errors](#compilation-errors)
4. [Movement Problems](#movement-problems)
5. [Shooting and Combat Issues](#shooting-and-combat-issues)
6. [Enemy AI Problems](#enemy-ai-problems)
7. [UI Issues](#ui-issues)
8. [Sound Problems](#sound-problems)
9. [Performance Issues](#performance-issues)
10. [Build Errors](#build-errors)
11. [Debug Techniques](#debug-techniques)
12. [Advanced Troubleshooting](#advanced-troubleshooting)

---

## Introduction

This guide helps you solve common problems you'll encounter while working with "Lawn Defense: Monsters Out". Each problem includes:

- **Symptom**: What you're experiencing
- **Common Causes**: Why this happens
- **Solution**: Step-by-step fix
- **Prevention**: How to avoid this in the future

**How to Use This Guide**:
1. Identify your problem category (Movement, Combat, UI, etc.)
2. Find the symptom that matches your issue
3. Follow the solution steps
4. Test the fix
5. Read the prevention tips to avoid repeating the issue

---

## Quick Diagnostic Checklist

Before diving into specific problems, run through this checklist:

### ✓ Basic Checks
- [ ] Is Unity in Play Mode? (Press Play button to test)
- [ ] Are there errors in the Console? (Window → General → Console)
- [ ] Are all required GameObjects in the scene?
- [ ] Are all Inspector fields assigned (no "None" values)?
- [ ] Is the correct scene loaded?

### ✓ Component Checks
- [ ] Does the GameObject have all required components?
- [ ] Are components enabled? (Check the checkbox next to component name)
- [ ] Are layer settings correct?
- [ ] Are tag settings correct?

### ✓ Code Checks
- [ ] Did you save the script? (Ctrl+S)
- [ ] Did Unity recompile? (Check bottom-right corner for spinning icon)
- [ ] Are there any typos in variable names?
- [ ] Are you using the correct namespace (RGame)?

**If all checks pass but problem persists, continue to specific sections below.**

---

## Compilation Errors

### Error 1: "The name 'X' does not exist in the current context"

**Symptom**: Red error in Console, code won't compile, Play button disabled.

**Example Error**:
```
Assets/Scripts/MyScript.cs(25,13): error CS0103: The name 'GameManager' does not exist in the current context
```

**Common Causes**:
1. Missing `using` statement
2. Typo in class name
3. Missing namespace

**Solution**:

**Step 1**: Check if you're missing the namespace
```csharp
// ❌ WRONG - Missing namespace
using UnityEngine;

public class MyScript : MonoBehaviour
{
    void Start()
    {
        GameManager.Instance.Victory(); // ERROR!
    }
}

// ✅ CORRECT - Add namespace
using UnityEngine;
using RGame; // Add this!

public class MyScript : MonoBehaviour
{
    void Start()
    {
        GameManager.Instance.Victory(); // Works!
    }
}
```

**Step 2**: If error persists, check the class name spelling
```csharp
// Common typos:
GameManger     // ❌ Missing 'a'
gameManager    // ❌ Wrong capitalization
Game_Manager   // ❌ Underscore instead of no space
GameManager    // ✅ Correct
```

**Prevention**:
- Always include `using RGame;` at the top of scripts
- Use Unity's autocomplete (Ctrl+Space) to avoid typos
- Keep Console window visible to catch errors immediately

---

### Error 2: "NullReferenceException: Object reference not set to an instance of an object"

**Symptom**: Game runs but crashes with error when certain code executes.

**Example Error**:
```
NullReferenceException: Object reference not set to an instance of an object
Player_Archer.Update () (at Assets/Scripts/Player_Archer.cs:45)
```

**Common Causes**:
1. Inspector field not assigned
2. GameObject not found
3. Component missing

**Solution**:

**Step 1**: Check the line number mentioned in error (line 45 in example)

```csharp
// Line 45 - The problematic line
healthBar.UpdateHealth(currentHealth); // ← This crashes
```

**Step 2**: Add a null check
```csharp
// ❌ WRONG - No safety check
void Update()
{
    healthBar.UpdateHealth(currentHealth); // Crashes if healthBar is null
}

// ✅ CORRECT - Add null check
void Update()
{
    if (healthBar != null)
    {
        healthBar.UpdateHealth(currentHealth);
    }
    else
    {
        Debug.LogError("HealthBar is not assigned!");
    }
}
```

**Step 3**: Find why the variable is null

**Scenario A**: Public variable not assigned in Inspector
```csharp
public HealthBar healthBar; // ← Check Inspector!
```
**Fix**: Select the GameObject → In Inspector, drag the HealthBar object to the field

**Scenario B**: GetComponent returns null
```csharp
void Start()
{
    healthBar = GetComponent<HealthBar>(); // Returns null if component missing

    if (healthBar == null)
    {
        Debug.LogError("HealthBar component is missing!");
    }
}
```
**Fix**: Add the missing component to the GameObject

**Scenario C**: FindObjectOfType returns null
```csharp
void Start()
{
    gameManager = FindObjectOfType<GameManager>(); // Returns null if not in scene

    if (gameManager == null)
    {
        Debug.LogError("GameManager not found in scene!");
    }
}
```
**Fix**: Add GameManager to the scene

**Prevention**:
- Always assign Inspector fields before playing
- Use null checks for GetComponent and Find operations
- Add Debug.LogError messages to identify which variable is null

---

### Error 3: "MissingReferenceException: The object of type 'X' has been destroyed"

**Symptom**: Error appears when trying to access a destroyed GameObject.

**Common Causes**:
1. Trying to access GameObject after Destroy() was called
2. Coroutine running after GameObject destroyed

**Solution**:

```csharp
// ❌ WRONG - Accessing destroyed object
void OnEnemyDeath(Enemy enemy)
{
    Destroy(enemy.gameObject);

    // This will fail - enemy is destroyed!
    Debug.Log(enemy.name); // ERROR!
}

// ✅ CORRECT - Get data before destroying
void OnEnemyDeath(Enemy enemy)
{
    string enemyName = enemy.name; // Get data first
    Destroy(enemy.gameObject);     // Then destroy

    Debug.Log(enemyName); // Works!
}

// ✅ CORRECT - Check if null
void Update()
{
    if (target != null) // Always check first
    {
        // Use target safely
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );
    }
}
```

**For Coroutines**:
```csharp
// ❌ WRONG - Coroutine continues after destroy
IEnumerator AttackSequence()
{
    yield return new WaitForSeconds(2f);
    // If GameObject destroyed during wait, this fails:
    animator.SetTrigger("Attack"); // ERROR!
}

// ✅ CORRECT - Check if still alive
IEnumerator AttackSequence()
{
    yield return new WaitForSeconds(2f);

    // Check if this GameObject still exists
    if (this != null && gameObject != null)
    {
        animator.SetTrigger("Attack"); // Safe
    }
}
```

**Prevention**:
- Get data from objects before destroying them
- Add null checks in Update() and coroutines
- Stop coroutines before destroying objects: `StopAllCoroutines();`

---

### Error 4: "Method 'X' does not contain a definition for 'Y'"

**Symptom**: Trying to call a method that doesn't exist.

**Example Error**:
```
error CS1061: 'GameManager' does not contain a definition for 'GetScore'
```

**Common Causes**:
1. Method name typo
2. Method is private (can't access from other scripts)
3. Wrong class

**Solution**:

**Step 1**: Check method name spelling
```csharp
// Common typos:
GameManager.Instance.Vicotry();  // ❌ Typo: Vicotry
GameManager.Instance.victory();  // ❌ Wrong case: victory
GameManager.Instance.Victory();  // ✅ Correct
```

**Step 2**: Check method access level
```csharp
// In GameManager.cs
private void Victory() // ❌ Private - can't access from other scripts
{
    // ...
}

public void Victory() // ✅ Public - accessible everywhere
{
    // ...
}
```

**Step 3**: Verify you're calling the right class
```csharp
// ❌ WRONG - Calling wrong manager
SoundManager.Instance.Victory(); // SoundManager doesn't have Victory()

// ✅ CORRECT - Call the right manager
GameManager.Instance.Victory(); // GameManager has Victory()
```

**Prevention**:
- Use Unity autocomplete (Ctrl+Space) to see available methods
- Make methods public if they need to be called from other scripts
- Read error message carefully - it tells you which class is missing the method

---

## Movement Problems

### Problem 1: Player Not Moving

**Symptom**: Pressing WASD or arrow keys does nothing, player stays still.

**Common Causes**:
1. Input system not set up
2. Controller2D not configured
3. Movement code disabled
4. Layer collision issue

**Solution**:

**Diagnosis Step 1**: Check if input is being received
```csharp
void Update()
{
    // Add this debug line temporarily
    Debug.Log("Input: " + Input.GetAxisRaw("Horizontal"));

    // Your normal movement code...
}
```
**Run the game**: If Console shows "Input: 0" always → Input system problem (go to Step 2)
**If Console shows "Input: -1" or "Input: 1"** → Input works, movement problem (go to Step 3)

**Step 2**: Fix Input System

Open **Edit → Project Settings → Input Manager**

Verify these axes exist:
- **Horizontal**: Left/Right movement
  - Negative Button: `left` or `a`
  - Positive Button: `right` or `d`
- **Vertical**: Up/Down movement
  - Negative Button: `down` or `s`
  - Positive Button: `up` or `w`

**If using new Input System** (InputSystem_Actions.inputactions):
```csharp
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private PlayerInput playerInput;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        if (playerInput == null)
        {
            Debug.LogError("PlayerInput component missing!");
        }
    }
}
```
**Fix**: Add PlayerInput component to player GameObject

**Step 3**: Check Controller2D Setup

Select Player GameObject → Inspector → Controller2D component

Verify these settings:
- **Collision Mask**: Should include "Ground" layer
- **Horizontal Ray Count**: Minimum 4
- **Vertical Ray Count**: Minimum 4
- **Skin Width**: Around 0.015

**Visual Check**:
```
[Controller2D Component]
┌─────────────────────────────┐
│ Collision Mask:             │
│  ☑ Default                  │ ← Must check layers
│  ☑ Ground                   │    player can collide with
│  ☐ Enemy                    │
│                             │
│ Horizontal Ray Count: 4     │ ← Minimum 4
│ Vertical Ray Count: 4       │ ← Minimum 4
│ Skin Width: 0.015          │
└─────────────────────────────┘
```

**Step 4**: Check if movement code is enabled

```csharp
public class Player_Archer : Enemy
{
    public bool allowMoveByPlayer = true; // ← Should be true

    void Update()
    {
        if (!allowMoveByPlayer) // ← If false, can't move!
            return;

        // Movement code...
    }
}
```

**In Inspector**: Make sure "Allow Move By Player" is checked ✓

**Step 5**: Verify player isn't frozen

```csharp
void Update()
{
    // If player is frozen, can't move
    if (canNotMoveByFreeze)
    {
        Debug.Log("Player is frozen!"); // Add this debug
        return;
    }
}
```

**Fix**: Wait for freeze effect to end, or reset: `canNotMoveByFreeze = false;`

**Prevention**:
- Use Debug.Log to identify which check is failing
- Test input system in a simple empty scene first
- Keep Controller2D settings consistent across all characters

---

### Problem 2: Player Falls Through Floor

**Symptom**: Player drops through the ground instead of standing on it.

**Common Causes**:
1. Wrong collision layers
2. Missing colliders
3. Incorrect Controller2D mask

**Solution**:

**Step 1**: Check Ground has a collider

Select ground GameObject → Inspector

**Required components**:
- ✓ **Collider2D** (BoxCollider2D, PolygonCollider2D, etc.)
- ✓ **Layer**: Set to "Ground"

```
[Ground GameObject]
┌──────────────────────┐
│ Tag: Untagged        │
│ Layer: Ground        │ ← Very important!
├──────────────────────┤
│ Transform            │
│ Sprite Renderer      │
│ Box Collider 2D      │ ← Must have collider
│  ☑ Is Trigger: NO    │ ← Must be UNCHECKED
└──────────────────────┘
```

**Step 2**: Verify player's Controller2D collision mask

Select Player → Inspector → Controller2D

**Collision Mask** must include "Ground":
```
Collision Mask:
☑ Ground       ← MUST be checked
☐ Enemy        ← Usually unchecked
☑ Obstacle     ← Check if player should collide with obstacles
```

**Step 3**: Check if player has collider

Player should have:
- ✓ Controller2D component
- ✓ BoxCollider2D or other collider
- ✓ Layer set to "Player" (NOT "Ground")

**Step 4**: Debug with Gizmos

Add this to Controller2D.cs to visualize raycasts:
```csharp
void OnDrawGizmos()
{
    // Draw vertical raycasts
    Gizmos.color = Color.red;

    Vector2 rayOrigin = transform.position;
    float rayLength = 1f;

    // Draw downward ray
    Gizmos.DrawLine(rayOrigin, rayOrigin + Vector2.down * rayLength);
}
```

**In Scene view**: You'll see red lines. If they don't touch the ground, the player is too high.

**Prevention**:
- Always set correct layers for GameObjects
- Double-check collision matrix: **Edit → Project Settings → Physics 2D → Layer Collision Matrix**
- Make sure "Player" layer can collide with "Ground" layer (checkbox should be ✓)

---

### Problem 3: Player Stuck on Walls or Slopes

**Symptom**: Player gets stuck when touching walls, or stutters on slopes.

**Common Causes**:
1. Skin width too large
2. Too few raycasts
3. Collider shape issues

**Solution**:

**Step 1**: Adjust Controller2D skin width

Select Player → Controller2D component

```csharp
public float skinWidth = 0.015f; // ← Try values between 0.01 - 0.02
```

**Too small** (< 0.01): Might get stuck in walls
**Too large** (> 0.03): Might float above ground

**Step 2**: Increase raycast count

```csharp
// In Controller2D Inspector
Horizontal Ray Count: 6  // ← Increase from 4
Vertical Ray Count: 6    // ← Increase from 4
```

**More rays = smoother collision detection**

**Visual representation**:
```
4 Rays (can miss slopes):        6 Rays (better detection):
    |                                 |
    |        ╱                         |      ╱
    |      ╱                           |    ╱
    |    ╱   ← Gap!                    |  ╱ ← No gap
    |  ╱                               |╱
```

**Step 3**: Fix collider shape

Select Player → BoxCollider2D

```
[Box Collider 2D]
┌────────────────────┐
│ Offset: 0, 0       │ ← Should be centered
│ Size: 0.8, 1.8     │ ← Match sprite size
│                    │
│ [Edit Collider]    │ ← Click to adjust visually
└────────────────────┘
```

**Tips**:
- Make collider **slightly smaller** than sprite (0.8 instead of 1.0) to prevent edge catches
- Center the collider on the sprite

**Step 4**: Smooth slope movement

```csharp
// In Player movement code
void Move(Vector2 velocity)
{
    // Add this for smooth slopes
    velocity.y = Mathf.Max(velocity.y, -maxFallSpeed);

    controller.Move(velocity * Time.deltaTime);
}
```

**Prevention**:
- Test movement on different terrain types (flat, slopes, stairs)
- Keep skinWidth consistent (0.015 works for most cases)
- Use 6+ raycasts for complex terrain

---

### Problem 4: Player Moves Too Fast or Too Slow

**Symptom**: Movement speed feels wrong.

**Solution**:

**Step 1**: Check speed variable

Select Player → Inspector

```
[Player_Archer]
┌──────────────────────┐
│ Speed: 5             │ ← Adjust this
│                      │
│ Recommended values:  │
│ Walk: 3-5            │
│ Run: 7-10            │
│ Slow: 1-2            │
└──────────────────────┘
```

**Step 2**: Verify Time.deltaTime usage

```csharp
// ❌ WRONG - No deltaTime (framerate dependent)
void Update()
{
    transform.position += new Vector3(speed, 0, 0); // Too fast on fast PCs!
}

// ✅ CORRECT - With deltaTime (framerate independent)
void Update()
{
    transform.position += new Vector3(speed, 0, 0) * Time.deltaTime;
}
```

**Step 3**: Check if speed modifiers are active

```csharp
void Update()
{
    float currentSpeed = speed;

    // Add debug to see actual speed
    Debug.Log("Current Speed: " + currentSpeed);

    // Check for modifiers
    if (canNotMoveByFreeze)
    {
        currentSpeed *= 0.5f; // Slowed by freeze
    }

    // Your movement code...
}
```

**Prevention**:
- Always use `Time.deltaTime` for movement
- Test speed on different framerates (vsync on/off)
- Document recommended speed values in comments

---

## Shooting and Combat Issues

### Problem 5: Arrows/Projectiles Not Spawning

**Symptom**: Pressing attack button does nothing, no arrows appear.

**Common Causes**:
1. Arrow prefab not assigned
2. Spawn point not set
3. Cooldown not finished
4. No ammo/mana

**Solution**:

**Step 1**: Check prefab assignment

Select Player → Inspector → Find the shooting script

```
[Player_Archer]
┌──────────────────────────────┐
│ Arrow Prefab: None (Game...) │ ← Should show arrow prefab
│ Spawn Point: None (Trans... │ ← Should show spawn point
└──────────────────────────────┘
```

**Fix**: Drag arrow prefab from Project to "Arrow Prefab" field

**Step 2**: Verify spawn point exists

Check player GameObject hierarchy:
```
Player
├── Sprite
├── SpawnPoint  ← This should exist!
└── ...
```

**If missing**:
1. Right-click Player → Create Empty
2. Name it "SpawnPoint"
3. Position it in front of player (X: 0.5, Y: 0, Z: 0)
4. Drag SpawnPoint to the "Spawn Point" field

**Step 3**: Check attack cooldown

```csharp
public float attackCooldown = 1f;  // 1 second between shots
private float lastAttackTime;

void Update()
{
    // Add debug
    if (Input.GetButtonDown("Fire1"))
    {
        float timeSinceLastShot = Time.time - lastAttackTime;
        Debug.Log("Time since last shot: " + timeSinceLastShot);

        if (timeSinceLastShot >= attackCooldown)
        {
            Shoot();
            lastAttackTime = Time.time;
        }
        else
        {
            Debug.Log("Cooldown not finished!");
        }
    }
}
```

**If "Cooldown not finished!" appears constantly**, reduce cooldown value.

**Step 4**: Check if attack animation is playing

```csharp
void Shoot()
{
    // Make sure animation doesn't block spawning
    Debug.Log("Shoot() called");

    // Spawn immediately, don't wait for animation
    GameObject arrow = Instantiate(arrowPrefab, spawnPoint.position, Quaternion.identity);
    Debug.Log("Arrow spawned: " + arrow.name);

    // Play animation after
    animator.SetTrigger("Attack");
}
```

**Prevention**:
- Test prefab assignments before playing
- Add Debug.Log to track execution flow
- Set reasonable cooldown values (0.5-1 second for rapid fire, 2-3 for heavy attacks)

---

### Problem 6: Arrows Go in Wrong Direction

**Symptom**: Arrows shoot backwards, upwards, or in random directions.

**Common Causes**:
1. Wrong transform.right direction
2. Player sprite flipped incorrectly
3. Spawn point positioned wrong

**Solution**:

**Step 1**: Visualize shoot direction

```csharp
void Update()
{
    if (Input.GetButtonDown("Fire1"))
    {
        // Draw a debug line showing shoot direction
        Debug.DrawRay(
            spawnPoint.position,
            transform.right * 2f,  // ← Check this direction
            Color.green,
            2f
        );

        Shoot();
    }
}
```

**In Scene view while playing**: You'll see a green line showing shoot direction.

**Step 2**: Fix direction based on player facing

```csharp
void Shoot()
{
    GameObject arrow = Instantiate(arrowPrefab, spawnPoint.position, Quaternion.identity);
    Arrow arrowScript = arrow.GetComponent<Arrow>();

    // Determine direction based on sprite flip
    Vector2 shootDirection;

    if (spriteRenderer.flipX) // Facing left
    {
        shootDirection = Vector2.left;
    }
    else // Facing right
    {
        shootDirection = Vector2.right;
    }

    Debug.Log("Shoot direction: " + shootDirection);
    arrowScript.SetDirection(shootDirection);
}
```

**Step 3**: Check spawn point position

Select Player → SpawnPoint child object

```
[SpawnPoint Transform]
Position X:  0.5   ← Positive = in front when facing right
Position Y:  0.3   ← Match character's hand/bow height
Position Z:  0
```

**Test both directions**:
- When facing RIGHT: X should be positive (0.5)
- When facing LEFT: X should be negative (-0.5)

**Auto-adjust spawn point**:
```csharp
void Shoot()
{
    // Adjust spawn point based on facing direction
    Vector3 spawnOffset = spriteRenderer.flipX
        ? new Vector3(-0.5f, 0.3f, 0)  // Left
        : new Vector3(0.5f, 0.3f, 0);  // Right

    Vector3 spawnPos = transform.position + spawnOffset;

    GameObject arrow = Instantiate(arrowPrefab, spawnPos, Quaternion.identity);
}
```

**Prevention**:
- Always test shooting in both directions (left and right)
- Use Debug.DrawRay to visualize directions
- Keep spawn point logic consistent with sprite flipping

---

### Problem 7: Auto-Aim Not Working

**Symptom**: Arrows should target enemies but shoot straight instead.

**Common Causes**:
1. No enemies in range
2. Layer mask not set
3. Target selection logic broken

**Solution**:

**Step 1**: Verify enemies are in range

```csharp
void FindTarget()
{
    // Add debug to see what's found
    Collider2D[] hits = Physics2D.OverlapCircleAll(
        transform.position,
        attackRange,
        enemyLayer
    );

    Debug.Log("Enemies found in range: " + hits.Length);

    foreach (var hit in hits)
    {
        Debug.Log("Found: " + hit.name);
    }
}

// Visualize range
void OnDrawGizmosSelected()
{
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(transform.position, attackRange);
}
```

**In Scene view**: You'll see a yellow circle. Enemies inside should be detected.

**Step 2**: Check enemy layer mask

Select Player → Inspector

```
[Player_Archer]
┌──────────────────────────┐
│ Attack Range: 10         │ ← Increase if too small
│ Enemy Layer: Enemy       │ ← Must be set to "Enemy"
└──────────────────────────┘
```

Verify enemies have "Enemy" layer:
Select enemy → Top of Inspector → Layer: Enemy

**Step 3**: Fix target selection

```csharp
public Transform FindNearestEnemy()
{
    Collider2D[] enemies = Physics2D.OverlapCircleAll(
        transform.position,
        attackRange,
        enemyLayer
    );

    if (enemies.Length == 0)
    {
        Debug.Log("No enemies in range");
        return null;
    }

    Transform nearest = null;
    float shortestDistance = Mathf.Infinity;

    foreach (var enemy in enemies)
    {
        float distance = Vector2.Distance(transform.position, enemy.transform.position);

        if (distance < shortestDistance)
        {
            shortestDistance = distance;
            nearest = enemy.transform;
        }
    }

    Debug.Log("Nearest enemy: " + (nearest != null ? nearest.name : "None"));
    return nearest;
}
```

**Step 4**: Verify trajectory calculation

```csharp
void ShootAtTarget(Transform target)
{
    if (target == null)
    {
        Debug.LogWarning("No target for auto-aim!");
        ShootStraight(); // Fallback to straight shot
        return;
    }

    Vector2 direction = (target.position - spawnPoint.position).normalized;
    Debug.DrawRay(spawnPoint.position, direction * 5f, Color.red, 1f);

    GameObject arrow = Instantiate(arrowPrefab, spawnPoint.position, Quaternion.identity);
    arrow.GetComponent<Arrow>().SetDirection(direction);
}
```

**Prevention**:
- Use OnDrawGizmosSelected to visualize attack range
- Test with multiple enemies at different distances
- Add fallback behavior if no target found

---

### Problem 8: Damage Not Being Applied

**Symptom**: Arrows hit enemies but don't reduce their health.

**Common Causes**:
1. ICanTakeDamage not implemented
2. Damage value set to 0
3. Collision/trigger not set up correctly

**Solution**:

**Step 1**: Check arrow collision

Select Arrow prefab → Inspector

```
[Arrow]
┌──────────────────────────────┐
│ Rigidbody 2D                 │
│  Body Type: Dynamic          │
│                              │
│ Circle Collider 2D           │
│  ☑ Is Trigger: YES           │ ← Must be checked for OnTriggerEnter2D
│  Radius: 0.1                 │
└──────────────────────────────┘
```

**Step 2**: Verify collision detection code

```csharp
// In Arrow.cs
void OnTriggerEnter2D(Collider2D other)
{
    Debug.Log("Arrow hit: " + other.name);

    // Check if target can take damage
    ICanTakeDamage damageable = other.GetComponent<ICanTakeDamage>();

    if (damageable != null)
    {
        Debug.Log("Applying " + damage + " damage to " + other.name);
        damageable.TakeDamage(damage, Vector2.zero, gameObject);
    }
    else
    {
        Debug.LogWarning(other.name + " cannot take damage (no ICanTakeDamage interface)");
    }

    Destroy(gameObject);
}
```

**Step 3**: Ensure enemy implements ICanTakeDamage

```csharp
// Enemy must have this
public class Enemy : MonoBehaviour, ICanTakeDamage
{
    public void TakeDamage(float damage, Vector2 force, GameObject instigator)
    {
        Debug.Log(name + " taking " + damage + " damage");
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }
}
```

**Step 4**: Check damage value

Select Arrow prefab → Inspector
```
[Arrow]
┌──────────────────────────┐
│ Damage: 10               │ ← Must be > 0
│ Speed: 15                │
└──────────────────────────┘
```

**If damage is 0**, enemy won't take damage even if everything else works!

**Step 5**: Verify layer collision matrix

**Edit → Project Settings → Physics 2D → Layer Collision Matrix**

Make sure "Arrow" layer can collide with "Enemy" layer (checkbox ✓)

**Prevention**:
- Always add Debug.Log in OnTriggerEnter2D to track collisions
- Test damage values in Inspector
- Double-check interface implementations

---

## Enemy AI Problems

### Problem 9: Enemies Not Moving Toward Player

**Symptom**: Enemies spawn but stand still, don't walk toward fortress.

**Common Causes**:
1. State stuck in SPAWNING or IDLE
2. Target not set
3. Speed set to 0
4. Controller2D issues

**Solution**:

**Step 1**: Check enemy state

```csharp
// In SmartEnemyGrounded.cs
void Update()
{
    // Debug current state
    Debug.Log(name + " State: " + State);

    if (State == ENEMYSTATE.WALK)
    {
        // Enemy should move here
    }
}
```

**Run game**: If Console shows "State: SPAWNING" or "State: IDLE" continuously, state isn't changing.

**Fix state transition**:
```csharp
void Start()
{
    // Make sure to transition from SPAWNING to WALK
    StartCoroutine(SpawnCo());
}

IEnumerator SpawnCo()
{
    State = ENEMYSTATE.SPAWNING;
    yield return new WaitForSeconds(1f);

    State = ENEMYSTATE.WALK; // ← Make sure this runs
    Debug.Log(name + " changed to WALK state");
}
```

**Step 2**: Verify target is set

```csharp
void Update()
{
    if (target == null)
    {
        Debug.LogError(name + " has no target!");

        // Try to find target
        target = FindObjectOfType<TheFortrest>();

        if (target != null)
        {
            Debug.Log("Found target: " + target.name);
        }
    }
}
```

**Step 3**: Check movement speed

Select enemy → Inspector
```
[Enemy]
┌──────────────────────────┐
│ Speed: 3                 │ ← Must be > 0
│ State: WALK              │
└──────────────────────────┘
```

**If speed is 0**, enemy won't move!

**Step 4**: Verify Controller2D setup

Enemy needs:
- ✓ Controller2D component
- ✓ Collision mask set to "Ground"
- ✓ BoxCollider2D

**Step 5**: Check if frozen

```csharp
void Update()
{
    if (canNotMoveByFreeze)
    {
        Debug.Log(name + " is frozen - can't move");
        return; // ← This stops movement!
    }

    if (State == ENEMYSTATE.WALK)
    {
        MoveTowardTarget();
    }
}
```

**Prevention**:
- Add state debug logging during development
- Test enemy movement in isolation (single enemy in scene)
- Verify all required components are present

---

### Problem 10: Enemies Not Attacking

**Symptom**: Enemies reach player/fortress but don't attack.

**Common Causes**:
1. Attack range too small
2. No attack module attached
3. State not changing to ATTACK
4. Animation event not firing

**Solution**:

**Step 1**: Visualize attack range

```csharp
void OnDrawGizmosSelected()
{
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(transform.position, attackDistance);
}
```

**In Scene view**: Red circle shows attack range. Player/fortress must be inside for attack to trigger.

**Step 2**: Increase attack range

Select enemy → Inspector
```
[Enemy]
┌──────────────────────────┐
│ Attack Distance: 1.5     │ ← Increase if too small (try 2-3)
└──────────────────────────┘
```

**Step 3**: Check attack module

Enemy should have one of these components:
- EnemyMeleeAttack
- EnemyRangeAttack
- EnemyThrowAttack

**If missing**:
1. Select enemy GameObject
2. Add Component → Scripts → (choose attack type)
3. Assign damage value

**Step 4**: Verify attack detection logic

```csharp
void Update()
{
    if (State == ENEMYSTATE.WALK)
    {
        // Check if player in attack range
        float distanceToTarget = Vector2.Distance(transform.position, target.position);
        Debug.Log("Distance to target: " + distanceToTarget + " / Attack Range: " + attackDistance);

        if (distanceToTarget <= attackDistance)
        {
            Debug.Log("In attack range - changing to ATTACK state");
            State = ENEMYSTATE.ATTACK;
        }
    }
}
```

**Step 5**: Check animation events

Open enemy animation:
1. Window → Animation → Animation
2. Select enemy in Hierarchy
3. Find "Attack" animation clip
4. Check for **Animation Event** (white markers on timeline)

```
Attack Animation Timeline:
├─── Frame 0: Animation starts
├─── Frame 10: Animation Event "DealDamage()" ← This calls attack code
└─── Frame 20: Animation ends
```

**If event missing**:
1. Scrub to damage frame (usually mid-swing)
2. Click "Add Event" button
3. Select function: "DealDamage" or similar

**Prevention**:
- Test attack range with Gizmos visualization
- Verify animation events are set up
- Add debug logs to track state transitions

---

### Problem 11: Too Many Enemies Spawn

**Symptom**: Game spawns hundreds of enemies, overwhelming the player.

**Common Causes**:
1. Spawn loop not stopping
2. Wave counter broken
3. Multiple spawners active

**Solution**:

**Step 1**: Check LevelEnemyManager

```csharp
IEnumerator SpawningProcess()
{
    int totalEnemiesSpawned = 0;

    foreach (var wave in waves)
    {
        Debug.Log("Spawning wave " + wave.name + " with " + wave.TotalEnemies + " enemies");

        for (int i = 0; i < wave.TotalEnemies; i++)
        {
            SpawnEnemy(wave.enemyPrefab);
            totalEnemiesSpawned++;

            Debug.Log("Total spawned so far: " + totalEnemiesSpawned);

            yield return new WaitForSeconds(wave.spawnInterval);
        }

        yield return new WaitForSeconds(wave.timeToNextWave);
    }

    Debug.Log("Finished spawning! Total: " + totalEnemiesSpawned);
}
```

**Step 2**: Verify wave settings

Select LevelEnemyManager → Inspector → Waves array

```
Wave 0:
├─ Enemy Prefab: Goblin
├─ Total Enemies: 10      ← Should be reasonable (5-20)
├─ Spawn Interval: 2      ← Time between each enemy (1-3 seconds)
└─ Time To Next Wave: 5   ← Break between waves
```

**If Total Enemies is 999**, reduce to reasonable number!

**Step 3**: Check for multiple spawners

**Hierarchy window**:
```
Scene
├── GameManager
├── LevelEnemyManager (1)  ← Should only have ONE
├── LevelEnemyManager (2)  ← DELETE DUPLICATES!
└── ...
```

**Prevention**:
- Test wave configurations with small numbers first (3-5 enemies)
- Add debug logs to track spawn count
- Use singleton pattern for managers to prevent duplicates

---

## UI Issues

### Problem 12: Buttons Not Clickable

**Symptom**: Clicking UI buttons does nothing.

**Common Causes**:
1. No EventSystem in scene
2. Button missing OnClick event
3. Canvas raycast blocker
4. Button not interactable

**Solution**:

**Step 1**: Check for EventSystem

**Hierarchy window** should have:
```
Scene
├── Canvas
├── EventSystem  ← Must exist!
└── ...
```

**If missing**:
1. Right-click Hierarchy
2. UI → Event System

**Step 2**: Verify button setup

Select button → Inspector

```
[Button Component]
┌──────────────────────────────┐
│ ☑ Interactable: YES          │ ← Must be checked
│                              │
│ OnClick()                    │ ← Must have event
│  Runtime                     │
│  MenuManager.PlayGame()      │ ← Function assigned
└──────────────────────────────┘
```

**If "Interactable" is unchecked**, button won't respond to clicks!

**Step 3**: Check Canvas settings

Select Canvas → Inspector

```
[Canvas]
┌──────────────────────────────┐
│ Render Mode: Screen Space    │ ← Usually this for UI
│              Overlay         │
│                              │
│ [Graphic Raycaster]          │ ← Must have this component
└──────────────────────────────┘
```

**If Graphic Raycaster missing**:
Add Component → Event → Graphic Raycaster

**Step 4**: Check for blocking elements

UI elements are drawn in order. An invisible blocker might be in front:

```
Canvas
├── Background Panel
│   └── [Image with Raycast Target ✓]  ← This blocks clicks!
└── Button
    └── (Can't be clicked)
```

**Fix**: Select blocking element → Image component → **Uncheck "Raycast Target"**

**Step 5**: Verify Z position

**Inspector → Transform**:
```
Button Transform:
Position Z: 0  ← Should be 0 or negative to be in front
```

**If Z > 0**, button might be behind other UI elements!

**Prevention**:
- Always check for EventSystem when creating UI
- Test buttons immediately after creating them
- Disable "Raycast Target" on decorative UI elements

---

### Problem 13: Health Bar Not Updating

**Symptom**: Player takes damage but health bar stays full.

**Common Causes**:
1. Slider not assigned
2. Update code not running
3. Lerp speed too slow

**Solution**:

**Step 1**: Check slider assignment

Select HealthBarUI GameObject → Inspector

```
[UI_UI Script]
┌──────────────────────────────┐
│ Health Slider: None (Slider) │ ← Must be assigned!
└──────────────────────────────┘
```

**Fix**: Drag health slider object to this field

**Step 2**: Verify update code

```csharp
// In UI_UI.cs
void IPlayer()
{
    Debug.Log("Updating health UI");

    if (healthSlider != null)
    {
        healthValue = GameManager.Instance.Player.currentHealth /
                     GameManager.Instance.Player.maxHealth;
        Debug.Log("Health value: " + healthValue);
    }
    else
    {
        Debug.LogError("Health Slider is not assigned!");
    }
}

void Update()
{
    if (healthSlider != null)
    {
        healthSlider.value = Mathf.Lerp(
            healthSlider.value,
            healthValue,
            lerpSpeed * Time.deltaTime
        );
    }
}
```

**Step 3**: Increase lerp speed

Select health bar → Inspector
```
[UI_UI]
┌──────────────────────────────┐
│ Lerp Speed: 5                │ ← Increase if too slow (try 10-20)
└──────────────────────────────┘
```

**Too slow** (< 3): Health bar lags behind actual health
**Too fast** (> 50): Health bar snaps instantly (no smooth animation)

**Step 4**: Check if IListener is registered

```csharp
void Start()
{
    // Must register as listener to receive updates
    GameManager.Instance.listeners.Add(this);
    Debug.Log("UI registered as listener");
}
```

**Step 5**: Verify player reference

```csharp
void Start()
{
    if (GameManager.Instance.Player == null)
    {
        Debug.LogError("GameManager has no Player reference!");
    }
    else
    {
        Debug.Log("Player found: " + GameManager.Instance.Player.name);
    }
}
```

**Prevention**:
- Test health UI by manually reducing player health in Inspector
- Add debug logs to track when IPlayer() is called
- Visualize health value with Debug.Log

---

### Problem 14: Victory/Fail Screen Not Showing

**Symptom**: Level ends but victory/fail screen doesn't appear.

**Common Causes**:
1. Menu prefab not assigned
2. MenuManager not in scene
3. IListener not registered
4. Canvas sort order wrong

**Solution**:

**Step 1**: Check MenuManager exists

**Hierarchy**:
```
Scene
├── MenuManager  ← Must exist
│   └── Menus
│       ├── VictoryMenu
│       └── FailMenu
└── ...
```

**Step 2**: Verify menu assignment

Select MenuManager → Inspector

```
[MenuManager]
┌──────────────────────────────┐
│ Menu Victory: None (Game...  │ ← Assign victory menu prefab
│ Menu Fail: None (GameObject) │ ← Assign fail menu prefab
└──────────────────────────────┘
```

**Step 3**: Check if menus are instantiated

```csharp
// In MenuManager.cs
public void ISuccess()
{
    Debug.Log("Victory triggered!");

    if (menuVictory != null)
    {
        GameObject menu = Instantiate(menuVictory);
        Debug.Log("Victory menu spawned: " + menu.name);
    }
    else
    {
        Debug.LogError("Menu Victory prefab is not assigned!");
    }
}
```

**Step 4**: Verify Canvas sort order

Victory menu might be behind other UI:

Select victory menu → Canvas component

```
[Canvas]
┌──────────────────────────────┐
│ Override Sorting: ✓          │ ← Check this
│ Sort Order: 100              │ ← High number = in front
└──────────────────────────────┘
```

**Step 5**: Check game state

```csharp
// In GameManager.cs
public void Victory()
{
    Debug.Log("Victory() called");
    State = GameState.Success;

    Debug.Log("Notifying " + listeners.Count + " listeners");

    foreach (var item in listeners)
    {
        if (item != null)
        {
            item.ISuccess();
        }
    }
}
```

**Prevention**:
- Test victory/fail conditions early
- Add debug logs to track game state changes
- Verify all listeners are registered in Start()

---

## Sound Problems

### Problem 15: No Sound Playing

**Symptom**: Game runs but no music or sound effects.

**Common Causes**:
1. Audio clips not assigned
2. Volume set to 0
3. Audio Listener missing
4. Muted in Unity/System

**Solution**:

**Step 1**: Check system volume

- **Unity Game view**: Look for mute icon, make sure not muted
- **System volume**: Check Windows/Mac volume mixer
- **Headphones/Speakers**: Verify they're connected and on

**Step 2**: Verify Audio Listener exists

**Must have exactly ONE Audio Listener in scene** (usually on Main Camera)

```
Main Camera
├── Transform
├── Camera
├── Audio Listener  ← Must have this!
└── ...
```

**If missing**:
Add Component → Audio → Audio Listener

**If multiple listeners exist**, remove extras (only keep one!)

**Step 3**: Check SoundManager setup

Select SoundManager → Inspector

```
[SoundManager]
┌──────────────────────────────┐
│ Music Volume: 0.5            │ ← Must be > 0
│ Sound Volume: 1              │ ← Must be > 0
│                              │
│ Music Sources [2]            │ ← Must have audio sources
│  Element 0: AudioSource      │
│  Element 1: AudioSource      │
│                              │
│ Sound Victory Panel:         │ ← Assign audio clips
│   None (Audio Clip)          │
└──────────────────────────────┘
```

**Step 4**: Verify audio clips assigned

```csharp
// In SoundManager.cs
public static void PlaySfx(AudioClip sound)
{
    if (sound == null)
    {
        Debug.LogWarning("Trying to play null sound!");
        return;
    }

    Debug.Log("Playing sound: " + sound.name);
    Instance.soundSource.PlayOneShot(sound);
}
```

**Step 5**: Check audio import settings

Select audio file in Project → Inspector

```
[Audio Clip Import Settings]
┌──────────────────────────────┐
│ Load Type: Decompress On     │ ← Try different options
│            Load               │
│                              │
│ Preload Audio Data: ✓        │ ← Check this
│                              │
│ [Apply] [Revert]             │ ← Click Apply!
└──────────────────────────────┘
```

**Prevention**:
- Test audio immediately after importing
- Add debug logs to all PlaySfx calls
- Keep volume sliders visible during testing

---

### Problem 16: Music Not Looping

**Symptom**: Background music plays once then stops.

**Solution**:

**Step 1**: Check AudioSource loop setting

Select SoundManager → Music Audio Source → Inspector

```
[Audio Source]
┌──────────────────────────────┐
│ Audio Clip: music GAME       │
│ ☑ Play On Awake              │
│ ☑ Loop                       │ ← MUST be checked!
│ Volume: 0.5                  │
└──────────────────────────────┘
```

**Step 2**: Verify in code

```csharp
void PlayMusic(AudioClip music)
{
    musicSource.clip = music;
    musicSource.loop = true;  // ← Make sure this is set
    musicSource.Play();

    Debug.Log("Playing music: " + music.name + ", Loop: " + musicSource.loop);
}
```

**Prevention**:
- Always set `loop = true` for background music
- Use separate AudioSource for music vs. sound effects

---

## Performance Issues

### Problem 17: Low FPS / Game Stuttering

**Symptom**: Game runs slowly, choppy framerate.

**Common Causes**:
1. Too many GameObjects
2. Inefficient code in Update()
3. Too many particle effects
4. Unoptimized sprites

**Solution**:

**Step 1**: Check FPS

Add FPS counter:
```csharp
public class FPSDisplay : MonoBehaviour
{
    private float deltaTime = 0f;

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.} FPS", fps);

        GUIStyle style = new GUIStyle();
        style.fontSize = 24;
        style.normal.textColor = fps < 30 ? Color.red : Color.green;

        GUI.Label(new Rect(10, 10, 100, 30), text, style);
    }
}
```

**Target FPS**:
- **> 60 FPS**: Excellent
- **30-60 FPS**: Acceptable
- **< 30 FPS**: Need optimization!

**Step 2**: Optimize Update() calls

```csharp
// ❌ BAD - FindObjectOfType every frame!
void Update()
{
    GameManager gm = FindObjectOfType<GameManager>(); // SLOW!
    // ...
}

// ✅ GOOD - Cache reference once
private GameManager gm;

void Start()
{
    gm = FindObjectOfType<GameManager>(); // Once only
}

void Update()
{
    // Use cached reference
    if (gm.State == GameState.Playing)
    {
        // ...
    }
}
```

**Common expensive operations to avoid in Update()**:
- ❌ FindObjectOfType / FindObjectsOfType
- ❌ GameObject.Find
- ❌ GetComponent (cache it in Start instead)
- ❌ String operations (use StringBuilder)

**Step 3**: Limit particle effects

Too many particles = lag

Select ParticleSystem → Inspector
```
[Particle System]
┌──────────────────────────────┐
│ Max Particles: 100           │ ← Reduce if lagging (try 50)
│ Emission Rate: 20            │ ← Lower = better performance
└──────────────────────────────┘
```

**Step 4**: Object pooling for frequently spawned objects

```csharp
// ❌ BAD - Creating/destroying constantly
void Shoot()
{
    GameObject arrow = Instantiate(arrowPrefab); // Creates garbage!
    // ...
    Destroy(arrow); // More garbage!
}

// ✅ GOOD - Object pooling
public class ArrowPool : MonoBehaviour
{
    private Queue<GameObject> pool = new Queue<GameObject>();
    public GameObject arrowPrefab;
    public int poolSize = 20;

    void Start()
    {
        // Pre-create arrows
        for (int i = 0; i < poolSize; i++)
        {
            GameObject arrow = Instantiate(arrowPrefab);
            arrow.SetActive(false);
            pool.Enqueue(arrow);
        }
    }

    public GameObject GetArrow()
    {
        if (pool.Count > 0)
        {
            GameObject arrow = pool.Dequeue();
            arrow.SetActive(true);
            return arrow;
        }

        // Pool empty, create new
        return Instantiate(arrowPrefab);
    }

    public void ReturnArrow(GameObject arrow)
    {
        arrow.SetActive(false);
        pool.Enqueue(arrow);
    }
}
```

**Step 5**: Optimize sprite settings

Select sprite → Inspector

```
[Sprite Import Settings]
┌──────────────────────────────┐
│ Texture Type: Sprite (2D)    │
│ Max Size: 2048               │ ← Reduce if too large (try 1024)
│ Compression: Automatic       │ ← Use compression!
│                              │
│ [Apply]                      │
└──────────────────────────────┘
```

**Prevention**:
- Profile regularly (Window → Analysis → Profiler)
- Use object pooling for bullets/enemies/effects
- Cache component references
- Test on target platform (mobile/PC)

---

### Problem 18: Memory Leaks

**Symptom**: Game uses more RAM over time, eventually crashes.

**Common Causes**:
1. Not removing listeners
2. Coroutines not stopped
3. Static references not cleared

**Solution**:

**Step 1**: Clean up listeners

```csharp
// ❌ BAD - Listener never removed
void Start()
{
    GameManager.Instance.listeners.Add(this);
}

// ✅ GOOD - Remove when destroyed
void Start()
{
    GameManager.Instance.listeners.Add(this);
}

void OnDestroy()
{
    if (GameManager.Instance != null)
    {
        GameManager.Instance.listeners.Remove(this);
    }
}
```

**Step 2**: Stop coroutines properly

```csharp
// ❌ BAD - Coroutine keeps running after object destroyed
void Start()
{
    StartCoroutine(RepeatForever());
}

IEnumerator RepeatForever()
{
    while (true)
    {
        // This runs forever, even after GameObject destroyed!
        yield return new WaitForSeconds(1f);
    }
}

// ✅ GOOD - Stop coroutine on destroy
private Coroutine repeatCoroutine;

void Start()
{
    repeatCoroutine = StartCoroutine(RepeatForever());
}

void OnDestroy()
{
    if (repeatCoroutine != null)
    {
        StopCoroutine(repeatCoroutine);
    }
}
```

**Step 3**: Clear static references

```csharp
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        // ✅ Clear static reference
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
```

**Prevention**:
- Always implement OnDestroy() to clean up
- Use Profiler to monitor memory usage
- Avoid unnecessary static references

---

## Build Errors

### Problem 19: "Scene 'X' is not in Build Settings"

**Symptom**: Game crashes when trying to load a scene.

**Solution**:

**Step 1**: Open Build Settings

**File → Build Settings**

**Step 2**: Add scenes

```
Scenes In Build:
┌──────────────────────────────┐
│ ☑ 0  MainMenu                │ ← All scenes must be listed
│ ☑ 1  Level1                  │
│ ☑ 2  Level2                  │
│ ☐ 3  TestScene               │ ← Uncheck test scenes
│                              │
│ [Add Open Scenes]            │ ← Click to add current scene
└──────────────────────────────┘
```

**Step 3**: Verify scene loading code

```csharp
// ❌ BAD - Scene name might be wrong
SceneManager.LoadScene("level1"); // Wrong capitalization!

// ✅ GOOD - Use exact name from Build Settings
SceneManager.LoadScene("Level1");

// ✅ BETTER - Use scene index (faster)
SceneManager.LoadScene(1);
```

**Prevention**:
- Add all scenes to Build Settings immediately after creating them
- Use scene indices instead of names when possible

---

### Problem 20: "Namespace 'RGame' could not be found" (Build only)

**Symptom**: Game works in Editor but fails to build.

**Solution**:

**Step 1**: Check for Editor-only code

```csharp
// ❌ BAD - UnityEditor only works in Editor!
using UnityEditor; // This breaks builds!

public class MyScript : MonoBehaviour
{
    // ...
}

// ✅ GOOD - Wrap editor code in #if
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MyScript : MonoBehaviour
{
    // ...

    #if UNITY_EDITOR
    [MenuItem("Tools/My Tool")]
    static void MyTool()
    {
        // Editor-only code
    }
    #endif
}
```

**Step 2**: Verify all scripts compile

**Window → General → Console**

Clear console, then try to build. Check for errors.

**Prevention**:
- Avoid UnityEditor namespace in gameplay scripts
- Test builds regularly, not just Editor playmode

---

## Debug Techniques

### Technique 1: Using Debug.Log Effectively

**Basic logging**:
```csharp
void Start()
{
    Debug.Log("Script started");
    Debug.LogWarning("This is a warning");
    Debug.LogError("This is an error");
}
```

**Contextual logging** (click log to select object):
```csharp
void Start()
{
    Debug.Log("Player health: " + currentHealth, this);
    //                                           ^^^^ Click log → selects this GameObject
}
```

**Rich text formatting**:
```csharp
void Update()
{
    Debug.Log("<color=red>DANGER!</color> Health low: " + currentHealth);
    Debug.Log("<b>Bold text</b> and <i>italic text</i>");
    Debug.Log("<size=20>Large text</size>");
}
```

**Conditional logging**:
```csharp
// Only log in Development builds
void Update()
{
    if (Debug.isDebugBuild)
    {
        Debug.Log("Frame: " + Time.frameCount);
    }
}

// Custom debug flag
public class MyScript : MonoBehaviour
{
    public bool enableDebugLogs = true;

    void DebugLog(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log(message);
        }
    }
}
```

---

### Technique 2: Using Debug.DrawRay and Debug.DrawLine

**Visualize raycasts**:
```csharp
void Update()
{
    Vector2 direction = transform.right;
    float distance = 5f;

    // Draw ray in Scene view
    Debug.DrawRay(
        transform.position,
        direction * distance,
        Color.red,
        2f // Duration in seconds
    );

    // Perform actual raycast
    RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance);

    if (hit.collider != null)
    {
        // Draw line to hit point
        Debug.DrawLine(transform.position, hit.point, Color.green, 2f);
    }
}
```

**Visualize paths**:
```csharp
void ShowPath(List<Vector3> waypoints)
{
    for (int i = 0; i < waypoints.Count - 1; i++)
    {
        Debug.DrawLine(
            waypoints[i],
            waypoints[i + 1],
            Color.yellow,
            5f
        );
    }
}
```

---

### Technique 3: Using Gizmos

**Draw shapes in Scene view**:
```csharp
void OnDrawGizmos()
{
    // Always visible
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(transform.position, attackRange);
}

void OnDrawGizmosSelected()
{
    // Only when GameObject selected
    Gizmos.color = Color.red;
    Gizmos.DrawWireCube(transform.position, new Vector3(1, 1, 0));

    // Draw line to target
    if (target != null)
    {
        Gizmos.DrawLine(transform.position, target.position);
    }
}
```

**Custom icons**:
```csharp
void OnDrawGizmos()
{
    // Draw icon at GameObject position
    Gizmos.DrawIcon(transform.position, "SpawnPoint.png", true);
}
```

---

### Technique 4: Using Breakpoints and Unity Debugger

**Step 1**: Set breakpoint

In your code editor (Visual Studio / VS Code):
1. Click in the left margin next to line number
2. Red dot appears = breakpoint set

**Step 2**: Attach Unity debugger

**In Visual Studio**:
- Debug → Attach to Unity

**In VS Code**:
- Install "Debugger for Unity" extension
- Press F5

**Step 3**: Debug features

When breakpoint hits:
- **Step Over (F10)**: Execute current line, move to next
- **Step Into (F11)**: Enter function calls
- **Step Out (Shift+F11)**: Exit current function
- **Continue (F5)**: Run until next breakpoint

**Inspect variables**:
- Hover over variable to see value
- Add to Watch window to track value
- Modify values in real-time!

---

### Technique 5: Unity Profiler

**Open Profiler**:
**Window → Analysis → Profiler**

**Key areas to check**:

**CPU Usage**:
- Shows which functions take most time
- Look for spikes (sudden high usage)
- Target: Stay under 16ms per frame (60 FPS)

**Memory**:
- Total Allocated: Should stabilize, not constantly grow
- GC Alloc: Garbage created per frame (lower is better)

**Rendering**:
- SetPass Calls: Number of material changes (lower is better)
- Batches: Draw calls (lower is better)

**How to use**:
1. Click "Record" button
2. Play game
3. Look for red areas (performance problems)
4. Click spike to see what caused it
5. Optimize that code

---

### Technique 6: Inspector Debugging

**Public variables for live tuning**:
```csharp
public class Enemy : MonoBehaviour
{
    [Header("Debug")]
    public bool showDebugInfo = true;

    [Header("Stats - Editable at Runtime")]
    public float speed = 3f;        // ← Can change while playing!
    public float health = 100f;

    void OnGUI()
    {
        if (showDebugInfo)
        {
            GUI.Label(new Rect(10, 10, 200, 20), "State: " + State);
            GUI.Label(new Rect(10, 30, 200, 20), "Health: " + health);
        }
    }
}
```

**Play Mode Tinting**:
**Edit → Preferences → Colors → Playmode tint**

Change color so you know when in Play Mode (prevents accidental edits)

**Lock Inspector**:
Click lock icon (top-right of Inspector) → Inspector stays on selected object even when clicking elsewhere

---

## Advanced Troubleshooting

### Advanced Problem 1: Coroutine Stops Working

**Symptom**: Coroutine starts but doesn't complete.

**Common Causes**:
1. GameObject disabled during execution
2. Exception thrown
3. Yield condition never met

**Solution**:

**Add debug logs at each yield point**:
```csharp
IEnumerator MyCoroutine()
{
    Debug.Log("Coroutine started");

    yield return new WaitForSeconds(2f);
    Debug.Log("After 2 second wait"); // ← Does this print?

    yield return StartCoroutine(OtherCoroutine());
    Debug.Log("After OtherCoroutine"); // ← Does this print?

    Debug.Log("Coroutine finished");
}
```

**Check if GameObject disabled**:
```csharp
IEnumerator MyCoroutine()
{
    yield return new WaitForSeconds(5f);

    // If GameObject disabled during wait, this never runs!
    Debug.Log("After wait");
}

// Solution: Check if still active
IEnumerator MyCoroutine()
{
    yield return new WaitForSeconds(5f);

    if (!gameObject.activeInHierarchy)
    {
        Debug.LogWarning("GameObject was disabled!");
        yield break; // Exit coroutine
    }

    Debug.Log("After wait");
}
```

**Catch exceptions**:
```csharp
IEnumerator MyCoroutine()
{
    try
    {
        yield return new WaitForSeconds(2f);

        // This might throw exception
        DoSomethingRisky();
    }
    catch (System.Exception e)
    {
        Debug.LogError("Coroutine exception: " + e.Message);
    }
}
```

---

### Advanced Problem 2: Singleton Pattern Issues

**Symptom**: "Instance is null" or multiple instances exist.

**Robust singleton implementation**:
```csharp
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();

                if (_instance == null)
                {
                    Debug.LogError("GameManager not found in scene!");
                }
            }

            return _instance;
        }
    }

    void Awake()
    {
        // Ensure only one instance exists
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("Duplicate GameManager found! Destroying: " + name);
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject); // Optional: Persist between scenes
    }

    void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}
```

**Usage safety**:
```csharp
// ❌ BAD - Might be null
void Start()
{
    GameManager.Instance.Victory(); // Crashes if no GameManager!
}

// ✅ GOOD - Check first
void Start()
{
    if (GameManager.Instance != null)
    {
        GameManager.Instance.Victory();
    }
    else
    {
        Debug.LogError("GameManager not found!");
    }
}
```

---

### Advanced Problem 3: Physics Not Working as Expected

**Common issues**:

**1. Collision not detected**:
```csharp
// For collision detection to work, you need:
// ✓ Both objects have Collider2D
// ✓ At least one has Rigidbody2D (or use overlap checks)
// ✓ Layers can collide (Physics 2D settings)
// ✓ Objects are on same Z plane (or close enough)

void OnCollisionEnter2D(Collision2D collision)
{
    Debug.Log("Collision detected: " + collision.gameObject.name);
}
```

**2. Trigger vs Collision**:
```csharp
// Use OnTriggerEnter2D when:
// - Collider "Is Trigger" is checked ✓
// - Objects should pass through each other
// - Detecting zones (pickup items, damage areas)

void OnTriggerEnter2D(Collider2D other)
{
    Debug.Log("Trigger entered: " + other.name);
}

// Use OnCollisionEnter2D when:
// - Collider "Is Trigger" is unchecked
// - Objects should physically block each other
// - Need collision force/impulse

void OnCollisionEnter2D(Collision2D collision)
{
    Debug.Log("Collision detected: " + collision.gameObject.name);
}
```

**3. Raycasts not hitting**:
```csharp
// Common mistakes:

// ❌ WRONG - Using 3D Physics2D.Raycast on 2D objects
RaycastHit hit;
Physics.Raycast(transform.position, Vector3.right, out hit);

// ✅ CORRECT - Use Physics2D for 2D games
RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, 10f);

// ❌ WRONG - Forgetting layer mask
RaycastHit2D hit = Physics2D.Raycast(start, direction); // Hits everything!

// ✅ CORRECT - Use layer mask
int layerMask = 1 << LayerMask.NameToLayer("Enemy");
RaycastHit2D hit = Physics2D.Raycast(start, direction, distance, layerMask);
```

---

## Summary

This troubleshooting guide covers the most common issues in "Lawn Defense: Monsters Out":

**Quick Reference by Category**:
- **Compilation**: Namespace issues, null references, method signatures
- **Movement**: Input system, Controller2D setup, collision layers
- **Combat**: Prefab assignment, collision detection, damage system
- **Enemy AI**: State machine, targeting, attack range
- **UI**: EventSystem, button setup, health bars, menus
- **Sound**: Audio Listener, volume, clip assignment
- **Performance**: FPS optimization, memory leaks, object pooling
- **Build**: Scene settings, platform-specific issues

**General Debugging Strategy**:
1. **Identify the problem category** (Movement? UI? Combat?)
2. **Add Debug.Log statements** to track code execution
3. **Use visualizations** (Debug.DrawRay, Gizmos) to see what's happening
4. **Isolate the problem** (test in empty scene with minimal setup)
5. **Check Inspector values** (are variables assigned?)
6. **Verify component setup** (all required components present?)
7. **Test incrementally** (add one feature at a time)

**Remember**:
- **Console is your friend** - read error messages carefully
- **Debug early, debug often** - don't wait until something breaks
- **Test in isolation** - remove complexity to find root cause
- **Save often** - use version control (Git) to revert changes

**Additional Resources**:
- Unity Manual: https://docs.unity3d.com/Manual/
- Unity Scripting API: https://docs.unity3d.com/ScriptReference/
- Unity Forums: https://forum.unity.com/
- Stack Overflow (Unity tag): https://stackoverflow.com/questions/tagged/unity3d

---

**Document End**

For more guides, see:
- **00_START_HERE.md** - Learning path and overview
- **10_How_To_Guides.md** - Step-by-step tutorials for adding features
- **12_Visual_Reference.md** - Visual diagrams and screenshots

Good luck with your game development!