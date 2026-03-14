# Caves of Qud Korean Localization

<p align="center">
  <img src="assets/logo.png" width="400">
</p>

**로그라이크 오픈월드 RPG**

**케이브 오브 커드([Caves of Qud](https://store.steampowered.com/app/333640/Caves_of_Qud/)) 한글화 프로젝트**

## 프로젝트 현황

원본 프로젝트([qudkorean/qud_korean_localization](https://github.com/qudkorean/qud_korean_localization))의 진행이 멈춰 있어, 독자적으로 포크하여 전면 번역을 진행했습니다.

### 번역 범위

| 항목 | 수량 | 상태 |
|------|------|------|
| 총 번역 문자열 | 19,606개 | **100% 완료** |
| 대화 (NPC 대사 + 선택지) | 3,663개 | 완료 |
| 아이템·생물·가구 등 (ObjectBlueprints) | 5,875개 | 완료 |
| HistorySpice (절차적 역사 텍스트) | 5,594개 | 완료 |
| Naming (이름 생성) | 2,811개 | 완료 |
| 서적 (Books) | 263개 | 완료 |
| 퀘스트 (Quests) | 247개 | 완료 |
| 스킬·돌연변이 | 229개 | 완료 |
| 옵션·커맨드·UI | 387개 | 완료 |
| 성별·세력·매뉴얼 등 | 537개 | 완료 |
| 캐릭터 작성 UI (EmbarkModules, Genotypes) | 별도 | 완료 |

### 번역되지 않는 부분

- 게임 시작 시 오프닝 팝업 텍스트 (코드 하드코딩, 모드 오버라이드 미지원)
- 일부 시스템 어노테이션 (`[begin water ritual]`, `[begin trade]` 등)
- C# 코드에서 동적 생성되는 문장 (영어 어순으로 조합되어 한국어와 혼재)
- `character creation` 등 게임 UI 프레임워크 타이틀

## 적용법

### 다운로드

[한글패치 최신파일 다운로드](https://github.com/NOBI327/qud_korean_localization/releases/latest)에서 `korean-test.zip`을 다운로드하여 압축 해제합니다.

### 설치

1. `korean-test` 폴더를 아래 경로의 `Mods` 폴더에 복사합니다:

   ```
   // Windows
   C:\Users\[사용자 이름]\AppData\LocalLow\Freehold Games\CavesOfQud\Mods\

   // Linux
   ~/.config/unity3d/Freehold Games/CavesOfQud/Mods/
   ```

2. 게임을 시작하고, 메인 화면에서 **Mods** 탭을 선택합니다 (최초 선택 시 모드 활성화 경고창이 뜹니다).
3. 한글패치 모드를 선택하여 **Enable** 상태로 만듭니다.
4. **새 게임**을 시작합니다.

> **주의**: 기존 세이브에는 적용되지 않을 수 있습니다. 새 게임을 권장합니다.

## 도구

번역 작업에 사용된 도구가 `tools/` 디렉토리에 포함되어 있습니다.

```bash
# 게임 원본에서 문자열 추출 → CSV
PYTHONIOENCODING=utf-8 python tools/extract_strings.py extract

# CSV의 번역을 모드 파일에 주입
PYTHONIOENCODING=utf-8 python tools/extract_strings.py inject

# 번역 진행률 확인
PYTHONIOENCODING=utf-8 python tools/extract_strings.py status
```

모든 번역은 `tools/strings.csv` (19,606개 문자열)에서 관리됩니다.

## 문서

| 문서 | 설명 |
|------|------|
| `docs/translation-guide.md` | 톤 앤 매너, 고유명사 처리 원칙 |
| `docs/glossary.md` | 용어집 |
| `docs/expression-dictionary.md` | 관용표현, 어조 패턴 |
| `docs/file-structures.md` | 게임 데이터 파일 구조 |
| `docs/localization-techniques.md` | 로컬라이제이션 기법 참조서 |
| `docs/localization-practical-notes.md` | 실전 노하우 (다른 언어 패치 제작 시 참조) |

## 기여

생성형 AI번역, 손번역, 코드 수정, 번역 검수, 어휘 제안, 작업방식 제안, 버그 제보 등 모든 방식의 기여를 환영합니다.

- [CoQ 한글화 디스코드](https://discord.gg/BrvgDncE)

## 라이선스

이 프로젝트는 Caves of Qud의 모드 시스템을 이용한 비공식 번역 패치입니다.
