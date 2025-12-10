---
**🌐 Ngôn ngữ:** Tiếng Việt
**📄 File gốc:** [README.md](README.md)
**🔄 Cập nhật lần cuối:** 2025-01-30
---

# Lawn Defense: Monsters Out - Tài Liệu Hướng Dẫn

**Tài Liệu Phát Triển Game Unity Chuyên Nghiệp**

📚 **Hướng dẫn đầy đủ** từ cơ bản đến nâng cao
🎯 **Tài liệu kỹ thuật** production-ready
🚀 **Thực hành** với tutorial và ví dụ cụ thể
⚡ **Tham khảo nhanh** cho developer có kinh nghiệm

---

## 🎮 Khởi Đầu Nhanh

**👉 MỚI VỚI PROJECT NÀY?**
→ **[BẮT ĐẦU TỪ ĐÂY](00_BAT_DAU_TU_DAY.md)** ⭐

**Đang tìm kiếm thứ gì đó cụ thể?**
→ Sử dụng [Điều Hướng Nhanh](#-điều-hướng-nhanh) bên dưới

**Cần giải thích một thuật ngữ?**
→ Kiểm tra **[Glossary](99_Glossary.md)** (Từ điển thuật ngữ)

---

## 📖 Mục Lục

1. [Điều Hướng Nhanh](#-điều-hướng-nhanh)
2. [Cấu Trúc Tài Liệu](#-cấu-trúc-tài-liệu)
3. [Lộ Trình Học Tập](#-lộ-trình-học-tập)
4. [Danh Sách File](#-danh-sách-file)
5. [Tham Khảo Nhanh](#-tham-khảo-nhanh)
6. [Đóng Góp](#-đóng-góp)

---

## 🧭 Điều Hướng Nhanh

### Theo Trình Độ

| Trình Độ | Bắt Đầu Tại | Đọc Tiếp | Cuối Cùng |
|-----------|-------------|----------|-----------|
| **Hoàn Toàn Mới** | [Unity Cơ Bản](00_Unity_Co_Ban.md) | [Kiến Trúc Project](01_Kien_Truc_Project.md) | [Player System](02_Player_System_Complete.md) |
| **Có Chút Kiến Thức Unity** | [Kiến Trúc Project](01_Kien_Truc_Project.md) | [BẮT ĐẦU TỪ ĐÂY](00_BAT_DAU_TU_DAY.md) | Tài liệu System theo nhu cầu |
| **Developer Có Kinh Nghiệm** | [BẮT ĐẦU TỪ ĐÂY](00_BAT_DAU_TU_DAY.md) | [Phân Tích Project](project-analysis.md) | Tài liệu system cụ thể |

### Theo Nhiệm Vụ

| Tôi muốn... | Đọc File Này |
|-------------|--------------|
| **Hiểu project** | [Kiến Trúc Project](01_Kien_Truc_Project.md) |
| **Sửa đổi cơ chế bắn của player** | [Player System](02_Player_System_Complete.md) |
| **Thêm enemy mới** | How-To Guides (sắp ra) |
| **Thay đổi UI/menu** | UI System docs (sắp ra) |
| **Sửa lỗi** | Troubleshooting (sắp ra) |
| **Hiểu một thuật ngữ** | [Glossary](99_Glossary.md) |
| **Xem ví dụ code** | Code Examples (sắp ra) |

### Theo Chủ Đề

| Chủ Đề | Tài Liệu |
|--------|----------|
| **Unity Cơ Bản** | [Unity Cơ Bản](00_Unity_Co_Ban.md) |
| **Kiến Trúc Game** | [Kiến Trúc Project](01_Kien_Truc_Project.md) |
| **Nhân Vật Player** | [Player System](02_Player_System_Complete.md) |
| **AI Enemy** | Enemy System (sắp ra) |
| **Hệ Thống UI** | UI System (sắp ra) |
| **Game Manager** | Managers (sắp ra) |
| **Pattern & Thực Hành** | [Kiến Trúc Project](01_Kien_Truc_Project.md) §5 |

---

## 📚 Cấu Trúc Tài Liệu

### Tài Liệu Cốt Lõi (Mới - Có Phiên Bản Tiếng Việt)

```
Documents/
├── 📄 README_VI.md                        ← BẠN ĐANG Ở ĐÂY
├── 🎯 00_BAT_DAU_TU_DAY.md               ← Điểm khởi đầu cho tất cả mọi người
│
├── 📘 Nền Tảng
│   ├── 00_Unity_Co_Ban.md                 ← Unity cơ bản từ con số 0
│   └── 01_Kien_Truc_Project.md            ← Cấu trúc project & pattern
│
├── 🔧 Tài Liệu System
│   ├── 02_Player_System_Complete.md       ← Chi tiết cơ chế Player
│   ├── 03_Enemy_System_Complete.md        ← AI Enemy (đang lên kế hoạch)
│   ├── 04_UI_System_Complete.md           ← User interface (đang lên kế hoạch)
│   ├── 05_Managers_Complete.md            ← Các class Manager (đang lên kế hoạch)
│   └── 06_AI_System_Complete.md           ← AI decision making (đang lên kế hoạch)
│
├── 📚 Hướng Dẫn Thực Hành
│   ├── 10_How_To_Guides.md                ← Tutorial từng bước (đang lên kế hoạch)
│   ├── 11_Troubleshooting.md              ← Các vấn đề thường gặp (đang lên kế hoạch)
│   └── 13_Code_Examples.md                ← Code snippet (đang lên kế hoạch)
│
├── 📖 Tài Liệu Tham Khảo
│   ├── 12_Visual_Reference.md             ← Sơ đồ & hình ảnh (đang lên kế hoạch)
│   ├── 99_Glossary.md                     ← Định nghĩa thuật ngữ A-Z
│   └── project-analysis.md                ← Phân tích kỹ thuật
│
└── 📂 scripts/ (Tài liệu cũ tiếng Việt)
    ├── Scripts-Overview.md
    ├── AI.md, Controllers.md, Helpers.md
    ├── Managers.md, Player.md, UI.md
    └── ... (20 file - tài liệu cũ hơn)
```

### Thứ Tự Đọc Ưu Tiên

**Tuần 1-2: Nền Tảng**
1. ⭐ [00_BAT_DAU_TU_DAY.md](00_BAT_DAU_TU_DAY.md)
2. 📘 [00_Unity_Co_Ban.md](00_Unity_Co_Ban.md) (nếu mới với Unity)
3. 🏗️ [01_Kien_Truc_Project.md](01_Kien_Truc_Project.md)

**Tuần 3-4: Hệ Thống Cốt Lõi**
4. 🏹 [02_Player_System_Complete.md](02_Player_System_Complete.md)
5. 👾 Enemy System (khi có sẵn)
6. 🎨 UI System (khi có sẵn)

**Liên Tục: Tham Khảo**
- 📖 [99_Glossary.md](99_Glossary.md) - Tra cứu thuật ngữ
- 🔧 How-To Guides (khi có sẵn)
- 🐛 Troubleshooting (khi có sẵn)

---

## 🎓 Lộ Trình Học Tập

### Lộ Trình A: Người Mới Hoàn Toàn (Chưa Bao Giờ Dùng Unity)

**Mục tiêu:** Hiểu Unity VÀ project này

**Thời gian:** 4-6 tuần (10 giờ/tuần)

```
Tuần 1-2: Unity Fundamentals
└─ Đọc: 00_Unity_Co_Ban.md
└─ Thực hành: Mở Unity, khám phá interface
└─ Checkpoint: Hiểu GameObject, Component, Prefab

Tuần 3: Cấu Trúc Project
└─ Đọc: 01_Kien_Truc_Project.md
└─ Thực hành: Điều hướng trong folder script, chạy game
└─ Checkpoint: Biết code của Player/Enemy/Manager ở đâu

Tuần 4: Player System
└─ Đọc: 02_Player_System_Complete.md
└─ Thực hành: Sửa đổi shootRate, test thay đổi
└─ Checkpoint: Hiểu cơ chế auto-targeting

Tuần 5-6: Thực Hành
└─ Đọc: How-To Guides (khi có sẵn)
└─ Thực hành: Thêm enemy mới, sửa UI
└─ Checkpoint: Hoàn thành sửa đổi feature đầu tiên
```

**Tiêu Chí Thành Công:**
- ✅ Có thể giải thích GameObject là gì
- ✅ Đã tìm và sửa đổi fire rate của player
- ✅ Hiểu kiến trúc project
- ✅ Hoàn thành một How-To guide

### Lộ Trình B: Trung Cấp (Có Chút Kinh Nghiệm Unity)

**Mục tiêu:** Hiểu project cụ thể này một cách nhanh chóng

**Thời gian:** 1-2 tuần (10 giờ/tuần)

```
Ngày 1-2: Tổng Quan Project
└─ Đọc lướt: 00_Unity_Co_Ban.md (ôn lại)
└─ Đọc kỹ: 01_Kien_Truc_Project.md (đầy đủ)
└─ Đọc kỹ: 02_Player_System_Complete.md
└─ Checkpoint: Hiểu Listener pattern, Player kế thừa Enemy

Ngày 3-4: Deep Dive Vào System
└─ Đọc: Tài liệu system liên quan đến mục tiêu
└─ Thực hành: Tìm các class chính, đọc code
└─ Checkpoint: Xác định được điểm cần sửa đổi

Tuần 2: Triển Khai
└─ Đọc: How-To Guides cho nhiệm vụ cụ thể
└─ Thực hành: Triển khai feature
└─ Checkpoint: Sửa đổi hoạt động tốt
```

**Tiêu Chí Thành Công:**
- ✅ Hiểu tất cả design pattern được dùng
- ✅ Tìm được tất cả core system
- ✅ Sửa đổi thành công ít nhất 2 system

### Lộ Trình C: Chuyên Gia (Unity Developer Có Kinh Nghiệm)

**Mục tiêu:** Định hướng nhanh, sau đó triển khai

**Thời gian:** 2-3 ngày

```
Giờ 1-2: High-Level
└─ Đọc: 01_Kien_Truc_Project.md
└─ Đọc: project-analysis.md
└─ Checkpoint: Hiểu kiến trúc

Giờ 3-4: Review Code
└─ Đọc: GameManager.cs, Player_Archer.cs, Enemy.cs
└─ Review: Cấu trúc folder, design pattern
└─ Checkpoint: Sẵn sàng sửa đổi

Ngày 2-3: Triển Khai
└─ Tham khảo: Tài liệu system khi cần
└─ Tham khảo: Glossary cho thuật ngữ riêng của project
└─ Checkpoint: Feature được triển khai và test
```

**Tiêu Chí Thành Công:**
- ✅ Hiểu toàn bộ project
- ✅ Sửa đổi codebase một cách tự tin
- ✅ Không cần nguồn tài nguyên bên ngoài

---

## 📁 Danh Sách File

### 🎯 File Thiết Yếu (Đọc Trước)

| Ưu Tiên | File | Mô Tả | Thời Gian Đọc |
|---------|------|-------|---------------|
| ⭐⭐⭐ | [00_BAT_DAU_TU_DAY.md](00_BAT_DAU_TU_DAY.md) | Lộ trình của bạn đến tất cả tài liệu | 5 phút |
| ⭐⭐⭐ | [99_Glossary.md](99_Glossary.md) | Định nghĩa thuật ngữ A-Z | 1-2 phút/thuật ngữ |
| ⭐⭐ | [01_Kien_Truc_Project.md](01_Kien_Truc_Project.md) | Cấu trúc project đầy đủ | 30-40 phút |

### 📘 Nền Tảng

| File | Mô Tả | Thời Gian Đọc | Đối Tượng |
|------|-------|----------------|-----------|
| [00_Unity_Co_Ban.md](00_Unity_Co_Ban.md) | Unity từ 0 đến hero | 2-3 giờ | Người mới |
| [01_Kien_Truc_Project.md](01_Kien_Truc_Project.md) | Cấu trúc & pattern | 30-40 phút | Tất cả |
| [project-analysis.md](project-analysis.md) | Deep dive kỹ thuật | 20-30 phút | Nâng cao |

### 🔧 Tài Liệu System

| File | Trạng Thái | Mô Tả | Thời Gian Đọc |
|------|------------|-------|----------------|
| [02_Player_System_Complete.md](02_Player_System_Complete.md) | ✅ **Hoàn Thành** | Cơ chế Player | 45 phút |
| 03_Enemy_System_Complete.md | 📝 Đang lên kế hoạch | AI & hành vi Enemy | ~40 phút |
| 04_UI_System_Complete.md | 📝 Đang lên kế hoạch | User interface | ~30 phút |
| 05_Managers_Complete.md | 📝 Đang lên kế hoạch | Các class Manager | ~35 phút |
| 06_AI_System_Complete.md | 📝 Đang lên kế hoạch | AI decision making | ~30 phút |

### 📚 Hướng Dẫn Thực Hành

| File | Trạng Thái | Mô Tả | Dùng Khi |
|------|------------|-------|----------|
| 10_How_To_Guides.md | 📝 Đang lên kế hoạch | Tutorial từng bước | Thực hiện thay đổi cụ thể |
| 11_Troubleshooting.md | 📝 Đang lên kế hoạch | Vấn đề thường gặp & cách sửa | Gặp lỗi |
| 13_Code_Examples.md | 📝 Đang lên kế hoạch | Code copy-paste | Cần triển khai nhanh |

### 📖 Tài Liệu Tham Khảo

| File | Trạng Thái | Mô Tả | Dùng Khi |
|------|------------|-------|----------|
| [99_Glossary.md](99_Glossary.md) | ✅ **Hoàn Thành** | Định nghĩa thuật ngữ | Không hiểu thuật ngữ |
| 12_Visual_Reference.md | 📝 Đang lên kế hoạch | Sơ đồ & flowchart | Người học bằng hình ảnh |

### 📂 Tài Liệu Cũ (Tiếng Việt)

Nằm trong folder con `scripts/` - tài liệu cũ hơn bằng tiếng Việt:

- Scripts-Overview.md
- AI.md, Controllers.md, Helpers.md
- Managers.md, Player.md, UI.md
- Roadmap.md, Unity-Concepts.md
- (tổng cộng 20 file)

**Lưu ý:** Tài liệu tiếng Anh mới ở trên thay thế các file này.

---

## 🔍 Tham Khảo Nhanh

### Nhiệm Vụ Thường Gặp

```markdown
# Thay đổi tốc độ bắn của player
→ Mở: GameObject Player trong scene
→ Component: Player_Archer
→ Field: shootRate
→ Giá trị nhỏ hơn = bắn nhanh hơn

# Thêm enemy mới
→ Đọc: How-To Guides (khi có sẵn)
→ Duplicate: Prefab enemy có sẵn
→ Sửa đổi: Sprite, stat, hành vi

# Sửa đổi UI
→ Tìm: Canvas trong Hierarchy
→ Chỉnh sửa: Các element UI
→ Script: Assets/_MonstersOut/Scripts/UI/

# Debug lỗi
→ Đọc: Console (dưới cùng Unity)
→ Double-click: Lỗi để nhảy đến code
→ Kiểm tra: Troubleshooting.md (khi có sẵn)
```

### Code Pattern

```csharp
// Truy cập GameManager
GameManager.Instance.Victory();

// Lấy component
var rb = GetComponent<Rigidbody2D>();

// Tìm GameObject
var player = GameObject.FindGameObjectWithTag("Player");

// Instantiate prefab
Instantiate(prefab, position, Quaternion.identity);

// Coroutine
IEnumerator Wait() {
    yield return new WaitForSeconds(1f);
}

// Event
public static event Action OnDeath;
OnDeath?.Invoke();
```

### Unity Lifecycle

```
Awake() → OnEnable() → Start() →
Update() / FixedUpdate() / LateUpdate() (lặp lại) →
OnDisable() → OnDestroy()
```

### Pattern Trong Project

**Singleton:** GameManager, SoundManager
→ Truy cập: property `Instance`

**Observer:** interface IListener
→ GameManager phát sóng event

**State Machine:** State của Enemy
→ SPAWNING, IDLE, WALK, ATTACK, HIT, DEATH

**Inheritance:** Player kế thừa Enemy
→ Tái sử dụng health, damage, effect

---

## 📊 Thống Kê Tài Liệu

**Trạng Thái Hiện Tại:**
- ✅ **Hoàn Thành:** 6 tài liệu (~5000+ dòng)
- 📝 **Đang lên kế hoạch:** 7 tài liệu
- 📂 **Legacy:** 20 tài liệu (Tiếng Việt)

**Phạm Vi:**
- Unity Fundamentals: ✅ Hoàn Thành
- Project Architecture: ✅ Hoàn Thành
- Player System: ✅ Hoàn Thành
- Enemy System: 📝 Đang lên kế hoạch
- UI System: 📝 Đang lên kế hoạch
- Managers: 📝 Đang lên kế hoạch
- AI System: 📝 Đang lên kế hoạch

**Chất Lượng:**
- ✅ Giải thích thân thiện với người mới
- ✅ Comment code từng dòng
- ✅ Sơ đồ ASCII trực quan
- ✅ Ví dụ thực tế
- ✅ Mục troubleshooting
- ✅ Tham chiếu chéo

---

## 🎯 Mục Tiêu Tài Liệu

### Mục Tiêu Chính

**✅ Đã Đạt Được:**
1. Giúp người mới hoàn toàn hiểu Unity fundamental
2. Cung cấp tổng quan kiến trúc project đầy đủ
3. Tài liệu hóa player system với chi tiết tính toán trajectory
4. Tạo cấu trúc tài liệu có thể điều hướng
5. Định nghĩa tất cả thuật ngữ kỹ thuật trong glossary

**📝 Đang Tiến Hành:**
6. Tài liệu hóa tất cả core system (Enemy, UI, Manager, AI)
7. Cung cấp hướng dẫn how-to từng bước
8. Tạo database troubleshooting
9. Biên soạn thư viện code example

### Tiêu Chí Thành Công

**Cho Người Mới:**
- [ ] Có thể mở Unity và điều hướng interface
- [x] Hiểu GameObject và Component
- [x] Có thể sửa đổi giá trị trong Inspector
- [ ] Hoàn thành sửa đổi feature đầu tiên

**Cho Trung Cấp:**
- [x] Hiểu kiến trúc project
- [x] Tìm được tất cả core system
- [x] Biết design pattern được dùng
- [ ] Triển khai custom feature

**Cho Nâng Cao:**
- [x] Hiểu toàn bộ codebase
- [x] Xác định tất cả extension point
- [ ] Đóng góp cải thiện code
- [ ] Tối ưu hiệu suất system

---

## 💡 Cách Sử Dụng Tài Liệu Này

### Mẹo Để Thành Công

**🔖 Đánh Dấu Những File Này:**
- [00_BAT_DAU_TU_DAY.md](00_BAT_DAU_TU_DAY.md) - Điểm khởi đầu chính
- [99_Glossary.md](99_Glossary.md) - Tra cứu thuật ngữ
- README này - Hub điều hướng

**📖 Chiến Lược Đọc:**
1. **Đọc lướt trước** - Có tổng quan
2. **Đọc chủ động** - Ghi chép
3. **Thực hành ngay** - Mở Unity, test
4. **Tham khảo thường xuyên** - Quay lại khi cần

**🎯 Đặt Mục Tiêu:**
- "Tôi sẽ hiểu Player system" ✅ Tốt
- "Tôi sẽ học Unity" ❌ Quá mơ hồ

**🔁 Lặp Lại:**
- Thay đổi nhỏ → Test → Học → Lặp lại
- Đừng cố hiểu tất cả cùng một lúc

### Cách Điều Hướng

**Theo Kinh Nghiệm:**
- Người mới → Theo Lộ trình A
- Trung cấp → Theo Lộ trình B
- Chuyên gia → Theo Lộ trình C

**Theo Mục Tiêu:**
- Hiểu → Đọc fundamental
- Sửa đổi → Đọc tài liệu system + how-to
- Mở rộng → Đọc architecture + pattern

**Theo Thời Gian:**
- 5 phút → Mục tham khảo nhanh
- 30 phút → Tài liệu một system
- 2-3 giờ → Fundamental đầy đủ

---

## 🤝 Đóng Góp

### Báo Cáo Vấn Đề

Tìm thấy vấn đề trong tài liệu?

**Cần báo cáo gì:**
- Lỗi chính tả và ngữ pháp
- Link bị hỏng
- Giải thích không rõ ràng
- Thông tin còn thiếu
- Lỗi code

**Cách báo cáo:**
- Tạo issue trong repository project
- Email team tài liệu
- Đánh dấu dòng cụ thể trong tài liệu

### Đề Xuất Cải Thiện

**Đề xuất tốt:**
- Thêm sơ đồ
- Thêm ví dụ code
- Làm rõ chủ đề phức tạp
- Hướng dẫn how-to mới
- Mẹo về hiệu suất

### Tiêu Chuẩn Tài Liệu

**Tất cả tài liệu tuân theo:**
- ✅ Ngôn ngữ thân thiện với người mới
- ✅ Ví dụ code có giải thích
- ✅ Sơ đồ trực quan khi hữu ích
- ✅ Tham chiếu chéo đến chủ đề liên quan
- ✅ Code thực của project (không lý thuyết)

---

## 📞 Nhận Trợ Giúp

### Thứ Tự Tìm Kiếm

1. **README này** - Kiểm tra tham khảo nhanh
2. **Glossary** - Tra cứu thuật ngữ
3. **Tài liệu System** - Deep dive vào system
4. **Troubleshooting** - Vấn đề thường gặp (khi có sẵn)
5. **Tài nguyên bên ngoài** - Unity docs, forum

### Tài Nguyên Bên Ngoài

**Unity Chính Thức:**
- Manual: https://docs.unity3d.com/Manual/
- Scripting API: https://docs.unity3d.com/ScriptReference/
- Learn: https://learn.unity.com/

**Cộng Đồng:**
- Forum: https://forum.unity.com/
- Reddit: r/Unity3D
- Stack Overflow: tag [unity3d]

**Project Này:**
- Tài liệu: Bạn đang đọc nó!
- Code: `Assets/_MonstersOut/Scripts/`
- Ví dụ: Prefab và scene có sẵn

---

## 📅 Lịch Sử Phiên Bản

**Version 2.0** (Tháng 10/2025) - Hiện tại
- ✅ Viết lại hoàn toàn bằng tiếng Anh
- ✅ Tiếp cận thân thiện với người mới
- ✅ Tiêu chuẩn tài liệu chuyên nghiệp
- ✅ Sơ đồ và ví dụ trực quan
- ✅ Glossary toàn diện
- ✅ Hệ thống điều hướng

**Version 1.0** (Gốc)
- Tài liệu tiếng Việt
- 20 file trong folder scripts/
- Mô tả system cơ bản
- Ví dụ tối thiểu

---

## 🚀 Bước Tiếp Theo

**1. Mới với Unity?**
→ Bắt đầu với [00_BAT_DAU_TU_DAY.md](00_BAT_DAU_TU_DAY.md)
→ Sau đó đọc [00_Unity_Co_Ban.md](00_Unity_Co_Ban.md)

**2. Biết Unity cơ bản?**
→ Nhảy đến [01_Kien_Truc_Project.md](01_Kien_Truc_Project.md)
→ Sau đó đọc [02_Player_System_Complete.md](02_Player_System_Complete.md)

**3. Developer chuyên gia?**
→ Đọc [project-analysis.md](project-analysis.md)
→ Đọc lướt tài liệu system khi cần

**4. Nhiệm vụ cụ thể?**
→ Kiểm tra [00_BAT_DAU_TU_DAY.md](00_BAT_DAU_TU_DAY.md) Mục 3 (Nhiệm Vụ Nhanh)

---

## 📌 Link Nhanh

**Quan Trọng Nhất:**
- 🎯 [BẮT ĐẦU TỪ ĐÂY](00_BAT_DAU_TU_DAY.md) - Bắt đầu hành trình
- 📖 [Glossary](99_Glossary.md) - Tra cứu thuật ngữ
- 🏗️ [Kiến Trúc](01_Kien_Truc_Project.md) - Hiểu cấu trúc

**Theo Chủ Đề:**
- Unity Cơ Bản → [Unity Cơ Bản](00_Unity_Co_Ban.md)
- Nhân Vật Player → [Player System](02_Player_System_Complete.md)
- Phân Tích Kỹ Thuật → [Phân Tích Project](project-analysis.md)

**Tham Khảo:**
- Tất cả thuật ngữ → [Glossary](99_Glossary.md)
- Code pattern → README này §Tham Khảo Nhanh
- Design pattern → [Kiến Trúc](01_Kien_Truc_Project.md) §5

---

**Sẵn sàng bắt đầu?** → [00_BAT_DAU_TU_DAY.md](00_BAT_DAU_TU_DAY.md) ⭐

**Có câu hỏi?** → [99_Glossary.md](99_Glossary.md) cho thuật ngữ

**Chúc may mắn! 🎮**

---

<p align="center">
<strong>Lawn Defense: Monsters Out</strong><br>
Tài Liệu Phát Triển Game Chuyên Nghiệp<br>
Version 2.0 • Tháng 10/2025
</p>
