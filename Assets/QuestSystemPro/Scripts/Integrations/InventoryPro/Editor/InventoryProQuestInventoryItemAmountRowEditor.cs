#if INVENTORY_PRO

using System;
using System.Collections.Generic;
using System.Reflection;
using Devdog.QuestSystemPro.Editors.ReflectionDrawers;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.QuestSystemPro.Integration.InventoryPro.Editor
{
    [CustomDrawer(typeof(InventoryProQuestInventoryItemAmountRow))]
    public class InventoryProQuestInventoryItemAmountRowEditor : ChildrenValueDrawerBase
    {
        public InventoryProQuestInventoryItemAmountRowEditor(FieldInfo fieldInfo, object value, object parentValue, int arrayIndex)
            : base(fieldInfo, value, parentValue, arrayIndex)
        {
        }

        protected override int GetHeightInternal()
        {
            return ReflectionDrawerStyles.singleLineHeight;
        }

        protected override object DrawInternal(Rect rect)
        {
            Assert.AreEqual(2, children.Count);

            var r = rect;
            r.width *= 0.7f;

            var r2 = rect;
            r2.x += r.width;
            r2.width *= 0.3f;

            var before = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 50;

            children[0].overrideFieldName = GUIContent.none;
            children[0].Draw(ref r);

            if (((uint) children[1].value) <= 0)
            {
                children[1].NotifyValueChanged(1u);
            }

            children[1].overrideFieldName = GUIContent.none;
            children[1].Draw(ref r2);

            EditorGUIUtility.labelWidth = before;

            return value;
        }
    }
}

#endif