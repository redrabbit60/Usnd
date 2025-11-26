# USnd Audio Parameters Implementation

## 実装済み機能

NAudioを使用して、UnityのUSndシステムと同等のオーディオパラメータをサポートしました。

### ✅ 完全実装されたパラメータ

1. **Volume（ボリューム）**
   - 範囲: 0.0 ～ 1.0
   - デフォルト値: 1.0
   - リアルタイムで調整可能

2. **Pan（パン）**
   - 範囲: -1.0（左）～ 1.0（右）
   - デフォルト値: 0.0（中央）
   - ステレオバランスを制御

3. **Pitch（ピッチ）**
   - 範囲: セント単位（100cent = 半音）
   - デフォルト値: 0（元のピッチ）
   - WdlResamplingSampleProviderを使用してリサンプリング
   - 例: 
     - +100 = 半音上げる
     - -100 = 半音下げる
     - +1200 = 1オクターブ上げる

4. **Delay（再生開始遅延）**
   - 範囲: 0秒以上
   - デフォルト値: 0秒
   - タイマーを使用して実装

5. **FadeIn（フェードイン）**
   - 範囲: 0秒以上
   - デフォルト値: 0秒
   - 10ms刻みでボリュームを徐々に上げる

6. **FadeOut（フェードアウト）**
   - 範囲: 0秒以上
   - デフォルト値: 0秒
   - 停止時に自動適用
   - 10ms刻みでボリュームを徐々に下げる

7. **Loop（ループ再生）**
   - TRUE/FALSE
   - LoopingSampleProviderを使用して実装

### 📝 使用方法

#### GUIから使用
1. ラベルを選択
2. 詳細パネルで以下のパラメータを設定：
   - Volume: ボリューム値（例: 0.8）
   - Pan: パン値（例: -0.5）
   - Pitch: ピッチシフト（例: 100）
   - Delay: 遅延秒数（例: 0.5）
   - FadeInTime: フェードイン時間（例: 1.0）
   - FadeOutTime: フェードアウト時間（例: 2.0）
   - Loop: TRUE/FALSE
3. 再生ボタンをクリック

#### ステータス表示
再生中は、適用されたパラメータがステータスバーに表示されます：
```
再生中: BGM_Title (path/to/file.wav) [Vol:0.80, Pan:-0.50, Pitch:100cent, Delay:0.50s, FadeIn:1.00s]
```

### 🔧 技術仕様

#### AudioPlayer.cs
- **SampleProviderチェーン構造**:
  ```
  AudioFileReader
    → LoopingSampleProvider (ループ時)
    → WdlResamplingSampleProvider (ピッチ変更時)
    → PanningSampleProvider (パン制御)
    → VolumeSampleProvider (ボリューム制御)
    → WaveOutEvent
  ```

#### パラメータ処理
- **ピッチ変換式**: `playbackRate = 2^(semitones/12)`
  - 100cent → 2^(1/12) ≈ 1.059倍
- **フェード処理**: 10ms毎にボリューム更新
- **遅延処理**: System.Threading.Timerを使用

### ⚠️ 制限事項

1. **ピッチシフト**
   - WdlResamplingSampleProviderを使用（リサンプリング方式）
   - 高度なピッチシフト（タイムストレッチなし）を行う場合は、SoundTouchライブラリ等の追加が必要

2. **未実装のUSndパラメータ**
   - Interval（再生間隔）- 次のバージョンで対応予定
   - Random系（VolRnd, PitchRnd, PanRnd）
   - Move系（MovePitch, MovePan）
   - Ducking（他音声のボリューム調整）
   - Category系の高度な制御

### 🚀 今後の拡張

1. **Intervalサポート**
   - 連続再生の間隔制御

2. **Random系パラメータ**
   - 再生毎にランダムなVol/Pitch/Panを適用

3. **Move系パラメータ**
   - 再生中にPitch/Panを動的に変化

4. **高度なピッチシフト**
   - NAudio.Extrasやサードパーティライブラリの統合

## コード例

```csharp
// 基本的な再生
_player.Play(filePath, loop: true);

// パラメータ付き再生
_player.PlayWithParameters(
    filePath: "audio/bgm.wav",
    loop: true,
    volume: 0.8f,      // 80%ボリューム
    pan: -0.3f,        // やや左
    pitchCent: 100,    // 半音上げる
    delay: 0.5f,       // 0.5秒後に開始
    fadeInTime: 2.0f   // 2秒かけてフェードイン
);

// フェードアウト付き停止
_player.StopWithFade(fadeOutTime: 3.0f); // 3秒かけてフェードアウト
```

---

**実装日**: 2025/11/26  
**バージョン**: 1.0  
**対応NAudio**: 2.2.1

