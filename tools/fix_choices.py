#!/usr/bin/env python3
"""누락된 선택지 번역 수정"""
import csv
import os

CSV_PATH = os.path.join(os.path.dirname(__file__), "strings.csv")

# key -> correct korean translation
FIXES = {
    # BaseSlynthMayor
    "conv:BaseSlynthMayor/choice:?#1": "여행 중에 새 고향을 찾는 슬린스라는 종족을 만났습니다.",
    "conv:BaseSlynthMayor/choice:?#2": "슬린스에 대해서...",
    "conv:BaseSlynthMayor/choice:?#3": "슬린스가 도착한 것 같은데, 잘 지내고 있습니까?",
    "conv:BaseSlynthMayor/choice:?#4": "일주일이 지났군요. 슬린스는 정착했습니까?",
    # MechanimistLibrarian (already correct, no change needed)
    # Argyve
    "conv:Argyve/choice:?#1": "여기 장신구입니다.",
    "conv:Argyve/choice:?#2": "와이어를 받으십시오.",
    "conv:Argyve/choice:?#3": "다시 오겠습니다.",
    "conv:Argyve/choice:?#4": "안녕히, Argyve.",
    # Otho
    "conv:Otho/choice:?#1": "감사합니다. 다시 오겠습니다.",
    "conv:Otho/choice:?#2": "감사합니다. 다시 오겠습니다.",
    "conv:Otho/choice:?#3": "[Otho에게 디스크를 건넨다]",
    "conv:Otho/choice:?#4": "살고 마시게.",
    "conv:Otho/choice:?#5": "저와 이야기하고 싶다고요? Barathrum 본인이요?",
    "conv:Otho/choice:?#6": "저와 이야기하고 싶다고요? Barathrum 본인이요?",
    "conv:Otho/choice:?#7": "저와 이야기하고 싶다고요? Barathrum 본인이요?",
    # Barathrum
    "conv:Barathrum/choice:?#1": "경고를 명심하겠습니다. 이만 가보겠습니다, Barathrum.",
    # PaxKlanq
    "conv:PaxKlanq/choice:?#1": "좋아요. 마음대로 하세요.",
    "conv:PaxKlanq/choice:?#2": "윽.",
    # PaxKlanq2
    "conv:PaxKlanq2/choice:?#1": "Creature를 주조하고 촉매화할 준비가 되었습니다.",
    "conv:PaxKlanq2/choice:?#2": "네.",
    "conv:PaxKlanq2/choice:?#3": "아직이요.",
    # Indrix
    "conv:Indrix/choice:?#1": "아니오, 워든. 이것은 내 것이오.",
    # Asphodel
    "conv:Asphodel/choice:?#1": "그렇다면 평의회를 소집하시오. 응하겠소.",
    # Neelahind
    "conv:Neelahind/choice:?#1": "다음은 네 차례다.",
    # Eskhind
    "conv:Eskhind/choice:?#1": "내 시간은 충분히 낭비했어. 죽을 준비나 해.",
    # Thah
    "conv:Thah/choice:?#1": "Argent Fathers가 당신을 지켜보시길.",
    "conv:Thah/choice:?#2": "지식을 찾으시길.",
    "conv:Thah/choice:?#3": "Oboroqoru가 자비를 베푸시길.",
    "conv:Thah/choice:?#4": "Eaters가 당신을 보호하시길.",
    "conv:Thah/choice:?#5": "Kasaphescence의 빛 속에서 영원히 안식하시기를.",
    "conv:Thah/choice:?#6": "바람이 당신 편이 되길.",
    "conv:Thah/choice:?#7": "기쁘게 울려 퍼지길.",
    "conv:Thah/choice:?#8": "운명이 당신에게 호의를 보이길.",
    # BaseConversation AskForWork
    "conv:BaseConversation/choice:AskForWork#1": "할 일이 있으십니까?",
    "conv:BaseConversation/choice:AskForWork#2": "일손이 필요하시다면 제가 도와드리겠습니다.",
    "conv:BaseConversation/choice:AskForWork#3": "이 근처에 할 만한 일이 있습니까?",
    # BaseConversation AskName
    "conv:BaseConversation/choice:AskName#1": "이름이 무엇입니까, =pronouns.formalAddressTerm=?",
    "conv:BaseConversation/choice:AskName#2": "뭐라고 불러 드리면 될까요, =pronouns.formalAddressTerm=?",
    "conv:BaseConversation/choice:AskName#3": "어떻게 불리시는지요, =pronouns.formalAddressTerm=?",
    "conv:BaseConversation/choice:AskName#4": "저는 =name=입니다, =pronouns.formalAddressTerm=. 이름이 무엇입니까?",
    "conv:BaseConversation/choice:AskName#5": "저는 =name=입니다, =pronouns.formalAddressTerm=. 뭐라고 불러 드리면 될까요?",
    # BaseConversation misc
    "conv:BaseConversation/choice:?#1": "감사합니다, =pronouns.formalAddressTerm=.",
    "conv:BaseConversation/choice:?#2": "만나 뵙게 되어 반갑습니다, =pronouns.formalAddressTerm=.",
    # ChavvahPrime
    "conv:ChavvahPrime/choice:?#1": "여러분 모두에게 감사드립니다.",
    # ChavvahFrontChime
    "conv:ChavvahFrontChime/choice:?#1": "[차임하는 바위를 만져 Chavvah에 동조한다.]",
    # Resheph
    "conv:Resheph/choice:?#1": "동의합니다. 계약이 성립되었습니다.",
    "conv:Resheph/choice:?#2": "그렇다면 동의합니다. 계약이 성립되었습니다.",
    "conv:Resheph/choice:?#3": "그렇다면 동의합니다. 계약이 성립되었습니다.",
    "conv:Resheph/choice:?#4": "이만 가보겠습니다.",
    "conv:Resheph/choice:?#5": "그들이 오기 전에 누스피어를 개간해야 합니다.",
    "conv:Resheph/choice:?#6": "저와 새로운 계약을 맺어 주십시오.",
    "conv:Resheph/choice:?#7": "동의합니다. 계약이 성립되었습니다.",
    "conv:Resheph/choice:?#8": "그렇다면 동의합니다. 계약이 성립되었습니다.",
    "conv:Resheph/choice:?#9": "그렇다면 동의합니다. 계약이 성립되었습니다.",
    "conv:Resheph/choice:?#10": "그들이 오기 전에 누스피어를 개간해야 합니다.",
    "conv:Resheph/choice:?#11": "옛 주역들은 사라졌습니다. 새로이 계약을 맺읍시다.",
}

def main():
    rows = []
    with open(CSV_PATH, "r", encoding="utf-8") as f:
        reader = csv.reader(f)
        for row in reader:
            rows.append(row)

    count = 0
    for row in rows:
        if len(row) >= 5 and row[1] in FIXES:
            old = row[4]
            row[4] = FIXES[row[1]]
            if old != row[4]:
                count += 1
                print(f"  Fixed {row[1]}: '{old[:30]}...' → '{row[4][:30]}...'")

    with open(CSV_PATH, "w", encoding="utf-8", newline="") as f:
        writer = csv.writer(f)
        for row in rows:
            writer.writerow(row)

    print(f"\nTotal: {count} choice translations fixed")

if __name__ == "__main__":
    main()
