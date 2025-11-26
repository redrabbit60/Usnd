using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Text;

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
        
        // XML Writer設定: インデント有効、タブ文字使用、UTF-8エンコーディング
        var settings = new XmlWriterSettings
        {
            Indent = true,
            IndentChars = "\t",
            Encoding = new UTF8Encoding(false), // BOMなし
            OmitXmlDeclaration = false
        };

        // xmlns:xsi名前空間を追加
        var namespaces = new System.Xml.Serialization.XmlSerializerNamespaces();
        namespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");

        using var fs = File.Create(path);
        using var writer = XmlWriter.Create(fs, settings);
        serializer.Serialize(writer, data, namespaces);
    }
}



