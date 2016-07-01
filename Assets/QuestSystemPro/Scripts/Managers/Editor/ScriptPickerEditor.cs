using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Devdog.QuestSystemPro.Editors;
using UnityEditor;
using UnityEngine;

namespace Devdog.QuestSystemPro.Editors
{
    public class ScriptPickerEditor : ObjectPickerBaseEditor<Type>
    {
        private static Type _type;
        private static Type[] _ignoreTypes = new Type[0];
        public static ScriptPickerEditor Get(System.Type type, params System.Type[] ignoreTypes)
        {
            _type = type;
            _ignoreTypes = ignoreTypes;

            var window = GetWindow<ScriptPickerEditor>(true);
            window.windowTitle = "Script type picker";
            window.isUtility = true;

            return window;
        }

        protected override IList<Type> FindObjects(bool searchProjectFolder)
        {
            var types = new List<Type>(16);
            foreach (var script in Resources.FindObjectsOfTypeAll<MonoScript>())
            {
                if (script == null)
                {
                    continue;
                }

                var c = script.GetClass();
                if (c != null &&
                    (c.IsSubclassOf(_type) || c == _type || _type.IsAssignableFrom(c)) &&
                    c.IsAbstract == false &&
                    _ignoreTypes.Contains(c) == false)
                {
                    types.Add(c);
                }
            }

            return types;
        }

        protected override bool MatchesSearch(Type obj, string search)
        {
            return obj.Name.ToLower().Contains(search);
        }

        protected override void DrawObjectButton(Type item)
        {
            if (GUILayout.Button(item.Name))
                NotifyPickedObject(item);
        }
    }
}
