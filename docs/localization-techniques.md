# Caves of Qud 로컬라이제이션 기법 참조서

> 한국어 패치 제작 과정에서 확립한 기법을 언어 비의존적으로 정리한 문서.
> 일본어 등 다른 언어 패치 제작 시 참조용.

---

## 1. 전체 아키텍처

### 1.1 Qud 모드 시스템의 작동 원리

Caves of Qud는 `Mods/` 폴더에 놓인 모드를 로드하여 **원본 XML을 override**하는 구조다. 번역 모드는 이 메커니즘을 이용하여 원본 게임 파일을 직접 수정하지 않고 텍스트를 교체한다.

```
게임 원본 (Base/)
  └─ Conversations.xml, Skills.xml, ObjectBlueprints/*.xml, ...
모드 (Mods/korean-test/)
  └─ 동일 파일명으로 번역된 XML/JSON을 배치 → 게임이 override로 로드
```

**장점**: 게임 업데이트에도 원본이 보존되므로, diff로 변경점 확인 후 모드만 갱신하면 된다.

### 1.2 모드 구성 요소

```
모드 폴더/
├── manifest.json          # 모드 메타데이터 (id, title, version, author, tags)
├── [폰트].bundle          # Unity AssetBundle — 대상 언어 폰트
├── GlobalConfig.json      # 폰트 설정
├── Scripts/Patches/       # C# Harmony 패치 (런타임 텍스트 번역)
├── ObjectBlueprints/      # 번역된 오브젝트 XML
└── *.xml / *.json         # 번역된 데이터 파일
```

### 1.3 번역이 적용되는 세 가지 계층

| 계층 | 방식 | 대상 |
|------|------|------|
| **XML/JSON override** | 모드 폴더에 번역된 파일 배치 | 정적 데이터 (대화, 스킬, 아이템 등) |
| **C# Harmony 패치** | 런타임 문자열 치환 | 코드에 하드코딩된 텍스트 (시스템 메시지, UI, 달력 등) |
| **XML 속성 캡처+주입** | Harmony로 DisplayName 등을 런타임 교체 | XML에 번역을 넣었지만 게임이 Name 속성만 읽는 경우 |

---

## 2. 문자열 추출/관리 파이프라인

### 2.1 설계 사상

게임의 모든 번역 대상 문자열을 **단일 CSV**로 관리한다. 이 CSV가 번역의 유일한 진실의 원천(Single Source of Truth)이 된다.

```
[게임 원본] ──extract──→ [strings.csv] ──inject──→ [모드 파일]
                            ↑
                       번역 작업은 여기서
```

### 2.2 CSV 스키마

```csv
file,key,field,original,{언어코드},status
```

| 컬럼 | 설명 |
|------|------|
| `file` | 원본 파일명 (예: `Conversations.xml`, `ObjectBlueprints/Items.xml`) |
| `key` | 문자열의 고유 식별자 (예: `conv:Argyve/node:Start`, `obj:Long Sword`) |
| `field` | 필드 유형 (예: `text`, `DisplayName`, `Description.Short`) |
| `original` | 원문 (영어) |
| `{언어코드}` | 번역문 (`korean`, `japanese` 등으로 교체) |
| `status` | `todo` / `done` |

**핵심**: `file|key|field`의 3키 조합이 각 문자열의 고유 식별자.

### 2.3 키 설계 패턴

XML 구조를 반영하여 계층적 키를 생성한다.

