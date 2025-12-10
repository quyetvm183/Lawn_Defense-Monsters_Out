# Learning Roadmap for "Lawn Defense: Monsters Out"

**Purpose**: Suggested learning path to understand and contribute to this project, from zero Unity knowledge to advanced features.

**Note**: For a comprehensive beginner's guide, see `00_START_HERE.md`.

**This roadmap**: Provides a structured 1-4 week learning plan with daily goals.

---

## Who Is This For?

**Beginner**: Never used Unity before → Follow full roadmap (3-4 weeks)

**Intermediate**: Know Unity basics → Start at Phase B (2-3 weeks)

**Advanced**: Experienced with Unity → Start at Phase C (1 week)

---

## Time Estimates

- **Phase A** (Unity Fundamentals): 2-4 days
- **Phase B** (Project Code): 2-4 days
- **Phase C** (Hands-On Practice): 2-5 days
- **Phase D** (Advanced Topics): 2-5 days

**Total**: 1-3 weeks (depending on daily time commitment)

---

## Phase A: Unity Fundamentals (2-4 days)

**Goal**: Learn essential Unity concepts before touching project code.

### Day 1-2: Unity Editor Basics

**What to Learn**:
- Unity interface: Scene, Game, Hierarchy, Inspector, Project, Console windows
- GameObject and Component architecture
- Prefabs and instances
- Scenes and scene management
- Tags and Layers

**How to Learn**:
1. Read `00_Unity_Fundamentals.md` → "Getting Started with Unity"
2. Follow Unity's official "Roll-a-ball" tutorial
3. Create a simple scene with 3 GameObjects

**Practice Task**:
```
Create a simple scene:
1. Add a cube (GameObject → 3D Object → Cube)
2. Add a Rigidbody component
3. Press Play, watch it fall
4. Add a plane below to catch it
```

### Day 3: MonoBehaviour & Scripting Basics

**What to Learn**:
- MonoBehaviour lifecycle (Awake, Start, Update, FixedUpdate)
- Basic C# syntax for Unity
- Getting components (`GetComponent<>()`)
- Finding GameObjects
- Coroutines basics

**How to Learn**:
1. Read `00_Unity_Fundamentals.md` → "MonoBehaviour Lifecycle"
2. Read `13_Code_Examples.md` → Basic examples
3. Write a simple script:

```csharp
using UnityEngine;

public class RotateCube : MonoBehaviour
{
    public float speed = 50f;

    void Update()
    {
        transform.Rotate(Vector3.up * speed * Time.deltaTime);
    }
}
```

### Day 4: Physics & UI Basics

**What to Learn**:
- Physics2D: Rigidbody2D, Collider2D
- Raycasts and collision detection
- Basic UI: Canvas, Text, Button

**How to Learn**:
1. Read `00_Unity_Fundamentals.md` → "Physics and Collisions"
2. Create a project with:
   - Moving character (arrow keys)
   - Collectable coins (OnTriggerEnter2D)
   - UI text showing score

**✅ Checkpoint**: You should now understand:
- How Unity editor works
- Basic C# scripting for Unity
- GameObject/Component relationship
- Physics and collision basics

---

## Phase B: Project Code Structure (2-4 days)

**Goal**: Understand this project's architecture and core systems.

### Day 5: Project Overview

**What to Study**:
1. Read `README.md`
2. Read `01_Project_Architecture.md`
3. Explore folder structure:
   - `Assets/_MonstersOut/Scripts/Managers/` → Core managers
   - `Assets/_MonstersOut/Scripts/Enemy/` → Enemy system
   - `Assets/_MonstersOut/Scripts/Player/` → Player system
   - `Assets/_MonstersOut/Scripts/UI/` → UI scripts

**Practice Task**:
```
Open project in Unity:
1. Locate GameManager in Game scene
2. Find Enemy.cs in Project window
3. Open MenuManager.cs and read through it
4. Identify 3 manager scripts and note their purposes
```

### Day 6-7: Core Systems Deep Dive

