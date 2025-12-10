# Lộ Trình Học Tập Cho "Lawn Defense: Monsters Out"

**Mục Đích**: Lộ trình học tập được đề xuất để hiểu và đóng góp cho project này, từ không biết gì về Unity đến các features nâng cao.

**Lưu Ý**: Để có comprehensive beginner's guide, xem `00_BAT_DAU_TU_DAY.md`.

**Lộ trình này**: Cung cấp kế hoạch học tập có cấu trúc 1-4 tuần với mục tiêu hàng ngày.

---

## Dành Cho Ai?

**Beginner**: Chưa bao giờ dùng Unity → Làm theo full roadmap (3-4 tuần)

**Intermediate**: Biết Unity basics → Bắt đầu từ Phase B (2-3 tuần)

**Advanced**: Có kinh nghiệm với Unity → Bắt đầu từ Phase C (1 tuần)

---

## Ước Tính Thời Gian

- **Phase A** (Unity Fundamentals): 2-4 ngày
- **Phase B** (Project Code): 2-4 ngày
- **Phase C** (Hands-On Practice): 2-5 ngày
- **Phase D** (Advanced Topics): 2-5 ngày

**Tổng**: 1-3 tuần (tùy thuộc vào daily time commitment)

---

## Phase A: Unity Fundamentals (2-4 ngày)

**Mục Tiêu**: Học các khái niệm Unity quan trọng trước khi chạm vào project code.

### Ngày 1-2: Unity Editor Basics

**Cần Học Gì**:
- Unity interface: Scene, Game, Hierarchy, Inspector, Project, Console windows
- GameObject và Component architecture
- Prefabs và instances
- Scenes và scene management
- Tags và Layers

**Cách Học**:
1. Đọc `00_Cac_Khai_Niem_Unity_Co_Ban.md` → "Getting Started with Unity"
2. Làm theo Unity's official "Roll-a-ball" tutorial
3. Tạo một simple scene với 3 GameObjects

**Practice Task**:
```
Tạo một simple scene:
1. Thêm một cube (GameObject → 3D Object → Cube)
2. Thêm Rigidbody component
3. Nhấn Play, xem nó rơi
4. Thêm một plane bên dưới để bắt nó
```

### Ngày 3: MonoBehaviour & Scripting Basics

**Cần Học Gì**:
- MonoBehaviour lifecycle (Awake, Start, Update, FixedUpdate)
- Basic C# syntax cho Unity
- Getting components (`GetComponent<>()`)
- Finding GameObjects
- Coroutines basics

**Cách Học**:
1. Đọc `00_Cac_Khai_Niem_Unity_Co_Ban.md` → "MonoBehaviour Lifecycle"
2. Đọc `13_Vi_Du_Code.md` → Basic examples
3. Viết một simple script:

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

### Ngày 4: Physics & UI Basics

**Cần Học Gì**:
- Physics2D: Rigidbody2D, Collider2D
- Raycasts và collision detection
- Basic UI: Canvas, Text, Button

**Cách Học**:
1. Đọc `00_Cac_Khai_Niem_Unity_Co_Ban.md` → "Physics and Collisions"
2. Tạo một project với:
   - Moving character (arrow keys)
   - Collectable coins (OnTriggerEnter2D)
   - UI text hiển thị score

**✅ Checkpoint**: Bây giờ bạn nên hiểu:
- Unity editor hoạt động như thế nào
- Basic C# scripting cho Unity
- GameObject/Component relationship
- Physics và collision basics

---

## Phase B: Project Code Structure (2-4 ngày)

**Mục Tiêu**: Hiểu architecture và core systems của project này.

### Ngày 5: Project Overview

**Cần Nghiên Cứu**:
1. Đọc `README_VI.md`
2. Đọc `01_Kien_Truc_Project.md`
3. Khám phá folder structure:
   - `Assets/_MonstersOut/Scripts/Managers/` → Core managers
   - `Assets/_MonstersOut/Scripts/Enemy/` → Enemy system
   - `Assets/_MonstersOut/Scripts/Player/` → Player system
   - `Assets/_MonstersOut/Scripts/UI/` → UI scripts

**Practice Task**:
```
Mở project trong Unity:
1. Tìm GameManager trong Game scene
2. Tìm Enemy.cs trong Project window
3. Mở MenuManager.cs và đọc qua nó
4. Xác định 3 manager scripts và ghi chú purposes của chúng
```

### Ngày 6-7: Core Systems Deep Dive

