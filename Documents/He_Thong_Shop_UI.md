# Hệ Thống Shop UI

**Mục Đích**: Hướng dẫn về shop/store system, bao gồm character upgrades, IAP (In-App Purchases), và coin-based purchases.

**Nội Dung Tài Liệu**:
- Shop UI navigation và panel switching
- Local coin-based purchases (dùng GlobalValue.SavedCoins)
- Unity IAP integration cho real money purchases
- Character upgrade system
- Thêm shop items mới

**Tài Liệu Liên Quan**:
- Xem `Thuoc_Tinh_Nhan_Vat.md` cho UpgradedCharacterParameter và upgrade mechanics
- Xem `04_He_Thong_UI_Day_Du.md` cho UI architecture
- Xem `05_Cac_Manager_Day_Du.md` cho manager systems
- Xem `10_Huong_Dan_Thuc_Hanh.md` cho practical tutorials

---

## Quick Reference

**Key Files**:
- `ShopManager.cs` - Quản lý shop panel switching (Upgrade/Buy Coins/Items tabs)
- `ShopItemUpgrade.cs` - Xử lý item upgrade purchases
- `ShopCharacterUpgrade.cs` - Xử lý character upgrade purchases
- `Purchaser.cs` - Unity IAP wrapper cho real money transactions
- `BuyCharacterBtn.cs` - UI button cho purchasing characters

---

## System Overview

Shop system có hai purchase flows:

```
┌─────────────────────────────────────────────────────────────┐
│                  LOCAL COIN PURCHASE FLOW                   │
└─────────────────────────────────────────────────────────────┘

Player Click "Buy Character" Button (1000 coins)
        │
        ├─► BuyCharacterBtn.OnClick()
        │   └─ Kiểm tra GlobalValue.SavedCoins >= 1000
        │
        ├─► Nếu đủ coins:
        │   ├─ GlobalValue.SavedCoins -= 1000
        │   ├─ Unlock character (set PlayerPrefs flag)
        │   ├─ Cập nhật UI (hiển thị checkmark, disable button)
        │   └─ Phát success sound/animation
        │
        └─► Nếu không đủ coins:
            └─ Hiển thị "Not enough coins!" message


┌─────────────────────────────────────────────────────────────┐
│               REAL MONEY IAP PURCHASE FLOW                  │
└─────────────────────────────────────────────────────────────┘

Player Click "Buy 1000 Coins ($0.99)" Button
        │
        ├─► Purchaser.BuyProductID("com.yourgame.coins1000")
        │   └─ Unity IAP mở platform store (Google Play/App Store)
        │
Player Hoàn Thành Purchase Trên Platform
        │
        ├─► Unity IAP gọi Purchaser.ProcessPurchase()
        │   ├─ Verify purchase hợp lệ
        │   ├─ Grant coins: GlobalValue.SavedCoins += 1000
        │   ├─ Save: GlobalValue.SavedCoins (auto-saves vào PlayerPrefs)
        │   └─ Hiển thị confirmation UI
        │
        └─► Return PurchaseProcessingResult.Complete
```

---

## ShopManager - Tab Navigation

**Location**: `Assets/_MonstersOut/Scripts/Managers/ShopManager.cs`

**Mục Đích**: Quản lý switching giữa shop tabs (Upgrade, Buy Coins, Items).

### Cách Hoạt Động

```csharp
public class ShopManager : MonoBehaviour
{
    public GameObject[] shopPanels;  // Array: [0] Upgrade, [1] BuyCoins, [2] Items
    public Image upgradeBut, buyCoinBut;
    public Sprite buttonActiveImage, buttonInActiveImage;

    void Start()
    {
        DisableObj();  // Ẩn tất cả panels
        ActivePanel(shopPanels[0]);  // Hiển thị first panel (Upgrade)
        SetActiveBut(0);  // Highlight first button
    }

    public void SwichPanel(GameObject obj)
    {
        // Tìm panel nào được clicked, activate nó
        for (int i = 0; i < shopPanels.Length; i++)
        {
            if (obj == shopPanels[i])
            {
                DisableObj();  // Ẩn tất cả
                ActivePanel(shopPanels[i]);  // Hiển thị selected
                SetActiveBut(i);  // Highlight button
                break;
            }
        }
        SoundManager.Click();
    }
}
```

