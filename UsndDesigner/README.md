# SkySound Designer for USnd

Unity版の音声設定デザイナーツールです。USndのXML設定ファイルを編集し、リアルタイムでパラメータをプレビューできます。

## 🎯 主な機能

- **XML編集**: Master, Category, Labelの設定を直感的に編集
- **リアルタイムプレビュー**: Unity Audio APIで正確な音声プレビュー
- **同時再生対応**: 複数の音声を同時に再生・テスト可能
- **USndパラメータ完全対応**:
  - Volume, Pan, Pitch
  - Delay, FadeIn/FadeOut
  - Loop (WAVループポイント自動認識)
  - Random系パラメータ
  - Move系パラメータ

## 📂 プロジェクト構造

```
UsndDesigner/
├─ Assets/
│   ├─ _Shared/              # WPFから移植した共通コード
│   │   ├─ Models.cs         # データモデル
│   │   └─ XmlStore.cs       # XML読み書き
│   │
│   ├─ Scripts/
│   │   ├─ Core/
│   │   │   └─ DataManager.cs    # データ管理
│   │   ├─ Audio/
│   │   │   └─ UnityAudioPlayer.cs  # 音声再生エンジン
│   │   └─ UI/                # UI Controllers (今後追加)
│   │
│   ├─ Scenes/
│   │   └─ EditorMain.unity  # メインシーン
│   │
│   └─ Resources/
│       ├─ Audio/            # テスト用音声ファイル
│       └─ XML/              # 設定XML
│
└─ ProjectSettings/          # Unity設定

```

## 🚀 セットアップ

### 1. Unityで開く

1. Unity Hub を開く
2. 「追加」→ `UsndDesigner` フォルダを選択
3. Unity 2019.4 LTS以降で開く

### 2. XMLファイルの配置

`Assets/Resources/XML/` に以下のファイルを配置：
- `MasterSettings.xml`
- `CategorySettings.xml`
- `LabelSettings.xml`

### 3. 音声ファイルの配置

`Assets/Resources/Audio/` にWAVファイルを配置

## 💡 使い方

### 基本操作

1. Unity Editorで `EditorMain.unity` シーンを開く
2. 再生ボタンをクリック
3. UI上でパラメータを編集
4. 音声を再生してプレビュー
5. XMLをエクスポート

### スクリプトAPI

```csharp
// DataManagerで設定を管理
DataManager dataManager = GetComponent<DataManager>();
dataManager.LoadXml();

// AudioPlayerで音声を再生
UnityAudioPlayer player = GetComponent<UnityAudioPlayer>();
int instanceId = player.PlayWithParameters(
    clip: audioClip,
    loop: true,
    volume: 0.8f,
    pan: -0.3f,
    pitchCent: 100,  // 半音上げる
    delay: 0.5f,
    fadeInTime: 2.0f
);

// 停止
player.Stop(instanceId, fadeOutTime: 1.0f);
```

## 🎨 Unity UI実装（Phase 2）

次のフェーズでuGUIを使用したエディタUIを実装予定：

- 3カラムレイアウト（Browser | Detail | Control）
- ドラッグ&ドロップ対応
- リアルタイム波形表示
- パラメータスライダー

## 🔧 技術仕様

- **Unity Version**: 2019.4 LTS以降推奨
- **C# Version**: 7.3以降
- **依存関係**: なし（Unity標準APIのみ使用）

## 📝 WPFバージョンとの違い

| 項目 | WPF版 | Unity版 |
|------|-------|---------|
| 音声API | NAudio | Unity Audio API |
| 同時再生 | 複雑 | 簡単 |
| ループポイント | 手動解析 | 自動認識 |
| UI | XAML | uGUI (Phase 2) |
| クロスプラットフォーム | Windows | Win/Mac/Linux |

## 🛠️ 開発ロードマップ

### Phase 1: コアシステム ✅
- [x] プロジェクト構造作成
- [x] 共通コード移植
- [x] UnityAudioPlayer実装
- [x] DataManager実装

### Phase 2: UI実装 (Next)
- [ ] uGUIレイアウト作成
- [ ] Label一覧表示
- [ ] パラメータ編集パネル
- [ ] 再生コントロール

### Phase 3: 高度な機能
- [ ] 波形表示
- [ ] ドラッグ&ドロップ
- [ ] プリセット機能
- [ ] エクスポートオプション

## 📄 ライセンス

USndプロジェクトに準拠

## 👤 作成者

SkySound Designer - Chang

---

**Note**: このプロジェクトは既存のWPFアプリを基に、Unity環境向けに再実装したものです。

