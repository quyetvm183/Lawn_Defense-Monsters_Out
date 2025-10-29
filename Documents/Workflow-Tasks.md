# Development Workflow & Common Tasks

**Purpose**: Practical guide to development workflows, git practices, and common tasks for working on "Lawn Defense: Monsters Out".

**What This Document Covers**:
- Setting up your development environment
- Git workflow and branching strategy
- Common development tasks (adding enemies, features, etc.)
- Testing and debugging procedures
- Pre-merge checklist

**Related Documentation**:
- See `00_START_HERE.md` for initial project setup
- See `10_How_To_Guides.md` for step-by-step tutorials
- See `11_Troubleshooting.md` for debugging strategies
- See `First-Tasks.md` for beginner tasks

---

## 1. Environment Setup

### Step 1: Open Unity Project

1. Ensure Unity version matches `ProjectSettings/ProjectVersion.txt`
   - Current project uses Unity 2020.3.x or later
2. Open project in Unity Hub
3. Wait for initial import (may take 5-10 minutes first time)

### Step 2: Verify Scene Setup

**Main Scenes**:
- `Menu` scene: Main menu, located in `Assets/_MonstersOut/Scene/`
- `Game` scene: Gameplay, contains GameManager and level system

**Quick Test**:
1. Open Menu scene
2. Press Play
3. Verify:
   - Menu loads
   - Music plays
   - Buttons are clickable
   - Can start game

### Step 3: Configure Git

**Important**: This project is currently on branch `refactor/folder-and-asset`

```bash
# Check current branch
git branch

# If not on correct branch:
git checkout refactor/folder-and-asset

# Pull latest changes
git pull origin refactor/folder-and-asset
```

---

## 2. Git Workflow

###  Branching Strategy

**Current Branches**:
- `feat/create-prj` - Main development branch (base for PRs)
- `refactor/folder-and-asset` - Current working branch (refactoring)
- Feature branches - For specific features (e.g., `feature/new-enemy`)

### Creating a New Feature Branch

**Scenario**: Adding a new enemy type

```bash
# 1. Start from refactor/folder-and-asset
git checkout refactor/folder-and-asset
git pull origin refactor/folder-and-asset

# 2. Create feature branch
git checkout -b feature/flying-enemy

# 3. Work on your feature...

# 4. Commit changes
git add Assets/_MonstersOut/Prefabs/Enemies/FlyingEnemy.prefab
git add Assets/_MonstersOut/Scripts/Enemy/FlyingEnemy.cs
git commit -m "feat: add flying enemy with aerial movement

- Created FlyingEnemy prefab with custom animations
- Implemented aerial movement AI
- Added swooping attack behavior
- Configured spawn in wave system"

# 5. Push to remote
git push -u origin feature/flying-enemy

# 6. Create Pull Request on GitHub (if using GitHub)
```

### Commit Message Format

Follow conventional commits:

```
<type>(<scope>): <subject>

<body>

<footer>
```

**Types**:
- `feat`: New feature
- `fix`: Bug fix
- `refactor`: Code refactoring
- `docs`: Documentation changes
- `test`: Adding tests
- `chore`: Maintenance tasks

**Examples**:

```bash
# Feature
git commit -m "feat(enemy): add poison damage over time effect"

# Bug fix
git commit -m "fix(player): correct archer arrow trajectory calculation

Fixed issue where arrows would overshoot at long distances.
Updated physics calculation to account for enemy movement."

# Refactor
git commit -m "refactor: move helper classes to Helpers folder"

# Documentation
git commit -m "docs: update Enemy-Deep.md with effect system details"
```

---

## 3. Common Development Tasks

### Task 1: Add New Enemy Type

**Goal**: Create a new enemy (e.g., "Ninja" enemy with teleport ability)

**Steps**:

1. **Create branch**:
   ```bash
   git checkout -b feature/ninja-enemy
   ```

2. **Duplicate existing enemy prefab**:
   - Navigate to `Assets/_MonstersOut/Prefabs/Enemies/`
   - Right-click `Goblin` prefab → Duplicate
   - Rename to `Ninja`

