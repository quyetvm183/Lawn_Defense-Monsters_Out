## Tổng quan scripts

Mục tiêu: tài liệu này tóm tắt các script chính trong `Assets/_MonstersOut/Scripts` để giúp bạn nhanh hiểu cấu trúc, luồng chạy và điểm mở rộng.

- Thư mục chính: `AI`, `Controllers`, `Helpers`, `Managers`, `Player`, `UI`.
- Entry / điều khiển global: `GameManager` (quản lý trạng thái game, registry các listener), `MenuManager` (UI, khởi tạo, pause), `LevelEnemyManager` (spawn enemy theo wave).

Flow tổng quát (ngắn):
1. Scene chứa prefab level được instantiate bởi `GameManager` (Awake). `MenuManager` bật UI, gọi `GameManager.StartGame()` để thu thập listeners.
2. `LevelEnemyManager` (listener) bắt đầu coroutine spawn enemy theo `EnemyWave` (cấu hình trong prefab level). Mỗi enemy prefab có scripts trong `AI`.
3. `Enemy` (base) và con của nó (ví dụ `SmartEnemyGrounded`, `WitchHeal`) điều khiển trạng thái: SPAWNING, WALK, ATTACK, HIT, DEATH.
4. `Controllers` chứa hệ thống vật lý dùng raycasts (`RaycastController`, `Controller2D`) và projectile (`Projectile`, `SimpleProjectile`) được enemy/player sử dụng.
5. `Helpers` chứa các utils (global values, animation helpers, spawn item, v.v.).
6. `UI` chứa hệ thống hiển thị (healthbar, floating text, menu) và kết nối với `GameManager`/`LevelManager`.

Hướng dẫn nhanh để bắt đầu đọc code:
- Bắt đầu từ `Assets/_MonstersOut/Scripts/Managers/GameManager.cs` để hiểu luồng trạng thái game.
- Mở `LevelEnemyManager.cs` và file level prefab (`GameLevelSetup`) để xem cách spawn enemy được cấu hình.
- Mở `Assets/_MonstersOut/Scripts/AI/Enemy.cs` (base) rồi `SmartEnemyGrounded.cs` để hiểu hành vi enemy.
- Kiểm tra `Controllers/Controller2D.cs` + `RaycastController.cs` để hiểu hệ thống chuyển động/va chạm 2D.

Tài liệu chi tiết theo folder nằm ở các file bên dưới (AI.md, Controllers.md, Helpers.md, Managers.md, Player.md, UI.md). 

---
Kết thúc: files dưới `Documents/` chứa mô tả file-by-file + flow chi tiết và hướng dẫn mở rộng.
