## Thư mục: Managers

Mục đích: chứa các hệ thống quản lý game-level, shop, sound, IAP, và thiết lập level.

File chính:

- `GameManager.cs` — trung tâm trạng thái game. Quản lý listeners (IListener), chuyển đổi GameState (Menu, Playing, Pause, GameOver, Success), spawn level prefab ở Awake, gọi StartGame để thu thập listeners và phát IPlay/IUnPause/IGameOver...
- `LevelManager.cs` — parameter của level (ví dụ mana) và singleton.
- `LevelEnemyManager.cs` — spawn enemy theo `EnemyWave` cấu hình; quản lý spawn coroutine.
- `GameLevelSetup.cs`, `LevelWave.cs`, `Level.cs` — (prefab/serializable) chứa cấu hình level (EnemyWaves, mana, etc.). Mở prefab level để xem cách level được cấu hình.
- `ShopManager.cs`, `Purchaser.cs`, `IAPItem.cs` — logic cửa hàng / in-app purchase (Unity Purchasing).
- `SoundManager.cs` — wrapper thuận tiện để play music/sfx (PlaySfx, PlayMusic). Dùng `SoundManager.Instance` để set volume.

Lưu ý chạy/kiểm tra:
- `Purchaser` sử dụng Unity Purchasing API; build platform cần cấu hình IAP để test.
