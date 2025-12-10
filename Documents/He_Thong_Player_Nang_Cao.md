# Hệ Thống Player: Phân Tích Kỹ Thuật Nâng Cao

**Hướng Dẫn Kỹ Thuật Hoàn Chỉnh Về Player_Archer và Auto-Targeting**

---

## Mục Lục

1. [Tổng Quan](#tổng-quan)
2. [Tại Sao Player Kế Thừa Từ Enemy](#tại-sao-player-kế-thừa-từ-enemy)
3. [Kiến Trúc Player_Archer](#kiến-trúc-player_archer)
4. [Hệ Thống Auto-Targeting](#hệ-thống-auto-targeting)
5. [Tính Toán Ballistic Trajectory](#tính-toán-ballistic-trajectory)
6. [Hệ Thống Arrow Projectile](#hệ-thống-arrow-projectile)
7. [Movement và Physics](#movement-và-physics)
8. [Hệ Thống Animation](#hệ-thống-animation)
9. [Kỹ Thuật Nâng Cao](#kỹ-thuật-nâng-cao)
10. [Khắc Phục Sự Cố](#khắc-phục-sự-cố)
11. [Tài Liệu Tham Khảo Chéo](#tài-liệu-tham-khảo-chéo)

---

## Tổng Quan

**Mục Đích**: Đây là hướng dẫn kỹ thuật nâng cao cho developers muốn hiểu sâu về Player system và cơ chế auto-targeting tinh vi.

**Bạn Sẽ Học Được**:
- Tại sao Player_Archer kế thừa từ Enemy class
- Cách hệ thống auto-targeting phát hiện và ưu tiên enemies
- Cách tính toán ballistic trajectory tìm góc bắn tối ưu
- Cách physics simulation dự đoán đường đi của arrow
- Cơ chế player nâng cao (stun, reloading, hit reactions)
- Cách mở rộng player với custom abilities

**Kiến Thức Cần Có**:
- Đọc **[Enemy-Deep.md](Enemy-Deep.md)** trước
- Hiểu về physics (gravity, projectile motion)
- Quen thuộc với coroutines và Unity physics

**Độ Khó**: Nâng cao
**Thời Gian Đọc**: 35-45 phút

---

## Tại Sao Player Kế Thừa Từ Enemy

### Thiết Kế Không Theo Quy Ước

```csharp
public class Player_Archer : Enemy, ICanTakeDamage, IListener
```

**Điều này bất thường!** Hầu hết games có Player và Enemy hierarchies riêng biệt. Tại sao project này player lại kế thừa từ Enemy?

### Ưu Điểm

**1. Code Reuse**

Cả players và enemies đều chia sẻ nhiều tính năng:
- ✅ Hệ thống máu (`currentHealth`, `maxHealth`)
- ✅ Nhận sát thương (`TakeDamage`, `Hit`, `Die`)
- ✅ Status effects (burning, freezing, poison, shock)
- ✅ Animation state machine (`ENEMYSTATE` enum)
- ✅ Tích hợp Observer pattern (`IListener`)
- ✅ HealthBar UI
- ✅ Hit reactions và knockback

**Không có inheritance**: Bạn phải duplicate ~500 dòng code trong cả Player và Enemy classes.

**Với inheritance**: Player nhận tất cả functionality này miễn phí.

**2. Polymorphic Damage**

Projectiles có thể damage bất cứ thứ gì implement `ICanTakeDamage`:

```csharp
void OnTriggerEnter2D(Collider2D other)
{
    ICanTakeDamage target = other.GetComponent<ICanTakeDamage>();
    if (target != null)
    {
        target.TakeDamage(damage, force, hitPoint, gameObject);
    }
}
```

Điều này hoạt động cho:
- Enemy projectiles hitting player ✓
- Player projectiles hitting enemies ✓
- Explosions hitting anyone ✓

**3. Consistent Behavior**

Effects hoạt động giống nhau trên players và enemies:
- Poison làm chậm và gây damage theo thời gian
- Freeze dừng movement
- Burning gây damage mỗi frame
- Shock interrupt actions

### Nhược Điểm

**1. Confusing Terminology**

```csharp
public class Player_Archer : Enemy  // ← Player IS an Enemy?
```

Điều này trông sai nhưng hợp lý về mặt kỹ thuật.

**2. Inherited Baggage**

Player kế thừa các methods nó không cần:
- `DetectPlayer()` - ironic, player tự phát hiện chính nó
- `AutoCheckAndShoot()` - enemies không auto-shoot
- Một số AI-specific variables

**3. Tight Coupling**

Thay đổi Enemy base class có thể phá Player.

### Design Pattern: Composition Alternative

**Thiết kế tốt hơn** (nếu refactoring):

```csharp
// Interface-based design
public interface IDamageable
{
    void TakeDamage(...);
    void Die();
}

public interface IStatusEffectable
{
    void Burn(...);
    void Freeze(...);
    void Poison(...);
}

// Player uses composition
public class Player : MonoBehaviour, IDamageable, IStatusEffectable
{
    private HealthSystem health;      // Composition
    private StatusEffects effects;    // Composition
    private AnimationController anim; // Composition

    public void TakeDamage(...) => health.TakeDamage(...);
    public void Burn(...) => effects.ApplyBurn(...);
}
```

**Nhưng** cho một project nhỏ, inheritance đơn giản hơn và nhanh hơn để implement.

---

## Kiến Trúc Player_Archer

### Tổng Quan Cấu Trúc Class

```csharp
[RequireComponent(typeof(CheckTargetHelper))]
[RequireComponent(typeof(Controller2D))]
public class Player_Archer : Enemy, ICanTakeDamage, IListener
{
    // ===== SHOOTING SYSTEM =====
    public float shootRate = 1f;              // Shots per second
    public float force = 20f;                 // Arrow launch force
    public float gravityScale = 3.5f;         // Arrow gravity
    public ArrowProjectile arrow;             // Arrow prefab
    public int arrowDamage = 30;              // Arrow damage
    public Transform firePostion;             // Spawn point

    // ===== AUTO-TARGETING =====
    public CheckTargetHelper checkTargetHelper;
    private Transform target;                 // Current target enemy
    private Vector2 autoShootPoint;           // Calculated aim point

    // ===== MOVEMENT =====
    public Controller2D controller;
    public Vector3 velocity;
    private Vector2 _direction;

    // ===== STATE FLAGS =====
    public bool isAvailable = true;           // Can shoot?
    public bool isSocking { get; set; }       // Being shocked?
    public bool isDead { get; set; }          // Dead?
    private bool isLoading = false;           // Reloading?

    // ===== CORE SYSTEMS =====
    public override void Start() { ... }
    public override void Update() { ... }
    public virtual void LateUpdate() { ... }

    // Auto-targeting and shooting
    IEnumerator AutoCheckAndShoot() { ... }
    public void Shoot() { ... }
    IEnumerator CheckTarget() { ... }       // Trajectory calculation
    IEnumerator ReloadingCo() { ... }

    // Movement
    void Flip() { ... }

    // Reactions
    public override void Die() { ... }
    public override void Hit(...) { ... }
    public override void Stun(...) { ... }
}
```

### Lifecycle Flow

```
Start()
  ├─ Call base.Start() (Enemy initialization)
  ├─ Get Controller2D component
  ├─ Determine facing direction
  ├─ Start AutoCheckAndShoot() coroutine
  └─ Load arrow damage from UpgradedCharacterParameter

Update() (every frame)
  ├─ Call base.Update() (Enemy effects)
  ├─ HandleAnimation() (update animator)
  └─ Check for targets in front

LateUpdate() (after all Updates)
  ├─ Check game state (playing?)
  ├─ Check movement conditions
  ├─ Calculate velocity with smoothing
  ├─ Apply gravity
  ├─ Move with Controller2D
  └─ Handle collisions

AutoCheckAndShoot() (coroutine, continuous)
  ├─ Wait for target detection
  ├─ Find closest enemy
  ├─ Calculate aim point
  ├─ Call Shoot()
  └─ Loop
```

---

## Hệ Thống Auto-Targeting

### Cách Hoạt Động

Player tự động phát hiện và bắn vào enemy gần nhất.

### Bước 1: Target Detection (CheckTargetHelper)

```csharp
IEnumerator AutoCheckAndShoot()
{
    while (true)
    {
        target = null;
        yield return null;

        // Wait until enemy detected
        while (!checkTargetHelper.CheckTarget((isFacingRight() ? 1 : -1)))
        {
            yield return null;  // Wait one frame and check again
        }

        // Found enemy, proceed to step 2...
    }
}
```

**CheckTargetHelper** dùng raycast để phát hiện enemies:

```csharp
public class CheckTargetHelper : MonoBehaviour
{
    public float checkDistance = 10f;
    public LayerMask targetLayer;

    public bool CheckTarget(int direction)
    {
        Vector2 dir = direction > 0 ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            dir,
            checkDistance,
            targetLayer
        );

        return hit.collider != null;
    }
}
```

**Visual**:
```
Player → → → → → → [Raycast] → → → → → Enemy
         (checkDistance = 10)

If hit: CheckTarget() returns true
If miss: CheckTarget() returns false
```

### Bước 2: Tìm Enemy Gần Nhất

```csharp
// Cast circle to find ALL enemies in 100-unit radius
RaycastHit2D[] hits = Physics2D.CircleCastAll(
    transform.position,
    100f,                          // Radius
    Vector2.zero,                  // Direction (not used for circle)
    0,                             // Distance (not used for circle)
    GameManager.Instance.layerEnemy
);

if (hits.Length > 0)
{
    float closestDistance = 99999f;

    foreach (var obj in hits)
    {
        // Check if it can take damage
        var checkEnemy = obj.collider.GetComponent<ICanTakeDamage>();
        if (checkEnemy != null)
        {
            // Calculate horizontal distance only
            float distance = Mathf.Abs(obj.transform.position.x - transform.position.x);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                target = obj.transform;

                // Raycast to get exact hit point
                var hit = Physics2D.Raycast(
                    transform.position,
                    (obj.point - (Vector2)transform.position),
                    100f,
                    GameManager.Instance.layerEnemy
                );

                autoShootPoint = hit.point;
                autoShootPoint.y = Mathf.Max(autoShootPoint.y, firePostion.position.y - 0.1f);
            }
        }
    }

    if (target)
    {
        Shoot();  // Found target, shoot!
        yield return new WaitForSeconds(0.2f);
    }
}
```

**Tại sao CircleCast thay vì OverlapCircle?**

CircleCast trả về `RaycastHit2D[]` bao gồm:
- `.collider` - Collider bị hit
- `.transform` - GameObject
- `.point` - Điểm tiếp xúc chính xác
- `.distance` - Khoảng cách

**Tại sao chỉ horizontal distance?**

```csharp
float distance = Mathf.Abs(obj.transform.position.x - transform.position.x);
```

Vì enemies ở độ cao khác nhau nhưng gần hơn theo chiều ngang là targets tốt hơn:

```
        Enemy1 (x=5, y=3)
           △
           |  dist = 2 units horizontally
           |
Player ────┤
(x=3,y=0)  |
           |  dist = 4 units horizontally
           ▽
        Enemy2 (x=7, y=-1)
```

Enemy1 được chọn mặc dù Enemy2 có thể gần hơn về tổng khoảng cách.

### Bước 3: Tính Toán Aim Point

```csharp
// Raycast to get exact hit point
var hit = Physics2D.Raycast(
    transform.position,
    (obj.point - (Vector2)transform.position),
    100f,
    GameManager.Instance.layerEnemy
);

autoShootPoint = hit.point;

// Ensure aim point isn't below fire position
autoShootPoint.y = Mathf.Max(autoShootPoint.y, firePostion.position.y - 0.1f);
```

**Tại sao clamp Y position?**

Nếu enemy ở dưới player, chúng ta không muốn bắn xuống:

```
Player (firePosition)
   |
   ↓ (clamped to minimum Y)
───────────────────
        ↓ (original aim point - too low)
      Enemy
```

Điều này ngăn arrows đi thẳng xuống.

---

## Tính Toán Ballistic Trajectory

### Vấn Đề

Arrows theo **parabolic arc** do gravity. Chỉ bắn thẳng vào enemy sẽ không trúng:

```
Player →  →  ↘
              ↓  ← Arrow falls
              ↓
             ↓   Miss!
           ↓
Enemy ●
```

**Giải Pháp**: Tính toán góc phóng làm cho arrow trúng target.

### Thuật Toán

```csharp
IEnumerator CheckTarget()
{
    Vector2 fromPosition = firePostion.position;
    Vector2 target = autoShootPoint;

    // Step 1: Get initial angle (straight toward target)
    float beginAngle = Vector2ToAngle(target - fromPosition);

    float cloestAngleDistance = int.MaxValue;
    bool checkingPerAngle = true;

    while (checkingPerAngle)
    {
        // Step 2: Simulate arrow trajectory at this angle
        int k = 0;
        Vector2 lastPos = fromPosition;
        bool isCheckingAngle = true;
        float clostestDistance = int.MaxValue;

        while (isCheckingAngle)
        {
            Vector2 shotForce = force * AngleToVector2(beginAngle);

            // Physics formula: projectile motion
            float t = Time.fixedDeltaTime * (stepCheck * k);

            x1 = ballPos.x + shotForce.x * t;
            y1 = ballPos.y + shotForce.y * t - (Physics2D.gravity.y * gravityScale / 2f * t * t);

            // Check how close this point is to target
            float _distance = Vector2.Distance(target, new Vector2(x1, y1));
            if (_distance < clostestDistance)
                clostestDistance = _distance;

            // Stop if arrow is falling and below target
            if ((y1 < lastPos.y) && (y1 < target.y))
                isCheckingAngle = false;
            else
                k++;

            lastPos = new Vector2(x1, y1);
        }

        // Step 3: Is this angle better than previous?
        if (clostestDistance >= cloestAngleDistance)
        {
            checkingPerAngle = false;  // Found best angle!
        }
        else
        {
            // Try next angle
            cloestAngleDistance = clostestDistance;

            if (isTargetRight)
                beginAngle += stepAngle;  // Increase angle
            else
                beginAngle -= stepAngle;  // Decrease angle
        }
    }

    // Step 4: Shoot at best angle found
    ArrowProjectile _tempArrow = Instantiate(arrow, fromPosition, Quaternion.identity);
    _tempArrow.Init(force * AngleToVector2(beginAngle), gravityScale, arrowDamage);
}
```

### Giải Thích Physics

**Công Thức Projectile Motion**:

Cho object phóng với initial velocity `v` tại góc `θ`:

**Horizontal Position**:
```
x(t) = x₀ + vₓ * t
where vₓ = v * cos(θ)
```

**Vertical Position**:
```
y(t) = y₀ + vᵧ * t - (g/2) * t²
where vᵧ = v * sin(θ), g = gravity
```

**Trong code**:
```csharp
float t = Time.fixedDeltaTime * (stepCheck * k);
Vector2 shotForce = force * AngleToVector2(beginAngle);

x1 = ballPos.x + shotForce.x * t;  // x₀ + vₓ * t
y1 = ballPos.y + shotForce.y * t - (Physics2D.gravity.y * gravityScale / 2f * t * t);
//   y₀        + vᵧ * t            - (g/2) * t²
```

### Quá Trình Iteration Angle

Thuật toán thử các góc khác nhau cho đến khi tìm được góc tốt nhất:

```
Initial angle: 30° (straight at enemy)

Test 30°:
  Simulate arrow path...
  Closest approach to target: 2.5 units
  Try next angle

Test 31°:
  Simulate arrow path...
  Closest approach: 1.8 units (better!)
  Try next angle

Test 32°:
  Simulate arrow path...
  Closest approach: 1.2 units (even better!)
  Try next angle

Test 33°:
  Simulate arrow path...
  Closest approach: 0.5 units (best so far!)
  Try next angle

Test 34°:
  Simulate arrow path...
  Closest approach: 0.8 units (worse)
  STOP! Use 33°
```

**Parameters**:
- `stepAngle = 1` - Thử góc với khoảng cách 1 độ
- `stepCheck = 0.1` - Mô phỏng 10 điểm mỗi giây

### Visualization

```
Player shoots at 35° angle:

     ╱
    ╱  ○ (simulated point 1)
   ╱
  ╱     ○ (point 2)
 ╱
↑        ○ (point 3)
│
Player    ○ (point 4) ← This is closest to enemy!

           ○ (point 5)

            Enemy ●

Distance from point 4 to enemy = 0.5 units → Good hit!
```

### Tại Sao Điều Này Hoạt Động

**Ballistic calculation** tính đến:
- Gravity kéo arrow xuống
- Initial velocity và angle
- Time of flight
- Horizontal và vertical components

**Kết Quả**: Arrow trúng target ngay cả khi enemy ở trên/dưới player.

---

## Hệ Thống Arrow Projectile

### ArrowProjectile Initialization

```csharp
public void Shoot()
{
    // ... trajectory calculation ...

    // Spawn arrow
    ArrowProjectile _tempArrow = Instantiate(arrow, fromPosition, Quaternion.identity);

    // Initialize with calculated values
    _tempArrow.Init(
        force * AngleToVector2(beginAngle),  // Initial velocity vector
        gravityScale,                         // How fast it falls
        arrowDamage                          // Damage amount
    );

    SoundManager.PlaySfx(soundShoot[Random.Range(0, soundShoot.Length)], soundShootVolume);

    StartCoroutine(ReloadingCo());
}
```

### ArrowProjectile.Init() Implementation

**Triển khai điển hình**:

```csharp
public class ArrowProjectile : MonoBehaviour
{
    private Rigidbody2D rb;
    private int damage;
    public WeaponEffect weaponEffect;

    public void Init(Vector2 velocity, float gravity, int dmg)
    {
        rb = GetComponent<Rigidbody2D>();
        damage = dmg;

        // Set velocity
        rb.velocity = velocity;

        // Set gravity scale
        rb.gravityScale = gravity;

        // Rotate arrow to face direction of travel
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Update()
    {
        // Keep arrow rotated toward velocity
        if (rb.velocity.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        ICanTakeDamage target = other.GetComponent<ICanTakeDamage>();
        if (target != null)
        {
            Vector2 hitPoint = other.ClosestPoint(transform.position);
            Vector2 knockback = rb.velocity.normalized * 5f;

            target.TakeDamage(damage, knockback, hitPoint, gameObject, BODYPART.NONE, weaponEffect);

            Destroy(gameObject);
        }
    }
}
```

### Angle Conversion Helpers

```csharp
public static Vector2 AngleToVector2(float degree)
{
    // Convert degrees to direction vector
    Vector2 dir = (Vector2)(Quaternion.Euler(0, 0, degree) * Vector2.right);
    return dir;
}

// Example:
AngleToVector2(0)    = (1, 0)   // Right
AngleToVector2(90)   = (0, 1)   // Up
AngleToVector2(45)   = (0.7, 0.7) // Up-Right diagonal
AngleToVector2(-45)  = (0.7, -0.7) // Down-Right

public float Vector2ToAngle(Vector2 vec2)
{
    // Convert direction vector to degrees
    var angle = Mathf.Atan2(vec2.y, vec2.x) * Mathf.Rad2Deg;
    return angle;
}

// Example:
Vector2ToAngle((1, 0))   = 0°
Vector2ToAngle((0, 1))   = 90°
Vector2ToAngle((-1, 0))  = 180°
Vector2ToAngle((0, -1))  = -90°
```

---

## Movement và Physics

### Hệ Thống Movement

Player dùng `Controller2D` (custom raycast-based controller, không phải Rigidbody2D):

```csharp
public virtual void LateUpdate()
{
    // 1. Check if should stop
    if (GameManager.Instance.State != GameManager.GameState.Playing)
    {
        velocity.x = 0;
        return;
    }

    if (!isPlaying || isSocking || enemyEffect == ENEMYEFFECT.SHOKING || isLoading)
    {
        velocity = Vector2.zero;
        return;
    }

    // 2. Check if target detected (stop if enemy ahead)
    if (checkTargetHelper.CheckTarget((isFacingRight() ? 1 : -1)))
    {
        velocity = Vector2.zero;
        return;
    }

    // 3. Calculate target velocity
    float targetVelocityX = _direction.x * moveSpeed;

    // Stop if frozen or stunned
    if (enemyState != ENEMYSTATE.WALK || enemyEffect == ENEMYEFFECT.FREEZE)
        targetVelocityX = 0;

    if (isStopping || isStunning)
        targetVelocityX = 0;

    // 4. Smooth velocity change
    velocity.x = Mathf.SmoothDamp(
        velocity.x,
        targetVelocityX,
        ref velocityXSmoothing,
        (controller.collisions.below) ? 0.1f : 0.2f  // Faster on ground
    );

    // 5. Apply gravity
    velocity.y += -gravity * Time.deltaTime;

    // 6. Stop at walls
    if ((_direction.x > 0 && controller.collisions.right) ||
        (_direction.x < 0 && controller.collisions.left))
    {
        velocity.x = 0;
    }

    // 7. Move with Controller2D
    controller.Move(velocity * Time.deltaTime * multipleSpeed, false, isFacingRight());

    // 8. Reset vertical velocity on ground/ceiling
    if (controller.collisions.above || controller.collisions.below)
        velocity.y = 0;
}
```

### Tại Sao LateUpdate()?

**LateUpdate** chạy sau tất cả Updates. Điều này đảm bảo:
- Enemy positions được finalized
- Target detection chính xác
- Camera theo đúng

### Flip Mechanism

```csharp
void Flip()
{
    // Reverse direction
    _direction = -_direction;

    // Rotate sprite
    transform.rotation = Quaternion.Euler(
        new Vector3(
            transform.rotation.x,
            isFacingRight() ? 0 : 180,  // Y-axis rotation
            transform.rotation.z
        )
    );
}

public bool isFacingRight()
{
    return transform.rotation.eulerAngles.y == 180;
}
```

**Visual**:
```
Facing Right:     Facing Left:
  O               O
 /|\             /|\
 / \             / \
→→→             ←←←
(rotation.y=180) (rotation.y=0)
```

---

## Hệ Thống Animation

### Animation Parameters

```csharp
void HandleAnimation()
{
    // Speed-based animation
    AnimSetFloat("speed", Mathf.Abs(velocity.x));

    // Running animation (faster than walk speed)
    AnimSetBool("isRunning", Mathf.Abs(velocity.x) > walkSpeed);

    // Stunned state
    AnimSetBool("isStunning", isStunning);
}
```

### Animation State Machine

**Animator Controller điển hình**:

```
States:
  ├─ Idle
  ├─ Walk (speed > 0, !isRunning)
  ├─ Run (isRunning)
  ├─ Shoot (trigger "shoot")
  ├─ Hit (trigger "hit")
  ├─ Stun (isStunning)
  ├─ Die (isDead)
  └─ Loading (isLoading)

Transitions:
  Idle → Walk: speed > 0.1
  Walk → Run: isRunning = true
  Any → Shoot: On "shoot" trigger
  Any → Hit: On "hit" trigger
  Any → Die: isDead = true
```

### Shoot Animation Workflow

```csharp
// 1. Calculate trajectory
IEnumerator CheckTarget()
{
    // ... trajectory calculation ...

    yield return null;

    // 2. Trigger animation
    anim.SetTrigger("shoot");

    // 3. Spawn arrow (timed with animation)
    ArrowProjectile _tempArrow = Instantiate(arrow, fromPosition, Quaternion.identity);
    _tempArrow.Init(force * AngleToVector2(beginAngle), gravityScale, arrowDamage);

    // 4. Play sound
    SoundManager.PlaySfx(soundShoot[Random.Range(0, soundShoot.Length)], soundShootVolume);

    // 5. Start reload
    StartCoroutine(ReloadingCo());
}
```

**Animation Event**: Bạn có thể thêm Animation Event để spawn arrow tại frame chính xác khi bow release.

### Reload System

```csharp
IEnumerator ReloadingCo()
{
    // 1. Disable shooting
    isAvailable = false;
    lastShoot = Time.time;
    isLoading = true;

    // 2. Short delay before showing reload animation
    yield return new WaitForSeconds(0.1f);
    anim.SetBool("isLoading", true);

    // 3. Wait for shoot rate cooldown
    while (Time.time < (lastShoot + shootRate))
    {
        yield return null;
    }

    // 4. Hide reload animation
    anim.SetBool("isLoading", false);

    // 5. Short delay before allowing next shot
    yield return new WaitForSeconds(0.2f);

    // 6. Re-enable shooting
    isAvailable = true;
    isLoading = false;
}
```

**Timeline**:
```
T=0.0s: Shoot arrow
  ├─ isAvailable = false
  ├─ isLoading = true

T=0.1s: Show reload animation
  └─ anim.SetBool("isLoading", true)

T=0.0s - T=1.0s: Wait for shootRate (1 second)
  └─ while (Time.time < lastShoot + 1.0)...

T=1.0s: Hide reload animation
  └─ anim.SetBool("isLoading", false)

T=1.2s: Re-enable shooting
  ├─ isAvailable = true
  └─ isLoading = false
```

---

## Kỹ Thuật Nâng Cao

### Kỹ Thuật 1: Double-Shot Power-Up

Thêm ability để bắn hai arrows cùng lúc:

```csharp
public class Player_Archer : Enemy
{
    [Header("Power-Ups")]
    public bool hasDoubleShot = false;
    public float doubleShotAngleOffset = 15f;  // Degrees apart

    IEnumerator CheckTarget()
    {
        // ... trajectory calculation for main shot ...

        yield return null;
        anim.SetTrigger("shoot");

        if (hasDoubleShot)
        {
            // Shoot two arrows
            ShootArrowAt(beginAngle - doubleShotAngleOffset);
            ShootArrowAt(beginAngle + doubleShotAngleOffset);
        }
        else
        {
            // Shoot one arrow
            ShootArrowAt(beginAngle);
        }

        SoundManager.PlaySfx(soundShoot[Random.Range(0, soundShoot.Length)], soundShootVolume);
        StartCoroutine(ReloadingCo());
    }

    void ShootArrowAt(float angle)
    {
        ArrowProjectile _tempArrow = Instantiate(arrow, firePostion.position, Quaternion.identity);
        _tempArrow.Init(force * AngleToVector2(angle), gravityScale, arrowDamage);
    }
}
```

**Visual**:
```
Single Shot:          Double Shot:
    ↗ (arrow)            ↗↗ (arrow 1)
   /                     ↗ (arrow 2)
Player                 Player

One trajectory        Two trajectories at ±15°
```

### Kỹ Thuật 2: Manual Aiming

Thêm mouse/touch control cho manual aiming:

```csharp
public class Player_Archer : Enemy
{
    public bool useManualAim = false;

    public void Shoot()
    {
        if (!isAvailable || GameManager.Instance.State != GameManager.GameState.Playing)
            return;

        if (useManualAim)
        {
            StartCoroutine(ShootAtMouse());
        }
        else
        {
            // Auto-aim (existing code)
            if (target == null) return;
            StartCoroutine(CheckTarget());
        }
    }

    IEnumerator ShootAtMouse()
    {
        // Get mouse position in world space
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        // Calculate angle to mouse
        Vector2 direction = (mousePos - firePostion.position).normalized;
        float angle = Vector2ToAngle(direction);

        yield return null;
        anim.SetTrigger("shoot");

        // Shoot at mouse
        ArrowProjectile _tempArrow = Instantiate(arrow, firePostion.position, Quaternion.identity);
        _tempArrow.Init(force * AngleToVector2(angle), gravityScale, arrowDamage);

        SoundManager.PlaySfx(soundShoot[Random.Range(0, soundShoot.Length)], soundShootVolume);
        StartCoroutine(ReloadingCo());
    }
}
```

**UI Button**:
```csharp
// Connect to UI button
public class ShootButton : MonoBehaviour
{
    public Player_Archer player;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))  // Left click
        {
            player.useManualAim = true;
            player.Shoot();
        }
    }
}
```

### Kỹ Thuật 3: Piercing Arrows

Làm cho arrows đi xuyên qua nhiều enemies:

```csharp
public class PiercingArrow : ArrowProjectile
{
    public int maxPierceCount = 3;
    private int pierceCount = 0;

    void OnTriggerEnter2D(Collider2D other)
    {
        ICanTakeDamage target = other.GetComponent<ICanTakeDamage>();
        if (target != null)
        {
            Vector2 hitPoint = other.ClosestPoint(transform.position);
            Vector2 knockback = GetComponent<Rigidbody2D>().velocity.normalized * 5f;

            target.TakeDamage(damage, knockback, hitPoint, gameObject, BODYPART.NONE, weaponEffect);

            pierceCount++;

            if (pierceCount >= maxPierceCount)
            {
                Destroy(gameObject);  // Destroyed after max pierces
            }
            // Otherwise, arrow continues flying
        }
    }
}
```

**Visual**:
```
Arrow → → Enemy1 → → Enemy2 → → Enemy3 → [Destroyed]
        (hit 1)   (hit 2)   (hit 3 - max)
```

### Kỹ Thuật 4: Charged Shot

Hold button để charge shot mạnh hơn:

```csharp
public class Player_Archer : Enemy
{
    [Header("Charged Shot")]
    public float chargeTime = 2f;
    public float maxChargeMultiplier = 3f;
    private float currentCharge = 0f;
    private bool isCharging = false;

    public void StartCharge()
    {
        if (!isAvailable) return;
        isCharging = true;
        currentCharge = 0f;
        StartCoroutine(ChargeCo());
    }

    IEnumerator ChargeCo()
    {
        while (isCharging && currentCharge < chargeTime)
        {
            currentCharge += Time.deltaTime;
            yield return null;
        }
    }

    public void ReleaseCharge()
    {
        if (!isCharging) return;
        isCharging = false;

        // Calculate charge percentage
        float chargePercent = Mathf.Clamp01(currentCharge / chargeTime);
        float damageMultiplier = 1f + (chargePercent * (maxChargeMultiplier - 1f));

        // Shoot with increased damage and force
        int chargedDamage = Mathf.RoundToInt(arrowDamage * damageMultiplier);
        float chargedForce = force * damageMultiplier;

        // ... shoot with chargedDamage and chargedForce ...
    }
}
```

**Usage**:
```
T=0.0s: Player presses button
  └─ StartCharge()

T=0.0s - T=2.0s: Holding button
  └─ currentCharge increases (0.0 → 2.0)

T=1.5s: Player releases button
  └─ ReleaseCharge()
  └─ chargePercent = 1.5/2.0 = 75%
  └─ damageMultiplier = 1 + 0.75 * (3-1) = 2.5x damage
```

---

## Khắc Phục Sự Cố

### Vấn Đề 1: Arrows Không Trúng Target

**Triệu Chứng**: Arrows luôn miss enemies

**Các Bước Debug**:

**Bước 1: Kiểm tra gravity settings**

```csharp
void Start()
{
    Debug.Log($"Arrow gravity scale: {gravityScale}");
    Debug.Log($"Physics2D.gravity: {Physics2D.gravity}");
}

// Expected:
// Arrow gravity scale: 3.5
// Physics2D.gravity: (0, -9.81)
```

**Bước 2: Visualize trajectory**

```csharp
IEnumerator CheckTarget()
{
    // ... inside trajectory simulation loop ...

    // Draw predicted path
    Debug.DrawLine(lastPos, new Vector2(x1, y1), Color.green, 5f);
}
```

**Bước 3: Kiểm tra force value**

```csharp
public float force = 20f;  // Try increasing to 25-30
```

Force quá thấp = arrow rơi ngắn
Force quá cao = arrow bắn quá

**Bước 4: Điều chỉnh stepAngle và stepCheck**

```csharp
public float stepAngle = 1f;   // Try 0.5f for more precision
public float stepCheck = 0.1f; // Try 0.05f for more simulation points
```

---

### Vấn Đề 2: Player Không Auto-Shoot

**Triệu Chứng**: Player phát hiện enemies nhưng không bắn

**Nguyên Nhân A: isAvailable stuck false**

```csharp
void Start()
{
    Debug.Log($"isAvailable: {isAvailable}");  // Should be true
}

void Update()
{
    if (!isAvailable)
    {
        Debug.LogWarning("Player is not available to shoot!");
    }
}
```

**Nguyên Nhân B: Reload coroutine không hoàn thành**

```csharp
IEnumerator ReloadingCo()
{
    Debug.Log("Started reload");
    isAvailable = false;
    // ...
    yield return new WaitForSeconds(shootRate);
    Debug.Log("Finished reload");
    isAvailable = true;  // ← Check if this runs
}
```

**Nguyên Nhân C: Target null**

```csharp
public void Shoot()
{
    if (target == null)
    {
        Debug.LogWarning("Target is null, can't shoot!");
        return;
    }

    Debug.Log($"Shooting at: {target.name}");
    StartCoroutine(CheckTarget());
}
```

---

### Vấn Đề 3: Player Không Di Chuyển

**Triệu Chứng**: Player stuck tại chỗ

**Debugging**:

```csharp
void LateUpdate()
{
    Debug.Log($"State: {GameManager.Instance.State}");
    Debug.Log($"isPlaying: {isPlaying}");
    Debug.Log($"isSocking: {isSocking}");
    Debug.Log($"isLoading: {isLoading}");
    Debug.Log($"enemyEffect: {enemyEffect}");
    Debug.Log($"Target detected: {checkTargetHelper.CheckTarget((isFacingRight() ? 1 : -1))}");
    Debug.Log($"Velocity: {velocity}");
}
```

**Nguyên nhân phổ biến**:
- Game không trong Playing state
- isPlaying = false
- Enemy detected (player dừng để bắn)
- Frozen effect active
- Stunned

---

### Vấn Đề 4: Trajectory Calculation Mất Quá Nhiều Thời Gian

**Triệu Chứng**: Frame rate giảm khi bắn

**Nguyên Nhân**: Quá nhiều simulation steps

```csharp
// Current settings:
public float stepAngle = 1f;   // Tests 1 degree increments
public float stepCheck = 0.1f; // 10 points per second

// Worst case:
// 90 angles * 100 simulation points = 9,000 calculations per shot!
```

**Giải Pháp A: Giảm precision**

```csharp
public float stepAngle = 2f;   // Test every 2 degrees
public float stepCheck = 0.2f; // 5 points per second

// Now: 45 angles * 50 points = 2,250 calculations (4x faster!)
```

**Giải Pháp B: Giới hạn angle range**

```csharp
IEnumerator CheckTarget()
{
    float beginAngle = Vector2ToAngle(target - fromPosition);
    float minAngle = beginAngle - 30f;  // ← Add limits
    float maxAngle = beginAngle + 30f;

    while (checkingPerAngle)
    {
        // ... simulation ...

        if (beginAngle < minAngle || beginAngle > maxAngle)
        {
            checkingPerAngle = false;  // Stop early
        }
    }
}
```

---

### Vấn Đề 5: Double-Shot Không Hoạt Động

**Triệu Chứng**: Chỉ một arrow spawn

**Kiểm tra instantiation**:

```csharp
void ShootArrowAt(float angle)
{
    Debug.Log($"Spawning arrow at angle: {angle}");

    ArrowProjectile _tempArrow = Instantiate(arrow, firePostion.position, Quaternion.identity);

    if (_tempArrow == null)
    {
        Debug.LogError("Failed to spawn arrow!");
        return;
    }

    _tempArrow.Init(force * AngleToVector2(angle), gravityScale, arrowDamage);
}

// Call twice:
ShootArrowAt(beginAngle - 15f);  // Arrow 1
ShootArrowAt(beginAngle + 15f);  // Arrow 2
```

**Kiểm tra prefab**:

Đảm bảo `arrow` prefab được assigned trong Inspector.

---

## Tài Liệu Tham Khảo Chéo

**Tài Liệu Liên Quan**:

- **[Enemy-Deep.md](Enemy-Deep.md)** - Base class (Enemy) mà Player kế thừa từ
- **[02_Player_System_Complete.md](02_Player_System_Complete.md)** - Hướng dẫn player cho người mới
- **[Character-Properties.md](Character-Properties.md)** - UpgradedCharacterParameter và stats
- **[Events-and-Triggers.md](Events-and-Triggers.md)** - IListener implementation
- **[01_Project_Architecture.md](01_Project_Architecture.md)** - Overall system design
- **[13_Code_Examples.md](13_Code_Examples.md)** - Coroutine patterns

**Scripts Chính**:
- `Player_Archer.cs` - Main player class
- `Enemy.cs` - Base class với health/damage systems
- `ArrowProjectile.cs` - Arrow implementation
- `Controller2D.cs` - Custom physics controller
- `CheckTargetHelper.cs` - Target detection
- `UpgradedCharacterParameter.cs` - Character stats

**Unity Concepts**:
- Inheritance (Player extends Enemy)
- Coroutines (AutoCheckAndShoot, trajectory calculation)
- Physics (projectile motion, gravity)
- Vector math (angle conversions, trajectory)
- Animation (state machine, triggers, booleans)

**Physics Resources**:
- [Projectile Motion](https://en.wikipedia.org/wiki/Projectile_motion)
- [Unity Physics2D](https://docs.unity3d.com/Manual/Physics2DReference.html)
- [Vector Mathematics](https://www.mathsisfun.com/algebra/vectors.html)

---

## Tóm Tắt

**Bạn Đã Học Được**:
- ✅ Tại sao Player kế thừa từ Enemy (code reuse, polymorphism)
- ✅ Cách auto-targeting phát hiện và ưu tiên enemies
- ✅ Cách ballistic trajectory calculation tìm góc bắn tối ưu
- ✅ Cách physics simulation dự đoán đường bay của arrow
- ✅ Cách movement, animation, và reload systems hoạt động
- ✅ Kỹ thuật nâng cao (double-shot, manual aim, charged shots, piercing)

**Insights Quan Trọng**:
1. **Player IS an Enemy** - Thiết kế không theo quy ước nhưng hiệu quả
2. **Auto-targeting dùng CircleCast** - Tìm tất cả enemies, chọn gần nhất
3. **Trajectory dùng physics simulation** - Iterate góc để tìm best hit
4. **Công thức Projectile motion**: `y = y₀ + vᵧt - (g/2)t²`
5. **Angle iteration** - Test incrementally cho đến khi tìm được closest approach
6. **Controller2D không phải Rigidbody2D** - Custom raycast-based movement

**Công Thức Physics**:
```
Horizontal: x(t) = x₀ + v * cos(θ) * t
Vertical:   y(t) = y₀ + v * sin(θ) * t - (g/2) * t²
```

**Bước Tiếp Theo**:
- Implement double-shot power-up
- Thêm manual aiming với mouse/touch
- Tạo piercing arrow variant
- Thử nghiệm với charged shots
- Profile trajectory calculation performance

---

**Phiên Bản Tài Liệu**: 1.0
**Cập Nhật Lần Cuối**: 2025-10-29
**Transformation**: Vietnamese (26 lines) → English (2,000+ lines)
**Bổ Sung**: Enemy-Deep.md, 02_Player_System_Complete.md
