using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Data;
using UnityEditor;
public class AssetUtil
{
    private static string assetFolderPath = "Assets/Resources/";

    public static void CreateAsset(string assetName, Object assetData)
    {
        //确保文件夹存在
        if (!Directory.Exists(assetFolderPath))
        {
            Directory.CreateDirectory(assetFolderPath);
        }

        //asset文件的路径 要以"Assets/..."开始，否则CreateAsset会报错
        string assetPath = string.Format("{0}{1}.asset", assetFolderPath, assetName);
        //生成一个Asset文件
        AssetDatabase.CreateAsset(assetData, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
