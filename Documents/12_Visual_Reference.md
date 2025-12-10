# Visual Reference Guide
## Diagrams, Layouts, and Visual Aids for "Lawn Defense: Monsters Out"

**Document Version**: 1.0
**Last Updated**: 2025-10-29
**Difficulty Level**: Beginner to Intermediate

---

## Table of Contents

1. [Unity Editor Layout](#unity-editor-layout)
2. [GameObject and Component Structure](#gameobject-and-component-structure)
3. [Game Loop and Execution Order](#game-loop-and-execution-order)
4. [Event System Flow Charts](#event-system-flow-charts)
5. [Physics and Collision System](#physics-and-collision-system)
6. [State Machine Diagrams](#state-machine-diagrams)
7. [Scene Hierarchy Examples](#scene-hierarchy-examples)
8. [Inspector Field Reference](#inspector-field-reference)
9. [Animation System](#animation-system)
10. [UI System Layout](#ui-system-layout)
11. [File and Folder Structure](#file-and-folder-structure)

---

## Unity Editor Layout

### Default Unity Editor Overview

```
┌────────────────────────────────────────────────────────────────────────────────┐
│ File  Edit  Assets  GameObject  Component  Window  Help          [▶ Play ⏸ ⏹] │
├─────────────────────┬──────────────────────────────────┬───────────────────────┤
│                     │                                  │                       │
│   HIERARCHY         │        SCENE VIEW                │    INSPECTOR          │
│                     │                                  │                       │
│ ☰ Scene            │  ┌────────────────────┐          │ ┌──────────────────┐  │
│   ├─ Main Camera    │  │                    │          │ │ GameObject       │  │
│   ├─ GameManager    │  │                    │          │ │                  │  │
│   ├─ Player         │  │    [Player]        │          │ │ Tag: Player      │  │
│   ├─ Enemy (3)      │  │                    │          │ │ Layer: Player    │  │
│   └─ Canvas         │  │                    │          │ │                  │  │
│                     │  └────────────────────┘          │ │ Transform        │  │
│                     │  [2D] [3D] [Shaded]              │ │ Position: 0,0,0  │  │
│                     │                                  │ │                  │  │
│  + Create [▼]       │                                  │ │ Add Component    │  │
│                     │                                  │ │                  │  │
├─────────────────────┴──────────────────────────────────┤                       │
│                                                         │                       │
│              PROJECT                                    │                       │
│                                                         │                       │
│  Assets ▶                                              │                       │
│    ├─ _MonstersOut ▼                                   │                       │
│    │   ├─ Scenes                                       │                       │
│    │   ├─ Scripts                                      │                       │
│    │   ├─ Prefabs                                      │                       │
│    │   └─ Sprites                                      │                       │
│    └─ Resources                                        │                       │
│                                                         │                       │
├─────────────────────────────────────────────────────────┴───────────────────────┤
│                                                                                  │
│  CONSOLE                                                         [Clear] [▼]    │
│                                                                                  │
│  ○ This is a log message                                                        │
│  ⚠ This is a warning                                                            │
│  ⊗ This is an error                                                             │
│                                                                                  │
└──────────────────────────────────────────────────────────────────────────────────┘
```

### Key Areas Explained

**HIERARCHY (Left Panel)**
- Shows all GameObjects in current scene
- Organize GameObjects in parent-child relationships
- Click to select, drag to rearrange

**SCENE VIEW (Center)**
- Visual representation of your game world
- Use mouse to navigate (Alt+drag to rotate camera)
- Gizmos show invisible elements (colliders, lights, etc.)

**GAME VIEW (Tab in Scene View)**
- Shows game as player sees it
- Press Play to test
- Click "Maximize on Play" for fullscreen testing

**INSPECTOR (Right Panel)**
- Shows properties of selected GameObject
- Modify component values here
- Drag assets from Project to assign references

**PROJECT (Bottom Left)**
- File browser for all assets
- Drag files here to import
- Organize in folders

**CONSOLE (Bottom)**
- Shows Debug.Log, warnings, errors
- Double-click error to jump to code
- Clear button removes old messages

---

### Recommended Layout for 2D Games

```
Window → Layouts → 2D

┌────────────────────────────────────────────────────────────────────┐
│                 [This layout optimizes for 2D development]         │
├────────────┬───────────────────────────────┬───────────────────────┤
│ HIERARCHY  │                               │                       │
│            │      SCENE (2D MODE)          │    INSPECTOR          │
│            │                               │                       │
│            │  ┌─────────────────────┐      │                       │
│            │  │                     │      │                       │
│            │  │  (Flat 2D view)     │      │                       │
│            │  │                     │      │                       │
│            │  └─────────────────────┘      │                       │
│            │                               │                       │
├────────────┴───────────────────────────────┴───────────────────────┤
│                                                                     │
│  PROJECT                                    CONSOLE                │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

---

## GameObject and Component Structure

### GameObject Anatomy

```
┌─────────────────────────────────────────────────────┐
│ GameObject: "Player"                                │ ← The container
│                                                     │
│  ┌────────────────────────────────────────────┐    │
│  │ Component 1: Transform                     │    │ ← Position, rotation, scale
│  │  • Position: (5, 2, 0)                     │    │
│  │  • Rotation: (0, 0, 0)                     │    │
│  │  • Scale: (1, 1, 1)                        │    │
│  └────────────────────────────────────────────┘    │
│                                                     │
│  ┌────────────────────────────────────────────┐    │
│  │ Component 2: Sprite Renderer               │    │ ← Visual appearance
│  │  • Sprite: player_idle_00                  │    │
│  │  • Color: White                            │    │
│  │  • Flip X: false                           │    │
│  └────────────────────────────────────────────┘    │
│                                                     │
│  ┌────────────────────────────────────────────┐    │
│  │ Component 3: Animator                      │    │ ← Animation control
│  │  • Controller: Player_Controller           │    │
│  └────────────────────────────────────────────┘    │
│                                                     │
│  ┌────────────────────────────────────────────┐    │
│  │ Component 4: Box Collider 2D               │    │ ← Collision detection
│  │  • Size: (0.8, 1.8)                        │    │
│  │  • Is Trigger: false                       │    │
│  └────────────────────────────────────────────┘    │
│                                                     │
│  ┌────────────────────────────────────────────┐    │
│  │ Component 5: Controller2D (Script)         │    │ ← Custom movement
│  │  • Collision Mask: Ground                  │    │
│  │  • Skin Width: 0.015                       │    │
│  └────────────────────────────────────────────┘    │
│                                                     │
│  ┌────────────────────────────────────────────┐    │
│  │ Component 6: Player_Archer (Script)        │    │ ← Game logic
│  │  • Speed: 5                                │    │
│  │  • Max Health: 100                         │    │
│  │  • Arrow Prefab: Arrow                     │    │
│  └────────────────────────────────────────────┘    │
│                                                     │
└─────────────────────────────────────────────────────┘

Key Concept: GameObject = Container, Components = Functionality
```

### Component Stack - Visual Order

```
                 ┌─────────────────┐
                 │   GameObject    │
                 └────────┬────────┘
                          │
        ┌─────────────────┼─────────────────┐
        │                 │                 │
        ▼                 ▼                 ▼
  ┌──────────┐      ┌──────────┐     ┌──────────┐
  │Transform │      │ Rendering│     │ Physics  │
  │ (Always) │      │Components│     │Components│
  └──────────┘      └────┬─────┘     └────┬─────┘
                         │                 │
                    ┌────┴─────┐      ┌────┴─────┐
                    ▼          ▼      ▼          ▼
              ┌─────────┐ ┌────────┐ ┌────────┐ ┌────────┐
              │Sprite   │ │Animator│ │Collider│ │Scripts │
              │Renderer │ │        │ │2D      │ │(Custom)│
              └─────────┘ └────────┘ └────────┘ └────────┘

Reading order: Top to bottom in Inspector = Execution order
```

### Parent-Child Hierarchy

```
Player (Parent)
│
├─── PlayerSprite (Child)
│    └─── Component: Sprite Renderer
│    └─── Component: Animator
│
├─── WeaponSpawnPoint (Child)
│    └─── Transform Position: (0.5, 0.3, 0)
│
├─── HealthBarCanvas (Child)
     └─── HealthBarSlider (Grandchild)
          └─── Component: Slider

Visual representation:

     [Player GameObject]
           │
     ┌─────┼─────┐
     │     │     │
   [Sprite][Weapon][Health]
           [Point] [Bar]

Children inherit parent's Transform:
- If Player moves → All children move with it
- If Player rotates → All children rotate
- If Player is destroyed → All children destroyed
```

---

## Game Loop and Execution Order

### Unity Method Execution Timeline

```
                    GAME STARTS
                         │
                         ▼
        ┌────────────────────────────────┐
        │      Awake()                   │ ◄── Called ONCE when object created
        │   • Initialize references      │     (Before Start, even if disabled)
        │   • Setup singletons          │
        └────────────┬───────────────────┘
                     │
                     ▼
        ┌────────────────────────────────┐
        │      OnEnable()                │ ◄── Called when object enabled
        │   • Register listeners         │     (Every time SetActive(true))
        └────────────┬───────────────────┘
                     │
                     ▼
        ┌────────────────────────────────┐
        │      Start()                   │ ◄── Called ONCE before first Update
        │   • Find other objects         │     (Only if object enabled)
        │   • Start coroutines          │
        └────────────┬───────────────────┘
                     │
                     ▼
        ┌────────────────────────────────┐
        │    ╔═══════════════════╗       │
        │    ║  GAME LOOP        ║       │ ◄── Repeats every frame
        │    ╚═══════════════════╝       │
        │                                 │
        │    ┌─────────────────────┐     │
        │    │ FixedUpdate()       │     │ ◄── Called at fixed intervals
        │    │  • Physics code     │     │     (Default: 50 times/second)
        │    │  • Rigidbody forces │     │
        │    └──────────┬──────────┘     │
        │               │                 │
        │               ▼                 │
        │    ┌─────────────────────┐     │
        │    │ Update()            │     │ ◄── Called every frame
        │    │  • Input checking   │     │     (60 FPS = 60 times/second)
        │    │  • Movement         │     │
        │    │  • Game logic       │     │
        │    └──────────┬──────────┘     │
        │               │                 │
        │               ▼                 │
        │    ┌─────────────────────┐     │
        │    │ LateUpdate()        │     │ ◄── After all Updates
        │    │  • Camera following │     │     (Use for camera, UI)
        │    │  • Final position   │     │
        │    └──────────┬──────────┘     │
        │               │                 │
        │               ▼                 │
        │    ┌─────────────────────┐     │
        │    │ OnGUI()             │     │ ◄── Multiple times per frame
        │    │  • Debug UI         │     │     (Don't use for game UI)
        │    └──────────┬──────────┘     │
        │               │                 │
        │               └─────────────────┤ ← Loop back to FixedUpdate
        │                                 │
        └────────────┬────────────────────┘
                     │
                     │ (When object disabled/destroyed)
                     ▼
        ┌────────────────────────────────┐
        │      OnDisable()               │ ◄── Called when disabled
        │   • Unregister listeners       │
        └────────────┬───────────────────┘
                     │
                     ▼
        ┌────────────────────────────────┐
        │      OnDestroy()               │ ◄── Called when destroyed
        │   • Clean up resources         │
        └────────────────────────────────┘
                     │
                     ▼
                 GAME ENDS
```

### Frame Timeline Example (60 FPS)

```
Time: 0.000s ────────────────────────────────────────────────────────>
              Frame 1          Frame 2          Frame 3

              ▼                ▼                ▼
┌─────────────┬────────────────┬────────────────┬───────>
│FixedUpdate  │FixedUpdate     │FixedUpdate     │
│ Update      │ Update         │ Update         │
│ LateUpdate  │ LateUpdate     │ LateUpdate     │
└─────────────┴────────────────┴────────────────┴───────>
   16.67ms        16.67ms         16.67ms

Legend:
• 60 FPS = 1 frame every 16.67 milliseconds
• FixedUpdate: Physics (50 times/second = every 20ms)
• Update: Game logic (every frame)
• LateUpdate: Camera, final adjustments (every frame)
```

### Time.deltaTime Visualization

```
Without deltaTime:                    With deltaTime:
───────────────────                   ───────────────────

Fast PC (120 FPS):                    Fast PC (120 FPS):
 Move 5 units → → → → → → → →         Move 5 * 0.0083 = 0.041 →
 Per second: 600 units! TOO FAST      Per second: 5 units ✓

Slow PC (30 FPS):                     Slow PC (30 FPS):
 Move 5 units →                        Move 5 * 0.033 = 0.165 →
 Per second: 150 units! TOO SLOW      Per second: 5 units ✓

Code:
transform.position += Vector3.right * speed * Time.deltaTime;
                                              ^^^^^^^^^^^^^
                                              Framerate independent!
```

---

## Event System Flow Charts

### Observer Pattern (IListener) Flow

```
                    GAME EVENT OCCURS
                           │
                           ▼
        ┌──────────────────────────────────┐
        │     GameManager.Victory()        │
        │   • State = GameState.Success    │
        └────────────┬─────────────────────┘
                     │
                     ▼
        ┌──────────────────────────────────┐
        │  Loop through listeners list:    │
        │  foreach (var listener in        │
        │           listeners)             │
        └────────────┬─────────────────────┘
                     │
        ┌────────────┴─────────────┐
        │                          │
        ▼                          ▼
┌──────────────┐          ┌──────────────┐
│ Listener 1   │          │ Listener 2   │
│ MenuManager  │          │ UI_UI        │
│              │          │              │
│ ISuccess()   │          │ ISuccess()   │
│  │           │          │  │           │
│  └─Shows     │          │  └─Updates   │
│    Victory   │          │    final     │
│    Screen    │          │    score     │
└──────────────┘          └──────────────┘

Registration Process:
──────────────────────

Start():
  GameManager.Instance.listeners.Add(this)

Now this object receives all events:
  ✓ ISuccess() - When level won
  ✓ IFail() - When level lost
  ✓ IPlayer() - When player damaged
  ✓ IPause() - When game paused

OnDestroy():
  GameManager.Instance.listeners.Remove(this)
  (Important: Prevent memory leaks!)
```

### Button Click Event Flow

```
                    USER CLICKS BUTTON
                           │
                           ▼
        ┌──────────────────────────────────┐
        │     EventSystem detects click    │
        │   • Raycast from mouse position  │
        │   • Hit Button GameObject?       │
        └────────────┬─────────────────────┘
                     │
                YES  │  NO
        ┌────────────┴─────────────┐
        │                          │
        ▼                          ▼
┌──────────────┐          ┌──────────────┐
│ Button       │          │ Ignore       │
│ Component    │          │              │
│              │          └──────────────┘
│ OnClick()    │
│ Event        │
└──────┬───────┘
       │
       ▼
┌──────────────────────────────────┐
│ Assigned Function Runs:          │
│                                  │
│ MenuManager.PlayGame()           │
│   │                              │
│   ├─ Stop current game           │
│   ├─ Load new scene              │
│   └─ Start fresh level           │
└──────────────────────────────────┘

Inspector Setup:
─────────────────

[Button Component]
┌─────────────────────────┐
│ OnClick()               │
│  ┌───────────────────┐  │
│  │ Runtime           │  │ ◄─ When to call (Runtime = during game)
│  │ MenuManager       │  │ ◄─ What object
│  │ PlayGame()        │  │ ◄─ What function
│  └───────────────────┘  │
│  [+] Add Event         │
└─────────────────────────┘
```

### Scene Loading Flow

```
        ┌─────────────────────────────┐
        │ Call: LoadScene("Level1")   │
        └──────────────┬──────────────┘
                       │
                       ▼
        ┌─────────────────────────────┐
        │ Current scene UNLOADS:      │
        │  1. OnDisable() on all      │
        │  2. OnDestroy() on all      │
        │  3. Clear memory            │
        └──────────────┬──────────────┘
                       │
                       ▼
        ┌─────────────────────────────┐
        │ New scene LOADS:            │
        │  1. Load scene file         │
        │  2. Instantiate GameObjects │
        │  3. Awake() on all          │
        │  4. OnEnable() on all       │
        │  5. Start() on all          │
        └──────────────┬──────────────┘
                       │
                       ▼
        ┌─────────────────────────────┐
        │ Game runs normally          │
        │ (Update loop begins)        │
        └─────────────────────────────┘

Important Notes:
────────────────
✗ All non-DontDestroyOnLoad objects are destroyed
✓ Static variables persist (be careful!)
✓ PlayerPrefs persist (use for save data)

Example with async:
──────────────────

AsyncOperation async = SceneManager.LoadSceneAsync("Level1");

while (!async.isDone)
{
    float progress = async.progress;
    // Update loading bar: 0% → 100%
    yield return null;
}
```

---

## Physics and Collision System

### 2D Physics Layers

```
Layer Collision Matrix (Edit → Project Settings → Physics 2D)
────────────────────────────────────────────────────────────

         Default  Ground  Player  Enemy  Projectile  Pickup
Default    ✓       ✓       ✓       ✓        ✓         ✓
Ground     ✓       ✗       ✓       ✓        ✓         ✗
Player     ✓       ✓       ✗       ✓        ✗         ✓
Enemy      ✓       ✓       ✓       ✗        ✓         ✗
Projectile ✓       ✓       ✗       ✓        ✗         ✗
Pickup     ✓       ✗       ✓       ✗        ✗         ✗

Legend:
✓ = Can collide
✗ = Cannot collide

Example: Player ✗ Player means players can't collide with each other
         Player ✓ Enemy means players can collide with enemies
```

### Collision vs Trigger

```
COLLISION (Is Trigger: ☐ Unchecked)
───────────────────────────────────

    Object A          Object B
    [██████]          [██████]
        │                 │
        └────Collide──────┘
             BOUNCE!

• Objects physically block each other
• Stops movement
• Generates collision forces
• Use for: Walls, platforms, solid objects

void OnCollisionEnter2D(Collision2D collision)
{
    // Called when objects touch
    // collision.gameObject = what we hit
    // collision.contacts = where we hit
    // collision.relativeVelocity = how hard we hit
}


TRIGGER (Is Trigger: ☑ Checked)
────────────────────────────────

    Object A          Object B
    [██████]          [██████]
        │                 │
        └─Pass Through────┘
          (Detected!)

• Objects pass through each other
• No physical blocking
• Still detected by code
• Use for: Pickups, damage zones, detection areas

void OnTriggerEnter2D(Collider2D other)
{
    // Called when objects overlap
    // other.gameObject = what we touched
    // No collision data (no force, no bounce)
}
```

### Raycast Visualization

```
SINGLE RAYCAST
──────────────

Start Point                              Hit Point
    ●──────────────────────────────────────●
    │                                      │
    │         Ray Direction →              │
    │         Length: 10 units             │
    │                                      ▼
    │                                   [Enemy]
    └─ Returns: RaycastHit2D

Code:
RaycastHit2D hit = Physics2D.Raycast(
    start,      // Starting position
    direction,  // Direction (normalized)
    distance,   // How far to check
    layerMask   // What layers to hit
);

if (hit.collider != null)
{
    // We hit something!
    Debug.Log("Hit: " + hit.collider.name);
    Debug.Log("Distance: " + hit.distance);
    Debug.Log("Point: " + hit.point);
}


CIRCLECAST (Better for character detection)
────────────────────────────────────────────

                    Radius
Start Point        ↙    ↘
    ◯─────────────○──────○──────○
    │             ↓      ↓      ↓
    │          Sweep circle along path
    │
    └─ Returns: RaycastHit2D (first hit)

Code:
RaycastHit2D hit = Physics2D.CircleCast(
    transform.position,  // Start
    radius,              // Circle size (0.5 = player size)
    Vector2.right,       // Direction
    distance,            // How far
    enemyLayer          // What to hit
);


CONTROLLER2D RAYCASTING (This game's approach)
──────────────────────────────────────────────

Player Box Collider with multiple raycasts:

    Vertical Rays:
    │   │   │   │   │
    ▼   ▼   ▼   ▼   ▼
  ┌─────────────────┐
  │                 │ ← Horizontal Rays
  │     PLAYER      │ →
  │                 │ →
  └─────────────────┘
    ▲   ▲   ▲   ▲   ▲
    │   │   │   │   │

Horizontal Ray Count: 6 (more = smoother detection)
Vertical Ray Count: 6

Why multiple rays?
• Single ray can miss slopes
• Multiple rays detect more accurately
• Prevents falling through thin platforms
```

### Overlap Detection

```
OVERLAP CIRCLE ALL
──────────────────

        Detection Radius
            ╱───────╲
           ╱         ╲
    Player●           ●Enemy 1
           ╲         ╱
      Enemy 2●      ●Enemy 3
             ╲─────╱

Code:
Collider2D[] enemies = Physics2D.OverlapCircleAll(
    transform.position,  // Center point
    attackRange,         // Radius
    enemyLayer          // Layer mask
);

// Returns ALL enemies in circle
foreach (var enemy in enemies)
{
    Debug.Log("Found: " + enemy.name);
}

Visualize in Scene:
void OnDrawGizmosSelected()
{
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(transform.position, attackRange);
}
```

---

## State Machine Diagrams

### Enemy State Machine

```
                         ┌──────────────┐
                    ┌───→│   SPAWNING   │
                    │    └──────┬───────┘
                    │           │
                    │      Spawn animation
                    │      complete (1s)
                    │           │
                    │           ▼
                    │    ┌──────────────┐
            Freeze  │    │     IDLE     │
            expires │    └──────┬───────┘
                    │           │
                    │      No target/
                    │      Waiting
                    │           │
                    │           ▼
                    │    ┌──────────────┐      Player in
                    └────│     WALK     │──→  attack range
                         └──────┬───────┘           │
                              ▲ │                   │
                    Player    │ │                   │
                    out of    │ │                   ▼
                    range     │ │           ┌──────────────┐
                              │ └───────────│    ATTACK    │
                              │    Attack   └──────┬───────┘
                              │    complete        │
                              │                    │
                              │                    │
                              │        Take damage │
                              │                    │
                              │                    ▼
                              │            ┌──────────────┐
                              │            │     HIT      │
                              │            └──────┬───────┘
                              │                   │
                              │        Hit animation
                              │        complete
                              │                   │
                              └───────────────────┘
                                                  │
                                       Health ≤ 0?│
                                                  │
                                                  ▼
                                          ┌──────────────┐
                                          │    DEATH     │
                                          └──────┬───────┘
                                                 │
                                        Death animation
                                        complete
                                                 │
                                                 ▼
                                            DESTROYED

Code Implementation:
────────────────────

public enum ENEMYSTATE
{
    SPAWNING, IDLE, WALK, ATTACK, HIT, DEATH
}

public ENEMYSTATE State;

void Update()
{
    switch (State)
    {
        case ENEMYSTATE.SPAWNING:
            // Play spawn animation
            // After delay → WALK
            break;

        case ENEMYSTATE.IDLE:
            // Stand still
            // Look for target
            break;

        case ENEMYSTATE.WALK:
            // Move toward player
            // If in range → ATTACK
            break;

        case ENEMYSTATE.ATTACK:
            // Play attack animation
            // Deal damage
            // After attack → WALK
            break;

        case ENEMYSTATE.HIT:
            // Play hurt animation
            // Can't move
            // After delay → WALK
            break;

        case ENEMYSTATE.DEATH:
            // Play death animation
            // Disable components
            // Destroy after delay
            break;
    }
}
```

### Game State Machine

```
                         ┌──────────────┐
                         │  MAIN MENU   │
                         └──────┬───────┘
                                │
                        Click "Play"
                                │
                                ▼
                         ┌──────────────┐
                    ┌───→│   PLAYING    │←────┐
                    │    └──────┬───────┘     │
                    │           │             │
         Press ESC  │    All enemies          │ Resume
                    │    defeated    Fortress │
                    │           │     destroyed
                    │           │         │   │
                    │           ▼         ▼   │
                    │    ┌──────────┐  ┌──────────┐
                    └────│  PAUSED  │  │  GAME    │
                         └──────────┘  │  OVER    │
                                       └──────┬───┘
                                              │
                                              ▼
                                       ┌──────────────┐
                         ┌─────────────│    MENU      │
                         │  Restart/   └──────────────┘
                         │  Next Level
                         │
                         ▼
                  ┌──────────────┐
                  │   PLAYING    │
                  │ (New Level)  │
                  └──────────────┘

Code:
─────

public enum GameState
{
    Prepare,  // Loading
    Playing,  // Active gameplay
    Pause,    // Paused
    Success,  // Victory
    Fail      // Game Over
}

public GameState State;

void Update()
{
    if (State == GameState.Playing)
    {
        // Game logic runs
        Time.timeScale = 1; // Normal speed
    }
    else if (State == GameState.Pause)
    {
        // Game frozen
        Time.timeScale = 0; // Freeze
    }
}
```

---

## Scene Hierarchy Examples

### Main Game Scene Structure

```
📁 Level1 (Scene)
│
├─ 🎮 === MANAGERS ===
│  ├─ 📦 GameManager
│  │  └─ Script: GameManager.cs
│  ├─ 📦 LevelEnemyManager
│  │  ├─ Script: LevelEnemyManager.cs
│  │  └─ Waves: [Wave1, Wave2, Wave3]
│  ├─ 📦 MenuManager
│  │  └─ Script: MenuManager.cs
│  └─ 📦 SoundManager
│     ├─ Script: SoundManager.cs
│     ├─ AudioSource (Music)
│     └─ AudioSource (SFX)
│
├─ 🎮 === PLAYER ===
│  └─ 📦 Player
│     ├─ 🔸 Transform (Position: -8, 0, 0)
│     ├─ 🖼️ Sprite Renderer
│     ├─ 🎬 Animator
│     ├─ ⬜ Box Collider 2D
│     ├─ 📜 Controller2D
│     ├─ 📜 Player_Archer
│     │
│     ├─ 📦 WeaponPoint (Child)
│     │  └─ 🔸 Transform (Position: 0.5, 0.3, 0)
│     │
│     └─ 📦 HealthBar (Child)
│        └─ Canvas → Slider
│
├─ 🎮 === ENVIRONMENT ===
│  ├─ 📦 Ground
│  │  ├─ 🖼️ Sprite Renderer
│  │  └─ ⬜ Box Collider 2D
│  │
│  ├─ 📦 PlayerFortress
│  │  ├─ 🖼️ Sprite Renderer
│  │  ├─ ⬜ Box Collider 2D
│  │  ├─ 📜 TheFortrest (healthCharacter: PLAYER)
│  │  └─ Canvas → HealthBar
│  │
│  └─ 📦 EnemySpawnPoint
│     └─ 🔸 Transform (Position: 15, 0, 0)
│
├─ 🎮 === ENEMIES === (Spawned at runtime)
│  ├─ 📦 Goblin(Clone)
│  ├─ 📦 Skeleton(Clone)
│  └─ 📦 TrollWarrior(Clone)
│
├─ 🎮 === UI ===
│  └─ 📦 Canvas
│     ├─ RenderMode: Screen Space - Overlay
│     ├─ Canvas Scaler
│     │
│     ├─ 📦 HUD
│     │  ├─ 📦 HealthDisplay
│     │  ├─ 📦 CoinDisplay
│     │  └─ 📦 WaveDisplay
│     │
│     └─ 📦 Buttons
│        ├─ 📦 PauseButton
│        └─ 📦 SettingsButton
│
└─ 🎮 === CAMERA ===
   └─ 📦 Main Camera
      ├─ 📷 Camera (Orthographic)
      ├─ 🔊 Audio Listener
      └─ 📜 CameraFollow (optional)

Legend:
📦 = GameObject
🔸 = Transform component
🖼️ = Sprite Renderer
🎬 = Animator
⬜ = Collider
📜 = Script
🔊 = Audio
📷 = Camera
```

### Menu Scene Structure

```
📁 MainMenu (Scene)
│
├─ 📦 Canvas
│  ├─ RenderMode: Screen Space - Overlay
│  │
│  ├─ 📦 Background
│  │  └─ 🖼️ Image (title.png)
│  │
│  ├─ 📦 TitleText
│  │  └─ 📝 Text: "LAWN DEFENSE"
│  │
│  ├─ 📦 ButtonPanel
│  │  ├─ 🔲 Button_Play
│  │  │  ├─ OnClick → MenuManager.PlayGame()
│  │  │  └─ 📝 Text: "PLAY"
│  │  │
│  │  ├─ 🔲 Button_Settings
│  │  │  ├─ OnClick → MenuManager.OpenSettings()
│  │  │  └─ 📝 Text: "SETTINGS"
│  │  │
│  │  └─ 🔲 Button_Quit
│  │     ├─ OnClick → MenuManager.QuitGame()
│  │     └─ 📝 Text: "QUIT"
│  │
│  └─ 📦 SettingsPanel (Initially disabled)
│     ├─ 🔊 MusicSlider
│     ├─ 🔊 SFXSlider
│     └─ 🔲 CloseButton
│
├─ 📦 EventSystem
│  └─ Standalone Input Module
│
├─ 📦 MenuManager
│  └─ 📜 MainMenuHomeScene.cs
│
└─ 📦 Main Camera
   ├─ 📷 Camera
   └─ 🔊 Audio Listener
```

---

## Inspector Field Reference

### Transform Component

```
┌───────────────────────────────────────┐
│ Transform                             │
├───────────────────────────────────────┤
│                                       │
│ Position                              │
│  X: 5.0      Y: 2.5      Z: 0        │
│  └─Left/Right └─Up/Down  └─Forward    │
│                             (Usually 0 in 2D)
│                                       │
│ Rotation                              │
│  X: 0        Y: 0        Z: 45       │
│  └─Pitch     └─Yaw       └─Roll       │
│                           (Use this for 2D rotation)
│                                       │
│ Scale                                 │
│  X: 1        Y: 1        Z: 1        │
│  └─Width     └─Height    └─Depth      │
│    (2 = 200% size)                    │
│                                       │
└───────────────────────────────────────┘

Shortcuts:
• Reset: Right-click Transform → Reset
• Copy: Right-click → Copy Component
• Paste: Right-click → Paste Component Values
```

### Sprite Renderer Component

```
┌───────────────────────────────────────┐
│ Sprite Renderer                       │
├───────────────────────────────────────┤
│                                       │
│ Sprite                                │
│  ┌─────────────────────────────────┐  │
│  │ [player_idle_00]  ●             │  │ ← Drag sprite here
│  └─────────────────────────────────┘  │
│                                       │
│ Color                                 │
│  ██████ White                         │ ← Click to change tint
│                                       │
│ Flip                                  │
│  ☐ X  ☐ Y                            │ ← Mirror sprite
│                                       │
│ Sorting Layer: Default           ▼   │
│ Order in Layer: 0                    │ ← Higher = in front
│                                       │
│ Material: Sprites-Default             │
│                                       │
└───────────────────────────────────────┘

Sorting Order Example:
Background:   -10
Ground:         0
Player:         5  ← Player in front of ground
Enemy:          5
UI:           100  ← Always in front
```

### Animator Component

```
┌───────────────────────────────────────┐
│ Animator                              │
├───────────────────────────────────────┤
│                                       │
│ Controller                            │
│  ┌─────────────────────────────────┐  │
│  │ Player_Controller  ●            │  │ ← Animation Controller asset
│  └─────────────────────────────────┘  │
│                                       │
│ Avatar: None                          │ ← (3D only, ignore for 2D)
│                                       │
│ ☑ Apply Root Motion                   │ ← Unchecked for code-driven movement
│                                       │
│ Update Mode: Normal              ▼   │
│  • Normal: Respects Time.timeScale    │
│  • Unscaled Time: Ignores pause       │
│                                       │
│ Parameters:                           │
│  • isDead (Bool)                      │
│  • Speed (Float)                      │
│  • Attack (Trigger)                   │
│                                       │
└───────────────────────────────────────┘

Controlling from code:
animator.SetBool("isDead", true);
animator.SetFloat("Speed", 5.0f);
animator.SetTrigger("Attack");
```

### Box Collider 2D Component

```
┌───────────────────────────────────────┐
│ Box Collider 2D                       │
├───────────────────────────────────────┤
│                                       │
│ ☐ Is Trigger                          │ ← Check for non-solid
│                                       │
│ Used By Effector: ☐                   │
│                                       │
│ Offset                                │
│  X: 0        Y: 0                     │ ← Shift collider position
│                                       │
│ Size                                  │
│  X: 0.8      Y: 1.8                   │ ← Collider dimensions
│                                       │
│ [Edit Collider]                       │ ← Visual editing mode
│                                       │
│ Material: None (Physics Material 2D)  │
│                                       │
└───────────────────────────────────────┘

Visual in Scene View:
┌─────────────────┐
│                 │ ← Green outline = collider
│      ████       │    (when GameObject selected)
│      ████       │
│      ████       │ ← Gray sprite
│                 │
└─────────────────┘

Tip: Make collider slightly smaller than sprite
     to prevent edge catching on walls
```

### Custom Script Component

```
┌───────────────────────────────────────┐
│ Player_Archer (Script)                │
├───────────────────────────────────────┤
│                                       │
│ Script: Player_Archer ⚙               │ ← Double-click to edit
│                                       │
│ [Header("Movement")]                  │
│ Speed: 5.0                            │
│ Allow Move By Player: ☑               │
│                                       │
│ [Header("Combat")]                    │
│ Max Health: 100                       │
│ Current Health: 100                   │
│                                       │
│ [Header("Weapon")]                    │
│ Arrow Prefab: None (GameObject)  ●   │ ← Drag prefab here
│ Spawn Point: None (Transform)    ●   │ ← Drag transform here
│ Attack Cooldown: 1.0                  │
│                                       │
│ [Header("Effects")]                   │
│ Hit Effect: None (GameObject)    ●   │
│                                       │
└───────────────────────────────────────┘

Inspector Attributes in Code:
─────────────────────────────

[Header("Movement")]        ← Creates header
public float speed = 5f;

[Range(0, 100)]            ← Creates slider
public float health;

[SerializeField]           ← Shows private variable
private int coins;

[HideInInspector]          ← Hides public variable
public bool debugMode;

[Tooltip("Speed of movement")]  ← Shows tooltip on hover
public float speed;
```

---

## Animation System

### Animator Controller Graph

```
┌────────────────────────────────────────────────────────────────┐
│ Animator: Player_Controller                                    │
├────────────────────────────────────────────────────────────────┤
│                                                                │
│  Parameters:                 States:                           │
│  ┌──────────────┐                                             │
│  │ Speed (Float)│           ┌─────────────┐                   │
│  │ isDead (Bool)│           │    IDLE     │                   │
│  │ Attack(Trig) │           │ (Animation: │                   │
│  └──────────────┘           │  idle_anim) │                   │
│                             └──────┬──────┘                   │
│                                    │                           │
│                      Speed > 0.1   │   Speed < 0.1            │
│                                    │                           │
│             ┌──────────────────────┴────────────────┐          │
│             │                                       │          │
│             ▼                                       ▼          │
│      ┌─────────────┐                        ┌─────────────┐   │
│      │    WALK     │◄───Attack Trigger──────│   ATTACK    │   │
│      │ (Animation: │                        │ (Animation: │   │
│      │  walk_anim) │──Attack Trigger───────→│ attack_anim)│   │
│      └──────┬──────┘                        └─────────────┘   │
│             │                                                  │
│        isDead = true                                          │
│             │                                                  │
│             ▼                                                  │
│      ┌─────────────┐                                          │
│      │    DEATH    │                                          │
│      │ (Animation: │                                          │
│      │  death_anim)│                                          │
│      └─────────────┘                                          │
│                                                                │
└────────────────────────────────────────────────────────────────┘

Transitions:
────────────

Idle → Walk:
  Condition: Speed > 0.1
  Duration: 0.2s

Walk → Idle:
  Condition: Speed < 0.1
  Duration: 0.2s

Any State → Attack:
  Condition: Attack trigger
  Duration: 0s (instant)

Attack → Previous State:
  Has Exit Time: ✓
  Exit Time: 0.8 (80% through animation)
```

### Animation Clip Timeline

```
Attack Animation Clip (Duration: 1 second, 30 FPS)
────────────────────────────────────────────────────

Frame:  0      5      10     15     20     25     30
        │      │      │      │      │      │      │
        ▼      ▼      ▼      ▼      ▼      ▼      ▼
Time:  0.0   0.17   0.33   0.50   0.67   0.83   1.0s

Sprite: [────][────][────][────][────][────][────]
        idle  windup swing  hit  follow follow idle
                                through through

Events:      ▲                   ▲
         Frame 10            Frame 30
         "DealDamage()"     "AttackComplete()"

Code Callbacks:
───────────────

public void DealDamage()
{
    // Called at frame 10 (when swing connects)
    Debug.Log("Damage dealt!");
}

public void AttackComplete()
{
    // Called at frame 30 (end of animation)
    Debug.Log("Attack finished!");
    canAttack = true;
}

Adding Animation Events:
────────────────────────
1. Select animation clip in Project
2. Window → Animation → Animation
3. Click white bar at bottom to add event marker
4. Select method to call
```

### Animation Layer Example

```
┌────────────────────────────────────────────────────┐
│ Layers                                             │
├────────────────────────────────────────────────────┤
│                                                    │
│ ▶ Base Layer                Weight: 1.0           │ ← Default layer
│   └─ Contains: Idle, Walk, Attack, Death          │
│                                                    │
│ ▶ Upper Body Layer         Weight: 0.5           │ ← Blended layer
│   └─ Contains: Wave, Reload, ThrowGrenade         │   (e.g., shoot while walking)
│   ☑ Blending: Override                            │
│                                                    │
└────────────────────────────────────────────────────┘

Result: Both layers play simultaneously
        Base Layer: Legs walking
        Upper Layer: Arms shooting
```

---

## UI System Layout

### Canvas Render Modes

```
1. SCREEN SPACE - OVERLAY (Most common for game UI)
──────────────────────────────────────────────────────

    ┌──────────────────────────────────┐
    │ Screen                           │ ◄─ Always in front
    │                                  │
    │  Game Scene (behind UI)          │
    │  ┌─────────────┐                 │
    │  │   Player    │                 │
    │  └─────────────┘                 │
    │                                  │
    │  ┌──────────────────────┐        │ ◄─ UI Layer
    │  │ Health: ████░░░░ 60% │        │    (always visible)
    │  └──────────────────────┘        │
    │                                  │
    └──────────────────────────────────┘

Settings:
• Render Mode: Screen Space - Overlay
• Pixel Perfect: ☑ (crisp UI)
• Sort Order: 0 (higher = in front)


2. SCREEN SPACE - CAMERA (For UI effects)
──────────────────────────────────────────

    Game Camera renders UI on top
    Allows post-processing on UI

Settings:
• Render Mode: Screen Space - Camera
• Render Camera: Main Camera
• Plane Distance: 1 (how far from camera)


3. WORLD SPACE (For in-game UI like health bars)
─────────────────────────────────────────────────

    ┌──────────────────┐
    │ Enemy            │
    │ Health: ████░░   │ ◄─ Moves with enemy
    │                  │
    │      ████        │
    │      ████        │
    │                  │
    └──────────────────┘

Settings:
• Render Mode: World Space
• Position: Above enemy (0, 2, 0)
• Scale: (0.01, 0.01, 0.01) ← Small!
```

### UI Hierarchy Structure

```
Canvas
│
├─── HealthBar (Top-left)
│    ├─ Background (Image)
│    ├─ Fill (Slider)
│    │  ├─ Background
│    │  ├─ Fill Area
│    │  │  └─ Fill (Image) ← This changes width
│    │  └─ Handle Slide Area (optional)
│    └─ Text (shows "60/100")
│
├─── CoinDisplay (Top-right)
│    ├─ Icon (Image)
│    └─ Text (shows "250")
│
├─── PauseButton (Top-right)
│    ├─ Button component
│    ├─ Image (button background)
│    └─ Text (shows "||")
│
└─── PauseMenu (Initially disabled)
     ├─ Background (dark overlay)
     ├─ Panel (menu box)
     │   ├─ ResumeButton
     │   ├─ RestartButton
     │   └─ QuitButton
     └─ Title Text

Anchors Example:
────────────────

Top-Left Anchor:          Top-Right Anchor:
┌───┐                            ┌───┐
│ ● │HealthBar              Coins│ ● │
└───┴──────────────────────────┴─┴───┘
  ▲                              ▲
  Stays here                     Stays here
  on all screen sizes            on all screen sizes
```

### Slider Component Breakdown

```
┌───────────────────────────────────────────────┐
│ Slider Component                              │
├───────────────────────────────────────────────┤
│                                               │
│ Fill Rect: Fill (Image)  ●                   │ ← What changes size
│ Handle Rect: None                             │ ← Draggable handle (optional)
│                                               │
│ Direction: Left To Right              ▼      │
│                                               │
│ Min Value: 0                                  │ ← Empty
│ Max Value: 100                                │ ← Full
│ Whole Numbers: ☐                              │ ← Decimal values
│                                               │
│ Value: 60                                     │ ← Current value
│                                               │
│ OnValueChanged()                              │ ← Event when value changes
│  Runtime: HealthBar.UpdateDisplay()           │
│                                               │
└───────────────────────────────────────────────┘

Visual Breakdown:

Full Health (value = 100):
┌─────────────────────────────────┐
│████████████████████████████████ │ ← Fill Image (100% width)
└─────────────────────────────────┘

Half Health (value = 50):
┌─────────────────────────────────┐
│████████████████░░░░░░░░░░░░░░░░│ ← Fill Image (50% width)
└─────────────────────────────────┘

Low Health (value = 20):
┌─────────────────────────────────┐
│██████░░░░░░░░░░░░░░░░░░░░░░░░░░│ ← Fill Image (20% width)
└─────────────────────────────────┘
```

### Button Component

```
┌───────────────────────────────────────────────┐
│ Button                                        │
├───────────────────────────────────────────────┤
│                                               │
│ ☑ Interactable                                │ ← Can be clicked
│                                               │
│ Transition: Color Tint               ▼       │
│  Normal Color:     White                      │
│  Highlighted:      Light Gray  ← On hover     │
│  Pressed:          Dark Gray   ← While click  │
│  Disabled:         Gray        ← Not usable   │
│  Fade Duration: 0.1                           │
│                                               │
│ Navigation: Automatic              ▼          │ ← Keyboard/gamepad
│                                               │
│ OnClick()                                     │
│  ┌─────────────────────────────────────────┐ │
│  │ Runtime                                  │ │
│  │ MenuManager.PlayGame()                   │ │
│  └─────────────────────────────────────────┘ │
│  [+] Add event                               │
│                                               │
└───────────────────────────────────────────────┘

Button Hierarchy:
─────────────────

PlayButton
├─ Image (background sprite)
│  • Color: Green
│  • Sprite: button_normal
├─ Text (label)
   • Text: "PLAY"
   • Font Size: 24
   • Color: White
```

---

## File and Folder Structure

### Project Folder Organization

```
Assets/
│
├─── _MonstersOut/                    ← Main game folder
│    │
│    ├─── Scenes/                     ← All game scenes
│    │    ├─ MainMenu.unity
│    │    ├─ Level_1.unity
│    │    ├─ Level_2.unity
│    │    └─ TestScene.unity
│    │
│    ├─── Scripts/                    ← All C# scripts
│    │    ├─ Controllers/             ← Input handling
│    │    │  ├─ Controller2D.cs
│    │    │  └─ PlayerInput.cs
│    │    │
│    │    ├─ Managers/                ← Singleton managers
│    │    │  ├─ GameManager.cs
│    │    │  ├─ SoundManager.cs
│    │    │  ├─ LevelEnemyManager.cs
│    │    │  └─ MenuManager.cs
│    │    │
│    │    ├─ Player/                  ← Player scripts
│    │    │  ├─ Player_Archer.cs
│    │    │  └─ PlayerShooting.cs
│    │    │
│    │    ├─ Enemy/                   ← Enemy scripts
│    │    │  ├─ Enemy.cs (base class)
│    │    │  ├─ SmartEnemyGrounded.cs
│    │    │  ├─ EnemyMeleeAttack.cs
│    │    │  ├─ EnemyRangeAttack.cs
│    │    │  └─ EnemyThrowAttack.cs
│    │    │
│    │    ├─ UI/                      ← UI scripts
│    │    │  ├─ HealthBarEnemyNew.cs
│    │    │  ├─ UI_UI.cs
│    │    │  ├─ Menu_Victory.cs
│    │    │  └─ Menu_Fail.cs
│    │    │
│    │    └─ Helpers/                 ← Utility scripts
│    │       ├─ GlobalValue.cs
│    │       ├─ IListener.cs
│    │       └─ ICanTakeDamage.cs
│    │
│    ├─── Prefabs/                    ← Reusable GameObjects
│    │    ├─ Player/
│    │    │  └─ Player.prefab
│    │    │
│    │    ├─ Enemies/
│    │    │  ├─ Goblin.prefab
│    │    │  ├─ Skeleton.prefab
│    │    │  └─ TrollWarrior.prefab
│    │    │
│    │    ├─ Projectiles/
│    │    │  ├─ Arrow.prefab
│    │    │  └─ Bomb.prefab
│    │    │
│    │    └─ UI/
│    │       ├─ HealthBar.prefab
│    │       ├─ VictoryMenu.prefab
│    │       └─ FailMenu.prefab
│    │
│    ├─── Animations/                 ← Animation files
│    │    ├─ Player/
│    │    │  ├─ Player_Controller.controller
│    │    │  ├─ player_idle.anim
│    │    │  ├─ player_walk.anim
│    │    │  └─ player_attack.anim
│    │    │
│    │    └─ Enemies/
│    │       ├─ Goblin_Controller.controller
│    │       └─ goblin_walk.anim
│    │
│    ├─── Sprites/                    ← Graphics
│    │    ├─ Player/
│    │    ├─ Enemies/
│    │    ├─ Environment/
│    │    └─ UI/
│    │
│    ├─── Audio/                      ← Sound files
│    │    ├─ Music/
│    │    │  ├─ music_MENU.wav
│    │    │  └─ music_GAME.wav
│    │    │
│    │    └─ Sound/
│    │       ├─ Click.wav
│    │       ├─ Victory.wav
│    │       └─ Weapon/
│    │          ├─ sword_hit.wav
│    │          └─ bow_shoot.wav
│    │
│    └─── Materials/                  ← Shaders, materials
│         └─ Sprite-Default.mat
│
├─── Resources/                       ← Loadable at runtime
│    └─ BillingMode.json
│
├─── Plugins/                         ← Third-party SDKs
│
└─── StreamingAssets/                 ← External files

Documents/                            ← Documentation (outside Assets)
├─ 00_START_HERE.md
├─ 00_Unity_Fundamentals.md
├─ 01_Project_Architecture.md
├─ 02_Player_System_Complete.md
├─ 03_Enemy_System_Complete.md
├─ 10_How_To_Guides.md
├─ 11_Troubleshooting.md
└─ 12_Visual_Reference.md (this file)
```

### Asset Naming Conventions

```
Scripts:
────────
UpperCamelCase with descriptive names
✓ GameManager.cs
✓ Player_Archer.cs
✓ EnemyRangeAttack.cs
✗ gm.cs
✗ script1.cs

Prefabs:
────────
UpperCamelCase, descriptive
✓ Goblin.prefab
✓ Arrow.prefab
✓ HealthBar.prefab
✗ prefab1.prefab
✗ new_prefab.prefab

Scenes:
───────
UpperCamelCase with context
✓ MainMenu.unity
✓ Level_1.unity
✓ TestScene.unity
✗ scene1.unity
✗ Untitled.unity

Sprites:
────────
lowercase_with_underscores
✓ player_idle_00.png
✓ goblin_walk_05.png
✓ button_normal.png
✗ PlayerIdle.png
✗ img1.png

Audio:
──────
descriptive_lowercase
✓ music_MENU.wav
✓ sword_hit.wav
✓ footstep.wav
✗ sound1.wav
✗ audio.mp3
```

---

## Summary

This visual reference guide provides diagrams and visual aids for understanding "Lawn Defense: Monsters Out":

### Key Takeaways:

1. **Unity Editor Layout**
   - Hierarchy: Scene structure
   - Inspector: Component properties
   - Project: Asset browser
   - Console: Debug messages

2. **GameObject Structure**
   - GameObject = container
   - Components = functionality
   - Parent-child hierarchy for organization

3. **Game Loop**
   - Awake → Start → Update loop
   - Use Update() for game logic
   - Use FixedUpdate() for physics
   - Always multiply movement by Time.deltaTime

4. **Event Systems**
   - Observer pattern with IListener
   - Button OnClick events
   - Scene loading flow

5. **Physics**
   - Collision vs Trigger
   - Layer collision matrix
   - Raycast visualization
   - Controller2D multi-ray system

6. **State Machines**
   - Enemy states: SPAWNING → IDLE → WALK → ATTACK → HIT → DEATH
   - Game states: MENU → PLAYING → PAUSE/SUCCESS/FAIL

7. **UI System**
   - Canvas render modes
   - Anchors for responsive design
   - Slider for health bars
   - Button events

8. **File Organization**
   - Organize by type (Scripts, Prefabs, Sprites)
   - Use subfolders for categories
   - Follow naming conventions
   - Keep related assets together

### Using These Diagrams:

- **Reference while coding**: Check execution order, component setup
- **Planning features**: Map out state machines before coding
- **Debugging**: Compare your setup to reference diagrams
- **Learning**: Understand how systems connect

### Additional Resources:

- **00_Unity_Fundamentals.md** - Learn Unity basics
- **01_Project_Architecture.md** - System overview
- **10_How_To_Guides.md** - Step-by-step tutorials
- **11_Troubleshooting.md** - Fix common problems

---

**Document End**

All diagrams are created with ASCII art for universal accessibility. For interactive learning, open Unity Editor and compare these diagrams with your actual project structure.