# Nhiệm Vụ Đầu Tiên: 10 Bài Tập Thực Hành
## Các Thay Đổi Nhanh để Học Codebase

**Phiên Bản Tài Liệu**: 2.0 (Updated October 2025)
**Bản Gốc**: Vietnamese (Version 1.0)
**Mức Độ**: Beginner
**Thời Gian Cần Thiết**: 1-2 giờ mỗi task
**Yêu Cầu**: Unity đã cài đặt, project đã mở
**Ngôn Ngữ**: Tiếng Việt (Vietnamese)

---

## Giới Thiệu

Tài liệu này cung cấp **10 nhiệm vụ thực hành nhanh** được thiết kế để giúp bạn:
- ✅ Làm quen với codebase
- ✅ Thấy kết quả ngay lập tức từ các thay đổi của bạn
- ✅ Xây dựng tự tin khi thực hiện modifications
- ✅ Học cấu trúc project theo cách thực hành

**Triết Lý**: Học bằng cách thực hiện các thay đổi nhỏ, từng bước với kết quả rõ ràng.

### Trước Khi Bắt Đầu

**Unity Basics Cần Thiết**:
Nếu bạn hoàn toàn mới với Unity, đọc những tài liệu này trước:
- **[00_Unity_Co_Ban.md](00_Unity_Co_Ban.md)** - Unity basics
- **[00_BAT_DAU_TU_DAY.md](00_BAT_DAU_TU_DAY.md)** - Tổng quan project

**Git Best Practice**:
Cho mỗi task, tạo một branch mới:
```bash
git checkout -b task/descriptive-name
# Thực hiện các thay đổi của bạn
git add .
git commit -m "Task 1: Adjusted enemy speed"
```

**Testing Protocol**:
- ✅ Thực hiện MỘT thay đổi nhỏ
- ✅ Test trong Unity Play Mode
- ✅ Xác minh thay đổi hoạt động
- ✅ Commit trước khi chuyển sang task tiếp theo

---

## Task 1: Chạy Game và Kiểm Tra Scene

**Mục Tiêu**: Hiểu cấu trúc game bằng cách chạy nó

**Thời Gian**: 15-20 phút

**Unity Basics Cần**:
- **Hierarchy**: Hiển thị tất cả GameObjects trong scene
- **Inspector**: Hiển thị properties của GameObject được chọn
- **Console**: Hiển thị debug messages và lỗi
- **Play Mode**: Nhấn nút ▶ để chạy game

### Các Bước

**1. Mở Main Menu Scene**
```
File → Open Scene → Assets/_MonstersOut/Scenes/MainMenu.unity
```

**2. Khám Phá Hierarchy**
```
Hierarchy (panel bên trái):
├── Canvas (UI elements)
├── EventSystem (xử lý UI input)
├── Main Camera
├── MenuManager (điều khiển menus)
└── SoundManager (phát audio)
```

**3. Chọn GameManager**
- **Hierarchy** → Click "GameManager"
- **Inspector** → Xem components:
  ```
  [GameManager Script]
  ├── Game Levels: Array của level prefabs
  ├── Player: Reference đến player prefab
  └── Listeners: List (trống lúc bắt đầu)
  ```

**4. Nhấn Play (nút ▶)**
- Quan sát main menu
- Click nút "PLAY"
- Xem level load
- Quan sát Console có lỗi không

**5. Trong Khi Chạy, Chọn Một Enemy**
- **Hierarchy** → Tìm "Enemy(Clone)"
- **Inspector** → Xem:
  ```
  Enemy Component:
  ├── Current Health: (xem nó thay đổi khi bị trúng)
  ├── State: WALK / ATTACK / DEATH
  └── Speed: (điều khiển di chuyển)
  ```

**6. Dừng Play Mode (nút ⏹)**

### Điều Bạn Đã Học

- ✅ Game bắt đầu từ MainMenu scene
- ✅ GameManager điều khiển game flow
- ✅ Enemies được spawn lúc runtime (hậu tố Clone)
- ✅ Inspector hiển thị giá trị live trong Play Mode

### Vấn Đề Phổ Biến

**Vấn đề**: Không tìm thấy GameManager
**Giải pháp**: Tìm trong Hierarchy, không phải Project. Nó ở trong scene.

**Vấn đề**: Console hiển thị lỗi màu đỏ
**Giải pháp**: Điều này bình thường nếu bạn chưa gán tất cả Inspector fields. Tiếp tục.

### Bước Tiếp Theo

Bây giờ bạn biết cách chạy và kiểm tra game. Hãy thực hiện một số thay đổi!

---

## Task 2: Chỉnh Sửa Tốc Độ Enemy

**Mục Tiêu**: Thay đổi tốc độ di chuyển enemy và xem hiệu quả

**Thời Gian**: 10-15 phút

**Unity Basics Cần**:
- **Prefab**: Template để tạo GameObjects
- **Inspector**: Chỉnh sửa giá trị component
- **Play Mode**: Test thay đổi

### Các Bước

**1. Tìm Enemy Prefab**
```
Project → Assets/_MonstersOut/Prefabs/Enemies/
Click: Goblin.prefab
```

**2. Xem trong Inspector**
```
[SmartEnemyGrounded Component]
├── Speed: 3          ← Giá trị hiện tại
└── Attack Distance: 1.5
```

**3. Thay Đổi Giá Trị Speed**
```
Speed: 3  →  Speed: 6  (gấp đôi tốc độ)
```

**4. Test Thay Đổi**
- Nhấn **Play** (▶)
- Load một level
- **Quan sát**: Enemies giờ di chuyển nhanh gấp đôi!

**5. Thử Nghiệm**
Thử các giá trị khác nhau:
- `Speed: 1` → Rất chậm (chế độ dễ)
- `Speed: 10` → Rất nhanh (chế độ khó)
- `Speed: 0` → Đóng băng (tuyệt vời cho debugging!)

### Hiểu Code

Giá trị speed được sử dụng trong `SmartEnemyGrounded.cs`:

```csharp
// Dòng ~85 trong SmartEnemyGrounded.cs
void Update()
{
    if (State == ENEMYSTATE.WALK)
    {
        // Di chuyển về phía player
        velocity.x = direction * speed; // ← 'speed' từ Inspector
        controller.Move(velocity * Time.deltaTime);
    }
}
```

**Giải thích**:
- `speed` được nhân với `direction` (-1 cho trái, 1 cho phải)
- `Time.deltaTime` làm nó độc lập với framerate
- Speed cao hơn = nhiều units mỗi giây hơn

### Điều Bạn Đã Học

- ✅ Prefabs lưu giá trị mặc định cho GameObjects
- ✅ Thay đổi prefabs ảnh hưởng đến tất cả instances
- ✅ Giá trị Inspector trực tiếp điều khiển hành vi code

### Vấn Đề Phổ Biến

**Vấn đề**: Thay đổi không xuất hiện trong game
**Giải pháp**:
1. Đảm bảo bạn chỉnh sửa **prefab** (icon màu xanh), không phải scene instance
2. Dừng và khởi động lại Play Mode
3. Kiểm tra xem bạn đã save prefab chưa (Ctrl+S)

**Vấn đề**: Enemy di chuyển quá nhanh và rơi xuyên sàn
**Giải pháp**: Speed > 15 có thể gây vấn đề physics. Giữ dưới 10.

### Thử Điều Này

Chỉnh sửa các giá trị enemy khác:
- **Attack Distance**: Gần đến mức nào trước khi tấn công
- **Max Health**: Nhận bao nhiêu damage trước khi chết
- **Damage**: Enemy gây bao nhiêu damage

### Tài Liệu Liên Quan

- **[03_He_Thong_Enemy.md](03_He_Thong_Enemy.md)** - Tài liệu enemy hoàn chỉnh
- **[12_Visual_Reference.md](12_Visual_Reference.md)** - Sơ đồ state machine

---

## Task 3: Tạo Enemy Mới (Phương Pháp Duplicate)

**Mục Tiêu**: Tạo biến thể enemy tùy chỉnh bằng cách duplicate prefab có sẵn

**Thời Gian**: 20-30 phút

**Unity Basics Cần**:
- **Prefab Duplication**: Sao chép templates có sẵn
- **Sprite**: Diện mạo trực quan
- **Animator**: Animation controller

### Các Bước

**1. Duplicate Goblin Prefab**
```
Project → Prefabs/Enemies/
Right-click: Goblin.prefab → Duplicate
Đổi tên: FastGoblin
```

**2. Chỉnh Sửa Stats**
Chọn `FastGoblin.prefab` → Inspector:
```
[Enemy Component]
Max Health: 50      → 30   (ít máu hơn)
Damage: 10          → 15   (nhiều damage hơn)

[SmartEnemyGrounded Component]
Speed: 3            → 6    (di chuyển nhanh hơn)
```

**3. Thay Đổi Diện Mạo (Optional)**
```
[Sprite Renderer Component]
Sprite: Goblin-Idle_00  → Skeleton-Idle_00
Color: White            → Red (tint)
```

**4. Test Custom Enemy Của Bạn**

Option A: **Thay Thế trong Wave**
```
Hierarchy → LevelEnemyManager
Inspector → Waves → Wave 0
Enemy Prefab: Goblin  →  FastGoblin
```

Option B: **Spawn Thủ Công**
```
Hierarchy → Right-click → Create Empty
Kéo: FastGoblin prefab vào Hierarchy
Nhấn Play → Xem enemy của bạn!
```

**5. So Sánh Hành Vi**
- Fast Goblin di chuyển nhanh hơn
- Chết nhanh hơn (ít máu hơn)
- Gây nhiều damage hơn

### Hiểu Prefab Workflow

```
Original Prefab (Goblin)
        │
        ▼
    Duplicate
        │
        ▼
  New Prefab (FastGoblin) ← Chỉnh sửa cái này
        │
        ▼
   Spawn trong Game
```

**Khái Niệm Chính**: Thay đổi FastGoblin không ảnh hưởng đến Goblin.

### Nâng Cao: Tạo Enemy Hoàn Toàn Mới

**Nếu bạn muốn animations khác nhau**:

1. Tạo Animator Controller mới:
   ```
   Project → Animations → Right-click
   → Create → Animator Controller
   Đặt tên: FastGoblin_Controller
   ```

2. Thêm animation states (sao chép từ Goblin_Controller)

3. Gán vào prefab:
   ```
   FastGoblin → Animator Component
   Controller: FastGoblin_Controller
   ```

### Điều Bạn Đã Học

- ✅ Duplication là cách nhanh nhất để tạo variants
- ✅ Bạn có thể mix-and-match sprites và stats
- ✅ Prefabs có thể test bằng cách kéo vào Hierarchy

### Vấn Đề Phổ Biến

**Vấn đề**: Enemy duplicate sử dụng sprite sai
**Giải pháp**: Kiểm tra component Sprite Renderer, gán sprite đúng

**Vấn đề**: Animations không chạy
**Giải pháp**: Đảm bảo Animator Controller được gán và có states

**Vấn đề**: Enemy không spawn trong level
**Giải pháp**: Gán vào LevelEnemyManager wave settings

### Thử Điều Này

Tạo các enemy variants này:
- **Tank**: Máu cao (200), chậm (speed: 1.5)
- **Assassin**: Máu thấp (20), rất nhanh (speed: 8)
- **Boss**: Máu khổng lồ (500), tốc độ trung bình (4), damage cao (30)

### Tài Liệu Liên Quan

- **[10_Huong_Dan_Thuc_Hanh.md](10_Huong_Dan_Thuc_Hanh.md)** § Guide 1 - Tạo loại enemy mới
- **[03_He_Thong_Enemy.md](03_He_Thong_Enemy.md)** - Chi tiết enemy system

---

## Task 4: Tăng Projectile Damage

**Mục Tiêu**: Làm cho arrows của player gây nhiều damage hơn

**Thời Gian**: 10 phút

**Unity Basics Cần**:
- **Script Editing**: Mở và chỉnh sửa C# code
- **Component Reference**: Tìm scripts trên GameObjects

