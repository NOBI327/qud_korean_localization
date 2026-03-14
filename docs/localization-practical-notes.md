# Caves of Qud ローカライゼーション 実践ノート

> 韓国語パッチ制作（2026-03-14）で得た実践的ノウハウ。
> `localization-techniques.md` の補完文書。日本語パッチ等への展開時に参照。

---

## 1. 韓国語パッチからそのまま流用可能なもの

### 1.1 ツール群（言語非依存）

| ファイル | 用途 | 流用方法 |
|----------|------|----------|
| `tools/extract_strings.py` | 抽出/注入/進捗 | CSV列名を `korean` → `japanese` に変更するだけ |
| `tools/review_inline.py` | 翻訳品質の自動検証 | そのまま使える（変数・カラーコード検証） |
| `tools/fix_quality.py` | 一括修正テンプレート | 修正ロジックを参考にして新規作成 |

### 1.2 C# Harmony パッチ（翻訳辞書部分のみ差替）

| ファイル | 機能 | 流用方法 |
|----------|------|----------|
| `Scripts/Patches/TextTranslator.cs` | ポップアップ文字列のランタイム翻訳 | `Replacements` 辞書と `TemplateRules` の翻訳文を日本語に差替 |
| `Scripts/Patches/PopupShowText.cs` | Popup.Showのフック | **そのまま流用** |
| `Scripts/Patches/CalendarText.cs` | カレンダー表示翻訳 | 翻訳文を差替 |
| `Scripts/Patches/TMPFallbackFontBundle.cs` | フォントバンドルロード | **そのまま流用**（バンドルファイルのみ差替） |
| `Scripts/Patches/TooltipTMPFallbackApply.cs` | ツールチップフォント | **そのまま流用** |
| `Scripts/Patches/BookWidth.cs` | 本の表示幅調整 | 日本語でも必要（CJK文字幅） |
| `Scripts/Patches/StomachStatusText.cs` | 空腹状態テキスト | 翻訳文を差替 |
| `Scripts/Patches/SkillTranslation/*.cs` | スキル名ランタイム翻訳 | 構造はそのまま、辞書を差替 |
| `Scripts/Patches/QuestTranslation/*.cs` | クエスト名ランタイム翻訳 | 同上 |
| `Scripts/Patches/MutationDescriptionTranslation/*.cs` | 変異説明翻訳 | 同上 |

### 1.3 モド構成ファイル

| ファイル | 流用方法 |
|----------|----------|
| `manifest.json` | id/title/description を変更 |
| `GlobalConfig.json` | フォント設定を日本語フォントに変更 |

### 1.4 ドキュメント類

| ファイル | 流用方法 |
|----------|----------|
| `docs/file-structures.md` | **そのまま流用**（ゲーム構造は言語非依存） |
| `docs/localization-techniques.md` | **そのまま流用** |
| この文書 | 参照 |

### 1.5 流用できないもの（言語固有で再制作が必要）

- `docs/translation-guide.md` — トーン&マナーは日本語用に再設計
- `docs/glossary.md` — 用語集は日本語で新規策定
- `docs/expression-dictionary.md` — 慣用表現辞典は日本語で新規策定
- `tools/strings.csv` の翻訳列 — 当然ながら全文再翻訳
- `Genders.xml` — 日本語の代名詞体系で再設計（彼/彼女/それ/彼ら）
- `Naming.xml` — 日本語名前生成は別途設計
- フォントバンドル (`.bundle`) — 日本語フォントで再ビルド

---

## 2. 韓国語パッチで発見した重大な落とし穴

### 2.1 choice要素のテキスト格納形式が2種類ある

**問題**: Conversations.xmlの`<choice>`要素には2つの形式がある:

```xml
<!-- 形式A: <text>子要素 (81個) -->
<choice ID="WaterRitualChoice">
    <text>Your thirst is mine, my water is yours.</text>
</choice>

<!-- 形式B: インラインテキスト (2,426個) -->
<choice GotoID="Who">Who are you?</choice>
```

初期のextractスクリプトは形式Aのみ対応していたため、**2,426個の選択肢テキストが翻訳漏れ**になった。

**解決策**: extractとinject両方に形式B対応を追加。キーは `choiceinline:{GotoID or Target}` で区別。

```python
# 形式B: インラインテキスト
if choice.find("text") is None:
    t = (choice.text or "").strip()
    if is_translatable(t):
        ref = choice.get("GotoID", "") or choice.get("Target", "") or choice_id
        key = f"conv:{conv_id}/choiceinline:{ref}"
```

