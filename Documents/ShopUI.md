# Shop UI System

**Purpose**: Guide to the shop/store system, including character upgrades, IAP (In-App Purchases), and coin-based purchases.

**What This Document Covers**:
- Shop UI navigation and panel switching
- Local coin-based purchases (using GlobalValue.SavedCoins)
- Unity IAP integration for real money purchases
- Character upgrade system
- Adding new shop items

**Related Documentation**:
- See `Character-Properties.md` for UpgradedCharacterParameter and upgrade mechanics
- See `04_UI_System_Complete.md` for UI architecture
- See `05_Managers_Complete.md` for manager systems
- See `10_How_To_Guides.md` for practical tutorials

---

## Quick Reference

**Key Files**:
- `ShopManager.cs` - Manages shop panel switching (Upgrade/Buy Coins/Items tabs)
- `ShopItemUpgrade.cs` - Handles item upgrade purchases
- `ShopCharacterUpgrade.cs` - Handles character upgrade purchases
- `Purchaser.cs` - Unity IAP wrapper for real money transactions
- `BuyCharacterBtn.cs` - UI button for purchasing characters

---

## System Overview

The shop system has two purchase flows:

```
┌─────────────────────────────────────────────────────────────┐
│                  LOCAL COIN PURCHASE FLOW                   │
└─────────────────────────────────────────────────────────────┘

Player Clicks "Buy Character" Button (1000 coins)
        │
        ├─► BuyCharacterBtn.OnClick()
        │   └─ Checks GlobalValue.SavedCoins >= 1000
        │
        ├─► If enough coins:
        │   ├─ GlobalValue.SavedCoins -= 1000
        │   ├─ Unlock character (set PlayerPrefs flag)
        │   ├─ Update UI (show checkmark, disable button)
        │   └─ Play success sound/animation
        │
        └─► If not enough coins:
            └─ Show "Not enough coins!" message


┌─────────────────────────────────────────────────────────────┐
│               REAL MONEY IAP PURCHASE FLOW                  │
└─────────────────────────────────────────────────────────────┘

Player Clicks "Buy 1000 Coins ($0.99)" Button
        │
        ├─► Purchaser.BuyProductID("com.yourgame.coins1000")
        │   └─ Unity IAP opens platform store (Google Play/App Store)
        │
Player Completes Purchase on Platform
        │
        ├─► Unity IAP calls Purchaser.ProcessPurchase()
        │   ├─ Verify purchase is valid
        │   ├─ Grant coins: GlobalValue.SavedCoins += 1000
        │   ├─ Save: GlobalValue.SavedCoins (auto-saves to PlayerPrefs)
        │   └─ Show confirmation UI
        │
        └─► Return PurchaseProcessingResult.Complete
```

---

## ShopManager - Tab Navigation

**Location**: `Assets/_MonstersOut/Scripts/Managers/ShopManager.cs`

**Purpose**: Manages switching between shop tabs (Upgrade, Buy Coins, Items).

### How It Works

```csharp
public class ShopManager : MonoBehaviour
{
    public GameObject[] shopPanels;  // Array: [0] Upgrade, [1] BuyCoins, [2] Items
    public Image upgradeBut, buyCoinBut;
    public Sprite buttonActiveImage, buttonInActiveImage;

    void Start()
    {
        DisableObj();  // Hide all panels
        ActivePanel(shopPanels[0]);  // Show first panel (Upgrade)
        SetActiveBut(0);  // Highlight first button
    }

    public void SwichPanel(GameObject obj)
    {
        // Find which panel was clicked, activate it
        for (int i = 0; i < shopPanels.Length; i++)
        {
            if (obj == shopPanels[i])
            {
                DisableObj();  // Hide all
                ActivePanel(shopPanels[i]);  // Show selected
                SetActiveBut(i);  // Highlight button
                break;
            }
        }
        SoundManager.Click();
    }
}
```

**Usage in Inspector**:
1. Assign shop tab panels to `shopPanels` array:
   - Element 0: UpgradePanel
   - Element 1: BuyCoinsPanel
   - Element 2: ItemsPanel
2. Assign button Images to `upgradeBut`, `buyCoinBut`
3. Assign button sprites (active/inactive)
4. Connect buttons: `OnClick()` → `ShopManager.SwichPanel(panel reference)`

---

## Local Coin Purchases

### Character Purchase Example

**File**: `BuyCharacterBtn.cs` (or similar)

```csharp
namespace RGame
{
    public class BuyCharacterBtn : MonoBehaviour
    {
        public int price = 1000;  // Character costs 1000 coins
        public string characterID = "character_archer";

        public void OnClick()
        {
            // Check if player has enough coins
            if (GlobalValue.SavedCoins >= price)
            {
                // Deduct coins
                GlobalValue.SavedCoins -= price;

                // Unlock character (save to PlayerPrefs)
                PlayerPrefs.SetInt(characterID + "_unlocked", 1);

                // Update UI
                GetComponent<Button>().interactable = false;
                transform.Find("Checkmark").gameObject.SetActive(true);

                // Play success effect
                SoundManager.PlaySfx(SoundManager.Instance.soundPurchaseSuccess);
                FloatingTextManager.Show("+Character Unlocked!", Color.green);
            }
            else
            {
                // Not enough coins
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
            // Deduct coins
            GlobalValue.SavedCoins -= price;

            // Apply upgrade
            int currentHealth = PlayerPrefs.GetInt("player_max_health", 100);
            PlayerPrefs.SetInt("player_max_health", currentHealth + healthIncrease);

            // Mark as purchased
            PlayerPrefs.SetInt(upgradeID + "_purchased", 1);

            // Update UI
            UpdateUI();
        }
    }
}
```