### Các Bước

**1. Tìm Arrow Prefab**
```
Project → Prefabs/Projectiles/
Click: Arrow.prefab
```

**2. Xem Damage Script**
```
Inspector → ArrowProjectile (Script)
Damage: 10  ← Damage hiện tại
```

**3. Tăng Damage**
```
Damage: 10  →  Damage: 25
```

**4. Test**
- Nhấn Play
- Bắn enemies
- **Chú ý**: Chúng chết nhanh hơn!

### Hiểu Damage System

**Arrow.cs** (đơn giản hóa):
```csharp
public class ArrowProjectile : MonoBehaviour
{
    public float damage = 10f;  // ← Giá trị Inspector

    void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra xem có trúng enemy không
        ICanTakeDamage target = other.GetComponent<ICanTakeDamage>();

        if (target != null)
        {
            // Gây damage
            target.TakeDamage(damage, Vector2.zero, gameObject);

            // Destroy arrow
            Destroy(gameObject);
        }
    }
}
```

**Flow**:
```
Arrow spawns → Bay qua không trung → Trúng enemy →
OnTriggerEnter2D() được gọi → TakeDamage(25) → Enemy mất 25 HP
```

### Code Explanation (Từng Dòng)

```csharp
public float damage = 10f;
```
- `public` = hiển thị trong Inspector
- `float` = số thập phân (10.5 được phép)
- `damage` = tên biến
- `= 10f` = giá trị mặc định (10.0)

```csharp
void OnTriggerEnter2D(Collider2D other)
```
- Được gọi tự động khi collider của arrow chạm collider khác
- `other` = thứ chúng ta trúng

```csharp
ICanTakeDamage target = other.GetComponent<ICanTakeDamage>();
```
- Tìm damage interface trên hit object
- Trả về null nếu object không thể nhận damage (như tường)

```csharp
if (target != null)
```
- Chỉ tiếp tục nếu chúng ta trúng thứ có thể nhận damage

```csharp
target.TakeDamage(damage, Vector2.zero, gameObject);
```
- Gọi method TakeDamage
- `damage` = bao nhiêu (25 trong trường hợp của chúng ta)
- `Vector2.zero` = không có knockback force
- `gameObject` = ai gây damage (arrow)

### Điều Bạn Đã Học

- ✅ Giá trị Inspector là các biến code
- ✅ Damage được gây qua interface `ICanTakeDamage`
- ✅ Collision triggers kích hoạt code execution

### Vấn Đề Phổ Biến

**Vấn đề**: Damage không thay đổi
**Giải pháp**: Đảm bảo bạn chỉnh sửa **Arrow prefab**, không phải instance trong scene

**Vấn đề**: Arrows bay xuyên qua enemies
**Giải pháp**:
1. Kiểm tra Arrow có **Collider2D** với **Is Trigger** được check
2. Kiểm tra Enemy có **Collider2D**
3. Xác minh **Layer Collision Matrix** (Edit → Project Settings → Physics 2D)

### Thử Điều Này

Chỉnh sửa các properties projectile khác:
- **Speed**: Arrow bay nhanh thế nào
- **Lifetime**: Bao lâu trước khi arrow despawns
- **Pierce**: Arrow có thể trúng nhiều enemies không?

### Tài Liệu Liên Quan

- **[13_Code_Examples.md](13_Code_Examples.md)** § Example 7 - Damage Dealer
- **[11_Xu_Ly_Su_Co.md](11_Xu_Ly_Su_Co.md)** § Problem 8 - Damage không được áp dụng

---

## Task 5: Thêm Character Upgrade Level

**Mục Tiêu**: Thêm một tier nâng cấp mới cho player character

**Thời Gian**: 15-20 phút

**Unity Basics Cần**:
- **Serializable Classes**: Cấu trúc dữ liệu hiển thị trong Inspector
- **Arrays**: Danh sách các giá trị

### Các Bước

**1. Tìm Player Prefab**
```
Project → Prefabs/Player/
Click: Player.prefab
```

**2. Tìm Upgrade Component**
```
Inspector → Tìm: UpgradedCharacterParameter
```

**3. Xem Upgrades Hiện Tại**
```
[Upgrade Steps]
Size: 3  ← Hiện tại 3 levels

Element 0: (Level 1)
├── Cost: 100
├── Health: 100
└── Damage: 10

Element 1: (Level 2)
├── Cost: 250
├── Health: 150
└── Damage: 15

Element 2: (Level 3)
├── Cost: 500
├── Health: 200
└── Damage: 20
```

**4. Thêm Upgrade Level Mới**
```
Size: 3  →  Size: 4  (thêm Element 3)
```

**5. Cấu Hình Level 4**
```
Element 3: (Level 4)
├── Cost: 1000       ← Đắt!
├── Health: 300      ← Max health
└── Damage: 30       ← Tấn công mạnh
```

**6. Test trong Shop**
- Play game
- Mở shop
- Mua upgrades
- Đạt Level 4!

### Hiểu Upgrade System

**Cấu trúc class UpgradeStep**:
```csharp
[System.Serializable]
public class UpgradeStep
{
    public int cost;       // ← Giá mua
    public float health;   // ← Max HP ở level này
    public float damage;   // ← Attack damage
    // ... các stats khác
}
```

**Quá trình upgrade**:
```
Player ở Level 1 (100 HP, 10 Damage)
        │
        ├─ Chi 250 coins trong shop
        │
        ▼
Player ở Level 2 (150 HP, 15 Damage)
        │
        ├─ Chi 500 coins
        │
        ▼
Player ở Level 3 (200 HP, 20 Damage)
        │
        ├─ Chi 1000 coins
        │
        ▼
Player ở Level 4 (300 HP, 30 Damage)  ← Level mới của bạn!
```

### Code Đằng Sau Upgrades

