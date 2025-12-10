# Plan Task cho Claude Code - Dịch Unity Project Documentation sang Tiếng Việt

## 📥 Available Input Files (Tài liệu cần dịch)

### Core Documentation Files:
```
/Documents/
├── 00_START_HERE.md
├── 00_Unity_Fundamentals.md
├── 01_Project_Architecture.md
├── 02_Player_System_Complete.md
├── 03_Enemy_System_Complete.md
├── 04_UI_System_Complete.md
├── 05_Managers_Complete.md
├── 10_How_To_Guides.md
├── 11_Troubleshooting.md
├── 12_Visual_Reference.md
├── 13_Code_Examples.md
├── 99_Glossary.md
├── README.md
└── README-docs.md
```

### Deep Dive Documentation Files:
```
/Documents/
├── Character-Properties.md
├── Core-Objects.md
├── Enemy-Deep.md
├── Events-and-Triggers.md
├── First-Tasks.md
├── Map.md
├── Namespaces.md
├── Player-Deep.md
├── ShopUI.md
├── Unity-Concepts.md
├── Workflow-Tasks.md
└── Roadmap.md
```

### Other Files:
```
/Documents/
├── project-analysis.md
├── req-1.md
└── action-1.md
```

**Tổng cộng:** ~30 markdown files cần dịch

---

## 🎯 Objective (Mục tiêu)

Dịch toàn bộ documentation của Unity project sang **Tiếng Việt** để phục vụ người học người Việt, với các yêu cầu:

1. **Localization thông minh** - Không phải dịch thuần túy mà phải tự nhiên với người Việt
2. **Giữ nguyên thuật ngữ chuyên môn** - Terminology quan trọng giữ tiếng Anh + giải thích
3. **Cấu trúc song ngữ** - Code, technical terms tiếng Anh + mô tả tiếng Việt
4. **Dễ đọc và thực hành** - Ưu tiên người Việt hiểu rõ hơn là dịch chính xác từng từ

---

## 📋 Translation Guidelines (Nguyên tắc dịch)

### ✅ Cần dịch sang tiếng Việt:
- Tiêu đề (Headings) và tiêu đề phụ
- Hướng dẫn, chỉ dẫn, instructions
- Giải thích, mô tả, explanations
- Câu hỏi và đáp án
- Ví dụ mô tả (example descriptions)
- Checklist items
- Warnings và notes
- Success criteria và objectives

### ⛔ KHÔNG dịch - Giữ nguyên tiếng Anh:
**1. Thuật ngữ Unity cốt lõi:**
- GameObject, Component, Prefab, Scene
- Transform, Rigidbody, Collider, Renderer
- Awake(), Start(), Update(), FixedUpdate()
- MonoBehaviour, ScriptableObject
- Canvas, Button, Slider, Text
- Inspector, Hierarchy, Project, Console

**2. Thuật ngữ lập trình:**
- public, private, protected, static
- void, int, float, string, bool
- class, interface, struct, enum
- if, else, for, while, switch
- return, yield, break, continue
- namespace, using, this, base

**3. Design patterns:**
- Singleton, Observer, Factory, Pool
- Event System, State Machine
- MVC, MVVM patterns

**4. Tên biến, hàm, class trong code:**
```csharp
// Giữ nguyên tên code, chỉ dịch comment
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;  // ← Tốc độ di chuyển của nhân vật

    void Update()  // ← Được gọi mỗi frame
    {
        // Code logic...
    }
}
```

**5. File paths và technical commands:**
- `/Assets/Scripts/Player/PlayerController.cs`
- `GetComponent<Rigidbody2D>()`
- `GameObject.Find("Player")`
- `Input.GetAxis("Horizontal")`

### 🔄 Thuật ngữ Song ngữ (Anh + Việt):
Lần đầu xuất hiện: **Term (Thuật ngữ tiếng Việt)**

**Ví dụ:**
- "**GameObject** (Đối tượng game)" - Lần đầu
- "GameObject" - Các lần sau có thể chỉ dùng tiếng Anh
- "**Prefab** (Template / Khuôn mẫu đối tượng)"
- "**Coroutine** (Hàm chạy theo thời gian)"
- "**Singleton Pattern** (Mẫu thiết kế Đơn thể)"

