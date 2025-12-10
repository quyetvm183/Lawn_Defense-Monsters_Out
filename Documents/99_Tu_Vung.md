# Từ Điển Thuật Ngữ - Unity & Project Terms A-Z

**Ngôn Ngữ**: Tiếng Việt (Vietnamese)
**Mục Đích**: Tham khảo nhanh các thuật ngữ Unity và thuật ngữ cụ thể của project
**Cách Dùng**: Tra cứu các thuật ngữ không quen khi đọc tài liệu
**Định Dạng**: Thuật Ngữ → Định Nghĩa Đơn Giản → Ngữ Cảnh/Ví Dụ

---

## A

**AdMob**
Nền tảng quảng cáo di động của Google. Dùng trong project này để hiển thị banner và interstitial ads.
→ Xem: `AdsManager.cs`

**Animator**
Component Unity điều khiển animation playback. Dùng Animation Controller với states và transitions.
→ Ví dụ: Player có Animator cho walk, shoot, die animations

**Animation Clip**
Một chuỗi animation đơn (ví dụ, animation "walk").
→ Được tạo trong Unity Animation window

**Animation Controller**
State machine quản lý animation nào chạy khi nào.
→ Ví dụ: "Idle" → "Walk" khi speed > 0

**Anchor (UI)**
Xác định UI element định vị thế nào so với parent. Quan trọng cho hỗ trợ multi-resolution.
→ Ví dụ: Top-left anchor giữ button ở góc

**API (Application Programming Interface)**
Tập hợp functions/classes được cung cấp bởi hệ thống. Unity API = tất cả Unity classes/methods.
→ Ví dụ: `GameObject.Find()` là một phần của Unity API

**ArrowProjectile**
Custom class cho hành vi mũi tên. Xử lý trajectory (quỹ đạo), collision (va chạm), damage (sát thương).
→ Vị trí: `Controllers/ArrowProjectile.cs`

**Asset**
Bất kỳ file nào trong Unity project (script, sprite, audio, prefab, scene).
→ Được lưu trong thư mục `Assets/`

**Async (Asynchronous)**
Code chạy độc lập với main thread. Dùng cho loading (tải), networking (mạng).
→ Ví dụ: `SceneManager.LoadSceneAsync()`

**ATTACKTYPE (Enum)**
Định nghĩa phương thức tấn công của enemy: RANGE, MELEE, THROW, NONE.
→ Được dùng bởi Enemy.cs để xác định combat behavior

**Audio Clip**
File âm thanh được import vào Unity (.wav, .mp3, .ogg).
→ Được phát qua AudioSource component

**AudioSource**
Unity component phát audio clips. Có volume, pitch, loop settings.

**Auto-Targeting**
Hệ thống mà player tự động nhắm vào enemies không cần manual input.
→ Được triển khai trong `Player_Archer.cs:276` (AutoCheckAndShoot coroutine)

**Awake()**
MonoBehaviour lifecycle method. Được gọi khi GameObject được tạo, trước Start().
→ Dùng để: Initialization (khởi tạo), GetComponent caching

---

## B

**Ballistic Trajectory**
Đường cong của projectile bị ảnh hưởng bởi gravity. Player tính toán cái này cho arrows.
→ Công thức Physics: y = y₀ + v₀t - ½gt²

**Base Class**
Class mà các classes khác kế thừa từ nó.
→ Ví dụ: `Enemy` là base class cho `Player_Archer` và `SmartEnemyGrounded`

**BODYPART (Enum)**
Chỉ định nơi damage hit xảy ra: NONE, HEAD, BODY, ARM, LEG.
→ Được dùng trong `ICanTakeDamage.TakeDamage()` cho critical hits

**Boolean (bool)**
Kiểu biến có hai giá trị: true hoặc false.
→ Ví dụ: `isAvailable = true;`

**BoxCollider2D**
Unity component định nghĩa vùng va chạm hình chữ nhật cho 2D objects.
→ Player và enemies dùng cái này cho hit detection

**Build**
Quá trình compile game thành executable (Android APK, iOS IPA, PC .exe).
→ File → Build Settings → Build

**Bullet (Projectile)**
Thuật ngữ chung cho bất kỳ object bắn ra nào (arrows, bullets, fireballs).
→ Project này dùng ArrowProjectile cụ thể

**Button (UI)**
Unity UI component cho clickable buttons. Kích hoạt onClick events.

---

