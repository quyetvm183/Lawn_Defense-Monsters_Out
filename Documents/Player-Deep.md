# Học Player (deep dive vào `Player_Archer`)

Player trong repository này được triển khai như một `Enemy` (lợi thế: reuse hệ thống health/hit). Dưới đây là các điểm chính để hiểu `Player_Archer`.

Files chính:
- `Player/Player_Archer.cs` — class player chính; kế thừa `Enemy` và implement ICanTakeDamage, IListener.
- `Controllers/ArrowProjectile.cs` — mũi tên (projectile) mà player spawn khi bắn.

Key behaviors:
- AutoCheckAndShoot coroutine: dò target (CircleCastAll) trong layer enemy, chọn target gần nhất, tính điểm bắn `autoShootPoint`.
- `Shoot()` gọi `CheckTarget()` coroutine để tìm góc bắn tốt nhất bằng mô phỏng trajectory.
- Trajectory calculation: phạm vi góc bắt đầu `beginAngle`, lặp qua góc với `stepAngle` và mô phỏng vị trí mũi tên tại từng step (`stepCheck`) để tìm angle có khoảng cách nhỏ nhất tới điểm mục tiêu.
- Khi có góc tốt, spawn `ArrowProjectile` và gọi `_tempArrow.Init(force * AngleToVector2(beginAngle), gravityScale, arrowDamage)` để khởi tạo.

Interaction với hệ thống:
- Player thừa hưởng `Enemy.TakeDamage` nên projectile enemy có thể gây damage tương tự.
- Player tự bắn dựa trên `checkTargetHelper` (component) và `GameManager.Instance.layerEnemy` layer.

Hướng mở rộng:
- Thêm skill: ví dụ double-shot — trong `Shoot()` spawn thêm 1 arrow với angle lệch nhỏ.
- Thêm input control: hiện Player bắn tự động; để thêm bắn tay, expose `Shoot()` qua UI button hoặc input event.

Debugging:
- Nếu arrow không bay đúng: kiểm tra `gravityScale` trong ArrowProjectile, `force` parameter, và `ArrowProjectile.Init` implementation.
- Nếu check target không detect: kiểm tra layer `GameManager.Instance.layerEnemy` và Collider của enemy.