| 파일 유형 | 키 패턴 | 예시 |
|-----------|---------|------|
| 대화 | `conv:{회화ID}/node:{노드ID}` | `conv:Argyve/node:Start` |
| 대화 선택지 | `conv:{회화ID}/choice:{선택지ID}` | `conv:Argyve/choice:Trade` |
| 조건부 텍스트 | `...//if:{조건}` | `conv:Base/choice:X/if:SociallyRepugnant` |
| 서적 | `book:{서적ID}/page:{페이지번호}` | `book:Skybear/page:0` |
| 스킬 | `skill:{스킬명}/power:{파워명}` | `skill:Axe/power:Cleave` |
| 돌연변이 | `mut:{변이명}` / `mutcat:{카테고리}` | `mut:Beak` |
| 오브젝트 | `obj:{오브젝트명}` | `obj:Long Sword` |
| 퀘스트 | `quest:{퀘스트명}/step:{단계명}` | `quest:ARealMess/step:Find` |
| 옵션 | `opt:{옵션ID}` | `opt:OptionMasterVolume` |
| 커맨드 | `cmd:{커맨드ID}` | `cmd:CmdMoveN` |
| 세력 | `faction:{세력명}` | `faction:Joppa` |
| 성별 | `gender:{성별명}` | `gender:male` |
| JSON경로 | `path.to.array[index]` | `spice.elements.glass.professions[0]` |

### 2.4 추출 시 필터링 규칙

모든 텍스트를 추출하면 노이즈가 많다. 다음을 제외한다:

```python
# 번역 불필요 항목 판별
def is_translatable(text):
    if not text.strip():                     return False  # 빈 문자열
    if re.fullmatch(r"[\d.,\-+%]+", text):   return False  # 숫자만
    if re.fullmatch(r"=[\w.]+=$", text):     return False  # 템플릿 변수만
    if re.fullmatch(r"[\w/\\._\-]+\.\w+", text):  return False  # 파일 경로
    if len(text.strip()) <= 1:               return False  # 1글자
    return True
```

### 2.5 잘못된 XML 문자 참조 처리

Qud의 XML에는 `&#x7;` 같은 **XML 표준 위반 문자 참조**가 포함되어 있다. `xml.etree.ElementTree`가 파싱을 거부하므로 전처리가 필요하다.

```python
def safe_parse_xml(filepath):
    content = filepath.read_text(encoding="utf-8")
    # &#xN; (N < 0x20, except 0x9, 0xA, 0xD) 를 안전한 플레이스홀더로 대체
    content = re.sub(r"&#x([0-9a-fA-F]+);", replace_if_invalid, content)
    content = re.sub(r"&#(\d+);",           replace_if_invalid_dec, content)
    return ET.ElementTree(ET.fromstring(content))
```

이 기법은 Qud뿐 아니라 비표준 XML을 사용하는 모든 게임에 적용 가능하다.

### 2.6 기존 번역 자동 감지

모드 폴더에 이미 번역이 들어가 있는 경우, 추출 시 자동으로 감지하여 CSV에 반영한다.

```
1. 원본 파일에서 문자열 추출 (키 생성)
2. 모드 파일에서 동일 키의 텍스트 추출
3. 대상 언어 문자가 포함되어 있으면 → status=done으로 매칭
```

**언어 감지 방법**: 정규식으로 해당 언어의 유니코드 범위를 확인한다.
- 한국어: `[\uAC00-\uD7A3]` (완성형 한글)
- 일본어: `[\u3040-\u309F\u30A0-\u30FF\u4E00-\u9FFF]` (히라가나+가타카나+한자)

---

## 3. C# Harmony 패치 기법

### 3.1 왜 필요한가

XML override로 처리할 수 없는 텍스트가 존재한다:
- C# 코드에 하드코딩된 문자열 (시스템 메시지, UI 텍스트)
- 런타임에 조합되는 문자열 (달력, 상태 표시)
- XML에 번역을 넣었으나 게임이 `Name` 속성을 ID로 사용하여 DisplayName을 무시하는 경우

### 3.2 직접 치환 패턴 (Dictionary 방식)

가장 단순한 패턴. 영어 원문을 키로, 번역문을 값으로 하는 딕셔너리.

```csharp
private static readonly Dictionary<string, string> Replacements = new()
{
    { "You cannot do that from here.", "ここからはできません。" },
    { "You embark for the caves of Qud.", "あなたはクッドの洞窟へ向かいます。" },
};

public static string Translate(string message)
{
    if (Replacements.TryGetValue(message, out string replacement))
        return replacement;
    return message;
}
```