### 2.2 重複キー問題

**問題**: 同一conversation内で同じ`choice ID="?"`が複数存在する。CSVは1キー1行なので、後の行が前の行を上書きしてしまう。

```xml
<choice ID="?"><text>Text A</text></choice>  <!-- キー: conv:X/choice:? -->
<choice ID="?"><text>Text B</text></choice>  <!-- 同じキー！ -->
```

**解決策**: 重複キーにインデックスを付与: `conv:X/choice:?`, `conv:X/choice:?#1`, `conv:X/choice:?#2`

```python
idx = choice_key_counter.get(base_key, 0)
choice_key_counter[base_key] = idx + 1
key = f"{base_key}#{idx}" if idx > 0 else base_key
```

inject側も同じカウンターロジックで一致させる。

### 2.3 Genotypes.xml — extrainfoのmerge挙動

**問題**: モッドのGenotypes.xmlで`<extrainfo>`を翻訳すると、ゲームが原本と**両方表示**する（追加扱い）。

```
・High starting attributes     ← 原本（英語）
・Access to cybernetics         ← 原本
・높은 초기 능력치                ← モッド（韓国語）
・사이버네틱 이용 가능            ← モッド
```

**解決策**: `Load="Merge"`と`Load="Replace"`を使う:

```xml
<genotype Name="True Kin" Load="Merge" DisplayName="順血種">
    <stat Name="Strength" ChargenDescription="翻訳..." />
    <extrainfo Load="Replace">高い初期能力値</extrainfo>
    <extrainfo Load="Replace">サイバネティック利用可</extrainfo>
</genotype>
```

### 2.4 Subtypes.xml — Name属性がIDと表示名を兼ねる

**問題**: `<subtype Name="Apostle">`のNameは内部IDと表示名を兼ねている。Nameを翻訳すると`Gear="StartingGear_Apostle"`との参照は壊れない（Gearは独立した参照）が、**injectスクリプトが原本Nameを基準に上書きするため、手動翻訳が毎回元に戻される**。

**解決策**: inject実行後にSubtypes翻訳を再適用するスクリプトを用意。または、Subtypes.xmlをinject対象から除外して手動管理。

### 2.5 Text.txt — モッドオーバーライド非対応

**問題**: `Text.txt`（ゲーム開始時のオープニングテキスト等）はXMLではなくJSON風のテキストファイル。**モッドのオーバーライド機構が効かない。**

**解決策**: C# Harmony パッチ（TextTranslator.cs）のTemplateRuleで対応しようとしたが、翻訳文内の`{{W|...}}`カラーコードがC#の`string.Format`と競合してクラッシュ。

**最終結論**: オープニングテキストは現時点では翻訳不可。TemplateRuleで対応するなら、`{{}}`をエスケープする必要がある（`{{{{W|` → 4重ブレース）。ただし本件では未解決のまま残した。

### 2.6 EmbarkModules.xml — キャラクター作成UI

**問題**: キャラクター作成画面のテキストは`EmbarkModules.xml`に格納されているが、抽出スクリプトが未対応。

**解決策**: 手動でモッドフォルダにコピーして翻訳。注意点:
- `<code>`要素（base64エンコードされたキャラデータ）は絶対に触らない
- `<stringgamestate>`は触らない
- `<grid>`要素は触らない
- 翻訳対象: `<title>`, `<name>`, `<description>`, `Title`属性, `Name`属性（location）

### 2.7 NPC対話ノードの内容不一致

**問題**: AI並列翻訳エージェント使用時に、**NPC Start ノードの翻訳が他のノードの内容に入れ替わる**ケースが17件発生。

原因: エージェントがノードの順序を間違えてマッピングした。

**教訓**:
- 翻訳後に必ず**原文との内容一致検証**を行う
- 特にStart ノード（プレイヤーが最初に見る）は重点チェック
- 自動検証スクリプトで原文の最初の数単語と翻訳の意味方向を比較

### 2.8 Options.xml — UI テキストのトーン問題

**問題**: 設定メニューのテキストを文形式（「～してください」）で翻訳すると、UIトグルラベルとして長すぎ・違和感がある。

**解決策**: UIラベルは簡潔な名詞句・動詞原形に統一:
```
✗ 부동 피해 수치를 표시하세요.    （文形式）
✓ 부동 피해 수치 표시             （名詞句）
```

---

## 3. 並列翻訳エージェント運用の実践知見

