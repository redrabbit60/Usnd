
# USnd

## 更新履歴

* 2021/10/07
    * Play2DにPlayOptionと同等の引数を渡せるバージョンを追加

* 2021/03/12
    * tvOS対応のUSndPluginを追加
    * tvOS追加に伴い、USndPluginを無効にする機能を追加
    * USndPlugin説明書をドキュメントに追加

* 2020/08/21
    * AndroidJarを更新プラグインを更新（ Exception の Log .v を削除、 d eprecated になっていた箇
所の対応）
    * GetLabelVolume が返す値が正しくなかったのを修正

* 2020/05/20
    * ver2.18.0で修正したダッキングのUpdate処理負荷軽減によりダッキング復帰時間が設定されているとダッキング復帰しなくなっていたのを修正

* 2020/05/18
    * パラメータエディタでUnityMixer、3D Groupの値を実行中に変更できるようApplyボタンを追加

* 2020/03/17
    * 現在発音中の総数を取得するGetCurrentPlayNumを追加
    * OffPauseCategoryの除外するインスタンスを指定するバージョンで、検査するインスタンスリストのインデックスが間違っていたのを修正
    * 再生に使用するAPIによって、エラーログが正しく出力されていなかったのを修正
    * USND_OUTPUT_CALL_LOGが有効なときにAssetBundleを出力するとAddCallLogが定義されていないというエラーが出てしまうのを修正
    * カテゴリが大量にあるとき、ダッキングの処理でUpdateの処理負荷が大きいのを軽減する対策を追加


* 2019/10/29
    * OffPauseCategoryに指定したインスタンスを除いて処理をするList<int>を引数にとるバージョンを追加
    * 指定したラベルはインターバル中か調べるIsIntervalを追加
    * targetTransformのリセットが抜けていたのを修正
    * USndToolの表示を調整、表示順の設定を追加


* 2019/05/31
    * USnd Designer TutorialにAudioMixerに関するTIPSを追加
    * USndツールがアクティブな状態で再生失敗したとき、ラベル情報の設定でエラーになるのを修正
    * USndTools.xlsのデバッグログに全角チェック、一部項目の出現チェックを追加
    * USndToolのレイアウト調整


* 2019/04/17 ver2.16.0
    * IsLoop、GetLabelMaxPlaybacksNum、GetCategoryMaxPlaybacksNum、GetCategoryMaxPlaybacksNumFromLabelを追加
    * 加藤さんよりいただいたパフォーマンス改善のコードをマージ（重複してUpdateされていた箇所の修正と、3Dサウンドのtransformのキャッシュ方法を修正
    * USndEditorのパラメータコピーが正しく動かなくなっていたのを修正

* 2018/05/07 ver2.15.0
    * ForceResetDucking, ForceResetDuckingAllを追加
    * ダッキング設定されているラベルがAudioManagerのUpdate実行前にラベル削除されていたときにダッキング復帰が正常に動作しなくなるのを修正

* 2018/03/09 ver2.14.0
    * GetRandomSourceNamesを追加

* 2018/02/22 ver2.13.0
    * GetAudioClipNamesを追加

* 2018/01/25 ver2.12.1
    * SetCategoryVolume,SetMasterVolumeのmoveTimeに0秒を指定してもパラメータが即時反映されなかったのを修正
    * SetCategoryVolume, SetMasterVolumeのmoveTimeのデフォルト引数を-1から0に変更

* 2017/12/27 ver2.12.0
    * 3Dサウンドのデザイナ向けエディット機能追加

* 2017/11/20 ver2.11.1
    * ダッキング開始が0秒のとき挙動が正しくなかったのを修正
    * USndViewer起動時にデフォルトサンプルレートを選択できるようにUIを追加

* 2017/11/10 ver2.11.0
    * Initialzieの引数でデフォルトの出力サンプルレートを指定できるように変更

* 2017/09/14 ver2.10.0
    * ・JSON形式でMaster, Category, Label情報とAudio3DSettingsを読み込む機能を追加
        * LoadJson
        * GetLoadJsonStatus
        * SetAudio3DSettingsFromJson
    * SetPositionで指定した値が反映されていない、反映タイミングが遅いのを修正
    * IsExistAudioClipを追加
    * SetAudioClipを行っていない状態で再生したときPlay前にAudioClipを検索して自動的に紐付けを行うよう変更

