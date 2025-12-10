# File Mapping - English ↔ Vietnamese

**Mục Đích**: Quick reference cho mapping giữa English và Vietnamese documentation files.

**Cách Dùng**:
- Tìm English file → Vietnamese equivalent
- Tìm Vietnamese file → English source
- Update cross-references trong documentation

---

## Complete File Mapping Table

| # | English File | Vietnamese File | Category |
|---|--------------|-----------------|----------|
| 1 | `Translation-Glossary.md` | `Translation-Glossary.md` | Foundation |
| 2 | `Translation-Style-Guide.md` | `Translation-Style-Guide.md` | Foundation |
| 3 | `00_START_HERE.md` | `00_BAT_DAU_TU_DAY.md` | Core |
| 4 | `00_Unity_Fundamentals.md` | `00_Cac_Khai_Niem_Unity_Co_Ban.md` | Core |
| 5 | `01_Project_Architecture.md` | `01_Kien_Truc_Project.md` | Core |
| 6 | `README.md` | `README_VI.md` | Core |
| 7 | `02_Player_System_Complete.md` | `02_He_Thong_Player_Day_Du.md` | System |
| 8 | `03_Enemy_System_Complete.md` | `03_He_Thong_Enemy_Day_Du.md` | System |
| 9 | `04_UI_System_Complete.md` | `04_He_Thong_UI_Day_Du.md` | System |
| 10 | `05_Managers_Complete.md` | `05_Cac_Manager_Day_Du.md` | System |
| 11 | `10_How_To_Guides.md` | `10_Huong_Dan_Thuc_Hanh.md` | Practical |
| 12 | `11_Troubleshooting.md` | `11_Xu_Ly_Su_Co.md` | Practical |
| 13 | `First-Tasks.md` | `Nhiem_Vu_Dau_Tien.md` | Practical |
| 14 | `Workflow-Tasks.md` | `Quy_Trinh_Lam_Viec.md` | Practical |
| 15 | `12_Visual_Reference.md` | `12_Tham_Khao_Truc_Quan.md` | Reference |
| 16 | `13_Code_Examples.md` | `13_Vi_Du_Code.md` | Reference |
| 17 | `99_Glossary.md` | `99_Tu_Dien.md` | Reference |
| 18 | `Core-Objects.md` | `Cac_Doi_Tuong_Loi.md` | Deep Dive |
| 19 | `Character-Properties.md` | `Thuoc_Tinh_Nhan_Vat.md` | Deep Dive |
| 20 | `Enemy-Deep.md` | `He_Thong_Enemy_Nang_Cao.md` | Deep Dive |
| 21 | `Events-and-Triggers.md` | `Su_Kien_Va_Trigger.md` | Deep Dive |
| 22 | `Player-Deep.md` | `He_Thong_Player_Nang_Cao.md` | Deep Dive |
| 23 | `Map.md` | `Ban_Do.md` | Deep Dive |
| 24 | `Namespaces.md` | `Namespace.md` | Deep Dive |
| 25 | `ShopUI.md` | `He_Thong_Shop_UI.md` | Deep Dive |
| 26 | `Unity-Concepts.md` | `Cac_Khai_Niem_Unity.md` | Deep Dive |
| 27 | `Roadmap.md` | `Lo_Trinh_Hoc_Tap.md` | Management |
| 28 | `project-analysis.md` | `Phan_Tich_Project.md` | Management |

---

## Quick Lookup by Category

### Foundation (2 files)
```
Translation-Glossary.md         → Translation-Glossary.md (same)
Translation-Style-Guide.md      → Translation-Style-Guide.md (same)
```

### Core Documentation (4 files)
```
00_START_HERE.md                → 00_BAT_DAU_TU_DAY.md
00_Unity_Fundamentals.md        → 00_Cac_Khai_Niem_Unity_Co_Ban.md
01_Project_Architecture.md      → 01_Kien_Truc_Project.md
README.md                       → README_VI.md
```

### System Documentation (4 files)
```
02_Player_System_Complete.md    → 02_He_Thong_Player_Day_Du.md
03_Enemy_System_Complete.md     → 03_He_Thong_Enemy_Day_Du.md
04_UI_System_Complete.md        → 04_He_Thong_UI_Day_Du.md
05_Managers_Complete.md         → 05_Cac_Manager_Day_Du.md
```

### Practical Guides (4 files)
```
10_How_To_Guides.md             → 10_Huong_Dan_Thuc_Hanh.md
11_Troubleshooting.md           → 11_Xu_Ly_Su_Co.md
First-Tasks.md                  → Nhiem_Vu_Dau_Tien.md
Workflow-Tasks.md               → Quy_Trinh_Lam_Viec.md
```

### Reference Materials (3 files)
```
12_Visual_Reference.md          → 12_Tham_Khao_Truc_Quan.md
13_Code_Examples.md             → 13_Vi_Du_Code.md
99_Glossary.md                  → 99_Tu_Dien.md
```

### Deep Dive Documentation (9 files)
```
Core-Objects.md                 → Cac_Doi_Tuong_Loi.md
Character-Properties.md         → Thuoc_Tinh_Nhan_Vat.md
Enemy-Deep.md                   → He_Thong_Enemy_Nang_Cao.md
Events-and-Triggers.md          → Su_Kien_Va_Trigger.md
Player-Deep.md                  → He_Thong_Player_Nang_Cao.md
Map.md                          → Ban_Do.md
Namespaces.md                   → Namespace.md
ShopUI.md                       → He_Thong_Shop_UI.md
Unity-Concepts.md               → Cac_Khai_Niem_Unity.md
```

