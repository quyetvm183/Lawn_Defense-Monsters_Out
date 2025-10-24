# First 10 hands-on tasks (bắt tay vào code)

Mục đích: danh sách các task nhỏ, nhanh (1–2 giờ mỗi task) giúp bạn làm quen codebase và có kết quả thấy được.

1) Run in Editor & inspect scene
- Mở Unity, mở Scene/menu level (xem `GameManager` awake). Chạy Play, quan sát Console và behavior enemy.

2) Tweak an enemy speed
- Mở một prefab enemy, thay `walkSpeed` và kiểm tra thay đổi trong Play mode.

3) Add a new enemy prefab (duplicate)
- Duplicate existing enemy prefab, đổi sprite/animator, đổi `attackType`, test spawn.

4) Change projectile damage
- Mở `Controllers/SimpleProjectile.cs` or `ArrowProjectile.cs`, tăng Damage, test effect.

5) Add a new UpgradeStep to a character
- Mở `Player/UpgradedCharacterParameter` trên 1 prefab, thêm `UpgradeStep` và test buying/upgrading in-shop (or via PlayerPrefs change).

6) Inspect and trigger an animation event
- Mở Animator Controller của enemy, thêm Animation Event that calls `AnimMeleeAttackStart`, test that method is called (add Debug.Log inside the method if needed).

7) Play with Shop (local coin)
- Find shop UI prefab, press BuyCharacterBtn, observe coin deduction (`GlobalValue.SavedCoins`) and FloatingText.

8) Add Debug logs for spawn/wave
- Add `Debug.Log` into `LevelEnemyManager.SpawnEnemyCo()` to print wave/spawn counts while testing.

9) Modify Camera limits
- Change `CameraController.limitLeft/limitRight` and check camera clamping while dragging.

10) Implement small feature: double-shot powerup
- Modify `Player_Archer.Shoot()` to optionally spawn two arrows with slight angle offsets when a flag is true; test via PlayerPrefs flag or toggle in inspector.

Notes: For each task create a new git branch (eg `task/tweak-enemy-speed`) and commit small, descriptive changes. Run the project after each change.
