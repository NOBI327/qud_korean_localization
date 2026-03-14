"""
Caves of Qud 번역 문자열 추출/주입 도구

사용법:
  python extract_strings.py extract   # 원본에서 문자열 추출 → strings.csv
  python extract_strings.py inject    # strings.csv에서 번역된 문자열 → korean-test/ XML/JSON
  python extract_strings.py status    # 번역 진행률 출력
"""

import csv
import json
import os
import re
import sys
import xml.etree.ElementTree as ET
from collections import defaultdict
from pathlib import Path


def safe_parse_xml(filepath: Path) -> ET.ElementTree:
    """잘못된 XML 문자 참조(&#x0;~&#x1F; 등)를 이스케이프 처리하여 파싱"""
    with open(filepath, "r", encoding="utf-8") as f:
        content = f.read()
    # &#xN; (N < 0x20, except 0x9, 0xA, 0xD) — XML에서 유효하지 않은 문자 참조를 대체
    def replace_invalid_char_ref(m):
        code = int(m.group(1), 16)
        if code in (0x9, 0xA, 0xD) or code >= 0x20:
            return m.group(0)
        return f"[CHR:0x{code:02X}]"
    content = re.sub(r"&#x([0-9a-fA-F]+);", replace_invalid_char_ref, content)
    # decimal variant
    def replace_invalid_char_ref_dec(m):
        code = int(m.group(1))
        if code in (0x9, 0xA, 0xD) or code >= 0x20:
            return m.group(0)
        return f"[CHR:0x{code:02X}]"
    content = re.sub(r"&#(\d+);", replace_invalid_char_ref_dec, content)
    return ET.ElementTree(ET.fromstring(content))

# 경로 설정
PROJECT_ROOT = Path(__file__).resolve().parent.parent
ORIGINAL_DIR = PROJECT_ROOT / "original_files"
GAME_BASE_DIR = Path(r"C:\Program Files (x86)\Steam\steamapps\common\Caves of Qud\CoQ_Data\StreamingAssets\Base")
KOREAN_DIR = PROJECT_ROOT / "korean-test"
CSV_PATH = PROJECT_ROOT / "tools" / "strings.csv"

# CSV 컬럼
FIELDNAMES = ["file", "key", "field", "original", "korean", "status"]

# 한글 감지 정규식
RE_KOREAN = re.compile(r"[\uac00-\ud7a3]")


def has_korean(text: str) -> bool:
    return bool(RE_KOREAN.search(text))


def is_translatable(text: str) -> bool:
    """번역 대상인지 판별 (빈 문자열, 숫자만, 순수 변수/태그만 등 제외)"""
    if not text or not text.strip():
        return False
    stripped = text.strip()
    # 숫자만
    if re.fullmatch(r"[\d.,\-+%]+", stripped):
        return False
    # 단일 변수/참조만
    if re.fullmatch(r"=[\w.]+=$", stripped):
        return False
    # 파일 경로/에셋 참조
    if re.fullmatch(r"[\w/\\._\-]+\.(png|bmp|wav|ogg|prefab|bundle)", stripped, re.I):
        return False
    # 매우 짧은 기호/코드
    if len(stripped) <= 1:
        return False
    return True


# ─────────────────────────────────────────────
#  XML 추출 규칙 정의
# ─────────────────────────────────────────────

def extract_conversations(filepath: Path) -> list[dict]:
    """Conversations.xml / HiddenConversations.xml"""
    rows = []
    tree = safe_parse_xml(filepath)
    fname = filepath.name
    for conv in tree.iter("conversation"):
        conv_id = conv.get("ID", "?")
        for node_tag in ("start", "node"):
            for node in conv.iter(node_tag):
                node_id = node.get("ID", "?")
                for text_el in node.findall("text"):
                    t = (text_el.text or "").strip()
                    if is_translatable(t):
                        key = f"conv:{conv_id}/node:{node_id}"
                        # text 태그에 고유 속성이 있으면 키에 추가
                        extra = text_el.get("IfHavePart", "") or text_el.get("IfNotHavePart", "")
                        if extra:
                            key += f"/if:{extra}"
                        rows.append({"file": fname, "key": key, "field": "text", "original": t})
        choice_key_counter: dict[str, int] = {}
        for choice in conv.iter("choice"):
            choice_id = choice.get("ID", "?")
            # 1) <text> 자식 요소의 텍스트
            for text_el in choice.findall("text"):
                t = (text_el.text or "").strip()
                if is_translatable(t):
                    base_key = f"conv:{conv_id}/choice:{choice_id}"
                    extra = text_el.get("IfHavePart", "") or text_el.get("IfNotHavePart", "")
                    if extra:
                        base_key += f"/if:{extra}"
                    # 중복 키에 인덱스 추가
                    idx = choice_key_counter.get(base_key, 0)
                    choice_key_counter[base_key] = idx + 1
                    key = f"{base_key}#{idx}" if idx > 0 else base_key
                    rows.append({"file": fname, "key": key, "field": "text", "original": t})
            # 2) 인라인 텍스트 (<choice GotoID="x">텍스트</choice>)
            if choice.find("text") is None:
                t = (choice.text or "").strip()
                if is_translatable(t):
                    goto = choice.get("GotoID", "")
                    target = choice.get("Target", "")
                    ref = goto or target or choice_id
                    base_key = f"conv:{conv_id}/choiceinline:{ref}"
                    idx = choice_key_counter.get(base_key, 0)
                    choice_key_counter[base_key] = idx + 1
                    key = f"{base_key}#{idx}" if idx > 0 else base_key
                    rows.append({"file": fname, "key": key, "field": "choice_inline", "original": t})
        # part 내 텍스트 (WaterRitual 등 서술 텍스트)
        for part in conv.iter("part"):
            pt = (part.text or "").strip()
            if is_translatable(pt) and len(pt) > 30:  # 긴 서술 텍스트만
                part_name = part.get("Name", "?")
                parent = part
                # 부모 choice/node 찾기
                # ElementTree에서 부모 추적이 안 되므로 key를 part 기준으로
                key = f"conv:{conv_id}/part:{part_name}"
                rows.append({"file": fname, "key": key, "field": "part_text", "original": pt})
    return rows


