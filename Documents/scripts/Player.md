## Thư mục: Player

Mục đích: chứa script nhân vật (player) và parameter upgrade.

File chính:

- `Player_Archer.cs` — implement player archer behaviour. Thừa kế từ `Enemy` (vì player dùng cùng hệ thống sức khỏe/hit) và thêm logic bắn mũi tên tự động, tính toán góc bắn bằng mô phỏng trajectory, reload/animation và interaction với `ArrowProjectile`.
- `CharacterManager.cs` — spawn nhân vật (dùng khi chọn/khởi tạo nhân vật trong màn chơi).
- `UpgradedCharacterParameter.cs` — lưu parameter nhân vật (upgrade steps, damage, health, weaponEffect). Dùng PlayerPrefs để lưu các upgrade.

Gợi ý phát triển:
- Player_Archer tận dụng hệ thống `Projectile` và `Controllers`. Nếu muốn thêm class player mới (ví dụ melee player), tạo script mới kế thừa `Enemy` hoặc copy mẫu `Player_Archer` và thay đổi attack logic.