3. **Create/modify script**:
   - Duplicate `SmartEnemyGrounded.cs`
   - Rename to `NinjaEnemy.cs`
   - Add custom behavior:

   ```csharp
   namespace RGame
   {
       public class NinjaEnemy : SmartEnemyGrounded
       {
           [Header("NINJA ABILITIES")]
           public float teleportDistance = 3f;
           public float teleportCooldown = 5f;
           private float lastTeleport;

           public override void Update()
           {
               base.Update();

               if (Time.time > lastTeleport + teleportCooldown)
               {
                   TeleportToPlayer();
                   lastTeleport = Time.time;
               }
           }

           void TeleportToPlayer()
           {
               // Teleport logic
               Transform player = FindObjectOfType<Player_Archer>().transform;
               transform.position = player.position + Vector3.left * teleportDistance;
           }
       }
   }
   ```

4. **Configure in Inspector**:
   - Open Ninja prefab
   - Set health, damage, speed
   - Assign animations
   - Assign sounds

5. **Add to level waves**:
   - Open level prefab (e.g., `Level_3`)
   - Add Ninja to wave configuration

6. **Test**:
   - Play in Editor
   - Verify teleport behavior
   - Check animations/sounds
   - Debug any issues

7. **Commit**:
   ```bash
   git add Assets/_MonstersOut/Prefabs/Enemies/Ninja.prefab
   git add Assets/_MonstersOut/Scripts/Enemy/NinjaEnemy.cs
   git commit -m "feat(enemy): add Ninja enemy with teleport ability"
   git push -u origin feature/ninja-enemy
   ```

---

### Task 2: Add Sound Effect

**Goal**: Add footstep sound to player

**Steps**:

1. **Import audio file**:
   - Copy `footstep.wav` to `Assets/Audio/Sound/`
   - Unity auto-imports

2. **Configure audio settings**:
   - Select `footstep.wav` in Project
   - Inspector → Force to Mono (for 2D game)
   - Load in Background: true
   - Compression Format: Vorbis

3. **Add to SoundManager**:
   - Open SoundManager prefab or scene object
   - Add to `soundEffects` array

4. **Play in code**:
   ```csharp
   public class Player_Archer : Enemy
   {
       public AudioClip footstepSound;

       void PlayFootstep()
       {
           SoundManager.PlaySfx(footstepSound, 0.5f);  // 50% volume
       }
   }
   ```

5. **Call from animation event**:
   - Open player walk animation
   - Add Animation Event at foot-down frame
   - Function: `PlayFootstep`

---

### Task 3: Add UI Text Element

**Goal**: Add "Combo x3!" text that appears on multi-kills

**Steps**:

1. **Create UI element**:
   - Open UI canvas
   - Right-click → UI → Text
   - Rename to "ComboText"
   - Position at screen center-top

2. **Configure Text component**:
   - Font size: 48
   - Color: Yellow
   - Alignment: Center
   - Horizontal Overflow: Overflow

3. **Add fade animation**:
   ```csharp
   public class ComboText : MonoBehaviour
   {
       public Text text;
       public float fadeTime = 1f;

       public void Show(int comboCount)
       {
           text.text = $"Combo x{comboCount}!";
           StartCoroutine(FadeCo());
       }

       IEnumerator FadeCo()
       {
           Color c = text.color;
           c.a = 1f;
           text.color = c;

           yield return new WaitForSeconds(0.5f);

           float elapsed = 0f;
           while (elapsed < fadeTime)
           {
               elapsed += Time.deltaTime;
               c.a = 1f - (elapsed / fadeTime);
               text.color = c;
               yield return null;
           }
       }
   }
   ```

4. **Call from GameManager**:
   ```csharp
   public class GameManager : MonoBehaviour
   {
       public ComboText comboText;
       private int comboCount = 0;

       public void OnEnemyKilled()
       {
           comboCount++;
           if (comboCount >= 3)
           {
               comboText.Show(comboCount);
           }
       }
   }
   ```

---

## 4. Testing Procedures

### Before Every Commit

**Quick Test Checklist**:
- [ ] Code compiles without errors
- [ ] No warnings in Console (or intentional warnings documented)
- [ ] Scene loads in Play mode
- [ ] Core gameplay works (spawn enemies, shoot, take damage)
- [ ] No visual glitches

