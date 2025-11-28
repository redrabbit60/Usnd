using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SkySoundDesigner
{
    /// <summary>
    /// XML設定データの管理
    /// WPFのMainViewModelに相当
    /// </summary>
    public class DataManager : MonoBehaviour
    {
        private string xmlDirectory = "XML";
        private string masterPath;
        private string categoryPath;
        private string labelPath;
        
        public MasterSettings MasterSettings { get; private set; }
        public CategorySettings CategorySettings { get; private set; }
        public LabelSettings LabelSettings { get; private set; }
        
        public List<LabelGroupData> LabelGroups { get; private set; } = new List<LabelGroupData>();
        
        private void Awake()
        {
            InitializePaths();
        }
        
        private void InitializePaths()
        {
            // Unityの場合、Application.dataPathまたはpersistentDataPathを使用
            string baseDir = Application.dataPath;
            xmlDirectory = Path.Combine(baseDir, "Resources", "XML");
            
            // ディレクトリが存在しない場合は作成
            if (!Directory.Exists(xmlDirectory))
            {
                Directory.CreateDirectory(xmlDirectory);
                Debug.Log($"XMLディレクトリを作成: {xmlDirectory}");
            }
            
            masterPath = Path.Combine(xmlDirectory, "MasterSettings.xml");
            categoryPath = Path.Combine(xmlDirectory, "CategorySettings.xml");
            labelPath = Path.Combine(xmlDirectory, "LabelSettings.xml");
        }
        
        /// <summary>
        /// XML設定をロード
        /// </summary>
        public void LoadXml()
        {
            MasterSettings = XmlStore.Load<MasterSettings>(masterPath);
            CategorySettings = XmlStore.Load<CategorySettings>(categoryPath);
            LabelSettings = XmlStore.Load<LabelSettings>(labelPath);
            
            // デフォルト値を設定
            if (MasterSettings.Items == null || MasterSettings.Items.Count == 0)
            {
                MasterSettings.Items = new List<MasterSet>
                {
                    new MasterSet { MasterName = "Master", Volume = "1" }
                };
            }
            
            if (CategorySettings.Items == null || CategorySettings.Items.Count == 0)
            {
                CategorySettings.Items = new List<CategorySet>
                {
                    new CategorySet { CategoryName = "BGM", Volume = "1", MaxNum = "1", MasterName = "Master" },
                    new CategorySet { CategoryName = "SE", Volume = "1", MaxNum = "8", MasterName = "Master" }
                };
            }
            
            if (LabelSettings.Items == null || LabelSettings.Items.Count == 0)
            {
                LabelSettings.Items = new List<LabelSet>();
            }
            
            // Label Groupsを構築
            BuildLabelGroups();
            
            Debug.Log($"XML読み込み完了: Master={MasterSettings.Items.Count}, Category={CategorySettings.Items.Count}, Label={LabelSettings.Items.Count}");
        }
        
        /// <summary>
        /// XML設定を保存
        /// </summary>
        public void SaveXml()
        {
            XmlStore.Save(masterPath, MasterSettings);
            XmlStore.Save(categoryPath, CategorySettings);
            XmlStore.Save(labelPath, LabelSettings);
            
            Debug.Log("XML保存完了");
        }
        
        /// <summary>
        /// Label Groupsを構築（カテゴリごとにグループ化）
        /// </summary>
        private void BuildLabelGroups()
        {
            LabelGroups.Clear();
            
            // カテゴリごとにラベルをグループ化
            var groupedLabels = LabelSettings.Items
                .GroupBy(l => string.IsNullOrWhiteSpace(l.CategoryName) ? "Non" : l.CategoryName);
            
            foreach (var group in groupedLabels)
            {
                var groupData = new LabelGroupData
                {
                    GroupName = group.Key,
                    LabelNames = group.Select(l => l.LabelName).ToList()
                };
                LabelGroups.Add(groupData);
            }
        }
        
        /// <summary>
        /// 新しいLabelを追加
        /// </summary>
        public LabelSet AddLabel(string labelName, string categoryName = "SE")
        {
            var newLabel = new LabelSet
            {
                LabelName = labelName,
                FileName = "",
                CategoryName = categoryName,
                Loop = "FALSE",
                Volume = "1",
                Priority = "64"
            };
            
            LabelSettings.Items.Add(newLabel);
            BuildLabelGroups();
            
            return newLabel;
        }
        
        /// <summary>
        /// Labelを削除
        /// </summary>
        public void RemoveLabel(string labelName)
        {
            var label = LabelSettings.Items.FirstOrDefault(l => l.LabelName == labelName);
            if (label != null)
            {
                LabelSettings.Items.Remove(label);
                BuildLabelGroups();
            }
        }
        
        /// <summary>
        /// Labelを名前で取得
        /// </summary>
        public LabelSet GetLabel(string labelName)
        {
            return LabelSettings.Items.FirstOrDefault(l => l.LabelName == labelName);
        }
        
        /// <summary>
        /// カテゴリに属するLabelを取得
        /// </summary>
        public List<LabelSet> GetLabelsByCategory(string categoryName)
        {
            return LabelSettings.Items
                .Where(l => l.CategoryName == categoryName)
                .ToList();
        }
        
        /// <summary>
        /// 文字列をfloatにパース
        /// </summary>
        public static float ParseFloat(string value, float defaultValue)
        {
            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;
            
            if (float.TryParse(value, out float result))
                return result;
            
            return defaultValue;
        }
        
        /// <summary>
        /// 文字列をintにパース
        /// </summary>
        public static int ParseInt(string value, int defaultValue)
        {
            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;
            
            if (int.TryParse(value, out int result))
                return result;
            
            return defaultValue;
        }
        
        /// <summary>
        /// 文字列をboolにパース
        /// </summary>
        public static bool ParseBool(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;
            
            return value.Equals("TRUE", System.StringComparison.OrdinalIgnoreCase) ||
                   value.Equals("true", System.StringComparison.OrdinalIgnoreCase);
        }
    }
}

