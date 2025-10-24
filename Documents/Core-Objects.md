# Core objects: Camera, Sound, Menu, GameManager

Giới thiệu: Các object này là backbone của gameplay và UI — hiểu chúng giúp bạn điều phối thay đổi trên toàn project.

1) `GameManager` (Managers/GameManager.cs)
- Single-instance (`Instance` static). Quản lý trạng thái game (`GameState { Menu, Playing, GameOver, Success, Pause }`).
- Quản lý listeners (IListener): `AddListener`, `RemoveListener`, và khi đổi state gọi IPlay/IPause/ISuccess/IGameOver cho các listeners.
- Chứa `gameLevels` array: instantiate level prefab trong `Awake()`.

2) `MenuManager` (UI/MenuManager.cs)
- Quản lý UI panels (StartUI, UI, VictotyUI, FailUI, PauseUI), âm thanh bật/tắt, load scene async (`LoadAsynchronously`).
- Kết nối với `GameManager`: gọi `GameManager.Instance.StartGame()` khi UI sẵn sàng.

3) `SoundManager` (Managers/SoundManager.cs)
- Singleton `Instance`, wrapper tiện lợi: `PlaySfx(...)`, `PlayMusic(...)`, `SoundVolume`/`MusicVolume`.
- Dùng `AudioSource.PlayOneShot` cho SFX và 1 `AudioSource` loop cho music.

4) `CameraController` (Controllers/CameraController.cs)
- Giới hạn camera (limitLeft, limitRight), kéo bằng chuột (drag) trên editor/device, smoothing bằng Lerp.
- `CameraHalfWidth` helper cho clamp.

Interaction nhanh:
- `MenuManager` thay đổi `Time.timeScale` khi pause; `GameManager` thay đổi `State` khi victory/gameover; listeners nhận event để pause behavior (ví dụ enemy `IGameOver` set isPlaying=false).
- `SoundManager.PlaySfx` được gọi trực tiếp từ nhiều script (Enemy.Hit, Enemy.Die, Player.Shoot...) — thêm clip vào inspector của prefab để định nghĩa sound.

Debug tips:
- Nếu music không phát: kiểm tra `SoundManager.Instance.musicsGame` gán đúng clip và `MusicVolume`.
- Nếu camera không clamp đúng: kiểm tra `limitLeft/limitRight` và `Camera.main.orthographicSize`.