### Before Creating Pull Request

**Full Test Checklist**:
- [ ] Play through 2-3 complete levels
- [ ] Test victory condition
- [ ] Test game over condition
- [ ] Test pause/resume
- [ ] Test menu navigation
- [ ] Check all new features work
- [ ] Verify no old features broken
- [ ] Test on device (if mobile game)

### Debugging Common Issues

**Issue: Enemy doesn't detect player**

```
Debug steps:
1. Check LayerMask in CheckTargetHelper
2. Add Debug.DrawRay to visualize detection raycast
3. Verify player is on correct layer (Player layer)
4. Check detection distance in Inspector
```

**Issue: Animation doesn't play**

```
Debug steps:
1. Check Animator Controller has transition to animation
2. Verify trigger/bool name matches code exactly (case-sensitive!)
3. Add Debug.Log before AnimSetTrigger call
4. Check Animation window → check animation exists
```

**Issue: Projectile doesn't deal damage**

```
Debug steps:
1. Verify projectile has correct LayerMask for targets
2. Check projectile has Collider2D (trigger enabled)
3. Verify target has ICanTakeDamage implementation
4. Add Debug.Log in OnTriggerEnter2D
```

---

## 5. Pre-Merge Checklist

Before merging your branch to main development branch:

### Code Quality
- [ ] No Debug.Log spam (remove or comment out debug logs)
- [ ] No TODO comments (or documented in separate issue)
- [ ] Code follows project namespace (RGame)
- [ ] No hardcoded values (use Inspector fields or const)
- [ ] Comments added for complex logic

### Unity Project
- [ ] All scenes saved
- [ ] All prefabs saved
- [ ] No missing script references (pink scripts)
- [ ] No missing prefab references in scenes
- [ ] ProjectSettings not changed unintentionally

### Git
- [ ] Commit message follows format
- [ ] No large binary files committed (unless necessary assets)
- [ ] .gitignore rules followed (no Library/, Temp/, etc.)
- [ ] Branch up to date with base branch

### Testing
- [ ] Builds without errors
- [ ] Playable from start to finish
- [ ] No performance regressions
- [ ] No console errors

### Documentation
- [ ] README.md updated (if needed)
- [ ] Code comments added
- [ ] Technical documentation updated (if system changed)

---

## 6. Quick Command Reference

```bash
# Git basics
git status                          # Check what's changed
git diff                            # See line-by-line changes
git log --oneline                   # See recent commits

# Git workflow
git checkout -b feature/my-feature  # Create feature branch
git add .                           # Stage all changes
git commit -m "feat: my feature"    # Commit with message
git push -u origin feature/my-feature  # Push to remote

# Git fixes
git reset HEAD~1                    # Undo last commit (keep changes)
git checkout -- file.txt            # Discard changes to file
git stash                           # Temporarily save changes
git stash pop                       # Restore stashed changes

# Unity meta files
git add -A                          # Add all files INCLUDING .meta files
```

---

## 7. Useful Unity Shortcuts

**Editor**:
- `Ctrl + P`: Play/Stop
- `Ctrl + Shift + P`: Pause
- `Ctrl + D`: Duplicate
- `F`: Frame selected object
- `Ctrl + Shift + F`: Frame selected object (Scene view)

**Debugging**:
- `Ctrl + Shift + C`: Open Console
- `Ctrl + 7`: Open Profiler
- Right-click Inspector tab → Debug: See private fields

---

## Summary

**Standard Workflow**:
1. Create feature branch from `refactor/folder-and-asset`
2. Make changes
3. Test thoroughly
4. Commit with conventional message format
5. Push to remote
6. Create PR to `feat/create-prj` (or current main branch)
7. Address review feedback
8. Merge when approved

**Key Practices**:
- Commit small, focused changes
- Write descriptive commit messages
- Test before committing
- Keep branches short-lived (merge within 1-2 weeks)
- Pull from base branch frequently to avoid conflicts

**Next Steps**:
- See `10_How_To_Guides.md` for detailed tutorials
- See `11_Troubleshooting.md` for debugging help
- See Git documentation for advanced workflows
