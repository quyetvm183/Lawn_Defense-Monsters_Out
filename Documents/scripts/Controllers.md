## Thư mục: Controllers

Mục đích: chứa hệ thống điều khiển chuyển động 2D, raycast collision và các projectile.

File chính:

- `RaycastController.cs` — base cho raycast detection. Cấu hình box collider, spacing cho các ray, tính toán origin cho raycasts.
- `Controller2D.cs` — dùng để di chuyển character với collision detection, climb/descend slope, check ground ahead, xử lý horizontal/vertical collisions. Hầu hết enemy/player dùng controller này để di chuyển ổn định trên tile/mặt phẳng.
- `Projectile.cs` — lớp trừu tượng cho projectile (owner, direction, damage, weaponEffect), xử lý va chạm chung (OnTriggerEnter2D) và gọi các hook `OnCollideTakeDamage`, `OnCollideOther`.
- `SimpleProjectile.cs` — implement projectile tiêu chuẩn: di chuyển thẳng, va chạm gây damage (gọi ICanTakeDamage), bật/hoạt destruction FX.
- `ArrowProjectile.cs` — projectile dành cho mũi tên (được Player_Archer sử dụng). (Mở file để kiểm tra param cụ thể như gravityScale, Init method.)
- `CameraController.cs` / `FixedCamera.cs` — logic camera follow / drag.

Gợi ý khi debug chuyển động:
- Đặt Debug.DrawRay/Log điểm va chạm, kiểm tra collider layer (collisionMask). Controller2D dùng raycasts nên việc thiếu/đặt sai layer sẽ dẫn đến xuyên vật.
- Nếu nhân vật bị kẹt trên slope/wall: kiểm tra horizontalRayCount / skinWidth / raySpacing và slopes giới hạn.
