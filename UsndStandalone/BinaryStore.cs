using System.Text.Json;
using System.IO;

namespace UsndStandalone;

public static class BinaryStore
{
    private static readonly string SaveDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SaveData");

    public static void Save(string fileName, SaveData data)
    {
        Directory.CreateDirectory(SaveDir);
        var path = Path.Combine(SaveDir, fileName);
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json);
    }

    public static SaveData Load(string fileName)
    {
        var path = Path.Combine(SaveDir, fileName);
        if (!File.Exists(path))
        {
            return new SaveData
            {
                Master = new MasterSettings(),
                Category = new CategorySettings(),
                Labels = new LabelSettings(),
                LabelGroups = new List<LabelGroupData>()
            };
        }

        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<SaveData>(json) ?? new SaveData
        {
            Master = new MasterSettings(),
            Category = new CategorySettings(),
            Labels = new LabelSettings(),
            LabelGroups = new List<LabelGroupData>()
        };
    }

    public static List<string> GetSaveFileList()
    {
        if (!Directory.Exists(SaveDir))
        {
            return new List<string>();
        }

        return Directory.GetFiles(SaveDir, "*.usnd")
            .Select(Path.GetFileName)
            .Where(f => f != null)
            .Cast<string>()
            .ToList();
    }

    public static void Delete(string fileName)
    {
        var path = Path.Combine(SaveDir, fileName);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}

public class SaveData
{
    public MasterSettings Master { get; set; } = new();
    public CategorySettings Category { get; set; } = new();
    public LabelSettings Labels { get; set; } = new();
    public List<LabelGroupData> LabelGroups { get; set; } = new();
}