**Cần Nghiên Cứu (chọn 2-3 mỗi ngày)**:
- **GameManager**: Đọc `05_Cac_Manager_Day_Du.md` → GameManager section
- **Enemy System**: Đọc `He_Thong_Enemy_Nang_Cao.md`
- **Player System**: Đọc `He_Thong_Player_Nang_Cao.md`
- **UI System**: Đọc `04_He_Thong_UI_Day_Du.md`

**Practice Tasks**:

**Ngày 6 - Enemy System**:
1. Mở Goblin prefab
2. Tìm Enemy.cs component
3. Đọc Enemy.cs → `TakeDamage()` method
4. Thêm Debug.Log để xem khi Goblin nhận damage
5. Chạy game, bắn Goblin, kiểm tra Console

**Ngày 7 - Player System**:
1. Mở Player_Archer prefab
2. Tìm Player_Archer.cs component
3. Đọc `He_Thong_Player_Nang_Cao.md` → Auto-targeting section
4. Hiểu cách archer tìm và bắn enemies
5. Sửa arrow damage trong Inspector, test

**✅ Checkpoint**: Bây giờ bạn nên hiểu:
- GameManager điều khiển game flow như thế nào
- Enemy.cs hoạt động như thế nào (TakeDamage, Die, effects)
- Player_Archer auto-targets và bắn như thế nào
- UI cập nhật như thế nào (health bars, wave progress)

---

## Phase C: Hands-On Practice (2-5 ngày)

**Mục Tiêu**: Thực hiện những thay đổi thực tế cho project.

### Ngày 8: Simple Modifications

**Tasks**:

1. **Sửa enemy health**:
   - Mở Goblin prefab
   - Thay đổi `maxHealth` từ 100 thành 200
   - Test: Mất nhiều hits hơn để kill

2. **Thay đổi player damage**:
   - Mở Player_Archer prefab
   - Tìm `UpgradedCharacterParameter` component
   - Thay đổi `defaultRangeDamage`
   - Test: Arrows gây nhiều damage hơn

3. **Thêm debug logs**:
   ```csharp
   // Trong Enemy.cs → TakeDamage():
   public override void TakeDamage(...)
   {
       Debug.Log($"{gameObject.name} took {damage} damage! Health: {currentHealth}");
       // ... phần còn lại của method
   }
   ```

### Ngày 9-10: Thêm Enemy Mới

**Mục Tiêu**: Tạo "Fast Goblin" variant

**Các Bước**:
1. Làm theo `10_Huong_Dan_Thuc_Hanh.md` → "Creating a New Enemy Type"
2. Duplicate Goblin prefab
3. Đổi tên thành "FastGoblin"
4. Tăng `moveSpeed` từ 2 lên 4
5. Thay đổi sprite color (tint sang đỏ)
6. Thêm vào Level_1 wave configuration
7. Test trong Play mode

### Ngày 11: Sửa UI

**Mục Tiêu**: Thêm "Enemies Killed" counter

**Các Bước**:
1. Mở UI canvas trong Game scene
2. Thêm UI Text element
3. Tạo script:

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

4. Connect vào GameManager event system
5. Test: Counter increments khi kills

**✅ Checkpoint**: Bây giờ bạn có thể:
- Sửa existing prefabs
- Tạo enemy variants mới
- Thêm simple UI elements
- Viết basic gameplay scripts

---

## Phase D: Advanced Features (2-5 ngày)

**Mục Tiêu**: Hiểu và implement các features phức tạp.

### Ngày 12: Character Upgrade System

**Cần Nghiên Cứu**:
- Đọc `Thuoc_Tinh_Nhan_Vat.md` toàn bộ
- Hiểu `UpgradedCharacterParameter.cs`
- Nghiên cứu upgrade save/load system (PlayerPrefs properties)

**Practice**:
1. Tạo upgrade tier mới:
   - Mở Player_Archer prefab
   - Tìm `UpgradeSteps` array
   - Thêm upgrade mới: +50 health, +20 damage, 1000 coin price
2. Test trong shop: Upgrade applies đúng

### Ngày 13: Level Creation

**Cần Nghiên Cứu**:
- Đọc `Ban_Do.md` → "Tạo Level Mới"
- Hiểu `GameLevelSetup.cs`
- Nghiên cứu wave configuration

**Practice**:
1. Tạo Level_4:
   - Duplicate Level_3 prefab
   - Sửa waves (thêm nhiều enemies, enemies khó hơn)
   - Đặt givenMana thành 1500
   - Đặt enemyFortrestLevel thành 4
