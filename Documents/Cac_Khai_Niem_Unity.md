# Các Khái Niệm Unity Quan Trọng (Quick Reference)

**Mục Đích**: Quick reference cho các khái niệm Unity quan trọng được dùng trong project này.

**Lưu Ý**: Để có Unity fundamentals đầy đủ, xem `00_Cac_Khai_Niem_Unity_Co_Ban.md` (1,200+ dòng với giải thích chi tiết).

**Tài liệu này cung cấp**: Quick lookups và reminders cho developers đã quen với Unity basics.

---

## 1. GameObject & Component System

**GameObject**: Container cho components (như một "thing" trong game của bạn)

**Component**: Behavior hoặc property attached vào GameObject (Transform, Collider, Script, etc.)

```csharp
// Lấy component
Rigidbody2D rb = GetComponent<Rigidbody2D>();

// Thêm component tại runtime
gameObject.AddComponent<BoxCollider2D>();

// Tìm GameObject
GameObject player = GameObject.Find("Player");
GameObject enemy = GameObject.FindWithTag("Enemy");
```

**Xem**: `00_Cac_Khai_Niem_Unity_Co_Ban.md` → "GameObject and Component Architecture"

---

## 2. Prefab System

**Prefab**: Template GameObject được lưu trong Project window

**Tại sao dùng prefabs**: Spawn nhiều copies, cập nhật tất cả instances cùng lúc

```csharp
// Instantiate prefab
GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

// Destroy instance
Destroy(enemy, 2f);  // Destroy sau 2 giây
```

**Xem**: `00_Cac_Khai_Niem_Unity_Co_Ban.md` → "Prefabs"

---

## 3. Scene Management

**Scene**: Container cho GameObjects (như một level hoặc menu)

```csharp
using UnityEngine.SceneManagement;

// Load scene
SceneManager.LoadScene("Game");

// Load asynchronously (với loading screen)
AsyncOperation op = SceneManager.LoadSceneAsync("Game");

// Lấy current scene
Scene current = SceneManager.GetActiveScene();
```

**Xem**: `Ban_Do.md` cho complete scene loading guide

---

## 4. MonoBehaviour Lifecycle

**Thứ tự execution**:
```
Awake() → OnEnable() → Start() → Update()/FixedUpdate()/LateUpdate() → OnDisable() → OnDestroy()
```

**Các methods phổ biến**:
- `Awake()`: Initialize trước Start (chạy ngay cả khi disabled)
- `Start()`: Initialize sau Awake (chỉ khi enabled)
- `Update()`: Mỗi frame (60 FPS = 60 lần/giây)
- `FixedUpdate()`: Fixed time step (cho physics, default 50 FPS)
- `LateUpdate()`: Sau tất cả Update() calls (cho cameras)

**Xem**: `00_Cac_Khai_Niem_Unity_Co_Ban.md` → "MonoBehaviour Lifecycle"

---

## 5. Coroutines

**Mục Đích**: Chạy code qua nhiều frames với delays

```csharp
IEnumerator MyCoroutine()
{
    Debug.Log("Start");
    yield return new WaitForSeconds(2f);  // Đợi 2 giây
    Debug.Log("After 2 seconds");
}

// Bắt đầu coroutine
StartCoroutine(MyCoroutine());

// Dừng coroutine
StopCoroutine(MyCoroutine());
StopAllCoroutines();
```

**Các yield returns phổ biến**:
- `yield return null` - Đợi một frame
- `yield return new WaitForSeconds(2f)` - Đợi 2 giây
- `yield return new WaitForFixedUpdate()` - Đợi đến next FixedUpdate
- `yield return new WaitUntil(() => condition)` - Đợi cho đến khi condition true

**Xem**: `00_Cac_Khai_Niem_Unity_Co_Ban.md` → "Coroutines"

---

## 6. Physics2D System

**Collider2D**: Định nghĩa collision shape

**Rigidbody2D**: Thêm physics simulation (gravity, forces)

```csharp
// Raycast
RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, layerMask);
if (hit.collider != null)
{
    Debug.Log("Hit: " + hit.collider.name);
}

// CircleCast (cho area detection)
RaycastHit2D[] hits = Physics2D.CircleCastAll(center, radius, Vector2.zero, 0, layerMask);

// Trigger collision
void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("Player"))
    {
        Debug.Log("Player entered trigger!");
    }
}
```

**Xem**: `00_Cac_Khai_Niem_Unity_Co_Ban.md` → "Physics and Collisions"

---

## 7. Animation System

**Animator**: State machine controller cho animations

**Animation Clip**: Animation data thực tế

```csharp
Animator anim = GetComponent<Animator>();

// Trigger animation
anim.SetTrigger("Jump");

// Set bool (cho transitions)
anim.SetBool("IsRunning", true);

// Set float (cho blend trees)
anim.SetFloat("Speed", 5.0f);
```

**Xem**: `He_Thong_Player_Nang_Cao.md` và `He_Thong_Enemy_Nang_Cao.md` cho animation system usage

---

## 8. PlayerPrefs (Save Data)

**Mục Đích**: Simple key-value storage (persists giữa các sessions)

