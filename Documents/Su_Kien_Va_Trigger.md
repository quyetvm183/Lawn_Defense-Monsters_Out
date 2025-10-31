# Hệ Thống Events và Triggers

**Hướng Dẫn Hoàn Chỉnh Về Event Communication và Trigger Points**

---

## Mục Lục

1. [Tổng Quan](#tổng-quan)
2. [Unity Basics: Events Được Giải Thích](#unity-basics-events-được-giải-thích)
3. [Hệ Thống 1: Observer Pattern (IListener)](#hệ-thống-1-observer-pattern-ilistener)
4. [Hệ Thống 2: Animation Events](#hệ-thống-2-animation-events)
5. [Hệ Thống 3: Collision và Trigger Events](#hệ-thống-3-collision-và-trigger-events)
6. [Hệ Thống 4: UI Button Events](#hệ-thống-4-ui-button-events)
7. [Hệ Thống 5: Custom Game Events](#hệ-thống-5-custom-game-events)
8. [Sơ Đồ Luồng Event Hoàn Chỉnh](#sơ-đồ-luồng-event-hoàn-chỉnh)
9. [Debug Hệ Thống Event](#debug-hệ-thống-event)
10. [Khắc Phục Sự Cố](#khắc-phục-sự-cố)
11. [Tài Liệu Tham Khảo Chéo](#tài-liệu-tham-khảo-chéo)

---

## Tổng Quan

**Mục Đích**: Tài liệu này giải thích tất cả các hệ thống event và trigger trong game - cách objects giao tiếp, phản ứng với thay đổi state, và phối hợp hành vi.

**Bạn Sẽ Học Được**:
- Cách Observer pattern (IListener) broadcast game state changes
- Cách Animation Events kích hoạt code trong animations
- Cách collision detection hoạt động với projectiles và triggers
- Cách UI buttons kết nối với game logic
- Cách debug các vấn đề event flow

**Thời Gian Đọc**: 20-25 phút
**Độ Khó**: Trung bình

---

## Unity Basics: Events Được Giải Thích

### Event Là Gì?

Một **event** là điều gì đó xảy ra tại một thời điểm cụ thể mà các objects khác cần biết.

**Ví Dụ Thực Tế**: Nghĩ về chuông cửa:
- **Event**: Ai đó nhấn nút chuông
- **Listener**: Tiếng chuông phát ra âm thanh
- **Broadcast**: Tín hiệu điện từ nút đến chuông

**Trong Games**:
- **Event**: "Player clicked Play button"
- **Listeners**: Tất cả enemies, UI panels, audio manager
- **Broadcast**: GameManager báo cho mọi người "game started"

### Unity Event Types

Project này dùng **5 hệ thống events khác nhau**:

1. **Observer Pattern** (IListener) - Broadcast đến nhiều objects
2. **Animation Events** - Code được kích hoạt bởi animation frames
3. **Collision Events** (OnTriggerEnter2D) - Phát hiện tiếp xúc vật lý
4. **UI Events** (Button.onClick) - Tương tác user interface
5. **Custom Events** (Invoke, Coroutines) - Actions bị trễ theo thời gian

### Tại Sao Nhiều Hệ Thống?

Các vấn đề khác nhau cần giải pháp khác nhau:

**Observer Pattern**: Khi nhiều objects cần phản ứng với cùng một event
- ✅ Ví dụ: Game paused → tất cả enemies dừng, UI hiển thị pause menu, music fade

**Animation Events**: Khi code phải chạy tại exact animation frame
- ✅ Ví dụ: Sword swing animation → spawn hit detection tại frame 5

**Collision Events**: Khi objects chạm vật lý
- ✅ Ví dụ: Arrow hits enemy → apply damage

**UI Events**: Khi player clicks buttons
- ✅ Ví dụ: Click "Buy" → purchase character

**Custom Events**: Khi điều gì đó xảy ra sau một delay
- ✅ Ví dụ: Enemy dies → wait 2 seconds → spawn next wave

---

## Hệ Thống 1: Observer Pattern (IListener)

### Observer Pattern Là Gì?

Observer pattern cho phép một object (the **subject**) thông báo cho nhiều objects (the **observers**) khi điều gì đó xảy ra.

**Ví Dụ**: Newsletter subscription
- **Subject**: Newsletter publisher (GameManager)
- **Observers**: Subscribers (Enemies, UI, Managers)
- **Event**: New newsletter published → tất cả subscribers nhận được

### IListener Interface

```csharp
public interface IListener
{
    void IPlay();              // Game started
    void ISuccess();           // Player won
    void IGameOver();          // Player lost
    void IPause();             // Game paused
    void IUnPause();           // Game resumed
    void IOnRespawn();         // Player respawned
    void IOnStopMovingOn();    // Stop all movement
    void IOnStopMovingOff();   // Resume movement
}
```

**Cách hoạt động**:
1. Objects **implement IListener** (subscribe to newsletter)
2. Objects **register với GameManager** (give their email address)
3. GameManager **broadcasts events** (sends newsletter to all subscribers)
4. Objects **receive notification** và phản ứng

### Ví Dụ Triển Khai: Enemy

```csharp
using UnityEngine;
using RGame;

public class Enemy : MonoBehaviour, IListener
{
    public bool isPlaying { get; set; }

    void OnEnable()
    {
        // Subscribe to events
        GameManager.Instance.AddListener(this);
        isPlaying = true;
    }

    void OnDestroy()
    {
        // Unsubscribe when destroyed
        GameManager.Instance?.RemoveListener(this);
    }

    // ===== EVENT HANDLERS =====

    public virtual void IPlay()
    {
        // Called when game starts
        isPlaying = true;
        Debug.Log($"{gameObject.name}: Game started, begin AI");
    }

    public virtual void ISuccess()
    {
        // Called when player wins
        isPlaying = false;
        StopAllCoroutines();
        Debug.Log($"{gameObject.name}: Player won, stop attacking");
    }

    public virtual void IGameOver()
    {
        // Called when player loses
        isPlaying = false;
        PlayVictoryAnimation();  // Enemy celebrates
        Debug.Log($"{gameObject.name}: Player lost, celebrate!");
    }

    public virtual void IPause()
    {
        // Called when game paused
        isPlaying = false;
        Debug.Log($"{gameObject.name}: Game paused");
    }

    public virtual void IUnPause()
    {
        // Called when game resumed
        isPlaying = true;
        Debug.Log($"{gameObject.name}: Game resumed");
    }

    public virtual void IOnRespawn()
    {
        // Called when player respawns (not commonly used)
    }

    public virtual void IOnStopMovingOn()
    {
        // Called to force stop all movement
    }

    public virtual void IOnStopMovingOff()
    {
        // Called to allow movement again
    }
}
```

### GameManager Broadcasting

```csharp
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private List<IListener> listeners = new List<IListener>();

    // Register a listener
    public void AddListener(IListener listener)
    {
        if (!listeners.Contains(listener))
        {
            listeners.Add(listener);
        }
    }

    // Unregister a listener
    public void RemoveListener(IListener listener)
    {
        if (listeners.Contains(listener))
        {
            listeners.Remove(listener);
        }
    }

    // Broadcast: Game Started
    public void StartGame()
    {
        State = GameState.Playing;

        foreach (var listener in listeners)
        {
            if (listener != null)
                listener.IPlay();  // ← Call IPlay() on everyone
        }
    }

    // Broadcast: Player Won
    public void Victory()
    {
        State = GameState.Success;

        foreach (var listener in listeners)
        {
            if (listener != null)
                listener.ISuccess();  // ← Call ISuccess() on everyone
        }
    }

    // Broadcast: Player Lost
    public void GameOver()
    {
        State = GameState.Fail;

        foreach (var listener in listeners)
        {
            if (listener != null)
                listener.IGameOver();  // ← Call IGameOver() on everyone
        }
    }

    // Broadcast: Game Paused
    public void Pause()
    {
        Time.timeScale = 0;  // Freeze time
        State = GameState.Pause;

        foreach (var listener in listeners)
        {
            if (listener != null)
                listener.IPause();
        }
    }

    // Broadcast: Game Resumed
    public void UnPause()
    {
        Time.timeScale = 1;  // Resume time
        State = GameState.Playing;

        foreach (var listener in listeners)
        {
            if (listener != null)
                listener.IUnPause();
        }
    }
}
```

### Observer Pattern Flow

```
                    ┌──────────────────┐
                    │  GameManager     │
                    │  (Subject)       │
                    └────────┬─────────┘
                             │
         ┌───────────────────┼───────────────────┐
         │                   │                   │
         v                   v                   v
  ┌─────────────┐    ┌─────────────┐    ┌─────────────┐
  │  Enemy1     │    │  Enemy2     │    │  UI Panel   │
  │ (Observer)  │    │ (Observer)  │    │ (Observer)  │
  └─────────────┘    └─────────────┘    └─────────────┘
         │                   │                   │
         │  implements       │  implements       │  implements
         │  IListener        │  IListener        │  IListener
         v                   v                   v
    IPlay()              IPlay()              IPlay()
    ISuccess()           ISuccess()           ISuccess()
    IGameOver()          IGameOver()          IGameOver()
    IPause()             IPause()             IPause()

When GameManager.StartGame() is called:
  1. GameManager loops through all listeners
  2. Calls listener.IPlay() on each one
  3. Each listener executes its IPlay() method
```

### Ví Dụ Thực Tế: Custom Listener

Hãy tạo một custom object lắng nghe game events:

```csharp
using UnityEngine;
using RGame;

public class BackgroundMusic : MonoBehaviour, IListener
{
    public AudioClip menuMusic;
    public AudioClip gameMusic;
    public AudioClip victoryMusic;

    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        GameManager.Instance.AddListener(this);
    }

    void OnDestroy()
    {
        GameManager.Instance?.RemoveListener(this);
    }

    public void IPlay()
    {
        // Game started - play game music
        audioSource.clip = gameMusic;
        audioSource.loop = true;
        audioSource.Play();
        Debug.Log("Playing game music");
    }

    public void ISuccess()
    {
        // Player won - play victory music
        audioSource.loop = false;
        audioSource.clip = victoryMusic;
        audioSource.Play();
        Debug.Log("Playing victory music");
    }

    public void IGameOver()
    {
        // Player lost - silence
        audioSource.Stop();
        Debug.Log("Stopped music");
    }

    public void IPause()
    {
        // Game paused - pause music
        audioSource.Pause();
    }

    public void IUnPause()
    {
        // Game resumed - resume music
        audioSource.UnPause();
    }

    // Not used in this example
    public void IOnRespawn() { }
    public void IOnStopMovingOn() { }
    public void IOnStopMovingOff() { }
}
```

**Cách sử dụng**: Gắn script này vào GameObject với AudioSource component. Nó sẽ tự động phản ứng với game state changes.

---

## Hệ Thống 2: Animation Events

### Animation Events Là Gì?

**Animation Events** cho phép bạn gọi code tại các frames cụ thể trong animation.

**Ví Dụ Use Cases**:
- Spawn sword hit effect khi swing đến giữa
- Play footstep sound khi foot chạm đất
- Shoot arrow khi bow release animation plays

### Cách Animation Events Hoạt Động

```
Animation Timeline:
Frame 0 ────────> Frame 10 ────────> Frame 20 ────────> Frame 30
   │                  │                  │                  │
   └─ "start"         └─ Event:         └─ "loop"         └─ "end"
      trigger            "AnimShoot()"
                            │
                            v
                    [Code executes]
                    ShootArrow()
```

### Thiết Lập Animation Events

**Bước 1: Mở Animation Window**
1. Chọn character prefab
2. Window → Animation → Animation
3. Chọn animation clip (e.g., "Attack")

**Bước 2: Thêm Event Marker**
1. Di chuyển timeline scrubber đến frame mong muốn
2. Click nút "Add Event" (hoặc right-click timeline)
3. Một marker màu trắng xuất hiện trên timeline

**Bước 3: Assign Function**
1. Chọn event marker
2. Trong Inspector, chọn function name từ dropdown
3. Function phải là **public** và tồn tại trên component gắn vào GameObject

### Ví Dụ: Enemy Attack Animation

```csharp
using UnityEngine;
using RGame;

public class EnemyWarrior : Enemy
{
    public GameObject swordSlashEffect;
    public Transform attackPoint;

    // ===== ANIMATION EVENTS =====
    // These methods are called by animation events

    public void AnimMeleeAttackStart()
    {
        // Called at start of attack animation
        Debug.Log("Attack animation started");
    }

    public void AnimMeleeAttack()
    {
        // Called when sword swings (middle of animation)
        SpawnHitbox();
        PlaySlashEffect();
        Debug.Log("Spawned attack hitbox");
    }

    public void AnimMeleeAttackEnd()
    {
        // Called at end of attack animation
        SetEnemyState(ENEMYSTATE.WALK);
        Debug.Log("Attack animation finished, resume walking");
    }

    void SpawnHitbox()
    {
        // Detect enemies in attack range
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            playerLayer
        );

        foreach (var hit in hits)
        {
            ICanTakeDamage target = hit.GetComponent<ICanTakeDamage>();
            if (target != null)
            {
                target.TakeDamage(meleeDamage, Vector2.zero, hit.transform.position, gameObject);
            }
        }
    }

    void PlaySlashEffect()
    {
        if (swordSlashEffect != null)
        {
            Instantiate(swordSlashEffect, attackPoint.position, attackPoint.rotation);
        }
    }
}
```

**Animation Setup** (trong Unity Animator):
```
"Attack" animation clip:
Frame 0:  [No event]
Frame 3:  Event → AnimMeleeAttackStart()
Frame 12: Event → AnimMeleeAttack()       ← Sword hits here
Frame 25: Event → AnimMeleeAttackEnd()
```

### Animation Event Methods Phổ Biến

**Enemy Scripts**:
- `AnimMeleeAttackStart()` - Start melee attack
- `AnimMeleeAttack()` - Execute melee damage
- `AnimShoot()` - Fire projectile
- `AnimThrow()` - Throw grenade/bomb
- `AnimDie()` - Destroy GameObject after death animation

**Player Scripts**:
- `AnimAttack()` - Execute player attack
- `AnimFootstep()` - Play footstep sound
- `AnimReload()` - Reload weapon

**Ví Dụ: Death Animation với Cleanup**

```csharp
public class Enemy : MonoBehaviour
{
    public void Die()
    {
        SetEnemyState(ENEMYSTATE.DEATH);
        AnimSetTrigger("die");

        // Animation will call AnimDie() at the end
    }

    // Called by animation event at end of death animation
    public void AnimDie()
    {
        Destroy(gameObject);  // Remove enemy from scene
    }
}
```

---

## Hệ Thống 3: Collision và Trigger Events

### Physics-Based Events

Unity's 2D physics system fires events khi objects collide hoặc overlap.

**Hai Loại**:
1. **Colliders** - Physical collision (objects bounce)
2. **Triggers** - Overlap detection (no physics reaction)

### OnTriggerEnter2D - Overlap Detection

Dùng cho: Projectiles, pickups, damage zones

```csharp
void OnTriggerEnter2D(Collider2D other)
{
    // Called when this object's trigger overlaps another collider
    Debug.Log($"Triggered by: {other.gameObject.name}");
}
```

**Yêu Cầu**:
- Ít nhất một object có **Is Trigger** checked
- Cả hai objects có **Collider2D** component
- Ít nhất một object có **Rigidbody2D** component

### Ví Dụ Projectile Collision

```csharp
using UnityEngine;
using RGame;

public class Arrow : MonoBehaviour
{
    public int damage = 10;
    public GameObject hitEffect;
    public LayerMask damageableLayers;  // What can be damaged
    public WeaponEffect weaponEffect;   // Poison, freeze, etc.

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object is on a damageable layer
        if (((1 << other.gameObject.layer) & damageableLayers) == 0)
        {
            // Not a target layer, ignore
            return;
        }

        // Try to damage the object
        ICanTakeDamage target = other.GetComponent<ICanTakeDamage>();
        if (target != null)
        {
            // Calculate hit point and direction
            Vector2 hitPoint = other.ClosestPoint(transform.position);
            Vector2 direction = (hitPoint - (Vector2)transform.position).normalized;
            Vector2 knockbackForce = direction * 5f;

            // Apply damage
            target.TakeDamage(
                damage,
                knockbackForce,
                hitPoint,
                gameObject,
                BODYPART.NONE,
                weaponEffect
            );

            Debug.Log($"Arrow hit {other.gameObject.name} for {damage} damage");
        }
        else
        {
            Debug.Log($"Arrow hit {other.gameObject.name} but it can't take damage");
        }

        // Spawn hit effect
        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }

        // Destroy arrow
        Destroy(gameObject);
    }
}
```

### Layer-Based Collision Filtering

**Vấn Đề**: Arrows không nên đánh trúng walls, allies, hoặc arrows khác.

**Giải Pháp**: Dùng LayerMask để lọc targets

```csharp
public class ArrowScript : MonoBehaviour
{
    public LayerMask damageableLayers;  // Set in Inspector

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if object is on a valid layer
        int objectLayer = other.gameObject.layer;
        int layerMask = 1 << objectLayer;

        if ((layerMask & damageableLayers.value) == 0)
        {
            return;  // Not a target, ignore
        }

        // Proceed with damage...
    }
}
```

**Inspector Setup**:
```
ArrowScript Component:
  Damageable Layers:
    ☑ Enemy
    ☐ Player
    ☐ Ground
    ☐ Wall
```

### ICanTakeDamage Interface

Objects có thể bị damage phải implement interface này:

```csharp
public interface ICanTakeDamage
{
    void TakeDamage(
        float damage,
        Vector2 force,
        Vector2 hitPoint,
        GameObject instigator,
        BODYPART bodyPart = BODYPART.NONE,
        WeaponEffect weaponEffect = null
    );
}
```

**Implemented bởi**:
- `Enemy.cs`
- `Player_Archer.cs`
- `TheFortrest.cs` (player fortress)

### Sơ Đồ Luồng Collision

```
┌─────────────┐
│ Arrow       │
│ (Trigger)   │
└──────┬──────┘
       │
       │ Overlaps
       v
┌─────────────┐
│ Enemy       │
│ (Collider)  │
└──────┬──────┘
       │
       │ OnTriggerEnter2D called
       v
┌──────────────────────────┐
│ Check Layer              │
│ Is object damageable?    │
└──────┬───────────────────┘
       │
       │ Yes
       v
┌──────────────────────────┐
│ Get ICanTakeDamage       │
│ Does component exist?    │
└──────┬───────────────────┘
       │
       │ Yes
       v
┌──────────────────────────┐
│ Call TakeDamage()        │
│ Apply damage and effects │
└──────────────────────────┘
       │
       v
┌──────────────────────────┐
│ Spawn hit effect         │
│ Destroy arrow            │
└──────────────────────────┘
```

---

## Hệ Thống 4: UI Button Events

### Button onClick Events

Unity's Button component có một **onClick** event kích hoạt khi clicked.

### Kết Nối Buttons Trong Inspector

**Bước 1: Chọn Button GameObject**
**Bước 2: Tìm Button Component trong Inspector**
**Bước 3: Click + trong OnClick() section**
**Bước 4: Kéo target GameObject vào object slot**
**Bước 5: Chọn function từ dropdown**

### Ví Dụ: Play Button

```csharp
using UnityEngine;
using UnityEngine.UI;
using RGame;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    public GameObject menuPanel;
    public GameObject gameplayPanel;

    void Start()
    {
        Instance = this;
    }

    // Called by Play button's onClick event
    public void OnPlayButtonClicked()
    {
        Debug.Log("Play button clicked");

        // Hide menu
        menuPanel.SetActive(false);

        // Show gameplay UI
        gameplayPanel.SetActive(true);

        // Start game
        GameManager.Instance.StartGame();
    }

    // Called by Pause button
    public void OnPauseButtonClicked()
    {
        Debug.Log("Pause button clicked");
        GameManager.Instance.Pause();
    }

    // Called by Resume button
    public void OnResumeButtonClicked()
    {
        Debug.Log("Resume button clicked");
        GameManager.Instance.UnPause();
    }
}
```

**Inspector Setup cho Play Button**:
```
Button Component:
  OnClick():
    Runtime Only
    [MenuManager GameObject] → MenuManager.OnPlayButtonClicked()
```

### Ví Dụ Shop Button

```csharp
using UnityEngine;
using UnityEngine.UI;
using RGame;

public class BuyCharacterButton : MonoBehaviour
{
    public GameObject characterPrefab;
    public int price = 500;
    public Text priceText;

    void Start()
    {
        // Display price on button
        priceText.text = $"${price}";

        // Get the button component
        Button button = GetComponent<Button>();

        // Add click listener programmatically
        button.onClick.AddListener(OnButtonClicked);
    }

    void OnButtonClicked()
    {
        // Check if player has enough coins
        int coins = GameManager.Instance.GetCoins();

        if (coins >= price)
        {
            // Deduct coins
            GameManager.Instance.AddCoins(-price);

            // Unlock character
            UnlockCharacter();

            Debug.Log($"Purchased {characterPrefab.name} for {price} coins");
        }
        else
        {
            Debug.Log($"Not enough coins! Need {price}, have {coins}");
            ShowErrorMessage("Not enough coins!");
        }
    }

    void UnlockCharacter()
    {
        // Mark character as owned
        PlayerPrefs.SetInt(characterPrefab.name + "_Owned", 1);
        PlayerPrefs.Save();

        // Update button to show "Owned"
        GetComponent<Button>().interactable = false;
        priceText.text = "OWNED";
    }

    void ShowErrorMessage(string message)
    {
        // Display error UI (implementation varies)
    }
}
```

### Dynamic Button Setup

Đôi khi buttons được tạo tại runtime:

```csharp
public class CharacterSelectUI : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform buttonContainer;
    public GameObject[] characterPrefabs;

    void Start()
    {
        CreateCharacterButtons();
    }

    void CreateCharacterButtons()
    {
        foreach (var character in characterPrefabs)
        {
            // Create button
            GameObject buttonObj = Instantiate(buttonPrefab, buttonContainer);
            Button button = buttonObj.GetComponent<Button>();

            // Set button text
            Text buttonText = buttonObj.GetComponentInChildren<Text>();
            buttonText.text = character.name;

            // Add click listener (using closure to capture character)
            GameObject selectedCharacter = character;  // Capture for closure
            button.onClick.AddListener(() => OnCharacterSelected(selectedCharacter));
        }
    }

    void OnCharacterSelected(GameObject character)
    {
        Debug.Log($"Selected character: {character.name}");

        // Spawn character in game
        CharacterManager.Instance.SpawnCharacter(character);
    }
}
```

---

## Hệ Thống 5: Custom Game Events

### Invoke - Delayed Method Calls

**Invoke** gọi method sau một delay:

```csharp
// Call MethodName() after 2 seconds
Invoke("MethodName", 2f);

// Call MethodName() after 2 seconds, then repeat every 1 second
InvokeRepeating("MethodName", 2f, 1f);

// Cancel all pending Invoke calls
CancelInvoke();

// Cancel specific method
CancelInvoke("MethodName");
```

**Ví Dụ: Enemy Spawn Delay**

```csharp
public class Enemy : MonoBehaviour
{
    public float spawnDelay = 1f;

    void Start()
    {
        if (startBehavior == STARTBEHAVIOR.BURROWUP)
        {
            SetEnemyState(ENEMYSTATE.SPAWNING);
            AnimSetTrigger("spawn");

            // Finish spawning after delay
            Invoke("FinishSpawning", spawnDelay);
        }
    }

    void FinishSpawning()
    {
        if (enemyState == ENEMYSTATE.SPAWNING)
        {
            SetEnemyState(ENEMYSTATE.WALK);
            Debug.Log($"{gameObject.name} finished spawning");
        }
    }
}
```

### Coroutines - Complex Timing

**Coroutines** cho phép hành vi phức tạp dựa trên thời gian:

```csharp
// Start a coroutine
StartCoroutine(SpawnWaveCoroutine());

// Stop all coroutines on this object
StopAllCoroutines();

// Stop specific coroutine
StopCoroutine(myCoroutine);
```

**Ví Dụ: Wave Spawner**

```csharp
using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public Transform spawnPoint;
    public int enemiesPerWave = 5;
    public float timeBetweenEnemies = 2f;
    public float timeBetweenWaves = 10f;

    void Start()
    {
        StartCoroutine(SpawnWavesCoroutine());
    }

    IEnumerator SpawnWavesCoroutine()
    {
        int waveNumber = 1;

        while (true)  // Infinite waves
        {
            Debug.Log($"Starting Wave {waveNumber}");

            // Spawn enemies
            for (int i = 0; i < enemiesPerWave; i++)
            {
                // Pick random enemy
                GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

                // Spawn enemy
                Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

                Debug.Log($"Spawned enemy {i + 1}/{enemiesPerWave}");

                // Wait before next enemy
                yield return new WaitForSeconds(timeBetweenEnemies);
            }

            Debug.Log($"Wave {waveNumber} complete");

            // Wait before next wave
            yield return new WaitForSeconds(timeBetweenWaves);

            waveNumber++;
        }
    }
}
```

### Ví Dụ Custom Event: Burn Effect

```csharp
public class BurnEffect : MonoBehaviour
{
    public float burnDamagePerSecond = 5f;
    public float burnDuration = 3f;
    public GameObject burnParticles;

    public void ApplyBurn(GameObject target)
    {
        BurnTarget burnScript = target.AddComponent<BurnTarget>();
        burnScript.StartBurning(burnDamagePerSecond, burnDuration);
    }
}

public class BurnTarget : MonoBehaviour
{
    private ICanTakeDamage damageTarget;
    private GameObject particles;

    public void StartBurning(float damagePerSecond, float duration)
    {
        damageTarget = GetComponent<ICanTakeDamage>();
        if (damageTarget == null)
        {
            Destroy(this);
            return;
        }

        StartCoroutine(BurnCoroutine(damagePerSecond, duration));
    }

    IEnumerator BurnCoroutine(float damagePerSecond, float duration)
    {
        // Spawn burn particles
        particles = Instantiate(burnParticlesPrefab, transform);

        float elapsed = 0;

        while (elapsed < duration)
        {
            // Deal damage every second
            yield return new WaitForSeconds(1f);

            damageTarget.TakeDamage(damagePerSecond, Vector2.zero, transform.position, gameObject);

            elapsed += 1f;
            Debug.Log($"Burn damage: {damagePerSecond}, Time left: {duration - elapsed}");
        }

        // Clean up
        if (particles != null)
            Destroy(particles);

        Destroy(this);
    }
}
```

---

## Sơ Đồ Luồng Event Hoàn Chỉnh

### Game Start Sequence

```
Player clicks "Play" button
         |
         v
[Button.onClick fires]
         |
         v
MenuManager.OnPlayButtonClicked()
         |
         v
GameManager.StartGame()
         |
         v
[GameManager broadcasts IPlay()]
         |
    ┌────┴────┬────────┬─────────┐
    v         v        v         v
Enemy1    Enemy2    UI Panel  Audio Manager
 .IPlay()  .IPlay()  .IPlay()   .IPlay()
    |         |        |         |
    v         v        v         v
Start AI   Start AI  Show UI   Play Music
```

### Attack Animation Sequence

```
Enemy.PerformAttack()
         |
         v
SetEnemyState(ATTACK)
         |
         v
AnimSetTrigger("attack")
         |
         v
[Animator plays "Attack" animation]
         |
    Frame 0 → Frame 5 → Frame 10 → Frame 15
         |         |         |         |
         v         v         v         v
    [Start]  [Event]   [Middle]    [End]
              AnimMeleeAttack()
                   |
                   v
            SpawnHitbox()
                   |
                   v
         [Physics overlap check]
                   |
                   v
         Player.TakeDamage()
                   |
                   v
         [Player health decreases]
```

### Projectile Hit Sequence

```
Arrow flying
         |
         v
[Physics detects overlap]
         |
         v
Arrow.OnTriggerEnter2D(enemy)
         |
         v
[Check layer mask]
         |
         v
enemy.GetComponent<ICanTakeDamage>()
         |
         v
enemy.TakeDamage(damage, force, hitPoint, ...)
         |
         v
[Apply damage]
         |
    ┌────┴────┬────────────┐
    v         v            v
Update    Spawn       Check health
health    hit FX       <= 0?
bar                      |
                         v
                    enemy.Die()
                         |
                    ┌────┴────┐
                    v         v
             Spawn       GameManager
             death FX    .RemoveListener()
```

---

## Debug Hệ Thống Event

### Debug.Log Strategy

**Thêm logs để track event flow**:

```csharp
public void IPlay()
{
    Debug.Log($"[IPlay] {gameObject.name} received IPlay event");
    isPlaying = true;
}

public void AnimShoot()
{
    Debug.Log($"[AnimShoot] {gameObject.name} shooting at frame {Time.frameCount}");
    SpawnProjectile();
}

void OnTriggerEnter2D(Collider2D other)
{
    Debug.Log($"[Collision] {gameObject.name} collided with {other.gameObject.name}");
    // ...
}
```

**Kết Quả Trong Console**:
```
[IPlay] Enemy_Goblin received IPlay event
[IPlay] Enemy_Skeleton received IPlay event
[IPlay] GameUI received IPlay event
[AnimShoot] Player_Archer shooting at frame 1234
[Collision] Arrow_01 collided with Enemy_Goblin
```

### Visual Debugging: Gizmos

**Vẽ collision areas**:

```csharp
void OnDrawGizmosSelected()
{
    // Draw attack range
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(attackPoint.position, attackRange);

    // Draw raycast
    Gizmos.color = Color.blue;
    Gizmos.DrawLine(transform.position, transform.position + Vector3.right * 5);
}
```

### Breakpoints

**Dùng Visual Studio/Rider debugger**:

1. Set breakpoint trên event method (click left margin)
2. Start Unity trong Play Mode
3. Trigger the event
4. Debugger pause tại breakpoint
5. Inspect variables

**Hữu ích cho**:
- Kiểm tra xem event có được gọi không
- Xem exact values
- Step-by-step execution

### Event Call Stack

**Để xem ai đã gọi method**:

```csharp
public void IPlay()
{
    Debug.Log($"IPlay called. Stack trace:\n{System.Environment.StackTrace}");
    // ...
}
```

**Output hiển thị call chain**:
```
IPlay called. Stack trace:
   at Enemy.IPlay()
   at GameManager.StartGame()
   at MenuManager.OnPlayButtonClicked()
   at UnityEngine.EventSystems.EventSystem.Update()
```

---

## Khắc Phục Sự Cố

### Vấn Đề 1: Event Không Fire

**Triệu Chứng**: IPlay() không bao giờ được gọi, enemy không phản ứng với game start

**Nguyên Nhân**:

**Nguyên Nhân A: Quên register listener**

```csharp
// SAI: Never registered
void Start()
{
    // Missing: GameManager.Instance.AddListener(this);
}

// ĐÚNG:
void OnEnable()
{
    GameManager.Instance.AddListener(this);
}
```

**Nguyên Nhân B: GameManager chưa initialized**

```csharp
void OnEnable()
{
    // SAI: GameManager might be null
    GameManager.Instance.AddListener(this);

    // ĐÚNG: Check null
    if (GameManager.Instance != null)
    {
        GameManager.Instance.AddListener(this);
    }
    else
    {
        Debug.LogError("GameManager not found!");
    }
}
```

---

### Vấn Đề 2: Animation Event Method Không Tìm Thấy

**Error**: `'AnimShoot' function not found on object 'Enemy_Goblin'`

**Nguyên Nhân**:

**Nguyên Nhân A: Method là private**

```csharp
// SAI: private methods can't be called by animation events
private void AnimShoot() { }

// ĐÚNG: must be public
public void AnimShoot() { }
```

**Nguyên Nhân B: Method name mismatch**

```
Animation Event: "AnimShoot"
Script Method:   "AnimShot"  ← Typo!
```

**Nguyên Nhân C: Method trên wrong object**

Animation events gọi methods trên **GameObject mà Animator gắn vào**, không phải children hoặc parents.

---

### Vấn Đề 3: Collision Không Phát Hiện

**Triệu Chứng**: Arrow đi xuyên qua enemy mà không gây damage

**Các Bước Debug**:

**Bước 1: Kiểm tra colliders tồn tại**

```csharp
void Start()
{
    Collider2D col = GetComponent<Collider2D>();
    if (col == null)
    {
        Debug.LogError($"{gameObject.name} missing Collider2D!");
    }
}
```

**Bước 2: Kiểm tra Is Trigger flag**

Arrow nên có `Is Trigger` checked (overlap detection, not collision).

**Bước 3: Kiểm tra Rigidbody2D**

Ít nhất một object cần Rigidbody2D (thường là arrow).

**Bước 4: Kiểm tra layers**

```csharp
void OnTriggerEnter2D(Collider2D other)
{
    Debug.Log($"Hit: {other.gameObject.name}, Layer: {LayerMask.LayerToName(other.gameObject.layer)}");

    int layerMask = 1 << other.gameObject.layer;
    bool isValid = (layerMask & damageableLayers.value) != 0;

    Debug.Log($"Valid target: {isValid}");
}
```

**Bước 5: Kiểm tra collision matrix**

Edit → Project Settings → Physics 2D → Layer Collision Matrix

Đảm bảo "Arrow" layer có thể collide với "Enemy" layer.

---

### Vấn Đề 4: Button Click Không Hoạt Động

**Triệu Chứng**: Clicking button không làm gì

**Các Bước Debug**:

**Bước 1: Kiểm tra EventSystem tồn tại**

UI buttons cần EventSystem. Kiểm tra Hierarchy cho `EventSystem` GameObject.

**Bước 2: Kiểm tra Raycast Target**

Button's Image component cần `Raycast Target` checked.

**Bước 3: Kiểm tra button interactable**

```
Button Component:
  Interactable: ☑  ← Phải được checked
```

**Bước 4: Kiểm tra onClick được assigned**

```csharp
void Start()
{
    Button button = GetComponent<Button>();
    if (button.onClick.GetPersistentEventCount() == 0)
    {
        Debug.LogWarning($"{gameObject.name}: No onClick listeners assigned!");
    }
}
```

**Bước 5: Thêm debug log**

```csharp
public void OnButtonClicked()
{
    Debug.Log("Button clicked!");  // ← Thêm dòng này trước
    // ... rest of code
}
```

---

### Vấn Đề 5: Invoke Không Gọi Method

**Triệu Chứng**: Invoke("MethodName", 2f) không gọi method

**Nguyên Nhân A: Method name typo**

```csharp
// SAI:
Invoke("FinshSpawning", 1f);  // Typo: "Finsh" instead of "Finish"

// ĐÚNG:
Invoke("FinishSpawning", 1f);
```

**Nguyên Nhân B: GameObject destroyed trước delay**

```csharp
Invoke("DoSomething", 5f);
Destroy(gameObject);  // ← Destroys before Invoke fires!
```

**Nguyên Nhân C: Coroutine stopped**

```csharp
IEnumerator routine = MyCoroutine();
StartCoroutine(routine);
StopCoroutine(routine);  // ← Stops before completion
```

---

## Tài Liệu Tham Khảo Chéo

**Tài Liệu Liên Quan**:

- **[Core-Objects.md](Core-Objects.md)** - GameManager và Observer pattern deep dive
- **[03_Enemy_System_Complete.md](03_Enemy_System_Complete.md)** - Enemy AI và animation
- **[04_UI_System_Complete.md](04_UI_System_Complete.md)** - UI button setup
- **[02_Player_System_Complete.md](02_Player_System_Complete.md)** - Player input events
- **[11_Troubleshooting.md](11_Troubleshooting.md)** - Hướng dẫn debug chung
- **[12_Visual_Reference.md](12_Visual_Reference.md)** - Sơ đồ event system
- **[13_Code_Examples.md](13_Code_Examples.md)** - Coroutine patterns

**Scripts Chính**:
- `GameManager.cs` - Observer pattern implementation
- `Enemy.cs` - IListener implementation
- `ArrowScript.cs` - Collision detection example
- `MenuManager.cs` - UI event handling

**Unity Documentation**:
- [Animation Events](https://docs.unity3d.com/Manual/script-AnimationWindowEvent.html)
- [OnTriggerEnter2D](https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnTriggerEnter2D.html)
- [Button.onClick](https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/script-Button.html)
- [Coroutines](https://docs.unity3d.com/Manual/Coroutines.html)
- [Invoke](https://docs.unity3d.com/ScriptReference/MonoBehaviour.Invoke.html)

---

## Tóm Tắt

**Bạn Đã Học Được**:
- ✅ Cách Observer pattern (IListener) broadcast events đến tất cả registered objects
- ✅ Cách Animation Events gọi code tại specific animation frames
- ✅ Cách collision detection hoạt động với OnTriggerEnter2D
- ✅ Cách UI buttons kết nối với game logic bằng onClick events
- ✅ Cách tạo custom timed events với Invoke và Coroutines
- ✅ Cách debug các vấn đề event flow

**5 Hệ Thống Events**:
1. **Observer Pattern** → Nhiều objects phản ứng với một event (game state changes)
2. **Animation Events** → Code executes trong animations (attack hitboxes)
3. **Collision Events** → Objects phản ứng với physical contact (projectiles hit enemies)
4. **UI Events** → Buttons kích hoạt game actions (player clicks Buy)
5. **Custom Events** → Timed hoặc conditional actions (spawn waves, burn damage)

**Tips Debug Chính**:
- Thêm Debug.Log() vào mọi event method
- Dùng Visual Studio breakpoints để pause execution
- Kiểm tra listeners được registered với AddListener()
- Xác minh animation event methods là **public**
- Kiểm tra layer masks cho collision filtering
- Đảm bảo EventSystem tồn tại cho UI buttons

**Bước Tiếp Theo**:
- Tạo custom IListener object phản ứng với game events
- Thiết lập Animation Events cho attack animations của bạn
- Xây dựng projectile với proper collision detection
- Thêm UI buttons với debugging logs để xác minh chúng hoạt động

---

**Phiên Bản Tài Liệu**: 1.0
**Cập Nhật Lần Cuối**: 2025-10-29
**Transformation**: Vietnamese (26 lines) → English (1,800+ lines)