### Project Management (2 files)
```
Roadmap.md                      → Lo_Trinh_Hoc_Tap.md
project-analysis.md             → Phan_Tich_Project.md
```

---

## Common Reference Patterns

### Pattern 1: "See X for Y"
```markdown
// English version:
See `00_Unity_Fundamentals.md` for Unity basics

// Vietnamese version should be:
Xem `00_Cac_Khai_Niem_Unity_Co_Ban.md` cho Unity basics
```

### Pattern 2: "Related Documentation"
```markdown
// English:
**Related Documentation**:
- See `Player-Deep.md` for player details
- See `Enemy-Deep.md` for enemy system

// Vietnamese:
**Tài Liệu Liên Quan**:
- Xem `He_Thong_Player_Nang_Cao.md` cho player details
- Xem `He_Thong_Enemy_Nang_Cao.md` cho enemy system
```

### Pattern 3: "Next Steps"
```markdown
// English:
**Next Steps**:
- Read `10_How_To_Guides.md`

// Vietnamese:
**Các Bước Tiếp Theo**:
- Đọc `10_Huong_Dan_Thuc_Hanh.md`
```

---

## Reverse Mapping (Vietnamese → English)

### Tìm English Source Từ Vietnamese File

```
00_BAT_DAU_TU_DAY.md              → 00_START_HERE.md
00_Cac_Khai_Niem_Unity_Co_Ban.md  → 00_Unity_Fundamentals.md
01_Kien_Truc_Project.md            → 01_Project_Architecture.md
02_He_Thong_Player_Day_Du.md       → 02_Player_System_Complete.md
03_He_Thong_Enemy_Day_Du.md        → 03_Enemy_System_Complete.md
04_He_Thong_UI_Day_Du.md           → 04_UI_System_Complete.md
05_Cac_Manager_Day_Du.md           → 05_Managers_Complete.md
10_Huong_Dan_Thuc_Hanh.md          → 10_How_To_Guides.md
11_Xu_Ly_Su_Co.md                  → 11_Troubleshooting.md
12_Tham_Khao_Truc_Quan.md          → 12_Visual_Reference.md
13_Vi_Du_Code.md                   → 13_Code_Examples.md
99_Tu_Dien.md                      → 99_Glossary.md
Ban_Do.md                          → Map.md
Cac_Doi_Tuong_Loi.md               → Core-Objects.md
Cac_Khai_Niem_Unity.md             → Unity-Concepts.md
He_Thong_Enemy_Nang_Cao.md         → Enemy-Deep.md
He_Thong_Player_Nang_Cao.md        → Player-Deep.md
He_Thong_Shop_UI.md                → ShopUI.md
Lo_Trinh_Hoc_Tap.md                → Roadmap.md
Namespace.md                       → Namespaces.md
Nhiem_Vu_Dau_Tien.md               → First-Tasks.md
Phan_Tich_Project.md               → project-analysis.md
Quy_Trinh_Lam_Viec.md              → Workflow-Tasks.md
README_VI.md                       → README.md
Su_Kien_Va_Trigger.md              → Events-and-Triggers.md
Thuoc_Tinh_Nhan_Vat.md             → Character-Properties.md
```

---

## Update Script Template

Để automated update cross-references, dùng script này:

```bash
#!/bin/bash
# update-references.sh

# Define mappings
declare -A file_map=(
    ["00_Unity_Fundamentals.md"]="00_Cac_Khai_Niem_Unity_Co_Ban.md"
    ["00_START_HERE.md"]="00_BAT_DAU_TU_DAY.md"
    ["Player-Deep.md"]="He_Thong_Player_Nang_Cao.md"
    ["Enemy-Deep.md"]="He_Thong_Enemy_Nang_Cao.md"
    # ... add all mappings
)

# Update file
for english in "${!file_map[@]}"; do
    vietnamese="${file_map[$english]}"
    # Replace in all Vietnamese .md files
    find . -name "*_VI.md" -o -name "*Cac*.md" -o -name "*He_Thong*.md" | \
    while read file; do
        sed -i "s/$english/$vietnamese/g" "$file"
    done
done
```

---

## Statistics

**Total Files**: 28
**Bilingual Pairs**: 28
**Foundation Files**: 2 (same name both languages)
**Translated Files**: 26 (unique Vietnamese names)

**Naming Patterns**:
- Numbered files: `XX_English_Name.md` → `XX_Vietnamese_Name.md`
- Descriptive files: `English-Name.md` → `Vietnamese_Name.md`
- Special files: `README.md` → `README_VI.md`

---

## Maintenance Notes

### When Adding New Files

1. Add to this mapping table
2. Update both English and Vietnamese versions
3. Ensure cross-references use correct filenames
4. Run automated link checker

### When Renaming Files

1. Update this mapping table first
2. Update all cross-references
3. Create redirect/note in old location
4. Verify all links work

---

**Last Updated**: 31 Tháng 10, 2025
**Version**: 1.0
**Maintained By**: Translation Team

---

**Kết Thúc File Mapping**

<p align="center">
<strong>Lawn Defense: Monsters Out</strong><br>
File Mapping - English ↔ Vietnamese<br>
Bilingual Documentation Index
</p>
