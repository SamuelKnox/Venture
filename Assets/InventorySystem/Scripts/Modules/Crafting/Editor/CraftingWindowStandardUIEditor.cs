using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace Devdog.InventorySystem.Editors
{
    [CustomEditor(typeof(CraftingWindowStandardUI), true)]
    public class CraftingWindowStandardUIEditor : InventoryEditorBase
    {
        //private CraftingStation item;
        private SerializedProperty _startCraftingCategoryID;

        public override void OnEnable()
        {
            base.OnEnable();

            _startCraftingCategoryID = serializedObject.FindProperty("_startCraftingCategoryID");
        }

        protected override void OnCustomInspectorGUI(params CustomOverrideProperty[] extraSpecific)
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(script);

            GUILayout.Label("Behavior", InventoryEditorStyles.titleStyle);
            var cat = InventoryEditorUtility.PopupField("Crafting category",
                ItemManager.database.craftingCategoriesStrings, ItemManager.database.craftingCategories,
                o => o.ID == _startCraftingCategoryID.intValue);
            if (cat != null)
            {
                _startCraftingCategoryID.intValue = cat.ID;
            }

            DrawPropertiesExcluding(serializedObject, new string[]
            {
                "m_Script",
                "_startCraftingCategoryID",
            });

            serializedObject.ApplyModifiedProperties();
        }
    }
}