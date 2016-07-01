using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Devdog.QuestSystemPro.Editors.ReflectionDrawers
{
    public abstract class SimpleValueDrawer : DrawerBase
    {
        protected SimpleValueDrawer(FieldInfo fieldInfo, object value, object parentValue, int arrayIndex)
            : base(fieldInfo, value, parentValue, arrayIndex)
        {

        }

        protected override int GetHeightInternal()
        {
            return ReflectionDrawerStyles.singleLineHeight;
        }
    }
}
