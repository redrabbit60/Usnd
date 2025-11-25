using System.Xml.Serialization;
using System.IO;

namespace UsndStandalone;

public static class XmlStore
{
    public static T Load<T>(string path) where T : class, new()
    {
        if (!File.Exists(path))
        {
            return new T();
        }

        try
        {
            using var fs = File.OpenRead(path);
            var serializer = new XmlSerializer(typeof(T));
            return (T)(serializer.Deserialize(fs) ?? new T());
        }
        catch
        {
            // 読み込めない場合は空のオブジェクトを返す
            return new T();
        }
    }

    public static void Save<T>(string path, T data)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path) ?? ".");

        var serializer = new XmlSerializer(typeof(T));
        using var fs = File.Create(path);
        serializer.Serialize(fs, data);
    }
}



