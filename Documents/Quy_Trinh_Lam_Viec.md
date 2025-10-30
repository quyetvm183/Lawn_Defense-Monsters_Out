# Quy Trình Làm Việc & Các Task Phổ Biến

**Mục Đích**: Hướng dẫn thực tế về quy trình làm việc, git practices, và các tasks phổ biến khi làm việc với "Lawn Defense: Monsters Out".

**Nội Dung Tài Liệu**:
- Thiết lập môi trường development
- Git workflow và branching strategy
- Các tasks development phổ biến (thêm enemies, features, v.v.)
- Testing và debugging procedures
- Pre-merge checklist

**Ngôn Ngữ**: Tiếng Việt (Vietnamese)

**Tài Liệu Liên Quan**:
- Xem `00_BAT_DAU_TU_DAY.md` cho initial project setup
- Xem `10_Huong_Dan_Thuc_Hanh.md` cho hướng dẫn từng bước
- Xem `11_Xu_Ly_Su_Co.md` cho debugging strategies
- Xem `Nhiem_Vu_Dau_Tien.md` cho beginner tasks

---

## 1. Thiết Lập Môi Trường

### Bước 1: Mở Unity Project

1. Đảm bảo Unity version khớp với `ProjectSettings/ProjectVersion.txt`
   - Project hiện tại dùng Unity 2020.3.x hoặc mới hơn
2. Mở project trong Unity Hub
3. Đợi initial import (có thể mất 5-10 phút lần đầu)

### Bước 2: Xác Minh Scene Setup

**Các Scene Chính**:
- `Menu` scene: Main menu, nằm trong `Assets/_MonstersOut/Scene/`
- `Game` scene: Gameplay, chứa GameManager và level system

**Quick Test**:
1. Mở Menu scene
2. Nhấn Play
3. Xác minh:
   - Menu loads
   - Music chạy
   - Buttons có thể click
   - Có thể start game

### Bước 3: Cấu Hình Git

**Quan Trọng**: Project này hiện đang ở branch `refactor/folder-and-asset`

```bash
# Kiểm tra branch hiện tại
git branch

# Nếu không ở branch đúng:
git checkout refactor/folder-and-asset

# Pull các thay đổi mới nhất
git pull origin refactor/folder-and-asset
```

---

## 2. Git Workflow

### Branching Strategy

**Các Branches Hiện Tại**:
- `feat/create-prj` - Main development branch (base cho PRs)
- `refactor/folder-and-asset` - Working branch hiện tại (refactoring)
- Feature branches - Cho các features cụ thể (ví dụ, `feature/new-enemy`)

### Tạo Feature Branch Mới

**Kịch Bản**: Thêm một loại enemy mới

```bash
# 1. Bắt đầu từ refactor/folder-and-asset
git checkout refactor/folder-and-asset
git pull origin refactor/folder-and-asset

# 2. Tạo feature branch
git checkout -b feature/flying-enemy

# 3. Làm việc trên feature của bạn...

# 4. Commit changes
git add Assets/_MonstersOut/Prefabs/Enemies/FlyingEnemy.prefab
git add Assets/_MonstersOut/Scripts/Enemy/FlyingEnemy.cs
git commit -m "feat: add flying enemy with aerial movement

- Created FlyingEnemy prefab with custom animations
- Implemented aerial movement AI
- Added swooping attack behavior
- Configured spawn in wave system"

# 5. Push lên remote
git push -u origin feature/flying-enemy

# 6. Tạo Pull Request trên GitHub (nếu dùng GitHub)
```

### Commit Message Format

Theo conventional commits:

```
<type>(<scope>): <subject>

<body>

<footer>
```

**Types**:
- `feat`: Feature mới
- `fix`: Bug fix
- `refactor`: Code refactoring
- `docs`: Thay đổi documentation
- `test`: Thêm tests
- `chore`: Maintenance tasks

**Ví Dụ**:

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

## 3. Các Tasks Development Phổ Biến

### Task 1: Thêm Loại Enemy Mới

**Mục Tiêu**: Tạo một enemy mới (ví dụ, "Ninja" enemy với khả năng teleport)

**Các Bước**:

1. **Tạo branch**:
   ```bash
   git checkout -b feature/ninja-enemy
   ```

