# Hướng Dẫn Xử Lý Sự Cố
## Các Vấn Đề Phổ Biến và Giải Pháp cho "Lawn Defense: Monsters Out"

**Phiên Bản Tài Liệu**: 1.0
**Cập Nhật Lần Cuối**: 2025-10-29
**Mức Độ**: Beginner to Intermediate
**Ngôn Ngữ**: Tiếng Việt (Vietnamese)

---

## Mục Lục

1. [Giới Thiệu](#giới-thiệu)
2. [Checklist Chẩn Đoán Nhanh](#checklist-chẩn-đoán-nhanh)
3. [Lỗi Compilation](#lỗi-compilation)
4. [Vấn Đề Di Chuyển](#vấn-đề-di-chuyển)
5. [Vấn Đề Bắn và Chiến Đấu](#vấn-đề-bắn-và-chiến-đấu)
6. [Vấn Đề Enemy AI](#vấn-đề-enemy-ai)
7. [Vấn Đề UI](#vấn-đề-ui)
8. [Vấn Đề Âm Thanh](#vấn-đề-âm-thanh)
9. [Vấn Đề Hiệu Năng](#vấn-đề-hiệu-năng)
10. [Lỗi Build](#lỗi-build)
11. [Kỹ Thuật Debug](#kỹ-thuật-debug)
12. [Xử Lý Sự Cố Nâng Cao](#xử-lý-sự-cố-nâng-cao)

---

## Giới Thiệu

Tài liệu này giúp bạn giải quyết các vấn đề phổ biến khi làm việc với "Lawn Defense: Monsters Out". Mỗi vấn đề bao gồm:

- **Symptom** (Triệu chứng): Điều bạn đang gặp phải
- **Common Causes** (Nguyên nhân phổ biến): Tại sao điều này xảy ra
- **Solution** (Giải pháp): Hướng dẫn sửa lỗi từng bước
- **Prevention** (Phòng ngừa): Cách tránh vấn đề này trong tương lai

**Cách Sử Dụng Tài Liệu Này**:
1. Xác định danh mục vấn đề của bạn (Movement, Combat, UI, v.v.)
2. Tìm triệu chứng khớp với vấn đề của bạn
3. Làm theo các bước giải pháp
4. Kiểm tra xem đã sửa được chưa
5. Đọc phần prevention để tránh lặp lại vấn đề

---

## Checklist Chẩn Đoán Nhanh

Trước khi đi vào các vấn đề cụ thể, hãy chạy qua checklist này:

### ✓ Kiểm Tra Cơ Bản
- [ ] Unity có đang ở Play Mode không? (Nhấn nút Play để test)
- [ ] Có lỗi trong Console không? (Window → General → Console)
- [ ] Tất cả các GameObject cần thiết có trong scene không?
- [ ] Tất cả các field trong Inspector đã được gán chưa (không có giá trị "None")?
- [ ] Scene đúng đã được load chưa?

### ✓ Kiểm Tra Component
- [ ] GameObject có tất cả các component cần thiết không?
- [ ] Các component đã được enable chưa? (Kiểm tra checkbox bên cạnh tên component)
- [ ] Layer settings đúng chưa?
- [ ] Tag settings đúng chưa?

### ✓ Kiểm Tra Code
- [ ] Bạn đã save script chưa? (Ctrl+S)
- [ ] Unity đã recompile chưa? (Kiểm tra góc dưới bên phải có icon quay tròn không)
- [ ] Có lỗi chính tả trong tên biến không?
- [ ] Bạn có đang sử dụng đúng namespace (RGame) không?

**Nếu tất cả kiểm tra đều pass nhưng vấn đề vẫn còn, tiếp tục đến các phần cụ thể bên dưới.**

---

## Lỗi Compilation

### Lỗi 1: "The name 'X' does not exist in the current context"

**Triệu chứng**: Lỗi màu đỏ trong Console, code không compile được, nút Play bị disable.

**Ví dụ Lỗi**:
```
Assets/Scripts/MyScript.cs(25,13): error CS0103: The name 'GameManager' does not exist in the current context
```

**Nguyên nhân phổ biến**:
1. Thiếu câu lệnh `using`
2. Lỗi chính tả trong tên class
3. Thiếu namespace

**Giải pháp**:

**Bước 1**: Kiểm tra xem bạn có thiếu namespace không
```csharp
// ❌ SAI - Thiếu namespace
using UnityEngine;

public class MyScript : MonoBehaviour
{
    void Start()
    {
        GameManager.Instance.Victory(); // LỖI!
    }
}

// ✅ ĐÚNG - Thêm namespace
using UnityEngine;
using RGame; // Thêm dòng này!

public class MyScript : MonoBehaviour
{
    void Start()
    {
        GameManager.Instance.Victory(); // Hoạt động!
    }
}
```

**Bước 2**: Nếu lỗi vẫn còn, kiểm tra chính tả tên class
```csharp
// Lỗi chính tả phổ biến:
GameManger     // ❌ Thiếu 'a'
gameManager    // ❌ Viết hoa sai
Game_Manager   // ❌ Gạch dưới thay vì không có khoảng trắng
GameManager    // ✅ Đúng
```

**Phòng ngừa**:
- Luôn include `using RGame;` ở đầu script
- Sử dụng autocomplete của Unity (Ctrl+Space) để tránh lỗi chính tả
- Giữ cửa sổ Console hiển thị để phát hiện lỗi ngay lập tức

---

### Lỗi 2: "NullReferenceException: Object reference not set to an instance of an object"

**Triệu chứng**: Game chạy nhưng crash với lỗi khi một đoạn code nhất định thực thi.

**Ví dụ Lỗi**:
```
NullReferenceException: Object reference not set to an instance of an object
Player_Archer.Update () (at Assets/Scripts/Player_Archer.cs:45)
```

**Nguyên nhân phổ biến**:
1. Field trong Inspector chưa được gán
2. GameObject không tìm thấy
3. Component bị thiếu

**Giải pháp**:

**Bước 1**: Kiểm tra dòng số được đề cập trong lỗi (dòng 45 trong ví dụ)

```csharp
// Dòng 45 - Dòng gây lỗi
healthBar.UpdateHealth(currentHealth); // ← Crash tại đây
```

**Bước 2**: Thêm null check
```csharp
// ❌ SAI - Không có kiểm tra an toàn
void Update()
{
    healthBar.UpdateHealth(currentHealth); // Crash nếu healthBar là null
}

// ✅ ĐÚNG - Thêm null check
void Update()
{
    if (healthBar != null)
    {
        healthBar.UpdateHealth(currentHealth);
    }
    else
    {
        Debug.LogError("HealthBar is not assigned!");
    }
}
```

**Bước 3**: Tìm xem tại sao biến lại null

**Trường hợp A**: Biến public chưa được gán trong Inspector
```csharp
public HealthBar healthBar; // ← Kiểm tra Inspector!
```
**Sửa**: Chọn GameObject → Trong Inspector, kéo object HealthBar vào field

**Trường hợp B**: GetComponent trả về null
```csharp
void Start()
{
    healthBar = GetComponent<HealthBar>(); // Trả về null nếu thiếu component

    if (healthBar == null)
    {
        Debug.LogError("HealthBar component is missing!");
    }
}
```
**Sửa**: Thêm component bị thiếu vào GameObject

**Trường hợp C**: FindObjectOfType trả về null
```csharp
void Start()
{
    gameManager = FindObjectOfType<GameManager>(); // Trả về null nếu không có trong scene

    if (gameManager == null)
    {
        Debug.LogError("GameManager not found in scene!");
    }
}
```
**Sửa**: Thêm GameManager vào scene

**Phòng ngừa**:
- Luôn gán các field trong Inspector trước khi play
- Sử dụng null check cho các thao tác GetComponent và Find
- Thêm thông báo Debug.LogError để xác định biến nào là null

---

### Lỗi 3: "MissingReferenceException: The object of type 'X' has been destroyed"

**Triệu chứng**: Lỗi xuất hiện khi cố gắng truy cập GameObject đã bị destroy.

**Nguyên nhân phổ biến**:
1. Cố gắng truy cập GameObject sau khi Destroy() được gọi
2. Coroutine vẫn chạy sau khi GameObject bị destroy

**Giải pháp**:

```csharp
// ❌ SAI - Truy cập object đã bị destroy
void OnEnemyDeath(Enemy enemy)
{
    Destroy(enemy.gameObject);

    // Lỗi - enemy đã bị destroy!
    Debug.Log(enemy.name); // LỖI!
}

// ✅ ĐÚNG - Lấy dữ liệu trước khi destroy
void OnEnemyDeath(Enemy enemy)
{
    string enemyName = enemy.name; // Lấy dữ liệu trước
    Destroy(enemy.gameObject);     // Sau đó destroy

    Debug.Log(enemyName); // Hoạt động!
}

// ✅ ĐÚNG - Kiểm tra nếu null
void Update()
{
    if (target != null) // Luôn kiểm tra trước
    {
        // Sử dụng target an toàn
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );
    }
}
```

**Cho Coroutines**:
```csharp
// ❌ SAI - Coroutine tiếp tục sau khi destroy
IEnumerator AttackSequence()
{
    yield return new WaitForSeconds(2f);
    // Nếu GameObject bị destroy trong lúc chờ, dòng này sẽ lỗi:
    animator.SetTrigger("Attack"); // LỖI!
}

// ✅ ĐÚNG - Kiểm tra nếu vẫn còn tồn tại
IEnumerator AttackSequence()
{
    yield return new WaitForSeconds(2f);

    // Kiểm tra GameObject này vẫn còn tồn tại
    if (this != null && gameObject != null)
    {
        animator.SetTrigger("Attack"); // An toàn
    }
}
```

**Phòng ngừa**:
- Lấy dữ liệu từ objects trước khi destroy chúng
- Thêm null check trong Update() và coroutines
- Dừng coroutines trước khi destroy objects: `StopAllCoroutines();`

---

### Lỗi 4: "Method 'X' does not contain a definition for 'Y'"

**Triệu chứng**: Cố gắng gọi một method không tồn tại.

**Ví dụ Lỗi**:
```
error CS1061: 'GameManager' does not contain a definition for 'GetScore'
```

**Nguyên nhân phổ biến**:
1. Lỗi chính tả tên method
2. Method là private (không thể truy cập từ script khác)
3. Sai class

**Giải pháp**:

**Bước 1**: Kiểm tra chính tả tên method
```csharp
// Lỗi chính tả phổ biến:
GameManager.Instance.Vicotry();  // ❌ Lỗi chính tả: Vicotry
GameManager.Instance.victory();  // ❌ Sai viết hoa: victory
GameManager.Instance.Victory();  // ✅ Đúng
```

**Bước 2**: Kiểm tra access level của method
```csharp
// Trong GameManager.cs
private void Victory() // ❌ Private - không thể truy cập từ script khác
{
    // ...
}

public void Victory() // ✅ Public - có thể truy cập mọi nơi
{
    // ...
}
```

**Bước 3**: Xác minh bạn đang gọi đúng class
```csharp
// ❌ SAI - Gọi sai manager
SoundManager.Instance.Victory(); // SoundManager không có Victory()

// ✅ ĐÚNG - Gọi đúng manager
GameManager.Instance.Victory(); // GameManager có Victory()
```

**Phòng ngừa**:
- Sử dụng autocomplete của Unity (Ctrl+Space) để xem các method có sẵn
- Đặt methods là public nếu cần gọi từ script khác
- Đọc kỹ thông báo lỗi - nó cho bạn biết class nào thiếu method

---

## Vấn Đề Di Chuyển

### Vấn đề 1: Player Không Di Chuyển

**Triệu chứng**: Nhấn phím WASD hoặc arrow keys không có tác dụng, player đứng yên.

**Nguyên nhân phổ biến**:
1. Input system chưa được thiết lập
2. Controller2D chưa được cấu hình
3. Code di chuyển bị disable
4. Vấn đề layer collision

**Giải pháp**:

**Bước chẩn đoán 1**: Kiểm tra xem input có được nhận không
```csharp
void Update()
{
    // Thêm dòng debug này tạm thời
    Debug.Log("Input: " + Input.GetAxisRaw("Horizontal"));

    // Code di chuyển bình thường của bạn...
}
```
**Chạy game**: Nếu Console hiển thị "Input: 0" luôn → Vấn đề input system (đi đến Bước 2)
**Nếu Console hiển thị "Input: -1" hoặc "Input: 1"** → Input hoạt động, vấn đề di chuyển (đi đến Bước 3)

**Bước 2**: Sửa Input System

Mở **Edit → Project Settings → Input Manager**

Xác minh các axis này tồn tại:
- **Horizontal**: Di chuyển Trái/Phải
  - Negative Button: `left` hoặc `a`
  - Positive Button: `right` hoặc `d`
- **Vertical**: Di chuyển Lên/Xuống
  - Negative Button: `down` hoặc `s`
  - Positive Button: `up` hoặc `w`

**Nếu dùng Input System mới** (InputSystem_Actions.inputactions):
```csharp
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private PlayerInput playerInput;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        if (playerInput == null)
        {
            Debug.LogError("PlayerInput component missing!");
        }
    }
}
```
**Sửa**: Thêm component PlayerInput vào player GameObject

**Bước 3**: Kiểm tra thiết lập Controller2D

Chọn Player GameObject → Inspector → Component Controller2D

Xác minh các setting này:
- **Collision Mask**: Phải bao gồm layer "Ground"
- **Horizontal Ray Count**: Tối thiểu 4
- **Vertical Ray Count**: Tối thiểu 4
- **Skin Width**: Khoảng 0.015

**Kiểm tra trực quan**:
```
[Controller2D Component]
┌─────────────────────────────┐
│ Collision Mask:             │
│  ☑ Default                  │ ← Phải check các layer
│  ☑ Ground                   │    player có thể va chạm
│  ☐ Enemy                    │
│                             │
│ Horizontal Ray Count: 4     │ ← Tối thiểu 4
│ Vertical Ray Count: 4       │ ← Tối thiểu 4
│ Skin Width: 0.015          │
└─────────────────────────────┘
```

**Bước 4**: Kiểm tra xem code di chuyển có được enable không

```csharp
public class Player_Archer : Enemy
{
    public bool allowMoveByPlayer = true; // ← Phải là true

    void Update()
    {
        if (!allowMoveByPlayer) // ← Nếu false, không thể di chuyển!
            return;

        // Code di chuyển...
    }
}
```

**Trong Inspector**: Đảm bảo "Allow Move By Player" được check ✓

**Bước 5**: Xác minh player không bị frozen

```csharp
void Update()
{
    // Nếu player bị frozen, không thể di chuyển
    if (canNotMoveByFreeze)
    {
        Debug.Log("Player is frozen!"); // Thêm debug này
        return;
    }
}
```

**Sửa**: Đợi hiệu ứng freeze kết thúc, hoặc reset: `canNotMoveByFreeze = false;`

**Phòng ngừa**:
- Sử dụng Debug.Log để xác định kiểm tra nào đang fail
- Test input system trong một empty scene đơn giản trước
- Giữ setting Controller2D nhất quán trên tất cả characters

---

### Vấn đề 2: Player Rơi Xuyên Sàn

**Triệu chứng**: Player rơi xuyên qua mặt đất thay vì đứng trên đó.

**Nguyên nhân phổ biến**:
1. Sai collision layers
2. Thiếu colliders
3. Controller2D mask không đúng

**Giải pháp**:

**Bước 1**: Kiểm tra Ground có collider

Chọn ground GameObject → Inspector

**Các component cần thiết**:
- ✓ **Collider2D** (BoxCollider2D, PolygonCollider2D, v.v.)
- ✓ **Layer**: Đặt thành "Ground"

```
[Ground GameObject]
┌──────────────────────┐
│ Tag: Untagged        │
│ Layer: Ground        │ ← Rất quan trọng!
├──────────────────────┤
│ Transform            │
│ Sprite Renderer      │
│ Box Collider 2D      │ ← Phải có collider
│  ☑ Is Trigger: NO    │ ← Phải BỎ CHECK
└──────────────────────┘
```

**Bước 2**: Xác minh collision mask của Controller2D của player

Chọn Player → Inspector → Controller2D

**Collision Mask** phải bao gồm "Ground":
```
Collision Mask:
☑ Ground       ← PHẢI được check
☐ Enemy        ← Thường không check
☑ Obstacle     ← Check nếu player phải va chạm với obstacles
```

**Bước 3**: Kiểm tra xem player có collider

Player phải có:
- ✓ Component Controller2D
- ✓ BoxCollider2D hoặc collider khác
- ✓ Layer đặt thành "Player" (KHÔNG phải "Ground")

**Bước 4**: Debug với Gizmos

Thêm đoạn này vào Controller2D.cs để hiển thị raycasts:
```csharp
void OnDrawGizmos()
{
    // Vẽ vertical raycasts
    Gizmos.color = Color.red;

    Vector2 rayOrigin = transform.position;
    float rayLength = 1f;

    // Vẽ ray hướng xuống
    Gizmos.DrawLine(rayOrigin, rayOrigin + Vector2.down * rayLength);
}
```

**Trong Scene view**: Bạn sẽ thấy các đường màu đỏ. Nếu chúng không chạm đất, player đang ở quá cao.

**Phòng ngừa**:
- Luôn đặt đúng layers cho GameObjects
- Kiểm tra kỹ collision matrix: **Edit → Project Settings → Physics 2D → Layer Collision Matrix**
- Đảm bảo layer "Player" có thể va chạm với layer "Ground" (checkbox phải ✓)

---

### Vấn đề 3: Player Bị Kẹt ở Tường hoặc Dốc

**Triệu chứng**: Player bị kẹt khi chạm tường, hoặc giật giật trên dốc.

**Nguyên nhân phổ biến**:
1. Skin width quá lớn
2. Quá ít raycasts
3. Vấn đề hình dạng collider

**Giải pháp**:

**Bước 1**: Điều chỉnh skin width của Controller2D

Chọn Player → Component Controller2D

```csharp
public float skinWidth = 0.015f; // ← Thử các giá trị từ 0.01 - 0.02
```

**Quá nhỏ** (< 0.01): Có thể bị kẹt trong tường
**Quá lớn** (> 0.03): Có thể lơ lửng trên mặt đất

**Bước 2**: Tăng số lượng raycast

```csharp
// Trong Inspector của Controller2D
Horizontal Ray Count: 6  // ← Tăng từ 4
Vertical Ray Count: 6    // ← Tăng từ 4
```

**Nhiều rays hơn = phát hiện va chạm mượt mà hơn**

**Biểu đồ trực quan**:
```
4 Rays (có thể bỏ sót dốc):     6 Rays (phát hiện tốt hơn):
    |                                 |
    |        ╱                         |      ╱
    |      ╱                           |    ╱
    |    ╱   ← Khoảng trống!           |  ╱ ← Không có khoảng trống
    |  ╱                               |╱
```

**Bước 3**: Sửa hình dạng collider

Chọn Player → BoxCollider2D

```
[Box Collider 2D]
┌────────────────────┐
│ Offset: 0, 0       │ ← Phải ở giữa
│ Size: 0.8, 1.8     │ ← Khớp với kích thước sprite
│                    │
│ [Edit Collider]    │ ← Click để điều chỉnh trực quan
└────────────────────┘
```

**Tips**:
- Làm collider **hơi nhỏ hơn** sprite (0.8 thay vì 1.0) để tránh bị kẹt góc
- Căn giữa collider trên sprite

**Bước 4**: Làm mượt di chuyển trên dốc

```csharp
// Trong code di chuyển Player
void Move(Vector2 velocity)
{
    // Thêm dòng này cho dốc mượt
    velocity.y = Mathf.Max(velocity.y, -maxFallSpeed);

    controller.Move(velocity * Time.deltaTime);
}
```

**Phòng ngừa**:
- Test di chuyển trên các loại địa hình khác nhau (phẳng, dốc, cầu thang)
- Giữ skinWidth nhất quán (0.015 hoạt động tốt cho hầu hết trường hợp)
- Sử dụng 6+ raycasts cho địa hình phức tạp

---

### Vấn đề 4: Player Di Chuyển Quá Nhanh hoặc Quá Chậm

**Triệu chứng**: Tốc độ di chuyển cảm giác không đúng.

**Giải pháp**:

**Bước 1**: Kiểm tra biến speed

Chọn Player → Inspector

```
[Player_Archer]
┌──────────────────────┐
│ Speed: 5             │ ← Điều chỉnh giá trị này
│                      │
│ Giá trị khuyến nghị: │
│ Walk: 3-5            │
│ Run: 7-10            │
│ Slow: 1-2            │
└──────────────────────┘
```

**Bước 2**: Xác minh việc sử dụng Time.deltaTime

```csharp
// ❌ SAI - Không có deltaTime (phụ thuộc framerate)
void Update()
{
    transform.position += new Vector3(speed, 0, 0); // Quá nhanh trên PC mạnh!
}

// ✅ ĐÚNG - Có deltaTime (độc lập framerate)
void Update()
{
    transform.position += new Vector3(speed, 0, 0) * Time.deltaTime;
}
```

**Bước 3**: Kiểm tra xem speed modifiers có đang active không

```csharp
void Update()
{
    float currentSpeed = speed;

    // Thêm debug để xem tốc độ thực tế
    Debug.Log("Current Speed: " + currentSpeed);

    // Kiểm tra modifiers
    if (canNotMoveByFreeze)
    {
        currentSpeed *= 0.5f; // Bị làm chậm bởi freeze
    }

    // Code di chuyển của bạn...
}
```

**Phòng ngừa**:
- Luôn sử dụng `Time.deltaTime` cho di chuyển
- Test tốc độ trên các framerate khác nhau (vsync bật/tắt)
- Ghi chú các giá trị speed khuyến nghị trong comments

---

## Vấn Đề Bắn và Chiến Đấu

### Vấn đề 5: Arrows/Projectiles Không Spawn

**Triệu chứng**: Nhấn nút tấn công không có tác dụng, không có arrows xuất hiện.

**Nguyên nhân phổ biến**:
1. Arrow prefab chưa được gán
2. Spawn point chưa được đặt
3. Cooldown chưa kết thúc
4. Không có ammo/mana

**Giải pháp**:

**Bước 1**: Kiểm tra gán prefab

Chọn Player → Inspector → Tìm shooting script

```
[Player_Archer]
┌──────────────────────────────┐
│ Arrow Prefab: None (Game...) │ ← Phải hiển thị arrow prefab
│ Spawn Point: None (Trans... │ ← Phải hiển thị spawn point
└──────────────────────────────┘
```

**Sửa**: Kéo arrow prefab từ Project vào field "Arrow Prefab"

**Bước 2**: Xác minh spawn point tồn tại

Kiểm tra hierarchy của player GameObject:
```
Player
├── Sprite
├── SpawnPoint  ← Phải có cái này!
└── ...
```

**Nếu thiếu**:
1. Right-click Player → Create Empty
2. Đặt tên "SpawnPoint"
3. Đặt vị trí phía trước player (X: 0.5, Y: 0, Z: 0)
4. Kéo SpawnPoint vào field "Spawn Point"

**Bước 3**: Kiểm tra attack cooldown

```csharp
public float attackCooldown = 1f;  // 1 giây giữa các phát bắn
private float lastAttackTime;

void Update()
{
    // Thêm debug
    if (Input.GetButtonDown("Fire1"))
    {
        float timeSinceLastShot = Time.time - lastAttackTime;
        Debug.Log("Time since last shot: " + timeSinceLastShot);

        if (timeSinceLastShot >= attackCooldown)
        {
            Shoot();
            lastAttackTime = Time.time;
        }
        else
        {
            Debug.Log("Cooldown not finished!");
        }
    }
}
```

**Nếu "Cooldown not finished!" xuất hiện liên tục**, giảm giá trị cooldown.

**Bước 4**: Kiểm tra xem animation tấn công có đang chạy

```csharp
void Shoot()
{
    // Đảm bảo animation không chặn việc spawn
    Debug.Log("Shoot() called");

    // Spawn ngay lập tức, không chờ animation
    GameObject arrow = Instantiate(arrowPrefab, spawnPoint.position, Quaternion.identity);
    Debug.Log("Arrow spawned: " + arrow.name);

    // Chạy animation sau
    animator.SetTrigger("Attack");
}
```

**Phòng ngừa**:
- Test gán prefab trước khi chạy
- Thêm Debug.Log để theo dõi luồng thực thi
- Đặt giá trị cooldown hợp lý (0.5-1 giây cho rapid fire, 2-3 cho heavy attacks)

---

### Vấn đề 6: Arrows Bắn Sai Hướng

**Triệu chứng**: Arrows bắn ngược, lên trên, hoặc theo hướng ngẫu nhiên.

**Nguyên nhân phổ biến**:
1. Sai hướng transform.right
2. Player sprite bị flip không đúng
3. Spawn point đặt vị trí sai

**Giải pháp**:

**Bước 1**: Hiển thị hướng bắn

```csharp
void Update()
{
    if (Input.GetButtonDown("Fire1"))
    {
        // Vẽ đường debug hiển thị hướng bắn
        Debug.DrawRay(
            spawnPoint.position,
            transform.right * 2f,  // ← Kiểm tra hướng này
            Color.green,
            2f
        );

        Shoot();
    }
}
```

**Trong Scene view khi chạy**: Bạn sẽ thấy đường màu xanh hiển thị hướng bắn.

**Bước 2**: Sửa hướng dựa trên player đang quay mặt

```csharp
void Shoot()
{
    GameObject arrow = Instantiate(arrowPrefab, spawnPoint.position, Quaternion.identity);
    Arrow arrowScript = arrow.GetComponent<Arrow>();

    // Xác định hướng dựa trên sprite flip
    Vector2 shootDirection;

    if (spriteRenderer.flipX) // Quay mặt trái
    {
        shootDirection = Vector2.left;
    }
    else // Quay mặt phải
    {
        shootDirection = Vector2.right;
    }

    Debug.Log("Shoot direction: " + shootDirection);
    arrowScript.SetDirection(shootDirection);
}
```

**Bước 3**: Kiểm tra vị trí spawn point

Chọn Player → SpawnPoint child object

```
[SpawnPoint Transform]
Position X:  0.5   ← Dương = phía trước khi quay mặt phải
Position Y:  0.3   ← Khớp với độ cao tay/cung của nhân vật
Position Z:  0
```

**Test cả hai hướng**:
- Khi quay mặt PHẢI: X phải dương (0.5)
- Khi quay mặt TRÁI: X phải âm (-0.5)

**Tự động điều chỉnh spawn point**:
```csharp
void Shoot()
{
    // Điều chỉnh spawn point dựa trên hướng quay mặt
    Vector3 spawnOffset = spriteRenderer.flipX
        ? new Vector3(-0.5f, 0.3f, 0)  // Trái
        : new Vector3(0.5f, 0.3f, 0);  // Phải

    Vector3 spawnPos = transform.position + spawnOffset;

    GameObject arrow = Instantiate(arrowPrefab, spawnPos, Quaternion.identity);
}
```

**Phòng ngừa**:
- Luôn test bắn theo cả hai hướng (trái và phải)
- Sử dụng Debug.DrawRay để hiển thị hướng
- Giữ logic spawn point nhất quán với sprite flipping

---

### Vấn đề 7: Auto-Aim Không Hoạt Động

**Triệu chứng**: Arrows phải ngắm enemy nhưng lại bắn thẳng.

**Nguyên nhân phổ biến**:
1. Không có enemy trong tầm
2. Layer mask chưa được đặt
3. Logic chọn target bị hỏng

**Giải pháp**:

**Bước 1**: Xác minh enemies trong tầm

```csharp
void FindTarget()
{
    // Thêm debug để xem tìm được gì
    Collider2D[] hits = Physics2D.OverlapCircleAll(
        transform.position,
        attackRange,
        enemyLayer
    );

    Debug.Log("Enemies found in range: " + hits.Length);

    foreach (var hit in hits)
    {
        Debug.Log("Found: " + hit.name);
    }
}

// Hiển thị tầm
void OnDrawGizmosSelected()
{
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(transform.position, attackRange);
}
```

**Trong Scene view**: Bạn sẽ thấy vòng tròn màu vàng. Enemies bên trong phải được phát hiện.

**Bước 2**: Kiểm tra enemy layer mask

Chọn Player → Inspector

```
[Player_Archer]
┌──────────────────────────┐
│ Attack Range: 10         │ ← Tăng nếu quá nhỏ
│ Enemy Layer: Enemy       │ ← Phải đặt thành "Enemy"
└──────────────────────────┘
```

Xác minh enemies có layer "Enemy":
Chọn enemy → Đầu Inspector → Layer: Enemy

**Bước 3**: Sửa logic chọn target

```csharp
public Transform FindNearestEnemy()
{
    Collider2D[] enemies = Physics2D.OverlapCircleAll(
        transform.position,
        attackRange,
        enemyLayer
    );

    if (enemies.Length == 0)
    {
        Debug.Log("No enemies in range");
        return null;
    }

    Transform nearest = null;
    float shortestDistance = Mathf.Infinity;

    foreach (var enemy in enemies)
    {
        float distance = Vector2.Distance(transform.position, enemy.transform.position);

        if (distance < shortestDistance)
        {
            shortestDistance = distance;
            nearest = enemy.transform;
        }
    }

    Debug.Log("Nearest enemy: " + (nearest != null ? nearest.name : "None"));
    return nearest;
}
```

**Bước 4**: Xác minh tính toán quỹ đạo

```csharp
void ShootAtTarget(Transform target)
{
    if (target == null)
    {
        Debug.LogWarning("No target for auto-aim!");
        ShootStraight(); // Fallback bắn thẳng
        return;
    }

    Vector2 direction = (target.position - spawnPoint.position).normalized;
    Debug.DrawRay(spawnPoint.position, direction * 5f, Color.red, 1f);

    GameObject arrow = Instantiate(arrowPrefab, spawnPoint.position, Quaternion.identity);
    arrow.GetComponent<Arrow>().SetDirection(direction);
}
```

**Phòng ngừa**:
- Sử dụng OnDrawGizmosSelected để hiển thị tầm tấn công
- Test với nhiều enemies ở các khoảng cách khác nhau
- Thêm hành vi fallback nếu không tìm thấy target

---

### Vấn đề 8: Damage Không Được Áp Dụng

**Triệu chứng**: Arrows trúng enemies nhưng không giảm máu.

**Nguyên nhân phổ biến**:
1. ICanTakeDamage chưa được implement
2. Giá trị damage đặt là 0
3. Collision/trigger chưa được thiết lập đúng

**Giải pháp**:

**Bước 1**: Kiểm tra arrow collision

Chọn Arrow prefab → Inspector

```
[Arrow]
┌──────────────────────────────┐
│ Rigidbody 2D                 │
│  Body Type: Dynamic          │
│                              │
│ Circle Collider 2D           │
│  ☑ Is Trigger: YES           │ ← Phải check cho OnTriggerEnter2D
│  Radius: 0.1                 │
└──────────────────────────────┘
```

**Bước 2**: Xác minh code phát hiện va chạm

```csharp
// Trong Arrow.cs
void OnTriggerEnter2D(Collider2D other)
{
    Debug.Log("Arrow hit: " + other.name);

    // Kiểm tra xem target có thể nhận damage không
    ICanTakeDamage damageable = other.GetComponent<ICanTakeDamage>();

    if (damageable != null)
    {
        Debug.Log("Applying " + damage + " damage to " + other.name);
        damageable.TakeDamage(damage, Vector2.zero, gameObject);
    }
    else
    {
        Debug.LogWarning(other.name + " cannot take damage (no ICanTakeDamage interface)");
    }

    Destroy(gameObject);
}
```

**Bước 3**: Đảm bảo enemy implement ICanTakeDamage

```csharp
// Enemy phải có này
public class Enemy : MonoBehaviour, ICanTakeDamage
{
    public void TakeDamage(float damage, Vector2 force, GameObject instigator)
    {
        Debug.Log(name + " taking " + damage + " damage");
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }
}
```

**Bước 4**: Kiểm tra giá trị damage

Chọn Arrow prefab → Inspector
```
[Arrow]
┌──────────────────────────┐
│ Damage: 10               │ ← Phải > 0
│ Speed: 15                │
└──────────────────────────┘
```

**Nếu damage là 0**, enemy sẽ không nhận damage dù mọi thứ khác hoạt động!

**Bước 5**: Xác minh layer collision matrix

**Edit → Project Settings → Physics 2D → Layer Collision Matrix**

Đảm bảo layer "Arrow" có thể va chạm với layer "Enemy" (checkbox ✓)

**Phòng ngừa**:
- Luôn thêm Debug.Log trong OnTriggerEnter2D để theo dõi collisions
- Test giá trị damage trong Inspector
- Kiểm tra kỹ việc implement interface

---

## Vấn Đề Enemy AI

### Vấn đề 9: Enemies Không Di Chuyển Về Phía Player

**Triệu chứng**: Enemies spawn nhưng đứng yên, không đi về phía pháo đài.

**Nguyên nhân phổ biến**:
1. State bị kẹt ở SPAWNING hoặc IDLE
2. Target chưa được đặt
3. Speed đặt là 0
4. Vấn đề Controller2D

**Giải pháp**:

**Bước 1**: Kiểm tra state của enemy

```csharp
// Trong SmartEnemyGrounded.cs
void Update()
{
    // Debug state hiện tại
    Debug.Log(name + " State: " + State);

    if (State == ENEMYSTATE.WALK)
    {
        // Enemy phải di chuyển ở đây
    }
}
```

**Chạy game**: Nếu Console hiển thị "State: SPAWNING" hoặc "State: IDLE" liên tục, state không đổi.

**Sửa chuyển state**:
```csharp
void Start()
{
    // Đảm bảo chuyển từ SPAWNING sang WALK
    StartCoroutine(SpawnCo());
}

IEnumerator SpawnCo()
{
    State = ENEMYSTATE.SPAWNING;
    yield return new WaitForSeconds(1f);

    State = ENEMYSTATE.WALK; // ← Đảm bảo dòng này chạy
    Debug.Log(name + " changed to WALK state");
}
```

**Bước 2**: Xác minh target được đặt

```csharp
void Update()
{
    if (target == null)
    {
        Debug.LogError(name + " has no target!");

        // Thử tìm target
        target = FindObjectOfType<TheFortrest>();

        if (target != null)
        {
            Debug.Log("Found target: " + target.name);
        }
    }
}
```

**Bước 3**: Kiểm tra tốc độ di chuyển

Chọn enemy → Inspector
```
[Enemy]
┌──────────────────────────┐
│ Speed: 3                 │ ← Phải > 0
│ State: WALK              │
└──────────────────────────┘
```

**Nếu speed là 0**, enemy sẽ không di chuyển!

**Bước 4**: Xác minh thiết lập Controller2D

Enemy cần:
- ✓ Component Controller2D
- ✓ Collision mask đặt thành "Ground"
- ✓ BoxCollider2D

**Bước 5**: Kiểm tra xem có bị frozen không

```csharp
void Update()
{
    if (canNotMoveByFreeze)
    {
        Debug.Log(name + " is frozen - can't move");
        return; // ← Dòng này dừng di chuyển!
    }

    if (State == ENEMYSTATE.WALK)
    {
        MoveTowardTarget();
    }
}
```

**Phòng ngừa**:
- Thêm state debug logging trong quá trình phát triển
- Test di chuyển enemy riêng lẻ (enemy đơn trong scene)
- Xác minh tất cả component cần thiết có mặt

---

### Vấn đề 10: Enemies Không Tấn Công

**Triệu chứng**: Enemies đến player/pháo đài nhưng không tấn công.

**Nguyên nhân phổ biến**:
1. Attack range quá nhỏ
2. Không có attack module gắn
3. State không chuyển sang ATTACK
4. Animation event không kích hoạt

**Giải pháp**:

**Bước 1**: Hiển thị attack range

```csharp
void OnDrawGizmosSelected()
{
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(transform.position, attackDistance);
}
```

**Trong Scene view**: Vòng tròn đỏ hiển thị tầm tấn công. Player/pháo đài phải ở bên trong để kích hoạt tấn công.

**Bước 2**: Tăng attack range

Chọn enemy → Inspector
```
[Enemy]
┌──────────────────────────┐
│ Attack Distance: 1.5     │ ← Tăng nếu quá nhỏ (thử 2-3)
└──────────────────────────┘
```

**Bước 3**: Kiểm tra attack module

Enemy phải có một trong các components này:
- EnemyMeleeAttack
- EnemyRangeAttack
- EnemyThrowAttack

**Nếu thiếu**:
1. Chọn enemy GameObject
2. Add Component → Scripts → (chọn loại tấn công)
3. Gán giá trị damage

**Bước 4**: Xác minh logic phát hiện tấn công

```csharp
void Update()
{
    if (State == ENEMYSTATE.WALK)
    {
        // Kiểm tra xem player có trong tầm tấn công không
        float distanceToTarget = Vector2.Distance(transform.position, target.position);
        Debug.Log("Distance to target: " + distanceToTarget + " / Attack Range: " + attackDistance);

        if (distanceToTarget <= attackDistance)
        {
            Debug.Log("In attack range - changing to ATTACK state");
            State = ENEMYSTATE.ATTACK;
        }
    }
}
```

**Bước 5**: Kiểm tra animation events

Mở enemy animation:
1. Window → Animation → Animation
2. Chọn enemy trong Hierarchy
3. Tìm animation clip "Attack"
4. Kiểm tra **Animation Event** (các dấu trắng trên timeline)

```
Attack Animation Timeline:
├─── Frame 0: Animation bắt đầu
├─── Frame 10: Animation Event "DealDamage()" ← Gọi code tấn công
└─── Frame 20: Animation kết thúc
```

**Nếu thiếu event**:
1. Kéo đến frame damage (thường giữa đòn)
2. Click nút "Add Event"
3. Chọn function: "DealDamage" hoặc tương tự

**Phòng ngừa**:
- Test attack range với visualization Gizmos
- Xác minh animation events đã được thiết lập
- Thêm debug logs để theo dõi chuyển state

---

### Vấn đề 11: Quá Nhiều Enemies Spawn

**Triệu chứng**: Game spawn hàng trăm enemies, làm choáng ngợp player.

**Nguyên nhân phổ biến**:
1. Vòng lặp spawn không dừng
2. Wave counter bị hỏng
3. Nhiều spawners active

**Giải pháp**:

**Bước 1**: Kiểm tra LevelEnemyManager

```csharp
IEnumerator SpawningProcess()
{
    int totalEnemiesSpawned = 0;

    foreach (var wave in waves)
    {
        Debug.Log("Spawning wave " + wave.name + " with " + wave.TotalEnemies + " enemies");

        for (int i = 0; i < wave.TotalEnemies; i++)
        {
            SpawnEnemy(wave.enemyPrefab);
            totalEnemiesSpawned++;

            Debug.Log("Total spawned so far: " + totalEnemiesSpawned);

            yield return new WaitForSeconds(wave.spawnInterval);
        }

        yield return new WaitForSeconds(wave.timeToNextWave);
    }

    Debug.Log("Finished spawning! Total: " + totalEnemiesSpawned);
}
```

**Bước 2**: Xác minh wave settings

Chọn LevelEnemyManager → Inspector → Waves array

```
Wave 0:
├─ Enemy Prefab: Goblin
├─ Total Enemies: 10      ← Phải hợp lý (5-20)
├─ Spawn Interval: 2      ← Thời gian giữa mỗi enemy (1-3 giây)
└─ Time To Next Wave: 5   ← Nghỉ giữa các waves
```

**Nếu Total Enemies là 999**, giảm xuống số hợp lý!

**Bước 3**: Kiểm tra có nhiều spawners không

**Cửa sổ Hierarchy**:
```
Scene
├── GameManager
├── LevelEnemyManager (1)  ← Chỉ nên có MỘT
├── LevelEnemyManager (2)  ← XÓA CÁC BẢN SAO!
└── ...
```

**Phòng ngừa**:
- Test cấu hình wave với số lượng nhỏ trước (3-5 enemies)
- Thêm debug logs để theo dõi spawn count
- Sử dụng singleton pattern cho managers để tránh duplicates

---

## Vấn Đề UI

### Vấn đề 12: Buttons Không Thể Click

**Triệu chứng**: Click UI buttons không có tác dụng.

**Nguyên nhân phổ biến**:
1. Không có EventSystem trong scene
2. Button thiếu OnClick event
3. Canvas raycast blocker
4. Button không interactable

**Giải pháp**:

**Bước 1**: Kiểm tra EventSystem

**Cửa sổ Hierarchy** phải có:
```
Scene
├── Canvas
├── EventSystem  ← Phải có!
└── ...
```

**Nếu thiếu**:
1. Right-click Hierarchy
2. UI → Event System

**Bước 2**: Xác minh thiết lập button

Chọn button → Inspector

```
[Button Component]
┌──────────────────────────────┐
│ ☑ Interactable: YES          │ ← Phải được check
│                              │
│ OnClick()                    │ ← Phải có event
│  Runtime                     │
│  MenuManager.PlayGame()      │ ← Function được gán
└──────────────────────────────┘
```

**Nếu "Interactable" bỏ check**, button sẽ không phản ứng với clicks!

**Bước 3**: Kiểm tra Canvas settings

Chọn Canvas → Inspector

```
[Canvas]
┌──────────────────────────────┐
│ Render Mode: Screen Space    │ ← Thường dùng cái này cho UI
│              Overlay         │
│                              │
│ [Graphic Raycaster]          │ ← Phải có component này
└──────────────────────────────┘
```

**Nếu thiếu Graphic Raycaster**:
Add Component → Event → Graphic Raycaster

**Bước 4**: Kiểm tra blocking elements

UI elements được vẽ theo thứ tự. Một blocker vô hình có thể ở phía trước:

```
Canvas
├── Background Panel
│   └── [Image với Raycast Target ✓]  ← Cái này chặn clicks!
└── Button
    └── (Không thể click được)
```

**Sửa**: Chọn blocking element → Component Image → **Bỏ check "Raycast Target"**

**Bước 5**: Xác minh vị trí Z

**Inspector → Transform**:
```
Button Transform:
Position Z: 0  ← Phải là 0 hoặc âm để ở phía trước
```

**Nếu Z > 0**, button có thể ở phía sau các UI elements khác!

**Phòng ngừa**:
- Luôn kiểm tra EventSystem khi tạo UI
- Test buttons ngay sau khi tạo chúng
- Disable "Raycast Target" trên các UI elements trang trí

---

### Vấn đề 13: Health Bar Không Update

**Triệu chứng**: Player nhận damage nhưng thanh máu vẫn đầy.

**Nguyên nhân phổ biến**:
1. Slider chưa được gán
2. Code update không chạy
3. Lerp speed quá chậm

**Giải pháp**:

**Bước 1**: Kiểm tra gán slider

Chọn HealthBarUI GameObject → Inspector

```
[UI_UI Script]
┌──────────────────────────────┐
│ Health Slider: None (Slider) │ ← Phải được gán!
└──────────────────────────────┘
```

**Sửa**: Kéo health slider object vào field này

**Bước 2**: Xác minh code update

```csharp
// Trong UI_UI.cs
void IPlayer()
{
    Debug.Log("Updating health UI");

    if (healthSlider != null)
    {
        healthValue = GameManager.Instance.Player.currentHealth /
                     GameManager.Instance.Player.maxHealth;
        Debug.Log("Health value: " + healthValue);
    }
    else
    {
        Debug.LogError("Health Slider is not assigned!");
    }
}

void Update()
{
    if (healthSlider != null)
    {
        healthSlider.value = Mathf.Lerp(
            healthSlider.value,
            healthValue,
            lerpSpeed * Time.deltaTime
        );
    }
}
```

**Bước 3**: Tăng lerp speed

Chọn health bar → Inspector
```
[UI_UI]
┌──────────────────────────────┐
│ Lerp Speed: 5                │ ← Tăng nếu quá chậm (thử 10-20)
└──────────────────────────────┘
```

**Quá chậm** (< 3): Thanh máu chậm hơn máu thực
**Quá nhanh** (> 50): Thanh máu nhảy ngay lập tức (không có animation mượt)

**Bước 4**: Kiểm tra IListener được đăng ký

```csharp
void Start()
{
    // Phải đăng ký là listener để nhận updates
    GameManager.Instance.listeners.Add(this);
    Debug.Log("UI registered as listener");
}
```

**Bước 5**: Xác minh player reference

```csharp
void Start()
{
    if (GameManager.Instance.Player == null)
    {
        Debug.LogError("GameManager has no Player reference!");
    }
    else
    {
        Debug.Log("Player found: " + GameManager.Instance.Player.name);
    }
}
```

**Phòng ngừa**:
- Test health UI bằng cách giảm player health thủ công trong Inspector
- Thêm debug logs để theo dõi khi IPlayer() được gọi
- Hiển thị giá trị health với Debug.Log

---

### Vấn đề 14: Victory/Fail Screen Không Hiện

**Triệu chứng**: Level kết thúc nhưng victory/fail screen không xuất hiện.

**Nguyên nhân phổ biến**:
1. Menu prefab chưa được gán
2. MenuManager không có trong scene
3. IListener chưa được đăng ký
4. Canvas sort order sai

**Giải pháp**:

**Bước 1**: Kiểm tra MenuManager tồn tại

**Hierarchy**:
```
Scene
├── MenuManager  ← Phải có
│   └── Menus
│       ├── VictoryMenu
│       └── FailMenu
└── ...
```

**Bước 2**: Xác minh gán menu

Chọn MenuManager → Inspector

```
[MenuManager]
┌──────────────────────────────┐
│ Menu Victory: None (Game...  │ ← Gán victory menu prefab
│ Menu Fail: None (GameObject) │ ← Gán fail menu prefab
└──────────────────────────────┘
```

**Bước 3**: Kiểm tra xem menus có được instantiated không

```csharp
// Trong MenuManager.cs
public void ISuccess()
{
    Debug.Log("Victory triggered!");

    if (menuVictory != null)
    {
        GameObject menu = Instantiate(menuVictory);
        Debug.Log("Victory menu spawned: " + menu.name);
    }
    else
    {
        Debug.LogError("Menu Victory prefab is not assigned!");
    }
}
```

**Bước 4**: Xác minh Canvas sort order

Victory menu có thể ở phía sau UI khác:

Chọn victory menu → Component Canvas

```
[Canvas]
┌──────────────────────────────┐
│ Override Sorting: ✓          │ ← Check cái này
│ Sort Order: 100              │ ← Số cao = ở phía trước
└──────────────────────────────┘
```

**Bước 5**: Kiểm tra game state

```csharp
// Trong GameManager.cs
public void Victory()
{
    Debug.Log("Victory() called");
    State = GameState.Success;

    Debug.Log("Notifying " + listeners.Count + " listeners");

    foreach (var item in listeners)
    {
        if (item != null)
        {
            item.ISuccess();
        }
    }
}
```

**Phòng ngừa**:
- Test victory/fail conditions sớm
- Thêm debug logs để theo dõi thay đổi game state
- Xác minh tất cả listeners được đăng ký trong Start()

---

## Vấn Đề Âm Thanh

### Vấn đề 15: Không Có Âm Thanh

**Triệu chứng**: Game chạy nhưng không có nhạc hoặc hiệu ứng âm thanh.

**Nguyên nhân phổ biến**:
1. Audio clips chưa được gán
2. Volume đặt là 0
3. Audio Listener thiếu
4. Muted trong Unity/System

**Giải pháp**:

**Bước 1**: Kiểm tra system volume

- **Unity Game view**: Tìm icon mute, đảm bảo không muted
- **System volume**: Kiểm tra Windows/Mac volume mixer
- **Headphones/Speakers**: Xác minh chúng được kết nối và bật

**Bước 2**: Xác minh Audio Listener tồn tại

**Phải có ĐÚNG MỘT Audio Listener trong scene** (thường trên Main Camera)

```
Main Camera
├── Transform
├── Camera
├── Audio Listener  ← Phải có cái này!
└── ...
```

**Nếu thiếu**:
Add Component → Audio → Audio Listener

**Nếu có nhiều listeners**, xóa các cái thừa (chỉ giữ một!)

**Bước 3**: Kiểm tra thiết lập SoundManager

Chọn SoundManager → Inspector

```
[SoundManager]
┌──────────────────────────────┐
│ Music Volume: 0.5            │ ← Phải > 0
│ Sound Volume: 1              │ ← Phải > 0
│                              │
│ Music Sources [2]            │ ← Phải có audio sources
│  Element 0: AudioSource      │
│  Element 1: AudioSource      │
│                              │
│ Sound Victory Panel:         │ ← Gán audio clips
│   None (Audio Clip)          │
└──────────────────────────────┘
```

**Bước 4**: Xác minh audio clips được gán

```csharp
// Trong SoundManager.cs
public static void PlaySfx(AudioClip sound)
{
    if (sound == null)
    {
        Debug.LogWarning("Trying to play null sound!");
        return;
    }

    Debug.Log("Playing sound: " + sound.name);
    Instance.soundSource.PlayOneShot(sound);
}
```

**Bước 5**: Kiểm tra audio import settings

Chọn audio file trong Project → Inspector

```
[Audio Clip Import Settings]
┌──────────────────────────────┐
│ Load Type: Decompress On     │ ← Thử các options khác
│            Load               │
│                              │
│ Preload Audio Data: ✓        │ ← Check cái này
│                              │
│ [Apply] [Revert]             │ ← Click Apply!
└──────────────────────────────┘
```

**Phòng ngừa**:
- Test audio ngay sau khi import
- Thêm debug logs vào tất cả lời gọi PlaySfx
- Giữ volume sliders hiển thị trong khi testing

---

### Vấn đề 16: Music Không Lặp

**Triệu chứng**: Nhạc nền chạy một lần rồi dừng.

**Giải pháp**:

**Bước 1**: Kiểm tra setting loop của AudioSource

Chọn SoundManager → Music Audio Source → Inspector

```
[Audio Source]
┌──────────────────────────────┐
│ Audio Clip: music GAME       │
│ ☑ Play On Awake              │
│ ☑ Loop                       │ ← PHẢI được check!
│ Volume: 0.5                  │
└──────────────────────────────┘
```

**Bước 2**: Xác minh trong code

```csharp
void PlayMusic(AudioClip music)
{
    musicSource.clip = music;
    musicSource.loop = true;  // ← Đảm bảo dòng này được đặt
    musicSource.Play();

    Debug.Log("Playing music: " + music.name + ", Loop: " + musicSource.loop);
}
```

**Phòng ngừa**:
- Luôn đặt `loop = true` cho nhạc nền
- Sử dụng AudioSource riêng cho music vs. sound effects

---

## Vấn Đề Hiệu Năng

### Vấn đề 17: FPS Thấp / Game Giật

**Triệu chứng**: Game chạy chậm, framerate giật.

**Nguyên nhân phổ biến**:
1. Quá nhiều GameObjects
2. Code không hiệu quả trong Update()
3. Quá nhiều particle effects
4. Sprites không được tối ưu

**Giải pháp**:

**Bước 1**: Kiểm tra FPS

Thêm FPS counter:
```csharp
public class FPSDisplay : MonoBehaviour
{
    private float deltaTime = 0f;

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.} FPS", fps);

        GUIStyle style = new GUIStyle();
        style.fontSize = 24;
        style.normal.textColor = fps < 30 ? Color.red : Color.green;

        GUI.Label(new Rect(10, 10, 100, 30), text, style);
    }
}
```

**Target FPS**:
- **> 60 FPS**: Xuất sắc
- **30-60 FPS**: Chấp nhận được
- **< 30 FPS**: Cần tối ưu!

**Bước 2**: Tối ưu lời gọi Update()

```csharp
// ❌ TỆ - FindObjectOfType mỗi frame!
void Update()
{
    GameManager gm = FindObjectOfType<GameManager>(); // CHẬM!
    // ...
}

// ✅ TỐT - Cache reference một lần
private GameManager gm;

void Start()
{
    gm = FindObjectOfType<GameManager>(); // Chỉ một lần
}

void Update()
{
    // Dùng cached reference
    if (gm.State == GameState.Playing)
    {
        // ...
    }
}
```

**Các thao tác tốn kém phổ biến cần tránh trong Update()**:
- ❌ FindObjectOfType / FindObjectsOfType
- ❌ GameObject.Find
- ❌ GetComponent (cache nó trong Start thay vì)
- ❌ String operations (dùng StringBuilder)

**Bước 3**: Giới hạn particle effects

Quá nhiều particles = lag

Chọn ParticleSystem → Inspector
```
[Particle System]
┌──────────────────────────────┐
│ Max Particles: 100           │ ← Giảm nếu lag (thử 50)
│ Emission Rate: 20            │ ← Thấp hơn = hiệu năng tốt hơn
└──────────────────────────────┘
```

**Bước 4**: Object pooling cho objects thường xuyên spawn

```csharp
// ❌ TỆ - Tạo/destroy liên tục
void Shoot()
{
    GameObject arrow = Instantiate(arrowPrefab); // Tạo garbage!
    // ...
    Destroy(arrow); // Thêm garbage!
}

// ✅ TỐT - Object pooling
public class ArrowPool : MonoBehaviour
{
    private Queue<GameObject> pool = new Queue<GameObject>();
    public GameObject arrowPrefab;
    public int poolSize = 20;

    void Start()
    {
        // Tạo trước arrows
        for (int i = 0; i < poolSize; i++)
        {
            GameObject arrow = Instantiate(arrowPrefab);
            arrow.SetActive(false);
            pool.Enqueue(arrow);
        }
    }

    public GameObject GetArrow()
    {
        if (pool.Count > 0)
        {
            GameObject arrow = pool.Dequeue();
            arrow.SetActive(true);
            return arrow;
        }

        // Pool hết, tạo mới
        return Instantiate(arrowPrefab);
    }

    public void ReturnArrow(GameObject arrow)
    {
        arrow.SetActive(false);
        pool.Enqueue(arrow);
    }
}
```

**Bước 5**: Tối ưu sprite settings

Chọn sprite → Inspector

```
[Sprite Import Settings]
┌──────────────────────────────┐
│ Texture Type: Sprite (2D)    │
│ Max Size: 2048               │ ← Giảm nếu quá lớn (thử 1024)
│ Compression: Automatic       │ ← Dùng compression!
│                              │
│ [Apply]                      │
└──────────────────────────────┘
```

**Phòng ngừa**:
- Profile thường xuyên (Window → Analysis → Profiler)
- Dùng object pooling cho bullets/enemies/effects
- Cache component references
- Test trên nền tảng target (mobile/PC)

---

### Vấn đề 18: Memory Leaks

**Triệu chứng**: Game sử dụng RAM ngày càng nhiều theo thời gian, cuối cùng crash.

**Nguyên nhân phổ biến**:
1. Không xóa listeners
2. Coroutines không dừng
3. Static references không được clear

**Giải pháp**:

**Bước 1**: Dọn dẹp listeners

```csharp
// ❌ TỆ - Listener không bao giờ bị xóa
void Start()
{
    GameManager.Instance.listeners.Add(this);
}

// ✅ TỐT - Xóa khi destroyed
void Start()
{
    GameManager.Instance.listeners.Add(this);
}

void OnDestroy()
{
    if (GameManager.Instance != null)
    {
        GameManager.Instance.listeners.Remove(this);
    }
}
```

**Bước 2**: Dừng coroutines đúng cách

```csharp
// ❌ TỆ - Coroutine tiếp tục chạy sau khi object destroyed
void Start()
{
    StartCoroutine(RepeatForever());
}

IEnumerator RepeatForever()
{
    while (true)
    {
        // Chạy mãi mãi, ngay cả sau khi GameObject destroyed!
        yield return new WaitForSeconds(1f);
    }
}

// ✅ TỐT - Dừng coroutine on destroy
private Coroutine repeatCoroutine;

void Start()
{
    repeatCoroutine = StartCoroutine(RepeatForever());
}

void OnDestroy()
{
    if (repeatCoroutine != null)
    {
        StopCoroutine(repeatCoroutine);
    }
}
```

**Bước 3**: Clear static references

```csharp
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        // ✅ Clear static reference
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
```

**Phòng ngừa**:
- Luôn implement OnDestroy() để dọn dẹp
- Dùng Profiler để monitor memory usage
- Tránh static references không cần thiết

---

## Lỗi Build

### Vấn đề 19: "Scene 'X' is not in Build Settings"

**Triệu chứng**: Game crash khi cố load một scene.

**Giải pháp**:

**Bước 1**: Mở Build Settings

**File → Build Settings**

**Bước 2**: Thêm scenes

```
Scenes In Build:
┌──────────────────────────────┐
│ ☑ 0  MainMenu                │ ← Tất cả scenes phải được liệt kê
│ ☑ 1  Level1                  │
│ ☑ 2  Level2                  │
│ ☐ 3  TestScene               │ ← Bỏ check test scenes
│                              │
│ [Add Open Scenes]            │ ← Click để thêm scene hiện tại
└──────────────────────────────┘
```

**Bước 3**: Xác minh code loading scene

```csharp
// ❌ SAI - Tên scene có thể sai
SceneManager.LoadScene("level1"); // Viết hoa sai!

// ✅ ĐÚNG - Dùng tên chính xác từ Build Settings
SceneManager.LoadScene("Level1");

// ✅ TỐT HƠN - Dùng scene index (nhanh hơn)
SceneManager.LoadScene(1);
```

**Phòng ngừa**:
- Thêm tất cả scenes vào Build Settings ngay sau khi tạo chúng
- Dùng scene indices thay vì names khi có thể

---

### Vấn đề 20: "Namespace 'RGame' could not be found" (Chỉ Build)

**Triệu chứng**: Game hoạt động trong Editor nhưng failed to build.

**Giải pháp**:

**Bước 1**: Kiểm tra code chỉ dành cho Editor

```csharp
// ❌ SAI - UnityEditor chỉ hoạt động trong Editor!
using UnityEditor; // Cái này phá vỡ builds!

public class MyScript : MonoBehaviour
{
    // ...
}

// ✅ ĐÚNG - Wrap editor code trong #if
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MyScript : MonoBehaviour
{
    // ...

    #if UNITY_EDITOR
    [MenuItem("Tools/My Tool")]
    static void MyTool()
    {
        // Code chỉ dành cho editor
    }
    #endif
}
```

**Bước 2**: Xác minh tất cả scripts compile

**Window → General → Console**

Clear console, sau đó thử build. Kiểm tra lỗi.

**Phòng ngừa**:
- Tránh namespace UnityEditor trong gameplay scripts
- Test builds thường xuyên, không chỉ Editor playmode

---

## Kỹ Thuật Debug

### Kỹ Thuật 1: Sử Dụng Debug.Log Hiệu Quả

**Logging cơ bản**:
```csharp
void Start()
{
    Debug.Log("Script started");
    Debug.LogWarning("This is a warning");
    Debug.LogError("This is an error");
}
```

**Contextual logging** (click log để chọn object):
```csharp
void Start()
{
    Debug.Log("Player health: " + currentHealth, this);
    //                                           ^^^^ Click log → chọn GameObject này
}
```

**Rich text formatting**:
```csharp
void Update()
{
    Debug.Log("<color=red>DANGER!</color> Health low: " + currentHealth);
    Debug.Log("<b>Bold text</b> and <i>italic text</i>");
    Debug.Log("<size=20>Large text</size>");
}
```

**Conditional logging**:
```csharp
// Chỉ log trong Development builds
void Update()
{
    if (Debug.isDebugBuild)
    {
        Debug.Log("Frame: " + Time.frameCount);
    }
}

// Custom debug flag
public class MyScript : MonoBehaviour
{
    public bool enableDebugLogs = true;

    void DebugLog(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log(message);
        }
    }
}
```

---

### Kỹ Thuật 2: Sử Dụng Debug.DrawRay và Debug.DrawLine

**Hiển thị raycasts**:
```csharp
void Update()
{
    Vector2 direction = transform.right;
    float distance = 5f;

    // Vẽ ray trong Scene view
    Debug.DrawRay(
        transform.position,
        direction * distance,
        Color.red,
        2f // Thời gian tồn tại tính bằng giây
    );

    // Thực hiện raycast thực tế
    RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance);

    if (hit.collider != null)
    {
        // Vẽ đường đến điểm trúng
        Debug.DrawLine(transform.position, hit.point, Color.green, 2f);
    }
}
```

**Hiển thị paths**:
```csharp
void ShowPath(List<Vector3> waypoints)
{
    for (int i = 0; i < waypoints.Count - 1; i++)
    {
        Debug.DrawLine(
            waypoints[i],
            waypoints[i + 1],
            Color.yellow,
            5f
        );
    }
}
```

---

### Kỹ Thuật 3: Sử Dụng Gizmos

**Vẽ shapes trong Scene view**:
```csharp
void OnDrawGizmos()
{
    // Luôn hiển thị
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(transform.position, attackRange);
}

void OnDrawGizmosSelected()
{
    // Chỉ khi GameObject được chọn
    Gizmos.color = Color.red;
    Gizmos.DrawWireCube(transform.position, new Vector3(1, 1, 0));

    // Vẽ đường đến target
    if (target != null)
    {
        Gizmos.DrawLine(transform.position, target.position);
    }
}
```

**Custom icons**:
```csharp
void OnDrawGizmos()
{
    // Vẽ icon tại vị trí GameObject
    Gizmos.DrawIcon(transform.position, "SpawnPoint.png", true);
}
```

---

### Kỹ Thuật 4: Sử Dụng Breakpoints và Unity Debugger

**Bước 1**: Đặt breakpoint

Trong code editor của bạn (Visual Studio / VS Code):
1. Click vào lề trái bên cạnh số dòng
2. Chấm đỏ xuất hiện = breakpoint đã đặt

**Bước 2**: Gắn Unity debugger

**Trong Visual Studio**:
- Debug → Attach to Unity

**Trong VS Code**:
- Cài extension "Debugger for Unity"
- Nhấn F5

**Bước 3**: Debug features

Khi breakpoint trúng:
- **Step Over (F10)**: Thực thi dòng hiện tại, chuyển sang dòng tiếp theo
- **Step Into (F11)**: Vào trong function calls
- **Step Out (Shift+F11)**: Thoát function hiện tại
- **Continue (F5)**: Chạy đến breakpoint tiếp theo

**Inspect variables**:
- Hover chuột lên biến để xem giá trị
- Thêm vào Watch window để theo dõi giá trị
- Sửa giá trị real-time!

---

### Kỹ Thuật 5: Unity Profiler

**Mở Profiler**:
**Window → Analysis → Profiler**

**Các vùng chính cần kiểm tra**:

**CPU Usage**:
- Hiển thị functions nào tốn nhiều thời gian nhất
- Tìm các spike (sử dụng cao đột ngột)
- Target: Dưới 16ms mỗi frame (60 FPS)

**Memory**:
- Total Allocated: Phải ổn định, không tăng liên tục
- GC Alloc: Garbage được tạo mỗi frame (thấp là tốt)

**Rendering**:
- SetPass Calls: Số lần thay đổi material (thấp là tốt)
- Batches: Draw calls (thấp là tốt)

**Cách sử dụng**:
1. Click nút "Record"
2. Chạy game
3. Tìm vùng màu đỏ (vấn đề hiệu năng)
4. Click spike để xem nguyên nhân
5. Tối ưu code đó

---

### Kỹ Thuật 6: Inspector Debugging

**Public variables cho live tuning**:
```csharp
public class Enemy : MonoBehaviour
{
    [Header("Debug")]
    public bool showDebugInfo = true;

    [Header("Stats - Có thể chỉnh khi Runtime")]
    public float speed = 3f;        // ← Có thể thay đổi khi đang chạy!
    public float health = 100f;

    void OnGUI()
    {
        if (showDebugInfo)
        {
            GUI.Label(new Rect(10, 10, 200, 20), "State: " + State);
            GUI.Label(new Rect(10, 30, 200, 20), "Health: " + health);
        }
    }
}
```

**Play Mode Tinting**:
**Edit → Preferences → Colors → Playmode tint**

Thay đổi màu để biết khi đang ở Play Mode (tránh chỉnh sửa nhầm)

**Lock Inspector**:
Click icon khóa (góc trên bên phải Inspector) → Inspector ở lại object được chọn ngay cả khi click nơi khác

---

## Xử Lý Sự Cố Nâng Cao

### Vấn đề Nâng Cao 1: Coroutine Ngừng Hoạt Động

**Triệu chứng**: Coroutine bắt đầu nhưng không hoàn thành.

**Nguyên nhân phổ biến**:
1. GameObject bị disable trong khi thực thi
2. Exception được throw
3. Điều kiện yield không bao giờ đạt được

**Giải pháp**:

**Thêm debug logs tại mỗi yield point**:
```csharp
IEnumerator MyCoroutine()
{
    Debug.Log("Coroutine started");

    yield return new WaitForSeconds(2f);
    Debug.Log("After 2 second wait"); // ← In ra không?

    yield return StartCoroutine(OtherCoroutine());
    Debug.Log("After OtherCoroutine"); // ← In ra không?

    Debug.Log("Coroutine finished");
}
```

**Kiểm tra xem GameObject có bị disabled không**:
```csharp
IEnumerator MyCoroutine()
{
    yield return new WaitForSeconds(5f);

    // Nếu GameObject disabled trong lúc chờ, dòng này không chạy!
    Debug.Log("After wait");
}

// Giải pháp: Kiểm tra xem vẫn active không
IEnumerator MyCoroutine()
{
    yield return new WaitForSeconds(5f);

    if (!gameObject.activeInHierarchy)
    {
        Debug.LogWarning("GameObject was disabled!");
        yield break; // Thoát coroutine
    }

    Debug.Log("After wait");
}
```

**Bắt exceptions**:
```csharp
IEnumerator MyCoroutine()
{
    try
    {
        yield return new WaitForSeconds(2f);

        // Dòng này có thể throw exception
        DoSomethingRisky();
    }
    catch (System.Exception e)
    {
        Debug.LogError("Coroutine exception: " + e.Message);
    }
}
```

---

### Vấn đề Nâng Cao 2: Vấn Đề Singleton Pattern

**Triệu chứng**: "Instance is null" hoặc nhiều instances tồn tại.

**Implement singleton chắc chắn**:
```csharp
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();

                if (_instance == null)
                {
                    Debug.LogError("GameManager not found in scene!");
                }
            }

            return _instance;
        }
    }

    void Awake()
    {
        // Đảm bảo chỉ có một instance tồn tại
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("Duplicate GameManager found! Destroying: " + name);
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject); // Optional: Giữ giữa các scenes
    }

    void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}
```

**An toàn khi sử dụng**:
```csharp
// ❌ SAI - Có thể null
void Start()
{
    GameManager.Instance.Victory(); // Crash nếu không có GameManager!
}

// ✅ ĐÚNG - Kiểm tra trước
void Start()
{
    if (GameManager.Instance != null)
    {
        GameManager.Instance.Victory();
    }
    else
    {
        Debug.LogError("GameManager not found!");
    }
}
```

---

### Vấn đề Nâng Cao 3: Physics Không Hoạt Động Như Mong Đợi

**Vấn đề phổ biến**:

**1. Collision không được phát hiện**:
```csharp
// Để collision detection hoạt động, bạn cần:
// ✓ Cả hai objects có Collider2D
// ✓ Ít nhất một có Rigidbody2D (hoặc dùng overlap checks)
// ✓ Layers có thể va chạm (Physics 2D settings)
// ✓ Objects trên cùng Z plane (hoặc đủ gần)

void OnCollisionEnter2D(Collision2D collision)
{
    Debug.Log("Collision detected: " + collision.gameObject.name);
}
```

**2. Trigger vs Collision**:
```csharp
// Dùng OnTriggerEnter2D khi:
// - Collider "Is Trigger" được check ✓
// - Objects phải đi xuyên qua nhau
// - Phát hiện zones (pickup items, damage areas)

void OnTriggerEnter2D(Collider2D other)
{
    Debug.Log("Trigger entered: " + other.name);
}

// Dùng OnCollisionEnter2D khi:
// - Collider "Is Trigger" không check
// - Objects phải chặn nhau vật lý
// - Cần collision force/impulse

void OnCollisionEnter2D(Collision2D collision)
{
    Debug.Log("Collision detected: " + collision.gameObject.name);
}
```

**3. Raycasts không trúng**:
```csharp
// Lỗi phổ biến:

// ❌ SAI - Dùng Physics2D.Raycast 3D trên objects 2D
RaycastHit hit;
Physics.Raycast(transform.position, Vector3.right, out hit);

// ✅ ĐÚNG - Dùng Physics2D cho games 2D
RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, 10f);

// ❌ SAI - Quên layer mask
RaycastHit2D hit = Physics2D.Raycast(start, direction); // Trúng mọi thứ!

// ✅ ĐÚNG - Dùng layer mask
int layerMask = 1 << LayerMask.NameToLayer("Enemy");
RaycastHit2D hit = Physics2D.Raycast(start, direction, distance, layerMask);
```

---

## Tóm Tắt

Tài liệu xử lý sự cố này bao gồm các vấn đề phổ biến nhất trong "Lawn Defense: Monsters Out":

**Tham Khảo Nhanh Theo Danh Mục**:
- **Compilation**: Vấn đề namespace, null references, method signatures
- **Movement**: Input system, thiết lập Controller2D, collision layers
- **Combat**: Gán prefab, phát hiện collision, damage system
- **Enemy AI**: State machine, targeting, attack range
- **UI**: EventSystem, thiết lập button, health bars, menus
- **Sound**: Audio Listener, volume, gán clip
- **Performance**: Tối ưu FPS, memory leaks, object pooling
- **Build**: Scene settings, vấn đề platform-specific

**Chiến Lược Debug Chung**:
1. **Xác định danh mục vấn đề** (Movement? UI? Combat?)
2. **Thêm Debug.Log statements** để theo dõi thực thi code
3. **Dùng visualizations** (Debug.DrawRay, Gizmos) để xem điều gì đang xảy ra
4. **Cô lập vấn đề** (test trong empty scene với setup tối thiểu)
5. **Kiểm tra giá trị Inspector** (biến có được gán không?)
6. **Xác minh thiết lập component** (tất cả component cần thiết có mặt không?)
7. **Test từng bước** (thêm một feature tại một thời điểm)

**Ghi Nhớ**:
- **Console là bạn của bạn** - đọc thông báo lỗi cẩn thận
- **Debug sớm, debug thường xuyên** - đừng đợi đến khi có gì đó hỏng
- **Test riêng lẻ** - loại bỏ độ phức tạp để tìm nguyên nhân gốc rễ
- **Save thường xuyên** - dùng version control (Git) để revert changes

**Tài Nguyên Bổ Sung**:
- Unity Manual: https://docs.unity3d.com/Manual/
- Unity Scripting API: https://docs.unity3d.com/ScriptReference/
- Unity Forums: https://forum.unity.com/
- Stack Overflow (Unity tag): https://stackoverflow.com/questions/tagged/unity3d

---

**Kết Thúc Tài Liệu**

Để xem thêm guides, tham khảo:
- **00_BAT_DAU_TU_DAY.md** - Lộ trình học tập và tổng quan
- **10_Huong_Dan_Thuc_Hanh.md** - Hướng dẫn từng bước để thêm features
- **12_Visual_Reference.md** - Sơ đồ và screenshots trực quan

Chúc bạn phát triển game thành công!