def extract_books(filepath: Path) -> list[dict]:
    """Books.xml"""
    rows = []
    tree = safe_parse_xml(filepath)
    for book in tree.iter("book"):
        book_id = book.get("ID", "?")
        title = book.get("Title", "")
        if is_translatable(title):
            rows.append({"file": "Books.xml", "key": f"book:{book_id}", "field": "Title", "original": title})
        for i, page in enumerate(book.findall("page")):
            t = (page.text or "").strip()
            if is_translatable(t):
                rows.append({"file": "Books.xml", "key": f"book:{book_id}/page:{i}", "field": "page", "original": t})
    return rows


def extract_skills(filepath: Path) -> list[dict]:
    """Skills.xml"""
    rows = []
    tree = safe_parse_xml(filepath)
    for skill in tree.iter("skill"):
        sname = skill.get("Name", "?")
        desc = skill.get("Description", "")
        if is_translatable(desc):
            rows.append({"file": "Skills.xml", "key": f"skill:{sname}", "field": "Description", "original": desc})
        for power in skill.findall("power"):
            pname = power.get("Name", "?")
            pdesc = power.get("Description", "")
            if is_translatable(pdesc):
                rows.append({"file": "Skills.xml", "key": f"skill:{sname}/power:{pname}", "field": "Description", "original": pdesc})
    return rows


def extract_mutations(filepath: Path) -> list[dict]:
    """Mutations.xml / HiddenMutations.xml"""
    rows = []
    fname = filepath.name
    tree = safe_parse_xml(filepath)
    for cat in tree.iter("category"):
        cat_dn = cat.get("DisplayName", "")
        cat_name = cat.get("Name", "?")
        if is_translatable(cat_dn):
            rows.append({"file": fname, "key": f"mutcat:{cat_name}", "field": "DisplayName", "original": cat_dn})
        for mut in cat.findall("mutation"):
            mname = mut.get("Name", "?")
            bearer = mut.get("BearerDescription", "")
            if is_translatable(bearer):
                rows.append({"file": fname, "key": f"mut:{mname}", "field": "BearerDescription", "original": bearer})
            # description 자식 요소
            for desc_el in mut.findall("description"):
                dt = (desc_el.text or "").strip()
                if is_translatable(dt):
                    rows.append({"file": fname, "key": f"mut:{mname}", "field": "description", "original": dt})
            # leveltext 자식 요소
            for lt in mut.findall("leveltext"):
                ltt = (lt.text or "").strip()
                level = lt.get("Level", "?")
                if is_translatable(ltt):
                    rows.append({"file": fname, "key": f"mut:{mname}/level:{level}", "field": "leveltext", "original": ltt})
    return rows


def extract_abilities(filepath: Path) -> list[dict]:
    """ActivatedAbilities.xml"""
    rows = []
    tree = safe_parse_xml(filepath)
    for ab in tree.iter("ability"):
        cmd = ab.get("Command", "?")
        # description 자식 요소
        for desc_el in ab.findall("description"):
            dt = (desc_el.text or "").strip()
            if is_translatable(dt):
                rows.append({"file": "ActivatedAbilities.xml", "key": f"ability:{cmd}", "field": "description", "original": dt})
        # DisplayName이 있을 수도
        dn = ab.get("DisplayName", "")
        if is_translatable(dn):
            rows.append({"file": "ActivatedAbilities.xml", "key": f"ability:{cmd}", "field": "DisplayName", "original": dn})
    return rows


def extract_commands(filepath: Path) -> list[dict]:
    """Commands.xml"""
    rows = []
    tree = safe_parse_xml(filepath)
    for cmd in tree.iter("command"):
        cid = cmd.get("ID", "?")
        dt = cmd.get("DisplayText", "")
        if is_translatable(dt):
            rows.append({"file": "Commands.xml", "key": f"cmd:{cid}", "field": "DisplayText", "original": dt})
    return rows


def extract_options(filepath: Path) -> list[dict]:
    """Options.xml"""
    rows = []
    tree = safe_parse_xml(filepath)
    for opt in tree.iter("option"):
        oid = opt.get("ID", "?")
        dt = opt.get("DisplayText", "")
        if is_translatable(dt):
            rows.append({"file": "Options.xml", "key": f"opt:{oid}", "field": "DisplayText", "original": dt})
        # helptext 자식 요소
        for ht in opt.findall("helptext"):
            htt = (ht.text or "").strip()
            if is_translatable(htt):
                rows.append({"file": "Options.xml", "key": f"opt:{oid}", "field": "helptext", "original": htt})
    return rows


