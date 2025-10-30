# Translation Glossary - Bảng Thuật Ngữ Dịch

**Mục đích:** Đảm bảo tính nhất quán trong việc dịch thuật ngữ kỹ thuật từ tiếng Anh sang tiếng Việt

**Nguyên tắc:**
- ✅ Giữ nguyên thuật ngữ chuyên môn quan trọng
- ✅ Giải thích bằng tiếng Việt khi cần
- ✅ Dịch instructions và explanations
- ✅ Nhất quán xuyên suốt tất cả documents

---

## 📚 Unity Core Terms (Thuật Ngữ Unity Cốt Lõi)

### **KHÔNG DỊCH** - Giữ nguyên tiếng Anh:

| English Term | Vietnamese Context | Notes |
|--------------|-------------------|-------|
| **Unity** | Unity | Tên engine, không dịch |
| **GameObject** | GameObject | Thuật ngữ cốt lõi, có thể thêm: "đối tượng game" khi giải thích lần đầu |
| **Component** | Component | Có thể thêm: "thành phần" khi cần giải thích |
| **Prefab** | Prefab | Có thể giải thích: "template/khuôn mẫu" |
| **Scene** | Scene | Có thể gọi là "màn chơi" khi giải thích casual |
| **Asset** | Asset | Có thể nói "tài nguyên" khi giải thích |
| **Transform** | Transform | Component xác định vị trí, xoay, scale |
| **Sprite** | Sprite | Hình ảnh 2D |
| **Canvas** | Canvas | Nền chứa UI |
| **Rigidbody** | Rigidbody | Component vật lý |
| **Collider** | Collider | Component va chạm |
| **Renderer** | Renderer | Component hiển thị graphics |
| **Animator** | Animator | Component animation |
| **Inspector** | Inspector | Panel chỉnh sửa properties |
| **Hierarchy** | Hierarchy | Cây cấu trúc GameObjects |
| **Project** | Project | Panel quản lý assets |
| **Console** | Console | Panel hiển thị logs |

### Ví dụ sử dụng:

```markdown
❌ Bad: "Đối tượng trò chơi là container cho các thành phần"
✅ Good: "GameObject là container chứa các component"

❌ Bad: "Bạn có thể tạo prefab trong Unity"
✅ Good: "Bạn có thể tạo prefab (template) trong Unity"

First mention: "**GameObject** (đối tượng game) là unit cơ bản trong Unity"
After that: "GameObject có thể chứa nhiều component"
```

---

## 💻 Programming Terms (Thuật Ngữ Lập Trình)

### **KHÔNG DỊCH** - Giữ nguyên:

| English Term | Vietnamese Explanation | Usage |
|--------------|----------------------|-------|
| **class** | class | "Class PlayerController kế thừa từ..." |
| **interface** | interface | Không dịch |
| **struct** | struct | Không dịch |
| **enum** | enum | Có thể nói "kiểu liệt kê" khi giải thích |
| **namespace** | namespace | Không gian tên |
| **using** | using | Câu lệnh import |
| **public** | public | "Biến public có thể truy cập từ bên ngoài" |
| **private** | private | "Biến private chỉ dùng internal" |
| **protected** | protected | Không dịch |
| **static** | static | "Static method không cần instance" |
| **void** | void | "Không trả về giá trị" |
| **int** | int | Số nguyên |
| **float** | float | Số thập phân |
| **string** | string | Chuỗi ký tự |
| **bool** | bool | True/False |
| **array** | array/mảng | Có thể dùng "mảng" |
| **list** | list/danh sách | Có thể dùng "danh sách" |
| **return** | return | Trả về |
| **if/else** | if/else | Điều kiện |
| **for/while** | for/while | Vòng lặp |
| **break/continue** | break/continue | Không dịch |
| **this** | this | "Tham chiếu đến object hiện tại" |
| **base** | base | "Tham chiếu đến class cha" |
| **null** | null | Giá trị rỗng |

---

## 🎮 Unity Scripting Terms

### **KHÔNG DỊCH** - Giữ nguyên:

| Method/Class | Explanation in Vietnamese |
|--------------|--------------------------|
| **MonoBehaviour** | Base class cho Unity scripts, không dịch |
| **ScriptableObject** | Class để tạo data containers, không dịch |
| **Awake()** | Method khởi tạo, chạy đầu tiên |
| **Start()** | Method khởi tạo, chạy sau Awake() |
| **Update()** | Method chạy mỗi frame |
| **FixedUpdate()** | Method chạy với fixed timestep (cho physics) |
| **LateUpdate()** | Method chạy sau tất cả Update() |
| **OnEnable()** | Gọi khi object được enable |
| **OnDisable()** | Gọi khi object bị disable |
| **OnDestroy()** | Gọi khi object bị destroy |
| **OnTriggerEnter()** | Gọi khi va chạm với trigger |
| **OnCollisionEnter()** | Gọi khi va chạm vật lý |
| **Instantiate()** | Tạo instance của object |
| **Destroy()** | Hủy object |
| **GetComponent()** | Lấy component từ GameObject |
| **FindObjectOfType()** | Tìm object theo type |
| **Time.deltaTime** | Thời gian giữa các frame |
| **Input.GetAxis()** | Đọc input từ user |
| **Vector2/Vector3** | Tọa độ 2D/3D |
| **Quaternion** | Biểu diễn rotation |
| **Coroutine** | Hàm chạy theo thời gian, có thể pause |
| **yield** | Tạm dừng coroutine |

