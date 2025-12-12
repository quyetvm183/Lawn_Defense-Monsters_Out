# 🎮 HƯỚNG DẪN BUILD GAME - Lawn Defense: Monsters Out

## 📋 **MỤC LỤC**
1. [Chuẩn bị trước khi build](#chuẩn-bị-trước-khi-build)
2. [Build Windows (PC)](#build-windows-pc)
3. [Build Android (APK)](#build-android-apk)
4. [Build WebGL (Browser)](#build-webgl-browser)
5. [Test build](#test-build)
6. [Khắc phục sự cố](#khắc-phục-sự-cố)

---

## 🔍 **CHUẨN BỊ TRƯỚC KHI BUILD**

### ✅ **Bước 1: Kiểm tra Scenes**

1. **Mở Build Settings:**
   ```
   File → Build Settings (Ctrl+Shift+B)
   ```

2. **Kiểm tra Scenes In Build:**
   ```
   ✅ Scene 0: Init Scene
   ✅ Scene 1: Menu
   ✅ Scene 2: Playing
   ```

3. **Nếu thiếu scene:**
   ```
   - Mở scene cần thêm
   - File → Build Settings
   - Click "Add Open Scenes"
   ```

### ✅ **Bước 2: Test trong Editor**

```
1. Nhấn Play (Ctrl+P)
2. Chơi thử vài level
3. Kiểm tra:
   ✓ Không có lỗi Console
   ✓ UI hiển thị đúng
   ✓ Sound hoạt động
   ✓ Game flow mượt mà
```

### ✅ **Bước 3: Save Project**

```
File → Save Project (Ctrl+S)
```

---

## 💻 **BUILD WINDOWS (PC)**

### **Bước 1: Chọn Platform**

1. **Mở Build Settings:**
   ```
   File → Build Settings (Ctrl+Shift+B)
   ```

2. **Chọn Windows:**
   ```
   Platform → PC, Mac & Linux Standalone
   → Click "Switch Platform" (nếu chưa chọn)
   ```

### **Bước 2: Player Settings**

1. **Mở Player Settings:**
   ```
   Build Settings → Click "Player Settings" (góc dưới bên trái)
   ```

2. **Cấu hình cơ bản:**
   ```
   ┌─ Company Name ──────────────┐
   │ YourStudioName              │ (Tên team/studio)
   └─────────────────────────────┘

   ┌─ Product Name ──────────────┐
   │ Lawn Defense Monsters Out   │ (Tên game)
   └─────────────────────────────┘

   ┌─ Version ───────────────────┐
   │ 1.0.0                       │ (Phiên bản)
   └─────────────────────────────┘

   ┌─ Default Icon ──────────────┐
   │ [Kéo icon.png vào đây]      │ (Icon game 1024x1024)
   └─────────────────────────────┘
   ```

3. **Resolution and Presentation:**
   ```
   ✓ Fullscreen Mode: Fullscreen Window
   ✓ Default Screen Width: 1920
   ✓ Default Screen Height: 1080
   ✓ Run In Background: ✓ (check)
   ```

4. **Splash Image (Tùy chọn):**
   ```
   ✓ Show Splash Screen: ✓
   → Thêm logo của bạn
   ```

### **Bước 3: Build**

1. **Quay lại Build Settings:**
   ```
   File → Build Settings
   ```

2. **Chọn Architecture:**
   ```
   Architecture: x86_64 (64-bit)
   ```

3. **Click Build:**
   ```
   1. Click "Build"
   2. Chọn folder lưu (ví dụ: E:\Builds\Windows\)
   3. Đặt tên: "LawnDefense.exe"
   4. Click "Save"
   ```

4. **Chờ build:**
   ```
   ⏳ Unity đang build...
   ⏳ Có thể mất 2-10 phút
   ```

5. **Hoàn tất:**
   ```
   ✅ Build thành công!
   → Folder sẽ chứa:
      - LawnDefense.exe
      - LawnDefense_Data/
      - UnityPlayer.dll
      - UnityCrashHandler64.exe
   ```

### **Bước 4: Test Build**

```
1. Mở folder build
2. Double-click LawnDefense.exe
3. Chơi thử game
4. Kiểm tra mọi thứ hoạt động
```

---

## 📱 **BUILD ANDROID (APK)**

### **Bước 1: Cài đặt Android Build Support**

1. **Kiểm tra đã cài chưa:**
   ```
   Unity Hub → Installs → [Unity version] → ⚙️ Settings
   → Modules → ✓ Android Build Support
   ```

2. **Nếu chưa cài:**
   ```
   → Add Modules
   → ✓ Android Build Support
      ✓ Android SDK & NDK Tools
      ✓ OpenJDK
   → Install
   ```

### **Bước 2: Switch Platform**

1. **Build Settings:**
   ```
   File → Build Settings
   ```

2. **Chọn Android:**
   ```
   Platform → Android
   → Click "Switch Platform"
   → Chờ Unity switch (1-3 phút)
   ```

### **Bước 3: Player Settings - Android**

1. **Other Settings:**
   ```
   ┌─ Package Name ──────────────────────────┐
   │ com.YourStudio.LawnDefense              │
   └─────────────────────────────────────────┘

   ┌─ Version ───────────────────────────────┐
   │ 1.0.0                                   │
   └─────────────────────────────────────────┘

   ┌─ Bundle Version Code ───────────────────┐
   │ 1                                       │
   └─────────────────────────────────────────┘

   ┌─ Minimum API Level ─────────────────────┐
   │ Android 5.0 'Lollipop' (API level 21)  │
   └─────────────────────────────────────────┘

   ┌─ Target API Level ──────────────────────┐
   │ Automatic (highest installed)           │
   └─────────────────────────────────────────┘
   ```

2. **Configuration:**
   ```
   ✓ Scripting Backend: IL2CPP
   ✓ ARM64: ✓ (check)
   ✓ ARMv7: ✓ (check - tùy chọn)
   ```

3. **Publishing Settings:**
   ```
   ✓ Create Keystore (lần đầu):
      → Tạo keystore mới
      → Đặt password
      → LƯU GIỮ PASSWORD NÀY!

   Hoặc:

   ✓ Browse Keystore (nếu đã có)
      → Chọn file .keystore
      → Nhập password
   ```

### **Bước 4: Build APK**

1. **Build Settings:**
   ```
   File → Build Settings
   ```

2. **Chọn Build Type:**
   ```
   Build System: Gradle

   ✓ Export Project: ☐ (uncheck)
   ✓ Build App Bundle (Google Play): ☐ (uncheck để tạo APK)

   Compression Method: LZ4
   ```

3. **Click Build:**
   ```
   1. Click "Build"
   2. Chọn folder: E:\Builds\Android\
   3. Đặt tên: "LawnDefense.apk"
   4. Save
   ```

4. **Chờ build:**
   ```
   ⏳ Build Android...
   ⏳ Mất 5-20 phút (lần đầu lâu hơn)
   ```

5. **Hoàn tất:**
   ```
   ✅ Build thành công!
   → File: LawnDefense.apk
   ```

### **Bước 5: Cài lên điện thoại**

**Cách 1: USB:**
```
1. Bật USB Debugging trên điện thoại
2. Kết nối USB
3. Copy file APK vào điện thoại
4. Mở File Manager → Cài đặt APK
5. Cho phép "Unknown Sources"
```

**Cách 2: Google Drive/Email:**
```
1. Upload APK lên Drive
2. Tải về điện thoại
3. Cài đặt
```

---

## 🌐 **BUILD WEBGL (BROWSER)**

### **Bước 1: Cài WebGL Build Support**

```
Unity Hub → Installs → Add Modules
→ ✓ WebGL Build Support
→ Install
```

### **Bước 2: Switch Platform**

```
File → Build Settings
→ Platform: WebGL
→ Switch Platform
```

### **Bước 3: Player Settings - WebGL**

```
┌─ Company Name ──────────────┐
│ YourStudio                  │
└─────────────────────────────┘

┌─ Product Name ──────────────┐
│ Lawn Defense                │
└─────────────────────────────┘

Resolution:
✓ Default Canvas Width: 1920
✓ Default Canvas Height: 1080
```

### **Bước 4: Build**

```
1. Build Settings → Build
2. Chọn folder: E:\Builds\WebGL\
3. Đặt tên folder: "LawnDefense"
4. Build
```

### **Bước 5: Upload lên web**

**Itch.io (Miễn phí):**
```
1. Đăng ký tài khoản itch.io
2. Create new project
3. Upload folder build WebGL
4. Set to Public
5. Chia sẻ link!
```

---

## 🧪 **TEST BUILD**

### **Checklist:**

```
Windows Build:
☐ Game chạy được
☐ Resolution đúng
☐ Sound hoạt động
☐ Input (keyboard/mouse) hoạt động
☐ Save/Load hoạt động
☐ Không crash

Android Build:
☐ Game chạy trên điện thoại
☐ Touch input hoạt động
☐ Orientation đúng
☐ Không lag
☐ Sound hoạt động
☐ Không crash khi minimize/restore

WebGL Build:
☐ Game load được trong browser
☐ Không bị CORS error
☐ Input hoạt động
☐ Sound hoạt động (sau khi click)
☐ Responsive
```

---

## 🐛 **KHẮC PHỤC SỰ CỐ**

### **Lỗi 1: "No scenes in build"**

```
❌ Lỗi: No valid scenes to build
✅ Fix:
   File → Build Settings
   → Add Open Scenes
```

### **Lỗi 2: "Android SDK not found"**

```
❌ Lỗi: Android SDK path not found
✅ Fix:
   Edit → Preferences → External Tools
   → Android → SDK Path
   → Browse đến SDK folder
   (Thường: C:\Program Files\Unity\Hub\Editor\[version]\Editor\Data\PlaybackEngines\AndroidPlayer\SDK)
```

### **Lỗi 3: "Keystore password incorrect"**

```
❌ Lỗi: Wrong keystore password
✅ Fix:
   Tạo keystore mới:
   Player Settings → Publishing Settings
   → Keystore Manager
   → Create New → Anywhere
```

### **Lỗi 4: Build quá lớn**

```
❌ Build > 500 MB
✅ Fix:
   Player Settings → Other Settings
   → Managed Stripping Level: Medium/High
   → Code Optimization: Size
   → Compression Method: LZ4HC
```

### **Lỗi 5: Crash khi mở**

```
❌ Game crash ngay khi mở
✅ Check:
   1. Xem Console trong Unity Editor
   2. Sửa lỗi
   3. Test trong Editor trước
   4. Build lại
```

---

## 📦 **PHÂN PHỐI GAME**

### **Windows:**
```
Nén folder build:
→ LawnDefense.zip
→ Upload lên:
   - Google Drive
   - Itch.io
   - GameJolt
   - Steam (cần Steamworks SDK)
```

### **Android:**
```
Upload APK lên:
→ Google Play Console (official)
→ Itch.io
→ APKPure
→ Hoặc share trực tiếp file APK
```

### **WebGL:**
```
Upload lên:
→ Itch.io (dễ nhất)
→ GitHub Pages
→ Netlify
→ Vercel
```

---

## 💰 **BUILD SETTINGS KHUYẾN NGHỊ**

### **Development Build (Test):**
```
✓ Development Build: ✓
✓ Autoconnect Profiler: ✓
✓ Script Debugging: ✓
→ Dùng để debug, tìm lỗi
```

### **Release Build (Phát hành):**
```
✓ Development Build: ☐
✓ Compression Method: LZ4HC
✓ Code Optimization: Size
→ Dùng để release cho người chơi
```

---

## 🎯 **CHECKLIST CUỐI CÙNG**

```
Trước khi release:
☐ Test đầy đủ mọi tính năng
☐ Không có lỗi Console
☐ Sound/Music hoạt động
☐ UI hiển thị đúng trên nhiều resolution
☐ Game không crash
☐ Save/Load hoạt động
☐ Performance tốt (60 FPS)
☐ Xóa code debug/cheat
☐ Version number đúng
☐ Icon/Splash screen đẹp
```

---

## 📄 **FILE CẦN GIỮ**

```
⚠️ LƯU GIỮ NHỮNG FILE NÀY:

Windows Build:
→ Toàn bộ folder build
→ Không tách rời các file

Android Build:
→ File .keystore (QUAN TRỌNG!)
→ Password keystore (GHI CHÚ!)
→ File .apk

Nếu mất keystore → KHÔNG thể update app trên Google Play!
```

---

**✅ XONG! Chúc bạn build game thành công!** 🎮

*Nếu gặp lỗi, đọc phần Khắc phục sự cố hoặc search lỗi trên Google*