**Khi player mua upgrade** (đơn giản hóa):
```csharp
public void UpgradeCharacter()
{
    // Lấy upgrade level hiện tại
    int currentLevel = GlobalValue.CharacterLevel;

    // Lấy stats upgrade tiếp theo
    UpgradeStep nextStep = upgradeSteps[currentLevel];

    // Kiểm tra có đủ coins không
    if (GlobalValue.SavedCoins >= nextStep.cost)
    {
        // Trừ coins
        GlobalValue.SavedCoins -= nextStep.cost;

        // Áp dụng stats mới
        maxHealth = nextStep.health;
        damage = nextStep.damage;

        // Tăng level
        GlobalValue.CharacterLevel++;

        // Save
        PlayerPrefs.Save();
    }
}
```

### Điều Bạn Đã Học

- ✅ Arrays lưu nhiều giá trị (upgrade tiers)
- ✅ `[Serializable]` làm classes hiển thị trong Inspector
- ✅ Upgrades là vĩnh viễn (saved trong PlayerPrefs)

### Vấn Đề Phổ Biến

**Vấn đề**: Không thấy Level 4 trong shop
**Giải pháp**: Bạn cần đủ coins. Dùng PlayerPrefs để thêm coins:
```
Unity Menu → Window → PlayerPrefs Editor (nếu đã cài)
Hoặc thủ công: PlayerPrefs.SetInt("coins", 10000);
```

**Vấn đề**: Stats không thay đổi sau upgrade
**Giải pháp**: Đảm bảo Player script đọc từ upgradeSteps array

### Thử Điều Này

Tạo một progression system:
```
Level 1: Beginner (100 HP, 10 DMG) - Cost: FREE
Level 2: Apprentice (150 HP, 15 DMG) - Cost: 250
Level 3: Warrior (200 HP, 20 DMG) - Cost: 500
Level 4: Knight (300 HP, 30 DMG) - Cost: 1000
Level 5: Legend (500 HP, 50 DMG) - Cost: 5000
```

### Tài Liệu Liên Quan

- **[13_Code_Examples.md](13_Code_Examples.md)** § Example 17 - Save/Load Systems
- **[02_He_Thong_Player.md](02_He_Thong_Player.md)** § Upgrade System

---

## Task 6: Thêm Animation Event

**Mục Tiêu**: Kích hoạt code từ animation timeline

**Thời Gian**: 20-25 phút

**Unity Basics Cần**:
- **Animation Window**: Chỉnh sửa animation clips
- **Animation Events**: Gọi functions tại các frames cụ thể

### Các Bước

**1. Mở Enemy Animator**
```
Project → Animations/Enemies/
Double-click: Goblin_Controller
```

**2. Mở Animation Window**
```
Window → Animation → Animation
```

**3. Chọn Attack Animation**
```
Animation Window → Dropdown → "Goblin-Attack"
```

**4. Thêm Animation Event**
```
Timeline → Click frame 10 (mid-swing)
Button: Add Event (hoặc white marker line)
```

**5. Gán Function**
```
Function: DealDamage()  ← Method từ Enemy script
```

**6. Thêm Debug Log**

Mở `Assets/_MonstersOut/Scripts/Enemy/EnemyMeleeAttack.cs`:

```csharp
public void DealDamage()
{
    // Thêm dòng này ở đầu
    Debug.Log(name + " dealing damage at frame 10!");

    // Code damage có sẵn...
    if (AttackTargetPlayer && player != null)
    {
        player.TakeDamage(damage, Vector2.zero, gameObject);
    }
}
```

**7. Test**
- Play game
- Đợi enemy tấn công
- **Console** hiển thị: "Goblin(Clone) dealing damage at frame 10!"

### Hiểu Animation Events

**Animation Timeline**:
```
Attack Animation (30 frames, 1 giây)

Frame:  0     5     10    15    20    25    30
        │     │     │     │     │     │     │
        ▼     ▼     ▼     ▼     ▼     ▼     ▼
Sprite: idle  windup SWING  hit  follow  follow idle
                      ▲
                      │
                 Event: DealDamage()
                 (damage được áp dụng ở đây!)
```

**Tại sao dùng events?**
- Đồng bộ code với animation
- Damage tại thời điểm trực quan đúng
- Sounds chạy khi chân chạm đất
- Particles spawn khi kiếm vung

### Code Explanation

```csharp
// Method này được gọi bởi Animation Event
public void DealDamage()
{
    Debug.Log(name + " dealing damage!");

    if (AttackTargetPlayer && player != null)
    {
        player.TakeDamage(damage, Vector2.zero, gameObject);
    }
}
```

**Flow**:
```
Enemy bắt đầu attack animation →
Frame 10 đạt được →
Unity gọi DealDamage() tự động →
Code chạy →
Player nhận damage
```

### Điều Bạn Đã Học

- ✅ Animation có thể kích hoạt code tại các frames cụ thể
- ✅ Events đồng bộ visuals với gameplay
- ✅ Methods phải public để được gọi bởi events

### Vấn Đề Phổ Biến

**Vấn đề**: Function không xuất hiện trong dropdown
**Giải pháp**:
1. Method phải là `public`
2. Method phải không có parameters (hoặc chỉ các types cụ thể)
3. Script phải được gắn vào cùng GameObject với Animator

**Vấn đề**: Event kích hoạt quá sớm/muộn
**Giải pháp**: Kéo event marker đến frame khác

**Vấn đề**: Event không kích hoạt
**Giải pháp**:
1. Kiểm tra Animator đang chạy animation
2. Xác minh script được gắn vào GameObject
3. Đảm bảo tên method khớp chính xác

### Thử Điều Này

Thêm nhiều animation events hơn:
- **Footstep Sound**: Phát sound khi chân chạm đất
- **Spawn Effect**: Tạo particles tại thời điểm tấn công
- **Shake Camera**: Lắc khi heavy attack hạ xuống

### Tài Liệu Liên Quan

- **[12_Visual_Reference.md](12_Visual_Reference.md)** § Animation System
- **[03_He_Thong_Enemy.md](03_He_Thong_Enemy.md)** § Attack System

---

## Task 7: Test Shop System

**Mục Tiêu**: Hiểu shop mechanics bằng cách test purchases

**Thời Gian**: 15-20 phút

