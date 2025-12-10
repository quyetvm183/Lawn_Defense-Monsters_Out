# Glossary - Unity & Project Terms A-Z

**Purpose:** Quick reference for Unity and project-specific terminology
**Usage:** Look up unfamiliar terms while reading documentation
**Format:** Term → Simple Definition → Context/Example

---

## A

**AdMob**
Mobile advertising platform by Google. Used in this project to show banner and interstitial ads.
→ See: `AdsManager.cs`

**Animator**
Unity component that controls animation playback. Uses Animation Controller with states and transitions.
→ Example: Player has Animator for walk, shoot, die animations

**Animation Clip**
Single animation sequence (e.g., "walk" animation).
→ Created in Unity Animation window

**Animation Controller**
State machine that manages which animation plays when.
→ Example: "Idle" → "Walk" when speed > 0

**Anchor (UI)**
Determines how UI element positions itself relative to parent. Crucial for multi-resolution support.
→ Example: Top-left anchor keeps button in corner

**API (Application Programming Interface)**
Set of functions/classes provided by a system. Unity API = all Unity classes/methods.
→ Example: `GameObject.Find()` is part of Unity API

**ArrowProjectile**
Custom class for arrow behavior. Handles trajectory, collision, damage.
→ Location: `Controllers/ArrowProjectile.cs`

**Asset**
Any file in Unity project (script, sprite, audio, prefab, scene).
→ Stored in `Assets/` folder

**Async (Asynchronous)**
Code that runs independently of main thread. Used for loading, networking.
→ Example: `SceneManager.LoadSceneAsync()`

**ATTACKTYPE (Enum)**
Defines enemy attack method: RANGE, MELEE, THROW, NONE.
→ Used by Enemy.cs to determine combat behavior

**Audio Clip**
Sound file imported into Unity (.wav, .mp3, .ogg).
→ Played via AudioSource component

**AudioSource**
Unity component that plays audio clips. Has volume, pitch, loop settings.

**Auto-Targeting**
System where player automatically aims at enemies without manual input.
→ Implemented in `Player_Archer.cs:276` (AutoCheckAndShoot coroutine)

**Awake()**
MonoBehaviour lifecycle method. Called when GameObject is created, before Start().
→ Use for: Initialization, GetComponent caching

---

## B

**Ballistic Trajectory**
Curved path of projectile affected by gravity. Player calculates this for arrows.
→ Physics formula: y = y₀ + v₀t - ½gt²

**Base Class**
Class that other classes inherit from.
→ Example: `Enemy` is base class for `Player_Archer` and `SmartEnemyGrounded`

**BODYPART (Enum)**
Specifies where damage hit occurred: NONE, HEAD, BODY, ARM, LEG.
→ Used in `ICanTakeDamage.TakeDamage()` for critical hits

**Boolean (bool)**
Variable type with two values: true or false.
→ Example: `isAvailable = true;`

**BoxCollider2D**
Unity component defining rectangular collision area for 2D objects.
→ Player and enemies use this for hit detection

**Build**
Process of compiling game into executable (Android APK, iOS IPA, PC .exe).
→ File → Build Settings → Build

**Bullet (Projectile)**
Generic term for any fired object (arrows, bullets, fireballs).
→ This project uses ArrowProjectile specifically

**Button (UI)**
Unity UI component for clickable buttons. Triggers onClick events.

---

## C

**C#**
Programming language used by Unity. Pronounced "C Sharp".
→ File extension: .cs

**Cache**
Store reference to avoid repeated lookups. Performance optimization.
→ Example: `Awake() { rb = GetComponent<Rigidbody2D>(); }`

**Canvas**
Unity UI component that holds all UI elements. Root of UI hierarchy.
→ Render modes: Screen Space Overlay, Camera, World Space

**CharacterManager**
Script that spawns player character at start of level.
→ Location: `Player/CharacterManager.cs`

**CheckTargetHelper**
Utility script for detecting targets using raycasts.
→ Used by Player and Enemy to find targets

**CircleCast**
Physics query that detects colliders in circular area. Like raycast but with width.
→ Example: `Physics2D.CircleCastAll(position, radius, ...)`