### 3.1 エージェント設計パターン

```
1. CSVから未翻訳行を抽出 → JSONに分割（N個）
2. 各グループをエージェントに割り当て
3. エージェントは翻訳辞書を含むPythonスクリプトを生成
4. メインプロセスでスクリプトを順次実行してCSVに適用
5. inject → ゲームに反映
```

### 3.2 品質管理

エージェント生成の翻訳は適用前に必ず検証:

```python
# 自動検証項目
for orig, kr in translations.items():
    # テンプレート変数の保全
    assert set(re.findall(r'=[\w.]+?=', orig)) == set(re.findall(r'=[\w.]+?=', kr))
    # カラーコードの保全
    assert len(re.findall(r'\{\{[A-Za-z]+\|', orig)) == len(re.findall(r'\{\{[A-Za-z]+\|', kr))
    # 空翻訳チェック
    assert kr.strip()
    # チルダ数一致
    if '~' in orig:
        assert orig.count('~') == kr.count('~')
```

### 3.3 大量翻訳の効率化

- 高頻度反復テキストを先に辞書化（例: "Live and drink."が292回出現）
- 残りをN分割して並列エージェント投入
- 韓国語パッチでの実績: 2,426個のインライン選択肢を3エージェントで約20分で翻訳完了

---

## 4. 日本語パッチへの展開チェックリスト

### Phase 0: 環境構築
- [ ] 韓国語パッチリポジトリをフォーク
- [ ] `extract_strings.py` の `korean` → `japanese` 列名変更
- [ ] 言語検出正規式を `[\u3040-\u309F\u30A0-\u30FF\u4E00-\u9FFF]` に変更
- [ ] 日本語フォント用 `.bundle` ファイル作成（Unity Editor必要）
- [ ] `manifest.json` の id/title を変更

### Phase 1: 基盤文書
- [ ] `translation-guide.md` 日本語版作成（敬語体系、文体ルール）
- [ ] `glossary.md` 日本語版作成（固有名詞の表記統一）
- [ ] `expression-dictionary.md` 日本語版作成

### Phase 2: 翻訳
- [ ] `extract` 実行 → 19,606個の文字列取得
- [ ] 優先度順に翻訳（Conversations → ObjectBlueprints → Skills → Quests → ...）
- [ ] `EmbarkModules.xml` 手動翻訳（キャラクター作成UI）
- [ ] `Genotypes.xml` 手動翻訳（`Load="Merge"` + `Load="Replace"` 方式）
- [ ] `Subtypes.xml` 手動翻訳（inject後に再適用が必要）
- [ ] `Genders.xml` 日本語代名詞体系設計
- [ ] `HistorySpice.json` 翻訳（語順問題に注意）
- [ ] `TextTranslator.cs` の辞書を日本語に差替

### Phase 3: 品質管理
- [ ] 自動検証スクリプト実行（変数・カラーコード・チルダ）
- [ ] トーン一貫性セルフレビュー
- [ ] 用語集一致チェック
- [ ] ゲーム内テスト（キャラ作成 → 序盤プレイ → NPC対話 → メニュー）

### Phase 4: リリース
- [ ] inject → モッドフォルダにコピー → 最終テスト
- [ ] README作成
- [ ] GitHub Release

---

## 5. 最終統計（韓国語パッチ参考値）

| 項目 | 数値 |
|------|------|
| 総文字列数 | 19,606 |
| XML/JSON翻訳 | 19,606 (100%) |
| C# Harmony パッチ | 12ファイル |
| 翻訳対象ファイル数 | 30+ |
| HistorySpice.json | 5,594個（最大ファイル） |
| Conversations.xml (node + choice) | 3,663個 |
| ObjectBlueprints 合計 | 5,875個 |
| 品質検修で発見・修正した問題 | 118+ 件 |
| 所要セッション | 5回（1日） |

---

## 6. 既知の未解決事項

| 問題 | 原因 | 影響度 |
|------|------|--------|
| Text.txt のオープニングテキストが英語 | モッドオーバーライド非対応 + C#パッチでカラーコード競合 | 低（メッセージログには韓国語で表示される） |
| `[begin water ritual; 1 dram of water]` 等のシステムアノテーション | C#コードでハードコード生成 | 低（機能に影響なし） |
| `character creation` タイトルが英語 | ゲームUIフレームワークのハードコード | 低 |
| 一部のインラインchoiceが動的コード生成 | C#依存 | 低 |