## C

**C#**
Ngôn ngữ lập trình được dùng bởi Unity. Phát âm "C Sharp".
→ File extension: .cs

**Cache**
Lưu trữ reference để tránh lookups lặp lại. Tối ưu hóa performance.
→ Ví dụ: `Awake() { rb = GetComponent<Rigidbody2D>(); }`

**Canvas**
Unity UI component chứa tất cả UI elements. Root của UI hierarchy.
→ Render modes: Screen Space Overlay, Camera, World Space

**CharacterManager**
Script spawn player character ở đầu level.
→ Vị trí: `Player/CharacterManager.cs`

**CheckTargetHelper**
Utility script để phát hiện targets dùng raycasts.
→ Được dùng bởi Player và Enemy để tìm targets

**CircleCast**
Physics query phát hiện colliders trong vùng hình tròn. Như raycast nhưng có width.
→ Ví dụ: `Physics2D.CircleCastAll(position, radius, ...)`

**Class**
Template để tạo objects. Định nghĩa properties và methods.
→ Ví dụ: `public class Player_Archer : Enemy { }`

**Collider**
Unity component định nghĩa collision shape (box, circle, polygon).
→ Types trong 2D: BoxCollider2D, CircleCollider2D, EdgeCollider2D

**Collision**
Tương tác vật lý giữa hai objects có colliders.
→ Triggers: `OnCollisionEnter2D()`, `OnCollisionStay2D()`, `OnCollisionExit2D()`

**Component**
Phần chức năng được gán vào GameObject (Transform, Renderer, Scripts).
→ Tất cả scripts đều là components

**Composition**
Design pattern: Xây dựng complex objects từ simple components.
→ GameObject + Component system của Unity dùng cái này

**Constructor**
Method được gọi khi tạo instance mới của class.
→ C# syntax: `public ClassName() { }`

**Controller2D**
Custom 2D physics controller dùng raycasts thay vì Rigidbody2D.
→ Vị trí: `Controllers/Controller2D.cs`
→ Kiểm soát chính xác hơn cho platformers/side-scrollers

**Coroutine**
Function có thể pause execution và resume sau. Dùng `yield`.
→ Ví dụ: `IEnumerator Countdown() { yield return new WaitForSeconds(1); }`

**Critical Hit**
Attack gây extra damage, thường lên specific body part (HEAD).
→ Được phát hiện qua BODYPART enum trong TakeDamage()

---

## D

**Damage Over Time (DoT)**
Damage được áp dụng dần dần trong khoảng thời gian (poison, burn).
→ Được triển khai trong Enemy.cs: BURNING, POISON effects

**Debug.Log()**
In message ra Unity Console để debugging.
→ Ví dụ: `Debug.Log("Health: " + health);`

**Debug.DrawRay()**
Hiển thị ray trong Scene view (không phải Game view). Hữu ích để debug raycasts.
→ Ví dụ: `Debug.DrawRay(start, direction * length, Color.red);`

**Delegate**
Reference đến method. Dùng cho events và callbacks.
→ Ví dụ: `public delegate void OnDeath();`

**Delta Time**
Thời gian kể từ frame trước. Dùng cho frame-independent movement.
→ Truy cập: `Time.deltaTime`
→ Ví dụ: `transform.position += velocity * Time.deltaTime;`

**Dependency**
Khi một system yêu cầu system khác để hoạt động.
→ Ví dụ: Player phụ thuộc vào Controller2D cho movement

**Destroy()**
Xóa GameObject khỏi scene và giải phóng memory.
→ Ví dụ: `Destroy(gameObject);`
→ Delayed: `Destroy(gameObject, 2f);` (sau 2 giây)

**DIEBEHAVIOR (Enum)**
Cách enemy hành xử khi chết: NORMAL, DESTROY, BLOWUP.

**DontDestroyOnLoad()**
Ngăn GameObject bị destroy khi load scene mới.
→ Ví dụ: `DontDestroyOnLoad(gameObject);`
→ Dùng cho persistent managers

---

## E

**Editor**
Ứng dụng Unity Editor nơi bạn thiết kế game.
→ Bao gồm: Scene view, Game view, Inspector, Hierarchy

**ENEMYEFFECT (Enum)**
Status effects: NONE, BURNING, FREEZE, SHOKING, POISON, EXPLOSION.
→ Được quản lý trong Enemy.cs