**Class**
Template for creating objects. Defines properties and methods.
→ Example: `public class Player_Archer : Enemy { }`

**Collider**
Unity component defining collision shape (box, circle, polygon).
→ Types in 2D: BoxCollider2D, CircleCollider2D, EdgeCollider2D

**Collision**
Physical interaction between two objects with colliders.
→ Triggers: `OnCollisionEnter2D()`, `OnCollisionStay2D()`, `OnCollisionExit2D()`

**Component**
Piece of functionality attached to GameObject (Transform, Renderer, Scripts).
→ All scripts are components

**Composition**
Design pattern: Build complex objects from simple components.
→ Unity's GameObject + Component system uses this

**Constructor**
Method called when creating new instance of class.
→ C# syntax: `public ClassName() { }`

**Controller2D**
Custom 2D physics controller using raycasts instead of Rigidbody2D.
→ Location: `Controllers/Controller2D.cs`
→ More precise control for platformers/side-scrollers

**Coroutine**
Function that can pause execution and resume later. Uses `yield`.
→ Example: `IEnumerator Countdown() { yield return new WaitForSeconds(1); }`

**Critical Hit**
Attack that deals extra damage, usually to specific body part (HEAD).
→ Detected via BODYPART enum in TakeDamage()

---

## D

**Damage Over Time (DoT)**
Damage applied gradually over period (poison, burn).
→ Implemented in Enemy.cs: BURNING, POISON effects

**Debug.Log()**
Print message to Unity Console for debugging.
→ Example: `Debug.Log("Health: " + health);`

**Debug.DrawRay()**
Visualize ray in Scene view (not Game view). Useful for debugging raycasts.
→ Example: `Debug.DrawRay(start, direction * length, Color.red);`

**Delegate**
Reference to method. Used for events and callbacks.
→ Example: `public delegate void OnDeath();`

**Delta Time**
Time since last frame. Use for frame-independent movement.
→ Access: `Time.deltaTime`
→ Example: `transform.position += velocity * Time.deltaTime;`

**Dependency**
When one system requires another to function.
→ Example: Player depends on Controller2D for movement

**Destroy()**
Remove GameObject from scene and free memory.
→ Example: `Destroy(gameObject);`
→ Delayed: `Destroy(gameObject, 2f);` (after 2 seconds)

**DIEBEHAVIOR (Enum)**
How enemy behaves when dying: NORMAL, DESTROY, BLOWUP.

**DontDestroyOnLoad()**
Prevent GameObject from being destroyed when loading new scene.
→ Example: `DontDestroyOnLoad(gameObject);`
→ Used for persistent managers

---

## E

**Editor**
Unity Editor application where you design game.
→ Includes: Scene view, Game view, Inspector, Hierarchy

**ENEMYEFFECT (Enum)**
Status effects: NONE, BURNING, FREEZE, SHOKING, POISON, EXPLOSION.
→ Managed in Enemy.cs

**ENEMYSTATE (Enum)**
Enemy state machine states: SPAWNING, IDLE, WALK, ATTACK, HIT, DEATH.

**Enemy**
Base class for all enemies AND player (unusual design choice).
→ Location: `AI/Enemy.cs`
→ Provides: Health, damage, effects, state machine

**Event**
Notification system. One object triggers, others listen.
→ Example: `public static event Action OnPlayerDeath;`

**Extension Method**
Add methods to existing classes without modifying them.
→ C# feature used in helpers

---

## F

**Field**
Variable declared in class. Can be public or private.
→ Example: `public float speed = 5f;`

**FixedUpdate()**
MonoBehaviour method called at fixed intervals (50 FPS default).
→ Use for: Physics calculations
→ More consistent than Update() for physics

**Flip**
Reverse character's facing direction (horizontally).
→ Implemented by rotating sprite 180° on Y-axis

**Float**
Decimal number type. Single-precision floating-point.
→ Example: `float speed = 5.5f;` (f suffix required)

**FloatingText**
Damage numbers that appear above characters.
→ Location: `UI/FloatingText.cs`, `UI/FloatingTextManager.cs`
→ Uses object pooling for performance

