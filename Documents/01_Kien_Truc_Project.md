# Project Architecture - Lawn Defense: Monsters Out

---
**🌐 Ngôn ngữ:** Tiếng Việt
**📄 File gốc:** [01_Project_Architecture.md](01_Project_Architecture.md)
**🔄 Cập nhật lần cuối:** 2025-01-XX
---

**Yêu cầu trước:** Đọc `00_Unity_Co_Ban.md` trước
**Đối tượng độc giả:** Developers muốn hiểu cấu trúc project
**Thời gian đọc ước tính:** 30-40 phút
**Tài liệu liên quan:** → `02_He_Thong_Player_Day_Du.md`, `03_He_Thong_Enemy_Day_Du.md`

---

## Mục Lục
1. [Tổng Quan High-Level](#1-tổng-quan-high-level)
2. [Loại Project & Chi Tiết Kỹ Thuật](#2-loại-project--chi-tiết-kỹ-thuật)
3. [Cấu Trúc Thư Mục](#3-cấu-trúc-thư-mục)
4. [Sơ Đồ Các Hệ Thống Chính](#4-sơ-đồ-các-hệ-thống-chính)
5. [Design Patterns Được Dùng](#5-design-patterns-được-dùng)
6. [Data Flow & Game Loop](#6-data-flow--game-loop)
7. [Scene Flow](#7-scene-flow)
8. [Bản Đồ System Dependencies](#8-bản-đồ-system-dependencies)
9. [Tham Khảo Các Class Chính](#9-tham-khảo-các-class-chính)
10. [Tổ Chức Namespace](#10-tổ-chức-namespace)

---

## 1. Tổng Quan High-Level

### 1.1 Mô Tả Project

**Tên:** Lawn Defense: Monsters Out
**Thể loại:** 2D Tower Defense / Action Defense
**Nền tảng:** Mobile (Android/iOS)
**Unity Version:** 2021.x

**Core Gameplay:**
- Player điều khiển **nhân vật Archer** tự động bắn tên vào enemies
- **Bảo vệ fortress** khỏi các làn sóng quái vật tiến từ bên phải
- **Hệ thống nâng cấp** cho nhân vật, vũ khí, và stats
- **Tiến độ theo level** với độ khó tăng dần
- **Monetization** thông qua IAP (In-App Purchases) và Unity Ads

**Tính Năng Chính:**
- ⚔️ **Bắn Tự Động:** Player archer tự nhắm và bắn dùng tính toán trajectory
- 🎯 **Sinh Theo Làn Sóng:** Enemies spawn theo waves đã cấu hình
- 💀 **Nhiều Loại Enemy:** Enemies cận chiến, tầm xa, bay, tank
- 🔥 **Hiệu Ứng Vũ Khí:** Độc, Cháy, Đóng băng, Điện giật
- 🛒 **Hệ Thống Shop:** Nâng cấp nhân vật, mua items bằng coins hoặc IAP
- 📊 **Tiến Độ:** Mở khóa levels, nhận sao dựa trên thành tích

### 1.2 Technical Stack

```
Unity Engine 2021.x
    ├─ C# Scripting Language
    ├─ Unity Input System
    ├─ Unity 2D Tools (Sprite Renderer, Tilemap)
    ├─ Custom 2D Physics (Raycast-based, không dùng Rigidbody2D)
    ├─ Unity IAP (In-App Purchases)
    ├─ Unity Ads + AdMob Integration
    ├─ PlayerPrefs để lưu data local
    └─ Namespace: RGame (tất cả scripts)
```

**Tại Sao Custom Physics?**
- Kiểm soát tốt hơn với character movement
- Collision detection chính xác dùng raycasts
- Performance tốt hơn cho 2D side-scrolling
- Tránh các quirks của Rigidbody2D (bouncing không mong muốn, rotation)

---

## 2. Loại Project & Chi Tiết Kỹ Thuật

### 2.1 Phân Tích Game Type

**Phân Tích Thể Loại:**
```
Tower Defense (50%)
├─ Vị trí player cố định (archer)
├─ Enemies tấn công theo làn sóng
├─ Bảo vệ mục tiêu (fortress)
└─ Tiến độ nâng cấp

Action Game (30%)
├─ Bắn do player điều khiển
├─ Nhắm thủ công (tự tính toán)
├─ Chiến đấu real-time
└─ Mechanics né tránh (player có thể di chuyển)

Idle Game Elements (20%)
├─ Bắn tự động (không cần fire thủ công)
├─ Nâng cấp dần dần
├─ Thu thập coins
└─ Tiến độ kiểu prestige
```

### 2.2 Core Game Loop

```
1. CHỌN LEVEL
   └─ Chọn từ levels đã mở trong Map UI
       ↓
2. LOAD GAME SCENE
   └─ GameManager instantiates level prefab
   └─ MenuManager khởi tạo UI
       ↓
3. NHẤN NÚT "PLAY"
   └─ GameManager.StartGame() được gọi
   └─ Game state thay đổi: Menu → Playing
   └─ Tất cả IListeners nhận IPlay() event
       ↓
4. GAMEPLAY LOOP (Playing State)
   ├─ [Player System]
   │   └─ Player_Archer tự phát hiện enemies
   │   └─ Tính toán trajectory
   │   └─ Bắn arrows với effects
   ├─ [Enemy System]
   │   └─ LevelEnemyManager spawns waves
   │   └─ Enemies đi về fortress
   │   └─ Enemies tấn công khi trong range
   ├─ [Combat System]
   │   └─ Arrows trúng enemies (TakeDamage)
   │   └─ Apply weapon effects (poison, burn, etc.)
   │   └─ Enemies chết, cho coins
   ├─ [UI System]
   │   └─ Update health bars
   │   └─ Hiển thị floating text (số damage)
   │   └─ Hiển thị wave progress
   └─ [Win/Loss Conditions]
       ├─ THẮNG: Tất cả waves bị tiêu diệt
       └─ THUA: Fortress health về 0
           ↓
5. END GAME
   ├─ Chiến thắng → Menu_Victory screen
   │   └─ Trao sao (1-3 dựa trên thành tích)
   │   └─ Mở khóa level tiếp
   │   └─ Cho coins
   └─ Thua → Game Over screen
       └─ Retry hoặc quay về map
```

---

## 3. Cấu Trúc Thư Mục

### 3.1 Tổ Chức Project

```
Lawn_Defense-Monsters_Out/
├── Assets/
│   ├── _MonstersOut/                 ← Thư mục game chính (underscore = top priority)
│   │   ├── AdController/             ← Scripts tích hợp quảng cáo
│   │   │   ├── AdmobController.cs
│   │   │   ├── AdsManager.cs
│   │   │   └── UnityAds.cs
│   │   │
│   │   ├── Editor/                   ← Custom Unity Editor scripts
│   │   │   ├── GameModeEditor.cs     ← Tùy chỉnh Inspector cho GameMode
│   │   │   └── ReadOnlyEditor.cs     ← [ReadOnly] attribute editor
│   │   │
│   │   ├── Scenes/                   ← Game scenes
│   │   │   └── SampleScene.unity     ← Scene game chính
│   │   │
│   │   ├── Prefabs/                  ← Game objects có thể tái sử dụng
│   │   │   ├── Enemies/
│   │   │   ├── Players/
│   │   │   ├── Projectiles/
│   │   │   └── UI/
│   │   │
│   │   └── Scripts/                  ← **TẤT CẢ GAME CODE (60+ scripts)**
│   │       ├── AI/                   ← Enemy behavior (11 scripts)
│   │       ├── Controllers/          ← Physics & projectiles (7 scripts)
│   │       ├── Helpers/              ← Utilities (9 scripts)
│   │       ├── Managers/             ← Game managers (14 scripts)
│   │       ├── Player/               ← Player scripts (3 scripts)
│   │       └── UI/                   ← UI scripts (16 scripts)
│   │
│   ├── Audio/                        ← Sound effects & nhạc
│   │   ├── Music/                    ← Nhạc nền
│   │   └── Sound/                    ← SFX (bắn, đánh, chết)
│   │
│   ├── Resources/                    ← Assets có thể load runtime
│   │   └── Sprite/                   ← Tất cả visual assets
│   │       ├── Background/           ← Backgrounds, battlefield
│   │       ├── Enemy/                ← 9 enemy sprite sets (animations)
│   │       ├── Player/               ← Player character sprites
│   │       ├── Fortress/             ← Fortress/base sprites
│   │       └── GUI/                  ← UI elements (buttons, icons)
│   │
│   ├── Scenes/                       ← Additional scenes
│   │   └── SampleScene.unity
│   │
│   ├── Settings/                     ← Unity project settings
│   │   ├── InputSystem_Actions      ← Input configuration
│   │   └── ... (project settings)
│   │
│   └── TutorialInfo/                 ← Unity tutorial assets (có thể bỏ qua)
│
├── Documents/                        ← **TÀI LIỆU NÀY**
│   ├── 00_Unity_Co_Ban.md
│   ├── 01_Kien_Truc_Project.md      ← BẠN Ở ĐÂY
│   ├── scripts/                      ← Legacy Vietnamese docs
│   └── ... (docs khác)
│
├── ProjectSettings/                  ← Unity configuration
├── Packages/                         ← Unity packages
└── Logs/                             ← Unity logs
```

### 3.2 Scripts Folder Deep Dive

**AI/ - Enemy Behavior (11 scripts):**
```
AI/
├── Enemy.cs                          ← **BASE CLASS** cho tất cả enemies
│   │ State Machine: SPAWNING, IDLE, WALK, ATTACK, HIT, DEATH
│   │ Effect System: BURN, POISON, FREEZE, SHOCK
│   │ Health management, damage handling
│   │ IListener implementation (game events)
│   │ ICanTakeDamage implementation (damage interface)
│   └── Dùng bởi: Tất cả enemy types VÀ Player (kế thừa!)
│
├── SmartEnemyGrounded.cs             ← **MAIN IMPLEMENTATION** cho ground enemies
│   │ Kế thừa từ Enemy
│   │ Implements movement với Controller2D
│   │ Xử lý attack logic (gọi attack modules)
│   │ Phát hiện target, đuổi theo player
│   └── Dùng bởi: Hầu hết enemy prefabs
│
├── EnemyMeleeAttack.cs               ← Melee attack module
├── EnemyRangeAttack.cs               ← Ranged attack module
├── EnemyThrowAttack.cs               ← Throwing attack module
├── EnemySpawn.cs                     ← Spawn configuration data class
├── GiveCoinWhenDie.cs                ← Rơi coins khi chết
├── ICanTakeDamage.cs                 ← Damage interface
├── ICanTakeDamageBodyPart.cs         ← Body part damage interface
├── TheFortrest.cs                    ← Fortress (căn cứ của player)
└── WitchHeal.cs                      ← Support enemy có khả năng hồi máu
```

**Controllers/ - Physics & Projectiles (7 scripts):**
```
Controllers/
├── Controller2D.cs                   ← **CORE** custom 2D physics controller
│   │ Collision detection dựa trên Raycasting
│   │ Movement không dùng Rigidbody2D
│   └── Dùng bởi: Player, Enemies
│
├── RaycastController.cs              ← Base class cho raycast collision
│   │ Quản lý raycasts để detect mặt đất/tường
│   └── Được kế thừa bởi: Controller2D
│
├── Projectile.cs                     ← Base projectile class
├── SimpleProjectile.cs               ← Projectile đường thẳng đơn giản
├── ArrowProjectile.cs                ← Arrow với gravity và trajectory
├── CameraController.cs               ← Camera follow mượt mà
└── FixedCamera.cs                    ← Camera tĩnh
```

**Helpers/ - Utilities (9 scripts):**
```
Helpers/
├── GlobalValue.cs                    ← **CENTRAL DATA STORE**
│   │ PlayerPrefs wrapper
│   │ Coins, level progress, unlocks
│   │ Save/load player data
│   └── Static class, truy cập global
│
├── AnimationHelper.cs                ← Animation utilities
├── CheckTargetHelper.cs              ← Target detection (raycasts)
├── SpawnItemHelper.cs                ← Item spawning utility
├── WeaponEffect.cs                   ← Weapon effect data (poison, burn, etc.)
├── AutoDestroy.cs                    ← Tự động destroy objects sau thời gian
├── RotateAround.cs                   ← Xoay object quanh điểm
├── SortingLayerHelper.cs             ← Sprite layer sorting
├── IListener.cs                      ← **LISTENER PATTERN INTERFACE**
│   └── Methods: IPlay(), IPause(), IGameOver(), ISuccess(), IUnPause()
└── ReadOnlyAttribute.cs              ← [ReadOnly] attribute cho Inspector
```

**Managers/ - Game Management (14 scripts):**
```
Managers/
├── GameManager.cs                    ← **SINGLETON, CORE CONTROLLER**
│   │ Quản lý game state (Menu, Playing, GameOver, Success, Pause)
│   │ Listener pattern coordinator
│   │ Spawns level prefabs
│   │ Gọi Victory()/GameOver()
│   └── Truy cập qua: GameManager.Instance
│
├── LevelEnemyManager.cs              ← **WAVE SPAWNING CONTROLLER**
│   │ Spawns enemies theo waves
│   │ IListener implementation
│   │ Quản lý enemy timing và counts
│   └── Được cấu hình bởi level prefab
│
├── LevelManager.cs                   ← Level progression logic
├── MenuManager.cs                    ← UI initialization và control
├── SoundManager.cs                   ← Audio management (Singleton)
├── ShopManager.cs                    ← Shop system logic
├── ShopCharacterUpgrade.cs           ← Character upgrade UI
├── ShopItemUpgrade.cs                ← Item upgrade UI
├── GameLevelSetup.cs                 ← Level prefab configuration
├── GameMode.cs                       ← Game mode data
├── Level.cs                          ← Level data class
├── LevelWave.cs                      ← Wave configuration data
├── IncreaseGameSpeed.cs              ← Game speed control
├── Purchaser.cs                      ← IAP handler (Unity IAP)
├── IAPItem.cs                        ← IAP item data
└── UnityAdsitem.cs                   ← Unity Ads item data
```

**Player/ - Player Character (3 scripts):**
```
Player/
├── Player_Archer.cs                  ← **MAIN PLAYER CLASS**
│   │ Kế thừa từ Enemy! (tái sử dụng health/damage system)
│   │ Auto-targeting system
│   │ Trajectory calculation cho arrows
│   │ Arrow shooting với reload time
│   │ Movement với Controller2D
│   └── Một trong những loại (player là enemy đặc biệt!)
│
├── CharacterManager.cs               ← Character spawning logic
└── UpgradedCharacterParameter.cs     ← Character upgrade stats
    │ Lưu trữ: health, damage, weapon effects
    │ Saved qua PlayerPrefs
    └── ScriptableObject (asset-based data)
```

**UI/ - User Interface (16 scripts):**
```
UI/
├── MenuManager.cs                    ← Main menu controller
├── Menu_Victory.cs                   ← Victory screen UI
├── MainMenuHomeScene.cs              ← Home screen UI
├── MapControllerUI.cs                ← Level select map UI
├── BuyCharacterBtn.cs                ← Character purchase button
├── NotEnoughCoins.cs                 ← Popup không đủ tiền
├── GiftVideoAd.cs                    ← Rewarded video ad UI
├── AutoAddManaUI.cs                  ← Auto-mana UI element
├── HealthBarEnemyNew.cs              ← Enemy health bar (theo enemy)
├── FloatingText.cs                   ← Popup số damage
├── FloatingTextManager.cs            ← Floating text object pool
├── BlackScreenUI.cs                  ← Hiệu ứng fade màn hình
├── FlashScene.cs                     ← Scene transition flash
├── RGFade.cs                         ← Color fade utility
├── Tutorial.cs                       ← In-game tutorial system
├── UI_UI.cs                          ← General UI utilities
└── Helper_Swipe.cs                   ← Swipe gesture detection
```

---

## 4. Sơ Đồ Các Hệ Thống Chính

### 4.1 System Overview

```
┌─────────────────────────────────────────────────────────────────────┐
│                         GAME MANAGER                                │
│                      (Central Controller)                           │
│                                                                     │
│  - Singleton Instance                                               │
│  - Game State Machine (Menu, Playing, Pause, GameOver, Success)    │
│  - Listener Pattern Coordinator                                     │
│  - Level Prefab Spawning                                            │
│  - LayerMask Configuration                                          │
└────────────┬────────────────────────────────────────────────────────┘
             │
             │ (Broadcast events đến tất cả IListeners)
             │
    ┌────────┼────────┬────────┬────────┬────────┬────────┐
    │        │        │        │        │        │        │
    ▼        ▼        ▼        ▼        ▼        ▼        ▼
┌────────┐ ┌──────┐ ┌──────┐ ┌─────┐ ┌──────┐ ┌──────┐ ┌──────┐
│ PLAYER │ │ENEMY │ │ MENU │ │ UI  │ │SOUND │ │LEVEL │ │ ADS  │
│ SYSTEM │ │SYSTEM│ │  MGR │ │ELEMS│ │ MGR  │ │ENEMY │ │ MGR  │
│        │ │      │ │      │ │     │ │      │ │ MGR  │ │      │
└────────┘ └──────┘ └──────┘ └─────┘ └──────┘ └──────┘ └──────┘
    │          │         │       │        │        │        │
    └──────────┴─────────┴───────┴────────┴────────┴────────┘
                         │
                  (Tất cả implement IListener)
```

### 4.2 Detailed System Interaction

```
┌─────────────────────────┐
│   GameManager.Start()   │
│   Load Level Prefab     │
└────────────┬────────────┘
             │
             ▼
┌─────────────────────────┐        ┌──────────────────────┐
│ MenuManager.ShowUI()    │───────→│  Player clicks PLAY  │
└────────────┬────────────┘        └───────────┬──────────┘
             │                                  │
             │                                  │
             ▼                                  ▼
┌─────────────────────────────────────────────────────────┐
│         GameManager.StartGame()                         │
│         State = Playing                                 │
│         Tìm tất cả IListeners                           │
│         Gọi IPlay() trên mỗi listener                   │
└───────────┬─────────────────────────────────────────────┘
            │
            └─────┬───────────┬─────────────┬──────────────┐
                  │           │             │              │
                  ▼           ▼             ▼              ▼
        ┌──────────────┐ ┌─────────┐ ┌──────────┐ ┌──────────────┐
        │ Player.IPlay │ │Enemy    │ │UI.IPlay  │ │LevelEnemy    │
        │ - Enable     │ │.IPlay   │ │- Hiển thị│ │Manager.IPlay │
        │ - Bắt đầu    │ │- Bắt đầu│ │  game UI │ │- Bắt đầu     │
        │   bắn tự động│ │  di     │ └──────────┘ │  wave spawn  │
        └──────────────┘ │  chuyển │              └──────┬───────┘
                         └─────────┘                     │
                ┌─────────────────────────────────────────┘
                │
                ▼
┌───────────────────────────────────────────────────────────────┐
│                    GAME LOOP (Playing)                        │
├───────────────────────────────────────────────────────────────┤
│                                                               │
│  ┌─────────────┐         ┌──────────────┐                   │
│  │   PLAYER    │         │   ENEMIES    │                   │
│  ├─────────────┤         ├──────────────┤                   │
│  │ Tự phát hiện│───────→ │ Đi về        │                   │
│  │ enemies     │         │ fortress     │                   │
│  │             │         │              │                   │
│  │ Tính toán   │         │ Tấn công khi │                   │
│  │ trajectory  │         │ trong range  │                   │
│  │             │         │              │                   │
│  │ Bắn arrow   │───────→ │ TakeDamage() │                   │
│  │             │         │              │                   │
│  │             │         │ Apply effect │                   │
│  │             │         │ (poison/burn)│                   │
│  │             │         │              │                   │
│  │             │         │ Die() →      │                   │
│  │             │         │ GiveCoin()   │                   │
│  └─────────────┘         └──────────────┘                   │
│         │                        │                          │
│         │                        │                          │
│         └────────┬───────────────┘                          │
│                  │                                           │
│                  ▼                                           │
│          ┌──────────────┐                                   │
│          │  UI UPDATES  │                                   │
│          ├──────────────┤                                   │
│          │ Health bars  │                                   │
│          │ Damage text  │                                   │
│          │ Coin count   │                                   │
│          │ Wave progress│                                   │
│          └──────────────┘                                   │
│                                                               │
└───────────────────────────────────────────────────────────────┘
                            │
                  ┌─────────┴──────────┐
                  │                    │
                  ▼                    ▼
        ┌──────────────────┐  ┌────────────────┐
        │ Tất cả waves clear│  │Fortress HP = 0 │
        │ GameManager.     │  │GameManager.    │
        │ Victory()        │  │ GameOver()     │
        └────────┬─────────┘  └────────┬───────┘
                 │                     │
                 │                     │
                 ▼                     ▼
        ┌──────────────────┐  ┌────────────────┐
        │ Gọi ISuccess()   │  │Gọi IGameOver() │
        │ Tất cả listeners │  │Tất cả listeners│
        │ Hiển thị victory │  │Hiển thị game   │
        │ UI               │  │over UI         │
        │ Award stars      │  │                │
        │ Unlock next level│  │                │
        └──────────────────┘  └────────────────┘
```

---

## 5. Design Patterns Được Dùng

Project này thể hiện nhiều professional design patterns.

### 5.1 Singleton Pattern

**Mục đích:** Đảm bảo chỉ một instance tồn tại và cung cấp truy cập global

**Implementation trong GameManager.cs:**
```csharp
public class GameManager : MonoBehaviour
{
    // Static property để truy cập global
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        // Gán instance này
        Instance = this;

        // Additional singleton setup
        // (Lưu ý: Project này không dùng DontDestroyOnLoad cho GameManager
        //  vì nó được tạo per scene)
    }
}
```

**Cách Dùng:**
```csharp
// Truy cập từ bất kỳ script nào
if (GameManager.Instance.State == GameManager.GameState.Playing)
{
    // Làm gì đó
}

// Kiểm tra layer
int enemyLayer = GameManager.Instance.layerEnemy;
```

**Singletons Trong Project Này:**
- `GameManager` - Game state controller
- `SoundManager` - Audio management
- `FloatingTextManager` - Damage number pool
- `AdsManager` - Ad system

**Tại Sao Singleton?**
- ✅ Truy cập global không cần FindObjectOfType (nhanh hơn)
- ✅ Ngăn nhiều instances gây conflict
- ✅ API sạch (GameManager.Instance.Victory())

### 5.2 Observer Pattern (Listener System)

**Mục đích:** Decouple systems - GameManager không cần biết về mỗi system trực tiếp

**IListener Interface:**
```csharp
// Định nghĩa trong Helpers/IListener.cs
public interface IListener
{
    void IPlay();        // Game bắt đầu
    void IPause();       // Game tạm dừng
    void IUnPause();     // Game tiếp tục
    void IGameOver();    // Game over (thua)
    void ISuccess();     // Chiến thắng
}
```

**GameManager Implementation:**
```csharp
public class GameManager : MonoBehaviour
{
    // Danh sách tất cả objects lắng nghe game events
    public List<IListener> listeners;

    public void StartGame()
    {
        State = GameState.Playing;

        // Tìm tất cả objects implementing IListener
        var listener_ = FindObjectsOfType<MonoBehaviour>().OfType<IListener>();
        foreach (var _listener in listener_)
        {
            listeners.Add(_listener);
        }

        // Broadcast IPlay đến tất cả listeners
        foreach (var item in listeners)
        {
            item.IPlay();
        }
    }

    public void Victory()
    {
        State = GameState.Success;

        // Broadcast ISuccess đến tất cả listeners
        foreach (var item in listeners)
        {
            if (item != null)
                item.ISuccess();
        }
    }

    // Tương tự cho Gamepause(), UnPause(), GameOver()
}
```

**Listener Example (LevelEnemyManager):**
```csharp
public class LevelEnemyManager : MonoBehaviour, IListener
{
    // IListener implementation
    public void IPlay()
    {
        // Bắt đầu spawning waves khi game start
        StartCoroutine(SpawnEnemyWaves());
    }

    public void IPause()
    {
        // Dừng spawning khi pause
        StopAllCoroutines();
    }

    public void IUnPause()
    {
        // Resume spawning
        StartCoroutine(SpawnEnemyWaves());
    }

    public void IGameOver()
    {
        // Dừng tất cả hoạt động
        StopAllCoroutines();
    }

    public void ISuccess()
    {
        // Chiến thắng - dừng spawning
    }
}
```

**Ai Implements IListener:**
- `Enemy` (base class - tất cả enemies)
- `Player_Archer`
- `LevelEnemyManager`
- UI components (menus, health bars)
- Các managers khác

**Tại Sao Observer Pattern?**
- ✅ **Decoupling:** GameManager không biết về các classes cụ thể
- ✅ **Scalability:** Thêm listeners mới không cần thay đổi GameManager
- ✅ **Synchronization:** Tất cả systems phản ứng với state changes đồng thời

**Sequence Diagram:**
```
GameManager          Enemy1          Enemy2      LevelEnemyManager      MenuManager
     │                 │               │                 │                   │
     │  StartGame()    │               │                 │                   │
     ├────────────────→│               │                 │                   │
     │                 │               │                 │                   │
     │  listeners.IPlay()               │                │                   │
     ├─────────────────┼───────────────┼─────────────────┼───────────────────┤
     │                 │               │                 │                   │
     │                 │  IPlay()      │                 │                   │
     │                 ├──────────────→│   IPlay()       │                   │
     │                 │  (bắt đầu di  │   (bắt đầu di   │    IPlay()        │
     │                 │   chuyển)     │    chuyển)      ├──────────────────→│
     │                 │               │                 │   (ẩn menu, hiện
     │                 │               │   (spawn waves) │    game UI)
```

### 5.3 State Machine Pattern

**Mục đích:** Quản lý hành vi object dựa trên state hiện tại

**Enemy State Machine:**
```csharp
// Định nghĩa trong AI/Enemy.cs
public enum ENEMYSTATE
{
    SPAWNING,    // Enemy đang xuất hiện (animation)
    IDLE,        // Enemy đang đứng yên
    WALK,        // Enemy đang di chuyển
    ATTACK,      // Enemy đang tấn công
    HIT,         // Enemy vừa bị đánh
    DEATH        // Enemy đã chết
}

public class Enemy : MonoBehaviour
{
    public ENEMYSTATE enemyState = ENEMYSTATE.IDLE;

    public void SetEnemyState(ENEMYSTATE state)
    {
        enemyState = state;
        // Transition logic
    }

    public virtual void Update()
    {
        // Hành vi dựa trên state
        switch (enemyState)
        {
            case ENEMYSTATE.SPAWNING:
                // Phát spawn animation, không thể di chuyển
                break;
            case ENEMYSTATE.WALK:
                // Di chuyển về target
                MoveToTarget();
                break;
            case ENEMYSTATE.ATTACK:
                // Tấn công target
                AttackTarget();
                break;
            case ENEMYSTATE.HIT:
                // Phát hit animation
                break;
            case ENEMYSTATE.DEATH:
                // Chết, cho coins, destroy
                break;
        }
    }
}
```

**State Transition Diagram:**
```
        START
          │
          ▼
     SPAWNING ──────────────┐
          │                 │
          │ (spawn done)    │
          ▼                 │
        WALK ◄──────────────┘
          │ ▲               │
          │ │               │ (detect target)
          │ │               │
          │ │               ▼
          │ │            ATTACK
          │ │               │
          │ └───────────────┘
          │
          │ (take damage)
          ▼
         HIT ────┐
          │      │ (damage < health)
          │      │
          │ ◄────┘
          │
          │ (health <= 0)
          ▼
        DEATH
          │
          ▼
       DESTROY
```

**Game State Machine (GameManager):**
```csharp
public enum GameState
{
    Menu,       // Main menu, level select
    Playing,    // Gameplay đang active
    GameOver,   // Thua
    Success,    // Thắng
    Pause       // Game tạm dừng
}

public GameState State { get; set; }
```

**Tại Sao State Machine?**
- ✅ **Clear behavior:** Mỗi state có actions xác định
- ✅ **Easy debugging:** Log current state
- ✅ **Prevention:** Không thể làm invalid actions (không thể attack khi spawning)

### 5.4 Interface Pattern (ICanTakeDamage)

**Mục đích:** Polymorphic damage system - bất cứ thứ gì có thể nhận damage

**Interface Definition:**
```csharp
// Định nghĩa trong AI/ICanTakeDamage.cs
public enum BODYPART
{
    NONE, HEAD, BODY, ARM, LEG
}

public interface ICanTakeDamage
{
    void TakeDamage(
        float damage,             // Lượng damage
        Vector2 force,            // Knockback force
        Vector2 hitPoint,         // Nơi bị trúng
        GameObject instigator,    // Ai gây damage
        BODYPART bodyPart,        // Body part bị hit (cho critical hits)
        WeaponEffect weaponEffect // Poison, burn, freeze, etc.
    );
}
```

**Implementation trong Enemy.cs:**
```csharp
public class Enemy : MonoBehaviour, ICanTakeDamage
{
    public void TakeDamage(
        float damage,
        Vector2 force,
        Vector2 hitPoint,
        GameObject instigator,
        BODYPART bodyPart = BODYPART.NONE,
        WeaponEffect weaponEffect = null)
    {
        // Đã chết? Bỏ qua
        if (enemyState == ENEMYSTATE.DEATH)
            return;

        // Giảm health
        currentHealth -= (int)damage;

        // Hiển thị floating damage text
        FloatingTextManager.Instance.ShowText(
            "" + (int)damage,
            healthBarOffset,
            Color.red,
            transform.position
        );

        // Spawn hit effect
        if (hitFX)
            Instantiate(hitFX, hitPoint, Quaternion.identity);

        // Update health bar
        if (healthBar)
            healthBar.UpdateValue(currentHealth / (float)health);

        // Kiểm tra chết
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Apply weapon effects (poison, freeze, etc.)
            if (weaponEffect != null)
            {
                switch (weaponEffect.effectType)
                {
                    case WEAPON_EFFECT.POISON:
                        Poison(weaponEffect.poisonDamagePerSec,
                               weaponEffect.poisonTime,
                               instigator);
                        break;
                    case WEAPON_EFFECT.FREEZE:
                        Freeze(weaponEffect.freezeTime, instigator);
                        break;
                }
            }

            Hit(force);  // Phát hit reaction
        }
    }
}
```

**Cách Dùng trong ArrowProjectile.cs:**
```csharp
void OnTriggerEnter2D(Collider2D other)
{
    // Thử lấy ICanTakeDamage component
    var takeDamage = (ICanTakeDamage)other.gameObject.GetComponent(typeof(ICanTakeDamage));

    if (takeDamage != null)
    {
        // Gây damage polymorphically
        takeDamage.TakeDamage(
            damage,
            Vector2.zero,
            transform.position,
            gameObject,
            BODYPART.NONE,
            weaponEffect
        );

        // Destroy arrow
        Destroy(gameObject);
    }
}
```

**Ai Implements ICanTakeDamage:**
- `Enemy` (base class - tất cả enemies)
- `Player_Archer` (kế thừa từ Enemy, nên tự động có)
- `TheFortrest` (căn cứ của player)

**Tại Sao Interface Pattern?**
- ✅ **Polymorphism:** Không quan tâm object là gì, chỉ gọi TakeDamage()
- ✅ **Extensibility:** Objects có thể nhận damage mới chỉ cần implement interface
- ✅ **Unified System:** Một damage calculation cho mọi thứ

### 5.5 Inheritance Hierarchy (Unconventional nhưng Clever)

**Player Kế Thừa từ Enemy!**

Điều này bất thường nhưng thực tế:

```
MonoBehaviour
      │
      └─── Enemy.cs (base class)
           ├─ Health system
           ├─ Damage handling (ICanTakeDamage)
           ├─ Animation system
           ├─ Effects (burn, poison, freeze, shock)
           ├─ IListener implementation
           │
           ├─── SmartEnemyGrounded.cs (hầu hết enemies)
           │    └─ Movement AI
           │    └─ Attack logic
           │
           ├─── WitchHeal.cs (special enemy)
           │    └─ Healing ability
           │
           └─── Player_Archer.cs (PLAYER!)
                └─ Auto-targeting
                └─ Trajectory shooting
                └─ Movement (có thể di chuyển như enemy)
```

**Tại Sao Player Kế Thừa từ Enemy?**

**Lợi ích:**
- ✅ **Code Reuse:** Player cần health, damage, effects - Enemy có tất cả
- ✅ **Unified System:** Một damage system cho mọi thứ
- ✅ **Consistent Behavior:** Player và enemies hoạt động giống nhau
- ✅ **Less Code:** Không cần implement lại health/damage/effects

**Cân nhắc:**
- ⚠️ **Unconventional:** Hầu hết games tách Player và Enemy hierarchies
- ⚠️ **Potentially Confusing:** Developers mới có thể ngạc nhiên
- ⚠️ **Tight Coupling:** Player changes có thể ảnh hưởng enemies

**Player_Archer Specific Additions:**
```csharp
public class Player_Archer : Enemy, ICanTakeDamage, IListener
{
    // UNIQUE TO PLAYER (không có trong Enemy base class)

    [Header("ARROW SHOOT")]
    public float shootRate = 1;       // Fire rate
    public float force = 20;          // Arrow force
    public ArrowProjectile arrow;     // Arrow prefab
    public int arrowDamage = 30;      // Arrow damage
    public Transform firePostion;     // Spawn point cho arrows

    // AUTO-TARGETING SYSTEM (chỉ player)
    IEnumerator AutoCheckAndShoot()
    {
        while (true)
        {
            // Phát hiện enemies
            // Tính toán trajectory
            // Bắn arrow
            yield return new WaitForSeconds(shootRate);
        }
    }

    // TRAJECTORY CALCULATION (chỉ player)
    IEnumerator CheckTarget()
    {
        // Physics simulation để tính góc hoàn hảo
        // Iterate qua các góc để tìm best shot
        // Spawns arrow với force đã tính
    }

    // KẾ THỪA TỪ ENEMY (tái sử dụng!)
    // - health, currentHealth
    // - TakeDamage()
    // - Die()
    // - Hit()
    // - Freeze(), Poison(), Burning(), Shoking()
    // - AnimSetTrigger(), AnimSetBool(), AnimSetFloat()
    // - checkTarget (target detection)
    // - enemyState, enemyEffect
}
```

**So Sánh:**
```
       Enemy (Goblin)               Player_Archer
       ─────────────               ─────────────
Health: ✓ (từ Enemy base)     Health: ✓ (kế thừa)
Damage: ✓ (từ Enemy base)     Damage: ✓ (kế thừa)
Effects: ✓ (burn, poison...)   Effects: ✓ (kế thừa)
AI: ✓ (đi về target)           AI: ❌ (thủ công/auto-aim)
Attack: ✓ (melee/range)        Attack: ✓ (custom trajectory)
```

### 5.6 Object Pooling (Partial Implementation)

**Mục đích:** Tái sử dụng objects thay vì Instantiate/Destroy mỗi frame

**FloatingTextManager Implementation:**
```csharp
public class FloatingTextManager : MonoBehaviour
{
    public static FloatingTextManager Instance;

    public List<FloatingText> pool;  // Pool các text objects

    public void ShowText(string text, Vector2 offset, Color color, Vector3 position)
    {
        // Lấy text available từ pool
        FloatingText floatingText = GetAvailableText();

        if (floatingText != null)
        {
            // Tái sử dụng object hiện có
            floatingText.gameObject.SetActive(true);
            floatingText.Show(text, offset, color, position);
        }
        else
        {
            // Pool rỗng, tạo mới
            FloatingText newText = Instantiate(floatingTextPrefab);
            pool.Add(newText);
            newText.Show(text, offset, color, position);
        }
    }

    FloatingText GetAvailableText()
    {
        // Tìm inactive text trong pool
        foreach (var text in pool)
        {
            if (!text.gameObject.activeInHierarchy)
                return text;
        }
        return null;  // Pool đầy
    }
}
```

**Tại Sao Object Pooling?**
- ✅ **Performance:** Instantiate/Destroy chậm
- ✅ **No GC Spikes:** Ít garbage collection hơn
- ✅ **Smooth Gameplay:** Không có frame drops từ spawning

**Nơi Dùng:**
- `FloatingTextManager` - Damage numbers (pools text objects)
- Projectiles - Một phần (arrows được instantiated, có thể dùng pool)

---

## 6. Data Flow & Game Loop

### 6.1 Complete Data Flow

```
   USER INPUT                  GAME LOGIC                   OUTPUT
┌──────────────┐           ┌──────────────┐           ┌──────────────┐
│              │           │              │           │              │
│ Touch/Click  │──────────→│ MenuManager  │──────────→│ UI Updates   │
│ "Play Button"│           │ .StartGame() │           │              │
│              │           │              │           │              │
└──────────────┘           └──────┬───────┘           └──────────────┘
                                  │
                                  ▼
                          ┌────────────────┐
                          │  GameManager   │
                          │  .StartGame()  │
                          │  State=Playing │
                          └────────┬───────┘
                                   │
                      ┌────────────┼────────────┐
                      │                         │
                      ▼                         ▼
         ┌──────────────────────┐   ┌──────────────────────┐
         │  LevelEnemyManager   │   │   Player_Archer      │
         │  .IPlay()            │   │   .IPlay()           │
         │  Bắt đầu wave spawn  │   │   Enable auto-shoot  │
         └──────────┬───────────┘   └──────────┬───────────┘
                    │                           │
                    │                           │
          ┌─────────▼────────┐                  │
          │ Spawn Enemy      │                  │
          │ (Instantiate)    │                  │
          └─────────┬────────┘                  │
                    │                           │
                    ▼                           │
          ┌──────────────────┐                  │
          │ SmartEnemy       │                  │
          │ Grounded.Start() │                  │
          │ - Init health    │                  │
          │ - State = WALK   │                  │
          │ - Di chuyển về   │                  │
          │   fortress       │◄─────────────────┘
          └─────────┬────────┘     (phát hiện enemy)
                    │                           │
                    │                           │
                    │         ┌─────────────────▼──────────┐
                    │         │ Player bắn arrow           │
                    │         │ ArrowProjectile.Init()     │
                    │         │ - Tính trajectory          │
                    │         │ - Bay về target            │
                    │         └─────────────┬──────────────┘
                    │                       │
                    │                       │ (collision)
                    │                       │
                    ▼◄──────────────────────┘
          ┌──────────────────────────────────────┐
          │ Enemy.TakeDamage()                   │
          │ - Giảm health                        │
          │ - Apply weapon effects               │
          │ - Update health bar                  │
          │ - Hiển thị floating damage text      │
          │ - Kiểm tra nếu health <= 0           │
          └─────────────────┬────────────────────┘
                            │
                   ┌────────┴────────┐
                   │                 │
                   │ (còn sống)      │ (chết)
                   ▼                 ▼
          ┌────────────────┐  ┌─────────────────┐
          │ Enemy.Hit()    │  │ Enemy.Die()     │
          │ - Hit reaction │  │ - State = DEATH │
          │ - Tiếp tục     │  │ - Phát animation│
          └────────────────┘  │ - Cho coins     │
                              │ - Xóa khỏi     │
                              │   active list   │
                              │ - Destroy       │
                              └────────┬────────┘
                                       │
                                       ▼
                              ┌─────────────────┐
                              │ GiveCoinWhenDie │
                              │ .GiveCoin()     │
                              └────────┬────────┘
                                       │
                                       ▼
                              ┌─────────────────┐
                              │ GlobalValue     │
                              │ .Coin += amount │
                              └────────┬────────┘
                                       │
                                       ▼
                              ┌─────────────────┐
                              │ UI Update       │
                              │ CoinText.text   │
                              └─────────────────┘
```

### 6.2 Frame-by-Frame Execution

**Mỗi Frame (60 FPS):**
```
1. Unity gọi Update() trên tất cả active scripts
   ├─ GameManager.Update() - Kiểm tra game state
   ├─ Player_Archer.Update() - Xử lý animation
   ├─ Enemy.Update() - Kiểm tra effects (burn, shock damage)
   ├─ MenuManager.Update() - UI interactions
   └─ ... (60+ Update methods được gọi)

2. Unity gọi FixedUpdate() trên physics scripts (50 FPS)
   ├─ Player_Archer.LateUpdate() - Di chuyển character (custom physics)
   ├─ SmartEnemyGrounded.FixedUpdate() - Di chuyển enemies
   └─ Controller2D.Move() - Raycast collision detection

3. Unity gọi LateUpdate() cho things phụ thuộc Update
   ├─ CameraController.LateUpdate() - Theo player (sau movement)
   ├─ HealthBar.LateUpdate() - Theo enemy (sau movement)
   └─ ...

4. Unity renders frame
   ├─ Sprite Renderers vẽ characters
   ├─ UI Canvas vẽ interface
   ├─ Particle systems vẽ effects
   └─ Camera captures và hiển thị

5. Unity xử lý input (touch, mouse, keyboard)
   └─ Input.GetKey(), Input.GetTouch(), etc.

⟲ LẶP LẠI (mỗi ~16ms cho 60 FPS)
```

---

## 7. Scene Flow

### 7.1 Scene Structure

Project này dùng **MỘT scene chính** với dynamic level loading:

```
SampleScene.unity (Scene Duy Nhất)
├─ Persistent Objects (DontDestroyOnLoad hoặc luôn có)
│   ├─ GameManager (spawns level)
│   ├─ MenuManager (UI controller)
│   ├─ SoundManager (audio)
│   ├─ EventSystem (UI input)
│   └─ Canvas (UI root)
│
├─ Level Prefab (Spawned lúc Runtime)
│   ├─ GameLevelSetup (configuration)
│   ├─ LevelEnemyManager (wave spawning)
│   ├─ Fortress (defense target)
│   ├─ Background (visual)
│   ├─ Ground (collision)
│   └─ Spawn Points (enemy entry)
│
├─ Player (Spawned lúc Runtime)
│   └─ Player_Archer
│
└─ UI Elements
    ├─ MainMenu (ẩn trong gameplay)
    ├─ GameUI (hiển thị trong gameplay)
    ├─ PauseMenu (hiển thị khi pause)
    ├─ VictoryScreen (hiển thị khi thắng)
    └─ GameOverScreen (hiển thị khi thua)
```

### 7.2 Level Loading Flow

```
Game Start
    │
    ▼
┌──────────────────────────────────────┐
│ GameManager.Awake()                  │
│ - Set target frame rate = 60         │
│ - Tạo Singleton instance             │
│ - State = Menu                        │
│ - Khởi tạo listeners list            │
│ - Lấy current level index từ        │
│   GlobalValue.levelPlaying           │
│ - Instantiate level prefab:          │
│   gameLevels[levelPlaying - 1]      │
└────────────────┬─────────────────────┘
                 │
                 ▼
┌──────────────────────────────────────┐
│ Level Prefab Instantiated            │
│ - GameLevelSetup.Awake()             │
│ - LevelEnemyManager.Awake()          │
│ - Fortress spawned                   │
│ - Background loaded                  │
│ - Player spawned (CharacterManager)  │
└────────────────┬─────────────────────┘
                 │
                 ▼
┌──────────────────────────────────────┐
│ MenuManager.Start()                  │
│ - Hiển thị main menu UI              │
│ - Hiển thị level info                │
│ - Đợi user nhấn "Play"               │
└────────────────┬─────────────────────┘
                 │
         ┌───────┴───────┐
         │ User clicks   │
         │ "PLAY" button │
         └───────┬───────┘
                 │
                 ▼
┌──────────────────────────────────────┐
│ MenuManager.OnPlayButtonClick()      │
│ - Gọi GameManager.StartGame()        │
└────────────────┬─────────────────────┘
                 │
                 ▼
┌──────────────────────────────────────┐
│ GameManager.StartGame()              │
│ - State = Playing                    │
│ - Tìm tất cả IListeners              │
│ - Gọi IPlay() trên mỗi listener      │
└────────────────┬─────────────────────┘
                 │
                 ▼
      [GAMEPLAY ACTIVE]
```

### 7.3 UI State Transitions

```
┌──────────────┐
│  MAIN MENU   │ ← Game bắt đầu ở đây
│              │
│ - Title      │
│ - Play Btn   │
│ - Shop Btn   │
│ - Settings   │
└──────┬───────┘
       │
       │ (click Map)
       ▼
┌──────────────┐
│  MAP SELECT  │
│              │
│ - Level icons│
│ - Stars      │
│ - Locked     │
└──────┬───────┘
       │
       │ (select level)
       ▼
┌──────────────┐
│ LEVEL INTRO  │
│              │
│ - Level info │
│ - Play button│
└──────┬───────┘
       │
       │ (click Play)
       ▼
┌──────────────┐
│  GAMEPLAY UI │
│              │
│ - Health bar │
│ - Coin count │
│ - Wave info  │
│ - Pause btn  │
└──────┬───────┘
       │
       ├─────────────────┐
       │                 │
       │ (trong game)    │ (Pause button)
       │                 ▼
       │          ┌──────────────┐
       │          │  PAUSE MENU  │
       │          │              │
       │          │ - Resume     │
       │          │ - Restart    │
       │          │ - Quit       │
       │          └──────┬───────┘
       │                 │
       │                 │ (Resume)
       │◄────────────────┘
       │
       ├─────────────────┬─────────────────┐
       │                 │                 │
       │ (tất cả waves   │ (fortress HP    │
       │  cleared)       │  = 0)           │
       ▼                 ▼                 ▼
┌──────────────┐  ┌──────────────┐  ┌──────────────┐
│VICTORY SCREEN│  │GAMEOVER      │  │   (playing)  │
│              │  │              │  │              │
│ - Stars (1-3)│  │ - Defeat msg │  │   tiếp tục   │
│ - Coins earned│  │ - Retry btn │  │              │
│ - Next level │  │ - Menu btn   │  │              │
│ - Rewards    │  └──────────────┘  └──────────────┘
└──────┬───────┘
       │
       │ (Next/Menu)
       ▼
┌──────────────┐
│  MAP SELECT  │ (quay lại)
└──────────────┘
```

---

## 8. Bản Đồ System Dependencies

### 8.1 Complete Dependency Graph

```
┌─────────────────────────────────────────────────────────────────┐
│                         GAMEMANAGER                             │
│                    (Core Coordinator)                           │
└───────────────────────────┬─────────────────────────────────────┘
                            │
        ┌───────────────────┼───────────────────┬────────────────┐
        │                   │                   │                │
        ▼                   ▼                   ▼                ▼
┌───────────────┐  ┌────────────────┐ ┌────────────────┐ ┌───────────┐
│ GLOBALVALUE   │  │ SOUNDMANAGER   │ │ FLOATINGTEXT   │ │ ADSMANAGER│
│ (Data Storage)│  │ (Audio)        │ │ MANAGER (UI)   │ │ (Ads)     │
└───────┬───────┘  └────────┬───────┘ └────────┬───────┘ └───────────┘
        │                   │                   │
        │                   │                   │
        ▼                   ▼                   ▼
   PlayerPrefs          AudioSource        ObjectPool

──────────────────────────────────────────────────────────────────────

         PLAYER SYSTEM                    ENEMY SYSTEM
┌───────────────────────────┐    ┌───────────────────────────┐
│ Player_Archer             │    │ SmartEnemyGrounded        │
│ (kế thừa Enemy)           │    │ (kế thừa Enemy)           │
└───────────┬───────────────┘    └───────────┬───────────────┘
            │                                 │
            │ phụ thuộc                       │ phụ thuộc
            │                                 │
     ┌──────┴──────────┬──────────┬──────────┴──────────┬─────────┐
     │                 │          │                     │         │
     ▼                 ▼          ▼                     ▼         ▼
┌─────────┐  ┌──────────────┐ ┌─────────┐  ┌─────────────────┐ ┌──────┐
│Controller│ │CheckTarget   │ │ Arrow   │  │ EnemyMelee     │ │Enemy │
│2D        │ │Helper        │ │Projectile│ │ Attack         │ │Range │
└─────────┘  └──────────────┘ └─────────┘  └─────────────────┘ │Attack│
     │                            │                │              └──────┘
     │                            │                │
     ▼                            ▼                ▼
RaycastController          ICanTakeDamage    CheckTargetHelper

──────────────────────────────────────────────────────────────────────

            UI SYSTEM                     MANAGERS
┌───────────────────────────┐    ┌───────────────────────────┐
│ MenuManager               │    │ LevelEnemyManager         │
└───────────┬───────────────┘    └───────────┬───────────────┘
            │                                 │
            │ quản lý                         │ quản lý
            │                                 │
     ┌──────┴──────────┬──────────┬──────────┴──────────┬─────────┐
     │                 │          │                     │         │
     ▼                 ▼          ▼                     ▼         ▼
┌─────────┐  ┌──────────────┐ ┌─────────┐  ┌─────────────────┐ ┌──────┐
│ Victory │  │ GameOver UI │ │ Pause   │  │ EnemyWave       │ │Enemy │
│ Screen  │  │             │ │ Menu    │  │ (config)        │ │Spawn │
└─────────┘  └──────────────┘ └─────────┘  └─────────────────┘ │(data)│
                                                                 └──────┘

──────────────────────────────────────────────────────────────────────

         HELPER SYSTEMS
┌─────────────────────┐
│ AnimationHelper     │ ← Dùng bởi Enemy, Player cho animation lengths
├─────────────────────┤
│ SpawnItemHelper     │ ← Dùng để spawning pickups
├─────────────────────┤
│ WeaponEffect        │ ← Data cho poison, burn, freeze, shock
├─────────────────────┤
│ AutoDestroy         │ ← Tự động destroy objects sau thời gian
└─────────────────────┘
```

### 8.2 Critical Dependencies

**Player_Archer phụ thuộc vào:**
```
Player_Archer.cs
├─ Enemy.cs (base class - KẾ THỪA)
│   ├─ ICanTakeDamage (interface)
│   ├─ IListener (interface)
│   ├─ CheckTargetHelper (target detection)
│   ├─ Animator (animations)
│   ├─ HealthBarEnemyNew (health display)
│   └─ FloatingTextManager (damage numbers)
│
├─ Controller2D.cs (movement)
│   └─ RaycastController.cs (collision detection)
│
├─ ArrowProjectile.cs (shooting)
│   ├─ Projectile.cs (base class)
│   └─ WeaponEffect.cs (arrow effects)
│
├─ CheckTargetHelper.cs (enemy detection)
├─ UpgradedCharacterParameter.cs (stats)
│   └─ PlayerPrefs (data storage)
│
├─ GameManager.cs (game state, listeners)
├─ SoundManager.cs (sound effects)
└─ GlobalValue.cs (player data)
```

**Enemy phụ thuộc vào:**
```
Enemy.cs (base class)
├─ ICanTakeDamage (interface - phải implement)
├─ IListener (interface - phải implement)
│
├─ CheckTargetHelper.cs (target detection)
├─ Animator (animations)
├─ HealthBarEnemyNew (health display)
│
├─ FloatingTextManager.cs (damage numbers)
├─ SoundManager.cs (sound effects)
├─ GameManager.cs (game state, register/remove)
│
├─ GiveCoinWhenDie.cs (optional - rewards)
├─ WeaponEffect.cs (effect data)
└─ UpgradedCharacterParameter.cs (enemy stats)
```

**GameManager phụ thuộc vào:**
```
GameManager.cs
├─ IListener (interface - broadcasts to)
│   ├─ Player_Archer (listener)
│   ├─ Enemy (listener)
│   ├─ LevelEnemyManager (listener)
│   ├─ MenuManager (listener)
│   └─ UI components (listeners)
│
├─ GameMode.cs (game mode data)
├─ GlobalValue.cs (player progress)
├─ AdsManager.cs (ad system)
└─ SoundManager.cs (audio control)
```

---

## 9. Tham Khảo Các Class Chính

### 9.1 Critical Classes Quick Reference

| Class | Type | Mục Đích | Access Pattern |
|-------|------|---------|----------------|
| `GameManager` | Singleton Manager | Game state control, listener coordinator | `GameManager.Instance` |
| `Enemy` | Base Class | Tất cả enemy/player health, damage, effects | Kế thừa bởi enemies/player |
| `Player_Archer` | Player Controller | Player character, auto-shooting | Direct reference hoặc FindObjectOfType |
| `Controller2D` | Physics Controller | Custom 2D movement với raycasts | GetComponent trên character |
| `LevelEnemyManager` | Manager | Wave spawning system | Direct reference hoặc FindObjectOfType |
| `MenuManager` | UI Controller | Menu navigation, UI state | Direct reference |
| `SoundManager` | Singleton Manager | Audio playback | `SoundManager.Instance` |
| `GlobalValue` | Static Data Store | Player progress, coins, unlocks | `GlobalValue.Coin`, `GlobalValue.LevelPass` |
| `FloatingTextManager` | Singleton Manager | Damage number pooling | `FloatingTextManager.Instance` |
| `IListener` | Interface | Game event listener | Implement trong classes cần game events |
| `ICanTakeDamage` | Interface | Damage receiver | Implement trong damageable objects |

### 9.2 Enums Reference

**Game States:**
```csharp
// GameManager.GameState
Menu       // Main menu, không chơi
Playing    // Gameplay active
GameOver   // Thua
Success    // Thắng
Pause      // Game tạm dừng
```

**Enemy States:**
```csharp
// ENEMYSTATE
SPAWNING   // Đang xuất hiện (spawn animation)
IDLE       // Đứng yên
WALK       // Di chuyển về target
ATTACK     // Đang tấn công
HIT        // Vừa nhận damage
DEATH      // Chết (death animation)
```

**Enemy Attack Types:**
```csharp
// ATTACKTYPE
RANGE      // Ranged attack (bắn projectile)
MELEE      // Melee attack (cận chiến)
THROW      // Throwing attack
NONE       // Không attack (passive enemy)
```

**Weapon Effects:**
```csharp
// ENEMYEFFECT
NONE       // Không effect
BURNING    // Damage theo thời gian (lửa)
FREEZE     // Làm chậm/immobilize
SHOKING    // Stun (không thể di chuyển)
POISON     // Damage theo thời gian (độc)
EXPLOSION  // Nổ khi chết
```

**Body Parts (cho critical hits):**
```csharp
// BODYPART
NONE       // Hit bình thường
HEAD       // Headshot (có thể damage nhiều hơn)
BODY       // Body hit
ARM        // Arm hit
LEG        // Leg hit
```

---

## 10. Tổ Chức Namespace

### 10.1 RGame Namespace

Tất cả game scripts được wrap trong `RGame` namespace:

```csharp
using UnityEngine;

namespace RGame  // Tất cả game code dùng namespace này
{
    public class GameManager : MonoBehaviour
    {
        // ...
    }
}
```

**Tại Sao Dùng Namespace?**
- ✅ **Prevent Name Conflicts:** Nếu bạn import asset có class tên "Enemy", nó sẽ không conflict với RGame.Enemy
- ✅ **Organization:** Nhóm tất cả game code lại
- ✅ **Professional Practice:** Industry standard cho projects lớn hơn
- ✅ **Import Clarity:** Scripts khác biết classes đến từ đâu

**Dùng RGame Classes:**
```csharp
// Option 1: Import namespace
using RGame;

public class MyScript : MonoBehaviour
{
    void Start()
    {
        // Có thể dùng RGame classes trực tiếp
        GameManager gm = GameManager.Instance;
        Enemy enemy = GetComponent<Enemy>();
    }
}

// Option 2: Fully qualify (không import)
public class MyScript : MonoBehaviour
{
    void Start()
    {
        // Specify namespace mỗi lần
        RGame.GameManager gm = RGame.GameManager.Instance;
        RGame.Enemy enemy = GetComponent<RGame.Enemy>();
    }
}
```

**RGame Namespace Bao Gồm:**
- Tất cả Managers (GameManager, LevelEnemyManager, etc.)
- Tất cả AI (Enemy, SmartEnemyGrounded, etc.)
- Tất cả Controllers (Controller2D, Projectile, etc.)
- Tất cả Helpers
- Player classes
- UI classes

**External Classes (không trong RGame):**
- Unity built-in classes (MonoBehaviour, GameObject, etc.)
- C# standard library (List, Dictionary, etc.)
- Plugins (AdMob, Unity IAP, etc.)

---

## 11. Performance Considerations

### 11.1 Optimization Strategies Used

**Custom Physics (Không Dùng Rigidbody2D):**
- ✅ Kiểm soát performance tốt hơn
- ✅ Collision detection chính xác
- ✅ Không có physics interactions không mong muốn

**Object Pooling (Floating Text):**
- ✅ Giảm Instantiate/Destroy calls
- ✅ Ngăn garbage collection spikes

**Coroutines cho Delays:**
- ✅ Non-blocking delays
- ✅ Tốt hơn Update() checks

**Caching Component References:**
```csharp
// TỐT - Cache trong Awake/Start
private Rigidbody2D rb;
void Awake() { rb = GetComponent<Rigidbody2D>(); }
void Update() { rb.velocity = ...; }

// TỆ - GetComponent mỗi frame
void Update() { GetComponent<Rigidbody2D>().velocity = ...; }
```

**Target Frame Rate:**
```csharp
void Awake()
{
    Application.targetFrameRate = 60;  // Mobile optimization
}
```

---

## 12. Tiếp Theo Là Gì?

Giờ bạn đã hiểu architecture, hãy đi sâu vào các systems cụ thể:

**Thứ Tự Đọc Đề Xuất:**
1. ✅ **`02_He_Thong_Player_Day_Du.md`** - Hiểu Player_Archer chi tiết
2. ✅ **`03_He_Thong_Enemy_Day_Du.md`** - Deep dive vào Enemy AI
3. ✅ **`04_He_Thong_UI_Day_Du.md`** - UI implementation
4. ✅ **`05_Cac_Manager_Day_Du.md`** - Manager classes giải thích
5. ✅ **`10_Huong_Dan_Thuc_Hanh.md`** - Practical modification guides

**Điểm Chính Cần Nhớ:**
- Game dùng **Singleton + Listener pattern** để coordination
- **Player kế thừa từ Enemy** (unconventional nhưng thực tế)
- **Custom 2D physics** dùng raycasts (không phải Rigidbody2D)
- **State machines** điều khiển enemy và game flow
- **Một scene** với dynamic level loading
- **Namespace RGame** wrap tất cả game code

---

**Bạn giờ đã có hiểu biết toàn diện về project architecture!**

**Tài Liệu Tiếp Theo:** → `02_He_Thong_Player_Day_Du.md`
