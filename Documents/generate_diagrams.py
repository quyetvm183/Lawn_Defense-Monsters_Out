#!/usr/bin/env python3
"""
Script to generate PlantUML diagrams using PlantUML web server
Usage: python generate_diagrams.py
"""

import os
import zlib
import base64
import urllib.request
import urllib.error

# PlantUML encoding
def encode_plantuml(text):
    """Encode PlantUML text for URL"""
    compressed = zlib.compress(text.encode('utf-8'))[2:-4]
    return encode64(compressed)

def encode64(data):
    """Encode bytes to PlantUML base64"""
    alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz-_"
    result = []
    for i in range(0, len(data), 3):
        if i + 2 < len(data):
            b1, b2, b3 = data[i], data[i+1], data[i+2]
        elif i + 1 < len(data):
            b1, b2, b3 = data[i], data[i+1], 0
        else:
            b1, b2, b3 = data[i], 0, 0

        result.append(alphabet[b1 >> 2])
        result.append(alphabet[((b1 & 0x3) << 4) | (b2 >> 4)])
        result.append(alphabet[((b2 & 0xF) << 2) | (b3 >> 6)])
        result.append(alphabet[b3 & 0x3F])

    return ''.join(result)

def download_diagram(plantuml_code, filename, output_dir, format='png'):
    """Download diagram from PlantUML server"""
    encoded = encode_plantuml(plantuml_code)
    url = f"https://www.plantuml.com/plantuml/{format}/{encoded}"

    filepath = os.path.join(output_dir, f"{filename}.{format}")

    try:
        print(f"  Downloading {filename}.{format}...")
        urllib.request.urlretrieve(url, filepath)
        print(f"  OK: {filepath}")
        return True
    except urllib.error.URLError as e:
        print(f"  ERROR: {filename} - {e}")
        return False

# Output directory
OUTPUT_DIR = os.path.join(os.path.dirname(__file__), "diagrams")
os.makedirs(OUTPUT_DIR, exist_ok=True)