**ENEMYSTATE (Enum)**
Enemy state machine states: SPAWNING, IDLE, WALK, ATTACK, HIT, DEATH.

**Enemy**
Base class cho tất cả enemies VÀ player (lựa chọn thiết kế bất thường).
→ Vị trí: `AI/Enemy.cs`
→ Cung cấp: Health, damage, effects, state machine

**Event**
Hệ thống thông báo. Một object kích hoạt, objects khác lắng nghe.
→ Ví dụ: `public static event Action OnPlayerDeath;`

**Extension Method**
Thêm methods vào existing classes mà không modify chúng.
→ C# feature được dùng trong helpers

---

## F

**Field**
Biến được khai báo trong class. Có thể là public hoặc private.
→ Ví dụ: `public float speed = 5f;`

**FixedUpdate()**
MonoBehaviour method được gọi ở fixed intervals (50 FPS default).
→ Dùng để: Physics calculations
→ Nhất quán hơn Update() cho physics

**Flip**
Đảo hướng nhìn của nhân vật (horizontally).
→ Được triển khai bằng cách rotate sprite 180° trên Y-axis

**Float**
Kiểu số thập phân. Single-precision floating-point.
→ Ví dụ: `float speed = 5.5f;` (f suffix bắt buộc)

**FloatingText**
Số damage xuất hiện phía trên nhân vật.
→ Vị trí: `UI/FloatingText.cs`, `UI/FloatingTextManager.cs`
→ Dùng object pooling cho performance

**Fortress (TheFortrest)**
Căn cứ của player phải được bảo vệ. Game over nếu bị phá hủy.
→ Vị trí: `AI/TheFortrest.cs`

**FPS (Frames Per Second)**
Bao nhiêu frames được render mỗi giây. Cao hơn = mượt hơn.
→ Target: 60 FPS (được set trong GameManager.Awake)

**Function**
Block code thực hiện task. Còn được gọi là "method" trong OOP.

---

## G

**GameObject**
Container object trong Unity scenes. Mọi thứ đều là một GameObject.
→ Luôn có Transform component
→ Có thể có nhiều components attached

**GameManager**
Singleton điều khiển game state, listeners, level loading.
→ Vị trí: `Managers/GameManager.cs`
→ Truy cập: `GameManager.Instance`

**GameState (Enum)**
Game state hiện tại: Menu, Playing, GameOver, Success, Pause.

**Garbage Collection (GC)**
Dọn dẹp memory tự động. Có thể gây frame drops nếu quá nhiều.
→ Tránh: Tạo objects trong Update()

**GetComponent**
Lấy component từ GameObject.
→ Ví dụ: `GetComponent<Rigidbody2D>()`
→ Chậm - cache results trong Awake/Start

**Gizmos**
Visual debugging helpers được vẽ trong Scene view.
→ Ví dụ: `OnDrawGizmos() { Gizmos.DrawWireSphere(...); }`

**GlobalValue**
Static class lưu trữ player progress data qua PlayerPrefs.
→ Vị trí: `Helpers/GlobalValue.cs`
→ Chứa: Coins, level progress, unlocks

**Gravity**
Lực hướng xuống áp dụng lên objects. Có thể modify per-object.
→ Player/Enemy: `gravity = 35f` field
→ Projectiles: `gravityScale` parameter

---

## H

**HealthBar**
UI element hiển thị máu hiện tại. Theo dõi enemy/player.
→ Vị trí: `UI/HealthBarEnemyNew.cs`

**Header Attribute**
Tổ chức Inspector bằng cách thêm section headers.
→ Ví dụ: `[Header("Movement Settings")]`

**Hierarchy**
Unity window hiển thị tất cả GameObjects trong scene hiện tại.
→ Cấu trúc cây (parent-child relationships)

**Hit Reaction**
Visual/audio feedback khi nhận sát thương.
→ Được triển khai trong Enemy.Hit() và Player_Archer.Hit()

**HideInInspector Attribute**
Ẩn public field khỏi Inspector.
→ Ví dụ: `[HideInInspector] public bool isDead;`

---

## I

**IAP (In-App Purchase)**
Mua items bằng tiền thật trong mobile games.
→ Vị trí: `Managers/Purchaser.cs`, `Managers/IAPItem.cs`

**ICanTakeDamage**
Interface cho objects có thể nhận damage.
→ Vị trí: `AI/ICanTakeDamage.cs`
→ Được triển khai bởi: Enemy, Player, Fortress