**Fortress (TheFortrest)**
Player's base that must be defended. Game over if destroyed.
→ Location: `AI/TheFortrest.cs`

**FPS (Frames Per Second)**
How many frames rendered per second. Higher = smoother.
→ Target: 60 FPS (set in GameManager.Awake)

**Function**
Block of code that performs task. Also called "method" in OOP.

---

## G

**GameObject**
Container object in Unity scenes. Everything is a GameObject.
→ Always has Transform component
→ Can have multiple components attached

**GameManager**
Singleton controlling game state, listeners, level loading.
→ Location: `Managers/GameManager.cs`
→ Access: `GameManager.Instance`

**GameState (Enum)**
Current game state: Menu, Playing, GameOver, Success, Pause.

**Garbage Collection (GC)**
Automatic memory cleanup. Can cause frame drops if excessive.
→ Avoid: Creating objects in Update()

**GetComponent**
Retrieve component from GameObject.
→ Example: `GetComponent<Rigidbody2D>()`
→ Slow - cache results in Awake/Start

**Gizmos**
Visual debugging helpers drawn in Scene view.
→ Example: `OnDrawGizmos() { Gizmos.DrawWireSphere(...); }`

**GlobalValue**
Static class storing player progress data via PlayerPrefs.
→ Location: `Helpers/GlobalValue.cs`
→ Contains: Coins, level progress, unlocks

**Gravity**
Downward force applied to objects. Can be modified per-object.
→ Player/Enemy: `gravity = 35f` field
→ Projectiles: `gravityScale` parameter

---

## H

**HealthBar**
UI element showing current health. Follows enemy/player.
→ Location: `UI/HealthBarEnemyNew.cs`

**Header Attribute**
Organizes Inspector by adding section headers.
→ Example: `[Header("Movement Settings")]`

**Hierarchy**
Unity window showing all GameObjects in current scene.
→ Tree structure (parent-child relationships)

**Hit Reaction**
Visual/audio feedback when taking damage.
→ Implemented in Enemy.Hit() and Player_Archer.Hit()

**HideInInspector Attribute**
Hide public field from Inspector.
→ Example: `[HideInInspector] public bool isDead;`

---

## I

**IAP (In-App Purchase)**
Buying items with real money in mobile games.
→ Location: `Managers/Purchaser.cs`, `Managers/IAPItem.cs`

**ICanTakeDamage**
Interface for objects that can receive damage.
→ Location: `AI/ICanTakeDamage.cs`
→ Implemented by: Enemy, Player, Fortress

**IDE (Integrated Development Environment)**
Code editor with advanced features. Unity uses Visual Studio or Rider.

**IEnumerator**
Return type for coroutines. Allows yield statements.
→ Example: `IEnumerator MyCoroutine() { yield return null; }`

**IListener**
Interface for game state event listeners.
→ Location: `Helpers/IListener.cs`
→ Methods: IPlay(), IPause(), IUnPause(), IGameOver(), ISuccess()

**Inheritance**
Class derives from another, gaining its properties/methods.
→ Example: `Player_Archer : Enemy`

**Inspector**
Unity window showing properties of selected GameObject/asset.
→ Modify public fields here

**Instantiate()**
Create copy of GameObject or prefab at runtime.
→ Example: `Instantiate(enemyPrefab, position, rotation);`

**Integer (int)**
Whole number (no decimals).
→ Example: `int health = 100;`

**Interface**
Contract defining methods a class must implement.
→ Example: `public interface ICanTakeDamage { void TakeDamage(...); }`

---

## J

**JSON**
Text format for storing data. Used for save files.
→ Example: `JsonUtility.ToJson(object)`

---

## K

**KeyCode**
Enum of all keyboard keys. Used with Input.GetKey().
→ Example: `Input.GetKey(KeyCode.Space)`

**Knockback**
Force pushing character backward when hit.
→ Implemented in Enemy.KnockBack() and Player_Archer.PushBack()

---

## L

**LateUpdate()**
MonoBehaviour method called after all Update() methods.
→ Use for: Camera following (after player moved)

**Layer**
Classification for GameObjects. Used for collision filtering and raycasts.
→ Example: "Player" layer, "Enemy" layer, "Ground" layer

