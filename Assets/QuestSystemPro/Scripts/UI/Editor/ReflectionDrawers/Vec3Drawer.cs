using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Devdog.QuestSystemPro.Editors.ReflectionDrawers
{
    public sealed class Vec3Drawer : DrawerBase
    {
        public Vec3Drawer(FieldInfo fieldInfo, object value, object parentValue, int arrayIndex)
            : base(fieldInfo, value, parentValue, arrayIndex)
        {

        }

        protected override int GetHeightInternal()
        {
            return ReflectionDrawerStyles.singleLineHeight * 2;
        }

        protected override object DrawInternal(Rect rect)
        {
            return EditorGUI.Vector3Field(rect, fieldName, (Vector3)value);
        }
    }
}
