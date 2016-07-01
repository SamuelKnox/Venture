using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Devdog.QuestSystemPro.ThirdParty.FullSerializer;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.QuestSystemPro
{
    public class fsUnityEngineObjectConverter : fsConverter
    {

        private bool IsAssetWrapper(Type type)
        {
            if (typeof(IAsset).IsAssignableFrom(type))
            {
                return true;
            }

            return false;
        }

        public override object CreateInstance(fsData data, Type storageType)
        {
//            var db = Serializer.Context.Get<List<UnityEngine.Object>>();
//            if (IsAssetWrapper(storageType))
//            {
//                var t = (IAsset)Activator.CreateInstance(storageType);
//                t.objectVal = db[(int)data.AsInt64];
//                return t;
//            }
//
//            return db[(int)data.AsInt64];
            return null;
        }

        public override bool CanProcess(Type type)
        {
            if (IsAssetWrapper(type))
            {
                return true;
            }

            if (ReflectionUtility.IsBuiltInUnityObjectType(type))
            {
                return true;
            }

            return false;
        }

        public override bool RequestCycleSupport(Type storageType)
        {
            return false;
        }

        public override bool RequestInheritanceSupport(Type storageType)
        {
            return false;
        }

        protected virtual UnityEngine.Object GetUnityEngineObject(object instance)
        {
            UnityEngine.Object unityObject = null;
            var iAsset = instance as IAsset;
            if (iAsset != null)
            {
                unityObject = iAsset.objectVal;
            }

            if (iAsset == null)
            {
                unityObject = instance as UnityEngine.Object;
            }

            return unityObject;
        }

        public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
        {
            var db = Serializer.Context.Get<List<UnityEngine.Object>>();

            var unityObject = GetUnityEngineObject(instance);
            int index = db.IndexOf(unityObject);
            if (index == -1)
            {
                index = db.Count; // Insert at the end of the list
                db.Add(unityObject);
            }

            serialized = new fsData((long)index);
            return fsResult.Success;
        }

        public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
        {
            try
            {
                var db = Serializer.Context.Get<List<UnityEngine.Object>>();
                if (data.IsNull == false)
                {
                    int index = (int)data.AsInt64;
                    if (index == -1 || index >= db.Count)
                    {
                        QuestLogger.LogError("Couldn't deserialize UnityEngine.Object : " + instance + " - not found in database. (index: " + index + ")");
                        return fsResult.Fail("Index out of range " + index);
                    }

                    if (IsAssetWrapper(storageType))
                    {
                        var def = typeof(Asset<>);
                        var t = def.MakeGenericType(storageType.GetGenericArguments()[0]);

                        var inst = (IAsset)Activator.CreateInstance(t);
                        inst.objectVal = db[index];
                        instance = inst;
                    }
                    else if (ReflectionUtility.IsBuiltInUnityObjectType(storageType))
                    {
                        instance = db[index];
                    }
                }
                else
                {
                    instance = null;
                }
            }
            catch (Exception e)
            {
                QuestLogger.LogError(e.Message + "\n" + e.StackTrace);
                return fsResult.Fail(e.Message);
            }

            return fsResult.Success;
        }
    }
}
