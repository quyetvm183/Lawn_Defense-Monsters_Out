## Thư mục: Helpers

Mục đích: tập hợp các tiện ích dùng chung (global value, animation helper, spawn helpers, v.v.).

File chính:

- `GlobalValue.cs` — lưu các giá trị persistent (PlayerPrefs): coins, settings âm thanh, level reached, item unlocks. Rất hữu ích để debug trạng thái lưu trữ.
- `AnimationHelper.cs` — lấy độ dài animation từ Animator.
- `SpawnItemHelper.cs` — spawn item khi hit/die.
- `CheckTargetHelper.cs` — (helper để kiểm tra target bằng raycast/circlecast) dùng rộng rãi cho enemy/player.
- `SortingLayerHelper.cs`, `RotateAround.cs`, `AutoDestroy.cs` — nhỏ, hỗ trợ hiệu ứng/đời sống object.
- `ReadOnlyAttribute.cs` — attribute dùng trong inspector để hiển thị readonly fields.

Gợi ý mở rộng:
- Khi thêm tính năng liên quan tới setting (ví dụ `isSound`, `isMusic`) hãy cập nhật `GlobalValue` để quản lý central state.