def extract_quests(filepath: Path) -> list[dict]:
    """Quests.xml"""
    rows = []
    tree = safe_parse_xml(filepath)
    for quest in tree.iter("quest"):
        qname = quest.get("Name", "?")
        for attr in ("Name", "Accomplishment", "Hagiograph", "Gospel"):
            val = quest.get(attr, "")
            if is_translatable(val):
                rows.append({"file": "Quests.xml", "key": f"quest:{qname}", "field": attr, "original": val})
        for step in quest.findall("step"):
            sname = step.get("Name", "?")
            # step Name
            if is_translatable(sname):
                rows.append({"file": "Quests.xml", "key": f"quest:{qname}/step:{sname}", "field": "Name", "original": sname})
            for text_el in step.findall("text"):
                t = (text_el.text or "").strip()
                if is_translatable(t):
                    rows.append({"file": "Quests.xml", "key": f"quest:{qname}/step:{sname}", "field": "text", "original": t})
    return rows


def extract_subtypes(filepath: Path) -> list[dict]:
    """Subtypes.xml"""
    rows = []
    tree = safe_parse_xml(filepath)
    for cls in tree.iter("class"):
        cid = cls.get("ID", "?")
        for attr in ("ChargenTitle", "SingularTitle"):
            val = cls.get(attr, "")
            if is_translatable(val):
                rows.append({"file": "Subtypes.xml", "key": f"class:{cid}", "field": attr, "original": val})
        for cat in cls.findall("category"):
            cat_dn = cat.get("DisplayName", "")
            cat_name = cat.get("Name", "?")
            if is_translatable(cat_dn):
                rows.append({"file": "Subtypes.xml", "key": f"class:{cid}/cat:{cat_name}", "field": "DisplayName", "original": cat_dn})
            for sub in cat.findall("subtype"):
                sub_name = sub.get("Name", "?")
                if is_translatable(sub_name):
                    rows.append({"file": "Subtypes.xml", "key": f"class:{cid}/subtype:{sub_name}", "field": "Name", "original": sub_name})
                # Tile, Description 등
                sub_desc = sub.get("Description", "")
                if is_translatable(sub_desc):
                    rows.append({"file": "Subtypes.xml", "key": f"class:{cid}/subtype:{sub_name}", "field": "Description", "original": sub_desc})
    return rows


def extract_objects(filepath: Path, rel_path: str) -> list[dict]:
    """ObjectBlueprints/*.xml (Items, Creatures, Foods, Furniture, Walls, WorldTerrain, ZoneTerrain, etc.)"""
    rows = []
    tree = safe_parse_xml(filepath)
    for obj in tree.iter("object"):
        obj_name = obj.get("Name", "?")
        for part in obj.findall("part"):
            pname = part.get("Name", "")
            if pname == "Render":
                dn = part.get("DisplayName", "")
                if is_translatable(dn):
                    rows.append({"file": rel_path, "key": f"obj:{obj_name}", "field": "DisplayName", "original": dn})
            elif pname == "Description":
                short = part.get("Short", "")
                if is_translatable(short):
                    rows.append({"file": rel_path, "key": f"obj:{obj_name}", "field": "Description.Short", "original": short})
            elif pname == "Food":
                msg = part.get("Message", "")
                if is_translatable(msg):
                    rows.append({"file": rel_path, "key": f"obj:{obj_name}", "field": "Food.Message", "original": msg})
        # xtagTextFragments (creatures)
        for xtag in obj.findall("xtagTextFragments"):
            for attr_name, attr_val in xtag.attrib.items():
                if is_translatable(attr_val):
                    rows.append({"file": rel_path, "key": f"obj:{obj_name}", "field": f"xtag.{attr_name}", "original": attr_val})
    return rows


def extract_naming(filepath: Path) -> list[dict]:
    """Naming.xml — 이름 조각 추출"""
    rows = []
    tree = safe_parse_xml(filepath)
    for style in tree.iter("namestyle"):
        style_name = style.get("Name", "?")
        for group_tag in ("prefixes", "infixes", "postfixes"):
            group = style.find(group_tag)
            if group is None:
                continue
            singular = group_tag.rstrip("es").rstrip("x") + "ix" if group_tag.endswith("ixes") else group_tag[:-2] if group_tag.endswith("es") else group_tag[:-1]
            # prefix, infix, postfix
            tag = group_tag.rstrip("es") if group_tag != "postfixes" else "postfix"
            if group_tag == "prefixes":
                tag = "prefix"
            elif group_tag == "infixes":
                tag = "infix"
            else:
                tag = "postfix"
            for item in group.findall(tag):
                name_val = item.get("Name", "")
                if is_translatable(name_val):
                    rows.append({"file": "Naming.xml", "key": f"namestyle:{style_name}/{group_tag}/{name_val}", "field": "Name", "original": name_val})
    # titles
    for titles in tree.iter("titles"):
        for title in titles.findall("title"):
            tname = title.get("Name", "")
            if is_translatable(tname):
                rows.append({"file": "Naming.xml", "key": f"title:{tname}", "field": "Name", "original": tname})
    return rows


def extract_factions(filepath: Path) -> list[dict]:
    """Factions.xml"""
    rows = []
    tree = safe_parse_xml(filepath)
    for faction in tree.iter("faction"):
        fname_val = faction.get("Name", "?")
        dn = faction.get("DisplayName", "")
        if is_translatable(dn):
            rows.append({"file": "Factions.xml", "key": f"faction:{fname_val}", "field": "DisplayName", "original": dn})
    return rows