**Unity Basics Cần**:
- **UI Navigation**: Tìm và tương tác với UI
- **PlayerPrefs**: Save data system

### Các Bước

**1. Mở Shop Scene**
```
File → Open Scene → Scenes/Shop.unity
(Hoặc play game và điều hướng đến shop)
```

**2. Tự Thêm Coins**

**Method A**: Qua Inspector (trong Play Mode)
```
Play Mode → Hierarchy → Tìm object với GlobalValue
Inspector → Saved Coins: 0  →  10000
```

**Method B**: Qua PlayerPrefs (trước Play Mode)

Tạo script tạm thời này:
```csharp
using UnityEngine;

public class CheatCoins : MonoBehaviour
{
    void Start()
    {
        PlayerPrefs.SetInt("coins", 10000);
        PlayerPrefs.Save();
        Debug.Log("Added 10000 coins!");
    }
}
```

Gắn vào bất kỳ GameObject nào, play một lần, sau đó xóa script.

**3. Mua Character**
```
Shop UI → Click character slot
Button: "Buy" (giá hiển thị)
Quan sát: Coins trừ, character unlocked
```

**4. Quan Sát Floating Text**
```
Khi mua: "Purchased!" text bay lên
Khi không đủ: "Not enough coins!" hiển thị
```

**5. Test Character trong Game**
```
Quay về main menu
Chọn character đã mở khóa
Play level
Character loads với stats đã mua!
```

### Hiểu Shop System

**Shop UI Hierarchy**:
```
Canvas
└── ShopPanel
    ├── CharacterSlot1
    │   ├── Character Image
    │   ├── Price Text
    │   └── Buy Button
    │       └── OnClick: ShopManager.BuyCharacter(0)
    │
    ├── CharacterSlot2
    └── CoinDisplay
        └── Text: Hiển thị GlobalValue.SavedCoins
```

**Purchase Flow**:
```
Player clicks "Buy" →
ShopManager.BuyCharacter(characterID) được gọi →
Kiểm tra: coins >= price? →
  YES → Trừ coins
     → Đặt character là owned
     → Save PlayerPrefs
     → Hiển thị "Purchased!" text
  NO  → Hiển thị "Not enough coins!" text
```

### Code Đằng Sau Shop (Đơn Giản Hóa)

```csharp
public class ShopManager : MonoBehaviour
{
    public void BuyCharacter(int characterID)
    {
        // Lấy character data
        CharacterData character = characters[characterID];

        // Kiểm tra đủ coins không
        if (GlobalValue.SavedCoins >= character.price)
        {
            // Trừ coins
            GlobalValue.SavedCoins -= character.price;

            // Đánh dấu là owned
            PlayerPrefs.SetInt("Character_" + characterID + "_Owned", 1);

            // Save
            PlayerPrefs.Save();

            // Hiển thị feedback
            FloatingTextManager.Instance.Show("Purchased!", Color.green);
        }
        else
        {
            // Không đủ
            FloatingTextManager.Instance.Show("Not enough coins!", Color.red);
        }
    }
}
```

### Điều Bạn Đã Học

- ✅ Shop sử dụng PlayerPrefs cho save data
- ✅ Floating text cung cấp user feedback
- ✅ UI buttons gọi script methods qua OnClick events

### Vấn Đề Phổ Biến

**Vấn đề**: Coins không tồn tại
**Giải pháp**: Đảm bảo `PlayerPrefs.Save()` được gọi

**Vấn đề**: Character đã mua không hiển thị trong game
**Giải pháp**: Kiểm tra character selection logic loads owned characters

**Vấn đề**: Buy button không hoạt động
**Giải pháp**: Xác minh OnClick event được gán trong Inspector

### Thử Điều Này

Chỉnh sửa hành vi shop:
- **Discounts**: Giảm giá 50%
- **Double Coins**: Cho 2x coins khi hoàn thành levels
- **New Currency**: Thêm gems làm premium currency

### Tài Liệu Liên Quan

- **[04_He_Thong_UI.md](04_He_Thong_UI.md)** § Shop System
- **[13_Code_Examples.md](13_Code_Examples.md)** § Example 17 - Save/Load

---

## Task 8: Thêm Debug Logs vào Wave Spawner

**Mục Tiêu**: Hiểu wave system bằng cách thêm logging

**Thời Gian**: 10 phút

**Unity Basics Cần**:
- **Debug.Log**: In messages ra Console
- **Coroutines**: Thực thi code dựa trên thời gian

### Các Bước

**1. Mở LevelEnemyManager Script**
```
Assets/_MonstersOut/Scripts/Managers/LevelEnemyManager.cs
```

**2. Tìm SpawnEnemyCo() Method**

Khoảng dòng 60-80, tìm method này.

**3. Thêm Debug Logs**

```csharp
IEnumerator SpawnEnemyCo()
{
    Debug.Log("=== WAVE SPAWNING STARTED ===");

    int totalSpawned = 0;

    foreach (var wave in waves)
    {
        Debug.Log("Starting wave: " + wave.waveName +
                 " with " + wave.enemyCount + " enemies");

        for (int i = 0; i < wave.enemyCount; i++)
        {
            // Spawn enemy
            SpawnEnemy(wave.enemyPrefab);
            totalSpawned++;

            Debug.Log("Spawned enemy #" + totalSpawned +
                     " (Wave: " + wave.waveName + ")");

            yield return new WaitForSeconds(wave.spawnInterval);
        }

        Debug.Log("Wave " + wave.waveName + " complete!");
        yield return new WaitForSeconds(wave.delayToNextWave);
    }

    Debug.Log("=== ALL WAVES COMPLETE! Total spawned: " + totalSpawned + " ===");
}
```

**4. Test**
- Play game
- Load level
- **Xem Console**: Hiển thị tiến trình spawn!

**Ví dụ Output**:
```
=== WAVE SPAWNING STARTED ===
Starting wave: Wave 1 with 5 enemies
Spawned enemy #1 (Wave: Wave 1)
Spawned enemy #2 (Wave: Wave 1)
Spawned enemy #3 (Wave: Wave 1)
Spawned enemy #4 (Wave: Wave 1)
Spawned enemy #5 (Wave: Wave 1)
Wave Wave 1 complete!
Starting wave: Wave 2 with 8 enemies
...
```

