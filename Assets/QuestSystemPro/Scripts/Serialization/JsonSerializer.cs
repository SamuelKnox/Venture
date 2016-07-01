using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Devdog.QuestSystemPro.ThirdParty.FullSerializer;

namespace Devdog.QuestSystemPro
{
    public static class JsonSerializer
    {
        private static readonly fsSerializer _serializer;
        private static readonly object _lockObject = new object();
        private static readonly fsUnityEngineObjectConverter _engineObjectConvertor;

        static JsonSerializer()
        {
            _serializer = new fsSerializer();
            _serializer.Config = new fsConfig() { SerializeEnumsAsInteger = true };
            _engineObjectConvertor = new fsUnityEngineObjectConverter();
            _serializer.AddConverter(_engineObjectConvertor);
        }

        private static void SetObjectReferences(List<UnityEngine.Object> objectReferences)
        {
            if (objectReferences != null)
            {
                _serializer.Context.Set(objectReferences);
            }
        }


        public static string Serialize<T>(T value, List<UnityEngine.Object> objectReferences)
        {
            return Serialize(typeof (T), value, objectReferences);
        }

        public static string Serialize(Type type, object value, List<UnityEngine.Object> objectReferences)
        {
            lock (_lockObject)
            {
                try
                {
                    fsData data;

                    SetObjectReferences(objectReferences);
                    _serializer.TrySerialize(type, value, out data).AssertSuccessWithoutWarnings();

                    return fsJsonPrinter.CompressedJson(data);
                }
                catch (Exception e)
                {
                    Debug.LogError("Couldn't serialize type " + type + " - " + e.Message);
//                    throw;
                }
            }

            return null;
        }

//        public static T Deserialize<T>(string json, List<UnityEngine.Object> objectReferences)
//        {
//            return (T)Deserialize(typeof(T), json, objectReferences);
//        }
//
//        public static object Deserialize(Type type, string json, List<UnityEngine.Object> objectReferences)
//        {
//            lock (_lockObject)
//            {
//                try
//                {
//                    fsData data = fsJsonParser.Parse(json);
//
//                    object deserialized = null;
//                    SetObjectReferences(objectReferences);
//                    _serializer.TryDeserialize(data, type, ref deserialized).AssertSuccessWithoutWarnings();
//
//                    return deserialized;
//                }
//                catch (Exception e)
//                {
//                    Debug.LogError("Couldn't deserialize type " + type + " - " + e.Message);
////                    throw;
//                }
//            }
//
//            return null;
//        }

        public static void DeserializeTo(object obj, Type type, string json, List<UnityEngine.Object> objectReferences)
        {
            DeserializeTo(ref obj, type, json, objectReferences);
        }

        public static void DeserializeTo(ref object obj, Type type, string json, List<UnityEngine.Object> objectReferences)
        {
            lock (_lockObject)
            {
                try
                {
                    fsData data = fsJsonParser.Parse(json);
                    SetObjectReferences(objectReferences);
                    _serializer.TryDeserialize(data, type, ref obj).AssertSuccessWithoutWarnings();
                }
                catch (Exception e)
                {
                    QuestLogger.LogError(e.Message + "\n" + e.StackTrace);
//                    throw;
                }
            }
        }

        public static void DeserializeTo<T>(T obj, string json, List<UnityEngine.Object> objectReferences)
        {
            DeserializeTo<T>(ref obj, json, objectReferences);
        }

        public static void DeserializeTo<T>(ref T obj, string json, List<UnityEngine.Object> objectReferences)
        {
            lock (_lockObject)
            {
                try
                {
                    fsData data = fsJsonParser.Parse(json);
                    SetObjectReferences(objectReferences);
                    _serializer.TryDeserialize<T>(data, ref obj).AssertSuccessWithoutWarnings();
                }
                catch (Exception e)
                {
                    Debug.LogError("Couldn't deserialize type " + typeof(T) + " - " + e.Message);
//                    throw;
                }
            }
        }
    }
}