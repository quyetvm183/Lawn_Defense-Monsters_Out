# Unity Fundamentals - Từ Zero Đến Hero

---
**🌐 Ngôn ngữ:** Tiếng Việt
**📄 File gốc:** [00_Unity_Fundamentals.md](00_Unity_Fundamentals.md)
**🔄 Cập nhật lần cuối:** 2025-01-XX
---

**Đối tượng độc giả:** Người mới hoàn toàn, chưa có kinh nghiệm Unity
**Yêu cầu trước:** Hiểu biết cơ bản về lập trình (biến, hàm)
**Thời gian đọc ước tính:** 2-3 giờ
**Tài liệu liên quan:** → `01_Kien_Truc_Project.md`, `99_Tu_Dien_Thuat_Ngu.md`

---

## Mục Lục
1. [Unity Là Gì?](#1-unity-là-gì)
2. [Giao Diện Unity Editor](#2-giao-diện-unity-editor)
3. [GameObject & Components](#3-gameobject--components)
4. [Prefabs (Template Objects)](#4-prefabs-template-objects)
5. [Scenes](#5-scenes)
6. [Scripting Cơ Bản](#6-scripting-cơ-bản)
7. [Input System](#7-input-system)
8. [Physics System](#8-physics-system)
9. [UI System (Canvas)](#9-ui-system-canvas)
10. [Resources & Asset Management](#10-resources--asset-management)
11. [Best Practices](#11-best-practices)

---

## 1. Unity Là Gì?

### 1.1 Tổng Quan Game Engine

**Unity** là một nền tảng phát triển game (gọi là "game engine") cung cấp công cụ và hệ thống để tạo ứng dụng tương tác 2D và 3D, chủ yếu là game. Hãy nghĩ về nó như một **bộ công cụ toàn diện** nơi bạn có thể:

- **Thiết kế thế giới game** một cách trực quan (giao diện kéo thả)
- **Viết game logic** sử dụng ngôn ngữ lập trình C#
- **Thêm vật lý** (trọng lực, va chạm, lực)
- **Tạo animations** (di chuyển nhân vật, chuyển cảnh UI)
- **Quản lý assets** (hình ảnh, âm thanh, 3D models)
- **Build cho nhiều nền tảng** (PC, Mobile, Console, Web)

**Ví dụ:** Unity giống như **Microsoft Word cho game** - Word giúp bạn tạo tài liệu, Unity giúp bạn tạo game.

### 1.2 Quy Trình Unity

Quy trình phát triển Unity điển hình:

```
1. TẠO (CREATE)
   ↓
Thiết kế scene → Thêm GameObjects → Attach Components
   ↓
2. LẬP TRÌNH (SCRIPT)
   ↓
Viết code C# → Attach scripts vào GameObjects → Định nghĩa hành vi
   ↓
3. TEST
   ↓
Nhấn nút Play → Test trong Game view → Debug vấn đề
   ↓
4. LẶP LẠI (ITERATE)
   ↓
Sửa bugs → Thêm tính năng → Cải thiện visuals
   ↓
5. BUILD
   ↓
Export ra nền tảng mục tiêu (Android, iOS, PC, etc.)
```

### 1.3 Thuật Ngữ Cốt Lõi

Trước khi đi sâu hơn, hãy hiểu các thuật ngữ cơ bản này:

| Thuật Ngữ | Giải Thích Đơn Giản |
|------|-------------------|
| **Scene** | Một màn chơi hoặc màn hình trong game (như "Main Menu" hoặc "Level 1") |
| **GameObject** | Bất kỳ object nào trong game (player, enemy, camera, button) |
| **Component** | Một phần chức năng được attach vào GameObject |
| **Prefab** | Template có thể tái sử dụng cho GameObjects |
| **Asset** | Bất kỳ file nào trong project (hình ảnh, âm thanh, script) |
| **Inspector** | Panel hiển thị properties của GameObject đã chọn |
| **Hierarchy** | Danh sách tất cả GameObjects trong scene hiện tại |

---

## 2. Giao Diện Unity Editor

Khi bạn mở Unity, bạn sẽ thấy nhiều panels. Hãy cùng phân tích từng panel:

### 2.1 Sơ Đồ Editor Layout

```
┌───────────────────────────────────────────────────────────────────────┐
│  Menu Bar: File  Edit  Assets  GameObject  Component  Window  Help    │
├─────────────────┬─────────────────────────────────────┬───────────────┤
│                 │                                     │               │
│   HIERARCHY     │         SCENE VIEW                  │   INSPECTOR   │
│                 │                                     │               │
│  Canvas         │   ┌─────────────────────────────┐   │  ┌─────────┐ │
│  ├─ Player      │   │                             │   │  │Transform│ │
│  ├─ Enemy       │   │    [Thế giới game trực quan]│   │  │         │ │
│  │  └─ Health   │   │                             │   │  │Position │ │
│  ├─ Camera      │   │    Kéo/chọn objects ở đây  │   │  │ X: 0    │ │
│  └─ Managers    │   │                             │   │  │ Y: 0    │ │
│                 │   └─────────────────────────────┘   │  │ Z: 0    │ │
│                 │                                     │  └─────────┘ │
├─────────────────┼─────────────────────────────────────┤               │
│                 │                                     │  Components: │
│   PROJECT       │         GAME VIEW                   │  - Rigidbody │
│                 │                                     │  - Collider  │
│  Assets/        │   ┌─────────────────────────────┐   │  - Script    │
│  ├─ Scenes      │   │                             │   │              │
│  ├─ Scripts     │   │  [Những gì player thấy]     │   │              │
│  ├─ Prefabs     │   │                             │   │              │
│  ├─ Sprites     │   │  (Nhấn Play để test)        │   │              │
│  └─ Audio       │   │                             │   │              │
│                 │   └─────────────────────────────┘   │              │
├─────────────────┴─────────────────────────────────────┴───────────────┤
│  CONSOLE - Debug messages, errors, warnings xuất hiện ở đây           │
│  ▶ "Player took damage: 10"                                           │
│  ⚠ "Warning: No AudioSource found"                                    │
│  ❌ "Error: NullReferenceException on line 42"                        │
└───────────────────────────────────────────────────────────────────────┘
```

### 2.2 Mô Tả Các Panel

#### **Hierarchy Panel** (Trên-Trái)
**Mục đích:** Hiển thị tất cả GameObjects trong scene hiện tại dạng cây

**Bạn sẽ làm gì ở đây:**
- Xem tất cả objects trong scene
- Tổ chức objects theo mối quan hệ cha-con
- Tạo objects mới (Right-click → Create)
- Xóa objects (Select → phím Delete)

**Ví dụ:**
```
Player (parent)
├─ PlayerSprite (con - phần hình ảnh)
├─ WeaponHolder (con - giữ vũ khí)
└─ HealthBar (con - hiển thị máu)
```

#### **Scene View** (Giữa-Trên)
**Mục đích:** Trình chỉnh sửa trực quan nơi bạn thiết kế thế giới game

**Bạn sẽ làm gì ở đây:**
- Kéo và đặt vị trí GameObjects
- Scale và xoay objects
- Di chuyển trong không gian 3D/2D
- Chọn objects để chỉnh sửa

**Điều khiển:**
- **Con lăn chuột:** Zoom in/out
- **Kéo chuột giữa:** Pan camera
- **Kéo chuột phải:** Xoay view (3D)
- **Q/W/E/R/T:** Chọn công cụ (Move, Rotate, Scale, etc.)

#### **Game View** (Giữa-Dưới)
**Mục đích:** Hiển thị những gì player sẽ thấy khi chơi

**Bạn sẽ làm gì ở đây:**
- Nhấn **nút Play** để test game
- Xem game thực tế khi chạy
- Test gameplay và mechanics
- **CẢNH BÁO:** Thay đổi trong chế độ Play sẽ mất khi bạn dừng!

#### **Inspector Panel** (Phải)
**Mục đích:** Hiển thị và chỉnh sửa properties của GameObject đã chọn

**Bạn sẽ làm gì ở đây:**
- Chỉnh sửa properties của GameObject (vị trí, scale, rotation)
- Thêm/xóa Components
- Điều chỉnh settings của Component
- Gán references giữa các objects

**Ví dụ Inspector cho Player GameObject:**
```
┌─────────────────────────────────────┐
│ GameObject: Player            [✓]   │ ← Checkbox Active
├─────────────────────────────────────┤
│ Tag: Player        Layer: Default   │ ← Nhận dạng
├─────────────────────────────────────┤
│ ▼ Transform                         │ ← Position/Rotation/Scale
│   Position  X: 0    Y: 0    Z: 0    │
│   Rotation  X: 0    Y: 0    Z: 0    │
│   Scale     X: 1    Y: 1    Z: 1    │
├─────────────────────────────────────┤
│ ▼ Sprite Renderer                   │ ← Hình ảnh hiển thị
│   Sprite: [PlayerImage]             │
│   Color: [White]                    │
├─────────────────────────────────────┤
│ ▼ Rigidbody 2D                      │ ← Vật lý
│   Mass: 1                           │
│   Gravity Scale: 0                  │
├─────────────────────────────────────┤
│ ▼ Box Collider 2D                   │ ← Va chạm
│   Size: X: 1    Y: 1                │
├─────────────────────────────────────┤
│ ▼ Player Controller (Script)        │ ← Hành vi tùy chỉnh
│   Move Speed: 5                     │
│   Jump Force: 10                    │
└─────────────────────────────────────┘
```

#### **Project Panel** (Dưới-Trái)
**Mục đích:** Trình duyệt file hiển thị tất cả assets trong project

**Bạn sẽ làm gì ở đây:**
- Duyệt folders và files
- Import assets mới (kéo files vào panel)
- Tạo assets mới (Right-click → Create)
- Tổ chức project files

**Cấu trúc điển hình:**
```
Assets/
├─ Scenes/          (files .unity - levels/menus)
├─ Scripts/         (files .cs - code C#)
├─ Prefabs/         (files .prefab - templates)
├─ Sprites/         (.png, .jpg - hình ảnh)
├─ Audio/           (.wav, .mp3 - âm thanh)
├─ Animations/      (.anim - animation clips)
└─ Resources/       (có thể load lúc runtime)
```

#### **Console Panel** (Dưới)
**Mục đích:** Hiển thị messages, warnings, và errors từ code

**Bạn sẽ làm gì ở đây:**
- Đọc error messages khi có gì đó bị lỗi
- Xem Debug.Log() messages từ scripts
- Theo dõi warnings về vấn đề tiềm ẩn
- Double-click errors để nhảy đến dòng code

**Các loại Message:**
- ℹ️ **Log:** Information messages (trắng)
- ⚠️ **Warning:** Vấn đề tiềm ẩn (vàng)
- ❌ **Error:** Lỗi code ngăn game chạy (đỏ)

---

## 3. GameObject & Components

Đây là **khái niệm quan trọng nhất** trong Unity. Mọi thứ trong game được xây dựng từ GameObjects và Components.

### 3.1 GameObject Là Gì?

**Định nghĩa:** GameObject là một **container** hoặc **hộp** chứa các Components. Bản thân GameObject không làm gì cả - nó chỉ là container rỗng với vị trí trong thế giới game.

**Ví dụ:** Hãy nghĩ GameObject như **vỏ điện thoại**:
- Vỏ bản thân chỉ là cái vỏ (GameObject)
- Bạn thêm chức năng bằng cách lắp các components (pin, camera, màn hình)
- Kết hợp khác nhau tạo thiết bị khác nhau

**Mỗi GameObject có:**
1. **Name:** Tên nhận dạng hiển thị trong Hierarchy ("Player", "Enemy", "Camera")
2. **Transform:** Position, Rotation, và Scale (LUÔN có)
3. **Tag:** Nhãn để nhận dạng ("Player", "Enemy", "Ground")
4. **Layer:** Dùng cho collision filtering và rendering

### 3.2 Hierarchy của GameObject

GameObjects có thể là **cha** và **con**, tạo thành cấu trúc cây:

```
Player (GameObject Cha)
├─ Transform         (X:0, Y:0, Z:0)  ← Vị trí cha
│
├─ Sprite            (GameObject Con)
│  └─ Transform      (X:0, Y:0, Z:0)  ← Tương đối với cha!
│
├─ WeaponHolder      (GameObject Con)
│  └─ Transform      (X:1, Y:0, Z:0)  ← 1 đơn vị bên phải cha
│     │
│     └─ Sword       (GameObject Cháu)
│        └─ Transform (X:0, Y:0, Z:0) ← Tương đối với WeaponHolder!
│
└─ HealthBar         (GameObject Con)
   └─ Transform      (X:0, Y:1, Z:0)  ← 1 đơn vị phía trên cha
```

**Quy tắc quan trọng:**
- **Vị trí con tương đối với cha** - Nếu cha di chuyển, con di chuyển theo
- **Nếu cha bị destroy, con cũng bị destroy**
- **Nếu cha bị disable, con cũng bị disable**
- Điều này hữu ích để tổ chức objects phức tạp (như nhân vật với vũ khí và UI)

### 3.3 Component Là Gì?

**Định nghĩa:** Component là một **phần chức năng** mà bạn attach vào GameObject để làm cho nó làm gì đó.

**Ví dụ:** Components giống như **ứng dụng trên điện thoại**:
- Điện thoại (GameObject) + Ứng dụng Camera (Component) = Chụp ảnh
- Điện thoại (GameObject) + Ứng dụng GPS (Component) = Định vị
- Điện thoại (GameObject) + Ứng dụng Nhạc (Component) = Phát nhạc

**Mỗi Component:**
- Phải được attach vào GameObject
- Có thể truy cập components khác trên cùng GameObject
- Có thể enable/disable độc lập
- Có properties bạn có thể điều chỉnh trong Inspector

### 3.4 Built-in Components

Unity cung cấp nhiều built-in components:

#### **Transform Component**
**Mục đích:** Định nghĩa vị trí, rotation, và scale
**Có trên:** Mọi GameObject (không thể xóa)

```
Transform
├─ Position: Object ở đâu (tọa độ X, Y, Z)
├─ Rotation: Object xoay như thế nào (góc X, Y, Z)
└─ Scale: Object to như thế nào (nhân X, Y, Z)
```

**Truy cập qua Code:**
```csharp
// Lấy vị trí
Vector3 pos = transform.position;

// Di chuyển object
transform.position = new Vector3(5, 0, 0);  // Di chuyển đến (5, 0, 0)
transform.position += new Vector3(1, 0, 0); // Di chuyển sang phải 1 đơn vị

// Xoay object
transform.Rotate(0, 0, 90);  // Xoay 90 độ trên trục Z

// Scale object
transform.localScale = new Vector3(2, 2, 1);  // To gấp 2 lần
```

#### **Renderer Components** (Làm object hiển thị)

**SpriteRenderer** - Cho hình ảnh 2D
```csharp
SpriteRenderer sr = GetComponent<SpriteRenderer>();
sr.sprite = mySprite;       // Thay đổi hình ảnh
sr.color = Color.red;       // Tô màu đỏ
sr.flipX = true;            // Lật ngang
sr.enabled = false;         // Làm ẩn
```

**MeshRenderer** - Cho 3D models (không dùng trong project 2D này)

#### **Collider Components** (Phát hiện va chạm)

**Mục đích:** Định nghĩa hình dạng va chạm của object

**BoxCollider2D** - Vùng va chạm hình chữ nhật (2D)
```csharp
BoxCollider2D collider = GetComponent<BoxCollider2D>();
collider.size = new Vector2(1, 2);      // Width: 1, Height: 2
collider.offset = new Vector2(0, 0.5);  // Dịch collision box
collider.isTrigger = true;              // Trigger thay vì solid collision
```

**CircleCollider2D** - Vùng va chạm hình tròn (2D)
```csharp
CircleCollider2D collider = GetComponent<CircleCollider2D>();
collider.radius = 0.5f;     // Bán kính hình tròn
collider.isTrigger = false; // Solid collision
```

**Trigger vs. Collider:**
- **Collider (isTrigger = false):** Vật lý, chặn di chuyển (tường, mặt đất)
- **Trigger (isTrigger = true):** Không vật lý, phát hiện overlap (vật phẩm, vùng cảm ứng)

#### **Rigidbody2D Component** (Thêm vật lý)

**Mục đích:** Làm GameObject phản ứng với vật lý (trọng lực, lực, vận tốc)

```csharp
Rigidbody2D rb = GetComponent<Rigidbody2D>();

// Di chuyển
rb.velocity = new Vector2(5, 0);        // Di chuyển phải ở 5 units/giây
rb.AddForce(new Vector2(10, 0));        // Apply lực (tăng tốc dần)

// Cấu hình
rb.gravityScale = 1;    // Trọng lực ảnh hưởng bao nhiêu (0 = không có trọng lực)
rb.mass = 1;            // Trọng lượng object
rb.drag = 0;            // Lực cản không khí (cao hơn = chậm hơn)
rb.bodyType = RigidbodyType2D.Dynamic;  // Dynamic, Kinematic, hoặc Static
rb.constraints = RigidbodyConstraints2D.FreezeRotation;  // Ngăn xoay
```

**Body Types:**
- **Dynamic:** Bị ảnh hưởng vật lý (rơi, va chạm, di chuyển)
- **Kinematic:** Không bị ảnh hưởng vật lý nhưng có thể di chuyển qua code
- **Static:** Không di chuyển (tường, mặt đất)

#### **Animator Component** (Điều khiển animations)

**Mục đích:** Phát animations và chuyển đổi giữa chúng

```csharp
Animator anim = GetComponent<Animator>();

// Trigger animations
anim.SetTrigger("Jump");        // Phát animation "Jump"
anim.SetBool("IsWalking", true); // Set boolean parameter
anim.SetFloat("Speed", 5.0f);   // Set float parameter
anim.SetInteger("Health", 100);  // Set integer parameter
```

#### **AudioSource Component** (Phát âm thanh)

**Mục đích:** Phát audio clips

```csharp
AudioSource audio = GetComponent<AudioSource>();

audio.clip = jumpSound;     // Gán sound clip
audio.Play();               // Phát âm thanh
audio.Stop();               // Dừng âm thanh
audio.volume = 0.5f;        // Âm lượng 50%
audio.loop = true;          // Lặp âm thanh
```

### 3.5 Script Components (Custom Components)

**Scripts** là custom components bạn tạo bằng C#. Chúng kế thừa từ **MonoBehaviour**.

**Tạo script component:**
1. Right-click trong Project → Create → C# Script → Đặt tên "PlayerController"
2. Double-click để mở trong code editor
3. Viết logic trong script
4. Kéo script vào GameObject trong Hierarchy hoặc add qua Inspector

**Cấu trúc Script Cơ Bản:**
```csharp
using UnityEngine;  // Import Unity functionality

public class PlayerController : MonoBehaviour  // Kế thừa từ MonoBehaviour
{
    // BIẾN (Properties hiển thị trong Inspector)
    public float speed = 5f;      // Có thể edit trong Inspector (public)
    private Rigidbody2D rb;       // Ẩn khỏi Inspector (private)

    // AWAKE - Được gọi khi GameObject được tạo (trước Start)
    void Awake()
    {
        // Khởi tạo references
        rb = GetComponent<Rigidbody2D>();
    }

    // START - Được gọi trước frame đầu tiên (sau tất cả Awakes)
    void Start()
    {
        // Khởi tạo trạng thái game
        Debug.Log("Player spawned!");
    }

    // UPDATE - Được gọi mỗi frame (~60 lần mỗi giây)
    void Update()
    {
        // Xử lý input và logic cần xảy ra mỗi frame
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space pressed!");
        }
    }

    // FIXEDUPDATE - Được gọi theo khoảng thời gian cố định (cho vật lý)
    void FixedUpdate()
    {
        // Xử lý tính toán vật lý
        float moveX = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveX * speed, rb.velocity.y);
    }
}
```

### 3.6 GameObject Lifecycle

Hiểu thứ tự Unity gọi các functions là rất quan trọng:

```
GameObject Created
      ↓
  Awake()           ← Khởi tạo object này (cache components)
      ↓
  OnEnable()        ← Gọi khi object được enable
      ↓
  Start()           ← Khởi tạo sau tất cả Awakes (truy cập objects khác)
      ↓
┌─────────────────┐
│  Update()       │ ← Mỗi frame (~60 FPS)
│  FixedUpdate()  │ ← Fixed timestep (50 FPS, cho physics)
│  LateUpdate()   │ ← Sau tất cả Updates (camera follow)
└────────┬────────┘
         │ (Lặp liên tục)
         ↓
  OnDisable()       ← Gọi khi object bị disable
      ↓
  OnDestroy()       ← Gọi khi object bị destroy
      ↓
GameObject Destroyed
```

**Khi nào dùng mỗi function:**

| Function | Use Case | Ví Dụ |
|----------|----------|---------|
| `Awake()` | Khởi tạo object này, cache components | `rb = GetComponent<Rigidbody2D>();` |
| `Start()` | Khởi tạo sau khi objects khác sẵn sàng | `player = GameObject.Find("Player");` |
| `Update()` | Logic theo frame (input, AI, timers) | Kiểm tra spacebar được nhấn |
| `FixedUpdate()` | Tính toán vật lý (timestep nhất quán) | Apply forces vào Rigidbody |
| `LateUpdate()` | Logic phụ thuộc tất cả Updates hoàn thành | Camera theo player |
| `OnDestroy()` | Cleanup khi object bị destroy | Lưu data, unsubscribe events |

### 3.7 Ví Dụ Component Communication

**Tình huống:** Player thu thập coin

```csharp
// Coin.cs - Script trên Coin GameObject
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int coinValue = 10;  // Giá trị coin này

    // Gọi khi collider khác chạm vào trigger này
    void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra xem object chạm ta có phải Player không
        if (other.gameObject.tag == "Player")
        {
            // Lấy component PlayerInventory từ Player
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();

            if (inventory != null)
            {
                // Thêm coins cho player
                inventory.AddCoins(coinValue);

                // Destroy coin này
                Destroy(gameObject);
            }
        }
    }
}

// PlayerInventory.cs - Script trên Player GameObject
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int coins = 0;  // Số coin hiện tại

    public void AddCoins(int amount)
    {
        coins += amount;
        Debug.Log("Coins: " + coins);
    }
}
```

**Điều gì xảy ra:**
1. Player GameObject chạm Coin GameObject (cả hai có Collider2D)
2. `OnTriggerEnter2D` được gọi trên Coin
3. Coin kiểm tra `other` có tag "Player" không
4. Coin lấy component `PlayerInventory` từ Player
5. Coin gọi method `AddCoins()`
6. Coin destroy chính nó

---

## 4. Prefabs (Template Objects)

### 4.1 Prefab Là Gì?

**Định nghĩa:** Prefab là **template có thể tái sử dụng** cho GameObjects. Hãy nghĩ về nó như blueprint hoặc khuôn bánh.

**Ví dụ:** Prefabs giống **bản vẽ thiết kế nhà**:
- Bản vẽ (Prefab) → Xây nhà (Instantiate)
- Thay đổi bản vẽ → Tất cả nhà trong tương lai dùng thiết kế mới
- Nhà hiện tại có thể cập nhật để match bản vẽ

**Tại sao dùng Prefabs?**
- ✅ **Tái sử dụng:** Tạo một lần, spawn nhiều lần
- ✅ **Nhất quán:** Tất cả instances có setup giống nhau
- ✅ **Cập nhật dễ:** Thay đổi prefab → tất cả instances cập nhật
- ✅ **Tổ chức:** Hierarchy gọn (spawn/destroy lúc runtime)

### 4.2 Tạo Prefab

**Phương pháp 1: Từ Scene GameObject**
1. Tạo GameObject trong scene (ví dụ: "Enemy")
2. Thêm components (SpriteRenderer, Collider, Scripts)
3. Kéo từ Hierarchy vào thư mục "Prefabs" trong Project panel
4. Giờ bạn có template tái sử dụng!

**Phương pháp 2: Tạo trực tiếp trong Project**
1. Right-click trong Project panel → Create → Prefab
2. Double-click để vào chế độ Prefab
3. Thêm/cấu hình GameObjects
4. Lưu và thoát chế độ Prefab

### 4.3 Sử Dụng Prefabs trong Code

**Spawning (Instantiating) Prefabs:**
```csharp
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // Gán prefab trong Inspector (kéo prefab vào đây)
    public GameObject enemyPrefab;

    void Start()
    {
        // Spawn enemy tại vị trí (5, 0, 0) không có rotation
        Vector3 spawnPos = new Vector3(5, 0, 0);
        Quaternion noRotation = Quaternion.identity;
        Instantiate(enemyPrefab, spawnPos, noRotation);

        // Spawn enemy với parent
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPos, noRotation);
        newEnemy.transform.parent = this.transform;  // Làm con của object này

        // Spawn và lấy reference để modify
        GameObject enemy = Instantiate(enemyPrefab, spawnPos, noRotation);
        enemy.name = "SpawnedEnemy";  // Đổi tên
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        enemyScript.health = 200;  // Modify sau khi spawn
    }
}
```

**Destroying Instantiated Objects:**
```csharp
// Destroy ngay lập tức
Destroy(gameObject);

// Destroy sau 2 giây
Destroy(gameObject, 2.0f);

// Destroy component cụ thể
Destroy(GetComponent<Rigidbody2D>());
```

### 4.4 Best Practices Cho Prefab

**NÊN:**
- ✅ Dùng prefabs cho mọi thứ spawn nhiều lần (enemies, bullets, pickups)
- ✅ Giữ prefabs trong folders có tổ chức (Prefabs/Enemies/, Prefabs/UI/)
- ✅ Dùng tên mô tả (Enemy_Goblin, Projectile_Arrow)
- ✅ Test prefabs bằng cách kéo vào scene tạm thời

**KHÔNG NÊN:**
- ❌ Tạo prefabs cho objects unique (main camera, game manager)
- ❌ Spawn hàng nghìn prefabs mỗi frame (dùng Object Pooling thay thế)
- ❌ Quên gán prefab references trong Inspector

---

## 5. Scenes

### 5.1 Scene Là Gì?

**Định nghĩa:** Scene là **container cho GameObjects** đại diện cho một level, menu, hoặc khu vực trong game.

**Ví dụ:** Scenes giống **chương trong sách** hoặc **phòng trong nhà**:
- Mỗi scene tự chứa
- Bạn có thể switch giữa scenes
- Scenes khác nhau có thể có nội dung khác nhau

**Các loại Scene phổ biến:**
- Main Menu scene (màn hình title, buttons)
- Gameplay scenes (Level 1, Level 2, Level 3)
- Game Over scene (kết quả, nút retry)
- Loading scene (thanh progress giữa levels)

### 5.2 Scene Management trong Code

**Loading Scenes:**
```csharp
using UnityEngine;
using UnityEngine.SceneManagement;  // Cần cho scene management

public class SceneLoader : MonoBehaviour
{
    // Load scene theo tên
    public void LoadLevel1()
    {
        SceneManager.LoadScene("Level1");
    }

    // Load scene theo index (build settings)
    public void LoadNextLevel()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene + 1);
    }

    // Reload scene hiện tại (restart level)
    public void RestartLevel()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    // Load scene asynchronously (với loading screen)
    public void LoadLevelAsync(string sceneName)
    {
        StartCoroutine(LoadAsync(sceneName));
    }

    IEnumerator LoadAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            float progress = operation.progress;  // 0 đến 1
            Debug.Log("Loading: " + (progress * 100) + "%");
            yield return null;  // Đợi một frame
        }
    }
}
```

### 5.3 Build Settings

Để scenes có thể load được, chúng phải được thêm vào **Build Settings**:
1. File → Build Settings
2. Kéo scenes từ Project vào list "Scenes in Build"
3. Scenes được gán số index (0, 1, 2...)

### 5.4 DontDestroyOnLoad

Mặc định, tất cả GameObjects bị destroy khi load scene mới. Để giữ object:

```csharp
using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Awake()
    {
        // Object này sống sót qua scene loads
        DontDestroyOnLoad(gameObject);
    }
}
```

**Use case:** Managers nên persist (AudioManager, GameManager, PlayerData)

---

## 6. Scripting Cơ Bản

### 6.1 MonoBehaviour Class

Tất cả Unity scripts kế thừa từ **MonoBehaviour**, cung cấp chức năng Unity:

```csharp
using UnityEngine;  // Import core functionality của Unity

// Phải match filename! (PlayerController.cs)
public class PlayerController : MonoBehaviour
{
    // Code của bạn ở đây
}
```

**MonoBehaviour cho bạn:**
- Lifecycle methods (Awake, Start, Update, etc.)
- Truy cập `gameObject`, `transform`, `name`
- Hỗ trợ Coroutine
- Truy cập Component system
- Unity event functions

### 6.2 Variables & Serialization

```csharp
public class ExampleScript : MonoBehaviour
{
    // PUBLIC - Hiển thị trong Inspector, scripts khác có thể truy cập
    public int health = 100;
    public float speed = 5.0f;
    public string playerName = "Hero";
    public GameObject target;

    // PRIVATE - Ẩn khỏi Inspector, chỉ script này truy cập
    private int secretValue = 42;
    private bool isAlive = true;

    // SERIALIZE PRIVATE - Ẩn khỏi scripts khác, hiển thị trong Inspector
    [SerializeField] private int coins = 0;

    // HIDE PUBLIC - Public cho scripts, ẩn khỏi Inspector
    [HideInInspector] public float internalTimer = 0;

    // HEADER - Tổ chức Inspector
    [Header("Movement Settings")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;

    [Header("Combat Settings")]
    public int damage = 10;
    public float attackRange = 2f;

    // TOOLTIP - Thêm mô tả hover trong Inspector
    [Tooltip("Bao nhiêu giây giữa các lần tấn công")]
    public float attackCooldown = 1.5f;

    // RANGE - Tạo slider trong Inspector
    [Range(0, 100)]
    public int volume = 50;
}
```

### 6.3 Common Patterns

#### **Getting Components**

```csharp
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;

    void Awake()
    {
        // Lấy component trên cùng GameObject
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        // Null check (quan trọng!)
        if (rb == null) {
            Debug.LogError("Rigidbody2D not found on " + gameObject.name);
        }
    }
}
```

**Tại sao cache trong Awake?**
- GetComponent() chậm, đừng gọi mỗi frame
- Cache một lần, dùng lại nhiều lần

#### **Finding GameObjects**

```csharp
public class Enemy : MonoBehaviour
{
    private GameObject player;
    private Transform playerTransform;

    void Start()
    {
        // Tìm theo tên (chậm, dùng ít thôi)
        player = GameObject.Find("Player");

        // Tìm theo tag (nhanh hơn, phương pháp ưu tiên)
        player = GameObject.FindGameObjectWithTag("Player");

        // Tìm theo type (tìm instance đầu tiên)
        GameManager gm = FindObjectOfType<GameManager>();

        // Truy cập trực tiếp transform
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }
}
```

**Best Practice:** Gán references trong Inspector khi có thể thay vì finding lúc runtime.

#### **Instantiate (Spawn Objects)**

```csharp
public class Spawner : MonoBehaviour
{
    public GameObject enemyPrefab;

    void SpawnEnemy()
    {
        // Spawn cơ bản
        Instantiate(enemyPrefab);

        // Spawn tại vị trí
        Vector3 spawnPos = new Vector3(5, 0, 0);
        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        // Spawn và giữ reference
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        newEnemy.name = "SpawnedEnemy";

        // Spawn làm con
        GameObject enemy = Instantiate(enemyPrefab, transform);
    }
}
```

#### **Destroy Objects**

```csharp
public class Coin : MonoBehaviour
{
    void OnCollect()
    {
        // Destroy ngay
        Destroy(gameObject);

        // Destroy sau delay (hữu ích cho particles/sounds)
        Destroy(gameObject, 2.0f);

        // Destroy component cụ thể
        Destroy(GetComponent<Collider2D>());

        // Disable thay vì destroy (có thể tái sử dụng)
        gameObject.SetActive(false);
    }
}
```

#### **Coroutines (Thực Thi Trì Hoãn)**

Coroutines cho phép bạn thực thi code theo thời gian hoặc với delays:

```csharp
public class Timer : MonoBehaviour
{
    void Start()
    {
        // Khởi động coroutine
        StartCoroutine(CountdownTimer());
        StartCoroutine(DelayedAction(3.0f));
    }

    // Coroutine phải return IEnumerator
    IEnumerator CountdownTimer()
    {
        for (int i = 3; i > 0; i--)
        {
            Debug.Log(i);
            yield return new WaitForSeconds(1.0f);  // Đợi 1 giây
        }
        Debug.Log("Go!");
    }

    IEnumerator DelayedAction(float delay)
    {
        Debug.Log("Starting...");
        yield return new WaitForSeconds(delay);
        Debug.Log("Finished after " + delay + " seconds!");
    }

    IEnumerator FadeOut()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color color = sr.color;

        // Fade trong 2 giây
        for (float t = 0; t < 2.0f; t += Time.deltaTime)
        {
            color.a = 1 - (t / 2.0f);  // Alpha từ 1 đến 0
            sr.color = color;
            yield return null;  // Đợi một frame
        }
    }

    // Dừng coroutine
    void StopCountdown()
    {
        StopCoroutine("CountdownTimer");
        // hoặc
        Coroutine c = StartCoroutine(CountdownTimer());
        StopCoroutine(c);
    }
}
```

**Yield Options:**
- `yield return null;` - Đợi một frame
- `yield return new WaitForSeconds(2.0f);` - Đợi 2 giây
- `yield return new WaitForFixedUpdate();` - Đợi physics update
- `yield return new WaitUntil(() => condition);` - Đợi đến khi điều kiện đúng

---

## 7. Input System

### 7.1 Keyboard Input

```csharp
public class InputExample : MonoBehaviour
{
    void Update()
    {
        // KIỂM TRA NẾU PHÍM ĐANG NHẤN (returns true mỗi frame khi giữ)
        if (Input.GetKey(KeyCode.W)) {
            Debug.Log("W đang được giữ");
        }

        // KIỂM TRA NẾU PHÍM VỪA ĐƯỢC NHẤN (returns true chỉ ở frame đầu tiên)
        if (Input.GetKeyDown(KeyCode.Space)) {
            Debug.Log("Space vừa được nhấn");
        }

        // KIỂM TRA NẾU PHÍM VỪA ĐƯỢC THẢ
        if (Input.GetKeyUp(KeyCode.Space)) {
            Debug.Log("Space vừa được thả");
        }

        // AXIS INPUT (returns -1 đến 1, mượt)
        float horizontal = Input.GetAxis("Horizontal");  // A/D hoặc mũi tên Trái/Phải
        float vertical = Input.GetAxis("Vertical");      // W/S hoặc mũi tên Lên/Xuống

        // RAW AXIS INPUT (returns -1, 0, hoặc 1, không làm mượt)
        float horizontalRaw = Input.GetAxisRaw("Horizontal");

        // BẤT KỲ PHÍM NÀO ĐƯỢC NHẤN
        if (Input.anyKeyDown) {
            Debug.Log("Có phím vừa được nhấn");
        }
    }
}
```

**KeyCodes Phổ Biến:**
- Phím mũi tên: `KeyCode.LeftArrow`, `KeyCode.RightArrow`, `KeyCode.UpArrow`, `KeyCode.DownArrow`
- WASD: `KeyCode.W`, `KeyCode.A`, `KeyCode.S`, `KeyCode.D`
- Số: `KeyCode.Alpha1`, `KeyCode.Alpha2`, etc.
- Function: `KeyCode.F1`, `KeyCode.F2`, etc.
- Modifiers: `KeyCode.LeftShift`, `KeyCode.LeftControl`, `KeyCode.LeftAlt`
- Khác: `KeyCode.Space`, `KeyCode.Return` (Enter), `KeyCode.Escape`

### 7.2 Mouse Input

```csharp
public class MouseExample : MonoBehaviour
{
    void Update()
    {
        // NÚT CHUỘT (0 = trái, 1 = phải, 2 = giữa)
        if (Input.GetMouseButton(0)) {
            Debug.Log("Chuột trái đang giữ");
        }

        if (Input.GetMouseButtonDown(0)) {
            Debug.Log("Chuột trái vừa click");
        }

        if (Input.GetMouseButtonUp(1)) {
            Debug.Log("Chuột phải vừa thả");
        }

        // VỊ TRÍ CHUỘT (tọa độ màn hình)
        Vector3 mousePos = Input.mousePosition;
        Debug.Log("Mouse: " + mousePos);

        // CHUYỂN ĐỔI SANG VỊ TRÍ THẾ GIỚI
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        worldPos.z = 0;  // Cho game 2D, set z = 0

        // CON LĂN CHUỘT
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0) {
            Debug.Log("Cuộn lên");
        } else if (scroll < 0) {
            Debug.Log("Cuộn xuống");
        }
    }
}
```

### 7.3 Touch Input (Mobile)

```csharp
public class TouchExample : MonoBehaviour
{
    void Update()
    {
        // SỐ LƯỢNG TOUCHES
        int touchCount = Input.touchCount;

        if (touchCount > 0)
        {
            // Lấy touch đầu tiên
            Touch touch = Input.GetTouch(0);

            // Vị trí touch
            Vector2 touchPos = touch.position;

            // Touch phase
            if (touch.phase == TouchPhase.Began) {
                Debug.Log("Touch bắt đầu");
            }
            else if (touch.phase == TouchPhase.Moved) {
                Debug.Log("Touch di chuyển");
            }
            else if (touch.phase == TouchPhase.Ended) {
                Debug.Log("Touch kết thúc");
            }
        }

        // MULTI-TOUCH
        if (Input.touchCount == 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            // Phát hiện pinch-to-zoom
            float prevDistance = (touch1.position - touch1.deltaPosition -
                                  touch2.position - touch2.deltaPosition).magnitude;
            float currentDistance = (touch1.position - touch2.position).magnitude;

            if (currentDistance > prevDistance) {
                Debug.Log("Pinch out (zoom in)");
            } else if (currentDistance < prevDistance) {
                Debug.Log("Pinch in (zoom out)");
            }
        }
    }
}
```

---

## 8. Physics System

Hệ thống vật lý của Unity xử lý va chạm, trọng lực, và lực.

### 8.1 Colliders (Collision Shapes)

Colliders định nghĩa ranh giới va chạm của GameObjects.

**Các loại Colliders (2D):**
- **BoxCollider2D** - Hình chữ nhật/vuông
- **CircleCollider2D** - Hình tròn
- **PolygonCollider2D** - Hình đa giác tùy chỉnh
- **EdgeCollider2D** - Đường/cạnh
- **CapsuleCollider2D** - Viên nang (chữ nhật bo tròn)

**Cấu hình:**
```csharp
BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
boxCollider.size = new Vector2(1, 2);       // Width x Height
boxCollider.offset = new Vector2(0, 0.5);   // Dịch từ center
boxCollider.isTrigger = false;              // Solid collision
```

### 8.2 Rigidbody2D (Physics Simulation)

Rigidbody2D thêm hành vi vật lý vào GameObjects.

**Cấu hình:**
```csharp
Rigidbody2D rb = GetComponent<Rigidbody2D>();

// Body Type
rb.bodyType = RigidbodyType2D.Dynamic;     // Bị ảnh hưởng vật lý
rb.bodyType = RigidbodyType2D.Kinematic;   // Di chuyển qua code, không vật lý
rb.bodyType = RigidbodyType2D.Static;      // Không di chuyển

// Properties
rb.mass = 1.0f;                // Trọng lượng
rb.gravityScale = 1.0f;        // 0 = không trọng lực, 1 = trọng lực bình thường
rb.drag = 0;                   // Linear damping (lực cản không khí)
rb.angularDrag = 0.05f;        // Rotation damping

// Constraints (đóng băng trục)
rb.constraints = RigidbodyConstraints2D.FreezeRotation;              // Không xoay
rb.constraints = RigidbodyConstraints2D.FreezePositionX;             // Không di chuyển X
rb.constraints = RigidbodyConstraints2D.FreezePosition;              // Không di chuyển
rb.constraints = RigidbodyConstraints2D.FreezeRotation |
                 RigidbodyConstraints2D.FreezePositionY;             // Kết hợp constraints

// Collision Detection
rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;  // Chính xác (cho objects nhanh)
rb.collisionDetectionMode = CollisionDetectionMode2D.Discrete;    // Nhanh (default)
```

### 8.3 Di Chuyển với Physics

```csharp
public class PhysicsMovement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 10f;
    private Rigidbody2D rb;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()  // Dùng FixedUpdate cho physics!
    {
        // PHƯƠNG PHÁP 1: Set velocity trực tiếp (thay đổi tốc độ ngay lập tức)
        float moveX = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveX * speed, rb.velocity.y);  // Giữ Y velocity

        // PHƯƠNG PHÁP 2: AddForce (tăng tốc dần)
        Vector2 force = new Vector2(moveX * speed, 0);
        rb.AddForce(force);

        // PHƯƠNG PHÁP 3: MovePosition (cho kinematic bodies)
        Vector2 movement = new Vector2(moveX, 0) * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        // NHẢY
        if (Input.GetKeyDown(KeyCode.Space)) {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
}
```

### 8.4 Collision Detection

**Hai loại va chạm:**

1. **Collision** (solid, chặn di chuyển)
```csharp
// Gọi khi collision bắt đầu
void OnCollisionEnter2D(Collision2D collision)
{
    Debug.Log("Hit: " + collision.gameObject.name);

    // Truy cập điểm va chạm
    Vector2 contactPoint = collision.contacts[0].point;

    // Truy cập collision normal (hướng)
    Vector2 normal = collision.contacts[0].normal;

    // Lấy component của object khác
    Enemy enemy = collision.gameObject.GetComponent<Enemy>();
    if (enemy != null) {
        enemy.TakeDamage(10);
    }
}

// Gọi mỗi frame khi đang va chạm
void OnCollisionStay2D(Collision2D collision)
{
    Debug.Log("Vẫn đang va chạm với: " + collision.gameObject.name);
}

// Gọi khi collision kết thúc
void OnCollisionExit2D(Collision2D collision)
{
    Debug.Log("Ngừng va chạm với: " + collision.gameObject.name);
}
```

2. **Trigger** (non-solid, phát hiện overlap)
```csharp
// Gọi khi vào trigger
void OnTriggerEnter2D(Collider2D other)
{
    Debug.Log("Vào trigger: " + other.gameObject.name);

    if (other.gameObject.tag == "Coin") {
        Destroy(other.gameObject);  // Thu thập coin
    }
}

// Gọi mỗi frame khi ở trong trigger
void OnTriggerStay2D(Collider2D other)
{
    Debug.Log("Trong trigger: " + other.gameObject.name);
}

// Gọi khi rời trigger
void OnTriggerExit2D(Collider2D other)
{
    Debug.Log("Rời trigger: " + other.gameObject.name);
}
```

**Collision Matrix (cái gì có thể va chạm):**
- Cần **ít nhất một Rigidbody2D** (trên một trong hai object)
- Cả hai objects phải có **Colliders**
- Colliders không được ở ignored layers

### 8.5 Raycasting (Line-of-Sight Detection)

Raycasting bắn tia vô hình để phát hiện objects.

```csharp
public class RaycastExample : MonoBehaviour
{
    public float rayDistance = 10f;
    public LayerMask targetLayer;

    void Update()
    {
        // RAYCAST CƠ BẢN
        Vector2 origin = transform.position;
        Vector2 direction = Vector2.right;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, rayDistance);

        if (hit.collider != null)
        {
            Debug.Log("Hit: " + hit.collider.gameObject.name);
            Debug.Log("Distance: " + hit.distance);
            Debug.Log("Point: " + hit.point);
        }

        // LAYERMASK RAYCAST (chỉ hit layers cụ thể)
        hit = Physics2D.Raycast(origin, direction, rayDistance, targetLayer);

        // RAYCAST VỚI TAG CHECK
        if (hit.collider != null && hit.collider.gameObject.tag == "Enemy")
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            enemy.TakeDamage(10);
        }

        // VISUALIZE RAYCAST (trong Scene view)
        Debug.DrawRay(origin, direction * rayDistance, Color.red);
    }

    // CIRCLECAST (raycast có độ rộng)
    void CircleCastExample()
    {
        Vector2 origin = transform.position;
        Vector2 direction = Vector2.right;
        float radius = 0.5f;

        RaycastHit2D hit = Physics2D.CircleCast(origin, radius, direction, rayDistance);

        if (hit.collider != null) {
            Debug.Log("CircleCast hit: " + hit.collider.gameObject.name);
        }
    }

    // OVERLAP DETECTION (kiểm tra vùng)
    void OverlapCircleExample()
    {
        Vector2 center = transform.position;
        float radius = 2.0f;

        // Lấy tất cả colliders trong bán kính
        Collider2D[] colliders = Physics2D.OverlapCircleAll(center, radius);

        foreach (Collider2D col in colliders)
        {
            Debug.Log("Trong range: " + col.gameObject.name);
        }

        // Visualize
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, radius);
    }
}
```

---

## 9. UI System (Canvas)

UI system của Unity dùng **Canvas** để hiển thị các phần tử giao diện 2D.

### 9.1 Canvas Setup

**Canvas** là root container cho tất cả UI elements.

**Canvas Render Modes:**
1. **Screen Space - Overlay:** UI vẽ trên mọi thứ (phổ biến nhất)
2. **Screen Space - Camera:** UI được render bởi camera cụ thể (cho hiệu ứng 3D)
3. **World Space:** UI tồn tại trong thế giới 3D (health bars trên nhân vật)

**Tạo UI:**
1. Right-click Hierarchy → UI → Canvas (tạo Canvas + EventSystem)
2. Thêm UI elements là con của Canvas
3. Dùng **RectTransform** thay vì Transform để positioning

### 9.2 Common UI Components

#### **Text (Khuyên dùng TextMeshPro)**
```csharp
using TMPro;  // TextMeshPro namespace

public class UIExample : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    void UpdateScore(int score)
    {
        scoreText.text = "Score: " + score;
        scoreText.color = Color.yellow;
        scoreText.fontSize = 24;
    }
}
```

#### **Button**
```csharp
using UnityEngine;
using UnityEngine.UI;

public class ButtonExample : MonoBehaviour
{
    public Button playButton;

    void Start()
    {
        // Thêm listener qua code
        playButton.onClick.AddListener(OnPlayClicked);
    }

    void OnPlayClicked()
    {
        Debug.Log("Play button clicked!");
    }

    // Hoặc gán trong Inspector:
    // 1. Select Button trong Hierarchy
    // 2. Trong Inspector, tìm OnClick() list
    // 3. Click +, kéo object có script
    // 4. Chọn function từ dropdown
}
```

#### **Slider**
```csharp
using UnityEngine.UI;

public class SliderExample : MonoBehaviour
{
    public Slider healthSlider;

    void SetHealth(int current, int max)
    {
        healthSlider.maxValue = max;
        healthSlider.value = current;

        // Phần trăm (0-1)
        healthSlider.value = (float)current / max;
    }

    void Start()
    {
        // Lắng nghe thay đổi slider
        healthSlider.onValueChanged.AddListener(OnHealthChanged);
    }

    void OnHealthChanged(float value)
    {
        Debug.Log("Health slider: " + value);
    }
}
```

#### **Image**
```csharp
using UnityEngine.UI;

public class ImageExample : MonoBehaviour
{
    public Image characterPortrait;
    public Sprite newSprite;

    void ChangeImage()
    {
        characterPortrait.sprite = newSprite;
        characterPortrait.color = Color.red;       // Tô màu
        characterPortrait.fillAmount = 0.5f;       // Fill (cho radial/filled images)
        characterPortrait.enabled = false;         // Ẩn
    }
}
```

### 9.3 Anchors & RectTransform

**Anchors** điều khiển cách UI elements scale và position tương đối với kích thước màn hình.

```
┌──────────────────────────────────┐
│  Canvas (Screen)                 │
│                                  │
│  ┌────────┐    ← Anchor: Top-Left
│  │ Button │      Ở góc
│  └────────┘                      │
│                                  │
│             [Button]             │ ← Anchor: Center
│         (ở giữa màn hình)        │   Ở giữa
│                                  │
│                        ┌────────┐│ ← Anchor: Bottom-Right
│                        │ Button ││   Ở góc
│                        └────────┘│
└──────────────────────────────────┘
```

**Common Anchor Presets:**
- Top-Left, Top-Center, Top-Right
- Middle-Left, Middle-Center, Middle-Right
- Bottom-Left, Bottom-Center, Bottom-Right
- Stretch (mở rộng theo màn hình)

**Setting Anchors trong Code:**
```csharp
RectTransform rectTransform = GetComponent<RectTransform>();

// Set anchored position
rectTransform.anchoredPosition = new Vector2(100, 50);

// Set size
rectTransform.sizeDelta = new Vector2(200, 100);

// Set anchors (0-1 range)
rectTransform.anchorMin = new Vector2(0.5f, 0.5f);  // Center
rectTransform.anchorMax = new Vector2(0.5f, 0.5f);  // Center
```

---

## 10. Resources & Asset Management

### 10.1 Resources Folder

Thư mục **Resources** cho phép loading assets lúc runtime.

**Cấu trúc:**
```
Assets/
└── Resources/          ← Tên thư mục đặc biệt
    ├── Prefabs/
    │   └── Enemy.prefab
    ├── Sprites/
    │   └── Icon.png
    └── Audio/
        └── Music.mp3
```

**Loading Resources:**
```csharp
public class ResourceLoader : MonoBehaviour
{
    void Start()
    {
        // Load prefab
        GameObject enemyPrefab = Resources.Load<GameObject>("Prefabs/Enemy");
        Instantiate(enemyPrefab);

        // Load sprite
        Sprite icon = Resources.Load<Sprite>("Sprites/Icon");

        // Load audio
        AudioClip music = Resources.Load<AudioClip>("Audio/Music");

        // Load tất cả assets theo type
        GameObject[] allPrefabs = Resources.LoadAll<GameObject>("Prefabs");
    }
}
```

**⚠️ Cảnh báo:** Đừng đặt mọi thứ vào Resources - nó tăng build size và memory. Chỉ dùng cho assets được load động.

### 10.2 Asset References

**Best Practice:** Gán references trong Inspector khi có thể.

```csharp
public class AssetExample : MonoBehaviour
{
    // PHƯƠNG PHÁP 1: Inspector Reference (TỐT NHẤT)
    public GameObject enemyPrefab;      // Kéo prefab vào đây trong Inspector
    public Sprite playerSprite;         // Kéo sprite vào đây
    public AudioClip jumpSound;         // Kéo audio clip vào đây

    void Start()
    {
        // Assets đã được load, sẵn sàng dùng
        Instantiate(enemyPrefab);
    }
}
```

---

## 11. Best Practices

### 11.1 Performance Optimization

**NÊN:**
- ✅ **Cache GetComponent calls** trong Awake/Start
```csharp
// TỐT
private Rigidbody2D rb;
void Awake() { rb = GetComponent<Rigidbody2D>(); }
void Update() { rb.velocity = ...; }

// TỆ (gọi GetComponent mỗi frame)
void Update() { GetComponent<Rigidbody2D>().velocity = ...; }
```

- ✅ **Dùng Object Pooling** cho objects spawn thường xuyên
```csharp
// Thay vì Instantiate/Destroy mỗi frame
// Tái sử dụng objects từ pool
```

- ✅ **Tránh Update() cho logic không phụ thuộc frame**
```csharp
// Dùng events, coroutines, hoặc InvokeRepeating thay thế
InvokeRepeating("CheckEnemies", 0, 0.5f);  // Mỗi 0.5 giây
```

- ✅ **Dùng FixedUpdate() cho physics**
```csharp
void FixedUpdate() { rb.AddForce(...); }  // Physics nhất quán
```

- ✅ **Minimize garbage collection**
```csharp
// TỐT - tái sử dụng Vector3
private Vector3 movement = Vector3.zero;
void Update() {
    movement.x = Input.GetAxis("Horizontal");
    transform.Translate(movement);
}

// TỆ - tạo Vector3 mới mỗi frame
void Update() {
    transform.Translate(new Vector3(Input.GetAxis("Horizontal"), 0, 0));
}
```

**KHÔNG NÊN:**
- ❌ **Đừng dùng Find/FindObjectOfType trong Update()**
```csharp
// TỆ - rất chậm
void Update() {
    GameObject player = GameObject.Find("Player");  // ĐỪNG LÀM NHƯ NÀY
}

// TỐT - tìm một lần
private GameObject player;
void Start() {
    player = GameObject.Find("Player");
}
```

- ❌ **Đừng spam Instantiate/Destroy**
- ❌ **Đừng dùng Camera.main trong loops** (cache nó)
- ❌ **Đừng bỏ qua compiler warnings**

### 11.2 Code Organization

**Naming Conventions:**
```csharp
public class PlayerController : MonoBehaviour  // PascalCase cho classes
{
    public int MaxHealth = 100;        // PascalCase cho public fields
    private float moveSpeed = 5f;      // camelCase cho private fields

    const int MAX_ENEMIES = 50;        // UPPER_CASE cho constants

    public void TakeDamage(int amount) // PascalCase cho methods
    {
        // ...
    }

    private void UpdateHealth()        // PascalCase cho methods
    {
        // ...
    }
}
```

**Folder Organization:**
```
Assets/
├── _YourGameName/         ← Thư mục game chính (underscore sort lên trên)
│   ├── Scenes/
│   ├── Scripts/
│   │   ├── Player/
│   │   ├── Enemies/
│   │   ├── Managers/
│   │   └── UI/
│   ├── Prefabs/
│   ├── Sprites/
│   └── Audio/
└── Plugins/               ← Assets của bên thứ ba
```

### 11.3 Debugging Tips

**Debug.Log Variations:**
```csharp
Debug.Log("Normal message");         // Trắng
Debug.LogWarning("Warning!");        // Vàng
Debug.LogError("Error occurred!");   // Đỏ

// Log với context (click để highlight object)
Debug.Log("Message", gameObject);

// Conditional logging
if (debugMode) Debug.Log("Debug info");
```

**Debug.DrawRay để Visualize:**
```csharp
void Update()
{
    // Vẽ đường trong Scene view (không phải Game view)
    Debug.DrawRay(transform.position, Vector3.forward * 10, Color.red);
    Debug.DrawLine(transform.position, targetPosition, Color.green);
}
```

**Gizmos để Visualization trong Editor:**
```csharp
void OnDrawGizmos()
{
    // Luôn hiển thị
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(transform.position, 2f);
}

void OnDrawGizmosSelected()
{
    // Chỉ khi object được chọn
    Gizmos.color = Color.red;
    Gizmos.DrawSphere(transform.position, 0.5f);
}
```

**Break Points:**
- Set breakpoints trong IDE (Visual Studio/Rider)
- Attach Unity debugger để inspect variables trong chế độ Play

---

## 12. Tiếp Theo Là Gì?

Chúc mừng! Bạn giờ đã hiểu Unity fundamentals. Đây là các bước tiếp theo:

**Bước Tiếp Theo Ngay:**
1. ✅ Đọc **01_Kien_Truc_Project.md** - Hiểu project cụ thể này
2. ✅ Đọc **02_He_Thong_Player_Day_Du.md** - Deep dive vào player mechanics
3. ✅ Mở Unity Editor và khám phá project
4. ✅ Chạy game trong chế độ Play và quan sát hành vi

**Tác Vụ Thực Hành:**
1. Tạo GameObject đơn giản với script di chuyển khi nhấn phím mũi tên
2. Làm object spawn khi nhấn spacebar
3. Thêm UI button thay đổi text khi clicked
4. Tạo trigger destroy objects khi vào

**Học Nâng Cao:**
1. Đọc documentation theo hệ thống (Enemy, UI, Managers)
2. Làm theo **10_Huong_Dan_Thuc_Hanh.md** để tạo modifications
3. Implement feature của riêng bạn từ đầu

---

## 13. Quick Reference Cheat Sheet

**Lifecycle Order:**
```
Awake() → OnEnable() → Start() → Update()/FixedUpdate()/LateUpdate() → OnDisable() → OnDestroy()
```

**Code Snippets Phổ Biến:**
```csharp
// Lấy component
Rigidbody2D rb = GetComponent<Rigidbody2D>();

// Tìm GameObject
GameObject player = GameObject.FindGameObjectWithTag("Player");

// Instantiate
Instantiate(prefab, position, Quaternion.identity);

// Destroy
Destroy(gameObject);
Destroy(gameObject, 2f);  // Sau 2 giây

// Coroutine delay
IEnumerator Example() {
    yield return new WaitForSeconds(2f);
    // Code này chạy sau 2 giây
}

// Input
if (Input.GetKeyDown(KeyCode.Space)) { }
float h = Input.GetAxis("Horizontal");

// Physics movement
rb.velocity = new Vector2(speed, rb.velocity.y);
rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

// UI
text.text = "Score: " + score;
button.onClick.AddListener(OnClick);
```

---

**Chúc mừng! Bạn giờ đã sẵn sàng đi sâu vào project documentation.**

**Tài Liệu Tiếp Theo:** → `01_Kien_Truc_Project.md`