* 2017/04/04 ver2.9.0
    * GetSpectrumDataを追加
    * AudioDebugLogをUSND_DEBUG_LOGが定義されているときのみ完全に有効になるように修正
    * usndplugin.jarをAndroidStudioで生成

* 2017/02/10 ver2.8.0
    * SetTime, SetTimeSamplesを追加

* 2017/01/26 ver2.7.1
    * ランダム関連のmin, maxパラメータが同値だったとき止まらずに動くように修正（Debug時は警告を表示）
    * ピッチの計算を省略し、値をテーブルで持つように変更
    * 処理の見直し
    * ダッキングスタート秒数が0のときに音量が復帰しなくなるのを修正

* 2016/12/06 ver2.7.0
    * Play3D、SetTrackingObjectに引数がTransformのものを追加
    * 3D設定があるラベルを強制的に2D再生するPlay2Dを追加
    * フェード処理がTime.timeScaleに影響して速度が変わっていたのでunscaledTimeを使用するように修正
    * moveTimeを0にしてSetVolumeを行なうと値が反映されていなかったのを修正
    * ラベルに指定しているオーディオファイルの総再生秒数を取得するGetLabelLength,総サンプル数を取得するGetLabelSamplesを追加
    * 再生を行ってから次に再生可能になるまでの間隔を設定するIntervalパラメータを追加(テーブルで設定)


* 2016/11/29 ver2.6.3
    * PreloadAudioDataがfalseのとき自動的にUnloadAudioDataを行なわないように変更

* 2016/11/24 ver2.6.2
    * インスタンスのリセット処理を呼び出す箇所を追加
    * ドキュメントに3Dサウンド使用時の手順を追記

* 2016/10/25 ver2.6.1
    * iOSのlibsnd.aをXcode8でビルドしなおし（ソースコードは変更なし）
    * [USndViewer]NameList更新を実行したときに自動的にReimportを行なうように処理を追加
    * [USndViewer]UIの表示不具合の修正
    * 再生途中から再開するフラグが立っているラベルが混在しているとseek errorが出ることがあるので再生前に開始位置をリセットするように修正


* 2016/10/13 ver2.6.0
    * TerminateとInitializeを同じフレーム内で行なうと正しく動作しない症状の対応を追加
    * AndroidでSoundPool Java APIを使用して再生する設定を追加
    * フェード制御の秒数で誤差が大きすぎるので、時間取得方法をTime.deltaTimeからTime.timeに変更

* 2016/06/15 ver2.5.1
    * ContainsKeyを使っていた箇所を一部TryGetValueに修正

* 2016/04/27 ver2.5.0
    * ラベルをアンロードせずにAudioClipの参照をはずせるようAPIを追加
        * UnsetAudioClipToLabel
        * UnsetAudioClipToLabelLoadId
        * UnsetAudioClipToLabelAll
    * USndPluginの各プラットフォーム依存部分を、各プラットフォーム環境以外ではビルド対象からはずすように変更
    * Androidのオーディオフォーカスがポーズから復帰したときに再取得されていなかったのを修正

* 2016/03/29 ver2.4.2
    * ver2.4.1で変更したStop処理が再生中のデータでもDelay完了前と判定されることがあったので判定方法を変更
    * AudioPlayer.cppのUpdateで行なっているUnloadAudioDataの前にclipがnullか判定を追加

* 2016/03/02 ver2.4.1
    * フェードアウトが設定されているラベルをDelay設定ありのPlayを行ったときDelay完了前にStopが実行されるとDelay秒後に再生を開始し同時にフェードアウト停止が実行されていたのを修正、Delay秒経過前は即停止するように変更
    * Androidのオーディオフォーカス有効時にonPauseで再生を止めていたのを止めないように修正（キーボードなどの割り込みで再生がとまってしまうため

