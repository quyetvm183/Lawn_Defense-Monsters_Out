# Hệ Thống Map (Chọn Level & Scene Loading)

**Mục Đích**: Hướng dẫn kỹ thuật đầy đủ về UI chọn level, hệ thống điều hướng map, và cách các level/scene được load và quản lý trong "Lawn Defense: Monsters Out".

**Nội Dung Tài Liệu**:
- Map UI navigation và pagination system
- Level selection và world switching
- Scene loading với async operations
- Level configuration với GameLevelSetup
- Tạo và cấu hình các level mới
- Troubleshooting các vấn đề map/level phổ biến

**Tài Liệu Liên Quan**:
- Xem `05_Cac_Manager_Day_Du.md` cho GameManager và scene management
- Xem `04_He_Thong_UI_Day_Du.md` cho UI architecture
- Xem `He_Thong_Enemy_Nang_Cao.md` cho enemy wave configuration
- Xem `10_Huong_Dan_Thuc_Hanh.md` cho practical tutorials

---

## Mục Lục

1. [Unity Basics Cần Biết](#unity-basics-cần-biết)
2. [System Overview](#system-overview)
3. [MapControllerUI - Navigation System](#mapcontrollerui---navigation-system)
4. [GameLevelSetup - Level Configuration](#gamelevelsetup---level-configuration)
5. [MenuManager - Scene Loading](#menumanager---scene-loading)
6. [Complete Level Loading Flow](#complete-level-loading-flow)
7. [Tạo Level Mới](#tạo-level-mới)
8. [Kỹ Thuật Nâng Cao](#kỹ-thuật-nâng-cao)
9. [Troubleshooting](#troubleshooting)

---

## Unity Basics Cần Biết

Trước khi hiểu map system, bạn cần biết các khái niệm Unity sau:

### 1. **Scene Management**
```
Scene là một Unity file chứa các game objects và configurations.
Nghĩ về nó như một "level" hoặc "room" trong game của bạn.

Ví dụ các scenes:
- Menu scene (main menu)
- Game scene (gameplay)
- Map scene (level selection)
```

### 2. **SceneManager.LoadSceneAsync**
```csharp
// Asynchronous = loads ở background trong khi hiển thị progress
AsyncOperation operation = SceneManager.LoadSceneAsync("Menu");

// Synchronous = đóng băng game cho đến khi loaded (TỆ cho large scenes!)
SceneManager.LoadScene("Menu"); // Đừng dùng cái này cho big scenes
```

### 3. **Prefab Instantiation**
```csharp
// Prefabs là templates được lưu trong Project window
// Instantiate = tạo một bản sao trong scene
GameObject level = Instantiate(levelPrefab, Vector2.zero, Quaternion.identity);
```

### 4. **RectTransform**
```
RectTransform được dùng cho UI positioning (không phải regular Transform).
- anchoredPosition: Vị trí tương đối với anchor point
- sizeDelta: Width và height
```

### 5. **Coroutines**
```csharp
// Coroutines chạy qua nhiều frames, cho phép delays
IEnumerator MyCoroutine()
{
    yield return new WaitForSeconds(1f); // Đợi 1 giây
    // Code ở đây chạy sau 1 giây
}

// Bắt đầu nó với:
StartCoroutine(MyCoroutine());
```

---

## System Overview

Map system có **ba main components** làm việc cùng nhau:

```
┌────────────────────────────────────────────────────────────────┐
│                     MAP SYSTEM ARCHITECTURE                     │
└────────────────────────────────────────────────────────────────┘

1. MapControllerUI
   ├─ Xử lý UI navigation (Previous/Next buttons)
   ├─ Hiển thị world/block indicators (dots)
   └─ Animate map scrolling

2. GameLevelSetup
   ├─ Lưu trữ level configurations (waves, mana, fortress HP)
   ├─ Cung cấp level data cho LevelEnemyManager
   └─ Tự động fill mana theo thời gian

3. MenuManager
   ├─ Loads scenes asynchronously
   ├─ Hiển thị loading screen với progress bar
   └─ Xử lý scene transitions (restart, next level, main menu)

┌─────────────────────────────────────────────────────────────────┐
│                        COMPLETE FLOW                            │
└─────────────────────────────────────────────────────────────────┘

Player Mở Map Screen
        │
        ├─► MapControllerUI.Start()
        │   └─ Đặt dots để hiển thị current world position
        │
Player Click "Next World" Button
        │
        ├─► MapControllerUI.Next()
        │   ├─ Phát click sound
        │   ├─ Hiển thị black screen effect
        │   ├─ Scroll map UI đến next block
        │   └─ Cập nhật dot indicators
        │
Player Chọn Level
        │
        ├─► MenuManager.LoadAsynchronously("Game")
        │   ├─ Hiển thị loading UI
        │   ├─ Load scene ở background
        │   └─ Cập nhật progress bar (0% → 100%)
        │
Scene Loads → GameManager.Awake()
        │
        ├─► Instantiates level prefab từ gameLevels array
        │   └─ Dùng GlobalValue.levelPlaying để pick level
        │
Level Prefab Chứa GameLevelSetup Component
        │
        └─► GameLevelSetup.Awake()
            ├─ Đặt chính nó làm singleton Instance
            ├─ Đặt GlobalValue.finishGameAtLevel
            └─ Cung cấp wave data cho LevelEnemyManager
```

---

## MapControllerUI - Navigation System

**Location**: `Assets/_MonstersOut/Scripts/UI/MapControllerUI.cs`

**Mục Đích**: Điều khiển map/world selection UI với horizontal scrolling navigation.

### Khái Niệm Chính

Nghĩ về map UI như một **horizontal carousel** của world blocks:

```
┌──────────────────────────────────────────────────────────────┐
│                        MAP CAROUSEL                          │
└──────────────────────────────────────────────────────────────┘

     ┌───────┐   ┌───────┐   ┌───────┐
     │ World │   │ World │   │ World │
     │   1   │   │   2   │   │   3   │  ← Blocks (children)
     └───────┘   └───────┘   └───────┘
         ▲
         │
    Vị Trí Hiện Tại

◉ ○ ○  ← Dots (indicators hiển thị world nào đang visible)

[◀ Previous]  [Next ▶]  ← Navigation buttons
```

### Class Structure

```csharp
namespace RGame
{
    public class MapControllerUI : MonoBehaviour
    {
        // CONFIGURATION
        public RectTransform BlockLevel;    // Container chứa tất cả world blocks
        public int howManyBlocks = 3;       // Tổng số worlds
        public float step = 720f;           // Khoảng cách scroll mỗi world (pixels)
        public Image[] Dots;                // Dot indicators cho mỗi world
        public AudioClip music;             // Music phát trên map screen

        // STATE
        private float newPosX = 0;          // Vị trí scroll hiện tại
        int currentPos = 0;                 // World index hiện tại (0, 1, 2...)
        bool allowPressButton = true;       // Ngăn double-clicking
    }
}
```

### Cách Hoạt Động: Initialization

**Bước 1: Start() - Thiết Lập Dots**

```csharp
void Start()
{
    SetDots();  // Initialize dot indicators
}

void SetDots()
{
    // Bước 1: Làm tất cả dots semi-transparent và nhỏ
    foreach (var obj in Dots)
    {
        obj.color = new Color(1, 1, 1, 0.5f);  // 50% opacity white
        obj.rectTransform.sizeDelta = new Vector2(28, 28);  // 28x28 pixels
    }

    // Bước 2: Highlight dot của current world
    Dots[currentPos].color = Color.yellow;  // Full yellow
    Dots[currentPos].rectTransform.sizeDelta = new Vector2(38, 38);  // Bigger (38x38)
}
```

**Điều Này Làm Gì**:
```
Trước SetDots():          Sau SetDots():
○ ○ ○                      ◉ ○ ○
(tất cả giống nhau)       (cái đầu tiên được highlight)
```

**Bước 2: OnEnable() - Phát Map Music**

```csharp
void OnEnable()
{
    SoundManager.PlayMusic(music);  // Phát map screen music
}

void OnDisable()
{
    SoundManager.PlayMusic(SoundManager.Instance.musicsGame);  // Khôi phục game music
}
```

**Tại Sao Điều Này Quan Trọng**: Khi player mở map, họ nghe music khác. Khi họ đóng nó, game music tiếp tục.

---

### Cách Hoạt Động: Navigation

**Click "Next" Button**

```csharp
public void Next()
{
    if (allowPressButton)  // Ngăn double-click
    {
        StartCoroutine(NextCo());
    }
}

IEnumerator NextCo()
{
    // BƯỚC 1: Ngăn double-clicking
    allowPressButton = false;
    SoundManager.Click();  // Phát click sound

    // BƯỚC 2: Kiểm tra nếu đã ở last world
    if (newPosX != (-step * (howManyBlocks - 1)))
    {
        // Có thể di chuyển forward
        currentPos++;  // Di chuyển đến next world (0 → 1, 1 → 2, etc.)

        newPosX -= step;  // Di chuyển left một step (ví dụ, -720 pixels)
        newPosX = Mathf.Clamp(newPosX, -step * (howManyBlocks - 1), 0);
        // Clamp đảm bảo không bao giờ scroll qua last world
    }
    else
    {
        // Đã ở last world, không thể đi xa hơn
        allowPressButton = true;
        yield break;  // Thoát coroutine
    }

    // BƯỚC 3: Animate transition
    BlackScreenUI.instance.Show(0.15f);  // Fade to black
    yield return new WaitForSeconds(0.15f);  // Đợi fade

    // BƯỚC 4: Cập nhật map position (instant snap khi screen đen)
    SetMapPosition();  // Di chuyển BlockLevel container

    // BƯỚC 5: Fade back in
    BlackScreenUI.instance.Hide(0.15f);  // Fade from black

    // BƯỚC 6: Cập nhật dot indicators
    SetDots();  // Highlight new current world

    // BƯỚC 7: Bật lại button
    allowPressButton = true;
}

void SetMapPosition()
{
    // Di chuyển toàn bộ BlockLevel container horizontally
    BlockLevel.anchoredPosition = new Vector2(newPosX, BlockLevel.anchoredPosition.y);
}
```

**Ví Dụ Trực Quan Về Navigation**:
```
Trạng Thái Ban Đầu (World 1):
┌─────────┐
│ [World1]│   World2   World3
└─────────┘
Position: newPosX = 0
Dots: ◉ ○ ○

Sau Next() (World 2):
   World1   [World2]   World3
            └─────────┐
Position: newPosX = -720
Dots: ○ ◉ ○

Sau Next() Lần Nữa (World 3):
   World1   World2   [World3]
                    └─────────┐
Position: newPosX = -1440
Dots: ○ ○ ◉
```

**Phân Tích Toán Học**:
```
howManyBlocks = 3
step = 720

Max scroll = -step * (howManyBlocks - 1)
           = -720 * (3 - 1)
           = -720 * 2
           = -1440 pixels

World 1: newPosX = 0
World 2: newPosX = -720
World 3: newPosX = -1440
```

**Click "Previous" Button**

```csharp
public void Pre()
{
    if (allowPressButton)
    {
        StartCoroutine(PreCo());
    }
}

IEnumerator PreCo()
{
    allowPressButton = false;
    SoundManager.Click();

    // Kiểm tra nếu đã ở first world
    if (newPosX != 0)
    {
        // Có thể di chuyển backward
        currentPos--;  // 2 → 1, 1 → 0

        newPosX += step;  // Di chuyển right một step (+720 pixels)
        newPosX = Mathf.Clamp(newPosX, -step * (howManyBlocks - 1), 0);
    }
    else
    {
        // Đã ở first world
        allowPressButton = true;
        yield break;
    }

    // Animation giống Next()
    BlackScreenUI.instance.Show(0.15f);
    yield return new WaitForSeconds(0.15f);
    SetMapPosition();
    BlackScreenUI.instance.Hide(0.15f);
    SetDots();
    allowPressButton = true;
}
```

---

### Utility: Unlock All Levels (Developer Tool)

```csharp
public void UnlockAllLevels()
{
    // Thêm 1000 vào levels passed (unlock mọi thứ)
    GlobalValue.LevelPass = (GlobalValue.LevelPass + 1000);

    // Reload current scene để refresh UI
    UnityEngine.SceneManagement.SceneManager.LoadScene(
        UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
    );

    SoundManager.Click();
}
```

**Cách Dùng**: Gắn vào một hidden debug button để testing.

---

## GameLevelSetup - Level Configuration

**Location**: `Assets/_MonstersOut/Scripts/Managers/GameLevelSetup.cs`

**Mục Đích**: Lưu trữ và cung cấp level-specific configurations (enemy waves, mana, fortress health).

### Khái Niệm Chính

Mỗi **level prefab** có một `GameLevelSetup` component lưu trữ:
- **Enemy waves** cho level đó
- **Starting mana**
- **Enemy fortress health**
- **Auto-mana refill rate**

```
Level Prefab Hierarchy:
┌──────────────────────────────┐
│ Level_1 (Prefab)             │
├──────────────────────────────┤
│ ├─ GameLevelSetup (Component)│
│ │  ├─ levelWaves: List       │
│ │  │  ├─ Level 1 (LevelWave) │
│ │  │  ├─ Level 2 (LevelWave) │
│ │  │  └─ Level 3 (LevelWave) │
│ │  ├─ amountMana: 2          │
│ │  └─ rate: 2.0              │
│ ├─ LevelEnemyManager         │
│ └─ Các level objects khác... │
└──────────────────────────────┘
```

### Class Structure

```csharp
namespace RGame
{
    public class GameLevelSetup : MonoBehaviour
    {
        public static GameLevelSetup Instance;  // Singleton

        [Header("===AUTO FILL MANA===")]
        public int amountMana = 2;      // Mana được thêm mỗi interval
        public float rate = 2;          // Interval tính bằng giây (thêm 2 mana mỗi 2 giây)

        [ReadOnly]
        public List<LevelWave> levelWaves = new List<LevelWave>();  // Tất cả level configs

        void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Persist qua scene loads

            // Đặt final level number
            GlobalValue.finishGameAtLevel = levelWaves.Count;
        }
    }
}
```

**LevelWave Structure**:

```csharp
public class LevelWave : MonoBehaviour
{
    public int level = 1;              // Level number (1, 2, 3...)
    public int givenMana = 1000;       // Starting mana cho level này

    [Range(1, 5)]
    public int enemyFortrestLevel = 1; // Enemy fortress health tier

    public EnemyWave[] Waves;          // Enemy spawn waves
}
```

**EnemyWave Structure** (từ LevelEnemyManager):

```csharp
[System.Serializable]
public class EnemyWave
{
    public EnemySpawn[] enemyInWaves;  // Enemies trong wave này
}

[System.Serializable]
public class EnemySpawn
{
    public GameObject enemyPrefab;     // Enemy nào sẽ spawn
    public int amount;                 // Bao nhiêu
    public float delay;                // Delay giữa các spawns
}
```

---

### Cách Hoạt Động: Cung Cấp Level Data

**Method 1: GetLevelWave() - Lấy Enemy Waves**

```csharp
public EnemyWave[] GetLevelWave()
{
    // Loop qua tất cả level configurations
    foreach (var obj in levelWaves)
    {
        // Tìm level khớp với current level đang chơi
        if (obj.level == GlobalValue.levelPlaying)
            return obj.Waves;  // Trả về enemy waves cho level này
    }

    return null;  // Level không tìm thấy
}
```

**Ví Dụ Sử Dụng**:
```csharp
// Trong LevelEnemyManager.cs:
EnemyWave[] waves = GameLevelSetup.Instance.GetLevelWave();

// waves[0] = First wave (ví dụ, 5 goblins)
// waves[1] = Second wave (ví dụ, 3 skeletons)
// waves[2] = Third wave (ví dụ, 1 boss)
```

**Method 2: GetEnemyFortrestLevel() - Lấy Enemy Fortress HP**

```csharp
public int GetEnemyFortrestLevel()
{
    foreach (var obj in levelWaves)
    {
        if (obj.level == GlobalValue.levelPlaying)
            return obj.enemyFortrestLevel;  // Trả về 1-5
    }

    return 1;  // Default là level 1
}
```

**Điều Này Có Nghĩa Là**:
```
enemyFortrestLevel = 1 → Enemy fortress có 500 HP
enemyFortrestLevel = 2 → Enemy fortress có 1000 HP
enemyFortrestLevel = 3 → Enemy fortress có 1500 HP
... etc.
```

**Method 3: GetGivenMana() - Lấy Starting Mana**

```csharp
public int GetGivenMana()
{
    foreach (var obj in levelWaves)
    {
        if (obj.level == GlobalValue.levelPlaying)
            return obj.givenMana;  // Starting mana cho level
    }

    return -1;  // Error: level không tìm thấy
}
```

**Method 4: isFinalLevel() - Kiểm Tra Nếu Đây Là Last Level**

```csharp
public bool isFinalLevel()
{
    return GlobalValue.levelPlaying == levelWaves.Count;
}
```

**Cách Dùng**:
```csharp
if (GameLevelSetup.Instance.isFinalLevel())
{
    // Hiển thị special victory screen
    // Unlock special reward
}
```

---

### Editor Tool: OnDrawGizmos()

Method này **tự động tạo level configs** trong Unity Editor:

```csharp
private void OnDrawGizmos()
{
    // Kiểm tra nếu số lượng child LevelWave objects đã thay đổi
    if (levelWaves.Count != transform.childCount)
    {
        // Re-scan tất cả child LevelWave components
        var waves = transform.GetComponentsInChildren<LevelWave>();
        levelWaves = new List<LevelWave>(waves);

        // Tự động đánh số chúng
        for (int i = 0; i < levelWaves.Count; i++)
        {
            levelWaves[i].level = i + 1;  // Level 1, 2, 3...
            levelWaves[i].gameObject.name = "Level " + levelWaves[i].level;
        }
    }
}
```

**Điều Này Làm Gì**:
```
Hierarchy Trước:                Hierarchy Sau OnDrawGizmos():
├─ GameLevelSetup               ├─ GameLevelSetup
│  ├─ LevelWave                 │  ├─ Level 1  (level = 1)
│  ├─ LevelWave                 │  ├─ Level 2  (level = 2)
│  └─ LevelWave                 │  └─ Level 3  (level = 3)
```

**Tại Sao Điều Này Hữu Ích**: Bạn không cần manually set level numbers—Unity làm tự động!

---

## MenuManager - Scene Loading

**Location**: `Assets/_MonstersOut/Scripts/UI/MenuManager.cs`

**Mục Đích**: Xử lý asynchronous scene loading với progress bar UI.

### Khái Niệm Chính

**Synchronous vs Asynchronous Loading**:

```
Synchronous (TỆ):
SceneManager.LoadScene("Game");
└─ Game đóng băng cho đến khi scene loads (có thể mất 3-5 giây!)

Asynchronous (TỐT):
SceneManager.LoadSceneAsync("Game");
├─ Game tiếp tục chạy
├─ Hiển thị loading screen
└─ Cập nhật progress bar (0% → 100%)
```

### LoadAsynchronously Method

```csharp
[Header("Load scene")]
public Slider slider;         // Progress bar UI
public Text progressText;     // "Loading... 45%" text

IEnumerator LoadAsynchronously(string name)
{
    // BƯỚC 1: Hiển thị loading screen
    LoadingUI.SetActive(true);

    // BƯỚC 2: Bắt đầu async scene load operation
    AsyncOperation operation = SceneManager.LoadSceneAsync(name);

    // BƯỚC 3: Cập nhật progress bar khi đang loading
    while (!operation.isDone)  // Loop cho đến khi scene hoàn tất loading
    {
        // operation.progress đi từ 0 đến 0.9, vì vậy chúng ta normalize về 0-1
        float progress = Mathf.Clamp01(operation.progress / 0.9f);

        // Cập nhật UI
        slider.value = progress;  // Đặt progress bar fill (0.0 đến 1.0)
        progressText.text = (int)(progress * 100f) + "%";  // "45%", "67%", etc.

        yield return null;  // Đợi một frame, sau đó kiểm tra lại
    }

    // BƯỚC 4: Scene bây giờ đã loaded! Unity tự động switch sang nó.
}
```

**Visual Flow**:
```
Frame 1:  progress = 0.0   → slider = 0%    → "0%"
Frame 50: progress = 0.25  → slider = 25%   → "25%"
Frame 100: progress = 0.5  → slider = 50%   → "50%"
Frame 150: progress = 0.75 → slider = 75%   → "75%"
Frame 200: progress = 1.0  → slider = 100%  → "100%"
          → isDone = true  → Loop thoát
          → Scene mới activates
```

**Tại Sao Chia Cho 0.9?**

Unity's `operation.progress` đi từ **0 đến 0.9**, không phải 0 đến 1.0. 10% cuối cùng được dành riêng cho scene activation. Chia cho 0.9 normalize nó:

```
operation.progress = 0.0  → 0.0 / 0.9 = 0.00 (0%)
operation.progress = 0.45 → 0.45 / 0.9 = 0.50 (50%)
operation.progress = 0.9  → 0.9 / 0.9 = 1.00 (100%)
```

---

### Scene Loading Helpers

**Load Main Menu**:
```csharp
public void LoadHomeMenuScene()
{
    SoundManager.Click();  // Phát click sound
    StartCoroutine(LoadAsynchronously("Menu"));  // Load "Menu" scene
}
```

**Restart Current Level**:
```csharp
public void RestarLevel()
{
    SoundManager.Click();
    StartCoroutine(LoadAsynchronously(
        SceneManager.GetActiveScene().name  // Reload current scene
    ));
}
```

**Load Next Level**:
```csharp
public void LoadNextLevel()
{
    SoundManager.Click();
    GlobalValue.levelPlaying++;  // Tăng level number (1 → 2 → 3)
    StartCoroutine(LoadAsynchronously(
        SceneManager.GetActiveScene().name  // Reload game scene với new level
    ));
}
```

**Lưu Ý Quan Trọng**: `LoadNextLevel()` không load một scene khác—nó **reload cùng scene** nhưng với `GlobalValue.levelPlaying` được tăng. GameManager sau đó instantiates một level prefab khác.

---

## Complete Level Loading Flow

Đây là **complete end-to-end flow** khi player bắt đầu một level:

### Flow Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│              COMPLETE LEVEL LOADING SEQUENCE                    │
└─────────────────────────────────────────────────────────────────┘

1. HÀNH ĐỘNG PLAYER
   └─ Player clicks "Start Level 5" button

2. UI EVENT
   └─ Button.onClick() → LevelButton.OnClick()
      └─ Đặt GlobalValue.levelPlaying = 5
      └─ Gọi MenuManager.LoadAsynchronously("Game")

3. ASYNC LOADING BẮT ĐẦU
   ├─ MenuManager hiển thị LoadingUI
   ├─ SceneManager.LoadSceneAsync("Game") bắt đầu
   ├─ Progress bar cập nhật (0% → 100%)
   └─ Scene loads ở background

4. GAME SCENE ACTIVATES
   └─ GameManager.Awake() chạy

5. LEVEL PREFAB INSTANTIATION
   └─ GameManager.Awake():
      if (GameMode.Instance == null)
          Instantiate(gameLevels[1])  // Default: Level 1
      else
          Instantiate(gameLevels[GlobalValue.levelPlaying - 1])
          └─ GlobalValue.levelPlaying = 5
             └─ Instantiates gameLevels[4] (index 4 = Level 5)

6. LEVEL SETUP INITIALIZATION
   └─ Level prefab được instantiate chứa GameLevelSetup component
      └─ GameLevelSetup.Awake():
         ├─ Instance = this (singleton)
         ├─ DontDestroyOnLoad(gameObject)
         └─ GlobalValue.finishGameAtLevel = levelWaves.Count

7. LEVEL ENEMY MANAGER ĐỌC CONFIG
   └─ LevelEnemyManager.Start():
      └─ EnemyWave[] waves = GameLevelSetup.Instance.GetLevelWave()
         └─ Tìm kiếm levelWaves cho level == 5
            └─ Trả về waves cho Level 5

8. GAME BẮT ĐẦU
   └─ GameManager.StartGame()
      └─ Broadcasts IPlay() đến tất cả listeners
         └─ LevelEnemyManager.IPlay()
            └─ Bắt đầu spawning enemies từ waves
```

### Code Walkthrough

**Bước 1: GameManager Instantiates Level**

Trong `GameManager.cs`:

```csharp
[Header("LEVELS")]
public GameObject[] gameLevels;  // Gán level prefabs trong Inspector

void Awake()
{
    // ... các setup khác ...

    // Spawn level prefab đúng
    if (GameMode.Instance == null)
    {
        // Không có level được chọn, dùng default (Level 1)
        Instantiate(gameLevels[1], Vector2.zero, Quaternion.identity);
    }
    else
    {
        // Dùng level player đã chọn
        // GlobalValue.levelPlaying = 1 → gameLevels[0]
        // GlobalValue.levelPlaying = 2 → gameLevels[1]
        // etc.
        Instantiate(
            gameLevels[GlobalValue.levelPlaying - 1],
            Vector2.zero,
            Quaternion.identity
        );
    }
}
```

**Ví Dụ**:
```
GlobalValue.levelPlaying = 3

gameLevels array trong Inspector:
[0] Level_1_Prefab
[1] Level_2_Prefab
[2] Level_3_Prefab  ← Cái này được instantiated!
[3] Level_4_Prefab
```

**Bước 2: GameLevelSetup Initializes**

Level prefab được instantiate có component này:

```csharp
void Awake()
{
    Instance = this;  // Làm cho nó accessible globally
    DontDestroyOnLoad(gameObject);  // Giữ nó khi scenes thay đổi

    // Nói với game có bao nhiêu levels tồn tại
    GlobalValue.finishGameAtLevel = levelWaves.Count;
}
```

**Bước 3: LevelEnemyManager Yêu Cầu Wave Data**

```csharp
void Start()
{
    EnemyWave[] waves = GameLevelSetup.Instance.GetLevelWave();
    // waves bây giờ chứa enemy spawn data cho current level
}
```

**Bước 4: GameManager Starts Game**

```csharp
public void StartGame()
{
    State = GameState.Playing;

    // Thông báo cho tất cả listeners (bao gồm LevelEnemyManager)
    foreach (var listener in listeners)
    {
        if (listener != null)
            listener.IPlay();
    }
}
```

**Bước 5: LevelEnemyManager Spawns Enemies**

```csharp
public void IPlay()
{
    isPlaying = true;
    StartCoroutine(SpawnEnemyCo());  // Bắt đầu spawning waves
}
```

---

## Tạo Level Mới

Làm theo step-by-step guide này để thêm một level mới vào game của bạn.

### Bước 1: Tạo Level Prefab

**Option A: Duplicate Level Có Sẵn**

1. Trong Project window, điều hướng đến `Assets/_MonstersOut/Prefabs/Levels/`
2. Right-click level prefab có sẵn (ví dụ, `Level_1`)
3. Click **Duplicate**
4. Đổi tên thành `Level_4` (hoặc số tiếp theo)

**Option B: Tạo Từ Đầu**

1. Trong Hierarchy, right-click → **Create Empty**
2. Đặt tên nó `Level_4`
3. Thêm `GameLevelSetup` component
4. Thêm `LevelEnemyManager` component
5. Tạo child GameObject tên là `Level 4`
6. Thêm `LevelWave` component vào child
7. Kéo từ Hierarchy sang Project window để tạo prefab

### Bước 2: Cấu Hình GameLevelSetup

Chọn level prefab mới của bạn và cấu hình:

```
GameLevelSetup Component:
├─ Amount Mana: 2          (Mana được thêm mỗi interval)
├─ Rate: 2.0               (Thêm mana mỗi 2 giây)
└─ Level Waves: (auto-populated, xem Bước 3)
```

### Bước 3: Cấu Hình LevelWave

`GameLevelSetup` prefab nên có child objects với `LevelWave` components.

**Để thêm một level config mới**:

1. Chọn `GameLevelSetup` trong Hierarchy
2. Right-click → **Create Empty**
3. Thêm `LevelWave` component
4. Cấu hình:

```
LevelWave Component:
├─ Level: 4                (Auto-set bởi OnDrawGizmos)
├─ Given Mana: 1500        (Starting mana cho player)
├─ Enemy Fortrest Level: 3 (Fortress HP tier, 1-5)
└─ Waves: (xem Bước 4)
```

### Bước 4: Cấu Hình Enemy Waves

Click nút **+** để thêm waves vào `Waves` array:

```
Wave 0: (First wave)
├─ Size: 2
│  ├─ Element 0:
│  │  ├─ Enemy Prefab: Goblin
│  │  ├─ Amount: 5
│  │  └─ Delay: 1.0
│  └─ Element 1:
│     ├─ Enemy Prefab: Skeleton
│     ├─ Amount: 3
│     └─ Delay: 2.0

Wave 1: (Second wave)
├─ Size: 1
│  └─ Element 0:
│     ├─ Enemy Prefab: TrollBoss
│     ├─ Amount: 1
│     └─ Delay: 0
```

**Điều Này Có Nghĩa Là**:
```
Wave 0 bắt đầu:
  - Spawn 5 Goblins, cách nhau 1 giây
  - Sau đó spawn 3 Skeletons, cách nhau 2 giây

Sau khi Wave 0 hoàn thành:
  - Wave 1 bắt đầu
  - Spawn 1 TrollBoss
```

### Bước 5: Thêm Level Vào GameManager

1. Mở Game scene của bạn
2. Chọn `GameManager` trong Hierarchy
3. Trong Inspector, tìm `Game Levels` array
4. Tăng **Size** thêm 1
5. Kéo level prefab mới của bạn vào slot mới

```
GameManager:
Game Levels:
  Size: 4
  Element 0: Level_1
  Element 1: Level_2
  Element 2: Level_3
  Element 3: Level_4  ← Level mới của bạn!
```

**QUAN TRỌNG**: Array index bắt đầu từ 0, nhưng level numbers bắt đầu từ 1!

```
GlobalValue.levelPlaying = 1 → gameLevels[0] (Level_1)
GlobalValue.levelPlaying = 2 → gameLevels[1] (Level_2)
GlobalValue.levelPlaying = 3 → gameLevels[2] (Level_3)
GlobalValue.levelPlaying = 4 → gameLevels[3] (Level_4)
```

### Bước 6: Test Level Của Bạn

**Quick Test (Bỏ qua Map UI)**:

```csharp
// Trong GameManager.cs Awake(), tạm thời force level của bạn:
void Awake()
{
    // TEST: Force load Level 4
    GlobalValue.levelPlaying = 4;

    Instantiate(gameLevels[GlobalValue.levelPlaying - 1], Vector2.zero, Quaternion.identity);
}
```

**Proper Test (Qua Map UI)**:

1. Chạy game
2. Mở Map screen
3. Điều hướng đến world mới của bạn
4. Click level button
5. Xác minh:
   - Starting mana đúng
   - Enemy waves đúng
   - Fortress HP đúng

### Bước 7: Cấu Hình Map UI

Nếu bạn thêm một **world** mới (không chỉ một level), cập nhật `MapControllerUI`:

1. Chọn `MapControllerUI` trong Map scene của bạn
2. Cấu hình:

```
MapControllerUI:
├─ Block Level: (gán container RectTransform)
├─ How Many Blocks: 4  (tăng từ 3)
├─ Step: 720
└─ Dots: (thêm một Image nữa vào array)
```

3. Thêm một dot indicator Image mới:
   - Duplicate dot có sẵn trong Hierarchy
   - Thêm vào `Dots` array trong Inspector

---

## Kỹ Thuật Nâng Cao

### 1. Dynamic Difficulty Scaling

Điều chỉnh enemy stats dựa trên player performance:

```csharp
public class GameLevelSetup : MonoBehaviour
{
    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        GlobalValue.finishGameAtLevel = levelWaves.Count;

        // MỚI: Scale difficulty dựa trên player win streak
        ScaleDifficulty();
    }

    void ScaleDifficulty()
    {
        int winStreak = GlobalValue.ConsecutiveWins;

        if (winStreak >= 3)
        {
            // Player đang làm tốt, tăng difficulty
            foreach (var levelWave in levelWaves)
            {
                levelWave.enemyFortrestLevel = Mathf.Min(5, levelWave.enemyFortrestLevel + 1);
                levelWave.givenMana = (int)(levelWave.givenMana * 0.9f);  // 10% ít mana hơn
            }
        }
        else if (winStreak == 0 && GlobalValue.ConsecutiveLosses >= 2)
        {
            // Player đang gặp khó khăn, giảm difficulty
            foreach (var levelWave in levelWaves)
            {
                levelWave.enemyFortrestLevel = Mathf.Max(1, levelWave.enemyFortrestLevel - 1);
                levelWave.givenMana = (int)(levelWave.givenMana * 1.2f);  // 20% nhiều mana hơn
            }
        }
    }
}
```

### 2. Random Level Selection

Tạo variety bằng cách randomize level nào được load:

```csharp
public class GameManager : MonoBehaviour
{
    public GameObject[] gameLevels;
    public bool randomizeLevels = false;

    void Awake()
    {
        // ... các setup khác ...

        if (randomizeLevels)
        {
            // Pick một random level thay vì sequential
            int randomIndex = Random.Range(0, gameLevels.Length);
            Instantiate(gameLevels[randomIndex], Vector2.zero, Quaternion.identity);
        }
        else
        {
            // Normal sequential loading
            Instantiate(gameLevels[GlobalValue.levelPlaying - 1], Vector2.zero, Quaternion.identity);
        }
    }
}
```

### 3. Endless Mode

Tạo endless mode cycle qua các levels với increasing difficulty:

```csharp
public class EndlessModeManager : MonoBehaviour
{
    void Start()
    {
        if (GlobalValue.isEndlessMode)
        {
            StartCoroutine(EndlessModeCo());
        }
    }

    IEnumerator EndlessModeCo()
    {
        int difficultyMultiplier = 1;

        while (true)
        {
            // Đợi level hoàn thành
            yield return new WaitUntil(() => GameManager.Instance.State == GameManager.GameState.Success);

            // Tăng difficulty
            difficultyMultiplier++;

            // Cycle qua levels
            int nextLevel = (GlobalValue.levelPlaying % GameLevelSetup.Instance.levelWaves.Count) + 1;
            GlobalValue.levelPlaying = nextLevel;

            // Scale enemy stats
            foreach (var enemy in FindObjectsOfType<Enemy>())
            {
                enemy.maxHealth = (int)(enemy.maxHealth * difficultyMultiplier * 1.1f);
            }

            // Đợi một chút, sau đó load next level
            yield return new WaitForSeconds(3f);
            MenuManager.Instance.LoadNextLevel();
        }
    }
}
```

### 4. Custom Loading Screen Animation

Tăng cường loading screen với animated elements:

```csharp
public class CustomLoadingScreen : MonoBehaviour
{
    public Image loadingIcon;  // Spinning icon
    public Text tipText;       // Gameplay tips
    public string[] tips;      // Array của tips

    IEnumerator LoadAsynchronously(string name)
    {
        LoadingUI.SetActive(true);

        // Hiển thị random tip
        tipText.text = tips[Random.Range(0, tips.Length)];

        AsyncOperation operation = SceneManager.LoadSceneAsync(name);

        while (!operation.isDone)
        {
            // Quay loading icon
            loadingIcon.transform.Rotate(Vector3.forward * -360 * Time.deltaTime);

            // Cập nhật progress
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            slider.value = progress;
            progressText.text = (int)(progress * 100f) + "%";

            // Thay đổi tip mỗi 3 giây
            if (Time.frameCount % 180 == 0)
            {
                tipText.text = tips[Random.Range(0, tips.Length)];
            }

            yield return null;
        }
    }
}
```

### 5. Map UI Smooth Scrolling

Thay thế instant snap bằng smooth animated scrolling:

```csharp
public class MapControllerUI : MonoBehaviour
{
    public float scrollSpeed = 5f;  // Speed của animation
    private float targetPosX;       // Target position

    IEnumerator NextCo()
    {
        allowPressButton = false;
        SoundManager.Click();

        if (newPosX != (-step * (howManyBlocks - 1)))
        {
            currentPos++;
            targetPosX = newPosX - step;  // Đặt target
            targetPosX = Mathf.Clamp(targetPosX, -step * (howManyBlocks - 1), 0);
        }
        else
        {
            allowPressButton = true;
            yield break;
        }

        // Smooth scroll thay vì instant snap
        BlackScreenUI.instance.Show(0.15f);
        yield return new WaitForSeconds(0.15f);

        // Animate từ current position đến target
        float elapsed = 0f;
        float duration = 0.5f;
        float startPosX = newPosX;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            newPosX = Mathf.Lerp(startPosX, targetPosX, elapsed / duration);
            SetMapPosition();
            yield return null;
        }

        newPosX = targetPosX;  // Đảm bảo exact position
        SetMapPosition();

        BlackScreenUI.instance.Hide(0.15f);
        SetDots();
        allowPressButton = true;
    }
}
```

---

## Troubleshooting

### Vấn Đề 1: Level Không Load (Black Screen)

**Triệu Chứng**:
- Click level button
- Loading screen hiển thị
- Progress bar đạt 100%
- Game ở trên black screen

**Nguyên Nhân & Giải Pháp**:

**Nguyên Nhân A: Scene name không khớp**
```csharp
// Kiểm tra MenuManager.cs:
StartCoroutine(LoadAsynchronously("Game"));  // ← Phải khớp scene name chính xác!

// Xác minh scene name:
// Unity → File → Build Settings → Scenes In Build
// Tìm "Game" scene
```

**Giải Pháp**: Đảm bảo scene name trong `LoadAsynchronously` khớp Build Settings.

**Nguyên Nhân B: Scene không có trong Build Settings**
```
File → Build Settings
└─ Nếu "Game" scene thiếu, click "Add Open Scenes"
```

**Nguyên Nhân C: GameManager thiếu từ scene**
```
Game scene phải có một GameManager object!
Nếu nó thiếu:
1. Mở Game scene
2. Tạo empty GameObject
3. Thêm GameManager component
4. Cấu hình gameLevels array
```

---

### Vấn Đề 2: Sai Level Được Load

**Triệu Chứng**:
- Click "Level 3" button
- Level 1 loads thay vào đó

**Nguyên Nhân**: `GlobalValue.levelPlaying` không được set đúng

**Giải Pháp**:
```csharp
// Trong level button script của bạn:
public void OnLevelButtonClick()
{
    GlobalValue.levelPlaying = 3;  // Set TRƯỚC KHI loading scene
    MenuManager.Instance.LoadAsynchronously("Game");
}
```

**Debug Check**:
```csharp
// Trong GameManager.Awake(), thêm debug log:
void Awake()
{
    Debug.Log("Loading level: " + GlobalValue.levelPlaying);
    Instantiate(gameLevels[GlobalValue.levelPlaying - 1], Vector2.zero, Quaternion.identity);
}
```

---

### Vấn Đề 3: IndexOutOfRangeException Khi Loading Level

**Error Message**:
```
IndexOutOfRangeException: Index was outside the bounds of the array.
GameManager.Awake() (at Assets/.../GameManager.cs:56)
```

**Nguyên Nhân**: `gameLevels` array không có đủ elements

**Ví Dụ**:
```csharp
GlobalValue.levelPlaying = 5

gameLevels array:
  Size: 3
  [0] Level_1
  [1] Level_2
  [2] Level_3

// Cố truy cập gameLevels[4] → CRASH!
```

**Giải Pháp 1: Thêm missing level prefabs**
```
1. Chọn GameManager trong Hierarchy
2. Trong Inspector, tăng "Game Levels" Size lên 5
3. Gán Level_4 và Level_5 prefabs
```

**Giải Pháp 2: Clamp level number**
```csharp
void Awake()
{
    // Ngăn crash bằng cách clamp về valid range
    int levelIndex = Mathf.Clamp(GlobalValue.levelPlaying - 1, 0, gameLevels.Length - 1);
    Instantiate(gameLevels[levelIndex], Vector2.zero, Quaternion.identity);
}
```

---

### Vấn Đề 4: Map Navigation Buttons Không Hoạt Động

**Triệu Chứng**:
- Click "Next" button
- Không có gì xảy ra
- Không có error trong Console

**Các Bước Debugging**:

**Bước 1: Kiểm tra button được connected**
```
1. Chọn "Next" button trong Hierarchy
2. Kiểm tra Inspector → Button component → On Click()
3. Xác minh:
   - Target: MapControllerUI
   - Function: MapControllerUI.Next
```

**Bước 2: Kiểm tra allowPressButton**
```csharp
// Thêm debug log trong Next():
public void Next()
{
    Debug.Log("Next button clicked. allowPressButton = " + allowPressButton);

    if (allowPressButton)
    {
        StartCoroutine(NextCo());
    }
}
```

**Bước 3: Kiểm tra nếu đã ở last world**
```csharp
// Trong NextCo(), thêm logs:
IEnumerator NextCo()
{
    Debug.Log("newPosX = " + newPosX);
    Debug.Log("Max position = " + (-step * (howManyBlocks - 1)));

    if (newPosX != (-step * (howManyBlocks - 1)))
    {
        // Có thể di chuyển
    }
    else
    {
        Debug.Log("Already at last world!");
        allowPressButton = true;
        yield break;
    }
}
```

---

### Vấn Đề 5: Không Có Enemy Waves Spawn

**Triệu Chứng**:
- Level loads thành công
- Player fortress xuất hiện
- Không có enemies spawn

**Các Bước Debugging**:

**Bước 1: Kiểm tra GameLevelSetup.Instance**
```csharp
// Trong LevelEnemyManager.Start():
void Start()
{
    if (GameLevelSetup.Instance == null)
    {
        Debug.LogError("GameLevelSetup.Instance is NULL!");
        return;
    }

    EnemyWave[] waves = GameLevelSetup.Instance.GetLevelWave();

    if (waves == null)
    {
        Debug.LogError("GetLevelWave() returned NULL!");
        return;
    }

    Debug.Log("Found " + waves.Length + " waves for level " + GlobalValue.levelPlaying);
}
```

**Bước 2: Kiểm tra level prefab có GameLevelSetup**
```
1. Chọn level prefab của bạn trong Project window
2. Xác minh nó có GameLevelSetup component
3. Kiểm tra rằng levelWaves list được populated
```

**Bước 3: Kiểm tra level number khớp**
```csharp
// Trong GameLevelSetup.GetLevelWave(), thêm logs:
public EnemyWave[] GetLevelWave()
{
    Debug.Log("Looking for level: " + GlobalValue.levelPlaying);

    foreach (var obj in levelWaves)
    {
        Debug.Log("Checking LevelWave with level = " + obj.level);

        if (obj.level == GlobalValue.levelPlaying)
        {
            Debug.Log("Found matching level! Returning " + obj.Waves.Length + " waves");
            return obj.Waves;
        }
    }

    Debug.LogError("No matching level found!");
    return null;
}
```

**Fix Phổ Biến**: Level number không khớp
```
GlobalValue.levelPlaying = 3

Nhưng levelWaves list chỉ có:
- Level 1
- Level 2

Giải pháp: Thêm Level 3 configuration vào GameLevelSetup!
```

---

### Vấn Đề 6: Progress Bar Stuck Ở 90%

**Triệu Chứng**:
- Loading screen hiển thị
- Progress bar đạt 90%
- Không bao giờ hoàn thành

**Nguyên Nhân**: `operation.progress` naturally dừng ở 0.9

**Đây là hành vi Unity BÌNH THƯỜNG!** Scene vẫn đang loading ở background.

**Giải Pháp**: Thêm timeout hoặc đợi `isDone`:
```csharp
IEnumerator LoadAsynchronously(string name)
{
    LoadingUI.SetActive(true);
    AsyncOperation operation = SceneManager.LoadSceneAsync(name);

    float timeout = 10f;  // Max 10 giây
    float elapsed = 0f;

    while (!operation.isDone && elapsed < timeout)
    {
        elapsed += Time.deltaTime;

        float progress = Mathf.Clamp01(operation.progress / 0.9f);
        slider.value = progress;
        progressText.text = (int)(progress * 100f) + "%";

        yield return null;
    }

    if (elapsed >= timeout)
    {
        Debug.LogError("Scene load timeout! Check for missing assets.");
    }
}
```

---

## Tóm Tắt

**Những Điểm Chính**:

1. **MapControllerUI**: Xử lý horizontal scrolling UI navigation với Previous/Next buttons và dot indicators
2. **GameLevelSetup**: Lưu trữ level configurations (waves, mana, fortress HP) và cung cấp data cho managers
3. **MenuManager**: Loads scenes asynchronously với progress bar, ngăn game freeze
4. **Level Flow**: Button click → Set GlobalValue.levelPlaying → LoadAsynchronously → GameManager instantiates level prefab → GameLevelSetup cung cấp config → LevelEnemyManager spawns waves
5. **Tạo Levels**: Duplicate prefab → Cấu hình LevelWave → Thêm vào GameManager.gameLevels array → Test

**Essential Code Patterns**:

```csharp
// 1. Load scene asynchronously
AsyncOperation operation = SceneManager.LoadSceneAsync("Game");
while (!operation.isDone)
{
    float progress = Mathf.Clamp01(operation.progress / 0.9f);
    yield return null;
}

// 2. Instantiate level prefab
Instantiate(gameLevels[GlobalValue.levelPlaying - 1], Vector2.zero, Quaternion.identity);

// 3. Lấy level configuration
EnemyWave[] waves = GameLevelSetup.Instance.GetLevelWave();

// 4. Navigate map UI
newPosX -= step;  // Di chuyển left một block
newPosX = Mathf.Clamp(newPosX, -step * (howManyBlocks - 1), 0);
BlockLevel.anchoredPosition = new Vector2(newPosX, BlockLevel.anchoredPosition.y);
```

**Các Bước Tiếp Theo**:
- Xem `He_Thong_Enemy_Nang_Cao.md` để cấu hình enemy waves
- Xem `05_Cac_Manager_Day_Du.md` cho GameManager details
- Xem `10_Huong_Dan_Thuc_Hanh.md` cho step-by-step tutorials
- Xem `11_Xu_Ly_Su_Co.md` cho general debugging strategies

---

**Kết Thúc Tài Liệu**

<p align="center">
<strong>Lawn Defense: Monsters Out</strong><br>
Hệ Thống Map (Chọn Level & Scene Loading)<br>
Map System Technical Guide
</p>