**IDE (Integrated Development Environment)**
Code editor với advanced features. Unity dùng Visual Studio hoặc Rider.

**IEnumerator**
Return type cho coroutines. Cho phép yield statements.
→ Ví dụ: `IEnumerator MyCoroutine() { yield return null; }`

**IListener**
Interface cho game state event listeners.
→ Vị trí: `Helpers/IListener.cs`
→ Methods: IPlay(), IPause(), IUnPause(), IGameOver(), ISuccess()

**Inheritance**
Class dẫn xuất từ class khác, nhận properties/methods của nó.
→ Ví dụ: `Player_Archer : Enemy`

**Inspector**
Unity window hiển thị properties của GameObject/asset được chọn.
→ Modify public fields ở đây

**Instantiate()**
Tạo bản sao của GameObject hoặc prefab tại runtime.
→ Ví dụ: `Instantiate(enemyPrefab, position, rotation);`

**Integer (int)**
Số nguyên (không có số thập phân).
→ Ví dụ: `int health = 100;`

**Interface**
Contract định nghĩa methods mà class phải implement.
→ Ví dụ: `public interface ICanTakeDamage { void TakeDamage(...); }`

---

## J

**JSON**
Text format để lưu trữ data. Dùng cho save files.
→ Ví dụ: `JsonUtility.ToJson(object)`

---

## K

**KeyCode**
Enum của tất cả keyboard keys. Dùng với Input.GetKey().
→ Ví dụ: `Input.GetKey(KeyCode.Space)`

**Knockback**
Lực đẩy nhân vật về phía sau khi bị đánh.
→ Được triển khai trong Enemy.KnockBack() và Player_Archer.PushBack()

---

## L

**LateUpdate()**
MonoBehaviour method được gọi sau tất cả Update() methods.
→ Dùng để: Camera following (sau khi player moved)

**Layer**
Phân loại cho GameObjects. Dùng cho collision filtering và raycasts.
→ Ví dụ: "Player" layer, "Enemy" layer, "Ground" layer

**LayerMask**
Filter cho raycasts chỉ định layers nào để detect.
→ Ví dụ: `GameManager.Instance.layerEnemy`

**Lerp (Linear Interpolation)**
Chuyển tiếp mượt mà giữa hai giá trị.
→ Ví dụ: `Mathf.Lerp(current, target, t);` với t = 0-1

**Level**
Stage hoặc mission của game. Mỗi level có enemies và configuration khác nhau.
→ Được quản lý bởi LevelManager và LevelEnemyManager

**LevelEnemyManager**
Spawn enemy waves cho level hiện tại.
→ Vị trí: `Managers/LevelEnemyManager.cs`

**Lifecycle**
Chuỗi method calls Unity thực hiện trên MonoBehaviour:
→ Awake → OnEnable → Start → Update/FixedUpdate/LateUpdate → OnDisable → OnDestroy

**Listener Pattern (Observer)**
Design pattern mà objects subscribe đến events.
→ Implementation: IListener interface + GameManager.listeners list

---

## M

**Method**
Function bên trong một class.
→ Ví dụ: `public void TakeDamage(int amount) { }`

**MonoBehaviour**
Base class cho tất cả Unity scripts. Cung cấp lifecycle methods.
→ Tất cả game scripts kế thừa từ cái này

**Multiplier**
Giá trị scale giá trị khác.
→ Ví dụ: `damage * critMultiplier`

---

## N

**Namespace**
Container cho classes để tránh name conflicts.
→ Project này: `namespace RGame { }`

**Normalize**
Scale vector đến length là 1 trong khi giữ direction.
→ Ví dụ: `direction.normalized`

**Null**
Sự vắng mặt của giá trị. Kiểm tra với `== null`.
→ Ví dụ: `if (target == null) return;`

**NullReferenceException**
Error khi truy cập biến là null.
→ Nguyên nhân phổ biến: Quên assign reference trong Inspector

---

## O

**Object Pooling**
Tái sử dụng objects thay vì Instantiate/Destroy cho performance.
→ Implementation: FloatingTextManager

**OnCollisionEnter2D()**
Được gọi khi collision bắt đầu (solid colliders).
→ Ví dụ: `void OnCollisionEnter2D(Collision2D collision) { }`

**OnDestroy()**
Được gọi khi GameObject bị destroy.
→ Dùng để: Cleanup, unsubscribe từ events

