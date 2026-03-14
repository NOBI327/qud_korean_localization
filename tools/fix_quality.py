#!/usr/bin/env python3
"""품질 검수 결과 일괄 수정 스크립트"""
import csv
import sys
import os

CSV_PATH = os.path.join(os.path.dirname(__file__), "strings.csv")

def load_csv(path):
    rows = []
    with open(path, "r", encoding="utf-8") as f:
        reader = csv.reader(f)
        for row in reader:
            rows.append(row)
    return rows

def save_csv(path, rows):
    with open(path, "w", encoding="utf-8", newline="") as f:
        writer = csv.writer(f)
        for row in rows:
            writer.writerow(row)

def fix_conversations(rows):
    """Fix 17 NPC Start nodes with wrong content"""
    fixes = {
        "conv:Dardi/node:Start": "누구야? 내 작업 공간에서 나가!",
        "conv:Q Girl/node:Start": "케찰! 당신은 누구예요, 낯선 분?",
        "conv:Jacobo/node:Start": "누구시오? 뭐라? 제발, 방해하지 마시오.",
        "conv:Hortensa/node:Start": "=player.OffspringTerm=, 자네는 누구인가?",
        "conv:Sparafucile/node:Start": "{{emote|*Sparafucile이 한쪽 눈썹을 치켜올리며, 경계하는 눈빛으로 당신을 바라본다.*}}",
        "conv:Eskhind/node:Start": "거기 서시오! 누구시오? 무엇을 원하시오?",
        "conv:Meyehind/node:Start": "Bey Lah는 사라졌어요. 당신이 만족하길 바라요.",
        "conv:Liihart/node:Start": "안녕, =name=. 살고 마시게.",
        "conv:Isahind/node:Start": "별로 이야기하고 싶지 않아요. 미안해요.",
        "conv:Kesehind/node:Start": "왜 나한테 말 거는 거예요?",
        "conv:Angohind/node:Start": "Kendren! 잠깐 시간 좀 내줄 수 있어요? 당신이 =name=, 맞죠? 여기 마을에서 수수께끼를 풀려고 하는 거죠?",
        "conv:Indrix/node:Start": "건방진 돼지 같으니! 감히 그 어둠의 부적을 내 앞에서 지니고 다니겠다는 것이오?",
        "conv:Thah/node:Start": "아, 낯선 분이시군요. 수경재배실에 오신 것을 환영합니다. 저는 Thah입니다.",
        "conv:JoppaFarmerConvert/node:SpeakNoMore": "더 이상 우리 사이에 나눌 말은 없네, 방랑자여.",
    }
    count = 0
    for row in rows:
        if len(row) >= 5 and row[1] in fixes:
            old = row[4]
            row[4] = fixes[row[1]]
            count += 1
            print(f"  [Conv] {row[1]}: '{old[:30]}...' → '{row[4][:30]}...'")
    return count

def fix_conversations_aloysius(rows):
    """Fix Aloysius separately (has template variables)"""
    count = 0
    for row in rows:
        if len(row) >= 5 and row[1] == "conv:Aloysius/node:Start":
            row[4] = "협정을 제안하겠다, =factionaddress:Barathrumites=: 당장 =player.reflexive=를 내 눈앞에서 치워라. 그러면 Ereshkigal에게 너희를 원자 단위로 분해해 달라고 요청하는 일은 삼가도록 하마."
            count += 1
            print(f"  [Conv] Aloysius fixed")
    return count

def fix_conversations_nacham(rows):
    """Fix Nacham separately"""
    count = 0
    for row in rows:
        if len(row) >= 5 and row[1] == "conv:Nacham/node:Start":
            row[4] = "잘 배우시오, =factionaddress:Mopango=. 그대가 그대의 과업에 성공하는 모습을 진심으로 보고 싶소."
            count += 1
            print(f"  [Conv] Nacham fixed")
    return count

def fix_conversations_slynth(rows):
    """Fix BaseSlynthMayor choice"""
    count = 0
    for row in rows:
        if len(row) >= 5 and row[1] == "conv:BaseSlynthMayor/choice:?":
            if "일주일이 지났군요" in row[4] or "slynth" in row[4]:
                row[4] = "감사합니다, 벗이여."
                count += 1
                print(f"  [Conv] BaseSlynthMayor choice fixed")
    return count

