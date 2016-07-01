#if PLY_GAME

using System.Collections.Generic;
using System.IO;
using System.Linq;
using plyGame;
using UnityEditor;
using UnityEngine;

namespace Devdog.InventorySystem.Editors
{
    public class InventoryPlySkillPrefabPickerBase : InventoryPrefabPickerBase<Skill>
    {
        public static InventoryPlySkillPrefabPickerBase Get(string title = "Item picker", Vector2 minSize = new Vector2())
        {
            var window = GetWindow<InventoryPlySkillPrefabPickerBase>(true);
            window.windowTitle = title;
            window.minSize = minSize;
//            window.isUtility = true;

            return window;
        }

        protected override IList<Skill> FindObjects(bool searchProjectFolder)
        {
            return base.FindObjects(searchProjectFolder);
        }

        protected override void DrawObjectButton(Skill item)
        {
            if (GUILayout.Button(item.ToString()))
            {
                NotifyPickedObject(item);
            }
        }
    }
}

#endif