---

## Unity IAP Integration

**File**: `Purchaser.cs`

**Purpose**: Wrapper for Unity IAP to handle real money transactions.

### Setup Steps

**Step 1: Enable Unity IAP**

1. Window → Package Manager
2. Search "In-App Purchasing"
3. Click Install

**Step 2: Configure Products**

In Unity Editor:
1. Window → Unity IAP → IAP Catalog
2. Add products:
   - ID: `com.yourgame.coins100`
   - Type: `Consumable`
   - Price: $0.99

**Step 3: Initialize IAP**

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

            // Add all IAP products
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
            // Grant rewards based on product ID
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

            // Show confirmation
            FloatingTextManager.Show("+Coins added!", Color.green);

            return PurchaseProcessingResult.Complete;
        }
    }
}
```

---

## Character Upgrade System

**See `Character-Properties.md` for complete details.**

**Quick Example**:

```csharp
// Get character's upgrade parameter
UpgradedCharacterParameter character = GetComponent<UpgradedCharacterParameter>();

// Check if player can afford upgrade
int price = character.UpgradeSteps[character.CurrentUpgrade].price;

if (GlobalValue.SavedCoins >= price)
{
    // Deduct coins
    GlobalValue.SavedCoins -= price;

    // Apply upgrade (increases health, damage, etc.)
    character.UpgradeCharacter(
        health: true,      // Upgrade health
        melee: true,       // Upgrade melee damage
        range: false,      // Don't upgrade range
        crit: false        // Don't upgrade crit
    );

    // character.CurrentUpgrade automatically increments
    // character.UpgradeHealth, UpgradeMeleeDamage automatically save to PlayerPrefs
}
```

---

## Adding New Shop Items

### Step 1: Create UI Button

1. Duplicate existing shop item button
2. Configure:
   - Set price text: "500 Coins"
   - Set item icon
   - Set item description

### Step 2: Create Purchase Script

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

                // Update UI
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

### Step 3: Attach and Configure

1. Add script to button GameObject
2. Set price in Inspector
3. Connect `Button.onClick()` → `BuySpeedBoost.OnClick()`

---

## Testing IAP (Without Real Money)

### Method 1: Use Test Mode

In `Purchaser.cs`:

```csharp
void InitializePurchasing()
{
    var builder = ConfigurationBuilder.Instance(
        StandardPurchasingModule.Instance(AppStore.GooglePlay)
    );

    // ... add products ...

    // Enable test mode (transactions don't charge real money)
    builder.Configure<IGooglePlayConfiguration>().SetDeferredPurchaseListener(null);

    UnityPurchasing.Initialize(this, builder);
}
```

### Method 2: Simulate Purchase in Editor

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

Add button in shop: `OnClick()` → `Purchaser.TestPurchase()`

---

## Common Patterns

### Check If Item Already Owned

```csharp
public void Start()
{
    string itemID = "premium_character";

    if (PlayerPrefs.GetInt(itemID + "_owned", 0) == 1)
    {
        // Already owned, show "Owned" label
        GetComponentInChildren<Text>().text = "OWNED";
        GetComponent<Button>().interactable = false;
    }
}
```

### Refund System (Developer Tool)

```csharp
public void RefundPurchase()
{
    GlobalValue.SavedCoins += price;  // Return coins
    PlayerPrefs.SetInt(itemID + "_owned", 0);  // Mark as not owned
    GetComponent<Button>().interactable = true;  // Re-enable button
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

### Problem: Coins Don't Save After Purchase

**Cause**: `GlobalValue.SavedCoins` property auto-saves to PlayerPrefs, but you might be setting coins directly without using the property.

**Solution**:
```csharp
// ❌ Wrong:
GlobalValue.savedCoins += 100;  // Lowercase field, doesn't trigger save!

// ✅ Correct:
GlobalValue.SavedCoins += 100;  // Uppercase property, auto-saves!
```

### Problem: IAP Buttons Don't Work

**Check**:
1. Unity IAP package installed?
2. Products configured in IAP Catalog?
3. `Purchaser.cs` initialized? (Check `Start()` is called)
4. Product IDs match exactly?

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

### Problem: Shop Panel Doesn't Switch

**Cause**: Panels not assigned to `ShopManager.shopPanels` array.

**Solution**:
1. Select ShopManager in Hierarchy
2. Inspector → Shop Panels → Set Size to 3
3. Assign:
   - Element 0: UpgradePanel
   - Element 1: BuyCoinsPanel
   - Element 2: ItemsPanel

---

## Summary

**Key Systems**:
1. **ShopManager**: Tab navigation (Upgrade/Buy Coins/Items)
2. **Local Purchases**: Use `GlobalValue.SavedCoins` (auto-saves to PlayerPrefs)
3. **IAP Purchases**: Use `Purchaser.BuyProductID()` → `ProcessPurchase()`
4. **Character Upgrades**: Use `UpgradedCharacterParameter.UpgradeCharacter()`

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

**Next Steps**:
- See `Character-Properties.md` for detailed upgrade system
- See `05_Managers_Complete.md` for GlobalValue details
- See Unity IAP documentation for platform-specific setup (Google Play, App Store)