**Cách Dùng Trong Inspector**:
1. Gán shop tab panels vào `shopPanels` array:
   - Element 0: UpgradePanel
   - Element 1: BuyCoinsPanel
   - Element 2: ItemsPanel
2. Gán button Images vào `upgradeBut`, `buyCoinBut`
3. Gán button sprites (active/inactive)
4. Connect buttons: `OnClick()` → `ShopManager.SwichPanel(panel reference)`

---

## Local Coin Purchases

### Character Purchase Example

**File**: `BuyCharacterBtn.cs` (hoặc tương tự)

```csharp
namespace RGame
{
    public class BuyCharacterBtn : MonoBehaviour
    {
        public int price = 1000;  // Character costs 1000 coins
        public string characterID = "character_archer";

        public void OnClick()
        {
            // Kiểm tra nếu player có đủ coins
            if (GlobalValue.SavedCoins >= price)
            {
                // Trừ coins
                GlobalValue.SavedCoins -= price;

                // Unlock character (save vào PlayerPrefs)
                PlayerPrefs.SetInt(characterID + "_unlocked", 1);

                // Cập nhật UI
                GetComponent<Button>().interactable = false;
                transform.Find("Checkmark").gameObject.SetActive(true);

                // Phát success effect
                SoundManager.PlaySfx(SoundManager.Instance.soundPurchaseSuccess);
                FloatingTextManager.Show("+Character Unlocked!", Color.green);
            }
            else
            {
                // Không đủ coins
                SoundManager.PlaySfx(SoundManager.Instance.soundError);
                FloatingTextManager.Show("Not enough coins!", Color.red);
            }
        }
    }
}
```

### Item/Upgrade Purchase Example

```csharp
public class ShopItemUpgrade : MonoBehaviour
{
    public string upgradeID = "health_upgrade";
    public int price = 500;
    public int healthIncrease = 50;

    public void BuyUpgrade()
    {
        if (GlobalValue.SavedCoins >= price)
        {
            // Trừ coins
            GlobalValue.SavedCoins -= price;

            // Apply upgrade
            int currentHealth = PlayerPrefs.GetInt("player_max_health", 100);
            PlayerPrefs.SetInt("player_max_health", currentHealth + healthIncrease);

            // Đánh dấu là purchased
            PlayerPrefs.SetInt(upgradeID + "_purchased", 1);

            // Cập nhật UI
            UpdateUI();
        }
    }
}
```

---

## Unity IAP Integration

**File**: `Purchaser.cs`

**Mục Đích**: Wrapper cho Unity IAP để xử lý real money transactions.

### Setup Steps

**Bước 1: Enable Unity IAP**

1. Window → Package Manager
2. Search "In-App Purchasing"
3. Click Install

**Bước 2: Cấu Hình Products**

Trong Unity Editor:
1. Window → Unity IAP → IAP Catalog
2. Thêm products:
   - ID: `com.yourgame.coins100`
   - Type: `Consumable`
   - Price: $0.99

**Bước 3: Initialize IAP**

```csharp
namespace RGame
{
    public class Purchaser : MonoBehaviour, IStoreListener
    {
        private static IStoreController storeController;
        private static IExtensionProvider storeExtensionProvider;

        void Start()
        {
            InitializePurchasing();
        }

        public void InitializePurchasing()
        {
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            // Thêm tất cả IAP products
            builder.AddProduct("com.yourgame.coins100", ProductType.Consumable);
            builder.AddProduct("com.yourgame.coins500", ProductType.Consumable);
            builder.AddProduct("com.yourgame.coins1000", ProductType.Consumable);

            UnityPurchasing.Initialize(this, builder);
        }

        public void BuyProductID(string productId)
        {
            if (storeController != null)
            {
                Product product = storeController.products.WithID(productId);

                if (product != null && product.availableToPurchase)
                {
                    storeController.InitiatePurchase(product);
                }
            }
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            // Grant rewards dựa trên product ID
            if (args.purchasedProduct.definition.id == "com.yourgame.coins100")
            {
                GlobalValue.SavedCoins += 100;
            }
            else if (args.purchasedProduct.definition.id == "com.yourgame.coins500")
            {
                GlobalValue.SavedCoins += 500;
            }
            else if (args.purchasedProduct.definition.id == "com.yourgame.coins1000")
            {
                GlobalValue.SavedCoins += 1000;
            }

            // Hiển thị confirmation
            FloatingTextManager.Show("+Coins added!", Color.green);

            return PurchaseProcessingResult.Complete;
        }
    }
}
```