def fix_quest_steps(rows):
    """Fix 해라체 → 해요체 in quest step text"""
    replacements = [
        ("찾아라.", "찾으세요."),
        ("돌려주라.", "돌려주세요."),
        ("돌아가라.", "돌아가세요."),
        ("전달하라.", "전달하세요."),
        ("여행하라.", "여행하세요."),
        ("활성화하라.", "활성화하세요."),
        ("얻어라.", "얻으세요."),
        ("회수하라.", "회수하세요."),
        ("물리치라.", "물리치세요."),
        ("말하라.", "말하세요."),
        ("해독하라.", "해독하세요."),
        ("제공하라.", "제공하세요."),
        ("제거하라.", "제거하세요."),
        ("수집하라.", "수집하세요."),
        ("조사하라.", "조사하세요."),
        ("따라가라.", "따라가세요."),
        ("확인하라.", "확인하세요."),
        ("가져가라.", "가져가세요."),
        ("대화하라.", "대화하세요."),
        ("보고하라.", "보고하세요."),
        ("만나라.", "만나세요."),
        ("도달하라.", "도달하세요."),
        ("탐험하라.", "탐험하세요."),
        ("파괴하라.", "파괴하세요."),
        ("처치하라.", "처치하세요."),
        ("구하라.", "구하세요."),
        ("가져와라.", "가져오세요."),
        ("건네라.", "건네세요."),
        ("찾아가라.", "찾아가세요."),
        ("입수하라.", "입수하세요."),
        ("모아라.", "모으세요."),
        ("도착하라.", "도착하세요."),
        ("완료하라.", "완료하세요."),
        ("구출하라.", "구출하세요."),
        ("생존하라.", "생존하세요."),
        ("방문하라.", "방문하세요."),
        ("획득하라.", "획득하세요."),
        ("해방시켜라.", "해방시키세요."),
        ("격파하라.", "격파하세요."),
        ("설치하라.", "설치하세요."),
        ("가동하라.", "가동하세요."),
    ]
    count = 0
    for row in rows:
        if len(row) >= 5 and row[0] == "Quests.xml":
            for old, new in replacements:
                if old in row[4]:
                    row[4] = row[4].replace(old, new)
                    count += 1
    print(f"  [Quest] {count} 해라체→해요체 replacements")
    return count

def fix_options_broken(rows):
    """Fix completely broken Options.xml translations"""
    fixes = {}
    # Find by searching content
    for i, row in enumerate(rows):
        if len(row) < 5 or row[0] != "Options.xml":
            continue
        k = row[4]
        # Broken translations
        if "배경음 획득을 화면합니다" in k:
            row[4] = "도전 과제 비활성화"
            fixes["DisableAchievements"] = True
        elif "화면 꽉 채우기로 설정하십시오" in k:
            row[4] = "전체 화면 왜곡 효과 비활성화"
            fixes["FullscreenWarp"] = True
        elif "희망 영역: 새벽별" in k:
            row[4] = "위시 지원을 위한 구역 이름 사전 생성"
            fixes["WishRegion"] = True
        elif "재색깔 수 있도록" in k:
            row[4] = "돌연변이에 의한 캐릭터 글리프 색상 변경 허용"
            fixes["MutationColor"] = True
        elif "배경음으로 유지하세요" in k:
            row[4] = "백그라운드에서 Caves of Qud 활성 유지"
            fixes["MusicBg"] = True
        elif "인접한 적들을 자동 탐색 중에 무시합니다" in k:
            row[4] = "자동 탐색 중 인접한 무시된 적 공격"
            fixes["AutoexploreAttack"] = True
        elif "자동 중단 지시사항을 깜박이는 빨간 상자로 대체합니다" in k:
            row[4] = "깜박이는 빨간 상자 대신 텍스트 자동행동 중단 표시 사용"
            fixes["TextAutowalk"] = True
        # Typos
        elif "사이드바르" in k:
            row[4] = row[4].replace("사이드바르", "사이드바")
            fixes["sidebarTypo"] = True
        elif "기프를 건강" in k:
            row[4] = "건강 상태에 따른 캐릭터 글리프 색상 변경"
            fixes["glyphTypo"] = True
        elif "재불리기" in k:
            row[4] = row[4].replace("재불리기", "다시 불러오기")
            fixes["reloadTypo"] = True
    print(f"  [Options] Fixed {len(fixes)} broken/typo entries: {list(fixes.keys())}")
    return len(fixes)

def fix_options_verbose(rows):
    """Convert sentence-form Options labels to concise noun phrases"""
    count = 0
    for row in rows:
        if len(row) < 5 or row[0] != "Options.xml":
            continue
        k = row[4]
        # Remove trailing sentence endings and convert to noun phrases
        # Pattern: ends with 하세요. or 하십시오. or 합니다.
        if k.endswith("하세요.") or k.endswith("하세요"):
            # Try to convert: "X를 Y하세요." → "X Y"
            k = k.rstrip(".")
            if k.endswith("하세요"):
                k = k[:-3]  # remove 하세요
                # Clean up 를/을 before the verb
                count += 1
                row[4] = k.rstrip()
        elif k.endswith("하십시오.") or k.endswith("하십시오"):
            k = k.rstrip(".")
            if k.endswith("하십시오"):
                k = k[:-4]  # remove 하십시오
                count += 1
                row[4] = k.rstrip()
    print(f"  [Options] Converted {count} sentence-form labels to concise form")
    return count

