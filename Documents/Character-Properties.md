# Thuộc tính Character (UpgradedCharacterParameter)

Mục đích: giải thích nơi lưu thuộc tính nhân vật, cách upgrade và cách thêm character mới.

Key files:
- `Player/UpgradedCharacterParameter.cs` — chứa data của character: defaultHealth, meleeDamage, rangeDamage, critical, weaponEffect, UpgradeSteps[] và logic lưu upgrade bằng `PlayerPrefs`.

Cấu trúc chính:
- `UpgradeStep` (price, healthStep, meleeDamageStep, rangeDamageStep, criticalStep) — dùng để tăng từng bước.
- `ID` string — dùng làm prefix trong PlayerPrefs keys (ví dụ `ID + "upgradeHealth"`).
- Properties: `UpgradeHealth`, `UpgradeMeleeDamage`, `UpgradeRangeDamage`, `UpgradeCriticalDamage` — wrappers cho PlayerPrefs.
- `CurrentUpgrade` — index upgrade hiện tại (lưu trong PlayerPrefs). Nếu `-1` nghĩa là đã max cấp.

Thêm character mới:
1. Tạo prefab nhân vật trong `Assets` (sprite, animator, collider, prefab) và thêm component `UpgradedCharacterParameter`.
2. Đặt `ID` duy nhất (ví dụ `archer_01`) để tránh xung đột PlayerPrefs.
3. Điền `defaultHealth`, damage, `UpgradeSteps[]` và `weaponEffect` nếu cần.
4. Tạo prefab vào `Character` container/scene và cập nhật `CharacterManager`/UI nếu cần.

Lưu ý khi debug:
- Nếu bạn thay `ID`, giá trị PlayerPrefs cũ sẽ không áp dụng. Để reset: xóa key trong Editor bằng `PlayerPrefs.DeleteKey("<key>")` hoặc xóa PlayerPrefs toàn bộ (chú ý mất dữ liệu khác).