**OnDrawGizmos()**
Vẽ debugging visuals trong Scene view.
→ Ví dụ: Vẽ detection range circle

**OnTriggerEnter2D()**
Được gọi khi trigger area bị overlapped (isTrigger = true).
→ Ví dụ: Coin collection, damage zones

**Override**
Thay thế parent class method với implementation mới.
→ Ví dụ: `public override void Die() { base.Die(); ... }`

---

## P

**Parameter**
Input value cho method.
→ Ví dụ: `void Move(float speed)` - speed là parameter

**Parent-Child**
Hierarchy relationship. Transform của child relative với parent.
→ Ví dụ: Hand là child của body

**Physics2D**
Unity system cho 2D physics (collisions, raycasts, forces).

**PlayerPrefs**
Simple key-value storage để saving data (coins, progress).
→ Ví dụ: `PlayerPrefs.SetInt("Coins", 100);`

**Player_Archer**
Main player class. Kế thừa từ Enemy (bất thường!).
→ Vị trí: `Player/Player_Archer.cs`

**Polymorphism**
Các classes khác nhau có thể được coi là cùng type qua interface/base class.
→ Ví dụ: Tất cả enemies implement ICanTakeDamage

**Prefab**
Reusable template GameObject. Được lưu như asset, instantiated tại runtime.
→ Ví dụ: Enemy prefab spawned nhiều lần

**Private**
Chỉ truy cập được trong cùng class. Ẩn khỏi Inspector.
→ Ví dụ: `private bool isDead;`

**Projectile**
Object bắn qua không khí (arrow, bullet, fireball).
→ Base class: `Controllers/Projectile.cs`

**Public**
Truy cập được từ classes khác. Hiển thị trong Inspector.
→ Ví dụ: `public float speed = 5f;`

---

## Q

**Quaternion**
Biểu diễn rotation. Tránh gimbal lock.
→ Ví dụ: `Quaternion.identity` (không rotation)
→ Đừng manually create - dùng Quaternion.Euler(x, y, z)

---

## R

**Range Attribute**
Tạo slider trong Inspector cho numeric values.
→ Ví dụ: `[Range(0, 100)] public int health;`

**Raycast**
Invisible ray kiểm tra collisions.
→ Ví dụ: `Physics2D.Raycast(origin, direction, distance);`

**RaycastController**
Base class cho raycast-based physics.
→ Vị trí: `Controllers/RaycastController.cs`

**ReadOnly Attribute**
Làm field visible nhưng uneditable trong Inspector.
→ Custom attribute trong project này

**RectTransform**
Transform cho UI elements. Có anchors, pivot, sizeDelta.

**Reference**
Link đến object khác.
→ Ví dụ: `public GameObject target;` - kéo object vào Inspector

**Renderer**
Component vẽ visuals (SpriteRenderer, MeshRenderer).

**Resources**
Thư mục Unity đặc biệt - assets có thể load tại runtime.
→ Ví dụ: `Resources.Load<GameObject>("Prefabs/Enemy");`

**Return**
Thoát method và tùy chọn cung cấp value.
→ Ví dụ: `return health > 0;`

**Rigidbody2D**
Unity component cho 2D physics simulation.
→ Project này dùng custom Controller2D thay thế

**RGame**
Namespace chứa tất cả game code trong project này.

---

## S

**Scene**
Container cho game level hoặc menu. File với .unity extension.
→ Project này: SampleScene.unity

**ScriptableObject**
Data-only asset. Không có MonoBehaviour overhead.
→ Ví dụ: UpgradedCharacterParameter

**Serialization**
Convert object thành storable format (JSON, binary).
→ Cho phép saving ra disk

**SerializeField Attribute**
Làm private field visible trong Inspector.
→ Ví dụ: `[SerializeField] private int coins;`

**Singleton**
Design pattern đảm bảo chỉ có một instance tồn tại.
→ Ví dụ: GameManager.Instance, SoundManager.Instance

**SmartEnemyGrounded**
Main enemy implementation cho ground-based enemies.
→ Vị trí: `AI/SmartEnemyGrounded.cs`

**SoundManager**
Singleton quản lý audio playback.
→ Vị trí: `Managers/SoundManager.cs`

**Spawn**
Tạo GameObject instance mới.
→ Thường qua Instantiate()

**SpriteRenderer**
Component hiển thị 2D image (sprite).