---

## Character Upgrade System

**Xem `Thuoc_Tinh_Nhan_Vat.md` cho chi tiết đầy đủ.**

**Ví Dụ Nhanh**:

```csharp
// Lấy character's upgrade parameter
UpgradedCharacterParameter character = GetComponent<UpgradedCharacterParameter>();

// Kiểm tra nếu player có thể afford upgrade
int price = character.UpgradeSteps[character.CurrentUpgrade].price;

if (GlobalValue.SavedCoins >= price)
{
    // Trừ coins
    GlobalValue.SavedCoins -= price;

    // Apply upgrade (tăng health, damage, etc.)
    character.UpgradeCharacter(
        health: true,      // Upgrade health
        melee: true,       // Upgrade melee damage
        range: false,      // Không upgrade range
        crit: false        // Không upgrade crit
    );

    // character.CurrentUpgrade tự động increments
    // character.UpgradeHealth, UpgradeMeleeDamage tự động save vào PlayerPrefs
}
```

---

## Thêm Shop Items Mới

### Bước 1: Tạo UI Button

1. Duplicate shop item button có sẵn
2. Cấu hình:
   - Đặt price text: "500 Coins"
   - Đặt item icon
   - Đặt item description

### Bước 2: Tạo Purchase Script

```csharp
namespace RGame
{
    public class BuySpeedBoost : MonoBehaviour
    {
        public int price = 500;

        public void OnClick()
        {
            if (GlobalValue.SavedCoins >= price)
            {
                GlobalValue.SavedCoins -= price;

                // Grant item
                PlayerPrefs.SetInt("speedBoost_owned", 1);

                // Cập nhật UI
                GetComponent<Button>().interactable = false;

                SoundManager.Click();
                FloatingTextManager.Show("+Speed Boost Unlocked!", Color.green);
            }
            else
            {
                SoundManager.PlaySfx(SoundManager.Instance.soundError);
            }
        }
    }
}
```

### Bước 3: Attach Và Cấu Hình

1. Thêm script vào button GameObject
2. Đặt price trong Inspector
3. Connect `Button.onClick()` → `BuySpeedBoost.OnClick()`

---

## Testing IAP (Không Cần Real Money)

### Method 1: Dùng Test Mode

Trong `Purchaser.cs`:

```csharp
void InitializePurchasing()
{
    var builder = ConfigurationBuilder.Instance(
        StandardPurchasingModule.Instance(AppStore.GooglePlay)
    );

    // ... add products ...

    // Enable test mode (transactions không charge real money)
    builder.Configure<IGooglePlayConfiguration>().SetDeferredPurchaseListener(null);

    UnityPurchasing.Initialize(this, builder);
}
```

### Method 2: Simulate Purchase Trong Editor

```csharp
#if UNITY_EDITOR
public void TestPurchase()
{
    // Simulate successful purchase
    GlobalValue.SavedCoins += 1000;
    Debug.Log("Test purchase: +1000 coins");
}
#endif
```

Thêm button trong shop: `OnClick()` → `Purchaser.TestPurchase()`

---

## Common Patterns

### Kiểm Tra Nếu Item Đã Owned

```csharp
public void Start()
{
    string itemID = "premium_character";

    if (PlayerPrefs.GetInt(itemID + "_owned", 0) == 1)
    {
        // Đã owned, hiển thị "Owned" label
        GetComponentInChildren<Text>().text = "OWNED";
        GetComponent<Button>().interactable = false;
    }
}
```

