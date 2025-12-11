#!/usr/bin/env python3
"""
Script to generate PlantUML diagrams using PlantUML web server
With retry and delay to avoid rate limiting
"""

import os
import zlib
import time
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

def download_diagram(plantuml_code, filename, output_dir, format='png', max_retries=3):
    """Download diagram from PlantUML server with retry"""
    encoded = encode_plantuml(plantuml_code)
    url = f"https://www.plantuml.com/plantuml/{format}/{encoded}"

    filepath = os.path.join(output_dir, f"{filename}.{format}")

    # Skip if already exists
    if os.path.exists(filepath) and os.path.getsize(filepath) > 1000:
        print(f"  SKIP (exists): {filename}.{format}")
        return True

    for attempt in range(max_retries):
        try:
            print(f"  Downloading {filename}.{format} (attempt {attempt + 1})...")

            req = urllib.request.Request(url)
            req.add_header('User-Agent', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36')

            with urllib.request.urlopen(req, timeout=30) as response:
                data = response.read()
                with open(filepath, 'wb') as f:
                    f.write(data)

            print(f"  OK: {filename}.{format}")
            return True

        except urllib.error.HTTPError as e:
            print(f"  Retry {attempt + 1}: HTTP {e.code}")
            if attempt < max_retries - 1:
                time.sleep(3)  # Wait before retry
        except Exception as e:
            print(f"  Retry {attempt + 1}: {e}")
            if attempt < max_retries - 1:
                time.sleep(3)

    print(f"  FAILED: {filename}")
    return False

# Output directory
OUTPUT_DIR = os.path.join(os.path.dirname(__file__), "diagrams")
os.makedirs(OUTPUT_DIR, exist_ok=True)

# All diagrams - simplified versions that work better
DIAGRAMS = {
    "02_BattlefieldZones": '''@startuml
skinparam backgroundColor white
title Battlefield Zones

rectangle "PLAYER ZONE\\n(-12 to -6)" as pzone #90EE90
rectangle "COMBAT ZONE\\n(-6 to +6)" as czone #FFFFE0
rectangle "ENEMY ZONE\\n(+6 to +12)" as ezone #FFA07A

pzone -right-> czone : Player Units
czone -right-> ezone : Enemy Units

@enduml''',

    "03_LevelLayouts": '''@startuml
skinparam backgroundColor white
title Level Layouts by Difficulty

rectangle "LEVEL 1-10\\nBasic Tutorial" as l1 #90EE90
rectangle "LEVEL 11-30\\nIntermediate" as l2 #FFFFE0
rectangle "LEVEL 31-60\\nAdvanced" as l3 #FFA500
rectangle "LEVEL 61-100\\nBoss & End Game" as l4 #FF6347

l1 -down-> l2
l2 -down-> l3
l3 -down-> l4

@enduml''',

    "04_BossLevelLayout": '''@startuml
skinparam backgroundColor white
title Boss Level Layout

rectangle "PLAYER FORTRESS\\nHP: 1500" as pfort #90EE90
rectangle "BATTLEFIELD" as battle #FFFFE0
rectangle "BOSS ARENA\\nHP: 2000+" as boss #FF0000

pfort -right-> battle
battle -right-> boss

@enduml''',

    "05_TechnologyStack": '''@startuml
skinparam backgroundColor white
title Technology Stack

package "Game Engine" #ADD8E6 {
    [Unity 2022.3 LTS]
    [C# Scripting]
}

package "Graphics" #90EE90 {
    [Aseprite]
    [Photoshop]
}

package "Audio" #FFFFE0 {
    [Audacity]
    [Unity Audio]
}

package "Version Control" #D3D3D3 {
    [Git + GitHub]
}

@enduml''',

    "06_TechnologyDetails": '''@startuml
skinparam backgroundColor white
title Technology Details

rectangle "Unity Engine" as unity #ADD8E6 {
    card "2D Sprite Renderer"
    card "Animation System"
    card "Physics 2D"
    card "UI System"
}

rectangle "C# Programming" as csharp #90EE90 {
    card "MonoBehaviour"
    card "Singleton Pattern"
    card "Observer Pattern"
    card "Object Pooling"
}

@enduml''',

    "07_UnityProjectStructure": '''@startuml
skinparam backgroundColor white
title Unity Project Structure

folder "Assets" {
    folder "_MonstersOut" {
        folder "Scenes"
        folder "Scripts"
        folder "Prefabs"
        folder "Sprite"
        folder "Audio"
        folder "Animation"
    }
    folder "Resources"
}

@enduml''',

    "09_TeamStructure": '''@startuml
skinparam backgroundColor white
title Team Structure

rectangle "PROJECT LEADER" as lead #FFD700

rectangle "GAME DESIGNER" as gd #ADD8E6
rectangle "PROGRAMMER" as prog #90EE90
rectangle "ARTIST" as artist #FFFFE0

lead -down-> gd
lead -down-> prog
lead -down-> artist

@enduml''',

    "10_TeamWorkflow": '''@startuml
skinparam backgroundColor white
title Team Workflow

start
:Ideation;
:Design;
:Develop;
:Test;
:Review;
:Release;
stop

@enduml''',

    "12_ProjectTimeline": '''@startuml
skinparam backgroundColor white
title Project Timeline

rectangle "Week 1\\nConcept" as w1 #90EE90
rectangle "Week 2\\nDesign" as w2 #ADD8E6
rectangle "Week 3-5\\nDevelopment" as w35 #FFFFE0
rectangle "Week 6-7\\nTesting" as w67 #FFA500

w1 -right-> w2
w2 -right-> w35
w35 -right-> w67

@enduml''',

    "13_PhaseBreakdown": '''@startuml
skinparam backgroundColor white
title Phase Breakdown

rectangle "PHASE 1: Concept (W1)" as p1 #90EE90
rectangle "PHASE 2: Design (W2)" as p2 #ADD8E6
rectangle "PHASE 3: Development (W3-5)" as p3 #FFFFE0
rectangle "PHASE 4: Testing (W6-7)" as p4 #FFA500

p1 -down-> p2 : 100%
p2 -down-> p3 : 100%
p3 -down-> p4 : 80%

@enduml''',

    "14_MainGameFlow": '''@startuml
skinparam backgroundColor white
title Main Game Flow

start
:Splash Screen;
:Main Menu;

fork
    :Play;
    :Level Select;
    :Gameplay;
    if (Win?) then (yes)
        :Victory;
    else (no)
        :Defeat;
    endif
fork again
    :Shop;
fork again
    :Settings;
end fork

stop

@enduml''',

    "15_BattleFlow": '''@startuml
skinparam backgroundColor white
title Battle Flow

start
:Battle Start;
:Initialize;

fork
    :Enemy Spawn;
fork again
    :Player Action;
fork again
    :Auto Battle;
end fork

:Check State;

if (Enemy HP = 0?) then (yes)
    :Victory;
elseif (Player HP = 0?) then (yes)
    :Defeat;
else
    :Continue;
endif

stop

@enduml''',

    "16_UnitCombatFlow": '''@startuml
skinparam backgroundColor white
title Unit Combat State Machine

[*] --> Idle
Idle --> Walk : No enemy
Idle --> Attack : Enemy found
Walk --> Idle : Check
Attack --> Idle : Complete
Attack --> Hurt : Hit
Hurt --> Idle : HP > 0
Hurt --> Death : HP = 0
Death --> [*]

@enduml''',

    "17_PlayerProgressionFlow": '''@startuml
skinparam backgroundColor white
title Player Progression

start
:New Player;
:Starting Resources;

repeat
    :Play Level;
    if (Win?) then (yes)
        :Coins Reward;
    else (no)
        :Retry;
    endif
    :Shop Upgrade;
repeat while (More levels?)

:Game Complete;
stop

@enduml''',

    "19_GameStateMachine": '''@startuml
skinparam backgroundColor white
title Game State Machine

[*] --> Boot
Boot --> Splash
Splash --> MainMenu
MainMenu --> LevelSelect : Play
MainMenu --> Shop
MainMenu --> Settings
LevelSelect --> Gameplay
Gameplay --> Victory : Win
Gameplay --> Defeat : Lose
Victory --> LevelSelect
Defeat --> Gameplay : Retry
Shop --> MainMenu
Settings --> MainMenu

@enduml''',
}

def main():
    print("=" * 60)
    print("PlantUML Diagram Generator v2")
    print("Lane Defend: Monster Out!")
    print("=" * 60)
    print(f"\nOutput directory: {OUTPUT_DIR}")
    print(f"Total diagrams to generate: {len(DIAGRAMS)}")
    print("\nGenerating diagrams with delay...\n")

    success_count = 0
    failed_count = 0

    for i, (name, code) in enumerate(DIAGRAMS.items()):
        result = download_diagram(code, name, OUTPUT_DIR, 'png')
        if result:
            success_count += 1
        else:
            failed_count += 1

        # Delay between requests to avoid rate limiting
        if i < len(DIAGRAMS) - 1:
            time.sleep(2)

    print("\n" + "=" * 60)
    print(f"COMPLETE!")
    print(f"Success: {success_count}/{len(DIAGRAMS)}")
    print(f"Failed: {failed_count}/{len(DIAGRAMS)}")
    print(f"\nDiagrams saved to: {OUTPUT_DIR}")
    print("=" * 60)

if __name__ == "__main__":
    main()
