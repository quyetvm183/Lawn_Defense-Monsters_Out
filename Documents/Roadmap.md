# Roadmap học để hiểu và phát triển project

Mục tiêu: cung cấp lộ trình học ngắn, tuần tự để bạn từ không biết Unity đến có thể phát triển, sửa và mở rộng project này.

Thời gian ước tính: 1–3 tuần (tùy thời gian mỗi ngày).

Giai đoạn A — Kiến thức nền tảng Unity (2–4 ngày)
- Hiểu Unity Editor: Scene, Game, Hierarchy, Inspector, Project, Console.
- Biết khái niệm: GameObject, Component, Prefab, Scene, Asset, Tag & Layer.
- MonoBehaviour basics: Start/Update/FixedUpdate/OnEnable/OnDisable, Coroutines (StartCoroutine, IEnumerator).
- Physics2D basics: Collider2D, Rigidbody2D, Raycast, LayerMask.
- UI basics: Canvas, RectTransform, Button, Text, Slider.

Giai đoạn B — Đọc code & cấu trúc project (2–4 ngày)
- Bắt đầu với `Documents/Scripts-Overview.md` để nắm folder-level.
- Đọc `GameManager.cs`, `MenuManager.cs`, `LevelEnemyManager.cs` để hiểu flow khởi tạo level và trạng thái game.
- Đọc `Enemy.cs` (base) → `SmartEnemyGrounded.cs` để nắm hành vi enemy.
- Đọc `Controller2D.cs` và `RaycastController.cs` để hiểu chuyển động/va chạm.

Giai đoạn C — Thực hành và debug (2–5 ngày)
- Chạy game trong Editor, bật Scene/Play, quan sát Console và Debug.DrawRay.
- Thêm một enemy prefab mới (duplicate), thay animation hoặc attack để thử.
- Sửa một value (ví dụ `walkSpeed`) và quan sát hành vi.

Giai đoạn D — Nâng cao & Lấy kết quả (2–5 ngày)
- Thêm feature: ví dụ thêm weapon effect mới, hoặc thêm purchasable character.
- Tối ưu: cải tiến collision/raycast, thêm editor scripts nếu cần.

Ghi chú: Khi gặp lỗi compile, đọc message trong Console, sửa lần lượt vì Unity ngừng build khi có lỗi.
