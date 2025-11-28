using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SkySoundDesigner
{
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
                using (var fs = File.OpenRead(path))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    return (T)(serializer.Deserialize(fs) ?? new T());
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogWarning($"XML読み込みエラー: {path}\n{ex.Message}");
                return new T();
            }
        }

        public static void Save<T>(string path, T data)
        {
            try
            {
                string directory = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

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
                var namespaces = new XmlSerializerNamespaces();
                namespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");

                using (var fs = File.Create(path))
                using (var writer = XmlWriter.Create(fs, settings))
                {
                    serializer.Serialize(writer, data, namespaces);
                }
                
                UnityEngine.Debug.Log($"XML保存完了: {path}");
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"XML保存エラー: {path}\n{ex.Message}");
            }
        }
    }
}

