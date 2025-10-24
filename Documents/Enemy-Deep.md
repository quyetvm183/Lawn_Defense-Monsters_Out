# Học Enemy (deep dive vào `Enemy` và các enemy cụ thể)

`Enemy.cs` là lớp base rất quan trọng — hầu hết enemy và player (Player_Archer) kế thừa từ class này:

Những điểm chính trong `Enemy.cs`:
- State machine: `ENEMYSTATE` (SPAWNING, IDLE, ATTACK, WALK, HIT, DEATH). Thay đổi bằng `SetEnemyState(...)` và dùng animation triggers.
- Effects hệ thống: `ENEMYEFFECT` (BURNING, FREEZE, SHOKING, POISON, EXPLOSION) với coroutine xử lý (Burning, Poison, Shoking, Freeze). Mỗi effect thay đổi `enemyEffect` và đôi khi điều chỉnh `multipleSpeed`.
- Health & healthbar: `health` + `currentHealth`. HealthBar prefab load bằng `Resources.Load("HealthBar")` và `healthBar.Init(transform, offset)`.
- Sát thương: `TakeDamage(...)` common xử lý damage, spawn hitFX/blood, gọi `Die()` hoặc `Hit()`.
- Hit behavior: `Hit()` gọi `KnockBack()` nếu `HITBEHAVIOR.CANKNOCKBACK`.

Script con thường gặp:
- `SmartEnemyGrounded.cs` — enemy đi bộ trên mặt đất: dùng `Controller2D` để di chuyển, định hướng, flip, kiểm tra attack dựa trên `ATTACKTYPE`.
- `WitchHeal.cs` — healer enemy: dò đồng đội để heal (CircleCast trên healTargetLayer).
- `EnemyRangeAttack.cs`, `EnemyMeleeAttack.cs`, `EnemyThrowAttack.cs` — modul attack chuyên biệt; gắn lên enemy prefab và gọi từ `SmartEnemyGrounded`.

Thêm enemy mới (workflow):
1. Duplicate một enemy prefab hiện có.
2. Gán sprite/animator nếu cần.
3. Chỉnh `attackType`, `health`, `walkSpeed`, `startBehavior`, `dieBehavior` trong Inspector.
4. Nếu cần behavior mới, tạo script kế thừa `Enemy` hoặc `SmartEnemyGrounded` và override các phương thức (Start, Update, Hit, Die, DetectPlayer).

Chi tiết debug:
- Nếu enemy không xuất hiện: kiểm tra prefab active, `LevelEnemyManager` spawn list, và `gameLevels` trong `GameManager`.
- Nếu animation không đồng bộ với trạng thái: mở Animator Controller và so khớp trigger/bool names với code (`AnimSetTrigger("melee")`, `AnimSetTrigger("shoot")`).
