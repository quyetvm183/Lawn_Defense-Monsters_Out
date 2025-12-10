# Thuộc Tính Nhân Vật và Hệ Thống Nâng Cấp

**Hướng Dẫn Hoàn Chỉnh Về UpgradedCharacterParameter**

---

## Mục Lục

1. [Tổng Quan](#tổng-quan)
2. [Kiến Thức Unity Cần Thiết](#kiến-thức-unity-cần-thiết)
3. [Hệ Thống UpgradedCharacterParameter](#hệ-thống-upgradedcharacterparameter)
4. [Hiểu Về Code](#hiểu-về-code)
5. [Cách Nâng Cấp Hoạt Động](#cách-nâng-cấp-hoạt-động)
6. [Tạo Nhân Vật Mới](#tạo-nhân-vật-mới)
7. [Ví Dụ Thực Tế](#ví-dụ-thực-tế)
8. [Sơ Đồ Luồng Trực Quan](#sơ-đồ-luồng-trực-quan)
9. [Khắc Phục Sự Cố](#khắc-phục-sự-cố)
10. [Tài Liệu Tham Khảo Chéo](#tài-liệu-tham-khảo-chéo)

---

## Tổng Quan

**Mục Đích**: Tài liệu này giải thích cách lưu trữ thuộc tính nhân vật, cách hệ thống nâng cấp hoạt động, và cách tạo nhân vật có thể chơi mới trong game.

**Bạn Sẽ Học Được**:
- Cách lưu trữ và truy xuất các chỉ số nhân vật (máu, sát thương, tỷ lệ chí mạng)
- Cách hệ thống nâng cấp sử dụng PlayerPrefs để lưu dữ liệu lâu dài
- Cách tạo nhân vật mới với chỉ số và kỹ năng tùy chỉnh
- Cách debug và sửa các vấn đề thường gặp liên quan đến nhân vật

**File Chính**: `Assets/_MonstersOut/Scripts/Player/UpgradedCharacterParameter.cs`

**Thời Gian Đọc**: 15-20 phút
**Độ Khó**: Trung bình

---

## Kiến Thức Unity Cần Thiết

### PlayerPrefs Là Gì?

**PlayerPrefs** là hệ thống built-in của Unity để lưu dữ liệu đơn giản giữa các phiên game.

**Ví Dụ Thực Tế**: Nghĩ về PlayerPrefs như browser cookies - những mẩu dữ liệu nhỏ tồn tại ngay cả sau khi bạn đóng ứng dụng.

**Cách hoạt động**:
```csharp
// Save data
PlayerPrefs.SetInt("Score", 100);           // Save integer
PlayerPrefs.SetFloat("Volume", 0.8f);       // Save decimal
PlayerPrefs.SetString("PlayerName", "Bob"); // Save text

// Load data (with default value if key doesn't exist)
int score = PlayerPrefs.GetInt("Score", 0);           // Returns 100, or 0 if not found
float volume = PlayerPrefs.GetFloat("Volume", 1.0f);  // Returns 0.8, or 1.0 if not found
string name = PlayerPrefs.GetString("PlayerName", ""); // Returns "Bob", or "" if not found

// Delete data
PlayerPrefs.DeleteKey("Score");  // Remove specific key
PlayerPrefs.DeleteAll();         // Remove ALL saved data (dangerous!)
```

**Dữ liệu lưu ở đâu?**
- **Windows**: Registry tại `HKCU\Software\[CompanyName]\[ProductName]`
- **Mac**: `~/Library/Preferences/com.[CompanyName].[ProductName].plist`
- **Linux**: `~/.config/unity3d/[CompanyName]/[ProductName]/prefs`

**Giới Hạn**:
- Chỉ lưu các kiểu đơn giản (int, float, string)
- Không mã hóa - đừng lưu dữ liệu nhạy cảm
- Kích thước giới hạn - dùng JSON hoặc database cho dữ liệu lớn

### [System.Serializable] Là Gì?

Attribute này báo cho Unity hiển thị class trong Inspector.

```csharp
[System.Serializable]
public class UpgradeStep
{
    public int price;        // ← Các fields này sẽ xuất hiện
    public int healthStep;   //   trong Unity Inspector
}
```

**Không có [System.Serializable]**: Class vẫn hoạt động trong code nhưng sẽ không xuất hiện trong Inspector để chỉnh sửa.

### Properties (get/set) Là Gì?

**Properties** cung cấp quyền truy cập có kiểm soát vào dữ liệu private.

```csharp
// Without property (direct access)
public int health = 100;
// Anyone can change: health = -999; ← No validation!

// With property (controlled access)
private int _health = 100;
public int Health
{
    get { return _health; }
    set { _health = Mathf.Max(0, value); } // ← Prevents negative health
}
```

Trong project này, properties được dùng để tự động save/load từ PlayerPrefs:

```csharp
public int UpgradeHealth
{
    get { return PlayerPrefs.GetInt(ID + "upgradeHealth", defaultHealth); }
    set { PlayerPrefs.SetInt(ID + "upgradeHealth", value); }
}

// Usage:
int currentHealth = UpgradeHealth;  // ← Automatically loads from PlayerPrefs
UpgradeHealth = 150;                // ← Automatically saves to PlayerPrefs
```

---

## Hệ Thống UpgradedCharacterParameter

### Nó Là Gì?

**UpgradedCharacterParameter** là component gắn vào mỗi nhân vật có thể chơi để:
1. Lưu trữ chỉ số cơ bản (máu, sát thương, tỷ lệ chí mạng)
2. Định nghĩa cách nhân vật có thể được nâng cấp
3. Save/load tiến trình nâng cấp bằng PlayerPrefs
4. Xác định khả năng nhân vật (Melee, Range, hoặc Healer)

### Các Khái Niệm Cốt Lõi

**1. Character ID** - Định danh duy nhất cho PlayerPrefs keys
```csharp
public string ID = "archer_01"; // ← PHẢI là duy nhất cho mỗi nhân vật!
```

Mỗi nhân vật cần một ID duy nhất để tránh xung đột. Nếu hai nhân vật dùng chung ID, dữ liệu nâng cấp của họ sẽ ghi đè lẫn nhau.

**2. Ability Types** - Xác định hành vi nhân vật
```csharp
public enum Abitity { Melee, Range, Healer }
public Abitity playerAbility; // Set in Inspector
```
- **Melee**: Chiến binh cận chiến (kiếm, búa)
- **Range**: Tấn công tầm xa (cung, súng)
- **Healer**: Nhân vật hỗ trợ (hồi máu)

**3. Upgrade Steps** - Định nghĩa tiến trình nâng cấp
```csharp
public UpgradeStep[] UpgradeSteps;
```

Mỗi bước nâng cấp định nghĩa:
- **price**: Chi phí bằng tiền trong game
- **healthStep**: Lượng máu tăng thêm
- **meleeDamageStep**: Lượng sát thương cận chiến tăng thêm
- **rangeDamageStep**: Lượng sát thương tầm xa tăng thêm
- **criticalStep**: Lượng tỷ lệ chí mạng tăng thêm

**4. Default Stats** - Giá trị khởi đầu trước khi nâng cấp
```csharp
public int defaultHealth = 100;
public int meleeDamage = 100;
public int rangeDamage = 100;
public int criticalDamagePercent = 10;
```

---

## Hiểu Về Code

Hãy phân tích file **UpgradedCharacterParameter.cs** từng dòng.

### Cấu Trúc Class

```csharp
namespace RGame
{
    [System.Serializable]
    public class UpgradeStep
    {
        public int price;              // Cost to buy this upgrade
        public int healthStep;         // Health increase
        public int meleeDamageStep;    // Melee damage increase
        public int rangeDamageStep;    // Range damage increase
        public int criticalStep;       // Critical chance increase
    }
```

**Tại sao tách class riêng?** Điều này cho phép bạn tạo array của các upgrades trong Inspector:

```
UpgradeSteps:
  [0] price: 100, healthStep: 10, meleeDamageStep: 5
  [1] price: 200, healthStep: 15, meleeDamageStep: 8
  [2] price: 400, healthStep: 20, meleeDamageStep: 12
```

### Định Danh Nhân Vật

```csharp
public class UpgradedCharacterParameter : MonoBehaviour
{
    public string ID = "unique ID";        // ← PHẢI là duy nhất cho mỗi nhân vật
    public int unlockAtLevel = 1;          // Level requirement to unlock
    public Abitity playerAbility;          // Melee, Range, or Healer
```

**Ví Dụ IDs**:
- `"archer_01"` - Nhân vật cung thủ đầu tiên
- `"knight_heavy"` - Nhân vật hiệp sĩ nặng
- `"mage_fire"` - Nhân vật pháp sư lửa

### Weapon Effects

```csharp
[Header("EFFECT")]
public WeaponEffect weaponEffect;          // Optional effect (fire, ice, poison, etc.)
```

Component `WeaponEffect` định nghĩa các hiệu ứng đặc biệt xảy ra khi nhân vật này tấn công (cháy, đóng băng, v.v.). Xem `03_Enemy_System_Complete.md` để biết chi tiết về effects.

### Upgrade Steps Array

```csharp
[Space]
public UpgradeStep[] UpgradeSteps;         // Upgrade progression
```

**Ví Dụ Trong Inspector**:
```
UpgradeSteps (Size: 5)
  Element 0
    ├─ price: 100
    ├─ healthStep: 20
    ├─ meleeDamageStep: 10
    ├─ rangeDamageStep: 0
    └─ criticalStep: 2
  Element 1
    ├─ price: 250
    ├─ healthStep: 30
    └─ ...
```

### Melee Attack Properties

```csharp
[Header("MELEE ATTACK")]
public int maxTargetPerHit = 1;   // How many enemies can be hit at once
```

**Ví Dụ Giá Trị**:
- `1` = Đánh một kẻ địch (single-target)
- `3` = Đánh ba kẻ địch (cleave attack)
- `10` = Đánh tất cả kẻ địch gần (area attack)

### Default Stats

```csharp
[Header("Default")]
public int defaultHealth = 100;
public int meleeDamage = 100;
public int rangeDamage = 100;
[Range(1, 100)]
public int criticalDamagePercent = 10;  // 10% chance for critical hit
```

Đây là **chỉ số cơ bản** trước mọi nâng cấp. Khi người chơi nâng cấp nhân vật, các giá trị này được **cộng thêm**, không thay thế.

### Current Upgrade Property

```csharp
public int CurrentUpgrade
{
    get
    {
        // Load current upgrade level from PlayerPrefs
        int current = PlayerPrefs.GetInt(ID + "upgradeHealth" + "Current", 0);

        // If upgrade level exceeds available steps, mark as maxed out
        if (current >= UpgradeSteps.Length)
            current = -1;  // -1 means "max level reached"

        return current;
    }
    set
    {
        // Save current upgrade level to PlayerPrefs
        PlayerPrefs.SetInt(ID + "upgradeHealth" + "Current", value);
    }
}
```

**Cách hoạt động**:
```csharp
// First time playing:
int level = CurrentUpgrade; // Returns 0 (no upgrades yet)

// After buying 2 upgrades:
CurrentUpgrade = 2;         // Saves 2 to PlayerPrefs
int level = CurrentUpgrade; // Returns 2

// After buying all 5 upgrades:
CurrentUpgrade = 5;         // Saves 5 to PlayerPrefs
int level = CurrentUpgrade; // Returns -1 (maxed out)
```

**Tại sao -1 cho max level?** Đây là flag báo cho UI hiển thị "MAX" thay vì giá nâng cấp tiếp theo.

### Upgrade Character Method

```csharp
public void UpgradeCharacter(bool health, bool melee, bool range, bool crit)
{
    // Can't upgrade if already at max level
    if (CurrentUpgrade == -1)
        return;

    // Get the current upgrade step
    UpgradeStep step = UpgradeSteps[CurrentUpgrade];

    // Upgrade each stat if requested
    if (health)
        UpgradeHealth += step.healthStep;

    if (melee)
        UpgradeMeleeDamage += step.meleeDamageStep;

    if (range)
        UpgradeRangeDamage += step.rangeDamageStep;

    if (crit)
        UpgradeCriticalDamage += step.criticalStep;

    // Move to next upgrade level
    CurrentUpgrade++;
}
```

**Ví Dụ Sử Dụng**:
```csharp
UpgradedCharacterParameter archer = GetComponent<UpgradedCharacterParameter>();

// Player buys "Upgrade All" button
archer.UpgradeCharacter(
    health: true,   // Increase health
    melee: false,   // Don't increase melee (archer doesn't use melee)
    range: true,    // Increase ranged damage
    crit: true      // Increase critical chance
);
```

### Upgrade Properties (PlayerPrefs Wrappers)

```csharp
public int UpgradeHealth
{
    get { return PlayerPrefs.GetInt(ID + "upgradeHealth", defaultHealth); }
    set { PlayerPrefs.SetInt(ID + "upgradeHealth", value); }
}

public int UpgradeMeleeDamage
{
    get { return PlayerPrefs.GetInt(ID + "UpgradedMeleeDamage", meleeDamage); }
    set { PlayerPrefs.SetInt(ID + "UpgradedMeleeDamage", value); }
}

public int UpgradeRangeDamage
{
    get { return PlayerPrefs.GetInt(ID + "UpgradeRangeDamage", rangeDamage); }
    set { PlayerPrefs.SetInt(ID + "UpgradeRangeDamage", value); }
}

public int UpgradeCriticalDamage
{
    get { return PlayerPrefs.GetInt(ID + "UpgradeCriticalDamage", criticalDamagePercent); }
    set { PlayerPrefs.SetInt(ID + "UpgradeCriticalDamage", value); }
}
```

**Cách chúng hoạt động cùng nhau**:

```csharp
// Initial state (no upgrades)
archer.ID = "archer_01";
archer.defaultHealth = 100;
int health = archer.UpgradeHealth; // Returns 100 (default value)

// After first upgrade (+20 health)
archer.UpgradeHealth += 20;        // Saves 120 to PlayerPrefs
int health = archer.UpgradeHealth; // Returns 120

// Next game session (loads from PlayerPrefs)
int health = archer.UpgradeHealth; // Returns 120 (persisted!)
```

**PlayerPrefs Keys Được Tạo** (cho ID = "archer_01"):
- `"archer_01upgradeHealthCurrent"` → Current upgrade level
- `"archer_01upgradeHealth"` → Total health after upgrades
- `"archer_01UpgradedMeleeDamage"` → Total melee damage
- `"archer_01UpgradeRangeDamage"` → Total range damage
- `"archer_01UpgradeCriticalDamage"` → Total critical chance

---

## Cách Nâng Cấp Hoạt Động

### Sơ Đồ Luồng Nâng Cấp

```
Player Clicks "Upgrade" Button
         |
         v
[Check if max level?]
    |            |
   Yes           No
    |            |
    v            v
[Return]    [Get current UpgradeStep]
                |
                v
         [Apply stat increases]
                |
                v
         [Save to PlayerPrefs]
                |
                v
         [Increment CurrentUpgrade]
                |
                v
         [Update UI to show new stats]
```

### Ví Dụ Tiến Trình Nâng Cấp

**Thiết Lập Nhân Vật**:
```
ID: "knight_01"
defaultHealth: 100
meleeDamage: 50
rangeDamage: 0

UpgradeSteps:
  [0] price: 100, healthStep: 20, meleeDamageStep: 10
  [1] price: 200, healthStep: 30, meleeDamageStep: 15
  [2] price: 400, healthStep: 40, meleeDamageStep: 20
```

**Timeline Nâng Cấp**:

**Trạng Thái Ban Đầu** (Chưa nâng cấp):
```
CurrentUpgrade: 0
UpgradeHealth: 100 (default)
UpgradeMeleeDamage: 50 (default)
```

**Sau Nâng Cấp #1** (Chi phí: 100 coins):
```
CurrentUpgrade: 1
UpgradeHealth: 120 (100 + 20)
UpgradeMeleeDamage: 60 (50 + 10)
```

**Sau Nâng Cấp #2** (Chi phí: 200 coins):
```
CurrentUpgrade: 2
UpgradeHealth: 150 (120 + 30)
UpgradeMeleeDamage: 75 (60 + 15)
```

**Sau Nâng Cấp #3** (Chi phí: 400 coins):
```
CurrentUpgrade: -1 (MAX LEVEL)
UpgradeHealth: 190 (150 + 40)
UpgradeMeleeDamage: 95 (75 + 20)
```

### Ví Dụ Code: Kiểm Tra Chi Phí Nâng Cấp

```csharp
void DisplayUpgradeCost()
{
    UpgradedCharacterParameter character = GetComponent<UpgradedCharacterParameter>();

    if (character.CurrentUpgrade == -1)
    {
        Debug.Log("Character is at MAX level!");
        return;
    }

    int nextUpgradeIndex = character.CurrentUpgrade;
    UpgradeStep nextStep = character.UpgradeSteps[nextUpgradeIndex];

    Debug.Log($"Next upgrade costs: {nextStep.price} coins");
    Debug.Log($"Health will increase by: {nextStep.healthStep}");
    Debug.Log($"Melee damage will increase by: {nextStep.meleeDamageStep}");
}
```

---

## Tạo Nhân Vật Mới

Làm theo các bước sau để tạo một nhân vật có thể chơi hoàn toàn mới.

### Bước 1: Tạo Character GameObject

1. **Tạo Empty GameObject**:
   - Right-click trong Hierarchy → Create Empty
   - Đặt tên `Player_Mage` (hoặc tên nhân vật của bạn)

2. **Thêm Visual Components**:
   ```
   Player_Mage
   ├─ SpriteRenderer (add the character sprite)
   ├─ Animator (add animation controller)
   ├─ CircleCollider2D (for collision detection)
   └─ Rigidbody2D (if using physics)
   ```

3. **Đặt Layer và Tag**:
   - Layer: `Player`
   - Tag: `Player`

### Bước 2: Thêm UpgradedCharacterParameter Component

1. Chọn `Player_Mage` trong Hierarchy
2. Click **Add Component** trong Inspector
3. Tìm kiếm `UpgradedCharacterParameter`
4. Click để thêm

### Bước 3: Cấu Hình Character Properties

**Đặt Unique ID**:
```
ID: "mage_fire_01"  ← PHẢI là duy nhất!
```

**⚠️ Cảnh Báo**: Nếu bạn copy-paste một nhân vật, hãy chắc chắn thay đổi ID! Nếu không, cả hai nhân vật sẽ chia sẻ dữ liệu nâng cấp.

**Đặt Unlock Level**:
```
unlockAtLevel: 5  ← Player phải hoàn thành level 5 để mở khóa nhân vật này
```

**Đặt Ability Type**:
```
playerAbility: Range  ← Đây là nhân vật tầm xa
```

**Cấu Hình Default Stats**:
```
defaultHealth: 80        ← Máu thấp hơn (mage dễ bị tổn thương)
meleeDamage: 10          ← Tấn công cận chiến yếu
rangeDamage: 150         ← Tấn công phép thuật mạnh
criticalDamagePercent: 15 ← 15% tỷ lệ chí mạng
maxTargetPerHit: 1       ← Phép đơn mục tiêu
```

**Thêm Weapon Effect** (tùy chọn):
- Kéo một prefab `WeaponEffect` vào slot `weaponEffect`
- Hoặc để trống nếu không có hiệu ứng đặc biệt

### Bước 4: Định Nghĩa Upgrade Steps

Click nút **+** dưới `UpgradeSteps` để thêm upgrades.

**Ví Dụ Cấu Hình**:
```
UpgradeSteps (Size: 4)

  Element 0 (Upgrade Level 1)
    price: 150
    healthStep: 15
    meleeDamageStep: 5
    rangeDamageStep: 25
    criticalStep: 3

  Element 1 (Upgrade Level 2)
    price: 300
    healthStep: 20
    meleeDamageStep: 5
    rangeDamageStep: 35
    criticalStep: 4

  Element 2 (Upgrade Level 3)
    price: 600
    healthStep: 25
    meleeDamageStep: 10
    rangeDamageStep: 50
    criticalStep: 5

  Element 3 (Upgrade Level 4 - Final)
    price: 1200
    healthStep: 30
    meleeDamageStep: 10
    rangeDamageStep: 75
    criticalStep: 8
```

**Chỉ Số Cuối Sau Tất Cả Nâng Cấp**:
```
Total Health: 80 + 15 + 20 + 25 + 30 = 170
Total Melee: 10 + 5 + 5 + 10 + 10 = 40
Total Range: 150 + 25 + 35 + 50 + 75 = 335
Total Crit: 15 + 3 + 4 + 5 + 8 = 35%
```

### Bước 5: Tạo Prefab

1. Tạo folder: `Assets/Resources/Characters/` (nếu chưa tồn tại)
2. Kéo `Player_Mage` từ Hierarchy vào folder này
3. Bạn giờ có một prefab có thể tái sử dụng
4. Xóa instance từ Hierarchy (prefab đã được lưu)

### Bước 6: Tích Hợp Với Character Selection

**Option A: Thêm vào Character Select Scene**

Nếu bạn có menu chọn nhân vật:
```csharp
public class CharacterSelectionUI : MonoBehaviour
{
    public GameObject[] characterPrefabs;  // Add Player_Mage to this array

    public void SelectCharacter(int index)
    {
        GameObject selectedCharacter = characterPrefabs[index];
        CharacterManager.Instance.SpawnCharacter(selectedCharacter);
    }
}
```

**Option B: Spawn Trực Tiếp Trong Game**

```csharp
public class GameController : MonoBehaviour
{
    public GameObject magePrefab;

    void Start()
    {
        // Spawn the mage at the character spawn point
        CharacterManager.Instance.SpawnCharacter(magePrefab);
    }
}
```

### Bước 7: Test Nhân Vật

1. Vào Play Mode
2. Kiểm tra nhân vật xuất hiện đúng
3. Test di chuyển cơ bản và tấn công
4. Debug.Log các chỉ số:

```csharp
void Start()
{
    UpgradedCharacterParameter param = GetComponent<UpgradedCharacterParameter>();
    Debug.Log($"Character: {param.ID}");
    Debug.Log($"Health: {param.UpgradeHealth}");
    Debug.Log($"Range Damage: {param.UpgradeRangeDamage}");
    Debug.Log($"Critical: {param.UpgradeCriticalDamage}%");
    Debug.Log($"Current Upgrade: {param.CurrentUpgrade}");
}
```

**Kết Quả Mong Đợi** (lần đầu):
```
Character: mage_fire_01
Health: 80
Range Damage: 150
Critical: 15%
Current Upgrade: 0
```

---

## Ví Dụ Thực Tế

### Ví Dụ 1: Hiển Thị Character Stats Trong UI

```csharp
using UnityEngine;
using UnityEngine.UI;
using RGame;

public class CharacterStatsUI : MonoBehaviour
{
    [Header("References")]
    public UpgradedCharacterParameter character;

    [Header("UI Elements")]
    public Text healthText;
    public Text damageText;
    public Text critText;
    public Text upgradeText;

    void Start()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        // Display current stats
        healthText.text = $"Health: {character.UpgradeHealth}";

        // Show appropriate damage type
        if (character.playerAbility == Abitity.Melee)
        {
            damageText.text = $"Melee: {character.UpgradeMeleeDamage}";
        }
        else if (character.playerAbility == Abitity.Range)
        {
            damageText.text = $"Range: {character.UpgradeRangeDamage}";
        }

        critText.text = $"Critical: {character.UpgradeCriticalDamage}%";

        // Show upgrade level
        if (character.CurrentUpgrade == -1)
        {
            upgradeText.text = "Level: MAX";
        }
        else
        {
            upgradeText.text = $"Level: {character.CurrentUpgrade + 1}/{character.UpgradeSteps.Length}";
        }
    }
}
```

### Ví Dụ 2: Upgrade Shop System

```csharp
using UnityEngine;
using UnityEngine.UI;
using RGame;

public class UpgradeShop : MonoBehaviour
{
    [Header("References")]
    public UpgradedCharacterParameter character;

    [Header("UI")]
    public Button upgradeButton;
    public Text costText;
    public Text statsPreviewText;

    private int playerCoins = 1000; // Example currency

    void Start()
    {
        UpdateShopUI();
    }

    void UpdateShopUI()
    {
        // Check if max level
        if (character.CurrentUpgrade == -1)
        {
            upgradeButton.interactable = false;
            costText.text = "MAX LEVEL";
            statsPreviewText.text = "Character fully upgraded!";
            return;
        }

        // Get next upgrade step
        UpgradeStep nextUpgrade = character.UpgradeSteps[character.CurrentUpgrade];

        // Display cost
        costText.text = $"Cost: {nextUpgrade.price} coins";

        // Show what stats will increase
        statsPreviewText.text = $"Next Upgrade:\n" +
            $"+{nextUpgrade.healthStep} Health\n" +
            $"+{nextUpgrade.meleeDamageStep} Melee\n" +
            $"+{nextUpgrade.rangeDamageStep} Range\n" +
            $"+{nextUpgrade.criticalStep}% Critical";

        // Enable/disable button based on affordability
        upgradeButton.interactable = (playerCoins >= nextUpgrade.price);
    }

    public void OnUpgradeButtonClicked()
    {
        if (character.CurrentUpgrade == -1)
            return; // Already max level

        UpgradeStep nextUpgrade = character.UpgradeSteps[character.CurrentUpgrade];

        // Check if player can afford it
        if (playerCoins < nextUpgrade.price)
        {
            Debug.Log("Not enough coins!");
            return;
        }

        // Deduct cost
        playerCoins -= nextUpgrade.price;

        // Apply upgrade (upgrade all stats for this example)
        character.UpgradeCharacter(
            health: true,
            melee: true,
            range: true,
            crit: true
        );

        Debug.Log($"Upgraded {character.ID} to level {character.CurrentUpgrade}!");

        // Refresh UI
        UpdateShopUI();
    }
}
```

### Ví Dụ 3: Reset Character Progress (Debug Tool)

```csharp
using UnityEngine;
using RGame;

public class CharacterDebugTools : MonoBehaviour
{
    public UpgradedCharacterParameter character;

    [ContextMenu("Reset Character Progress")]
    void ResetProgress()
    {
        // Delete all PlayerPrefs keys for this character
        PlayerPrefs.DeleteKey(character.ID + "upgradeHealthCurrent");
        PlayerPrefs.DeleteKey(character.ID + "upgradeHealth");
        PlayerPrefs.DeleteKey(character.ID + "UpgradedMeleeDamage");
        PlayerPrefs.DeleteKey(character.ID + "UpgradeRangeDamage");
        PlayerPrefs.DeleteKey(character.ID + "UpgradeCriticalDamage");

        PlayerPrefs.Save(); // Force save

        Debug.Log($"Reset all upgrades for {character.ID}");
    }

    [ContextMenu("Unlock All Upgrades")]
    void UnlockAll()
    {
        // Max out all stats
        foreach (UpgradeStep step in character.UpgradeSteps)
        {
            character.UpgradeCharacter(true, true, true, true);
        }

        Debug.Log($"Maxed out {character.ID}!");
    }

    [ContextMenu("Give Free Upgrade")]
    void GiveFreeUpgrade()
    {
        if (character.CurrentUpgrade == -1)
        {
            Debug.Log("Already at max level!");
            return;
        }

        character.UpgradeCharacter(true, true, true, true);
        Debug.Log($"Gave free upgrade to {character.ID}");
    }

    [ContextMenu("Print Current Stats")]
    void PrintStats()
    {
        Debug.Log($"===== {character.ID} Stats =====");
        Debug.Log($"Upgrade Level: {character.CurrentUpgrade}");
        Debug.Log($"Health: {character.UpgradeHealth}");
        Debug.Log($"Melee: {character.UpgradeMeleeDamage}");
        Debug.Log($"Range: {character.UpgradeRangeDamage}");
        Debug.Log($"Critical: {character.UpgradeCriticalDamage}%");
    }
}
```

**Cách sử dụng**: Gắn script này vào bất kỳ GameObject nào, assign nhân vật, sau đó right-click component header trong Inspector và chọn menu option.

### Ví Dụ 4: Selective Stat Upgrades

Thay vì nâng cấp tất cả chỉ số cùng lúc, cho phép người chơi chọn:

```csharp
public class SelectiveUpgradeShop : MonoBehaviour
{
    public UpgradedCharacterParameter character;

    public void UpgradeHealthOnly()
    {
        if (character.CurrentUpgrade == -1) return;

        // Only upgrade health, not other stats
        character.UpgradeCharacter(
            health: true,
            melee: false,
            range: false,
            crit: false
        );

        // Note: CurrentUpgrade still increments!
        // You might want to modify the system for independent stat upgrades
    }

    public void UpgradeDamageOnly()
    {
        if (character.CurrentUpgrade == -1) return;

        // Choose melee or range based on character type
        bool isMelee = (character.playerAbility == Abitity.Melee);

        character.UpgradeCharacter(
            health: false,
            melee: isMelee,
            range: !isMelee,
            crit: false
        );
    }
}
```

**⚠️ Quan Trọng**: Hệ thống hiện tại tăng `CurrentUpgrade` bất kể bạn nâng cấp chỉ số nào. Nếu muốn nâng cấp độc lập từng chỉ số, bạn cần sửa đổi hệ thống.

---

## Sơ Đồ Luồng Trực Quan

### Character Upgrade Data Flow

```
┌─────────────────────────────────────────────────────────────┐
│                    GAME STARTS                              │
└────────────────┬────────────────────────────────────────────┘
                 │
                 v
┌─────────────────────────────────────────────────────────────┐
│  UpgradedCharacterParameter.Start()                         │
│  ├─ Read PlayerPrefs for character ID                       │
│  └─ If no saved data, use default stats                     │
└────────────────┬────────────────────────────────────────────┘
                 │
                 v
┌─────────────────────────────────────────────────────────────┐
│  DISPLAY STATS IN UI                                        │
│  ├─ Health: UpgradeHealth (loads from PlayerPrefs)          │
│  ├─ Damage: UpgradeMeleeDamage or UpgradeRangeDamage        │
│  └─ Crit: UpgradeCriticalDamage                             │
└────────────────┬────────────────────────────────────────────┘
                 │
                 v
        [Player Clicks Upgrade]
                 │
                 v
┌─────────────────────────────────────────────────────────────┐
│  UpgradeCharacter()                                         │
│  ├─ Check CurrentUpgrade != -1 (not max level)              │
│  ├─ Get UpgradeSteps[CurrentUpgrade]                        │
│  ├─ Add healthStep to UpgradeHealth                         │
│  ├─ Add meleeDamageStep to UpgradeMeleeDamage               │
│  ├─ Add rangeDamageStep to UpgradeRangeDamage               │
│  ├─ Add criticalStep to UpgradeCriticalDamage               │
│  └─ Increment CurrentUpgrade                                │
└────────────────┬────────────────────────────────────────────┘
                 │
                 v
┌─────────────────────────────────────────────────────────────┐
│  AUTOMATIC SAVE (via property setters)                      │
│  ├─ PlayerPrefs.SetInt("archer_01upgradeHealth", 120)       │
│  ├─ PlayerPrefs.SetInt("archer_01UpgradedMeleeDamage", 60)  │
│  └─ PlayerPrefs.SetInt("archer_01upgradeHealthCurrent", 1)  │
└────────────────┬────────────────────────────────────────────┘
                 │
                 v
┌─────────────────────────────────────────────────────────────┐
│  GAME CLOSED                                                │
│  └─ PlayerPrefs data persists on disk                       │
└────────────────┬────────────────────────────────────────────┘
                 │
                 v
┌─────────────────────────────────────────────────────────────┐
│  NEXT GAME SESSION                                          │
│  └─ UpgradeHealth loads 120 from PlayerPrefs (not 100!)     │
└─────────────────────────────────────────────────────────────┘
```

### PlayerPrefs Key Structure

```
For character with ID = "archer_01":

PlayerPrefs Storage:
├─ "archer_01upgradeHealthCurrent" → 2 (current upgrade level)
├─ "archer_01upgradeHealth" → 140 (total health)
├─ "archer_01UpgradedMeleeDamage" → 50 (total melee)
├─ "archer_01UpgradeRangeDamage" → 180 (total range)
└─ "archer_01UpgradeCriticalDamage" → 18 (total crit %)

For character with ID = "knight_01":

PlayerPrefs Storage:
├─ "knight_01upgradeHealthCurrent" → 0 (no upgrades yet)
├─ "knight_01upgradeHealth" → 200 (default health)
├─ "knight_01UpgradedMeleeDamage" → 150 (default melee)
├─ "knight_01UpgradeRangeDamage" → 0 (default range)
└─ "knight_01UpgradeCriticalDamage" → 5 (default crit)
```

### Character Creation Workflow

```
START: Tôi muốn tạo một nhân vật mới
  |
  v
[Create GameObject]
  ├─ Add SpriteRenderer
  ├─ Add Animator
  ├─ Add Collider
  └─ Set Layer/Tag
  |
  v
[Add UpgradedCharacterParameter Component]
  |
  v
[Configure in Inspector]
  ├─ Set UNIQUE ID (e.g., "mage_01")
  ├─ Set unlockAtLevel
  ├─ Choose playerAbility (Melee/Range/Healer)
  ├─ Set defaultHealth, defaultDamage, defaultCrit
  └─ Fill UpgradeSteps array
  |
  v
[Create Prefab]
  └─ Drag to Assets/Resources/Characters/
  |
  v
[Integrate with Game]
  ├─ Add to Character Selection UI
  └─ OR spawn directly in code
  |
  v
[Test in Play Mode]
  ├─ Verify stats display correctly
  ├─ Test upgrades save/load
  └─ Test ability works as expected
  |
  v
DONE: Nhân vật mới đã sẵn sàng!
```

---

## Khắc Phục Sự Cố

### Vấn Đề 1: Upgrades Không Lưu Giữa Các Phiên

**Triệu Chứng**:
- Chỉ số nhân vật reset về mặc định mỗi khi bạn khởi động lại game
- Upgrades hoạt động trong một phiên nhưng không tồn tại lâu dài

**Nguyên Nhân**:
1. PlayerPrefs không được lưu vào ổ đĩa
2. Character ID thay đổi giữa các phiên
3. Platform không hỗ trợ PlayerPrefs

**Giải Pháp**:

**Giải Pháp A: Force Save PlayerPrefs**
```csharp
public void UpgradeCharacter(bool health, bool melee, bool range, bool crit)
{
    // ... existing upgrade code ...

    PlayerPrefs.Save(); // ← Thêm dòng này để force write vào ổ đĩa
}
```

**Giải Pháp B: Xác Minh ID Consistency**
```csharp
void Start()
{
    UpgradedCharacterParameter param = GetComponent<UpgradedCharacterParameter>();
    Debug.Log($"Character ID: {param.ID}"); // Should be same every time!
}
```

**Giải Pháp C: Kiểm Tra Platform Support**

Một số platforms có hạn chế về PlayerPrefs. Đối với WebGL builds, browser cookies phải được bật.

---

### Vấn Đề 2: Hai Nhân Vật Chia Sẻ Upgrade Data

**Triệu Chứng**:
- Nâng cấp một nhân vật cũng nâng cấp nhân vật khác
- Các nhân vật khác nhau có chỉ số giống hệt nhau

**Nguyên Nhân**: Cả hai nhân vật có cùng ID

**Giải Pháp**:

Kiểm tra IDs trong Inspector:
```
Player_Archer
  └─ UpgradedCharacterParameter
      └─ ID: "archer_01" ✓ Good

Player_Mage
  └─ UpgradedCharacterParameter
      └─ ID: "archer_01" ✗ SAI! Phải là "mage_01"
```

**Sửa**: Đảm bảo mỗi nhân vật có một ID duy nhất.

**Bulk ID Checker Script**:
```csharp
using UnityEngine;
using RGame;
using System.Collections.Generic;

public class IDValidator : MonoBehaviour
{
    [ContextMenu("Check for Duplicate IDs")]
    void CheckDuplicateIDs()
    {
        UpgradedCharacterParameter[] allCharacters = FindObjectsOfType<UpgradedCharacterParameter>();

        Dictionary<string, string> idMap = new Dictionary<string, string>();

        foreach (var character in allCharacters)
        {
            if (idMap.ContainsKey(character.ID))
            {
                Debug.LogError($"DUPLICATE ID FOUND: '{character.ID}' used by both " +
                    $"'{idMap[character.ID]}' and '{character.gameObject.name}'");
            }
            else
            {
                idMap[character.ID] = character.gameObject.name;
                Debug.Log($"✓ {character.gameObject.name} has unique ID: {character.ID}");
            }
        }
    }
}
```

---

### Vấn Đề 3: Upgrade Costs Không Tăng

**Triệu Chứng**:
- Mỗi upgrade đều tốn cùng một lượng tiền
- Mong đợi: 100, 200, 400... Thực tế: 100, 100, 100...

**Nguyên Nhân**: Dùng sai UpgradeStep index

**Code Sai**:
```csharp
// SAI: Luôn dùng bước nâng cấp đầu tiên
UpgradeStep step = character.UpgradeSteps[0];
int cost = step.price; // Always returns UpgradeSteps[0].price
```

**Code Đúng**:
```csharp
// ĐÚNG: Dùng level nâng cấp hiện tại
int currentLevel = character.CurrentUpgrade;
UpgradeStep step = character.UpgradeSteps[currentLevel];
int cost = step.price; // Returns correct price for current level
```

---

### Vấn Đề 4: Không Thể Nâng Cấp Quá Level Đầu Tiên

**Triệu Chứng**:
- Nâng cấp đầu tiên hoạt động
- Tất cả các nâng cấp tiếp theo thất bại hoặc không làm gì
- `CurrentUpgrade` ở mức 0

**Nguyên Nhân**: Quên tăng `CurrentUpgrade`

**Kiểm Tra Code Của Bạn**:
```csharp
// Nếu bạn viết upgrade method riêng:
void CustomUpgrade()
{
    // Add stats...
    character.UpgradeHealth += 20;

    // ← Thiếu: character.CurrentUpgrade++;
}
```

**Giải Pháp**: Dùng method `UpgradeCharacter()` có sẵn, nó tự động tăng counter.

---

### Vấn Đề 5: NullReferenceException on Upgrade

**Error Message**:
```
NullReferenceException: Object reference not set to an instance of an object
UpgradedCharacterParameter.UpgradeCharacter()
```

**Nguyên Nhân**: `UpgradeSteps` array rỗng hoặc null

**Kiểm Tra Inspector**:
```
UpgradedCharacterParameter
  ├─ ID: "archer_01"
  └─ UpgradeSteps (Size: 0) ← Vấn đề! Phải > 0
```

**Giải Pháp**: Thêm ít nhất một upgrade step trong Inspector.

---

### Vấn Đề 6: Character Unlocks Quá Sớm/Muộn

**Triệu Chứng**:
- Character có sẵn trước level đã chỉ định
- Hoặc không bao giờ unlock ngay cả sau khi hoàn thành level yêu cầu

**Kiểm Tra**:
```csharp
public int unlockAtLevel = 5; // Should unlock after level 5
```

**Xác Minh Unlock Logic Của Bạn**:
```csharp
void CheckUnlock()
{
    UpgradedCharacterParameter character = GetComponent<UpgradedCharacterParameter>();
    int completedLevels = GameManager.Instance.GetCompletedLevels(); // Example

    if (completedLevels >= character.unlockAtLevel)
    {
        // Character is unlocked
        Debug.Log($"{character.ID} is now unlocked!");
    }
    else
    {
        Debug.Log($"{character.ID} requires level {character.unlockAtLevel}");
    }
}
```

**Lưu Ý**: Project này không tự động kiểm tra `unlockAtLevel`. Bạn cần implement logic này trong character selection UI.

---

### Vấn Đề 7: Reset Progress Không Hoạt Động

**Triệu Chứng**:
- Đã gọi `PlayerPrefs.DeleteAll()` nhưng nhân vật vẫn có upgrades

**Nguyên Nhân**: Properties reload từ cache thay vì PlayerPrefs

**Giải Pháp**: Restart scene sau khi xóa PlayerPrefs

```csharp
void ResetAndRestart()
{
    PlayerPrefs.DeleteAll();
    PlayerPrefs.Save();

    // Reload the current scene
    UnityEngine.SceneManagement.SceneManager.LoadScene(
        UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
    );
}
```

---

## Tài Liệu Tham Khảo Chéo

**Tài Liệu Liên Quan**:

- **[02_Player_System_Complete.md](02_Player_System_Complete.md)** - Cách hoạt động của player characters, di chuyển, bắn
- **[03_Enemy_System_Complete.md](03_Enemy_System_Complete.md)** - Hiểu về WeaponEffect và hệ thống sát thương
- **[04_UI_System_Complete.md](04_UI_System_Complete.md)** - Tạo UI để hiển thị chỉ số nhân vật
- **[10_How_To_Guides.md](10_How_To_Guides.md)** - Tutorial: "Cách Tạo Custom Upgrades"
- **[13_Code_Examples.md](13_Code_Examples.md)** - Ví dụ hệ thống Save/Load
- **[99_Glossary.md](99_Glossary.md)** - Định nghĩa: PlayerPrefs, Property, Serializable

**Classes Chính**:
- `UpgradedCharacterParameter.cs` - Class chính được documented ở đây
- `CharacterManager.cs` - Xử lý character spawning
- `GameManager.cs` - Quản lý game state và level progression
- `ShopUI.cs` - Shop interface (nếu được implement)

**Unity Documentation**:
- [PlayerPrefs](https://docs.unity3d.com/ScriptReference/PlayerPrefs.html)
- [Properties (C#)](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/properties)
- [Serialization](https://docs.unity3d.com/Manual/script-Serialization.html)

---

## Tóm Tắt

**Bạn Đã Học Được**:
- ✅ Cách `UpgradedCharacterParameter` lưu trữ và truy xuất chỉ số nhân vật
- ✅ Cách PlayerPrefs cung cấp data persistence giữa các phiên
- ✅ Cách hệ thống nâng cấp tăng chỉ số theo tiến trình
- ✅ Cách tạo nhân vật mới với abilities và stats tùy chỉnh
- ✅ Cách debug các vấn đề thường gặp liên quan đến nhân vật

**Điểm Chính**:
1. **Mỗi nhân vật PHẢI có ID duy nhất** để tránh xung đột dữ liệu
2. **PlayerPrefs tự động lưu** khi bạn dùng upgrade properties
3. **CurrentUpgrade = -1 nghĩa là max level** đã đạt
4. **UpgradeSteps array định nghĩa** toàn bộ hệ thống progression
5. **Default stats là giá trị khởi đầu**, upgrades cộng thêm vào chúng

**Bước Tiếp Theo**:
- Thử tạo nhân vật riêng của bạn theo hướng dẫn từng bước
- Thử nghiệm với các progression và costs khác nhau
- Xây dựng UI shop system để người chơi mua upgrades
- Đọc `04_UI_System_Complete.md` để học cách hiển thị stats

---

**Phiên Bản Tài Liệu**: 1.0
**Cập Nhật Lần Cuối**: 2025-10-29
**Transformation**: Vietnamese (22 lines) → English (1,700+ lines)
