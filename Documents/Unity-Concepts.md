# Essential Unity Concepts (Quick Reference)

**Purpose**: Quick reference for essential Unity concepts used in this project.

**Note**: For complete Unity fundamentals, see `00_Unity_Fundamentals.md` (1,200+ lines with detailed explanations).

**This document provides**: Quick lookups and reminders for developers already familiar with Unity basics.

---

## 1. GameObject & Component System

**GameObject**: Container for components (like a "thing" in your game)

**Component**: Behavior or property attached to GameObject (Transform, Collider, Script, etc.)

```csharp
// Get component
Rigidbody2D rb = GetComponent<Rigidbody2D>();

// Add component at runtime
gameObject.AddComponent<BoxCollider2D>();

// Find GameObject
GameObject player = GameObject.Find("Player");
GameObject enemy = GameObject.FindWithTag("Enemy");
```

**See**: `00_Unity_Fundamentals.md` → "GameObject and Component Architecture"

---

## 2. Prefab System

**Prefab**: Template GameObject stored in Project window

**Why use prefabs**: Spawn multiple copies, update all instances at once

```csharp
// Instantiate prefab
GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

// Destroy instance
Destroy(enemy, 2f);  // Destroy after 2 seconds
```

**See**: `00_Unity_Fundamentals.md` → "Prefabs"

---

## 3. Scene Management

**Scene**: Container for GameObjects (like a level or menu)

```csharp
using UnityEngine.SceneManagement;

// Load scene
SceneManager.LoadScene("Game");

// Load asynchronously (with loading screen)
AsyncOperation op = SceneManager.LoadSceneAsync("Game");

// Get current scene
Scene current = SceneManager.GetActiveScene();
```

**See**: `Map.md` for complete scene loading guide

---

## 4. MonoBehaviour Lifecycle

**Order of execution**:
```
Awake() → OnEnable() → Start() → Update()/FixedUpdate()/LateUpdate() → OnDisable() → OnDestroy()
```

**Common methods**:
- `Awake()`: Initialize before Start (runs even if disabled)
- `Start()`: Initialize after Awake (only if enabled)
- `Update()`: Every frame (60 FPS = 60 times/second)
- `FixedUpdate()`: Fixed time step (for physics, default 50 FPS)
- `LateUpdate()`: After all Update() calls (for cameras)

**See**: `00_Unity_Fundamentals.md` → "MonoBehaviour Lifecycle"

---

## 5. Coroutines

**Purpose**: Run code over multiple frames with delays

```csharp
IEnumerator MyCoroutine()
{
    Debug.Log("Start");
    yield return new WaitForSeconds(2f);  // Wait 2 seconds
    Debug.Log("After 2 seconds");
}

// Start coroutine
StartCoroutine(MyCoroutine());

// Stop coroutine
StopCoroutine(MyCoroutine());
StopAllCoroutines();
```

**Common yield returns**:
- `yield return null` - Wait one frame
- `yield return new WaitForSeconds(2f)` - Wait 2 seconds
- `yield return new WaitForFixedUpdate()` - Wait until next FixedUpdate
- `yield return new WaitUntil(() => condition)` - Wait until condition true

**See**: `00_Unity_Fundamentals.md` → "Coroutines"

---

## 6. Physics2D System

**Collider2D**: Defines collision shape

**Rigidbody2D**: Adds physics simulation (gravity, forces)

```csharp
// Raycast
RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, layerMask);
if (hit.collider != null)
{
    Debug.Log("Hit: " + hit.collider.name);
}

// CircleCast (for area detection)
RaycastHit2D[] hits = Physics2D.CircleCastAll(center, radius, Vector2.zero, 0, layerMask);

// Trigger collision
void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("Player"))
    {
        Debug.Log("Player entered trigger!");
    }
}
```

**See**: `00_Unity_Fundamentals.md` → "Physics and Collisions"

---

## 7. Animation System

**Animator**: State machine controller for animations

**Animation Clip**: The actual animation data

```csharp
Animator anim = GetComponent<Animator>();

// Trigger animation
anim.SetTrigger("Jump");

// Set bool (for transitions)
anim.SetBool("IsRunning", true);

// Set float (for blend trees)
anim.SetFloat("Speed", 5.0f);
```

**See**: `Player-Deep.md` and `Enemy-Deep.md` for animation system usage

---

## 8. PlayerPrefs (Save Data)

**Purpose**: Simple key-value storage (persists between sessions)