def fix_objectblueprints(rows):
    """Fix ObjectBlueprints critical issues"""
    count = 0
    for row in rows:
        if len(row) < 5:
            continue
        # Hindi character
        if "विशाल하고" in row[4]:
            row[4] = row[4].replace("विशाल하고", "광대하고")
            count += 1
            print(f"  [OBJ] Fixed Hindi character in {row[1]}")
        # Partial translation goatfolk
        if row[4] == "염소folk qlippoth":
            row[4] = "염소인 클리포트"
            count += 1
            print(f"  [OBJ] Fixed partial translation: 염소folk → 염소인 클리포트")
        # snapjaw brute
        if row[4] == "스냅조 난봉꾼":
            row[4] = "스냅조 야만인"
            count += 1
            print(f"  [OBJ] Fixed mistranslation: 난봉꾼 → 야만인")
        # Scorpiock spelling
        if "스콜피오크" in row[4]:
            row[4] = row[4].replace("스콜피오크", "스코르피오크")
            count += 1
        # Sleetbeard spelling
        if "슬릿비어드" in row[4]:
            row[4] = row[4].replace("슬릿비어드", "슬리트비어드")
            count += 1
    print(f"  [OBJ] {count} ObjectBlueprints fixes")
    return count

def fix_description_tone(rows):
    """Fix Description.Short ending in 평서체 → 합쇼체 for Creatures/Walls"""
    fixes = {
        "낳는다.": "낳습니다.",
        "휘둘린다.": "휘둘립니다.",
        "가득하다.": "가득합니다.",
        "다듬어져 있다.": "다듬어져 있습니다.",
    }
    count = 0
    for row in rows:
        if len(row) < 5:
            continue
        if row[0].startswith("ObjectBlueprints/") and row[2] == "Description.Short":
            for old, new in fixes.items():
                if row[4].endswith(old):
                    row[4] = row[4][:-len(old)] + new
                    count += 1
    print(f"  [OBJ] {count} Description tone fixes")
    return count

def fix_skills_placeholder(rows):
    """Fix Skills.xml Make Camp placeholder"""
    count = 0
    for row in rows:
        if len(row) >= 5 and row[0] == "Skills.xml":
            if "테스트 Start a campfire" in row[4]:
                row[4] = "모닥불을 피워 요리와 식품 보존을 할 수 있습니다."
                count += 1
                print(f"  [Skills] Fixed Make Camp placeholder")
    return count

def fix_manual_glossary(rows):
    """Fix Manual.xml terminology"""
    count = 0
    for row in rows:
        if len(row) >= 5 and row[0] == "Manual.xml":
            if "물리적 돌연변이" in row[4]:
                row[4] = row[4].replace("물리적 돌연변이", "신체 변이")
                count += 1
            if "정신적 돌연변이" in row[4]:
                row[4] = row[4].replace("정신적 돌연변이", "정신 변이")
                count += 1
    print(f"  [Manual] {count} glossary fixes")
    return count

def fix_books_placeholder(rows):
    """Fix TornGraphPaper title placeholder"""
    count = 0
    for row in rows:
        if len(row) >= 5 and row[0] == "Books.xml" and "TornGraphPaper" in row[1] and row[2] == "Title":
            if row[4] == "{{W|제목}}":
                row[4] = "{{W|찢어진 모눈종이 한 장}}"
                count += 1
                print(f"  [Books] Fixed TornGraphPaper title")
    return count

def main():
    print("Loading CSV...")
    rows = load_csv(CSV_PATH)
    print(f"Loaded {len(rows)} rows")

    total = 0
    print("\n=== Conversations.xml fixes ===")
    total += fix_conversations(rows)
    total += fix_conversations_aloysius(rows)
    total += fix_conversations_nacham(rows)
    total += fix_conversations_slynth(rows)

    print("\n=== Quest steps fixes ===")
    total += fix_quest_steps(rows)

    print("\n=== Options.xml fixes ===")
    total += fix_options_broken(rows)
    total += fix_options_verbose(rows)

    print("\n=== ObjectBlueprints fixes ===")
    total += fix_objectblueprints(rows)
    total += fix_description_tone(rows)

    print("\n=== Skills.xml fix ===")
    total += fix_skills_placeholder(rows)

    print("\n=== Manual.xml fix ===")
    total += fix_manual_glossary(rows)

    print("\n=== Books.xml fix ===")
    total += fix_books_placeholder(rows)

    print(f"\n=== Total: {total} fixes applied ===")
    print("Saving CSV...")
    save_csv(CSV_PATH, rows)
    print("Done!")

if __name__ == "__main__":
    main()