### Ví dụ:

```csharp
// ❌ Don't translate code:
void Start()
{
    Rigidbody rb = GetComponent<Rigidbody>();
}

// ✅ Translate only comments:
void Start()  // ← Được gọi trước frame đầu tiên
{
    // Lấy component Rigidbody từ GameObject này
    Rigidbody rb = GetComponent<Rigidbody>();
}
```

---

## 🏗️ Design Patterns (Mẫu Thiết Kế)

### **KHÔNG DỊCH** - Giữ nguyên tên pattern:

| Pattern | Vietnamese Explanation | Usage |
|---------|----------------------|-------|
| **Singleton** | Singleton (Mẫu đơn thể) | Đảm bảo chỉ 1 instance |
| **Observer** | Observer (Mẫu quan sát) | Event-driven pattern |
| **Factory** | Factory (Mẫu nhà máy) | Tạo objects |
| **Object Pool** | Object Pool (Hồ chứa đối tượng) | Tái sử dụng objects |
| **State Machine** | State Machine (Máy trạng thái) | Quản lý states |
| **MVC** | MVC (Model-View-Controller) | Architecture pattern |
| **Event System** | Event System (Hệ thống sự kiện) | Decoupling pattern |

### Ví dụ:

```markdown
✅ "**Singleton Pattern** là mẫu thiết kế đảm bảo chỉ có 1 instance của class tồn tại"
✅ "Sử dụng **Observer Pattern** để các system không phụ thuộc trực tiếp vào nhau"
✅ "**Object Pool** giúp tái sử dụng object thay vì Instantiate/Destroy liên tục"
```

---

## 📝 Common Phrases Translation

### Instructional Phrases:

| English | Tiếng Việt | Notes |
|---------|-----------|-------|
| Let's create... | Hãy tạo... | Casual, friendly |
| Now we will... | Bây giờ chúng ta sẽ... / Giờ ta sẽ... | Shorter is better |
| For example | Ví dụ | |
| In other words | Nói cách khác | |
| Note that | Lưu ý rằng | |
| This means | Điều này có nghĩa là | |
| As you can see | Như bạn thấy | |
| Make sure to... | Hãy đảm bảo... | |
| Don't forget to... | Đừng quên... | |
| You can also... | Bạn cũng có thể... | |
| To do this... | Để làm điều này... | |
| Step-by-step | Từng bước một | |
| First, second, third | Đầu tiên, tiếp theo, cuối cùng | |
| Before/After | Trước/Sau | |
| Complete beginner | Người mới hoàn toàn | |
| Prerequisites | Yêu cầu trước / Điều kiện tiên quyết | |
| Target audience | Đối tượng độc giả | |
| Estimated time | Thời gian ước tính | |
| Quick reference | Tra cứu nhanh | |
| Getting started | Bắt đầu | |
| Troubleshooting | Khắc phục sự cố | |
| Common issues | Vấn đề thường gặp | |
| Best practices | Các phương pháp tốt nhất | |
| Pro tip | Mẹo chuyên nghiệp | |
| Warning | Cảnh báo | |
| Important | Quan trọng | |

### Action Verbs:

| English | Tiếng Việt | Context |
|---------|-----------|---------|
| Create | Tạo | Create GameObject → Tạo GameObject |
| Add | Thêm | Add component → Thêm component |
| Remove | Xóa/Gỡ | Remove component → Gỡ component |
| Delete | Xóa | Delete file → Xóa file |
| Modify | Chỉnh sửa | Modify value → Chỉnh sửa giá trị |
| Change | Thay đổi | Change speed → Thay đổi tốc độ |
| Assign | Gán | Assign reference → Gán reference |
| Attach | Gắn | Attach script → Gắn script vào object |
| Drag and drop | Kéo thả | Không dịch, quen thuộc |
| Click | Click | Không dịch |
| Press | Nhấn | Press Play button → Nhấn nút Play |
| Select | Chọn | Select GameObject → Chọn GameObject |
| Open | Mở | Open Unity Editor → Mở Unity Editor |
| Close | Đóng | Close window → Đóng cửa sổ |
| Build | Build | Build game → Build game (không dịch) |
| Run | Chạy | Run game → Chạy game |
| Debug | Debug | Debug code → Debug code (không dịch) |
| Test | Test | Test game → Test game |
| Compile | Compile | Compile code → Compile code |
| Deploy | Deploy | Deploy to mobile → Deploy lên mobile |

