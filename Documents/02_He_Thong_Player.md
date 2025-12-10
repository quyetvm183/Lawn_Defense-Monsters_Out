---
**🌐 Ngôn ngữ:** Tiếng Việt
**📄 File gốc:** [02_Player_System_Complete.md](02_Player_System_Complete.md)
**🔄 Cập nhật lần cuối:** 2025-01-30
---

# Hệ Thống Player - Hướng Dẫn Đầy Đủ

**Yêu cầu:** Đọc `00_Unity_Co_Ban.md` và `01_Kien_Truc_Project.md`
**Đối tượng:** Developer muốn hiểu hoặc sửa đổi nhân vật player
**Thời gian đọc ước tính:** 45 phút
**Vị trí script:** `Assets/_MonstersOut/Scripts/Player/Player_Archer.cs`

---

## Mục Lục
1. [Tổng Quan Hệ Thống](#1-tổng-quan-hệ-thống)
2. [Kiến Trúc Player](#2-kiến-trúc-player)
3. [Hệ Thống Auto-Targeting](#3-hệ-thống-auto-targeting)
4. [Tính Toán Trajectory](#4-tính-toán-trajectory)
5. [Cơ Chế Bắn](#5-cơ-chế-bắn)
6. [Hệ Thống Di Chuyển](#6-hệ-thống-di-chuyển)
7. [Hệ Thống Damage & Health](#7-hệ-thống-damage--health)
8. [Điều Khiển Animation](#8-điều-khiển-animation)
9. [Cấu Hình Inspector](#9-cấu-hình-inspector)
10. [Cách Sửa Đổi](#10-cách-sửa-đổi)
11. [Vấn Đề Thường Gặp & Giải Pháp](#11-vấn-đề-thường-gặp--giải-pháp)

---

## 1. Tổng Quan Hệ Thống

### 1.1 Hệ Thống Player Là Gì?

Hệ Thống Player điều khiển **nhân vật Archer (cung thủ)** - đơn vị phòng thủ của người chơi, tự động bắn mũi tên vào kẻ địch đang tiến lại.

**Đặc Điểm Chính:**
- 🏹 **Auto-Shooting (Tự động bắn):** Player tự động ngắm và bắn kẻ địch
- 🎯 **Trajectory Calculation (Tính toán quỹ đạo):** Mô phỏng vật lý để trúng mục tiêu di chuyển
- 🚶 **Movable (Di chuyển được):** Player có thể đi trái/phải (khác tower defense truyền thống)
- ❤️ **Health System (Hệ thống máu):** Có thể nhận damage và chết
- ⚡ **Weapon Effect (Hiệu ứng vũ khí):** Mũi tên có thể gây độc, đốt, đóng băng enemy
- 📈 **Upgradeable (Có thể nâng cấp):** Stat được lưu qua UpgradedCharacterParameter

### 1.2 Thiết Kế Độc Đáo: Player Kế Thừa Từ Enemy

**Điều này khác thường nhưng rất thông minh!**

```
MonoBehaviour
      │
      └─── Enemy.cs (class cơ sở)
           ├─ Health system (currentHealth, maxHealth)
           ├─ Damage handling (TakeDamage, Die, Hit)
           ├─ Effect (Poison, Burn, Freeze, Shock)
           ├─ Animation helper
           ├─ State machine (SPAWNING, WALK, ATTACK, HIT, DEATH)
           │
           └─── Player_Archer.cs (PLAYER)
                ├─ Hệ thống auto-targeting
                ├─ Tính toán trajectory
                ├─ Bắn mũi tên
                └─ Điều khiển di chuyển
```

**Tại Sao Player Kế Thừa Từ Enemy?**
- ✅ **Code Reuse (Tái sử dụng code):** Enemy đã có health, damage, effect - player cần tất cả
- ✅ **Unified System (Hệ thống thống nhất):** Một TakeDamage() cho mọi thứ
- ✅ **Consistent Behavior (Hành vi nhất quán):** Player và enemy hoạt động giống nhau
- ✅ **Less Maintenance (Bảo trì ít hơn):** Sửa bug một lần trong Enemy, ảnh hưởng cả hai

**Player Thêm Gì Trên Enemy:**
- Auto-targeting enemy
- Tính toán ballistic trajectory (quỹ đạo đạn đạo)
- Spawn và bắn arrow
- Hệ thống reload/cooldown
- Custom movement (có thể đi, khác hầu hết enemy)

### 1.3 Sơ Đồ Component Của Player

```
Player GameObject
├─ Transform                        ← Vị trí, xoay, scale
├─ Sprite Renderer                  ← Hình ảnh (sprite archer)
├─ Box Collider 2D (x2)            ← Phát hiện va chạm (body + hitbox)
├─ Animator                         ← Controller animation
├─ Controller2D                     ← Controller vật lý tùy chỉnh
├─ CheckTargetHelper               ← Phát hiện enemy (raycast)
└─ Player_Archer (Script)          ← Logic player chính
    │
    ├── Kế thừa từ Enemy:
    │   ├─ health, currentHealth
    │   ├─ TakeDamage(), Die(), Hit()
    │   ├─ Freeze(), Poison(), Burning(), Shoking()
    │   ├─ enemyState, enemyEffect
    │   ├─ anim (Animator)
    │   ├─ checkTarget (CheckTargetHelper)
    │   └─ healthBar (HealthBarEnemyNew)
    │
    └── Riêng Của Player:
        ├─ Coroutine auto-targeting
        ├─ Tính toán trajectory
        ├─ Bắn arrow
        ├─ Hệ thống reload
        └─ Logic di chuyển
```

---

## 2. Kiến Trúc Player

### 2.1 Cấu Trúc Class

**File:** `Player_Archer.cs` (445 dòng)

**Chuỗi Kế Thừa:**
```csharp
MonoBehaviour  →  Enemy  →  Player_Archer
```

**Interface Được Triển Khai:**
```csharp
public class Player_Archer : Enemy, ICanTakeDamage, IListener
```
- `ICanTakeDamage` - Kế thừa từ Enemy, cho phép nhận damage
- `IListener` - Nhận event game state (IPlay, IPause, v.v.)

### 2.2 Property Chính

**Cấu Hình Bắn Arrow:**
```csharp
[Header("ARROW SHOOT")]
public float shootRate = 1;           // Giây giữa các phát bắn
public float force = 20;              // Lực bắn arrow
[Range(0.01f, 0.1f)]
public float stepCheck = 0.1f;        // Độ chính xác trajectory
public float stepAngle = 1;           // Bước lặp góc
public float gravityScale = 3.5f;     // Trọng lực arrow
public bool onlyShootTargetInFront = true;  // Chỉ bắn phía trước

[Header("ARROW DAMAGE")]
public ArrowProjectile arrow;         // Prefab arrow
public WeaponEffect weaponEffect;     // Poison, burn, freeze, v.v.
public int arrowDamage = 30;          // Damage cơ bản
public Transform firePostion;         // Điểm spawn arrow
```

**Sound Effect:**
```csharp
[Header("Sound")]
public float soundShootVolume = 0.5f;
public AudioClip[] soundShoot;        // Sound bắn ngẫu nhiên
```

**Internal State:**
```csharp
private Vector2 _direction;           // Hướng di chuyển
private float velocityXSmoothing = 0; // Di chuyển mượt
private bool isAvailable = true;      // Có thể bắn? (không reload)
private bool isLoading = false;       // Đang reload?
private bool isDead = false;          // Player chết?
private Transform target;             // Enemy target hiện tại
private Vector2 autoShootPoint;       // Điểm ngắm đã tính
```

### 2.3 Sơ Đồ Luồng Hệ Thống

```
Player Spawn
      │
      ▼
Start() - Khởi tạo
├─ Lấy component Controller2D
├─ Set hướng facing
├─ Lấy arrow damage từ UpgradedCharacterParameter
└─ Bắt đầu coroutine AutoCheckAndShoot()
      │
      ▼
┌─────────────────────────────────────────────┐
│  Coroutine AutoCheckAndShoot() (Vô hạn)   │
├─────────────────────────────────────────────┤
│  1. Đợi enemy trong range                   │
│  2. Phát hiện tất cả enemy (CircleCast)     │
│  3. Tìm enemy gần nhất                      │
│  4. Raycast xác nhận line-of-sight          │
│  5. Tính điểm ngắm                          │
│  6. Gọi Shoot()                             │
│  7. Đợi 0.2 giây                            │
│  8. Lặp lại                                 │
└─────────────────┬───────────────────────────┘
                  │
                  ▼
          Gọi Shoot()
                  │
                  ▼
    Coroutine CheckTarget()
    ├─ Tính góc trajectory tốt nhất
    ├─ Mô phỏng vật lý cho mỗi góc
    ├─ Tìm góc gần target nhất
    └─ Spawn arrow với force đã tính
                  │
                  ▼
         Coroutine ReloadingCo()
         ├─ Disable bắn (isAvailable = false)
         ├─ Play animation reload
         ├─ Đợi shootRate giây
         └─ Enable bắn (isAvailable = true)
                  │
                  ▼
         (Quay lại AutoCheckAndShoot)
```

---

## 3. Hệ Thống Auto-Targeting

Player tự động phát hiện và nhắm enemy mà không cần input thủ công.

### 3.1 Coroutine Phát Hiện

**Vị trí code:** `Player_Archer.cs:276-320`

```csharp
IEnumerator AutoCheckAndShoot()
{
    while (true)  // Vòng lặp vô hạn
    {
        // BƯỚC 1: Reset target
        target = null;
        yield return null;  // Đợi một frame

        // BƯỚC 2: Đợi cho đến khi phát hiện enemy
        // checkTargetHelper kiểm tra có enemy phía trước không
        while (!checkTargetHelper.CheckTarget((isFacingRight() ? 1 : -1)))
        {
            yield return null;  // Tiếp tục đợi mỗi frame
        }

        // BƯỚC 3: Phát hiện enemy! Tìm tất cả enemy trong bán kính lớn
        RaycastHit2D[] hits = Physics2D.CircleCastAll(
            transform.position,      // Điểm trung tâm (vị trí player)
            100,                      // Bán kính (rất lớn để bắt tất cả enemy)
            Vector2.zero,             // Hướng (không dùng, chỉ phát hiện trong khu vực)
            0,                        // Khoảng cách (0 = chỉ check tại center)
            GameManager.Instance.layerEnemy  // Chỉ phát hiện layer Enemy
        );

        // BƯỚC 4: Xử lý tất cả enemy trúng
        if (hits.Length > 0)
        {
            float closestDistance = 99999;  // Theo dõi enemy gần nhất

            foreach (var obj in hits)
            {
                // Thử lấy component ICanTakeDamage
                var checkEnemy = (ICanTakeDamage)obj.collider.gameObject
                    .GetComponent(typeof(ICanTakeDamage));

                if (checkEnemy != null)
                {
                    // Tính khoảng cách ngang đến enemy
                    float distance = Mathf.Abs(obj.transform.position.x -
                                               transform.position.x);

                    // Có gần hơn enemy gần nhất hiện tại không?
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        target = obj.transform;

                        // BƯỚC 5: Xác minh line-of-sight với raycast
                        var hit = Physics2D.Raycast(
                            transform.position,
                            (obj.point - (Vector2)transform.position),
                            100,
                            GameManager.Instance.layerEnemy
                        );

                        // Visualization debug (đường đỏ trong Scene view)
                        Debug.DrawRay(
                            transform.position,
                            (obj.point - (Vector2)transform.position) * 100,
                            Color.red
                        );

                        // BƯỚC 6: Set điểm ngắm
                        autoShootPoint = hit.point;
                        // Đảm bảo điểm ngắm không quá thấp
                        autoShootPoint.y = Mathf.Max(
                            autoShootPoint.y,
                            firePostion.position.y - 0.1f
                        );
                    }
                }
            }

            // BƯỚC 7: Bắn vào enemy gần nhất
            if (target)
            {
                Shoot();
                yield return new WaitForSeconds(0.2f);  // Tạm dừng ngắn
            }
        }
    }
}
```

**Cách Hoạt Động:**

1. **Continuous Detection Loop (Vòng lặp phát hiện liên tục):** Chạy mãi mãi khi player active
2. **Wait for Enemy (Đợi enemy):** Tạm dừng cho đến khi CheckTargetHelper phát hiện thứ gì đó
3. **Find All Enemies (Tìm tất cả enemy):** Dùng CircleCast để lấy tất cả enemy trong vùng rộng
4. **Select Closest (Chọn gần nhất):** So sánh khoảng cách, chọn enemy gần nhất
5. **Verify Line-of-Sight (Xác minh tầm nhìn):** Raycast đảm bảo không có vật cản
6. **Calculate Aim Point (Tính điểm ngắm):** Set nơi cần bắn
7. **Fire Arrow (Bắn mũi tên):** Gọi method Shoot()
8. **Brief Cooldown (Cooldown ngắn):** Tạm dừng 0.2 giây trước lần lặp tiếp

**Sơ Đồ Trực Quan:**
```
Vị trí Player
      │
      │ CheckTargetHelper (phát hiện hình nón phía trước)
      │        ╱
      │       ╱ Enemy trong range?
      │      ╱
      ▼─────▼─────────────────────────────
      │     ╲                    Enemy 1 (xa)
      │      ╲
      │       ╲
      │        ╲ Enemy 2 (gần) ← Nhắm cái này!
      │         │
      │         │ Raycast để xác minh
      │         │
      │ ────────┼─────→ autoShootPoint
      │         │
      │     [Bắn arrow]
```

### 3.2 CheckTargetHelper

**Chức năng:** Phát hiện có enemy nào phía trước player không

**Vị trí:** `CheckTargetHelper.cs` (component đính kèm)

**Trả về:** `true` nếu phát hiện enemy, `false` nếu không

**Cách dùng:**
```csharp
// Kiểm tra bên phải nếu facing right, bên trái nếu facing left
bool enemyDetected = checkTargetHelper.CheckTarget(isFacingRight() ? 1 : -1);
```

---

## 4. Tính Toán Trajectory

Đây là **phần phức tạp nhất** của hệ thống player. Player tính toán cung đạn đạo để trúng mục tiêu di chuyển.

### 4.1 Tại Sao Phải Tính Trajectory?

**Vấn đề:** Enemy đang di chuyển. Nếu bắn thẳng vào họ, arrow sẽ trượt.

**Giải pháp:** Mô phỏng vật lý để tìm góc hoàn hảo trúng target.

**Ví dụ:** Như một cầu thủ bóng rổ tính cung cần thiết để ném vào rổ.

### 4.2 Thuật Toán Trajectory

**Vị trí code:** `Player_Archer.cs:336-407`

```csharp
IEnumerator CheckTarget()
{
    // BƯỚC 1: Lấy vị trí target
    Vector3 mouseTempLook = autoShootPoint;  // Nơi muốn trúng
    mouseTempLook -= transform.position;     // Tương đối với player
    mouseTempLook.x *= (isFacingRight() ? -1 : 1);  // Lật nếu cần
    yield return null;

    // BƯỚC 2: Setup tính toán trajectory
    Vector2 fromPosition = firePostion.position;  // Điểm spawn arrow
    Vector2 target = autoShootPoint;              // Vị trí target

    // BƯỚC 3: Tính ước lượng góc ban đầu
    float beginAngle = Vector2ToAngle(target - fromPosition);
    Vector2 ballPos = fromPosition;

    // BƯỚC 4: Tìm góc tốt nhất bằng lặp
    float closestAngleDistance = int.MaxValue;  // Theo dõi kết quả tốt nhất
    bool checkingPerAngle = true;

    while (checkingPerAngle)
    {
        // Khởi tạo check cho mỗi góc
        int k = 0;
        Vector2 lastPos = fromPosition;
        bool isCheckingAngle = true;
        float closestDistance = int.MaxValue;

        // BƯỚC 5: Mô phỏng trajectory cho góc này
        while (isCheckingAngle)
        {
            // Tính vector force cho góc hiện tại
            Vector2 shotForce = force * AngleToVector2(beginAngle);

            // MÔ PHỎNG VẬT LÝ (projectile motion đơn giản hóa)
            // Vị trí X: vận tốc không đổi
            x1 = ballPos.x + shotForce.x * Time.fixedDeltaTime * (stepCheck * k);

            // Vị trí Y: bị ảnh hưởng bởi trọng lực
            // Công thức: y = y0 + v0*t - (1/2)*g*t^2
            y1 = ballPos.y + shotForce.y * Time.fixedDeltaTime * (stepCheck * k)
                 - (-(Physics2D.gravity.y * gravityScale) / 2f
                    * Time.fixedDeltaTime * Time.fixedDeltaTime
                    * (stepCheck * k) * (stepCheck * k));

            // BƯỚC 6: Kiểm tra khoảng cách đến target tại điểm này
            float distance = Vector2.Distance(target, new Vector2(x1, y1));

            if (distance < closestDistance)
                closestDistance = distance;

            // BƯỚC 7: Dừng nếu trajectory đang rơi và thấp hơn target
            if ((y1 < lastPos.y) && (y1 < target.y))
                isCheckingAngle = false;
            else
                k++;

            lastPos = new Vector2(x1, y1);
        }

        // BƯỚC 8: Góc này có tốt hơn các lần thử trước không?
        if (closestDistance >= closestAngleDistance)
        {
            // Không cải thiện, dừng lặp
            checkingPerAngle = false;
        }
        else
        {
            // Tìm được góc tốt hơn! Lưu lại và thử góc tiếp theo
            closestAngleDistance = closestDistance;

            // Điều chỉnh góc cho lần lặp tiếp
            if (isTargetRight)
                beginAngle += stepAngle;  // Tăng
            else
                beginAngle -= stepAngle;  // Giảm
        }
    }

    // BƯỚC 9: Tìm được góc tốt nhất! Chuẩn bị bắn
    var lookAt = AngleToVector2(beginAngle) * 10;
    lookAt.x *= (isFacingRight() ? -1 : 1);

    yield return null;

    // BƯỚC 10: Kích hoạt animation bắn
    anim.SetTrigger("shoot");

    // BƯỚC 11: Spawn arrow với trajectory đã tính
    ArrowProjectile _tempArrow = Instantiate(
        arrow,
        fromPosition,
        Quaternion.identity
    );

    // Khởi tạo arrow với force và gravity
    _tempArrow.Init(
        force * AngleToVector2(beginAngle),  // Force bắn
        gravityScale,                         // Gravity
        arrowDamage                           // Damage
    );

    // BƯỚC 12: Play sound effect
    SoundManager.PlaySfx(
        soundShoot[Random.Range(0, soundShoot.Length)],
        soundShootVolume
    );

    // BƯỚC 13: Bắt đầu cooldown reload
    StartCoroutine(ReloadingCo());
}
```

### 4.3 Visualization Trajectory

**Thuật toán làm gì:**

```
Thử nghiệm 1: Góc 45°
   ╭────╮
  ╱      ╲
 │        ╲      ← Cung quá cao, trượt
 │         ╲
 │          ✗ (trượt)
Player      Target

Thử nghiệm 2: Góc 44°
   ╭───╮
  ╱     ╲
 │       ╲     ← Cung vẫn quá cao
 │        ╲
 │         ✗ (trượt)
Player      Target

Thử nghiệm 3: Góc 43°
   ╭──╮
  ╱    ╲
 │      ╲    ← Cung hoàn hảo!
 │       ╲
 │        ✓ (trúng!)
Player      Target
```

**Phân Tích Từng Bước:**

1. **Bắt đầu với góc ước lượng:** Tính góc thô đến target
2. **Mô phỏng trajectory:** Với góc hiện tại, tính đường đi arrow từng điểm
3. **Kiểm tra độ chính xác:** Đo khoảng cách gần nhất đến target trong quá trình bay
4. **Cải thiện góc:** Tăng/giảm góc một chút
5. **Lặp lại:** Tiếp tục lặp cho đến khi góc trở nên tệ hơn (đã tìm được tốt nhất)
6. **Bắn:** Dùng góc tốt nhất để bắn arrow

**Helper Function:**

```csharp
// Chuyển góc (độ) thành vector hướng
public static Vector2 AngleToVector2(float degree)
{
    // Quaternion.Euler tạo rotation
    // Nhân với Vector2.right để lấy hướng
    Vector2 dir = (Vector2)(Quaternion.Euler(0, 0, degree) * Vector2.right);
    return dir;
}

// Chuyển vector hướng thành góc (độ)
public float Vector2ToAngle(Vector2 vec2)
{
    // Atan2 trả về radian, chuyển sang độ
    var angle = Mathf.Atan2(vec2.y, vec2.x) * Mathf.Rad2Deg;
    return angle;
}
```

**Tại Sao Cách Này Hiệu Quả:**

Mô phỏng vật lý tại mỗi góc dự đoán arrow sẽ hạ cánh ở đâu. Bằng cách thử nhiều góc và so sánh kết quả, ta tìm được góc hạ cánh gần target nhất.

**Lưu Ý Hiệu Suất:**

Tính toán này xảy ra **mỗi phát bắn**, nhưng đã được tối ưu:
- Dùng `stepAngle = 1` (chỉ check mỗi 1 độ)
- Dùng `stepCheck = 0.1` (mô phỏng mỗi 0.1 đơn vị thời gian)
- Dừng sớm khi góc trở nên tệ hơn

---

## 5. Cơ Chế Bắn

### 5.1 Method Shoot()

**Trigger:** Được gọi bởi AutoCheckAndShoot khi phát hiện target

**Vị trí code:** `Player_Archer.cs:322-333`

```csharp
public void Shoot()
{
    // VALIDATION: Không thể bắn nếu...
    if (!isAvailable ||                                    // Đang reload
        target == null ||                                  // Không có target
        GameManager.Instance.State != GameManager.GameState.Playing)  // Không đang chơi
        return;

    // KIỂM TRA HƯỚNG: Target ở bên phải hay trái?
    isTargetRight = autoShootPoint.x > transform.position.x;

    // TÙY CHỌN: Chỉ bắn target phía trước
    if (onlyShootTargetInFront &&
        ((isTargetRight && !isFacingRight()) ||           // Target bên phải, facing trái
         (isFacingRight() && !isTargetRight)))            // Facing phải, target trái
        return;

    // BẮT ĐẦU TÍNH TOÁN TRAJECTORY
    StartCoroutine(CheckTarget());
}
```

**Tại Sao Có Check `onlyShootTargetInFront`?**
- Ngăn player bắn ngược lại
- Thực tế hơn (cung thủ không thể xoay người)
- Khuyến khích positioning của player

### 5.2 Spawn Arrow

**Điều gì xảy ra khi arrow được tạo:**

```csharp
// Tạo instance arrow
ArrowProjectile _tempArrow = Instantiate(
    arrow,          // Prefab
    fromPosition,   // Spawn tại firePosition (cung)
    Quaternion.identity  // Không xoay (script arrow xử lý rotation)
);

// Khởi tạo arrow
_tempArrow.Init(
    force * AngleToVector2(beginAngle),  // Vector vận tốc bắn
    gravityScale,                         // Arrow rơi nhanh thế nào
    arrowDamage                           // Damage khi trúng
);
```

**Khởi Tạo Arrow (trong ArrowProjectile.cs):**
```csharp
public void Init(Vector2 velocity, float gravity, int damage)
{
    this.velocity = velocity;      // Set vận tốc ban đầu
    this.gravityScale = gravity;   // Set gravity
    this.damage = damage;          // Set damage
    // Script arrow tiếp quản từ đây
}
```

### 5.3 Hệ Thống Reload

**Vị trí code:** `Player_Archer.cs:410-428`

```csharp
IEnumerator ReloadingCo()
{
    // BƯỚC 1: Disable bắn
    isAvailable = false;
    lastShoot = Time.time;  // Ghi lại khi bắn
    isLoading = true;

    // BƯỚC 2: Delay ngắn trước animation reload
    yield return new WaitForSeconds(0.1f);

    // BƯỚC 3: Hiện animation reload
    anim.SetBool("isLoading", true);

    // BƯỚC 4: Đợi thời gian reload
    while (Time.time < (lastShoot + shootRate))
    {
        yield return null;  // Đợi mỗi frame
    }

    // BƯỚC 5: Ẩn animation reload
    anim.SetBool("isLoading", false);

    // BƯỚC 6: Delay ngắn
    yield return new WaitForSeconds(0.2f);

    // BƯỚC 7: Sẵn sàng bắn lại!
    isAvailable = true;
    isLoading = false;
}
```

**Timeline Reload:**
```
Bắn Arrow
    │
    ├─ isAvailable = false (không thể bắn)
    │
    ├─ Delay 0.1s
    │
    ├─ Hiện animation "isLoading"
    │
    ├─ Đợi shootRate giây (vd: 1 giây)
    │
    ├─ Ẩn animation "isLoading"
    │
    ├─ Delay 0.2s
    │
    └─ isAvailable = true (có thể bắn lại)
```

**Cấu Hình Inspector:**
- `shootRate = 1.0f` → 1 arrow mỗi giây
- `shootRate = 0.5f` → 2 arrow mỗi giây (nhanh hơn)
- `shootRate = 2.0f` → 1 arrow mỗi 2 giây (chậm hơn)

---

## 6. Hệ Thống Di Chuyển

Player dùng **vật lý 2D tùy chỉnh** (Controller2D), không phải Rigidbody2D.

### 6.1 Code Di Chuyển

**Vị trí code:** `Player_Archer.cs:89-127`

```csharp
public virtual void LateUpdate()
{
    // ĐIỀU KIỆN DỪNG 1: Game không đang chơi
    if (GameManager.Instance.State != GameManager.GameState.Playing)
    {
        velocity.x = 0;
        return;
    }

    // ĐIỀU KIỆN DỪNG 2: Các state ngăn di chuyển
    else if (!isPlaying ||           // Không active
             isSocking ||             // Đang bị shock
             enemyEffect == ENEMYEFFECT.SHOKING ||  // Hiệu ứng shock
             isLoading ||             // Đang reload
             checkTargetHelper.CheckTarget((isFacingRight() ? 1 : -1)))  // Enemy trong range
    {
        velocity = Vector2.zero;
        return;
    }

    // TÍNH VELOCITY TARGET
    float targetVelocityX = _direction.x * moveSpeed;

    // ĐIỀU KIỆN DỪNG 3: State đặc biệt
    if (isSocking || enemyEffect == ENEMYEFFECT.SHOKING)
        targetVelocityX = 0;

    if (enemyState != ENEMYSTATE.WALK || enemyEffect == ENEMYEFFECT.FREEZE)
        targetVelocityX = 0;

    if (isStopping || isStunning)
        targetVelocityX = 0;

    // SMOOTH VELOCITY (tăng/giảm tốc dần)
    velocity.x = Mathf.SmoothDamp(
        velocity.x,                  // Velocity hiện tại
        targetVelocityX,             // Velocity target
        ref velocityXSmoothing,      // Biến smoothing (truyền by ref)
        (controller.collisions.below) ? 0.1f : 0.2f  // Thời gian smoothing
    );

    // ÁP DỤNG GRAVITY
    velocity.y += -gravity * Time.deltaTime;

    // VA CHẠM TƯỜNG: Dừng nếu đụng tường
    if ((_direction.x > 0 && controller.collisions.right) ||
        (_direction.x < 0 && controller.collisions.left))
        velocity.x = 0;

    // DI CHUYỂN CHARACTER dùng Controller2D
    controller.Move(
        velocity * Time.deltaTime * multipleSpeed,  // Delta di chuyển
        false,                                       // Không nhảy
        isFacingRight()                             // Hướng facing
    );

    // VA CHẠM SÀN/TRẦN: Dừng di chuyển dọc
    if (controller.collisions.above || controller.collisions.below)
        velocity.y = 0;
}
```

**Điểm Quan Trọng:**

1. **LateUpdate vs Update:**
   - LateUpdate chạy sau Update
   - Đảm bảo di chuyển xảy ra sau tất cả update logic

2. **Smooth Movement (Di chuyển mượt):**
   - Dùng `Mathf.SmoothDamp` cho tăng tốc dần
   - Tự nhiên hơn thay đổi velocity tức thì

3. **Controller2D:**
   - Vật lý tùy chỉnh dùng raycast
   - Không cần Rigidbody2D
   - Kiểm soát chính xác hơn

4. **Điều Kiện Dừng Di Chuyển:**
   - Game không đang chơi
   - Player đang reload
   - Player bị stun/freeze/shock
   - Enemy trong range phát hiện (dừng để bắn)

### 6.2 Điều Khiển Hướng

**Method Flip:**
```csharp
void Flip()
{
    // Đảo vector hướng
    _direction = -_direction;

    // Xoay sprite (0° = phải, 180° = trái)
    transform.rotation = Quaternion.Euler(
        new Vector3(
            transform.rotation.x,
            isFacingRight() ? 0 : 180,  // Rotation Y
            transform.rotation.z
        )
    );
}
```

**Kiểm Tra Hướng Facing:**
```csharp
public bool isFacingRight()
{
    // Rotation Y 180° = facing right (sprite bị flip)
    return transform.rotation.eulerAngles.y == 180 ? true : false;
}
```

**Setup Hướng Ban Đầu:**
```csharp
void Start()
{
    // Set hướng dựa trên rotation ban đầu
    _direction = isFacingRight() ? Vector2.right : Vector2.left;

    // Nếu startBehavior xung đột với facing, flip
    if ((_direction == Vector2.right && startBehavior == STARTBEHAVIOR.WALK_LEFT) ||
        (_direction == Vector2.left && startBehavior == STARTBEHAVIOR.WALK_RIGHT))
    {
        Flip();
    }
}
```

---

## 7. Hệ Thống Damage & Health

Player **kế thừa** hệ thống health từ class cơ sở Enemy.

### 7.1 Nhận Damage

**Kế thừa từ Enemy.cs:**
```csharp
public void TakeDamage(
    float damage,
    Vector2 force,
    Vector2 hitPoint,
    GameObject instigator,
    BODYPART bodyPart = BODYPART.NONE,
    WeaponEffect weaponEffect = null)
{
    if (enemyState == ENEMYSTATE.DEATH)
        return;

    // Giảm health
    currentHealth -= (int)damage;

    // Hiện số damage
    FloatingTextManager.Instance.ShowText(
        "" + (int)damage,
        healthBarOffset,
        Color.red,
        transform.position
    );

    // Update thanh health
    if (healthBar)
        healthBar.UpdateValue(currentHealth / (float)health);

    // Kiểm tra chết
    if (currentHealth <= 0)
    {
        Die();
    }
    else
    {
        // Áp dụng weapon effect (poison, freeze, v.v.)
        if (weaponEffect != null)
        {
            // Xử lý poison, freeze, burn, shock
        }

        Hit(force);  // Play phản ứng hit
    }
}
```

### 7.2 Player Chết

**Override trong Player_Archer.cs:**
```csharp
public override void Die()
{
    // Đã chết rồi? Dừng
    if (isDead)
        return;

    base.Die();  // Gọi Enemy.Die() trước

    // Set flag chết
    isDead = true;

    CancelInvoke();  // Hủy action đã lên lịch

    // Disable collider (không thể bị hit nữa)
    var cols = GetComponents<BoxCollider2D>();
    foreach (var col in cols)
        col.enabled = false;

    // Play animation chết
    AnimSetBool("isDead", true);
    if (Random.Range(0, 2) == 1)
        AnimSetTrigger("die2");  // Animation chết thay thế

    // Hiệu ứng chết đặc biệt
    if (enemyEffect == ENEMYEFFECT.BURNING)
        return;  // Giữ đốt

    if (enemyEffect == ENEMYEFFECT.EXPLOSION || dieBehavior == DIEBEHAVIOR.DESTROY)
    {
        gameObject.SetActive(false);
        return;
    }

    // Dừng tất cả coroutine
    StopAllCoroutines();

    // Disable sau khi animation chết kết thúc
    StartCoroutine(DisableEnemy(
        AnimationHelper.getAnimationLength(anim, "Die") + 2f
    ));
}
```

**Điều Gì Xảy Ra Khi Player Chết:**
1. Dừng tất cả action (bắn, di chuyển)
2. Disable collider (không thể bị hit lại)
3. Play animation chết
4. Đợi animation kết thúc
5. Disable GameObject
6. GameManager.GameOver() được gọi (từ Enemy.Die())

### 7.3 Phản Ứng Hit

**Vị trí code:** `Player_Archer.cs:216-232`

```csharp
public override void Hit(Vector2 force, bool pushBack = false, bool knockDownRagdoll = false, bool shock = false)
{
    // Không thể phản ứng nếu không đang chơi hoặc bị stun
    if (!isPlaying || isStunning)
        return;

    base.Hit(force, pushBack, knockDownRagdoll, shock);  // Gọi Enemy.Hit()

    if (isDead)
        return;

    // Play animation hit
    AnimSetTrigger("hit");

    // Áp dụng knockback
    if (pushBack)
        StartCoroutine(PushBack(force));
    else if (shock)
        StartCoroutine(Shock());
}
```

**Hiệu Ứng PushBack:**
```csharp
public IEnumerator PushBack(Vector2 force)
{
    // Áp dụng force để đẩy player lùi
    SetForce(force.x, force.y);

    if (isDead)
    {
        Die();
        yield break;
    }
}
```

---

## 8. Điều Khiển Animation

Player dùng Animator của Unity với parameter.

### 8.1 Animation Parameter

**Animator Parameter (set trong Unity Animator):**
- `speed` (float) - Tốc độ di chuyển cho animation walk
- `isRunning` (bool) - Animation chạy
- `isStunning` (bool) - Animation stun
- `shoot` (trigger) - Animation bắn
- `isLoading` (bool) - Animation reload
- `hit` (trigger) - Animation phản ứng hit
- `isDead` (bool) - Animation chết
- `die2` (trigger) - Animation chết thay thế
- `stun` (trigger) - Trigger stun

### 8.2 Animation Update

**Vị trí code:** `Player_Archer.cs:169-175`

```csharp
void HandleAnimation()
{
    // Update animation di chuyển dựa trên velocity
    AnimSetFloat("speed", Mathf.Abs(velocity.x));

    // Running nếu di chuyển nhanh hơn walkSpeed
    AnimSetBool("isRunning", Mathf.Abs(velocity.x) > walkSpeed);

    // Hiện animation stun
    AnimSetBool("isStunning", isStunning);
}
```

**Animation Helper Kế Thừa (từ Enemy.cs):**
```csharp
public void AnimSetTrigger(string name)
{
    if (anim)
        anim.SetTrigger(name);
}

public void AnimSetBool(string name, bool value)
{
    if (anim)
        anim.SetBool(name, value);
}

public void AnimSetFloat(string name, float value)
{
    if (anim)
        anim.SetFloat(name, value);
}
```

**Khi Nào Gọi Mỗi Cái:**
- **Update():** HandleAnimation() - mỗi frame cho animation mượt
- **Shoot():** AnimSetTrigger("shoot") - action một lần
- **Die():** AnimSetBool("isDead", true) - state liên tục

---

## 9. Cấu Hình Inspector

### 9.1 Setting Thiết Yếu

**Setting Bắn Arrow:**
```
Shoot Rate: 1.0        // 1 arrow mỗi giây
Force: 20              // Sức mạnh bắn arrow
Step Check: 0.1        // Độ chính xác trajectory (thấp hơn = chính xác hơn)
Step Angle: 1          // Bước lặp góc
Gravity Scale: 3.5     // Tốc độ rơi arrow
Only Shoot Target In Front: ✓  // Ngăn bắn ngược
```

**Arrow Damage:**
```
Arrow: [ArrowProjectile Prefab]     // Kéo prefab arrow vào đây
Weapon Effect: [WeaponEffect]       // Hiệu ứng Poison/Burn/Freeze
Arrow Damage: 30                    // Damage cơ bản (bị override bởi upgrade)
Fire Position: [Transform]          // Điểm spawn cung
```

**Sound:**
```
Sound Shoot Volume: 0.5
Sound Shoot: [Mảng AudioClip]  // Sound bắn ngẫu nhiên
```

**Kế Thừa Từ Enemy (cũng có thể cấu hình):**
```
Gravity: 35                // Tốc độ rơi
Walk Speed: 3              // Tốc độ di chuyển
Health: 100                // Health tối đa
```

### 9.2 Component Bắt Buộc

**Phải có trên cùng GameObject:**
- ✅ Animator (với controller đã cấu hình)
- ✅ Controller2D (vật lý tùy chỉnh)
- ✅ CheckTargetHelper (phát hiện enemy)
- ✅ Box Collider 2D (ít nhất một, cho va chạm)
- ✅ Sprite Renderer (hình ảnh)

**Inspector Checklist:**
```
Component Player_Archer
├─ ✓ Prefab arrow được gán
├─ ✓ Weapon Effect được gán (nếu dùng effect)
├─ ✓ Fire Position được set (child transform tại cung)
├─ ✓ Mảng Sound Shoot đã điền
├─ ✓ Upgraded Character Parameter được gán
└─ ✓ Tất cả field Enemy kế thừa đã cấu hình

Component Controller2D
├─ ✓ Collision Mask set (layer sàn)
├─ ✓ Setting raycast đã cấu hình
└─ ✓ Horizontal/Vertical ray count đã set

Component CheckTargetHelper
├─ ✓ Range phát hiện đã set
├─ ✓ Target layer đã set (layer Enemy)
└─ ✓ Góc phát hiện đã cấu hình
```

---

## 10. Cách Sửa Đổi

### 10.1 Thay Đổi Fire Rate

**Làm player bắn nhanh/chậm hơn:**

```csharp
// Trong Inspector hoặc code
public float shootRate = 0.5f;  // 2 arrow mỗi giây (nhanh hơn)
public float shootRate = 2.0f;  // 1 arrow mỗi 2 giây (chậm hơn)
```

### 10.2 Thêm Manual Aiming

**Cho phép player ngắm bằng chuột:**

```csharp
// Thêm vào Update()
void Update()
{
    base.Update();
    HandleAnimation();

    // MỚI: Chế độ ngắm thủ công
    if (Input.GetMouseButton(0))  // Click trái để ngắm
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        autoShootPoint = mousePos;  // Set điểm ngắm tại chuột
        Shoot();  // Bắn tại vị trí chuột
    }

    // Phần code còn lại...
}
```

### 10.3 Thêm Weapon Upgrade

**Các loại arrow khác nhau:**

```csharp
[Header("Arrow Types")]
public ArrowProjectile normalArrow;
public ArrowProjectile fireArrow;
public ArrowProjectile iceArrow;
private ArrowProjectile currentArrow;

void Start()
{
    base.Start();
    // Set dựa trên level upgrade
    if (GlobalValue.weaponLevel >= 3)
        currentArrow = iceArrow;
    else if (GlobalValue.weaponLevel >= 2)
        currentArrow = fireArrow;
    else
        currentArrow = normalArrow;
}

// Trong coroutine CheckTarget(), thay đổi:
ArrowProjectile _tempArrow = Instantiate(currentArrow, fromPosition, Quaternion.identity);
```

### 10.4 Thêm Dash Ability

**Cơ chế né tránh nhanh:**

```csharp
[Header("Dash Settings")]
public float dashSpeed = 20f;
public float dashDuration = 0.2f;
public KeyCode dashKey = KeyCode.Space;
private bool isDashing = false;

void Update()
{
    base.Update();

    // MỚI: Input dash
    if (Input.GetKeyDown(dashKey) && !isDashing)
    {
        StartCoroutine(Dash());
    }
}

IEnumerator Dash()
{
    isDashing = true;
    float dashTime = 0;

    while (dashTime < dashDuration)
    {
        // Di chuyển với tốc độ dash
        velocity.x = (isFacingRight() ? 1 : -1) * dashSpeed;
        dashTime += Time.deltaTime;
        yield return null;
    }

    isDashing = false;
}
```

### 10.5 Thêm Multi-Shot

**Bắn nhiều arrow cùng lúc:**

```csharp
[Header("Multi-Shot")]
public int arrowCount = 3;        // Số arrow mỗi phát
public float spreadAngle = 15f;   // Góc giữa các arrow

// Sửa coroutine CheckTarget():
// Sau khi tính beginAngle, thay vì spawn một arrow:

for (int i = 0; i < arrowCount; i++)
{
    // Tính spread
    float angleOffset = spreadAngle * (i - (arrowCount - 1) / 2f);
    float finalAngle = beginAngle + angleOffset;

    // Spawn arrow
    ArrowProjectile _tempArrow = Instantiate(
        arrow,
        fromPosition,
        Quaternion.identity
    );

    _tempArrow.Init(
        force * AngleToVector2(finalAngle),
        gravityScale,
        arrowDamage
    );
}
```

---

## 11. Vấn Đề Thường Gặp & Giải Pháp

### 11.1 Player Không Bắn

**Vấn đề:** Player không bắn vào enemy

**Nguyên Nhân & Giải Pháp:**

**1. CheckTargetHelper không phát hiện enemy**
- **Kiểm tra:** `checkTargetHelper` đã được gán chưa?
- **Giải pháp:** Đảm bảo component CheckTargetHelper tồn tại và đã cấu hình
- **Xác minh:** Tìm tia debug đỏ trong Scene view khi enemy tiến lại

**2. Cấu hình layer sai**
- **Kiểm tra:** Enemy có ở layer đúng không?
- **Giải pháp:** Xác minh `GameManager.layerEnemy` khớp với layer GameObject enemy

**3. Prefab arrow chưa được gán**
- **Kiểm tra:** Inspector → field Arrow
- **Giải pháp:** Kéo prefab ArrowProjectile vào field

**4. Thiếu fire position**
- **Kiểm tra:** Inspector → Fire Position
- **Giải pháp:** Tạo child GameObject rỗng tại cung, gán vào Fire Position

**5. Player ở state sai**
- **Kiểm tra:** `isAvailable`, `isLoading`, `isDead`
- **Debug:** Thêm `Debug.Log("Can shoot: " + isAvailable);` trong Shoot()

### 11.2 Arrow Trượt Target

**Vấn đề:** Arrow bay qua hoặc dưới enemy

**Nguyên Nhân & Giải Pháp:**

**1. Độ chính xác trajectory quá thấp**
- **Giải pháp:** Giảm `stepCheck` xuống 0.05 hoặc thấp hơn (chính xác hơn)
- **Trade-off:** Giá trị thấp hơn = dùng CPU nhiều hơn

**2. Step angle quá lớn**
- **Giải pháp:** Giảm `stepAngle` xuống 0.5 (điều chỉnh góc mịn hơn)

**3. Gravity không khớp**
- **Giải pháp:** Đảm bảo `gravityScale` khớp với setting gravity của arrow

**4. Target di chuyển**
- **Lưu ý:** Hệ thống hiện tại ngắm vị trí hiện tại, không dự đoán di chuyển
- **Giải pháp nâng cao:** Triển khai predictive aiming (tính nơi enemy sẽ ở)

### 11.3 Player Không Di Chuyển

**Vấn đề:** Player bị kẹt tại chỗ

**Nguyên Nhân:**

**1. Controller2D chưa cấu hình**
- **Kiểm tra:** Component Controller2D tồn tại
- **Giải pháp:** Thêm Controller2D, cấu hình layer va chạm

**2. Luôn ở state reload**
- **Kiểm tra:** `isLoading` luôn true
- **Debug:** Thêm `Debug.Log("Loading: " + isLoading);`
- **Giải pháp:** Kiểm tra ReloadingCo() có hoàn thành đúng không

**3. Enemy luôn được phát hiện**
- **Vấn đề:** Range phát hiện CheckTargetHelper quá lớn
- **Giải pháp:** Giảm range phát hiện trong setting CheckTargetHelper

**4. Bị đóng băng bởi effect**
- **Kiểm tra:** `enemyEffect == ENEMYEFFECT.FREEZE`
- **Giải pháp:** Kiểm tra cái gì đang áp dụng freeze effect

### 11.4 Vấn Đề Hiệu Suất

**Vấn đề:** Game lag khi player bắn

**Giải pháp:**

**1. Tối ưu tính toán trajectory**
```csharp
// Giảm độ chính xác (nhanh hơn nhưng kém chính xác)
public float stepCheck = 0.15f;  // Thay vì 0.1
public float stepAngle = 2f;     // Thay vì 1
```

**2. Giới hạn tần suất tìm target**
```csharp
// Trong AutoCheckAndShoot, thêm delay
if (target)
{
    Shoot();
    yield return new WaitForSeconds(0.5f);  // Delay dài hơn
}
```

**3. Dùng object pooling cho arrow**
- Tạo pool arrow thay vì Instantiate/Destroy
- Tái sử dụng GameObject arrow

### 11.5 Health Không Update

**Vấn đề:** Player nhận damage nhưng thanh health không thay đổi

**Nguyên Nhân:**

**1. Health bar chưa được gán**
- **Kiểm tra:** Biến `healthBar` kế thừa
- **Giải pháp:** Health bar tự động tạo trong Enemy.Start()
- **Xác minh:** `healthBar != null` trong TakeDamage()

**2. UpgradedCharacterParameter chưa set**
- **Kiểm tra:** Inspector → field Upgraded Character ID
- **Giải pháp:** Gán ScriptableObject với stat character

**3. TakeDamage không được gọi**
- **Debug:** Thêm `Debug.Log("Took damage: " + damage);` trong TakeDamage()
- **Kiểm tra:** Đảm bảo arrow/enemy gọi TakeDamage() đúng

---

## 12. Hệ Thống Liên Quan

**Player_Archer phụ thuộc vào:**

| Hệ Thống | Mục Đích | Vị Trí |
|----------|----------|--------|
| Enemy (class cơ sở) | Health, damage, effect | AI/Enemy.cs |
| Controller2D | Vật lý di chuyển | Controllers/Controller2D.cs |
| CheckTargetHelper | Phát hiện enemy | Helpers/CheckTargetHelper.cs |
| ArrowProjectile | Hành vi arrow | Controllers/ArrowProjectile.cs |
| UpgradedCharacterParameter | Lưu stat | Player/UpgradedCharacterParameter.cs |
| GameManager | Game state | Managers/GameManager.cs |
| SoundManager | Audio | Managers/SoundManager.cs |
| FloatingTextManager | Số damage | UI/FloatingTextManager.cs |

**Xem Thêm:**
- `03_Enemy_System_Complete.md` - Chi tiết class Enemy cơ sở
- `05_Managers_Complete.md` - GameManager, SoundManager
- `10_How_To_Guides.md` - Tutorial sửa đổi thực tế

---

**Bây giờ bạn đã hiểu đầy đủ về Hệ Thống Player!**

**Tài liệu tiếp theo:** → `03_Enemy_System_Complete.md`
