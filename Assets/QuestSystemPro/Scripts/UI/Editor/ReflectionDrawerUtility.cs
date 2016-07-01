using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.QuestSystemPro.Editors.ReflectionDrawers
{
    public static class ReflectionDrawerUtility
    {
        private static Dictionary<Type, CustomDrawerAttribute> _customDrawers;
        private static Dictionary<Type, DerivedTypeInformation> _derivedTypesCache = new Dictionary<Type, DerivedTypeInformation>();
//        private static Type[] _customDrawerClasses;

        static ReflectionDrawerUtility()
        {
            _customDrawers = new Dictionary<Type, CustomDrawerAttribute>();
            var customDrawerClasses = ReflectionUtility.GetAllClassesWithAttribute(typeof(CustomDrawerAttribute));
            foreach (var c in customDrawerClasses)
            {
                _customDrawers[c] = (CustomDrawerAttribute)c.GetCustomAttributes(typeof (CustomDrawerAttribute), true).FirstOrDefault();
            }
        }


        public static DrawerBase BuildEditorHierarchy(FieldInfo field, object value, int index = -1)
        {
            var fieldType = field.FieldType;
            object fieldValue = null;

            object parentValue = value;
            if (index >= 0)
            {
                // Is array
                Assert.IsTrue(value.GetType().IsArray, "Index given but value is not an array!!");

                fieldValue = ((Array) value).GetValue(index);
                if (fieldValue == null)
                {
                    // Emtpy array element, get type from array
                    fieldType = value.GetType().GetElementType();
                }
                else
                {
                    fieldType = fieldValue.GetType();
                }
            }
            else
            {
                // Is not an array
                if (value != null)
                {
                    fieldValue = field.GetValue(value);
                }
            }



            if (fieldType == typeof(string))
            {
                return new StringDrawer(field, fieldValue, parentValue, index);
            }
            if (fieldType == typeof(bool))
            {
                return new BoolDrawer(field, fieldValue, parentValue, index);
            }
            if (fieldType == typeof(float))
            {
                return new FloatDrawer(field, fieldValue, parentValue, index);
            }
            if (fieldType == typeof(int))
            {
                return new IntDrawer(field, fieldValue, parentValue, index);
            }
            if (fieldType == typeof(uint))
            {
                return new UintDrawer(field, fieldValue, parentValue, index);
            }
            if (fieldType == typeof(Vector2))
            {
                return new Vec2Drawer(field, fieldValue, parentValue, index);
            }
            if (fieldType == typeof(Vector3))
            {
                return new Vec3Drawer(field, fieldValue, parentValue, index);
            }
            if (fieldType == typeof(Vector4))
            {
                return new Vec4Drawer(field, fieldValue, parentValue, index);
            }
            if (fieldType == typeof(UnityEngine.Color))
            {
                return new ColorDrawer(field, fieldValue, parentValue, index);
            }
            if (fieldType.IsEnum)
            {
                return new EnumDrawer(field, fieldValue, parentValue, index);
            }
            if (fieldType == typeof(UnityEngine.AnimationCurve))
            {
                return new AnimationCurveDrawer(field, fieldValue, parentValue, index);
            }
            if (typeof (IDictionary).IsAssignableFrom(fieldType))
            {
                return new DictionaryDrawer(field, fieldValue, parentValue, index);
            }

            // Did all default types, check for custom drawers.
            var customDrawer = TryGetCustomDrawer(field, fieldValue, parentValue, index, fieldType);
            if (customDrawer != null)
            {
                return customDrawer;
            }

            
            if (typeof(UnityEngine.Object).IsAssignableFrom(fieldType))
            {
                return new UnityObjectDrawer(field, fieldValue, parentValue, index);
            }
            if (fieldType.IsArray)
            {
                return new ArrayDrawer(field, fieldValue, parentValue, index);
            }
            if (fieldType.IsInterface || (value.GetType().IsArray && value.GetType().GetElementType().IsInterface))
            {
                return new InterfaceDrawer(field, fieldValue, parentValue, index);
            }
            if (fieldType.IsClass)
            {
                return new ClassDrawer(field, fieldValue, parentValue, index);
            }
            if (fieldType.IsValueType)
            {
                return new ValueTypeDrawer(field, fieldValue, parentValue, index);
            }

            Debug.LogWarning("Could not create editor for field type: " + field.FieldType);
            return null;
        }

        public static DrawerBase TryGetCustomDrawer(FieldInfo field, object fieldValue, object parentValue, int index, Type fieldType)
        {
            Type drawerType = null;
            foreach (var c in _customDrawers)
            {
                if (c.Value.onlyForRoot && field != null)
                {
                    continue;
                }

                if (c.Value.type.IsAssignableFrom(fieldType))
                {
                    drawerType = c.Key;
                    break;
                }
                
                if (c.Value.type.IsGenericType &&
                    fieldType.IsGenericType && 
                    c.Value.type.GetGenericArguments()[0] == fieldType)
                {
                    drawerType = c.Key;
                    break;
                }
            }

            if (drawerType == null)
            {
                // If no specific drawer has been found try a 'wide' generic one Type<> (without specific type).
                foreach (var c in _customDrawers)
                {
                    if (c.Value.onlyForRoot && field != null)
                    {
                        continue;
                    }

                    if (c.Value.type.IsGenericType &&
                        c.Value.type.GetGenericArguments()[0].AssemblyQualifiedName == null &&
                        fieldType.IsGenericType)
                    {
                        var a = c.Value.type.GetGenericTypeDefinition();
                        var b = fieldType.GetGenericTypeDefinition();
                        if (a == b)
                        {
                            // Generic for all types (Type<>)
                            drawerType = c.Key;
                            break;
                        }
                    }
                }
            }


            if (drawerType == null)
            {
                return null;
            }

            // Grab the constructor.
            var constructor = drawerType.GetConstructor(
                   BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance,
                   null,
                   new[] { typeof(FieldInfo), typeof(object), typeof(object), typeof(int) },
                   null
               );

            // Create new instance of this custom editor.
            return (DrawerBase)constructor.Invoke(new object[] { field, fieldValue, parentValue, index });
        }

        public static DerivedTypeInformation GetDerivedTypesFrom(Type fromType)
        {
            if (_derivedTypesCache.ContainsKey(fromType) == false)
            {
                var implementable = new DerivedTypeInformation();

                implementable.types = ReflectionUtility.GetAllTypesThatImplement(fromType);
                implementable.content = implementable.types.Select(o => new GUIContent(GetPopupNameForType(o))).ToArray();

                _derivedTypesCache[fromType] = implementable;
                return implementable;
            }

            return _derivedTypesCache[fromType];
        }


        public static string GetPopupNameForType(Type type)
        {
            if (type.Namespace == null)
            {
                return type.Name;
            }

            string name = "";
            foreach (var s in type.Namespace.Split('.'))
            {
                name += s + "/";
            }

            name += type.Name;
            return name;
        }
    }
}
