using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.QuestSystemPro.Editors.ReflectionDrawers
{
    public static class ObjectPickerUtility
    {
        public static ObjectPickerBase GetObjectPickerForType(Type objectType, Action<UnityEngine.Object> callback)
        {
            var editorType = GetObjectPickerTypeFor(objectType);

            var window = (ObjectPickerBase)EditorWindow.GetWindow(editorType, true);
            window.searchType = typeof(UnityEngine.Component).IsAssignableFrom(objectType) ? ObjectPickerBase.SearchType.Components : ObjectPickerBase.SearchType.ObjectTypes;
            window.foundObjects.Clear();
            window.callback = callback;
            window.type = objectType;
            window.allowInherited = true;
            window.wantsMouseMove = false;
            window.minSize = new Vector2(300, 300);
            window.Init();

            return window;
        }

        public static Type GetObjectPickerTypeFor(Type objectType)
        {
            var types = ReflectionUtility.GetAllClassesWithAttribute(typeof (CustomObjectPickerAttribute));

            types = types.OrderByDescending(o => ((CustomObjectPickerAttribute) o.GetCustomAttributes(typeof (CustomObjectPickerAttribute), true).First()).priority).ToArray();
            foreach (var type in types)
            {
                var attrib = (CustomObjectPickerAttribute)type.GetCustomAttributes(typeof (CustomObjectPickerAttribute), true).First();
                if (attrib.type.IsAssignableFrom(objectType))
                {
                    return type;
                }
            }

            QuestLogger.LogWarning("No object picker found for type: " + objectType.Name);
            return null;
        }

//        /// <summary>
//        /// Count how many 'step' a type is away. (number of inheritances -> Base steps required to get a type).
//        /// The closer the type, the lower the step count.
//        /// </summary>
//        private static int GetStepCountToType(Type starType, Type aim)
//        {
//            int count = 0;
//
//            Type tempType = starType;
//            while (tempType != null && tempType != aim)
//            {
//                tempType = tempType.BaseType;
//                count++;
//            }
//
//            return count;
//        }
    }
}