def extract_genders(filepath: Path) -> list[dict]:
    """Genders.xml"""
    rows = []
    tree = safe_parse_xml(filepath)
    gender_attrs = [
        "Subjective", "Objective", "PossessiveAdjective", "SubstantivePossessive",
        "Reflexive", "PersonTerm", "ImmaturePersonTerm", "FormalAddressTerm",
        "OffspringTerm", "SiblingTerm", "ParentTerm"
    ]
    for gender in tree.iter("gender"):
        gname = gender.get("Name", "?")
        for attr in gender_attrs:
            val = gender.get(attr, "")
            if is_translatable(val):
                rows.append({"file": "Genders.xml", "key": f"gender:{gname}", "field": attr, "original": val})
    return rows


def extract_manual(filepath: Path) -> list[dict]:
    """Manual.xml"""
    rows = []
    tree = safe_parse_xml(filepath)
    for topic in tree.iter("topic"):
        tname = topic.get("name", "?")
        if is_translatable(tname):
            rows.append({"file": "Manual.xml", "key": f"topic:{tname}", "field": "name", "original": tname})
        t = (topic.text or "").strip()
        if is_translatable(t):
            rows.append({"file": "Manual.xml", "key": f"topic:{tname}", "field": "content", "original": t})
    return rows


def extract_genotypes(filepath: Path) -> list[dict]:
    """Genotypes.xml"""
    rows = []
    tree = safe_parse_xml(filepath)
    for gt in tree.iter("genotype"):
        gname = gt.get("Name", "?")
        for attr in ("Name", "DisplayName", "Description", "Tile"):
            val = gt.get(attr, "")
            if attr == "Tile":
                continue
            if is_translatable(val):
                rows.append({"file": "Genotypes.xml", "key": f"genotype:{gname}", "field": attr, "original": val})
    return rows


def extract_embark_modules(filepath: Path) -> list[dict]:
    """EmbarkModules.xml"""
    rows = []
    tree = safe_parse_xml(filepath)
    for mod in tree.iter("module"):
        mid = mod.get("ID", mod.get("Name", "?"))
        for attr in ("Title", "DisplayName", "Description", "InitialPrompt"):
            val = mod.get(attr, "")
            if is_translatable(val):
                rows.append({"file": "EmbarkModules.xml", "key": f"module:{mid}", "field": attr, "original": val})
        # 자식 텍스트
        for child in mod:
            if child.text and is_translatable(child.text.strip()):
                rows.append({"file": "EmbarkModules.xml", "key": f"module:{mid}/{child.tag}", "field": "text", "original": child.text.strip()})
    return rows


def extract_history_spice(filepath: Path) -> list[dict]:
    """HistorySpice.json — 모든 문자열 값 추출"""
    rows = []
    with open(filepath, "r", encoding="utf-8") as f:
        data = json.load(f)

    def walk_json(obj, path=""):
        if isinstance(obj, dict):
            for k, v in obj.items():
                walk_json(v, f"{path}.{k}" if path else k)
        elif isinstance(obj, list):
            for i, item in enumerate(obj):
                if isinstance(item, str):
                    if is_translatable(item):
                        rows.append({
                            "file": "HistorySpice.json",
                            "key": f"{path}[{i}]",
                            "field": "value",
                            "original": item,
                        })
                else:
                    walk_json(item, f"{path}[{i}]")
        # 단일 문자열 값 (루트가 아닌 깊은 곳)
        elif isinstance(obj, str) and is_translatable(obj):
            rows.append({
                "file": "HistorySpice.json",
                "key": path,
                "field": "value",
                "original": obj,
            })

    walk_json(data)
    return rows


# ─────────────────────────────────────────────
#  메인 추출 로직
# ─────────────────────────────────────────────

FILE_EXTRACTORS = {
    "Conversations.xml": extract_conversations,
    "HiddenConversations.xml": extract_conversations,
    "Books.xml": extract_books,
    "Skills.xml": extract_skills,
    "Mutations.xml": extract_mutations,
    "HiddenMutations.xml": extract_mutations,
    "ActivatedAbilities.xml": extract_abilities,
    "Commands.xml": extract_commands,
    "Options.xml": extract_options,
    "Quests.xml": extract_quests,
    "Subtypes.xml": extract_subtypes,
    "Naming.xml": extract_naming,
    "Factions.xml": extract_factions,
    "Genders.xml": extract_genders,
    "Manual.xml": extract_manual,
    "Genotypes.xml": extract_genotypes,
    "EmbarkModules.xml": extract_embark_modules,
    "HistorySpice.json": extract_history_spice,
}

OBJECT_BLUEPRINT_FILES = [
    "Creatures.xml", "Foods.xml", "Furniture.xml", "Items.xml",
    "Walls.xml", "WorldTerrain.xml", "ZoneTerrain.xml",
    "HiddenObjects.xml", "PhysicalPhenomena.xml", "Data.xml", "Widgets.xml",
]


def load_existing_korean(csv_path: Path) -> dict[str, str]:
    """기존 CSV에서 한국어 번역을 로드"""
    korean_map = {}
    if csv_path.exists():
        with open(csv_path, "r", encoding="utf-8", newline="") as f:
            reader = csv.DictReader(f)
            for row in reader:
                compound_key = f"{row['file']}|{row['key']}|{row['field']}"
                if row.get("korean", "").strip():
                    korean_map[compound_key] = row["korean"]
    return korean_map