### 💡 Nguyên tắc Localization:

**1. Tự nhiên với người Việt:**
❌ Bad: "Bạn có thể thêm một component vào GameObject"
✅ Good: "Bạn có thể thêm component vào GameObject"

❌ Bad: "Method này được gọi khi GameObject được khởi tạo"
✅ Good: "Method này được gọi khi GameObject khởi tạo"

**2. Dùng từ quen thuộc:**
❌ "Khởi động trò chơi" → ✅ "Chạy game"
❌ "Gỡ lỗi" → ✅ "Debug"
❌ "Biên dịch" → ✅ "Build" hoặc "Compile"
❌ "Khung hình" → ✅ "Frame"

**3. Giữ cấu trúc câu đơn giản:**
❌ "Điều này được thực hiện bằng cách sử dụng..."
✅ "Làm điều này bằng cách..."

**4. Emoji và formatting:**
- Giữ nguyên emoji: ✅, ❌, 🎯, 📋, 💡
- Giữ formatting: **bold**, `code`, > quotes
- Giữ ASCII diagrams nguyên bản

**5. Code comments:**
Dịch comments trong code sang tiếng Việt:
```csharp
// ❌ Original (English):
// Get component reference
Rigidbody2D rb = GetComponent<Rigidbody2D>();

// ✅ Translated (Vietnamese):
// Lấy reference đến component Rigidbody2D
Rigidbody2D rb = GetComponent<Rigidbody2D>();
```

---

## 📋 Task Plan

### **Phase 1: Preparation & Glossary Creation**
**Duration:** ~30 minutes

**Actions:**
- [ ] Đọc qua tất cả 30 files để identify:
  - Các thuật ngữ xuất hiện nhiều nhất
  - Các pattern dịch cần consistent
  - Special cases cần chú ý

- [ ] Tạo **Translation Glossary** (`/Documents/Translation-Glossary.md`):
  ```markdown
  # Translation Glossary - Bảng Thuật ngữ Dịch

  ## Unity Core Terms
  | English | Tiếng Việt | Notes |
  |---------|-----------|-------|
  | GameObject | GameObject | Không dịch - thuật ngữ cốt lõi |
  | Component | Component | Không dịch |
  | Scene | Scene | Có thể nói "màn chơi" khi giải thích |
  | Prefab | Prefab | Có thể giải thích "template" hoặc "khuôn mẫu" |
  | Sprite | Sprite | Hình ảnh 2D |
  | Canvas | Canvas | Nền UI |

  ## Programming Terms
  | English | Tiếng Việt | Notes |
  |---------|-----------|-------|
  | Method | Method | Có thể dùng "hàm" khi casual |
  | Function | Function / Hàm | Tùy context |
  | Variable | Biến | Dịch OK |
  | Property | Property | Không dịch |
  | Field | Field | Không dịch |
  | Event | Event | Không dịch |

  ## Common Phrases
  | English | Tiếng Việt |
  |---------|-----------|
  | Let's create... | Hãy tạo... |
  | Now we will... | Bây giờ ta sẽ... |
  | For example | Ví dụ |
  | Note that | Lưu ý rằng |
  | This means | Điều này có nghĩa là |
  | In other words | Nói cách khác |
  | Step-by-step | Từng bước |
  | Quick reference | Tra cứu nhanh |
  | Troubleshooting | Khắc phục sự cố |
  | Prerequisites | Yêu cầu trước |
  ```

- [ ] Tạo **Translation Style Guide** (`/Documents/Translation-Style-Guide.md`):
  - Formatting rules
  - Tone and voice guidelines
  - Common patterns
  - Do's and Don'ts
  - Example translations

**Output:** Glossary & Style Guide để ensure consistency

---

### **Phase 2: Translate Core Documentation (Priority 1)**
**Duration:** ~2 hours

**Priority Files** (quan trọng nhất, dịch trước):

#### 2.1 - START_HERE & Fundamentals
- [ ] `00_START_HERE.md` → `00_BAT_DAU_TU_DAY.md`
  - Dịch toàn bộ learning path
  - Dịch quick start checklist
  - Giữ file names trong cross-references

