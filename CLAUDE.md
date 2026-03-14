# Caves of Qud 한국어 번역 프로젝트

## 프로젝트 요약
- Caves of Qud (로그라이크 RPG) 한국어 번역 패치 모드
- 원본: https://github.com/qudkorean/qud_korean_localization (방치됨)
- 포크: https://github.com/NOBI327/qud_korean_localization

## 핵심 경로
- 프로젝트: `C:\claude_pj\qud_korean_localization`
- 게임 원본 데이터: `C:\Program Files (x86)\Steam\steamapps\common\Caves of Qud\CoQ_Data\StreamingAssets\Base\`
- 번역 모드: `korean-test/`
- 추출/주입 도구: `tools/extract_strings.py`
- 통합 번역 리스트: `tools/strings.csv` (17,122개 문자열)

## 작업 문서
- `docs/project-status.md` — 완료된 작업, 현재 진행률 상세
- `docs/next-steps.md` — 다음 작업 계획 및 우선순위
- `docs/file-structures.md` — 게임 데이터 파일 구조 레퍼런스

## 도구 사용법
```bash
PYTHONIOENCODING=utf-8 python tools/extract_strings.py extract  # 게임→CSV
PYTHONIOENCODING=utf-8 python tools/extract_strings.py inject   # CSV→모드
PYTHONIOENCODING=utf-8 python tools/extract_strings.py status   # 진행률
```

## 현재 상태 (2026-03-14 세션 4 이후)
- 전체 진행률: **17,122 / 17,122 (100.0%)**
- 전 28개 파일 100% 번역 완료
- 특수 파일 3개 설계 완료:
  - Genders.xml — 한국어 대명사 체계 (그/그녀/그것/그들), 네오프로노운 원문 유지
  - Naming.xml — 음절 원문 유지, 템플릿/변수 290개 번역
  - HistorySpice.json — 8개 병렬 에이전트로 5,594개 완전 번역
- 다음 작업: 품질 검수 → 게임 테스트
