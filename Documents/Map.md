# Học Map (Level selection & Map UI)

Mục đích: giải thích cách map/level được quản lý, UI chọn level, và cách scene/level được load.

Files liên quan:
- `UI/MapControllerUI.cs` — (nằm trong `Assets/_MonstersOut/Scripts/UI`) quản lý map interface, điều hướng giữa các level.
- `Managers/GameManager.cs` — giữ mảng `gameLevels` (prefab level) và instantiate level prefab ở `Awake()`.
- `MenuManager.LoadAsynchronously(string name)` — hàm dùng `SceneManager.LoadSceneAsync` để load scene (dùng cho nút LoadNextLevel/Restart).
- Prefabs/Assets: level prefabs và `GameLevelSetup` (cấu hình mana, waves) — mở prefab level trong Project để xem `GameLevelSetup` components.

Flow tổng quát:
1. Khi app khởi chạy, `GameManager.Awake()` instantiate level prefab: 
   - Nếu `GameMode.Instance` null -> instantiate `gameLevels[1]` (mặc định), ngược lại instantiate `gameLevels[GlobalValue.levelPlaying - 1]`.
2. `GameLevelSetup` (component trên prefab level) chứa cấu hình như `EnemyWave[]`, mana, spawn points. `LevelEnemyManager` đọc `GameLevelSetup.Instance.GetLevelWave()` để lấy waves.
3. Người dùng chọn level qua `MapControllerUI` → gọi `MenuManager.LoadAsynchronously(SceneName)` hoặc set `GlobalValue.levelPlaying` và reload scene.

Tips để chỉnh map/level:
- Để thêm level mới: tạo prefab level (duplicate existing), đặt `GameLevelSetup` parameters (mana, waves, EnemyWave/EnemySpawn arrays), rồi thêm prefab vào `GameManager.gameLevels` array.
- Để debug spawn: bật gizmos hoặc thêm Debug.Log trong `LevelEnemyManager.SpawnEnemyCo()`.

Kiểm tra nhanh:
- Đảm bảo tên scene trùng với tên bạn dùng ở `LoadAsynchronously` nếu bạn dùng scenes thay vì prefabs.
- Mỗi level prefab nên chứa `LevelEnemyManager` và `GameLevelSetup` để spawn enemy đúng.
