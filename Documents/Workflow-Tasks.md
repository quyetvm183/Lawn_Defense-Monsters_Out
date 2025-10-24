# Workflow và các task cụ thể cho project này

Mục tiêu: hướng dẫn quy trình làm việc khi phát triển tính năng hoặc sửa bug trong repository hiện tại.

1) Thiết lập môi trường
- Mở Unity Editor (phiên bản tương ứng trong `ProjectSettings/ProjectVersion.txt`).
- Mở scene level hoặc Scene chứa Menu (kiểm tra trong `Assets/_MonstersOut/_MonstersOut/Scene` hoặc prefab `gameLevels` trong `GameManager`).

2) Quy trình làm task (ví dụ: thêm enemy mới)
- Bước 1: tạo branch riêng (ví dụ `feature/new-enemy`).
- Bước 2: duplicate prefab enemy trong `Assets/_MonstersOut/_MonstersOut/Prefab/` (nếu có) hoặc tạo folder `Assets/Prefabs/YourEnemy`.
- Bước 3: gán script (thừa kế `SmartEnemyGrounded` hoặc `Enemy`) và chỉnh parameter trong Inspector (health, attackType, dieFX...).
- Bước 4: test trong Editor (Play), quan sát Console, sửa lỗi.
- Bước 5: commit, push, mở PR.

3) Debugging common issues
- Lỗi không detect player: kiểm tra LayerMask, CheckTargetHelper, collider size.
- Projectile không gây sát thương: kiểm tra LayerCollision trên projectile, và chắc chắn target implement `ICanTakeDamage`.
- Animation không trigger: kiểm tra Animator Controller, trigger name khớp với code `AnimSetTrigger("shoot")`.

4) Các task nhỏ lặp lại
- Thêm sound FX: update `SoundManager` hoặc gán clip vào inspector và gọi `SoundManager.PlaySfx(...)`.
- Thêm UI text: sửa prefabs UI trong `Assets/_MonstersOut/_MonstersOut/Scene/UI` và update `MenuManager`/`UI_UI` nếu cần.

5) Kiểm tra trước khi merge
- Build project trong Editor (nếu cần), chạy Play và test flows chính: start game, spawn enemies, wave complete, victory/gameover.
- Tắt debug logs (nếu quá nhiều) và đảm bảo không có compile errors.
