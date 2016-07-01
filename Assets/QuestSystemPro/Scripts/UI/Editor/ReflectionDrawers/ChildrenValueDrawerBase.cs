using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Devdog.QuestSystemPro.Editors.ReflectionDrawers
{
    public abstract class ChildrenValueDrawerBase : DrawerBase
    {
        public List<DrawerBase> children = new List<DrawerBase>();
        private bool _hideTypePicker = false;

        protected ChildrenValueDrawerBase(FieldInfo fieldInfo, object value, object parentValue, int arrayIndex)
            : base(fieldInfo, value, parentValue, arrayIndex)
        {
            if (fieldInfo != null)
            {
                _hideTypePicker = fieldInfo.GetCustomAttributes(typeof(HideTypePicker), true).Length > 0; // Allow by default, unless otherwise specified
            }

            Update();
        }

        protected override int GetHeightInternal()
        {
            if (hideGroup)
            {
                return children.Sum(o => o.GetHeight());
            }

            if (FoldoutBlockUtility.IsUnFolded(this))
            {
                return children.Sum(o => o.GetHeight()) + ReflectionDrawerStyles.singleLineHeight;
            }

            return ReflectionDrawerStyles.singleLineHeight;
        }

        protected override object DrawInternal(Rect rect)
        {
            if (_hideTypePicker == false)
            {
                DrawTypeSelecter(rect);
            }
            DrawChildren(rect);

            return value;
        }

        protected virtual void DrawChildren(Rect rect)
        {
            if (hideGroup)
            {
                foreach (var child in children)
                {
                    child.Draw(ref rect);
                }
            }
            else
            {
                var r2 = rect;
                r2.width = EditorGUIUtility.labelWidth;
                r2.x += 13;

                EditorGUI.LabelField(r2, GetFoldoutName());
                using (var foldout = new FoldoutBlock(this, new Rect(rect.x, rect.y, rect.width, ReflectionDrawerStyles.singleLineHeight), GUIContent.none))
                {
                    rect.y += ReflectionDrawerStyles.singleLineHeight;

                    using (var indent = new IndentBlock(rect))
                    {
                        if (foldout.isUnFolded)
                        {
                            foreach (var child in children)
                            {
                                child.Draw(ref indent.rect);
                            }
                        }
                    }
                }
            }
        }

        private GUIContent GetFoldoutName()
        {
            string childValueName = "";
            if (children.Count > 0 && children[0] is StringDrawer)
            {
                if (children[0].value != null)
                {
                    childValueName = children[0].value.ToString();
                }
            }

            if (string.IsNullOrEmpty(childValueName) == false)
            {
                return new GUIContent(childValueName);
            }


            return fieldName;
        }

        protected virtual void DrawTypeSelecter(Rect rect)
        {
            var t = GetFieldType(true);
            if (t.IsValueType == false)
            {
                var r = rect;
                r.width -= EditorGUIUtility.labelWidth + 15;
                r.x += EditorGUIUtility.labelWidth;
                r.x += 15; // Indentation

                if (required && value == null)
                {
                    GUI.color = Color.red;
                }

                GUI.Button(r, new GUIContent(value == null ? "(empty)" : GetFieldTypeName(false)), "MiniPopup");
                if (r.Contains(Event.current.mousePosition) && Event.current.type == EventType.Used)
                {
                    var implementableTypes = ReflectionDrawerUtility.GetDerivedTypesFrom(GetFieldType(true));

                    var n = new GenericMenu();
                    for (int i = 0; i < implementableTypes.types.Length; i++)
                    {
                        int index = i;
                        n.AddItem(implementableTypes.content[i], false, (obj) =>
                        {

                            ChangeImplementation(implementableTypes.types[index]);
                            Update();

                        }, implementableTypes.types[i]);
                    }

                    n.ShowAsContext();
                    Event.current.Use();
                }

                GUI.color = Color.white;
            }
        }

        private void ChangeImplementation(Type type)
        {
            // TODO: Try to copy values from the old object to the new object
            var oldValue = value;
            value = Activator.CreateInstance(type);
            if (oldValue != null)
            {
                var oldValueFields = new List<FieldInfo>();
                ReflectionUtility.GetAllSerializableFieldsInherited(oldValue.GetType(), oldValueFields);

                var newValueFields = new List<FieldInfo>();
                ReflectionUtility.GetAllSerializableFieldsInherited(value.GetType(), newValueFields);

                foreach (var oldValueField in oldValueFields)
                {
                    var newValueField = newValueFields.FirstOrDefault(o => o.Name == oldValueField.Name);
                    if (newValueField != null)
                    {
                        if (oldValueField.FieldType.IsAssignableFrom(newValueField.FieldType))
                        {
                            // Copy value to new object.
                            newValueField.SetValue(value, oldValueField.GetValue(oldValue));
                        }
                    }
                }
            }

            NotifyValueChanged(value);
        }

        public void Update()
        {
            children.Clear();
            var fieldType = GetFieldType(false);

            if (typeof (UnityEngine.Object).IsAssignableFrom(fieldType))
            {
                return;
            }

            var list = new List<FieldInfo>();
            ReflectionUtility.GetAllSerializableFieldsInherited(GetFieldType(false), list);

            if (value == null)
            {
                // Check if we can create a new object of this type
                // TODO: Move to utility, can be reused - Utility.CanCreateNewObjectOfType
                if ((fieldType.IsClass && fieldType.IsAbstract == false) ||
                    fieldType.IsValueType)
                {
                    value = Activator.CreateInstance(fieldType);
                    NotifyValueChanged(value);
                }
            }

            foreach (var field in list)
            {
                var child = ReflectionDrawerUtility.BuildEditorHierarchy(field, value);
                children.Add(child);
            }

            GUI.changed = true;
        }
    }
}
