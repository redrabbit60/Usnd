using UnityEditor;
using System.IO;

public class CreateAssetBundles
{
    [MenuItem ("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles ()
    {
        string path = "AssetBundles/" + EditorUserBuildSettings.activeBuildTarget.ToString();
        if ( !Directory.Exists(path) )
        {
            Directory.CreateDirectory(path);
        }
        BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
    }
}
