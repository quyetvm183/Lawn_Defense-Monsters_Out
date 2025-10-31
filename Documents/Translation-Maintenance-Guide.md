# Translation Maintenance Guide
## Hướng Dẫn Bảo Trì Bản Dịch

**Version**: 1.0
**Created**: 31 Tháng 10, 2025
**Purpose**: Comprehensive guide for maintaining and updating bilingual documentation

---

## Table of Contents / Mục Lục

1. [Overview](#1-overview--tng-quan)
2. [Translation Philosophy](#2-translation-philosophy--trit-lý-dch-thut)
3. [Update Workflow](#3-update-workflow--quy-trình-cp-nht)
4. [Quality Standards](#4-quality-standards--tiêu-chun-cht-lng)
5. [Tools & Resources](#5-tools--resources--công-c--tài-nguyn)
6. [Common Scenarios](#6-common-scenarios--tình-hung-thng-gp)
7. [Best Practices](#7-best-practices--thc-hành-tt-nht)
8. [Troubleshooting](#8-troubleshooting--x-lý-s-c)

---

## 1. Overview / Tổng Quan

### 1.1 Documentation Structure

**Current Status**:
- **Total Files**: 28 bilingual documentation files
- **English Files**: 26 unique documents
- **Vietnamese Files**: 26 translations + 2 foundation files
- **Bilingual Resources**: 2 (Glossary, Style Guide)
- **Quality Score**: 95/100

### 1.2 Maintenance Goals

**Primary Objectives**:
1. ✅ Keep translations synchronized with English updates
2. ✅ Maintain terminology consistency
3. ✅ Ensure cross-references remain accurate
4. ✅ Preserve technical accuracy
5. ✅ Provide timely updates

**Success Metrics**:
- Translation lag < 1 week after English updates
- Quality score > 90/100
- Zero broken cross-references
- Consistent terminology usage

---

## 2. Translation Philosophy / Triết Lý Dịch Thuật

### 2.1 Core Principles

As defined in `Translation-Style-Guide.md`:

**1. Technical Terms in English**
```
✅ KEEP: GameObject, MonoBehaviour, Prefab, Coroutine
❌ NEVER: Vật thể game, Đơn hành vi, Tiền chế
```

**2. Natural Vietnamese for Explanations**
```
✅ GOOD: "GameObject là container cho các components"
❌ BAD: "GameObject is a container for components" (too literal)
```

**3. Code Preservation**
```
All code blocks, class names, method names → Keep exactly as-is
Comments in code → Keep in original language
```

**4. Bilingual Approach**
```
Technical concept + Vietnamese explanation
Example: "Singleton pattern giúp đảm bảo chỉ có một instance"
```

### 2.2 What to Translate

**✅ DO TRANSLATE**:
- Instructions and explanations
- Section headers and titles
- Learning objectives
- Step-by-step guides
- Summaries and conclusions
- Table of contents

**❌ DO NOT TRANSLATE**:
- Technical terms (from Translation-Glossary.md)
- Code blocks and snippets
- Class names, method names, variable names
- Unity API calls
- File paths and folder names
- Design pattern names

---

## 3. Update Workflow / Quy Trình Cập Nhật

### 3.1 When English Documentation Changes

**Step-by-Step Process**:

#### Step 1: Identify Changes
```bash
# Check git diff for changed files
git diff main..HEAD -- Documents/*.md

# Or use git log
git log --oneline --since="1 week ago" -- Documents/
```

#### Step 2: Assess Impact
- [ ] What changed? (content, structure, code examples)
- [ ] Is it a major update or minor fix?
- [ ] Does it affect cross-references?
- [ ] Are new technical terms introduced?

#### Step 3: Update Translation

**For Minor Changes** (typos, small clarifications):
1. Directly update Vietnamese file
2. Maintain same section structure
3. Verify cross-references still work

**For Major Changes** (new sections, restructuring):
1. Read full English update
2. Update `Translation-Glossary.md` if new terms
3. Translate new content following style guide
4. Update cross-references
5. Update Table of Contents if needed

#### Step 4: Quality Check
```bash
# Run consistency checks
grep -r "GameObject" Vietnamese-files/ # Should find many
grep -r "Vật thể game" Vietnamese-files/ # Should find zero

# Check cross-references
# (Use patterns from Cross-Reference-Update-Summary.md)
```

#### Step 5: Document Changes
```bash
# Git commit with clear message
git add Documents/Vietnamese-File.md
git commit -m "docs: update Vietnamese translation for [File]

- Translated new Section X
- Updated cross-references
- Added new terms to glossary"
```

### 3.2 Monthly Maintenance Routine

**First Week of Month**:
- [ ] Review all English files changed in past month
- [ ] Update corresponding Vietnamese files
- [ ] Run quality checks (see Section 4.3)

**Second Week**:
- [ ] Update `Translation-Glossary.md` with new terms
- [ ] Review cross-references for accuracy
- [ ] Check for consistency issues

**Third Week**:
- [ ] User feedback review (if any)
- [ ] Address reported issues
- [ ] Update Translation-QA-Report.md

**Fourth Week**:
- [ ] Generate updated statistics
- [ ] Plan next month's priorities
- [ ] Documentation cleanup

---

## 4. Quality Standards / Tiêu Chuẩn Chất Lượng

### 4.1 Quality Checklist

Before committing any translation update:

**Terminology** ✅
- [ ] All technical terms preserved in English
- [ ] Consistent with Translation-Glossary.md
- [ ] No Vietnamese translations of Unity API

**Structure** ✅
- [ ] Same section hierarchy as English
- [ ] Table of contents updated
- [ ] Headers match English structure

**Cross-References** ✅
- [ ] All internal links point to Vietnamese files
- [ ] No broken references
- [ ] Relative paths correct

**Code Blocks** ✅
- [ ] Exact copy from English version
- [ ] Proper markdown formatting
- [ ] No translation in code

**Formatting** ✅
- [ ] Lists formatted correctly
- [ ] Tables aligned properly
- [ ] ASCII diagrams preserved

### 4.2 Automated Checks

Use these commands before committing:

```bash
# Check for accidentally translated technical terms
grep -r "Vật thể game\|Đơn hành vi\|Tiền chế" Documents/*VI*.md
# Should return zero results

# Check for English "See" in Vietnamese files
grep -r "^See \|See \`" Documents/*{BAT,Cac,He,Thuoc}*.md
# Should return zero results

# Check for old filename references
grep -r "Unity_Co_Ban\.md\|Tu_Dien_Thuat" Documents/
# Should return zero results (except this guide)

# Verify Vietnamese filenames in cross-references
grep -r "`.*\.md`" Documents/00_BAT_DAU_TU_DAY.md | \
  grep -v "Cac_Khai_Niem_Unity_Co_Ban\|He_Thong_Player"
# Should only show correct Vietnamese names
```

### 4.3 Quality Score Calculation

Based on `Translation-QA-Report.md` methodology:

**Scoring System**:
- **Terminology Consistency**: 25 points
- **Technical Accuracy**: 25 points
- **Formatting**: 20 points
- **Completeness**: 15 points
- **Style Compliance**: 10 points
- **Usability**: 5 points

**Passing Grade**: 90/100 minimum

---

## 5. Tools & Resources / Công Cụ & Tài Nguyên

### 5.1 Essential Files

**Reference Documents**:
1. `Translation-Glossary.md` - Terminology database
2. `Translation-Style-Guide.md` - Style rules
3. `File-Mapping-Bilingual.md` - Filename mappings
4. `Bilingual-Navigation-Index.md` - Navigation guide
5. `Translation-QA-Report.md` - Quality baseline

### 5.2 Helpful Commands

**Search for specific patterns**:
```bash
# Find all .md files with specific term
grep -r "auto-targeting" Documents/*.md

# Find Vietnamese translations of English file
grep "English-File.md" Documents/File-Mapping-Bilingual.md

# Count total lines in Vietnamese docs
wc -l Documents/*{BAT,Cac,Kien,He}*.md | tail -1
```

**Batch operations**:
```bash
# Replace old reference across multiple files
for file in Documents/*VI*.md; do
  sed -i 's/Old-Reference\.md/New-Reference.md/g' "$file"
done

# Find files modified in last week
find Documents/ -name "*.md" -mtime -7
```

### 5.3 Markdown Tools

**Recommended Tools**:
- **VS Code**: With markdown extensions
- **Obsidian**: For viewing linked documents
- **Typora**: For WYSIWYG editing
- **Pandoc**: For format conversions (if needed)

**VS Code Extensions**:
- Markdown All in One
- Markdown Preview Enhanced
- Vietnamese Language Support

---

## 6. Common Scenarios / Tình Huống Thường Gặp

### 6.1 Scenario: New English File Created

**Example**: English team creates `06_Sound_System_Complete.md`

**Actions**:
1. Review English file for technical terms
2. Add new terms to `Translation-Glossary.md`
3. Create `06_He_Thong_Anh_Thanh_Day_Du.md`
4. Translate following style guide
5. Update `File-Mapping-Bilingual.md`
6. Update `Bilingual-Navigation-Index.md`
7. Update cross-references in related files

**Template**:
```markdown
# [Vietnamese Title]

**Ngôn ngữ:** Tiếng Việt
**File gốc:** [06_Sound_System_Complete.md](06_Sound_System_Complete.md)
**Cập nhật lần cuối:** [Date]

[Translation content...]
```

### 6.2 Scenario: English File Structure Changed

**Example**: `02_Player_System_Complete.md` sections reordered

**Actions**:
1. Read full English update
2. Update `02_He_Thong_Player_Day_Du.md` structure
3. Re-translate affected sections
4. Update Table of Contents
5. Verify cross-references from other docs
6. Test internal navigation links

### 6.3 Scenario: New Technical Term Introduced

**Example**: English docs now use "State Machine Pattern"

**Actions**:
1. Add to `Translation-Glossary.md`:
   ```
   State Machine Pattern | State Machine Pattern | Không dịch
   ```
2. Update style guide if needed
3. Ensure consistency in all Vietnamese files
4. Add usage example if complex

### 6.4 Scenario: Broken Cross-Reference Reported

**Example**: User reports `10_Huong_Dan_Thuc_Hanh.md` links to wrong file

**Actions**:
1. Identify the broken reference
2. Check `File-Mapping-Bilingual.md` for correct filename
3. Update reference in affected file
4. Run automated check (Section 4.2)
5. Update `Cross-Reference-Update-Summary.md`
6. Commit with clear message

### 6.5 Scenario: User Suggests Better Translation

**Example**: User suggests better Vietnamese phrase for a concept

**Review Process**:
1. Evaluate against Translation-Style-Guide.md
2. Check if change affects multiple files
3. If approved:
   - Update all affected files
   - Document in git commit
   - Add note to Translation-Glossary.md if applicable
4. Thank contributor!

---

## 7. Best Practices / Thực Hành Tốt Nhất

### 7.1 Translation Best Practices

**DO**:
✅ Read full English section before translating
✅ Use consistent terminology from glossary
✅ Maintain natural Vietnamese flow
✅ Test code examples if modified
✅ Update cross-references immediately
✅ Document significant changes in commits

**DON'T**:
❌ Translate technical terms
❌ Change code structure or logic
❌ Skip quality checks
❌ Commit without testing links
❌ Mix multiple unrelated updates
❌ Forget to update TOC

### 7.2 Git Workflow

**Commit Message Format**:
```
docs(vi): [action] for [file]

- Bullet point of changes
- Another change
- Reference to issue if applicable

Co-authored-by: [Name] <email>
```

**Examples**:
```
docs(vi): update Player system translation

- Translated new Section 8: Advanced Targeting
- Updated cross-references to He_Thong_Enemy_Nang_Cao.md
- Added 3 new terms to glossary

docs(vi): fix cross-references in 00_BAT_DAU_TU_DAY.md

- Replaced 00_Unity_Co_Ban.md with 00_Cac_Khai_Niem_Unity_Co_Ban.md
- Fixed ShopUI_VI.md to He_Thong_Shop_UI.md
```

### 7.3 Review Process

**Self-Review Checklist**:
- [ ] Read translation aloud - does it sound natural?
- [ ] All technical terms in English?
- [ ] Cross-references point to Vietnamese files?
- [ ] Code blocks unchanged from English?
- [ ] Formatting preserved?

**Peer Review** (if available):
1. Request review from bilingual developer
2. Focus on naturalness and clarity
3. Verify technical accuracy
4. Check consistency with existing translations

### 7.4 Documentation Updates

**When to Update Support Docs**:

Update `Translation-Glossary.md` when:
- New technical term introduced
- Term usage clarification needed

Update `File-Mapping-Bilingual.md` when:
- New file pair created
- File renamed
- File deleted

Update `Bilingual-Navigation-Index.md` when:
- New file added
- File category changed
- Navigation path updated

Update `Translation-QA-Report.md` when:
- Monthly review completed
- Quality issues found and fixed
- Major update batch completed

---

## 8. Troubleshooting / Xử Lý Sự Cố

### 8.1 Common Issues

**Issue**: "I accidentally translated a technical term"

**Solution**:
1. Check `Translation-Glossary.md` for correct term
2. Use global search/replace to fix
3. Run automated checks (Section 4.2)
4. Commit fix with clear message

**Issue**: "Cross-reference links are broken"

**Solution**:
1. Check `File-Mapping-Bilingual.md`
2. Verify filename exactly matches (case-sensitive)
3. Use relative paths, not absolute
4. Test link by clicking in preview

**Issue**: "Translation doesn't sound natural"

**Solution**:
1. Review Translation-Style-Guide.md Section 2.3
2. Prioritize clarity over literal translation
3. Use bilingual approach: technical term + explanation
4. Ask for peer review

**Issue**: "Can't find Vietnamese equivalent of English file"

**Solution**:
1. Check `File-Mapping-Bilingual.md`
2. Use `Bilingual-Navigation-Index.md` lookup tables
3. Search by English filename in File-Mapping

**Issue**: "Quality score dropped below 90"

**Solution**:
1. Run all automated checks (Section 4.2)
2. Review recent changes for issues
3. Check `Translation-QA-Report.md` for past issues
4. Systematically address each category

### 8.2 Getting Help

**Resources**:
1. **Translation-Style-Guide.md**: Style questions
2. **Translation-Glossary.md**: Term questions
3. **Translation-QA-Report.md**: Quality baseline
4. **GitHub Issues**: Report bugs or suggest improvements

**Community**:
- Open an issue with [Translation] tag
- Provide specific examples
- Reference relevant guide sections

---

## Appendix A: Quick Reference

### Translation Decision Tree

```
New English content to translate?
    │
    ├─→ Is it a technical term?
    │   ├─→ YES: Check Translation-Glossary.md
    │   │         → Keep in English
    │   └─→ NO: Continue
    │
    ├─→ Is it code?
    │   ├─→ YES: Copy exactly, no translation
    │   └─→ NO: Continue
    │
    ├─→ Is it a file reference?
    │   ├─→ YES: Check File-Mapping-Bilingual.md
    │   │         → Use Vietnamese filename
    │   └─→ NO: Continue
    │
    └─→ Regular content
        → Translate to natural Vietnamese
        → Use bilingual approach for concepts
```

### File Update Checklist

When updating a Vietnamese translation file:

- [ ] Read full English section
- [ ] Check Translation-Glossary.md for terms
- [ ] Translate following Style Guide
- [ ] Update cross-references
- [ ] Update Table of Contents
- [ ] Run automated checks
- [ ] Self-review for naturalness
- [ ] Test all internal links
- [ ] Commit with clear message
- [ ] Update support docs if needed

---

## Appendix B: Statistics & Metrics

### Current Translation Status (as of Oct 31, 2025)

**Files**:
- Total Documentation: 28 files
- English Files: 26 unique
- Vietnamese Files: 26 translations
- Foundation Files: 2 (bilingual)

**Lines**:
- Total Lines Translated: ~15,000+
- Average File Length: ~580 lines
- Largest File: Phan_Tich_Project.md (2,016 lines)

**Quality**:
- Overall Score: 95/100
- Terminology Consistency: 100%
- Technical Accuracy: 100%
- Completeness: 100%

**Maintenance**:
- Last Full Update: 31 Tháng 10, 2025
- Translation Lag: 0 days
- Known Issues: 0

---

## Appendix C: Contact & Contribution

### Maintainers

**Translation Team**:
- Lead: [Name]
- Contributors: [Names]

**Contribution Guidelines**:
1. Read Translation-Style-Guide.md
2. Follow workflow in Section 3
3. Test changes thoroughly
4. Submit clear pull requests
5. Respond to review feedback

**Pull Request Template**:
```markdown
## Translation Update

**English File**: [filename]
**Vietnamese File**: [filename]

### Changes Made:
- [ ] Translated new Section X
- [ ] Updated cross-references
- [ ] Added N new terms to glossary

### Quality Checks:
- [ ] All automated checks passed
- [ ] Cross-references tested
- [ ] Code blocks unchanged
- [ ] Natural Vietnamese flow

### Additional Notes:
[Any special considerations]
```

---

## Summary / Tóm Tắt

**This guide provides**:
✅ Translation philosophy and principles
✅ Step-by-step update workflow
✅ Quality standards and checks
✅ Common scenarios and solutions
✅ Best practices and troubleshooting

**Key Takeaways**:
1. **Consistency is critical**: Use Translation-Glossary.md and Style Guide
2. **Quality over speed**: Maintain 90+ quality score
3. **Test everything**: Cross-references, links, formatting
4. **Document changes**: Clear git commits, updated support docs
5. **Natural Vietnamese**: Prioritize readability and clarity

**Next Steps**:
- Bookmark this guide for reference
- Review Translation-Style-Guide.md
- Set up monthly maintenance routine
- Join translation community

---

**End of Translation Maintenance Guide**

<p align="center">
<strong>Lawn Defense: Monsters Out</strong><br>
Translation Maintenance Guide<br>
Hướng Dẫn Bảo Trì Bản Dịch
</p>

**Version**: 1.0
**Maintained By**: Translation Team
**Last Updated**: 31 Tháng 10, 2025
