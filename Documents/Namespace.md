# Namespaces Trong C# Và Unity (RGame Namespace)

**Mục Đích**: Hướng dẫn đầy đủ để hiểu namespaces trong C#, tại sao project này dùng `RGame` namespace, và cách làm việc với namespaces hiệu quả trong Unity.

**Nội Dung Tài Liệu**:
- C# namespace fundamentals
- Tại sao project này dùng `namespace RGame`
- Cách thêm scripts vào namespace
- Best practices cho namespace organization
- Troubleshooting các lỗi liên quan đến namespace

**Tài Liệu Liên Quan**:
- Xem `00_Cac_Khai_Niem_Unity_Co_Ban.md` cho C# basics
- Xem `01_Kien_Truc_Project.md` cho code organization
- Xem `13_Vi_Du_Code.md` cho practical coding patterns
- Xem `11_Xu_Ly_Su_Co.md` cho compiler error solutions

---

## Mục Lục

1. [Namespaces Là Gì?](#namespaces-là-gì)
2. [Tại Sao Project Này Dùng RGame](#tại-sao-project-này-dùng-rgame)
3. [Namespaces Hoạt Động Như Thế Nào](#namespaces-hoạt-động-như-thế-nào)
4. [Thêm Scripts Vào RGame Namespace](#thêm-scripts-vào-rgame-namespace)
5. [Dùng Classes Từ Namespaces Khác](#dùng-classes-từ-namespaces-khác)
6. [Kỹ Thuật Namespace Nâng Cao](#kỹ-thuật-namespace-nâng-cao)
7. [Best Practices](#best-practices)
8. [Troubleshooting](#troubleshooting)

---

## Namespaces Là Gì?

### Giải Thích Đơn Giản

Nghĩ về namespace như **folders cho code** của bạn:

```
Không Có Namespaces (lộn xộn):
┌───────────────────────────┐
│ Tất Cả Classes Ở Một Chỗ  │
├───────────────────────────┤
│ GameManager               │
│ MenuManager               │
│ Enemy                     │
│ Player                    │
│ ThirdPartyGameManager     │  ← Name collision!
│ ThirdPartyEnemy           │  ← Name collision!
└───────────────────────────┘

Có Namespaces (được tổ chức):
┌───────────────────────────┐
│ RGame Namespace           │
├───────────────────────────┤
│ RGame.GameManager         │
│ RGame.MenuManager         │
│ RGame.Enemy               │
│ RGame.Player              │
└───────────────────────────┘
┌───────────────────────────┐
│ ThirdParty Namespace      │
├───────────────────────────┤
│ ThirdParty.GameManager    │  ← Không có collision!
│ ThirdParty.Enemy          │  ← Không có collision!
└───────────────────────────┘
```

**Ví Dụ Thực Tế**:

Hãy tưởng tượng một thư viện với các sách:

```
Không có namespaces:
- "Animals" của John Smith
- "Animals" của Jane Doe     ← Bạn muốn cái nào?
- "Animals" của Bob Johnson

Có namespaces:
- Science.Animals của John Smith
- Fiction.Animals của Jane Doe
- Textbook.Animals của Bob Johnson  ← Rõ ràng cái nào!
```

---

### Định Nghĩa Kỹ Thuật

**Namespace** là một C# feature nhóm các classes, interfaces, structs, và enums liên quan lại với nhau.

**Cú Pháp Cơ Bản**:

```csharp
namespace RGame
{
    public class GameManager : MonoBehaviour
    {
        // Class code ở đây
    }
}
```

**Không có namespace** (KHÔNG được khuyến khích):

```csharp
public class GameManager : MonoBehaviour
{
    // GameManager này ở trong "global namespace"
    // Có thể conflict với các GameManager classes khác!
}
```

---

### Tại Sao Namespaces Tồn Tại

**Vấn Đề**: Không có namespaces, hai files này gây ra conflict:

**File 1: YourGameManager.cs**
```csharp
public class GameManager : MonoBehaviour
{
    public void StartGame() { /* code của bạn */ }
}
```

**File 2: ThirdPartyPlugin.cs** (từ một asset bạn import)
```csharp
public class GameManager : MonoBehaviour
{
    public void Initialize() { /* code của họ */ }
}
```

**Kết Quả**: Compiler error!
```
error CS0101: The namespace 'global' already contains a definition for 'GameManager'
```

**Giải Pháp**: Dùng namespaces!

**File 1: YourGameManager.cs**
```csharp
namespace RGame
{
    public class GameManager : MonoBehaviour
    {
        public void StartGame() { /* code của bạn */ }
    }
}
```

**File 2: ThirdPartyPlugin.cs**
```csharp
namespace ThirdPartyPlugin
{
    public class GameManager : MonoBehaviour
    {
        public void Initialize() { /* code của họ */ }
    }
}
```

**Kết Quả**: Không có conflict! Bạn có `RGame.GameManager` và `ThirdPartyPlugin.GameManager`.

---

## Tại Sao Project Này Dùng RGame

Project này bọc tất cả game code trong `namespace RGame` vì ba lý do chính:

### 1. **Organization**

Tất cả project code được xác định rõ ràng:

```csharp
namespace RGame
{
    public class GameManager : MonoBehaviour { }
    public class Enemy : MonoBehaviour { }
    public class Player_Archer : Enemy { }
    // ... tất cả game classes
}
```

**Lợi Ích**:
- Dễ xác định classes nào thuộc về project này
- Phân tách rõ ràng với Unity classes (`UnityEngine.UI`, etc.)
- Phân tách rõ ràng với third-party assets

### 2. **Tránh Name Collisions**

Nhiều Unity assets dùng tên phổ biến:

```
Các class names phổ biến có thể conflict:
- GameManager
- MenuManager
- SoundManager
- Enemy
- Player
- Weapon
- Item
```

Với `namespace RGame`, bạn có thể an toàn import assets không có conflicts:

```csharp
// Code của bạn:
namespace RGame
{
    public class GameManager : MonoBehaviour { }
}

// Third-party asset (ví dụ, "Super Game Framework"):
namespace SuperGameFramework
{
    public class GameManager : MonoBehaviour { }
}

// Cả hai có thể cùng tồn tại! Không có conflict.
```

### 3. **Encapsulation**

Namespace cung cấp logical grouping:

```
RGame namespace chứa:
├─ Core Systems (GameManager, MenuManager, SoundManager)
├─ Character Systems (Player, Enemy, Controller2D)
├─ UI Systems (UI_UI, MapControllerUI, MenuManager)
├─ Helper Classes (AnimationHelper, CheckTargetHelper)
└─ Data Classes (GlobalValue, UpgradedCharacterParameter)
```

**Giống như folder structure**, nhưng cho code organization.

---

## Namespaces Hoạt Động Như Thế Nào

### Khai Báo Cơ Bản

Mỗi script trong project này bắt đầu với:

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGame
{
    public class Enemy : MonoBehaviour
    {
        // Class code ở đây
    }
}
```

**Phân Tích Structure**:

```csharp
// 1. Using directives (import các namespaces khác)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 2. Khai báo namespace của bạn
namespace RGame
{
    // 3. Mọi thứ bên trong block này là part của RGame namespace
    public class Enemy : MonoBehaviour
    {
        public int health = 100;

        public void TakeDamage(int damage)
        {
            health -= damage;
        }
    }
}
// 4. Closing brace kết thúc namespace
```

---

### Truy Cập Classes Trong Cùng Namespace

**Classes trong cùng namespace có thể reference lẫn nhau trực tiếp**:

```csharp
// File: Enemy.cs
namespace RGame
{
    public class Enemy : MonoBehaviour
    {
        public int health = 100;
    }
}

// File: Player.cs
namespace RGame
{
    public class Player : MonoBehaviour
    {
        void AttackEnemy()
        {
            Enemy enemy = FindObjectOfType<Enemy>();  // ← Có thể dùng trực tiếp!
            // Không cần "RGame.Enemy" hay "using RGame;"
        }
    }
}
```

**Tại Sao**: Cả `Enemy` và `Player` đều ở trong `RGame` namespace, nên họ có thể thấy nhau.

---

### Truy Cập Classes Từ Namespaces Khác

**Option 1: Fully Qualified Name** (dùng full path)

```csharp
namespace RGame
{
    public class GameManager : MonoBehaviour
    {
        void Start()
        {
            // Truy cập Unity UI classes với full path
            UnityEngine.UI.Text myText = GetComponent<UnityEngine.UI.Text>();
        }
    }
}
```

**Option 2: Using Directive** (import namespace)

```csharp
using UnityEngine.UI;  // ← Import UI namespace

namespace RGame
{
    public class GameManager : MonoBehaviour
    {
        void Start()
        {
            Text myText = GetComponent<Text>();  // ← Có thể dùng short name!
        }
    }
}
```

**Option 2 được ưu tiên** vì nó sạch hơn và dễ đọc hơn.

---

### Visual Flow Của Using Directives

```csharp
// Những cái này giống "import" statements trong các ngôn ngữ khác
using System.Collections;         // Import System.Collections namespace
using System.Collections.Generic; // Import System.Collections.Generic namespace
using UnityEngine;                // Import UnityEngine namespace
using UnityEngine.UI;             // Import UnityEngine.UI namespace

namespace RGame
{
    public class MenuManager : MonoBehaviour
    {
        public Text scoreText;  // ← Từ UnityEngine.UI (imported ở trên)

        void Start()
        {
            // Có thể dùng IEnumerator từ System.Collections (imported ở trên)
            StartCoroutine(CountdownCo());
        }

        IEnumerator CountdownCo()
        {
            // Có thể dùng List<T> từ System.Collections.Generic (imported ở trên)
            List<int> numbers = new List<int> { 3, 2, 1 };

            foreach (int num in numbers)
            {
                scoreText.text = num.ToString();
                yield return new WaitForSeconds(1f);  // ← Từ UnityEngine (imported ở trên)
            }
        }
    }
}
```

---

## Thêm Scripts Vào RGame Namespace

### Khi Nào Thêm Scripts Vào RGame

**Thêm vào RGame namespace** nếu:
- ✅ Script specific cho game này
- ✅ Script là một core system (manager, controller, helper)
- ✅ Script là một game object behavior (player, enemy, item)
- ✅ Script sẽ được dùng qua nhiều scenes

**Đừng thêm vào RGame namespace** nếu:
- ❌ Script là một generic tool (có thể reuse trong các projects khác)
- ❌ Script là một editor tool (Unity Editor customization)
- ❌ Bạn đang prototype và script có thể bị xóa

---

### Từng Bước: Thêm Script Mới

**Bước 1: Tạo Script**

Trong Unity Editor:
1. Right-click trong Project window → Create → C# Script
2. Đặt tên nó `MyNewScript`
3. Double-click để mở trong code editor của bạn

**Bước 2: Thêm Namespace**

Unity tạo ra cái này:

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyNewScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
```

**Thay đổi thành cái này**:

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGame  // ← Thêm cái này
{              // ← Thêm opening brace
    public class MyNewScript : MonoBehaviour
    {
        void Start()
        {

        }

        void Update()
        {

        }
    }
}              // ← Thêm closing brace
```

**Bước 3: Save và Compile**

Unity sẽ tự động recompile script.

---

### Template Cho Scripts Mới

**Copy template này** cho tất cả scripts mới:

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGame
{
    /// <summary>
    /// Mô tả ngắn gọn về class này làm gì.
    /// </summary>
    public class MyClassName : MonoBehaviour
    {
        [Header("CONFIGURATION")]
        public int myVariable = 10;

        [Header("STATE")]
        private bool isActive = false;

        void Start()
        {
            // Initialization code
        }

        void Update()
        {
            // Per-frame code
        }

        // Các custom methods của bạn ở đây
        public void MyMethod()
        {

        }
    }
}
```

---

## Dùng Classes Từ Namespaces Khác

### Scenario 1: Truy Cập RGame Classes Từ Script Khác

**Script của bạn** (trong `RGame` namespace):

```csharp
namespace RGame
{
    public class GameManager : MonoBehaviour
    {
        public void StartGame()
        {
            Debug.Log("Game started!");
        }
    }
}
```

**Third-party script** (trong namespace khác):

```csharp
using RGame;  // ← Import RGame namespace

namespace ThirdPartyTool
{
    public class ToolScript : MonoBehaviour
    {
        void Start()
        {
            GameManager gm = FindObjectOfType<GameManager>();  // ← Có thể truy cập!
            gm.StartGame();
        }
    }
}
```

---

### Scenario 2: Truy Cập Third-Party Classes Từ RGame

**Third-party asset** (ví dụ, "Dialogue System"):

```csharp
namespace DialogueSystem
{
    public class DialogueManager : MonoBehaviour
    {
        public void ShowDialogue(string text)
        {
            Debug.Log(text);
        }
    }
}
```

**Script của bạn**:

```csharp
using DialogueSystem;  // ← Import namespace của họ

namespace RGame
{
    public class NPC : MonoBehaviour
    {
        void OnTriggerEnter2D(Collider2D other)
        {
            DialogueManager dm = FindObjectOfType<DialogueManager>();  // ← Có thể truy cập!
            dm.ShowDialogue("Hello, traveler!");
        }
    }
}
```

---

### Scenario 3: Name Collision - Cùng Class Name

**Vấn Đề**: Cả `RGame` và một third-party asset đều có class tên `GameManager`.

**Giải Pháp Tệ** (gây ra error):

```csharp
using RGame;
using ThirdPartyAsset;  // Cả hai đều có GameManager class!

namespace MyCode
{
    public class MyScript : MonoBehaviour
    {
        void Start()
        {
            GameManager gm = new GameManager();  // ← ERROR: Ambiguous reference!
        }
    }
}
```

**Giải Pháp Tốt 1: Fully Qualified Names**

```csharp
namespace MyCode
{
    public class MyScript : MonoBehaviour
    {
        void Start()
        {
            RGame.GameManager rgm = FindObjectOfType<RGame.GameManager>();
            ThirdPartyAsset.GameManager tpm = FindObjectOfType<ThirdPartyAsset.GameManager>();
        }
    }
}
```

**Giải Pháp Tốt 2: Alias**

```csharp
using RGM = RGame.GameManager;  // ← Tạo alias
using TPM = ThirdPartyAsset.GameManager;  // ← Tạo alias

namespace MyCode
{
    public class MyScript : MonoBehaviour
    {
        void Start()
        {
            RGM rgm = FindObjectOfType<RGM>();  // ← Rõ ràng cái nào!
            TPM tpm = FindObjectOfType<TPM>();
        }
    }
}
```

---

## Kỹ Thuật Namespace Nâng Cao

### 1. Nested Namespaces

Bạn có thể tạo **sub-namespaces** để organization tốt hơn:

```csharp
namespace RGame.Enemies
{
    public class Goblin : Enemy { }
    public class Skeleton : Enemy { }
}

namespace RGame.Players
{
    public class Warrior : Player { }
    public class Archer : Player { }
}

namespace RGame.UI
{
    public class MenuManager : MonoBehaviour { }
    public class MapControllerUI : MonoBehaviour { }
}
```

**Trực quan hóa directory structure**:

```
RGame (namespace)
├─ Enemies (sub-namespace)
│  ├─ Goblin
│  └─ Skeleton
├─ Players (sub-namespace)
│  ├─ Warrior
│  └─ Archer
└─ UI (sub-namespace)
   ├─ MenuManager
   └─ MapControllerUI
```

**Truy cập nested namespace classes**:

```csharp
using RGame.Enemies;  // Import sub-namespace

namespace RGame
{
    public class GameManager : MonoBehaviour
    {
        void Start()
        {
            Goblin goblin = Instantiate(goblinPrefab);  // Có thể truy cập trực tiếp
        }
    }
}
```

---

### 2. Global Using Directives (C# 10+)

**Nếu bạn dùng cùng namespace trong MỌI file**, thêm global using:

**Tạo file: GlobalUsings.cs**

```csharp
global using System.Collections;
global using System.Collections.Generic;
global using UnityEngine;
```

**Bây giờ tất cả files tự động có các imports này**:

```csharp
// Không cần "using UnityEngine;" nữa!

namespace RGame
{
    public class MyScript : MonoBehaviour  // ← Hoạt động không cần "using UnityEngine;"
    {
        void Start()
        {
            Debug.Log("Hello!");  // ← Hoạt động!
        }
    }
}
```

**Lưu Ý**: Feature này yêu cầu C# 9.0+ và Unity 2021.2+.

---

### 3. Namespace Aliases

**Dùng aliases** để shorten long namespace names:

```csharp
using UI = UnityEngine.UI;  // ← Alias cho UnityEngine.UI

namespace RGame
{
    public class MenuManager : MonoBehaviour
    {
        public UI.Text scoreText;  // ← Ngắn hơn "UnityEngine.UI.Text"
        public UI.Button playButton;
        public UI.Slider healthBar;
    }
}
```

---

### 4. File-Scoped Namespaces (C# 10+)

**Cú pháp cũ** (verbose):

```csharp
namespace RGame
{
    public class GameManager : MonoBehaviour
    {
        // Code ở đây
    }
}
```

**Cú pháp mới** (sạch hơn):

```csharp
namespace RGame;  // ← File-scoped namespace (một dòng!)

public class GameManager : MonoBehaviour
{
    // Code ở đây - tự động trong RGame namespace
}
```

**Lợi Ích**: Ít indentation hơn, code sạch hơn.

**Yêu Cầu**: C# 10+ (Unity 2021.2+)

---

## Best Practices

### 1. Consistent Namespace Usage

**✅ NÊN**: Dùng `namespace RGame` trong tất cả game-specific scripts

```csharp
// ✅ Tốt
namespace RGame
{
    public class GameManager : MonoBehaviour { }
}
```

**❌ KHÔNG NÊN**: Mix namespaced và non-namespaced scripts

```csharp
// ❌ Tệ
public class GameManager : MonoBehaviour { }  // Không có namespace
```

---

### 2. Match Namespace Với Folder Structure

**Folder structure**:
```
Assets/
└─ _MonstersOut/
   └─ Scripts/
      ├─ Managers/
      │  └─ GameManager.cs
      ├─ Player/
      │  └─ Player_Archer.cs
      └─ Enemy/
         └─ Enemy.cs
```

**Option 1: Single namespace** (cách tiếp cận của current project)

```csharp
// Tất cả scripts dùng: namespace RGame
```

**Option 2: Nested namespaces** (tổ chức hơn cho large projects)

```csharp
// GameManager.cs:
namespace RGame.Managers
{
    public class GameManager : MonoBehaviour { }
}

// Player_Archer.cs:
namespace RGame.Players
{
    public class Player_Archer : Enemy { }
}

// Enemy.cs:
namespace RGame.Enemies
{
    public class Enemy : MonoBehaviour { }
}
```

**Khuyến Nghị**: Dùng **Option 1** (single namespace) trừ khi project của bạn có 200+ scripts.

---

### 3. Dùng Clear Namespace Names

**✅ NÊN**: Dùng tên descriptive, unique

```csharp
namespace LawnDefenseMonsters { }  // Unique, descriptive
namespace RGame { }                // Ngắn, unique
```

**❌ KHÔNG NÊN**: Dùng generic names

```csharp
namespace Game { }      // Quá generic, conflicts khả năng cao
namespace MyGame { }    // Quá generic
namespace Project { }   // Quá generic
```

---

### 4. Nhóm Related Classes

**✅ NÊN**: Giữ related classes trong cùng namespace

```csharp
namespace RGame
{
    public class Enemy : MonoBehaviour { }
    public class SmartEnemyGrounded : Enemy { }
    public class EnemyHealth : MonoBehaviour { }
}
```

**❌ KHÔNG NÊN**: Tách tightly coupled classes vào các namespaces khác

```csharp
// ❌ Tệ: Enemy và SmartEnemyGrounded rất liên quan chặt chẽ!
namespace RGame.Core
{
    public class Enemy : MonoBehaviour { }
}

namespace RGame.Advanced
{
    public class SmartEnemyGrounded : Enemy { }
}
```

---

### 5. Document Namespace Purpose

Thêm comment ở đầu namespace của bạn:

```csharp
using UnityEngine;

/// <summary>
/// RGame namespace chứa tất cả core game systems cho "Lawn Defense: Monsters Out".
/// Bao gồm managers, player/enemy systems, UI, helpers, và data classes.
/// </summary>
namespace RGame
{
    public class GameManager : MonoBehaviour
    {
        // ...
    }
}
```

---

## Troubleshooting

### Vấn Đề 1: "The type or namespace name could not be found"

**Error Message**:
```
error CS0246: The type or namespace name 'Enemy' could not be found
(are you missing a using directive or an assembly reference?)
```

**Nguyên Nhân**: Script đang cố dùng một class từ namespace khác mà không import nó.

**Ví Dụ**:

```csharp
// Enemy.cs
namespace RGame
{
    public class Enemy : MonoBehaviour { }
}

// ThirdPartyScript.cs
namespace ThirdParty
{
    public class ThirdPartyScript : MonoBehaviour
    {
        void Start()
        {
            Enemy enemy = FindObjectOfType<Enemy>();  // ← ERROR!
        }
    }
}
```

**Giải Pháp**: Thêm `using RGame;`

```csharp
using RGame;  // ← Import namespace

namespace ThirdParty
{
    public class ThirdPartyScript : MonoBehaviour
    {
        void Start()
        {
            Enemy enemy = FindObjectOfType<Enemy>();  // ← Hoạt động!
        }
    }
}
```

---

### Vấn Đề 2: "The namespace 'XXX' already contains a definition for 'YYY'"

**Error Message**:
```
error CS0101: The namespace 'RGame' already contains a definition for 'GameManager'
```

**Nguyên Nhân**: Hai classes cùng tên trong cùng namespace.

**Ví Dụ**:

```csharp
// File: GameManager.cs
namespace RGame
{
    public class GameManager : MonoBehaviour { }
}

// File: GameManagerNew.cs
namespace RGame
{
    public class GameManager : MonoBehaviour { }  // ← ERROR: Duplicate!
}
```

**Giải Pháp 1**: Đổi tên một trong các classes

```csharp
// File: GameManagerNew.cs
namespace RGame
{
    public class GameManagerV2 : MonoBehaviour { }  // ← Tên khác
}
```

**Giải Pháp 2**: Di chuyển sang namespace khác

```csharp
// File: GameManagerNew.cs
namespace RGame.Experimental  // ← Namespace khác
{
    public class GameManager : MonoBehaviour { }
}
```

---

### Vấn Đề 3: "Ambiguous reference between 'RGame.X' and 'ThirdParty.X'"

**Error Message**:
```
error CS0104: 'GameManager' is an ambiguous reference between
'RGame.GameManager' and 'ThirdPartyAsset.GameManager'
```

**Nguyên Nhân**: Cả hai namespaces được imported, cả hai có cùng class name.

**Ví Dụ**:

```csharp
using RGame;
using ThirdPartyAsset;

public class MyScript : MonoBehaviour
{
    void Start()
    {
        GameManager gm = new GameManager();  // ← Cái nào?!
    }
}
```

**Giải Pháp 1**: Remove một `using` statement và dùng fully qualified name

```csharp
using RGame;  // Chỉ import RGame

public class MyScript : MonoBehaviour
{
    void Start()
    {
        GameManager rgm = new GameManager();  // RGame.GameManager
        ThirdPartyAsset.GameManager tpm = new ThirdPartyAsset.GameManager();  // Full path
    }
}
```

**Giải Pháp 2**: Dùng aliases

```csharp
using RGM = RGame.GameManager;
using TPM = ThirdPartyAsset.GameManager;

public class MyScript : MonoBehaviour
{
    void Start()
    {
        RGM rgm = new RGM();  // Rõ ràng!
        TPM tpm = new TPM();  // Rõ ràng!
    }
}
```

---

### Vấn Đề 4: MonoBehaviour Script Không Attach Được Trong Inspector

**Triệu Chứng**: Không thể kéo script lên GameObject trong Inspector.

**Nguyên Nhân**: Script có namespace, nhưng bạn dùng Unity version cũ không hỗ trợ namespaced MonoBehaviour trong Inspector.

**Kiểm Tra Unity Version**: Unity 5.3+ hỗ trợ namespaced MonoBehaviour.

**Giải Pháp 1**: Update Unity lên 5.3 hoặc mới hơn.

**Giải Pháp 2**: Remove namespace (KHÔNG được khuyến khích):

```csharp
// Remove namespace tạm thời
public class MyScript : MonoBehaviour
{
    // ...
}
```

**Giải Pháp 3**: Dùng `AddComponent<>()` trong code thay vào:

```csharp
namespace RGame
{
    public class GameManager : MonoBehaviour
    {
        void Start()
        {
            gameObject.AddComponent<MyScript>();
        }
    }
}
```

---

### Vấn Đề 5: Serialization Issues Với Namespaced Classes

**Triệu Chứng**: Custom class data không save trong Inspector.

**Ví Dụ**:

```csharp
namespace RGame
{
    [System.Serializable]
    public class CharacterData
    {
        public string name;
        public int health;
    }

    public class GameManager : MonoBehaviour
    {
        public CharacterData player;  // ← Có thể không serialize đúng trong Unity cũ
    }
}
```

**Nguyên Nhân**: Unity versions rất cũ (< 5.0) có serialization issues với namespaced classes.

**Giải Pháp 1**: Update Unity (5.0+).

**Giải Pháp 2**: Dùng fully qualified name trong attribute:

```csharp
namespace RGame
{
    [System.Serializable]
    [UnityEngine.SerializeField]  // ← Fully qualified
    public class CharacterData
    {
        public string name;
        public int health;
    }
}
```

---

## Tóm Tắt

**Những Điểm Chính**:

1. **Namespaces** giống như folders cho code—chúng tổ chức classes và ngăn name collisions
2. **RGame namespace** bọc tất cả game-specific code trong project này
3. **Using directives** import classes từ các namespaces khác (`using UnityEngine;`)
4. **Classes trong cùng namespace** có thể reference lẫn nhau trực tiếp
5. **Classes trong namespaces khác** yêu cầu `using` directive hoặc fully qualified name
6. **Best practice**: Luôn thêm game scripts mới vào `namespace RGame`

**Essential Patterns**:

```csharp
// 1. Standard script template
using UnityEngine;

namespace RGame
{
    public class MyScript : MonoBehaviour
    {
        // Code của bạn
    }
}

// 2. Import các namespaces khác
using RGame;  // Import RGame namespace

namespace MyNamespace
{
    public class MyScript : MonoBehaviour
    {
        void Start()
        {
            GameManager gm = FindObjectOfType<GameManager>();  // Có thể truy cập RGame classes
        }
    }
}

// 3. Xử lý name collisions với aliases
using RGM = RGame.GameManager;
using TPM = ThirdParty.GameManager;

public class MyScript : MonoBehaviour
{
    void Start()
    {
        RGM myGM = new RGM();
        TPM theirGM = new TPM();
    }
}

// 4. Nested namespaces
namespace RGame.Managers
{
    public class GameManager : MonoBehaviour { }
}

namespace RGame.UI
{
    public class MenuManager : MonoBehaviour { }
}
```

**Quick Reference**:

```csharp
// Truy cập class trong cùng namespace
Enemy enemy = new Enemy();  // Truy cập trực tiếp

// Truy cập class trong namespace khác (Option 1: using directive)
using ThirdParty;
ThirdPartyClass obj = new ThirdPartyClass();

// Truy cập class trong namespace khác (Option 2: fully qualified name)
ThirdParty.ThirdPartyClass obj = new ThirdParty.ThirdPartyClass();

// Xử lý name collision với alias
using MyEnemy = RGame.Enemy;
using TheirEnemy = ThirdParty.Enemy;
```

**Các Bước Tiếp Theo**:
- Xem `13_Vi_Du_Code.md` cho practical coding patterns
- Xem `01_Kien_Truc_Project.md` cho overall code organization
- Xem `11_Xu_Ly_Su_Co.md` cho compiler error solutions

---

**Kết Thúc Tài Liệu**

<p align="center">
<strong>Lawn Defense: Monsters Out</strong><br>
Namespaces Trong C# Và Unity (RGame Namespace)<br>
C# Namespaces Technical Guide
</p>