**장점**: 구현이 간단하고, 정확히 일치하는 경우에만 치환하므로 안전.
**단점**: 동적 텍스트(변수가 포함된 메시지)는 처리 불가.

### 3.3 템플릿 규칙 패턴

동적 텍스트를 처리하기 위한 패턴. 고정 부분을 기준으로 변수 부분을 추출하고 재조합한다.

```csharp
// "You discover the location of {X}." → "{0}の位置を発見しました。"
new TemplateRule("You discover the location of ", ".", "{0}の位置を発見しました。"),

// "You have finished the step, {{G|{X}}}, of the quest {Y}!\nYou gain {{C|{Z}}} XP!"
new TemplateRule(
    new[] { "You have finished the step, {{G|", "}}, of the quest ", "!\nYou gain {{C|", "}} XP!" },
    "クエスト{1}の段階{{{{G|{0}}}}}を完了しました！\n{2}XPを獲得しました！"
),
```

**구현 핵심**:
1. 메시지가 Parts[0]로 시작하는か確認
2. 各Partの間のテキストを変数として抽出
3. 最後のPartがメッセージの末尾と一致するか確認
4. `string.Format(Format, values)` で再組み立て

### 3.4 Harmony Postfix パッチ

ゲームのメソッド戻り値を翻訳で置き換える基本パターン:

```csharp
[HarmonyPatch(typeof(Calendar), nameof(Calendar.GetMonth), new[] { typeof(int) })]
public static class Calendar_GetMonth_Patch
{
    public static void Postfix(ref string __result)
    {
        __result = CalendarText.Translate(__result);
    }
}
```

**Postfix**を使う理由: 元のメソッドの処理結果を受け取ってから翻訳するため、ゲームロジックを壊さない。

### 3.5 XML属性キャプチャ+ランタイム注入パターン

ゲームがXML属性`Name`をIDとして使う場合、`DisplayName`属性を追加してもゲームが読まない。この場合:

1. **キャプチャ**: XMLロード時にHarmonyで`DisplayName`属性の値を辞書に保存
2. **注入**: 表示時にHarmonyで`Name`を`DisplayName`に差し替え

```
XML: <skill Name="Axe" DisplayName="斧術" Description="...">
  → ゲーム内部ではName="Axe"で参照
  → 表示時にHarmonyがDisplayName="斧術"を返す
```

この手法はSkills, Quests, Mutationsなど多くのファイルで使われている。

---

## 4. フォント統合

### 4.1 Unity AssetBundle

Qudはデフォルトで英語フォントのみ含む。CJK文字を表示するには:

1. Unity EditorでCJKフォント（TTF/OTF）をTextMeshProフォントアセットに変換
2. AssetBundleとしてビルド（`.bundle`ファイル）
3. Harmonyパッチでフォールバックフォントとして登録

```csharp
// TMPFallbackFontBundle.cs の核心
// バンドルからフォントをロードし、TMPのフォールバックリストに追加
```

### 4.2 フォントサイズ調整

CJK文字は英語より横幅が広い場合がある。必要に応じて:
- テキストスケーリング（UITextSkinFontScale.cs）
- ツールチップのフォントスケール（LookTooltipFontScale.cs）
- 本の表示幅調整（BookWidth.cs: 80→65文字）

---

## 5. 翻訳ワークフロー設計

### 5.1 三段階パイプライン

```
Phase 1: 抽出 (extract)
  ゲーム原本 → CSV（全文字列を網羅）

Phase 2: 翻訳 (translate)
  CSVの翻訳列を埋める（人力 or AI翻訳 + レビュー）

Phase 3: 注入 (inject)
  CSV → MODファイル（XMLにもJSONにも対応）
```

### 5.2 進捗追跡