**LayerMask**
Filter for raycasts specifying which layers to detect.
→ Example: `GameManager.Instance.layerEnemy`

**Lerp (Linear Interpolation)**
Smoothly transition between two values.
→ Example: `Mathf.Lerp(current, target, t);` where t = 0-1

**Level**
Game stage or mission. Each level has different enemies and configuration.
→ Managed by LevelManager and LevelEnemyManager

**LevelEnemyManager**
Spawns enemy waves for current level.
→ Location: `Managers/LevelEnemyManager.cs`

**Lifecycle**
Sequence of method calls Unity makes on MonoBehaviour:
→ Awake → OnEnable → Start → Update/FixedUpdate/LateUpdate → OnDisable → OnDestroy

**Listener Pattern (Observer)**
Design pattern where objects subscribe to events.
→ Implementation: IListener interface + GameManager.listeners list

---

## M

**Method**
Function inside a class.
→ Example: `public void TakeDamage(int amount) { }`

**MonoBehaviour**
Base class for all Unity scripts. Provides lifecycle methods.
→ All game scripts inherit from this

**Multiplier**
Value that scales another value.
→ Example: `damage * critMultiplier`

---

## N

**Namespace**
Container for classes to avoid name conflicts.
→ This project: `namespace RGame { }`

**Normalize**
Scale vector to length of 1 while keeping direction.
→ Example: `direction.normalized`

**Null**
Absence of value. Check with `== null`.
→ Example: `if (target == null) return;`

**NullReferenceException**
Error when accessing variable that is null.
→ Common cause: Forgot to assign reference in Inspector

---

## O

**Object Pooling**
Reuse objects instead of Instantiate/Destroy for performance.
→ Implementation: FloatingTextManager

**OnCollisionEnter2D()**
Called when collision starts (solid colliders).
→ Example: `void OnCollisionEnter2D(Collision2D collision) { }`

**OnDestroy()**
Called when GameObject is destroyed.
→ Use for: Cleanup, unsubscribe from events

**OnDrawGizmos()**
Draw debugging visuals in Scene view.
→ Example: Draw detection range circle

**OnTriggerEnter2D()**
Called when trigger area overlapped (isTrigger = true).
→ Example: Coin collection, damage zones

**Override**
Replace parent class method with new implementation.
→ Example: `public override void Die() { base.Die(); ... }`

---

## P

**Parameter**
Input value for method.
→ Example: `void Move(float speed)` - speed is parameter

**Parent-Child**
Hierarchy relationship. Child's transform is relative to parent.
→ Example: Hand is child of body

**Physics2D**
Unity system for 2D physics (collisions, raycasts, forces).

**PlayerPrefs**
Simple key-value storage for saving data (coins, progress).
→ Example: `PlayerPrefs.SetInt("Coins", 100);`

**Player_Archer**
Main player class. Inherits from Enemy (unusual!).
→ Location: `Player/Player_Archer.cs`

**Polymorphism**
Different classes can be treated as same type via interface/base class.
→ Example: All enemies implement ICanTakeDamage

**Prefab**
Reusable template GameObject. Stored as asset, instantiated at runtime.
→ Example: Enemy prefab spawned multiple times

**Private**
Accessible only within same class. Hidden from Inspector.
→ Example: `private bool isDead;`

**Projectile**
Object fired through air (arrow, bullet, fireball).
→ Base class: `Controllers/Projectile.cs`

**Public**
Accessible from other classes. Visible in Inspector.
→ Example: `public float speed = 5f;`

---

## Q

**Quaternion**
Rotation representation. Avoids gimbal lock.
→ Example: `Quaternion.identity` (no rotation)
→ Don't manually create - use Quaternion.Euler(x, y, z)

---

## R

**Range Attribute**
Creates slider in Inspector for numeric values.
→ Example: `[Range(0, 100)] public int health;`

**Raycast**
Invisible ray checking for collisions.
→ Example: `Physics2D.Raycast(origin, direction, distance);`

**RaycastController**
Base class for raycast-based physics.
→ Location: `Controllers/RaycastController.cs`

**ReadOnly Attribute**
Makes field visible but uneditable in Inspector.
→ Custom attribute in this project