```csharp
// Save data
PlayerPrefs.SetInt("highScore", 1000);
PlayerPrefs.SetFloat("volume", 0.8f);
PlayerPrefs.SetString("playerName", "Hero");
PlayerPrefs.Save();  // Force save (auto-saves khi quit)

// Load data
int score = PlayerPrefs.GetInt("highScore", 0);  // Default 0 nếu không tìm thấy
float vol = PlayerPrefs.GetFloat("volume", 1.0f);
string name = PlayerPrefs.GetString("playerName", "Player");

// Delete data
PlayerPrefs.DeleteKey("highScore");
PlayerPrefs.DeleteAll();  // Xóa mọi thứ
```

**Xem**: `Thuoc_Tinh_Nhan_Vat.md` cho advanced PlayerPrefs usage

---

## 9. Events & Delegates

**UnityEvent**: Inspector-assignable event (như Button.onClick)

```csharp
using UnityEngine.Events;

public class MyScript : MonoBehaviour
{
    public UnityEvent onPlayerDied;  // Visible trong Inspector

    void Die()
    {
        onPlayerDied.Invoke();  // Trigger tất cả listeners
    }
}
```

**C# Delegate**:
```csharp
public delegate void OnEnemyKilled(Enemy enemy);
public static OnEnemyKilled onEnemyKilled;

// Subscribe
void OnEnable()
{
    onEnemyKilled += HandleEnemyKilled;
}

void OnDisable()
{
    onEnemyKilled -= HandleEnemyKilled;
}

void HandleEnemyKilled(Enemy enemy)
{
    Debug.Log(enemy.name + " was killed!");
}

// Invoke
onEnemyKilled?.Invoke(this);
```

**Xem**: `Su_Kien_Va_Trigger.md` cho complete event system guide

---

## 10. Layers & Tags

**Layers**: Cho collision filtering và raycasts (tối đa 32 layers)

**Tags**: Cho identifying GameObjects (không giới hạn)

```csharp
// Kiểm tra tag
if (other.CompareTag("Enemy"))
{
    // Làm gì đó
}

// Kiểm tra layer
if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
{
    // Làm gì đó
}

// LayerMask cho raycasts (chỉ hit specific layers)
LayerMask enemyLayer = LayerMask.GetMask("Enemy", "Boss");
RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, enemyLayer);
```

**Xem**: `00_Cac_Khai_Niem_Unity_Co_Ban.md` → "Layers and Tags"

---

## 11. Namespaces

**Mục Đích**: Tổ chức code, tránh name conflicts

```csharp
namespace RGame
{
    public class Enemy : MonoBehaviour
    {
        // Đây là RGame.Enemy
    }
}

// Dùng với "using" directive
using RGame;

void Start()
{
    Enemy enemy = FindObjectOfType<Enemy>();  // Không cần RGame.Enemy
}
```

**Xem**: `Namespace.md` cho complete namespace guide

---

## 12. Common Patterns Trong Project Này

### Singleton Pattern

```csharp
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }
}

// Cách dùng:
GameManager.Instance.StartGame();
```

### Observer Pattern (IListener)

```csharp
public interface IListener
{
    void IPlay();
    void IPause();
}

public class Enemy : MonoBehaviour, IListener
{
    void OnEnable()
    {
        GameManager.Instance.AddListener(this);
    }

    public void IPlay()
    {
        // Bắt đầu enemy AI
    }
}
```

### State Machine (ENEMYSTATE enum)

```csharp
public enum ENEMYSTATE
{
    IDLE, WALK, ATTACK, HIT, DEATH
}

public ENEMYSTATE currentState;

void Update()
{
    switch (currentState)
    {
        case ENEMYSTATE.IDLE:
            // Idle behavior
            break;
        case ENEMYSTATE.WALK:
            // Walk behavior
            break;
    }
}
```

---

## Quick Comparison Table

| Khái Niệm | Mục Đích | Ví Dụ |
|---------|---------|---------|
| GameObject | Container cho components | `GameObject player` |
| Component | Behavior/property | `Rigidbody2D`, `BoxCollider2D` |
| Prefab | Template cho GameObjects | `Instantiate(enemyPrefab)` |
| Scene | Level/menu container | `SceneManager.LoadScene("Game")` |
| Coroutine | Time-based operations | `yield return new WaitForSeconds(2f)` |
| PlayerPrefs | Save data | `PlayerPrefs.SetInt("score", 100)` |
| LayerMask | Collision filtering | `Physics2D.Raycast(..., layerMask)` |
| Tag | Identify GameObject | `CompareTag("Enemy")` |
| Namespace | Code organization | `namespace RGame { }` |

---

## Tóm Tắt

Tài liệu này cung cấp quick lookups cho Unity concepts. **Để có detailed explanations, tutorials, và examples**:

- **Complete Unity fundamentals**: Xem `00_Cac_Khai_Niem_Unity_Co_Ban.md`
- **Project-specific patterns**: Xem `01_Kien_Truc_Project.md`
- **How-to guides**: Xem `10_Huong_Dan_Thuc_Hanh.md`
- **Troubleshooting**: Xem `11_Xu_Ly_Su_Co.md`
- **Code examples**: Xem `13_Vi_Du_Code.md`

**Các Bước Tiếp Theo**:
- Mới với Unity? Bắt đầu với `00_BAT_DAU_TU_DAY.md`
- Cần specific examples? Kiểm tra `13_Vi_Du_Code.md`
- Bị stuck trên một vấn đề? Xem `11_Xu_Ly_Su_Co.md`

---

**Kết Thúc Tài Liệu**

<p align="center">
<strong>Lawn Defense: Monsters Out</strong><br>
Các Khái Niệm Unity Quan Trọng (Quick Reference)<br>
Essential Unity Concepts Quick Reference
</p>