- [ ] `00_Unity_Fundamentals.md` → `00_Unity_Co_Ban.md`
  - Dịch tất cả explanations
  - Giữ code examples nguyên vẹn
  - Dịch comments trong code
  - Giữ terminology (GameObject, Component, etc.)

#### 2.2 - Project Architecture
- [ ] `01_Project_Architecture.md` → `01_Kien_Truc_Project.md`
  - Dịch system descriptions
  - Giữ ASCII diagrams + dịch labels
  - Dịch pattern explanations
  - Giữ design pattern names (Singleton, Observer)

#### 2.3 - README Files
- [ ] `README.md` → `README_VI.md`
  - Dịch master index
  - Dịch table headers
  - Update file references (link đến files _VI.md)

- [ ] `README-docs.md` → `README-docs_VI.md`
  - Dịch documentation overview

**Translation Template cho mỗi file:**

```markdown
---
**🌐 Language:** Tiếng Việt (Vietnamese)
**📄 Original:** [Original-File-Name.md]
**🔄 Last Synced:** [Date]
---

# [Tiêu đề dịch sang Tiếng Việt]

> 📘 **[Mô tả ngắn bằng tiếng Việt]**

## [Section tiếng Việt]

[Nội dung dịch với thuật ngữ giữ nguyên...]

### Code Example:
```csharp
// Comment dịch sang tiếng Việt
public class ExampleClass : MonoBehaviour
{
    // Biến tốc độ di chuyển
    public float speed = 5f;