2. Thêm vào GameManager.gameLevels array
3. Test: Có thể load và complete level mới

### Ngày 14: Custom Enemy Ability

**Mục Tiêu**: Thêm "Regenerating Enemy"

**Các Bước**:
1. Tạo script:

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

2. Tạo "RegeneratingGoblin" prefab
3. Attach RegeneratingEnemy script
4. Test: Health regenerates theo thời gian

### Ngày 15: Advanced Systems

**Chọn một để nghiên cứu sâu**:
- **Event System**: Đọc `Su_Kien_Va_Trigger.md`, implement custom event
- **Scene Management**: Đọc `Ban_Do.md`, tạo custom loading screen
- **Shop System**: Đọc `He_Thong_Shop_UI.md`, thêm purchasable item mới

**✅ Checkpoint**: Bây giờ bạn có thể:
- Tạo complex enemy behaviors
- Thiết kế và implement levels mới
- Làm việc với character upgrade system
- Hiểu advanced project systems

---

## Tiếp Theo Là Gì?

Sau khi hoàn thành roadmap này, bạn sẽ có thể:

### Đóng Góp Features
- Thêm enemies mới với unique abilities
- Tạo levels và boss fights mới
- Thiết kế upgrade systems
- Implement UI features mới

### Fix Bugs
- Debug gameplay issues
- Fix animation problems
- Giải quyết collision bugs
- Optimize performance

### Mở Rộng Systems
- Thêm multiplayer support
- Implement game modes mới
- Tạo level editor
- Thêm achievements system

---

## Tài Nguyên Học Tập

### Internal Documentation
- `00_BAT_DAU_TU_DAY.md` - Comprehensive beginner guide
- `00_Cac_Khai_Niem_Unity_Co_Ban.md` - Unity basics (1,200+ dòng)
- `10_Huong_Dan_Thuc_Hanh.md` - Step-by-step tutorials
- `11_Xu_Ly_Su_Co.md` - Debugging help
- `13_Vi_Du_Code.md` - Code patterns

### External Resources
- [Unity Learn](https://learn.unity.com/) - Official tutorials
- [Unity Manual](https://docs.unity3d.com/Manual/) - Reference documentation
- [Unity Scripting Reference](https://docs.unity3d.com/ScriptReference/) - API docs

---

## Daily Time Commitment

**Beginner** (không có Unity experience):
- 2-3 giờ/ngày trong 3-4 tuần
- Tập trung vào hiểu concepts, không phải speed

**Intermediate** (biết Unity basics):
- 1-2 giờ/ngày trong 2-3 tuần
- Tập trung vào project-specific systems

**Advanced** (Unity experienced):
- 1 giờ/ngày trong 1-2 tuần
- Tập trung vào project architecture và advanced features

---

## Tips Để Thành Công

1. **Đừng vội**: Hiểu concepts quan trọng hơn hoàn thành nhanh
2. **Practice hàng ngày**: 30 phút mỗi ngày > 3 giờ mỗi tuần một lần
3. **Đặt câu hỏi**: Comment trong code, viết notes, thảo luận với team
4. **Phá vỡ mọi thứ**: Thử nghiệm trong test branch, học từ errors
5. **Đọc code**: Nghiên cứu existing systems trước khi viết code mới
6. **Test thường xuyên**: Play-test sau mỗi thay đổi

---

## Tóm Tắt

**Tổng Quan Roadmap**:
```
Phase A (2-4 ngày): Học Unity fundamentals
        ↓
Phase B (2-4 ngày): Nghiên cứu project code architecture
        ↓
Phase C (2-5 ngày): Thực hiện simple modifications
        ↓
Phase D (2-5 ngày): Implement advanced features
        ↓
Sẵn sàng đóng góp! 🎉
```

**Nhớ Rằng**: Đây là lộ trình *đề xuất*. Điều chỉnh dựa trên:
- Unity knowledge hiện có của bạn
- Thời gian available
- Learning style (một số thích đọc, một số thích thử nghiệm)
- Project needs (bạn đang làm features gì?)

**Các Bước Tiếp Theo**:
- Bắt đầu với `00_BAT_DAU_TU_DAY.md` nếu hoàn toàn mới
- Nhảy đến Phase B nếu bạn biết Unity basics
- Đào sâu vào specific documentation khi cần
- Happy coding! 🚀

---

**Kết Thúc Tài Liệu**

<p align="center">
<strong>Lawn Defense: Monsters Out</strong><br>
Lộ Trình Học Tập<br>
Learning Roadmap
</p>
