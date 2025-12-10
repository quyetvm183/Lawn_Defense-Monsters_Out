# Lawn Defense: Monsters Out - Documentation

**Professional Unity Game Development Documentation**

📚 **Complete guide** from beginner to advanced
🎯 **Production-ready** technical documentation
🚀 **Hands-on** practical tutorials and examples
⚡ **Quick reference** for experienced developers

---

## 🎮 Quick Start

**👉 NEW TO THIS PROJECT?**
→ **[START HERE](00_START_HERE.md)** ⭐

**Looking for something specific?**
→ Use the [Quick Navigation](#-quick-navigation) below

**Need a term explained?**
→ Check the **[Glossary](99_Glossary.md)**

---

## 📖 Table of Contents

1. [Quick Navigation](#-quick-navigation)
2. [Documentation Structure](#-documentation-structure)
3. [Learning Paths](#-learning-paths)
4. [File Directory](#-file-directory)
5. [Quick Reference](#-quick-reference)
6. [Contributing](#-contributing)

---

## 🧭 Quick Navigation

### By Experience Level

| Experience | Start Here | Then Read | Finally |
|------------|------------|-----------|---------|
| **Complete Beginner** | [Unity Fundamentals](00_Unity_Fundamentals.md) | [Project Architecture](01_Project_Architecture.md) | [Player System](02_Player_System_Complete.md) |
| **Some Unity Knowledge** | [Project Architecture](01_Project_Architecture.md) | [START HERE](00_START_HERE.md) | System docs as needed |
| **Experienced Developer** | [START HERE](00_START_HERE.md) | [Project Analysis](project-analysis.md) | Specific system docs |

### By Task

| I want to... | Read This |
|--------------|-----------|
| **Understand the project** | [Project Architecture](01_Project_Architecture.md) |
| **Modify player shooting** | [Player System](02_Player_System_Complete.md) |
| **Add new enemy** | How-To Guides (coming) |
| **Change UI/menus** | UI System docs (coming) |
| **Fix an error** | Troubleshooting (coming) |
| **Understand a term** | [Glossary](99_Glossary.md) |
| **See code examples** | Code Examples (coming) |

### By Topic

| Topic | Documentation |
|-------|---------------|
| **Unity Basics** | [Unity Fundamentals](00_Unity_Fundamentals.md) |
| **Game Architecture** | [Project Architecture](01_Project_Architecture.md) |
| **Player Character** | [Player System](02_Player_System_Complete.md) |
| **Enemy AI** | Enemy System (coming) |
| **UI System** | UI System (coming) |
| **Game Managers** | Managers (coming) |
| **Patterns & Practices** | [Project Architecture](01_Project_Architecture.md) §5 |

---

## 📚 Documentation Structure

### Core Documentation (New - English)

```
Documents/
├── 📄 README.md                           ← YOU ARE HERE
├── 🎯 00_START_HERE.md                    ← Entry point for all users
│
├── 📘 Fundamentals
│   ├── 00_Unity_Fundamentals.md           ← Unity basics from zero
│   └── 01_Project_Architecture.md         ← Project structure & patterns
│
├── 🔧 System Documentation
│   ├── 02_Player_System_Complete.md       ← Player mechanics in detail
│   ├── 03_Enemy_System_Complete.md        ← Enemy AI (planned)
│   ├── 04_UI_System_Complete.md           ← User interface (planned)
│   ├── 05_Managers_Complete.md            ← Manager classes (planned)
│   └── 06_AI_System_Complete.md           ← AI decision making (planned)
│
├── 📚 Practical Guides
│   ├── 10_How_To_Guides.md                ← Step-by-step tutorials (planned)
│   ├── 11_Troubleshooting.md              ← Common problems (planned)
│   └── 13_Code_Examples.md                ← Code snippets (planned)
│
├── 📖 Reference
│   ├── 12_Visual_Reference.md             ← Diagrams & visuals (planned)
│   ├── 99_Glossary.md                     ← A-Z term definitions
│   └── project-analysis.md                ← Technical analysis
│
└── 📂 scripts/ (Legacy Vietnamese docs)
    ├── Scripts-Overview.md
    ├── AI.md, Controllers.md, Helpers.md
    ├── Managers.md, Player.md, UI.md
    └── ... (20 files - older documentation)
```

### Priority Reading Order

**Week 1-2: Foundation**
1. ⭐ [00_START_HERE.md](00_START_HERE.md)
2. 📘 [00_Unity_Fundamentals.md](00_Unity_Fundamentals.md) (if new to Unity)
3. 🏗️ [01_Project_Architecture.md](01_Project_Architecture.md)

**Week 3-4: Core Systems**
4. 🏹 [02_Player_System_Complete.md](02_Player_System_Complete.md)
5. 👾 Enemy System (when available)
6. 🎨 UI System (when available)

**Ongoing: Reference**
- 📖 [99_Glossary.md](99_Glossary.md) - Look up terms
- 🔧 How-To Guides (when available)
- 🐛 Troubleshooting (when available)

---

## 🎓 Learning Paths

### Path A: Complete Beginner (Never Used Unity)

**Goal:** Understand Unity AND this project

**Time:** 4-6 weeks (10 hours/week)

```
Week 1-2: Unity Fundamentals
└─ Read: 00_Unity_Fundamentals.md
└─ Practice: Open Unity, explore interface
└─ Checkpoint: Understand GameObjects, Components, Prefabs

Week 3: Project Structure
└─ Read: 01_Project_Architecture.md
└─ Practice: Navigate scripts folder, run game
└─ Checkpoint: Know where Player/Enemy/Manager code is

Week 4: Player System
└─ Read: 02_Player_System_Complete.md
└─ Practice: Modify shootRate, test changes
└─ Checkpoint: Understand auto-targeting

Week 5-6: Hands-On
└─ Read: How-To Guides (when available)
└─ Practice: Add new enemy, modify UI
└─ Checkpoint: Made first feature modification
```

**Success Criteria:**
- ✅ Can explain what a GameObject is
- ✅ Located and modified player fire rate
- ✅ Understands project architecture
- ✅ Completed one How-To guide

### Path B: Intermediate (Some Unity Experience)

**Goal:** Understand this specific project quickly

**Time:** 1-2 weeks (10 hours/week)

```
Day 1-2: Project Overview
└─ Skim: 00_Unity_Fundamentals.md (refresh)
└─ Read: 01_Project_Architecture.md (complete)
└─ Read: 02_Player_System_Complete.md
└─ Checkpoint: Understand Listener pattern, Player inherits Enemy

Day 3-4: System Deep Dives
└─ Read: System docs relevant to goals
└─ Practice: Locate key classes, read code
└─ Checkpoint: Identified modification points

Week 2: Implementation
└─ Read: How-To Guides for specific tasks
└─ Practice: Implement features
└─ Checkpoint: Working modifications
```

**Success Criteria:**
- ✅ Understands all design patterns used
- ✅ Located all core systems
- ✅ Modified at least 2 systems successfully

### Path C: Expert (Experienced Unity Dev)

**Goal:** Quick orientation, then implement

**Time:** 2-3 days

```
Hour 1-2: High-Level
└─ Read: 01_Project_Architecture.md
└─ Read: project-analysis.md
└─ Checkpoint: Understand architecture

Hour 3-4: Code Review
└─ Read: GameManager.cs, Player_Archer.cs, Enemy.cs
└─ Review: Folder structure, design patterns
└─ Checkpoint: Ready to modify

Day 2-3: Implementation
└─ Reference: System docs as needed
└─ Reference: Glossary for project-specific terms
└─ Checkpoint: Feature implemented and tested
```

**Success Criteria:**
- ✅ Full project comprehension
- ✅ Modified codebase confidently
- ✅ No external resources needed

---

## 📁 File Directory

### 🎯 Essential Files (Read First)

| Priority | File | Description | Read Time |
|----------|------|-------------|-----------|
| ⭐⭐⭐ | [00_START_HERE.md](00_START_HERE.md) | Your roadmap to all documentation | 5 min |
| ⭐⭐⭐ | [99_Glossary.md](99_Glossary.md) | A-Z term definitions | 1-2 min per term |
| ⭐⭐ | [01_Project_Architecture.md](01_Project_Architecture.md) | Complete project structure | 30-40 min |

### 📘 Fundamentals

| File | Description | Read Time | Audience |
|------|-------------|-----------|----------|
| [00_Unity_Fundamentals.md](00_Unity_Fundamentals.md) | Unity from zero to hero | 2-3 hours | Beginners |
| [01_Project_Architecture.md](01_Project_Architecture.md) | Structure & patterns | 30-40 min | All |
| [project-analysis.md](project-analysis.md) | Technical deep dive | 20-30 min | Advanced |

### 🔧 System Documentation

| File | Status | Description | Read Time |
|------|--------|-------------|-----------|
| [02_Player_System_Complete.md](02_Player_System_Complete.md) | ✅ **Complete** | Player mechanics | 45 min |
| 03_Enemy_System_Complete.md | 📝 Planned | Enemy AI & behavior | ~40 min |
| 04_UI_System_Complete.md | 📝 Planned | User interface | ~30 min |
| 05_Managers_Complete.md | 📝 Planned | Manager classes | ~35 min |
| 06_AI_System_Complete.md | 📝 Planned | AI decision making | ~30 min |

### 📚 Practical Guides

| File | Status | Description | Use When |
|------|--------|-------------|----------|
| 10_How_To_Guides.md | 📝 Planned | Step-by-step tutorials | Making specific changes |
| 11_Troubleshooting.md | 📝 Planned | Common problems & fixes | Stuck with errors |
| 13_Code_Examples.md | 📝 Planned | Copy-paste code | Need quick implementation |

### 📖 Reference Materials

| File | Status | Description | Use When |
|------|--------|-------------|----------|
| [99_Glossary.md](99_Glossary.md) | ✅ **Complete** | Term definitions | Don't understand term |
| 12_Visual_Reference.md | 📝 Planned | Diagrams & flowcharts | Visual learner |

### 📂 Legacy Documentation (Vietnamese)

Located in `scripts/` subfolder - older documentation in Vietnamese:

- Scripts-Overview.md
- AI.md, Controllers.md, Helpers.md
- Managers.md, Player.md, UI.md
- Roadmap.md, Unity-Concepts.md
- (20 files total)

**Note:** New English documentation above supersedes these files.

---

## 🔍 Quick Reference

### Common Tasks

```markdown
# Change player fire rate
→ Open: Player GameObject in scene
→ Component: Player_Archer
→ Field: shootRate
→ Lower value = faster shooting

# Add new enemy
→ Read: How-To Guides (when available)
→ Duplicate: Existing enemy prefab
→ Modify: Sprite, stats, behavior

# Modify UI
→ Find: Canvas in Hierarchy
→ Edit: UI elements
→ Scripts: Assets/_MonstersOut/Scripts/UI/

# Debug errors
→ Read: Console (bottom of Unity)
→ Double-click: Error to jump to code
→ Check: Troubleshooting.md (when available)
```

### Code Patterns

```csharp
// Access GameManager
GameManager.Instance.Victory();

// Get component
var rb = GetComponent<Rigidbody2D>();

// Find GameObject
var player = GameObject.FindGameObjectWithTag("Player");

// Instantiate prefab
Instantiate(prefab, position, Quaternion.identity);

// Coroutine
IEnumerator Wait() {
    yield return new WaitForSeconds(1f);
}

// Event
public static event Action OnDeath;
OnDeath?.Invoke();
```

### Unity Lifecycle

```
Awake() → OnEnable() → Start() →
Update() / FixedUpdate() / LateUpdate() (loop) →
OnDisable() → OnDestroy()
```

### Project Patterns

**Singleton:** GameManager, SoundManager
→ Access: `Instance` property

**Observer:** IListener interface
→ GameManager broadcasts events

**State Machine:** Enemy states
→ SPAWNING, IDLE, WALK, ATTACK, HIT, DEATH

**Inheritance:** Player inherits Enemy
→ Reuses health, damage, effects

---

## 📊 Documentation Statistics

**Current Status:**
- ✅ **Complete:** 6 documents (~5000+ lines)
- 📝 **Planned:** 7 documents
- 📂 **Legacy:** 20 documents (Vietnamese)

**Coverage:**
- Unity Fundamentals: ✅ Complete
- Project Architecture: ✅ Complete
- Player System: ✅ Complete
- Enemy System: 📝 Planned
- UI System: 📝 Planned
- Managers: 📝 Planned
- AI System: 📝 Planned

**Quality Metrics:**
- ✅ Beginner-friendly explanations
- ✅ Line-by-line code comments
- ✅ Visual ASCII diagrams
- ✅ Practical examples
- ✅ Troubleshooting sections
- ✅ Cross-references

---

## 🎯 Documentation Goals

### Primary Objectives

**✅ Achieved:**
1. Enable complete beginners to understand Unity fundamentals
2. Provide comprehensive project architecture overview
3. Document player system with trajectory calculation details
4. Create navigable documentation structure
5. Define all technical terms in glossary

**📝 In Progress:**
6. Document all core systems (Enemy, UI, Managers, AI)
7. Provide step-by-step how-to guides
8. Create troubleshooting database
9. Compile code example library

### Success Criteria

**For Beginners:**
- [ ] Can open Unity and navigate interface
- [x] Understands GameObjects and Components
- [x] Can modify Inspector values
- [ ] Completed first feature modification

**For Intermediate:**
- [x] Understands project architecture
- [x] Located all core systems
- [x] Knows design patterns used
- [ ] Implemented custom feature

**For Advanced:**
- [x] Full codebase comprehension
- [x] Identified all extension points
- [ ] Contributed code improvements
- [ ] Optimized system performance

---

## 💡 Using This Documentation

### Tips for Success

**🔖 Bookmark These:**
- [00_START_HERE.md](00_START_HERE.md) - Main entry point
- [99_Glossary.md](99_Glossary.md) - Term lookup
- This README - Navigation hub

**📖 Reading Strategy:**
1. **Skim first** - Get overview
2. **Read actively** - Take notes
3. **Practice immediately** - Open Unity, test
4. **Reference often** - Come back when needed

**🎯 Set Goals:**
- "I will understand Player system" ✅ Good
- "I will learn Unity" ❌ Too vague

**🔁 Iterate:**
- Small changes → Test → Learn → Repeat
- Don't try to understand everything at once

### How to Navigate

**By Experience:**
- Beginner → Follow Path A
- Intermediate → Follow Path B
- Expert → Follow Path C

**By Goal:**
- Understand → Read fundamentals
- Modify → Read system docs + how-tos
- Extend → Read architecture + patterns

**By Time:**
- 5 minutes → Quick reference section
- 30 minutes → Single system doc
- 2-3 hours → Complete fundamentals

---

## 🤝 Contributing

### Reporting Issues

Found a problem in documentation?

**What to report:**
- Typos and grammatical errors
- Broken links
- Unclear explanations
- Missing information
- Code errors

**How to report:**
- Create issue in project repository
- Email documentation team
- Mark specific line in document

### Suggesting Improvements

**Good suggestions:**
- Additional diagrams
- More code examples
- Clarification of complex topics
- New how-to guides
- Performance tips

### Documentation Standards

**All documentation follows:**
- ✅ Beginner-friendly language
- ✅ Code examples with explanations
- ✅ Visual diagrams where helpful
- ✅ Cross-references to related topics
- ✅ Real project code (not theoretical)

---

## 📞 Getting Help

### Search Order

1. **This README** - Check quick reference
2. **Glossary** - Look up term
3. **System Docs** - Deep dive into system
4. **Troubleshooting** - Common problems (when available)
5. **External Resources** - Unity docs, forums

### External Resources

**Unity Official:**
- Manual: https://docs.unity3d.com/Manual/
- Scripting API: https://docs.unity3d.com/ScriptReference/
- Learn: https://learn.unity.com/

**Community:**
- Forum: https://forum.unity.com/
- Reddit: r/Unity3D
- Stack Overflow: [unity3d] tag

**This Project:**
- Documentation: You're reading it!
- Code: `Assets/_MonstersOut/Scripts/`
- Examples: Existing prefabs and scenes

---

## 📅 Version History

**Version 2.0** (October 2025) - Current
- ✅ Complete rewrite in English
- ✅ Beginner-friendly approach
- ✅ Professional documentation standards
- ✅ Visual diagrams and examples
- ✅ Comprehensive glossary
- ✅ Navigation system

**Version 1.0** (Original)
- Vietnamese documentation
- 20 files in scripts/ folder
- Basic system descriptions
- Minimal examples

---

## 🚀 Next Steps

**1. New to Unity?**
→ Start with [00_START_HERE.md](00_START_HERE.md)
→ Then read [00_Unity_Fundamentals.md](00_Unity_Fundamentals.md)

**2. Know Unity basics?**
→ Jump to [01_Project_Architecture.md](01_Project_Architecture.md)
→ Then read [02_Player_System_Complete.md](02_Player_System_Complete.md)

**3. Expert developer?**
→ Read [project-analysis.md](project-analysis.md)
→ Skim system docs as needed

**4. Specific task?**
→ Check [00_START_HERE.md](00_START_HERE.md) Section 3 (Quick Tasks)

---

## 📌 Quick Links

**Most Important:**
- 🎯 [START HERE](00_START_HERE.md) - Begin your journey
- 📖 [Glossary](99_Glossary.md) - Look up terms
- 🏗️ [Architecture](01_Project_Architecture.md) - Understand structure

**By Topic:**
- Unity Basics → [Unity Fundamentals](00_Unity_Fundamentals.md)
- Player Character → [Player System](02_Player_System_Complete.md)
- Technical Analysis → [Project Analysis](project-analysis.md)

**Reference:**
- All terms → [Glossary](99_Glossary.md)
- Code patterns → This README §Quick Reference
- Design patterns → [Architecture](01_Project_Architecture.md) §5

---

**Ready to begin?** → [00_START_HERE.md](00_START_HERE.md) ⭐

**Questions?** → [99_Glossary.md](99_Glossary.md) for terms

**Good luck! 🎮**

---

<p align="center">
<strong>Lawn Defense: Monsters Out</strong><br>
Professional Game Development Documentation<br>
Version 2.0 • October 2025
</p>
