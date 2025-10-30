# Translation Style Guide - Hướng Dẫn Phong Cách Dịch

**Mục đích:** Đảm bảo tất cả tài liệu dịch có phong cách nhất quán, tự nhiên và dễ hiểu cho người Việt

**Nguyên tắc chính:**
1. 🎯 **Ưu tiên sự hiểu rõ** hơn là dịch từng từ
2. 🗣️ **Tự nhiên với người Việt** - viết như đang nói chuyện
3. 📖 **Giữ technical accuracy** - không làm sai lệch ý nghĩa kỹ thuật
4. 🔄 **Consistency** - dùng thuật ngữ giống nhau xuyên suốt

---

## 📝 Table of Contents

1. [Tone & Voice (Giọng Văn)](#1-tone--voice)
2. [Sentence Structure (Cấu Trúc Câu)](#2-sentence-structure)
3. [Paragraph Organization](#3-paragraph-organization)
4. [Code Documentation](#4-code-documentation)
5. [Headers & Titles](#5-headers--titles)
6. [Lists & Bullet Points](#6-lists--bullet-points)
7. [Examples & Analogies](#7-examples--analogies)
8. [Formatting Guidelines](#8-formatting-guidelines)
9. [Common Patterns](#9-common-patterns)
10. [Quality Checklist](#10-quality-checklist)

---

## 1. Tone & Voice (Giọng Văn)

### 1.1 Writing Style

**✅ DO - Phong cách khuyến khích:**

- **Friendly & Encouraging** (Thân thiện & Khích lệ):
  ```markdown
  ✅ "Tuyệt vời! Bây giờ bạn đã hiểu GameObject rồi."
  ✅ "Đừng lo nếu chưa hiểu rõ, ta sẽ đi sâu vào chi tiết."
  ✅ "Hãy thử modify giá trị này và xem điều gì xảy ra!"
  ```

- **Direct & Clear** (Trực tiếp & Rõ ràng):
  ```markdown
  ✅ "Bước 1: Mở Unity Editor"
  ✅ "Method này chạy mỗi frame"
  ✅ "Bạn cần thêm component Rigidbody"
  ```

- **Conversational** (Như đang trò chuyện):
  ```markdown
  ✅ "Giờ ta sẽ tạo một Enemy mới"
  ✅ "Bạn có thể thấy..."
  ✅ "Điều này giúp..."
  ```

**❌ DON'T - Tránh:**

- **Overly Formal** (Quá trang trọng):
  ```markdown
  ❌ "Người dùng cần phải thực hiện các bước sau đây một cách tuần tự"
  ✅ "Hãy làm theo các bước sau"
  ```

- **Too Casual/Slang** (Quá suồng sã):
  ```markdown
  ❌ "Cái này ngon lành, xài đi bro!"
  ✅ "Cách này hiệu quả, hãy thử áp dụng!"
  ```

- **Passive Voice** (Thụ động không cần thiết):
  ```markdown
  ❌ "Method Update() được gọi bởi Unity mỗi frame"
  ✅ "Unity gọi method Update() mỗi frame"
  ```

### 1.2 Person & Perspective

**Use "Bạn" (You) for instructions:**
```markdown
✅ "Bạn có thể thêm component..."
✅ "Khi bạn chạy game..."
```

**Use "Ta/Chúng ta" (We) for collaborative tone:**
```markdown
✅ "Hãy cùng tạo một Enemy mới"
✅ "Ta sẽ implement feature này"
✅ "Giờ chúng ta sẽ test code"
```

**Avoid "Tôi" (I):**
```markdown
❌ "Tôi sẽ hướng dẫn bạn..."
✅ "Bây giờ hãy cùng..."
```

---

## 2. Sentence Structure (Cấu Trúc Câu)

### 2.1 Keep It Simple

**English vs Vietnamese sentence length:**

```markdown
❌ Bad (too long):
"Trong Unity, GameObject là đối tượng cơ bản nhất được sử dụng để
biểu diễn bất kỳ thực thể nào trong game, và mỗi GameObject có thể
chứa nhiều component khác nhau để định nghĩa hành vi của nó."

✅ Good (broken into shorter sentences):
"GameObject là đối tượng cơ bản nhất trong Unity. Nó biểu diễn các
thực thể trong game. Mỗi GameObject có thể chứa nhiều component để
định nghĩa hành vi."
```

### 2.2 Active Voice

```markdown
❌ Passive: "Method này được gọi bởi Unity"
✅ Active: "Unity gọi method này"

❌ Passive: "Component Rigidbody được sử dụng cho vật lý"
✅ Active: "Component Rigidbody xử lý vật lý"
```

### 2.3 Verb Placement

```markdown
✅ "Ta cần thêm component"
✅ "Bạn có thể modify giá trị"
✅ "Hãy test game"

(Động từ đặt gần đầu câu, tự nhiên với người Việt)
```

---

## 3. Paragraph Organization

### 3.1 Structure Template

**Mỗi section nên có:**

```markdown
## [Section Title]

[1-2 câu giới thiệu tổng quan]

### Concept Explanation
[Giải thích concept chính]

### Code Example
[Code với comments tiếng Việt]

### Practical Usage
[Ví dụ thực tế]

### Common Mistakes
[Lỗi thường gặp]
```

### 3.2 Paragraph Length

**Ideal: 3-5 sentences per paragraph**

```markdown
✅ Good:
GameObject là container chứa các component. Component định nghĩa
hành vi của GameObject. Ví dụ: Rigidbody component thêm vật lý,
Collider component xử lý va chạm.

❌ Too long (split it):
GameObject là container... [10 câu liên tục]
```

### 3.3 Visual Breaks

**Use headings, code blocks, and lists để chia nhỏ content:**

```markdown
## Main Topic

Brief intro...

### Subtopic 1
Content...

```code
Example
```

### Subtopic 2
Content...

- Bullet point 1
- Bullet point 2
```

---

## 4. Code Documentation

### 4.1 Code Block Translation Rules

**✅ ONLY translate comments:**

```csharp
// ✅ CORRECT:
// Class điều khiển player, xử lý movement và shooting
public class PlayerController : MonoBehaviour
{
    // Tốc độ di chuyển (units per second)
    public float moveSpeed = 5f;

    // Reference đến Rigidbody component
    private Rigidbody rb;

    // Được gọi khi game khởi động
    void Start()
    {
        // Lấy component Rigidbody
        rb = GetComponent<Rigidbody>();
    }

    // Chạy mỗi frame để xử lý input
    void Update()
    {
        // Đọc input từ bàn phím
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // Tính vector di chuyển
        Vector3 movement = new Vector3(h, 0, v);

        // Apply movement vào Rigidbody
        rb.velocity = movement * moveSpeed;
    }
}
```

**❌ NEVER translate code itself:**

```csharp
// ❌ WRONG - Don't do this:
lớp công khai PlayerController : MonoBehaviour
{
    công khai số thực moveSpeed = 5f;
}
```

### 4.2 Inline Code

**Format:** `Code` - Giải thích

```markdown
✅ `GetComponent<Rigidbody>()` - Lấy component Rigidbody từ GameObject
✅ `Update()` - Method chạy mỗi frame
✅ `public float speed` - Biến tốc độ, có thể chỉnh trong Inspector
```

### 4.3 Code Explanation Pattern

**After code block, explain in detail:**

```markdown
```csharp
void Update()
{
    transform.Translate(Vector3.forward * speed * Time.deltaTime);
}
```

**Giải thích:**
- `transform`: Reference đến Transform component của GameObject này
- `Translate()`: Method di chuyển object
- `Vector3.forward`: Hướng về phía trước (0, 0, 1)
- `speed`: Tốc độ di chuyển
- `Time.deltaTime`: Thời gian giữa các frame (để movement frame-independent)
```

---

## 5. Headers & Titles

### 5.1 Header Translation

**Main titles - Dịch sang tiếng Việt:**

```markdown
# Unity Fundamentals - Từ Zero đến Hero
## 1. Unity Là Gì?
### 1.1 Tổng Quan Game Engine
```

**Keep English when it's a proper term:**

```markdown
# GameObject và Component System
# Player System - Hệ Thống Nhân Vật
# Animation Controller Workflow
```

### 5.2 Capitalization

**Vietnamese:**
```markdown
✅ Chỉ viết hoa chữ đầu: "Hướng dẫn tạo enemy"
❌ Không viết hoa tất cả: "HƯỚNG DẪN TẠO ENEMY"
```

**English terms trong tiêu đề:**
```markdown
✅ "Hệ Thống Player và Enemy"
✅ "Singleton Pattern trong Unity"
✅ "Sử Dụng Coroutine"
```

---

## 6. Lists & Bullet Points

### 6.1 Unordered Lists

**Use consistent style:**

```markdown
✅ Format:
- Item một
- Item hai
- Item ba

Or:

• Item một
• Item hai
• Item ba
```

### 6.2 Ordered Lists

**Use Vietnamese numbering terms:**

```markdown
✅ Good:
1. Đầu tiên, mở Unity Editor
2. Tiếp theo, tạo GameObject mới
3. Cuối cùng, attach script

Also OK:
1. Bước 1: Mở Unity Editor
2. Bước 2: Tạo GameObject mới
3. Bước 3: Attach script
```

### 6.3 Nested Lists

```markdown
✅ Clear hierarchy:
- System chính:
  - Player System
    - Movement
    - Shooting
  - Enemy System
    - AI
    - Pathfinding
```

---

## 7. Examples & Analogies

### 7.1 Using Analogies

**Make them relatable to Vietnamese culture:**

```markdown
✅ "GameObject giống như một cái hộp, còn component giống như đồ vật bên trong hộp"

✅ "Scene giống như một phòng trong nhà, mỗi phòng có các đồ vật (GameObject) khác nhau"

✅ "Prefab giống như khuôn bánh - bạn có thể tạo nhiều bánh (instance) từ một khuôn"
```

### 7.2 Real-World Examples

```markdown
✅ Example section:
**Ví dụ thực tế:**

Trong game Lawn Defense, Player_Archer là GameObject có:
- Transform: Vị trí của archer trên battlefield
- Player_Archer script: Logic bắn tên tự động
- Collider: Vùng va chạm với enemy
- SpriteRenderer: Hiển thị hình archer
```

---

## 8. Formatting Guidelines

### 8.1 Bold & Italics

**Bold for emphasis:**
```markdown
✅ "Điều này **rất quan trọng**"
✅ "**GameObject** là đối tượng cơ bản"
✅ "**Lưu ý:** Không dùng Instantiate trong Update()"
```

**Italics for subtle emphasis:**
```markdown
✅ "Bạn *có thể* dùng cách này"
✅ "*Khuyến nghị:* Sử dụng Object Pool"
```

### 8.2 Code Formatting

**Inline code:**
```markdown
✅ Method `Update()` chạy mỗi frame
✅ Biến `moveSpeed` điều khiển tốc độ
✅ Dùng `GetComponent<T>()` để lấy component
```

**File paths:**
```markdown
✅ File này nằm ở `/Assets/Scripts/Player/PlayerController.cs`
✅ Mở thư mục `Assets/_MonstersOut/`
```

### 8.3 Emoji Usage

**Use sparingly and consistently:**

```markdown
✅ Section icons:
📚 Documentation
🎯 Practice
✅ Completion Check
💡 Pro Tip
⚠️ Warning
❌ Don't Do This
✅ Do This
🎮 Game Feature
🏗️ Architecture
⚙️ System

✅ In context:
"✅ **Đúng:** Dùng Object Pool cho bullets"
"❌ **Sai:** Instantiate bullets mỗi frame"
```

### 8.4 Blockquotes

```markdown
> 💡 **Pro Tip:** Cache GetComponent calls trong Awake() để optimize performance

> ⚠️ **Cảnh báo:** Không dùng FindObjectOfType() trong Update() - rất chậm!

> 📖 **Lưu ý:** Đọc `99_Glossary.md` để hiểu thuật ngữ
```

---

## 9. Common Patterns

### 9.1 Instruction Pattern

**Step-by-step format:**

```markdown
## Hướng Dẫn Tạo Enemy Mới

### Bước 1: Tạo GameObject
1. Vào Hierarchy panel
2. Right-click → **Create Empty**
3. Đặt tên "Zombie"

### Bước 2: Add Components
1. Select Zombie trong Hierarchy
2. Trong Inspector, click **Add Component**
3. Chọn **Rigidbody 2D**
4. Set Body Type = Dynamic

### Bước 3: Create Script
1. Trong Project panel, vào `Assets/Scripts/Enemy/`
2. Right-click → **Create → C# Script**
3. Đặt tên `ZombieController`
4. Double-click để mở script

[Code example với comments tiếng Việt]
```

### 9.2 Explanation Pattern

**Concept → Example → Details:**

```markdown
## Coroutine Là Gì?

**Coroutine** là method đặc biệt cho phép tạm dừng execution và tiếp tục sau một khoảng thời gian.

**Ví dụ:** Hiển thị message sau 2 giây:

```csharp
void Start()
{
    StartCoroutine(ShowMessageAfterDelay());
}

IEnumerator ShowMessageAfterDelay()
{
    // Đợi 2 giây
    yield return new WaitForSeconds(2f);

    // Code này chạy sau 2 giây
    Debug.Log("2 giây đã trôi qua!");
}
```

**Chi tiết:**
- `IEnumerator`: Return type của coroutine
- `yield return`: Tạm dừng coroutine
- `WaitForSeconds(2f)`: Đợi 2 giây
- `StartCoroutine()`: Khởi động coroutine
```

### 9.3 Troubleshooting Pattern

```markdown
## Vấn Đề: Player Không Di Chuyển

**Triệu chứng:**
- Nhấn WASD nhưng player không động
- Không có error trong Console

**Nguyên nhân có thể:**

### 1. Rigidbody bị freeze
**Kiểm tra:**
- Select Player trong Hierarchy
- Xem Inspector → Rigidbody → Constraints
- Nếu Freeze Position X/Y được check → Đây là vấn đề

**Giải pháp:**
Uncheck Freeze Position X và Y

### 2. Script chưa được attach
**Kiểm tra:**
- Select Player
- Xem Inspector - có component PlayerController không?

**Giải pháp:**
Click Add Component → tìm PlayerController → Add

[...]
```

### 9.4 Comparison Pattern

```markdown
## Update() vs FixedUpdate()

| Aspect | Update() | FixedUpdate() |
|--------|----------|---------------|
| **Tần suất** | Mỗi frame (~60 FPS) | Fixed timestep (50 FPS) |
| **Dùng cho** | Input, UI, logic thường | Physics, Rigidbody movement |
| **Thời gian** | Không cố định | Cố định (0.02s) |

**Khi nào dùng Update():**
✅ Đọc input: `Input.GetKey()`
✅ Update UI
✅ Game logic không liên quan physics

**Khi nào dùng FixedUpdate():**
✅ Di chuyển bằng Rigidbody
✅ Apply force
✅ Physics calculations
```

---

## 10. Quality Checklist

### 10.1 Before Submitting Translation

**Content Quality:**
- [ ] Tất cả instructions rõ ràng và dễ follow?
- [ ] Code examples có comments tiếng Việt?
- [ ] Technical terms consistent với Glossary?
- [ ] Không có dịch máy artifacts (câu lủng củng)?
- [ ] Tone friendly và encouraging?

**Technical Accuracy:**
- [ ] Thuật ngữ Unity giữ nguyên tiếng Anh?
- [ ] Code không bị translate?
- [ ] Class/method/variable names giữ nguyên?
- [ ] Technical explanations chính xác?

**Formatting:**
- [ ] Emoji sử dụng nhất quán?
- [ ] Code blocks formatted đúng?
- [ ] Headers có hierarchy hợp lý?
- [ ] Lists và tables hiển thị đúng?

**Consistency:**
- [ ] Cross-references updated (link đến _VI.md)?
- [ ] Thuật ngữ dùng giống nhau throughout file?
- [ ] Tone consistent với files khác?

### 10.2 Self-Review Questions

**Ask yourself:**

1. ❓ Nếu tôi là người Việt không biết tiếng Anh, tôi có hiểu được không?
   - If NO → Simplify or add explanation

2. ❓ Câu này có nghe tự nhiên khi đọc to không?
   - If NO → Restructure

3. ❓ Technical terms có match với Glossary không?
   - If NO → Fix consistency

4. ❓ Code có bị translate nhầm không?
   - If YES → Fix immediately

5. ❓ Instructions có thể follow được không?
   - If NO → Add more details

---

## 📋 Translation Workflow

### Step-by-Step Process:

```markdown
1. **Read English version completely**
   - Understand context
   - Identify key terms
   - Note technical concepts

2. **Check Glossary**
   - Verify term translations
   - Add new terms if needed

3. **Translate content**
   - Headers → Vietnamese
   - Instructions → Vietnamese
   - Explanations → Vietnamese
   - Code → Keep English, translate comments only

4. **Add Vietnamese metadata**
   ```markdown
   ---
   **🌐 Ngôn ngữ:** Tiếng Việt
   **📄 File gốc:** [Original.md](Original.md)
   **🔄 Cập nhật:** 2025-01-XX
   ---
   ```

5. **Update cross-references**
   - Link to _VI.md files
   - Update navigation

6. **Self-review**
   - Read aloud
   - Check checklist
   - Verify formatting

7. **Test**
   - Render markdown
   - Click links
   - Check code syntax highlighting
```

---

## 🎯 Key Principles Summary

**Remember the 4 C's:**

1. **Clear** (Rõ ràng)
   - Simple sentences
   - Direct instructions
   - No ambiguity

2. **Consistent** (Nhất quán)
   - Same terms throughout
   - Same tone and style
   - Follow patterns

3. **Correct** (Chính xác)
   - Technical accuracy
   - No translation errors
   - Code integrity

4. **Comfortable** (Tự nhiên)
   - Natural Vietnamese
   - Friendly tone
   - Easy to read

---

**🎓 Final Advice:**

> "Dịch để người Việt hiểu, không phải để dịch cho xong"
>
> "Write as you would teach a friend who speaks Vietnamese"
>
> "When in doubt, keep it simple and clear"

---

**Last Updated:** 2025-01-XX
**Version:** 1.0
**Maintained by:** Claude Code Translation Team