* 2016/02/17 ver2.4.0
    * RemoveLabelが内部でAudioSourceの終了処理が終わっていないうちに発生するとAudioSourceをActiveのままロックしてしまっていたのを修正
    * RemoveLabelの戻り値をboolに変更
    * RemoveLabelが成功するか調べるAPIを追加
        * CanRemoveLabel

* 2016/02/15 ver2.3.0
    * Git管理開始

-----

## 概要

USndはUnity5用サウンドクラスライブラリです。  
Unity5の純正APIを使用してオーディオの再生を行い、Unityに足りない機能をUSnd側で吸収し再生管理を行うクラスです。  
一部サポート機能はプラグインで実装しています。  
下記のような機能が使用できます。  


**サポートしている機能**

- ラベル単位で発音数制御を行う
- ラベルを任意のグループにわけ、発音制御を行う
- フェードイン、フェードアウト
- 最後に停止した位置から再生を再開する
- ピッチをセント単位で扱う
- ピッチを指定した時間で変更する
- パンを指定した時間で変更する
- ランダムボリューム
- ランダムピッチ
- ランダムパン
- ランダムに指定したAudioSourceを再生する
- グループ化した単位でのダッキング
- グループ化した単位での音量変更
- ミュート
- AudioClipのロード/アンロード(Preload Audio Data無効時)
- AudioSnapshotの設定切り替え


**プラグインでサポートしている機能**

- 他のアプリケーションがオーディオ再生しているか取得する(iOSのみ)
- 端末スピーカーから再生されているか取得する
- マナーモードか取得する
- Androidのオーディオフォーカス取得(有効時のみ、デフォルトは無効)



**UnityEditorのみの機能**

- コンパイル設定（サウンド関連のdefine設定切替）
- USndTool(ロード済み情報の参照、再生インスタンスの表示、ログ表示)
- パラメータエディタ（マスター、カテゴリ、ラベルの設定を実行中に変更する）


**Unityでサポートされている機能**

- サステインループの読み込み(Windows版SoundForgeのsmplチャンクを反映)
- AudioMixerによるボリューム管理
- AudioMixerによるダッキング
- AudioMixerによるエフェクト設定
etc…


----

## フォルダ内容

**dll**

- USndのDLL版を生成するためのファイルが配置されています。

**docs**

- USndのドキュメントが配置されています。プログラマー向けとデザイナー向けの2種類があります。

**plugins**

- USndで使用するスタティックライブラリの生成用プロジェクトが配置されています。


**release**

- USnd配布用のファイル一式が配置されています。実装の際はここから必要なファイルを取得してください。
- C#スクリプトファイルのほか、DLL版、iOSのbit_codeフラグをyes/noにしたものそれぞれ2種が配置されています。

**tests**

- USndの挙動チェック用プロジェクトが配置されています。

**tools**

- デザイナー向けUSnd動作確認ツールUSndViewerのプロジェクトが配置されています。

**xml**

- USndで使用するサウンドテーブルを編集するためのエクセルファイルとXSDファイルが配置されています。


----

## 質問・修正依頼について

**質問**

- 使用方法などで不明点がある場合は、ノーツメールにてSuwa_Yukiko10015349/kde@konamiまでご連絡ください。
- ドキュメントに使用方法・トラブルシューティングなど記載がありますので、そちらもご参照ください。

**機能追加要望**

- 機能追加のご要望がある場合はノーツメールにてSuwa_Yukiko10015349/kde@konamiまでご連絡ください。
- USndは汎用ライブラリであることが目的なので、タイトルに依存する修正・機能追加はご要望に添えない場合がありますがご了承ください。

**修正依頼**

- 不具合を発見した場合、ノーツメールにてSuwa_Yukiko10015349/kde@konamiまでご連絡ください。
- もしくはGitLabのIssuesから報告を行なってください。

**マージリクエスト**

- 不具合の修正を行なわれた場合は、該当箇所をノーツメールにてSuwa_Yukiko10015349/kde@konamiまで送付してください。
- もしくはGitLabのMergeRequestsを使用し修正内容のマージを行なってください。
- 該当ファイルのサイズが大きい場合はアップローダーなどサーバー経由で渡してください。


ノーツメール経由、GitLab経由どちらでも問題ありません。
やりやすい方でお気軽にご連絡ください。