def scan_korean_mod_for_translations() -> dict[str, str]:
    """korean-test/ 모드 파일에서 기존 번역을 스캔하여 매핑"""
    translations = {}

    # korean-test의 XML 파일들을 파싱하여 한국어 텍스트 추출
    # 각 파일 유형별로 원본과 비교하여 한국어로 번역된 부분 탐지
    for fname, extractor in FILE_EXTRACTORS.items():
        kr_path = KOREAN_DIR / fname
        if kr_path.exists():
            try:
                kr_rows = extractor(kr_path)
                for row in kr_rows:
                    if has_korean(row["original"]):
                        compound_key = f"{row['file']}|{row['key']}|{row['field']}"
                        translations[compound_key] = row["original"]
            except Exception:
                pass

    # ObjectBlueprints
    for obj_file in OBJECT_BLUEPRINT_FILES:
        kr_path = KOREAN_DIR / "ObjectBlueprints" / obj_file
        if kr_path.exists():
            rel = f"ObjectBlueprints/{obj_file}"
            try:
                kr_rows = extract_objects(kr_path, rel)
                for row in kr_rows:
                    if has_korean(row["original"]):
                        compound_key = f"{row['file']}|{row['key']}|{row['field']}"
                        translations[compound_key] = row["original"]
            except Exception:
                pass

    return translations


def do_extract():
    """원본 게임 파일에서 모든 번역 대상 문자열 추출"""
    all_rows = []

    # 기존 CSV의 한국어 번역 보존
    existing_korean = load_existing_korean(CSV_PATH)

    # korean-test에서 기존 번역 스캔
    mod_translations = scan_korean_mod_for_translations()

    # 일반 파일 추출
    for fname, extractor in FILE_EXTRACTORS.items():
        filepath = GAME_BASE_DIR / fname
        if filepath.exists():
            print(f"  추출 중: {fname}")
            rows = extractor(filepath)
            all_rows.extend(rows)
        else:
            print(f"  [건너뜀] {fname} — 파일 없음")

    # ObjectBlueprints
    for obj_file in OBJECT_BLUEPRINT_FILES:
        filepath = GAME_BASE_DIR / "ObjectBlueprints" / obj_file
        rel = f"ObjectBlueprints/{obj_file}"
        if filepath.exists():
            print(f"  추출 중: {rel}")
            rows = extract_objects(filepath, rel)
            all_rows.extend(rows)
        else:
            print(f"  [건너뜀] {rel} — 파일 없음")

    # 한국어 번역 매칭 및 상태 설정
    for row in all_rows:
        compound_key = f"{row['file']}|{row['key']}|{row['field']}"
        # 기존 CSV 우선
        if compound_key in existing_korean:
            row["korean"] = existing_korean[compound_key]
            row["status"] = "done"
        elif compound_key in mod_translations:
            row["korean"] = mod_translations[compound_key]
            row["status"] = "done"
        else:
            row["korean"] = ""
            row["status"] = "todo"

    # 중복 제거 (같은 key에 대해)
    seen = set()
    unique_rows = []
    for row in all_rows:
        compound_key = f"{row['file']}|{row['key']}|{row['field']}"
        if compound_key not in seen:
            seen.add(compound_key)
            unique_rows.append(row)

    # CSV 저장
    CSV_PATH.parent.mkdir(parents=True, exist_ok=True)
    with open(CSV_PATH, "w", encoding="utf-8", newline="") as f:
        writer = csv.DictWriter(f, fieldnames=FIELDNAMES)
        writer.writeheader()
        writer.writerows(unique_rows)

    print(f"\n총 {len(unique_rows)}개 문자열 추출 → {CSV_PATH}")
    done = sum(1 for r in unique_rows if r["status"] == "done")
    print(f"  번역 완료: {done}")
    print(f"  미번역:    {len(unique_rows) - done}")


# ─────────────────────────────────────────────
#  주입 로직
# ─────────────────────────────────────────────

def load_translations() -> dict[str, dict]:
    """CSV에서 번역 데이터 로드, file별로 그룹화"""
    groups = defaultdict(list)
    with open(CSV_PATH, "r", encoding="utf-8", newline="") as f:
        reader = csv.DictReader(f)
        for row in reader:
            if row.get("korean", "").strip():
                groups[row["file"]].append(row)
    return dict(groups)


def inject_conversations(filepath: Path, output_path: Path, translations: list[dict]):
    """대화 XML에 번역 주입"""
    tree = safe_parse_xml(filepath)
    tr_map = {}
    for t in translations:
        tr_map[f"{t['key']}|{t['field']}"] = t["korean"]

    for conv in tree.iter("conversation"):
        conv_id = conv.get("ID", "?")
        for node_tag in ("start", "node"):
            for node in conv.iter(node_tag):
                node_id = node.get("ID", "?")
                for text_el in node.findall("text"):
                    key = f"conv:{conv_id}/node:{node_id}"
                    extra = text_el.get("IfHavePart", "") or text_el.get("IfNotHavePart", "")
                    if extra:
                        key += f"/if:{extra}"
                    lookup = f"{key}|text"
                    if lookup in tr_map:
                        text_el.text = tr_map[lookup]
        inject_choice_counter: dict[str, int] = {}
        for choice in conv.iter("choice"):
            choice_id = choice.get("ID", "?")
            for text_el in choice.findall("text"):
                base_key = f"conv:{conv_id}/choice:{choice_id}"
                extra = text_el.get("IfHavePart", "") or text_el.get("IfNotHavePart", "")
                if extra:
                    base_key += f"/if:{extra}"
                idx = inject_choice_counter.get(base_key, 0)
                inject_choice_counter[base_key] = idx + 1
                key = f"{base_key}#{idx}" if idx > 0 else base_key
                lookup = f"{key}|text"
                if lookup in tr_map:
                    text_el.text = tr_map[lookup]
            # 인라인 선택지 텍스트 주입
            if choice.find("text") is None:
                t = (choice.text or "").strip()
                if t:
                    goto = choice.get("GotoID", "")
                    target = choice.get("Target", "")
                    ref = goto or target or choice_id
                    base_key = f"conv:{conv_id}/choiceinline:{ref}"
                    idx = inject_choice_counter.get(base_key, 0)
                    inject_choice_counter[base_key] = idx + 1
                    key = f"{base_key}#{idx}" if idx > 0 else base_key
                    lookup = f"{key}|choice_inline"
                    if lookup in tr_map:
                        choice.text = tr_map[lookup]
        for part in conv.iter("part"):
            part_name = part.get("Name", "?")
            lookup = f"conv:{conv_id}/part:{part_name}|part_text"
            if lookup in tr_map:
                part.text = tr_map[lookup]

    output_path.parent.mkdir(parents=True, exist_ok=True)
    tree.write(output_path, encoding="utf-8", xml_declaration=True)


