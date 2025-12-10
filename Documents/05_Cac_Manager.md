# Hệ Thống Managers - Hướng Dẫn Đầy Đủ

---
**🌐 Ngôn ngữ:** Tiếng Việt
**📄 File gốc:** [05_Managers_Complete.md](05_Managers_Complete.md)
**🔄 Cập nhật lần cuối:** 2025-01-30
---

> **Dành cho**: Người mới bắt đầu đã hoàn thành Unity Fundamentals
> **Thời gian đọc**: 35-45 phút
> **Yêu cầu**: 00_Unity_Co_Ban.md, 01_Kien_Truc_Project.md

---

## Mục Lục
1. [Tổng Quan Hệ Thống](#tổng-quan-hệ-thống)
2. [GameManager - Core Controller](#gamemanager---core-controller)
3. [Observer Pattern (IListener)](#observer-pattern-ilistener)
4. [LevelEnemyManager - Wave System](#levelenemymanager---wave-system)
5. [LevelManager - Quản Lý Tài Nguyên](#levelmanager---quản-lý-tài-nguyên)
6. [SoundManager - Hệ Thống Audio](#soundmanager---hệ-thống-audio)
7. [Luồng Giao Tiếp Giữa Các Manager](#luồng-giao-tiếp-giữa-các-manager)
8. [Cách Tạo Custom Manager](#cách-tạo-custom-manager)
9. [Các Vấn Đề Thường Gặp & Giải Pháp](#các-vấn-đề-thường-gặp--giải-pháp)

---

## Tổng Quan Hệ Thống

### Managers Là Gì?

**Managers** là các class **singleton** điều khiển các hệ thống game toàn cục. Chúng:
- **Phối hợp** giữa các hệ thống khác nhau (Player, Enemy, UI)
- **Duy trì** trạng thái game (Playing, Paused, Victory, GameOver)
- **Broadcast** (phát sóng) sự kiện đến các listener
- **Duy trì** qua các scene (một số manager)

### Tại Sao Cần Managers?

Không có manager, mọi script sẽ cần reference đến mọi script khác:

```
❌ KHÔNG CÓ MANAGERS:
Enemy → cần reference Player
Enemy → cần reference UI
Enemy → cần reference SoundManager
Enemy → cần reference LevelManager
(rối, tightly coupled)

✓ CÓ MANAGERS:
Enemy → gọi GameManager.Victory()
GameManager → broadcast đến tất cả listener
(sạch, loosely coupled)
```

### Sơ Đồ Kiến Trúc Manager

```
┌─────────────────────────────────────────────┐
│            GAME MANAGER                     │
│       (Central State Controller)            │
└────────────┬────────────────────────────────┘
             │
       Broadcast Events
             │
    ┌────────┼────────┬────────┬────────┐
    │        │        │        │        │
    ▼        ▼        ▼        ▼        ▼
┌────────┐ ┌──────┐ ┌──────┐ ┌─────┐ ┌──────┐
│ Level  │ │Menu  │ │Enemy │ │Sound│ │Level │
│ Enemy  │ │ Mgr  │ │  AI  │ │ Mgr │ │ Mgr  │
│Manager │ │      │ │      │ │     │ │      │
└────────┘ └──────┘ └──────┘ └─────┘ └──────┘
```

### Các File Chính

| File | Vị Trí | Mục Đích |
|------|--------|----------|
| `GameManager.cs` | `Assets/_MonstersOut/Scripts/Managers/` | Trạng thái game & sự kiện cốt lõi |
| `LevelEnemyManager.cs` | `Assets/_MonstersOut/Scripts/Managers/` | Spawn wave enemy |
| `LevelManager.cs` | `Assets/_MonstersOut/Scripts/Managers/` | Mana & tài nguyên |
| `SoundManager.cs` | `Assets/_MonstersOut/Scripts/Managers/` | Nhạc & hiệu ứng âm thanh |

---

## GameManager - Core Controller

### Tổng Quan GameManager

**File**: `GameManager.cs`

**Mục đích**: Controller trạng thái game trung tâm

**Trách nhiệm**:
- Quản lý trạng thái game (Menu, Playing, Pause, Success, GameOver)
- Broadcast sự kiện đến tất cả listener
- Theo dõi enemy còn sống
- Load level prefab
- Xử lý logic Victory/GameOver

### Singleton Pattern

Tại `GameManager.cs:14` và `:44-48`

```csharp
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        Application.targetFrameRate = 60;  // Khóa ở 60 FPS
        Instance = this;                    // Đặt singleton

        State = GameState.Menu;             // Trạng thái ban đầu
        listeners = new List<IListener>();  // Khởi tạo list listener

        // Load level prefab dựa trên tiến trình
        if (GameMode.Instance == null)
            Instantiate(gameLevels[1], Vector2.zero, Quaternion.identity);
        else
            Instantiate(gameLevels[GlobalValue.levelPlaying - 1], Vector2.zero, Quaternion.identity);
    }
}
```

**Tại Sao Dùng Singleton?**

```csharp
// Bất kỳ script nào cũng có thể truy cập GameManager
GameManager.Instance.Victory();
GameManager.Instance.State;
GameManager.Instance.AddListener(this);

// Không cần FindObjectOfType
// Không cần truyền reference
```

### Game States (Enum)

Tại `GameManager.cs:18`

```csharp
public enum GameState
{
    Menu,      // Màn hình tiêu đề, chưa chơi
    Playing,   // Game đang chạy
    GameOver,  // Player thua (pháo đài bị phá hủy)
    Success,   // Player thắng (tất cả enemy chết)
    Pause      // Game bị tạm dừng bởi player
}

public GameState State { get; set; }
```

### Sơ Đồ Luồng Trạng Thái

```
[MENU]
   │
   │ MenuManager gọi StartGame()
   │
   ▼
[PLAYING]
   │
   ├──────────┬──────────┐
   │          │          │
   │          │          ▼
   │          │      [PAUSE]
   │          │          │
   │          │ UnPause()│
   │          │          │
   │          │          ▼
   │          │      [PLAYING]
   │          │
   │    Pháo đài   Tất cả enemy
   │      HP=0        chết
   │          │          │
   ▼          ▼          ▼
[GAMEOVER] [SUCCESS]
```

### StartGame() Method

Tại `GameManager.cs:68-83`

```csharp
// Được gọi bởi MenuManager
public void StartGame()
{
    // Đổi trạng thái sang Playing
    State = GameState.Playing;

    // Tìm tất cả object có interface IListener
    var listener_ = FindObjectsOfType<MonoBehaviour>().OfType<IListener>();

    // Thêm tất cả listener vào list
    foreach (var _listener in listener_)
    {
        listeners.Add(_listener);
    }

    // Broadcast IPlay() đến tất cả listener
    foreach (var item in listeners)
    {
        item.IPlay();
    }
}
```

**Cách Hoạt Động**:

```
Frame 1: MenuManager.Start() kết thúc đếm ngược
         └─ GameManager.Instance.StartGame() được gọi

Frame 2: GameManager.StartGame() thực thi
         ├─ State = Playing
         ├─ Tìm tất cả IListener trong scene:
         │   ├─ MenuManager (implement IListener)
         │   ├─ LevelEnemyManager (implement IListener)
         │   ├─ Tất cả Enemy GameObject (implement IListener)
         │   └─ Player_Archer (implement IListener)
         │
         └─ Gọi IPlay() trên mỗi listener:
             ├─ MenuManager.IPlay() → (trống)
             ├─ LevelEnemyManager.IPlay() → StartCoroutine(SpawnEnemyCo())
             ├─ Enemy.IPlay() → isPlaying = true
             └─ Player_Archer.IPlay() → (trống, kế thừa từ Enemy)

Frame 3: Game bắt đầu
         └─ Enemy bắt đầu spawn
```

**Giải Thích FindObjectsOfType()**:

```csharp
// Tìm TẤT CẢ MonoBehaviour
var allScripts = FindObjectsOfType<MonoBehaviour>();

// Lọc chỉ những cái implement IListener
var listeners = allScripts.OfType<IListener>();

// Tương đương với:
List<IListener> listeners = new List<IListener>();
foreach (MonoBehaviour script in FindObjectsOfType<MonoBehaviour>())
{
    if (script is IListener)
        listeners.Add(script as IListener);
}
```

### Victory() Method

Tại `GameManager.cs:101-126`

```csharp
public void Victory()
{
    // Ngăn kích hoạt nhiều lần
    if (State == GameState.Success)
        return;

    Time.timeScale = 1;  // Reset time scale (phòng khi bị pause)

    // Tạm dừng nhạc
    SoundManager.Instance.PauseMusic(true);

    // Phát âm thanh chiến thắng
    SoundManager.PlaySfx(SoundManager.Instance.soundVictory, 0.6f);

    // Đổi trạng thái sang Success
    State = GameState.Success;

    // Hiển thị quảng cáo (nếu có ads manager)
    if (AdsManager.Instance)
    {
        AdsManager.Instance.ShowAdmobBanner(true);
        AdsManager.Instance.ShowNormalAd(State);
    }

    // Broadcast ISuccess() đến tất cả listener
    foreach (var item in listeners)
    {
        if (item != null)
            item.ISuccess();
    }

    // Lưu tiến trình level (mở khóa level tiếp)
    if (GlobalValue.levelPlaying > GlobalValue.LevelPass)
        GlobalValue.LevelPass = GlobalValue.levelPlaying;
}
```

**Khi Nào Victory() Được Gọi?**

```csharp
// Trong LevelEnemyManager.SpawnEnemyCo()
while (isEnemyAlive()) { yield return new WaitForSeconds(0.1f); }
// Sau khi loop thoát (tất cả enemy chết)
GameManager.Instance.Victory();
```

**Luồng Victory**:

```
Frame 500:  Enemy cuối cùng chết
            └─ LevelEnemyManager kiểm tra isEnemyAlive()

Frame 501:  isEnemyAlive() trả về false
            └─ GameManager.Victory() được gọi

Frame 502:  Victory() thực thi
            ├─ State = Success
            ├─ Tạm dừng nhạc
            ├─ Phát âm thanh chiến thắng
            └─ Broadcast ISuccess():
                ├─ MenuManager.ISuccess() → Hiển thị victory UI
                ├─ LevelEnemyManager.ISuccess() → StopAllCoroutines
                └─ Tất cả enemy → (đã bị xóa khỏi listener, đã chết)

Frame 503:  Victory UI xuất hiện
            └─ Animation sao bắt đầu
```

### GameOver() Method

Tại `GameManager.cs:134-152`

```csharp
public void GameOver()
{
    Time.timeScale = 1;  // Reset time scale

    // Dừng nhạc
    SoundManager.Instance.PauseMusic(true);

    // Ngăn kích hoạt nhiều lần
    if (State == GameState.GameOver)
        return;

    // Đặt trạng thái GameOver
    State = GameState.GameOver;

    // Hiển thị quảng cáo
    if (AdsManager.Instance)
    {
        AdsManager.Instance.ShowAdmobBanner(true);
        AdsManager.Instance.ShowNormalAd(State);
    }

    // Broadcast IGameOver() đến tất cả listener
    foreach (var item in listeners)
        item.IGameOver();
}
```

**Khi Nào GameOver() Được Gọi?**

```csharp
// Trong TheFortrest.cs (hệ thống máu pháo đài)
public override void TakeDamage(...)
{
    currentHealth -= (int)damage;

    if (currentHealth <= 0)
    {
        GameManager.Instance.GameOver();
        Die();
    }
}
```

### Pause/Resume Method

Tại `GameManager.cs:85-99`

```csharp
public void Gamepause()
{
    // Đặt trạng thái Pause
    State = GameState.Pause;

    // Broadcast IPause() đến tất cả listener
    foreach (var item in listeners)
        item.IPause();
}

public void UnPause()
{
    // Đặt lại trạng thái Playing
    State = GameState.Playing;

    // Broadcast IUnPause() đến tất cả listener
    foreach (var item in listeners)
        item.IUnPause();
}
```

**Cách Pause Hoạt Động**:

```
User click button Pause
    │
    ▼
MenuManager.Pause() được gọi
    ├─ Time.timeScale = 0 (đóng băng physics)
    ├─ GameManager.Instance.Gamepause()
    │   └─ Broadcast IPause() đến listener
    └─ Hiển thị pause UI

Trong Pause:
    - Update() vẫn chạy
    - FixedUpdate() KHÔNG chạy
    - Animation dừng
    - Di chuyển enemy dừng
    - Player không thể bắn

User click button Resume
    │
    ▼
MenuManager.Pause() được gọi lại
    ├─ Time.timeScale = 1 (tiếp tục physics)
    ├─ GameManager.Instance.UnPause()
    │   └─ Broadcast IUnPause() đến listener
    └─ Ẩn pause UI
```

### Theo Dõi Enemy

Tại `GameManager.cs:154-175`

```csharp
[HideInInspector]
public List<GameObject> enemyAlives;
[HideInInspector]
public List<GameObject> listEnemyChasingPlayer;

public void RigisterEnemy(GameObject obj)
{
    // Thêm enemy vào list còn sống
    enemyAlives.Add(obj);
}

public void RemoveEnemy(GameObject obj)
{
    // Xóa enemy khỏi list còn sống
    enemyAlives.Remove(obj);
}

public int EnemyAlive()
{
    // Trả về số lượng enemy còn sống
    return enemyAlives.Count;
}
```

**Lưu ý**: Code này tồn tại nhưng **KHÔNG thực sự được dùng** trong project hiện tại. Thay vào đó, `LevelEnemyManager` theo dõi enemy qua `listEnemySpawned`.

---

## Observer Pattern (IListener)

### Observer Pattern Là Gì?

**Observer Pattern** cho phép các object **subscribe** (đăng ký) sự kiện và được **thông báo** khi sự kiện xảy ra.

**Vấn Đề Không Có Observer**:
```csharp
// ❌ Tightly coupled
public class GameManager
{
    public MenuManager menuManager;
    public Enemy[] enemies;

    void Victory()
    {
        menuManager.ShowVictoryScreen();
        foreach (var enemy in enemies)
            enemy.Stop();
    }
}
```

**Giải Pháp Với Observer**:
```csharp
// ✓ Loosely coupled
public class GameManager
{
    List<IListener> listeners;

    void Victory()
    {
        foreach (var listener in listeners)
            listener.ISuccess();
    }
}
```

### IListener Interface

```csharp
public interface IListener
{
    void IPlay();         // Game bắt đầu
    void ISuccess();      // Thắng level
    void IGameOver();     // Thua level
    void IPause();        // Game tạm dừng
    void IUnPause();      // Game tiếp tục
    void IOnRespawn();    // Player hồi sinh (không dùng)
    void IOnStopMovingOn();  // Khóa di chuyển (không dùng)
    void IOnStopMovingOff(); // Mở khóa di chuyển (không dùng)
}
```

### AddListener() / RemoveListener()

Tại `GameManager.cs:27-39`

```csharp
public void AddListener(IListener _listener)
{
    // Kiểm tra nếu chưa được thêm
    if (!listeners.Contains(_listener))
        listeners.Add(_listener);
}

public void RemoveListener(IListener _listener)
{
    // Kiểm tra nếu tồn tại, sau đó xóa
    if (listeners.Contains(_listener))
        listeners.Remove(_listener);
}
```

**Khi Nào Add/Remove**:

```csharp
// Trong Enemy.cs
protected virtual void OnEnable()
{
    if (GameManager.Instance)
        GameManager.Instance.AddListener(this);
    isPlaying = true;
}

public virtual void Die()
{
    isPlaying = false;
    GameManager.Instance.RemoveListener(this);  // Dừng nhận sự kiện
    // ... logic chết
}
```

**Tại Sao Remove Khi Chết?**
- Enemy đã chết không nên nhận sự kiện
- Ngăn lỗi null reference
- Cải thiện performance (ít listener hơn để duyệt qua)

### Ví Dụ Implement IListener

**Enemy.cs** (implement IListener):

```csharp
public class Enemy : MonoBehaviour, ICanTakeDamage, IListener
{
    #region IListener implementation

    public virtual void IPlay()
    {
        // Game bắt đầu - không làm gì (đã xử lý trong Start)
    }

    public virtual void ISuccess()
    {
        // Thắng level - không làm gì
    }

    public virtual void IPause()
    {
        // Game tạm dừng - không làm gì
    }

    public virtual void IUnPause()
    {
        // Game tiếp tục - không làm gì
    }

    public virtual void IGameOver()
    {
        if (!isPlaying)
            return;

        isPlaying = false;           // Dừng hành động enemy
        SetEnemyState(ENEMYSTATE.IDLE);  // Đóng băng tại chỗ
    }

    // ... method IListener khác
    #endregion
}
```

**MenuManager.cs** (implement IListener):

```csharp
public class MenuManager : MonoBehaviour, IListener
{
    public void ISuccess()
    {
        StartCoroutine(VictoryCo());  // Hiển thị victory UI
    }

    IEnumerator VictoryCo()
    {
        UI.SetActive(false);
        CharacterContainer.SetActive(false);
        yield return new WaitForSeconds(1.5f);
        VictotyUI.SetActive(true);
    }

    public void IGameOver()
    {
        StartCoroutine(GameOverCo());  // Hiển thị game over UI
    }

    // ... method IListener khác
}
```

### Luồng Broadcast Sự Kiện

```
GameManager.Victory() được gọi
    │
    ├─ foreach (var item in listeners)
    │   │
    │   ├─ MenuManager.ISuccess()
    │   │   └─ Hiển thị victory UI
    │   │
    │   ├─ LevelEnemyManager.ISuccess()
    │   │   └─ StopAllCoroutines()
    │   │
    │   └─ Enemy instance.ISuccess()
    │       └─ (trống, không làm gì)
    │
    └─ Tất cả listener được thông báo đồng thời
```

---

## LevelEnemyManager - Wave System

### Tổng Quan LevelEnemyManager

**File**: `LevelEnemyManager.cs`

**Mục đích**: Spawn enemy theo wave

**Trách nhiệm**:
- Load cấu hình enemy wave
- Spawn enemy theo thời gian
- Theo dõi enemy đã spawn
- Phát hiện khi tất cả enemy chết → kích hoạt Victory

### Cấu Trúc Dữ Liệu Enemy Wave

Tại `LevelEnemyManager.cs:137-144`

```csharp
[System.Serializable]
public class EnemyWave
{
    public float wait = 3;  // Delay trước khi wave này bắt đầu
    public EnemySpawn[] enemySpawns;  // Danh sách nhóm enemy
}

// Lưu ý: EnemySpawn được định nghĩa ở nơi khác
public class EnemySpawn
{
    public GameObject enemy;     // Prefab enemy
    public int numberEnemy;      // Số lượng spawn
    public float wait;           // Delay trước nhóm này
    public float rate;           // Delay giữa mỗi enemy
}
```

**Ví Dụ Cấu Hình Wave**:

```
Wave 1:
  wait: 2 giây
  EnemySpawns:
    - Goblin x5 (rate: 0.5s)
    - Skeleton x3 (rate: 1s)

Wave 2:
  wait: 10 giây
  EnemySpawns:
    - Troll x2 (rate: 2s)
    - Bomber x4 (rate: 1.5s)
```

### Start() Method (Đếm Tổng Enemy)

Tại `LevelEnemyManager.cs:25-47`

```csharp
void Start()
{
    // Load cấu hình wave từ level prefab
    if (GameLevelSetup.Instance)
        EnemyWaves = GameLevelSetup.Instance.GetLevelWave();

    // Đếm tổng enemy trong level
    totalEnemy = 0;

    // Duyệt qua mỗi wave
    for (int i = 0; i < EnemyWaves.Length; i++)
    {
        // Duyệt qua mỗi nhóm enemy spawn
        for (int j = 0; j < EnemyWaves[i].enemySpawns.Length; j++)
        {
            var enemySpawn = EnemyWaves[i].enemySpawns[j];

            // Đếm từng enemy riêng lẻ
            for (int k = 0; k < enemySpawn.numberEnemy; k++)
            {
                totalEnemy++;
            }
        }
    }

    currentSpawn = 0;
}
```

**Ví Dụ Tính Toán**:

```
Wave 1:
  Goblin x5 = 5 enemy
  Skeleton x3 = 3 enemy

Wave 2:
  Troll x2 = 2 enemy
  Bomber x4 = 4 enemy

totalEnemy = 5 + 3 + 2 + 4 = 14
```

### SpawnEnemyCo() Coroutine

Tại `LevelEnemyManager.cs:49-87`

```csharp
IEnumerator SpawnEnemyCo()
{
    // Duyệt qua mỗi wave
    for (int i = 0; i < EnemyWaves.Length; i++)
    {
        // Đợi trước khi wave bắt đầu
        yield return new WaitForSeconds(EnemyWaves[i].wait);

        // Duyệt qua mỗi nhóm enemy spawn trong wave
        for (int j = 0; j < EnemyWaves[i].enemySpawns.Length; j++)
        {
            var enemySpawn = EnemyWaves[i].enemySpawns[j];

            // Đợi trước khi nhóm bắt đầu
            yield return new WaitForSeconds(enemySpawn.wait);

            // Spawn từng enemy trong nhóm
            for (int k = 0; k < enemySpawn.numberEnemy; k++)
            {
                // Vị trí Y ngẫu nhiên trong spawn zone
                spawnPosition = transform.position
                                + Vector3.up * Random.Range(-spawnHeightZone, spawnHeightZone);

                // Instantiate enemy
                GameObject _temp = Instantiate(
                    enemySpawn.enemy,
                    spawnPosition + Vector2.up * 0.1f,
                    Quaternion.identity
                ) as GameObject;

                _temp.SetActive(false);           // Tắt trước
                _temp.transform.parent = transform;  // Đặt parent

                // Đợi 0.1s trước khi activate
                yield return new WaitForSeconds(0.1f);

                _temp.SetActive(true);  // Activate enemy

                // Thêm vào list đã spawn
                listEnemySpawned.Add(_temp);

                // Tăng bộ đếm spawn
                currentSpawn++;

                // Cập nhật thanh tiến trình wave
                MenuManager.Instance.UpdateEnemyWavePercent(currentSpawn, totalEnemy);

                // Đợi trước enemy tiếp theo
                yield return new WaitForSeconds(enemySpawn.rate);
            }
        }
    }

    // Tất cả enemy đã spawn, đợi đến khi tất cả chết
    while (isEnemyAlive()) { yield return new WaitForSeconds(0.1f); }
}
```

**Ví Dụ Timeline**:

```
Frame 1:    IPlay() được gọi
            └─ StartCoroutine(SpawnEnemyCo())

Frame 2:    Đợi Wave 1 delay (2 giây)
Frame 120:  Wave 1 bắt đầu

Frame 120:  Đợi Goblin group delay (0 giây)
            └─ Spawn Goblin #1

Frame 150:  Đợi rate (0.5 giây)
            └─ Spawn Goblin #2

Frame 180:  Đợi rate (0.5 giây)
            └─ Spawn Goblin #3

... (tiếp tục spawn)

Frame 500:  Tất cả enemy đã spawn
            └─ while (isEnemyAlive()) loop bắt đầu

Frame 1000: Enemy cuối cùng chết
            └─ isEnemyAlive() trả về false

Frame 1001: SpawnEnemyCo() thoát
            └─ (Không có Victory call ở đây - xử lý ở nơi khác)
```

**Tại Sao SetActive(false) Rồi SetActive(true)?**

```csharp
GameObject _temp = Instantiate(...);
_temp.SetActive(false);   // Tắt
yield return new WaitForSeconds(0.1f);
_temp.SetActive(true);    // Bật

// Lý do:
// 1. OnEnable() gọi AddListener() - chúng ta muốn delay nhỏ
// 2. Ngăn enemy hành động trước khi được đặt vị trí hoàn toàn
// 3. Cho thời gian cho tất cả component khởi tạo
```

### isEnemyAlive() Method

Tại `LevelEnemyManager.cs:90-100`

```csharp
bool isEnemyAlive()
{
    // Kiểm tra tất cả enemy đã spawn
    for (int i = 0; i < listEnemySpawned.Count; i++)
    {
        // Nếu bất kỳ enemy nào active, trả về true
        if (listEnemySpawned[i].activeInHierarchy)
            return true;
    }

    // Không có enemy active, trả về false
    return false;
}
```

**Cách Hoạt Động**:

```
listEnemySpawned = [Goblin1, Goblin2, Goblin3]

Goblin1.activeInHierarchy = true
Goblin2.activeInHierarchy = true
Goblin3.activeInHierarchy = true
isEnemyAlive() = true ✓

Goblin1 chết → SetActive(false)
Goblin1.activeInHierarchy = false
Goblin2.activeInHierarchy = true
Goblin3.activeInHierarchy = true
isEnemyAlive() = true ✓

Goblin2 chết → SetActive(false)
Goblin3 chết → SetActive(false)
Tất cả .activeInHierarchy = false
isEnemyAlive() = false → Victory!
```

### IPlay() Method

Tại `LevelEnemyManager.cs:122-125`

```csharp
public void IPlay()
{
    StartCoroutine(SpawnEnemyCo());
}
```

**Cách Được Gọi**:

```
GameManager.StartGame()
    └─ foreach (var item in listeners)
        item.IPlay()
            └─ LevelEnemyManager.IPlay()
                └─ StartCoroutine(SpawnEnemyCo())
```

### ISuccess() Method

Tại `LevelEnemyManager.cs:127-130`

```csharp
public void ISuccess()
{
    StopAllCoroutines();  // Dừng spawn enemy
}
```

**Tại Sao Stop Coroutine?**
- Player thắng, không cần spawn thêm enemy
- Ngăn enemy spawn trong victory screen
- Dọn dẹp

---

## LevelManager - Quản Lý Tài Nguyên

### Tổng Quan LevelManager

**File**: `LevelManager.cs`

**Mục đích**: Quản lý tài nguyên level (chủ yếu là mana)

**Trách nhiệm**:
- Khởi tạo mana từ cấu hình level
- Cung cấp truy cập toàn cục đến giá trị mana

### Awake() Method

Tại `LevelManager.cs:10-19`

```csharp
private void Awake()
{
    Instance = this;  // Singleton

    // Load mana từ cấu hình level
    if (GameLevelSetup.Instance)
    {
        mana = GameLevelSetup.Instance.GetGivenMana();
    }
}
```

**Cách Mana Được Sử Dụng**:

```csharp
// Trong UI_UI.Update()
manaTxt.text = LevelManager.Instance.mana + "";

// Sử dụng giả định (không có trong code hiện tại):
public void SpawnUnit(int manaCost)
{
    if (LevelManager.Instance.mana >= manaCost)
    {
        LevelManager.Instance.mana -= manaCost;
        // Spawn unit
    }
}
```

**Lưu ý**: Implementation hiện tại rất đơn giản. Hệ thống mana có vẻ chưa hoàn thiện - không có code nào thực sự **sử dụng** hoặc **giảm** mana.

---

## SoundManager - Hệ Thống Audio

### Tổng Quan SoundManager

**File**: `SoundManager.cs`

**Mục đích**: Quản lý audio tập trung

**Trách nhiệm**:
- Phát nhạc (background loop)
- Phát hiệu ứng âm thanh (one-shot)
- Điều khiển volume
- Tắt/bật tiếng

### Singleton + Audio Source

Tại `SoundManager.cs:64-72`

```csharp
void Awake()
{
    Instance = this;

    // Tạo AudioSource cho nhạc
    musicAudio = gameObject.AddComponent<AudioSource>();
    musicAudio.loop = true;      // Nhạc loop mãi mãi
    musicAudio.volume = 0.5f;    // Volume mặc định 50%

    // Tạo AudioSource cho hiệu ứng âm thanh
    soundFx = gameObject.AddComponent<AudioSource>();
}
```

**Tại Sao Hai AudioSource?**

```
musicAudio:
- Phát một clip tại một thời điểm
- Loop liên tục
- Có thể pause/resume
- Volume điều khiển riêng

soundFx:
- Phát nhiều clip đồng thời (PlayOneShot)
- KHÔNG loop
- Dùng cho hiệu ứng ngắn
- Volume điều khiển riêng
```

### PlaySfx() Method

Tại `SoundManager.cs:88-112`

```csharp
// Phát một hiệu ứng âm thanh
public static void PlaySfx(AudioClip clip)
{
    if (Instance != null)
    {
        Instance.PlaySound(clip, Instance.soundFx);
    }
}

// Phát âm thanh với volume tùy chỉnh
public static void PlaySfx(AudioClip clip, float volume)
{
    if (Instance != null)
        Instance.PlaySound(clip, Instance.soundFx, volume);
}

// Phát âm thanh ngẫu nhiên từ mảng
public static void PlaySfx(AudioClip[] clips)
{
    if (Instance != null && clips.Length > 0)
        Instance.PlaySound(clips[Random.Range(0, clips.Length)], Instance.soundFx);
}

// Phát âm thanh ngẫu nhiên với volume tùy chỉnh
public static void PlaySfx(AudioClip[] clips, float volume)
{
    if (Instance != null && clips.Length > 0)
        Instance.PlaySound(clips[Random.Range(0, clips.Length)], Instance.soundFx, volume);
}
```

**Ví Dụ Sử Dụng**:

```csharp
// Âm thanh đơn
SoundManager.PlaySfx(soundVictory);

// Âm thanh với volume
SoundManager.PlaySfx(soundClick, 0.8f);

// Âm thanh ngẫu nhiên từ mảng
AudioClip[] hurtSounds = {hurt1, hurt2, hurt3};
SoundManager.PlaySfx(hurtSounds);  // Phát âm thanh hurt ngẫu nhiên
```

### PlayMusic() Method

Tại `SoundManager.cs:114-122`

```csharp
public static void PlayMusic(AudioClip clip)
{
    Instance.PlaySound(clip, Instance.musicAudio);
}

public static void PlayMusic(AudioClip clip, float volume)
{
    Instance.PlaySound(clip, Instance.musicAudio, volume);
}
```

**Sử dụng**:

```csharp
// Trong MainMenuHomeScene.Start()
SoundManager.PlayMusic(SoundManager.Instance.musicsGame);
```

### PlaySound() Private Method

Tại `SoundManager.cs:124-161`

```csharp
private void PlaySound(AudioClip clip, AudioSource audioOut)
{
    if (clip == null)
        return;

    if (Instance == null)
        return;

    // Nếu music audio
    if (audioOut == musicAudio)
    {
        audioOut.clip = clip;  // Đặt clip
        audioOut.Play();       // Phát từ đầu
    }
    else  // Hiệu ứng âm thanh
        audioOut.PlayOneShot(clip, SoundVolume);
}

private void PlaySound(AudioClip clip, AudioSource audioOut, float volume)
{
    if (clip == null)
        return;

    // Nếu music audio
    if (audioOut == musicAudio)
    {
        // Kiểm tra cài đặt GlobalValue
        audioOut.volume = GlobalValue.isMusic ? volume : 0;
        audioOut.clip = clip;
        audioOut.Play();
    }
    else  // Hiệu ứng âm thanh
    {
        // Kiểm tra cài đặt GlobalValue
        if (!GlobalValue.isSound) return;
        audioOut.PlayOneShot(clip, SoundVolume * volume);
    }
}
```

**Play() vs PlayOneShot()**:

```
Play():
- Dừng clip hiện tại và phát cái mới
- Chỉ một clip tại một thời điểm
- Dùng cho nhạc

PlayOneShot(clip):
- Phát clip mà không dừng clip khác
- Nhiều clip có thể phát đồng thời
- Dùng cho hiệu ứng âm thanh

Ví dụ:
musicAudio.Play(music1);
musicAudio.Play(music2);  // Dừng music1, phát music2

soundFx.PlayOneShot(gunshot);
soundFx.PlayOneShot(explosion);  // Cả hai phát cùng lúc
```

### Volume Property

Tại `SoundManager.cs:51-62`

```csharp
public static float MusicVolume
{
    set { Instance.musicAudio.volume = value; }
    get { return Instance.musicAudio.volume; }
}

public static float SoundVolume
{
    set { Instance.soundFx.volume = value; }
    get { return Instance.soundFx.volume; }
}
```

**Sử dụng**:

```csharp
// Đặt volume nhạc
SoundManager.MusicVolume = 0.7f;  // 70%

// Tắt hiệu ứng âm thanh
SoundManager.SoundVolume = 0;

// Lấy volume hiện tại
float currentVolume = SoundManager.MusicVolume;
```

### PauseMusic() Method

Tại `SoundManager.cs:43-49`

```csharp
public void PauseMusic(bool isPause)
{
    if (isPause)
        Instance.musicAudio.mute = true;   // Tắt tiếng
    else
        Instance.musicAudio.mute = false;  // Bật tiếng
}
```

**Sử dụng**:

```csharp
// Trong GameManager.Victory()
SoundManager.Instance.PauseMusic(true);  // Tắt tiếng nhạc
SoundManager.PlaySfx(soundVictory);      // Phát âm thanh chiến thắng

// Sau đó
SoundManager.Instance.PauseMusic(false);  // Bật tiếng nhạc
```

**Lưu ý**: `mute` không thực sự pause - nó chỉ làm im lặng. Nhạc vẫn tiếp tục phát.

### Click() Helper

Tại `SoundManager.cs:78-81`

```csharp
public static void Click()
{
    PlaySfx(Instance.soundClick);
}
```

**Sử dụng**:

```csharp
// Trong button khắp UI
public void OnButtonClick()
{
    SoundManager.Click();  // Shorthand cho PlaySfx(soundClick)
    // Thực hiện hành động button
}
```

---

## Luồng Giao Tiếp Giữa Các Manager

### Sơ Đồ Luồng Game Hoàn Chỉnh

```
GAME START
    │
    ▼
GameManager.Awake()
    ├─ Đặt targetFrameRate = 60
    ├─ Tạo Instance
    ├─ State = Menu
    └─ Instantiate level prefab
        └─ LevelEnemyManager spawn
        └─ LevelManager spawn
        └─ SoundManager spawn

MenuManager.Start()
    ├─ Hiển thị countdown UI
    ├─ Đợi 1 giây
    └─ Gọi GameManager.StartGame()

GameManager.StartGame()
    ├─ State = Playing
    ├─ Tìm tất cả IListener
    └─ Broadcast IPlay()
        ├─ LevelEnemyManager.IPlay()
        │   └─ Bắt đầu spawn enemy
        └─ Enemy.IPlay()
            └─ isPlaying = true

LevelEnemyManager.SpawnEnemyCo()
    ├─ Spawn wave 1
    ├─ Spawn wave 2
    ├─ ...
    └─ Đợi đến khi tất cả chết
        └─ (Victory phát hiện ở nơi khác)

Enemy nhận damage
    ├─ currentHealth -= damage
    └─ Nếu health <= 0:
        └─ Enemy.Die()
            ├─ SetActive(false)
            └─ RemoveListener()

LevelEnemyManager kiểm tra
    └─ isEnemyAlive() trả về false
        └─ GameManager.Victory() được gọi

GameManager.Victory()
    ├─ State = Success
    ├─ Dừng nhạc
    ├─ Phát âm thanh chiến thắng
    └─ Broadcast ISuccess()
        ├─ MenuManager.ISuccess()
        │   └─ Hiển thị victory UI
        └─ LevelEnemyManager.ISuccess()
            └─ StopAllCoroutines()

Menu_Victory.Start()
    ├─ Kiểm tra % máu pháo đài
    ├─ Trao sao (1-3)
    └─ Hiển thị button (Menu, Restart, Next)

GAME END
```

---

## Cách Tạo Custom Manager

### Từng Bước: Tạo ItemManager

#### Bước 1: Tạo Script

Tạo `ItemManager.cs`:

```csharp
using UnityEngine;
using System.Collections.Generic;

namespace RGame
{
    public class ItemManager : MonoBehaviour
    {
        // Singleton
        public static ItemManager Instance { get; private set; }

        // Dữ liệu item
        public List<GameObject> activeItems = new List<GameObject>();
        public GameObject[] itemPrefabs;

        // Cài đặt
        public float spawnInterval = 5f;
        public Transform spawnArea;

        void Awake()
        {
            // Đặt singleton
            Instance = this;
        }

        void Start()
        {
            // Bắt đầu spawn item
            InvokeRepeating("SpawnRandomItem", spawnInterval, spawnInterval);
        }

        void SpawnRandomItem()
        {
            // Kiểm tra nếu game đang chơi
            if (GameManager.Instance.State != GameManager.GameState.Playing)
                return;

            // Vị trí ngẫu nhiên
            Vector2 spawnPos = new Vector2(
                Random.Range(spawnArea.position.x - 5, spawnArea.position.x + 5),
                Random.Range(spawnArea.position.y - 2, spawnArea.position.y + 2)
            );

            // Item ngẫu nhiên
            GameObject itemPrefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];

            // Spawn
            GameObject item = Instantiate(itemPrefab, spawnPos, Quaternion.identity);
            activeItems.Add(item);
        }

        public void RemoveItem(GameObject item)
        {
            activeItems.Remove(item);
            Destroy(item);
        }

        public int GetActiveItemCount()
        {
            return activeItems.Count;
        }
    }
}
```

#### Bước 2: Tạo GameObject

1. Tạo GameObject rỗng: `GameObject → Create Empty`
2. Đặt tên: `ItemManager`
3. Thêm script: `Add Component → ItemManager`
4. Gán prefab trong Inspector

#### Bước 3: Sử Dụng Trong Script Khác

```csharp
// Trong Player.cs
void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("Item"))
    {
        // Thu thập item
        Heal(10);

        // Xóa khỏi manager
        ItemManager.Instance.RemoveItem(other.gameObject);
    }
}
```

### Best Practice Cho Manager

1. **Dùng Singleton Pattern**
   ```csharp
   public static MyManager Instance { get; private set; }

   void Awake()
   {
       Instance = this;
   }
   ```

2. **DontDestroyOnLoad Cho Manager Duy Trì**
   ```csharp
   void Awake()
   {
       if (Instance == null)
       {
           Instance = this;
           DontDestroyOnLoad(gameObject);
       }
       else
       {
           Destroy(gameObject);
       }
   }
   ```

3. **Xóa Dữ Liệu Khi Load Scene**
   ```csharp
   void OnEnable()
   {
       SceneManager.sceneLoaded += OnSceneLoaded;
   }

   void OnDisable()
   {
       SceneManager.sceneLoaded -= OnSceneLoaded;
   }

   void OnSceneLoaded(Scene scene, LoadSceneMode mode)
   {
       // Xóa list
       activeItems.Clear();
   }
   ```

4. **Kiểm Tra Null**
   ```csharp
   if (GameManager.Instance != null)
       GameManager.Instance.Victory();
   ```

---

## Các Vấn Đề Thường Gặp & Giải Pháp

### Vấn Đề 1: NullReferenceException Trên Manager.Instance

**Triệu chứng**:
- `NullReferenceException: Object reference not set to an instance`
- Xảy ra khi truy cập `Manager.Instance`

**Nguyên Nhân & Cách Sửa**:

1. **GameObject Manager Không Có Trong Scene**
   ```
   Kiểm tra Hierarchy:
   - GameManager GameObject ✓
   - SoundManager GameObject ✓

   Cách sửa: Thêm manager GameObject vào scene
   ```

2. **Truy Cập Trước Awake()**
   ```csharp
   // ❌ Thứ tự sai
   void Awake()
   {
       GameManager.Instance.AddListener(this);  // Instance chưa được set!
       Instance = this;
   }

   // ✓ Thứ tự đúng
   void Awake()
   {
       Instance = this;  // Đặt trước
   }

   void OnEnable()
   {
       if (GameManager.Instance)
           GameManager.Instance.AddListener(this);  // Giờ an toàn
   }
   ```

3. **Script Execution Order**
   ```
   Edit → Project Settings → Script Execution Order
   - GameManager: -100 (chạy trước)
   - Manager khác: 0
   - Game object: 100
   ```

### Vấn Đề 2: Listener Không Nhận Sự Kiện

**Triệu chứng**:
- GameManager broadcast sự kiện, nhưng listener không phản hồi

**Nguyên Nhân & Cách Sửa**:

1. **Không Implement IListener**
   ```csharp
   // ❌ Thiếu interface
   public class MyScript : MonoBehaviour
   {
       public void IPlay() { }  // Không work!
   }

   // ✓ Đúng
   public class MyScript : MonoBehaviour, IListener
   {
       public void IPlay() { }
   }
   ```

2. **Không Được Thêm Vào Listener List**
   ```csharp
   // Kiểm tra trong Start():
   Debug.Log("Listeners count: " + GameManager.Instance.listeners.Count);

   // Cách sửa: Thêm thủ công
   void OnEnable()
   {
       GameManager.Instance.AddListener(this);
   }
   ```

3. **Bị Remove Quá Sớm**
   ```csharp
   // ❌ Removed trước sự kiện
   void Die()
   {
       GameManager.Instance.RemoveListener(this);
       // Sự kiện Victory kích hoạt giờ → không nhận được
   }

   // ✓ Remove sau khi xử lý
   public void ISuccess()
   {
       // Xử lý sự kiện
       GameManager.Instance.RemoveListener(this);
   }
   ```

### Vấn Đề 3: Âm Thanh Không Phát

**Triệu chứng**:
- Gọi `SoundManager.PlaySfx()`, không có âm thanh phát

**Nguyên Nhân & Cách Sửa**:

1. **Audio Clip Chưa Được Gán**
   ```csharp
   // Kiểm tra trong Inspector
   SoundManager → soundClick: None (AudioClip) ✗

   // Cách sửa: Kéo file audio vào field
   ```

2. **GlobalValue.isSound = false**
   ```csharp
   // Kiểm tra cài đặt
   Debug.Log("Sound enabled: " + GlobalValue.isSound);

   // Cách sửa: Bật âm thanh
   GlobalValue.isSound = true;
   SoundManager.SoundVolume = 1;
   ```

3. **Không Có AudioListener Trong Scene**
   ```
   Kiểm tra Main Camera:
   - Audio Listener component ✓

   Cách sửa: Thêm AudioListener vào Camera
   ```

4. **Volume = 0**
   ```csharp
   // Kiểm tra volume
   Debug.Log("Sound volume: " + SoundManager.SoundVolume);

   // Cách sửa: Tăng volume
   SoundManager.SoundVolume = 1;
   ```

### Vấn Đề 4: Victory/GameOver Không Kích Hoạt

**Triệu chứng**:
- Tất cả enemy chết, nhưng không có màn hình chiến thắng
- Pháo đài bị phá hủy, nhưng không có màn hình game over

**Nguyên Nhân & Cách Sửa**:

1. **Victory() Không Được Gọi**
   ```csharp
   // Trong LevelEnemyManager, thêm debug:
   while (isEnemyAlive()) { yield return new WaitForSeconds(0.1f); }
   Debug.Log("All enemies dead!");
   GameManager.Instance.Victory();  // Đảm bảo được gọi
   ```

2. **State Đã Được Set**
   ```csharp
   // Trong GameManager.Victory()
   if (State == GameState.Success)
       return;  // Đã thắng rồi, thoát sớm

   // Cách sửa: Chỉ gọi Victory() một lần
   ```

3. **Listener Không Phản Hồi**
   ```csharp
   // Trong MenuManager.ISuccess(), thêm debug:
   public void ISuccess()
   {
       Debug.Log("MenuManager received ISuccess!");
       StartCoroutine(VictoryCo());
   }
   ```

### Vấn Đề 5: Enemy Spawn Tất Cả Cùng Lúc

**Triệu chứng**:
- Tất cả enemy xuất hiện tức thì thay vì theo thời gian

**Nguyên Nhân & Cách Sửa**:

1. **Giá Trị Wait Sai**
   ```csharp
   // Kiểm tra cấu hình wave
   EnemyWave[0].wait = 0;  // Nên > 0
   EnemySpawn.rate = 0;    // Nên > 0

   // Cách sửa: Đặt delay phù hợp
   EnemyWave[0].wait = 2;
   EnemySpawn.rate = 0.5f;
   ```

2. **Coroutine Không Start**
   ```csharp
   // Trong LevelEnemyManager.IPlay(), thêm debug:
   public void IPlay()
   {
       Debug.Log("Starting spawn coroutine");
       StartCoroutine(SpawnEnemyCo());
   }
   ```

3. **Time.timeScale = 0**
   ```csharp
   // Kiểm tra time scale
   Debug.Log("Time scale: " + Time.timeScale);

   // Cách sửa: Đảm bảo time scale = 1
   Time.timeScale = 1;
   ```

### Vấn Đề 6: Nhạc Không Dừng

**Triệu chứng**:
- Gọi `PauseMusic(true)`, nhạc vẫn tiếp tục

**Nguyên Nhân & Cách Sửa**:

1. **Method Sai**
   ```csharp
   // ❌ Tắt tiếng nhưng không dừng
   SoundManager.Instance.PauseMusic(true);

   // ✓ Thực sự dừng
   SoundManager.Instance.musicAudio.Stop();
   ```

2. **Nhiều Audio Source**
   ```
   Kiểm tra scene có duplicate:
   - SoundManager (1)
   - SoundManager (1) ← Thừa!

   Cách sửa: Xóa duplicate
   ```

---

## Tóm Tắt

**Hệ Thống Managers** phối hợp tất cả hệ thống game thông qua:

1. **GameManager** - Controller trạng thái trung tâm
   - Quản lý trạng thái game (Menu, Playing, Pause, Success, GameOver)
   - Observer pattern qua interface IListener
   - Broadcast sự kiện đến tất cả listener
   - Xử lý logic Victory/GameOver

2. **LevelEnemyManager** - Spawn wave enemy
   - Load cấu hình wave
   - Spawn enemy theo thời gian với delay
   - Theo dõi enemy đã spawn
   - Phát hiện điều kiện chiến thắng

3. **LevelManager** - Quản lý tài nguyên
   - Khởi tạo mana từ cấu hình level
   - Cung cấp truy cập mana toàn cục

4. **SoundManager** - Hệ thống audio
   - Singleton pattern cho truy cập toàn cục
   - Hai AudioSource (nhạc + hiệu ứng âm thanh)
   - Điều khiển volume
   - Chức năng tắt/bật tiếng

**Khái Niệm Chính**:
- **Singleton Pattern**: Truy cập toàn cục qua `.Instance`
- **Observer Pattern**: Hệ thống sự kiện loosely-coupled qua IListener
- **Coroutine**: Spawn enemy dựa trên thời gian
- **DRY Principle**: Manager tập trung giảm trùng lặp code

**Best Practice**:
- Luôn kiểm tra null `Manager.Instance`
- Add/remove listener trong OnEnable/OnDisable
- Dùng singleton cho hệ thống toàn cục
- Broadcast sự kiện thay vì reference trực tiếp

**Bước Tiếp Theo**:
- Đọc `10_Huong_Dan_Thuc_Hanh.md` cho ví dụ thực tế
- Đọc `02_He_Thong_Player.md` để xem player dùng manager như thế nào
- Đọc `03_He_Thong_Enemy.md` để xem implement IListener

---

**Cập Nhật Lần Cuối**: 2025
**File**: `Documents/05_Cac_Manager.md`