```csharp
// Save data
PlayerPrefs.SetInt("highScore", 1000);
PlayerPrefs.SetFloat("volume", 0.8f);
PlayerPrefs.SetString("playerName", "Hero");
PlayerPrefs.Save();  // Force save (auto-saves on quit)

// Load data
int score = PlayerPrefs.GetInt("highScore", 0);  // Default 0 if not found
float vol = PlayerPrefs.GetFloat("volume", 1.0f);
string name = PlayerPrefs.GetString("playerName", "Player");

// Delete data
PlayerPrefs.DeleteKey("highScore");
PlayerPrefs.DeleteAll();  // Clear everything
```

**See**: `Character-Properties.md` for advanced PlayerPrefs usage

---

## 9. Events & Delegates

**UnityEvent**: Inspector-assignable event (like Button.onClick)

```csharp
using UnityEngine.Events;

public class MyScript : MonoBehaviour
{
    public UnityEvent onPlayerDied;  // Visible in Inspector

    void Die()
    {
        onPlayerDied.Invoke();  // Trigger all listeners
    }
}
```

**C# Delegate**:
```csharp
public delegate void OnEnemyKilled(Enemy enemy);
public static OnEnemyKilled onEnemyKilled;

// Subscribe
void OnEnable()
{
    onEnemyKilled += HandleEnemyKilled;
}

void OnDisable()
{
    onEnemyKilled -= HandleEnemyKilled;
}

void HandleEnemyKilled(Enemy enemy)
{
    Debug.Log(enemy.name + " was killed!");
}

// Invoke
onEnemyKilled?.Invoke(this);
```

**See**: `Events-and-Triggers.md` for complete event system guide

---

## 10. Layers & Tags

**Layers**: For collision filtering and raycasts (up to 32 layers)

**Tags**: For identifying GameObjects (unlimited)

```csharp
// Check tag
if (other.CompareTag("Enemy"))
{
    // Do something
}

// Check layer
if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
{
    // Do something
}

// LayerMask for raycasts (only hit specific layers)
LayerMask enemyLayer = LayerMask.GetMask("Enemy", "Boss");
RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, enemyLayer);
```

**See**: `00_Unity_Fundamentals.md` → "Layers and Tags"

---

## 11. Namespaces

**Purpose**: Organize code, avoid name conflicts

```csharp
namespace RGame
{
    public class Enemy : MonoBehaviour
    {
        // This is RGame.Enemy
    }
}

// Use with "using" directive
using RGame;

void Start()
{
    Enemy enemy = FindObjectOfType<Enemy>();  // No need for RGame.Enemy
}
```

**See**: `Namespaces.md` for complete namespace guide

---

## 12. Common Patterns in This Project

### Singleton Pattern

```csharp
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }
}

// Usage:
GameManager.Instance.StartGame();
```

### Observer Pattern (IListener)

```csharp
public interface IListener
{
    void IPlay();
    void IPause();
}

public class Enemy : MonoBehaviour, IListener
{
    void OnEnable()
    {
        GameManager.Instance.AddListener(this);
    }

    public void IPlay()
    {
        // Start enemy AI
    }
}
```

### State Machine (ENEMYSTATE enum)

```csharp
public enum ENEMYSTATE
{
    IDLE, WALK, ATTACK, HIT, DEATH
}

public ENEMYSTATE currentState;

void Update()
{
    switch (currentState)
    {
        case ENEMYSTATE.IDLE:
            // Idle behavior
            break;
        case ENEMYSTATE.WALK:
            // Walk behavior
            break;
    }
}
```

---

## Quick Comparison Table

| Concept | Purpose | Example |
|---------|---------|---------|
| GameObject | Container for components | `GameObject player` |
| Component | Behavior/property | `Rigidbody2D`, `BoxCollider2D` |
| Prefab | Template for GameObjects | `Instantiate(enemyPrefab)` |
| Scene | Level/menu container | `SceneManager.LoadScene("Game")` |
| Coroutine | Time-based operations | `yield return new WaitForSeconds(2f)` |
| PlayerPrefs | Save data | `PlayerPrefs.SetInt("score", 100)` |
| LayerMask | Collision filtering | `Physics2D.Raycast(..., layerMask)` |
| Tag | Identify GameObject | `CompareTag("Enemy")` |
| Namespace | Code organization | `namespace RGame { }` |

---

## Summary

This document provides quick lookups for Unity concepts. **For detailed explanations, tutorials, and examples**:

- **Complete Unity fundamentals**: See `00_Unity_Fundamentals.md`
- **Project-specific patterns**: See `01_Project_Architecture.md`
- **How-to guides**: See `10_How_To_Guides.md`
- **Troubleshooting**: See `11_Troubleshooting.md`
- **Code examples**: See `13_Code_Examples.md`

**Next Steps**:
- New to Unity? Start with `00_START_HERE.md`
- Need specific examples? Check `13_Code_Examples.md`
- Stuck on a problem? See `11_Troubleshooting.md`
