#!/usr/bin/env python3
"""인라인 선택지 중 고빈도 반복 텍스트 일괄 번역"""
import csv
import os

CSV_PATH = os.path.join(os.path.dirname(__file__), "strings.csv")

# 고빈도 반복 텍스트 번역 사전
COMMON = {
    "Live and drink.": "살고 마시게.",
    "Live and drink, watcher.": "살고 마시게, 감시자여.",
    "Who are you?": "당신은 누구시오?",
    "I must think on this further.": "좀 더 생각해 봐야겠습니다.",
    "I am =name=. Who are you?": "저는 =name=입니다. 당신은 누구시오?",
    "I'd like to ask about something else.": "다른 것을 여쭤보고 싶습니다.",
    "What can you tell me about Grit Gate?": "그릿 게이트에 대해 알려주실 수 있습니까?",
    "What is this place?": "이곳은 어디입니까?",
    "Tell me about yourself.": "자기소개를 부탁드립니다.",
    "Let's trade.": "거래합시다.",
    "Farewell.": "안녕히.",
    "Thank you.": "감사합니다.",
    "Yes.": "네.",
    "No.": "아니오.",
    "Goodbye.": "안녕히 가시오.",
    "I'll be going now.": "이만 가보겠습니다.",
    "What do you have to trade?": "거래할 물건이 있습니까?",
    "Tell me about the area.": "이 지역에 대해 알려주십시오.",
    "I have more questions.": "질문이 더 있습니다.",
    "Never mind.": "아무것도 아닙니다.",
    "I'll think about it.": "생각해 보겠습니다.",
    "What else can you tell me?": "다른 것은요?",
    "Go on.": "계속하시오.",
    "Continue.": "계속.",
    "I see.": "알겠습니다.",
    "Interesting.": "흥미롭군요.",
    "Tell me more.": "더 알려주십시오.",
    "What happened?": "무슨 일이 있었습니까?",
    "What do you mean?": "무슨 뜻입니까?",
    "I understand.": "알겠습니다.",
    "Where?": "어디에서요?",
    "When?": "언제요?",
    "Why?": "왜입니까?",
    "How?": "어떻게요?",
    "Who?": "누구요?",
    "I don't understand.": "이해할 수 없습니다.",
    "I accept.": "수락하겠습니다.",
    "I refuse.": "거절하겠습니다.",
    "What do you want?": "무엇을 원하시오?",
    "I'm ready.": "준비되었습니다.",
    "Not yet.": "아직이요.",
    "Barathrum.": "Barathrum.",
    "Otho.": "Otho.",
    "Jacobo.": "Jacobo.",
    "Sparafucile.": "Sparafucile.",
    "Q Girl.": "Q Girl.",
    "Mafeo.": "Mafeo.",
    "Iseppa.": "Iseppa.",
    "Neek.": "Neek.",
    "Dardi.": "Dardi.",
    "Shem -1.": "Shem -1.",
    "Aloysius.": "Aloysius.",
    "Ereshkigal.": "Ereshkigal.",
    "What is a Warden?": "수호자란 무엇입니까?",
}

def main():
    rows = []
    with open(CSV_PATH, "r", encoding="utf-8") as f:
        reader = csv.reader(f)
        for row in reader:
            rows.append(row)

    count = 0
    for row in rows:
        if len(row) >= 6 and row[5] == "todo" and row[2] == "choice_inline":
            original = row[3]
            if original in COMMON:
                row[4] = COMMON[original]
                row[5] = "done"
                count += 1

    with open(CSV_PATH, "w", encoding="utf-8", newline="") as f:
        writer = csv.writer(f)
        for row in rows:
            writer.writerow(row)

    print(f"Translated {count} common inline choices")

    # Count remaining
    remaining = sum(1 for r in rows if len(r) >= 6 and r[5] == "todo")
    print(f"Remaining untranslated: {remaining}")

if __name__ == "__main__":
    main()
