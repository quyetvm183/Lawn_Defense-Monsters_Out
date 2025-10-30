# 🎮 BẮT ĐẦU TỪ ĐÂY - Lawn Defense: Monsters Out Documentation

---
**🌐 Ngôn ngữ:** Tiếng Việt
**📄 File gốc:** [00_START_HERE.md](00_START_HERE.md)
**🔄 Cập nhật lần cuối:** 2025-01-XX
---

**Chào mừng bạn!** Đây là điểm khởi đầu để hiểu và chỉnh sửa Unity game project này.

**Đối tượng độc giả:** Từ người mới hoàn toàn đến Unity veterans
**Thời gian ước tính:** 5 phút để hiểu lộ trình học tập
**Cập nhật lần cuối:** Tháng 10, 2025

---

## 📌 Điều Hướng Nhanh

**Mới với Unity?** → Bắt đầu với Mục 1 (Lộ trình cho người mới)
**Đã quen với Unity?** → Nhảy đến Mục 2 (Lộ trình trung cấp)
**Chỉ muốn chỉnh sửa gì đó?** → Đi đến Mục 3 (Hướng dẫn nhanh)

---

## Mục Lục
1. [Lộ Trình Học Tập Cho Người Mới](#1-lộ-trình-học-tập-cho-người-mới)
2. [Lộ Trình Cho Developer Trung Cấp](#2-lộ-trình-cho-developer-trung-cấp)
3. [Hướng Dẫn Tác Vụ Nhanh](#3-hướng-dẫn-tác-vụ-nhanh)
4. [Bản Đồ Tài Liệu](#4-bản-đồ-tài-liệu)
5. [Checklist Ngày Đầu Tiên](#5-checklist-ngày-đầu-tiên)
6. [Nhận Trợ Giúp](#6-nhận-trợ-giúp)

---

## 1. Lộ Trình Học Tập Cho Người Mới

**Nếu bạn chưa bao giờ dùng Unity, hãy theo lộ trình này tuần tự:**

### Tuần 1-2: Unity Fundamentals (Nền tảng)

**Mục tiêu:** Hiểu Unity cơ bản mà không bị overwhelm

**📚 Đọc:**
1. **`00_Unity_Co_Ban.md`** (2-3 giờ)
   - Unity là gì và hoạt động như thế nào
   - GameObject, Component, Prefab
   - Scripting cơ bản (MonoBehaviour, Update, Start)
   - Hệ thống Physics, Input, UI
   - **Hoàn thành trước khi chuyển tiếp!**

**🎯 Thực hành:**
- Mở Unity Editor và khám phá giao diện
- Chạy game ở chế độ Play
- Chọn objects trong Hierarchy, xem trong Inspector
- Chỉnh sửa một biến public và quan sát thay đổi

**✅ Kiểm tra hoàn thành:**
- [ ] Bạn có thể giải thích GameObject là gì không?
- [ ] Bạn có biết sự khác biệt giữa Update() và Start() không?
- [ ] Bạn có thể tìm và chỉnh sửa biến trong Inspector không?
- [ ] Bạn có hiểu Prefab là gì không?

### Tuần 2-3: Project Architecture (Hiểu Game Này)

**Mục tiêu:** Hiểu cấu trúc của project CỤ THỂ này

**📚 Đọc:**
2. **`01_Kien_Truc_Project.md`** (30-40 phút)
   - Tổng quan high-level về game
   - Cấu trúc thư mục và tổ chức
   - Các hệ thống chính (GameManager, Player, Enemy)
   - Design patterns được dùng (Singleton, Observer, State Machine)

**🎯 Thực hành:**
- Di chuyển trong thư mục `Assets/_MonstersOut/Scripts/`
- Tìm GameManager.cs và đọc comments
- Tìm Player_Archer.cs và Enemy.cs
- Chạy game và quan sát các hệ thống hoạt động

**✅ Kiểm tra hoàn thành:**
- [ ] Bạn có thể giải thích vòng lặp chính của game không?
- [ ] Bạn có biết tìm scripts của Player và Enemy ở đâu không?
- [ ] Bạn có thể mô tả Listener pattern không?
- [ ] Bạn có hiểu tại sao Player kế thừa từ Enemy không?

### Tuần 3-4: Core Systems (Đi Sâu)

**Mục tiêu:** Hiểu chi tiết các hệ thống chính

**📚 Đọc (chọn cái nào bạn quan tâm nhất):**
3. **`02_He_Thong_Player_Day_Du.md`** (45 phút)
   - Cách archer tự động nhắm enemies
   - Giải thích tính toán trajectory
   - Cơ chế bắn
   - Hệ thống di chuyển

4. **`99_Tu_Dien_Thuat_Ngu.md`** (tham khảo, 10 phút)
   - Tra cứu các thuật ngữ bạn chưa hiểu
   - Giữ file này mở khi đọc docs khác

**🎯 Thực hành:**
- Tìm Player_Archer.cs trong project
- Đọc code auto-targeting
- Thay đổi shootRate trong Inspector và test
- Chỉnh sửa sát thương của arrow và quan sát

**✅ Kiểm tra hoàn thành:**
- [ ] Bạn có thể giải thích auto-targeting hoạt động thế nào không?
- [ ] Bạn có thể modify fire rate của player không?
- [ ] Bạn có hiểu tính toán trajectory không (về mặt concept)?

### Tuần 4-6: Thực Hành Chỉnh Sửa

**Mục tiêu:** Tạo những thay đổi đầu tiên trong game

**📚 Đọc:**
5. **`10_Huong_Dan_Thuc_Hanh.md`** (hướng dẫn thực tế)
   - Tutorials từng bước
   - Code examples để copy-paste
   - Hướng dẫn testing

**🎯 Thực hành:**
- Làm theo guide "Cách thêm Enemy mới"
- Làm theo guide "Cách thay đổi stats của Player"
- Làm theo guide "Cách thêm UI element"

**✅ Kiểm tra hoàn thành:**
- [ ] Bạn đã thêm thành công enemy type mới chưa?
- [ ] Bạn có thể tạo UI button mới làm gì đó không?
- [ ] Bạn đã chỉnh sửa và test một tính năng game chưa?

### Liên Tục: Tham Khảo & Khắc Phục Sự Cố

**📚 Sử dụng khi cần:**
- **`11_Khac_Phuc_Su_Co.md`** - Khi gặp lỗi
- **`99_Tu_Dien_Thuat_Ngu.md`** - Khi thấy thuật ngữ lạ
- **`13_Vi_Du_Code.md`** - Khi cần code snippets

---

## 2. Lộ Trình Cho Developer Trung Cấp

**Nếu bạn đã biết Unity cơ bản:**

### Ngày 1: Tổng Quan Project (1-2 giờ)

**Đọc theo thứ tự:**
1. **`01_Kien_Truc_Project.md`** - Hiểu cấu trúc
2. **`02_He_Thong_Player_Day_Du.md`** - Hệ thống auto-targeting độc đáo
3. Đọc lướt **`00_Unity_Co_Ban.md`** - Bỏ qua phần đã biết

**Hành động:**
- Mở project trong Unity
- Chạy game, quan sát các hệ thống
- Browse cấu trúc thư mục Scripts
- Đọc GameManager.cs

**Checkpoint:**
- [ ] Hiểu implementation của Listener pattern
- [ ] Biết tại sao Player kế thừa từ Enemy
- [ ] Đã tìm thấy tất cả hệ thống chính (Player, Enemy, Managers)

### Ngày 2-3: Đi Sâu Vào Hệ Thống (3-4 giờ)

**Chọn hệ thống liên quan đến mục tiêu của bạn:**

**Muốn chỉnh sửa gameplay?**
- `02_He_Thong_Player_Day_Du.md` - Cơ chế Player
- Docs của Enemy System - AI behavior

**Muốn chỉnh sửa UI/menu?**
- Docs của UI System - Implementation giao diện
- ShopUI_VI.md - Hệ thống shop

**Muốn thêm tính năng?**
- `10_Huong_Dan_Thuc_Hanh.md` - Tutorials thực tế
- `13_Vi_Du_Code.md` - Code snippets

**Checkpoint:**
- [ ] Đã hiểu kỹ các hệ thống đã chọn
- [ ] Đã xác định extension points trong code
- [ ] Biết class nào cần modify cho mục tiêu của mình

### Tuần 1-2: Implementation (liên tục)

**Tài nguyên:**
- `11_Khac_Phuc_Su_Co.md` - Vấn đề thường gặp
- `99_Tu_Dien_Thuat_Ngu.md` - Tham khảo thuật ngữ
- Docs hiện có cho các hệ thống cụ thể

**Best Practice:**
- Tạo thay đổi nhỏ và test thường xuyên
- Đọc code hiện có trước khi modify
- Dùng version control (Git) để backup

---

## 3. Hướng Dẫn Tác Vụ Nhanh

**Muốn hoàn thành tác vụ cụ thể? Tìm ở đây:**

### "Tôi muốn thay đổi cách player bắn"

**→ Đọc:** `02_He_Thong_Player_Day_Du.md` (Mục 5: Cơ Chế Bắn)
**→ Chỉnh sửa:** `Player_Archer.cs`
**→ Các biến cần thay đổi:**
- `shootRate` - Tốc độ bắn
- `force` - Lực của arrow
- `arrowDamage` - Sát thương mỗi arrow

### "Tôi muốn thêm enemy type mới"

**→ Đọc:** `10_Huong_Dan_Thuc_Hanh.md` (Guide: Thêm Enemy Mới)
**→ Các bước:**
1. Duplicate enemy prefab hiện có
2. Thay đổi sprite
3. Điều chỉnh stats trong Inspector
4. Cấu hình trong LevelWave

### "Tôi muốn chỉnh sửa UI/menu"

**→ Đọc:** Documentation của UI System
**→ Chỉnh sửa:** Scripts trong `Assets/_MonstersOut/Scripts/UI/`
**→ Các class chính:**
- `MenuManager.cs` - Main menu
- `Menu_Victory.cs` - Màn hình chiến thắng
- `MapControllerUI.cs` - Chọn màn chơi

### "Tôi muốn hiểu hệ thống máu"

**→ Đọc:**
- `02_He_Thong_Player_Day_Du.md` (Mục 7: Damage & Health)
- Docs của Enemy System (Implementation TakeDamage)
**→ Khái niệm chính:**
- `ICanTakeDamage` interface
- Enemy.cs base class
- HealthBarEnemyNew.cs

### "Tôi muốn thêm weapon upgrades"

**→ Đọc:**
- `02_He_Thong_Player_Day_Du.md` (Mục 10.3: Thêm Weapon Upgrades)
- `10_Huong_Dan_Thuc_Hanh.md` (Guide Upgrade System)
**→ Chỉnh sửa:**
- `UpgradedCharacterParameter.cs` - Lưu trữ stats
- `Player_Archer.cs` - Apply upgrades
- `ShopManager.cs` - Logic mua hàng

### "Tôi gặp lỗi và không biết tại sao"

**→ Đọc:** `11_Khac_Phuc_Su_Co.md`
**→ Tìm category của lỗi:**
- Vấn đề di chuyển
- Vấn đề bắn
- Vấn đề UI
- Lỗi Compilation
- Lỗi Build

### "Tôi cần code example cụ thể"

**→ Đọc:** `13_Vi_Du_Code.md`
**→ Categories:**
- Movement patterns
- Combat systems
- AI behaviors
- UI implementations
- Particle effects
- Sound effects

---

## 4. Bản Đồ Tài Liệu

### 📘 Fundamentals (Bắt Đầu Ở Đây)

| File | Mục đích | Thời gian đọc | Ưu tiên |
|------|---------|---------------|---------|
| **00_BAT_DAU_TU_DAY.md** | File này - lộ trình của bạn | 5 phút | ⭐⭐⭐ |
| **00_Unity_Co_Ban.md** | Unity basics từ zero | 2-3 giờ | ⭐⭐⭐ (người mới) |
| **01_Kien_Truc_Project.md** | Cấu trúc project & patterns | 30-40 phút | ⭐⭐⭐ |

### 🔧 System Documentation (Đi Sâu)

| File | Mục đích | Thời gian đọc | Khi nào đọc |
|------|---------|---------------|-------------|
| **02_He_Thong_Player_Day_Du.md** | Chi tiết Player mechanics | 45 phút | Khi chỉnh Player |
| **03_He_Thong_Enemy_Day_Du.md** | Enemy AI & behavior | 40 phút | Khi chỉnh Enemy |
| **04_He_Thong_UI_Day_Du.md** | User interface | 30 phút | Khi chỉnh UI |
| **05_Cac_Manager_Day_Du.md** | Các Manager classes | 35 phút | Hiểu game flow |
| **06_AI_System_Complete.md** | AI decision making | 30 phút | Chỉnh sửa AI nâng cao |

### 📚 Hướng Dẫn Thực Tế (How-To)

| File | Mục đích | Thời gian đọc | Khi nào dùng |
|------|---------|---------------|--------------|
| **10_Huong_Dan_Thuc_Hanh.md** | Tutorials từng bước | 15-30 phút/guide | Tạo thay đổi cụ thể |
| **11_Khac_Phuc_Su_Co.md** | Vấn đề thường gặp & cách sửa | 5-10 phút | Khi gặp lỗi |
| **13_Vi_Du_Code.md** | Snippets để copy-paste | 2-5 phút | Cần code nhanh |

### 📖 Tài Liệu Tham Khảo (Tra Cứu)

| File | Mục đích | Thời gian đọc | Khi nào dùng |
|------|---------|---------------|--------------|
| **99_Tu_Dien_Thuat_Ngu.md** | Định nghĩa thuật ngữ A-Z | 1-2 phút/từ | Không hiểu từ |
| **12_Tham_Chieu_Truc_Quan.md** | Diagrams & visuals | 10-15 phút | Người học bằng hình |
| **project-analysis_VI.md** | Phân tích kỹ thuật | 20-30 phút | Insight kỹ thuật sâu |

### 📂 Legacy Documentation (Tùy Chọn)

Nằm trong `/Documents/scripts/` - Documentation tiếng Việt gốc:
- AI.md, Controllers.md, Helpers.md, Managers.md, Player.md, UI.md
- Roadmap.md, Unity-Concepts.md, Workflow-Tasks.md
- (Nên dùng docs tiếng Anh mới, những file này cũ hơn)

---

## 5. Checklist Ngày Đầu Tiên

**Hoàn thành các tác vụ này trong ngày đầu để làm quen:**

### ✅ Setup Môi Trường (30 phút)

- [ ] Mở project trong Unity Editor
- [ ] Verify project load không có lỗi
- [ ] Nhấn nút Play, game chạy đúng
- [ ] Scene view navigation hoạt động (pan, zoom, rotate)
- [ ] Inspector hiển thị properties khi chọn objects

### ✅ Khám Phá Ban Đầu (30 phút)

- [ ] Tìm thư mục `Assets/_MonstersOut/Scripts/`
- [ ] Mở `GameManager.cs` trong code editor
- [ ] Mở `Player_Archer.cs` trong code editor
- [ ] Tìm một Prefab trong Project panel
- [ ] Xem Hierarchy trong chế độ Play

### ✅ Đọc Đầu Tiên (1-2 giờ)

**Chọn dựa trên kinh nghiệm của bạn:**

**Người Mới Hoàn Toàn:**
- [ ] Đọc `00_Unity_Co_Ban.md` phần 1-3
- [ ] Hiểu GameObject & Components
- [ ] Biết Prefab là gì

**Trung Cấp:**
- [ ] Đọc hoàn chỉnh `01_Kien_Truc_Project.md`
- [ ] Hiểu game flow và hệ thống
- [ ] Biết mỗi hệ thống nằm ở đâu

### ✅ Chỉnh Sửa Đầu Tiên (30 phút)

**Thử một trong các thay đổi đơn giản này:**

**Option A: Thay Đổi Fire Rate của Player**
1. Mở SampleScene trong Unity
2. Chọn Player GameObject trong Hierarchy
3. Tìm Player_Archer component trong Inspector
4. Đổi "Shoot Rate" từ 1.0 thành 0.5
5. Nhấn Play và quan sát bắn nhanh hơn

**Option B: Thay Đổi Máu Enemy**
1. Tìm Enemy prefab trong Project
2. Double-click để vào chế độ Prefab
3. Tìm Enemy component
4. Đổi giá trị "Health"
5. Thoát chế độ Prefab, Play và test

**Option C: Thay Đổi UI Text**
1. Chạy game ở chế độ Play
2. Mở Hierarchy khi đang chạy
3. Tìm UI text element
4. Dừng chế độ Play
5. Chọn text, thay đổi trong Inspector
6. Play lại để xem thay đổi

### ✅ Review Cuối Ngày

- [ ] Game vẫn chạy không có lỗi
- [ ] Đã hiểu workflow Unity cơ bản (edit → test → iterate)
- [ ] Đã tìm documentation cho tham khảo sau
- [ ] Đã bookmark docs hữu ích trong browser/notes

---

## 6. Nhận Trợ Giúp

### 🔍 Chiến Lược Tìm Kiếm

**Khi bạn có câu hỏi:**

1. **Kiểm Tra Glossary Trước:** `99_Tu_Dien_Thuat_Ngu.md`
   - Định nghĩa thuật ngữ nhanh
   - Khái niệm Unity thường gặp

2. **Kiểm Tra Troubleshooting:** `11_Khac_Phuc_Su_Co.md`
   - Error messages
   - Vấn đề thường gặp
   - Giải pháp đã test trong project này

3. **Tìm Trong Documentation:** Ctrl+F trong doc liên quan
   - Vấn đề Player → `02_He_Thong_Player_Day_Du.md`
   - Vấn đề Enemy → `03_He_Thong_Enemy_Day_Du.md`
   - v.v.

4. **Kiểm Tra Code Examples:** `13_Vi_Du_Code.md`
   - Giải pháp copy-paste
   - Code snippets hoạt động

### 📚 Tài Nguyên Bên Ngoài

**Unity Official:**
- Unity Manual: https://docs.unity3d.com/Manual/
- Unity Scripting API: https://docs.unity3d.com/ScriptReference/
- Unity Learn: https://learn.unity.com/

**Cộng Đồng:**
- Unity Forum: https://forum.unity.com/
- Stack Overflow: Tag [unity3d]
- Reddit: r/Unity3D, r/gamedev

### 🐛 Mẹo Debug

**Khi có gì đó bị hỏng:**

1. **Đọc Console Errors:**
   - Lỗi đỏ ngăn game chạy
   - Warning vàng là vấn đề tiềm ẩn
   - Double-click lỗi để nhảy đến dòng code

2. **Kiểm Tra Thay Đổi Gần Đây:**
   - Bạn đã modify gì cuối cùng?
   - Undo (Ctrl+Z) và test lại
   - Dùng Git để revert nếu cần

3. **Thêm Debug Logs:**
   ```csharp
   Debug.Log("Code này đang chạy!");
   Debug.Log("Health: " + currentHealth);
   ```

4. **Test Độc Lập:**
   - Tạm thời disable scripts khác
   - Test từng feature một
   - Đơn giản hóa đến khi nó hoạt động

### 💬 Hỏi Trợ Giúp

**Khi post câu hỏi:**

**Format Câu Hỏi Tốt:**
```
Tôi đang cố gắng làm gì:
- [Mô tả mục tiêu]

Tôi đã thử gì:
- [Các bước đã làm]
- [Code đã modify]

Điều gì đã xảy ra:
- [Kết quả thực tế]
- [Error message nếu có]

Kết quả mong đợi:
- [Điều gì nên xảy ra]

Chi tiết project:
- Unity version: [version]
- Lawn Defense: Monsters Out project
```

**Bao gồm:**
- Error messages cụ thể
- Code snippets (chỉ phần liên quan)
- Những gì bạn đã thử
- Unity và project version

---

## 7. Các Mốc Học Tập

**Track tiến độ của bạn:**

### 🥉 Bronze Level (Tuần 1-2)
- [ ] Hoàn thành Unity Fundamentals
- [ ] Chạy game thành công
- [ ] Tạo chỉnh sửa đơn giản đầu tiên
- [ ] Dùng Inspector để thay đổi giá trị

### 🥈 Silver Level (Tuần 3-4)
- [ ] Hiểu project architecture
- [ ] Chỉnh sửa player fire rate
- [ ] Thay đổi health/damage của enemy
- [ ] Thêm UI element

### 🥇 Gold Level (Tuần 5-8)
- [ ] Tạo enemy type mới
- [ ] Implement hiệu ứng weapon mới
- [ ] Chỉnh sửa game mechanics
- [ ] Build và test game

### 💎 Diamond Level (Liên tục)
- [ ] Thiết kế feature mới từ đầu
- [ ] Optimize vấn đề performance
- [ ] Đóng góp vào codebase
- [ ] Dạy developer khác

---

## 8. Lịch Học Tập Đề Xuất

### Học Full-Time (40 giờ/tuần)

**Tuần 1:**
- Thứ 2-4: Unity Fundamentals (hoàn thành)
- Thứ 5-6: Project Architecture
- Cuối tuần: Khám phá hands-on

**Tuần 2:**
- Thứ 2-3: Player System deep dive
- Thứ 4-5: Enemy System deep dive
- Thứ 6: UI System overview
- Cuối tuần: Thực hành chỉnh sửa

**Tuần 3-4:**
- Làm theo How-To Guides
- Implement custom features
- Build portfolio project

### Học Part-Time (10 giờ/tuần)

**Tuần 1-2:** Unity Fundamentals
**Tuần 3-4:** Project Architecture
**Tuần 5-6:** Player & Enemy Systems
**Tuần 7-8:** Chỉnh sửa thực tế
**Tuần 9-12:** Custom features

### Học Cuối Tuần (5 giờ/tuần)

**Tháng 1-2:** Fundamentals & Architecture
**Tháng 3:** System Deep Dives
**Tháng 4+:** Custom development

---

## 9. Mục Tiêu Project

**Bạn có thể build gì với kiến thức này?**

### Ngắn Hạn (1-2 tuần)
- Điều chỉnh game balance (damage, health, speed)
- Thay đổi UI text và layout
- Thêm sound effects đơn giản
- Chỉnh sửa enemy behavior hiện có

### Trung Hạn (1-2 tháng)
- Tạo enemy types mới
- Thiết kế màn chơi mới
- Implement weapon types mới
- Thêm power-up items
- Tạo UI screens mới

### Dài Hạn (3+ tháng)
- Thiết kế game modes mới
- Implement multiplayer (nếu tham vọng)
- Tạo level editor
- Build game hoàn toàn mới dùng cùng architecture

---

## 10. Mẹo Thành Công

**🎯 Đặt Mục Tiêu Rõ Ràng**
- "Tôi muốn thêm fire arrow" ✅
- "Tôi muốn học Unity" ❌ (quá mơ hồ)

**📝 Ghi Chú**
- Giữ learning journal
- Document các chỉnh sửa của bạn
- Lưu code snippets hữu ích

**🔁 Iterate Nhanh**
- Tạo thay đổi nhỏ
- Test ngay lập tức
- Sửa lỗi trước khi tiếp tục

**🤝 Đặt Câu Hỏi**
- Không có câu hỏi nào quá cơ bản
- Cộng đồng sẵn sàng giúp đỡ
- Tìm kiếm trước khi hỏi (có thể đã có câu trả lời)

**💪 Kiên Nhẫn**
- Học tập cần thời gian
- Lỗi là điều bình thường
- Ăn mừng những thắng lợi nhỏ

**🔄 Thực Hành Đều Đặn**
- 30 phút mỗi ngày > 5 giờ một lần
- Tính nhất quán xây dựng kỹ năng
- Hands-on tốt hơn đọc

---

## 11. Tiếp Theo Là Gì?

**Chọn lộ trình của bạn:**

**Lộ Trình A: Người Mới (Chưa bao giờ dùng Unity)**
→ Đi đến `00_Unity_Co_Ban.md`

**Lộ Trình B: Trung Cấp (Có kinh nghiệm Unity)**
→ Đi đến `01_Kien_Truc_Project.md`

**Lộ Trình C: Nâng Cao (Developer có kinh nghiệm)**
→ Browse system docs dựa trên quan tâm

**Lộ Trình D: Tác Vụ Cụ Thể (Biết muốn làm gì)**
→ Kiểm tra Mục 3 (Hướng Dẫn Tác Vụ Nhanh) ở trên

---

## 12. Triết Lý Documentation

**Tại sao documentation này tồn tại:**

### Cho Người Mới
- Không giả định kiến thức trước
- Giải thích từng bước
- Diagrams trực quan
- Ví dụ thực tế

### Cho Chuyên Nghiệp
- Điều hướng nhanh
- Độ chính xác kỹ thuật
- Architecture patterns
- Extension points

### Cho Tất Cả Mọi Người
- Nội dung có thể search
- Cấu trúc nhất quán
- Ví dụ code thực
- Giải pháp đã test

---

**Sẵn sàng bắt đầu?** Chọn lộ trình ở trên và bắt đầu hành trình!

**Có câu hỏi?** Kiểm tra `99_Tu_Dien_Thuat_Ngu.md` cho thuật ngữ, `11_Khac_Phuc_Su_Co.md` cho vấn đề.

**Chúc may mắn! 🚀**

---

**Phiên Bản Tài Liệu:** 2.0
**Cập Nhật Lần Cuối:** Tháng 10, 2025
**Được Duy Trì Bởi:** Project Documentation Team
