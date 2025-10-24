# Khái niệm Unity cần biết (tóm tắt)

1. GameObject & Component
- GameObject: đối tượng trong scene. Component: behaviour/thuộc tính gắn lên GameObject. Scripts (MonoBehaviour) là component.

2. Prefab
- Prefab là template GameObject lưu ở Project; instantiate prefab để spawn runtime.

3. Scene
- Scene chứa các GameObject. Thường một scene là một màn chơi/level hoặc menu.

4. MonoBehaviour lifecycle
- Awake(), OnEnable(), Start(), Update(), FixedUpdate(), LateUpdate(), OnDisable(), OnDestroy().
- Coroutines: dùng IEnumerator và StartCoroutine để chạy hàm có delay/time.

5. Physics2D & Raycasts
- Collider2D, Rigidbody2D, Raycast2D, CircleCast, LayerMask: dùng để dò va chạm mà không cần Rigidbody.

6. Animation
- Animator + AnimationClips + Triggers/Bools/Floats để điều khiển animation (xem `AnimationHelper`).

7. PlayerPrefs
- Dùng để lưu cài đặt và tiến trình đơn giản (coins, upgrades). Xem `GlobalValue` và `UpgradedCharacterParameter`.

8. Events & Listener pattern
- `IListener` interface được dùng để broadcast trạng thái game từ `GameManager` (IPlay, IPause, IGameOver...).

9. Layers & Tags
- Layers để lọc collision/raycast (LayerMask). Tags là nhãn đơn giản.

10. Namespaces
- namespace (ví dụ `RGame`) đóng gói các class tránh xung đột tên khi import packages/libraries. Nó cũng giúp tổ chức code.
