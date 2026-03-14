# Caves of Qud 한국어 번역 프로젝트 — 작업 현황

## 프로젝트 개요

- **원본 레포**: https://github.com/qudkorean/qud_korean_localization
- **포크 레포**: https://github.com/NOBI327/qud_korean_localization
- **로컬 경로**: `C:\claude_pj\qud_korean_localization`
- **게임 경로**: `C:\Program Files (x86)\Steam\steamapps\common\Caves of Qud`
- **게임 데이터**: `CoQ_Data\StreamingAssets\Base\` (XML/JSON 기반)
- **모드 방식**: Mods 폴더에 `korean-test/` 설치 → 게임이 XML override 로드

## 디렉토리 구조

```
qud_korean_localization/
├── korean-test/          # 실제 모드 파일 (게임 Mods 폴더에 복사하여 사용)
│   ├── manifest.json     # 모드 메타데이터
│   ├── d2coding.bundle   # 한글 폰트 에셋
│   ├── GlobalConfig.json # 폰트/설정
│   ├── Scripts/Patches/  # C# Harmony 패치 (런타임 번역 지원)
│   ├── ObjectBlueprints/ # 번역된 오브젝트 XML
│   └── *.xml / *.json    # 번역된 데이터 파일
├── original_files/       # 원본 영문 XML 스냅샷 (현재 게임 버전과 동일 확인됨)
├── localization_files/   # 작업 중간 파일 (병합 완료)
├── tools/
│   ├── extract_strings.py    # 추출/주입/상태 스크립트
│   ├── merge_localization.py # localization_files 병합 스크립트 (완료)
│   ├── strings.csv           # 전체 번역 문자열 통합 리스트 (17,122개)
│   ├── todo_groups/          # 에이전트 번역용 JSON (완료분 보관)
│   └── todo_historyspice/    # HistorySpice 번역용 JSON (완료분 보관)
└── docs/                 # 문서
```

## 완료된 작업

### 세션 1 (2026-03-14 오전)
1. 포크 및 클론
2. 원본 파일 비교 — 전부 동일 확인
3. 번역 진행률 분석
4. 추출/주입 도구 제작 (`tools/extract_strings.py`)
5. 문서 체계 구축 (translation-guide, glossary, expression-dictionary, localization-techniques)

### 세션 2 (2026-03-14 오후)
1. `localization_files/` 병합 — `merge_localization.py` 스크립트로 Quests.xml(154건), Books.xml 병합 완료
2. 멀티에이전트 병렬 번역 — 6개 에이전트 투입, 2개 완료(425개), 4개 토큰 소진
3. 품질 리뷰 & 수정 — 기술적 검증 통과, 3건 표현 수정
4. inject 실행 — 7,001개 번역 문자열 모드에 주입

### 세션 3 (2026-03-14 야간)
1. 세션 2에서 토큰 소진된 4개 그룹 번역 재시도
   - 대용량 그룹은 분할 (Group 2→2분할, Group 3→2분할, Group 6→3분할)
   - 에이전트가 Bash 권한 없이 Python 스크립트를 작성 → 메인에서 실행하는 패턴 확립
2. 전 그룹 기술적 검수 완료 — 색상코드/템플릿변수/닫힘태그 100% 보존 확인
   - Group 6-2에서 5건 변수 누락 발견 → 수정 적용
3. 1,595개 번역을 strings.csv에 통합 + inject 실행 (8,592개 주입)
4. **진행률: 41.1% → 50.4%** (7,034 → 8,629)
5. **100% 완료 파일: 15개 → 25개** (10개 신규 완료)

### 세션 4 (2026-03-14 심야)
1. **Genders.xml 완전 번역 (120개)**
   - 한국어 대명사 체계 설계: 그/그녀/그것/그들
   - 관계 용어 번역: 사내/여인, 형제/자매, 아버지/어머니 등
   - 힌드렌 호칭: 수사슴이여/암사슴이여
   - hartind: 고유명사 유지 (hartind여)
   - elverson: 네오프로노운 원문 유지 (ey/em/eir), 관계 용어만 번역
   - 절차적 대명사 생성 알고리즘: 원문 유지 (비활성 상태)
2. **Naming.xml 완전 번역 (2,811개)**
   - 음절 조합 (prefix/infix/postfix): 2,811개 원문 유지
   - 장소 템플릿 17개 번역 (바나나 숲, 폐허, 염습지 등)
   - 영웅 칭호/호칭 템플릿 50여 개 번역
   - 템플릿 변수값 ~200개 번역 (형용사, 직업, 지형, 호칭 등)
3. **HistorySpice.json 완전 번역 (5,594개)**
   - 순수 참조 531개 자동 처리 (원문 유지)
   - 8개 sonnet 에이전트 병렬 투입:
     - words_0~4: 단어 4,017개 (5개 에이전트)
     - phrases_0~1: 구문 820개 (2개 에이전트)
     - templates: 긴 템플릿 194개 (1개 에이전트)
   - 5,031개 신규 번역 + 531개 자동 처리 + 32개 기존 = 5,594개 완료
4. **진행률: 50.4% → 100.0%** (8,629 → 17,122)
5. **100% 완료 파일: 25개 → 28개** (전체 완료)

## 현재 번역 진행률 (2026-03-14 세션 4 이후)

**전체: 17,122 / 17,122 (100.0%)**

### 100% 완료 — 28개 파일 (전체)
| 파일 | 완료 | 전체 |
|------|------|------|
| ActivatedAbilities.xml | 33 | 33 |
| Books.xml | 263 | 263 |
| Commands.xml | 198 | 198 |
| Conversations.xml | 1,190 | 1,190 |
| Factions.xml | 65 | 65 |
| Genders.xml | 120 | 120 |
| Genotypes.xml | 4 | 4 |
| HiddenConversations.xml | 211 | 211 |
| HiddenMutations.xml | 50 | 50 |
| HistorySpice.json | 5,594 | 5,594 |
| Manual.xml | 24 | 24 |
| Mutations.xml | 85 | 85 |
| Naming.xml | 2,811 | 2,811 |
| ObjectBlueprints/Creatures.xml | 1,832 | 1,832 |
| ObjectBlueprints/Data.xml | 73 | 73 |
| ObjectBlueprints/Foods.xml | 310 | 310 |
| ObjectBlueprints/Furniture.xml | 708 | 708 |
| ObjectBlueprints/HiddenObjects.xml | 558 | 558 |
| ObjectBlueprints/Items.xml | 1,735 | 1,735 |
| ObjectBlueprints/PhysicalPhenomena.xml | 57 | 57 |
| ObjectBlueprints/Walls.xml | 298 | 298 |
| ObjectBlueprints/Widgets.xml | 9 | 9 |
| ObjectBlueprints/WorldTerrain.xml | 95 | 95 |
| ObjectBlueprints/ZoneTerrain.xml | 200 | 200 |
| Options.xml | 189 | 189 |
| Quests.xml | 247 | 247 |
| Skills.xml | 144 | 144 |
| Subtypes.xml | 19 | 19 |