### Hiểu Wave System

**Wave Configuration**:
```csharp
[System.Serializable]
public class Wave
{
    public string waveName;           // Tên hiển thị
    public GameObject enemyPrefab;    // Cái gì để spawn
    public int enemyCount;            // Bao nhiêu
    public float spawnInterval;       // Thời gian giữa mỗi (1-3 sec)
    public float delayToNextWave;     // Nghỉ giữa waves (5-10 sec)
}
```

**Timeline Visualization**:
```
Thời gian: 0s ─────5s ─────10s ────15s ────20s ────25s ────30s ───→

Wave 1: Enemy Enemy Enemy Enemy Enemy ──(chờ 5s)──→
                  ▲
                  spawnInterval (1s giữa mỗi)

Wave 2: Enemy Enemy Enemy ──(chờ 5s)──→
                  ▲
                  spawnInterval (2s)
```

### Code Explanation

```csharp
foreach (var wave in waves)
```
- Lặp qua tất cả waves theo thứ tự

```csharp
for (int i = 0; i < wave.enemyCount; i++)
```
- Spawn `enemyCount` enemies mỗi wave

```csharp
yield return new WaitForSeconds(wave.spawnInterval);
```
- Đợi trước khi spawn enemy tiếp theo
- `yield return` tạm dừng coroutine

```csharp
Debug.Log("Message: " + variable);
```
- In ra Console
- Dùng `+` để kết hợp strings và numbers

### Điều Bạn Đã Học

- ✅ Debug.Log giúp hiểu code flow
- ✅ Waves spawn enemies tuần tự với delays
- ✅ Coroutines xử lý spawning dựa trên thời gian

### Vấn Đề Phổ Biến

**Vấn đề**: Console ngập messages
**Giải pháp**: Xóa các lời gọi Debug.Log() sau khi hiểu system

**Vấn đề**: Logs không xuất hiện
**Giải pháp**:
1. Kiểm tra Console hiển thị (Window → General → Console)
2. Đảm bảo code thực sự đang chạy (thêm breakpoint)

### Thử Điều Này

Log thêm thông tin:
- **Enemy Health**: Log khi enemy nhận damage
- **Player Actions**: Log khi player bắn
- **Victory/Defeat**: Log các điều kiện kết thúc game

### Tài Liệu Liên Quan

- **[05_Cac_Manager.md](05_Cac_Manager.md)** § LevelEnemyManager
- **[11_Xu_Ly_Su_Co.md](11_Xu_Ly_Su_Co.md)** § Debug Techniques

---

## Task 9: Chỉnh Sửa Giới Hạn Di Chuyển Camera

**Mục Tiêu**: Thay đổi ranh giới camera

**Thời Gian**: 10 phút

**Unity Basics Cần**:
- **Camera**: Những gì player nhìn thấy
- **Transform**: Vị trí trong thế giới

### Các Bước

**1. Tìm Main Camera**
```
Hierarchy → Main Camera
```

**2. Xem Camera Controller**
```
Inspector → CameraController (Script)
├── Limit Left: -5     ← Ranh giới trái
├── Limit Right: 5     ← Ranh giới phải
└── Smooth Speed: 3    ← Độ mượt theo dõi
```

**3. Mở Rộng Camera Range**
```
Limit Left: -5   →  -10   (có thể thấy xa hơn bên trái)
Limit Right: 5   →  15    (có thể thấy xa hơn bên phải)
```

**4. Test**
- Play game
- Kéo màn hình trái/phải (kéo chuột hoặc touch)
- Camera di chuyển trong giới hạn mới!

### Hiểu Camera System

**Camera Clamping**:
```
                Limit Left          Limit Right
                    │                   │
                    ▼                   ▼
World: ────────────[-10]═══════════[15]────────────
                    ◄─────────────►
                    Camera có thể di chuyển
                    trong phạm vi này
```

**Code Đằng Sau Camera Movement**:
```csharp
void Update()
{
    // Lấy vị trí mong muốn (từ player hoặc drag)
    float desiredX = GetDesiredCameraX();

    // Clamp về giới hạn
    float clampedX = Mathf.Clamp(
        desiredX,
        limitLeft,     // Không thể đi xa hơn bên trái
        limitRight     // Không thể đi xa hơn bên phải
    );

    // Di chuyển mượt
    Vector3 targetPos = new Vector3(clampedX, transform.position.y, -10);
    transform.position = Vector3.Lerp(
        transform.position,
        targetPos,
        smoothSpeed * Time.deltaTime
    );
}
```

### Code Explanation

```csharp
Mathf.Clamp(value, min, max)
```
- Giới hạn value giữa min và max
- Ví dụ: `Clamp(12, -10, 15)` → 12 (trong phạm vi)
- Ví dụ: `Clamp(-20, -10, 15)` → -10 (clamped về min)

```csharp
Vector3.Lerp(current, target, speed * Time.deltaTime)
```
- Di chuyển mượt từ current đến target
- `speed * Time.deltaTime` = di chuyển dần dần
- Speed cao hơn = theo dõi nhanh hơn

### Điều Bạn Đã Học

- ✅ Giới hạn camera ngăn nhìn thấy ngoài level
- ✅ Mathf.Clamp giới hạn giá trị trong phạm vi
- ✅ Lerp tạo di chuyển camera mượt mà

### Vấn Đề Phổ Biến

**Vấn đề**: Camera hiển thị không gian trống
**Giải pháp**: Đặt giới hạn khớp với kích thước level

**Vấn đề**: Camera quá giật
**Giải pháp**: Tăng `smoothSpeed` (thử 5-10)

**Vấn đề**: Không thể kéo camera
**Giải pháp**: Xác minh input handling code đang active

### Thử Điều Này

Các hành vi camera nâng cao:
- **Follow Player**: Làm camera theo dõi vị trí player
- **Zoom Control**: Thay đổi Camera.orthographicSize
- **Shake Effect**: Thêm offset ngẫu nhiên khi nhận damage

