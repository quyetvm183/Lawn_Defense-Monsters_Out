# Học Shop UI

Files liên quan chính:
- `Managers/ShopManager.cs`, `Managers/ShopItemUpgrade.cs`, `Managers/ShopCharacterUpgrade.cs` — logic mua và upgrade.
- `Managers/Purchaser.cs` — wrapper Unity IAP (nếu bạn dùng IAP thật trên store).
- UI button scripts: `UI/BuyCharacterBtn.cs`, `UI/ShopManager.cs` (nếu có), và prefab cửa hàng trong Scene/Canvas.

Flow mua hàng (local coins)
1. Người chơi nhấn nút buy trong UI (`BuyCharacterBtn.cs` hoặc tương tự) → gọi `ShopManager`.
2. `ShopManager` kiểm tra `GlobalValue.SavedCoins` và giá item; nếu đủ thì trừ coin và apply upgrade/unlock.
3. Gọi `FloatingTextManager` hoặc update UI để phản hồi.

Flow mua IAP (thực tế)
1. Người chơi nhấn buy → `Purchaser.BuyProductID(productId)`.
2. Unity IAP xử lý giao dịch; callback `ProcessPurchase` nhận thông tin sản phẩm.
3. Trong `ProcessPurchase` code thêm coin / unlock item (xem `Purchaser.ProcessPurchase`).

Mẹo để hiểu/extend Shop UI
- Tìm prefab UI trong `Assets` có nội dung shop (Canvas → Shop panel). Mở prefab để xem hierarchy và scripts gán.
- Nếu thêm item mới: tạo `ShopItem` data (scriptable object hoặc array trong `ShopManager`), gán giá, icon, effect.
- Để test IAP offline: tạm thời gọi hàm xử lý sau `ProcessPurchase` trực tiếp (ví dụ: add coins) để mô phỏng.
