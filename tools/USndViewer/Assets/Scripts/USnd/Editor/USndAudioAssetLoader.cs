using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class USndAudioAssetLoader : EditorWindow {

    private static int IMPORT_STATUS_WAIT = 0;
    private static int IMPORT_STATUS_COROUTINE = 1;
    private static int IMPORT_STATUS_PROCESS = 2;


    private static int importStatus = IMPORT_STATUS_WAIT;
    private static bool IMPORT_LOAD_IN_BACKGROUND;
    private static AudioImporterSampleSettings IMPORT_SETTINGS;

    private static int ASSET_PATH = 0;
    private static int LOAD_IN_BACKGROUND = 1;
    private static int LOAD_TYPE = 2;
    private static int COMPRESSION_FORMAT = 3;
    private static int QUARITY = 4;
    private static int SAMPLERATE_SETTINGS = 5;
    private static int SAMPLERATE_OVERRIDE = 6;

    private static System.IO.StreamReader reader;


    [MenuItem("USnd/AudioClip設定変更")]
    public static void Open()
    {
        USndAudioAssetLoader window = CreateInstance<USndAudioAssetLoader>();
        window.titleContent = new GUIContent("AudioClip設定");
        window.Show();
    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.wordWrap = true;
        EditorGUILayout.LabelField("1.Resourcesフォルダ以下にあるAudioClipの情報をCSV形式で保存", style);
        if (GUILayout.Button("CSV作成", GUILayout.Height(50)))
        {
            GenerateCSV();
        }
        GUILayout.Space(10);
        EditorGUILayout.LabelField("2.CSVを読み込んでAudioClipのパラメータを変更", style);
        if (GUILayout.Button("AudioClip設定変更", GUILayout.Height(50)))
        {
            ChangeAudioClipSettings();
        }
    }

    // CSV読み込み、AudioClip設定変更
    private void ChangeAudioClipSettings()
    {
        if (importStatus != IMPORT_STATUS_WAIT)
        {
            Debug.Log("<color=red>CSV読み込み中のため実行できません。</color>");
        }
        try
        {
            // 読み込むCSVを指定
            string csvPath = EditorUtility.OpenFilePanel("読み込むCSVを指定", Application.dataPath, "csv");
            reader = new System.IO.StreamReader(csvPath);

            // ヘッダーをすてる
            reader.ReadLine();
            // コルーチン起動
            importStatus = IMPORT_STATUS_COROUTINE;
            loadCSV().MoveNext();

            Debug.Log("<color=yellow>CSV読み込み完了</color>");
        }
        catch (System.Exception e)
        {
            importStatus = IMPORT_STATUS_WAIT;
            Debug.Log(e.ToString());
            Debug.Log("<color=red>CSV読み込み失敗</color>");
        }
    }

    // CSV読み込み
    private IEnumerator loadCSV()
    {
        while (true)
        {
            while (importStatus == IMPORT_STATUS_PROCESS)
            {
                yield return null;
            }

            // 最後の行までいってたら終了
            if (reader.Peek() < 0)
            {
                reader.Close();
                yield break;
            }
            string line = reader.ReadLine();
            string[] param = line.Split(',');

            if (param[0] == null)
            {
                Debug.Log("<color=red>CSVの内容が不正です</color>");
                reader.Close();
                yield break;
            }

            AudioImporter importer = AssetImporter.GetAtPath(param[ASSET_PATH]) as AudioImporter;
            if (importer != null)
            {
                IMPORT_SETTINGS = importer.defaultSampleSettings;
                bool.TryParse(param[LOAD_IN_BACKGROUND], out IMPORT_LOAD_IN_BACKGROUND);
                uint.TryParse(param[SAMPLERATE_OVERRIDE], out IMPORT_SETTINGS.sampleRateOverride);
                float.TryParse(param[QUARITY], out IMPORT_SETTINGS.quality);

                //AudioCompressionFormat
                switch (param[COMPRESSION_FORMAT])
                {
                    case "PCM":
                        IMPORT_SETTINGS.compressionFormat = AudioCompressionFormat.PCM;
                        break;
                    case "Vorbis":
                        IMPORT_SETTINGS.compressionFormat = AudioCompressionFormat.Vorbis;
                        break;
                    case "ADPCM":
                        IMPORT_SETTINGS.compressionFormat = AudioCompressionFormat.ADPCM;
                        break;
                    default:
                        Debug.Log("<color=red>" + param[ASSET_PATH] + " " + param[COMPRESSION_FORMAT] + "は使用できないパラメータです</color>");
                        break;
                }

                //AudioClipLoadType
                switch (param[LOAD_TYPE])
                {
                    case "DecompressOnLoad":
                        IMPORT_SETTINGS.loadType = AudioClipLoadType.DecompressOnLoad;
                        break;
                    case "CompressedInMemory":
                        IMPORT_SETTINGS.loadType = AudioClipLoadType.CompressedInMemory;
                        break;
                    case "Streaming":
                        IMPORT_SETTINGS.loadType = AudioClipLoadType.Streaming;
                        break;
                    default:
                        Debug.Log("<color=red>" + param[ASSET_PATH] + " " + param[LOAD_TYPE] + "は使用できないパラメータです</color>");
                        break;
                }

                //AudioSampleRateSetting
                switch (param[SAMPLERATE_SETTINGS])
                {
                    case "PreserveSampleRate":
                        IMPORT_SETTINGS.sampleRateSetting = AudioSampleRateSetting.PreserveSampleRate;
                        break;
                    case "OptimizeSampleRate":
                        IMPORT_SETTINGS.sampleRateSetting = AudioSampleRateSetting.OptimizeSampleRate;
                        break;
                    case "OverrideSampleRate":
                        IMPORT_SETTINGS.sampleRateSetting = AudioSampleRateSetting.OverrideSampleRate;
                        break;
                    default:
                        Debug.Log("<color=red>" + param[ASSET_PATH] + " " + param[SAMPLERATE_SETTINGS] + "は使用できないパラメータです</color>");
                        break;
                }

                // CSVと差があるものだけ読み込む
                if (importer.loadInBackground == IMPORT_LOAD_IN_BACKGROUND &&
                    importer.defaultSampleSettings.loadType == IMPORT_SETTINGS.loadType &&
                    importer.defaultSampleSettings.quality == IMPORT_SETTINGS.quality &&
                    importer.defaultSampleSettings.compressionFormat == IMPORT_SETTINGS.compressionFormat &&
                    importer.defaultSampleSettings.sampleRateOverride == IMPORT_SETTINGS.sampleRateOverride &&
                    importer.defaultSampleSettings.sampleRateSetting == IMPORT_SETTINGS.sampleRateSetting)
                {
                    //Debug.Log(param[ASSET_PATH] + "はパラメータ変更がありません");
                }
                else
                {
                    Debug.Log(param[ASSET_PATH] + "の設定を更新");
                    // 更新
                    importStatus = IMPORT_STATUS_PROCESS;
                    AssetDatabase.ImportAsset(param[ASSET_PATH], ImportAssetOptions.ForceUpdate);
                }
            }
            else
            {
                Debug.Log("<color=red>" + param[ASSET_PATH] + "が見つかりません</color>");
            }
        }
    }


    // CSV生成
    private void GenerateCSV()
    {
        try
        { 

            string savePath = EditorUtility.SaveFilePanelInProject("CSV保存", "AudioClip設定", "csv", "CSV保存先を指定してください。");

            // 指定されたパスへCSV形式でファイルリストを保存

            // Resources以下のファイル一覧を取得
            string[] names = System.IO.Directory.GetFiles(Application.dataPath + "\\Resources", "*.wav", System.IO.SearchOption.AllDirectories);

            if (names.Length > 0)
            {
                System.Text.Encoding enc = System.Text.Encoding.GetEncoding("Shift_JIS");

                System.IO.StreamWriter sr = new System.IO.StreamWriter(savePath, false, enc);

                sr.Write("AssetPath");
                sr.Write(",");
                sr.Write("LoadInBackground");
                sr.Write(",");
                sr.Write("LoadType");
                sr.Write(",");
                sr.Write("CompressionFormat");
                sr.Write(",");
                sr.Write("Quality");
                sr.Write(",");
                sr.Write("SampleRateSettings");
                sr.Write(",");
                sr.Write("SampleRateOverride");
                sr.Write("\r\n");

                for (int i = 0; i < names.Length; ++i)
                {
                    // AssetDatabaseで読み込んで現在のパラメータを取得する
                    // パスはAssets/Resources/XXXX/XXXX の形になるのでAssetsより上のパスを削除

                    string assetPath = names[i].Replace(Application.dataPath, "Assets");

                    AudioImporter importer = AssetImporter.GetAtPath(assetPath) as AudioImporter;

                    sr.Write(assetPath);
                    sr.Write(",");
                    sr.Write(importer.loadInBackground);
                    sr.Write(",");
                    sr.Write(importer.defaultSampleSettings.loadType);
                    sr.Write(",");
                    sr.Write(importer.defaultSampleSettings.compressionFormat);
                    sr.Write(",");
                    sr.Write(importer.defaultSampleSettings.quality);
                    sr.Write(",");
                    sr.Write(importer.defaultSampleSettings.sampleRateSetting);
                    sr.Write(",");
                    sr.Write(importer.defaultSampleSettings.sampleRateOverride);
                    
                    sr.Write("\r\n");

                }

                sr.Close();
                Debug.Log("<color=yellow>CSV保存出力完了：" + savePath + "</color>");
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());

            Debug.Log("<color=red>CS保存失敗</color>");

        }

    }
    
    // AudioClipのインポート
    private class AudioImport : AssetPostprocessor
    {
        void OnPreprocessAudio()
        {
            if (importStatus == IMPORT_STATUS_PROCESS)
            {
                AudioImporter importer = assetImporter as AudioImporter;
                importer.defaultSampleSettings = IMPORT_SETTINGS;
                importer.loadInBackground = IMPORT_LOAD_IN_BACKGROUND;

                importStatus = IMPORT_STATUS_COROUTINE;
            }
        }
    }
    
}