**RectTransform**
Transform for UI elements. Has anchors, pivot, sizeDelta.

**Reference**
Link to another object.
→ Example: `public GameObject target;` - drag object in Inspector

**Renderer**
Component that draws visuals (SpriteRenderer, MeshRenderer).

**Resources**
Special Unity folder - assets can be loaded at runtime.
→ Example: `Resources.Load<GameObject>("Prefabs/Enemy");`

**Return**
Exit method and optionally provide value.
→ Example: `return health > 0;`

**Rigidbody2D**
Unity component for 2D physics simulation.
→ This project uses custom Controller2D instead

**RGame**
Namespace containing all game code in this project.

---

## S

**Scene**
Container for game level or menu. File with .unity extension.
→ This project: SampleScene.unity

**ScriptableObject**
Data-only asset. No MonoBehaviour overhead.
→ Example: UpgradedCharacterParameter

**Serialization**
Convert object to storable format (JSON, binary).
→ Enables saving to disk

**SerializeField Attribute**
Make private field visible in Inspector.
→ Example: `[SerializeField] private int coins;`

**Singleton**
Design pattern ensuring only one instance exists.
→ Example: GameManager.Instance, SoundManager.Instance

**SmartEnemyGrounded**
Main enemy implementation for ground-based enemies.
→ Location: `AI/SmartEnemyGrounded.cs`

**SoundManager**
Singleton managing audio playback.
→ Location: `Managers/SoundManager.cs`

**Spawn**
Create new GameObject instance.
→ Usually via Instantiate()

**SpriteRenderer**
Component displaying 2D image (sprite).

**Start()**
MonoBehaviour method called before first frame.
→ Called after all Awake() methods
→ Use for: Initialization requiring other objects

**StartCoroutine()**
Begin running coroutine.
→ Example: `StartCoroutine(WaitAndAct());`

**State Machine**
System where object can be in one state at a time.
→ Example: Enemy states (IDLE, WALK, ATTACK, DEATH)

**Static**
Belongs to class, not instance. Shared by all objects.
→ Example: `public static GameManager Instance;`

**String**
Text data type.
→ Example: `string name = "Player";`

