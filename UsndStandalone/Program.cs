using UsndStandalone;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var baseDir = AppContext.BaseDirectory;
var xmlDir = Path.Combine(baseDir, "..", "..", "..", "xml");

string masterPath = Path.GetFullPath(Path.Combine(xmlDir, "MasterSettings.xml"));
string categoryPath = Path.GetFullPath(Path.Combine(xmlDir, "CategorySettings.xml"));
string labelPath = Path.GetFullPath(Path.Combine(xmlDir, "LabelSettings.xml"));

var master = XmlStore.Load<MasterSettings>(masterPath);
var category = XmlStore.Load<CategorySettings>(categoryPath);
var label = XmlStore.Load<LabelSettings>(labelPath);

using var player = new AudioPlayer();

while (true)
{
    Console.WriteLine("==== USnd Standalone Tool ====");
    Console.WriteLine("XML ディレクトリ: " + xmlDir);
    Console.WriteLine("1) ラベル一覧表示");
    Console.WriteLine("2) ラベル再生");
    Console.WriteLine("3) ラベルの音量を編集");
    Console.WriteLine("4) XML を保存");
    Console.WriteLine("5) 再読込");
    Console.WriteLine("0) 終了");
    Console.Write("選択: ");

    var input = Console.ReadLine();
    if (input == "0")
    {
        break;
    }

    switch (input)
    {
        case "1":
            ShowLabels(label);
            break;
        case "2":
            PlayLabel(label, player, baseDir);
            break;
        case "3":
            EditLabelVolume(label);
            break;
        case "4":
            XmlStore.Save(masterPath, master);
            XmlStore.Save(categoryPath, category);
            XmlStore.Save(labelPath, label);
            Console.WriteLine("XML を保存しました。");
            break;
        case "5":
            master = XmlStore.Load<MasterSettings>(masterPath);
            category = XmlStore.Load<CategorySettings>(categoryPath);
            label = XmlStore.Load<LabelSettings>(labelPath);
            Console.WriteLine("XML を再読込しました。");
            break;
        default:
            Console.WriteLine("不明なコマンドです。");
            break;
    }

    Console.WriteLine();
}

static void ShowLabels(LabelSettings label)
{
    if (label.Items.Count == 0)
    {
        Console.WriteLine("ラベルがありません。LabelSettings.xml を確認してください。");
        return;
    }

    for (int i = 0; i < label.Items.Count; i++)
    {
        var l = label.Items[i];
        Console.WriteLine($"{i}: {l.LabelName}  file={l.FileName}  vol={l.Volume}");
    }
}

static void PlayLabel(LabelSettings label, AudioPlayer player, string baseDir)
{
    if (label.Items.Count == 0)
    {
        Console.WriteLine("ラベルがありません。");
        return;
    }

    ShowLabels(label);
    Console.Write("再生したいラベル番号: ");
    if (!int.TryParse(Console.ReadLine(), out int index))
    {
        Console.WriteLine("番号が不正です。");
        return;
    }

    if (index < 0 || index >= label.Items.Count)
    {
        Console.WriteLine("範囲外の番号です。");
        return;
    }

    var l = label.Items[index];
    if (string.IsNullOrEmpty(l.FileName))
    {
        Console.WriteLine("FileName が設定されていません。");
        return;
    }

    // WAV などの実ファイルパスは、プロジェクトの任意のルールに合わせて調整してください。
    // ここでは exe と同じディレクトリ、またはその下の \"Audio\" フォルダにある想定にします。
    var candidatePaths = new[]
    {
        Path.Combine(baseDir, l.FileName),
        Path.Combine(baseDir, "Audio", l.FileName)
    };

    var file = candidatePaths.FirstOrDefault(File.Exists);
    if (file == null)
    {
        Console.WriteLine("音声ファイルが見つかりません。検索したパス:");
        foreach (var c in candidatePaths)
        {
            Console.WriteLine("  " + c);
        }
        return;
    }

    bool loop = string.Equals(l.Loop, "true", StringComparison.OrdinalIgnoreCase) ||
                l.Loop == "1";

    Console.WriteLine($"再生: {file}  (loop={loop})");
    player.Play(file, loop);
    Console.WriteLine("停止するには Enter を押してください。");
    Console.ReadLine();
    player.Stop();
}

static void EditLabelVolume(LabelSettings label)
{
    if (label.Items.Count == 0)
    {
        Console.WriteLine("ラベルがありません。");
        return;
    }

    ShowLabels(label);
    Console.Write("編集したいラベル番号: ");
    if (!int.TryParse(Console.ReadLine(), out int index))
    {
        Console.WriteLine("番号が不正です。");
        return;
    }

    if (index < 0 || index >= label.Items.Count)
    {
        Console.WriteLine("範囲外の番号です。");
        return;
    }

    var l = label.Items[index];
    Console.WriteLine($"現在の Volume: {l.Volume}");
    Console.Write("新しい Volume 値を入力 (例: 0.5) : ");
    var v = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(v))
    {
        Console.WriteLine("変更をキャンセルしました。");
        return;
    }

    l.Volume = v.Trim();
    Console.WriteLine("Volume を更新しました。XML保存(メニュー4)を実行してください。");
}




