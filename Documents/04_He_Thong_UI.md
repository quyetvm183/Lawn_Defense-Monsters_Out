# Hệ Thống UI - Hướng Dẫn Đầy Đủ

---
**🌐 Ngôn ngữ:** Tiếng Việt
**📄 File gốc:** [04_UI_System_Complete.md](04_UI_System_Complete.md)
**🔄 Cập nhật lần cuối:** 2025-01-30
---

> **Dành cho**: Người mới bắt đầu đã hoàn thành Unity Fundamentals
> **Thời gian đọc**: 30-40 phút
> **Yêu cầu**: 00_Unity_Co_Ban.md, 01_Kien_Truc_Project.md

---

## Mục Lục
1. [Tổng Quan Hệ Thống](#tổng-quan-hệ-thống)
2. [Kiến Trúc UI](#kiến-trúc-ui)
3. [Hệ Thống Main Menu](#hệ-thống-main-menu)
4. [In-Game HUD](#in-game-hud)
5. [Menu Manager (Pause, Victory, Fail)](#menu-manager)
6. [Hệ Thống Health Bar](#hệ-thống-health-bar)
7. [Hệ Thống Scene Loading](#hệ-thống-scene-loading)
8. [Global Values](#global-values)
9. [Cách Tạo Custom UI](#cách-tạo-custom-ui)
10. [Các Vấn Đề Thường Gặp & Giải Pháp](#các-vấn-đề-thường-gặp--giải-pháp)

---

## Tổng Quan Hệ Thống

### UI System Là Gì?

**UI System** (hệ thống giao diện người dùng) quản lý tất cả các yếu tố giao diện trực quan mà người chơi tương tác:
- **Main Menu**: Màn hình tiêu đề, chọn level, cài đặt, shop
- **In-Game HUD** (Heads-Up Display): Thanh máu, số coin, tiến trình wave
- **Pause Menu**: Tạm dừng/tiếp tục, bật/tắt âm thanh/nhạc nền
- **Victory/Fail Screens**: Màn hình hoàn thành level với rating sao
- **Loading Screens**: Hiệu ứng chuyển cảnh

### Tại Sao Điều Này Quan Trọng?

UI System cung cấp **phản hồi cho người chơi** và **điều khiển game**. Hiểu về UI giúp bạn:
- Thêm menu và màn hình mới
- Tùy chỉnh các yếu tố HUD
- Triển khai hệ thống save/load
- Tạo tutorial overlay
- Debug các vấn đề về trạng thái game

### Sơ Đồ Kiến Trúc UI System

```
┌─────────────────────────────────────────────────────┐
│                  UI SYSTEM                          │
└───────────┬─────────────────────────────────────────┘
            │
       ┌────┼──────┬──────────┬──────────┐
       │    │      │          │          │
       ▼    ▼      ▼          ▼          ▼
┌──────────┐ ┌──────┐ ┌──────────┐ ┌─────────┐ ┌────────┐
│MainMenu  │ │Menu  │ │  UI_UI   │ │Victory  │ │Health  │
│HomeScene │ │Manager│ │  (HUD)   │ │/Fail    │ │Bars    │
└──────────┘ └──────┘ └──────────┘ └─────────┘ └────────┘
     │           │          │             │           │
     ▼           ▼          ▼             ▼           ▼
Map, Shop   Pause, Load  Sliders,    Star Rating  Follow
Settings    Scene Mgmt   Coin, Mana   Buttons     Enemy
```

### Các File Chính

| File | Vị Trí | Mục Đích |
|------|--------|----------|
| `MainMenuHomeScene.cs` | `Assets/_MonstersOut/Scripts/UI/` | Controller cho main menu |
| `MenuManager.cs` | `Assets/_MonstersOut/Scripts/UI/` | Hệ thống menu trong game |
| `UI_UI.cs` | `Assets/_MonstersOut/Scripts/UI/` | Controller cho in-game HUD |
| `Menu_Victory.cs` | `Assets/_MonstersOut/Scripts/UI/` | Màn hình chiến thắng với sao |
| `HealthBarEnemyNew.cs` | `Assets/_MonstersOut/Scripts/UI/` | Thanh máu của enemy |
| `GlobalValue.cs` | `Assets/_MonstersOut/Scripts/` | Dữ liệu lưu và cài đặt |

---

## Kiến Trúc UI

### Cơ Bản Về Unity UI System

Unity sử dụng hệ thống **Canvas** cho UI:

```
Canvas (Screen Space - Overlay)
    │
    ├─ Panel (Background)
    │   └─ Button (Play)
    │       └─ Text (Label)
    │
    ├─ Slider (Health Bar)
    │   ├─ Background
    │   ├─ Fill Area
    │   └─ Handle Slide Area
    │
    └─ Text (Coin Counter)
```

**Canvas Render Modes** (chế độ render):
- **Screen Space - Overlay**: UI được render trên mọi thứ (phổ biến nhất)
- **Screen Space - Camera**: UI được render bởi camera (cho phép depth)
- **World Space**: UI tồn tại trong không gian 3D (hiếm)

### Cấu Trúc UI Của Project

```
Scene: Menu
└─ Canvas
    ├─ MainPanel
    ├─ MapUI (ẩn mặc định)
    ├─ ShopUI (ẩn mặc định)
    ├─ SettingsUI (ẩn mặc định)
    └─ LoadingUI (ẩn mặc định)

Scene: Playing
└─ Canvas
    ├─ StartUI (đếm ngược)
    ├─ UI (HUD - máu, coin, wave)
    ├─ PauseUI (ẩn)
    ├─ VictoryUI (ẩn)
    ├─ FailUI (ẩn)
    └─ LoadingUI (ẩn)
```

### IListener Pattern Cho UI

**Vấn đề**: UI làm sao biết khi trạng thái game thay đổi (GameOver, Victory)?

**Giải pháp**: **Observer Pattern** thông qua interface IListener

```csharp
public interface IListener
{
    void IPlay();       // Được gọi khi game bắt đầu
    void ISuccess();    // Được gọi khi thắng level
    void IGameOver();   // Được gọi khi thua level
    void IPause();      // Được gọi khi tạm dừng
    void IUnPause();    // Được gọi khi tiếp tục
    // ... các method khác
}
```

**MenuManager** implement IListener:
```csharp
public class MenuManager : MonoBehaviour, IListener
{
    public void ISuccess()
    {
        StartCoroutine(VictoryCo());  // Hiển thị màn hình chiến thắng
    }

    public void IGameOver()
    {
        StartCoroutine(GameOverCo());  // Hiển thị màn hình thua
    }
}
```

**Cách Hoạt Động**:
1. MenuManager đăng ký với GameManager khi start
2. GameManager gọi `ISuccess()` khi tất cả enemy chết
3. MenuManager tự động hiển thị màn hình chiến thắng

---

## Hệ Thống Main Menu

### Tổng Quan MainMenuHomeScene

**File**: `MainMenuHomeScene.cs`

**Mục đích**: Điều khiển màn hình tiêu đề và main menu

**Trách nhiệm**:
- Hiển thị/ẩn các panel UI (Map, Shop, Settings)
- Load các scene game
- Hiển thị số coin
- Xử lý bật/tắt âm thanh/nhạc nền
- Liên kết mạng xã hội

### Sơ Đồ Luồng Scene

```
Game Start
    │
    ▼
┌─────────────┐
│ MAIN MENU   │
└──────┬──────┘
       │
   ┌───┼────┬──────┬─────────┐
   │   │    │      │         │
   ▼   ▼    ▼      ▼         ▼
 Play Map  Shop Settings  Tutorial
   │
   ▼
┌─────────────┐
│ LOADING     │
└──────┬──────┘
       │
       ▼
┌─────────────┐
│ PLAYING     │
└──────┬──────┘
       │
   ┌───┴───┐
   │       │
   ▼       ▼
Victory  GameOver
   │       │
   └───┬───┘
       │
    ┌──┴──┐
    │     │
    ▼     ▼
  Retry Home
```

### Awake() Method

Tại `MainMenuHomeScene.cs:26-38`

```csharp
void Awake()
{
    // Đặt singleton instance
    Instance = this;

    // Ẩn tất cả panel UI khi start
    if (Loading != null)
        Loading.SetActive(false);
    if (MapUI != null)
        MapUI.SetActive(false);
    if (Settings)
        Settings.SetActive(false);
    if (ShopUI)
        ShopUI.SetActive(false);
}
```

**Tại Sao?**
- Tất cả panel bắt đầu ở trạng thái ẩn
- Chỉ hiển thị panel khi người chơi click button
- Tránh rối mắt khi startup

### Start() Coroutine

Tại `MainMenuHomeScene.cs:58-74`

```csharp
IEnumerator Start()
{
    // Kiểm tra và áp dụng cài đặt âm thanh/nhạc nền
    CheckSoundMusic();

    // Nếu lần đầu mở menu
    if (GlobalValue.isFirstOpenMainMenu)
    {
        GlobalValue.isFirstOpenMainMenu = false;

        // Tạm dừng nhạc nền
        SoundManager.Instance.PauseMusic(true);

        // Phát âm thanh intro
        SoundManager.PlaySfx(SoundManager.Instance.beginSoundInMainMenu);

        // Đợi intro kết thúc
        yield return new WaitForSeconds(
            SoundManager.Instance.beginSoundInMainMenu.length
        );

        // Tiếp tục nhạc nền
        SoundManager.Instance.PauseMusic(false);
        SoundManager.PlayMusic(SoundManager.Instance.musicsGame);
    }

    // Ẩn banner quảng cáo (nếu có hệ thống ads)
    if (AdsManager.Instance)
        AdsManager.Instance.ShowAdmobBanner(false);
}
```

**Giải Thích Luồng**:
```
Frame 1: Start() bắt đầu
         └─ CheckSoundMusic()

Frame 2: isFirstOpenMainMenu? CÓ
         ├─ Đặt thành false (không phát lại nữa)
         ├─ Tạm dừng nhạc
         └─ Phát âm thanh intro (3 giây)

Frame 180: (3 giây @ 60fps)
         ├─ Intro kết thúc
         ├─ Tiếp tục nhạc
         └─ Phát nhạc nền

Frame 181+: Trạng thái menu bình thường
```

### Update() Method

Tại `MainMenuHomeScene.cs:76-85`

```csharp
void Update()
{
    // Liên tục kiểm tra trạng thái âm thanh/nhạc
    CheckSoundMusic();

    // Cập nhật hiển thị coin trên tất cả text element
    foreach (var ct in coinTxt)
    {
        ct.text = GlobalValue.SavedCoins + "";
    }
}
```

**Tại Sao Mỗi Frame?**
- Cài đặt âm thanh/nhạc có thể thay đổi ở scene khác
- Số coin cập nhật khi người chơi kiếm được coin
- Luôn hiển thị giá trị đúng

### Mở Panel

**OpenMap() Method** (`MainMenuHomeScene.cs:87-101`)

```csharp
public void OpenMap(bool open)
{
    // Phát âm thanh click
    SoundManager.Click();

    // Gọi coroutine để xử lý transition
    StartCoroutine(OpenMapCo(open));
}

IEnumerator OpenMapCo(bool open)
{
    yield return null;  // Đợi một frame

    // Fade sang màu đen
    BlackScreenUI.instance.Show(0.2f);

    // Bật/tắt map UI
    MapUI.SetActive(open);

    // Fade từ màu đen
    BlackScreenUI.instance.Hide(0.2f);
}
```

**Hiệu Ứng Trực Quan**:
```
Frame 100: OpenMap(true) được gọi
           └─ Phát âm thanh click

Frame 101: OpenMapCo() bắt đầu
           └─ Đợi một frame

Frame 102: BlackScreenUI.Show(0.2f)
           └─ Màn hình fade sang đen (12 frame @ 60fps)

Frame 114: MapUI.SetActive(true)
           └─ Panel map được bật

Frame 115: BlackScreenUI.Hide(0.2f)
           └─ Màn hình fade từ đen (12 frame)

Frame 127: Map hiển thị hoàn toàn
```

**Tại Sao Dùng BlackScreenUI?**
- Chuyển tiếp trực quan mượt mà
- Che giấu việc swap panel tức thời
- Tạo tính chuyên nghiệp

### Bật/Tắt Âm Thanh/Nhạc

**TurnSound() Method** (`MainMenuHomeScene.cs:128-135`)

```csharp
public void TurnSound()
{
    // Toggle trạng thái âm thanh
    GlobalValue.isSound = !GlobalValue.isSound;

    // Cập nhật hình ảnh button
    soundImage.sprite = GlobalValue.isSound ? soundImageOn : soundImageOff;

    // Đặt volume âm thanh (1 = full, 0 = tắt)
    SoundManager.SoundVolume = GlobalValue.isSound ? 1 : 0;
}
```

**TurnMusic() Method** (`MainMenuHomeScene.cs:137-144`)

```csharp
public void TurnMusic()
{
    // Toggle trạng thái nhạc
    GlobalValue.isMusic = !GlobalValue.isMusic;

    // Cập nhật hình ảnh button
    musicImage.sprite = GlobalValue.isMusic ? musicImageOn : musicImageOff;

    // Đặt volume nhạc
    SoundManager.MusicVolume = GlobalValue.isMusic
        ? SoundManager.Instance.musicsGameVolume
        : 0;
}
```

**Cách Hoạt Động**:
1. User click button âm thanh
2. `TurnSound()` được gọi qua Button OnClick event
3. Toggle `GlobalValue.isSound` (true ↔ false)
4. Cập nhật sprite button (icon loa bật/tắt)
5. Đặt `SoundManager.SoundVolume` (1 hoặc 0)

**Tại Sao Dùng GlobalValue?**
- Duy trì qua các scene
- Được lưu vào PlayerPrefs
- Chia sẻ với tất cả script

---

## In-Game HUD

### Tổng Quan UI_UI

**File**: `UI_UI.cs`

**Mục đích**: Hiển thị thông số trong game (máu, coin, tiến trình wave)

**Thành phần**:
- Thanh máu player + text
- Thanh máu enemy + text (cho boss fight)
- Thanh tiến trình wave
- Bộ đếm coin
- Bộ đếm mana
- Tên level

### Kiến Trúc UI_UI

```
UI_UI GameObject
    │
    ├─ Player Health Section
    │   ├─ Slider (healthSlider)
    │   └─ Text (health)
    │
    ├─ Enemy Health Section
    │   ├─ Slider (enemyHealthSlider)
    │   └─ Text (enemyHealth)
    │
    ├─ Wave Progress Section
    │   └─ Slider (enemyWavePercentSlider)
    │
    └─ Counters
        ├─ Text (coinTxt)
        ├─ Text (manaTxt)
        └─ Text (levelName)
```

### Start() Method

Tại `UI_UI.cs:32-41`

```csharp
private void Start()
{
    // Khởi tạo giá trị mặc định
    healthValue = 1;          // 100% máu
    enemyWaveValue = 0;       // 0% tiến trình wave

    // Reset các slider
    healthSlider.value = 1;
    enemyWavePercentSlider.value = 0;

    // Đặt tên level
    levelName.text = "Level " + GlobalValue.levelPlaying;
}
```

**Trạng Thái Ban Đầu**:
```
healthSlider:             [████████████████████] 100%
enemyWavePercentSlider:   [                    ] 0%
levelName:                "Level 1"
coinTxt:                  "0"
manaTxt:                  "0"
```

### Update() Method (Smooth Interpolation)

Tại `UI_UI.cs:43-52`

```csharp
private void Update()
{
    // Lerp mượt mà slider máu đến giá trị mục tiêu
    healthSlider.value = Mathf.Lerp(
        healthSlider.value,     // Hiện tại
        healthValue,            // Mục tiêu
        lerpSpeed * Time.deltaTime  // Tốc độ
    );

    // Lerp mượt mà slider máu enemy
    enemyHealthSlider.value = Mathf.Lerp(
        enemyHealthSlider.value,
        enemyHealthValue,
        lerpSpeed * Time.deltaTime
    );

    // Lerp mượt mà tiến trình wave
    enemyWavePercentSlider.value = Mathf.Lerp(
        enemyWavePercentSlider.value,
        enemyWaveValue,
        lerpSpeed * Time.deltaTime
    );

    // Cập nhật text (tức thời, không cần lerp)
    coinTxt.text = GlobalValue.SavedCoins + "";
    manaTxt.text = LevelManager.Instance.mana + "";
}
```

**Tại Sao Dùng Mathf.Lerp()?**

**Không Có Lerp** (tức thời):
```
healthValue thay đổi: 1.0 → 0.5
healthSlider nhảy:  ████████████ → ██████
                    (tức thời, gây shock)
```

**Có Lerp** (mượt mà):
```
Frame 100: healthValue = 0.5
           healthSlider = 1.0

Frame 101: healthSlider = Lerp(1.0, 0.5, 0.1) = 0.95
Frame 102: healthSlider = Lerp(0.95, 0.5, 0.1) = 0.90
Frame 103: healthSlider = Lerp(0.90, 0.5, 0.1) = 0.86
...
Frame 120: healthSlider ≈ 0.5

Kết quả: Animation mượt trong ~20 frame
```

**Tính Toán Lerp Speed**:
```csharp
lerpSpeed = 1;  // Giá trị mặc định
Time.deltaTime = 0.0166f;  // Ở 60 FPS

lerpSpeed * Time.deltaTime = 1 * 0.0166 = 0.0166

Mỗi frame: Lerp(current, target, 0.0166)
           → Di chuyển 1.66% về phía mục tiêu mỗi frame
           → Đạt mục tiêu trong ~60 frame (1 giây)
```

### UpdateHealthbar() Method

Tại `UI_UI.cs:54-67`

```csharp
public void UpdateHealthbar(float currentHealth, float maxHealth,
                           HEALTH_CHARACTER healthBarType)
{
    // Cập nhật máu player
    if (healthBarType == HEALTH_CHARACTER.PLAYER)
    {
        // Tính phần trăm (0.0 đến 1.0)
        healthValue = Mathf.Clamp01(currentHealth / maxHealth);

        // Cập nhật text (ví dụ: "50/100")
        health.text = (int)currentHealth + "/" + (int)maxHealth;
    }
    // Cập nhật máu enemy (cho boss battle)
    else if (healthBarType == HEALTH_CHARACTER.ENEMY)
    {
        enemyHealthValue = Mathf.Clamp01(currentHealth / maxHealth);
        enemyHealth.text = (int)currentHealth + "/" + (int)maxHealth;
    }
}
```

**Cách Được Gọi**:

```csharp
// Trong TheFortrest.cs (máu pháo đài)
MenuManager.Instance.UpdateHealthbar(
    currentHealth,    // 75
    maxHealth,        // 100
    HEALTH_CHARACTER.PLAYER
);

// Kết quả:
healthValue = 75 / 100 = 0.75
health.text = "75/100"
healthSlider sẽ lerp đến 0.75 (75%)
```

**Giải Thích Mathf.Clamp01()**:
```csharp
Mathf.Clamp01(value)  // Giới hạn giữa 0 và 1

Ví dụ:
Mathf.Clamp01(0.5)  = 0.5   ✓
Mathf.Clamp01(1.2)  = 1.0   (bị giới hạn)
Mathf.Clamp01(-0.3) = 0.0   (bị giới hạn)

Mục đích: Ngăn slider xuống dưới 0% hoặc lên trên 100%
```

### UpdateEnemyWavePercent() Method

Tại `UI_UI.cs:69-73`

```csharp
public void UpdateEnemyWavePercent(float currentSpawn, float maxValue)
{
    // Tính phần trăm tiến trình wave
    enemyWaveValue = Mathf.Clamp01(currentSpawn / maxValue);
}
```

**Ví Dụ Sử Dụng**:
```
Tổng enemy trong level: 50
Enemy đã spawn: 25

UpdateEnemyWavePercent(25, 50)
enemyWaveValue = 25 / 50 = 0.5

Slider tiến trình wave hiển thị 50%
```

---

## Menu Manager

### Tổng Quan MenuManager

**File**: `MenuManager.cs`

**Mục đích**: Quản lý các panel UI trong game và chuyển đổi trạng thái game

**Trách nhiệm**:
- Hiển thị/ẩn panel (Start, Victory, Fail, Pause)
- Xử lý tạm dừng/tiếp tục
- Load scene
- Lắng nghe sự kiện game (IListener)

### Singleton Pattern

Tại `MenuManager.cs:26-29`

```csharp
private void Awake()
{
    // Đặt singleton instance
    Instance = this;

    // Vô hiệu hóa tất cả panel UI khi start
    StartUI.SetActive(false);
    VictotyUI.SetActive(false);
    FailUI.SetActive(false);
    PauseUI.SetActive(false);
    LoadingUI.SetActive(false);
    CharacterContainer.SetActive(false);

    // Lấy component UI_UI
    uiControl = gameObject.GetComponentInChildren<UI_UI>(true);
}
```

**Tại Sao Dùng Singleton?**
```csharp
// Bất kỳ script nào cũng có thể truy cập MenuManager
MenuManager.Instance.UpdateHealthbar(...);
MenuManager.Instance.Pause();

// Không cần dùng FindObjectOfType hoặc reference
```

### Start() Coroutine

Tại `MenuManager.cs:39-58`

```csharp
IEnumerator Start()
{
    // Áp dụng cài đặt âm thanh/nhạc
    soundImage.sprite = GlobalValue.isSound ? soundImageOn : soundImageOff;
    musicImage.sprite = GlobalValue.isMusic ? musicImageOn : musicImageOff;

    if (!GlobalValue.isSound)
        SoundManager.SoundVolume = 0;
    if (!GlobalValue.isMusic)
        SoundManager.MusicVolume = 0;

    // Hiển thị UI đếm ngược
    StartUI.SetActive(true);

    // Đợi 1 giây (animation đếm ngược)
    yield return new WaitForSeconds(1);

    // Ẩn đếm ngược
    StartUI.SetActive(false);

    // Hiển thị UI game chính
    UI.SetActive(true);
    CharacterContainer.SetActive(true);

    // Bắt đầu game
    GameManager.Instance.StartGame();
}
```

**Sơ Đồ Luồng**:
```
Frame 1:   Start() bắt đầu
           └─ Áp dụng cài đặt
           └─ StartUI.SetActive(true)

Frame 1-60: UI đếm ngược hiển thị ("Ready... GO!")

Frame 60:  yield kết thúc
           ├─ StartUI.SetActive(false)
           ├─ UI.SetActive(true)
           └─ GameManager.Instance.StartGame()

Frame 61+: Game đang chơi, enemy spawn
```

### Pause() Method

Tại `MenuManager.cs:73-96`

```csharp
public void Pause()
{
    // Phát âm thanh pause
    SoundManager.PlaySfx(SoundManager.Instance.soundPause);

    // Nếu game đang chạy (timeScale != 0)
    if (Time.timeScale != 0)
    {
        // Lưu time scale hiện tại
        currentTimeScale = Time.timeScale;

        // Đóng băng game
        Time.timeScale = 0;

        // Ẩn UI game
        UI.SetActive(false);

        // Hiển thị menu pause
        PauseUI.SetActive(true);

        CharacterContainer.SetActive(false);
    }
    else  // Game đang pause, tiếp tục
    {
        // Khôi phục time scale
        Time.timeScale = currentTimeScale;

        // Hiển thị UI game
        UI.SetActive(true);

        // Ẩn menu pause
        PauseUI.SetActive(false);

        CharacterContainer.SetActive(true);
    }
}
```

**Giải Thích Time.timeScale**:

```csharp
Time.timeScale = 1;   // Tốc độ bình thường
Time.timeScale = 0.5; // Nửa tốc độ (slow motion)
Time.timeScale = 2;   // Gấp đôi tốc độ (fast forward)
Time.timeScale = 0;   // Đóng băng (pause)

Khi timeScale = 0:
- Update() vẫn chạy
- FixedUpdate() KHÔNG chạy
- Time.deltaTime = 0
- Animation dừng
- Physics dừng
```

**Luồng Pause/Resume**:
```
[PLAYING]
    │
    │ User nhấn Pause
    │
    ├─ Time.timeScale = 0
    ├─ UI ẩn
    └─ PauseUI hiển thị
    │
    ▼
[PAUSED]
    │
    │ User nhấn Resume
    │
    ├─ Time.timeScale = 1
    ├─ PauseUI ẩn
    └─ UI hiển thị
    │
    ▼
[PLAYING]
```

### ISuccess() Method (Victory)

Tại `MenuManager.cs:103-115`

```csharp
public void ISuccess()
{
    StartCoroutine(VictoryCo());
}

IEnumerator VictoryCo()
{
    // Ẩn UI game
    UI.SetActive(false);
    CharacterContainer.SetActive(false);

    // Đợi 1.5 giây
    yield return new WaitForSeconds(1.5f);

    // Hiển thị màn hình chiến thắng
    VictotyUI.SetActive(true);
}
```

**Khi Nào Method Này Được Gọi?**

```csharp
// Trong LevelEnemyManager.cs
if (allEnemiesDead && GameManager.Instance.State == GameManager.GameState.Playing)
{
    GameManager.Instance.Victory();
    // GameManager broadcast ISuccess() đến tất cả listener
    // MenuManager.ISuccess() được gọi
}
```

**Luồng Victory**:
```
Frame 500:  Enemy cuối cùng chết
            └─ LevelEnemyManager phát hiện tất cả chết

Frame 501:  GameManager.Victory() được gọi
            └─ Broadcast ISuccess() đến listener

Frame 502:  MenuManager.ISuccess() được gọi
            ├─ UI ẩn
            └─ Bắt đầu đợi 1.5 giây

Frame 592:  (1.5 giây @ 60fps)
            └─ VictotyUI hiển thị (animation sao bắt đầu)
```

### IGameOver() Method (Fail)

Tại `MenuManager.cs:128-141`

```csharp
public void IGameOver()
{
    StartCoroutine(GameOverCo());
}

IEnumerator GameOverCo()
{
    // Ẩn UI game
    UI.SetActive(false);
    CharacterContainer.SetActive(false);

    // Đợi 1.5 giây
    yield return new WaitForSeconds(1.5f);

    // Hiển thị màn hình thua
    FailUI.SetActive(true);
}
```

**Khi Nào Method Này Được Gọi?**

```csharp
// Trong TheFortrest.cs (máu pháo đài)
if (currentHealth <= 0)
{
    GameManager.Instance.GameOver();
    // GameManager broadcast IGameOver() đến tất cả listener
}
```

---

## Hệ Thống Health Bar

### Tổng Quan HealthBarEnemyNew

**File**: `HealthBarEnemyNew.cs`

**Mục đích**: Thanh máu cá nhân theo dõi enemy

**Tính năng**:
- Theo vị trí enemy
- Tự động ẩn sau khi nhận damage
- Fade out mượt mà
- Scale theo phần trăm máu

### Kiến Trúc

```
 HealthBarEnemyNew GameObject
    │
    ├─ backgroundImage (SpriteRenderer)
    │   └─ Thanh nền màu đỏ
    │
    └─ healthBar Transform
        └─ barImage (SpriteRenderer)
            └─ Thanh máu màu xanh (scale)
```

### Init() Method

Tại `HealthBarEnemyNew.cs:35-40`

```csharp
public void Init(Transform _target, Vector3 _offset)
{
    // Đặt target để theo
    target = _target;

    // Đặt offset từ vị trí target
    offset = _offset;
}
```

**Sử dụng**:
```csharp
// Trong Enemy.Start()
var healthBarObj = (HealthBarEnemyNew)Resources.Load("HealthBar", typeof(HealthBarEnemyNew));
healthBar = (HealthBarEnemyNew)Instantiate(healthBarObj, healthBarOffset, Quaternion.identity);

healthBar.Init(transform, (Vector3)healthBarOffset);
//             ^^^^^^^^^  ^^^^^^^^^^^^^^^^^^^^^^
//             Enemy này   Offset (0, 1.5)
```

### Update() Method (Follow Enemy)

Tại `HealthBarEnemyNew.cs:42-49`

```csharp
private void Update()
{
    // Nếu target tồn tại, theo nó
    if (target)
    {
        transform.position = target.position + offset;
    }
}
```

**Cách Hoạt Động**:
```
Frame 1:   Enemy ở (5, 0)
           healthBar.offset = (0, 1.5)
           healthBar.position = (5, 0) + (0, 1.5) = (5, 1.5)

Frame 2:   Enemy di chuyển đến (5.1, 0)
           healthBar.position = (5.1, 0) + (0, 1.5) = (5.1, 1.5)

Kết quả: Thanh máu luôn bay phía trên enemy
```

### UpdateValue() Method

Tại `HealthBarEnemyNew.cs:51-66`

```csharp
public void UpdateValue(float value)
{
    // Dừng tất cả animation đang chạy
    StopAllCoroutines();
    CancelInvoke();

    // Hiển thị thanh máu (làm cho visible)
    backgroundImage.color = oriBGImage;
    barImage.color = oriBarImage;

    // Giới hạn value giữa 0 và 1
    value = Mathf.Max(0, value);

    // Scale thanh máu (1 = full, 0 = empty)
    healthBar.localScale = new Vector2(value, healthBar.localScale.y);

    // Nếu máu > 0, ẩn sau showTime giây
    if (value > 0)
        Invoke("HideBar", showTime);  // Mặc định 1 giây
    else
        gameObject.SetActive(false);  // Chết, vô hiệu hóa hoàn toàn
}
```

**Ví Dụ Trực Quan**:

```
Máu: 100/100 (value = 1.0)
healthBar.localScale = (1.0, 1.0)
[████████████████████] 100%

Máu: 75/100 (value = 0.75)
healthBar.localScale = (0.75, 1.0)
[███████████████     ] 75%

Máu: 25/100 (value = 0.25)
healthBar.localScale = (0.25, 1.0)
[█████               ] 25%

Máu: 0/100 (value = 0.0)
gameObject.SetActive(false)
[                    ] (vô hiệu hóa)
```

### HideBar() Method (Fade Out)

Tại `HealthBarEnemyNew.cs:68-76`

```csharp
private void HideBar()
{
    // Kiểm tra nếu GameObject vẫn active
    if (gameObject.activeInHierarchy)
    {
        // Fade background sang trong suốt
        StartCoroutine(RGFade.FadeSpriteRenderer(
            backgroundImage,
            hideSpeed,  // Mặc định 0.5 giây
            new Color(oriBGImage.r, oriBGImage.g, oriBGImage.b, 0)
            //                                                  ^
            //                                                  Alpha = 0 (trong suốt)
        ));

        // Fade thanh máu sang trong suốt
        StartCoroutine(RGFade.FadeSpriteRenderer(
            barImage,
            hideSpeed,
            new Color(oriBarImage.r, oriBarImage.g, oriBarImage.b, 0)
        ));
    }
}
```

**Timeline Fade**:
```
Frame 100:  Enemy nhận damage
            ├─ UpdateValue(0.75) được gọi
            ├─ healthBar.localScale = (0.75, 1)
            ├─ backgroundImage.color = (R, G, B, 1) [hiển thị]
            └─ Invoke("HideBar", 1.0f)

Frame 101-159: Thanh máu hiển thị ở 75%

Frame 160:  HideBar() được gọi (1 giây sau)
            ├─ Bắt đầu fade coroutine
            └─ Fade từ alpha 1 → 0 trong 0.5 giây

Frame 161-190: Đang fade out (30 frame @ 60fps)
            └─ alpha: 1.0 → 0.9 → 0.8 → ... → 0.1 → 0.0

Frame 191:  Thanh máu hoàn toàn trong suốt (invisible)
```

---

## Hệ Thống Scene Loading

### LoadAsynchronously() Method

Tại `MenuManager.cs:205-220`

```csharp
IEnumerator LoadAsynchronously(string name)
{
    // Hiển thị UI loading
    LoadingUI.SetActive(true);

    // Bắt đầu load scene async
    AsyncOperation operation = SceneManager.LoadSceneAsync(name);

    // Đợi đến khi load xong
    while (!operation.isDone)
    {
        // Tính tiến trình (0.0 đến 1.0)
        float progress = Mathf.Clamp01(operation.progress / 0.9f);

        // Cập nhật slider
        slider.value = progress;

        // Cập nhật text phần trăm
        progressText.text = (int)progress * 100f + "%";

        yield return null;  // Đợi một frame
    }
}
```

**Tại Sao Chia Cho 0.9?**

`operation.progress` của Unity đi từ 0.0 đến 0.9, sau đó nhảy đến 1.0 khi hoàn thành.

```
Không chia:
operation.progress = 0.9
slider hiển thị 90%, nhưng scene vẫn đang load

Có chia:
operation.progress = 0.9
progress = 0.9 / 0.9 = 1.0
slider hiển thị 100%, cảm giác hoàn thành
```

**Luồng Loading**:
```
Frame 1:    LoadAsynchronously("Menu") được gọi
            ├─ LoadingUI hiển thị
            └─ Bắt đầu async load

Frame 2:    operation.progress = 0.0
            ├─ slider.value = 0.0
            └─ progressText = "0%"

Frame 30:   operation.progress = 0.3
            ├─ slider.value = 0.33
            └─ progressText = "33%"

Frame 60:   operation.progress = 0.6
            ├─ slider.value = 0.66
            └─ progressText = "66%"

Frame 90:   operation.progress = 0.9
            ├─ slider.value = 1.0
            └─ progressText = "100%"

Frame 91:   operation.isDone = true
            └─ Coroutine kết thúc, scene load
```

### Các Method Chuyển Scene

**LoadHomeMenuScene()** (`MenuManager.cs:180-185`)

```csharp
public void LoadHomeMenuScene()
{
    SoundManager.Click();  // Phát âm thanh click
    StartCoroutine(LoadAsynchronously("Menu"));
}
```

**RestarLevel()** (`MenuManager.cs:187-192`)

```csharp
public void RestarLevel()
{
    SoundManager.Click();
    // Load lại scene hiện tại
    StartCoroutine(LoadAsynchronously(SceneManager.GetActiveScene().name));
}
```

**LoadNextLevel()** (`MenuManager.cs:194-200`)

```csharp
public void LoadNextLevel()
{
    SoundManager.Click();

    // Tăng số level
    GlobalValue.levelPlaying++;

    // Reload scene hiện tại (GameManager sẽ load level mới)
    StartCoroutine(LoadAsynchronously(SceneManager.GetActiveScene().name));
}
```

**Cách LoadNextLevel() Hoạt Động**:
```
Trạng thái hiện tại:
- GlobalValue.levelPlaying = 1
- Scene: "Playing"

User click "Next Level":
1. GlobalValue.levelPlaying = 2
2. Reload scene "Playing"
3. GameManager.Awake() chạy
4. GameManager instantiate gameLevels[1] (level 2)
```

---

## Global Values

### Tổng Quan GlobalValue

**GlobalValue** là một **static class** lưu trữ cài đặt và dữ liệu lưu game toàn cục.

**Tại Sao Static?**
- Duy trì qua các scene
- Không cần GameObject
- Truy cập từ mọi nơi

### Các Property GlobalValue Phổ Biến

```csharp
// Tiến trình player
public static int levelPlaying;        // Level hiện tại (1, 2, 3...)
public static int SavedCoins;          // Tổng coin kiếm được
public static int finishGameAtLevel;   // Level tối đa trong game

// Cài đặt âm thanh
public static bool isSound = true;     // Bật/tắt hiệu ứng âm thanh
public static bool isMusic = true;     // Bật/tắt nhạc nền

// Flag lần đầu
public static bool isFirstOpenMainMenu = true;  // Phát intro?
```

### Cách GlobalValue Được Sử Dụng

**Lưu Tiến Trình**:
```csharp
// Trong một script nào đó
GlobalValue.SavedCoins += 10;  // Kiếm được 10 coin

// Trong UI_UI.Update()
coinTxt.text = GlobalValue.SavedCoins + "";  // Hiển thị: "10"
```

**Quản Lý Level**:
```csharp
// Trong GameManager.Awake()
Instantiate(gameLevels[GlobalValue.levelPlaying - 1], Vector2.zero, Quaternion.identity);
//                     ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
//                     0 = level 1, 1 = level 2, etc.
```

**Cài Đặt Âm Thanh**:
```csharp
// Trong MainMenuHomeScene.Start()
if (!GlobalValue.isSound)
    SoundManager.SoundVolume = 0;
if (!GlobalValue.isMusic)
    SoundManager.MusicVolume = 0;
```

### Tích Hợp PlayerPrefs

GlobalValue có thể lưu/load từ PlayerPrefs:

```csharp
// Method save giả định
public static void Save()
{
    PlayerPrefs.SetInt("Coins", SavedCoins);
    PlayerPrefs.SetInt("Level", levelPlaying);
    PlayerPrefs.SetInt("Sound", isSound ? 1 : 0);
    PlayerPrefs.SetInt("Music", isMusic ? 1 : 0);
    PlayerPrefs.Save();
}

// Method load giả định
public static void Load()
{
    SavedCoins = PlayerPrefs.GetInt("Coins", 0);
    levelPlaying = PlayerPrefs.GetInt("Level", 1);
    isSound = PlayerPrefs.GetInt("Sound", 1) == 1;
    isMusic = PlayerPrefs.GetInt("Music", 1) == 1;
}
```

---

## Cách Tạo Custom UI

### Từng Bước: Thêm Panel UI Mới

#### Bước 1: Tạo UI Trong Scene

1. Right-click Hierarchy → `UI → Panel`
2. Đặt tên: `CustomPanel`
3. Thêm child element:
   - `UI → Button` (Button đóng)
   - `UI → Text` (Tiêu đề)
   - `UI → Image` (Background)

```
Canvas
└─ CustomPanel
    ├─ Background (Image)
    ├─ Title (Text)
    └─ CloseButton (Button)
        └─ Text ("X")
```

#### Bước 2: Tạo Script

Tạo `CustomPanel.cs`:

```csharp
using UnityEngine;
using UnityEngine.UI;

namespace RGame
{
    public class CustomPanel : MonoBehaviour
    {
        public Text titleText;

        void Start()
        {
            // Ẩn khi start
            gameObject.SetActive(false);
        }

        public void Show(string title)
        {
            titleText.text = title;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void OnCloseButtonClick()
        {
            SoundManager.Click();
            Hide();
        }
    }
}
```

#### Bước 3: Kết Nối Button

1. Chọn `CloseButton` trong Hierarchy
2. Trong Inspector, tìm component `Button`
3. Click `+` dưới `OnClick()`
4. Kéo GameObject `CustomPanel` vào object field
5. Chọn function: `CustomPanel → OnCloseButtonClick()`

#### Bước 4: Truy Cập Từ Script Khác

```csharp
// Trong MenuManager.cs, thêm:
public CustomPanel customPanel;

// Để hiển thị panel:
public void ShowCustomPanel()
{
    customPanel.Show("Hello!");
}
```

### Ví Dụ: Thêm Leaderboard Panel

**LeaderboardPanel.cs**:

```csharp
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace RGame
{
    public class LeaderboardPanel : MonoBehaviour
    {
        public Transform entryContainer;  // Nơi entry spawn
        public GameObject entryPrefab;     // Template entry

        void Start()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            // Xóa entry cũ
            foreach (Transform child in entryContainer)
                Destroy(child.gameObject);

            // Load điểm số
            List<int> scores = LoadScores();

            // Tạo entry cho mỗi điểm số
            for (int i = 0; i < scores.Count; i++)
            {
                GameObject entry = Instantiate(entryPrefab, entryContainer);
                entry.GetComponent<Text>().text = $"{i + 1}. {scores[i]} points";
            }

            gameObject.SetActive(true);
        }

        List<int> LoadScores()
        {
            // Trong game thật, load từ PlayerPrefs hoặc server
            return new List<int> { 1000, 850, 720, 650, 500 };
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
```

---

## Các Vấn Đề Thường Gặp & Giải Pháp

### Vấn Đề 1: UI Không Hiển Thị

**Triệu chứng**:
- GameObject UI active, nhưng không có gì hiển thị trên màn hình

**Nguyên Nhân & Cách Sửa**:

1. **Canvas Render Mode Sai**
   ```csharp
   // Kiểm tra component Canvas
   Canvas Render Mode: Screen Space - Overlay  ✓
   Canvas Render Mode: World Space            ✗ (không hiển thị)
   ```

2. **UI Ở Sau Camera**
   - Kiểm tra sorting order
   - Canvas phải ở trên cùng
   - Cách sửa: Tăng Canvas sorting order lên 100

3. **Alpha = 0**
   ```csharp
   // Kiểm tra component Image/Text
   Color: (R, G, B, 0)  ✗ (trong suốt)
   Color: (R, G, B, 255) ✓ (hiển thị)
   ```

4. **Raycast Target Bị Tắt**
   - Kiểm tra component Image
   - "Raycast Target" phải được check cho element có thể click

### Vấn Đề 2: Button Không Click Được

**Triệu chứng**:
- Button hiển thị nhưng OnClick() không kích hoạt

**Nguyên Nhân & Cách Sửa**:

1. **Không Có EventSystem**
   ```
   Hierarchy phải có:
   - GameObject EventSystem
   ```
   Cách sửa: `GameObject → UI → Event System`

2. **Button Bị Che Bởi UI Khác**
   - Kiểm tra thứ tự sibling trong Hierarchy
   - Sibling sau render ở trên
   - Cách sửa: Sắp xếp lại trong Hierarchy

3. **OnClick Chưa Được Cấu Hình**
   ```
   Component Button → OnClick():
   - Phải có ít nhất một entry
   - Phải reference đúng GameObject
   - Phải chọn đúng function
   ```

4. **Interactable = False**
   ```csharp
   // Kiểm tra component Button
   Interactable: ✓  (bật)
   Interactable: ✗  (tắt, màu xám)
   ```

### Vấn Đề 3: Slider Không Cập Nhật

**Triệu chứng**:
- Gọi slider.value = X, nhưng slider không di chuyển

**Nguyên Nhân & Cách Sửa**:

1. **Range Value Sai**
   ```csharp
   // Kiểm tra component Slider
   Min Value: 0
   Max Value: 1  ✓ (cho 0-100%)

   // Nếu Max = 100:
   slider.value = 0.5;  // Hiển thị 0.5% (sai)
   slider.value = 50;   // Hiển thị 50% (đúng)
   ```

2. **Lerp Không Bao Giờ Đạt Target**
   ```csharp
   // Trong Update():
   slider.value = Mathf.Lerp(slider.value, target, 0.1f);
   //                                               ^^^^
   //                                               Quá chậm nếu không * Time.deltaTime

   // Cách sửa:
   slider.value = Mathf.Lerp(slider.value, target, 5 * Time.deltaTime);
   ```

3. **Fill Rect Chưa Được Gán**
   - Kiểm tra component Slider
   - "Fill Rect" phải reference đến fill image
   - Cách sửa: Kéo Fill Image vào field Fill Rect

### Vấn Đề 4: Text Không Cập Nhật

**Triệu chứng**:
- Thay đổi giá trị text.text, nhưng hiển thị không đổi

**Nguyên Nhân & Cách Sửa**:

1. **Reference Text Sai**
   ```csharp
   public Text coinText;  // Được gán trong Inspector

   // Kiểm tra:
   Debug.Log(coinText);  // Không nên null

   // Cách sửa: Kéo component Text vào field trong Inspector
   ```

2. **Font Size Quá Nhỏ**
   - Text có thể tồn tại nhưng không nhìn thấy
   - Cách sửa: Tăng font size lên 24+

3. **RectTransform Quá Nhỏ**
   - Text bị cắt bởi kích thước container
   - Cách sửa: Tăng width/height của RectTransform

4. **TextMesh vs Text Component**
   ```csharp
   // Unity có hai hệ thống text:
   using UnityEngine.UI;
   public Text uiText;  // Cho Canvas UI ✓

   using TMPro;
   public TextMeshProUGUI tmpText;  // TextMeshPro
   ```

### Vấn Đề 5: Panel Không Ẩn

**Triệu chứng**:
- Gọi SetActive(false), nhưng panel vẫn hiển thị

**Nguyên Nhân & Cách Sửa**:

1. **Nhiều Instance**
   ```csharp
   // Kiểm tra Hierarchy
   - PauseUI (inactive)
   - PauseUI (1) (active) ← Trùng!

   // Cách sửa: Xóa bản trùng
   ```

2. **Reference Sai**
   ```csharp
   public GameObject pauseUI;

   // Trong Start():
   Debug.Log(pauseUI.name);  // Xác minh đúng panel

   // Cách sửa: Gán lại trong Inspector
   ```

3. **Child Override Parent**
   ```csharp
   // Parent inactive, nhưng child có script đặt nó active
   // Cách sửa: Xóa script hoặc kiểm tra trạng thái active
   ```

### Vấn Đề 6: Scene Không Load

**Triệu chứng**:
- Gọi LoadScene(), nhưng không có gì xảy ra

**Nguyên Nhân & Cách Sửa**:

1. **Scene Không Có Trong Build Settings**
   ```
   File → Build Settings → Scenes In Build
   - Phải bao gồm tất cả scene
   - Kiểm tra thứ tự (0, 1, 2...)

   Cách sửa: Kéo scene vào list
   ```

2. **Tên Scene Sai**
   ```csharp
   SceneManager.LoadScene("menu");  ✗ (phân biệt hoa thường)
   SceneManager.LoadScene("Menu");  ✓

   // Cách sửa: Dùng tên scene chính xác
   ```

3. **SceneManager Chưa Import**
   ```csharp
   using UnityEngine.SceneManagement;  // Bắt buộc

   SceneManager.LoadScene("Menu");
   ```

### Vấn Đề 7: Time.timeScale Không Reset

**Triệu chứng**:
- Sau pause, game vẫn đóng băng ngay cả sau resume

**Nguyên Nhân & Cách Sửa**:

1. **OnDisable() Chưa Được Implement**
   ```csharp
   // Trong MenuManager.cs
   private void OnDisable()
   {
       // Luôn reset time scale
       Time.timeScale = 1;
   }
   ```

2. **Nhiều Script Đặt timeScale**
   ```csharp
   // Script A:
   Time.timeScale = 0;

   // Script B:
   Time.timeScale = 0;

   // Resume chỉ đặt một cái về 1
   // Cách sửa: Dùng single manager cho timeScale
   ```

### Vấn Đề 8: UI Bị Kéo Giãn Sai

**Triệu chứng**:
- UI trông đúng trong editor, bị kéo giãn trong game

**Nguyên Nhân & Cách Sửa**:

1. **Canvas Scaler Chưa Được Cấu Hình**
   ```
   Component Canvas Scaler:
   - UI Scale Mode: Scale With Screen Size
   - Reference Resolution: 1920 x 1080
   - Match: 0.5 (cân bằng width/height)
   ```

2. **Anchor Point Sai**
   ```
   Cho element đặt ở giữa:
   - Anchor: Center
   - Position: (0, 0)

   Cho element đặt ở góc:
   - Anchor: Top Left (cho UI góc trên trái)
   - Position: (10, -10)
   ```

---

## Tóm Tắt

**UI System** cung cấp phản hồi trực quan và điều khiển người dùng thông qua:

1. **Main Menu** (MainMenuHomeScene): Màn hình tiêu đề, chọn level, cài đặt
2. **In-Game HUD** (UI_UI): Thanh máu, bộ đếm coin/mana, tiến trình wave
3. **Menu Manager** (MenuManager): Màn hình pause, victory, fail
4. **Health Bars** (HealthBarEnemyNew): Hiển thị máu enemy cá nhân
5. **Loading System**: Chuyển scene async với thanh tiến trình
6. **Global Values**: Cài đặt và dữ liệu lưu duy trì

**Khái Niệm Chính**:
- **Singleton Pattern**: MenuManager.Instance cho truy cập toàn cục
- **Observer Pattern**: IListener cho phản hồi sự kiện game
- **Lerp cho Smoothness**: Animation thanh máu mượt mà
- **Time.timeScale**: Logic pause/resume game
- **AsyncOperation**: Load scene không blocking

**Best Practice**:
- Ẩn panel mặc định (SetActive(false) trong Awake)
- Dùng Lerp cho chuyển tiếp UI mượt mà
- Luôn reset Time.timeScale trong OnDisable()
- Dùng GlobalValue cho dữ liệu duy trì
- Implement IListener cho cập nhật UI event-driven

**Bước Tiếp Theo**:
- Đọc `05_Cac_Manager.md` để hiểu GameManager, LevelManager
- Đọc `10_Huong_Dan_Thuc_Hanh.md` cho ví dụ UI thực tế
- Đọc `02_He_Thong_Player.md` để xem player cập nhật thanh máu như thế nào

---

**Cập Nhật Lần Cuối**: 2025
**File**: `Documents/04_He_Thong_UI.md`
