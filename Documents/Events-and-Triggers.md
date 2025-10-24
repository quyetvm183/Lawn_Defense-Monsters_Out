# Events & Triggers trong project

Mục đích: liệt kê điểm event chính (game state, animation hooks, button callbacks, projectile collisions) và hướng dẫn theo dõi khi debug.

1) IListener & GameManager events
- `IListener` interface được implement bởi nhiều object (Enemies, Projectiles, Managers). Phương thức: IPlay, ISuccess, IPause, IUnPause, IGameOver, IOnRespawn, IOnStopMovingOn, IOnStopMovingOff.
- `GameManager` quản lý list `listeners` (AddListener/RemoveListener) và gọi các event tương ứng khi thay đổi trạng thái game (StartGame => gọi IPlay trên tất cả listeners, Victory => ISuccess ...).

2) Animation event hooks
- Các event animation được gọi thông qua phương thức public trong scripts (ví dụ `AnimMeleeAttackStart`, `AnimShoot`, `AnimThrow` trong enemy scripts). Những phương thức này thường được gọi từ Animation clips (Add Event trong Animator) - mở Animator Controller để xem event names.

3) Projectile/Collision events
- `Projectile.OnTriggerEnter2D(Collider2D other)` xử lý va chạm: check layer (LayerCollision) và gọi `OnCollideTakeDamage` hoặc `OnCollideOther`.
- Nếu object bị trừ HP: nó cần implement `ICanTakeDamage` hoặc `ICanTakeDamageBodyPart`.

4) UI & Button events
- UI buttons (Buy, Load Next Level, Pause) thường gán trực tiếp phương thức public của `MenuManager`, `ShopManager` hoặc `BuyCharacterBtn` trong Inspector. Kiểm tra các Button component trên prefab UI.

5) Custom game events / triggers
- Những trigger khác (ví dụ spawn, call minion, heal) thường được thực hiện bằng gọi trực tiếp method trên component (ví dụ `CallMinion()` trên enemy) hoặc `Instantiate` object có script. Tìm `AnimSetTrigger( ... )`/`Invoke(...)` để thấy nơi trigger.

Debugging tips:
- Muốn biết khi nào một event được gọi: đặt `Debug.Log("IPlay called on: " + gameObject.name);` trong IListener methods.
- Muốn tìm animation event: mở Animator Controller của prefab và kiểm tra Events (Animation -> Events list).
- Nếu projectile không gây sát thương: kiểm tra LayerCollision và xem `other.gameObject.GetComponent(typeof(ICanTakeDamage))` có trả về null không.