### Tài Liệu Liên Quan

- **[13_Code_Examples.md](13_Code_Examples.md)** § Camera Systems
- **[12_Visual_Reference.md](12_Visual_Reference.md)** § Scene Structure

---

## Task 10: Implement Double-Shot Power-Up

**Mục Tiêu**: Tạo một feature từ đầu - player bắn hai arrows

**Thời Gian**: 30-40 phút

**Unity Basics Cần**:
- **Script Modification**: Chỉnh sửa C# code có sẵn
- **Quaternion**: Rotation system
- **Boolean Flags**: Biến true/false

### Các Bước

**1. Mở Player Script**
```
Assets/_MonstersOut/Scripts/Player/Player_Archer.cs
```

**2. Thêm Double Shot Variable**

Khoảng dòng 20-30, thêm:
```csharp
[Header("Power-Ups")]
public bool hasDoubleShot = false;  // ← Enable trong Inspector để testing
public float doubleShotAngle = 15f; // ← Góc spread
```

**3. Tìm Shoot() Method**

Khoảng dòng 100-150, tìm shooting code.

**4. Chỉnh Sửa Shoot() Method**

```csharp
void Shoot()
{
    if (!allowMoveByPlayer)
        return;

    if (Time.time - lastShootTime < shootRate)
        return; // Cooldown chưa sẵn sàng

    lastShootTime = Time.time;

    if (hasDoubleShot)
    {
        // Bắn hai arrows với góc spread
        ShootArrowWithAngle(-doubleShotAngle); // Arrow trái
        ShootArrowWithAngle(doubleShotAngle);  // Arrow phải
    }
    else
    {
        // Bắn đơn bình thường
        ShootArrowWithAngle(0); // Thẳng
    }

    // Phát sound
    SoundManager.PlaySfx(SoundManager.Instance.soundShoot);

    // Chạy animation
    animator.SetTrigger("Shoot");
}

// Helper method mới
void ShootArrowWithAngle(float angleOffset)
{
    GameObject arrow = Instantiate(
        arrowPrefab,
        shootPoint.position,
        Quaternion.identity
    );

    // Tính toán hướng với góc
    Vector2 baseDirection = transform.localScale.x > 0
        ? Vector2.right
        : Vector2.left;

    // Áp dụng angle offset
    float angleRad = angleOffset * Mathf.Deg2Rad;
    float cos = Mathf.Cos(angleRad);
    float sin = Mathf.Sin(angleRad);

    Vector2 rotatedDirection = new Vector2(
        baseDirection.x * cos - baseDirection.y * sin,
        baseDirection.x * sin + baseDirection.y * cos
    );

    // Đặt hướng arrow
    ArrowScript arrowScript = arrow.GetComponent<ArrowScript>();
    if (arrowScript != null)
    {
        arrowScript.Initialize(rotatedDirection);
    }
}
```

**5. Test**

```
Inspector → Player GameObject
→ Player_Archer Component
→ Has Double Shot: ☑ (check cái này!)
```

Play game → Bắn → Hai arrows spread ra!

### Kết Quả Trực Quan

**Single Shot**:
```
        →
Player ───→ Enemy
```

**Double Shot**:
```
       ↗
Player ─→  Enemy
       ↘
```

### Hiểu Math

**Công Thức Xoay Góc**:
```csharp
// Xoay vector theo góc
rotatedX = x * cos(angle) - y * sin(angle)
rotatedY = x * sin(angle) + y * cos(angle)
```

**Ví dụ**: Xoay (1, 0) theo 15°:
```
cos(15°) = 0.966
sin(15°) = 0.259

rotatedX = 1 * 0.966 - 0 * 0.259 = 0.966
rotatedY = 1 * 0.259 + 0 * 0.966 = 0.259

Kết quả: (0.966, 0.259) → Nghiêng 15° lên trên
```

### Điều Bạn Đã Học

- ✅ Tạo feature mới từ code có sẵn
- ✅ Sử dụng trigonometry cho tính toán góc
- ✅ Boolean flags bật/tắt features

### Vấn Đề Phổ Biến

**Vấn đề**: Arrows không spread
**Giải pháp**: Tăng `doubleShotAngle` (thử 30°)

**Vấn đề**: Arrows bắn sai hướng
**Giải pháp**: Kiểm tra tính toán `baseDirection` dựa trên player facing

**Vấn đề**: Chỉ một arrow spawn
**Giải pháp**: Đảm bảo `hasDoubleShot` được check trong Inspector

### Thử Điều Này

Mở rộng feature này:
- **Triple Shot**: Bắn 3 arrows (trái, giữa, phải)
- **Powerup Duration**: Bật trong 10 giây, sau đó tắt
- **Rapid Fire**: Giảm `shootRate` khi power-up active

### Nâng Cao: Làm Nó Collectible

**1. Tạo Powerup Prefab**:
```
GameObject → 3D Object → Cube
Thêm: BoxCollider2D (Is Trigger: ✓)
Thêm: Script: DoubleShotPowerup.cs
```

**2. Powerup Script**:
```csharp
public class DoubleShotPowerup : MonoBehaviour
{
    public float duration = 10f;

    void OnTriggerEnter2D(Collider2D other)
    {
        Player_Archer player = other.GetComponent<Player_Archer>();

        if (player != null)
        {
            player.ActivateDoubleShot(duration);
            Destroy(gameObject);
        }
    }
}
```

**3. Thêm vào Player**:
```csharp
public void ActivateDoubleShot(float duration)
{
    StartCoroutine(DoubleShotCo(duration));
}

IEnumerator DoubleShotCo(float duration)
{
    hasDoubleShot = true;
    Debug.Log("Double shot activated!");

    yield return new WaitForSeconds(duration);

    hasDoubleShot = false;
    Debug.Log("Double shot expired!");
}
```

### Tài Liệu Liên Quan