def inject_books(filepath: Path, output_path: Path, translations: list[dict]):
    tree = safe_parse_xml(filepath)
    tr_map = {f"{t['key']}|{t['field']}": t["korean"] for t in translations}
    for book in tree.iter("book"):
        book_id = book.get("ID", "?")
        lookup = f"book:{book_id}|Title"
        if lookup in tr_map:
            book.set("Title", tr_map[lookup])
        for i, page in enumerate(book.findall("page")):
            lookup = f"book:{book_id}/page:{i}|page"
            if lookup in tr_map:
                page.text = tr_map[lookup]
    output_path.parent.mkdir(parents=True, exist_ok=True)
    tree.write(output_path, encoding="utf-8", xml_declaration=True)


def inject_skills(filepath: Path, output_path: Path, translations: list[dict]):
    tree = safe_parse_xml(filepath)
    tr_map = {f"{t['key']}|{t['field']}": t["korean"] for t in translations}
    for skill in tree.iter("skill"):
        sname = skill.get("Name", "?")
        lookup = f"skill:{sname}|Description"
        if lookup in tr_map:
            skill.set("Description", tr_map[lookup])
        for power in skill.findall("power"):
            pname = power.get("Name", "?")
            lookup = f"skill:{sname}/power:{pname}|Description"
            if lookup in tr_map:
                power.set("Description", tr_map[lookup])
    output_path.parent.mkdir(parents=True, exist_ok=True)
    tree.write(output_path, encoding="utf-8", xml_declaration=True)


def inject_mutations(filepath: Path, output_path: Path, translations: list[dict]):
    tree = safe_parse_xml(filepath)
    tr_map = {f"{t['key']}|{t['field']}": t["korean"] for t in translations}
    for cat in tree.iter("category"):
        cat_name = cat.get("Name", "?")
        lookup = f"mutcat:{cat_name}|DisplayName"
        if lookup in tr_map:
            cat.set("DisplayName", tr_map[lookup])
        for mut in cat.findall("mutation"):
            mname = mut.get("Name", "?")
            lookup = f"mut:{mname}|BearerDescription"
            if lookup in tr_map:
                mut.set("BearerDescription", tr_map[lookup])
            for desc_el in mut.findall("description"):
                lookup = f"mut:{mname}|description"
                if lookup in tr_map:
                    desc_el.text = tr_map[lookup]
            for lt in mut.findall("leveltext"):
                level = lt.get("Level", "?")
                lookup = f"mut:{mname}/level:{level}|leveltext"
                if lookup in tr_map:
                    lt.text = tr_map[lookup]
    output_path.parent.mkdir(parents=True, exist_ok=True)
    tree.write(output_path, encoding="utf-8", xml_declaration=True)


def inject_commands(filepath: Path, output_path: Path, translations: list[dict]):
    tree = safe_parse_xml(filepath)
    tr_map = {f"{t['key']}|{t['field']}": t["korean"] for t in translations}
    for cmd in tree.iter("command"):
        cid = cmd.get("ID", "?")
        lookup = f"cmd:{cid}|DisplayText"
        if lookup in tr_map:
            cmd.set("DisplayText", tr_map[lookup])
    output_path.parent.mkdir(parents=True, exist_ok=True)
    tree.write(output_path, encoding="utf-8", xml_declaration=True)


def inject_options(filepath: Path, output_path: Path, translations: list[dict]):
    tree = safe_parse_xml(filepath)
    tr_map = {f"{t['key']}|{t['field']}": t["korean"] for t in translations}
    for opt in tree.iter("option"):
        oid = opt.get("ID", "?")
        lookup = f"opt:{oid}|DisplayText"
        if lookup in tr_map:
            opt.set("DisplayText", tr_map[lookup])
        for ht in opt.findall("helptext"):
            lookup = f"opt:{oid}|helptext"
            if lookup in tr_map:
                ht.text = tr_map[lookup]
    output_path.parent.mkdir(parents=True, exist_ok=True)
    tree.write(output_path, encoding="utf-8", xml_declaration=True)


