---
**🌐 Ngôn ngữ:** Tiếng Việt
**📄 File gốc:** [03_Enemy_System_Complete.md](03_Enemy_System_Complete.md)
**🔄 Cập nhật lần cuối:** 2025-01-30
---

# Hệ Thống Enemy - Hướng Dẫn Đầy Đủ

> **Cho**: Người mới đã hoàn thành Unity Fundamentals
> **Thời gian đọc**: 40-50 phút
> **Yêu cầu**: 00_Unity_Co_Ban.md, 01_Kien_Truc_Project.md

---

## Mục Lục
1. [Tổng Quan Hệ Thống](#tổng-quan-hệ-thống)
2. [Kiến Trúc Enemy](#kiến-trúc-enemy)
3. [Hệ Thống State Machine](#hệ-thống-state-machine)
4. [Hệ Thống Effect (Burn, Freeze, Poison, Shock)](#hệ-thống-effect)
5. [Attack Module (Melee, Range, Throw)](#attack-module)
6. [Di Chuyển & AI](#di-chuyển--ai)
7. [Hệ Thống Health & Damage](#hệ-thống-health--damage)
8. [Điều Khiển Animation](#điều-khiển-animation)
9. [Cấu Hình Inspector](#cấu-hình-inspector)
10. [Cách Tạo Custom Enemy](#cách-tạo-custom-enemy)
11. [Vấn Đề Thường Gặp & Giải Pháp](#vấn-đề-thường-gặp--giải-pháp)

---

## Tổng Quan Hệ Thống

### Hệ Thống Enemy Là Gì?

Hệ Thống Enemy là **hệ thống đối thủ điều khiển bởi AI** trong game này. Enemy:
- **Đi** về phía pháo đài của player
- **Phát hiện** player khi trong range
- **Tấn công** dùng Melee, Range, hoặc Throw
- **Nhận damage** và áp dụng effect hình ảnh/âm thanh
- **Phản ứng** với weapon effect (Freeze, Burn, Poison, Shock)
- **Chết** với animation và rơi coin

### Tại Sao Điều Này Quan Trọng?

Hệ Thống Enemy là **thách thức cốt lõi** của game. Hiểu cách enemy hoạt động cho phép bạn:
- Tạo enemy type mới
- Điều chỉnh độ khó bằng cách thay đổi stat enemy
- Thêm attack pattern mới
- Triển khai weapon effect mới
- Debug vấn đề hành vi AI

### Sơ Đồ Kiến Trúc Hệ Thống

```
┌─────────────────────────────────────────────────────────┐
│                  CLASS CƠ SỞ ENEMY                      │
│  (Health, Effect, State Machine, IListener)             │
└───────────────────┬─────────────────────────────────────┘
                    │
          ┌─────────┴─────────┐
          │                   │
          ▼                   ▼
┌──────────────────┐   ┌─────────────────┐
│ SmartEnemy       │   │ Player_Archer   │ (khác thường!)
│ Grounded         │   │ (kế thừa Enemy) │
└────────┬─────────┘   └─────────────────┘
         │
         │ Dùng Attack Module này:
         │
    ┌────┼─────┬─────────┬─────────┐
    │    │     │         │         │
    ▼    ▼     ▼         ▼         ▼
┌──────┐┌────┐┌───────┐┌───────┐┌──────┐
│Melee ││Range│Throw  ││Check  ││Spawn │
│Attack││Attack│Attack ││Target ││Item  │
└──────┘└────┘└───────┘└───────┘└──────┘
```

### File Chính

| File | Vị Trí | Mục Đích |
|------|--------|----------|
| `Enemy.cs` | `Assets/_MonstersOut/Scripts/AI/` | Class cơ sở cho TẤT CẢ enemy |
| `SmartEnemyGrounded.cs` | `Assets/_MonstersOut/Scripts/AI/` | Triển khai enemy chính |
| `EnemyMeleeAttack.cs` | `Assets/_MonstersOut/Scripts/AI/` | Module tấn công Melee |
| `EnemyRangeAttack.cs` | `Assets/_MonstersOut/Scripts/AI/` | Module tấn công Range |
| `EnemyThrowAttack.cs` | `Assets/_MonstersOut/Scripts/AI/` | Module tấn công Throw |
| `CheckTargetHelper.cs` | `Assets/_MonstersOut/Scripts/Helpers/` | Helper phát hiện target |

---

## Kiến Trúc Enemy

### Cấu Trúc Kế Thừa

```
MonoBehaviour
    │
    ├─ ICanTakeDamage (interface)
    │       │
    │       └─ Cho phép bất kỳ object nào nhận damage
    │
    ├─ IListener (interface)
    │       │
    │       └─ Nhận event game state (Pause, GameOver, v.v.)
    │
    └─ Enemy (class cơ sở)
            │
            ├─ Triển khai ICanTakeDamage
            ├─ Triển khai IListener
            ├─ Có hệ thống health
            ├─ Có hệ thống effect
            ├─ Có state machine
            │
            └─ SmartEnemyGrounded (triển khai chính)
                    │
                    ├─ Thêm logic di chuyển
                    ├─ Thêm logic tấn công
                    └─ Thêm hành vi AI
```

### Tại Sao Thiết Kế Này?

**Lựa Chọn Thiết Kế Khác Thường**: Class cơ sở `Enemy` được dùng bởi CẢ enemy VÀ player!

**Lý do**: Tái sử dụng code. Cả enemy và player đều cần:
- Hệ thống health
- Hệ thống damage
- Hệ thống effect (freeze, burn, poison, shock)
- Hệ thống animation

**Trade-off**:
- **Ưu điểm**: Ít code trùng lặp, bảo trì dễ hơn
- **Nhược điểm**: Đặt tên gây nhầm lẫn (player kế thừa từ class "Enemy")

---

## Hệ Thống State Machine

### State Machine Là Gì?

**State Machine** là hệ thống mà một object có thể ở MỘT state tại một thời điểm, và chuyển đổi giữa các state dựa trên event.

**Ví dụ**: State của Enemy
- IDLE → Phát hiện Player → WALK
- WALK → Player trong range → ATTACK
- ATTACK → Tấn công xong → WALK
- Bất kỳ state → Health = 0 → DEATH

### Enemy State (Enum ENEMYSTATE)

Vị trí trong `Enemy.cs:18-26`

```csharp
public enum ENEMYSTATE
{
    SPAWNING,  // Enemy đang spawn (animation đào lên)
    IDLE,      // Enemy đứng yên
    ATTACK,    // Enemy đang tấn công
    WALK,      // Enemy đang đi
    HIT,       // Enemy bị đánh (không dùng tích cực trong code)
    DEATH      // Enemy chết
}
```

### Sơ Đồ Luồng State

```
┌─────────┐
│ SPAWNING│ (animation đào lên play)
└────┬────┘
     │ spawnDelay (1 giây)
     ▼
┌─────────┐
│  IDLE   │ (đứng yên, đợi)
└────┬────┘
     │ Phát hiện Player
     ▼
┌─────────┐
│  WALK   │───────────┐ (đi về pháo đài)
└────┬────┘           │
     │                │
     │ Player trong   │ Tấn công xong
     │ attack range   │
     │                │
     ▼                │
┌─────────┐           │
│ ATTACK  │───────────┘ (tấn công player)
└────┬────┘
     │ Health = 0
     ▼
┌─────────┐
│  DEATH  │ (play animation chết, cho coin)
└─────────┘
```

### Cách Set State

**Method SetEnemyState()** (`Enemy.cs:234-237`)

```csharp
public void SetEnemyState(ENEMYSTATE state)
{
    enemyState = state;  // Đơn giản cập nhật biến state
}
```

**Ví Dụ Sử Dụng**: Start behavior trong `Enemy.cs:189-207`

```csharp
switch (startBehavior)
{
    case STARTBEHAVIOR.BURROWUP:
        SoundManager.PlaySfx(soundSpawn, soundSpawnVol);
        SetEnemyState(ENEMYSTATE.SPAWNING);  // Set state thành SPAWNING
        AnimSetTrigger("spawn");              // Trigger animation spawn
        Invoke("FinishSpawning", spawnDelay); // Gọi FinishSpawning sau delay
        break;

    case STARTBEHAVIOR.NONE:
    case STARTBEHAVIOR.WALK_LEFT:
    case STARTBEHAVIOR.WALK_RIGHT:
        SetEnemyState(ENEMYSTATE.WALK);  // Set state thành WALK ngay lập tức
        break;
}
```

**Cách Hoạt Động**:
1. Enemy spawn
2. `Start()` kiểm tra setting `startBehavior`
3. Nếu `BURROWUP`, set state thành `SPAWNING` và play animation spawn
4. Sau `spawnDelay` giây, `FinishSpawning()` được gọi
5. `FinishSpawning()` set state thành `WALK`

**Method FinishSpawning()** (`Enemy.cs:210-214`)

```csharp
void FinishSpawning()
{
    // Chỉ chuyển nếu vẫn ở state SPAWNING và game đang chơi
    if (enemyState == ENEMYSTATE.SPAWNING && isPlaying)
        SetEnemyState(ENEMYSTATE.WALK);
}
```

### Sử Dụng State Trong Update Loop

**SmartEnemyGrounded.Update()** (`SmartEnemyGrounded.cs:83-97`)

```csharp
public override void Update()
{
    base.Update();  // Gọi Update parent (xử lý effect)

    HandleAnimation();  // Update animation dựa trên velocity

    // Nếu KHÔNG ở state WALK, dừng di chuyển
    if (enemyState != ENEMYSTATE.WALK || GameManager.Instance.State != GameManager.GameState.Playing)
    {
        velocity.x = 0;  // Set velocity ngang thành 0
        return;           // Thoát sớm
    }

    // Nếu ở state WALK và phát hiện player, bắt đầu đuổi theo
    if (checkTarget.CheckTarget(isFacingRight() ? 1 : -1))
        DetectPlayer(delayChasePlayerWhenDetect);
}
```

**Cách Hoạt Động**:
- Mỗi frame, kiểm tra state hiện tại
- Nếu state KHÔNG phải `WALK`, set velocity thành 0 (dừng di chuyển)
- Nếu state LÀ `WALK`, cho phép di chuyển và kiểm tra player

---

## Hệ Thống Effect

### Effect Là Gì?

**Effect** là điều kiện trạng thái tạm thời áp dụng cho enemy bởi weapon attack. Chúng thay đổi hành vi enemy và gây damage theo thời gian.

### Loại Effect (Enum ENEMYEFFECT)

Vị trí trong `Enemy.cs:28-36`

```csharp
public enum ENEMYEFFECT
{
    NONE,        // Không có effect active
    BURNING,     // Damage theo thời gian (DoT) mỗi frame
    FREEZE,      // Enemy không thể di chuyển, animation play
    SHOKING,     // Damage theo thời gian + stun
    POISON,      // Damage theo thời gian mỗi giây + chậm di chuyển
    EXPLOSION    // Explosion hình ảnh khi chết
}
```

### Sơ Đồ Hệ Thống Effect

```
WEAPON ATTACK
     │
     ├─ Có component WeaponEffect?
     │       │
     │       NO──→ Chỉ damage thường
     │       │
     │       YES
     │       │
     │       └─ Kiểm tra effectType:
     │           │
     │           ├─ FREEZE ──→ Freeze(time)
     │           ├─ POISON ──→ Poison(damage, time)
     │           ├─ BURN ───→ Burning(damage)
     │           └─ SHOCK ──→ Shoking(damage)
     │
     ▼
EFFECT ACTIVE
     │
     ├─ Update() kiểm tra enemyEffect mỗi frame
     ├─ Áp dụng damage/thay đổi hành vi
     └─ Sau duration, xóa effect
```

### Hệ Thống Ưu Tiên Effect

**Chỉ MỘT effect có thể active tại một thời điểm**. Effect có thể ghi đè lẫn nhau:

**Freeze Effect** (`Enemy.cs:416-434`)

```csharp
public virtual void Freeze(float time, GameObject instigator)
{
    // Không thể freeze nếu đã bị freeze
    if (enemyEffect == ENEMYEFFECT.FREEZE)
        return;

    // Nếu đang burn, dừng burn trước
    if (enemyEffect == ENEMYEFFECT.BURNING)
        BurnOut();

    // Nếu đang shock, dừng shock trước
    if (enemyEffect == ENEMYEFFECT.SHOKING)
    {
        UnShock();
    }

    // Áp dụng freeze nếu enemy có thể bị freeze
    if (canBeFreeze)
    {
        enemyEffect = ENEMYEFFECT.FREEZE;  // Set effect hiện tại
        StartCoroutine(UnFreezeCo(time));   // Bắt đầu timer để unfreeze
    }
}
```

**Cách Hoạt Động**:
1. Kiểm tra đã bị freeze chưa → return sớm
2. Nếu đang burn/shock → hủy effect đó
3. Set `enemyEffect = FREEZE`
4. Bắt đầu coroutine để xóa effect sau `time` giây

**Quy Tắc Tương Tác Effect**:
- **Freeze** hủy Burn và Shock
- **Burn** hủy Freeze và Shock
- **Poison** hủy Freeze và Shock
- **Shock** hủy Freeze và Burn
- **Explosion** chỉ áp dụng khi chết

### Freeze Effect (Chi Tiết)

**Method Freeze()** (`Enemy.cs:416-434`)

```csharp
public virtual void Freeze(float time, GameObject instigator)
{
    // Return sớm nếu đã freeze
    if (enemyEffect == ENEMYEFFECT.FREEZE)
        return;

    // Hủy effect xung đột
    if (enemyEffect == ENEMYEFFECT.BURNING)
        BurnOut();

    if (enemyEffect == ENEMYEFFECT.SHOKING)
    {
        UnShock();
    }

    // Áp dụng freeze nếu được phép
    if (canBeFreeze)
    {
        enemyEffect = ENEMYEFFECT.FREEZE;
        StartCoroutine(UnFreezeCo(time));
    }
}
```

**Coroutine UnFreezeCo()** (`Enemy.cs:436-445`)

```csharp
IEnumerator UnFreezeCo(float time)
{
    AnimSetBool("isFreezing", true);  // Bật animation freeze

    // Kiểm tra an toàn (trong trường hợp effect bị hủy)
    if (enemyEffect != ENEMYEFFECT.FREEZE)
        yield break;

    yield return new WaitForSeconds(time);  // Đợi duration
    UnFreeze();  // Xóa freeze effect
}
```

**Method UnFreeze()** (`Enemy.cs:447-454`)

```csharp
void UnFreeze()
{
    // Kiểm tra an toàn
    if (enemyEffect != ENEMYEFFECT.FREEZE)
        return;

    enemyEffect = ENEMYEFFECT.NONE;        // Xóa effect
    AnimSetBool("isFreezing", false);       // Tắt animation freeze
}
```

**Ngăn Chặn Di Chuyển** (`SmartEnemyGrounded.cs:118-119`)

```csharp
// Trong LateUpdate, ngăn di chuyển nếu freeze
if (enemyState != ENEMYSTATE.WALK || enemyEffect == ENEMYEFFECT.FREEZE)
    targetVelocityX = 0;
```

**Sơ Đồ Timeline**:
```
Frame 100: Freeze(3.0f) được gọi
           ├─ enemyEffect = FREEZE
           ├─ AnimSetBool("isFreezing", true)
           └─ Bắt đầu coroutine với wait 3 giây

Frame 101-280: (3 giây = 180 frame @ 60fps)
           ├─ Mỗi frame: Update() kiểm tra enemyEffect
           ├─ LateUpdate() set targetVelocityX = 0
           └─ Animation freeze play

Frame 281: Coroutine kết thúc
           ├─ UnFreeze() được gọi
           ├─ enemyEffect = NONE
           ├─ AnimSetBool("isFreezing", false)
           └─ Enemy có thể di chuyển lại
```

### Burn Effect (Chi Tiết)

**Method Burning()** (`Enemy.cs:459-481`)

```csharp
public virtual void Burning(float damage, GameObject instigator)
{
    // Không thể burn nếu đã burn
    if (enemyEffect == ENEMYEFFECT.BURNING)
        return;

    // Hủy effect xung đột
    if (enemyEffect == ENEMYEFFECT.FREEZE)
    {
        UnFreeze();
    }

    if (enemyEffect == ENEMYEFFECT.SHOKING)
    {
        UnShock();
    }

    // Áp dụng burn nếu được phép
    if (canBeBurn)
    {
        damageBurningPerFrame = damage;      // Lưu damage amount
        enemyEffect = ENEMYEFFECT.BURNING;   // Set effect

        StartCoroutine(BurnOutCo(1));        // Burn kéo dài 1 giây
    }
}
```

**Áp Dụng Damage** (`Enemy.cs:244-247`)

```csharp
public virtual void Update()
{
    // Áp dụng burn damage MỖI FRAME
    if (enemyEffect == ENEMYEFFECT.BURNING)
        CheckDamagePerFrame(damageBurningPerFrame);

    // Áp dụng shock damage MỖI FRAME
    if (enemyEffect == ENEMYEFFECT.SHOKING)
        CheckDamagePerFrame(damageShockingPerFrame);
}
```

**Method CheckDamagePerFrame()** (`Enemy.cs:361-372`)

```csharp
private void CheckDamagePerFrame(float _damage)
{
    // Không áp dụng damage nếu đã chết
    if (enemyState == ENEMYSTATE.DEATH)
        return;

    currentHealth -= (int)_damage;  // Giảm health

    // Update thanh health
    if (healthBar)
        healthBar.UpdateValue(currentHealth / (float)health);

    // Kiểm tra chết
    if (currentHealth <= 0)
        Die();
}
```

**Coroutine BurnOutCo()** (`Enemy.cs:483-499`)

```csharp
IEnumerator BurnOutCo(float time)
{
    // Kiểm tra an toàn
    if (enemyEffect != ENEMYEFFECT.BURNING)
        yield break;

    yield return new WaitForSeconds(time);  // Đợi 1 giây

    // Nếu enemy chết trong lúc burn, disable GameObject
    if (enemyState == ENEMYSTATE.DEATH)
    {
        BurnOut();
        gameObject.SetActive(false);
    }

    BurnOut();  // Xóa burn effect
}
```

**Ví Dụ Tính Toán Burn Damage**:
```
Health Enemy: 100
Burn Damage Mỗi Frame: 0.5
Frame Rate: 60 FPS
Burn Duration: 1 giây

Tổng Frame: 60 frame
Tổng Damage: 0.5 × 60 = 30 damage
Health Cuối: 100 - 30 = 70
```

### Poison Effect (Chi Tiết)

**Method Poison()** (`Enemy.cs:511-536`)

```csharp
public virtual void Poison(float damage, float time, GameObject instigator)
{
    // Không thể poison nếu đã poison hoặc burn
    if (enemyEffect == ENEMYEFFECT.BURNING)
        return;

    if (enemyEffect == ENEMYEFFECT.POISON)
        return;

    // Hủy effect xung đột
    if (enemyEffect == ENEMYEFFECT.FREEZE)
    {
        UnFreeze();
    }

    if (enemyEffect == ENEMYEFFECT.SHOKING)
    {
        UnShock();
    }

    // Áp dụng poison nếu được phép
    if (canBePoison)
    {
        damagePoisonPerSecond = damage;      // Lưu damage mỗi giây
        enemyEffect = ENEMYEFFECT.POISON;    // Set effect

        StartCoroutine(PoisonCo(time));      // Bắt đầu timer poison
    }
}
```

**Coroutine PoisonCo()** (`Enemy.cs:538-575`)

```csharp
IEnumerator PoisonCo(float time)
{
    AnimSetBool("isPoisoning", true);        // Bật animation poison
    multipleSpeed = 1 - poisonSlowSpeed;     // Chậm di chuyển (mặc định 30%)

    // Kiểm tra an toàn
    if (enemyEffect != ENEMYEFFECT.POISON)
        yield break;

    int wait = (int)time;  // Chuyển thành giây integer

    // Áp dụng damage mỗi giây
    while (wait > 0)
    {
        yield return new WaitForSeconds(1);  // Đợi 1 giây

        // Tính damage với resistance
        int _damage = (int)(damagePoisonPerSecond
                      * Random.Range(90 - resistPoisonPercent, 100f - resistPoisonPercent)
                      * 0.01f);

        currentHealth -= _damage;  // Áp dụng damage

        // Update thanh health
        if (healthBar)
            healthBar.UpdateValue(currentHealth / (float)health);

        // Hiện số damage
        FloatingTextManager.Instance.ShowText("" + (int)_damage,
                                               healthBarOffset,
                                               Color.red,
                                               transform.position);

        // Kiểm tra chết
        if (currentHealth <= 0)
        {
            PoisonEnd();
            Die();
            yield break;
        }

        wait -= 1;  // Giảm timer
    }

    // Poison duration kết thúc
    if (enemyState == ENEMYSTATE.DEATH)
    {
        BurnOut();  // (Có vẻ là bug - nên là PoisonEnd)
        gameObject.SetActive(false);
    }

    PoisonEnd();  // Xóa poison effect
}
```

**Ví Dụ Tính Toán Poison Damage**:
```
Base Poison Damage Mỗi Giây: 10
Poison Duration: 5 giây
Resist Poison Percent: 10%

Damage Range Mỗi Giây:
  Min: 10 × (90 - 10) × 0.01 = 10 × 0.80 = 8
  Max: 10 × (100 - 10) × 0.01 = 10 × 0.90 = 9

Tổng Damage Trong 5 Giây: 8-9 damage × 5 giây = 40-45 damage

Tốc Độ Di Chuyển:
  poisonSlowSpeed = 0.3 (30% slow)
  multipleSpeed = 1 - 0.3 = 0.7 (70% tốc độ bình thường)
```

**Chậm Di Chuyển** (`SmartEnemyGrounded.cs:131`)

```csharp
// Di chuyển được nhân với multipleSpeed
controller.Move(velocity * Time.deltaTime * multipleSpeed, false, isFacingRight());
```

### Shock Effect (Chi Tiết)

**Method Shoking()** (`Enemy.cs:591-610`)

```csharp
public virtual void Shoking(float damage, GameObject instigator)
{
    // Không thể shock nếu đã shock
    if (enemyEffect == ENEMYEFFECT.SHOKING)
        return;

    // Hủy effect xung đột
    if (enemyEffect == ENEMYEFFECT.FREEZE)
    {
        UnFreeze();
    }

    if (enemyEffect == ENEMYEFFECT.BURNING)
        BurnOut();

    // Áp dụng shock nếu được phép
    if (canBeShock)
    {
        damageShockingPerFrame = damage;      // Lưu damage mỗi frame
        enemyEffect = ENEMYEFFECT.SHOKING;    // Set effect
        StartCoroutine(UnShockCo());          // Bắt đầu timer shock
    }
}
```

**Áp Dụng Damage** (`Enemy.cs:249-250`)

```csharp
// Trong Update(), áp dụng shock damage mỗi frame (giống burn)
if (enemyEffect == ENEMYEFFECT.SHOKING)
    CheckDamagePerFrame(damageShockingPerFrame);
```

**Ngăn Chặn Di Chuyển** (`SmartEnemyGrounded.cs:105-109`)

```csharp
// Trong LateUpdate, ngăn di chuyển nếu shocking
else if (!isPlaying || isSocking || enemyEffect == ENEMYEFFECT.SHOKING)
{
    velocity = Vector2.zero;  // Dừng hoàn toàn
    return;
}
```

**Coroutine UnShockCo()** (`Enemy.cs:612-620`)

```csharp
IEnumerator UnShockCo()
{
    // Kiểm tra an toàn
    if (enemyEffect != ENEMYEFFECT.SHOKING)
        yield break;

    yield return new WaitForSeconds(timeShocking);  // Mặc định 2 giây

    UnShock();  // Xóa shock effect
}
```

**Ví Dụ Tính Toán Shock Damage**:
```
Shock Damage Mỗi Frame: 0.3
Shock Duration: 2 giây
Frame Rate: 60 FPS

Tổng Frame: 60 × 2 = 120 frame
Tổng Damage: 0.3 × 120 = 36 damage

Hành vi:
- Enemy KHÔNG THỂ di chuyển (velocity = 0)
- Enemy KHÔNG THỂ tấn công
- Nhận 36 damage trong 2 giây
```

### Explosion Effect

**Explosion** là effect đặc biệt chỉ kích hoạt khi chết.

**Method TakeDamage()** (`Enemy.cs:690-698`)

```csharp
if (currentHealth <= 0)
{
    // Kiểm tra enemy có nên phát nổ khi chết
    if (isExplosion || dieBehavior == DIEBEHAVIOR.BLOWUP)
    {
        SetEnemyEffect(ENEMYEFFECT.EXPLOSION);
    }

    Die();
}
```

**Method Die() Với Explosion** (`Enemy.cs:337-355`)

```csharp
// Nếu explosion effect đang active
if (enemyEffect == ENEMYEFFECT.EXPLOSION)
{
    // Spawn blood puddle
    if (bloodPuddleFX)
    {
        for (int i = 0; i < Random.Range(2, 5); i++)
        {
            Instantiate(bloodPuddleFX,
                       (Vector2)transform.position + new Vector2(
                           Random.Range(-(randomBloodPuddlePoint.x * 2), randomBloodPuddlePoint.x * 2),
                           Random.Range(-(2 * randomBloodPuddlePoint.y), 2 * randomBloodPuddlePoint.y)
                       ),
                       Quaternion.identity);
        }
    }

    // Spawn explosion effect
    if (explosionFX.Length > 0)
    {
        for (int i = 0; i < Random.Range(1, 3); i++)
        {
            Instantiate(explosionFX[Random.Range(0, explosionFX.Length)],
                       transform.position,
                       Quaternion.identity);
        }
    }

    // Play explosion sound
    SoundManager.PlaySfx(soundDieBlow, soundDieBlowVol);
}
else
    SoundManager.PlaySfx(soundDie, soundDieVol);  // Sound chết bình thường
```

---

## Attack Module

### Tổng Quan Hệ Thống Attack

Enemy dùng **component attack module** đính kèm vào GameObject enemy. Điều này cho phép mix và match loại attack mà không trùng code.

### Loại Attack (Enum ATTACKTYPE)

Vị trí trong `Enemy.cs:10-16`

```csharp
public enum ATTACKTYPE
{
    RANGE,   // Bắn projectile (súng)
    MELEE,   // Tấn công cận chiến (kiếm, móng vuốt)
    THROW,   // Ném lựu đạn/bom
    NONE     // Không tấn công (enemy passive)
}
```

### Kiến Trúc Attack Module

```
SmartEnemyGrounded
    │
    ├─ attackType = RANGE/MELEE/THROW
    │
    ├─ Method CheckAttack()
    │       │
    │       └─ switch(attackType) {
    │           ├─ RANGE  → rangeAttack.Action()
    │           ├─ MELEE  → meleeAttack.Action()
    │           └─ THROW  → throwAttack.Action()
    │           }
    │
    └─ Attack Module (GetComponent)
            │
            ├─ EnemyRangeAttack
            ├─ EnemyMeleeAttack
            └─ EnemyThrowAttack
```

### Melee Attack Module

**File**: `EnemyMeleeAttack.cs`

**Mục đích**: Tấn công cận chiến dùng phát hiện CircleCast

**Cách Hoạt Động**:

1. **Phát Hiện** (method `CheckPlayer()`)
```csharp
public bool CheckPlayer(bool _isFacingRight)
{
    isFacingRight = _isFacingRight;

    // Cast circle tại checkPoint để phát hiện player
    RaycastHit2D hit = Physics2D.CircleCast(
        checkPoint.position,   // Center
        radiusCheck,           // Radius (mặc định 1)
        Vector2.zero,          // Hướng (không có, chỉ check vùng)
        0,                     // Distance (0 = chỉ check tại center)
        targetLayer            // LayerMask (thường là layer player)
    );

    if (hit)
        return true;  // Player trong range
    else
        return false; // Không phát hiện player
}
```

2. **Thực Hiện Attack** (method `Check4Hit()`)

```csharp
public void Check4Hit()
{
    // Tìm TẤT CẢ target trong range (radius hơi lớn hơn)
    RaycastHit2D[] hits = Physics2D.CircleCastAll(
        checkPoint.position,
        radiusCheck * 1.2f,   // 20% lớn hơn để phát hiện hit tốt hơn
        Vector2.zero,
        0,
        targetLayer
    );

    int counterHit = 0;  // Theo dõi bao nhiêu target trúng

    if (hits.Length > 0)
    {
        foreach (var hit in hits)
        {
            // Chỉ hit tối đa maxTargetPerHit target
            if (counterHit < maxTargetPerHit)
            {
                // Kiểm tra target có thể nhận damage
                var takeDamage = (ICanTakeDamage)hit.collider.gameObject
                                 .GetComponent(typeof(ICanTakeDamage));

                if (takeDamage != null)
                {
                    // Tính damage với random variance
                    float _damage = dealDamage + (int)(Random.Range(-0.1f, 0.1f) * dealDamage);

                    // Kiểm tra critical hit (mặc định 10% chance)
                    if (Random.Range(0, 100) < criticalPercent)
                    {
                        _damage *= 2;  // Damage gấp đôi
                        FloatingTextManager.Instance.ShowText(
                            "CRIT!",
                            Vector3.up,
                            Color.red,
                            hit.collider.gameObject.transform.position,
                            30
                        );
                    }

                    // Áp dụng damage với weapon effect
                    if (hasWeaponEffect != null)
                    {
                        takeDamage.TakeDamage(_damage, Vector2.zero, hit.point,
                                              gameObject, BODYPART.NONE, hasWeaponEffect);
                    }
                    else
                        takeDamage.TakeDamage(_damage, Vector2.zero, hit.point, gameObject);

                    counterHit++;  // Tăng counter hit
                }

                // Play attack sound
                if (soundAttacks.Length > 0)
                    SoundManager.PlaySfx(soundAttacks[Random.Range(0, soundAttacks.Length)],
                                        soundAttacksVol);
            }
        }
    }
}
```

**Timeline Attack**:
```
Frame 100: CheckPlayer() trả về true (player trong range)
           ├─ CheckAttack() được gọi
           └─ Action() set lastShoot = Time.time

Frame 101: AnimSetTrigger("melee") trigger animation melee
           └─ Animation play

Frame 115: Animation Event gọi AnimMeleeAttackStart()
           ├─ Check4Hit() được gọi
           ├─ CircleCast phát hiện player
           ├─ Tính damage (18-22, hoặc 36-44 nếu crit)
           ├─ Gọi player.TakeDamage()
           └─ Play attack sound

Frame 130: Animation Event gọi AnimMeleeAttackEnd()
           ├─ EndCheck4Hit() được gọi
           └─ Invoke("EndAttack", 1) lên lịch EndAttack trong 1 giây

Frame 190: (1 giây sau)
           ├─ EndAttack() được gọi
           ├─ isAttacking = false
           └─ Enemy có thể tấn công lại
```

**Setting Inspector**:
- `targetLayer`: Có thể tấn công cái gì (thường là layer Player)
- `checkPoint`: Transform nơi circle được cast (thường ở trước enemy)
- `radiusCheck`: Melee reach bao xa (mặc định 1 mét)
- `dealDamage`: Damage cơ bản (mặc định 20)
- `criticalPercent`: Chance gây 2x damage (mặc định 10%)
- `meleeRate`: Cooldown giữa các attack (mặc định 1 giây)

### Range Attack Module

**File**: `EnemyRangeAttack.cs`

**Mục đích**: Bắn projectile vào player từ khoảng cách

**Cách Hoạt Động**:

1. **Phát Hiện Target** (method `CheckPlayer()`)

```csharp
public bool CheckPlayer(bool isFacingRight)
{
    dir = isFacingRight ? Vector2.right : Vector2.left;
    bool isHit = false;

    // Tìm TẤT CẢ enemy trong circle radius rất lớn
    RaycastHit2D[] hits = Physics2D.CircleCastAll(
        checkPoint.position,
        detectDistance,      // Mặc định 5 mét
        Vector2.zero,
        0,
        enemyLayer           // Thực ra nhắm layer player (đặt tên nhầm)
    );

    if (hits.Length > 0)
    {
        float closestDistance = 99999;

        // Tìm target gần nhất
        foreach (var obj in hits)
        {
            var checkEnemy = (ICanTakeDamage)obj.collider.gameObject
                            .GetComponent(typeof(ICanTakeDamage));

            if (checkEnemy != null)
            {
                // Tính khoảng cách ngang
                if (Mathf.Abs(obj.transform.position.x - checkPoint.position.x) < closestDistance)
                {
                    closestDistance = Mathf.Abs(obj.transform.position.x - checkPoint.position.x);

                    // Xác minh line of sight với raycast
                    var hit = Physics2D.Raycast(
                        checkPoint.position,
                        (obj.point - (Vector2)checkPoint.position),
                        detectDistance,
                        enemyLayer
                    );

                    // Vẽ ray debug (thấy trong Scene view)
                    Debug.DrawRay(
                        checkPoint.position,
                        (obj.point - (Vector2)checkPoint.position) * 100,
                        Color.red
                    );

                    // Lưu vị trí target
                    _target = obj.collider.gameObject.transform.position;
                    isHit = true;
                }
            }
        }
    }

    return isHit;
}
```

2. **Spawn Projectile** (method `Shoot()`)

```csharp
public void Shoot(bool isFacingRight)
{
    // Bắt đầu coroutine để xử lý multi-shot
    StartCoroutine(ShootCo(isFacingRight));
}

IEnumerator ShootCo(bool isFacingRight)
{
    // Lặp cho multi-shot (mặc định 1)
    for (int i = 0; i < multiShoot; i++)
    {
        SoundManager.PlaySfx(soundShoot, soundShootVolume);

        // Tính góc bắn (0 = phải, 180 = trái)
        float shootAngle = 0;
        shootAngle = isFacingRight ? 0 : 180;

        // Spawn bullet tại shooting point
        var obj = Instantiate(
            bullet.gameObject,
            shootingPoint != null ? shootingPoint.position : firePosition(),
            Quaternion.Euler(0, 0, shootAngle)
        );

        var projectile = obj.GetComponent<Projectile>();

        // Tính hướng bắn
        Vector3 _dir;
        if (aimTarget)
        {
            // Ngắm vị trí target
            _dir = _target - shootingPoint.position;
            _dir += (Vector3)aimTargetOffset;  // Thêm offset (thường lên trên)
            _dir.Normalize();                   // Làm unit vector
        }
        else
        {
            // Bắn thẳng
            _dir = Vector2.right * (isFacingRight ? 1 : -1);
        }

        // Khởi tạo projectile với weapon effect
        if (hasWeaponEffect != null)
        {
            projectile.Initialize(
                gameObject,           // Owner
                _dir,                 // Hướng
                Vector2.zero,         // Force (không dùng)
                false,                // Is crit
                damage * 0.9f,        // Min damage
                damage * 1.1f,        // Max damage
                0,                    // Crit percent (xử lý riêng)
                Vector2.zero,         // Knockback
                hasWeaponEffect       // Weapon effect
            );
        }
        else
            projectile.Initialize(gameObject, _dir, Vector2.zero, false,
                                 damage * 0.9f, damage * 1.1f, 0);

        projectile.gameObject.SetActive(true);

        // Đợi trước shot tiếp (cho multi-shot)
        yield return new WaitForSeconds(multiShootRate);
    }

    CancelInvoke();
    Invoke("EndAttack", 1);  // Đánh dấu attack kết thúc sau 1 giây
}
```

**Luồng Attack**:
```
1. CheckPlayer() phát hiện player trong radius 5-mét
   └─ Xác minh line of sight với raycast

2. Action() set lastShoot time và isAttacking = true

3. AnimSetTrigger("shoot") play animation bắn

4. Animation Event gọi AnimShoot()

5. Shoot(isFacingRight) được gọi
   └─ Coroutine ShootCo() bắt đầu

6. Cho mỗi bullet trong multiShoot:
   ├─ Play shoot sound
   ├─ Spawn bullet prefab
   ├─ Tính hướng (aim hoặc thẳng)
   ├─ Khởi tạo projectile với damage/effect
   └─ Đợi multiShootRate giây

7. Sau tất cả bullet, đợi 1 giây
   └─ EndAttack() set isAttacking = false
```

**Setting Inspector**:
- `enemyLayer`: Nhắm cái gì (thường là layer Player)
- `checkPoint`: Center point phát hiện
- `firePoint`: Nơi bullet spawn hình ảnh (nòng súng)
- `shootingPoint`: Vị trí spawn thực tế (có thể khác)
- `damage`: Damage cơ bản (mặc định 30)
- `detectDistance`: Có thể phát hiện player bao xa (mặc định 5)
- `bullet`: Projectile prefab để spawn
- `shootingRate`: Cooldown giữa các attack (mặc định 1 giây)
- `aimTarget`: Nếu true, ngắm player. Nếu false, bắn thẳng
- `aimTargetOffset`: Offset cho ngắm (thường lên để trúng thân)

### Throw Attack Module

**File**: `EnemyThrowAttack.cs`

**Mục đích**: Ném lựu đạn/bom dùng vật lý

**Cách Hoạt Động**:

1. **Phát Hiện Target** (method `CheckPlayer()`)

```csharp
public bool CheckPlayer()
{
    // Kiểm tra player trong radius
    RaycastHit2D[] hits = Physics2D.CircleCastAll(
        checkPoint.position,
        radiusDetectPlayer,   // Mặc định 5 mét
        Vector2.zero,
        0,
        targetPlayer
    );

    if (hits.Length > 0)
    {
        foreach (var hit in hits)
        {
            // Nếu onlyAttackTheFortrest = true, chỉ ném vào fortress
            if (onlyAttackTheFortrest)
            {
                if (hit.collider.gameObject.GetComponent<TheFortrest>())
                    return true;
            }
            else
                return true;  // Tấn công bất kỳ target phát hiện
        }
    }
    return false;
}
```

2. **Thực Hiện Ném** (method `Throw()`)

```csharp
public void Throw(bool isFacingRight)
{
    // Lấy vị trí ném
    Vector3 throwPos = throwPosition.position;

    // Spawn grenade prefab
    GameObject obj = Instantiate(_Grenade, throwPos, Quaternion.identity) as GameObject;

    // Tính góc ném
    float angle;
    angle = isFacingRight ? angleThrow : 135;  // Mặc định 60° hoặc 135°

    // Xoay grenade đến góc
    obj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

    // Áp dụng force lên Rigidbody2D
    obj.GetComponent<Rigidbody2D>().AddRelativeForce(
        obj.transform.right * Random.Range(throwForceMin, throwForceMax)
    );

    // Áp dụng torque (xoay trong lúc bay)
    obj.GetComponent<Rigidbody2D>().AddTorque(
        obj.transform.right.x * addTorque
    );
}
```

**Giải Thích Vật Lý**:

```
Trajectory Grenade:

                    ╱╲
                  ╱    ╲
                ╱        ╲
              ╱            ╲
Enemy ───────              ─────── Sàn
        60°                Điểm Impact

Force: 290-320
Angle: 60° (nếu facing right)
Gravity: -35 (set trong Rigidbody2D)

Công thức:
  Khoảng Cách Ngang = (Force² × sin(2×Angle)) / Gravity
  Ví dụ: (300² × sin(120°)) / 35 ≈ 2.2 mét
```

**Luồng Attack**:
```
1. CheckPlayer() phát hiện fortress/player trong range

2. Action() set lastShoot time

3. AnimSetTrigger("throw") play animation ném

4. Animation Event gọi AnimThrow()

5. Throw(isFacingRight) được gọi
   ├─ Spawn grenade tại throwPosition
   ├─ Xoay đến angle (60° hoặc 135°)
   ├─ AddRelativeForce (di chuyển theo hướng đã xoay)
   └─ AddTorque (xoay grenade)

6. Grenade bay theo cung (mô phỏng vật lý)

7. Grenade hạ cánh và phát nổ (xử lý bởi script grenade)
```

**Setting Inspector**:
- `angleThrow`: Góc ném khi facing right (mặc định 60°)
- `throwForceMin/Max`: Range force (mặc định 290-320)
- `addTorque`: Tốc độ xoay (mặc định 100)
- `throwRate`: Cooldown giữa các ném (mặc định 0.5 giây)
- `throwPosition`: Nơi grenade spawn
- `_Grenade`: Grenade prefab để ném
- `targetPlayer`: Phát hiện cái gì
- `onlyAttackTheFortrest`: Chỉ ném vào fortress (không player)
- `radiusDetectPlayer`: Radius phát hiện (mặc định 5)

### Hệ Thống Kiểm Tra Attack

**Method CheckAttack()** (`SmartEnemyGrounded.cs:188-258`)

```csharp
void CheckAttack()
{
    // Switch dựa trên enum attackType
    switch (attackType)
    {
        case ATTACKTYPE.RANGE:
            // Kiểm tra cooldown kết thúc chưa
            if (rangeAttack.AllowAction())
            {
                // Set state thành attacking
                SetEnemyState(ENEMYSTATE.ATTACK);

                // Kiểm tra player có trong range không
                if (rangeAttack.CheckPlayer(isFacingRight()))
                {
                    rangeAttack.Action();           // Bắt đầu cooldown attack
                    AnimSetTrigger("shoot");        // Play animation
                    DetectPlayer();                 // Đảm bảo player được đánh dấu đã phát hiện
                }
                else if (!rangeAttack.isAttacking && enemyState == ENEMYSTATE.ATTACK)
                {
                    // Player di chuyển ra khỏi range, tiếp tục đi
                    SetEnemyState(ENEMYSTATE.WALK);
                }
            }
            break;

        case ATTACKTYPE.MELEE:
            if (meleeAttack.AllowAction())
            {
                if (meleeAttack.CheckPlayer(isFacingRight()))
                {
                    SetEnemyState(ENEMYSTATE.ATTACK);
                    meleeAttack.Action();
                    AnimSetTrigger("melee");
                }
                else if (!meleeAttack.isAttacking && enemyState == ENEMYSTATE.ATTACK)
                {
                    SetEnemyState(ENEMYSTATE.WALK);
                }
            }
            break;

        case ATTACKTYPE.THROW:
            if (throwAttack.AllowAction())
            {
                SetEnemyState(ENEMYSTATE.ATTACK);

                if (throwAttack.CheckPlayer())
                {
                    throwAttack.Action();
                    AnimSetTrigger("throw");
                }
                else if (!throwAttack.isAttacking && enemyState == ENEMYSTATE.ATTACK)
                {
                    SetEnemyState(ENEMYSTATE.WALK);
                }
            }
            break;

        default:
            break;
    }
}
```

**Khi Nào CheckAttack() Được Gọi?**

```csharp
// Trong LateUpdate() của SmartEnemyGrounded
if (isPlaying && isPlayerDetected && allowCheckAttack && enemyEffect != ENEMYEFFECT.FREEZE)
{
    CheckAttack();
}
```

**Điều Kiện**:
- `isPlaying` = true (game đang chơi)
- `isPlayerDetected` = true (player được phát hiện)
- `allowCheckAttack` = true (không đang thực hiện action đặc biệt)
- `enemyEffect != FREEZE` (không bị freeze)

---

## Di Chuyển & AI

### Tổng Quan Hệ Thống Di Chuyển

Enemy dùng **Controller2D** cho di chuyển dựa trên vật lý tùy chỉnh (giống player).

### Luồng Di Chuyển

```
LateUpdate() được gọi mỗi frame vật lý
    │
    ├─ Kiểm tra game đang chơi
    ├─ Kiểm tra enemy có thể di chuyển
    │   ├─ isPlaying?
    │   ├─ isSocking?
    │   └─ enemyEffect == FREEZE?
    │
    ├─ Tính target velocity
    │   └─ targetVelocityX = direction.x × moveSpeed
    │
    ├─ Áp dụng gravity
    │   └─ velocity.y += -gravity × Time.deltaTime
    │
    ├─ Smooth velocity
    │   └─ SmoothDamp cho tăng tốc mượt
    │
    ├─ Kiểm tra collision
    │   └─ Nếu đụng tường, dừng
    │
    └─ Di chuyển character
        └─ controller.Move(velocity × deltaTime × multipleSpeed)
```

### Method LateUpdate() (Đầy Đủ)

Vị trí trong `SmartEnemyGrounded.cs:99-140`

```csharp
public virtual void LateUpdate()
{
    // Nếu game không đang chơi, dừng
    if (GameManager.Instance.State != GameManager.GameState.Playing)
        return;

    // Nếu enemy đang dừng hoặc shocking, set velocity thành 0
    else if (!isPlaying || isSocking || enemyEffect == ENEMYEFFECT.SHOKING)
    {
        velocity = Vector2.zero;
        return;
    }

    // Tính target horizontal velocity
    float targetVelocityX = _direction.x * moveSpeed;

    // Nếu shocking, dừng
    if (isSocking || enemyEffect == ENEMYEFFECT.SHOKING)
    {
        targetVelocityX = 0;
    }

    // Nếu không đi hoặc freeze, dừng
    if (enemyState != ENEMYSTATE.WALK || enemyEffect == ENEMYEFFECT.FREEZE)
        targetVelocityX = 0;

    // Nếu thủ công dừng hoặc stun, dừng
    if (isStopping || isStunning)
        targetVelocityX = 0;

    // Smooth thay đổi velocity (ngăn tăng tốc tức thì)
    velocity.x = Mathf.SmoothDamp(
        velocity.x,              // Velocity hiện tại
        targetVelocityX,         // Velocity target
        ref velocityXSmoothing,  // Biến smoothing
        (controller.collisions.below) ? 0.1f : 0.2f  // Thời gian smoothing
    );

    // Áp dụng gravity
    velocity.y += -gravity * Time.deltaTime;

    // Nếu đụng tường, dừng di chuyển ngang
    if ((_direction.x > 0 && controller.collisions.right) ||
        (_direction.x < 0 && controller.collisions.left))
        velocity.x = 0;

    // Di chuyển character controller
    controller.Move(
        velocity * Time.deltaTime * multipleSpeed,  // Lượng di chuyển
        false,                                       // Không nhảy
        isFacingRight()                              // Hướng facing
    );

    // Nếu đụng trần hoặc sàn, dừng di chuyển dọc
    if (controller.collisions.above || controller.collisions.below)
        velocity.y = 0;

    // Kiểm tra có thể tấn công
    if (isPlaying && isPlayerDetected && allowCheckAttack && enemyEffect != ENEMYEFFECT.FREEZE)
    {
        CheckAttack();
    }
}
```

### Giải Thích Smooth Velocity

**Mathf.SmoothDamp()** tạo tăng/giảm tốc mượt.

```csharp
velocity.x = Mathf.SmoothDamp(
    velocity.x,              // Hiện tại: 0
    targetVelocityX,         // Target: 3
    ref velocityXSmoothing,  // Biến reference (lưu state)
    0.1f                     // Thời gian đạt target
);
```

**Cách Hoạt Động**:

```
Frame 1:  velocity = 0,    target = 3  →  velocity = 1.5
Frame 2:  velocity = 1.5,  target = 3  →  velocity = 2.4
Frame 3:  velocity = 2.4,  target = 3  →  velocity = 2.8
Frame 4:  velocity = 2.8,  target = 3  →  velocity = 2.95
Frame 5:  velocity = 2.95, target = 3  →  velocity = 3.0

Kết quả: Đường cong tăng tốc mượt thay vì nhảy tức thì
```

**Tại Sao Smoothing Time Khác Nhau?**

```csharp
(controller.collisions.below) ? 0.1f : 0.2f
```

- **0.1f** (below = true): Trên sàn → tăng tốc nhanh hơn
- **0.2f** (below = false): Trong không khí → tăng tốc chậm hơn (thực tế hơn)

### Hướng & Facing

**Hướng Ban Đầu** (`SmartEnemyGrounded.Start()` dòng 43-49)

```csharp
// Lấy hướng dựa trên rotation
_direction = isFacingRight() ? Vector2.right : Vector2.left;

// Nếu hướng không khớp start behavior, flip
if ((_direction == Vector2.right && startBehavior == STARTBEHAVIOR.WALK_LEFT) ||
    (_direction == Vector2.left && startBehavior == STARTBEHAVIOR.WALK_RIGHT))
{
    Flip();
}
```

**Method isFacingRight()** (`Enemy.cs:155-159`)

```csharp
public bool isFacingRight()
{
    // Kiểm tra rotation Y: 180 = facing right, 0 = facing left
    return transform.rotation.eulerAngles.y == 180 ? true : false;
}
```

**Method Flip()** (`SmartEnemyGrounded.cs:142-147`)

```csharp
void Flip()
{
    // Đảo hướng
    _direction = -_direction;

    // Update rotation
    transform.rotation = Quaternion.Euler(
        new Vector3(
            transform.rotation.x,
            isFacingRight() ? 0 : 180,  // Chuyển giữa 0 và 180
            transform.rotation.z
        )
    );
}
```

**Sơ Đồ Rotation**:
```
Facing TRÁI:                 Facing PHẢI:
Rotation.y = 0               Rotation.y = 180
     ◄───                         ───►
    Enemy                        Enemy

Sprite quay trái            Sprite quay phải
```

### Phát Hiện Player

**Method DetectPlayer()** (`Enemy.cs:257-263`)

```csharp
public virtual void DetectPlayer(float delayChase = 0)
{
    // Nếu đã phát hiện, không làm gì
    if (isPlayerDetected)
        return;

    // Đuổi player sau delay
    StartCoroutine(DelayBeforeChasePlayer(delayChase));
}
```

**Coroutine DelayBeforeChasePlayer()** (`Enemy.cs:266-286`)

```csharp
protected IEnumerator DelayBeforeChasePlayer(float delay)
{
    yield return null;  // Đợi một frame

    // Đợi cho đến khi không dừng hoặc stun
    while (isStopping || isStunning) { yield return null; }

    isPlayerDetected = true;  // Đánh dấu player đã phát hiện

    if (delay > 0)
    {
        // Dừng di chuyển trong delay
        SetEnemyState(ENEMYSTATE.IDLE);

        // Đợi delay time
        yield return new WaitForSeconds(delay);
    }

    // Nếu đã đang tấn công, không đổi state
    if (enemyState == ENEMYSTATE.ATTACK)
    {
        yield break;
    }

    // Bắt đầu đi về player
    SetEnemyState(ENEMYSTATE.WALK);
}
```

**Luồng Phát Hiện**:
```
Frame 100: checkTarget.CheckTarget() trả về true
           └─ DetectPlayer(1.0f) được gọi

Frame 101: Coroutine DelayBeforeChasePlayer bắt đầu
           ├─ isPlayerDetected = true
           ├─ SetEnemyState(IDLE)
           └─ Bắt đầu wait 1 giây

Frame 160: (1 giây sau)
           ├─ Kiểm tra đang tấn công
           └─ SetEnemyState(WALK)

Frame 161+: Enemy đi về player
            └─ CheckAttack() được gọi mỗi frame
```

---

## Hệ Thống Health & Damage

### Khởi Tạo Health

**Method Start()** (`Enemy.cs:170-187`)

```csharp
public virtual void Start()
{
    // Nếu upgraded character ID tồn tại, dùng health upgraded
    if (upgradedCharacterID != null)
    {
        health = upgradedCharacterID.UpgradeHealth;
    }

    currentHealth = health;  // Set current health thành max

    moveSpeed = walkSpeed;   // Khởi tạo move speed

    // Spawn health bar từ folder Resources
    var healthBarObj = (HealthBarEnemyNew)Resources.Load("HealthBar", typeof(HealthBarEnemyNew));
    healthBar = (HealthBarEnemyNew)Instantiate(healthBarObj, healthBarOffset, Quaternion.identity);

    healthBar.Init(transform, (Vector3)healthBarOffset);  // Đính kèm vào enemy

    // Lấy component
    anim = GetComponent<Animator>();
    checkTarget = GetComponent<CheckTargetHelper>();

    // Xử lý start behavior (animation spawn, v.v.)
    // ... (xem phần State Machine)
}
```

### Method TakeDamage() (Đầy Đủ)

Vị trí trong `Enemy.cs:662-723`

```csharp
public void TakeDamage(float damage, Vector2 force, Vector2 hitPoint,
                       GameObject instigator, BODYPART bodyPart = BODYPART.NONE,
                       WeaponEffect weaponEffect = null)
{
    // Không action nếu đã chết
    if (enemyState == ENEMYSTATE.DEATH)
        return;

    // Không action nếu thủ công dừng
    if (isStopping)
        return;

    // Lưu parameter
    _bodyPart = bodyPart;
    _bodyPartForce = force;
    _damage = damage;

    // Lấy hit point cho effect
    hitPos = hitPoint;
    bool isExplosion = false;

    // Giảm health
    currentHealth -= (int)damage;

    // Hiện số damage
    FloatingTextManager.Instance.ShowText(
        "" + (int)damage,
        healthBarOffset,
        Color.red,
        transform.position
    );

    knockBackForce = force;

    // Spawn hit effect tại vị trí random gần hit point
    if (hitFX)
        Instantiate(hitFX,
                   hitPos + new Vector2(
                       Random.Range(-randomHitPoint.x, randomHitPoint.x),
                       Random.Range(-randomHitPoint.y, randomHitPoint.y)
                   ),
                   Quaternion.identity);

    // Spawn blood puddle
    if (bloodPuddleFX)
        Instantiate(bloodPuddleFX,
                   (Vector2)transform.position + new Vector2(
                       Random.Range(-randomBloodPuddlePoint.x, randomBloodPuddlePoint.x),
                       Random.Range(-randomBloodPuddlePoint.y, randomBloodPuddlePoint.y)
                   ),
                   Quaternion.identity);

    // Update health bar
    if (healthBar)
        healthBar.UpdateValue(currentHealth / (float)health);

    // Kiểm tra chết
    if (currentHealth <= 0)
    {
        // Kiểm tra có nên phát nổ khi chết
        if (isExplosion || dieBehavior == DIEBEHAVIOR.BLOWUP)
        {
            SetEnemyEffect(ENEMYEFFECT.EXPLOSION);
        }

        Die();
    }
    else
    {
        // Nếu sống, kiểm tra weapon effect
        if (weaponEffect != null)
        {
            switch (weaponEffect.effectType)
            {
                case WEAPON_EFFECT.POISON:
                    // Áp dụng poison
                    Poison(weaponEffect.poisonDamagePerSec,
                          weaponEffect.poisonTime,
                          instigator);
                    return;

                case WEAPON_EFFECT.FREEZE:
                    Freeze(weaponEffect.freezeTime, instigator);
                    return;

                case WEAPON_EFFECT.NORMAL:
                    break;

                default:
                    break;
            }
        }

        Hit(force);  // Gọi method Hit cho damage không chết mạng
    }
}
```

### Sơ Đồ Luồng Damage

```
Weapon trúng enemy
    │
    ├─ TakeDamage() được gọi
    │
    ├─ Kiểm tra chết/đang dừng → return
    │
    ├─ currentHealth -= damage
    │
    ├─ Hiện số damage (FloatingText)
    │
    ├─ Spawn hit effect (blood splash)
    │
    ├─ Spawn blood puddle
    │
    ├─ Update health bar
    │
    ├─ Kiểm tra health:
    │   │
    │   ├─ currentHealth <= 0?
    │   │   │
    │   │   YES─→ Die()
    │   │   │
    │   │   NO──→ Kiểm tra weaponEffect:
    │   │         │
    │   │         ├─ POISON → Poison()
    │   │         ├─ FREEZE → Freeze()
    │   │         └─ NORMAL → Hit()
    │   │
    │   └─ End
    │
    └─ End
```

### Method Die() (Đầy Đủ)

Vị trí trong `Enemy.cs:316-359`

```csharp
public virtual void Die()
{
    // Dừng game
    isPlaying = false;

    // Xóa khỏi listener list
    GameManager.Instance.RemoveListener(this);

    isPlayerDetected = false;

    SetEnemyState(ENEMYSTATE.DEATH);

    // Cho coin (nếu có component GiveCoinWhenDie)
    if (GetComponent<GiveCoinWhenDie>())
    {
        GetComponent<GiveCoinWhenDie>().GiveCoin();
    }

    // Spawn death effect
    if (dieFX)
        Instantiate(dieFX, transform.position, dieFX.transform.rotation);

    // Nếu chết trong lúc freeze, spawn frozen death effect
    if (enemyEffect == ENEMYEFFECT.FREEZE && dieFrozenFX)
        Instantiate(dieFrozenFX, hitPos, Quaternion.identity);

    // Nếu shocking, xóa shock effect
    if (enemyEffect == ENEMYEFFECT.SHOKING)
        UnShock();

    // Nếu explosion effect, spawn blood và explosion
    if (enemyEffect == ENEMYEFFECT.EXPLOSION)
    {
        // Spawn 2-5 blood puddle
        if (bloodPuddleFX)
        {
            for (int i = 0; i < Random.Range(2, 5); i++)
            {
                Instantiate(bloodPuddleFX,
                           (Vector2)transform.position + new Vector2(
                               Random.Range(-(randomBloodPuddlePoint.x * 2), randomBloodPuddlePoint.x * 2),
                               Random.Range(-(2 * randomBloodPuddlePoint.y), 2 * randomBloodPuddlePoint.y)
                           ),
                           Quaternion.identity);
            }
        }

        // Spawn 1-3 explosion effect
        if (explosionFX.Length > 0)
        {
            for (int i = 0; i < Random.Range(1, 3); i++)
            {
                Instantiate(explosionFX[Random.Range(0, explosionFX.Length)],
                           transform.position,
                           Quaternion.identity);
            }
        }

        // Play explosion sound
        SoundManager.PlaySfx(soundDieBlow, soundDieBlowVol);
    }
    else
        SoundManager.PlaySfx(soundDie, soundDieVol);  // Sound chết bình thường
}
```

**Override SmartEnemyGrounded.Die()** (`SmartEnemyGrounded.cs:296-330`)

```csharp
public override void Die()
{
    // Dừng nếu đã chết (ngăn double death)
    if (isDead)
        return;

    base.Die();  // Gọi Die() parent

    isDead = true;  // Đánh dấu chết

    CancelInvoke();  // Hủy tất cả scheduled call

    // Disable tất cả collider
    var cols = GetComponents<BoxCollider2D>();
    foreach (var col in cols)
        col.enabled = false;

    // Spawn item drop (nếu có SpawnItemHelper)
    if (spawnItem && spawnItem.spawnWhenDie)
        spawnItem.Spawn();

    // Set animation chết
    AnimSetBool("isDead", true);

    // 50% chance dùng animation chết thay thế
    if (Random.Range(0, 2) == 1)
        AnimSetTrigger("die2");

    // Nếu burn, return sớm (xử lý bởi burn effect)
    if (enemyEffect == ENEMYEFFECT.BURNING)
        return;

    // Nếu explosion hoặc destroy behavior, disable ngay lập tức
    if (enemyEffect == ENEMYEFFECT.EXPLOSION || dieBehavior == DIEBEHAVIOR.DESTROY)
    {
        gameObject.SetActive(false);
        return;
    }

    // Nếu không, đợi animation kết thúc
    StopAllCoroutines();
    StartCoroutine(DisableEnemy(AnimationHelper.getAnimationLength(anim, "Die") + 2f));
}
```

**Coroutine DisableEnemy()** (`SmartEnemyGrounded.cs:382-390`)

```csharp
IEnumerator DisableEnemy(float delay)
{
    // Đợi animation chết kết thúc
    yield return new WaitForSeconds(delay);

    // Spawn disable effect (effect xác biến mất)
    if (disableFX)
        Instantiate(disableFX,
                   spawnDisableFX != null ? spawnDisableFX.position : transform.position,
                   Quaternion.identity);

    // Disable GameObject (trả về pool hoặc destroy)
    gameObject.SetActive(false);
}
```

**Timeline Chết**:
```
Frame 100: currentHealth = 0
           ├─ Die() được gọi
           ├─ isPlaying = false
           ├─ Xóa khỏi GameManager listener
           ├─ SetEnemyState(DEATH)
           ├─ GiveCoin()
           ├─ Spawn death FX
           └─ Play death sound

Frame 101: SmartEnemyGrounded.Die() được gọi
           ├─ isDead = true
           ├─ Disable tất cả collider
           ├─ AnimSetBool("isDead", true)
           ├─ Trigger animation chết
           └─ Bắt đầu coroutine DisableEnemy

Frame 102-220: (2 giây @ 60fps)
           └─ Animation chết play

Frame 221: Coroutine DisableEnemy kết thúc
           ├─ Spawn disableFX (xác biến mất)
           └─ gameObject.SetActive(false)
```

---

## Điều Khiển Animation

### Animation Method

**AnimSetTrigger()** (`Enemy.cs:216-220`)

```csharp
public void AnimSetTrigger(string name)
{
    if (anim)
        anim.SetTrigger(name);  // Trigger animation một lần
}
```

**AnimSetBool()** (`Enemy.cs:222-226`)

```csharp
public void AnimSetBool(string name, bool value)
{
    if (anim)
        anim.SetBool(name, value);  // Set parameter bool liên tục
}
```

**AnimSetFloat()** (`Enemy.cs:228-232`)

```csharp
public void AnimSetFloat(string name, float value)
{
    if (anim)
        anim.SetFloat(name, value);  // Set parameter float
}
```

### Animation Parameter

**Animation Parameter Thường Gặp**:
- `speed` (float): Độ lớn velocity ngang → điều khiển tốc độ animation walk
- `spawn` (trigger): Play animation spawn/đào lên
- `shoot` (trigger): Play animation bắn
- `melee` (trigger): Play animation tấn công melee
- `throw` (trigger): Play animation ném
- `hit` (trigger): Play animation hit/bị đau
- `stun` (trigger): Play animation stun
- `die2` (trigger): Animation chết thay thế
- `isDead` (bool): State chết liên tục
- `isFreezing` (bool): Animation effect freeze
- `isPoisoning` (bool): Animation effect poison

### Method HandleAnimation()

Vị trí trong `SmartEnemyGrounded.cs:265-269`

```csharp
void HandleAnimation()
{
    // Update parameter speed dựa trên velocity
    AnimSetFloat("speed", Mathf.Abs(velocity.x));
}
```

**Cách Hoạt Động**:
```
velocity.x = 0    → speed = 0   → Animation Idle play
velocity.x = 1.5  → speed = 1.5 → Animation Walk play ở 50% tốc độ
velocity.x = 3.0  → speed = 3.0 → Animation Walk play ở 100% tốc độ
velocity.x = -3.0 → speed = 3.0 → Animation Walk play (Abs xóa âm)
```

### Animation Event

**Animation Event** là marker trong Unity animation gọi method script tại frame cụ thể.

**Ví dụ**: Melee Attack Animation

```
Animation Melee (duration 1 giây)
│
├─ Frame 0:     Animation bắt đầu
├─ Frame 15:    Swing bắt đầu
├─ Frame 30:    Event: AnimMeleeAttackStart() ← Kiểm tra hit
├─ Frame 40:    Swing kết thúc
├─ Frame 45:    Event: AnimMeleeAttackEnd() ← End attack
└─ Frame 60:    Animation kết thúc
```

**AnimMeleeAttackStart()** (`SmartEnemyGrounded.cs:276-279`)

```csharp
// Được gọi bởi Animation Event
public void AnimMeleeAttackStart()
{
    meleeAttack.Check4Hit();  // Kiểm tra có hit player
}
```

**AnimMeleeAttackEnd()** (`SmartEnemyGrounded.cs:281-284`)

```csharp
// Được gọi bởi Animation Event
public void AnimMeleeAttackEnd()
{
    meleeAttack.EndCheck4Hit();  // End attack cooldown
}
```

**Animation Event Khác**:

```csharp
// Được gọi bởi throw animation
public void AnimThrow()
{
    throwAttack.Throw(isFacingRight());
}

// Được gọi bởi shoot animation
public void AnimShoot()
{
    rangeAttack.Shoot(isFacingRight());
}
```

---

## Cấu Hình Inspector

### Enemy Base Setting

**Mục Health**:
- `health`: Health tối đa (mặc định 100)
- `healthBarOffset`: Vị trí health bar phía trên enemy (mặc định 0, 1.5)

**Mục Setup**:
- `gravity`: Gia tốc rơi (mặc định 35)
- `walkSpeed`: Tốc độ di chuyển ngang (mặc định 3)

**Mục Behavior**:
- `attackType`: RANGE, MELEE, THROW, hoặc NONE
- `startBehavior`: BURROWUP, WALK_LEFT, WALK_RIGHT, hoặc NONE
- `spawnDelay`: Thời gian trước khi spawn kết thúc (mặc định 1 giây)

**Tùy Chọn Effect**:
- `canBeFreeze`: Có thể bị freeze? (mặc định true)
- `canBeBurn`: Có thể bị burn? (mặc định true)
- `canBePoison`: Có thể bị poison? (mặc định true)
- `canBeShock`: Có thể bị shock? (mặc định true)
- `resistPoisonPercent`: Giảm poison damage theo % (mặc định 10%)
- `poisonSlowSpeed`: Chậm di chuyển trong poison (mặc định 0.3 = 30%)
- `timeShocking`: Shock duration (mặc định 2 giây)

**Sound**:
- `soundHit`: Mảng hit sound effect
- `soundHitVol`: Hit sound volume (0-1)
- `soundDie`: Mảng death sound effect
- `soundDieVol`: Death sound volume (0-1)

### SmartEnemyGrounded Setting

Kế thừa tất cả Enemy setting, cộng thêm:

**Attack Module** (gán qua GetComponent):
- `EnemyRangeAttack`: Module range attack
- `EnemyMeleeAttack`: Module melee attack
- `EnemyThrowAttack`: Module throw attack

**Visual Object**:
- GunObj: Visual gun object (bật nếu RANGE attack)
- MeleeObj: Visual melee weapon (bật nếu MELEE attack)

### UpgradedCharacterParameter

**Nó Là Gì?**

Một **ScriptableObject** lưu upgrade stat enemy cho progression độ khó.

**Field**:
- `UpgradeHealth`: Health max đã upgrade
- `UpgradeMeleeDamage`: Melee damage đã upgrade
- `UpgradeCriticalDamage`: Critical hit chance đã upgrade
- `UpgradeRangeDamage`: Range damage đã upgrade
- `weaponEffect`: Weapon effect để áp dụng
- `maxTargetPerHit`: Bao nhiêu target có thể hit cùng lúc

**Cách Dùng** (`SmartEnemyGrounded.Start()` dòng 68-80):

```csharp
// Override stat với giá trị upgraded
if (upgradedCharacterID != null)
{
    if (meleeAttack)
    {
        meleeAttack.dealDamage = upgradedCharacterID.UpgradeMeleeDamage;
        meleeAttack.criticalPercent = upgradedCharacterID.UpgradeCriticalDamage;
    }
    if (rangeAttack)
    {
        rangeAttack.damage = upgradedCharacterID.UpgradeRangeDamage;
    }
}
```

**Tại Sao Dùng Cái Này?**

Thay vì chỉnh thủ công mỗi enemy prefab, bạn có thể:
1. Tạo difficulty ScriptableObject (Easy, Normal, Hard)
2. Gán vào field `upgradedCharacterID`
3. Enemy prefab giống nhau trở nên mạnh hơn tự động

---

## Cách Tạo Custom Enemy

### Hướng Dẫn Từng Bước

#### Bước 1: Tạo Enemy Prefab

1. Tạo GameObject rỗng: `GameObject → Create Empty`
2. Đặt tên: `Enemy_MyNewEnemy`
3. Thêm Sprite Renderer:
   - `Add Component → Sprite Renderer`
   - Gán sprite enemy của bạn
4. Thêm Animator:
   - `Add Component → Animator`
   - Tạo Animation Controller
5. Thêm Collider:
   - `Add Component → Box Collider 2D`
   - Điều chỉnh size để fit sprite
   - Set làm trigger nếu cần

#### Bước 2: Thêm Enemy Script

1. Thêm component `SmartEnemyGrounded`:
   - `Add Component → SmartEnemyGrounded`
2. Thêm component `Controller2D` (bắt buộc):
   - `Add Component → Controller2D`
   - Tạo collision raycast (xem Controller2D docs)
3. Thêm component `CheckTargetHelper` (bắt buộc):
   - `Add Component → CheckTargetHelper`
   - Set range phát hiện

#### Bước 3: Cấu Hình Enemy Setting

**Health Setting**:
```
health: 150
gravity: 35
walkSpeed: 2.5
```

**Behavior Setting**:
```
attackType: MELEE
startBehavior: WALK_LEFT
spawnDelay: 1
```

**Effect Setting**:
```
canBeFreeze: true
canBeBurn: true
canBePoison: true
canBeShock: true
resistPoisonPercent: 10
poisonSlowSpeed: 0.3
timeShocking: 2
```

#### Bước 4: Thêm Attack Module

**Cho Melee Attack**:

1. Thêm component `EnemyMeleeAttack`
2. Tạo child GameObject: `MeleeCheckPoint`
   - Đặt vị trí phía trước enemy
3. Cấu hình setting:
   ```
   targetLayer: Player
   checkPoint: MeleeCheckPoint transform
   radiusCheck: 1.5
   dealDamage: 25
   criticalPercent: 15
   meleeRate: 1.2
   ```

**Cho Range Attack**:

1. Thêm component `EnemyRangeAttack`
2. Tạo child object:
   - `RangeCheckPoint`: Center phát hiện
   - `FirePoint`: Visual spawn point
   - `ShootingPoint`: Projectile spawn thực tế
3. Cấu hình setting:
   ```
   enemyLayer: Player
   checkPoint: RangeCheckPoint transform
   firePoint: FirePoint transform
   shootingPoint: ShootingPoint transform
   damage: 30
   detectDistance: 8
   bullet: YourProjectilePrefab
   shootingRate: 2
   aimTarget: true
   aimTargetOffset: (0, 0.5)
   ```

**Cho Throw Attack**:

1. Thêm component `EnemyThrowAttack`
2. Tạo child object:
   - `ThrowCheckPoint`: Center phát hiện
   - `ThrowPosition`: Vị trí spawn
3. Cấu hình setting:
   ```
   angleThrow: 60
   throwForceMin: 290
   throwForceMax: 320
   addTorque: 100
   throwRate: 3
   throwPosition: ThrowPosition transform
   _Grenade: YourGrenadePrefab
   targetPlayer: Player layer
   onlyAttackTheFortrest: true
   radiusDetectPlayer: 10
   ```

#### Bước 5: Tạo Animation

**Animation Bắt Buộc**:
- Idle
- Walk
- Attack (melee/shoot/throw)
- Hit
- Die

**Setup Animation Controller**:

```
State:
├─ Idle (default)
├─ Walk
├─ Attack
├─ Hit
├─ Die

Parameter:
├─ speed (float)
├─ melee/shoot/throw (trigger)
├─ hit (trigger)
├─ isDead (bool)
├─ isFreezing (bool)
├─ isPoisoning (bool)

Transition:
├─ Idle → Walk: speed > 0.1
├─ Walk → Idle: speed < 0.1
├─ Any State → Attack: melee/shoot/throw trigger
├─ Any State → Hit: hit trigger
├─ Any State → Die: isDead = true
```

**Thêm Animation Event**:

Cho animation **Melee** attack:
- Frame 30: Event: `AnimMeleeAttackStart`
- Frame 45: Event: `AnimMeleeAttackEnd`

Cho animation **Range** attack:
- Frame 20: Event: `AnimShoot`

Cho animation **Throw** attack:
- Frame 25: Event: `AnimThrow`

#### Bước 6: Thêm Health Bar

Health bar được spawn tự động trong `Enemy.Start()`:

```csharp
var healthBarObj = (HealthBarEnemyNew)Resources.Load("HealthBar", typeof(HealthBarEnemyNew));
healthBar = (HealthBarEnemyNew)Instantiate(healthBarObj, healthBarOffset, Quaternion.identity);
healthBar.Init(transform, (Vector3)healthBarOffset);
```

**Yêu Cầu**:
- Prefab health bar phải ở `Resources/HealthBar`
- Điều chỉnh `healthBarOffset` trong Inspector (mặc định 0, 1.5)

#### Bước 7: Thêm Coin Drop

1. Thêm component `GiveCoinWhenDie`
2. Cấu hình:
   ```
   coinAmount: 10
   coinPrefab: CoinPrefab
   ```

Coin được cho trong `Enemy.Die()`:
```csharp
if (GetComponent<GiveCoinWhenDie>())
{
    GetComponent<GiveCoinWhenDie>().GiveCoin();
}
```

#### Bước 8: Test Enemy

1. Thêm enemy vào scene
2. Play game
3. Kiểm tra:
   - ✓ Đi về fortress
   - ✓ Phát hiện player
   - ✓ Tấn công khi trong range
   - ✓ Nhận damage
   - ✓ Effect hoạt động (freeze, burn, poison, shock)
   - ✓ Chết đúng
   - ✓ Rơi coin

### Ví Dụ: Tạo Flying Enemy

**Vấn đề**: SmartEnemyGrounded chỉ hoạt động trên mặt đất.

**Giải pháp**: Tạo custom flying enemy kế thừa từ class cơ sở Enemy.

```csharp
using UnityEngine;
namespace RGame
{
    public class FlyingEnemy : Enemy
    {
        public float flyHeight = 3f;
        public float flySpeed = 2f;
        private Vector3 targetPosition;

        public override void Start()
        {
            base.Start();

            // Bay về fortress
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

            // Di chuyển về target nếu playing
            if (isPlaying && enemyState == ENEMYSTATE.WALK)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPosition,
                    flySpeed * Time.deltaTime
                );

                // Face hướng di chuyển
                if (targetPosition.x < transform.position.x)
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                else
                    transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        }

        public override void Die()
        {
            base.Die();

            // Thêm fall animation
            StartCoroutine(FallDown());
        }

        IEnumerator FallDown()
        {
            float timer = 0;
            Vector3 startPos = transform.position;

            while (timer < 1f)
            {
                timer += Time.deltaTime;
                transform.position = new Vector3(
                    startPos.x,
                    Mathf.Lerp(startPos.y, 0, timer),
                    startPos.z
                );
                yield return null;
            }

            gameObject.SetActive(false);
        }
    }
}
```

---

## Vấn Đề Thường Gặp & Giải Pháp

### Vấn Đề 1: Enemy Không Di Chuyển

**Triệu chứng**:
- Enemy spawn nhưng đứng yên
- `velocity.x` luôn là 0

**Nguyên Nhân & Sửa**:

1. **State Sai**
   ```csharp
   // Kiểm tra state hiện tại
   Debug.Log("Enemy State: " + enemyState);

   // Sửa: Đảm bảo state là WALK
   SetEnemyState(ENEMYSTATE.WALK);
   ```

2. **isPlaying = false**
   ```csharp
   // Kiểm tra trong Update()
   Debug.Log("isPlaying: " + isPlaying);

   // Sửa: Set trong OnEnable()
   isPlaying = true;
   ```

3. **Frozen Effect**
   ```csharp
   // Kiểm tra effect
   Debug.Log("Effect: " + enemyEffect);

   // Sửa: Xóa effect
   enemyEffect = ENEMYEFFECT.NONE;
   ```

4. **Controller2D Chưa Setup**
   - Kiểm tra Controller2D có raycast được cấu hình
   - Sửa: Thêm horizontal và vertical raycast

### Vấn Đề 2: Enemy Không Tấn Công

**Triệu chứng**:
- Enemy đi đến player nhưng không tấn công
- CheckAttack() không bao giờ kích hoạt

**Nguyên Nhân & Sửa**:

1. **Không Phát Hiện**
   ```csharp
   // Kiểm tra detection
   Debug.Log("isPlayerDetected: " + isPlayerDetected);

   // Sửa: Gọi thủ công
   DetectPlayer(0);
   ```

2. **Attack Module Thiếu**
   ```csharp
   // Kiểm tra trong Start()
   Debug.Log("Range Attack: " + (rangeAttack != null));
   Debug.Log("Melee Attack: " + (meleeAttack != null));

   // Sửa: Thêm component attack phù hợp
   ```

3. **allowCheckAttack = false**
   ```csharp
   // Kiểm tra flag
   Debug.Log("allowCheckAttack: " + allowCheckAttack);

   // Sửa: Đảm bảo không thực hiện action đặc biệt
   allowCheckAttack = true;
   ```

4. **attackType Sai**
   ```csharp
   // Kiểm tra type
   Debug.Log("Attack Type: " + attackType);

   // Sửa: Set type đúng trong Inspector
   attackType = ATTACKTYPE.MELEE;
   ```

### Vấn Đề 3: Health Bar Không Hiện

**Triệu chứng**:
- Enemy spawn nhưng không có health bar

**Nguyên Nhân & Sửa**:

1. **Prefab Thiếu**
   ```csharp
   // Kiểm tra folder Resources
   var healthBarObj = Resources.Load("HealthBar", typeof(HealthBarEnemyNew));
   Debug.Log("Health Bar Prefab: " + (healthBarObj != null));

   // Sửa: Tạo prefab trong Resources/HealthBar
   ```

2. **Offset Sai**
   ```csharp
   // Kiểm tra offset
   Debug.Log("Health Bar Offset: " + healthBarOffset);

   // Sửa: Điều chỉnh trong Inspector
   healthBarOffset = new Vector2(0, 1.5f);
   ```

3. **Canvas Sorting**
   - Health bar có thể ở sau sprite enemy
   - Sửa: Tăng sorting layer của health bar

### Vấn Đề 4: Effect Không Hoạt Động

**Triệu chứng**:
- Freeze/Burn/Poison không áp dụng
- Enemy bỏ qua weapon effect

**Nguyên Nhân & Sửa**:

1. **Effect Bị Disable**
   ```csharp
   // Kiểm tra flag
   Debug.Log("Can Freeze: " + canBeFreeze);
   Debug.Log("Can Burn: " + canBeBurn);
   Debug.Log("Can Poison: " + canBePoison);

   // Sửa: Bật trong Inspector
   canBeFreeze = true;
   ```

2. **Effect Xung Đột**
   ```csharp
   // Kiểm tra effect hiện tại
   Debug.Log("Current Effect: " + enemyEffect);

   // Một số effect hủy effect khác
   // Sửa: Xóa effect trước
   enemyEffect = ENEMYEFFECT.NONE;
   ```

3. **WeaponEffect Chưa Gán**
   ```csharp
   // Trong weapon script, đảm bảo WeaponEffect được truyền
   takeDamage.TakeDamage(damage, force, hitPoint, gameObject, BODYPART.NONE, weaponEffect);
   //                                                                        ^^^^^^^^^^^^^
   ```

### Vấn Đề 5: Enemy Chết Ngay

**Triệu chứng**:
- Enemy spawn và chết ngay lập tức
- Health là 0 tại Start()

**Nguyên Nhân & Sửa**:

1. **Health Chưa Set**
   ```csharp
   // Kiểm tra trong Start()
   Debug.Log("Health: " + health);
   Debug.Log("Current Health: " + currentHealth);

   // Sửa: Set health trong Inspector
   health = 100;
   ```

2. **Upgrade Parameter Sai**
   ```csharp
   // Kiểm tra upgrade
   if (upgradedCharacterID != null)
       Debug.Log("Upgraded Health: " + upgradedCharacterID.UpgradeHealth);

   // Sửa: Set giá trị đúng trong ScriptableObject
   ```

3. **Nhận Damage Khi Spawn**
   - Kiểm tra spawn bên trong vùng gây damage
   - Sửa: Di chuyển spawn point

### Vấn Đề 6: Enemy Đi Xuyên Tường

**Triệu chứng**:
- Enemy bỏ qua collision terrain
- Đi xuyên object solid

**Nguyên Nhân & Sửa**:

1. **Controller2D Không Hoạt Động**
   ```csharp
   // Kiểm tra collision
   Debug.Log("Below: " + controller.collisions.below);
   Debug.Log("Left: " + controller.collisions.left);
   Debug.Log("Right: " + controller.collisions.right);

   // Sửa: Cấu hình raycast trong Controller2D
   ```

2. **LayerMask Sai**
   - Controller2D có setting collision mask
   - Sửa: Set thành terrain layer

3. **Collider Disabled**
   ```csharp
   // Kiểm tra collider
   Debug.Log("Collider Enabled: " + GetComponent<BoxCollider2D>().enabled);

   // Sửa: Bật collider
   ```

### Vấn Đề 7: Animation Không Play

**Triệu chứng**:
- Enemy di chuyển nhưng animation không play
- Kẹt animation idle

**Nguyên Nhân & Sửa**:

1. **Parameter speed Chưa Set**
   ```csharp
   // Kiểm tra trong HandleAnimation()
   Debug.Log("Velocity: " + velocity.x);

   // Sửa: Đảm bảo HandleAnimation() được gọi
   AnimSetFloat("speed", Mathf.Abs(velocity.x));
   ```

2. **Animator Thiếu**
   ```csharp
   // Kiểm tra component
   Debug.Log("Animator: " + (anim != null));

   // Sửa: Thêm Animator component
   ```

3. **Điều Kiện Transition Sai**
   - Kiểm tra transition Animation Controller
   - Sửa: Set giá trị parameter đúng

### Vấn Đề 8: Số Damage Không Hiện

**Triệu chứng**:
- Enemy nhận damage nhưng không có số xuất hiện

**Nguyên Nhân & Sửa**:

1. **FloatingTextManager Thiếu**
   ```csharp
   // Kiểm tra singleton
   Debug.Log("FloatingTextManager: " + (FloatingTextManager.Instance != null));

   // Sửa: Đảm bảo FloatingTextManager tồn tại trong scene
   ```

2. **Canvas Sorting**
   - Floating text có thể ở sau camera
   - Sửa: Set canvas thành "Screen Space - Overlay"

---

## Tổng Kết

**Hệ Thống Enemy** là hệ thống AI tinh vi với:

1. **State Machine**: Điều khiển hành vi enemy (SPAWNING → IDLE → WALK → ATTACK → DEATH)
2. **Hệ Thống Effect**: 5 weapon effect (Freeze, Burn, Poison, Shock, Explosion)
3. **Modular Attack**: 3 loại attack (Melee, Range, Throw) là component riêng biệt
4. **Vật Lý Tùy Chỉnh**: Controller2D cho di chuyển 2D chính xác
5. **Hệ Thống Health**: Damage, health bar, death effect
6. **Observer Pattern**: Lắng nghe thay đổi game state

**Điểm Quan Trọng**:
- Class cơ sở Enemy có thể tái sử dụng (kể cả player dùng!)
- Effect có thể ghi đè lẫn nhau (chỉ một active)
- Attack module là component độc lập
- State machine điều khiển tất cả hành vi
- Hệ thống di chuyển tùy chỉnh (không phải Rigidbody2D)

**Bước Tiếp Theo**:
- Đọc `04_UI_System_Complete.md` để hiểu menu và HUD
- Đọc `05_Managers_Complete.md` để hiểu điều khiển luồng game
- Đọc `10_How_To_Guides.md` cho ví dụ thực tế

---

**Cập nhật lần cuối**: 2025
**File**: `Documents/03_He_Thong_Enemy.md`