**Start()**
MonoBehaviour method được gọi trước first frame.
→ Được gọi sau tất cả Awake() methods
→ Dùng để: Initialization yêu cầu objects khác

**StartCoroutine()**
Bắt đầu chạy coroutine.
→ Ví dụ: `StartCoroutine(WaitAndAct());`

**State Machine**
Hệ thống mà object có thể ở một state tại một thời điểm.
→ Ví dụ: Enemy states (IDLE, WALK, ATTACK, DEATH)

**Static**
Thuộc về class, không phải instance. Được chia sẻ bởi tất cả objects.
→ Ví dụ: `public static GameManager Instance;`

**String**
Kiểu dữ liệu text.
→ Ví dụ: `string name = "Player";`

**Stun**
Vô hiệu hóa tạm thời nhân vật (không thể move/act).
→ Được triển khai trong Enemy.Stun() và Player_Archer.Stun()

---

## T

**Tag**
Label đơn giản cho GameObjects. Dùng để nhận diện nhanh.
→ Ví dụ: GameObject.FindGameObjectWithTag("Player")

**TakeDamage()**
Method để áp dụng damage lên nhân vật.
→ Interface: ICanTakeDamage
→ Implementation: Enemy.cs

**Target**
Object đang được nhắm vào hoặc theo dõi.
→ Ví dụ: Enemy targets player, player targets enemy

**this**
Reference đến current object instance.
→ Ví dụ: `this.transform.position`

**Time.deltaTime**
Giây kể từ frame trước. Cần thiết cho smooth movement.
→ Ví dụ: `position += velocity * Time.deltaTime;`

**Time.fixedDeltaTime**
Thời gian giữa các FixedUpdate calls. Giá trị constant.

**Time.time**
Thời gian kể từ khi game bắt đầu tính bằng giây.

**Tooltip Attribute**
Thêm hover tooltip trong Inspector.
→ Ví dụ: `[Tooltip("Speed in units per second")] public float speed;`

**Tower Defense**
Thể loại game mà player bảo vệ chống lại waves của enemies.
→ Project này là tower defense hybrid

**Trajectory**
Đường cong của projectile.
→ Player tính optimal trajectory cho arrows

**Transform**
Component lưu position, rotation, scale. Mọi GameObject đều có một.

**Trigger**
Collider phát hiện overlap không có physical collision.
→ Set: isTrigger = true
→ Events: OnTriggerEnter2D, OnTriggerStay2D, OnTriggerExit2D

---

## U

**UI (User Interface)**
Menus, buttons, health bars - bất cứ thứ gì player nhìn thấy trên màn hình.
→ Unity UI dùng Canvas system

**Unity**
Game engine mà project này dùng. Được tạo bởi Unity Technologies.

**Unity Ads**
Nền tảng quảng cáo của Unity. Được tích hợp trong project này.
→ Vị trí: `AdController/UnityAds.cs`

**Update()**
MonoBehaviour method được gọi mỗi frame.
→ Dùng để: Input, non-physics logic
→ ~60 calls mỗi giây (60 FPS)

**UpgradedCharacterParameter**
ScriptableObject lưu character upgrade stats.
→ Vị trí: `Player/UpgradedCharacterParameter.cs`

---

## V

**Variable**
Named storage cho data.
→ Ví dụ: `int health = 100;`

**Vector2**
2D coordinate (x, y).
→ Ví dụ: `Vector2 position = new Vector2(5, 3);`

**Vector3**
3D coordinate (x, y, z).
→ Được dùng trong 2D games với z = 0

**Velocity**
Speed và direction của movement.
→ Ví dụ: `velocity = direction * speed;`

**Virtual**
Method có thể overridden bởi child classes.
→ Ví dụ: `public virtual void Die() { }`

**Void**
Return type chỉ ra method không return gì.
→ Ví dụ: `void Start() { }`

---

## W

**Wave**
Nhóm enemies được spawn cùng nhau.
→ Được cấu hình trong LevelWave.cs

**WeaponEffect**
Data class định nghĩa weapon effects (poison, burn, freeze).
→ Vị trí: `Helpers/WeaponEffect.cs`

**while**
Loop tiếp tục trong khi condition là true.
→ Ví dụ: `while (health > 0) { }`

---

## X

*Không có Unity/project terms phổ biến bắt đầu bằng X*

---

## Y

**yield**
Keyword cho coroutines. Pause execution.
→ `yield return null;` - Đợi một frame
→ `yield return new WaitForSeconds(2);` - Đợi 2 giây

