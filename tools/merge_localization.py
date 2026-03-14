"""
localization_files/의 번역을 strings.csv에 병합하는 스크립트.
Quests.xml과 Books.xml에서 한국어 번역을 추출하여 strings.csv의 해당 항목에 반영.
퀘스트 step은 이미 한국어로 Name이 바뀌어 있으므로 위치 기반 매칭 사용.
"""

import csv
import re
import xml.etree.ElementTree as ET
from pathlib import Path

PROJECT_ROOT = Path(__file__).resolve().parent.parent
CSV_PATH = PROJECT_ROOT / "tools" / "strings.csv"
LOC_DIR = PROJECT_ROOT / "localization_files"
GAME_BASE_DIR = Path(r"C:\Program Files (x86)\Steam\steamapps\common\Caves of Qud\CoQ_Data\StreamingAssets\Base")
FIELDNAMES = ["file", "key", "field", "original", "korean", "status"]

RE_KOREAN = re.compile(r"[\uac00-\ud7a3]")


def safe_parse_xml(filepath: Path) -> ET.ElementTree:
    with open(filepath, "r", encoding="utf-8") as f:
        content = f.read()
    def replace_invalid(m):
        code = int(m.group(1), 16)
        if code in (0x9, 0xA, 0xD) or code >= 0x20:
            return m.group(0)
        return f"[CHR:0x{code:02X}]"
    content = re.sub(r"&#x([0-9a-fA-F]+);", replace_invalid, content)
    def replace_invalid_dec(m):
        code = int(m.group(1))
        if code in (0x9, 0xA, 0xD) or code >= 0x20:
            return m.group(0)
        return f"[CHR:0x{code:02X}]"
    content = re.sub(r"&#(\d+);", replace_invalid_dec, content)
    return ET.ElementTree(ET.fromstring(content))


def extract_quests_positional(filepath: Path) -> list[dict]:
    """퀘스트를 위치 기반으로 추출: (quest_name, step_index, field) -> value"""
    results = []
    tree = safe_parse_xml(filepath)
    for quest in tree.iter("quest"):
        qname = quest.get("Name", "?")
        for attr in ("Accomplishment", "Hagiograph", "Gospel"):
            val = quest.get(attr, "")
            if val.strip():
                results.append({"quest": qname, "step_idx": -1, "field": attr, "value": val, "step_name": None})
        for idx, step in enumerate(quest.findall("step")):
            sname = step.get("Name", "?")
            results.append({"quest": qname, "step_idx": idx, "field": "Name", "value": sname, "step_name": sname})
            for text_el in step.findall("text"):
                t = (text_el.text or "").strip()
                if t:
                    results.append({"quest": qname, "step_idx": idx, "field": "text", "value": t, "step_name": sname})
    return results


def merge_quests(loc_path: Path, orig_path: Path) -> dict[str, str]:
    """위치 기반으로 원본 step Name과 번역 step을 매칭"""
    translations = {}

    loc_data = extract_quests_positional(loc_path)
    orig_data = extract_quests_positional(orig_path)

    # 원본에서 (quest, step_idx) -> step_name 매핑 구축
    orig_step_names = {}
    for item in orig_data:
        if item["step_idx"] >= 0 and item["field"] == "Name":
            orig_step_names[(item["quest"], item["step_idx"])] = item["value"]

    # 번역 데이터에서 한국어가 있는 항목을 원본 키로 매핑
    for item in loc_data:
        val = item["value"]
        if not val or not RE_KOREAN.search(val):
            continue

        qname = item["quest"]
        if item["step_idx"] < 0:
            # quest-level attributes
            compound_key = f"Quests.xml|quest:{qname}|{item['field']}"
            translations[compound_key] = val
        else:
            # step-level: use original English step name for key
            orig_sname = orig_step_names.get((qname, item["step_idx"]))
            if orig_sname:
                compound_key = f"Quests.xml|quest:{qname}/step:{orig_sname}|{item['field']}"
                translations[compound_key] = val

    return translations


def extract_books_translations(filepath: Path) -> dict[str, str]:
    translations = {}
    tree = safe_parse_xml(filepath)
    for book in tree.iter("book"):
        book_id = book.get("ID", "?")
        title = book.get("Title", "")
        if title and RE_KOREAN.search(title):
            translations[f"Books.xml|book:{book_id}|Title"] = title
        for i, page in enumerate(book.findall("page")):
            t = (page.text or "").strip()
            if t and RE_KOREAN.search(t):
                translations[f"Books.xml|book:{book_id}/page:{i}|page"] = t
    return translations


def main():
    all_translations = {}

    # Quests: position-based merge
    quests_loc = LOC_DIR / "Quests.xml"
    quests_orig = GAME_BASE_DIR / "Quests.xml"
    if quests_loc.exists() and quests_orig.exists():
        qt = merge_quests(quests_loc, quests_orig)
        all_translations.update(qt)
        print(f"Quests.xml: {len(qt)}개 번역 발견")

    # Books: direct key match
    books_path = LOC_DIR / "Books.xml"
    if books_path.exists():
        bt = extract_books_translations(books_path)
        all_translations.update(bt)
        print(f"Books.xml: {len(bt)}개 번역 발견")

    if not all_translations:
        print("병합할 번역이 없습니다.")
        return

    # Load CSV
    rows = []
    with open(CSV_PATH, "r", encoding="utf-8", newline="") as f:
        reader = csv.DictReader(f)
        for row in reader:
            rows.append(row)

    # Build index
    csv_keys = {f"{r['file']}|{r['key']}|{r['field']}": i for i, r in enumerate(rows)}

    merged = 0
    skipped_already_done = 0
    not_found_keys = []

    for compound_key, korean in all_translations.items():
        if compound_key in csv_keys:
            idx = csv_keys[compound_key]
            if rows[idx]["status"] == "done" and rows[idx]["korean"].strip():
                skipped_already_done += 1
            else:
                rows[idx]["korean"] = korean
                rows[idx]["status"] = "done"
                merged += 1
        else:
            not_found_keys.append(compound_key)

    # Save
    with open(CSV_PATH, "w", encoding="utf-8", newline="") as f:
        writer = csv.DictWriter(f, fieldnames=FIELDNAMES)
        writer.writeheader()
        writer.writerows(rows)

    print(f"\n=== 병합 결과 ===")
    print(f"  새로 병합: {merged}")
    print(f"  이미 완료: {skipped_already_done}")
    print(f"  키 미매칭: {len(not_found_keys)}")
    if not_found_keys:
        for k in not_found_keys[:10]:
            print(f"    {k}")
        if len(not_found_keys) > 10:
            print(f"    ... 외 {len(not_found_keys) - 10}건")
    print(f"  CSV 저장: {CSV_PATH}")


if __name__ == "__main__":
    main()