`status`列で管理。`extract_strings.py status`でファイル別進捗率を一覧表示。

```
파일                                     완료   전체   진행률
-----------------------------------------------------------------
Conversations.xml                       1161   1190    97.6% [done]
Skills.xml                                 2    144     1.4% [todo]
합계                                    6278  17122    36.7%
```

### 5.3 既存翻訳の保全

`extract`を再実行しても既存の翻訳は失われない:

```python
# 1. 既存CSVの翻訳を読み込み
existing = load_existing_korean(csv_path)
# 2. 新規抽出した行に既存翻訳をマッチング
if compound_key in existing:
    row["korean"] = existing[compound_key]
    row["status"] = "done"
```

ゲームがアップデートされた場合:
1. `extract`を再実行 → 新規文字列が`todo`で追加、既存翻訳は保持
2. 差分のみ翻訳すればよい

---

## 6. 特殊テキストの処理

### 6.1 テンプレート変数

ゲーム内テキストに埋め込まれた変数。**絶対に翻訳・改変してはならない。**

```
=name=                      # エンティティ名
=subject.waterRitualLiquid= # 水の儀式の液体名
=pronouns.siblingTerm=      # 代名詞：兄弟姉妹呼称
=verb:grab=                 # 活用される動詞
=player.reflexive=          # プレイヤーの再帰代名詞
```

### 6.2 カラーコード

テキストの色を制御するマークアップ。**構造を保持し、内部テキストのみ翻訳。**

```
{{W|White Text}}    → {{W|白いテキスト}}
{{G|Green Text}}    → {{G|緑のテキスト}}
{{emote|action}}    → {{emote|動作}}
```

### 6.3 助詞問題（言語固有）

**韓国語**: 前の文字の終声（パッチム）有無で助詞が変わる。テンプレート変数の後は両方を併記:
```
{0}을(를) 발견했습니다!
```

**日本語**: 助詞問題は基本的に発生しない。ただし「の」「を」「が」等の選択は文脈依存。

### 6.4 チルダ(~)区切り

会話の選択肢で`~`はランダム選択肢の区切り。構造をそのまま維持する:
```
選択肢A~
選択肢B~
選択肢C
```

### 6.5 その他保全すべきマークアップ

```
~CmdMoveN          # キーバインド参照
{{?KB|...}}         # キーボードバインド
{{?Gamepad|...}}    # ゲームパッドバインド
<spice.xxx.!random> # HistorySpice参照
<entity.name>       # エンティティ参照
*var*               # プレースホルダー
[[A.]]              # 選択肢マーカー（Books）
```

---

## 7. ファイル別の注意点

### 7.1 Genders.xml — 代名詞体系の再設計

英語の代名詞体系（he/she/they + 関係語）がテンプレート変数として全システムに組み込まれている。

```xml
<gender Name="male" Subjective="he" Objective="him"
        PossessiveAdjective="his" Reflexive="himself"
        PersonTerm="man" SiblingTerm="brother" ParentTerm="father" />
```

これらは`=pronouns.siblingTerm=`等でゲーム全体から参照される。
- **韓国語**: 性別区分が弱い言語。「그/그녀」よりも文脈で処理
- **日本語**: 同様に性別区分が弱い。「彼/彼女」は使えるが、PersonTerm等は要検討（男/女、兄/姉、父/母 etc.）

### 7.2 Naming.xml — 名前生成規則

prefix/infix/postfixの組み合わせで名前を生成するシステム。単純翻訳ではなく、対象言語の名前体系を設計する必要がある。

### 7.3 HistorySpice.json — 手続き的テキスト生成

テンプレート参照（`<spice.xxx.!random>`等）を含む文字列断片の巨大なコレクション（5,594個）。断片同士が組み合わさって文章になるため:
- 各断片の文法的役割（主語・述語・修飾語）を理解して翻訳
- 組み合わせ後に自然な文になるか検証が必要
- 言語によっては語順の問題で根本的な再設計が必要になる可能性

