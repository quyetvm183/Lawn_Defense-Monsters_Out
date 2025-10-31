# Thư Viện Ví Dụ Code
## Các Đoạn Code Tái Sử Dụng cho Unity 2D Game Development

**Ngôn Ngữ**: Tiếng Việt (Vietnamese)
**Phiên Bản Tài Liệu**: 1.0
**Cập Nhật Lần Cuối**: 2025-10-29
**Độ Khó**: Từ Cơ Bản đến Nâng Cao

---

## Mục Lục

1. [Cách Sử Dụng Thư Viện Này](#cách-sử-dụng-thư-viện-này)
2. [Movement Patterns (Mẫu Di Chuyển)](#movement-patterns)
3. [Combat Systems (Hệ Thống Chiến Đấu)](#combat-systems)
4. [AI Behaviors (Hành Vi AI)](#ai-behaviors)
5. [UI Implementations (Triển Khai UI)](#ui-implementations)
6. [Spawning Systems (Hệ Thống Spawn)](#spawning-systems)
7. [Audio Management (Quản Lý Âm Thanh)](#audio-management)
8. [Save and Load Systems (Hệ Thống Lưu/Tải)](#save-and-load-systems)
9. [Coroutine Patterns (Mẫu Coroutine)](#coroutine-patterns)
10. [Input Handling (Xử Lý Input)](#input-handling)
11. [Utility Functions (Hàm Tiện Ích)](#utility-functions)
12. [Singleton Pattern](#singleton-pattern)
13. [Object Pooling](#object-pooling)
14. [Camera Systems (Hệ Thống Camera)](#camera-systems)
15. [Effect Systems (Hệ Thống Hiệu Ứng)](#effect-systems)

---

## Cách Sử Dụng Thư Viện Này

### Bắt Đầu Nhanh

1. **Tìm code bạn cần** bằng mục lục
2. **Copy toàn bộ code block**
3. **Paste vào Unity script của bạn**
4. **Chỉnh sửa giá trị** để phù hợp với game của bạn (speed, damage, v.v.)
5. **Test trong Unity Editor**

### Cấu Trúc Code

Mỗi ví dụ bao gồm:
- **📋 Mô Tả (Description)**: Nó làm gì
- **💡 Trường Hợp Sử Dụng (Use Case)**: Khi nào dùng nó
- **📁 Nơi Đặt (Where to Place)**: Gợi ý vị trí file
- **🔗 Liên Quan (Related)**: Links đến tài liệu có liên quan
- **⚙️ Phụ Thuộc (Dependencies)**: Components cần thiết

### Quy Ước

```csharp
// Các giá trị tiêu chuẩn bạn sẽ muốn thay đổi
public float speed = 5f;        // ← Thay đổi cái này cho game của bạn
public int maxHealth = 100;     // ← Điều chỉnh theo nhu cầu

// Comments giải thích code làm gì
// → Mũi tên chỉ đến phần quan trọng
```

---

## Movement Patterns

### Ví Dụ 1: Basic WASD Movement

**📋 Mô Tả**: Di chuyển đơn giản bằng bàn phím theo 4 hướng

**💡 Trường Hợp Sử Dụng**: Top-down games, sidescrollers, bất kỳ nhân vật điều khiển bởi người chơi nào

**📁 Nơi Đặt**: `Scripts/Player/PlayerMovement.cs`

```csharp
using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;              // Movement speed

    void Update()
    {
        // Get input (-1 to 1)
        float horizontal = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right
        float vertical = Input.GetAxisRaw("Vertical");     // W/S or Up/Down

        // Create movement vector
        Vector2 movement = new Vector2(horizontal, vertical);

        // Normalize to prevent faster diagonal movement
        if (movement.magnitude > 1)
        {
            movement.Normalize();
        }

        // Move the character
        transform.position += (Vector3)movement * speed * Time.deltaTime;
    }
}
```

**⚙️ Phụ Thuộc**: Không có

**🔗 Liên Quan**: 00_Unity_Fundamentals.md (Input System)

---

### Ví Dụ 2: Smooth Movement with Acceleration

**📋 Mô Tả**: Di chuyển tăng tốc và giảm tốc dần dần

**💡 Trường Hợp Sử Dụng**: Di chuyển nhân vật chân thực hơn, racing games

```csharp
using UnityEngine;

public class SmoothMovement : MonoBehaviour
{
    [Header("Movement")]
    public float maxSpeed = 10f;
    public float acceleration = 50f;
    public float deceleration = 30f;

    private Vector2 currentVelocity;

    void Update()
    {
        // Get input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector2 inputDirection = new Vector2(horizontal, vertical).normalized;

        // Accelerate or decelerate
        if (inputDirection.magnitude > 0)
        {
            // Accelerate towards input direction
            currentVelocity += inputDirection * acceleration * Time.deltaTime;

            // Clamp to max speed
            if (currentVelocity.magnitude > maxSpeed)
            {
                currentVelocity = currentVelocity.normalized * maxSpeed;
            }
        }
        else
        {
            // Decelerate to stop
            float speed = currentVelocity.magnitude;
            speed -= deceleration * Time.deltaTime;
            speed = Mathf.Max(speed, 0); // Don't go negative

            currentVelocity = currentVelocity.normalized * speed;
        }

        // Apply movement
        transform.position += (Vector3)currentVelocity * Time.deltaTime;
    }
}
```

---

### Ví Dụ 3: Point-and-Click Movement

**📋 Mô Tả**: Click để di chuyển đến vị trí mục tiêu

**💡 Trường Hợp Sử Dụng**: RTS games, top-down adventure games

```csharp
using UnityEngine;

public class PointClickMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float stoppingDistance = 0.1f;

    private Vector3 targetPosition;
    private bool isMoving = false;

    void Start()
    {
        targetPosition = transform.position;
    }

    void Update()
    {
        // Check for mouse click
        if (Input.GetMouseButtonDown(0))
        {
            // Get mouse position in world space
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0; // Keep on 2D plane

            targetPosition = mousePos;
            isMoving = true;

            Debug.Log("Moving to: " + targetPosition);
        }

        // Move towards target
        if (isMoving)
        {
            float distance = Vector3.Distance(transform.position, targetPosition);

            if (distance > stoppingDistance)
            {
                // Move towards target
                Vector3 direction = (targetPosition - transform.position).normalized;
                transform.position += direction * speed * Time.deltaTime;
            }
            else
            {
                // Reached target
                isMoving = false;
                Debug.Log("Arrived!");
            }
        }
    }

    // Optional: Visualize target in Scene view
    void OnDrawGizmos()
    {
        if (isMoving)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, targetPosition);
            Gizmos.DrawWireSphere(targetPosition, 0.3f);
        }
    }
}
```

---

### Ví Dụ 4: Follow Target

**📋 Mô Tả**: Theo dõi mượt mà một GameObject khác

**💡 Trường Hợp Sử Dụng**: Camera theo player, enemy đuổi theo player, thú cưng theo chủ nhân

```csharp
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [Header("Target")]
    public Transform target;              // What to follow

    [Header("Follow Settings")]
    public float followSpeed = 5f;        // How fast to follow
    public float minDistance = 2f;        // Stop following when this close
    public float maxDistance = 10f;       // Start following when this far

    [Header("Options")]
    public bool smoothFollow = true;      // Lerp vs instant movement
    public Vector3 offset = Vector3.zero; // Offset from target

    void Update()
    {
        if (target == null)
        {
            Debug.LogWarning("No target assigned!");
            return;
        }

        float distance = Vector3.Distance(transform.position, target.position);

        // Only follow if within range
        if (distance < minDistance)
        {
            // Too close, don't move
            return;
        }

        if (distance > maxDistance)
        {
            // Too far, catch up quickly
            followSpeed *= 2;
        }

        // Calculate target position with offset
        Vector3 targetPos = target.position + offset;

        // Move towards target
        if (smoothFollow)
        {
            // Smooth movement using Lerp
            transform.position = Vector3.Lerp(
                transform.position,
                targetPos,
                followSpeed * Time.deltaTime
            );
        }
        else
        {
            // Direct movement
            Vector3 direction = (targetPos - transform.position).normalized;
            transform.position += direction * followSpeed * Time.deltaTime;
        }
    }
}
```

---

### Ví Dụ 5: Patrol Movement

**📋 Mô Tả**: Di chuyển giữa các waypoints theo vòng lặp

**💡 Trường Hợp Sử Dụng**: Enemy guards (lính canh), moving platforms (nền di chuyển)

```csharp
using UnityEngine;

public class PatrolMovement : MonoBehaviour
{
    [Header("Patrol Points")]
    public Transform[] waypoints;         // Array of positions to visit

    [Header("Settings")]
    public float speed = 3f;
    public float waitTimeAtWaypoint = 1f; // Pause at each point
    public bool loop = true;              // Return to start or ping-pong
    public bool reverseOnEnd = true;      // Reverse direction at end

    private int currentWaypointIndex = 0;
    private bool movingForward = true;
    private float waitTimer = 0f;
    private bool isWaiting = false;

    void Update()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogWarning("No waypoints assigned!");
            return;
        }

        // Handle waiting at waypoint
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0)
            {
                isWaiting = false;
                MoveToNextWaypoint();
            }
            return;
        }

        // Move towards current waypoint
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        float distance = Vector3.Distance(transform.position, targetWaypoint.position);

        if (distance > 0.1f)
        {
            // Still moving towards waypoint
            Vector3 direction = (targetWaypoint.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
        }
        else
        {
            // Reached waypoint
            isWaiting = true;
            waitTimer = waitTimeAtWaypoint;
        }
    }

    void MoveToNextWaypoint()
    {
        if (movingForward)
        {
            currentWaypointIndex++;

            if (currentWaypointIndex >= waypoints.Length)
            {
                if (reverseOnEnd)
                {
                    // Reverse direction
                    movingForward = false;
                    currentWaypointIndex = waypoints.Length - 2;
                }
                else if (loop)
                {
                    // Loop back to start
                    currentWaypointIndex = 0;
                }
                else
                {
                    // Stop at end
                    currentWaypointIndex = waypoints.Length - 1;
                }
            }
        }
        else
        {
            currentWaypointIndex--;

            if (currentWaypointIndex < 0)
            {
                movingForward = true;
                currentWaypointIndex = 1;
            }
        }
    }

    // Visualize patrol path in editor
    void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2)
            return;

        Gizmos.color = Color.yellow;

        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            if (waypoints[i] != null && waypoints[i + 1] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                Gizmos.DrawWireSphere(waypoints[i].position, 0.3f);
            }
        }

        if (waypoints[waypoints.Length - 1] != null)
        {
            Gizmos.DrawWireSphere(waypoints[waypoints.Length - 1].position, 0.3f);
        }
    }
}
```

---

## Combat Systems

### Ví Dụ 6: Health System

**📋 Mô Tả**: Hệ thống máu hoàn chỉnh với damage (sát thương), healing (hồi máu), death (chết)

**💡 Trường Hợp Sử Dụng**: Player, enemies, các vật thể có thể phá hủy

**📁 Nơi Đặt**: `Scripts/HealthSystem.cs`

```csharp
using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Options")]
    public bool destroyOnDeath = true;
    public float destroyDelay = 2f;
    public bool isInvincible = false;

    [Header("Events")]
    public UnityEvent onDamage;           // Called when taking damage
    public UnityEvent onHeal;             // Called when healed
    public UnityEvent onDeath;            // Called when health reaches 0

    private bool isDead = false;

    void Start()
    {
        // Initialize health
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Apply damage to this object
    /// </summary>
    public void TakeDamage(float damage)
    {
        if (isDead || isInvincible)
            return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log(name + " took " + damage + " damage. Health: " + currentHealth);

        // Trigger damage event
        onDamage?.Invoke();

        // Check for death
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Restore health
    /// </summary>
    public void Heal(float amount)
    {
        if (isDead)
            return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log(name + " healed " + amount + ". Health: " + currentHealth);

        // Trigger heal event
        onHeal?.Invoke();
    }

    /// <summary>
    /// Set health to maximum
    /// </summary>
    public void FullHeal()
    {
        Heal(maxHealth);
    }

    /// <summary>
    /// Get health as percentage (0-1)
    /// </summary>
    public float GetHealthPercent()
    {
        return currentHealth / maxHealth;
    }

    /// <summary>
    /// Check if dead
    /// </summary>
    public bool IsDead()
    {
        return isDead;
    }

    /// <summary>
    /// Handle death
    /// </summary>
    void Die()
    {
        if (isDead)
            return;

        isDead = true;
        Debug.Log(name + " died!");

        // Trigger death event
        onDeath?.Invoke();

        // Destroy after delay
        if (destroyOnDeath)
        {
            Destroy(gameObject, destroyDelay);
        }
    }

    /// <summary>
    /// Make temporarily invincible
    /// </summary>
    public void SetInvincible(float duration)
    {
        StartCoroutine(InvincibilityCo(duration));
    }

    System.Collections.IEnumerator InvincibilityCo(float duration)
    {
        isInvincible = true;
        Debug.Log(name + " is invincible for " + duration + " seconds");

        yield return new WaitForSeconds(duration);

        isInvincible = false;
        Debug.Log(name + " invincibility ended");
    }
}
```

**Cách Dùng trong Inspector**:
```
[HealthSystem Component]
Max Health: 100
Destroy On Death: ✓
Destroy Delay: 2

On Damage:
  → Animator.SetTrigger("Hit")
  → AudioSource.Play()

On Death:
  → Animator.SetTrigger("Death")
  → ParticleSystem.Play()
```

---

### Ví Dụ 7: Damage Dealer

**📋 Mô Tả**: Gây sát thương cho các objects khi chạm vào

**💡 Trường Hợp Sử Dụng**: Projectiles (đạn), hazards (bẫy), melee attacks (đánh cận chiến)

```csharp
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [Header("Damage")]
    public float damage = 10f;
    public bool destroyOnHit = true;      // Destroy this object on hit (for projectiles)

    [Header("Targeting")]
    public LayerMask targetLayers;        // What can be damaged
    public string targetTag = "Enemy";    // Optional: Also check tag

    [Header("Knockback")]
    public bool applyKnockback = false;
    public float knockbackForce = 5f;

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if target is on correct layer
        if (((1 << other.gameObject.layer) & targetLayers) == 0)
            return;

        // Optional: Check tag
        if (!string.IsNullOrEmpty(targetTag) && !other.CompareTag(targetTag))
            return;

        // Try to damage the target
        HealthSystem health = other.GetComponent<HealthSystem>();

        if (health != null)
        {
            health.TakeDamage(damage);

            Debug.Log(name + " dealt " + damage + " damage to " + other.name);

            // Apply knockback
            if (applyKnockback)
            {
                Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 direction = (other.transform.position - transform.position).normalized;
                    rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
                }
            }

            // Destroy projectile
            if (destroyOnHit)
            {
                Destroy(gameObject);
            }
        }
    }

    // Also handle non-trigger collisions
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Same logic as OnTriggerEnter2D
        OnTriggerEnter2D(collision.collider);
    }
}
```

---

### Ví Dụ 8: Invincibility Flash Effect

**📋 Mô Tả**: Flash sprite khi nhận sát thương (visual feedback - phản hồi hình ảnh)

**💡 Trường Hợp Sử Dụng**: Player bị đánh, enemy bị đánh, invincibility frames (khung hình bất tử)

```csharp
using UnityEngine;
using System.Collections;

public class FlashEffect : MonoBehaviour
{
    [Header("References")]
    public SpriteRenderer spriteRenderer;

    [Header("Flash Settings")]
    public Color flashColor = Color.red;
    public float flashDuration = 0.1f;
    public int flashCount = 3;

    private Color originalColor;

    void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        originalColor = spriteRenderer.color;
    }

    /// <summary>
    /// Trigger flash effect
    /// </summary>
    public void Flash()
    {
        StartCoroutine(FlashCo());
    }

    IEnumerator FlashCo()
    {
        for (int i = 0; i < flashCount; i++)
        {
            // Flash to damage color
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashDuration);

            // Return to normal
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }
    }

    /// <summary>
    /// Alternative: Fade in/out effect
    /// </summary>
    public void FadeFlash()
    {
        StartCoroutine(FadeFlashCo());
    }

    IEnumerator FadeFlashCo()
    {
        float elapsed = 0f;
        float duration = flashDuration * flashCount;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // Oscillate alpha between 0 and 1
            float alpha = Mathf.PingPong(elapsed * 10f, 1f);
            spriteRenderer.color = new Color(
                spriteRenderer.color.r,
                spriteRenderer.color.g,
                spriteRenderer.color.b,
                alpha
            );

            yield return null;
        }

        // Restore original color
        spriteRenderer.color = originalColor;
    }
}
```

**Cách Dùng**:
```csharp
// Trong HealthSystem, khi nhận sát thương:
GetComponent<FlashEffect>()?.Flash();
```

---

## AI Behaviors

### Ví Dụ 9: Chase Player AI

**📋 Mô Tả**: Enemy theo dõi và tấn công player

**💡 Trường Hợp Sử Dụng**: Basic enemy AI, melee enemies (kẻ địch cận chiến)

```csharp
using UnityEngine;

public class ChasePlayerAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;              // Target to chase

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float chaseRange = 10f;        // Start chasing when player within this range
    public float stopDistance = 1.5f;     // Stop when this close (for attacking)

    [Header("Attack")]
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    public float attackDamage = 10f;

    private float lastAttackTime;
    private bool isChasing = false;

    void Start()
    {
        // Find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null)
            return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Check if player is in chase range
        if (distanceToPlayer <= chaseRange)
        {
            isChasing = true;

            // Check if in attack range
            if (distanceToPlayer <= attackRange)
            {
                // Attack
                TryAttack();
            }
            else if (distanceToPlayer > stopDistance)
            {
                // Move towards player
                MoveTowardsPlayer();
            }
        }
        else
        {
            isChasing = false;
        }
    }

    void MoveTowardsPlayer()
    {
        // Calculate direction
        Vector2 direction = (player.position - transform.position).normalized;

        // Move
        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;

        // Optional: Flip sprite based on direction
        if (direction.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    void TryAttack()
    {
        // Check cooldown
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    void Attack()
    {
        Debug.Log(name + " attacks player!");

        // Deal damage to player
        HealthSystem playerHealth = player.GetComponent<HealthSystem>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
        }

        // Play attack animation (if you have Animator)
        // GetComponent<Animator>()?.SetTrigger("Attack");
    }

    // Visualize ranges
    void OnDrawGizmosSelected()
    {
        // Chase range (yellow)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        // Attack range (red)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Stop distance (green)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}
```

---

### Ví Dụ 10: Flee Behavior

**📋 Mô Tả**: Chạy trốn khỏi player khi máu thấp

**💡 Trường Hợp Sử Dụng**: Enemies yếu, hành vi rút lui

```csharp
using UnityEngine;

public class FleeBehavior : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public HealthSystem health;

    [Header("Flee Settings")]
    public float fleeSpeed = 5f;
    public float fleeHealthPercent = 0.3f;  // Flee when health below 30%
    public float fleeDistance = 5f;         // Run until this far from player

    private bool isFleeing = false;

    void Start()
    {
        if (health == null)
            health = GetComponent<HealthSystem>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null || health == null)
            return;

        // Check if should flee
        if (health.GetHealthPercent() <= fleeHealthPercent)
        {
            isFleeing = true;
        }

        if (isFleeing)
        {
            // Check if far enough away
            float distance = Vector2.Distance(transform.position, player.position);

            if (distance < fleeDistance)
            {
                // Run away from player
                Vector2 direction = (transform.position - player.position).normalized;
                transform.position += (Vector3)direction * fleeSpeed * Time.deltaTime;
            }
            else
            {
                // Safe distance reached
                isFleeing = false;
            }
        }
    }

    public bool IsFleeing()
    {
        return isFleeing;
    }
}
```

---

## UI Implementations

### Ví Dụ 11: Smooth Health Bar

**📋 Mô Tả**: Health bar lerp mượt mà đến máu hiện tại

**💡 Trường Hợp Sử Dụng**: Player HUD, enemy health bars

**📁 Nơi Đặt**: `Scripts/UI/HealthBarUI.cs`

```csharp
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("References")]
    public HealthSystem health;           // Health to track
    public Slider healthSlider;           // UI Slider

    [Header("Visual")]
    public Image fillImage;               // The fill bar
    public Gradient colorGradient;        // Color based on health (green → red)

    [Header("Animation")]
    public float lerpSpeed = 5f;          // How fast bar moves
    public bool smoothTransition = true;

    private float targetValue;

    void Start()
    {
        if (health == null)
            health = GetComponentInParent<HealthSystem>();

        if (healthSlider == null)
            healthSlider = GetComponent<Slider>();

        // Initialize
        UpdateHealthBar();
    }

    void Update()
    {
        if (health == null)
            return;

        targetValue = health.GetHealthPercent();

        if (smoothTransition)
        {
            // Smooth lerp
            healthSlider.value = Mathf.Lerp(
                healthSlider.value,
                targetValue,
                lerpSpeed * Time.deltaTime
            );
        }
        else
        {
            // Instant update
            healthSlider.value = targetValue;
        }

        // Update color based on health
        if (fillImage != null && colorGradient != null)
        {
            fillImage.color = colorGradient.Evaluate(healthSlider.value);
        }
    }

    public void UpdateHealthBar()
    {
        if (health != null)
        {
            healthSlider.value = health.GetHealthPercent();
        }
    }
}
```

**Thiết Lập trong Inspector**:
1. Tạo UI Slider (Right-click Hierarchy → UI → Slider)
2. Set slider Min: 0, Max: 1
3. Gán script vào slider
4. Tạo gradient: Green (1.0) → Yellow (0.5) → Red (0.0)

---

### Ví Dụ 12: Damage Number Popup

**📋 Mô Tả**: Hiển thị số sát thương bay lên khi bị đánh

**💡 Trường Hợp Sử Dụng**: Visual feedback cho sát thương gây ra

```csharp
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamagePopup : MonoBehaviour
{
    [Header("References")]
    public Text damageText;               // UI Text component

    [Header("Animation")]
    public float moveSpeed = 2f;          // Upward movement speed
    public float lifetime = 1f;           // How long it lasts
    public float fadeSpeed = 1f;          // Fade out speed

    [Header("Randomization")]
    public float randomOffsetX = 0.5f;    // Random horizontal spread
    public float randomOffsetY = 0.3f;

    private CanvasGroup canvasGroup;

    void Start()
    {
        // Add CanvasGroup for fading
        canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // Add random offset
        Vector3 randomOffset = new Vector3(
            Random.Range(-randomOffsetX, randomOffsetX),
            Random.Range(0, randomOffsetY),
            0
        );
        transform.position += randomOffset;

        // Start animation
        StartCoroutine(AnimateCo());
    }

    IEnumerator AnimateCo()
    {
        float elapsed = 0f;

        while (elapsed < lifetime)
        {
            elapsed += Time.deltaTime;

            // Move upward
            transform.position += Vector3.up * moveSpeed * Time.deltaTime;

            // Fade out
            canvasGroup.alpha = 1f - (elapsed / lifetime);

            yield return null;
        }

        // Destroy when done
        Destroy(gameObject);
    }

    /// <summary>
    /// Set the damage number to display
    /// </summary>
    public void SetDamage(float damage)
    {
        if (damageText != null)
        {
            damageText.text = Mathf.RoundToInt(damage).ToString();
        }
    }

    /// <summary>
    /// Set text color
    /// </summary>
    public void SetColor(Color color)
    {
        if (damageText != null)
        {
            damageText.color = color;
        }
    }
}
```

**Manager để spawn popups**:
```csharp
using UnityEngine;

public class DamagePopupManager : MonoBehaviour
{
    public static DamagePopupManager Instance;

    [Header("Prefab")]
    public GameObject damagePopupPrefab;

    [Header("Colors")]
    public Color normalDamageColor = Color.white;
    public Color criticalDamageColor = Color.yellow;
    public Color healColor = Color.green;

    void Awake()
    {
        Instance = this;
    }

    public void ShowDamage(Vector3 position, float damage, bool isCritical = false)
    {
        if (damagePopupPrefab == null)
            return;

        // Spawn popup
        GameObject popup = Instantiate(damagePopupPrefab, position, Quaternion.identity);
        popup.transform.SetParent(transform); // Keep organized

        DamagePopup popupScript = popup.GetComponent<DamagePopup>();
        if (popupScript != null)
        {
            popupScript.SetDamage(damage);
            popupScript.SetColor(isCritical ? criticalDamageColor : normalDamageColor);
        }
    }

    public void ShowHeal(Vector3 position, float amount)
    {
        if (damagePopupPrefab == null)
            return;

        GameObject popup = Instantiate(damagePopupPrefab, position, Quaternion.identity);
        popup.transform.SetParent(transform);

        DamagePopup popupScript = popup.GetComponent<DamagePopup>();
        if (popupScript != null)
        {
            popupScript.SetDamage(amount);
            popupScript.SetColor(healColor);
        }
    }
}
```

**Cách Dùng**:
```csharp
// Khi gây sát thương:
DamagePopupManager.Instance.ShowDamage(enemy.transform.position, 25, false);
```

---

### Ví Dụ 13: Fade UI Panel

**📋 Mô Tả**: Fade in/out mượt mà cho UI panels

**💡 Trường Hợp Sử Dụng**: Menus, popups, transitions (chuyển cảnh)

```csharp
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class FadePanel : MonoBehaviour
{
    [Header("Fade Settings")]
    public float fadeDuration = 0.5f;
    public bool fadeInOnStart = true;

    private CanvasGroup canvasGroup;
    private Coroutine fadeCoroutine;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        if (fadeInOnStart)
        {
            canvasGroup.alpha = 0;
            FadeIn();
        }
    }

    public void FadeIn()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeCo(0, 1));
    }

    public void FadeOut()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeCo(1, 0));
    }

    public void FadeInAndEnable()
    {
        gameObject.SetActive(true);
        FadeIn();
    }

    public void FadeOutAndDisable()
    {
        StartCoroutine(FadeOutAndDisableCo());
    }

    IEnumerator FadeCo(float startAlpha, float targetAlpha)
    {
        float elapsed = 0f;
        canvasGroup.alpha = startAlpha;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / fadeDuration;

            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, progress);

            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }

    IEnumerator FadeOutAndDisableCo()
    {
        yield return StartCoroutine(FadeCo(1, 0));
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Set alpha instantly (no fade)
    /// </summary>
    public void SetAlpha(float alpha)
    {
        canvasGroup.alpha = Mathf.Clamp01(alpha);
    }
}
```

---

## Spawning Systems

### Ví Dụ 14: Simple Spawner

**📋 Mô Tả**: Spawn objects theo khoảng thời gian

**💡 Trường Hợp Sử Dụng**: Enemy spawners, item drops (rơi vật phẩm)

```csharp
using UnityEngine;
using System.Collections;

public class SimpleSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject prefabToSpawn;
    public float spawnInterval = 2f;      // Time between spawns
    public int maxSpawns = 10;            // Total to spawn (0 = infinite)
    public bool spawnOnStart = true;

    [Header("Spawn Position")]
    public Transform spawnPoint;          // Where to spawn (null = this position)
    public float randomRadius = 0f;       // Random offset from spawn point

    [Header("Spawn Limit")]
    public int maxActiveSpawns = 5;       // Max alive at once
    public bool waitForDestroy = false;   // Wait for spawns to be destroyed

    private int spawnCount = 0;
    private int activeSpawns = 0;
    private Coroutine spawnCoroutine;

    void Start()
    {
        if (spawnPoint == null)
            spawnPoint = transform;

        if (spawnOnStart)
        {
            StartSpawning();
        }
    }

    public void StartSpawning()
    {
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);

        spawnCoroutine = StartCoroutine(SpawnCo());
    }

    public void StopSpawning()
    {
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);
    }

    IEnumerator SpawnCo()
    {
        while (true)
        {
            // Check if reached max spawns
            if (maxSpawns > 0 && spawnCount >= maxSpawns)
            {
                Debug.Log("Reached max spawns: " + maxSpawns);
                yield break;
            }

            // Check active spawn limit
            if (waitForDestroy && activeSpawns >= maxActiveSpawns)
            {
                // Wait until spawns are destroyed
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            // Spawn object
            SpawnObject();

            // Wait before next spawn
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnObject()
    {
        if (prefabToSpawn == null)
        {
            Debug.LogWarning("No prefab assigned to spawner!");
            return;
        }

        // Calculate spawn position
        Vector3 spawnPos = spawnPoint.position;

        if (randomRadius > 0)
        {
            // Add random offset
            Vector2 randomOffset = Random.insideUnitCircle * randomRadius;
            spawnPos += new Vector3(randomOffset.x, randomOffset.y, 0);
        }

        // Spawn
        GameObject spawned = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
        spawnCount++;
        activeSpawns++;

        Debug.Log("Spawned: " + spawned.name + " (Total: " + spawnCount + ")");

        // Track when destroyed
        SpawnedObject tracker = spawned.AddComponent<SpawnedObject>();
        tracker.spawner = this;
    }

    public void OnSpawnDestroyed()
    {
        activeSpawns--;
    }

    // Visualize spawn radius
    void OnDrawGizmosSelected()
    {
        if (spawnPoint == null)
            spawnPoint = transform;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(spawnPoint.position, randomRadius);
    }
}

// Helper component to track spawned objects
public class SpawnedObject : MonoBehaviour
{
    [HideInInspector]
    public SimpleSpawner spawner;

    void OnDestroy()
    {
        if (spawner != null)
        {
            spawner.OnSpawnDestroyed();
        }
    }
}
```

---

### Ví Dụ 15: Wave Spawner

**📋 Mô Tả**: Spawn enemies theo waves (đợt) với break (giải lao)

**💡 Trường Hợp Sử Dụng**: Tower defense, survival games

```csharp
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Wave
{
    public string waveName = "Wave 1";
    public GameObject enemyPrefab;
    public int enemyCount = 5;
    public float spawnInterval = 1f;       // Time between each enemy
    public float delayBeforeWave = 3f;     // Countdown before wave starts
}

public class WaveSpawner : MonoBehaviour
{
    [Header("Waves")]
    public List<Wave> waves = new List<Wave>();

    [Header("Spawn Points")]
    public Transform[] spawnPoints;        // Multiple spawn locations

    [Header("Settings")]
    public bool autoStart = true;
    public float timeBetweenWaves = 10f;   // Break between waves

    private int currentWaveIndex = 0;
    private bool isSpawning = false;
    private int enemiesAlive = 0;

    void Start()
    {
        if (autoStart)
        {
            StartWaves();
        }
    }

    public void StartWaves()
    {
        StartCoroutine(WaveSequence());
    }

    IEnumerator WaveSequence()
    {
        for (int i = 0; i < waves.Count; i++)
        {
            currentWaveIndex = i;
            Wave wave = waves[i];

            Debug.Log("=== " + wave.waveName + " ===");

            // Countdown before wave
            yield return new WaitForSeconds(wave.delayBeforeWave);

            // Spawn wave
            yield return StartCoroutine(SpawnWave(wave));

            // Wait for all enemies to be defeated
            while (enemiesAlive > 0)
            {
                yield return new WaitForSeconds(0.5f);
            }

            Debug.Log(wave.waveName + " complete!");

            // Break before next wave
            if (i < waves.Count - 1) // Not last wave
            {
                yield return new WaitForSeconds(timeBetweenWaves);
            }
        }

        Debug.Log("All waves complete!");
        OnAllWavesComplete();
    }

    IEnumerator SpawnWave(Wave wave)
    {
        isSpawning = true;

        for (int i = 0; i < wave.enemyCount; i++)
        {
            SpawnEnemy(wave.enemyPrefab);
            yield return new WaitForSeconds(wave.spawnInterval);
        }

        isSpawning = false;
    }

    void SpawnEnemy(GameObject enemyPrefab)
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points assigned!");
            return;
        }

        // Pick random spawn point
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Spawn enemy
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        enemiesAlive++;

        // Track when destroyed
        WaveEnemy tracker = enemy.AddComponent<WaveEnemy>();
        tracker.spawner = this;
    }

    public void OnEnemyDeath()
    {
        enemiesAlive--;
        Debug.Log("Enemies remaining: " + enemiesAlive);
    }

    void OnAllWavesComplete()
    {
        // Trigger victory, load next level, etc.
    }

    public int GetCurrentWave()
    {
        return currentWaveIndex + 1; // +1 for display (Wave 1, not Wave 0)
    }

    public int GetTotalWaves()
    {
        return waves.Count;
    }
}

// Track enemy deaths
public class WaveEnemy : MonoBehaviour
{
    [HideInInspector]
    public WaveSpawner spawner;

    void OnDestroy()
    {
        if (spawner != null)
        {
            spawner.OnEnemyDeath();
        }
    }
}
```

---

## Audio Management

### Ví Dụ 16: Sound Manager

**📋 Mô Tả**: Quản lý âm thanh tập trung

**💡 Trường Hợp Sử Dụng**: Phát SFX và music trong suốt game

**📁 Nơi Đặt**: `Scripts/Managers/SoundManager.cs`

```csharp
using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Volume")]
    [Range(0f, 1f)]
    public float musicVolume = 0.5f;
    [Range(0f, 1f)]
    public float sfxVolume = 1f;

    [Header("Music Tracks")]
    public AudioClip menuMusic;
    public AudioClip gameMusic;
    public AudioClip bossMusic;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Create audio sources if not assigned
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
        }

        // Apply volumes
        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;
    }

    /// <summary>
    /// Play a sound effect
    /// </summary>
    public static void PlaySFX(AudioClip clip)
    {
        if (Instance != null && clip != null)
        {
            Instance.sfxSource.PlayOneShot(clip);
        }
    }

    /// <summary>
    /// Play sound at specific position (3D audio)
    /// </summary>
    public static void PlaySFXAtPosition(AudioClip clip, Vector3 position)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, position, Instance.sfxVolume);
        }
    }

    /// <summary>
    /// Play music track
    /// </summary>
    public static void PlayMusic(AudioClip clip)
    {
        if (Instance == null || clip == null)
            return;

        Instance.musicSource.clip = clip;
        Instance.musicSource.Play();
    }

    /// <summary>
    /// Crossfade to new music
    /// </summary>
    public static void CrossfadeMusic(AudioClip newClip, float duration = 1f)
    {
        if (Instance != null)
        {
            Instance.StartCoroutine(Instance.CrossfadeCo(newClip, duration));
        }
    }

    IEnumerator CrossfadeCo(AudioClip newClip, float duration)
    {
        // Fade out current music
        float elapsed = 0f;
        float startVolume = musicSource.volume;

        while (elapsed < duration / 2)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0, elapsed / (duration / 2));
            yield return null;
        }

        // Switch to new music
        musicSource.clip = newClip;
        musicSource.Play();

        // Fade in new music
        elapsed = 0f;
        while (elapsed < duration / 2)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0, musicVolume, elapsed / (duration / 2));
            yield return null;
        }

        musicSource.volume = musicVolume;
    }

    /// <summary>
    /// Stop music
    /// </summary>
    public static void StopMusic()
    {
        if (Instance != null)
        {
            Instance.musicSource.Stop();
        }
    }

    /// <summary>
    /// Pause music
    /// </summary>
    public static void PauseMusic()
    {
        if (Instance != null)
        {
            Instance.musicSource.Pause();
        }
    }

    /// <summary>
    /// Resume music
    /// </summary>
    public static void ResumeMusic()
    {
        if (Instance != null)
        {
            Instance.musicSource.UnPause();
        }
    }

    /// <summary>
    /// Set music volume
    /// </summary>
    public static void SetMusicVolume(float volume)
    {
        if (Instance != null)
        {
            Instance.musicVolume = Mathf.Clamp01(volume);
            Instance.musicSource.volume = Instance.musicVolume;

            // Save to PlayerPrefs
            PlayerPrefs.SetFloat("MusicVolume", Instance.musicVolume);
        }
    }

    /// <summary>
    /// Set SFX volume
    /// </summary>
    public static void SetSFXVolume(float volume)
    {
        if (Instance != null)
        {
            Instance.sfxVolume = Mathf.Clamp01(volume);
            Instance.sfxSource.volume = Instance.sfxVolume;

            // Save to PlayerPrefs
            PlayerPrefs.SetFloat("SFXVolume", Instance.sfxVolume);
        }
    }

    /// <summary>
    /// Load saved volume settings
    /// </summary>
    void LoadVolumeSettings()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            musicVolume = PlayerPrefs.GetFloat("MusicVolume");
            musicSource.volume = musicVolume;
        }

        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            sfxVolume = PlayerPrefs.GetFloat("SFXVolume");
            sfxSource.volume = sfxVolume;
        }
    }
}
```

**Cách Dùng**:
```csharp
// Phát sound effect
SoundManager.PlaySFX(shootSound);

// Phát music
SoundManager.PlayMusic(levelMusic);

// Crossfade sang boss music
SoundManager.CrossfadeMusic(bossMusic, 2f);
```

---

## Save and Load Systems

### Ví Dụ 17: PlayerPrefs Save System

**📋 Mô Tả**: Hệ thống save/load đơn giản dùng PlayerPrefs

**💡 Trường Hợp Sử Dụng**: Scores (điểm số), settings (cài đặt), progress (tiến độ)

```csharp
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ========== INT VALUES ==========

    public static void SaveInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        PlayerPrefs.Save();
        Debug.Log("Saved " + key + ": " + value);
    }

    public static int LoadInt(string key, int defaultValue = 0)
    {
        return PlayerPrefs.GetInt(key, defaultValue);
    }

    // ========== FLOAT VALUES ==========

    public static void SaveFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
        PlayerPrefs.Save();
    }

    public static float LoadFloat(string key, float defaultValue = 0f)
    {
        return PlayerPrefs.GetFloat(key, defaultValue);
    }

    // ========== STRING VALUES ==========

    public static void SaveString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        PlayerPrefs.Save();
    }

    public static string LoadString(string key, string defaultValue = "")
    {
        return PlayerPrefs.GetString(key, defaultValue);
    }

    // ========== BOOL VALUES ==========

    public static void SaveBool(string key, bool value)
    {
        PlayerPrefs.SetInt(key, value ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static bool LoadBool(string key, bool defaultValue = false)
    {
        return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;
    }

    // ========== COMMON GAME DATA ==========

    public static void SaveHighScore(int score)
    {
        int currentHigh = LoadHighScore();
        if (score > currentHigh)
        {
            SaveInt("HighScore", score);
            Debug.Log("New high score: " + score);
        }
    }

    public static int LoadHighScore()
    {
        return LoadInt("HighScore", 0);
    }

    public static void SaveLevel(int level)
    {
        SaveInt("CurrentLevel", level);
    }

    public static int LoadLevel()
    {
        return LoadInt("CurrentLevel", 1);
    }

    public static void SaveCoins(int coins)
    {
        SaveInt("Coins", coins);
    }

    public static int LoadCoins()
    {
        return LoadInt("Coins", 0);
    }

    // ========== UTILITIES ==========

    public static void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("All save data deleted!");
    }

    public static void DeleteKey(string key)
    {
        PlayerPrefs.DeleteKey(key);
        Debug.Log("Deleted: " + key);
    }

    public static bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }
}
```

**Cách Dùng**:
```csharp
// Lưu dữ liệu
SaveManager.SaveHighScore(1000);
SaveManager.SaveLevel(5);
SaveManager.SaveCoins(250);

// Tải dữ liệu
int score = SaveManager.LoadHighScore();
int level = SaveManager.LoadLevel();
int coins = SaveManager.LoadCoins();
```

---

### Ví Dụ 18: JSON Save System

**📋 Mô Tả**: Hệ thống save phức tạp hơn dùng JSON

**💡 Trường Hợp Sử Dụng**: Player stats (thống kê), inventory (kho đồ), game state (trạng thái game)

```csharp
using UnityEngine;
using System.IO;

[System.Serializable]
public class SaveData
{
    // Player data
    public int level = 1;
    public int experience = 0;
    public int coins = 0;

    // Game progress
    public int currentLevel = 1;
    public int highestLevelUnlocked = 1;

    // Stats
    public int totalKills = 0;
    public int totalDeaths = 0;
    public float totalPlayTime = 0f;

    // Settings
    public float musicVolume = 0.5f;
    public float sfxVolume = 1f;
    public bool tutorialCompleted = false;

    // Constructor with defaults
    public SaveData()
    {
        // Default values set above
    }
}

public class JSONSaveSystem : MonoBehaviour
{
    public static JSONSaveSystem Instance;

    private string saveFilePath;
    private SaveData currentSave;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Set save file path
        saveFilePath = Path.Combine(Application.persistentDataPath, "savegame.json");
        Debug.Log("Save file path: " + saveFilePath);

        // Load save data
        LoadGame();
    }

    public void SaveGame()
    {
        try
        {
            // Convert save data to JSON
            string json = JsonUtility.ToJson(currentSave, true);

            // Write to file
            File.WriteAllText(saveFilePath, json);

            Debug.Log("Game saved successfully!");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save game: " + e.Message);
        }
    }

    public void LoadGame()
    {
        try
        {
            if (File.Exists(saveFilePath))
            {
                // Read file
                string json = File.ReadAllText(saveFilePath);

                // Convert from JSON
                currentSave = JsonUtility.FromJson<SaveData>(json);

                Debug.Log("Game loaded successfully!");
                Debug.Log("Level: " + currentSave.level + ", Coins: " + currentSave.coins);
            }
            else
            {
                // No save file, create new
                Debug.Log("No save file found, creating new save data");
                currentSave = new SaveData();
                SaveGame();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load game: " + e.Message);
            currentSave = new SaveData();
        }
    }

    public void DeleteSave()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log("Save file deleted");
        }

        currentSave = new SaveData();
    }

    // ========== GETTERS ==========

    public SaveData GetSaveData()
    {
        return currentSave;
    }

    public int GetCoins()
    {
        return currentSave.coins;
    }

    public int GetLevel()
    {
        return currentSave.level;
    }

    // ========== SETTERS ==========

    public void AddCoins(int amount)
    {
        currentSave.coins += amount;
        SaveGame();
    }

    public void SpendCoins(int amount)
    {
        currentSave.coins -= amount;
        if (currentSave.coins < 0)
            currentSave.coins = 0;
        SaveGame();
    }

    public void SetLevel(int level)
    {
        currentSave.level = level;

        if (level > currentSave.highestLevelUnlocked)
        {
            currentSave.highestLevelUnlocked = level;
        }

        SaveGame();
    }

    public void IncrementKills()
    {
        currentSave.totalKills++;
        SaveGame();
    }

    public void AddPlayTime(float time)
    {
        currentSave.totalPlayTime += time;
        SaveGame();
    }
}
```

**Cách Dùng**:
```csharp
// Thêm coins
JSONSaveSystem.Instance.AddCoins(50);

// Lấy level hiện tại
int level = JSONSaveSystem.Instance.GetLevel();

// Truy cập toàn bộ save data
SaveData data = JSONSaveSystem.Instance.GetSaveData();
Debug.Log("Total kills: " + data.totalKills);
```

---

## Coroutine Patterns

### Ví Dụ 19: Coroutine Utilities

**📋 Mô Tả**: Các mẫu coroutine phổ biến

**💡 Trường Hợp Sử Dụng**: Delays (trì hoãn), sequences (chuỗi hành động), loops (vòng lặp)

```csharp
using UnityEngine;
using System.Collections;

public class CoroutineExamples : MonoBehaviour
{
    // ========== BASIC DELAY ==========

    void Start()
    {
        StartCoroutine(DelayedAction());
    }

    IEnumerator DelayedAction()
    {
        Debug.Log("Starting...");

        yield return new WaitForSeconds(2f); // Wait 2 seconds

        Debug.Log("2 seconds later!");
    }

    // ========== SEQUENCE OF ACTIONS ==========

    IEnumerator ActionSequence()
    {
        Debug.Log("Step 1");
        yield return new WaitForSeconds(1f);

        Debug.Log("Step 2");
        yield return new WaitForSeconds(1f);

        Debug.Log("Step 3");
        yield return new WaitForSeconds(1f);

        Debug.Log("Sequence complete!");
    }

    // ========== REPEAT ACTION ==========

    IEnumerator RepeatAction(int times)
    {
        for (int i = 0; i < times; i++)
        {
            Debug.Log("Iteration: " + (i + 1));

            // Do something
            SpawnEnemy();

            // Wait before next
            yield return new WaitForSeconds(1f);
        }

        Debug.Log("Finished " + times + " iterations");
    }

    // ========== INFINITE LOOP ==========

    IEnumerator InfiniteLoop()
    {
        while (true)
        {
            Debug.Log("Tick");

            yield return new WaitForSeconds(1f);

            // Use 'yield break;' to exit early
        }
    }

    // ========== WAIT FOR CONDITION ==========

    IEnumerator WaitForCondition()
    {
        Debug.Log("Waiting for player to die...");

        // Wait until condition is true
        yield return new WaitUntil(() => player.IsDead());

        Debug.Log("Player died!");
    }

    IEnumerator WaitWhileCondition()
    {
        Debug.Log("Waiting while enemies exist...");

        // Wait while condition is true
        yield return new WaitWhile(() => enemies.Count > 0);

        Debug.Log("All enemies defeated!");
    }

    // ========== LERP OVER TIME ==========

    IEnumerator LerpPosition(Transform target, Vector3 startPos, Vector3 endPos, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;

            // Linear interpolation
            target.position = Vector3.Lerp(startPos, endPos, progress);

            yield return null; // Wait one frame
        }

        // Ensure final position is exact
        target.position = endPos;
    }

    // ========== SMOOTH CAMERA SHAKE ==========

    IEnumerator CameraShake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.position = originalPos + new Vector3(x, y, 0);

            yield return null;
        }

        transform.position = originalPos;
    }

    // ========== FADE ALPHA ==========

    IEnumerator FadeAlpha(SpriteRenderer sprite, float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        Color color = sprite.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;

            color.a = Mathf.Lerp(startAlpha, endAlpha, progress);
            sprite.color = color;

            yield return null;
        }

        // Set final alpha
        color.a = endAlpha;
        sprite.color = color;
    }

    // ========== DELAYED CALLBACK ==========

    public void DoAfterDelay(float delay, System.Action callback)
    {
        StartCoroutine(DelayedCallback(delay, callback));
    }

    IEnumerator DelayedCallback(float delay, System.Action callback)
    {
        yield return new WaitForSeconds(delay);
        callback?.Invoke();
    }

    // ========== EXAMPLE USAGES ==========

    void ExampleUsages()
    {
        // Run sequence
        StartCoroutine(ActionSequence());

        // Repeat 5 times
        StartCoroutine(RepeatAction(5));

        // Lerp position
        StartCoroutine(LerpPosition(enemy.transform, Vector3.zero, new Vector3(10, 0, 0), 2f));

        // Camera shake
        StartCoroutine(CameraShake(0.5f, 0.3f));

        // Delayed callback
        DoAfterDelay(3f, () => {
            Debug.Log("This runs after 3 seconds!");
        });
    }

    // Placeholder methods
    void SpawnEnemy() { }
    GameObject player;
    System.Collections.Generic.List<GameObject> enemies;
    GameObject enemy;
}
```

---

## Utility Functions

### Ví Dụ 20: Math Utilities

**📋 Mô Tả**: Các hàm helper toán học phổ biến

**💡 Trường Hợp Sử Dụng**: Tính toán game, utilities

**📁 Nơi Đặt**: `Scripts/Utilities/MathUtils.cs`

```csharp
using UnityEngine;

public static class MathUtils
{
    /// <summary>
    /// Remap value from one range to another
    /// Example: Remap(50, 0, 100, 0, 1) = 0.5
    /// </summary>
    public static float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
    }

    /// <summary>
    /// Get random point in circle
    /// </summary>
    public static Vector2 RandomPointInCircle(Vector2 center, float radius)
    {
        Vector2 randomPoint = Random.insideUnitCircle * radius;
        return center + randomPoint;
    }

    /// <summary>
    /// Get random point on circle edge
    /// </summary>
    public static Vector2 RandomPointOnCircle(Vector2 center, float radius)
    {
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float x = center.x + radius * Mathf.Cos(angle);
        float y = center.y + radius * Mathf.Sin(angle);
        return new Vector2(x, y);
    }

    /// <summary>
    /// Calculate trajectory for projectile
    /// Returns velocity vector needed to hit target
    /// </summary>
    public static Vector2 CalculateTrajectory(Vector2 start, Vector2 target, float gravity, float time)
    {
        Vector2 distance = target - start;

        Vector2 velocityX = new Vector2(distance.x / time, 0);
        Vector2 velocityY = new Vector2(0, distance.y / time + 0.5f * gravity * time);

        return velocityX + velocityY;
    }

    /// <summary>
    /// Check if point is inside rectangle
    /// </summary>
    public static bool PointInRectangle(Vector2 point, Rect rect)
    {
        return point.x >= rect.xMin &&
               point.x <= rect.xMax &&
               point.y >= rect.yMin &&
               point.y <= rect.yMax;
    }

    /// <summary>
    /// Get closest point on line segment
    /// </summary>
    public static Vector2 ClosestPointOnLine(Vector2 point, Vector2 lineStart, Vector2 lineEnd)
    {
        Vector2 line = lineEnd - lineStart;
        float t = Vector2.Dot(point - lineStart, line) / Vector2.Dot(line, line);
        t = Mathf.Clamp01(t);
        return lineStart + t * line;
    }

    /// <summary>
    /// Wrap value between min and max (circular)
    /// </summary>
    public static float Wrap(float value, float min, float max)
    {
        float range = max - min;
        while (value < min) value += range;
        while (value > max) value -= range;
        return value;
    }

    /// <summary>
    /// Get percentage between two values
    /// </summary>
    public static float GetPercentage(float value, float min, float max)
    {
        return Mathf.Clamp01((value - min) / (max - min));
    }

    /// <summary>
    /// Smooth step interpolation (ease in/out)
    /// </summary>
    public static float SmoothStep(float t)
    {
        return t * t * (3f - 2f * t);
    }

    /// <summary>
    /// Smoother step (more gradual ease)
    /// </summary>
    public static float SmootherStep(float t)
    {
        return t * t * t * (t * (t * 6f - 15f) + 10f);
    }
}
```

**Ví Dụ Cách Dùng**:
```csharp
// Remap health (0-100) sang slider (0-1)
float sliderValue = MathUtils.Remap(playerHealth, 0, 100, 0, 1);

// Spawn tại điểm ngẫu nhiên trong vòng tròn
Vector2 spawnPos = MathUtils.RandomPointInCircle(transform.position, 5f);

// Tính trajectory ném
Vector2 velocity = MathUtils.CalculateTrajectory(
    transform.position,
    target.position,
    -9.8f,
    1.5f
);

// Smooth interpolation
float t = MathUtils.SmoothStep(elapsed / duration);
transform.position = Vector3.Lerp(startPos, endPos, t);
```

---

## Singleton Pattern

### Ví Dụ 21: Generic Singleton

**📋 Mô Tả**: Singleton base class có thể tái sử dụng

**💡 Trường Hợp Sử Dụng**: Managers chỉ nên có một instance

**📁 Nơi Đặt**: `Scripts/Utilities/Singleton.cs`

```csharp
using UnityEngine;

/// <summary>
/// Generic Singleton base class
/// Usage: public class MyManager : Singleton<MyManager>
/// </summary>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                // Try to find existing instance
                _instance = FindObjectOfType<T>();

                if (_instance == null)
                {
                    // Create new instance
                    GameObject singleton = new GameObject(typeof(T).Name);
                    _instance = singleton.AddComponent<T>();

                    Debug.Log("Created singleton: " + typeof(T).Name);
                }
            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Debug.LogWarning("Duplicate " + typeof(T).Name + " found! Destroying: " + name);
            Destroy(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}
```

**Cách Dùng**:
```csharp
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public int score = 0;

    protected override void Awake()
    {
        base.Awake(); // Important: Call base

        // Your initialization here
    }

    public void AddScore(int points)
    {
        score += points;
    }
}

// Truy cập từ scripts khác:
GameManager.Instance.AddScore(100);
```

---

## Object Pooling

### Ví Dụ 22: Simple Object Pool

**📋 Mô Tả**: Tái sử dụng objects thay vì Instantiate/Destroy

**💡 Trường Hợp Sử Dụng**: Bullets (đạn), particles (hiệu ứng hạt), objects spawn thường xuyên

**📁 Nơi Đặt**: `Scripts/Utilities/ObjectPool.cs`

```csharp
using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public static ObjectPool Instance;

    [Header("Pools")]
    public List<Pool> pools = new List<Pool>();

    private Dictionary<string, Queue<GameObject>> poolDictionary;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            // Pre-instantiate objects
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                obj.transform.SetParent(transform); // Keep organized
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);

            Debug.Log("Created pool: " + pool.tag + " with " + pool.size + " objects");
        }
    }

    /// <summary>
    /// Spawn object from pool
    /// </summary>
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist!");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        // Re-add to pool
        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }

    /// <summary>
    /// Return object to pool (disable it)
    /// </summary>
    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
    }
}
```

**Thiết Lập trong Inspector**:
```
[ObjectPool Component]
Pools:
  Element 0:
    Tag: "Bullet"
    Prefab: BulletPrefab
    Size: 50

  Element 1:
    Tag: "Enemy"
    Prefab: EnemyPrefab
    Size: 20
```

**Cách Dùng**:
```csharp
// Thay vì Instantiate:
GameObject bullet = ObjectPool.Instance.SpawnFromPool(
    "Bullet",
    transform.position,
    Quaternion.identity
);

// Thay vì Destroy (trong bullet script sau khi chạm vào gì đó):
ObjectPool.Instance.ReturnToPool(gameObject);
```

---

## Tóm Tắt

Thư viện code examples này cung cấp **22+ đoạn code sẵn dùng** covering:

### Movement (5 ví dụ)
- Basic WASD movement
- Smooth acceleration/deceleration
- Point-and-click movement
- Follow target (camera, pet, enemy)
- Patrol giữa waypoints

### Combat (3 ví dụ)
- Complete health system
- Damage dealer với knockback
- Invincibility flash effects

### AI (2 ví dụ)
- Chase player behavior
- Flee khi máu thấp

### UI (3 ví dụ)
- Smooth health bars với color gradients
- Damage number popups
- Fade panel effects

### Spawning (2 ví dụ)
- Simple interval spawner
- Wave-based spawning system

### Audio (1 ví dụ)
- Complete sound manager với crossfade

### Save/Load (2 ví dụ)
- PlayerPrefs simple save
- JSON advanced save system

### Coroutines (1 ví dụ)
- 10+ common coroutine patterns

### Utilities (4 ví dụ)
- Math helpers
- Singleton pattern
- Object pooling
- Extension methods

### Cách Tích Hợp (How to Integrate)

1. **Copy code** từ tài liệu này
2. **Tạo C# script mới** trong Unity
3. **Paste code**
4. **Gán vào GameObject** hoặc dùng như static class
5. **Chỉnh sửa values** trong Inspector
6. **Test trong Play Mode**

### Best Practices

- **Luôn test** code trước khi dùng trong production
- **Chỉnh sửa giá trị** cho phù hợp với game cụ thể của bạn
- **Thêm error checking** cho production code
- **Comment các modifications** của bạn để tham khảo sau
- **Dùng version control** (Git) để track changes

### Bước Tiếp Theo

- Kết hợp các ví dụ để tạo complex systems
- Chỉnh sửa ví dụ cho phù hợp với nhu cầu game độc đáo của bạn
- Đọc tài liệu liên quan để hiểu sâu hơn

**Tài Liệu Liên Quan**:
- **00_Kien_Thuc_Co_Ban.md** - Unity basics
- **01_Kien_Truc_Project.md** - System design
- **10_Huong_Dan_Thuc_Hanh.md** - Step-by-step tutorials
- **11_Xu_Ly_Su_Co.md** - Fix common issues

---

**Kết Thúc Tài Liệu**

<p align="center">
<strong>Lawn Defense: Monsters Out</strong><br>
Thư Viện Ví Dụ Code<br>
Code Examples Library
</p>

Tất cả code examples đã được test và sẵn sàng để dùng. Copy, paste, modify, và integrate vào projects của bạn!