2. **Duplicate enemy prefab có sẵn**:
   - Điều hướng đến `Assets/_MonstersOut/Prefabs/Enemies/`
   - Right-click `Goblin` prefab → Duplicate
   - Đổi tên thành `Ninja`

3. **Tạo/chỉnh sửa script**:
   - Duplicate `SmartEnemyGrounded.cs`
   - Đổi tên thành `NinjaEnemy.cs`
   - Thêm hành vi tùy chỉnh:

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
               // Logic teleport
               Transform player = FindObjectOfType<Player_Archer>().transform;
               transform.position = player.position + Vector3.left * teleportDistance;
           }
       }
   }
   ```

4. **Cấu hình trong Inspector**:
   - Mở Ninja prefab
   - Đặt health, damage, speed
   - Gán animations
   - Gán sounds

5. **Thêm vào level waves**:
   - Mở level prefab (ví dụ, `Level_3`)
   - Thêm Ninja vào wave configuration

6. **Test**:
   - Play trong Editor
   - Xác minh hành vi teleport
   - Kiểm tra animations/sounds
   - Debug bất kỳ vấn đề nào

7. **Commit**:
   ```bash
   git add Assets/_MonstersOut/Prefabs/Enemies/Ninja.prefab
   git add Assets/_MonstersOut/Scripts/Enemy/NinjaEnemy.cs
   git commit -m "feat(enemy): add Ninja enemy with teleport ability"
   git push -u origin feature/ninja-enemy
   ```

---

### Task 2: Thêm Sound Effect

**Mục Tiêu**: Thêm footstep sound cho player

**Các Bước**:

1. **Import audio file**:
   - Copy `footstep.wav` vào `Assets/Audio/Sound/`
   - Unity tự động imports

2. **Cấu hình audio settings**:
   - Chọn `footstep.wav` trong Project
   - Inspector → Force to Mono (cho game 2D)
   - Load in Background: true
   - Compression Format: Vorbis

3. **Thêm vào SoundManager**:
   - Mở SoundManager prefab hoặc scene object
   - Thêm vào `soundEffects` array

4. **Phát trong code**:
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

5. **Gọi từ animation event**:
   - Mở player walk animation
   - Thêm Animation Event tại foot-down frame
   - Function: `PlayFootstep`

---

### Task 3: Thêm UI Text Element

**Mục Tiêu**: Thêm "Combo x3!" text xuất hiện khi multi-kills

**Các Bước**:

1. **Tạo UI element**:
   - Mở UI canvas
   - Right-click → UI → Text
   - Đổi tên thành "ComboText"
   - Đặt vị trí ở center-top màn hình

2. **Cấu hình Text component**:
   - Font size: 48
   - Color: Yellow
   - Alignment: Center
   - Horizontal Overflow: Overflow

3. **Thêm fade animation**:
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

4. **Gọi từ GameManager**:
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

### Trước Mỗi Commit

**Quick Test Checklist**:
- [ ] Code compiles không có lỗi
- [ ] Không có warnings trong Console (hoặc warnings có chủ ý được ghi chép)
- [ ] Scene loads trong Play mode
- [ ] Core gameplay hoạt động (spawn enemies, bắn, nhận damage)
- [ ] Không có visual glitches

### Trước Khi Tạo Pull Request

**Full Test Checklist**:
- [ ] Play qua 2-3 levels hoàn chỉnh
- [ ] Test victory condition
- [ ] Test game over condition
- [ ] Test pause/resume
- [ ] Test menu navigation
- [ ] Kiểm tra tất cả features mới hoạt động
- [ ] Xác minh không có old features bị hỏng
- [ ] Test trên device (nếu là mobile game)

### Debugging Các Vấn Đề Phổ Biến

**Vấn đề: Enemy không phát hiện player**

```
Debug steps:
1. Kiểm tra LayerMask trong CheckTargetHelper
2. Thêm Debug.DrawRay để hiển thị detection raycast
3. Xác minh player đang ở layer đúng (Player layer)
4. Kiểm tra detection distance trong Inspector
```

**Vấn đề: Animation không chạy**

```
Debug steps:
1. Kiểm tra Animator Controller có transition đến animation
2. Xác minh trigger/bool name khớp code chính xác (case-sensitive!)
3. Thêm Debug.Log trước AnimSetTrigger call
4. Kiểm tra Animation window → xác nhận animation tồn tại
```

**Vấn đề: Projectile không gây damage**

```
Debug steps:
1. Xác minh projectile có LayerMask đúng cho targets
2. Kiểm tra projectile có Collider2D (trigger enabled)
3. Xác minh target có ICanTakeDamage implementation
4. Thêm Debug.Log trong OnTriggerEnter2D
```

---

## 5. Pre-Merge Checklist

Trước khi merge branch của bạn vào main development branch:

### Code Quality
- [ ] Không có Debug.Log spam (xóa hoặc comment debug logs)
- [ ] Không có TODO comments (hoặc ghi chép trong separate issue)
- [ ] Code theo project namespace (RGame)
- [ ] Không có hardcoded values (dùng Inspector fields hoặc const)
- [ ] Comments đã thêm cho logic phức tạp

### Unity Project
- [ ] Tất cả scenes đã save
- [ ] Tất cả prefabs đã save
- [ ] Không có missing script references (pink scripts)
- [ ] Không có missing prefab references trong scenes
- [ ] ProjectSettings không thay đổi ngoài ý muốn

### Git
- [ ] Commit message theo format
- [ ] Không có large binary files được commit (trừ khi là assets cần thiết)
- [ ] .gitignore rules được tuân theo (không có Library/, Temp/, v.v.)
- [ ] Branch up to date với base branch

### Testing
- [ ] Builds không có lỗi
- [ ] Playable từ start đến finish
- [ ] Không có performance regressions
- [ ] Không có console errors

### Documentation
- [ ] README.md updated (nếu cần)
- [ ] Code comments đã thêm
- [ ] Technical documentation updated (nếu system thay đổi)

---

## 6. Quick Command Reference

```bash
# Git basics
git status                          # Kiểm tra gì đã thay đổi
git diff                            # Xem thay đổi từng dòng
git log --oneline                   # Xem các commits gần đây