def inject_quests(filepath: Path, output_path: Path, translations: list[dict]):
    tree = safe_parse_xml(filepath)
    tr_map = {f"{t['key']}|{t['field']}": t["korean"] for t in translations}
    for quest in tree.iter("quest"):
        qname = quest.get("Name", "?")
        for attr in ("Name", "Accomplishment", "Hagiograph", "Gospel"):
            lookup = f"quest:{qname}|{attr}"
            if lookup in tr_map:
                quest.set(attr, tr_map[lookup])
        for step in quest.findall("step"):
            sname = step.get("Name", "?")
            lookup = f"quest:{qname}/step:{sname}|Name"
            if lookup in tr_map:
                step.set("Name", tr_map[lookup])
            for text_el in step.findall("text"):
                lookup = f"quest:{qname}/step:{sname}|text"
                if lookup in tr_map:
                    text_el.text = tr_map[lookup]
    output_path.parent.mkdir(parents=True, exist_ok=True)
    tree.write(output_path, encoding="utf-8", xml_declaration=True)


def inject_subtypes(filepath: Path, output_path: Path, translations: list[dict]):
    tree = safe_parse_xml(filepath)
    tr_map = {f"{t['key']}|{t['field']}": t["korean"] for t in translations}
    for cls in tree.iter("class"):
        cid = cls.get("ID", "?")
        for attr in ("ChargenTitle", "SingularTitle"):
            lookup = f"class:{cid}|{attr}"
            if lookup in tr_map:
                cls.set(attr, tr_map[lookup])
        for cat in cls.findall("category"):
            cat_name = cat.get("Name", "?")
            lookup = f"class:{cid}/cat:{cat_name}|DisplayName"
            if lookup in tr_map:
                cat.set("DisplayName", tr_map[lookup])
            for sub in cat.findall("subtype"):
                sub_name = sub.get("Name", "?")
                lookup = f"class:{cid}/subtype:{sub_name}|Name"
                if lookup in tr_map:
                    sub.set("Name", tr_map[lookup])
                lookup = f"class:{cid}/subtype:{sub_name}|Description"
                if lookup in tr_map:
                    sub.set("Description", tr_map[lookup])
    output_path.parent.mkdir(parents=True, exist_ok=True)
    tree.write(output_path, encoding="utf-8", xml_declaration=True)


def inject_objects(filepath: Path, output_path: Path, translations: list[dict]):
    tree = safe_parse_xml(filepath)
    tr_map = {f"{t['key']}|{t['field']}": t["korean"] for t in translations}
    for obj in tree.iter("object"):
        obj_name = obj.get("Name", "?")
        for part in obj.findall("part"):
            pname = part.get("Name", "")
            if pname == "Render":
                lookup = f"obj:{obj_name}|DisplayName"
                if lookup in tr_map:
                    part.set("DisplayName", tr_map[lookup])
            elif pname == "Description":
                lookup = f"obj:{obj_name}|Description.Short"
                if lookup in tr_map:
                    part.set("Short", tr_map[lookup])
            elif pname == "Food":
                lookup = f"obj:{obj_name}|Food.Message"
                if lookup in tr_map:
                    part.set("Message", tr_map[lookup])
        for xtag in obj.findall("xtagTextFragments"):
            for attr_name in list(xtag.attrib.keys()):
                lookup = f"obj:{obj_name}|xtag.{attr_name}"
                if lookup in tr_map:
                    xtag.set(attr_name, tr_map[lookup])
    output_path.parent.mkdir(parents=True, exist_ok=True)
    tree.write(output_path, encoding="utf-8", xml_declaration=True)


def inject_factions(filepath: Path, output_path: Path, translations: list[dict]):
    tree = safe_parse_xml(filepath)
    tr_map = {f"{t['key']}|{t['field']}": t["korean"] for t in translations}
    for faction in tree.iter("faction"):
        fname_val = faction.get("Name", "?")
        lookup = f"faction:{fname_val}|DisplayName"
        if lookup in tr_map:
            faction.set("DisplayName", tr_map[lookup])
    output_path.parent.mkdir(parents=True, exist_ok=True)
    tree.write(output_path, encoding="utf-8", xml_declaration=True)


def inject_genders(filepath: Path, output_path: Path, translations: list[dict]):
    tree = safe_parse_xml(filepath)
    tr_map = {f"{t['key']}|{t['field']}": t["korean"] for t in translations}
    for gender in tree.iter("gender"):
        gname = gender.get("Name", "?")
        for attr in ["Subjective", "Objective", "PossessiveAdjective", "SubstantivePossessive",
                      "Reflexive", "PersonTerm", "ImmaturePersonTerm", "FormalAddressTerm",
                      "OffspringTerm", "SiblingTerm", "ParentTerm"]:
            lookup = f"gender:{gname}|{attr}"
            if lookup in tr_map:
                gender.set(attr, tr_map[lookup])
    output_path.parent.mkdir(parents=True, exist_ok=True)
    tree.write(output_path, encoding="utf-8", xml_declaration=True)


def inject_manual(filepath: Path, output_path: Path, translations: list[dict]):
    tree = safe_parse_xml(filepath)
    tr_map = {f"{t['key']}|{t['field']}": t["korean"] for t in translations}
    for topic in tree.iter("topic"):
        tname = topic.get("name", "?")
        lookup = f"topic:{tname}|name"
        if lookup in tr_map:
            topic.set("name", tr_map[lookup])
        lookup = f"topic:{tname}|content"
        if lookup in tr_map:
            topic.text = tr_map[lookup]
    output_path.parent.mkdir(parents=True, exist_ok=True)
    tree.write(output_path, encoding="utf-8", xml_declaration=True)