---

## Z

*Không có Unity/project terms phổ biến bắt đầu bằng Z*

---

## Ký Tự Đặc Biệt & Syntax

**→** (Arrow/Mũi Tên)
Chỉ "xem thêm" hoặc ví dụ.

**{ }** (Braces/Ngoặc Nhọn)
Code block delimiters.
→ Ví dụ: `if (condition) { code here }`

**[ ]** (Brackets/Ngoặc Vuông)
Array hoặc attribute syntax.
→ Ví dụ: `int[] numbers` hoặc `[SerializeField]`

**( )** (Parentheses/Ngoặc Đơn)
Method parameters hoặc mathematical grouping.
→ Ví dụ: `Move(5)` hoặc `(a + b) * c`

**;** (Semicolon/Dấu Chấm Phẩy)
Statement terminator trong C#.
→ Ví dụ: `int x = 5;`

**//** (Double Slash)
Single-line comment.
→ Ví dụ: `// This is a comment`

**/* */** (Slash-Asterisk)
Multi-line comment.
→ Ví dụ: `/* Multiple lines of comments */`

**::** (Double Colon)
Namespace separator.
→ Ví dụ: `UnityEngine.GameObject`

**??** (Null-Coalescing)
Cung cấp default nếu null.
→ Ví dụ: `value = nullableValue ?? default;`

**?.** (Null-Conditional)
Truy cập member chỉ nếu không null.
→ Ví dụ: `player?.TakeDamage(10);`

---

## Các Viết Tắt Phổ Biến (Common Abbreviations)

**AI** - Artificial Intelligence (Trí Tuệ Nhân Tạo)
**API** - Application Programming Interface
**CPU** - Central Processing Unit
**DOF** - Depth of Field
**DoT** - Damage over Time (Sát Thương Theo Thời Gian)
**FPS** - Frames Per Second (Khung Hình Mỗi Giây)
**GC** - Garbage Collection (Dọn Rác)
**GPU** - Graphics Processing Unit
**GUI** - Graphical User Interface
**HP** - Health Points / Hit Points (Điểm Máu)
**IAP** - In-App Purchase (Mua Trong Ứng Dụng)
**IDE** - Integrated Development Environment
**JSON** - JavaScript Object Notation
**NPC** - Non-Player Character (Nhân Vật Không Phải Người Chơi)
**OOP** - Object-Oriented Programming (Lập Trình Hướng Đối Tượng)
**SDK** - Software Development Kit
**SFX** - Sound Effects (Hiệu Ứng Âm Thanh)
**UI** - User Interface (Giao Diện Người Dùng)
**VFX** - Visual Effects (Hiệu Ứng Hình Ảnh)
**XML** - Extensible Markup Language

---

## Tham Khảo Nhanh: Common Code Patterns

```csharp
// Get component
Rigidbody2D rb = GetComponent<Rigidbody2D>();

// Find GameObject
GameObject player = GameObject.FindGameObjectWithTag("Player");

// Instantiate prefab
Instantiate(prefab, position, Quaternion.identity);

// Destroy object
Destroy(gameObject);

// Wait in coroutine
yield return new WaitForSeconds(2f);

// Check null
if (target == null) return;

// Access singleton
GameManager.Instance.Victory();

// Trigger event
OnPlayerDeath?.Invoke();

// Loop through array
foreach (var item in items) { }

// Debug log
Debug.Log("Message: " + value);
```

---

## Cách Sử Dụng Từ Điển Này:

1. **Ctrl+F** để tìm kiếm thuật ngữ
2. **Bookmark** trang này để truy cập nhanh
3. **Đọc định nghĩa + ví dụ** để hiểu
4. **Theo mũi tên (→)** cho thông tin liên quan

**Không tìm thấy thuật ngữ?**
- Kiểm tra documentation index
- Tìm trong project code
- Hỏi trong Unity forums
- Kiểm tra Unity manual

---

**Phiên Bản Tài Liệu**: 2.0
**Cập Nhật Lần Cuối**: Tháng 10 năm 2025
**Số Thuật Ngữ**: 200+ định nghĩa Unity và project-specific

---

**Kết Thúc Tài Liệu**

<p align="center">
<strong>Lawn Defense: Monsters Out</strong><br>
Từ Điển Thuật Ngữ<br>
Unity & Project Terms Glossary
</p>