### Refund System (Developer Tool)

```csharp
public void RefundPurchase()
{
    GlobalValue.SavedCoins += price;  // Trả lại coins
    PlayerPrefs.SetInt(itemID + "_owned", 0);  // Đánh dấu là not owned
    GetComponent<Button>().interactable = true;  // Bật lại button
}
```

### Limited-Time Sale

```csharp
public class SaleItem : MonoBehaviour
{
    public int originalPrice = 1000;
    public int salePrice = 500;
    public string saleEndDate = "2025-12-31";

    void Start()
    {
        if (System.DateTime.Now < System.DateTime.Parse(saleEndDate))
        {
            // Sale active
            priceText.text = salePrice + " Coins";
            saleLabel.SetActive(true);
        }
        else
        {
            // Sale ended
            priceText.text = originalPrice + " Coins";
        }
    }
}
```

---

## Troubleshooting

### Vấn Đề: Coins Không Save Sau Purchase

**Nguyên Nhân**: `GlobalValue.SavedCoins` property auto-saves vào PlayerPrefs, nhưng bạn có thể đang set coins trực tiếp mà không dùng property.

**Giải Pháp**:
```csharp
// ❌ Sai:
GlobalValue.savedCoins += 100;  // Lowercase field, không trigger save!

// ✅ Đúng:
GlobalValue.SavedCoins += 100;  // Uppercase property, auto-saves!
```

### Vấn Đề: IAP Buttons Không Hoạt Động

**Kiểm Tra**:
1. Unity IAP package đã install?
2. Products đã cấu hình trong IAP Catalog?
3. `Purchaser.cs` đã initialized? (Kiểm tra `Start()` được called)
4. Product IDs khớp chính xác?

**Debug**:
```csharp
public void BuyProductID(string productId)
{
    Debug.Log("Attempting to buy: " + productId);

    if (storeController == null)
    {
        Debug.LogError("Store not initialized!");
        return;
    }

    Product product = storeController.products.WithID(productId);

    if (product == null)
    {
        Debug.LogError("Product not found: " + productId);
        return;
    }

    Debug.Log("Initiating purchase...");
    storeController.InitiatePurchase(product);
}
```

### Vấn Đề: Shop Panel Không Switch

**Nguyên Nhân**: Panels không được gán vào `ShopManager.shopPanels` array.

**Giải Pháp**:
1. Chọn ShopManager trong Hierarchy
2. Inspector → Shop Panels → Set Size thành 3
3. Gán:
   - Element 0: UpgradePanel
   - Element 1: BuyCoinsPanel
   - Element 2: ItemsPanel

---

## Tóm Tắt

**Key Systems**:
1. **ShopManager**: Tab navigation (Upgrade/Buy Coins/Items)
2. **Local Purchases**: Dùng `GlobalValue.SavedCoins` (auto-saves vào PlayerPrefs)
3. **IAP Purchases**: Dùng `Purchaser.BuyProductID()` → `ProcessPurchase()`
4. **Character Upgrades**: Dùng `UpgradedCharacterParameter.UpgradeCharacter()`

**Essential Code**:

```csharp
// Local coin purchase
if (GlobalValue.SavedCoins >= price)
{
    GlobalValue.SavedCoins -= price;
    PlayerPrefs.SetInt(itemID + "_owned", 1);
}

// IAP purchase
Purchaser.Instance.BuyProductID("com.yourgame.coins1000");

// Character upgrade
character.UpgradeCharacter(health: true, melee: true, range: false, crit: false);
```

**Các Bước Tiếp Theo**:
- Xem `Thuoc_Tinh_Nhan_Vat.md` cho detailed upgrade system
- Xem `05_Cac_Manager_Day_Du.md` cho GlobalValue details
- Xem Unity IAP documentation cho platform-specific setup (Google Play, App Store)

---

**Kết Thúc Tài Liệu**

<p align="center">
<strong>Lawn Defense: Monsters Out</strong><br>
Hệ Thống Shop UI<br>
Shop UI System Technical Guide
</p>
