using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System;

namespace Devdog.QuestSystemPro.Editors
{
    public static class ScriptableObjectUtility
    {

        public static T CreateAsset<T>(string path, string fileName) where T : ScriptableObject
        {
            return (T)CreateAsset(typeof (T), path, fileName);
        }

        public static ScriptableObject CreateAsset(Type type, string savePath, string fileName)
        {
            if (savePath == string.Empty || Directory.Exists(savePath) == false)
            {
                Debug.LogWarning("The directory you're trying to save to doesn't exist! (" + savePath + ")");
                return null;
            }

            var asset = ScriptableObject.CreateInstance(type);
            if (fileName.EndsWith(".asset") == false)
            {
                fileName += ".asset";
            }

            AssetDatabase.CreateAsset(asset, savePath + "/" + fileName + ".asset");
            AssetDatabase.SetLabels(asset, new string[] { type.Name });
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return asset;
        }
    }
}