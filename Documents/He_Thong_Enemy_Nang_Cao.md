# Hệ Thống Enemy: Phân Tích Kỹ Thuật Nâng Cao

**Hướng Dẫn Kỹ Thuật Hoàn Chỉnh Về Enemy.cs và Enemy Implementation**

---

## Mục Lục

1. [Tổng Quan](#tổng-quan)
2. [Tại Sao Tài Liệu Này Tồn Tại](#tại-sao-tài-liệu-này-tồn-tại)
3. [Kiến Trúc Enemy.cs Base Class](#kiến-trúc-enemycs-base-class)
4. [State Machine Chi Tiết](#state-machine-chi-tiết)
5. [Triển Khai Hệ Thống Effects](#triển-khai-hệ-thống-effects)
6. [Hệ Thống Máu và Sát Thương](#hệ-thống-máu-và-sát-thương)
7. [Các Chuyên Môn Hóa Enemy](#các-chuyên-môn-hóa-enemy)
8. [Tạo Custom Enemies](#tạo-custom-enemies)
9. [Kỹ Thuật Nâng Cao](#kỹ-thuật-nâng-cao)
10. [Lỗi Thường Gặp và Giải Pháp](#lỗi-thường-gặp-và-giải-pháp)
11. [Tài Liệu Tham Khảo Chéo](#tài-liệu-tham-khảo-chéo)

---

## Tổng Quan

**Mục Đích**: Đây là hướng dẫn kỹ thuật nâng cao dành cho developers muốn hiểu sâu về hệ thống Enemy và tạo custom enemies phức tạp.

**Bạn Sẽ Học Được**:
- Kiến trúc nội bộ của `Enemy.cs` base class
- Cách state machine điều khiển hành vi enemy
- Cách triển khai effects (burning, freeze, poison, shock, explosion)
- Cách kế thừa từ Enemy.cs và tạo hành vi chuyên biệt
- Patterns và kỹ thuật AI nâng cao cho enemy
- Cách debug các vấn đề phức tạp về hành vi enemy

**Kiến Thức Cần Có**:
- Đọc **[03_Enemy_System_Complete.md](03_Enemy_System_Complete.md)** trước
- Kiến thức C# cơ bản (inheritance, enums, coroutines)
- Quen thuộc với Unity Animator system

**Độ Khó**: Nâng cao
**Thời Gian Đọc**: 30-40 phút

---

## Tại Sao Tài Liệu Này Tồn Tại

### Mối Quan Hệ Với Các Tài Liệu Khác

Tài liệu này khác với tài liệu Enemy System toàn diện:

**03_Enemy_System_Complete.md**:
- Giải thích NHỮNG GÌ enemies làm và CÁCH sử dụng chúng
- Thân thiện với người mới bắt đầu với hướng dẫn từng bước
- Bao phủ tất cả các loại enemy từ góc nhìn người dùng

**Enemy-Deep.md (tài liệu này)**:
- Giải thích CÁCH và TẠI SAO hệ thống hoạt động bên trong
- Chi tiết triển khai kỹ thuật nâng cao
- Dành cho developers muốn tạo custom enemy types hoặc sửa đổi core behavior

### Khi Nào Sử Dụng Tài Liệu Này

Sử dụng tài liệu này khi bạn cần:
- Tạo một enemy type hoàn toàn mới (không chỉ là biến thể)
- Hiểu các hệ thống effects dựa trên coroutine
- Debug các vấn đề state machine phức tạp
- Sửa đổi core enemy behavior (hit reactions, death behaviors, v.v.)
- Triển khai weapon effects hoặc damage types mới

---

## Kiến Trúc Enemy.cs Base Class

### Nền Tảng Của Tất Cả Characters

**Insight Quan Trọng**: `Enemy.cs` không chỉ dành cho enemies - nó là base class cho **cả enemies VÀ player**!

```csharp
// Hệ thống thừa kế không theo quy ước:
Enemy.cs (base)
  ├─ SmartEnemyGrounded.cs (walking enemies)
  │   ├─ Goblin
  │   ├─ Skeleton
  │   └─ Troll
  ├─ Player_Archer.cs (đúng vậy, player!)
  ├─ WitchHeal.cs (healer enemy)
  └─ Other specialized enemies...
```

**Tại sao thiết kế này?** Cả players và enemies đều chia sẻ các tính năng chung:
- Hệ thống máu
- Nhận sát thương
- Status effects (burning, freezing, v.v.)
- Animation state machine
- Tích hợp Observer pattern (IListener)

### Core Enums

File `Enemy.cs` định nghĩa **6 enums quan trọng** điều khiển mọi hành vi:

#### 1. ATTACKTYPE
```csharp
public enum ATTACKTYPE
{
    RANGE,   // Bow, gun, magic projectiles
    MELEE,   // Sword, hammer, close combat
    THROW,   // Grenades, bombs
    NONE     // Doesn't attack (passive enemies)
}
```

**Sử dụng**: Xác định attack module nào được dùng.

#### 2. ENEMYSTATE
```csharp
public enum ENEMYSTATE
{
    SPAWNING,  // Playing spawn animation
    IDLE,      // Standing still
    ATTACK,    // Attacking player/fortress
    WALK,      // Moving toward target
    HIT,       // Taking damage (knockback)
    DEATH      // Dead (playing death animation)
}
```

**Sử dụng**: Điều khiển animation triggers và behavior logic.

#### 3. ENEMYEFFECT
```csharp
public enum ENEMYEFFECT
{
    NONE,       // No effect
    BURNING,    // Taking fire damage over time
    FREEZE,     // Frozen in place (can't move)
    SHOKING,    // Taking electric damage
    POISON,     // Poisoned (damage + slow)
    EXPLOSION   // Explodes on death
}
```

**Sử dụng**: Status effects loại trừ lẫn nhau.

#### 4. STARTBEHAVIOR
```csharp
public enum STARTBEHAVIOR
{
    NONE,         // Starts immediately
    BURROWUP,     // Burrows out of ground (spawn animation)
    PARACHUTE,    // Parachutes from sky
    WALK_RIGHT,   // Spawns walking right
    WALK_LEFT     // Spawns walking left
}
```

**Sử dụng**: Điều khiển cách enemy vào game.

#### 5. DIEBEHAVIOR
```csharp
public enum DIEBEHAVIOR
{
    NORMAL,   // Normal death animation
    DESTROY,  // Instantly destroyed (no animation)
    BLOWUP    // Explodes on death (bomber enemy)
}
```

**Sử dụng**: Điều khiển death effects và coin dropping.

#### 6. HITBEHAVIOR
```csharp
public enum HITBEHAVIOR
{
    NONE,         // Takes damage but doesn't react
    CANKNOCKBACK  // Gets knocked back when hit
}
```

**Sử dụng**: Xác định phản ứng vật lý với damage.

### Tổng Quan Cấu Trúc Class

```csharp
public class Enemy : MonoBehaviour, ICanTakeDamage, IListener
{
    // === CONFIGURATION (Inspector) ===
    public UpgradedCharacterParameter upgradedCharacterID;  // Stats from upgrade system
    public float gravity = 35f;
    public float walkSpeed = 3;
    public ATTACKTYPE attackType;
    public STARTBEHAVIOR startBehavior;
    public int health = 100;

    // === RUNTIME STATE ===
    [ReadOnly] public ENEMYSTATE enemyState = ENEMYSTATE.IDLE;
    protected ENEMYEFFECT enemyEffect;
    [ReadOnly] public int currentHealth;
    [ReadOnly] public float multipleSpeed = 1;  // Modified by effects

    // === INTERNAL SYSTEMS ===
    protected HealthBarEnemyNew healthBar;      // UI health bar
    protected Animator anim;                    // Animation controller
    public CheckTargetHelper checkTarget;       // Target detection

    // === CORE METHODS ===
    public virtual void Start() { ... }
    public virtual void Update() { ... }
    public virtual void FixedUpdate() { ... }

    // Damage and effects
    public void TakeDamage(...) { ... }
    public virtual void Hit(...) { ... }
    public virtual void Die() { ... }
    public virtual void Freeze(...) { ... }
    public virtual void Burning(...) { ... }
    public virtual void Poison(...) { ... }
    public virtual void Shoking(...) { ... }

    // State management
    public void SetEnemyState(ENEMYSTATE state) { ... }
    public void SetEnemyEffect(ENEMYEFFECT effect) { ... }

    // Observer pattern (IListener)
    public virtual void IPlay() { ... }
    public virtual void IGameOver() { ... }
    // ... other IListener methods
}
```

---

## State Machine Chi Tiết

### ENEMYSTATE Hoạt Động Như Thế Nào

State machine điều khiển **enemy đang làm gì**. Thay đổi state kích hoạt animations và behavior changes.

### Sơ Đồ Chuyển Đổi State

```
                    ┌──────────────┐
                    │  SPAWNING    │
                    │ (spawn anim) │
                    └──────┬───────┘
                           │
                           v
                    ┌──────────────┐
              ┌─────│     WALK     │────┐
              │     │ (moving)     │    │
              │     └──────┬───────┘    │
              │            │            │
              │            v            │
              │     ┌──────────────┐    │
              │     │     IDLE     │    │
              │     │ (waiting)    │    │
              │     └──────┬───────┘    │
              │            │            │
 [Player detected]         v            │
              │     ┌──────────────┐    │
              └────>│    ATTACK    │    │
                    │ (shooting/   │    │
                    │  melee)      │    │
                    └──────┬───────┘    │
                           │            │
          [Takes damage]   v            │
                    ┌──────────────┐    │
                    │     HIT      │────┘
                    │ (knockback)  │ [Recovers]
                    └──────┬───────┘
                           │
          [Health <= 0]    v
                    ┌──────────────┐
                    │    DEATH     │
                    │ (dead)       │
                    └──────────────┘
```

### Triển Khai SetEnemyState()

```csharp
public void SetEnemyState(ENEMYSTATE state)
{
    enemyState = state;  // Simply sets the state variable
}
```

**Quan Trọng**: Method này **không** tự động kích hoạt animations. Bạn phải gọi `AnimSetTrigger()` riêng:

```csharp
// Correct usage:
SetEnemyState(ENEMYSTATE.ATTACK);
AnimSetTrigger("attack");  // Triggers animation

// Incorrect (won't play animation):
SetEnemyState(ENEMYSTATE.ATTACK);  // State changes but no animation!
```

### Pattern Logic Dựa Trên State

Hầu hết enemy behaviors dùng pattern này:

```csharp
void Update()
{
    // Early exits based on state
    if (enemyState == ENEMYSTATE.DEATH)
        return;  // Dead enemies don't update

    if (enemyState == ENEMYSTATE.SPAWNING)
        return;  // Wait for spawn animation to finish

    if (!isPlaying)
        return;  // Game is paused

    // State-specific behavior
    switch (enemyState)
    {
        case ENEMYSTATE.WALK:
            MoveTowardTarget();
            CheckForPlayer();
            break;

        case ENEMYSTATE.ATTACK:
            FacePlayer();
            if (Time.time > lastAttackTime + attackCooldown)
            {
                PerformAttack();
            }
            break;

        case ENEMYSTATE.IDLE:
            // Do nothing, just play idle animation
            break;

        case ENEMYSTATE.HIT:
            // Handled by Hit() method and knockback coroutine
            break;
    }
}
```

### Animation Trigger Helpers

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

**Tại sao wrap Animator calls?** Safety checks - ngăn null reference errors nếu Animator thiếu.

**Common Animator Parameters**:
```
Triggers: "spawn", "attack", "melee", "shoot", "die", "hit"
Bools: "isFreezing", "isPoisoning", "isWalking"
Floats: "speed", "health"
```

---

## Triển Khai Hệ Thống Effects

### ENEMYEFFECT Hoạt Động Như Thế Nào

Effects là **mutually exclusive** - một enemy chỉ có thể có MỘT effect tại một thời điểm. Áp dụng effect mới sẽ hủy effect cũ.

### Ma Trận Độ Ưu Tiên và Tương Tác Effect

```
         │ Apply Burning │ Apply Freeze │ Apply Poison │ Apply Shock │
─────────┼───────────────┼──────────────┼──────────────┼─────────────┤
Burning  │ No effect     │ Cancel burn  │ Block poison │ Cancel burn │
Freeze   │ Cancel freeze │ No effect    │ Cancel freeze│ Cancel freeze│
Poison   │ Block poison  │ Cancel poison│ No effect    │ Cancel poison│
Shock    │ Cancel shock  │ Cancel shock │ Cancel shock │ No effect   │
```

**Quy Tắc Chính**:
1. **Freeze hủy mọi thứ** (bao gồm cả burn/poison damage đang diễn ra)
2. **Burning chặn poison** (không thể bị poison khi burning)
3. **Effects không stack** (áp dụng burn hai lần không tăng gấp đôi damage)

### Burning Effect - Từng Dòng

```csharp
public virtual void Burning(float damage, GameObject instigator)
{
    // 1. Check if already burning (prevent stacking)
    if (enemyEffect == ENEMYEFFECT.BURNING)
        return;

    // 2. Cancel freeze if frozen
    if (enemyEffect == ENEMYEFFECT.FREEZE)
    {
        UnFreeze();
    }

    // 3. Cancel shock if shocked
    if (enemyEffect == ENEMYEFFECT.SHOKING)
    {
        UnShock();
    }

    // 4. Check if this enemy can be burned
    if (canBeBurn)
    {
        damageBurningPerFrame = damage;  // Store damage amount
        enemyEffect = ENEMYEFFECT.BURNING;  // Set effect state

        StartCoroutine(BurnOutCo(1));  // Start burn timer (1 second)
    }
}
```

**Áp Dụng Burn Damage** (trong Update):
```csharp
public virtual void Update()
{
    if (enemyEffect == ENEMYEFFECT.BURNING)
        CheckDamagePerFrame(damageBurningPerFrame);  // Apply damage every frame

    // ... other update logic ...
}
```

**Burn Timer Coroutine**:
```csharp
IEnumerator BurnOutCo(float time)
{
    if (enemyEffect != ENEMYEFFECT.BURNING)
        yield break;  // Effect was cancelled

    yield return new WaitForSeconds(time);  // Wait 1 second

    // If enemy died while burning, clean up
    if (enemyState == ENEMYSTATE.DEATH)
    {
        BurnOut();
        gameObject.SetActive(false);
    }

    BurnOut();  // End burn effect
}

void BurnOut()
{
    if (enemyEffect != ENEMYEFFECT.BURNING)
        return;

    enemyEffect = ENEMYEFFECT.NONE;  // Clear effect
}
```

**Tính Toán Tổng Damage**:
```
Burn kéo dài 1 giây ở 60 FPS:
Total damage = damageBurningPerFrame * 60 frames
Ví dụ: 0.5 damage/frame * 60 = 30 total damage
```

### Freeze Effect - Phân Tích Chi Tiết

```csharp
public virtual void Freeze(float time, GameObject instigator)
{
    // 1. Already frozen? Do nothing
    if (enemyEffect == ENEMYEFFECT.FREEZE)
        return;

    // 2. Cancel burn if burning
    if (enemyEffect == ENEMYEFFECT.BURNING)
        BurnOut();

    // 3. Cancel shock if shocked
    if (enemyEffect == ENEMYEFFECT.SHOKING)
    {
        UnShock();
    }

    // 4. Check if can be frozen
    if (canBeFreeze)
    {
        enemyEffect = ENEMYEFFECT.FREEZE;
        StartCoroutine(UnFreezeCo(time));  // Freeze for specified duration
    }
}

IEnumerator UnFreezeCo(float time)
{
    AnimSetBool("isFreezing", true);  // Trigger freeze animation

    if (enemyEffect != ENEMYEFFECT.FREEZE)
        yield break;  // Effect was cancelled

    yield return new WaitForSeconds(time);  // Stay frozen
    UnFreeze();
}

void UnFreeze()
{
    if (enemyEffect != ENEMYEFFECT.FREEZE)
        return;

    enemyEffect = ENEMYEFFECT.NONE;
    AnimSetBool("isFreezing", false);  // End freeze animation
}
```

**Cách Freeze Dừng Movement**:

Trong các child classes như `SmartEnemyGrounded.cs`:
```csharp
void FixedUpdate()
{
    // Check freeze status before moving
    if (enemyEffect == ENEMYEFFECT.FREEZE)
    {
        velocity.x = 0;  // Stop horizontal movement
        return;  // Skip rest of update
    }

    // Normal movement code here...
}
```

### Poison Effect - Triển Khai Nâng Cao

Poison là effect phức tạp nhất vì nó:
1. Gây sát thương theo thời gian (mỗi giây, không phải mỗi frame)
2. Làm chậm movement speed
3. Có resistance scaling
4. Hiển thị floating damage text

```csharp
public virtual void Poison(float damage, float time, GameObject instigator)
{
    // 1. Burning blocks poison
    if (enemyEffect == ENEMYEFFECT.BURNING)
        return;

    // 2. Already poisoned? Do nothing
    if (enemyEffect == ENEMYEFFECT.POISON)
        return;

    // 3. Cancel other effects
    if (enemyEffect == ENEMYEFFECT.FREEZE)
    {
        UnFreeze();
    }

    if (enemyEffect == ENEMYEFFECT.SHOKING)
    {
        UnShock();
    }

    // 4. Apply poison
    if (canBePoison)
    {
        damagePoisonPerSecond = damage;
        enemyEffect = ENEMYEFFECT.POISON;

        StartCoroutine(PoisonCo(time));
    }
}
```

**Poison Damage Loop**:
```csharp
IEnumerator PoisonCo(float time)
{
    AnimSetBool("isPoisoning", true);
    multipleSpeed = 1 - poisonSlowSpeed;  // Slow movement (e.g., 30% slower)

    if (enemyEffect != ENEMYEFFECT.POISON)
        yield break;

    int wait = (int)time;  // Convert to seconds

    while (wait > 0)
    {
        yield return new WaitForSeconds(1);  // Wait 1 second

        // Calculate damage with resistance
        int _damage = (int)(damagePoisonPerSecond
            * Random.Range(90 - resistPoisonPercent, 100f - resistPoisonPercent)
            * 0.01f);

        // Apply damage
        currentHealth -= _damage;
        if (healthBar)
            healthBar.UpdateValue(currentHealth / (float)health);

        // Show floating text
        FloatingTextManager.Instance.ShowText(
            "" + (int)_damage,
            healthBarOffset,
            Color.red,
            transform.position
        );

        // Check for death
        if (currentHealth <= 0)
        {
            PoisonEnd();
            Die();
            yield break;
        }

        wait -= 1;  // Countdown
    }

    PoisonEnd();
}

void PoisonEnd()
{
    if (enemyEffect != ENEMYEFFECT.POISON)
        return;

    AnimSetBool("isPoisoning", false);
    multipleSpeed = 1;  // Restore normal speed

    enemyEffect = ENEMYEFFECT.NONE;
}
```

**Công Thức Poison Damage**:
```
Base damage: 10 damage/giây
Resistance: 20% resist
Time: 5 giây

Damage mỗi tick = 10 * Random(70-80) * 0.01
                = 10 * 70-80%
                = 7-8 damage mỗi giây

Total damage = 7-8 * 5 ticks = 35-40 damage
```

### Shocking Effect - Sát Thương Liên Tục

```csharp
public virtual void Shoking(float damage, GameObject instigator)
{
    if (enemyEffect == ENEMYEFFECT.SHOKING)
        return;

    // Cancel freeze and burn
    if (enemyEffect == ENEMYEFFECT.FREEZE)
    {
        UnFreeze();
    }

    if (enemyEffect == ENEMYEFFECT.BURNING)
        BurnOut();

    if (canBeShock)
    {
        damageShockingPerFrame = damage;  // Damage applied every frame
        enemyEffect = ENEMYEFFECT.SHOKING;
        StartCoroutine(UnShockCo());
    }
}

IEnumerator UnShockCo()
{
    if (enemyEffect != ENEMYEFFECT.SHOKING)
        yield break;

    yield return new WaitForSeconds(timeShocking);  // Default 2 seconds

    UnShock();
}

void UnShock()
{
    if (enemyEffect != ENEMYEFFECT.SHOKING)
        return;

    enemyEffect = ENEMYEFFECT.NONE;
}
```

**Áp Dụng Shock Damage** (tương tự burning):
```csharp
public virtual void Update()
{
    if (enemyEffect == ENEMYEFFECT.SHOKING)
        CheckDamagePerFrame(damageShockingPerFrame);

    // ...
}
```

### Explosion Effect

Explosion là duy nhất - nó được set khi **chết**, không phải khi còn sống:

```csharp
public void TakeDamage(...)
{
    currentHealth -= (int)damage;

    if (currentHealth <= 0)
    {
        // Check if should explode
        if (isExplosion || dieBehavior == DIEBEHAVIOR.BLOWUP)
        {
            SetEnemyEffect(ENEMYEFFECT.EXPLOSION);
        }

        Die();
    }
}

public virtual void Die()
{
    // ... standard death logic ...

    // Check for explosion
    if (enemyEffect == ENEMYEFFECT.EXPLOSION)
    {
        // Spawn blood puddles
        if (bloodPuddleFX)
        {
            for (int i = 0; i < Random.Range(2, 5); i++)
            {
                Instantiate(bloodPuddleFX, ...);
            }
        }

        // Spawn explosion effects
        if (explosionFX.Length > 0)
        {
            for (int i = 0; i < Random.Range(1, 3); i++)
            {
                Instantiate(explosionFX[Random.Range(0, explosionFX.Length)], ...);
            }
        }

        SoundManager.PlaySfx(soundDieBlow, soundDieBlowVol);
    }
    else
        SoundManager.PlaySfx(soundDie, soundDieVol);
}
```

---

## Hệ Thống Máu và Sát Thương

### TakeDamage() - Method Chủ Đạo

Mọi nguồn sát thương gọi method này:

```csharp
public void TakeDamage(
    float damage,             // How much damage
    Vector2 force,            // Knockback force
    Vector2 hitPoint,         // Where the hit landed (for FX)
    GameObject instigator,    // Who dealt the damage
    BODYPART bodyPart = BODYPART.NONE,  // (Optional) Critical hits
    WeaponEffect weaponEffect = null     // (Optional) Status effects
)
{
    // 1. Safety checks
    if (enemyState == ENEMYSTATE.DEATH)
        return;  // Can't damage dead enemies

    if (isStopping)
        return;  // Invulnerable during certain states

    // 2. Store parameters for use in child classes
    _bodyPart = bodyPart;
    _bodyPartForce = force;
    _damage = damage;

    // 3. Apply damage
    hitPos = hitPoint;
    currentHealth -= (int)damage;

    // 4. Show damage number
    FloatingTextManager.Instance.ShowText(
        "" + (int)damage,
        healthBarOffset,
        Color.red,
        transform.position
    );

    // 5. Spawn visual effects
    knockBackForce = force;
    if (hitFX)
        Instantiate(hitFX, hitPos + randomOffset, Quaternion.identity);
    if (bloodPuddleFX)
        Instantiate(bloodPuddleFX, position + randomOffset, Quaternion.identity);

    // 6. Update health bar
    if (healthBar)
        healthBar.UpdateValue(currentHealth / (float)health);

    // 7. Check for death
    if (currentHealth <= 0)
    {
        // Check for explosion
        if (isExplosion || dieBehavior == DIEBEHAVIOR.BLOWUP)
        {
            SetEnemyEffect(ENEMYEFFECT.EXPLOSION);
        }

        Die();
    }
    else
    {
        // 8. Apply weapon effects (poison, freeze, etc.)
        if (weaponEffect != null)
        {
            switch (weaponEffect.effectType)
            {
                case WEAPON_EFFECT.POISON:
                    Poison(weaponEffect.poisonDamagePerSec, weaponEffect.poisonTime, instigator);
                    return;  // Skip Hit() call
                case WEAPON_EFFECT.FREEZE:
                    Freeze(weaponEffect.freezeTime, instigator);
                    return;
                case WEAPON_EFFECT.NORMAL:
                    break;
            }
        }

        // 9. Trigger hit reaction
        Hit(force);
    }
}
```

### Hit() - Phản Ứng Vật Lý

```csharp
public virtual void Hit(Vector2 force = default(Vector2), bool pushBack = false, bool knockDownRagdoll = false, bool shock = false)
{
    SoundManager.PlaySfx(soundHit, soundHitVol);

    // Check hit behavior
    switch (hitBehavior)
    {
        case HITBEHAVIOR.CANKNOCKBACK:
            KnockBack(knockBackForce);  // Push enemy back
            break;
        case HITBEHAVIOR.NONE:
            // Takes damage but no physical reaction
            break;
    }
}
```

**Quan Trọng**: `Hit()` là virtual - child classes override nó cho custom reactions:

```csharp
// In SmartEnemyGrounded.cs
public override void Hit(Vector2 force = default(Vector2), bool pushBack = false, bool knockDownRagdoll = false, bool shock = false)
{
    base.Hit(force, pushBack, knockDownRagdoll, shock);  // Call parent

    // Custom logic
    if (enemyState == ENEMYSTATE.ATTACK)
    {
        StopAttack();  // Interrupt attack animation
    }

    SetEnemyState(ENEMYSTATE.HIT);
    AnimSetTrigger("hit");
}
```

### Die() - Chuỗi Death

```csharp
public virtual void Die()
{
    // 1. Stop all gameplay
    isPlaying = false;
    GameManager.Instance.RemoveListener(this);  // Unregister from events
    isPlayerDetected = false;
    SetEnemyState(ENEMYSTATE.DEATH);

    // 2. Award coins
    if (GetComponent<GiveCoinWhenDie>())
    {
        GetComponent<GiveCoinWhenDie>().GiveCoin();
    }

    // 3. Spawn death FX
    if (dieFX)
        Instantiate(dieFX, transform.position, dieFX.transform.rotation);

    // 4. Special effect deaths
    if (enemyEffect == ENEMYEFFECT.FREEZE && dieFrozenFX)
        Instantiate(dieFrozenFX, hitPos, Quaternion.identity);

    if (enemyEffect == ENEMYEFFECT.SHOKING)
        UnShock();

    // 5. Explosion death
    if (enemyEffect == ENEMYEFFECT.EXPLOSION)
    {
        // Spawn blood puddles
        if (bloodPuddleFX)
        {
            for (int i = 0; i < Random.Range(2, 5); i++)
            {
                Instantiate(bloodPuddleFX, randomPosition, Quaternion.identity);
            }
        }

        // Spawn explosions
        if (explosionFX.Length > 0)
        {
            for (int i = 0; i < Random.Range(1, 3); i++)
            {
                Instantiate(explosionFX[Random.Range(0, explosionFX.Length)], transform.position, Quaternion.identity);
            }
        }

        SoundManager.PlaySfx(soundDieBlow, soundDieBlowVol);
    }
    else
        SoundManager.PlaySfx(soundDie, soundDieVol);

    // Note: Actual GameObject destruction is handled in child classes
    // or by animation events calling Destroy(gameObject)
}
```

---

## Các Chuyên Môn Hóa Enemy

### SmartEnemyGrounded.cs - Walking Enemies

Hầu hết ground enemies kế thừa từ class này.

**Tính Năng Chính**:
- Dùng `Controller2D` cho physics-based movement
- Tự động phát hiện và đuổi theo target
- Kiểm tra attack range
- Flip sprite theo hướng

**Pattern Phổ Biến**:
```csharp
public class SmartEnemyGrounded : Enemy
{
    protected Controller2D controller;

    public override void Start()
    {
        base.Start();  // Call Enemy.Start()
        controller = GetComponent<Controller2D>();
    }

    public override void FixedUpdate()
    {
        // Don't move if dead, frozen, or game paused
        if (enemyState == ENEMYSTATE.DEATH ||
            enemyEffect == ENEMYEFFECT.FREEZE ||
            !isPlaying)
        {
            velocity.x = 0;
            return;
        }

        // Move toward target
        if (enemyState == ENEMYSTATE.WALK)
        {
            velocity.x = isFacingRight() ? walkSpeed : -walkSpeed;
            velocity.x *= multipleSpeed;  // Apply poison slow
        }

        // Apply gravity
        velocity.y -= gravity * Time.deltaTime;

        // Move with collision detection
        controller.Move(velocity * Time.deltaTime);

        // Stop if hit wall
        if (controller.collisions.left || controller.collisions.right)
        {
            velocity.x = 0;
            Flip();  // Turn around
        }
    }

    void Flip()
    {
        // Flip sprite by rotating 180 degrees on Y axis
        Vector3 rotation = transform.eulerAngles;
        rotation.y = (rotation.y == 0) ? 180 : 0;
        transform.eulerAngles = rotation;
    }
}
```

### WitchHeal.cs - Healer Enemy

Enemy đặc biệt hồi máu cho các enemies khác thay vì tấn công.

**Tính Năng Chính**:
- Phát hiện đồng minh gần bằng `Physics2D.CircleCastAll()`
- Ưu tiên targets máu thấp
- Có heal cooldown

**Core Logic**:
```csharp
public class WitchHeal : SmartEnemyGrounded
{
    public LayerMask healTargetLayer;  // What can be healed
    public float healRange = 3f;
    public int healAmount = 20;
    public float healCooldown = 3f;

    void Update()
    {
        if (Time.time > lastHealTime + healCooldown)
        {
            TryHealNearbyAlly();
        }
    }

    void TryHealNearbyAlly()
    {
        // Find all enemies in range
        RaycastHit2D[] hits = Physics2D.CircleCastAll(
            transform.position,
            healRange,
            Vector2.zero,
            0,
            healTargetLayer
        );

        // Find most damaged ally
        Enemy mostDamaged = null;
        float lowestHealthPercent = 1f;

        foreach (var hit in hits)
        {
            Enemy ally = hit.collider.GetComponent<Enemy>();
            if (ally != null && ally != this)
            {
                float healthPercent = ally.currentHealth / (float)ally.health;
                if (healthPercent < lowestHealthPercent)
                {
                    lowestHealthPercent = healthPercent;
                    mostDamaged = ally;
                }
            }
        }

        // Heal the most damaged ally
        if (mostDamaged != null)
        {
            mostDamaged.Heal(healAmount);
            PlayHealAnimation();
            lastHealTime = Time.time;
        }
    }
}
```

### Attack Modules

Thay vì đặt attack code trong mỗi enemy, hệ thống dùng **modular attack components**:

**EnemyRangeAttack.cs** - Ranged attacks (bow, gun)
**EnemyMeleeAttack.cs** - Melee attacks (sword, hammer)
**EnemyThrowAttack.cs** - Throwing attacks (grenade, bomb)

**Pattern**:
```csharp
public class EnemyRangeAttack : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public float projectileSpeed = 10f;

    public void Attack(Vector2 direction)
    {
        // Spawn projectile
        GameObject projectile = Instantiate(
            projectilePrefab,
            shootPoint.position,
            Quaternion.identity
        );

        // Set projectile velocity
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.velocity = direction * projectileSpeed;
    }
}

// In SmartEnemyGrounded.cs:
EnemyRangeAttack rangeAttack;

void Start()
{
    rangeAttack = GetComponent<EnemyRangeAttack>();
}

void PerformAttack()
{
    if (attackType == ATTACKTYPE.RANGE && rangeAttack != null)
    {
        Vector2 directionToPlayer = (playerPosition - transform.position).normalized;
        rangeAttack.Attack(directionToPlayer);

        SetEnemyState(ENEMYSTATE.ATTACK);
        AnimSetTrigger("shoot");
    }
}
```

---

## Tạo Custom Enemies

### Workflow: Thêm Enemy Type Mới

**Bước 1: Duplicate Existing Enemy**

Bắt đầu với một enemy tương tự làm template:
- Cho walking enemy → Duplicate Goblin
- Cho flying enemy → Duplicate Bee
- Cho healer → Duplicate WitchHeal

**Bước 2: Sửa Đổi Prefab**

1. Thay đổi sprite và animations
2. Điều chỉnh kích thước collider
3. Cấu hình giá trị Inspector:

```
Enemy Component:
├─ walkSpeed: 3 (slower) or 5 (faster)
├─ health: 50 (weak) or 200 (strong)
├─ attackType: MELEE / RANGE / THROW
├─ startBehavior: WALK_LEFT / BURROWUP / etc.
├─ dieBehavior: NORMAL / BLOWUP
└─ hitBehavior: CANKNOCKBACK / NONE
```

**Bước 3: Test Không Thay Đổi Code**

Chạy game và xác minh:
- Enemy spawn đúng
- Đi về fortress/player
- Tấn công khi trong tầm
- Chết và rơi coins

**Bước 4: Thêm Custom Behavior (Tùy Chọn)**

Nếu cần hành vi duy nhất, tạo script mới:

```csharp
using UnityEngine;
using RGame;

public class CustomBoss : SmartEnemyGrounded
{
    [Header("Boss Settings")]
    public int phase = 1;
    public float enrageHealthPercent = 0.5f;

    public override void Start()
    {
        base.Start();  // LUÔN gọi base.Start()
        Debug.Log("Boss initialized");
    }

    public override void Update()
    {
        base.Update();  // LUÔN gọi base.Update()

        // Custom phase transition
        float healthPercent = currentHealth / (float)health;
        if (healthPercent < enrageHealthPercent && phase == 1)
        {
            EnterPhase2();
        }
    }

    void EnterPhase2()
    {
        phase = 2;
        walkSpeed *= 1.5f;  // Move faster
        multipleSpeed = 1.5f;
        Debug.Log("Boss entered Phase 2!");
    }

    public override void Die()
    {
        // Custom death behavior
        Debug.Log("Boss defeated!");

        // Spawn special loot
        SpawnBossLoot();

        base.Die();  // LUÔN gọi base.Die()
    }

    void SpawnBossLoot()
    {
        // Spawn rare items, etc.
    }
}
```

**Quy Tắc Override Quan Trọng**:
1. **Luôn gọi `base.MethodName()`** trừ khi bạn thay thế hoàn toàn behavior
2. **Đừng override `TakeDamage()`** trừ khi thực sự cần thiết - nó rất phức tạp
3. **Override `Hit()` và `Die()`** cho custom reactions
4. **Đừng sửa `enemyState` hoặc `enemyEffect` trực tiếp** - dùng setter methods

---

## Kỹ Thuật Nâng Cao

### Kỹ Thuật 1: Conditional Immunity

Làm cho một số enemies miễn nhiễm với effects cụ thể:

```csharp
public class FireElemental : SmartEnemyGrounded
{
    public override void Start()
    {
        base.Start();

        // Fire elementals can't be burned
        canBeBurn = false;

        // But they CAN be frozen
        canBeFreeze = true;
    }

    public override void Burning(float damage, GameObject instigator)
    {
        // Heal from fire damage instead!
        Heal((int)damage);

        // Don't call base.Burning()
    }
}
```

### Kỹ Thuật 2: Custom Effect Visual

Thêm custom particle effects cho status effects:

```csharp
public class PoisonEnemy : SmartEnemyGrounded
{
    public GameObject poisonAuraFX;
    GameObject activePoisonFX;

    public override void Poison(float damage, float time, GameObject instigator)
    {
        base.Poison(damage, time, instigator);  // Apply poison normally

        // Spawn poison aura visual
        if (poisonAuraFX != null)
        {
            activePoisonFX = Instantiate(poisonAuraFX, transform);
            activePoisonFX.transform.localPosition = Vector3.zero;
        }
    }

    void PoisonEnd()
    {
        // Destroy aura when poison ends
        if (activePoisonFX != null)
        {
            Destroy(activePoisonFX);
        }
    }
}
```

### Kỹ Thuật 3: Multi-Phase Boss

```csharp
public class MultiphaseBoss : SmartEnemyGrounded
{
    public GameObject[] phaseWeapons;  // Different weapons per phase
    public float[] phaseHealthThresholds = { 0.75f, 0.5f, 0.25f };

    int currentPhase = 0;

    public override void Update()
    {
        base.Update();

        CheckPhaseTransition();
    }

    void CheckPhaseTransition()
    {
        float healthPercent = currentHealth / (float)health;

        for (int i = currentPhase; i < phaseHealthThresholds.Length; i++)
        {
            if (healthPercent < phaseHealthThresholds[i])
            {
                TransitionToPhase(i + 1);
                break;
            }
        }
    }

    void TransitionToPhase(int newPhase)
    {
        currentPhase = newPhase;

        // Disable old weapons
        foreach (var weapon in phaseWeapons)
            weapon.SetActive(false);

        // Enable new weapon
        if (newPhase - 1 < phaseWeapons.Length)
            phaseWeapons[newPhase - 1].SetActive(true);

        // Become invulnerable during transition
        StartCoroutine(PhaseTransitionInvulnerability(2f));
    }

    IEnumerator PhaseTransitionInvulnerability(float duration)
    {
        isStopping = true;  // Invulnerable flag

        SetEnemyState(ENEMYSTATE.IDLE);
        AnimSetTrigger("transform");  // Play transformation animation

        yield return new WaitForSeconds(duration);

        isStopping = false;
        SetEnemyState(ENEMYSTATE.WALK);
    }
}
```

### Kỹ Thuật 4: Summoner Enemy

Spawn minions trong battle:

```csharp
public class Summoner : SmartEnemyGrounded
{
    public GameObject minionPrefab;
    public int maxMinions = 3;
    public float summonCooldown = 10f;

    List<GameObject> activeMinions = new List<GameObject>();
    float lastSummonTime = 0;

    void Update()
    {
        // Clean up dead minions from list
        activeMinions.RemoveAll(m => m == null);

        // Summon new minions if under max
        if (activeMinions.Count < maxMinions && Time.time > lastSummonTime + summonCooldown)
        {
            SummonMinion();
        }
    }

    void SummonMinion()
    {
        SetEnemyState(ENEMYSTATE.IDLE);
        AnimSetTrigger("summon");

        // Spawn after animation delay
        Invoke("SpawnMinion", 0.5f);

        lastSummonTime = Time.time;
    }

    void SpawnMinion()
    {
        Vector3 spawnPos = transform.position + new Vector3(Random.Range(-2f, 2f), 0, 0);
        GameObject minion = Instantiate(minionPrefab, spawnPos, Quaternion.identity);

        activeMinions.Add(minion);

        SetEnemyState(ENEMYSTATE.WALK);
    }

    public override void Die()
    {
        // Kill all minions when summoner dies
        foreach (var minion in activeMinions)
        {
            if (minion != null)
            {
                Enemy enemy = minion.GetComponent<Enemy>();
                if (enemy != null)
                    enemy.Die();
            }
        }

        base.Die();
    }
}
```

---

## Lỗi Thường Gặp và Giải Pháp

### Vấn Đề 1: Enemy Không Spawn

**Triệu Chứng**:
- Enemy không bao giờ xuất hiện trong game
- Không có lỗi trong console

**Nguyên Nhân Phổ Biến**:

**Nguyên Nhân A: Prefab không có trong spawn list**
```csharp
// Check LevelEnemyManager
public LevelSetup[] levelSetup;

// Make sure your enemy prefab is in the wavesSetup array for the level
```

**Nguyên Nhân B: Start behavior blocking**
```csharp
// If using BURROWUP but missing spawn animation:
public override void Start()
{
    base.Start();

    // Force skip spawn if animation missing
    if (startBehavior == STARTBEHAVIOR.BURROWUP && anim == null)
    {
        startBehavior = STARTBEHAVIOR.NONE;
        SetEnemyState(ENEMYSTATE.WALK);
    }
}
```

**Nguyên Nhân C: Enemy spawning ngoài camera**
```csharp
// Check spawn position in LevelEnemyManager
Debug.DrawLine(spawnPoint.position, spawnPoint.position + Vector3.up * 5, Color.red, 5f);
```

---

### Vấn Đề 2: Animation Không Khớp State

**Triệu Chứng**:
- Enemy đi nhưng animation hiển thị idle
- Attack animation không chạy

**Nguyên Nhân**: Tên Animator parameter không khớp code

**Giải Pháp**: Kiểm tra Animator Controller

```
Animator Parameters (phải khớp chính xác):
- "spawn" (trigger)
- "die" (trigger)
- "hit" (trigger)
- "attack" (trigger for melee)
- "shoot" (trigger for range)
- "isWalking" (bool)
- "isFreezing" (bool)
- "isPoisoning" (bool)
```

**Debug Code**:
```csharp
public override void Update()
{
    base.Update();

    // Debug current state
    Debug.Log($"{gameObject.name}: State={enemyState}, Effect={enemyEffect}");

    // Force animation sync
    if (anim != null)
    {
        anim.SetBool("isWalking", enemyState == ENEMYSTATE.WALK);
    }
}
```

---

### Vấn Đề 3: Effect Không Áp Dụng

**Triệu Chứng**:
- WeaponEffect áp dụng nhưng không có visual effect
- Enemy không frozen/burning/poisoned

**Các Bước Debug**:

**Bước 1: Kiểm tra flags**
```csharp
void Start()
{
    Debug.Log($"{gameObject.name} - canBeFreeze: {canBeFreeze}, canBeBurn: {canBeBurn}, canBePoison: {canBePoison}");
}
```

**Bước 2: Kiểm tra WeaponEffect setup**
```csharp
// In weapon projectile script:
void OnTriggerEnter2D(Collider2D other)
{
    ICanTakeDamage target = other.GetComponent<ICanTakeDamage>();
    if (target != null)
    {
        Debug.Log($"Applying weapon effect: {weaponEffect.effectType}");
        target.TakeDamage(damage, force, hitPoint, gameObject, BODYPART.NONE, weaponEffect);
    }
}
```

**Bước 3: Override effect method để debug**
```csharp
public override void Freeze(float time, GameObject instigator)
{
    Debug.Log($"Freeze called: canBeFreeze={canBeFreeze}, currentEffect={enemyEffect}");
    base.Freeze(time, instigator);
    Debug.Log($"After freeze: enemyEffect={enemyEffect}");
}
```

---

### Vấn Đề 4: Enemy Stuck ở SPAWNING State

**Triệu Chứng**:
- Enemy xuất hiện nhưng không di chuyển
- `enemyState` stuck ở SPAWNING

**Nguyên Nhân**: `FinishSpawning()` không bao giờ được gọi

**Giải Pháp**:

```csharp
public override void Start()
{
    base.Start();

    // Add safety timeout
    if (startBehavior == STARTBEHAVIOR.BURROWUP)
    {
        Invoke("ForceFinishSpawning", spawnDelay + 1f);  // +1s safety margin
    }
}

void ForceFinishSpawning()
{
    if (enemyState == ENEMYSTATE.SPAWNING)
    {
        Debug.LogWarning($"{gameObject.name} forced out of SPAWNING state");
        SetEnemyState(ENEMYSTATE.WALK);
    }
}
```

---

### Vấn Đề 5: Health Bar Không Update

**Triệu Chứng**:
- Enemy nhận damage nhưng health bar vẫn đầy

**Nguyên Nhân**: HealthBar không được initialized

**Giải Pháp**:

```csharp
public override void Start()
{
    base.Start();

    // Verify health bar exists
    if (healthBar == null)
    {
        Debug.LogError($"{gameObject.name}: HealthBar failed to load!");

        // Try manual load
        var healthBarObj = Resources.Load<HealthBarEnemyNew>("HealthBar");
        if (healthBarObj != null)
        {
            healthBar = Instantiate(healthBarObj);
            healthBar.Init(transform, healthBarOffset);
        }
    }
}

public override void TakeDamage(...)
{
    // ... damage logic ...

    if (healthBar != null)
    {
        healthBar.UpdateValue(currentHealth / (float)health);
    }
    else
    {
        Debug.LogWarning($"{gameObject.name}: Can't update health bar - null reference");
    }
}
```

---

### Vấn Đề 6: Poison Không Làm Chậm Enemy

**Triệu Chứng**:
- Enemy poisoned (damage ticks hiển thị)
- Movement speed không thay đổi

**Nguyên Nhân**: `multipleSpeed` không được áp dụng vào velocity

**Giải Pháp**:

```csharp
// In SmartEnemyGrounded.FixedUpdate():
void FixedUpdate()
{
    if (enemyState == ENEMYSTATE.WALK)
    {
        velocity.x = isFacingRight() ? walkSpeed : -walkSpeed;

        // QUAN TRỌNG: Áp dụng multipleSpeed modifier
        velocity.x *= multipleSpeed;  // ← Thêm dòng này
    }

    controller.Move(velocity * Time.deltaTime);
}
```

---

### Vấn Đề 7: Enemy Tấn Công Khi Frozen

**Triệu Chứng**:
- Frozen enemy vẫn bắn projectiles
- Attack animation chạy trong freeze

**Nguyên Nhân**: Attack logic không kiểm tra `enemyEffect`

**Giải Pháp**:

```csharp
void PerformAttack()
{
    // Check if frozen
    if (enemyEffect == ENEMYEFFECT.FREEZE)
        return;

    // Check if dead
    if (enemyState == ENEMYSTATE.DEATH)
        return;

    // Proceed with attack
    SetEnemyState(ENEMYSTATE.ATTACK);
    AnimSetTrigger("shoot");
    rangeAttack.Attack(directionToPlayer);
}
```

---

## Tài Liệu Tham Khảo Chéo

**Tài Liệu Liên Quan**:

- **[03_Enemy_System_Complete.md](03_Enemy_System_Complete.md)** - Hướng dẫn enemy toàn diện cho người mới
- **[02_Player_System_Complete.md](02_Player_System_Complete.md)** - Player kế thừa từ Enemy.cs
- **[01_Project_Architecture.md](01_Project_Architecture.md)** - Kiến trúc hệ thống tổng thể
- **[10_How_To_Guides.md](10_How_To_Guides.md)** - Hướng dẫn từng bước
- **[11_Troubleshooting.md](11_Troubleshooting.md)** - Hướng dẫn debug chung
- **[12_Visual_Reference.md](12_Visual_Reference.md)** - Sơ đồ state machine
- **[13_Code_Examples.md](13_Code_Examples.md)** - Patterns và ví dụ code

**Scripts Chính**:
- `Enemy.cs` (`Assets/_MonstersOut/Scripts/AI/Enemy.cs`)
- `SmartEnemyGrounded.cs` - Base cho walking enemies
- `WitchHeal.cs` - Ví dụ healer enemy
- `LevelEnemyManager.cs` - Hệ thống enemy spawning
- `WeaponEffect.cs` - Cấu hình status effect
- `Controller2D.cs` - Movement physics

**Unity Concepts**:
- Coroutines (cho timed effects)
- State machines (ENEMYSTATE enum)
- Inheritance và virtual methods
- Interfaces (ICanTakeDamage, IListener)
- Animator triggers và bools

---

## Tóm Tắt

**Bạn Đã Học Được**:
- ✅ Kiến trúc nội bộ của Enemy.cs base class
- ✅ Cách state machine (ENEMYSTATE) điều khiển behavior
- ✅ Chi tiết triển khai của tất cả effects (burn, freeze, poison, shock, explosion)
- ✅ Cách damage flow qua TakeDamage() → Hit() → Die()
- ✅ Cách tạo custom enemies bằng kế thừa từ Enemy.cs
- ✅ Kỹ thuật nâng cao (phases, summoners, immunity)
- ✅ Cách debug các vấn đề phức tạp về enemy behavior

**Insights Quan Trọng**:
1. **Enemy.cs được chia sẻ bởi enemies VÀ player** - thiết kế cẩn thận
2. **Effects là mutually exclusive** - chỉ một active tại một thời điểm
3. **Luôn gọi `base.MethodName()`** khi override
4. **State changes không kích hoạt animations** - gọi AnimSetTrigger() riêng
5. **`multipleSpeed` ảnh hưởng movement** - được sửa đổi bởi poison effect
6. **TakeDamage() là master damage method** - đừng override trừ khi cần thiết

**Patterns Nâng Cao**:
- Dùng **modular attack components** thay vì monolithic enemy classes
- Override **Hit()** và **Die()** cho custom reactions
- Dùng **coroutines** cho timed behaviors (effects, phase transitions)
- Dùng **isStopping** flag cho invulnerability periods
- Kiểm tra **enemyEffect** trước khi cho phép actions (attacks, movement)

**Bước Tiếp Theo**:
- Tạo custom enemy đầu tiên của bạn dùng workflow trong tài liệu này
- Thử nghiệm với effect combinations và immunities
- Xây dựng multi-phase boss enemy
- Đọc source code SmartEnemyGrounded.cs cho movement implementation

---

**Phiên Bản Tài Liệu**: 1.0
**Cập Nhật Lần Cuối**: 2025-10-29
**Transformation**: Vietnamese (26 lines) → English (2,400+ lines)
**Bổ Sung**: 03_Enemy_System_Complete.md