- **[10_Huong_Dan_Thuc_Hanh.md](10_Huong_Dan_Thuc_Hanh.md)** § Guide 6 - Power-Up Items
- **[02_He_Thong_Player.md](02_He_Thong_Player.md)** § Shooting System
- **[13_Code_Examples.md](13_Code_Examples.md)** § Math Utilities

---

## Tóm Tắt & Bước Tiếp Theo

### Những Gì Bạn Đã Hoàn Thành

✅ **Task 1**: Khám phá cấu trúc game và hierarchy
✅ **Task 2**: Chỉnh sửa tốc độ enemy (giá trị prefab)
✅ **Task 3**: Tạo biến thể enemy tùy chỉnh
✅ **Task 4**: Tăng projectile damage
✅ **Task 5**: Thêm character upgrade tier
✅ **Task 6**: Kích hoạt code từ animations
✅ **Task 7**: Test shop và save system
✅ **Task 8**: Thêm debug logging vào spawner
✅ **Task 9**: Chỉnh sửa ranh giới camera
✅ **Task 10**: Implement double-shot feature

### Kỹ Năng Đạt Được

**Unity Editor**:
- Điều hướng Hierarchy, Inspector, Project
- Chỉnh sửa giá trị prefab
- Test trong Play Mode
- Sử dụng Console để debugging

**Code Understanding**:
- Đọc và hiểu C# scripts
- Thêm debug logs
- Chỉnh sửa hành vi method
- Tạo features mới

**Game Systems**:
- Enemy AI và spawning
- Player shooting mechanics
- Shop và save system
- Animation events
- Camera control

### Các Bước Tiếp Theo Được Khuyến Nghị

**Tuần 1**: Xem lại những gì bạn đã học
1. Làm lại các tasks mà không nhìn hướng dẫn
2. Chỉnh sửa giá trị thử nghiệm
3. Phá vỡ thứ gì đó cố ý, sau đó sửa lại

**Tuần 2**: Mở rộng features
1. Kết hợp nhiều tasks (enemy nhanh với damage cao)
2. Tạo ý tưởng task của riêng bạn
3. Chia sẻ với team/cộng đồng

**Tuần 3**: Nghiên cứu systems chuyên sâu
1. Đọc **[02_He_Thong_Player.md](02_He_Thong_Player.md)**
2. Đọc **[03_He_Thong_Enemy.md](03_He_Thong_Enemy.md)**
3. Đọc **[05_Cac_Manager.md](05_Cac_Manager.md)**

**Tuần 4**: Xây dựng thứ gì đó mới
1. Theo **[10_Huong_Dan_Thuc_Hanh.md](10_Huong_Dan_Thuc_Hanh.md)**
2. Implement một guide từ đầu
3. Ghi chép lại quá trình của bạn

### Git Best Practices Review

**Sau khi hoàn thành các tasks này**:

```bash
# Xem lại các thay đổi của bạn
git status
git diff

# Tạo commits logic
git add Assets/Prefabs/Enemies/FastGoblin.prefab
git commit -m "Task 3: Created FastGoblin enemy variant

- Duplicated Goblin prefab
- Increased speed to 6
- Reduced health to 30
- Increased damage to 15"

# Push công việc của bạn
git push origin task/fast-goblin

# Merge khi sẵn sàng
git checkout main
git merge task/fast-goblin
```

### Troubleshooting Tips

**Nếu bạn bị kẹt**:
1. **Đọc error messages** cẩn thận trong Console
2. **Kiểm tra guide này** cho các vấn đề phổ biến
3. **Tham khảo tài liệu**:
   - [11_Xu_Ly_Su_Co.md](11_Xu_Ly_Su_Co.md) - Các vấn đề phổ biến
   - [99_Glossary.md](99_Glossary.md) - Định nghĩa thuật ngữ
4. **Xin trợ giúp** với error messages cụ thể

**Trước khi xin trợ giúp**:
- ✅ Bạn đang cố làm gì?
- ✅ Bạn mong đợi điều gì xảy ra?
- ✅ Điều gì thực sự xảy ra?
- ✅ Bạn đã thử gì rồi?

### Ăn Mừng Tiến Trình Của Bạn!

Bạn đã hoàn thành 10 hands-on tasks và học được:
- Unity Editor basics
- Code modification
- Game system understanding
- Debugging techniques
- Git workflow

**Bạn đã sẵn sàng cho các chủ đề nâng cao hơn!**

---

## Tài Nguyên Bổ Sung

### Unity Learning
- **[00_Unity_Co_Ban.md](00_Unity_Co_Ban.md)** - Complete Unity basics
- Unity Learn: https://learn.unity.com/
- Unity Manual: https://docs.unity3d.com/Manual/

### Project Documentation
- **[00_BAT_DAU_TU_DAY.md](00_BAT_DAU_TU_DAY.md)** - Điểm bắt đầu chính
- **[01_Kien_Truc_Project.md](01_Kien_Truc_Project.md)** - Cấu trúc project
- **[99_Glossary.md](99_Glossary.md)** - Định nghĩa thuật ngữ

### Practical Guides
- **[10_Huong_Dan_Thuc_Hanh.md](10_Huong_Dan_Thuc_Hanh.md)** - Hướng dẫn từng bước
- **[11_Xu_Ly_Su_Co.md](11_Xu_Ly_Su_Co.md)** - Sửa vấn đề
- **[13_Code_Examples.md](13_Code_Examples.md)** - Copy-paste code

### Reference
- **[12_Visual_Reference.md](12_Visual_Reference.md)** - Diagrams & visuals
- **[project-analysis.md](project-analysis.md)** - Technical deep dive

---

**Sẵn sàng cho nhiều hơn?** → [10_Huong_Dan_Thuc_Hanh.md](10_Huong_Dan_Thuc_Hanh.md)

**Cần trợ giúp?** → [11_Xu_Ly_Su_Co.md](11_Xu_Ly_Su_Co.md)

**Chúc bạn may mắn với hành trình phát triển game! 🎮**

---

<p align="center">
<strong>Lawn Defense: Monsters Out</strong><br>
Nhiệm Vụ Đầu Tiên - Học Thực Hành<br>
Phiên Bản 2.0 • Tháng 10 2025
</p>
