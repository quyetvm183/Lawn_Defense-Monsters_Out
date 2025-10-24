## Thư mục: UI

Mục đích: chứa các thành phần giao diện: healthbar, floating text, menu, tutorial và các popup.

File chính:

- `MenuManager.cs` — quản lý UI chính, trạng thái pause/victory/gameover, gọi `GameManager` để start game; xử lý âm thanh bật/tắt.
- `UI_UI.cs` — cập nhật thanh HP (player/enemy) và percent wave; hiển thị coins/mana.
- `FloatingTextManager.cs` — spawn floating text dùng trong damage/heal/coin.
- `HealthBarEnemyNew.cs` — prefab health bar cho enemy (cập nhật khi hit, ẩn sau một thời gian).
- Các script UI khác: `MainMenuHomeScene.cs`, `MapControllerUI.cs`, `BuyCharacterBtn.cs`, `NotEnoughCoins.cs`, `Menu_Victory.cs` đều xử lý các phần tương ứng của giao diện.

Gợi ý:
- UI sử dụng `MenuManager.Instance` làm điểm trung tâm; khi thêm UI mới, register nó hoặc gọi `MenuManager` để cập nhật giá trị.