**Stun**
Temporary disabling of character (can't move/act).
→ Implemented in Enemy.Stun() and Player_Archer.Stun()

---

## T

**Tag**
Simple label for GameObjects. Used for quick identification.
→ Example: GameObject.FindGameObjectWithTag("Player")

**TakeDamage()**
Method for applying damage to character.
→ Interface: ICanTakeDamage
→ Implementation: Enemy.cs

**Target**
Object being aimed at or followed.
→ Example: Enemy targets player, player targets enemy

**this**
Reference to current object instance.
→ Example: `this.transform.position`

**Time.deltaTime**
Seconds since last frame. Essential for smooth movement.
→ Example: `position += velocity * Time.deltaTime;`

**Time.fixedDeltaTime**
Time between FixedUpdate calls. Constant value.

**Time.time**
Time since game started in seconds.

**Tooltip Attribute**
Adds hover tooltip in Inspector.
→ Example: `[Tooltip("Speed in units per second")] public float speed;`

**Tower Defense**
Game genre where player defends against waves of enemies.
→ This project is tower defense hybrid

**Trajectory**
Curved path of projectile.
→ Player calculates optimal trajectory for arrows

**Transform**
Component storing position, rotation, scale. Every GameObject has one.

**Trigger**
Collider that detects overlap without physical collision.
→ Set: isTrigger = true
→ Events: OnTriggerEnter2D, OnTriggerStay2D, OnTriggerExit2D

---

## U

**UI (User Interface)**
Menus, buttons, health bars - anything player sees on screen.
→ Unity UI uses Canvas system

**Unity**
Game engine this project uses. Made by Unity Technologies.

**Unity Ads**
Unity's advertising platform. Integrated in this project.
→ Location: `AdController/UnityAds.cs`

**Update()**
MonoBehaviour method called every frame.
→ Use for: Input, non-physics logic
→ ~60 calls per second (60 FPS)

**UpgradedCharacterParameter**
ScriptableObject storing character upgrade stats.
→ Location: `Player/UpgradedCharacterParameter.cs`

---

## V

**Variable**
Named storage for data.
→ Example: `int health = 100;`

**Vector2**
2D coordinate (x, y).
→ Example: `Vector2 position = new Vector2(5, 3);`

**Vector3**
3D coordinate (x, y, z).
→ Used in 2D games with z = 0

**Velocity**
Speed and direction of movement.
→ Example: `velocity = direction * speed;`

**Virtual**
Method that can be overridden by child classes.
→ Example: `public virtual void Die() { }`

**Void**
Return type indicating method returns nothing.
→ Example: `void Start() { }`

---

## W

**Wave**
Group of enemies spawned together.
→ Configured in LevelWave.cs

**WeaponEffect**
Data class defining weapon effects (poison, burn, freeze).
→ Location: `Helpers/WeaponEffect.cs`

**while**
Loop that continues while condition is true.
→ Example: `while (health > 0) { }`

---

## X

*No common Unity/project terms starting with X*

---

## Y

**yield**
Keyword for coroutines. Pauses execution.
→ `yield return null;` - Wait one frame
→ `yield return new WaitForSeconds(2);` - Wait 2 seconds

---

## Z

*No common Unity/project terms starting with Z*

---

## Special Characters & Syntax

**→** (Arrow)
Indicates see also or example.

**{ }** (Braces)
Code block delimiters.
→ Example: `if (condition) { code here }`

**[ ]** (Brackets)
Array or attribute syntax.
→ Example: `int[] numbers` or `[SerializeField]`

**( )** (Parentheses)
Method parameters or mathematical grouping.
→ Example: `Move(5)` or `(a + b) * c`

**;** (Semicolon)
Statement terminator in C#.
→ Example: `int x = 5;`

**//** (Double Slash)
Single-line comment.
→ Example: `// This is a comment`

**/* */** (Slash-Asterisk)
Multi-line comment.
→ Example: `/* Multiple lines of comments */`

**::** (Double Colon)
Namespace separator.
→ Example: `UnityEngine.GameObject`

**??** (Null-Coalescing)
Provide default if null.
→ Example: `value = nullableValue ?? default;`

**?.** (Null-Conditional)
Access member only if not null.
→ Example: `player?.TakeDamage(10);`

---

## Common Abbreviations

**AI** - Artificial Intelligence
**API** - Application Programming Interface
**CPU** - Central Processing Unit
**DOF** - Depth of Field
**DoT** - Damage over Time
**FPS** - Frames Per Second
**GC** - Garbage Collection
**GPU** - Graphics Processing Unit
**GUI** - Graphical User Interface
**HP** - Health Points / Hit Points
**IAP** - In-App Purchase
**IDE** - Integrated Development Environment
**JSON** - JavaScript Object Notation
**NPC** - Non-Player Character
**OOP** - Object-Oriented Programming
**SDK** - Software Development Kit
**SFX** - Sound Effects
**UI** - User Interface
**VFX** - Visual Effects
**XML** - Extensible Markup Language

---

## Quick Reference: Common Code Patterns

```csharp
// Get component
Rigidbody2D rb = GetComponent<Rigidbody2D>();

// Find GameObject
GameObject player = GameObject.FindGameObjectWithTag("Player");

// Instantiate prefab
Instantiate(prefab, position, Quaternion.identity);

// Destroy object
Destroy(gameObject);

// Wait in coroutine
yield return new WaitForSeconds(2f);

// Check null
if (target == null) return;

// Access singleton
GameManager.Instance.Victory();

// Trigger event
OnPlayerDeath?.Invoke();

// Loop through array
foreach (var item in items) { }

// Debug log
Debug.Log("Message: " + value);
```

---

**How to Use This Glossary:**

1. **Ctrl+F** to search for term
2. **Bookmark** this page for quick access
3. **Read definition + example** for understanding
4. **Follow arrows (→)** for related information

**Not finding a term?**
- Check documentation index
- Search project code
- Ask in Unity forums
- Check Unity manual

---

**Document Version:** 2.0
**Last Updated:** October 2025
**Terms:** 200+ Unity and project-specific definitions