**What to Study (pick 2-3 per day)**:
- **GameManager**: Read `05_Managers_Complete.md` → GameManager section
- **Enemy System**: Read `Enemy-Deep.md`
- **Player System**: Read `Player-Deep.md`
- **UI System**: Read `04_UI_System_Complete.md`

**Practice Tasks**:

**Day 6 - Enemy System**:
1. Open Goblin prefab
2. Find Enemy.cs component
3. Read Enemy.cs → `TakeDamage()` method
4. Add Debug.Log to see when Goblin takes damage
5. Run game, shoot Goblin, check Console

**Day 7 - Player System**:
1. Open Player_Archer prefab
2. Find Player_Archer.cs component
3. Read `Player-Deep.md` → Auto-targeting section
4. Understand how archer finds and shoots enemies
5. Modify arrow damage in Inspector, test

**✅ Checkpoint**: You should now understand:
- How GameManager controls game flow
- How Enemy.cs works (TakeDamage, Die, effects)
- How Player_Archer auto-targets and shoots
- How UI updates (health bars, wave progress)

---

## Phase C: Hands-On Practice (2-5 days)

**Goal**: Make actual changes to the project.

### Day 8: Simple Modifications

**Tasks**:

1. **Modify enemy health**:
   - Open Goblin prefab
   - Change `maxHealth` from 100 to 200
   - Test: Takes more hits to kill

2. **Change player damage**:
   - Open Player_Archer prefab
   - Find `UpgradedCharacterParameter` component
   - Change `defaultRangeDamage`
   - Test: Arrows deal more damage

3. **Add debug logs**:
   ```csharp
   // In Enemy.cs → TakeDamage():
   public override void TakeDamage(...)
   {
       Debug.Log($"{gameObject.name} took {damage} damage! Health: {currentHealth}");
       // ... rest of method
   }
   ```

### Day 9-10: Add New Enemy

**Goal**: Create "Fast Goblin" variant

**Steps**:
1. Follow `10_How_To_Guides.md` → "Creating a New Enemy Type"
2. Duplicate Goblin prefab
3. Rename to "FastGoblin"
4. Increase `moveSpeed` from 2 to 4
5. Change sprite color (tint to red)
6. Add to Level_1 wave configuration
7. Test in Play mode

### Day 11: Modify UI

**Goal**: Add "Enemies Killed" counter

**Steps**:
1. Open UI canvas in Game scene
2. Add UI Text element
3. Create script:

```csharp
namespace RGame
{
    public class KillCounter : MonoBehaviour, IListener
    {
        public Text killText;
        private int killCount = 0;

        void OnEnable()
        {
            GameManager.Instance.AddListener(this);
        }

        public void OnEnemyKilled()
        {
            killCount++;
            killText.text = "Kills: " + killCount;
        }

        public void IPlay() { killCount = 0; }
        public void ISuccess() { }
        public void IGameOver() { }
        public void IPause() { }
        public void IUnPause() { }
    }
}
```

4. Connect to GameManager event system
5. Test: Counter increments on kills

**✅ Checkpoint**: You can now:
- Modify existing prefabs
- Create new enemy variants
- Add simple UI elements
- Write basic gameplay scripts

---

## Phase D: Advanced Features (2-5 days)

**Goal**: Understand and implement complex features.

### Day 12: Character Upgrade System

**What to Study**:
- Read `Character-Properties.md` in full
- Understand `UpgradedCharacterParameter.cs`
- Study upgrade save/load system (PlayerPrefs properties)

**Practice**:
1. Create new upgrade tier:
   - Open Player_Archer prefab
   - Find `UpgradeSteps` array
   - Add new upgrade: +50 health, +20 damage, 1000 coin price
2. Test in shop: Upgrade applies correctly

### Day 13: Level Creation

**What to Study**:
- Read `Map.md` → "Creating New Levels"
- Understand `GameLevelSetup.cs`
- Study wave configuration