# All diagrams
DIAGRAMS = {
    # 1. Layout Diagrams
    "01_GameScreenLayout": '''@startuml GameScreenLayout
!theme plain
skinparam backgroundColor #FEFEFE
skinparam handwritten false

title Game Screen Layout - Lane Defend: Monster Out!

rectangle "GAME SCREEN" as screen {

    rectangle "TOP UI BAR" as topbar #LightBlue {
        rectangle "Player\\nFortress HP\\n████░░" as php #LightGreen
        rectangle "Money" as money #Gold
        rectangle "Mana" as mana #LightCyan
        rectangle "Level" as level #LightGray
        rectangle "Wave\\nProgress\\n▓▓▓░░░" as wave #Orange
        rectangle "Enemy\\nFortress HP\\n████░░" as ehp #LightCoral
    }

    rectangle "BATTLEFIELD" as battlefield #Wheat {
        rectangle "PLAYER\\nFORTRESS\\n(X: -10)" as pfort #LightGreen

        rectangle "COMBAT ZONE\\n(X: -6 to +6)\\n\\nPlayer Units <-> Enemy Units" as combat #LightYellow

        rectangle "ENEMY\\nFORTRESS\\n(X: +10)" as efort #LightCoral
    }

    rectangle "BOTTOM UI BAR" as bottombar #LightBlue {
        rectangle "CHARACTER SUMMON PANEL" as summon #LightGray {
            rectangle "Archer\\n10" as archer #White
            rectangle "Gunner\\n15" as gunner #White
            rectangle "Shield\\n20" as shield #White
            rectangle "Strong\\n25" as strong #White
            rectangle "Warrior\\n12" as warrior #White
            rectangle "Healer\\n30" as healer #White
            rectangle "Ice\\n35" as ice #White
            rectangle "Poison\\n35" as poison #White
        }
        rectangle "Pause" as pause #Gray
        rectangle "Speed" as speed #Gray
    }
}

php -[hidden]r- money
money -[hidden]r- mana
mana -[hidden]r- level
level -[hidden]r- wave
wave -[hidden]r- ehp

pfort -[hidden]r- combat
combat -[hidden]r- efort

@enduml''',

    "02_BattlefieldZones": '''@startuml BattlefieldZones
!theme plain
skinparam backgroundColor #FEFEFE

title Battlefield Zones (X-axis: -12 to +12)

rectangle "PLAYER ZONE\\n(-12 to -6)" as pzone #LightGreen {
    rectangle "BASE\\nSpawn Point\\nX: -8.5" as pbase
}

rectangle "COMBAT ZONE\\n(-6 to +6)" as czone #LightYellow {
    rectangle "BATTLE AREA\\nUnits Fight Here" as battle
}

rectangle "ENEMY ZONE\\n(+6 to +12)" as ezone #LightCoral {
    rectangle "CAVE\\nEnemy Spawn\\nX: +9" as ecave
}

pzone -right-> czone : "Player Units\\nMove Right"
czone -right-> ezone : "Enemy Units\\nMove Left"

note bottom of czone
  Ground Level: Y = -1.5 (baseline)
  Player spawn: X: -8 to -9
  Enemy spawn: X: +9 to +10
end note

@enduml''',

    "03_LevelLayouts": '''@startuml LevelLayouts
!theme plain
skinparam backgroundColor #FEFEFE

title Level Layouts by Difficulty

rectangle "LEVEL 1-10: Basic (Tutorial)" as l1 #LightGreen {
    note right
        "Forest Outskirts"
        Enemies: Goblin, Mini Skeleton
        Waves: 3-5
        Enemy Fort HP: 200-500
    end note
}

rectangle "LEVEL 11-30: Intermediate" as l2 #LightYellow {
    note right
        "Dark Swamp / Ruins"
        Enemies: + Spearman, Gunner Zombie, Fly Bee
        Waves: 5-8
        Enemy Fort HP: 500-1500
    end note
}

rectangle "LEVEL 31-60: Advanced" as l3 #Orange {
    note right
        "Mountain / Cave"
        Enemies: + Bomber, Troll Warrior
        Waves: 8-12
        Enemy Fort HP: 1500-2500
    end note
}

rectangle "LEVEL 61-100: Boss & End Game" as l4 #LightCoral {
    note right
        "Troll Kingdom / Final"
        Enemies: ALL + Troll Shield, Troll Boss
        Waves: 10-15 + Boss wave
        Enemy Fort HP: 2500-3000
    end note
}

l1 -down-> l2
l2 -down-> l3
l3 -down-> l4

@enduml''',

    "04_BossLevelLayout": '''@startuml BossLevelLayout
!theme plain
skinparam backgroundColor #FEFEFE

title Boss Level Layout (Level 10, 25, 50, 75, 100)

rectangle "BOSS BATTLE ARENA" as arena #LightCoral {

    rectangle "PLAYER FORTRESS\\nHP: 1500" as pfort #LightGreen

    rectangle "EXTENDED BATTLEFIELD\\n\\nFULL PLAYER ARMY" as battle #LightYellow

    rectangle "BOSS ARENA\\n\\nTROLL BOSS\\nHP: 2000+" as boss #Red

    pfort -right-> battle
    battle -right-> boss
}

note bottom of arena
  Boss Mechanics:
  * Multi-stage fight (3 phases)
  * Summons minions periodically
  * Increased damage in later phases
  * Victory rewards: 50-100 coins
end note

@enduml''',

    # 2. Technology Diagrams
    "05_TechnologyStack": '''@startuml TechnologyStack
!theme plain
skinparam backgroundColor #FEFEFE

title Technology Architecture - Lane Defend: Monster Out!

package "GAME ENGINE" as engine #LightBlue {
    component "Unity 2022.3 LTS" as unity
    component "C# Scripting" as csharp
}

package "GRAPHICS" as graphics #LightGreen {
    component "Aseprite\\n(Sprites)" as aseprite
    component "Photoshop/GIMP\\n(UI)" as photoshop
}

package "AUDIO" as audio #LightYellow {
    component "Audacity\\n(SFX)" as audacity
    component "Unity Audio\\nSystem" as unityaudio
}

package "VERSION CONTROL" as vcs #LightGray {
    component "Git + GitHub" as git
}

package "IDE" as ide #LightCyan {
    component "Visual Studio 2022\\n/VS Code" as vs
}

unity --> csharp : uses
unity --> unityaudio : integrates
graphics --> unity : assets
audacity --> unityaudio : SFX files
csharp --> git : commits
vs --> csharp : edits

@enduml''',

    "06_TechnologyDetails": '''@startuml TechnologyDetails
!theme plain
skinparam backgroundColor #FEFEFE

title Technology Details

rectangle "GAME ENGINE" as ge #LightBlue {
    rectangle "Unity Engine 2022.3 LTS" as unity {
        card "2D Sprite Renderer"
        card "Animation System (Mecanim)"
        card "Physics 2D"
        card "UI System (Canvas)"
        card "Audio Source & Mixer"
    }
}

rectangle "PROGRAMMING" as prog #LightGreen {
    rectangle "C# (C-Sharp)" as csharp {
        card "MonoBehaviour Scripts"
        card "ScriptableObjects"
        card "Singleton Pattern"
        card "Observer Pattern"
        card "Object Pooling"
    }
}

rectangle "GRAPHICS TOOLS" as gfx #LightYellow {
    rectangle "Art Creation" as art {
        card "Aseprite - Pixel Art"
        card "Photoshop/GIMP - UI"
    }
}

rectangle "VERSION CONTROL" as vc #LightGray {
    rectangle "Git + GitHub" as git {
        card "Feature branch workflow"
        card "Pull request reviews"
        card "Issue tracking"
    }
}

ge -[hidden]down- prog
prog -[hidden]down- gfx
gfx -[hidden]down- vc

@enduml''',

    "07_UnityProjectStructure": '''@startuml UnityProjectStructure
!theme plain
skinparam backgroundColor #FEFEFE

title Unity Project Structure

package "Assets/" as assets {

    package "_MonstersOut/" as main {

        folder "Scenes/" as scenes {
            file "MainMenu.unity"
            file "MapSelection.unity"
            file "Shop.unity"
            file "GamePlay.unity"
            file "Settings.unity"
        }

        folder "Scripts/" as scripts {
            folder "AI/"
            folder "Controllers/"
            folder "Managers/"
            folder "UI/"
            folder "Helpers/"
        }

        folder "Prefabs/" as prefabs {
            folder "Character/"
            folder "Enemy/"
            folder "Projectile/"
            folder "VFX/"
        }

        folder "Sprite/" as sprite {
            folder "Character/"
            folder "Enemy/"
            folder "Background/"
            folder "GUI/"
        }

        folder "Audio/" as audio {
            folder "Music/"
            folder "Sound/"
        }

        folder "Animation/" as anim {
            folder "Character/"
            folder "Enemy/"
        }
    }

    folder "Resources/" as resources {
        folder "LevelData/"
        folder "CharacterData/"
    }
}

@enduml''',

    "08_DataFlowArchitecture": '''@startuml DataFlowArchitecture
!theme plain
skinparam backgroundColor #FEFEFE

title Data Flow Architecture

actor "Player" as player

rectangle "INPUT" as input #LightCyan {
    usecase "Click UI" as click
    usecase "Summon Unit" as summon
}

rectangle "UI SYSTEM" as uisys #LightBlue {
    component "MainMenuUI" as menuui
    component "GamePlayUI" as gameui
    component "ShopUI" as shopui
}

rectangle "MANAGERS (Singleton)" as managers #LightYellow {
    component "GameManager\\n(Game State)" as gm
    component "SpawnManager\\n(Waves)" as spawn
    component "SoundManager\\n(Audio)" as sound
    component "DataManager\\n(Save/Load)" as data
}

rectangle "BATTLEFIELD" as battlefield #LightGreen {
    component "Player Units" as punits
    component "Enemy Units" as eunits
    component "Player Fortress" as pfort
    component "Enemy Fortress" as efort
}

database "PlayerPrefs\\n(JSON Data)" as db

player --> input
input --> uisys
uisys <--> gm
gm --> spawn
gm --> sound
gm <--> data
data --> db

spawn --> punits
spawn --> eunits

punits <--> eunits : Combat
punits --> efort : Attack
eunits --> pfort : Attack

@enduml''',

    # 3. Team Diagrams
    "09_TeamStructure": '''@startuml TeamStructure
!theme plain
skinparam backgroundColor #FEFEFE

title Design Team Structure - Lane Defend: Monster Out!

rectangle "PROJECT LEADER (Team Lead/PM)" as lead #Gold {
    note right
        Responsibilities:
        * Project management
        * Task assignment
        * Review & QA
    end note
}

rectangle "GAME DESIGNER" as gd #LightBlue {
    note right
        Responsibilities:
        * Game mechanics
        * Level design
        * Balance tuning
        * Documentation

        Tools: Excel, Docs
    end note
}

rectangle "PROGRAMMER" as prog #LightGreen {
    note right
        Responsibilities:
        * Core gameplay
        * AI systems
        * UI programming
        * Optimization

        Tools: Unity, VS, Git
    end note
}

rectangle "ARTIST" as artist #LightYellow {
    note right
        Responsibilities:
        * Character design
        * UI/UX design
        * Animation
        * Visual effects

        Tools: Aseprite, PS, GIMP
    end note
}

lead -down-> gd
lead -down-> prog
lead -down-> artist

@enduml''',

    "10_TeamWorkflow": '''@startuml TeamWorkflow
!theme plain
skinparam backgroundColor #FEFEFE

title Team Workflow Diagram

|#LightCyan|Ideation|
start
:Brainstorm;
:Research;

|#LightBlue|Design|
:Document;
:Prototype;

|#LightGreen|Develop|
:Implement;
:Integrate;

|#LightYellow|Test|
:QA & Debug;
:Feedback;

|#Orange|Review|
:Code Review;
:Art Review;
:Play Test;

|#LightGreen|Release|
:Build;
:Deploy;
:Document;
stop

@enduml''',

    "11_CommunicationDiagram": '''@startuml CommunicationDiagram
!theme plain
skinparam backgroundColor #FEFEFE

title Communication Diagram

cloud "PROJECT REPOSITORY\\n(GitHub)" as github #LightGray {
    card "Code commits"
    card "Pull requests"
    card "Issue tracking"
    card "Documentation"
}

rectangle "DISCORD SERVER" as discord #LightBlue {
    card "Real-time Discussion"
    card "Voice chat"
}

rectangle "TRELLO BOARD" as trello #LightGreen {
    card "Task management"
    card "Sprint plan"
    card "Deadlines"
}

rectangle "GOOGLE DRIVE" as drive #LightYellow {
    card "Assets"
    card "Documents"
    card "Backups"
}

github <--> discord
github <--> trello
github <--> drive
discord <--> trello
trello <--> drive

note bottom
    Meeting Schedule:
    Monday: Sprint Planning (30 min)
    Wednesday: Progress Check-in (15 min)
    Friday: Sprint Review & Demo (45 min)
end note

@enduml''',

    # 4. Implementation Diagrams
    "12_ProjectTimeline": '''@startgantt ProjectTimeline
title Implementation Timeline - Lane Defend: Monster Out!

printscale weekly

Project starts 2024-11-01

-- Phase 1: Concept --
[Concept & Research] lasts 5 days
[GDD & TDD Documents] lasts 7 days

-- Phase 2: Design --
[UI/UX Design] lasts 10 days
[Character Design] lasts 12 days
[Level Design Plan] lasts 12 days

-- Phase 3: Development --
[Core Systems Dev] lasts 10 days
[Player Units] lasts 12 days
[Enemy Units] lasts 12 days
[UI Implementation] lasts 14 days
[Shop System] lasts 10 days

-- Phase 4: Content --
[Level Content] lasts 12 days
[Audio Integration] lasts 10 days
[VFX & Polish] lasts 10 days

-- Phase 5: Testing --
[Testing & QA] lasts 12 days
[Bug Fixes] lasts 10 days
[Final Build] lasts 5 days

@endgantt''',

    "13_PhaseBreakdown": '''@startuml PhaseBreakdown
!theme plain
skinparam backgroundColor #FEFEFE

title Phase Breakdown

rectangle "PHASE 1: CONCEPT (Week 1)" as p1 #LightGreen {
    card "Research games"
    card "Core mechanics"
    card "Gameplay flow"
    card "Art style"
}

rectangle "PHASE 2: DESIGN (Week 2)" as p2 #LightBlue {
    card "UI/UX Design"
    card "Character stats"
    card "Level design"
    card "Unity setup"
}

rectangle "PHASE 3: DEVELOPMENT (Week 3-5)" as p3 #LightYellow {
    card "Core Systems"
    card "8 Player characters"
    card "9 Enemy types"
    card "Shop system"
    card "100 Levels"
}

rectangle "PHASE 4: TESTING (Week 6-7)" as p4 #Orange {
    card "Playtesting"
    card "Bug fixing"
    card "Balance"
    card "Final build"
}

p1 -down-> p2 : 100%
p2 -down-> p3 : 100%
p3 -down-> p4 : 80%

@enduml''',

    # 5. Gameplay Diagrams
    "14_MainGameFlow": '''@startuml MainGameFlow
!theme plain
skinparam backgroundColor #FEFEFE

title Main Game Flow Diagram

start

:START GAME;

:SPLASH SCREEN;

:MAIN MENU;

fork
    :PLAY;
    :LEVEL SELECTION;
    :LOADING SCREEN;
    :GAMEPLAY;

    fork
        :VICTORY;
        :+Coins, +Stars;
        :Next Level?;
    fork again
        :DEFEAT;
        :Retry?;
    fork again
        :PAUSE MENU;
        :Resume/Restart/Menu;
    end fork

fork again
    :SHOP;
    :Upgrade Characters;
fork again
    :SETTINGS;
    :Volume/Graphics;
end fork

:Back to MAIN MENU;

stop

@enduml''',

    "15_BattleFlow": '''@startuml BattleFlow
!theme plain
skinparam backgroundColor #FEFEFE

title Battle Flow Diagram

start

:BATTLE START;

:INITIALIZE BATTLE;
note right
    * Load level config
    * Set fortress HP
    * Init mana & coins
    * Start wave timer
end note

fork
    :ENEMY SPAWN;
fork again
    :PLAYER ACTION;
fork again
    :AUTO BATTLE;
end fork

:GAME STATE CHECK;

if (Enemy Fort HP = 0?) then (yes)
    :PLAYER WINS;
    :Victory UI;
    :Give rewards;
elseif (Player Fort HP = 0?) then (yes)
    :PLAYER LOSES;
    :Defeat UI;
    :Offer retry;
else (Both > 0)
    :CONTINUE;
endif

stop

@enduml''',

    "16_UnitCombatFlow": '''@startuml UnitCombatFlow
!theme plain
skinparam backgroundColor #FEFEFE

title Unit Combat Flow - State Machine

[*] --> Spawned

Spawned --> Idle : Initialize

Idle --> CheckEnemy : Every frame

CheckEnemy --> Walk : No enemy
CheckEnemy --> CheckRange : Enemy found

Walk --> CheckEnemy : Move toward base

CheckRange --> Attack : In range
CheckRange --> Walk : Out of range

Attack --> CheckEnemy : Complete
Attack --> TakeDamage : Hit

Walk --> TakeDamage : Hit

TakeDamage --> CheckHP

CheckHP --> Hurt : HP > 0
CheckHP --> Death : HP <= 0

Hurt --> CheckEnemy

Death --> DropCoins
DropCoins --> [*]

@enduml''',

    "17_PlayerProgressionFlow": '''@startuml PlayerProgressionFlow
!theme plain
skinparam backgroundColor #FEFEFE

title Player Progression Flow

start

:NEW PLAYER (LV.1);

:STARTING RESOURCES;
note right
    Units: Archer, Warrior Boy
    Coins: 100
    Levels: 1 unlocked
end note

repeat
    :PLAY LEVEL;

    fork
        :WIN LEVEL;
        :REWARD COINS;

        if (HP >= 80%) then (3 stars)
            :1.5x coins;
        elseif (HP >= 50%) then (2 stars)
            :1.2x coins;
        else (1 star)
            :1.0x coins;
        endif

    fork again
        :LOSE LEVEL;
        :Retry;
    end fork

    :SHOP SYSTEM;

repeat while (More levels?)

:GAME COMPLETE;

stop

@enduml''',

    "18_CharacterUnlock": '''@startuml CharacterUnlock
!theme plain
skinparam backgroundColor #FEFEFE

title Character Unlock Progression

rectangle "LEVEL 1" as l1 #LightGreen {
    card "Archer"
    card "Warrior Boy"
}

rectangle "LEVEL 3" as l3 #LightGreen {
    card "Gunner"
}

rectangle "LEVEL 5" as l5 #LightBlue {
    card "Shieldman"
}

rectangle "LEVEL 8" as l8 #LightBlue {
    card "Strongman"
}

rectangle "LEVEL 10" as l10 #LightYellow {
    card "Wizard Healer"
}

rectangle "LEVEL 12" as l12 #Orange {
    card "Wizard Ice"
    card "Wizard Poison"
}

l1 -down-> l3 : Play & Win
l3 -down-> l5 : Play & Win
l5 -down-> l8 : Play & Win
l8 -down-> l10 : Play & Win
l10 -down-> l12 : Play & Win

@enduml''',

    "19_GameStateMachine": '''@startuml GameStateMachine
!theme plain
skinparam backgroundColor #FEFEFE

title Complete Game State Machine

[*] --> Boot

Boot --> Splash : App Start

Splash --> MainMenu : Timer/Click

MainMenu --> LevelSelect : Play
MainMenu --> Shop : Shop
MainMenu --> Settings : Settings
MainMenu --> [*] : Exit

Shop --> MainMenu : Back
Settings --> MainMenu : Back

LevelSelect --> Loading : Select Level
LevelSelect --> MainMenu : Back

Loading --> Gameplay : Load Complete

Gameplay --> Pause : Pause Button
Gameplay --> Victory : Enemy Fort HP = 0
Gameplay --> Defeat : Player Fort HP = 0

Pause --> Gameplay : Resume
Pause --> LevelSelect : Menu
Pause --> Gameplay : Restart

Victory --> LevelSelect : Continue
Victory --> MainMenu : Menu
Victory --> Gameplay : Replay

Defeat --> Gameplay : Retry
Defeat --> LevelSelect : Level Select
Defeat --> MainMenu : Menu

@enduml''',

    "20_EnemyWaveSystem": '''@startuml EnemyWaveSystem
!theme plain
skinparam backgroundColor #FEFEFE

title Enemy Wave System

start

:Level Start;

:Load Wave Config;

:currentWave = 1;

while (currentWave <= totalWaves?) is (yes)
    :Start Wave Timer;
    :Display "Wave X";

    while (Enemies to spawn?) is (yes)
        :Wait spawnDelay;
        :Select enemy type;
        :Spawn enemy;
    endwhile

    :Wait all enemies dead;

    if (Boss wave?) then (yes)
        :Spawn BOSS;
        :Wait BOSS dead;
    endif

    :currentWave++;
endwhile (no)

:All waves complete;

if (Player HP > 0?) then (yes)
    :VICTORY;
else (no)
    :DEFEAT;
endif

stop

@enduml''',

    "21_ShopUpgradeSystem": '''@startuml ShopUpgradeSystem
!theme plain
skinparam backgroundColor #FEFEFE

title Shop Upgrade System

start

:Open Shop;

:Display Character List;

:Select Character;

:Display Upgrade Options;

fork
    :Upgrade HP;
fork again
    :Upgrade Damage;
fork again
    :Upgrade Crit Rate;
fork again
    :Upgrade Fortress;
end fork

if (Enough coins?) then (yes)
    :Deduct coins;
    :Apply upgrade;
    :Save data;
    :Show success;
else (no)
    :Show "Not enough coins";
endif

:Back to Shop;

stop

@enduml''',

    # 6. Class Diagrams
    "22_CoreClasses": '''@startuml CoreClasses
!theme plain
skinparam backgroundColor #FEFEFE

title Core Classes Diagram

abstract class Unit {
    - health: float
    - maxHealth: float
    - damage: float
    - attackRange: float
    - moveSpeed: float
    - currentState: UnitState
    --
    + TakeDamage(amount: float)
    + Attack(target: Unit)
    + Move(direction: Vector2)
    + Die()
}

class PlayerUnit extends Unit {
    - manaCost: int
    - critRate: float
    - upgradeLevel: int
    --
    + Spawn()
    + ApplyUpgrade()
}

class EnemyUnit extends Unit {
    - coinDrop: int
    - waveNumber: int
    --
    + Spawn()
    + DropCoins()
}

class Fortress {
    - health: float
    - maxHealth: float
    - isPlayerFortress: bool
    --
    + TakeDamage(amount: float)
    + OnDestroyed()
}

enum UnitState {
    IDLE
    WALK
    ATTACK
    HURT
    DEATH
}

Unit --> UnitState

@enduml''',

    "23_ManagerClasses": '''@startuml ManagerClasses
!theme plain
skinparam backgroundColor #FEFEFE

title Manager Classes (Singleton Pattern)

class GameManager <<Singleton>> {
    - {static} instance: GameManager
    - gameState: GameState
    - isPaused: bool
    --
    + {static} Instance: GameManager
    + StartGame()
    + PauseGame()
    + ResumeGame()
    + GameOver(isWin: bool)
}

class SoundManager <<Singleton>> {
    - {static} instance: SoundManager
    - bgmSource: AudioSource
    - sfxSources: AudioSource[]
    --
    + {static} Instance: SoundManager
    + PlayBGM(clip: AudioClip)
    + PlaySFX(clip: AudioClip)
    + SetVolume(vol: float)
}

class DataManager <<Singleton>> {
    - {static} instance: DataManager
    - playerData: PlayerData
    --
    + {static} Instance: DataManager
    + SaveGame()
    + LoadGame()
    + GetCoins(): int
    + AddCoins(amount: int)
}

class SpawnManager <<Singleton>> {
    - {static} instance: SpawnManager
    - playerPool: ObjectPool
    - enemyPool: ObjectPool
    --
    + {static} Instance: SpawnManager
    + SpawnPlayer(type: PlayerType)
    + SpawnEnemy(type: EnemyType)
    + StartWave(waveNum: int)
}

GameManager --> SoundManager : uses
GameManager --> DataManager : uses
GameManager --> SpawnManager : uses

@enduml''',
}

def main():
    print("=" * 60)
    print("PlantUML Diagram Generator")
    print("Lane Defend: Monster Out!")
    print("=" * 60)
    print(f"\nOutput directory: {OUTPUT_DIR}")
    print(f"Total diagrams: {len(DIAGRAMS)}")
    print("\nGenerating diagrams...\n")

    success_count = 0
    failed_count = 0

    for name, code in DIAGRAMS.items():
        result = download_diagram(code, name, OUTPUT_DIR, 'png')
        if result:
            success_count += 1
        else:
            failed_count += 1

    print("\n" + "=" * 60)
    print(f"COMPLETE!")
    print(f"Success: {success_count}/{len(DIAGRAMS)}")
    print(f"Failed: {failed_count}/{len(DIAGRAMS)}")
    print(f"\nDiagrams saved to: {OUTPUT_DIR}")
    print("=" * 60)

if __name__ == "__main__":
    main()
