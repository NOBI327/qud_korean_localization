# 다음 작업 계획

## 세션 4 완료 사항 (2026-03-14 심야)

### 특수 파일 3개 완전 번역
- **Genders.xml** (120개): 한국어 대명사 체계 설계 (그/그녀/그것/그들)
- **Naming.xml** (2,811개): 음절 원문 유지 + 템플릿/변수 290개 번역
- **HistorySpice.json** (5,594개): 8개 sonnet 에이전트 병렬 투입, 완전 번역

### 진행률 변화
- **전체: 8,629 → 17,122 (50.4% → 100.0%)**
- 100% 완료 파일: 25개 → 28개 (전체 완료)

### inject 실행 완료
- 14,274개 번역 문자열 → korean-test/ 모드에 주입 완료

---

## 이전 세션 요약

- **세션 3** (야간): 미완료 4그룹 번역 완료 (41.1%→50.4%)
- **세션 2** (오후): 멀티에이전트 병렬 번역 (36.7%→41.1%)
- **세션 1** (오전): 문서 체계 구축, 도구 제작

---

## 다음 세션에서 할 일

### 1. 품질 검수
- Conversations.xml (1,190개) — 플레이어 노출 빈도 최고, 톤 일관성 검토
- HistorySpice.json — 절차적 텍스트 조합 결과가 자연스러운지 확인
- Naming.xml — 생성된 이름이 게임에서 정상 표시되는지 확인
- Genders.xml — 대명사 치환이 문맥에서 자연스러운지 확인
- glossary.md / expression-dictionary.md 기준 톤 일관성 검토
- AI번역과 손번역 혼재 부분 확인

### 2. 게임 테스트
- korean-test/ → `%LOCALAPPDATA%Low\Freehold Games\CavesOfQud\Mods\`에 복사
- 실제 플레이하며 확인할 항목:
  - 번역 누락/깨짐/어색한 부분
  - 대명사 치환 (`=pronouns.possessive=` 등) 정상 작동
  - 이름 생성 (Naming.xml) 정상 작동, NameGenFail 오류 없음
  - HistorySpice 절차적 텍스트가 자연스러운 한국어 문장으로 조합되는지
  - UI 공간 초과 (번역이 원문보다 길어서 잘리는 경우)
  - 색상 코드/템플릿 변수 깨짐 없음
  - 조사 처리 (을/를, 이/가) 자연스러운지

### 3. 수정 및 개선
- 게임 테스트에서 발견된 문제 수정
- 커뮤니티 피드백 반영
- GitHub 릴리즈 준비

---

## 도구

| 파일 | 용도 |
|------|------|
| `tools/extract_strings.py` | 추출/주입/상태 (extract, inject, status) |
| `tools/merge_localization.py` | localization_files/ → strings.csv 병합 (완료) |
| `tools/strings.csv` | 전체 번역 문자열 통합 리스트 (17,122개) |
| `tools/translate_genders.py` | Genders.xml 번역 스크립트 |
| `tools/translate_naming.py` | Naming.xml 번역 스크립트 |
| `tools/translate_hs_*.py` | HistorySpice 번역 스크립트 (8개) |
| `tools/todo_groups/` | 에이전트 번역용 JSON (완료분 보관) |
| `tools/todo_historyspice/` | HistorySpice 번역용 JSON (완료분 보관) |

## 참조 문서

| 문서 | 용도 |
|------|------|
| `docs/translation-guide.md` | 톤 앤 매너, 고유명사 처리 원칙 |
| `docs/glossary.md` | 용어집 (번역어 참조) |
| `docs/expression-dictionary.md` | 관용표현, 어조 패턴, 시스템 메시지 템플릿 |
| `docs/localization-techniques.md` | 기법 참조서 (일본어 패치 대비) |
| `docs/file-structures.md` | 게임 데이터 XML/JSON 구조 |
| `docs/project-status.md` | 진행률 상세 |