---

## 🎯 Technical Verbs (Động Từ Kỹ Thuật)

| English | Tiếng Việt | Usage |
|---------|-----------|-------|
| Initialize | Khởi tạo | Initialize variable |
| Instantiate | Instantiate | Không dịch, "tạo instance" nếu giải thích |
| Spawn | Spawn | "Sinh ra" hoặc giữ "spawn" |
| Destroy | Destroy | Hủy/Phá hủy object |
| Enable/Disable | Enable/Disable | Bật/Tắt |
| Activate/Deactivate | Activate/Deactivate | Kích hoạt/Vô hiệu |
| Trigger | Trigger | Kích hoạt |
| Invoke | Invoke | Gọi method |
| Call | Call | Gọi hàm |
| Execute | Execute | Thực thi |
| Implement | Implement | Triển khai |
| Override | Override | Ghi đè |
| Inherit | Inherit | Kế thừa |
| Subscribe/Unsubscribe | Subscribe/Unsubscribe | Đăng ký/Hủy đăng ký (event) |
| Fire event | Fire event | Phát sự kiện / Trigger event |
| Listen | Listen | Lắng nghe (event) |

---

## 🎨 UI Terms

| English | Tiếng Việt | Notes |
|---------|-----------|-------|
| Button | Button | Nút bấm |
| Text | Text | Text component |
| Image | Image | Hình ảnh |
| Slider | Slider | Thanh trượt |
| Input Field | Input Field | Ô nhập liệu |
| Dropdown | Dropdown | Menu thả xuống |
| Toggle | Toggle | Nút chuyển đổi |
| Scroll View | Scroll View | Vùng cuộn |
| Panel | Panel | Panel UI |
| Canvas | Canvas | Canvas (nền UI) |
| Event System | Event System | Hệ thống sự kiện |
| Layout | Layout | Bố cục |

---

## 🔊 Audio Terms

| English | Tiếng Việt |
|---------|-----------|
| Audio Source | Audio Source (nguồn âm thanh) |
| Audio Clip | Audio Clip |
| Audio Listener | Audio Listener |
| Sound Effect | Sound effect / Hiệu ứng âm thanh |
| Background Music | Nhạc nền |
| Volume | Âm lượng |
| Pitch | Pitch (cao độ) |

---

## 🎬 Animation Terms

| English | Tiếng Việt |
|---------|-----------|
| Animation | Animation |
| Animator | Animator |
| Animation Clip | Animation Clip |
| State | State (trạng thái) |
| Transition | Transition (chuyển cảnh) |
| Trigger | Trigger |
| Parameter | Parameter (tham số) |

---

## 📁 File System Terms

| English | Tiếng Việt | Usage |
|---------|-----------|-------|
| Folder | Folder / Thư mục | Có thể dùng cả hai |
| Directory | Directory / Thư mục | |
| File | File / Tệp | |
| Path | Path / Đường dẫn | |
| Script | Script | File C# |
| Assets folder | Thư mục Assets | |
| Resources folder | Thư mục Resources | |

---

## 🎮 Game Terms

### **CÓ THỂ DỊCH:**

| English | Tiếng Việt | Notes |
|---------|-----------|-------|
| Player | Player / Người chơi | Tùy context |
| Enemy | Enemy / Kẻ địch | Tùy context |
| Character | Nhân vật | Dịch OK |
| Level | Màn chơi / Level | Cả hai đều OK |
| Health | Máu / Health | |
| Damage | Sát thương / Damage | |
| Speed | Tốc độ | Dịch OK |
| Score | Điểm số | Dịch OK |
| Game Over | Game Over | Không dịch |
| Pause | Pause / Tạm dừng | |
| Resume | Resume / Tiếp tục | |
| Menu | Menu | Không dịch |
| Settings | Cài đặt | Dịch OK |
| Gameplay | Gameplay | Không dịch |

---

## 📐 Math & Physics Terms

