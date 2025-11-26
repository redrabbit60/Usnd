using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class CheckFileName {

    [MenuItem("Assets/Check FileName")]
    static void ErrorCheckFileName()
    {
        // Resources以下に別フォルダ内に同じファイル名が入っていないかチェックして出力する
        string path = "AssetBundles/" + EditorUserBuildSettings.activeBuildTarget.ToString();
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        path = path + "\\CheckFileName.txt";

        // pathに結果を出力
        string[] files = System.IO.Directory.GetFiles(@"Assets\Resources\", "*", System.IO.SearchOption.AllDirectories);
        List<string> sameFiles = new List<string>();
        List<string> orgFiles = new List<string>(files);
        bool addOrg = false;

        orgFiles.RemoveAll(ckeckGitKeep);

        for (int i = orgFiles.Count - 1; i >= 1; --i)
        {
            addOrg = false;
            string name01 = Path.GetFileName(orgFiles[i]);
            if ( !Path.GetExtension(orgFiles[i]).Contains("meta") )
            {
                for (int j = i-1; j >= 0; --j)
                {
                    if (!Path.GetExtension(orgFiles[j]).Contains("meta"))
                    {
                        string name02 = Path.GetFileName(orgFiles[j]);
                        if (name01.CompareTo(name02) == 0)
                        {
                            if (addOrg == false)
                            {
                                addOrg = true;
                                sameFiles.Add(orgFiles[i]);
                            }
                            Debug.Log("name:" + orgFiles[j] + " name01:" + name01 + " name02:" + name02);
                            sameFiles.Add(orgFiles[j]);
                            orgFiles.RemoveAt(j);
                            --i;
                        }
                    }
                }
                if (addOrg == true )
                {
                    orgFiles.RemoveAt(i);
                    sameFiles.Add("");
                }
            }
        }
        Debug.Log("end");
        WriteText(sameFiles.ToArray(), path);
    }

    static bool ckeckGitKeep(string s)
    {
        return Path.GetExtension(s).Contains("gitkeep");
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
