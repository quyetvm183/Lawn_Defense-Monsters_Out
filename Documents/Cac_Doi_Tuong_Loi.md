# Các Đối Tượng Lõi: Xương Sống Kiến Trúc
## Hiểu Về GameManager, MenuManager, SoundManager, và CameraController

**Ngôn Ngữ**: Tiếng Việt (Vietnamese)
**Phiên Bản Tài Liệu**: 2.0 (Updated October 2025)
**Bản Gốc**: Vietnamese (Version 1.0)
**Độ Khó**: Intermediate (Trung Cấp)
**Thời Gian Đọc**: 40-50 phút
**Điều Kiện Tiên Quyết**: Kiến thức Unity cơ bản, hiểu về classes và objects

---

## Mục Lục

1. [Giới Thiệu](#giới-thiệu)
2. [Tổng Quan: Bộ Tứ Chính](#tổng-quan-bộ-tứ-chính)
3. [GameManager - Bộ Điều Khiển Game State](#gamemanager---bộ-điều-khiển-game-state)
4. [MenuManager - Bộ Điều Khiển UI](#menumanager---bộ-điều-khiển-ui)
5. [SoundManager - Hệ Thống Âm Thanh](#soundmanager---hệ-thống-âm-thanh)
6. [CameraController - Quản Lý Góc Nhìn](#cameracontroller---quản-lý-góc-nhìn)
7. [Cách Chúng Hoạt Động Cùng Nhau](#cách-chúng-hoạt-động-cùng-nhau)
8. [Observer Pattern Giải Thích](#observer-pattern-giải-thích)
9. [Ví Dụ Thực Tế](#ví-dụ-thực-tế)
10. [Troubleshooting (Xử Lý Sự Cố)](#troubleshooting)
11. [Tóm Tắt](#tóm-tắt)

---

## Giới Thiệu

Tài liệu này giải thích **bốn đối tượng lõi** tạo thành xương sống của "Lawn Defense: Monsters Out":

1. **GameManager** - Điều khiển game state (menu, playing, paused, victory, defeat)
2. **MenuManager** - Quản lý UI panels và scene transitions
3. **SoundManager** - Xử lý tất cả audio (music và sound effects)
4. **CameraController** - Điều khiển camera movement và boundaries

**Tại Sao Chúng Quan Trọng**:
- Chúng điều phối **tất cả các systems khác** trong game
- Hiểu chúng là **cần thiết để modifications**
- Chúng dùng **design patterns** bạn sẽ thấy trong professional games
- Chúng là **singleton objects** (chỉ có một instance tồn tại)

**Bạn Sẽ Học Được**:
- Cách game state được quản lý
- Cách UI panels được hiển thị/ẩn
- Cách audio được phát trong suốt game
- Cách camera theo dõi hành động
- Cách các systems giao tiếp qua Observer pattern

---

## Tổng Quan: Bộ Tứ Chính

### Sơ Đồ Kiến Trúc

```
                    ┌─────────────────┐
                    │  GAME MANAGER   │ ◄─── Master Controller
                    │  (Singleton)    │      Controls game state
                    └────────┬────────┘
                             │
                ┌────────────┼────────────┐
                │            │            │
                ▼            ▼            ▼
        ┌──────────┐  ┌──────────┐  ┌──────────┐
        │  MENU    │  │  SOUND   │  │  CAMERA  │
        │ MANAGER  │  │ MANAGER  │  │CONTROLLER│
        └────┬─────┘  └────┬─────┘  └────┬─────┘
             │             │             │
             │             │             │
        ┌────┴─────┐  ┌────┴─────┐  ┌────┴─────┐
        │ UI Panel │  │  Music   │  │ Position │
        │ Control  │  │  & SFX   │  │& Limits  │
        └──────────┘  └──────────┘  └──────────┘

        All other game objects register as listeners
        and respond to GameManager state changes
```

### Ma Trận Trách Nhiệm (Responsibility Matrix)

| Manager | Vai Trò Chính | Key Methods | Được Dùng Bởi |
|---------|---------------|-------------|----------------|
| **GameManager** | Điều khiển game state | `StartGame()`, `Victory()`, `GameOver()` | Mọi thứ |
| **MenuManager** | Quản lý UI | `ShowPanel()`, `LoadScene()` | UI elements |
| **SoundManager** | Phát audio | `PlaySFX()`, `PlayMusic()` | Tất cả scripts |
| **CameraController** | Điều khiển view | `SetLimits()`, `FollowTarget()` | Player, Level |

### Singleton Pattern

Tất cả bốn managers dùng **Singleton pattern**:

```csharp
public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // ← Một global instance

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist giữa scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicates
        }
    }
}

// Usage từ bất kỳ đâu:
GameManager.Instance.Victory();
```

**Lợi Ích**:
- Truy cập từ bất kỳ script nào mà không cần `GetComponent` hay `Find`
- Đảm bảo single instance
- Persists qua scene loads

---

## GameManager - Bộ Điều Khiển Game State

**File**: `Assets/_MonstersOut/Scripts/Managers/GameManager.cs`

**Mục Đích**: Master controller cho game state và event broadcasting

### Trách Nhiệm Cốt Lõi

```
┌─────────────────────────────────────────┐
│           GAME MANAGER                  │
├─────────────────────────────────────────┤
│                                         │
│ 1. Quản Lý State                        │
│    ├─ Menu (không chơi)                 │
│    ├─ Playing (gameplay chủ động)       │
│    ├─ Pause (game đóng băng)            │
│    ├─ Success (thắng level)             │
│    └─ GameOver (thua level)             │
│                                         │
│ 2. Quản Lý Listener                     │
│    ├─ Thêm listeners (IListener)        │
│    ├─ Xóa listeners                     │
│    └─ Broadcast events đến tất cả       │
│                                         │
│ 3. Quản Lý Level                        │
│    ├─ Load level prefabs                │
│    ├─ Theo dõi current level            │
│    └─ Quản lý player reference          │
│                                         │
└─────────────────────────────────────────┘
```

### Game State Enum

```csharp
public enum GameState
{
    Prepare,   // Loading/initializing
    Playing,   // Active gameplay
    Pause,     // Paused (Time.timeScale = 0)
    Success,   // Level hoàn thành
    Fail       // Level thất bại
}

public GameState State = GameState.Prepare;
```

### Sơ Đồ Luồng State

```
        START
          │
          ▼
    ┌──────────┐
    │ PREPARE  │ ◄─── Scene loading
    └────┬─────┘
         │
         │ StartGame()
         │
         ▼
    ┌──────────┐
    │ PLAYING  │ ◄─── Active gameplay
    └────┬─────┘
         │
    ┌────┴─────────────────┬─────────────┐
    │                      │             │
    │ Pause()         Victory()     GameOver()
    │                      │             │
    ▼                      ▼             ▼
┌─────────┐          ┌─────────┐   ┌─────────┐
│ PAUSE   │          │ SUCCESS │   │  FAIL   │
└────┬────┘          └─────────┘   └─────────┘
     │
     │ Resume()
     │
     └───────────────────┘
```

### Các Methods Quan Trọng Giải Thích

#### StartGame()

```csharp
public void StartGame()
{
    Debug.Log("Game starting!");

    // Thay đổi state
    State = GameState.Playing;

    // Thông báo tất cả listeners
    foreach (var listener in listeners)
    {
        if (listener != null)
            listener.IPlay(); // ← Gọi IPlay() trên mỗi listener
    }
}
```

**Chuyện gì xảy ra**:
1. State thay đổi thành `Playing`
2. Mỗi registered listener nhận được `IPlay()` call
3. Listeners phản ứng (ví dụ, enemy AI starts, UI updates)

**Biểu diễn trực quan**:
```
StartGame() được gọi
      │
      ├─→ State = Playing
      │
      └─→ Broadcast đến listeners:
          ├─ Enemy.IPlay()     → Khởi động AI
          ├─ UI.IPlay()        → Hiển thị HUD
          ├─ Spawner.IPlay()   → Bắt đầu waves
          └─ Timer.IPlay()     → Khởi động countdown
```

#### Victory()

```csharp
public void Victory()
{
    Debug.Log("Victory!");

    // Thay đổi state
    State = GameState.Success;

    // Thông báo tất cả listeners
    foreach (var listener in listeners)
    {
        if (listener != null)
            listener.ISuccess(); // ← Victory event
    }
}
```

**Chuyện gì xảy ra**:
1. State thay đổi thành `Success`
2. Broadcast `ISuccess()` đến tất cả listeners
3. Listeners phản ứng:
   - MenuManager hiển thị victory screen
   - Enemies ngừng tấn công
   - Score được tính toán
   - Coins được thưởng

#### GameOver()

```csharp
public void GameOver()
{
    Debug.Log("Game Over!");

    // Thay đổi state
    State = GameState.Fail;

    // Thông báo tất cả listeners
    foreach (var listener in listeners)
    {
        if (listener != null)
            listener.IGameOver(); // ← Failure event
    }
}
```

#### Pause/Resume

```csharp
public void Pause()
{
    State = GameState.Pause;
    Time.timeScale = 0; // ← Đóng băng game time

    foreach (var listener in listeners)
    {
        if (listener != null)
            listener.IPause();
    }
}

public void Resume()
{
    State = GameState.Playing;
    Time.timeScale = 1; // ← Khôi phục normal time

    foreach (var listener in listeners)
    {
        if (listener != null)
            listener.IPlay();
    }
}
```

**Time.timeScale giải thích**:
```
timeScale = 1.0    Tốc độ bình thường (60 FPS = 60 updates/sec)
timeScale = 0.5    Nửa tốc độ        (60 FPS = 30 updates/sec)
timeScale = 0.0    Đóng băng         (không có Update() calls)
timeScale = 2.0    Gấp đôi tốc độ    (60 FPS = 120 updates/sec)
```

### Quản Lý Listener

```csharp
public List<IListener> listeners = new List<IListener>();

public void AddListener(IListener listener)
{
    if (!listeners.Contains(listener))
    {
        listeners.Add(listener);
        Debug.Log("Listener added: " + listener);
    }
}

public void RemoveListener(IListener listener)
{
    if (listeners.Contains(listener))
    {
        listeners.Remove(listener);
        Debug.Log("Listener removed: " + listener);
    }
}
```

**Objects đăng ký chính họ**:
```csharp
// Trong Enemy.cs, MenuManager.cs, UI.cs, v.v.
void Start()
{
    GameManager.Instance.AddListener(this);
}

void OnDestroy()
{
    if (GameManager.Instance != null)
        GameManager.Instance.RemoveListener(this);
}
```

### Quản Lý Level

```csharp
[Header("Levels")]
public GameObject[] gameLevels;     // Array của level prefabs
public int currentLevel = 0;        // Current level index

void Awake()
{
    // Instantiate current level
    if (currentLevel < gameLevels.Length)
    {
        GameObject level = Instantiate(
            gameLevels[currentLevel],
            Vector3.zero,
            Quaternion.identity
        );

        Debug.Log("Loaded level: " + level.name);
    }
}
```

**Luồng level loading**:
```
GameManager.Awake()
      │
      ├─→ Đọc currentLevel (ví dụ, 0)
      │
      ├─→ Lấy gameLevels[0] (Level 1 prefab)
      │
      ├─→ Instantiate level trong scene
      │
      └─→ Level spawns:
          ├─ Ground
          ├─ Fortresses
          ├─ Spawn points
          └─ LevelEnemyManager
```

### Ví Dụ Cách Dùng Thực Tế

**Ví dụ 1: Kiểm tra nếu game đang chơi**
```csharp
void Update()
{
    // Chỉ di chuyển nếu game đang chơi
    if (GameManager.Instance.State == GameState.Playing)
    {
        transform.position += Vector3.right * speed * Time.deltaTime;
    }
}
```

**Ví dụ 2: Kích hoạt victory từ script**
```csharp
void OnAllEnemiesDefeated()
{
    Debug.Log("All enemies defeated!");
    GameManager.Instance.Victory();
}
```

**Ví dụ 3: Lắng nghe game events**
```csharp
public class MyScript : MonoBehaviour, IListener
{
    void Start()
    {
        GameManager.Instance.AddListener(this);
    }

    // Implement IListener interface
    public void IPlay()
    {
        Debug.Log("Game started!");
    }

    public void ISuccess()
    {
        Debug.Log("Level won!");
    }

    public void IGameOver()
    {
        Debug.Log("Level lost!");
    }

    public void IPause()
    {
        Debug.Log("Game paused!");
    }

    void OnDestroy()
    {
        GameManager.Instance?.RemoveListener(this);
    }
}
```

---

## MenuManager - Bộ Điều Khiển UI

**File**: `Assets/_MonstersOut/Scripts/UI/MenuManager.cs`

**Mục Đích**: Quản lý UI panels và scene transitions

### Trách Nhiệm Cốt Lõi

```
┌─────────────────────────────────────────┐
│          MENU MANAGER                   │
├─────────────────────────────────────────┤
│                                         │
│ 1. Quản Lý Panel                        │
│    ├─ StartUI (main menu)               │
│    ├─ UI (in-game HUD)                  │
│    ├─ VictoryUI (win screen)            │
│    ├─ FailUI (lose screen)              │
│    └─ PauseUI (pause menu)              │
│                                         │
│ 2. Scene Loading                        │
│    ├─ Async scene transitions           │
│    ├─ Loading screen                    │
│    └─ Progress tracking                 │
│                                         │
│ 3. Điều Khiển Audio                     │
│    ├─ Sound toggle                      │
│    ├─ Music toggle                      │
│    └─ Volume sliders                    │
│                                         │
└─────────────────────────────────────────┘
```

### Hệ Thống Panel

**UI Hierarchy**:
```
Canvas
├── StartUI (Main Menu)
│   ├── PlayButton
│   ├── SettingsButton
│   └── QuitButton
│
├── UI (In-Game HUD)
│   ├── HealthBar
│   ├── CoinDisplay
│   ├── WaveCounter
│   └── PauseButton
│
├── VictoryUI (Win Screen)
│   ├── Stars (1-3 based on performance)
│   ├── Coins Earned
│   ├── NextButton
│   └── MenuButton
│
├── FailUI (Lose Screen)
│   ├── "Defeat" Message
│   ├── RetryButton
│   └── MenuButton
│
└── PauseUI (Pause Menu)
    ├── ResumeButton
    ├── RestartButton
    └── QuitButton
```

### Các Methods Quan Trọng

#### ShowPanel()

```csharp
public void ShowStartUI()
{
    StartUI.SetActive(true);
    UI.SetActive(false);
    VictoryUI.SetActive(false);
    FailUI.SetActive(false);
    PauseUI.SetActive(false);

    Debug.Log("Showing start UI");
}

public void ShowGameUI()
{
    StartUI.SetActive(false);
    UI.SetActive(true);           // ← Hiển thị in-game HUD
    VictoryUI.SetActive(false);
    FailUI.SetActive(false);
    PauseUI.SetActive(false);

    // Bắt đầu game
    GameManager.Instance.StartGame();
}
```

**Panel visibility pattern**:
```
CHỈ MỘT panel active tại một thời điểm:

[StartUI] ─────┐
               ├──→ Chỉ một ✓
[UI]       ────┤
               │
[VictoryUI]────┤
               │
[FailUI]   ────┤
               │
[PauseUI]  ────┘
```

#### Scene Loading (Async)

```csharp
using UnityEngine.SceneManagement;

public void LoadSceneWithName(string sceneName)
{
    StartCoroutine(LoadAsynchronously(sceneName));
}

IEnumerator LoadAsynchronously(string sceneName)
{
    // Bắt đầu loading scene ở background
    AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

    // Hiển thị loading panel
    loadingPanel.SetActive(true);

    // Đợi đến khi scene được load
    while (!operation.isDone)
    {
        // Cập nhật loading bar (0.0 đến 1.0)
        float progress = Mathf.Clamp01(operation.progress / 0.9f);
        loadingSlider.value = progress;
        loadingText.text = (progress * 100f) + "%";

        yield return null; // Đợi một frame
    }

    // Scene đã load!
    Debug.Log("Scene loaded: " + sceneName);
}
```

**Lợi ích async loading**:
- Game không đóng băng trong khi loading
- Có thể hiển thị loading bar
- Trải nghiệm người dùng tốt hơn

**Loading timeline**:
```
Time: 0s ────────────1s────────────2s────────────3s
      │             │             │             │
      ▼             ▼             ▼             ▼
   Bắt đầu load  30% loaded    70% loaded   Hoàn thành
      │             │             │             │
      └─────────────┴─────────────┴─────────────┘
              Loading bar fills →
```

#### Audio Toggle

```csharp
public void SoundVolume()
{
    // Toggle sound on/off
    if (SoundManager.Instance.soundVolume == 0)
    {
        SoundManager.Instance.soundVolume = 1;
        soundButton.sprite = soundOnSprite;
    }
    else
    {
        SoundManager.Instance.soundVolume = 0;
        soundButton.sprite = soundOffSprite;
    }
}

public void MusicVolume()
{
    // Toggle music on/off
    if (SoundManager.Instance.musicVolume == 0)
    {
        SoundManager.Instance.musicVolume = 0.5f;
        musicButton.sprite = musicOnSprite;
        SoundManager.Instance.musicsGame.UnPause();
    }
    else
    {
        SoundManager.Instance.musicVolume = 0;
        musicButton.sprite = musicOffSprite;
        SoundManager.Instance.musicsGame.Pause();
    }
}
```

### IListener Implementation

MenuManager lắng nghe GameManager events:

```csharp
public class MenuManager : MonoBehaviour, IListener
{
    void Start()
    {
        GameManager.Instance.AddListener(this);
    }

    // Được gọi khi level hoàn thành
    public void ISuccess()
    {
        Debug.Log("Victory UI triggered");
        ShowVictoryUI();
    }

    // Được gọi khi level thất bại
    public void IGameOver()
    {
        Debug.Log("Fail UI triggered");
        ShowFailUI();
    }

    // IListener methods khác...
    public void IPlay() { }
    public void IPause() { }

    void OnDestroy()
    {
        GameManager.Instance?.RemoveListener(this);
    }
}
```

**Luồng event**:
```
GameManager.Victory() được gọi
        │
        └─→ Broadcasts ISuccess() đến listeners
            │
            ├─→ MenuManager.ISuccess()
            │   └─→ Hiển thị VictoryUI
            │
            ├─→ Enemy.ISuccess()
            │   └─→ Ngừng tấn công
            │
            └─→ UI.ISuccess()
                └─→ Cập nhật final score
```

### Ví Dụ Thực Tế

**Ví dụ 1: Bắt đầu game từ button**
```csharp
// Gán vào "Play" button
public void OnPlayButtonClicked()
{
    ShowGameUI();              // Chuyển sang game HUD
    // GameManager.StartGame() được gọi tự động trong ShowGameUI()
}
```

**Ví dụ 2: Pause game**
```csharp
// Gán vào pause button
public void OnPauseButtonClicked()
{
    GameManager.Instance.Pause();  // Đóng băng game
    ShowPauseUI();                  // Hiển thị pause menu
}
```

**Ví dụ 3: Load next level**
```csharp
// Gán vào "Next Level" button
public void OnNextLevelClicked()
{
    GlobalValue.levelPlaying++;            // Tăng level
    LoadSceneWithName("GameplayScene");    // Reload gameplay scene
}
```

---

## SoundManager - Hệ Thống Âm Thanh

**File**: `Assets/_MonstersOut/Scripts/Managers/SoundManager.cs`

**Mục Đích**: Phát audio tập trung cho music và sound effects

### Trách Nhiệm Cốt Lõi

```
┌─────────────────────────────────────────┐
│          SOUND MANAGER                  │
├─────────────────────────────────────────┤
│                                         │
│ 1. Phát Music                           │
│    ├─ Background music looping          │
│    ├─ Music volume control              │
│    └─ Music transitions                 │
│                                         │
│ 2. Sound Effects (SFX)                  │
│    ├─ One-shot sounds                   │
│    ├─ Positional audio                  │
│    └─ SFX volume control                │
│                                         │
│ 3. Audio Sources                        │
│    ├─ Music AudioSource (looping)       │
│    └─ SFX AudioSource (one-shot)        │
│                                         │
└─────────────────────────────────────────┘
```

### Kiến Trúc Hệ Thống Audio

```
┌──────────────────────────────────┐
│      SOUND MANAGER               │
│                                  │
│  ┌────────────┐  ┌────────────┐ │
│  │ Music      │  │ SFX        │ │
│  │ Source     │  │ Source     │ │
│  └─────┬──────┘  └─────┬──────┘ │
└────────┼───────────────┼────────┘
         │               │
         ▼               ▼
    ┌─────────┐     ┌─────────┐
    │Background│    │Shoot    │
    │ Music    │    │Hit      │
    │(Looping) │    │Jump     │
    │          │    │Die      │
    └─────────┘     │(One-shot)
                    └─────────┘
```

### Các Components Quan Trọng

#### Audio Sources

```csharp
[Header("Audio Sources")]
public AudioSource musicsGame;     // Cho background music (looping)
public AudioSource[] sounds;       // Cho sound effects (one-shot)

[Header("Volume")]
[Range(0f, 1f)]
public float musicVolume = 0.5f;   // Music volume (0 = mute, 1 = full)
[Range(0f, 1f)]
public float soundVolume = 1f;     // SFX volume
```

**AudioSource setup**:
```
Music AudioSource:
├─ Loop: ✓ (enabled)
├─ Play On Awake: ✓
├─ Volume: 0.5
└─ Clip: background_music.mp3

SFX AudioSource:
├─ Loop: ✗ (disabled)
├─ Play On Awake: ✗
├─ Volume: 1.0
└─ Clip: (none - assigned tại runtime)
```

#### Sound Clips Library

```csharp
[Header("Sound Clips")]
public AudioClip soundClick;
public AudioClip soundShoot;
public AudioClip soundHit;
public AudioClip soundDie;
public AudioClip soundVictory;
public AudioClip soundFail;
public AudioClip soundCoin;
public AudioClip soundUpgrade;
// ... nhiều hơn
```

### Các Methods Quan Trọng

#### PlaySfx() - Phát Sound Effect

```csharp
public static void PlaySfx(AudioClip sound)
{
    if (Instance.soundVolume == 0)
        return; // Muted

    if (sound != null)
    {
        // Phát one-shot (không interrupt current sound)
        Instance.sounds[0].PlayOneShot(sound, Instance.soundVolume);
    }
}
```

**Ví dụ cách dùng**:
```csharp
// Khi player bắn
SoundManager.PlaySfx(SoundManager.Instance.soundShoot);

// Khi enemy chết
SoundManager.PlaySfx(SoundManager.Instance.soundDie);

// Khi coin được thu thập
SoundManager.PlaySfx(SoundManager.Instance.soundCoin);
```

**PlayOneShot giải thích**:
```
Nhiều sounds có thể phát đồng thời:

Timeline:
0s ─────1s──────2s──────3s──────4s
│        │       │       │       │
Shoot ───┘       │       │       │  (0.5s duration)
         Hit ────┘       │       │  (0.3s duration)
                 Coin ───┘       │  (0.4s duration)
                         Die ────┘  (1.0s duration)

Tất cả sounds overlap tự nhiên!
```

#### PlayMusic() - Phát Background Music

```csharp
public static void PlayMusic(AudioClip music)
{
    if (Instance.musicsGame.isPlaying)
    {
        Instance.musicsGame.Stop();
    }

    Instance.musicsGame.clip = music;
    Instance.musicsGame.volume = Instance.musicVolume;
    Instance.musicsGame.loop = true;  // ← Loop liên tục
    Instance.musicsGame.Play();

    Debug.Log("Playing music: " + music.name);
}
```

**Cách Dùng**:
```csharp
// Trong menu
SoundManager.PlayMusic(SoundManager.Instance.musicMenu);

// Trong game
SoundManager.PlayMusic(SoundManager.Instance.musicGame);

// Boss fight
SoundManager.PlayMusic(SoundManager.Instance.musicBoss);
```

#### Volume Control

```csharp
public void SetMusicVolume(float volume)
{
    musicVolume = Mathf.Clamp01(volume); // Đảm bảo 0-1
    musicsGame.volume = musicVolume;

    // Lưu vào PlayerPrefs
    PlayerPrefs.SetFloat("MusicVolume", musicVolume);
    PlayerPrefs.Save();
}

public void SetSoundVolume(float volume)
{
    soundVolume = Mathf.Clamp01(volume);

    // Lưu vào PlayerPrefs
    PlayerPrefs.SetFloat("SoundVolume", soundVolume);
    PlayerPrefs.Save();
}
```

**Kết nối với UI sliders**:
```
[Settings Panel]
┌──────────────────────────────┐
│ Music Volume:                │
│ [─────────●──] 0.5           │ ← Slider
│  OnValueChanged:             │
│  → SoundManager.SetMusicVolume(value)
│                              │
│ SFX Volume:                  │
│ [───────────●] 1.0           │ ← Slider
│  OnValueChanged:             │
│  → SoundManager.SetSoundVolume(value)
└──────────────────────────────┘
```

### Ví Dụ Tích Hợp Audio

**Ví dụ 1: Enemy nhận sát thương**
```csharp
// Trong Enemy.cs
public void TakeDamage(float damage)
{
    currentHealth -= damage;

    // Phát hurt sound
    SoundManager.PlaySfx(SoundManager.Instance.soundHurt);

    if (currentHealth <= 0)
    {
        Die();
    }
}
```

**Ví dụ 2: Player bắn arrow**
```csharp
// Trong Player_Archer.cs
void Shoot()
{
    // Spawn arrow
    GameObject arrow = Instantiate(arrowPrefab, shootPoint.position, Quaternion.identity);

    // Phát shoot sound
    SoundManager.PlaySfx(SoundManager.Instance.soundShoot);

    // Phát animation
    animator.SetTrigger("Shoot");
}
```

**Ví dụ 3: Victory celebration**
```csharp
// Trong GameManager.cs
public void Victory()
{
    State = GameState.Success;

    // Phát victory sound
    SoundManager.PlaySfx(SoundManager.Instance.soundVictory);

    // Thông báo listeners
    foreach (var listener in listeners)
    {
        listener?.ISuccess();
    }
}
```

### Best Practices

**NÊN**:
✅ Dùng `PlaySfx()` cho short sounds (< 2 giây)
✅ Dùng `PlayMusic()` cho background music
✅ Kiểm tra volume trước khi phát (SoundManager làm cái này)
✅ Assign clips trong Inspector trước khi phát
✅ Lưu volume preferences vào PlayerPrefs

**KHÔNG NÊN**:
❌ Phát music với `PlaySfx()` (sẽ không loop)
❌ Phát SFX với nhiều AudioSources (dùng một)
❌ Quên kiểm tra nếu clip được assigned (null check)
❌ Phát sounds mỗi frame (gây audio spam)

---

## CameraController - Quản Lý Góc Nhìn

**File**: `Assets/_MonstersOut/Scripts/Controllers/CameraController.cs`

**Mục Đích**: Điều khiển camera position, boundaries, và smooth movement

### Trách Nhiệm Cốt Lõi

```
┌─────────────────────────────────────────┐
│       CAMERA CONTROLLER                 │
├─────────────────────────────────────────┤
│                                         │
│ 1. Position Clamping                    │
│    ├─ Left boundary limit               │
│    ├─ Right boundary limit              │
│    └─ Giữ camera trong bounds           │
│                                         │
│ 2. Smooth Movement                      │
│    ├─ Lerp đến target position          │
│    ├─ Smooth speed control              │
│    └─ Không có jarring movements        │
│                                         │
│ 3. Input Handling                       │
│    ├─ Mouse drag (PC)                   │
│    ├─ Touch drag (Mobile)               │
│    └─ Follow target (optional)          │
│                                         │
└─────────────────────────────────────────┘
```

### Camera Boundaries

**Level Layout**:
```
                 Camera View (vùng visible)
                 ┌─────────────┐
                 │             │
  Limit Left     │   Player    │     Limit Right
      │          │             │          │
      ▼          └─────────────┘          ▼
──────┼──────────────────────────────────┼──────
     -10                                  15
      ◄────────────────────────────────►
          Camera movement range (25 units)

Ngoài bounds: Empty space, không có enemies
```

### Các Biến Quan Trọng

```csharp
[Header("Camera Limits")]
public float limitLeft = -5f;      // Vị trí trái nhất
public float limitRight = 5f;      // Vị trí phải nhất

[Header("Movement")]
public float smoothSpeed = 3f;     // Camera follows nhanh như thế nào
public bool followTarget = false;  // Auto-follow player?
public Transform target;           // Theo dõi cái gì (player)
```

### Hệ Thống Movement

#### Clamping Position

```csharp
void Update()
{
    // Lấy desired camera position
    Vector3 targetPos = GetTargetPosition();

    // Clamp X position đến limits
    float clampedX = Mathf.Clamp(
        targetPos.x,
        limitLeft,
        limitRight
    );

    // Tạo final position
    Vector3 finalPos = new Vector3(
        clampedX,
        transform.position.y,  // Giữ Y unchanged
        transform.position.z   // Giữ Z unchanged (-10 for 2D)
    );

    // Smooth movement
    transform.position = Vector3.Lerp(
        transform.position,
        finalPos,
        smoothSpeed * Time.deltaTime
    );
}
```

**Mathf.Clamp visualization**:
```
Desired X: -12    →  Clamped: -10  (ở left limit)
Desired X: -5     →  Clamped: -5   (trong range)
Desired X: 0      →  Clamped: 0    (trong range)
Desired X: 20     →  Clamped: 15   (ở right limit)

Công thức: Mathf.Clamp(value, limitLeft, limitRight)
```

#### Smooth Following (Lerp)

```csharp
// Linear Interpolation (Lerp)
newPos = Lerp(currentPos, targetPos, t)

// t = speed * Time.deltaTime (0.0 đến 1.0)
// t = 0.0 → Ở lại current (không di chuyển)
// t = 0.5 → Di chuyển nửa đường
// t = 1.0 → Nhảy đến target (tức thì)

// Ví dụ với smoothSpeed = 5:
t = 5 * 0.016 = 0.08 mỗi frame
→ Di chuyển 8% gần hơn mỗi frame
→ Smooth, gradual movement
```

**Ví dụ trực quan**:
```
Frame 1: Current (0)  ─────────→  Target (10)
         Di chuyển 8% → New position: 0.8

Frame 2: Current (0.8) ────────→  Target (10)
         Di chuyển 8% → New position: 1.54

Frame 3: Current (1.54) ───────→  Target (10)
         Di chuyển 8% → New position: 2.21

... dần dần tiến gần target
```

#### Drag Input

```csharp
private bool isDragging = false;
private Vector3 dragStartPos;

void Update()
{
    // Mouse button down
    if (Input.GetMouseButtonDown(0))
    {
        isDragging = true;
        dragStartPos = Input.mousePosition;
    }

    // Mouse button held
    if (Input.GetMouseButton(0) && isDragging)
    {
        Vector3 delta = Input.mousePosition - dragStartPos;

        // Di chuyển camera ngược với drag (như scrolling)
        float moveAmount = -delta.x * dragSensitivity;
        Vector3 newPos = transform.position + new Vector3(moveAmount, 0, 0);

        // Áp dụng clamping
        newPos.x = Mathf.Clamp(newPos.x, limitLeft, limitRight);
        transform.position = newPos;

        dragStartPos = Input.mousePosition;
    }

    // Mouse button released
    if (Input.GetMouseButtonUp(0))
    {
        isDragging = false;
    }
}
```

**Drag behavior**:
```
Mouse drag LEFT (←):
    Camera di chuyển RIGHT (→)
    (Hiển thị content bên trái)

Mouse drag RIGHT (→):
    Camera di chuyển LEFT (←)
    (Hiển thị content bên phải)

Như scrolling một web page!
```

### Camera Helper Methods

#### Get Camera Width

```csharp
public float GetCameraHalfWidth()
{
    // Camera width trong world units
    float height = Camera.main.orthographicSize * 2f;
    float width = height * Camera.main.aspect;

    return width / 2f;
}
```

**Cách Dùng**:
```csharp
// Đảm bảo level rộng hơn camera view
float cameraWidth = GetCameraHalfWidth();
limitLeft = -levelWidth / 2 + cameraWidth;
limitRight = levelWidth / 2 - cameraWidth;
```

**Orthographic size giải thích**:
```
orthographicSize = 5  (height trong world units)

┌───────────────────┐
│                   │  ▲
│                   │  │ 5 units
│     Camera        │  │ (top đến center)
│     View          │  ▼
│                   │  ─── Center
│                   │  ▲
│                   │  │ 5 units
└───────────────────┘  ▼ (center đến bottom)

Total height = 10 units

Width = height * aspect ratio
      = 10 * (16/9)
      = 17.78 units (cho 16:9 screen)
```

### Ví Dụ Thực Tế

**Ví dụ 1: Set camera limits từ level**
```csharp
// Trong LevelManager
void Start()
{
    CameraController cam = Camera.main.GetComponent<CameraController>();

    // Set limits dựa trên level size
    cam.limitLeft = -levelWidth / 2f;
    cam.limitRight = levelWidth / 2f;

    Debug.Log("Camera limits set: " + cam.limitLeft + " to " + cam.limitRight);
}
```

**Ví dụ 2: Follow player mượt mà**
```csharp
// Enable trong CameraController
public bool followTarget = true;
public Transform target; // Assign player

void GetTargetPosition()
{
    if (followTarget && target != null)
    {
        return target.position;
    }

    return transform.position; // Ở lại nơi chúng ta đang ở
}
```

**Ví dụ 3: Camera shake effect**
```csharp
public void Shake(float duration, float magnitude)
{
    StartCoroutine(ShakeCo(duration, magnitude));
}

IEnumerator ShakeCo(float duration, float magnitude)
{
    Vector3 originalPos = transform.localPosition;
    float elapsed = 0f;

    while (elapsed < duration)
    {
        float x = Random.Range(-1f, 1f) * magnitude;
        float y = Random.Range(-1f, 1f) * magnitude;

        transform.localPosition = originalPos + new Vector3(x, y, 0);

        elapsed += Time.deltaTime;
        yield return null;
    }

    transform.localPosition = originalPos; // Reset
}
```

---

## Cách Chúng Hoạt Động Cùng Nhau

### Sơ Đồ Tương Tác Hệ Thống

```
                    USER INPUT
                        │
                        ▼
              ┌─────────────────┐
              │  MENU MANAGER   │
              │  Hiển thị UI    │
              └────────┬────────┘
                       │ Calls
                       ▼
              ┌─────────────────┐
              │  GAME MANAGER   │ ◄─── Master Controller
              │  Thay đổi State │
              └────────┬────────┘
                       │ Broadcasts
           ┌───────────┼───────────┐
           │           │           │
           ▼           ▼           ▼
    ┌──────────┐ ┌──────────┐ ┌──────────┐
    │  ENEMY   │ │   UI     │ │  SPAWNER │
    │Listeners │ │Listeners │ │Listeners │
    └──────────┘ └──────────┘ └──────────┘

    Tất cả phát appropriate sounds qua:
              │
              ▼
    ┌──────────────────┐
    │  SOUND MANAGER   │
    │  Phát Audio      │
    └──────────────────┘

    Trong khi player visible trong:
              │
              ▼
    ┌──────────────────┐
    │ CAMERA CONTROLLER│
    │  Hiển thị Action │
    └──────────────────┘
```

### Typical Game Flow

**1. Game Start Sequence**:
```
User clicks "Play" button
        │
        ▼
MenuManager.ShowGameUI()
        │
        ├─→ Ẩn StartUI
        ├─→ Hiển thị UI (HUD)
        └─→ GameManager.StartGame()
                │
                ├─→ State = Playing
                ├─→ Time.timeScale = 1
                └─→ Broadcasts IPlay() đến tất cả listeners
                        │
                        ├─→ Enemy.IPlay() → Khởi động AI
                        ├─→ Spawner.IPlay() → Bắt đầu waves
                        ├─→ UI.IPlay() → Cập nhật HUD
                        └─→ SoundManager.PlayMusic(gameMusic)
```

**2. Victory Sequence**:
```
Tất cả enemies defeated
        │
        ▼
LevelEnemyManager phát hiện victory
        │
        ▼
GameManager.Victory()
        │
        ├─→ State = Success
        ├─→ SoundManager.PlaySfx(victorySound)
        └─→ Broadcasts ISuccess() đến tất cả listeners
                │
                ├─→ MenuManager.ISuccess()
                │   └─→ Hiển thị VictoryUI
                │       ├─→ Tính toán stars
                │       ├─→ Thưởng coins
                │       └─→ Unlocks next level
                │
                ├─→ Enemy.ISuccess()
                │   └─→ Ngừng tấn công
                │
                └─→ UI.ISuccess()
                    └─→ Hiển thị final stats
```

**3. Pause Sequence**:
```
User clicks pause button
        │
        ▼
MenuManager.ShowPauseUI()
        │
        ├─→ Hiển thị PauseUI
        └─→ GameManager.Pause()
                │
                ├─→ State = Pause
                ├─→ Time.timeScale = 0  ← ĐÓNG BĂNG GAME
                ├─→ SoundManager.musicsGame.Pause()
                └─→ Broadcasts IPause() đến tất cả listeners
                        │
                        └─→ Tất cả Update() loops frozen
                            (không movement, không spawning, không AI)

User clicks resume
        │
        ▼
MenuManager.Resume()
        │
        ├─→ Ẩn PauseUI
        └─→ GameManager.Resume()
                │
                ├─→ State = Playing
                ├─→ Time.timeScale = 1  ← MỞ BĂNG
                ├─→ SoundManager.musicsGame.UnPause()
                └─→ Game tiếp tục từ exact state
```

### Communication Patterns

**Pattern 1: Direct Call**
```csharp
// MenuManager gọi trực tiếp GameManager
GameManager.Instance.StartGame();
```

**Pattern 2: Event Broadcasting**
```csharp
// GameManager broadcasts đến tất cả listeners
foreach (var listener in listeners)
{
    listener.ISuccess();
}
```

**Pattern 3: Singleton Access**
```csharp
// Bất kỳ script nào có thể truy cập managers
SoundManager.Instance.PlaySfx(sound);
CameraController cam = Camera.main.GetComponent<CameraController>();
```

---

## Observer Pattern Giải Thích

### Observer Pattern Là Gì?

**Observer Pattern** là một design pattern mà:
- **Subject** (GameManager) duy trì một list của **Observers** (listeners)
- Khi Subject's state thay đổi, nó **thông báo** tất cả Observers
- Observers **phản ứng** với notification

**Ví dụ thực tế**:
```
Newsletter (Subject):
    Có list subscribers (Observers)

Khi new article published:
    → Gửi email đến tất cả subscribers
    → Mỗi subscriber quyết định cách phản ứng

Trong game của chúng ta:
    GameManager = Newsletter
    Listeners = Subscribers
    State change = New article
    ISuccess()/IPlay() = Email notification
```

### IListener Interface

```csharp
public interface IListener
{
    void IPlay();      // Được gọi khi game starts
    void ISuccess();   // Được gọi khi level won
    void IGameOver();  // Được gọi khi level lost
    void IPause();     // Được gọi khi game paused
}
```

**Tại sao dùng interface?**
- Đảm bảo tất cả listeners có required methods
- GameManager có thể gọi methods mà không biết specific class
- Polymorphism: Các classes khác nhau phản ứng khác nhau với cùng event

### Implementation Example

**Bước 1: Implement Interface**
```csharp
public class Enemy : MonoBehaviour, IListener
{
    private bool isPlaying = false;

    void Start()
    {
        // Đăng ký là listener
        GameManager.Instance.AddListener(this);
    }

    // Implement interface methods
    public void IPlay()
    {
        isPlaying = true;
        Debug.Log("Enemy AI started");
    }

    public void ISuccess()
    {
        isPlaying = false;
        StopAttacking();
        Debug.Log("Enemy celebrating player victory");
    }

    public void IGameOver()
    {
        isPlaying = false;
        Celebrate();
        Debug.Log("Enemy celebrating player defeat");
    }

    public void IPause()
    {
        isPlaying = false;
        Debug.Log("Enemy AI paused");
    }

    void Update()
    {
        if (!isPlaying)
            return; // Không làm gì khi không chơi

        // AI behavior...
    }

    void OnDestroy()
    {
        // Hủy đăng ký
        GameManager.Instance?.RemoveListener(this);
    }
}
```

**Bước 2: GameManager Thông Báo**
```csharp
public void Victory()
{
    State = GameState.Success;

    // Thông báo TẤT CẢ listeners
    foreach (var listener in listeners)
    {
        if (listener != null)
            listener.ISuccess(); // ← Gọi Enemy.ISuccess(), UI.ISuccess(), v.v.
    }
}
```

### Lợi Ích của Observer Pattern

✅ **Loose Coupling**: GameManager không cần biết về specific classes
✅ **Flexibility**: Dễ thêm new listeners mà không thay đổi GameManager
✅ **Reusability**: Cùng pattern hoạt động cho UI, enemies, effects, v.v.
✅ **Maintainability**: Tách bạch rõ ràng

**Không có Observer Pattern** (tệ):
```csharp
// GameManager sẽ cần biết về mọi class:
public void Victory()
{
    enemy1.StopAttacking();
    enemy2.StopAttacking();
    ui.ShowVictoryScreen();
    spawner.StopSpawning();
    timer.Stop();
    // ... phải cập nhật cái này cho mọi object mới!
}
```

**Với Observer Pattern** (tốt):
```csharp
// GameManager chỉ broadcasts:
public void Victory()
{
    foreach (var listener in listeners)
    {
        listener.ISuccess(); // Mỗi object tự xử lý!
    }
}
```

---

## Ví Dụ Thực Tế

### Ví dụ 1: Tạo Custom Listener

```csharp
using UnityEngine;

public class ParticleController : MonoBehaviour, IListener
{
    public ParticleSystem victoryParticles;
    public ParticleSystem defeatParticles;

    void Start()
    {
        GameManager.Instance.AddListener(this);

        // Dừng particles ban đầu
        victoryParticles.Stop();
        defeatParticles.Stop();
    }

    public void IPlay()
    {
        // Dừng bất kỳ playing particles
        victoryParticles.Stop();
        defeatParticles.Stop();
    }

    public void ISuccess()
    {
        // Phát victory particles
        victoryParticles.Play();
        SoundManager.PlaySfx(SoundManager.Instance.soundVictory);
    }

    public void IGameOver()
    {
        // Phát defeat particles
        defeatParticles.Play();
        SoundManager.PlaySfx(SoundManager.Instance.soundFail);
    }

    public void IPause()
    {
        // Pause particles
        victoryParticles.Pause();
        defeatParticles.Pause();
    }

    void OnDestroy()
    {
        GameManager.Instance?.RemoveListener(this);
    }
}
```

### Ví dụ 2: Custom Menu Panel

```csharp
using UnityEngine;
using UnityEngine.UI;

public class CustomPanel : MonoBehaviour
{
    public GameObject panel;
    public Text messageText;

    public void ShowPanel(string message)
    {
        panel.SetActive(true);
        messageText.text = message;

        // Phát sound
        SoundManager.PlaySfx(SoundManager.Instance.soundClick);
    }

    public void HidePanel()
    {
        panel.SetActive(false);
    }

    // Gọi từ button
    public void OnCloseButtonClicked()
    {
        HidePanel();
        SoundManager.PlaySfx(SoundManager.Instance.soundClick);
    }
}
```

### Ví dụ 3: Music Transition

```csharp
using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public AudioClip menuMusic;
    public AudioClip gameMusic;
    public AudioClip bossMusic;

    void Start()
    {
        // Phát menu music ban đầu
        SoundManager.PlayMusic(menuMusic);
    }

    public void OnGameStart()
    {
        SoundManager.PlayMusic(gameMusic);
    }

    public void OnBossAppear()
    {
        StartCoroutine(CrossfadeMusic(bossMusic, 1f));
    }

    IEnumerator CrossfadeMusic(AudioClip newMusic, float duration)
    {
        // Fade out current
        float startVolume = SoundManager.Instance.musicVolume;

        for (float t = 0; t < duration / 2; t += Time.deltaTime)
        {
            SoundManager.Instance.musicsGame.volume = Mathf.Lerp(startVolume, 0, t / (duration / 2));
            yield return null;
        }

        // Switch music
        SoundManager.PlayMusic(newMusic);

        // Fade in new
        for (float t = 0; t < duration / 2; t += Time.deltaTime)
        {
            SoundManager.Instance.musicsGame.volume = Mathf.Lerp(0, startVolume, t / (duration / 2));
            yield return null;
        }

        SoundManager.Instance.musicsGame.volume = startVolume;
    }
}
```

---

## Troubleshooting

### Problem 1: Music Không Phát

**Triệu chứng**:
- Không có background music
- SoundManager tồn tại nhưng im lặng

**Chẩn đoán**:
```csharp
void Start()
{
    Debug.Log("SoundManager exists: " + (SoundManager.Instance != null));
    Debug.Log("Music volume: " + SoundManager.Instance.musicVolume);
    Debug.Log("Music source playing: " + SoundManager.Instance.musicsGame.isPlaying);
    Debug.Log("Music clip assigned: " + (SoundManager.Instance.musicsGame.clip != null));
}
```

**Nguyên Nhân Phổ Biến**:
1. ❌ Music volume set thành 0
   - **Sửa**: `SoundManager.Instance.musicVolume = 0.5f;`

2. ❌ Không có AudioListener trong scene
   - **Sửa**: Thêm AudioListener vào Main Camera

3. ❌ Music clip không được assigned
   - **Sửa**: Assign clip trong SoundManager Inspector

4. ❌ AudioSource không playing
   - **Sửa**: Gọi `SoundManager.PlayMusic(clip);`

### Problem 2: GameManager State Không Thay Đổi

**Triệu chứng**:
- Game stuck trong một state
- Listeners không nhận events

**Chẩn đoán**:
```csharp
void Update()
{
    if (Input.GetKeyDown(KeyCode.Space))
    {
        Debug.Log("Current state: " + GameManager.Instance.State);
        Debug.Log("Listener count: " + GameManager.Instance.listeners.Count);
    }
}
```

**Nguyên Nhân Phổ Biến**:
1. ❌ State change không được gọi
   - **Sửa**: Gọi `GameManager.Instance.StartGame()` khi sẵn sàng

2. ❌ Listeners không được đăng ký
   - **Sửa**: Đảm bảo `AddListener(this)` được gọi trong Start()

3. ❌ GameManager destroyed
   - **Sửa**: Thêm `DontDestroyOnLoad(gameObject)` trong Awake()

### Problem 3: UI Panel Không Hiển Thị

**Triệu chứng**:
- Panel tồn tại nhưng invisible
- SetActive(true) không hoạt động

**Chẩn đoán**:
```csharp
void ShowPanel()
{
    Debug.Log("Panel exists: " + (panel != null));
    Debug.Log("Panel active: " + panel.activeSelf);
    Debug.Log("Canvas exists: " + (GetComponentInParent<Canvas>() != null));

    panel.SetActive(true);
}
```

**Nguyên Nhân Phổ Biến**:
1. ❌ Panel reference null
   - **Sửa**: Assign panel trong Inspector

2. ❌ Parent Canvas disabled
   - **Sửa**: Enable parent Canvas trước

3. ❌ Panel scale set thành 0
   - **Sửa**: Kiểm tra RectTransform scale

4. ❌ Panel phía sau UI khác
   - **Sửa**: Tăng Canvas sort order

### Problem 4: Camera Không Di Chuyển

**Triệu chứng**:
- Camera stuck ở một position
- Drag input không hoạt động

**Chẩn đoán**:
```csharp
void Update()
{
    Debug.Log("Camera position: " + transform.position);
    Debug.Log("Limits: " + limitLeft + " to " + limitRight);
    Debug.Log("Mouse position: " + Input.mousePosition);
}
```

**Nguyên Nhân Phổ Biến**:
1. ❌ Limits quá restrictive
   - **Sửa**: Tăng range (limitLeft = -10, limitRight = 10)

2. ❌ smoothSpeed quá thấp
   - **Sửa**: Tăng lên 5-10

3. ❌ Input không được detected
   - **Sửa**: Kiểm tra EventSystem tồn tại

4. ❌ Camera position clamped
   - **Sửa**: Verify clamp logic

---

## Tóm Tắt

### Những Điểm Chính

**GameManager**:
- ✅ Điều khiển game state (Menu, Playing, Pause, Success, Fail)
- ✅ Broadcasts events đến tất cả listeners qua Observer pattern
- ✅ Quản lý level loading và player reference
- ✅ Singleton - truy cập qua `GameManager.Instance`

**MenuManager**:
- ✅ Hiển thị/ẩn UI panels
- ✅ Xử lý scene transitions với async loading
- ✅ Implements IListener để phản ứng với game events
- ✅ Điều khiển audio toggle buttons

**SoundManager**:
- ✅ Phát music (looping background)
- ✅ Phát SFX (one-shot sounds)
- ✅ Quản lý volume cho cả hai
- ✅ Singleton - gọi `SoundManager.PlaySfx(clip)`

**CameraController**:
- ✅ Clamps camera position đến level boundaries
- ✅ Smooth movement qua Lerp
- ✅ Xử lý drag input cho panning
- ✅ Optional target following

### Design Patterns Được Dùng

**Singleton**:
- Một instance accessible globally
- Được dùng bởi tất cả bốn core managers
- Pattern: `ClassName.Instance.Method()`

**Observer**:
- Subject (GameManager) thông báo Observers (listeners)
- Loose coupling giữa systems
- Pattern: `AddListener()`, broadcast events

**Component**:
- GameObject-Component architecture của Unity
- Mỗi manager là một component trên GameObject
- Pattern: `GetComponent<ManagerType>()`

### Integration Checklist

Khi thêm new features, nhớ:

- [ ] Đăng ký là listener nếu cần game events
  ```csharp
  GameManager.Instance.AddListener(this);
  ```

- [ ] Phát sounds cho feedback
  ```csharp
  SoundManager.PlaySfx(soundClip);
  ```

- [ ] Hiển thị/ẩn UI qua MenuManager
  ```csharp
  menuManager.ShowPanel(panelName);
  ```

- [ ] Kiểm tra game state trước khi hành động
  ```csharp
  if (GameManager.Instance.State == GameState.Playing)
  ```

- [ ] Tôn trọng camera boundaries
  ```csharp
  float x = Mathf.Clamp(pos.x, cam.limitLeft, cam.limitRight);
  ```

### Bước Tiếp Theo

**Cho Beginners**:
1. Đọc **00_Kien_Thuc_Co_Ban.md** - Unity basics
2. Hoàn thành **Nhiem_Vu_Dau_Tien.md** - Bài tập thực hành
3. Nghiên cứu **01_Kien_Truc_Project.md** - Project structure

**Cho Intermediate**:
1. Đọc **05_Managers_Complete.md** - Deep dive vào managers
2. Nghiên cứu **12_Tham_Chieu_Truc_Quan.md** - Visual diagrams
3. Thử **10_Huong_Dan_Thuc_Hanh.md** - Practical tutorials

**Cho Advanced**:
1. Đọc **13_Vi_Du_Code.md** - Reusable code
2. Nghiên cứu **project-analysis.md** - Technical analysis
3. Mở rộng managers với custom features

---

## Tài Liệu Liên Quan

### Core Systems
- **05_Managers_Complete.md** - Complete manager documentation
- **01_Kien_Truc_Project.md** - Overall architecture

### Practical Guides
- **Nhiem_Vu_Dau_Tien.md** - Bài tập thực hành
- **10_Huong_Dan_Thuc_Hanh.md** - Step-by-step tutorials
- **11_Xu_Ly_Su_Co.md** - Problem solving

### Reference
- **12_Tham_Chieu_Truc_Quan.md** - Visual diagrams
- **13_Vi_Du_Code.md** - Code snippets
- **99_Tu_Vung.md** - Term definitions

---

**Hiểu bốn core objects này là cần thiết để làm việc với "Lawn Defense: Monsters Out"!**

Chúng tạo thành xương sống điều phối tất cả các systems khác trong game.

---

<p align="center">
<strong>Lawn Defense: Monsters Out</strong><br>
Tài Liệu Các Đối Tượng Lõi<br>
Core Game Objects Documentation<br>
Version 2.0 • October 2025
</p>
