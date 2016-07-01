using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Devdog.QuestSystemPro.Dialogue;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.QuestSystemPro.Editors.ReflectionDrawers
{
    public class ArrayDrawer : DrawerBase
    {
        public List<DrawerBase> children = new List<DrawerBase>();
        private DerivedTypeInformation _derivedTypeInfo;
        private ArrayControlOptionsAttribute _arrayControlOptionsAttribute;
        public bool drawAddButton
        {
            get
            {
                if (_arrayControlOptionsAttribute == null)
                {
                    return true;
                }

                return _arrayControlOptionsAttribute.canAddItems;
            }
        }

        public bool drawRemoveButton
        {
            get
            {
                if (_arrayControlOptionsAttribute == null)
                {
                    return true;
                }

                return _arrayControlOptionsAttribute.canRemoveItems;
            }
        }

        public ArrayDrawer(FieldInfo fieldInfo, object value, object parentValue, int arrayIndex)
            : base(fieldInfo, value, parentValue, arrayIndex)
        {
            if (fieldInfo != null)
            {
                _arrayControlOptionsAttribute = (ArrayControlOptionsAttribute)fieldInfo.GetCustomAttributes(typeof(ArrayControlOptionsAttribute), true).FirstOrDefault();
            }

            Update();
        }

        protected override int GetHeightInternal()
        {
            int add = 0;
            if (drawAddButton)
            {
                add += ReflectionDrawerStyles.singleLineHeight;
            }

            if (hideGroup)
            {
                return children.Sum(o => o.GetHeight()) + add;
            }

            if (FoldoutBlockUtility.IsUnFolded(this))
            {
                add += ReflectionDrawerStyles.singleLineHeight;
                return children.Sum(o => o.GetHeight()) + add;
            }

            return add;
        }

        protected override object DrawInternal(Rect rect)
        {
            if (hideGroup)
            {
                DrawChildren(ref rect);
                if (drawAddButton)
                {
                    DrawArrayAddButton(rect);
                }
            }
            else
            {
                using (var foldout = new FoldoutBlock(this, rect, new GUIContent(fieldName)))
                {
                    rect.y += ReflectionDrawerStyles.singleLineHeight;
                    if (foldout.isUnFolded)
                    {
                        using (var indent = new IndentBlock(rect))
                        {
                            DrawChildren(ref indent.rect);
                            if (drawAddButton)
                            {
                                DrawArrayAddButton(indent.rect);
                            }
                        }
                    }
                }
            }

            return value;
        }

        private void DrawChildren(ref Rect rect)
        {
            if (drawRemoveButton)
            {
                rect.width -= 20f;
            }

            int elementRemoved = -1;
            for (int index = 0; index < children.Count; index++)
            {
                if (drawRemoveButton)
                {
                    var removed = DrawArrayRemoveButton(rect);
                    if (removed)
                    {
                        elementRemoved = index;
                        break;
                    }
                }

                children[index].Draw(ref rect);
            }

            if (elementRemoved != -1)
            {
                RemoveElementAt(elementRemoved);
                Update();
            }

            if (drawRemoveButton)
            {
                rect.width += 20f;
            }
        }

        private bool DrawArrayRemoveButton(Rect rect)
        {
            rect.x += rect.width + 4;
            rect.width = EditorGUIUtility.singleLineHeight;
            rect.height = EditorGUIUtility.singleLineHeight;

            return GUI.Button(rect, "X");
        }

        private void DrawArrayAddButton(Rect rect)
        {
            if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
            {
                UpdateDerivedTypesInfo();
            }

            rect.height = EditorGUIUtility.singleLineHeight;
            if (GUI.Button(rect, "Add type", "DropDownButton"))
            {
                var arrFieldType = fieldInfo.FieldType.GetElementType();
                if (onlyDerivedTypesAttribute != null)
                {
                    arrFieldType = onlyDerivedTypesAttribute.type;
                }

                rect.width -= 25;
                if (rect.Contains(Event.current.mousePosition) && arrFieldType.IsAbstract == false && arrFieldType.IsInterface == false)
                {
                    // Clicked left side
                    AddElement(arrFieldType);
                    Update();
                }
                else
                {
                    // Clicked right side (pick implementation)
                    var n = new GenericMenu();
                    for (int i = 0; i < _derivedTypeInfo.types.Length; i++)
                    {
                        n.AddItem(_derivedTypeInfo.content[i], false, (obj) =>
                        {
                            
                            AddElement((Type)obj);
                            Update();

                        }, _derivedTypeInfo.types[i]);
                    }

                    n.ShowAsContext();
                    Event.current.Use();
                }
            }
        }

        private void UpdateDerivedTypesInfo()
        {
            Type newType = fieldInfo.FieldType.GetElementType();
            if (onlyDerivedTypesAttribute == null)
            {
                if (newType.IsGenericType)
                {
                    newType = newType.GetGenericTypeDefinition();
                }
            }
            else
            {
                newType = onlyDerivedTypesAttribute.type;
            }


            _derivedTypeInfo = ReflectionDrawerUtility.GetDerivedTypesFrom(newType);
            if (newType.IsGenericType)
            {
                // Get standard things like string : TODO: Make this dynamic in the future, allowing users to select their own types.
                _derivedTypeInfo = new DerivedTypeInformation()
                {
                    types = new Type[]
                    {
                        newType.MakeGenericType(typeof (string)),
                        newType.MakeGenericType(typeof (bool)),
                        newType.MakeGenericType(typeof (int)),
                        newType.MakeGenericType(typeof (uint)),
                        newType.MakeGenericType(typeof (Vector2)),
                        newType.MakeGenericType(typeof (Vector3)),
                        newType.MakeGenericType(typeof (Vector4)),
                        newType.MakeGenericType(typeof (Quaternion)),
                        newType.MakeGenericType(typeof (UnityEngine.Object)),
                    }
                };

                _derivedTypeInfo.content = _derivedTypeInfo.types.Select(o => new GUIContent(GetGenericNiceName(o))).ToArray();
            }
        }

        private void AddElement(Type createOfType)
        {
            var arrayValue = (Array) value;

//            Debug.Log("Add array element");
            var newArray = Array.CreateInstance(fieldInfo.FieldType.GetElementType(), arrayValue.Length + 1);
            for (int j = 0; j < newArray.Length - 1; j++)
            {
                newArray.SetValue(arrayValue.GetValue(j), j);
            }

            object newValue = null;

            // Can't construct UnityEngine.Object values
            if (typeof (UnityEngine.Object).IsAssignableFrom(createOfType) == false)
            {
                newValue = Activator.CreateInstance(createOfType);
            }

            newArray.SetValue(newValue, newArray.Length - 1);
            value = newArray;

            NotifyValueChanged(value);
        }

        private void RemoveElementAt(int i)
        {
            var arrayValue = (Array) value;

//            Debug.Log("Removing array element " + i);
            var newArray = Array.CreateInstance(fieldInfo.FieldType.GetElementType(), arrayValue.Length - 1);
            for (int j = 0; j < i; j++)
            {
                newArray.SetValue(arrayValue.GetValue(j), j);
            }
            for (int j = i; j < arrayValue.Length - 1; j++)
            {
                newArray.SetValue(arrayValue.GetValue(j + 1), j); // Move items over
            }

            value = newArray;
            NotifyValueChanged(value);
        }

        public void Update()
        {
            children.Clear();
            
            if (value == null)
            {
                value = Array.CreateInstance(fieldInfo.FieldType.GetElementType(), 0);
                NotifyValueChanged(value);
            }

            var arrValue = (Array)value;
            for (int i = 0; i < arrValue.Length; i++)
            {
                var child = ReflectionDrawerUtility.BuildEditorHierarchy(fieldInfo, arrValue, i);
                children.Add(child);
            }

            GUI.changed = true;
        }
    }
}