def inject_history_spice(filepath: Path, output_path: Path, translations: list[dict]):
    with open(filepath, "r", encoding="utf-8") as f:
        data = json.load(f)
    tr_map = {t["key"]: t["korean"] for t in translations}

    def walk_and_replace(obj, path=""):
        if isinstance(obj, dict):
            for k in obj:
                obj[k] = walk_and_replace(obj[k], f"{path}.{k}" if path else k)
            return obj
        elif isinstance(obj, list):
            for i in range(len(obj)):
                if isinstance(obj[i], str):
                    key = f"{path}[{i}]"
                    if key in tr_map:
                        obj[i] = tr_map[key]
                else:
                    obj[i] = walk_and_replace(obj[i], f"{path}[{i}]")
            return obj
        elif isinstance(obj, str):
            if path in tr_map:
                return tr_map[path]
            return obj
        return obj

    walk_and_replace(data)
    output_path.parent.mkdir(parents=True, exist_ok=True)
    with open(output_path, "w", encoding="utf-8") as f:
        json.dump(data, f, ensure_ascii=False, indent=2)


FILE_INJECTORS = {
    "Conversations.xml": inject_conversations,
    "HiddenConversations.xml": inject_conversations,
    "Books.xml": inject_books,
    "Skills.xml": inject_skills,
    "Mutations.xml": inject_mutations,
    "HiddenMutations.xml": inject_mutations,
    "ActivatedAbilities.xml": None,  # 별도 구조라 별도 처리 필요
    "Commands.xml": inject_commands,
    "Options.xml": inject_options,
    "Quests.xml": inject_quests,
    "Subtypes.xml": inject_subtypes,
    "Naming.xml": None,  # 이름 조합 규칙은 단순 주입 불가
    "Factions.xml": inject_factions,
    "Genders.xml": inject_genders,
    "Manual.xml": inject_manual,
    "Genotypes.xml": None,  # 작은 파일, 필요시 추가
    "EmbarkModules.xml": None,  # 필요시 추가
    "HistorySpice.json": inject_history_spice,
}


def do_inject():
    """CSV의 번역을 korean-test/ XML/JSON에 주입"""
    if not CSV_PATH.exists():
        print(f"오류: {CSV_PATH} 파일이 없습니다. 먼저 extract를 실행하세요.")
        return

    translations = load_translations()
    injected_count = 0

    for fname, injector in FILE_INJECTORS.items():
        if fname not in translations:
            continue
        if injector is None:
            print(f"  [건너뜀] {fname} — 자동 주입 미지원 (수동 편집 필요)")
            continue
        source = GAME_BASE_DIR / fname
        output = KOREAN_DIR / fname
        if source.exists():
            print(f"  주입 중: {fname} ({len(translations[fname])}개 문자열)")
            injector(source, output, translations[fname])
            injected_count += len(translations[fname])

    # ObjectBlueprints
    for obj_file in OBJECT_BLUEPRINT_FILES:
        rel = f"ObjectBlueprints/{obj_file}"
        if rel not in translations:
            continue
        source = GAME_BASE_DIR / "ObjectBlueprints" / obj_file
        output = KOREAN_DIR / "ObjectBlueprints" / obj_file
        if source.exists():
            print(f"  주입 중: {rel} ({len(translations[rel])}개 문자열)")
            inject_objects(source, output, translations[rel])
            injected_count += len(translations[rel])

    print(f"\n총 {injected_count}개 번역 문자열 주입 완료 → {KOREAN_DIR}")


# ─────────────────────────────────────────────
#  상태 출력
# ─────────────────────────────────────────────

def do_status():
    """번역 진행률 출력"""
    if not CSV_PATH.exists():
        print(f"오류: {CSV_PATH} 파일이 없습니다. 먼저 extract를 실행하세요.")
        return

    stats = defaultdict(lambda: {"total": 0, "done": 0})
    with open(CSV_PATH, "r", encoding="utf-8", newline="") as f:
        reader = csv.DictReader(f)
        for row in reader:
            fname = row["file"]
            stats[fname]["total"] += 1
            if row.get("status") == "done":
                stats[fname]["done"] += 1

    total_all = 0
    done_all = 0
    print(f"\n{'파일':<40} {'완료':>6} {'전체':>6} {'진행률':>8}")
    print("-" * 65)
    for fname in sorted(stats.keys()):
        s = stats[fname]
        total_all += s["total"]
        done_all += s["done"]
        pct = (s["done"] / s["total"] * 100) if s["total"] > 0 else 0
        bar = "done" if pct >= 80 else "wip" if pct >= 20 else "todo"
        print(f"{fname:<40} {s['done']:>6} {s['total']:>6} {pct:>7.1f}% [{bar}]")
    print("-" * 65)
    pct_all = (done_all / total_all * 100) if total_all > 0 else 0
    print(f"{'합계':<40} {done_all:>6} {total_all:>6} {pct_all:>7.1f}%")


# ─────────────────────────────────────────────
#  엔트리 포인트
# ─────────────────────────────────────────────

def main():
    if len(sys.argv) < 2:
        print(__doc__)
        return

    cmd = sys.argv[1].lower()
    if cmd == "extract":
        print("=== 문자열 추출 시작 ===")
        do_extract()
    elif cmd == "inject":
        print("=== 번역 주입 시작 ===")
        do_inject()
    elif cmd == "status":
        do_status()
    else:
        print(f"알 수 없는 명령: {cmd}")
        print(__doc__)


if __name__ == "__main__":
    main()
