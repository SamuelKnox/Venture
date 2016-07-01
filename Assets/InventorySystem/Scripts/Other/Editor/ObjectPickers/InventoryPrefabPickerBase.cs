using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Devdog.InventorySystem.Editors
{
    public abstract class InventoryPrefabPickerBase<T> : InventoryObjectPickerBase<T> where T : UnityEngine.Object
    {
        public override void Show(bool useless)
        {
            Show(FindObjects(true));
        }
        
        protected override IList<T> FindObjects(bool searchProjectFolder)
        {
            return GetAssetsOfType(".prefab");
        }

        private static T[] GetAssetsOfType(string fileExtension)
        {
            var tempObjects = new List<T>();
            var directory = new DirectoryInfo(Application.dataPath);
            var goFileInfo = directory.GetFiles("*" + fileExtension, SearchOption.AllDirectories);

            int i = 0;
            int goFileInfoLength = goFileInfo.Length;

            FileInfo tempGoFileInfo;
            string tempFilePath;
            Object tempGO = null;

            for (; i < goFileInfoLength; i++)
            {
                tempGoFileInfo = goFileInfo[i];
                if (tempGoFileInfo == null)
                    continue;

                tempFilePath = tempGoFileInfo.FullName;
                tempFilePath = tempFilePath.Replace(@"\", "/").Replace(Application.dataPath, "Assets");

                tempGO = AssetDatabase.LoadAssetAtPath(tempFilePath, typeof(Object)) as Object;
                if (tempGO != null && tempGO is T)
                {
                    tempObjects.Add(tempGO as T);
                    continue;
                }

                var gameObject = tempGO as GameObject;
                if (gameObject != null)
                {
                    var comp = gameObject.GetComponent<T>();
                    if (comp != null)
                    {
                        tempObjects.Add(comp);
                    }
                }
            }

            return tempObjects.ToArray();
        }


        protected override bool MatchesSearch(T obj, string search)
        {
            return obj.name.ToLower().Contains(search);
        }


        protected override void DrawObjectButton(T item)
        {
            if (GUILayout.Button(item.name + " - " + AssetDatabase.GetAssetPath(item)))
            {
                NotifyPickedObject(item);
            }
        }
    }
}