| English | Tiếng Việt | Notes |
|---------|-----------|-------|
| Vector | Vector | Không dịch |
| Position | Vị trí | Dịch OK |
| Rotation | Góc xoay / Rotation | |
| Scale | Kích thước / Scale | |
| Velocity | Vận tốc / Velocity | |
| Acceleration | Gia tốc | Dịch OK |
| Force | Lực | Dịch OK |
| Gravity | Trọng lực | Dịch OK |
| Mass | Khối lượng | Dịch OK |
| Drag | Lực cản | Dịch OK |
| Friction | Ma sát | Dịch OK |
| Collision | Va chạm | Dịch OK |
| Trigger | Trigger | Vùng cảm ứng |
| Raycast | Raycast | Bắn tia |
| Distance | Khoảng cách | Dịch OK |
| Angle | Góc | Dịch OK |
| Magnitude | Độ lớn | Dịch OK |
| Normalized | Normalized | Chuẩn hóa (length = 1) |
| Lerp | Lerp | Linear interpolation |
| Clamp | Clamp | Giới hạn giá trị |

---

## 💡 Common Mistakes to Avoid

### ❌ KHÔNG nên:

```markdown
❌ "Bạn cần phải lấy thành phần Rigidbody2D"
✅ "Bạn cần lấy component Rigidbody2D"

❌ "Đối tượng trò chơi này có nhiều thành phần"
✅ "GameObject này có nhiều component"

❌ "Phương thức Cập Nhật được gọi mỗi khung hình"
✅ "Method Update() được gọi mỗi frame"

❌ "Tạo một thể hiện của prefab"
✅ "Tạo instance của prefab" hoặc "Instantiate prefab"

❌ "Kế thừa từ Hành Vi Đơn"
✅ "Kế thừa từ MonoBehaviour"

❌ "Đính kèm kịch bản vào đối tượng"
✅ "Attach script vào GameObject"
```

---

## 🎯 Context-Based Translation

### Casual Context (Tutorial, Explanation):
```markdown
✅ "GameObject là đối tượng cơ bản trong Unity"
✅ "Bạn có thể thêm component vào GameObject"
✅ "Scene giống như một màn chơi trong game"
```

### Technical Context (Code Documentation):
```markdown
✅ "GameObject chứa Transform component"
✅ "GetComponent<Rigidbody>() trả về Rigidbody component"
✅ "Awake() được gọi trước Start()"
```

### Mixed Context:
```markdown
✅ "**GameObject** (đối tượng game) là container chứa các **component**"
✅ "**Prefab** (template) giúp tái sử dụng GameObject"
✅ "Method **Update()** chạy mỗi frame (khoảng 60 lần/giây)"
```

---

## 📋 Special Cases

### 1. Code Blocks:
```csharp
// ✅ ONLY translate comments:

// Class Player kế thừa từ MonoBehaviour
public class Player : MonoBehaviour
{
    // Tốc độ di chuyển của nhân vật (đơn vị: units/giây)
    public float speed = 5f;

    // Được gọi khi game bắt đầu
    void Start()
    {
        // Khởi tạo player
    }
}
```

### 2. File Paths:
```markdown
✅ Giữ nguyên: `/Assets/Scripts/Player/PlayerController.cs`
✅ Giữ nguyên: `Assets/_MonstersOut/Scripts/Controllers/`
```

### 3. Unity Menu Paths:
```markdown
✅ Giữ nguyên + giải thích:
"Vào **Edit → Project Settings → Input Manager**"
"Chọn **GameObject → Create Empty**"
```

### 4. Inspector Settings:
```markdown
✅ Giữ tên setting, dịch giải thích:
"Set **Body Type** = Dynamic (Kiểu vật lý động)"
"Adjust **Gravity Scale** (Hệ số trọng lực) = 1"
```

---

## 🔄 Consistency Rules

### 1. First Mention Rule:
```markdown
Lần đầu: **GameObject** (đối tượng game trong Unity)
Lần sau: GameObject
```

### 2. Code + Explanation Rule:
```markdown
`GetComponent<Rigidbody>()` - Lấy component Rigidbody từ GameObject
```

### 3. Technical Term Rule:
```markdown
Nếu là thuật ngữ Unity/C# chính thức → Giữ nguyên tiếng Anh
Nếu là mô tả chung → Dịch sang tiếng Việt
```

---

## ✅ Quick Reference

**Khi dịch, hỏi bản thân:**

1. ❓ Đây có phải thuật ngữ Unity chính thức không?
   - ✅ Yes → Giữ tiếng Anh
   - ❌ No → Có thể dịch

2. ❓ Đây có phải tên class/method/variable trong code không?
   - ✅ Yes → Giữ tiếng Anh
   - ❌ No → Có thể dịch

3. ❓ Thuật ngữ này có trong Unity documentation không?
   - ✅ Yes → Giữ tiếng Anh
   - ❌ No → Có thể dịch

4. ❓ Người Việt quen với thuật ngữ tiếng Anh này không?
   - ✅ Yes → Giữ tiếng Anh (ví dụ: "frame", "debug", "build")
   - ❌ No → Dịch hoặc giải thích

---

**Last Updated:** 2025-01-XX
**Maintained by:** Claude Code Translation Team
**For questions:** Refer to Translation-Style-Guide.md
