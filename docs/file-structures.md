# 게임 데이터 파일 구조 레퍼런스

## 게임 데이터 위치
- 게임 원본: `C:\Program Files (x86)\Steam\steamapps\common\Caves of Qud\CoQ_Data\StreamingAssets\Base\`
- 번역 모드: `korean-test/` (CoQ 모드 시스템으로 XML override)

## XML 파일별 번역 대상 필드

### Conversations.xml / HiddenConversations.xml
```xml
<conversations>
  <conversation ID="Argyve">
    <node ID="Start">
      <text>번역 대상 텍스트</text>        <!-- <text> 요소의 텍스트 -->
      <choice ID="Choice1" Target="Node2">
        <text>선택지 텍스트</text>          <!-- 선택지 <text> -->
      </choice>
    </node>
  </conversation>
</conversations>
```
- 번역 대상: `<text>` 요소 내용
- 템플릿 변수 보존 필요: `=name=`, `=subject.waterRitualLiquid=`, `=pronouns.siblingTerm=`, `=player.t=`
- 색상 코드 보존: `{{W|텍스트}}`, `{{C|텍스트}}`, `{{emote|동작}}`

### Books.xml
```xml
<books>
  <book ID="Skybear" Title="{{W|Song of the Sky-Bear}}">
    <page>페이지 내용 (여러 줄)</page>
    <page>다음 페이지...</page>
  </book>
</books>
```
- 번역 대상: `Title` 속성, `<page>` 요소 내용
- `[[A.]]` 등 선택지 마커 보존 필요

### Skills.xml
```xml
<skills>
  <skill Name="Axe" Description="번역 대상">
    <power Name="Cleave" Description="번역 대상" Cost="150" />
  </skill>
</skills>
```
- 번역 대상: `skill/@Description`, `power/@Description`
- Name은 ID로 사용되므로 번역하지 않음

### Mutations.xml / HiddenMutations.xml
```xml
<mutations>
  <category Name="Physical" DisplayName="{{G|Physical Mutations}}">
    <mutation Name="Beak" BearerDescription="those with beaks">
      <description>설명 텍스트</description>
      <leveltext Level="1">레벨별 설명</leveltext>
    </mutation>
  </category>
</mutations>
```
- 번역 대상: `category/@DisplayName`, `mutation/@BearerDescription`, `<description>`, `<leveltext>`

### ObjectBlueprints/*.xml (Items, Creatures, Foods, Furniture, Walls, etc.)
```xml
<objects>
  <object Name="Long Sword" Inherits="LongBlade">
    <part Name="Render" DisplayName="long sword" />
    <part Name="Description" Short="A long, sharp blade." />
    <part Name="Food" Message="Tasty!" />                    <!-- Foods만 -->
  </object>
</objects>
```
- 번역 대상: `Render/@DisplayName`, `Description/@Short`, `Food/@Message`
- `DisplayName`에 색상 코드 포함 가능: `{{c|projectile}}`

### Quests.xml
```xml
<quests>
  <quest Name="퀘스트 이름" Accomplishment="업적 설명" Hagiograph="성인전 텍스트" Gospel="복음서 텍스트">
    <step Name="단계 이름" XP="50">
      <text>단계 설명</text>
    </step>
  </quest>
</quests>
```
- 번역 대상: `quest/@Name`, `@Accomplishment`, `@Hagiograph`, `@Gospel`, `step/@Name`, `step/text`
- 템플릿 변수: `=name=`, `=month=`, `=year=`, `=player.subjective=`, `<spice...>`

### Options.xml
```xml
<options>
  <option ID="OptionMasterVolume" DisplayText="Main volume" Type="Slider">
    <helptext>도움말 텍스트</helptext>
  </option>
</options>
```
- 번역 대상: `@DisplayText`, `<helptext>`

### Commands.xml
```xml
<commands>
  <command ID="CmdMoveN" DisplayText="Move north" Category="Movement" />
</commands>
```
- 번역 대상: `@DisplayText`

### Factions.xml
```xml
<factions>
  <faction Name="Joppa" DisplayName="Joppa" Visible="true" />
</factions>
```
- 번역 대상: `@DisplayName`

### Genders.xml
```xml
<genders>
  <gender Name="male" Subjective="he" Objective="him" PossessiveAdjective="his"
          Reflexive="himself" PersonTerm="man" SiblingTerm="brother" ParentTerm="father" />
</genders>
```
- 번역 대상: 모든 대명사/관계 속성
- 한국어 대명사 체계에 맞게 재설계 필요

### Manual.xml
```xml
<help>
  <topic name="Quickstart">
    도움말 내용 (여러 줄, {{W|강조}}, ~CmdMoveN 등 포함)
  </topic>
</help>
```
- 번역 대상: `@name`, topic 내부 텍스트
- 키바인딩 참조 보존: `~CmdMoveN`, `{{?KB|...}}`, `{{?Gamepad|...}}`

### Subtypes.xml
```xml
<subtypes>
  <class ID="Mutant" ChargenTitle="Mutant" SingularTitle="mutant">
    <category Name="..." DisplayName="표시 이름">
      <subtype Name="이름" Description="설명">
        ...
      </subtype>
    </category>
  </class>
</subtypes>
```
- 번역 대상: `@ChargenTitle`, `@SingularTitle`, `category/@DisplayName`, `subtype/@Name`, `subtype/@Description`

### HistorySpice.json
```json
{
  "spice": {
    "elements": {
      "glass": {
        "professions": ["glassblower", "window maker"],
        "adjectives": ["glazed", "stained"],
        "murdermethods": ["by trapping <spice.pronouns.object.!random> in a prism"]
      }
    }
  }
}
```
- 번역 대상: 모든 배열 내 문자열 값
- 템플릿 참조 보존 필요: `<spice.xxx.!random>`, `<entity.possessivePronoun>`, `<^.materials.!random>`
- `*var*` 플레이스홀더 보존 필요

### Naming.xml
- 이름 생성용 prefix/infix/postfix 조각
- 단순 번역이 아닌 한국어 이름 체계 재설계 필요
- 현재 extract에 포함되나 inject 미지원

### EmbarkModules.xml, Genotypes.xml
- 소수 문자열, extract 포함 / inject 미지원 (필요시 추가)

## C# 패치 파일 (korean-test/Scripts/Patches/)

XML로 처리 불가능한 런타임 텍스트를 Harmony 패치로 번역:
- `TMPFallbackFontBundle.cs` — 한글 폰트 로딩
- `TextTranslator.cs` — 범용 텍스트 번역
- `MessageQueueTranslate.cs` — 게임 메시지 번역
- `CalendarText.cs` — 달력/날짜 형식
- `StomachStatusText.cs` — 배고픔 상태 텍스트
- `SkillTranslation/` — 스킬 표시명 런타임 번역
- `QuestTranslation/` — 퀘스트 표시명 런타임 번역
- `MutationDescriptionTranslation/` — 돌연변이 설명 런타임 번역
