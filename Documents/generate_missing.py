#!/usr/bin/env python3
"""Generate remaining missing diagrams"""

import os
import zlib
import time
import urllib.request
import urllib.error

def encode_plantuml(text):
    compressed = zlib.compress(text.encode('utf-8'))[2:-4]
    return encode64(compressed)

def encode64(data):
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

def download_diagram(plantuml_code, filename, output_dir, max_retries=5):
    encoded = encode_plantuml(plantuml_code)
    url = f"https://www.plantuml.com/plantuml/png/{encoded}"
    filepath = os.path.join(output_dir, f"{filename}.png")

    if os.path.exists(filepath) and os.path.getsize(filepath) > 1000:
        print(f"  SKIP: {filename}.png exists")
        return True

    for attempt in range(max_retries):
        try:
            print(f"  Downloading {filename}.png (attempt {attempt + 1})...")
            time.sleep(5)  # Longer delay

            req = urllib.request.Request(url)
            req.add_header('User-Agent', 'Mozilla/5.0')

            with urllib.request.urlopen(req, timeout=30) as response:
                data = response.read()
                with open(filepath, 'wb') as f:
                    f.write(data)
            print(f"  OK: {filename}.png")
            return True
        except Exception as e:
            print(f"  Retry {attempt + 1}: {e}")
            time.sleep(5)

    print(f"  FAILED: {filename}")
    return False

OUTPUT_DIR = os.path.join(os.path.dirname(__file__), "diagrams")

MISSING = {
    "03_LevelLayouts": '''@startuml
title Levels
rectangle L1 #90EE90
rectangle L2 #FFFFE0
rectangle L3 #FFA500
rectangle L4 #FF6347
L1 -d-> L2
L2 -d-> L3
L3 -d-> L4
@enduml''',

    "04_BossLevelLayout": '''@startuml
title Boss Level
rectangle Player #90EE90
rectangle Battle #FFFFE0
rectangle Boss #FF0000
Player -r-> Battle
Battle -r-> Boss
@enduml''',

    "14_MainGameFlow": '''@startuml
title Game Flow
start
:Menu;
:Play;
:Result;
stop
@enduml''',

    "17_PlayerProgressionFlow": '''@startuml
title Player Progress
start
:New Player;
:Play;
:Reward;
:Upgrade;
stop
@enduml''',
}

def main():
    print("Generating missing diagrams...")
    for name, code in MISSING.items():
        download_diagram(code, name, OUTPUT_DIR)
        time.sleep(3)
    print("Done!")

if __name__ == "__main__":
    main()
