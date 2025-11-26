using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class CreateNameList : MonoBehaviour {

    static string MASTER_TABLE_LIST = "Assets/Resources/NameList/MasterName.txt";
    static string CATEGORY_TABLE_LIST = "Assets/Resources/NameList/CategoryName.txt";
    static string LABEL_TABLE_LIST = "Assets/Resources/NameList/LabelName.txt";

    static string MASTER_TABLE_LIST_BINARY = "Assets/Resources/NameList/MasterNameBinary.txt";
    static string CATEGORY_TABLE_LIST_BINARY = "Assets/Resources/NameList/CategoryNameBinary.txt";
    static string LABEL_TABLE_LIST_BINARY = "Assets/Resources/NameList/LabelNameBinary.txt";

    static string MASTER_TABLE_LIST_Json = "Assets/Resources/NameList/MasterNameJson.txt";
    static string CATEGORY_TABLE_LIST_Json = "Assets/Resources/NameList/CategoryNameJson.txt";
    static string LABEL_TABLE_LIST_Json = "Assets/Resources/NameList/LabelNameJson.txt";


    static string DIRECTORY_TABLE_LIST = "Assets/Resources/NameList/Directory.txt";
    
    static string LABEL_XML_PATH =  "Assets/Resources/LabelXML";
    static string LABEL_BINARY_PATH =  "Assets/Resources/LabelBinary";
    static string LABEL_Json_PATH = "Assets/Resources/LabelJson";

    static string MASTER_XML_PATH =  "Assets/Resources/MasterXML";
    static string MASTER_BINARY_PATH =  "Assets/Resources/MasterBinary";
    static string MASTER_Json_PATH = "Assets/Resources/MasterJson";

    static string CATEGORY_XML_PATH =  "Assets/Resources/CategoryXML";
    static string CATEGORY_BINARY_PATH =  "Assets/Resources/CategoryBinary";
    static string CATEGORY_Json_PATH = "Assets/Resources/CategoryJson";

    static string AUDIOCLIP_PATH = "Assets/Resources/AudioClip";


    [MenuItem("USnd/NameList更新")]
	public static void UpdateNameList()
    {
        // Label
        string[] guidXML = AssetDatabase.FindAssets("t:TextAsset", new[] { LABEL_XML_PATH });
        string[] guidBinary = AssetDatabase.FindAssets("t:TextAsset", new[] { LABEL_BINARY_PATH });
        string[] guidJson = AssetDatabase.FindAssets("t:TextAsset", new[] { LABEL_Json_PATH });
        WriteTextConvertGUID(guidXML, LABEL_TABLE_LIST);
        WriteTextConvertGUID(guidBinary, LABEL_TABLE_LIST_BINARY);
        WriteTextConvertGUID(guidJson, LABEL_TABLE_LIST_Json);

        // Master
        guidXML = AssetDatabase.FindAssets("t:TextAsset", new[] { MASTER_XML_PATH });
        guidBinary = AssetDatabase.FindAssets("t:TextAsset", new[] { MASTER_BINARY_PATH });
        guidJson = AssetDatabase.FindAssets("t:TextAsset", new[] { MASTER_Json_PATH });
        WriteTextConvertGUID(guidXML, MASTER_TABLE_LIST);
        WriteTextConvertGUID(guidBinary, MASTER_TABLE_LIST_BINARY);
        WriteTextConvertGUID(guidJson, MASTER_TABLE_LIST_Json);

        // Category
        guidXML = AssetDatabase.FindAssets("t:TextAsset", new[] { CATEGORY_XML_PATH });
        guidBinary = AssetDatabase.FindAssets("t:TextAsset", new[] { CATEGORY_BINARY_PATH });
        guidJson = AssetDatabase.FindAssets("t:TextAsset", new[] { CATEGORY_Json_PATH });
        WriteTextConvertGUID(guidXML, CATEGORY_TABLE_LIST);
        WriteTextConvertGUID(guidBinary, CATEGORY_TABLE_LIST_BINARY);
        WriteTextConvertGUID(guidJson, CATEGORY_TABLE_LIST_Json);

        // Directory
        List<string> dirList = new List<string>();
        GetSubDirectories(dirList, AUDIOCLIP_PATH, null);
        WriteText(dirList.ToArray(), DIRECTORY_TABLE_LIST);

        AssetDatabase.ImportAsset(MASTER_TABLE_LIST);
        AssetDatabase.ImportAsset(CATEGORY_TABLE_LIST);
        AssetDatabase.ImportAsset(LABEL_TABLE_LIST);
        AssetDatabase.ImportAsset(MASTER_TABLE_LIST_BINARY);
        AssetDatabase.ImportAsset(CATEGORY_TABLE_LIST_BINARY);
        AssetDatabase.ImportAsset(LABEL_TABLE_LIST_BINARY);
        AssetDatabase.ImportAsset(MASTER_TABLE_LIST_Json);
        AssetDatabase.ImportAsset(CATEGORY_TABLE_LIST_Json);
        AssetDatabase.ImportAsset(LABEL_TABLE_LIST_Json);
        AssetDatabase.ImportAsset(DIRECTORY_TABLE_LIST);


        Debug.Log("Complete");
    }


    static void GetSubDirectories(List<string> dir, string path, string addPath)
    {
        // Unity2017でなぜかフォルダ取得できなくなったのでSystem.IO.Directoryを使う
        string[] tmp = Directory.GetDirectories(path);
        //string[] tmp = AssetDatabase.GetSubFolders(path);
        string newAddPath = "";
        string dirName;
        for (int i = 0; i < tmp.Length; ++i)
        {
            dirName = Path.GetFileName(tmp[i]);
            if ( addPath != null )
            {
                newAddPath = addPath + "/" + dirName;
            }
            else
            {
                newAddPath = dirName;                
            }
            dir.Add(newAddPath);

            GetSubDirectories(dir, path + "/" + dirName, newAddPath);
        }
    }


    static void WriteTextConvertGUID(string[] guidList, string savePath)
    {
        StreamWriter sw = new StreamWriter(savePath, false);
        for (int i = 0; i < guidList.Length; ++i)
        {
            sw.WriteLine(Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(guidList[i])));
        }

        sw.Flush();
        sw.Close();
    }

    static void WriteText(string[] str, string savePath)
    {
        StreamWriter sw = new StreamWriter(savePath, false);
        for (int i = 0; i < str.Length; ++i)
        {
            sw.WriteLine(str[i]);
        }

        sw.Flush();
        sw.Close();
    }

}