# Git workflow
git checkout -b feature/my-feature  # Tạo feature branch
git add .                           # Stage tất cả changes
git commit -m "feat: my feature"    # Commit với message
git push -u origin feature/my-feature  # Push lên remote

# Git fixes
git reset HEAD~1                    # Undo commit cuối (giữ changes)
git checkout -- file.txt            # Loại bỏ changes của file
git stash                           # Tạm thời save changes
git stash pop                       # Khôi phục stashed changes

# Unity meta files
git add -A                          # Add tất cả files BAO GỒM .meta files
```

---

## 7. Unity Shortcuts Hữu Ích

**Editor**:
- `Ctrl + P`: Play/Stop
- `Ctrl + Shift + P`: Pause
- `Ctrl + D`: Duplicate
- `F`: Frame selected object
- `Ctrl + Shift + F`: Frame selected object (Scene view)

**Debugging**:
- `Ctrl + Shift + C`: Mở Console
- `Ctrl + 7`: Mở Profiler
- Right-click Inspector tab → Debug: Xem private fields

---

## Tóm Tắt

**Quy Trình Chuẩn**:
1. Tạo feature branch từ `refactor/folder-and-asset`
2. Thực hiện các thay đổi
3. Test kỹ lưỡng
4. Commit với conventional message format
5. Push lên remote
6. Tạo PR đến `feat/create-prj` (hoặc main branch hiện tại)
7. Giải quyết review feedback
8. Merge khi được approved

**Các Practices Chính**:
- Commit các thay đổi nhỏ, tập trung
- Viết commit messages mô tả rõ
- Test trước khi committing
- Giữ branches ngắn hạn (merge trong vòng 1-2 tuần)
- Pull từ base branch thường xuyên để tránh conflicts

**Bước Tiếp Theo**:
- Xem `10_Huong_Dan_Thuc_Hanh.md` cho detailed tutorials
- Xem `11_Xu_Ly_Su_Co.md` cho debugging help
- Xem Git documentation cho advanced workflows

---

**Kết Thúc Tài Liệu**

<p align="center">
<strong>Lawn Defense: Monsters Out</strong><br>
Quy Trình Làm Việc & Các Task Phổ Biến<br>
Development Workflow Guide
</p>
