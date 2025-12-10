## Thư mục: AI

Mục đích: chứa logic hành vi kẻ địch (enemy) và các script liên quan tới việc tấn công, nhận sát thương, spawn item khi chết, v.v.

Danh sách file chính và ý nghĩa ngắn:

- `Enemy.cs` — lớp base cho mọi enemy. Quản lý trạng thái (SPAWN, WALK, ATTACK, HIT, DEATH), hệ thống effect (BURN, POISON, FREEZE, SHOCK), âm thanh, health, healthbar, method TakeDamage/Die/Hit/Freeze/Burning/Poison/Shoking. Đây là điểm bắt đầu để hiểu lifecycle của 1 enemy.

- `SmartEnemyGrounded.cs` — triển khai chi tiết cho enemy đi bộ trên mặt đất. Kết hợp `Controller2D` để di chuyển, kiểm tra target, gọi `EnemyRangeAttack`/`EnemyMeleeAttack`/`EnemyThrowAttack` tùy `ATTACKTYPE`. Xử lý animation events (AnimMeleeAttackStart / AnimShoot / AnimThrow). Đây là script được dùng phổ biến cho enemy.

- `EnemyMeleeAttack.cs` — xử lý tấn công cận chiến: kiểm tra target bằng CircleCast, tính damage/crit và gọi TakeDamage trên target.

- `EnemyRangeAttack.cs` — xử lý tấn công tầm xa: kiểm tra target bằng circle/raycast, instantiate projectile (dựa trên `Projectile`) và initialize.

- `EnemyThrowAttack.cs` — (tương tự Range) xử lý việc ném đồ (không đọc chi tiết trong lần scan nhưng tên và pattern giống các attack khác).

- `EnemySpawn.cs` — class serializable dùng để cấu hình spawn (wait, numberEnemy, rate) — dùng bởi `LevelEnemyManager` / `EnemyWave`.

- `GiveCoinWhenDie.cs` — khi enemy chết, cộng coin và show floating text.

- `ICanTakeDamage.cs` / `ICanTakeDamageBodyPart.cs` — interfaces được các đối tượng ('enemy', 'player', projectile) dùng để gọi TakeDamage polymorphic.

- `SmartEnemyGrounded`, `TheFortrest`, `WitchHeal` — ví dụ enemy cụ thể: WitchHeal heal đồng đội; TheFortrest có thể là enemy hỗ trợ/phá công trình (khuyến nghị mở code để xem chi tiết).

Gợi ý đọc theo thứ tự: `Enemy.cs` → `ICanTakeDamage` → `SmartEnemyGrounded.cs` → attack scripts (EnemyMeleeAttack, EnemyRangeAttack) → spawn helper `EnemySpawn`.

Điểm mở rộng phổ biến:
- Thêm loại enemy mới: tạo prefab enemy base với script kế thừa `Enemy` hoặc `SmartEnemyGrounded` và gán `ATTACKTYPE`/animation.
- Thêm effect/skill mới: mở `Enemy.cs` để thêm enum + xử lý TakeDamage hoặc effect coroutine tương ứng.
