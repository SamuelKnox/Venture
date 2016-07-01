using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Devdog.QuestSystemPro.Editors.ReflectionDrawers
{
    public class InterfaceDrawer : ChildrenValueDrawerBase
    {
        public InterfaceDrawer(FieldInfo fieldInfo, object value, object parentValue, int arrayIndex)
            : base(fieldInfo, value, parentValue, arrayIndex)
        {

        }


        protected override void DrawChildren(Rect rect)
        {
            if (children.Count > 0)
            {
                base.DrawChildren(rect);
            }
        }
    }
}
