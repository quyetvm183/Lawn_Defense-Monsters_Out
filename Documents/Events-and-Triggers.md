# Events and Triggers System

**Complete Guide to Event Communication and Trigger Points**

---

## Table of Contents

1. [Overview](#overview)
2. [Unity Basics: Events Explained](#unity-basics-events-explained)
3. [System 1: Observer Pattern (IListener)](#system-1-observer-pattern-ilistener)
4. [System 2: Animation Events](#system-2-animation-events)
5. [System 3: Collision and Trigger Events](#system-3-collision-and-trigger-events)
6. [System 4: UI Button Events](#system-4-ui-button-events)
7. [System 5: Custom Game Events](#system-5-custom-game-events)
8. [Complete Event Flow Diagrams](#complete-event-flow-diagrams)
9. [Debugging Event System](#debugging-event-system)
10. [Troubleshooting](#troubleshooting)
11. [Cross-References](#cross-references)

---

## Overview

**Purpose**: This document explains all event and trigger systems in the game - how objects communicate, react to state changes, and coordinate behavior.

**What You'll Learn**:
- How the Observer pattern (IListener) broadcasts game state changes
- How Animation Events trigger code during animations
- How collision detection works with projectiles and triggers
- How UI buttons connect to game logic
- How to debug event flow problems

**Time to Read**: 20-25 minutes
**Difficulty**: Intermediate

---

## Unity Basics: Events Explained

### What is an Event?

An **event** is something that happens at a specific moment that other objects need to know about.

**Real-World Analogy**: Think of a doorbell:
- **Event**: Someone presses the doorbell button
- **Listener**: The doorbell chime that makes a sound
- **Broadcast**: The electrical signal from button to chime

**In Games**:
- **Event**: "Player clicked Play button"
- **Listeners**: All enemies, UI panels, audio manager
- **Broadcast**: GameManager tells everyone "game started"

### Unity Event Types

This project uses **5 different event systems**:

1. **Observer Pattern** (IListener) - Broadcast to many objects
2. **Animation Events** - Code triggered by animation frames
3. **Collision Events** (OnTriggerEnter2D) - Physical contact detection
4. **UI Events** (Button.onClick) - User interface interactions
5. **Custom Events** (Invoke, Coroutines) - Time-delayed actions

### Why Multiple Systems?

Different problems need different solutions:

**Observer Pattern**: When many objects need to react to the same event
- ✅ Example: Game paused → all enemies stop, UI shows pause menu, music fades

**Animation Events**: When code must run at exact animation frame
- ✅ Example: Sword swing animation → spawn hit detection at frame 5

**Collision Events**: When objects physically touch
- ✅ Example: Arrow hits enemy → apply damage

**UI Events**: When player clicks buttons
- ✅ Example: Click "Buy" → purchase character

**Custom Events**: When something happens after a delay
- ✅ Example: Enemy dies → wait 2 seconds → spawn next wave

---

## System 1: Observer Pattern (IListener)

### What is the Observer Pattern?

The Observer pattern lets one object (the **subject**) notify many objects (the **observers**) when something happens.

**Analogy**: Newsletter subscription
- **Subject**: Newsletter publisher (GameManager)
- **Observers**: Subscribers (Enemies, UI, Managers)
- **Event**: New newsletter published → all subscribers get it

### The IListener Interface

```csharp
public interface IListener
{
    void IPlay();              // Game started
    void ISuccess();           // Player won
    void IGameOver();          // Player lost
    void IPause();             // Game paused
    void IUnPause();           // Game resumed
    void IOnRespawn();         // Player respawned
    void IOnStopMovingOn();    // Stop all movement
    void IOnStopMovingOff();   // Resume movement
}
```

**How it works**:
1. Objects **implement IListener** (subscribe to newsletter)
2. Objects **register with GameManager** (give their email address)
3. GameManager **broadcasts events** (sends newsletter to all subscribers)
4. Objects **receive notification** and react

### Implementation Example: Enemy

```csharp
using UnityEngine;
using RGame;

public class Enemy : MonoBehaviour, IListener
{
    public bool isPlaying { get; set; }

    void OnEnable()
    {
        // Subscribe to events
        GameManager.Instance.AddListener(this);
        isPlaying = true;
    }

    void OnDestroy()
    {
        // Unsubscribe when destroyed
        GameManager.Instance?.RemoveListener(this);
    }

    // ===== EVENT HANDLERS =====

    public virtual void IPlay()
    {
        // Called when game starts
        isPlaying = true;
        Debug.Log($"{gameObject.name}: Game started, begin AI");
    }

    public virtual void ISuccess()
    {
        // Called when player wins
        isPlaying = false;
        StopAllCoroutines();
        Debug.Log($"{gameObject.name}: Player won, stop attacking");
    }

    public virtual void IGameOver()
    {
        // Called when player loses
        isPlaying = false;
        PlayVictoryAnimation();  // Enemy celebrates
        Debug.Log($"{gameObject.name}: Player lost, celebrate!");
    }

    public virtual void IPause()
    {
        // Called when game paused
        isPlaying = false;
        Debug.Log($"{gameObject.name}: Game paused");
    }

    public virtual void IUnPause()
    {
        // Called when game resumed
        isPlaying = true;
        Debug.Log($"{gameObject.name}: Game resumed");
    }

    public virtual void IOnRespawn()
    {
        // Called when player respawns (not commonly used)
    }

    public virtual void IOnStopMovingOn()
    {
        // Called to force stop all movement
    }

    public virtual void IOnStopMovingOff()
    {
        // Called to allow movement again
    }
}
```

### GameManager Broadcasting

```csharp
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private List<IListener> listeners = new List<IListener>();

    // Register a listener
    public void AddListener(IListener listener)
    {
        if (!listeners.Contains(listener))
        {
            listeners.Add(listener);
        }
    }

    // Unregister a listener
    public void RemoveListener(IListener listener)
    {
        if (listeners.Contains(listener))
        {
            listeners.Remove(listener);
        }
    }

    // Broadcast: Game Started
    public void StartGame()
    {
        State = GameState.Playing;

        foreach (var listener in listeners)
        {
            if (listener != null)
                listener.IPlay();  // ← Call IPlay() on everyone
        }
    }

    // Broadcast: Player Won
    public void Victory()
    {
        State = GameState.Success;

        foreach (var listener in listeners)
        {
            if (listener != null)
                listener.ISuccess();  // ← Call ISuccess() on everyone
        }
    }

    // Broadcast: Player Lost
    public void GameOver()
    {
        State = GameState.Fail;

        foreach (var listener in listeners)
        {
            if (listener != null)
                listener.IGameOver();  // ← Call IGameOver() on everyone
        }
    }

    // Broadcast: Game Paused
    public void Pause()
    {
        Time.timeScale = 0;  // Freeze time
        State = GameState.Pause;

        foreach (var listener in listeners)
        {
            if (listener != null)
                listener.IPause();
        }
    }

    // Broadcast: Game Resumed
    public void UnPause()
    {
        Time.timeScale = 1;  // Resume time
        State = GameState.Playing;

        foreach (var listener in listeners)
        {
            if (listener != null)
                listener.IUnPause();
        }
    }
}
```

### Observer Pattern Flow

```
                    ┌──────────────────┐
                    │  GameManager     │
                    │  (Subject)       │
                    └────────┬─────────┘
                             │
         ┌───────────────────┼───────────────────┐
         │                   │                   │
         v                   v                   v
  ┌─────────────┐    ┌─────────────┐    ┌─────────────┐
  │  Enemy1     │    │  Enemy2     │    │  UI Panel   │
  │ (Observer)  │    │ (Observer)  │    │ (Observer)  │
  └─────────────┘    └─────────────┘    └─────────────┘
         │                   │                   │
         │  implements       │  implements       │  implements
         │  IListener        │  IListener        │  IListener
         v                   v                   v
    IPlay()              IPlay()              IPlay()
    ISuccess()           ISuccess()           ISuccess()
    IGameOver()          IGameOver()          IGameOver()
    IPause()             IPause()             IPause()

When GameManager.StartGame() is called:
  1. GameManager loops through all listeners
  2. Calls listener.IPlay() on each one
  3. Each listener executes its IPlay() method
```

### Practical Example: Custom Listener

Let's create a custom object that listens to game events:

```csharp
using UnityEngine;
using RGame;

public class BackgroundMusic : MonoBehaviour, IListener
{
    public AudioClip menuMusic;
    public AudioClip gameMusic;
    public AudioClip victoryMusic;

    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        GameManager.Instance.AddListener(this);
    }

    void OnDestroy()
    {
        GameManager.Instance?.RemoveListener(this);
    }

    public void IPlay()
    {
        // Game started - play game music
        audioSource.clip = gameMusic;
        audioSource.loop = true;
        audioSource.Play();
        Debug.Log("Playing game music");
    }

    public void ISuccess()
    {
        // Player won - play victory music
        audioSource.loop = false;
        audioSource.clip = victoryMusic;
        audioSource.Play();
        Debug.Log("Playing victory music");
    }

    public void IGameOver()
    {
        // Player lost - silence
        audioSource.Stop();
        Debug.Log("Stopped music");
    }

    public void IPause()
    {
        // Game paused - pause music
        audioSource.Pause();
    }

    public void IUnPause()
    {
        // Game resumed - resume music
        audioSource.UnPause();
    }

    // Not used in this example
    public void IOnRespawn() { }
    public void IOnStopMovingOn() { }
    public void IOnStopMovingOff() { }
}
```

**To use**: Attach this script to a GameObject with an AudioSource component. It will automatically react to game state changes.

---

## System 2: Animation Events

### What are Animation Events?

**Animation Events** let you call code at specific frames in an animation.

**Example Use Cases**:
- Spawn sword hit effect when swing reaches the middle
- Play footstep sound when foot touches ground
- Shoot arrow when bow release animation plays

### How Animation Events Work

```
Animation Timeline:
Frame 0 ────────> Frame 10 ────────> Frame 20 ────────> Frame 30
   │                  │                  │                  │
   └─ "start"         └─ Event:         └─ "loop"         └─ "end"
      trigger            "AnimShoot()"
                            │
                            v
                    [Code executes]
                    ShootArrow()
```

### Setting Up Animation Events

**Step 1: Open Animation Window**
1. Select your character prefab
2. Window → Animation → Animation
3. Select the animation clip (e.g., "Attack")

**Step 2: Add Event Marker**
1. Move the timeline scrubber to the desired frame
2. Click the "Add Event" button (or right-click timeline)
3. A white marker appears on the timeline

**Step 3: Assign Function**
1. Select the event marker
2. In the Inspector, choose the function name from the dropdown
3. The function must be **public** and exist on a component attached to the GameObject

### Example: Enemy Attack Animation

```csharp
using UnityEngine;
using RGame;

public class EnemyWarrior : Enemy
{
    public GameObject swordSlashEffect;
    public Transform attackPoint;

    // ===== ANIMATION EVENTS =====
    // These methods are called by animation events

    public void AnimMeleeAttackStart()
    {
        // Called at start of attack animation
        Debug.Log("Attack animation started");
    }

    public void AnimMeleeAttack()
    {
        // Called when sword swings (middle of animation)
        SpawnHitbox();
        PlaySlashEffect();
        Debug.Log("Spawned attack hitbox");
    }

    public void AnimMeleeAttackEnd()
    {
        // Called at end of attack animation
        SetEnemyState(ENEMYSTATE.WALK);
        Debug.Log("Attack animation finished, resume walking");
    }

    void SpawnHitbox()
    {
        // Detect enemies in attack range
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            playerLayer
        );

        foreach (var hit in hits)
        {
            ICanTakeDamage target = hit.GetComponent<ICanTakeDamage>();
            if (target != null)
            {
                target.TakeDamage(meleeDamage, Vector2.zero, hit.transform.position, gameObject);
            }
        }
    }

    void PlaySlashEffect()
    {
        if (swordSlashEffect != null)
        {
            Instantiate(swordSlashEffect, attackPoint.position, attackPoint.rotation);
        }
    }
}
```

**Animation Setup** (in Unity Animator):
```
"Attack" animation clip:
Frame 0:  [No event]
Frame 3:  Event → AnimMeleeAttackStart()
Frame 12: Event → AnimMeleeAttack()       ← Sword hits here
Frame 25: Event → AnimMeleeAttackEnd()
```

### Common Animation Event Methods

**Enemy Scripts**:
- `AnimMeleeAttackStart()` - Start melee attack
- `AnimMeleeAttack()` - Execute melee damage
- `AnimShoot()` - Fire projectile
- `AnimThrow()` - Throw grenade/bomb
- `AnimDie()` - Destroy GameObject after death animation

**Player Scripts**:
- `AnimAttack()` - Execute player attack
- `AnimFootstep()` - Play footstep sound
- `AnimReload()` - Reload weapon

**Example: Death Animation with Cleanup**

```csharp
public class Enemy : MonoBehaviour
{
    public void Die()
    {
        SetEnemyState(ENEMYSTATE.DEATH);
        AnimSetTrigger("die");

        // Animation will call AnimDie() at the end
    }

    // Called by animation event at end of death animation
    public void AnimDie()
    {
        Destroy(gameObject);  // Remove enemy from scene
    }
}
```

---

## System 3: Collision and Trigger Events

### Physics-Based Events

Unity's 2D physics system fires events when objects collide or overlap.

**Two Types**:
1. **Colliders** - Physical collision (objects bounce)
2. **Triggers** - Overlap detection (no physics reaction)

### OnTriggerEnter2D - Overlap Detection

Used for: Projectiles, pickups, damage zones

```csharp
void OnTriggerEnter2D(Collider2D other)
{
    // Called when this object's trigger overlaps another collider
    Debug.Log($"Triggered by: {other.gameObject.name}");
}
```

**Requirements**:
- At least one object has **Is Trigger** checked
- Both objects have a **Collider2D** component
- At least one object has a **Rigidbody2D** component

### Projectile Collision Example

```csharp
using UnityEngine;
using RGame;

public class Arrow : MonoBehaviour
{
    public int damage = 10;
    public GameObject hitEffect;
    public LayerMask damageableLayers;  // What can be damaged
    public WeaponEffect weaponEffect;   // Poison, freeze, etc.

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object is on a damageable layer
        if (((1 << other.gameObject.layer) & damageableLayers) == 0)
        {
            // Not a target layer, ignore
            return;
        }

        // Try to damage the object
        ICanTakeDamage target = other.GetComponent<ICanTakeDamage>();
        if (target != null)
        {
            // Calculate hit point and direction
            Vector2 hitPoint = other.ClosestPoint(transform.position);
            Vector2 direction = (hitPoint - (Vector2)transform.position).normalized;
            Vector2 knockbackForce = direction * 5f;

            // Apply damage
            target.TakeDamage(
                damage,
                knockbackForce,
                hitPoint,
                gameObject,
                BODYPART.NONE,
                weaponEffect
            );

            Debug.Log($"Arrow hit {other.gameObject.name} for {damage} damage");
        }
        else
        {
            Debug.Log($"Arrow hit {other.gameObject.name} but it can't take damage");
        }

        // Spawn hit effect
        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }

        // Destroy arrow
        Destroy(gameObject);
    }
}
```

### Layer-Based Collision Filtering

**Problem**: Arrows shouldn't hit walls, allies, or other arrows.

**Solution**: Use LayerMask to filter targets

```csharp
public class ArrowScript : MonoBehaviour
{
    public LayerMask damageableLayers;  // Set in Inspector

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if object is on a valid layer
        int objectLayer = other.gameObject.layer;
        int layerMask = 1 << objectLayer;

        if ((layerMask & damageableLayers.value) == 0)
        {
            return;  // Not a target, ignore
        }

        // Proceed with damage...
    }
}
```

**Inspector Setup**:
```
ArrowScript Component:
  Damageable Layers:
    ☑ Enemy
    ☐ Player
    ☐ Ground
    ☐ Wall
```

### ICanTakeDamage Interface

Objects that can be damaged must implement this interface:

```csharp
public interface ICanTakeDamage
{
    void TakeDamage(
        float damage,
        Vector2 force,
        Vector2 hitPoint,
        GameObject instigator,
        BODYPART bodyPart = BODYPART.NONE,
        WeaponEffect weaponEffect = null
    );
}
```

**Implemented by**:
- `Enemy.cs`
- `Player_Archer.cs`
- `TheFortrest.cs` (player fortress)

### Collision Flow Diagram

```
┌─────────────┐
│ Arrow       │
│ (Trigger)   │
└──────┬──────┘
       │
       │ Overlaps
       v
┌─────────────┐
│ Enemy       │
│ (Collider)  │
└──────┬──────┘
       │
       │ OnTriggerEnter2D called
       v
┌──────────────────────────┐
│ Check Layer              │
│ Is object damageable?    │
└──────┬───────────────────┘
       │
       │ Yes
       v
┌──────────────────────────┐
│ Get ICanTakeDamage       │
│ Does component exist?    │
└──────┬───────────────────┘
       │
       │ Yes
       v
┌──────────────────────────┐
│ Call TakeDamage()        │
│ Apply damage and effects │
└──────────────────────────┘
       │
       v
┌──────────────────────────┐
│ Spawn hit effect         │
│ Destroy arrow            │
└──────────────────────────┘
```

---

## System 4: UI Button Events

### Button onClick Events

Unity's Button component has an **onClick** event that triggers when clicked.

### Connecting Buttons in Inspector

**Step 1: Select Button GameObject**
**Step 2: Find Button Component in Inspector**
**Step 3: Click + in OnClick() section**
**Step 4: Drag the target GameObject to the object slot**
**Step 5: Select the function from dropdown**

### Example: Play Button

```csharp
using UnityEngine;
using UnityEngine.UI;
using RGame;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    public GameObject menuPanel;
    public GameObject gameplayPanel;

    void Start()
    {
        Instance = this;
    }

    // Called by Play button's onClick event
    public void OnPlayButtonClicked()
    {
        Debug.Log("Play button clicked");

        // Hide menu
        menuPanel.SetActive(false);

        // Show gameplay UI
        gameplayPanel.SetActive(true);

        // Start game
        GameManager.Instance.StartGame();
    }

    // Called by Pause button
    public void OnPauseButtonClicked()
    {
        Debug.Log("Pause button clicked");
        GameManager.Instance.Pause();
    }

    // Called by Resume button
    public void OnResumeButtonClicked()
    {
        Debug.Log("Resume button clicked");
        GameManager.Instance.UnPause();
    }
}
```

**Inspector Setup for Play Button**:
```
Button Component:
  OnClick():
    Runtime Only
    [MenuManager GameObject] → MenuManager.OnPlayButtonClicked()
```

### Shop Button Example

```csharp
using UnityEngine;
using UnityEngine.UI;
using RGame;

public class BuyCharacterButton : MonoBehaviour
{
    public GameObject characterPrefab;
    public int price = 500;
    public Text priceText;

    void Start()
    {
        // Display price on button
        priceText.text = $"${price}";

        // Get the button component
        Button button = GetComponent<Button>();

        // Add click listener programmatically
        button.onClick.AddListener(OnButtonClicked);
    }

    void OnButtonClicked()
    {
        // Check if player has enough coins
        int coins = GameManager.Instance.GetCoins();

        if (coins >= price)
        {
            // Deduct coins
            GameManager.Instance.AddCoins(-price);

            // Unlock character
            UnlockCharacter();

            Debug.Log($"Purchased {characterPrefab.name} for {price} coins");
        }
        else
        {
            Debug.Log($"Not enough coins! Need {price}, have {coins}");
            ShowErrorMessage("Not enough coins!");
        }
    }

    void UnlockCharacter()
    {
        // Mark character as owned
        PlayerPrefs.SetInt(characterPrefab.name + "_Owned", 1);
        PlayerPrefs.Save();

        // Update button to show "Owned"
        GetComponent<Button>().interactable = false;
        priceText.text = "OWNED";
    }

    void ShowErrorMessage(string message)
    {
        // Display error UI (implementation varies)
    }
}
```

### Dynamic Button Setup

Sometimes buttons are created at runtime:

```csharp
public class CharacterSelectUI : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform buttonContainer;
    public GameObject[] characterPrefabs;

    void Start()
    {
        CreateCharacterButtons();
    }

    void CreateCharacterButtons()
    {
        foreach (var character in characterPrefabs)
        {
            // Create button
            GameObject buttonObj = Instantiate(buttonPrefab, buttonContainer);
            Button button = buttonObj.GetComponent<Button>();

            // Set button text
            Text buttonText = buttonObj.GetComponentInChildren<Text>();
            buttonText.text = character.name;

            // Add click listener (using closure to capture character)
            GameObject selectedCharacter = character;  // Capture for closure
            button.onClick.AddListener(() => OnCharacterSelected(selectedCharacter));
        }
    }

    void OnCharacterSelected(GameObject character)
    {
        Debug.Log($"Selected character: {character.name}");

        // Spawn character in game
        CharacterManager.Instance.SpawnCharacter(character);
    }
}
```

---

## System 5: Custom Game Events

### Invoke - Delayed Method Calls

**Invoke** calls a method after a delay:

```csharp
// Call MethodName() after 2 seconds
Invoke("MethodName", 2f);

// Call MethodName() after 2 seconds, then repeat every 1 second
InvokeRepeating("MethodName", 2f, 1f);

// Cancel all pending Invoke calls
CancelInvoke();

// Cancel specific method
CancelInvoke("MethodName");
```

**Example: Enemy Spawn Delay**

```csharp
public class Enemy : MonoBehaviour
{
    public float spawnDelay = 1f;

    void Start()
    {
        if (startBehavior == STARTBEHAVIOR.BURROWUP)
        {
            SetEnemyState(ENEMYSTATE.SPAWNING);
            AnimSetTrigger("spawn");

            // Finish spawning after delay
            Invoke("FinishSpawning", spawnDelay);
        }
    }

    void FinishSpawning()
    {
        if (enemyState == ENEMYSTATE.SPAWNING)
        {
            SetEnemyState(ENEMYSTATE.WALK);
            Debug.Log($"{gameObject.name} finished spawning");
        }
    }
}
```

### Coroutines - Complex Timing

**Coroutines** allow complex time-based behavior:

```csharp
// Start a coroutine
StartCoroutine(SpawnWaveCoroutine());

// Stop all coroutines on this object
StopAllCoroutines();

// Stop specific coroutine
StopCoroutine(myCoroutine);
```

**Example: Wave Spawner**

```csharp
using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public Transform spawnPoint;
    public int enemiesPerWave = 5;
    public float timeBetweenEnemies = 2f;
    public float timeBetweenWaves = 10f;

    void Start()
    {
        StartCoroutine(SpawnWavesCoroutine());
    }

    IEnumerator SpawnWavesCoroutine()
    {
        int waveNumber = 1;

        while (true)  // Infinite waves
        {
            Debug.Log($"Starting Wave {waveNumber}");

            // Spawn enemies
            for (int i = 0; i < enemiesPerWave; i++)
            {
                // Pick random enemy
                GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

                // Spawn enemy
                Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

                Debug.Log($"Spawned enemy {i + 1}/{enemiesPerWave}");

                // Wait before next enemy
                yield return new WaitForSeconds(timeBetweenEnemies);
            }

            Debug.Log($"Wave {waveNumber} complete");

            // Wait before next wave
            yield return new WaitForSeconds(timeBetweenWaves);

            waveNumber++;
        }
    }
}
```

### Custom Event Example: Burn Effect

```csharp
public class BurnEffect : MonoBehaviour
{
    public float burnDamagePerSecond = 5f;
    public float burnDuration = 3f;
    public GameObject burnParticles;

    public void ApplyBurn(GameObject target)
    {
        BurnTarget burnScript = target.AddComponent<BurnTarget>();
        burnScript.StartBurning(burnDamagePerSecond, burnDuration);
    }
}

public class BurnTarget : MonoBehaviour
{
    private ICanTakeDamage damageTarget;
    private GameObject particles;

    public void StartBurning(float damagePerSecond, float duration)
    {
        damageTarget = GetComponent<ICanTakeDamage>();
        if (damageTarget == null)
        {
            Destroy(this);
            return;
        }

        StartCoroutine(BurnCoroutine(damagePerSecond, duration));
    }

    IEnumerator BurnCoroutine(float damagePerSecond, float duration)
    {
        // Spawn burn particles
        particles = Instantiate(burnParticlesPrefab, transform);

        float elapsed = 0;

        while (elapsed < duration)
        {
            // Deal damage every second
            yield return new WaitForSeconds(1f);

            damageTarget.TakeDamage(damagePerSecond, Vector2.zero, transform.position, gameObject);

            elapsed += 1f;
            Debug.Log($"Burn damage: {damagePerSecond}, Time left: {duration - elapsed}");
        }

        // Clean up
        if (particles != null)
            Destroy(particles);

        Destroy(this);
    }
}
```

---

## Complete Event Flow Diagrams

### Game Start Sequence

```
Player clicks "Play" button
         |
         v
[Button.onClick fires]
         |
         v
MenuManager.OnPlayButtonClicked()
         |
         v
GameManager.StartGame()
         |
         v
[GameManager broadcasts IPlay()]
         |
    ┌────┴────┬────────┬─────────┐
    v         v        v         v
Enemy1    Enemy2    UI Panel  Audio Manager
 .IPlay()  .IPlay()  .IPlay()   .IPlay()
    |         |        |         |
    v         v        v         v
Start AI   Start AI  Show UI   Play Music
```

### Attack Animation Sequence

```
Enemy.PerformAttack()
         |
         v
SetEnemyState(ATTACK)
         |
         v
AnimSetTrigger("attack")
         |
         v
[Animator plays "Attack" animation]
         |
    Frame 0 → Frame 5 → Frame 10 → Frame 15
         |         |         |         |
         v         v         v         v
    [Start]  [Event]   [Middle]    [End]
              AnimMeleeAttack()
                   |
                   v
            SpawnHitbox()
                   |
                   v
         [Physics overlap check]
                   |
                   v
         Player.TakeDamage()
                   |
                   v
         [Player health decreases]
```

### Projectile Hit Sequence

```
Arrow flying
         |
         v
[Physics detects overlap]
         |
         v
Arrow.OnTriggerEnter2D(enemy)
         |
         v
[Check layer mask]
         |
         v
enemy.GetComponent<ICanTakeDamage>()
         |
         v
enemy.TakeDamage(damage, force, hitPoint, ...)
         |
         v
[Apply damage]
         |
    ┌────┴────┬────────────┐
    v         v            v
Update    Spawn       Check health
health    hit FX       <= 0?
bar                      |
                         v
                    enemy.Die()
                         |
                    ┌────┴────┐
                    v         v
             Spawn       GameManager
             death FX    .RemoveListener()
```

---

## Debugging Event System

### Debug.Log Strategy

**Add logs to track event flow**:

```csharp
public void IPlay()
{
    Debug.Log($"[IPlay] {gameObject.name} received IPlay event");
    isPlaying = true;
}

public void AnimShoot()
{
    Debug.Log($"[AnimShoot] {gameObject.name} shooting at frame {Time.frameCount}");
    SpawnProjectile();
}

void OnTriggerEnter2D(Collider2D other)
{
    Debug.Log($"[Collision] {gameObject.name} collided with {other.gameObject.name}");
    // ...
}
```

**Result in Console**:
```
[IPlay] Enemy_Goblin received IPlay event
[IPlay] Enemy_Skeleton received IPlay event
[IPlay] GameUI received IPlay event
[AnimShoot] Player_Archer shooting at frame 1234
[Collision] Arrow_01 collided with Enemy_Goblin
```

### Visual Debugging: Gizmos

**Draw collision areas**:

```csharp
void OnDrawGizmosSelected()
{
    // Draw attack range
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(attackPoint.position, attackRange);

    // Draw raycast
    Gizmos.color = Color.blue;
    Gizmos.DrawLine(transform.position, transform.position + Vector3.right * 5);
}
```

### Breakpoints

**Use Visual Studio/Rider debugger**:

1. Set breakpoint on event method (click left margin)
2. Start Unity in Play Mode
3. Trigger the event
4. Debugger pauses at breakpoint
5. Inspect variables

**Useful for**:
- Checking if event is called
- Seeing exact values
- Step-by-step execution

### Event Call Stack

**To see who called a method**:

```csharp
public void IPlay()
{
    Debug.Log($"IPlay called. Stack trace:\n{System.Environment.StackTrace}");
    // ...
}
```

**Output shows the call chain**:
```
IPlay called. Stack trace:
   at Enemy.IPlay()
   at GameManager.StartGame()
   at MenuManager.OnPlayButtonClicked()
   at UnityEngine.EventSystems.EventSystem.Update()
```

---

## Troubleshooting

### Problem 1: Event Not Firing

**Symptoms**: IPlay() never called, enemy doesn't react to game start

**Causes**:

**Cause A: Forgot to register listener**

```csharp
// WRONG: Never registered
void Start()
{
    // Missing: GameManager.Instance.AddListener(this);
}

// CORRECT:
void OnEnable()
{
    GameManager.Instance.AddListener(this);
}
```

**Cause B: GameManager not initialized**

```csharp
void OnEnable()
{
    // WRONG: GameManager might be null
    GameManager.Instance.AddListener(this);

    // CORRECT: Check null
    if (GameManager.Instance != null)
    {
        GameManager.Instance.AddListener(this);
    }
    else
    {
        Debug.LogError("GameManager not found!");
    }
}
```

---

### Problem 2: Animation Event Method Not Found

**Error**: `'AnimShoot' function not found on object 'Enemy_Goblin'`

**Causes**:

**Cause A: Method is private**

```csharp
// WRONG: private methods can't be called by animation events
private void AnimShoot() { }

// CORRECT: must be public
public void AnimShoot() { }
```

**Cause B: Method name mismatch**

```
Animation Event: "AnimShoot"
Script Method:   "AnimShot"  ← Typo!
```

**Cause C: Method on wrong object**

Animation events call methods on the **GameObject the Animator is attached to**, not children or parents.

---

### Problem 3: Collision Not Detected

**Symptoms**: Arrow passes through enemy without dealing damage

**Debugging Steps**:

**Step 1: Check colliders exist**

```csharp
void Start()
{
    Collider2D col = GetComponent<Collider2D>();
    if (col == null)
    {
        Debug.LogError($"{gameObject.name} missing Collider2D!");
    }
}
```

**Step 2: Check Is Trigger flag**

Arrow should have `Is Trigger` checked (overlap detection, not collision).

**Step 3: Check Rigidbody2D**

At least one object needs a Rigidbody2D (usually the arrow).

**Step 4: Check layers**

```csharp
void OnTriggerEnter2D(Collider2D other)
{
    Debug.Log($"Hit: {other.gameObject.name}, Layer: {LayerMask.LayerToName(other.gameObject.layer)}");

    int layerMask = 1 << other.gameObject.layer;
    bool isValid = (layerMask & damageableLayers.value) != 0;

    Debug.Log($"Valid target: {isValid}");
}
```

**Step 5: Check collision matrix**

Edit → Project Settings → Physics 2D → Layer Collision Matrix

Ensure "Arrow" layer can collide with "Enemy" layer.

---

### Problem 4: Button Click Not Working

**Symptoms**: Clicking button does nothing

**Debugging Steps**:

**Step 1: Check EventSystem exists**

UI buttons require an EventSystem. Check Hierarchy for `EventSystem` GameObject.

**Step 2: Check Raycast Target**

Button's Image component needs `Raycast Target` checked.

**Step 3: Check button is interactable**

```
Button Component:
  Interactable: ☑  ← Must be checked
```

**Step 4: Check onClick is assigned**

```csharp
void Start()
{
    Button button = GetComponent<Button>();
    if (button.onClick.GetPersistentEventCount() == 0)
    {
        Debug.LogWarning($"{gameObject.name}: No onClick listeners assigned!");
    }
}
```

**Step 5: Add debug log**

```csharp
public void OnButtonClicked()
{
    Debug.Log("Button clicked!");  // ← Add this first
    // ... rest of code
}
```

---

### Problem 5: Invoke Not Calling Method

**Symptoms**: Invoke("MethodName", 2f) doesn't call the method

**Cause A: Method name typo**

```csharp
// WRONG:
Invoke("FinshSpawning", 1f);  // Typo: "Finsh" instead of "Finish"

// CORRECT:
Invoke("FinishSpawning", 1f);
```

**Cause B: GameObject destroyed before delay**

```csharp
Invoke("DoSomething", 5f);
Destroy(gameObject);  // ← Destroys before Invoke fires!
```

**Cause C: Coroutine stopped**

```csharp
IEnumerator routine = MyCoroutine();
StartCoroutine(routine);
StopCoroutine(routine);  // ← Stops before completion
```

---

## Cross-References

**Related Documentation**:

- **[Core-Objects.md](Core-Objects.md)** - GameManager and Observer pattern deep dive
- **[03_Enemy_System_Complete.md](03_Enemy_System_Complete.md)** - Enemy AI and animation
- **[04_UI_System_Complete.md](04_UI_System_Complete.md)** - UI button setup
- **[02_Player_System_Complete.md](02_Player_System_Complete.md)** - Player input events
- **[11_Troubleshooting.md](11_Troubleshooting.md)** - General debugging guide
- **[12_Visual_Reference.md](12_Visual_Reference.md)** - Event system diagrams
- **[13_Code_Examples.md](13_Code_Examples.md)** - Coroutine patterns

**Key Scripts**:
- `GameManager.cs` - Observer pattern implementation
- `Enemy.cs` - IListener implementation
- `ArrowScript.cs` - Collision detection example
- `MenuManager.cs` - UI event handling

**Unity Documentation**:
- [Animation Events](https://docs.unity3d.com/Manual/script-AnimationWindowEvent.html)
- [OnTriggerEnter2D](https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnTriggerEnter2D.html)
- [Button.onClick](https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/script-Button.html)
- [Coroutines](https://docs.unity3d.com/Manual/Coroutines.html)
- [Invoke](https://docs.unity3d.com/ScriptReference/MonoBehaviour.Invoke.html)

---

## Summary

**What You Learned**:
- ✅ How the Observer pattern (IListener) broadcasts events to all registered objects
- ✅ How Animation Events call code at specific animation frames
- ✅ How collision detection works with OnTriggerEnter2D
- ✅ How UI buttons connect to game logic with onClick events
- ✅ How to create custom timed events with Invoke and Coroutines
- ✅ How to debug event flow problems

**5 Event Systems**:
1. **Observer Pattern** → Many objects react to one event (game state changes)
2. **Animation Events** → Code executes during animations (attack hitboxes)
3. **Collision Events** → Objects react to physical contact (projectiles hit enemies)
4. **UI Events** → Buttons trigger game actions (player clicks Buy)
5. **Custom Events** → Timed or conditional actions (spawn waves, burn damage)

**Key Debugging Tips**:
- Add Debug.Log() to every event method
- Use Visual Studio breakpoints to pause execution
- Check that listeners are registered with AddListener()
- Verify animation event methods are **public**
- Check layer masks for collision filtering
- Ensure EventSystem exists for UI buttons

**Next Steps**:
- Create a custom IListener object that reacts to game events
- Set up Animation Events for your own attack animations
- Build a projectile with proper collision detection
- Add UI buttons with debugging logs to verify they work

---

**Document Version**: 1.0
**Last Updated**: 2025-10-29
**Transformation**: Vietnamese (26 lines) → English (1,800+ lines)