    void Update()
    {
        // Logic di chuyển
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}
```

**Giải thích:**
- `MonoBehaviour`: Base class cho Unity scripts (giữ nguyên tiếng Anh)
- `speed`: Biến lưu tốc độ (đọc code tiếng Anh, hiểu ý nghĩa tiếng Việt)
- `Update()`: Method được Unity gọi mỗi frame

[Tiếp tục nội dung...]
```

**Output:** 5 core documentation files được dịch

---

### **Phase 3: Translate System Documentation (Priority 2)**
**Duration:** ~3 hours

**System Files:**

- [ ] `02_Player_System_Complete.md` → `02_He_Thong_Player_Day_Du.md`
  - Dịch system overview
  - Dịch code walkthrough explanations
  - Giữ class names, method names
  - Dịch "How to Modify" section
  - Dịch "Common Issues" section

- [ ] `03_Enemy_System_Complete.md` → `03_He_Thong_Enemy_Day_Du.md`
  - Tương tự player system
  - Dịch AI behavior descriptions

- [ ] `04_UI_System_Complete.md` → `04_He_Thong_UI_Day_Du.md`
  - Dịch UI component explanations
  - Giữ Unity UI terms (Canvas, Button, Text, etc.)

- [ ] `05_Managers_Complete.md` → `05_Cac_Manager_Day_Du.md`
  - Dịch singleton pattern explanation
  - Dịch manager responsibilities

**Special attention for System Docs:**
- Maintain consistency in technical terms
- Code blocks: Chỉ dịch comments
- Method explanations: Full Vietnamese translation
- Parameter descriptions: Vietnamese
- Return value descriptions: Vietnamese

**Output:** 4 system documentation files được dịch

---

### **Phase 4: Translate Practical Guides (Priority 3)**
**Duration:** ~2 hours

**Guide Files:**

- [ ] `10_How_To_Guides.md` → `10_Huong_Dan_Thuc_Hanh.md`
  - **Critical:** Step-by-step instructions phải rõ ràng
  - Dịch từng bước thành tiếng Việt
  - Giữ technical actions (Add Component, Inspector, etc.)
  - Example: "Bước 1: Vào Hierarchy → Right-click → Create Empty"

- [ ] `11_Troubleshooting.md` → `11_Khac_Phuc_Su_Co.md`
  - Dịch problem descriptions
  - Dịch solutions
  - Giữ technical settings names
  - Example: "**Vấn đề:** Nhân vật không di chuyển"

- [ ] `First-Tasks.md` → `First-Tasks_VI.md`
  - Dịch task descriptions
  - Dịch expected outcomes

- [ ] `Workflow-Tasks.md` → `Workflow-Tasks_VI.md`
  - Dịch workflow steps
  - Dịch best practices

**Output:** 4 practical guide files được dịch

---

### **Phase 5: Translate Reference Materials (Priority 4)**
**Duration:** ~2 hours

**Reference Files:**

- [ ] `12_Visual_Reference.md` → `12_Tham_Chieu_Truc_Quan.md`
  - **ASCII diagrams:** Dịch labels bên trong
  - Example:
    ```
    Unity Editor Layout
    ┌─────────────────────────────────────────────┐
    │  Menu Bar: File Edit Assets GameObject      │  ← Giữ nguyên
    ├──────────┬───────────────────┬──────────────┤
    │ Hierarchy│   Scene View      │  Inspector   │  ← Giữ nguyên
    │          │ [Thế giới game]   │              │  ← Dịch mô tả
    ```

- [ ] `13_Code_Examples.md` → `13_Vi_Du_Code.md`
  - Dịch category names: "Movement Patterns" → "Các Mẫu Di Chuyển"
  - Dịch explanations
  - Giữ code 100% nguyên bản
  - Dịch code comments

- [ ] `99_Glossary.md` → `99_Tu_Dien_Thuat_Ngu.md`
  - **Format:**
    ```markdown
    ## A
    **Awake()**: Unity method được gọi khi GameObject được khởi tạo, chạy trước `Start()`.

    **Asset**: Tài nguyên được import vào Unity project (ảnh, âm thanh, script, v.v.)

    ## G
    **GameObject**: Đối tượng cơ bản trong Unity, đóng vai trò như container chứa các Component.
    ```

**Output:** 3 reference files được dịch

---

### **Phase 6: Translate Deep Dive Documentation (Priority 5)**
**Duration:** ~3 hours

**Deep Dive Files:**

- [ ] `Character-Properties.md` → `Character-Properties_VI.md`
- [ ] `Core-Objects.md` → `Core-Objects_VI.md`
- [ ] `Enemy-Deep.md` → `Enemy-Deep_VI.md`
- [ ] `Events-and-Triggers.md` → `Events-and-Triggers_VI.md`
- [ ] `Map.md` → `Map_VI.md`
- [ ] `Namespaces.md` → `Namespaces_VI.md`
- [ ] `Player-Deep.md` → `Player-Deep_VI.md`
- [ ] `ShopUI.md` → `ShopUI_VI.md`
- [ ] `Unity-Concepts.md` → `Unity-Concepts_VI.md`

**Approach:**
- Deep technical content - giữ accuracy cao
- Maintain technical term consistency
- Dịch explanations chi tiết
- Code architecture descriptions → Vietnamese
- Code itself → English with Vietnamese comments

**Output:** 9 deep dive files được dịch

---

### **Phase 7: Translate Project Management Docs (Priority 6)**
**Duration:** ~1 hour

**Management Files:**

- [ ] `Roadmap.md` → `Roadmap_VI.md`
  - Dịch milestone descriptions
  - Dịch task lists
  - Giữ technical feature names

- [ ] `project-analysis.md` → `project-analysis_VI.md`
  - Dịch analysis findings
  - Dịch recommendations

**Output:** 2 project management files được dịch

---

### **Phase 8: Quality Assurance & Consistency Check**
**Duration:** ~1.5 hours

**Actions:**

- [ ] **Terminology Consistency Check:**
  - Search for inconsistent translations
  - Verify all technical terms match Glossary
  - Fix any translation drift

- [ ] **Cross-Reference Validation:**
  - Update all internal links to point to `_VI.md` files
  - Verify all file references work
  - Check table of contents

- [ ] **Code Block Verification:**
  - Ensure no code was accidentally translated
  - Verify all comments are in Vietnamese
  - Check code formatting intact

- [ ] **Readability Review:**
  - Read 2-3 random files từ đầu đến cuối
  - Check if natural cho người Việt
  - Adjust awkward phrasings

- [ ] **Formatting Check:**
  - Verify markdown rendering
  - Check emoji display
  - Verify ASCII diagrams alignment
  - Test code syntax highlighting

**Quality Checklist cho mỗi file:**
- [ ] Tiêu đề đã dịch sang tiếng Việt
- [ ] Instructions/explanations đã dịch
- [ ] Code giữ nguyên 100%, chỉ comment dịch
- [ ] Technical terms consistent với Glossary
- [ ] Emoji và formatting intact
- [ ] Cross-references updated
- [ ] ASCII diagrams có labels tiếng Việt (nếu cần)
- [ ] Tone tự nhiên cho người Việt
- [ ] No translation artifacts (dịch máy weird)

**Output:** All files polished và consistent

---

### **Phase 9: Create Bilingual Navigation**
**Duration:** ~30 minutes

**Actions:**

- [ ] Tạo `README_VI.md` (Vietnamese master index):
  - Link đến tất cả `_VI.md` files
  - Vietnamese navigation
  - Vietnamese learning path

- [ ] Tạo `LANGUAGE_SWITCH.md`:
  ```markdown
  # 🌐 Language / Ngôn ngữ

  ## Choose Your Language / Chọn Ngôn Ngữ

  ### 🇬🇧 English Documentation
  👉 [Start Here - English](README.md)

  ### 🇻🇳 Tài Liệu Tiếng Việt
  👉 [Bắt Đầu Từ Đây - Tiếng Việt](README_VI.md)

  ## File Navigation

  | English | Tiếng Việt |
  |---------|-----------|
  | [00_START_HERE.md](00_START_HERE.md) | [00_BAT_DAU_TU_DAY.md](00_BAT_DAU_TU_DAY.md) |
  | [00_Unity_Fundamentals.md](00_Unity_Fundamentals.md) | [00_Unity_Co_Ban.md](00_Unity_Co_Ban.md) |
  | ... | ... |
  ```

- [ ] Update main `README.md`:
  - Add language selector at top:
    ```markdown
    # Unity Project Documentation

    **🌐 Languages:** [English](#) | [Tiếng Việt](README_VI.md)
    ```

**Output:** Bilingual navigation system

---

### **Phase 10: Create Translation Maintenance Guide**
**Duration:** ~30 minutes

**Actions:**

- [ ] Tạo `TRANSLATION_MAINTENANCE.md`:
  ```markdown
  # Translation Maintenance Guide

  ## Keeping Vietnamese Docs Synced

  ### When English docs update:
  1. Check git diff for changed files
  2. Identify corresponding `_VI.md` file
  3. Apply same changes with Vietnamese translation
  4. Update "Last Synced" date in header

  ### Translation workflow:
  ```bash
  # Example: Updating Player System doc
  1. English updated: 02_Player_System_Complete.md
  2. Open Vietnamese: 02_He_Thong_Player_Day_Du.md
  3. Compare changes
  4. Translate new sections
  5. Update sync date
  ```

  ### Using Translation Glossary:
  - Always refer to `Translation-Glossary.md`
  - Add new terms when encountered
  - Discuss term translations before using

  ### Quality Standards:
  - Run consistency check after updates
  - Verify technical terms unchanged
  - Test code examples unchanged
  - Review readability
  ```

**Output:** Maintenance documentation

---

## ✅ Quality Checklist

### File-Level Quality:
- [ ] ✅ Tiêu đề và headings đã dịch tiếng Việt
- [ ] ✅ Nội dung giải thích đã dịch tiếng Việt
- [ ] ✅ Code blocks giữ nguyên (chỉ dịch comments)
- [ ] ✅ Technical terms consistent với Glossary
- [ ] ✅ Thuật ngữ Unity giữ tiếng Anh
- [ ] ✅ Design patterns giữ tiếng Anh
- [ ] ✅ File paths và commands giữ nguyên
- [ ] ✅ Cross-references updated để trỏ đến files _VI
- [ ] ✅ Emoji và formatting intact
- [ ] ✅ ASCII diagrams readable (có thể có labels tiếng Việt)

### Project-Level Quality:
- [ ] ✅ Tất cả 30 files đã dịch
- [ ] ✅ Translation Glossary hoàn chỉnh
- [ ] ✅ Style Guide rõ ràng
- [ ] ✅ Terminology consistent across files
- [ ] ✅ Navigation system bilingual
- [ ] ✅ README_VI.md comprehensive
- [ ] ✅ Maintenance guide clear
- [ ] ✅ No broken links
- [ ] ✅ Git history preserved (translations as separate commits)

### Readability Quality:
- [ ] ✅ Tự nhiên cho người Việt đọc
- [ ] ✅ Không có dịch máy artifacts
- [ ] ✅ Câu văn mượt mà
- [ ] ✅ Instructions rõ ràng, dễ follow
- [ ] ✅ Examples dễ hiểu
- [ ] ✅ Tone friendly và encouraging

---

## 📦 Final Deliverables

### Vietnamese Documentation Structure:
```
/Documents/
├── LANGUAGE_SWITCH.md (new)
├── README_VI.md (new)
├── Translation-Glossary.md (new)
├── Translation-Style-Guide.md (new)
├── TRANSLATION_MAINTENANCE.md (new)
│
├── 00_BAT_DAU_TU_DAY.md (translated)
├── 00_Unity_Co_Ban.md (translated)
├── 01_Kien_Truc_Project.md (translated)
├── 02_He_Thong_Player_Day_Du.md (translated)
├── 03_He_Thong_Enemy_Day_Du.md (translated)
├── 04_He_Thong_UI_Day_Du.md (translated)
├── 05_Cac_Manager_Day_Du.md (translated)
├── 10_Huong_Dan_Thuc_Hanh.md (translated)
├── 11_Khac_Phuc_Su_Co.md (translated)
├── 12_Tham_Chieu_Truc_Quan.md (translated)
├── 13_Vi_Du_Code.md (translated)
├── 99_Tu_Dien_Thuat_Ngu.md (translated)
│
├── Character-Properties_VI.md (translated)
├── Core-Objects_VI.md (translated)
├── Enemy-Deep_VI.md (translated)
├── Events-and-Triggers_VI.md (translated)
├── First-Tasks_VI.md (translated)
├── Map_VI.md (translated)
├── Namespaces_VI.md (translated)
├── Player-Deep_VI.md (translated)
├── ShopUI_VI.md (translated)
├── Unity-Concepts_VI.md (translated)
├── Workflow-Tasks_VI.md (translated)
├── Roadmap_VI.md (translated)
└── project-analysis_VI.md (translated)
```

**Total New Files:** ~35 Vietnamese files + 5 supporting files

---

## 💡 Critical Instructions for Claude Code

### Translation Philosophy:

**1. Think like a Vietnamese reader:**
- Would they understand this?
- Does this sound natural in Vietnamese?
- Is the meaning clear without being overly formal?

**2. Balance between English and Vietnamese:**
```markdown
❌ Bad: "Chúng ta cần phải lấy component Rigidbody2D từ GameObject"
✅ Good: "Ta cần lấy component Rigidbody2D từ GameObject"

❌ Bad: "Bạn có thể instantiate prefab này"
✅ Good: "Bạn có thể instantiate (tạo instance) prefab này"
```

**3. Code translation approach:**
```csharp
// ❌ Don't translate:
public class PlayerController : MonoBehaviour

// ✅ Translate only comments:
/// <summary>
/// Class điều khiển player, xử lý input và movement
/// </summary>
public class PlayerController : MonoBehaviour
{
    // Tốc độ di chuyển (units per second)
    public float moveSpeed = 5f;
}
```

**4. Technical terms - First mention format:**
- English term + (Vietnamese explanation)
- **GameObject** (Đối tượng game - là container chứa các component)
- Then use English term consistently

**5. Maintain teaching tone:**
```markdown
❌ Formal: "Điều này sẽ thực thi method Update() mỗi frame"
✅ Friendly: "Method Update() sẽ chạy mỗi frame"

❌ Too casual: "Cái này ngon, xài đi!"
✅ Professional-friendly: "Cách này hiệu quả, hãy thử áp dụng!"
```

### File Naming Convention:

**Pattern 1:** Core numbered files
- `00_START_HERE.md` → `00_BAT_DAU_TU_DAY.md`
- `01_Project_Architecture.md` → `01_Kien_Truc_Project.md`

**Pattern 2:** Descriptive files
- `How_To_Guides.md` → `Huong_Dan_Thuc_Hanh.md`
- `Troubleshooting.md` → `Khac_Phuc_Su_Co.md`

**Pattern 3:** Technical files (keep partial English)
- `Player-Deep.md` → `Player-Deep_VI.md`
- `Unity-Concepts.md` → `Unity-Concepts_VI.md`

### Header Format for Translated Files:

```markdown
---
**🌐 Ngôn ngữ:** Tiếng Việt
**📄 File gốc:** [Original-File-Name.md](Original-File-Name.md)
**🔄 Cập nhật lần cuối:** 2024-01-XX
**📝 Người dịch:** Claude Code
**✅ Đã review:** [Date]
---

[Content starts here]
```

### Translation Priority:

1. **High Priority** (Core learning path):
   - START_HERE, Fundamentals, Architecture, System docs

2. **Medium Priority** (Practical usage):
   - How-To Guides, Troubleshooting, Code Examples

3. **Lower Priority** (Reference):
   - Deep dives, Glossary, Visual Reference

### Quality Assurance Steps:

**For each file:**
1. Translate content
2. Review technical terms against Glossary
3. Test code blocks unchanged
4. Check cross-references
5. Read aloud (does it sound natural?)
6. Compare side-by-side with English
7. Mark as completed

---

## 🎯 Success Criteria

Translation succeeds if:

1. ✅ **Người Việt không biết tiếng Anh có thể học Unity** từ bộ docs này
2. ✅ **Technical accuracy maintained** - Không có sai sót về kỹ thuật
3. ✅ **Natural Vietnamese** - Đọc mượt mà, không giống dịch máy
4. ✅ **Consistency** - Thuật ngữ giống nhau throughout all files
5. ✅ **Code integrity** - Tất cả code examples work perfectly
6. ✅ **Navigation clear** - Dễ switch giữa English và Vietnamese
7. ✅ **Maintainable** - Có hướng dẫn rõ để sync updates
8. ✅ **Comprehensive** - 100% files được dịch

### Testing Method:

**Test với 3 nhóm người:**
1. **Người mới học Unity (newbie):**
   - Có hiểu được instructions không?
   - Có làm được theo guides không?

2. **Developer có kinh nghiệm:**
   - Technical accuracy có đúng không?
   - Terminology có consistent không?

3. **Người Việt không biết tiếng Anh:**
   - Có hiểu được concepts không?
   - Có cảm thấy tự nhiên không?

---

## 📝 Additional Notes

### Common Translation Challenges:

**1. Verb tenses in code comments:**
```csharp
// English: "Updates player position every frame"
// Vietnamese: "Cập nhật vị trí player mỗi frame" (simple present)
```

**2. Plural forms:**
- English: "GameObjects" → Vietnamese: "các GameObject" or just "GameObject"
- Context-dependent

**3. Articles (a, an, the):**
- Vietnamese doesn't have articles
- Translate meaning, not word-for-word

**4. Passive voice:**
- English loves passive: "is called by Unity"
- Vietnamese prefers active: "Unity gọi method này"

**5. Technical ambiguity:**
- When unsure, keep English + add explanation
- Example: "**Instantiate** (tạo instance của object)"

### Files to NOT Translate:

- `req-1.md` (internal plan doc, keep English)
- `action-1.md` (internal action doc, keep English)
- Code files (`.cs`, `.json`, etc.)
- Asset files
- Unity meta files

### Git Commit Strategy:

```bash
# Commit pattern:
git commit -m "docs(vi): translate 00_START_HERE to Vietnamese"
git commit -m "docs(vi): translate Player System documentation"
git commit -m "docs(vi): add Translation Glossary"
git commit -m "docs(vi): complete Vietnamese translation project"
```

---

**Estimated Total Time:** ~15-20 hours
**Complexity:** Medium-High (requires bilingual technical expertise)
**Impact:** High (enables Vietnamese developers to learn Unity)

---

**🎯 This plan enables Claude Code to create comprehensive Vietnamese documentation that feels native to Vietnamese readers while maintaining technical accuracy and consistency.**