**Practice**:
1. Create Level_4:
   - Duplicate Level_3 prefab
   - Modify waves (add more enemies, tougher enemies)
   - Set givenMana to 1500
   - Set enemyFortrestLevel to 4
2. Add to GameManager.gameLevels array
3. Test: Can load and complete new level

### Day 14: Custom Enemy Ability

**Goal**: Add "Regenerating Enemy"

**Steps**:
1. Create script:

```csharp
namespace RGame
{
    public class RegeneratingEnemy : SmartEnemyGrounded
    {
        public int regenPerSecond = 5;

        protected override void Start()
        {
            base.Start();
            StartCoroutine(RegenerateCo());
        }

        IEnumerator RegenerateCo()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);

                if (currentHealth < maxHealth)
                {
                    currentHealth += regenPerSecond;
                    currentHealth = Mathf.Min(currentHealth, maxHealth);
                }
            }
        }
    }
}
```

2. Create "RegeneratingGoblin" prefab
3. Attach RegeneratingEnemy script
4. Test: Health regenerates over time

### Day 15: Advanced Systems

**Choose one to study deeply**:
- **Event System**: Read `Events-and-Triggers.md`, implement custom event
- **Scene Management**: Read `Map.md`, create custom loading screen
- **Shop System**: Read `ShopUI.md`, add new purchasable item

**✅ Checkpoint**: You can now:
- Create complex enemy behaviors
- Design and implement new levels
- Work with character upgrade system
- Understand advanced project systems

---

## What's Next?

After completing this roadmap, you should be able to:

### Contribute Features
- Add new enemies with unique abilities
- Create new levels and boss fights
- Design upgrade systems
- Implement new UI features

### Fix Bugs
- Debug gameplay issues
- Fix animation problems
- Resolve collision bugs
- Optimize performance

### Extend Systems
- Add multiplayer support
- Implement new game modes
- Create level editor
- Add achievements system

---

## Learning Resources

### Internal Documentation
- `00_START_HERE.md` - Comprehensive beginner guide
- `00_Unity_Fundamentals.md` - Unity basics (1,200+ lines)
- `10_How_To_Guides.md` - Step-by-step tutorials
- `11_Troubleshooting.md` - Debugging help
- `13_Code_Examples.md` - Code patterns

### External Resources
- [Unity Learn](https://learn.unity.com/) - Official tutorials
- [Unity Manual](https://docs.unity3d.com/Manual/) - Reference documentation
- [Unity Scripting Reference](https://docs.unity3d.com/ScriptReference/) - API docs

---

## Daily Time Commitment

**Beginner** (no Unity experience):
- 2-3 hours/day for 3-4 weeks
- Focus on understanding concepts, not speed

**Intermediate** (know Unity basics):
- 1-2 hours/day for 2-3 weeks
- Focus on project-specific systems

**Advanced** (Unity experienced):
- 1 hour/day for 1-2 weeks
- Focus on project architecture and advanced features

---

## Tips for Success

1. **Don't rush**: Understanding concepts is more important than finishing quickly
2. **Practice daily**: 30 minutes daily > 3 hours once a week
3. **Ask questions**: Comment in code, write notes, discuss with team
4. **Break things**: Experiment in a test branch, learn from errors
5. **Read code**: Study existing systems before writing new code
6. **Test frequently**: Play-test after every change

---

## Summary

**Roadmap Overview**:
```
Phase A (2-4 days): Learn Unity fundamentals
        ↓
Phase B (2-4 days): Study project code architecture
        ↓
Phase C (2-5 days): Make simple modifications
        ↓
Phase D (2-5 days): Implement advanced features
        ↓
Ready to contribute! 🎉
```

**Remember**: This is a *suggested* path. Adjust based on:
- Your existing Unity knowledge
- Available time
- Learning style (some prefer reading, others prefer experimenting)
- Project needs (what features are you working on?)

**Next Steps**:
- Start with `00_START_HERE.md` if completely new
- Jump to Phase B if you know Unity basics
- Dive into specific documentation as needed
- Happy coding! 🚀
