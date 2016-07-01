using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Devdog.QuestSystemPro.Dialogue;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.QuestSystemPro.Editors.ReflectionDrawers
{
    public abstract class DrawerBase : IEquatable<DrawerBase>
    {
        public object value { get; set; }
        public object parentValue { get; protected set; }
        public FieldInfo fieldInfo { get; protected set; }


        public bool hideInProperties { get; protected set; }
        public bool showInNode { get; protected set; }
        public bool readOnly { get; protected set; }
        public bool required { get; set; }

        public HeaderAttribute headerAttribute { get; protected set; }
        public HideGroupAttribute hideGroupAttribute { get; protected set; }
        public OnlyDerivedTypesAttribute onlyDerivedTypesAttribute { get; protected set; }

        public bool hideGroup
        {
            get
            {
                if (hideGroupAttribute == null)
                {
                    return false;
                }

                if (isInArray)
                {
                    return hideGroupAttribute.includeArrayChildren;
                }

                return true;
            }
        }


        public virtual GUIContent overrideFieldName { get; set; }
        public virtual GUIContent fieldName
        {
            get
            {
                if (overrideFieldName != null)
                {
                    return overrideFieldName;
                }

                string lengthText = "";
                var arr = value as Array;
                if (arr != null)
                {
                    lengthText = " : " + arr.Length;
                }

                if (isInArray)
                {
                    return new GUIContent("Element " + arrayIndex);
                }

                if (fieldInfo == null)
                {
                    return new GUIContent(value.GetType().Name + lengthText);
                }

                return new GUIContent(fieldInfo.Name + lengthText);
            }
        }


        public bool isInArray
        {
            get { return arrayIndex >= 0; }
        }
        public int arrayIndex { get; protected set; }

        protected DrawerBase(FieldInfo fieldInfo, object value, object parentValue, int arrayIndex)
        {
            this.fieldInfo = fieldInfo;
            this.value = value;
            this.parentValue = parentValue;
            this.arrayIndex = arrayIndex;

            if (fieldInfo != null)
            {
                hideInProperties = fieldInfo.GetCustomAttributes(typeof(HideInPropertiesAttribute), true).Length > 0;
                showInNode = fieldInfo.GetCustomAttributes(typeof(ShowInNodeAttribute), true).Length > 0;
                readOnly = fieldInfo.GetCustomAttributes(typeof(InspectorReadOnlyAttribute), true).Length > 0;
                required = fieldInfo.GetCustomAttributes(typeof(RequiredAttribute), true).Length > 0;
                headerAttribute = (HeaderAttribute)fieldInfo.GetCustomAttributes(typeof(HeaderAttribute), true).FirstOrDefault();
                hideGroupAttribute = (HideGroupAttribute)fieldInfo.GetCustomAttributes(typeof(HideGroupAttribute), true).FirstOrDefault();
                onlyDerivedTypesAttribute = (OnlyDerivedTypesAttribute)fieldInfo.GetCustomAttributes(typeof(OnlyDerivedTypesAttribute), true).FirstOrDefault();
            }
        }


        public static bool operator ==(DrawerBase left, DrawerBase right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DrawerBase left, DrawerBase right)
        {
            return !Equals(left, right);
        }

        public string GetFieldTypeName(bool getLowestType)
        {
            var fieldType = GetFieldType(getLowestType);
            if (fieldType.IsGenericType)
            {
                if (isInArray)
                {
                    var arr = (Array)parentValue;
                    var v = arr.GetValue(arrayIndex);

                    // Couldn't determine type, going with array's type.
                    if (v == null || getLowestType)
                    {
                        return GetGenericNiceName(fieldType);
                    }

                    if (v.GetType().IsGenericType)
                    {
                        return GetGenericNiceName(v.GetType());
                    }

                    return v.GetType().Name;
                }

                return GetGenericNiceName(fieldType);
            }

            return fieldType.Name;
        }

        protected string GetGenericNiceName(Type type)
        {
            string[] names = type.GetGenericArguments().Select(o => o.Name).ToArray();
            return type.GetGenericTypeDefinition().Name + "<" + string.Join(",", names) + ">";
        }

        public Type GetFieldType(bool getLowestType)
        {
            if (isInArray)
            {
                var arr = (Array)parentValue;
                var v = arr.GetValue(arrayIndex);

                // Couldn't determine type, going with array's type.
                if (v == null || getLowestType)
                {
                    return fieldInfo.FieldType.GetElementType();
                }

                return v.GetType();
            }

//            if (onlyDerivedTypesAttribute != null)
//            {
//                return onlyDerivedTypesAttribute.type;
//            }

            if ((getLowestType || value == null) && fieldInfo != null)
            {
                return fieldInfo.FieldType;
            }

            return value.GetType();
        }

        protected Rect GetSingleLineHeightRect(Rect rect)
        {
            rect.height = EditorGUIUtility.singleLineHeight;
            rect.y += (ReflectionDrawerStyles.singleLineHeight - rect.height) / 2;

            return rect;
        }

        protected int GetExtraHeight()
        {
            if (headerAttribute != null && isInArray == false)
            {
                return ReflectionDrawerStyles.headerSize;
            }

            return 0;
        }

        public int GetHeight()
        {
            return GetHeightInternal() + GetExtraHeight();
        }

        protected abstract int GetHeightInternal();

        public void Draw(ref Rect rect)
        {
            GUIDisabledBlock block = null;
            if (readOnly)
            {
                block = new GUIDisabledBlock();
            }
            else
            {
                EditorGUI.BeginChangeCheck();
            }


            if (headerAttribute != null && isInArray == false)
            {
                GUI.color = new Color(1, 1, 1, 0.85f);

                rect.y += 12;
                EditorGUI.LabelField(rect, headerAttribute.header, UnityEditor.EditorStyles.boldLabel);
                rect.y += ReflectionDrawerStyles.headerSize - 12;

                GUI.color = Color.white;
            }

            var val = DrawInternal(rect);
            rect.y += GetHeightInternal();

            if (readOnly && block != null)
            {
                block.Dispose();
            }
            else
            {
                if (EditorGUI.EndChangeCheck())
                {
                    NotifyValueChanged(val);
                }
            }
        }

        protected abstract object DrawInternal(Rect rect);

        public virtual void NotifyValueChanged(object newValue)
        {
            if (isInArray)
            {
                var arr = (Array) parentValue;
                arr.SetValue(newValue, arrayIndex);
            }
            else
            {
                if (fieldInfo != null)
                {
                    fieldInfo.SetValue(parentValue, newValue);
                }
            }

            this.value = newValue;
        }

        public bool Equals(DrawerBase other)
        {
            if (other == null)
            {
                return false;
            }

            return ReferenceEquals(this.value, other.value) && this.arrayIndex == other.arrayIndex;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DrawerBase);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((fieldInfo != null ? fieldInfo.GetHashCode() : 0) * 397) ^ arrayIndex;
            }
        }
    }
}