---

## 8. 品質管理チェックリスト

翻訳注入前に以下を自動/手動で確認:

- [ ] テンプレート変数(`=...=`)が破損していないか
- [ ] カラーコード(`{{X|...}}`)の構造が保持されているか
- [ ] チルダ(`~`)区切りが維持されているか
- [ ] 改行・空白が原文と同一か
- [ ] XMLとしてパース可能か（壊れたタグがないか）
- [ ] 用語集との一貫性（固有名詞の表記統一）
- [ ] ゲーム内で実際に表示確認（UI幅に収まるか）

### 自動検証スクリプトのアイデア

```python
# CSV内の翻訳をスキャンして問題を検出
for row in csv:
    original = row["original"]
    translated = row["translated"]
    # テンプレート変数の保全チェック
    orig_vars = re.findall(r"=[\w.]+?=", original)
    trans_vars = re.findall(r"=[\w.]+?=", translated)
    if set(orig_vars) != set(trans_vars):
        warn(f"変数不一致: {row['key']}")
    # カラーコードの保全チェック
    orig_colors = re.findall(r"\{\{(\w)\|", original)
    trans_colors = re.findall(r"\{\{(\w)\|", translated)
    if orig_colors != trans_colors:
        warn(f"カラーコード不一致: {row['key']}")
```

---

## 9. 新言語への展開手順

### Step 1: フォーク & 環境構築
```
1. 既存の翻訳MODリポジトリをフォーク
2. extract_strings.py の言語コード列を変更 (korean → japanese 等)
3. 言語検出正規式を変更
4. フォントバンドルを対象言語用に作成
```

### Step 2: 翻訳ガイドライン策定
```
1. 公式サイト・Wikiで世界観を把握
2. 既存の他言語翻訳があればトーン&マナーを参考に
3. 用語集を先に確定（固有名詞の表記方針）
4. 文体ルール策定（敬語レベル、話者別の口調）
5. 関用表現辞典の作成（繰り返し登場する定型表現）
```

### Step 3: 翻訳実行
```
1. extract → CSV生成
2. 優先度の高いファイルから翻訳
   高: Conversations > Skills > Quests > Mutations
   中: Commands > Factions > Manual
   低: HistorySpice > Naming (要再設計)
3. inject → ゲーム内確認
4. C#パッチの翻訳辞書を更新
```

### Step 4: 保守
```
1. ゲームアップデート時: extract再実行 → 差分翻訳
2. 用語変更時: CSV全体をsed/置換 → inject
3. 品質改善: CSV上で修正 → inject
```

---

## 10. 既知の落とし穴

### 10.1 Windows環境のエンコーディング

Windowsのデフォルトエンコーディング（cp932等）でPythonを実行すると、UTF-8のCSVが文字化けする。

```bash
# 常にこの環境変数を設定
PYTHONIOENCODING=utf-8 python tools/extract_strings.py extract
```

### 10.2 ElementTreeのXML出力

PythonのElementTreeは出力時に:
- 属性の順序を変える場合がある
- 名前空間宣言を移動する場合がある
- CDATA等を保持しない

→ diffが大量に出るが、実質的な変更ではない。gitの差分ノイズになる点に注意。

### 10.3 inject未対応ファイル

一部ファイルはextractはできるがinjectのロジックが未実装:
- `ActivatedAbilities.xml` — 構造が特殊
- `Naming.xml` — 名前生成規則は単純置換不可
- `Genotypes.xml`, `EmbarkModules.xml` — 小規模、必要時に追加

### 10.4 C#パッチとXMLの二重管理

同じテキストがXML翻訳とC#パッチの両方で処理されると競合する可能性がある。どちらで処理するかを明確に分離すること。

原則:
- **静的テキスト** → XML override（CSVで管理）
- **動的・ハードコードテキスト** → C# Harmony パッチ
- **XML属性をIDとして使うケース** → C#キャプチャ+注入パターン
