# Hướng Dẫn Thực Hành - Tutorials Chi Tiết

---
**🌐 Ngôn ngữ:** Tiếng Việt
**📄 File gốc:** [10_How_To_Guides.md](10_How_To_Guides.md)
**🔄 Cập nhật lần cuối:** 2025-01-30
---

> **Dành cho**: Developer sẵn sàng chỉnh sửa game
> **Thời gian đọc**: 60+ phút (tài liệu tham khảo)
> **Yêu cầu**: Tất cả tài liệu hệ thống cốt lõi (02-05)

---

## Mục Lục
1. [Cách Thêm Loại Enemy Mới](#cách-thêm-loại-enemy-mới)
2. [Cách Tạo Custom UI Panel](#cách-tạo-custom-ui-panel)
3. [Cách Thêm Weapon Effect Mới](#cách-thêm-weapon-effect-mới)
4. [Cách Chỉnh Sửa Player Stats](#cách-chỉnh-sửa-player-stats)
5. [Cách Thêm Level Mới](#cách-thêm-level-mới)
6. [Cách Thêm Power-Up Item](#cách-thêm-power-up-item)
7. [Cách Tạo Custom Health Bar](#cách-tạo-custom-health-bar)
8. [Cách Thêm Sound Effect](#cách-thêm-sound-effect)
9. [Cách Triển Khai Save/Load System](#cách-triển-khai-saveload-system)
10. [Cách Thay Đổi Độ Khó Game](#cách-thay-đổi-độ-khó-game)

---

## Cách Thêm Loại Enemy Mới

### Mục Tiêu
Tạo enemy mới tên "Ghost" với hành vi bay và khả năng xuyên qua.

### Yêu Cầu
- Đọc `03_He_Thong_Enemy.md`
- Hiểu Enemy base class và SmartEnemyGrounded

### Bước 1: Tạo Enemy Sprite

1. Import sprite: `Assets/Resources/Sprite/Enemy/10. Ghost/`
2. Tạo animation:
   - `Ghost_Idle` (loop)
   - `Ghost_Fly` (loop)
   - `Ghost_Attack` (once)
   - `Ghost_Die` (once)

3. Tạo Animation Controller: `Ghost_AnimController`
   ```
   Parameter:
   - speed (float)
   - attack (trigger)
   - isDead (bool)

   Transition:
   - Idle → Fly: speed > 0.1
   - Fly → Idle: speed < 0.1
   - Any State → Attack: attack trigger
   - Any State → Die: isDead = true
   ```

### Bước 2: Tạo Ghost Script

Tạo `Enemy_Ghost.cs` trong `Assets/_MonstersOut/Scripts/AI/`:

```csharp
using UnityEngine;
using System.Collections;

namespace RGame
{
    [AddComponentMenu("ADDP/Enemy AI/Ghost Enemy")]
    public class Enemy_Ghost : Enemy, ICanTakeDamage
    {
        [Header("Flying Settings")]
        public float flySpeed = 3f;
        public float flyHeight = 2f;
        public float floatSpeed = 1f;
        public float floatAmount = 0.5f;

        private Vector3 targetPosition;
        private float floatTimer;
        private EnemyRangeAttack rangeAttack;

        public override void Start()
        {
            base.Start();

            // Lấy attack component
            rangeAttack = GetComponent<EnemyRangeAttack>();

            // Tìm pháo đài để bay đến
            var fortress = FindObjectOfType<TheFortrest>();
            if (fortress)
            {
                targetPosition = new Vector3(
                    fortress.transform.position.x,
                    flyHeight,
                    0
                );
            }
        }

        public override void Update()
        {
            base.Update();

            // Chỉ di chuyển nếu đang chơi và ở trạng thái WALK
            if (isPlaying && enemyState == ENEMYSTATE.WALK)
            {
                // Di chuyển về phía pháo đài
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPosition,
                    flySpeed * Time.deltaTime
                );

                // Chuyển động lơ lửng (lên xuống)
                floatTimer += Time.deltaTime * floatSpeed;
                float newY = flyHeight + Mathf.Sin(floatTimer) * floatAmount;
                transform.position = new Vector3(
                    transform.position.x,
                    newY,
                    transform.position.z
                );

                // Cập nhật animation
                AnimSetFloat("speed", flySpeed);

                // Quay mặt về hướng di chuyển
                if (targetPosition.x < transform.position.x)
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                else
                    transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else
            {
                AnimSetFloat("speed", 0);
            }
        }

        public override void DetectPlayer(float delayChase = 0)
        {
            base.DetectPlayer(delayChase);

            // Kiểm tra nếu trong tầm tấn công
            if (rangeAttack && rangeAttack.CheckPlayer(isFacingRight()))
            {
                // Dừng di chuyển
                SetEnemyState(ENEMYSTATE.ATTACK);

                // Tấn công
                if (rangeAttack.AllowAction())
                {
                    rangeAttack.Action();
                    AnimSetTrigger("attack");
                }
            }
        }

        public override void Die()
        {
            base.Die();

            // Animation rơi xuống
            StartCoroutine(FallDown());
        }

        IEnumerator FallDown()
        {
            float timer = 0;
            float fallDuration = 1f;
            Vector3 startPos = transform.position;

            while (timer < fallDuration)
            {
                timer += Time.deltaTime;
                float newY = Mathf.Lerp(startPos.y, -2f, timer / fallDuration);
                transform.position = new Vector3(
                    transform.position.x,
                    newY,
                    transform.position.z
                );
                yield return null;
            }

            gameObject.SetActive(false);
        }
    }
}
```

### Bước 3: Tạo Ghost Prefab

1. Tạo GameObject rỗng: `Enemy_Ghost`
2. Thêm component:
   - `SpriteRenderer` → gán Ghost sprite
   - `Animator` → gán Ghost_AnimController
   - `BoxCollider2D` → điều chỉnh theo kích thước sprite
   - `Enemy_Ghost` script
   - `EnemyRangeAttack` script
   - `CheckTargetHelper` script
   - `GiveCoinWhenDie` script (optional)

3. Cấu hình `Enemy_Ghost` setting:
   ```
   Health: 80
   Walk Speed: 3
   Gravity: 0 (flying enemy)
   Attack Type: RANGE
   Start Behavior: WALK_LEFT

   Can Be Freeze: true
   Can Be Burn: true
   Can Be Poison: true
   Can Be Shock: true
   ```

4. Cấu hình `EnemyRangeAttack`:
   ```
   Enemy Layer: Player
   Check Point: Tạo child "CheckPoint"
   Fire Point: Tạo child "FirePoint"
   Shooting Point: Tạo child "ShootingPoint"
   Damage: 25
   Detect Distance: 8
   Bullet: Prefab projectile của bạn
   Shooting Rate: 2
   Aim Target: true
   ```

5. Lưu dạng prefab: `Assets/Resources/Prefabs/Enemies/Enemy_Ghost.prefab`

### Bước 4: Thêm Vào Enemy Wave

Trong cấu hình level:

```csharp
EnemyWave wave3 = new EnemyWave
{
    wait = 15f,  // Đợi 15 giây
    enemySpawns = new EnemySpawn[]
    {
        new EnemySpawn
        {
            enemy = ghostPrefab,  // Reference đến Ghost prefab
            numberEnemy = 3,       // Spawn 3 ghost
            wait = 0f,            // Bắt đầu ngay lập tức
            rate = 2f             // 2 giây giữa mỗi lần spawn
        }
    }
};
```

### Bước 5: Test

1. Chơi scene
2. Đợi Ghost wave (15 giây)
3. Xác minh:
   - ✓ Ghost bay về phía pháo đài
   - ✓ Ghost lơ lửng lên xuống
   - ✓ Ghost tấn công khi trong tầm
   - ✓ Ghost rơi xuống khi bị giết
   - ✓ Effect hoạt động (freeze, burn, etc.)

### Kết Quả Mong Đợi

```
Timeline:
0s:    Game bắt đầu
15s:   Ghost #1 spawn, bay về phía pháo đài
17s:   Ghost #2 spawn
19s:   Ghost #3 spawn
22s:   Ghost #1 trong tầm, bắn projectile
25s:   Ghost #1 bị giết, rơi xuống
```

---

## Cách Tạo Custom UI Panel

### Mục Tiêu
Tạo "Statistics Panel" hiển thị số kills, damage gây ra, và thời gian chơi.

### Yêu Cầu
- Đọc `04_He_Thong_UI.md`
- Hiểu Canvas và UI component

### Bước 1: Tạo UI Element

1. Trong Hierarchy, tìm Canvas
2. Right-click Canvas → `UI → Panel`
3. Đặt tên: `StatisticsPanel`
4. Thêm child element:

```
StatisticsPanel (Panel)
├─ Background (Image)
├─ Title (Text)
│   └─ Text: "Statistics"
├─ CloseButton (Button)
│   └─ Text: "X"
├─ KillsText (Text)
│   └─ Text: "Kills: 0"
├─ DamageText (Text)
│   └─ Text: "Damage: 0"
└─ TimeText (Text)
    └─ Text: "Time: 00:00"
```

### Bước 2: Đặt Vị Trí UI Element

```
StatisticsPanel:
- Anchor: Center
- Position: (0, 0)
- Width: 400
- Height: 300

Title:
- Anchor: Top Center
- Position: (0, -30)
- Font Size: 32

CloseButton:
- Anchor: Top Right
- Position: (-20, -20)
- Width: 40, Height: 40

KillsText, DamageText, TimeText:
- Anchor: Center
- Position: (0, 50), (0, 0), (0, -50)
- Font Size: 24
```

### Bước 3: Tạo StatisticsPanel Script

Tạo `StatisticsPanel.cs` trong `Assets/_MonstersOut/Scripts/UI/`:

```csharp
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace RGame
{
    public class StatisticsPanel : MonoBehaviour
    {
        [Header("UI References")]
        public Text killsText;
        public Text damageText;
        public Text timeText;

        [Header("Data")]
        private int totalKills = 0;
        private int totalDamage = 0;
        private float playTime = 0f;

        void Start()
        {
            // Ẩn khi start
            gameObject.SetActive(false);
        }

        void Update()
        {
            // Chỉ đếm thời gian khi panel hiển thị
            if (gameObject.activeInHierarchy)
            {
                playTime += Time.deltaTime;
                UpdateDisplay();
            }
        }

        public void Show()
        {
            // Reset stat
            totalKills = 0;
            totalDamage = 0;
            playTime = 0f;

            // Load từ StatisticsTracker nếu tồn tại
            if (StatisticsTracker.Instance)
            {
                totalKills = StatisticsTracker.Instance.GetKills();
                totalDamage = StatisticsTracker.Instance.GetDamage();
                playTime = StatisticsTracker.Instance.GetPlayTime();
            }

            // Hiển thị panel
            gameObject.SetActive(true);
            UpdateDisplay();
        }

        public void Hide()
        {
            SoundManager.Click();
            gameObject.SetActive(false);
        }

        void UpdateDisplay()
        {
            killsText.text = $"Kills: {totalKills}";
            damageText.text = $"Damage: {totalDamage}";

            // Format time dạng MM:SS
            int minutes = Mathf.FloorToInt(playTime / 60);
            int seconds = Mathf.FloorToInt(playTime % 60);
            timeText.text = $"Time: {minutes:00}:{seconds:00}";
        }
    }
}
```

### Bước 4: Tạo StatisticsTracker

Tạo `StatisticsTracker.cs` trong `Assets/_MonstersOut/Scripts/Managers/`:

```csharp
using UnityEngine;

namespace RGame
{
    public class StatisticsTracker : MonoBehaviour, IListener
    {
        public static StatisticsTracker Instance { get; private set; }

        private int totalKills = 0;
        private int totalDamage = 0;
        private float playTime = 0f;
        private bool isPlaying = false;

        void Awake()
        {
            Instance = this;
        }

        void Update()
        {
            // Đếm thời gian chơi
            if (isPlaying)
                playTime += Time.deltaTime;
        }

        public void AddKill()
        {
            totalKills++;
        }

        public void AddDamage(int damage)
        {
            totalDamage += damage;
        }

        public int GetKills() => totalKills;
        public int GetDamage() => totalDamage;
        public float GetPlayTime() => playTime;

        // IListener implementation
        public void IPlay()
        {
            isPlaying = true;
            totalKills = 0;
            totalDamage = 0;
            playTime = 0f;
        }

        public void ISuccess()
        {
            isPlaying = false;
        }

        public void IGameOver()
        {
            isPlaying = false;
        }

        public void IPause() { }
        public void IUnPause() { }
        public void IOnRespawn() { }
        public void IOnStopMovingOn() { }
        public void IOnStopMovingOff() { }
    }
}
```

### Bước 5: Kết Nối Statistics Tracking

Trong `Enemy.cs` Die() method, thêm:

```csharp
public virtual void Die()
{
    // Code hiện có...

    // Theo dõi kill
    if (StatisticsTracker.Instance)
        StatisticsTracker.Instance.AddKill();

    // Code hiện có...
}
```

Trong `Enemy.cs` TakeDamage() method, thêm:

```csharp
public void TakeDamage(float damage, ...)
{
    // Code hiện có...

    currentHealth -= (int)damage;

    // Theo dõi damage
    if (StatisticsTracker.Instance)
        StatisticsTracker.Instance.AddDamage((int)damage);

    // Code hiện có...
}
```

### Bước 6: Thêm Button Hiển Thị Statistics

Trong Victory UI, thêm button:

```csharp
// Trong Menu_Victory.cs
public StatisticsPanel statisticsPanel;

public void OnStatisticsButtonClick()
{
    statisticsPanel.Show();
}
```

### Bước 7: Cấu Hình Button

1. Chọn Statistics button trong Victory UI
2. Button component → OnClick()
3. Thêm entry: `Menu_Victory → OnStatisticsButtonClick()`

### Bước 8: Test

1. Chơi game
2. Giết enemy
3. Hoàn thành level
4. Click button Statistics
5. Xác minh:
   - ✓ Hiển thị số kill đúng
   - ✓ Hiển thị tổng damage gây ra
   - ✓ Hiển thị thời gian chơi
   - ✓ Button đóng hoạt động

### Kết Quả Mong Đợi

```
Victory Screen:
┌─────────────────────────┐
│      VICTORY!           │
│   ★ ★ ★                 │
│                         │
│  [Statistics] [Menu]    │
└─────────────────────────┘

Click Statistics:
┌─────────────────────────┐
│    Statistics      [X]  │
│                         │
│  Kills: 47              │
│  Damage: 3,842          │
│  Time: 03:45            │
│                         │
└─────────────────────────┘
```

---

## Cách Thêm Weapon Effect Mới

### Mục Tiêu
Thêm hiệu ứng vũ khí "Lightning" lan truyền đến enemy gần đó.

### Yêu Cầu
- Đọc `03_He_Thong_Enemy.md` (phần Effect System)
- Hiểu WeaponEffect và ENEMYEFFECT enum

### Bước 1: Thêm Lightning Vào ENEMYEFFECT Enum

Trong `Enemy.cs`, chỉnh sửa enum:

```csharp
public enum ENEMYEFFECT
{
    NONE,
    BURNING,
    FREEZE,
    SHOKING,
    POISON,
    EXPLOSION,
    LIGHTNING  // ← Thêm cái này
}
```

### Bước 2: Thêm Lightning Effect Method Vào Enemy

Trong `Enemy.cs`, thêm:

```csharp
#region ICanLightning implementation

[Header("Lightning Option")]
[HideInInspector] public bool canBeLightning = true;
[HideInInspector] public int lightningChainCount = 3;
[HideInInspector] public float lightningChainRadius = 5f;

public virtual void Lightning(float damage, GameObject instigator)
{
    // Không thể lightning nếu đang lightning
    if (enemyEffect == ENEMYEFFECT.LIGHTNING)
        return;

    if (canBeLightning)
    {
        // Áp dụng damage
        currentHealth -= (int)damage;

        // Hiển thị damage
        FloatingTextManager.Instance.ShowText(
            "" + (int)damage,
            healthBarOffset,
            Color.yellow,
            transform.position
        );

        // Cập nhật thanh máu
        if (healthBar)
            healthBar.UpdateValue(currentHealth / (float)health);

        // Kiểm tra nếu chết
        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        // Lan truyền đến enemy gần
        StartCoroutine(LightningChain(damage, instigator));
    }
}

IEnumerator LightningChain(float damage, GameObject instigator)
{
    enemyEffect = ENEMYEFFECT.LIGHTNING;

    // Tìm enemy gần
    RaycastHit2D[] hits = Physics2D.CircleCastAll(
        transform.position,
        lightningChainRadius,
        Vector2.zero,
        0,
        GameManager.Instance.layerEnemy
    );

    int chained = 0;

    foreach (var hit in hits)
    {
        // Bỏ qua bản thân
        if (hit.collider.gameObject == gameObject)
            continue;

        // Kiểm tra nếu có thể nhận damage
        var enemy = hit.collider.GetComponent<Enemy>();
        if (enemy != null && chained < lightningChainCount)
        {
            // Vẽ hiệu ứng lightning (visual)
            DrawLightning(transform.position, enemy.transform.position);

            // Lan truyền lightning
            enemy.Lightning(damage * 0.7f, instigator);  // 70% damage

            chained++;

            yield return new WaitForSeconds(0.1f);  // Delay nhỏ
        }
    }

    // Xóa effect
    yield return new WaitForSeconds(0.5f);
    enemyEffect = ENEMYEFFECT.NONE;
}

void DrawLightning(Vector3 start, Vector3 end)
{
    // Tạo lightning line renderer
    GameObject lightningObj = new GameObject("Lightning");
    LineRenderer line = lightningObj.AddComponent<LineRenderer>();

    line.startWidth = 0.1f;
    line.endWidth = 0.1f;
    line.positionCount = 2;
    line.SetPosition(0, start);
    line.SetPosition(1, end);

    // Đặt màu (vàng)
    line.material = new Material(Shader.Find("Sprites/Default"));
    line.startColor = Color.yellow;
    line.endColor = Color.yellow;

    // Destroy sau 0.2 giây
    Destroy(lightningObj, 0.2f);
}

#endregion
```

### Bước 3: Thêm LIGHTNING Vào WEAPON_EFFECT Enum

Trong `WeaponEffect.cs` (hoặc tạo nếu không tồn tại):

```csharp
public enum WEAPON_EFFECT
{
    NORMAL,
    FREEZE,
    POISON,
    LIGHTNING  // ← Thêm cái này
}
```

### Bước 4: Chỉnh Sửa WeaponEffect ScriptableObject

Trong `WeaponEffect.cs`:

```csharp
[System.Serializable]
public class WeaponEffect : ScriptableObject
{
    public WEAPON_EFFECT effectType = WEAPON_EFFECT.NORMAL;

    // ... field freeze/poison hiện có ...

    [Header("Lightning")]
    public float lightningDamage = 30f;
}
```

### Bước 5: Cập Nhật TakeDamage Để Xử Lý Lightning

Trong `Enemy.cs` TakeDamage() method:

```csharp
public void TakeDamage(float damage, Vector2 force, Vector2 hitPoint,
                       GameObject instigator, BODYPART bodyPart = BODYPART.NONE,
                       WeaponEffect weaponEffect = null)
{
    // ... code hiện có ...

    if (currentHealth <= 0)
    {
        Die();
    }
    else
    {
        if (weaponEffect != null)
        {
            switch (weaponEffect.effectType)
            {
                case WEAPON_EFFECT.POISON:
                    Poison(weaponEffect.poisonDamagePerSec,
                          weaponEffect.poisonTime,
                          instigator);
                    return;

                case WEAPON_EFFECT.FREEZE:
                    Freeze(weaponEffect.freezeTime, instigator);
                    return;

                case WEAPON_EFFECT.LIGHTNING:  // ← Thêm cái này
                    Lightning(weaponEffect.lightningDamage, instigator);
                    return;

                case WEAPON_EFFECT.NORMAL:
                    break;
            }
        }

        Hit(force);
    }
}
```

### Bước 6: Tạo Lightning Weapon Effect Asset

1. Trong Project window: `Create → Weapon Effect → Lightning Effect`
2. Đặt tên: `LightningEffect`
3. Cấu hình:
   ```
   Effect Type: LIGHTNING
   Lightning Damage: 30
   ```

### Bước 7: Gán Cho Weapon

Trong arrow/bullet prefab:

```csharp
public class Projectile : MonoBehaviour
{
    public WeaponEffect weaponEffect;  // Gán LightningEffect ở đây

    void OnTriggerEnter2D(Collider2D other)
    {
        var takeDamage = other.GetComponent<ICanTakeDamage>();
        if (takeDamage != null)
        {
            takeDamage.TakeDamage(
                damage,
                force,
                transform.position,
                owner,
                BODYPART.NONE,
                weaponEffect  // Truyền weapon effect
            );
        }
    }
}
```

### Bước 8: Test

1. Chơi game
2. Bắn enemy với vũ khí lightning
3. Xác minh:
   - ✓ Enemy nhận lightning damage
   - ✓ Lightning lan truyền đến 3 enemy gần
   - ✓ Chain damage là 70% của damage gốc
   - ✓ Hiệu ứng lightning visual xuất hiện
   - ✓ Enemy bị lan truyền có thể chết

### Kết Quả Mong Đợi

```
Player bắn enemy A với mũi tên lightning:

Enemy A: Nhận 30 damage
         └─ Lan truyền đến Enemy B (5 unit away)
                Nhận 21 damage (70%)
                └─ Lan truyền đến Enemy C
                       Nhận 14.7 damage (70% của 21)
                       └─ Lan truyền đến Enemy D
                              Nhận 10.3 damage (70% của 14.7)

Visual: Đường màu vàng kết nối A→B→C→D
```

---

## Cách Chỉnh Sửa Player Stats

### Mục Tiêu
Tăng máu player và damage mũi tên.

### Yêu Cầu
- Đọc `02_He_Thong_Player.md`
- Hiểu Player_Archer component

### Phương Pháp 1: Chỉnh Sửa Trực Tiếp Inspector

**Dễ nhất** nhưng yêu cầu thay đổi thủ công mỗi instance.

1. Chọn Player_Archer GameObject trong scene
2. Trong Inspector, tìm component `Enemy` (base class)
3. Chỉnh sửa:
   ```
   Health: 100 → 150
   ```

4. Tìm component `Player_Archer`
5. Chỉnh sửa:
   ```
   Arrow Damage: 20 → 30
   ```

6. Lưu scene

**Ưu điểm**: Nhanh, không cần code
**Nhược điểm**: Không duy trì qua level, phải thay đổi từng level

### Phương Pháp 2: Chỉnh Sửa Prefab

**Tốt hơn** - thay đổi áp dụng cho tất cả instance.

1. Tìm Player prefab: `Assets/Resources/Prefabs/Player_Archer.prefab`
2. Double-click để edit prefab
3. Chỉnh sửa setting (giống Phương Pháp 1)
4. Lưu prefab (Ctrl+S)

**Ưu điểm**: Áp dụng cho tất cả level
**Nhược điểm**: Vẫn thủ công

### Phương Pháp 3: ScriptableObject Upgrade System

**Tốt nhất** - nâng cấp động với hệ thống save.

#### Bước 1: Tạo UpgradeData ScriptableObject

Tạo `PlayerUpgradeData.cs`:

```csharp
using UnityEngine;

namespace RGame
{
    [CreateAssetMenu(fileName = "PlayerUpgrade", menuName = "RGame/Player Upgrade Data")]
    public class PlayerUpgradeData : ScriptableObject
    {
        [Header("Health")]
        public int baseHealth = 100;
        public int healthUpgradePerLevel = 10;

        [Header("Damage")]
        public float baseDamage = 20f;
        public float damageUpgradePerLevel = 2f;

        [Header("Attack Speed")]
        public float baseReloadTime = 0.5f;
        public float reloadTimeReduction = 0.05f;  // Nhanh hơn mỗi level

        public int GetHealth(int upgradeLevel)
        {
            return baseHealth + (healthUpgradePerLevel * upgradeLevel);
        }

        public float GetDamage(int upgradeLevel)
        {
            return baseDamage + (damageUpgradePerLevel * upgradeLevel);
        }

        public float GetReloadTime(int upgradeLevel)
        {
            return Mathf.Max(0.1f, baseReloadTime - (reloadTimeReduction * upgradeLevel));
        }
    }
}
```

#### Bước 2: Tạo Upgrade Asset

1. Right-click trong Project: `Create → RGame → Player Upgrade Data`
2. Đặt tên: `PlayerUpgradeData`
3. Cấu hình stat cơ bản

#### Bước 3: Chỉnh Sửa Player_Archer Để Dùng Upgrade

Trong `Player_Archer.cs`:

```csharp
public class Player_Archer : Enemy
{
    [Header("Upgrade Data")]
    public PlayerUpgradeData upgradeData;

    public override void Start()
    {
        base.Start();

        // Áp dụng upgrade nếu data tồn tại
        if (upgradeData != null)
        {
            int healthLevel = GlobalValue.GetPlayerHealthLevel();  // Lấy level đã lưu
            int damageLevel = GlobalValue.GetPlayerDamageLevel();
            int speedLevel = GlobalValue.GetPlayerSpeedLevel();

            // Override máu
            health = upgradeData.GetHealth(healthLevel);
            currentHealth = health;

            // Override damage
            arrowDamage = upgradeData.GetDamage(damageLevel);

            // Override reload time
            timeReload = upgradeData.GetReloadTime(speedLevel);

            // Cập nhật thanh máu
            if (healthBar)
                healthBar.UpdateValue(currentHealth / (float)health);
        }
    }
}
```

#### Bước 4: Thêm Save/Load Vào GlobalValue

Trong `GlobalValue.cs`:

```csharp
public static class GlobalValue
{
    // Level nâng cấp
    public static int playerHealthLevel = 0;
    public static int playerDamageLevel = 0;
    public static int playerSpeedLevel = 0;

    public static void UpgradePlayerHealth()
    {
        playerHealthLevel++;
        PlayerPrefs.SetInt("PlayerHealthLevel", playerHealthLevel);
    }

    public static void UpgradePlayerDamage()
    {
        playerDamageLevel++;
        PlayerPrefs.SetInt("PlayerDamageLevel", playerDamageLevel);
    }

    public static void UpgradePlayerSpeed()
    {
        playerSpeedLevel++;
        PlayerPrefs.SetInt("PlayerSpeedLevel", playerSpeedLevel);
    }

    public static int GetPlayerHealthLevel()
    {
        return PlayerPrefs.GetInt("PlayerHealthLevel", 0);
    }

    public static int GetPlayerDamageLevel()
    {
        return PlayerPrefs.GetInt("PlayerDamageLevel", 0);
    }

    public static int GetPlayerSpeedLevel()
    {
        return PlayerPrefs.GetInt("PlayerSpeedLevel", 0);
    }

    public static void ResetUpgrades()
    {
        playerHealthLevel = 0;
        playerDamageLevel = 0;
        playerSpeedLevel = 0;
        PlayerPrefs.DeleteAll();
    }
}
```

#### Bước 5: Tạo Upgrade Shop UI

Trong shop UI, thêm button nâng cấp:

```csharp
public class ShopManager : MonoBehaviour
{
    public int healthUpgradeCost = 100;
    public int damageUpgradeCost = 150;
    public int speedUpgradeCost = 200;

    public void BuyHealthUpgrade()
    {
        if (GlobalValue.SavedCoins >= healthUpgradeCost)
        {
            GlobalValue.SavedCoins -= healthUpgradeCost;
            GlobalValue.UpgradePlayerHealth();

            SoundManager.PlaySfx(SoundManager.Instance.soundUpgrade);
            UpdateUI();
        }
        else
        {
            SoundManager.PlaySfx(SoundManager.Instance.soundNotEnoughCoin);
        }
    }

    // Tương tự cho damage và speed
}
```

#### Bước 6: Test

1. Mở shop
2. Mua nâng cấp máu (giá 100 coin)
3. Chơi level
4. Kiểm tra máu player là 110 (đã là 100, +10)
5. Mua nâng cấp damage
6. Kiểm tra mũi tên gây damage nhiều hơn

### Kết Quả Mong Đợi

```
Stat Ban Đầu:
Health: 100
Damage: 20
Reload: 0.5s

Sau 1 Nâng Cấp Máu:
Health: 110
Damage: 20
Reload: 0.5s
Coin: -100

Sau 2 Nâng Cấp Damage:
Health: 110
Damage: 24
Reload: 0.5s
Coin: -400 (100 + 150 + 150)

Stat duy trì qua các level!
```

---

## Cách Thêm Level Mới

### Mục Tiêu
Tạo Level 6 với wave enemy tùy chỉnh và mana.

### Yêu Cầu
- Đọc `05_Cac_Manager.md` (phần LevelEnemyManager)
- Hiểu GameLevelSetup và LevelManager

### Bước 1: Duplicate Level Prefab Hiện Có

1. Tìm level hiện có: `Assets/Resources/Prefabs/Levels/Level_5.prefab`
2. Duplicate (Ctrl+D)
3. Đổi tên: `Level_6.prefab`

### Bước 2: Cấu Hình Level Setting

Double-click Level_6 prefab để edit:

```
GameLevelSetup component:
- Given Mana: 1500 (đã là 1000)
```

### Bước 3: Thiết Kế Enemy Wave

Trong LevelEnemyManager component:

```csharp
Wave 1: (Khởi đầu dễ)
  wait: 3
  EnemySpawns:
    - Goblin x5 (wait: 0, rate: 0.5)

Wave 2: (Độ khó trung bình)
  wait: 10
  EnemySpawns:
    - Skeleton x4 (wait: 0, rate: 1)
    - Goblin x3 (wait: 2, rate: 0.5)

Wave 3: (Khó)
  wait: 15
  EnemySpawns:
    - Troll x2 (wait: 0, rate: 2)
    - Bomber x3 (wait: 5, rate: 1.5)

Wave 4: (Boss wave)
  wait: 20
  EnemySpawns:
    - TrollBoss x1 (wait: 0, rate: 0)
    - Skeleton x5 (wait: 3, rate: 0.5)
```

### Bước 4: Cấu Hình Wave Trong Inspector

1. Chọn LevelEnemyManager trong Level_6 prefab
2. Mở rộng mảng "Enemy Waves"
3. Đặt Size: 4
4. Cấu hình từng wave:

```
Element 0 (Wave 1):
  Wait: 3
  Enemy Spawns (Size: 1):
    Element 0:
      Enemy: Goblin prefab
      Number Enemy: 5
      Wait: 0
      Rate: 0.5

Element 1 (Wave 2):
  Wait: 10
  Enemy Spawns (Size: 2):
    Element 0:
      Enemy: Skeleton prefab
      Number Enemy: 4
      Wait: 0
      Rate: 1
    Element 1:
      Enemy: Goblin prefab
      Number Enemy: 3
      Wait: 2
      Rate: 0.5

(Tiếp tục cho wave 3 và 4)
```

### Bước 5: Thêm Level Vào GameManager

1. Mở Playing scene
2. Tìm GameManager GameObject
3. Trong Inspector, tìm mảng "Game Levels"
4. Tăng Size lên 6
5. Gán Level_6 prefab cho Element 5

### Bước 6: Cập Nhật GlobalValue

Trong `GlobalValue.cs`:

```csharp
public static int finishGameAtLevel = 6;  // Đã là 5
```

### Bước 7: Test Level Trực Tiếp

Để test mà không chơi hết level trước:

```csharp
// Tạm thời: Trong GameManager.Awake()
GlobalValue.levelPlaying = 6;  // Ép level 6
```

Hoặc tạo test scene:

1. `File → New Scene`
2. Lưu dạng: `TestLevel6`
3. Thêm GameManager, MenuManager, Canvas
4. Đặt GameManager → Game Levels[0] = Level_6
5. Chơi

### Bước 8: Tính Toán Tổng Enemy Để Cân Bằng

```
Wave 1: 5 enemy
Wave 2: 4 + 3 = 7 enemy
Wave 3: 2 + 3 = 5 enemy
Wave 4: 1 + 5 = 6 enemy

Tổng: 23 enemy

Mana đã cho: 1500
Mana trung bình mỗi enemy: 1500 / 23 ≈ 65 mana
(Tốt nếu unit giá 50-100 mana mỗi cái)
```

### Kết Quả Mong Đợi

```
Timeline Level 6:

0:00  - Game bắt đầu, cho 1500 mana
0:03  - Wave 1: Goblin x5 spawn (qua 2.5s)
0:13  - Wave 2: Skeleton x4 spawn, sau đó Goblin x3
0:28  - Wave 3: Troll x2, sau đó Bomber x3
0:48  - Wave 4: TrollBoss spawn, sau đó Skeleton x5
1:15  - Tất cả enemy bị đánh bại → Victory!

Độ khó: Trung bình-Khó (Boss ở cuối)
```

---

## Cách Thêm Power-Up Item

### Mục Tiêu
Tạo health pack và speed boost power-up rơi ra từ enemy.

### Yêu Cầu
- Đọc `03_He_Thong_Enemy.md`
- Hiểu collision và pickup system

### Bước 1: Tạo PowerUp Base Script

Tạo `PowerUp.cs` trong `Assets/_MonstersOut/Scripts/`:

```csharp
using UnityEngine;

namespace RGame
{
    public enum POWERUP_TYPE
    {
        HEALTH,
        SPEED,
        DAMAGE,
        INVINCIBILITY
    }

    [RequireComponent(typeof(CircleCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class PowerUp : MonoBehaviour
    {
        [Header("Settings")]
        public POWERUP_TYPE powerUpType;
        public float value = 25f;          // Số lượng máu hoặc % boost
        public float duration = 5f;         // Cho buff tạm thời
        public AudioClip pickupSound;

        [Header("Visual")]
        public GameObject pickupEffect;
        public float rotateSpeed = 100f;
        public float bobSpeed = 2f;
        public float bobAmount = 0.3f;

        private float bobTimer;
        private Vector3 startPos;

        void Start()
        {
            startPos = transform.position;

            // Setup physics
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0;  // Lơ lửng trong không khí
            rb.isKinematic = true;

            // Setup collider dạng trigger
            CircleCollider2D col = GetComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.5f;

            // Tự động destroy sau 10 giây
            Destroy(gameObject, 10f);
        }

        void Update()
        {
            // Xoay
            transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);

            // Lơ lửng lên xuống
            bobTimer += Time.deltaTime * bobSpeed;
            float newY = startPos.y + Mathf.Sin(bobTimer) * bobAmount;
            transform.position = new Vector3(
                transform.position.x,
                newY,
                transform.position.z
            );
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            // Kiểm tra nếu player nhặt
            if (other.CompareTag("Player"))
            {
                var player = other.GetComponent<Player_Archer>();
                if (player != null)
                {
                    ApplyPowerUp(player);
                    Pickup();
                }
            }
        }

        void ApplyPowerUp(Player_Archer player)
        {
            switch (powerUpType)
            {
                case POWERUP_TYPE.HEALTH:
                    player.Heal((int)value);
                    FloatingTextManager.Instance.ShowText(
                        "+" + (int)value + " HP",
                        Vector3.up,
                        Color.green,
                        player.transform.position
                    );
                    break;

                case POWERUP_TYPE.SPEED:
                    player.StartCoroutine(player.SpeedBoost(value / 100f, duration));
                    FloatingTextManager.Instance.ShowText(
                        "SPEED UP!",
                        Vector3.up,
                        Color.cyan,
                        player.transform.position
                    );
                    break;

                case POWERUP_TYPE.DAMAGE:
                    player.StartCoroutine(player.DamageBoost(value / 100f, duration));
                    FloatingTextManager.Instance.ShowText(
                        "POWER UP!",
                        Vector3.up,
                        Color.red,
                        player.transform.position
                    );
                    break;

                case POWERUP_TYPE.INVINCIBILITY:
                    player.StartCoroutine(player.Invincibility(duration));
                    FloatingTextManager.Instance.ShowText(
                        "INVINCIBLE!",
                        Vector3.up,
                        Color.yellow,
                        player.transform.position
                    );
                    break;
            }
        }

        void Pickup()
        {
            // Phát âm thanh
            if (pickupSound)
                SoundManager.PlaySfx(pickupSound);

            // Spawn effect
            if (pickupEffect)
                Instantiate(pickupEffect, transform.position, Quaternion.identity);

            // Destroy pickup
            Destroy(gameObject);
        }
    }
}
```

### Bước 2: Thêm Boost Method Vào Player_Archer

Trong `Player_Archer.cs`, thêm:

```csharp
public class Player_Archer : Enemy
{
    private float speedMultiplier = 1f;
    private float damageMultiplier = 1f;
    private bool isInvincible = false;

    public IEnumerator SpeedBoost(float multiplier, float duration)
    {
        speedMultiplier = 1f + multiplier;  // vd: 50% = 1.5x
        yield return new WaitForSeconds(duration);
        speedMultiplier = 1f;
    }

    public IEnumerator DamageBoost(float multiplier, float duration)
    {
        damageMultiplier = 1f + multiplier;
        yield return new WaitForSeconds(duration);
        damageMultiplier = 1f;
    }

    public IEnumerator Invincibility(float duration)
    {
        isInvincible = true;

        // Hiệu ứng nhấp nháy
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        float timer = 0;
        while (timer < duration)
        {
            sprite.color = new Color(1, 1, 1, 0.5f);
            yield return new WaitForSeconds(0.1f);
            sprite.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            timer += 0.2f;
        }

        isInvincible = false;
    }

    // Chỉnh sửa method hiện có để dùng multiplier:
    public override void FixedUpdate()
    {
        // Áp dụng speed multiplier
        velocity.x = direction.x * moveSpeed * speedMultiplier;
        // ... phần còn lại của di chuyển
    }

    void Shoot()
    {
        // Áp dụng damage multiplier
        float finalDamage = arrowDamage * damageMultiplier;
        // ... spawn arrow với finalDamage
    }

    public override void TakeDamage(...)
    {
        // Kiểm tra bất tử
        if (isInvincible)
            return;

        // Damage bình thường
        base.TakeDamage(...);
    }
}
```

### Bước 3: Tạo PowerUp Prefab

**Health Pack**:
1. Tạo sprite: Icon trái tim
2. Tạo GameObject: `PowerUp_Health`
3. Thêm component:
   - SpriteRenderer (sprite trái tim)
   - CircleCollider2D (trigger)
   - Rigidbody2D (kinematic)
   - PowerUp script
4. Cấu hình PowerUp:
   ```
   PowerUp Type: HEALTH
   Value: 25
   Pickup Sound: Heal sound
   ```
5. Lưu dạng prefab

**Speed Boost**:
- Tương tự Health Pack
- Dùng sprite tia chớp
- PowerUp Type: SPEED
- Value: 50 (50% speed boost)
- Duration: 5

### Bước 4: Thêm Drop System Vào Enemy

Trong `Enemy.cs`, thêm:

```csharp
[Header("Drops")]
public GameObject[] possibleDrops;
[Range(0, 100)]
public float dropChance = 20f;  // 20% cơ hội

public virtual void Die()
{
    // ... code hiện có ...

    // Kiểm tra nếu nên rơi item
    if (possibleDrops.Length > 0 && Random.Range(0, 100f) < dropChance)
    {
        // Rơi ngẫu nhiên
        GameObject drop = possibleDrops[Random.Range(0, possibleDrops.Length)];
        Instantiate(drop, transform.position + Vector3.up, Quaternion.identity);
    }

    // ... code hiện có ...
}
```

### Bước 5: Cấu Hình Enemy Drop

1. Chọn enemy prefab (vd: Goblin)
2. Cấu hình drop:
   ```
   Possible Drops (Size: 2):
     Element 0: PowerUp_Health
     Element 1: PowerUp_Speed
   Drop Chance: 20
   ```

### Bước 6: Test

1. Chơi game
2. Giết enemy
3. Khoảng 20% nên rơi power-up
4. Đi player qua power-up
5. Xác minh:
   - ✓ Health pack hồi máu player
   - ✓ Speed boost tăng di chuyển
   - ✓ Effect hiển thị floating text
   - ✓ Power-up xoay và lơ lửng
   - ✓ Power-up tự destroy sau 10s

### Kết Quả Mong Đợi

```
Enemy chết:
  20% cơ hội → spawn power-up
  Power-up: Xoay, lơ lửng lên/xuống

Player chạm power-up:
  Health Pack: Player hồi +25 HP
               Text xanh "+25 HP" xuất hiện

  Speed Boost: Player di chuyển nhanh 50% trong 5 giây
               Text cyan "SPEED UP!" xuất hiện
               Tốc độ bình thường trở lại sau 5s
```

---

## Cách Tạo Custom Health Bar

### Mục Tiêu
Tạo thanh máu fancy với background, fill, damage overlay, và animation mượt.

### Yêu Cầu
- Đọc `04_He_Thong_UI.md` (phần Health Bar)
- Hiểu Canvas và UI component

### Bước 1: Tạo Health Bar Prefab

1. Tạo GameObject rỗng: `FancyHealthBar`
2. Thêm Canvas component:
   ```
   Render Mode: World Space
   Width: 100
   Height: 20
   Scale: 0.01, 0.01, 0.01
   ```

3. Thêm child UI element:

```
FancyHealthBar (Canvas)
├─ Background (Image)
│   └─ Color: Xám đậm (0.2, 0.2, 0.2)
├─ DamageOverlay (Image)
│   └─ Color: Đỏ (1, 0, 0, 0.5)
├─ HealthFill (Image)
│   └─ Color: Xanh (0, 1, 0)
└─ Border (Image)
    └─ Color: Viền trắng
```

### Bước 2: Tạo FancyHealthBar Script

Tạo `FancyHealthBar.cs`:

```csharp
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace RGame
{
    public class FancyHealthBar : MonoBehaviour
    {
        [Header("References")]
        public Image healthFill;
        public Image damageOverlay;
        public Image background;

        [Header("Settings")]
        public float updateSpeed = 5f;
        public float damageDelay = 0.5f;
        public float hideDelay = 2f;
        public float fadeSpeed = 2f;

        private float targetFill = 1f;
        private float damageFill = 1f;
        private Transform target;
        private Vector3 offset;
        private CanvasGroup canvasGroup;

        void Start()
        {
            // Thêm canvas group để fade
            canvasGroup = GetComponent<CanvasGroup>();
            if (!canvasGroup)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();

            // Bắt đầu ẩn
            canvasGroup.alpha = 0;
        }

        public void Initialize(Transform _target, Vector3 _offset)
        {
            target = _target;
            offset = _offset;
        }

        void Update()
        {
            // Theo target
            if (target)
            {
                transform.position = target.position + offset;

                // Quay về camera
                transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                                Camera.main.transform.rotation * Vector3.up);
            }

            // Smooth health fill
            if (healthFill.fillAmount != targetFill)
            {
                healthFill.fillAmount = Mathf.Lerp(
                    healthFill.fillAmount,
                    targetFill,
                    updateSpeed * Time.deltaTime
                );
            }

            // Smooth damage overlay (delay)
            if (damageOverlay.fillAmount != targetFill)
            {
                damageOverlay.fillAmount = Mathf.Lerp(
                    damageOverlay.fillAmount,
                    targetFill,
                    updateSpeed * 0.5f * Time.deltaTime  // Chậm hơn health
                );
            }
        }

        public void UpdateHealth(float currentHealth, float maxHealth)
        {
            // Hiển thị bar
            StopAllCoroutines();
            canvasGroup.alpha = 1;

            // Tính fill amount
            targetFill = Mathf.Clamp01(currentHealth / maxHealth);

            // Bắt đầu damage overlay animation
            StartCoroutine(DamageOverlayCo());

            // Tự động ẩn sau delay
            if (targetFill > 0)
                StartCoroutine(HideBarCo());
            else
                gameObject.SetActive(false);  // Chết
        }

        IEnumerator DamageOverlayCo()
        {
            // Đợi trước khi damage overlay theo kịp
            yield return new WaitForSeconds(damageDelay);

            // Giảm damage overlay mượt
            float timer = 0;
            float startFill = damageOverlay.fillAmount;

            while (timer < 1f)
            {
                timer += Time.deltaTime * updateSpeed;
                damageOverlay.fillAmount = Mathf.Lerp(startFill, targetFill, timer);
                yield return null;
            }
        }

        IEnumerator HideBarCo()
        {
            yield return new WaitForSeconds(hideDelay);

            // Fade out
            float timer = 0;
            while (timer < 1f)
            {
                timer += Time.deltaTime * fadeSpeed;
                canvasGroup.alpha = Mathf.Lerp(1, 0, timer);
                yield return null;
            }
        }
    }
}
```

### Bước 3: Cài Đặt UI Element

Cấu hình RectTransform:

```
Background:
- Anchor: Stretch (fill parent)
- Offset: 0, 0, 0, 0
- Image Type: Sliced (viền optional)

HealthFill:
- Anchor: Left
- Image Type: Filled
- Fill Method: Horizontal
- Fill Origin: Left
- Fill Amount: 1

DamageOverlay:
- Giống HealthFill
- Color: Đỏ trong suốt

Border:
- Stretch để fill
- Sprite: Sprite viền
```

### Bước 4: Dùng Trong Enemy

Trong `Enemy.cs` Start() method, thay thế spawn thanh máu:

```csharp
public virtual void Start()
{
    // ... code hiện có ...

    // Spawn fancy health bar
    var healthBarPrefab = (FancyHealthBar)Resources.Load("FancyHealthBar", typeof(FancyHealthBar));
    var fancyBar = Instantiate(healthBarPrefab, transform.position + (Vector3)healthBarOffset, Quaternion.identity);
    fancyBar.Initialize(transform, healthBarOffset);

    // Lưu reference (cần chỉnh sửa Enemy class)
    fancyHealthBar = fancyBar;

    // ... code hiện có ...
}
```

### Bước 5: Cập Nhật Health Bar Call

Chỉnh sửa `TakeDamage()` để dùng fancy health bar:

```csharp
public void TakeDamage(...)
{
    // ... code hiện có ...

    currentHealth -= (int)damage;

    // Cập nhật fancy health bar
    if (fancyHealthBar)
        fancyHealthBar.UpdateHealth(currentHealth, health);

    // ... code hiện có ...
}
```

### Kết Quả Mong Đợi

```
Enemy nhận damage:
  Thanh xanh co ngay lập tức → 70%
  Overlay đỏ giữ ở 100% trong 0.5s
  Overlay đỏ co mượt → 70%
  Thanh máu hiển thị trong 2 giây
  Thanh máu fade out

Visual:
Trước damage:  [████████████████████] 100%
Sau damage:    [██████████████      ] 70% (xanh)
               [████████████████    ] ~85% (overlay đỏ)
               (overlay đỏ theo kịp theo thời gian)
```

---

## Cách Thêm Sound Effect

### Mục Tiêu
Thêm âm thanh bước chân cho di chuyển player và âm thanh hurt cho enemy nhận damage.

### Yêu Cầu
- Đọc `05_Cac_Manager.md` (phần SoundManager)
- Có sẵn file audio

### Bước 1: Import Audio File

1. Import audio clip:
   ```
   Assets/Audio/Sound/Player/
   ├─ footstep1.wav
   ├─ footstep2.wav
   ├─ footstep3.wav
   ├─ hurt1.wav
   └─ hurt2.wav
   ```

2. Chọn tất cả file audio trong Project window
3. Setting Inspector:
   ```
   Force To Mono: true (cho sound effect)
   Load Type: Decompress On Load (file nhỏ)
   Compression Format: PCM (chất lượng tốt nhất cho âm thanh ngắn)
   ```

### Bước 2: Thêm Sound Field Vào Player

Trong `Player_Archer.cs`:

```csharp
public class Player_Archer : Enemy
{
    [Header("Sounds")]
    public AudioClip[] footstepSounds;
    [Range(0, 1)]
    public float footstepVolume = 0.3f;
    public float footstepInterval = 0.4f;

    private float footstepTimer = 0f;

    // ... code hiện có ...
}
```

### Bước 3: Phát Footstep Sound

Trong `Player_Archer.cs` FixedUpdate():

```csharp
public override void FixedUpdate()
{
    base.FixedUpdate();

    // ... code di chuyển hiện có ...

    // Phát footstep khi di chuyển
    if (Mathf.Abs(velocity.x) > 0.1f && controller.collisions.below)
    {
        footstepTimer += Time.fixedDeltaTime;

        if (footstepTimer >= footstepInterval)
        {
            // Phát footstep ngẫu nhiên
            if (footstepSounds.Length > 0)
            {
                SoundManager.PlaySfx(footstepSounds, footstepVolume);
            }

            footstepTimer = 0f;
        }
    }
    else
    {
        footstepTimer = 0f;
    }

    // ... phần còn lại của di chuyển
}
```

### Bước 4: Thêm Randomized Pitch

Để đa dạng, thêm pitch randomization:

```csharp
public static void PlaySfxWithPitch(AudioClip clip, float volume, float pitchVariation = 0.1f)
{
    if (Instance != null && clip != null)
    {
        // Randomize pitch
        Instance.soundFx.pitch = Random.Range(1f - pitchVariation, 1f + pitchVariation);

        // Phát âm thanh
        Instance.soundFx.PlayOneShot(clip, volume);

        // Reset pitch
        Instance.soundFx.pitch = 1f;
    }
}
```

Sử dụng:

```csharp
SoundManager.PlaySfxWithPitch(
    footstepSounds[Random.Range(0, footstepSounds.Length)],
    footstepVolume,
    0.15f  // ±15% biến thiên pitch
);
```

### Bước 5: Thêm Vào SoundManager

Trong `SoundManager.cs`, thêm method tiện lợi:

```csharp
public static void PlayFootstep(AudioClip[] clips, float volume = 0.3f)
{
    if (clips == null || clips.Length == 0)
        return;

    // Clip ngẫu nhiên
    AudioClip clip = clips[Random.Range(0, clips.Length)];

    // Pitch ngẫu nhiên để đa dạng
    Instance.soundFx.pitch = Random.Range(0.9f, 1.1f);
    Instance.soundFx.PlayOneShot(clip, volume * SoundVolume);
    Instance.soundFx.pitch = 1f;
}
```

Sử dụng đơn giản:

```csharp
SoundManager.PlayFootstep(footstepSounds, footstepVolume);
```

### Bước 6: Gán Trong Inspector

1. Chọn Player_Archer trong scene
2. Tìm header "Sounds"
3. Đặt Footstep Sounds array size: 3
4. Kéo footstep1/2/3 vào array slot
5. Đặt Footstep Volume: 0.3
6. Đặt Footstep Interval: 0.4

### Bước 7: Test

1. Chơi game
2. Di chuyển player trái/phải
3. Xác minh:
   - ✓ Footstep phát mỗi 0.4 giây khi di chuyển
   - ✓ Clip footstep ngẫu nhiên phát
   - ✓ Không có footstep khi đứng yên
   - ✓ Không có footstep khi ở trên không
   - ✓ Pitch biến thiên nhẹ

---

## Cách Triển Khai Save/Load System

### Mục Tiêu
Lưu tiến trình player, level mở khóa, và coin dùng PlayerPrefs.

### Yêu Cầu
- Hiểu GlobalValue static class
- Kiến thức cơ bản về serialization

### Bước 1: Tạo SaveData Class

Tạo `SaveData.cs`:

```csharp
using UnityEngine;
using System;

namespace RGame
{
    [Serializable]
    public class SaveData
    {
        // Tiến trình Player
        public int currentLevel = 1;
        public int highestLevelUnlocked = 1;
        public int totalCoins = 0;

        // Nâng cấp Player
        public int healthLevel = 0;
        public int damageLevel = 0;
        public int speedLevel = 0;

        // Level Stars (3 sao mỗi level)
        public int[] levelStars = new int[10];  // 10 level

        // Cài đặt
        public bool soundEnabled = true;
        public bool musicEnabled = true;

        // Thống kê
        public int totalKills = 0;
        public int totalDeaths = 0;
        public float totalPlayTime = 0f;

        // Constructor với giá trị mặc định
        public SaveData()
        {
            // Khởi tạo mảng level stars
            for (int i = 0; i < levelStars.Length; i++)
                levelStars[i] = 0;
        }
    }
}
```

### Bước 2: Tạo SaveSystem

Tạo `SaveSystem.cs`:

```csharp
using UnityEngine;

namespace RGame
{
    public static class SaveSystem
    {
        private const string SAVE_KEY = "GameSaveData";

        public static void Save(SaveData data)
        {
            // Convert sang JSON
            string json = JsonUtility.ToJson(data);

            // Lưu vào PlayerPrefs
            PlayerPrefs.SetString(SAVE_KEY, json);
            PlayerPrefs.Save();

            Debug.Log("Game Saved!");
        }

        public static SaveData Load()
        {
            // Kiểm tra nếu save tồn tại
            if (PlayerPrefs.HasKey(SAVE_KEY))
            {
                // Load JSON
                string json = PlayerPrefs.GetString(SAVE_KEY);

                // Convert sang object
                SaveData data = JsonUtility.FromJson<SaveData>(json);

                Debug.Log("Game Loaded!");
                return data;
            }
            else
            {
                // Không có file save, trả về save mới
                Debug.Log("No save file found. Creating new save.");
                return new SaveData();
            }
        }

        public static void DeleteSave()
        {
            PlayerPrefs.DeleteKey(SAVE_KEY);
            PlayerPrefs.Save();
            Debug.Log("Save deleted!");
        }

        public static bool HasSave()
        {
            return PlayerPrefs.HasKey(SAVE_KEY);
        }
    }
}
```

### Bước 3: Tích Hợp Với GlobalValue

Trong `GlobalValue.cs`, chỉnh sửa để dùng SaveSystem:

```csharp
public static class GlobalValue
{
    // Save data hiện tại
    private static SaveData currentSave;

    // Property đọc/ghi vào save data
    public static int levelPlaying
    {
        get => currentSave.currentLevel;
        set
        {
            currentSave.currentLevel = value;
            SaveGame();
        }
    }

    public static int SavedCoins
    {
        get => currentSave.totalCoins;
        set
        {
            currentSave.totalCoins = value;
            SaveGame();
        }
    }

    public static bool isSound
    {
        get => currentSave.soundEnabled;
        set
        {
            currentSave.soundEnabled = value;
            SaveGame();
        }
    }

    public static bool isMusic
    {
        get => currentSave.musicEnabled;
        set
        {
            currentSave.musicEnabled = value;
            SaveGame();
        }
    }

    // Initialize (gọi khi game start)
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        currentSave = SaveSystem.Load();
    }

    // Lưu game
    public static void SaveGame()
    {
        SaveSystem.Save(currentSave);
    }

    // Lấy level star
    public static int GetLevelStars(int level)
    {
        if (level >= 1 && level <= currentSave.levelStars.Length)
            return currentSave.levelStars[level - 1];
        return 0;
    }

    // Đặt level star
    public static void SetLevelStars(int level, int stars)
    {
        if (level >= 1 && level <= currentSave.levelStars.Length)
        {
            // Chỉ lưu nếu tốt hơn trước
            if (stars > currentSave.levelStars[level - 1])
            {
                currentSave.levelStars[level - 1] = stars;
                SaveGame();
            }
        }
    }

    // Reset tất cả data
    public static void ResetAllData()
    {
        SaveSystem.DeleteSave();
        currentSave = new SaveData();
    }
}
```

### Bước 4: Auto-Save Trên Sự Kiện Chính

Trong `GameManager.cs` Victory():

```csharp
public void Victory()
{
    // ... code hiện có ...

    // Lưu tiến trình level
    if (GlobalValue.levelPlaying > GlobalValue.currentSave.highestLevelUnlocked)
    {
        GlobalValue.currentSave.highestLevelUnlocked = GlobalValue.levelPlaying + 1;
    }

    // Lưu sao
    GlobalValue.SetLevelStars(GlobalValue.levelPlaying, levelStarGot);

    // Lưu game
    GlobalValue.SaveGame();

    // ... code hiện có ...
}
```

Trong shop khi mua nâng cấp:

```csharp
public void BuyHealthUpgrade()
{
    if (GlobalValue.SavedCoins >= healthUpgradeCost)
    {
        GlobalValue.SavedCoins -= healthUpgradeCost;  // Auto-save
        GlobalValue.UpgradePlayerHealth();
    }
}
```

### Bước 5: Thêm Button Save/Load Thủ Công

Trong main menu:

```csharp
public class MainMenuHomeScene : MonoBehaviour
{
    public void OnSaveButtonClick()
    {
        GlobalValue.SaveGame();
        ShowMessage("Game Saved!");
    }

    public void OnLoadButtonClick()
    {
        // Reload scene để áp dụng data đã load
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        ShowMessage("Game Loaded!");
    }

    public void OnResetDataButtonClick()
    {
        if (ConfirmDialog("Reset all progress?"))
        {
            GlobalValue.ResetAllData();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
```

### Bước 6: Test Save System

1. Chơi game, hoàn thành level 1 với 3 sao
2. Thu thập 100 coin
3. Thoát game (đóng Unity hoặc build)
4. Khởi động lại game
5. Xác minh:
   - ✓ Level 2 được mở khóa
   - ✓ 100 coin vẫn còn
   - ✓ Level 1 hiển thị 3 sao
   - ✓ Cài đặt (sound/music) được bảo toàn

### Hành Vi Mong Đợi

```
Dữ liệu PlayerPrefs (lưu trong registry/plist):
{
  "currentLevel": 2,
  "highestLevelUnlocked": 2,
  "totalCoins": 100,
  "healthLevel": 1,
  "damageLevel": 0,
  "speedLevel": 0,
  "levelStars": [3, 0, 0, 0, 0, 0, 0, 0, 0, 0],
  "soundEnabled": true,
  "musicEnabled": false,
  "totalKills": 47,
  "totalDeaths": 2,
  "totalPlayTime": 125.5
}

Lưu vào: PlayerPrefs["GameSaveData"]
Vị trí (Windows): Registry HKCU\Software\[CompanyName]\[ProductName]
```

---

## Cách Thay Đổi Độ Khó Game

### Mục Tiêu
Thêm chế độ độ khó Easy/Normal/Hard với máu và damage enemy khác nhau.

### Yêu Cầu
- Hiểu UpgradedCharacterParameter
- Đọc tài liệu Enemy System

### Bước 1: Tạo Difficulty Enum

Tạo `GameDifficulty.cs`:

```csharp
namespace RGame
{
    public enum GameDifficulty
    {
        Easy,
        Normal,
        Hard,
        Insane
    }

    public static class DifficultySettings
    {
        // Độ khó hiện tại
        public static GameDifficulty CurrentDifficulty = GameDifficulty.Normal;

        // Multiplier máu enemy
        public static float GetHealthMultiplier()
        {
            switch (CurrentDifficulty)
            {
                case GameDifficulty.Easy:
                    return 0.7f;  // 70% máu
                case GameDifficulty.Normal:
                    return 1.0f;  // 100% máu
                case GameDifficulty.Hard:
                    return 1.5f;  // 150% máu
                case GameDifficulty.Insane:
                    return 2.0f;  // 200% máu
                default:
                    return 1.0f;
            }
        }

        // Multiplier damage enemy
        public static float GetDamageMultiplier()
        {
            switch (CurrentDifficulty)
            {
                case GameDifficulty.Easy:
                    return 0.5f;  // 50% damage
                case GameDifficulty.Normal:
                    return 1.0f;  // 100% damage
                case GameDifficulty.Hard:
                    return 1.25f; // 125% damage
                case GameDifficulty.Insane:
                    return 1.5f;  // 150% damage
                default:
                    return 1.0f;
            }
        }

        // Multiplier tốc độ enemy
        public static float GetSpeedMultiplier()
        {
            switch (CurrentDifficulty)
            {
                case GameDifficulty.Easy:
                    return 0.8f;  // 80% tốc độ
                case GameDifficulty.Normal:
                    return 1.0f;  // 100% tốc độ
                case GameDifficulty.Hard:
                    return 1.1f;  // 110% tốc độ
                case GameDifficulty.Insane:
                    return 1.3f;  // 130% tốc độ
                default:
                    return 1.0f;
            }
        }

        // Phần thưởng coin
        public static float GetCoinMultiplier()
        {
            switch (CurrentDifficulty)
            {
                case GameDifficulty.Easy:
                    return 0.8f;  // 80% coin
                case GameDifficulty.Normal:
                    return 1.0f;  // 100% coin
                case GameDifficulty.Hard:
                    return 1.5f;  // 150% coin
                case GameDifficulty.Insane:
                    return 2.0f;  // 200% coin
                default:
                    return 1.0f;
            }
        }
    }
}
```

### Bước 2: Áp Dụng Độ Khó Cho Enemy

Trong `Enemy.cs` Start() method:

```csharp
public virtual void Start()
{
    // ... code hiện có ...

    // Áp dụng difficulty multiplier
    health = (int)(health * DifficultySettings.GetHealthMultiplier());
    currentHealth = health;

    walkSpeed *= DifficultySettings.GetSpeedMultiplier();

    // ... phần còn lại của Start()
}
```

### Bước 3: Áp Dụng Cho Enemy Attack

Trong `EnemyMeleeAttack.cs`:

```csharp
void Start()
{
    // ... code hiện có ...

    // Áp dụng độ khó cho damage
    dealDamage *= DifficultySettings.GetDamageMultiplier();
}
```

Trong `EnemyRangeAttack.cs`:

```csharp
void Start()
{
    // ... code hiện có ...

    damage *= DifficultySettings.GetDamageMultiplier();
}
```

### Bước 4: Áp Dụng Cho Coin Reward

Trong `GiveCoinWhenDie.cs`:

```csharp
public void GiveCoin()
{
    // Tính coin với difficulty multiplier
    int coinAmount = Random.Range(coinGiveMin, coinGiveMax + 1);
    coinAmount = (int)(coinAmount * DifficultySettings.GetCoinMultiplier());

    GlobalValue.SavedCoins += coinAmount;

    // ... spawn coin effect
}
```

### Bước 5: Tạo Difficulty Selection UI

Trong main menu, thêm button độ khó:

```csharp
public class DifficultySelector : MonoBehaviour
{
    public Text currentDifficultyText;

    void Start()
    {
        UpdateDifficultyText();
    }

    public void SetEasy()
    {
        DifficultySettings.CurrentDifficulty = GameDifficulty.Easy;
        UpdateDifficultyText();
        SoundManager.Click();
    }

    public void SetNormal()
    {
        DifficultySettings.CurrentDifficulty = GameDifficulty.Normal;
        UpdateDifficultyText();
        SoundManager.Click();
    }

    public void SetHard()
    {
        DifficultySettings.CurrentDifficulty = GameDifficulty.Hard;
        UpdateDifficultyText();
        SoundManager.Click();
    }

    public void SetInsane()
    {
        DifficultySettings.CurrentDifficulty = GameDifficulty.Insane;
        UpdateDifficultyText();
        SoundManager.Click();
    }

    void UpdateDifficultyText()
    {
        currentDifficultyText.text = "Difficulty: " + DifficultySettings.CurrentDifficulty.ToString();

        // Mã màu
        switch (DifficultySettings.CurrentDifficulty)
        {
            case GameDifficulty.Easy:
                currentDifficultyText.color = Color.green;
                break;
            case GameDifficulty.Normal:
                currentDifficultyText.color = Color.white;
                break;
            case GameDifficulty.Hard:
                currentDifficultyText.color = new Color(1f, 0.5f, 0f);  // Cam
                break;
            case GameDifficulty.Insane:
                currentDifficultyText.color = Color.red;
                break;
        }
    }
}
```

### Bước 6: Lưu Difficulty Setting

Thêm vào `SaveData.cs`:

```csharp
public class SaveData
{
    // ... field hiện có ...

    public int difficultyLevel = 1;  // 0=Easy, 1=Normal, 2=Hard, 3=Insane

    // ... phần còn lại của class
}
```

Thêm vào `GlobalValue.cs`:

```csharp
public static GameDifficulty Difficulty
{
    get => (GameDifficulty)currentSave.difficultyLevel;
    set
    {
        currentSave.difficultyLevel = (int)value;
        DifficultySettings.CurrentDifficulty = value;
        SaveGame();
    }
}
```

### Bước 7: Test Độ Khó

**Easy Mode** (Level 1):
- Máu Goblin: 70 (đã là 100)
- Damage Goblin: 10 (đã là 20)
- Tốc độ Goblin: 2.4 (đã là 3)
- Coin: 8 (đã là 10)

**Hard Mode** (Level 1):
- Máu Goblin: 150 (đã là 100)
- Damage Goblin: 25 (đã là 20)
- Tốc độ Goblin: 3.3 (đã là 3)
- Coin: 15 (đã là 10)

**Insane Mode** (Level 1):
- Máu Goblin: 200 (đã là 100)
- Damage Goblin: 30 (đã là 20)
- Tốc độ Goblin: 3.9 (đã là 3)
- Coin: 20 (đã là 10)

### Kết Quả Mong Đợi

```
Main Menu:
┌──────────────────────────┐
│   Select Difficulty:     │
│                          │
│  [Easy]   (Xanh)         │
│  [Normal] (Trắng) ✓      │
│  [Hard]   (Cam)          │
│  [Insane] (Đỏ)           │
│                          │
│  Current: Normal         │
└──────────────────────────┘

In-Game (Normal → Hard):
Trước: Goblin có 100 HP
Sau:   Goblin có 150 HP (+50%)
```

---

## Tóm Tắt

Tài liệu Hướng Dẫn Thực Hành này đã đề cập:

1. **Thêm Loại Enemy Mới** - Tạo flying Ghost enemy
2. **Custom UI Panel** - Statistics panel với theo dõi kill
3. **Weapon Effect Mới** - Lightning lan truyền giữa enemy
4. **Chỉnh Sửa Player Stats** - 3 phương pháp (Inspector, Prefab, ScriptableObject)
5. **Thêm Level Mới** - Cấu hình wave tùy chỉnh
6. **Power-Up Item** - Health pack và speed boost với drop system
7. **Custom Health Bar** - Thanh máu fancy với damage overlay
8. **Sound Effect** - Bước chân và âm thanh hurt
9. **Save/Load System** - Save system dựa trên JSON PlayerPrefs
10. **Chế Độ Độ Khó** - Easy/Normal/Hard/Insane với multiplier

Tất cả hướng dẫn bao gồm:
- Mục tiêu và yêu cầu rõ ràng
- Hướng dẫn từng bước
- Ví dụ code hoàn chỉnh
- Kết quả mong đợi và bước test

**Bước Tiếp Theo**:
- Dùng các hướng dẫn này làm template cho chỉnh sửa của bạn
- Kết hợp nhiều hướng dẫn cho tính năng phức tạp
- Đọc hướng dẫn troubleshooting cho vấn đề thường gặp

---

**Cập Nhật Lần Cuối**: 2025
**File**: `Documents/10_Huong_Dan_Thuc_Hanh.md`
