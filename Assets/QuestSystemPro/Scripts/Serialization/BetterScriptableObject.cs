using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.QuestSystemPro.ThirdParty.FullSerializer;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Devdog.QuestSystemPro
{
    public class BetterScriptableObject : ScriptableObject, ISerializationCallbackReceiver
    {
        /// <summary>
        /// Used to store the state of this scriptable object. 
        /// Fetched through reflection to avoid people messing with it.
        /// </summary>
        [SerializeField]
        //        [InspectorReadOnly]
        //        [HideInInspector]
        [fsIgnore] // Ignore in custom serializer - Let unity seriarlize this.
        private string _serializedJsonString = "{}";


        [SerializeField]
        [fsIgnore] // Ignore in custom serializer - Let unity seriarlize this.
        private List<UnityEngine.Object> _objectReferences;

        [NonSerialized]
        private bool _isSerializing = false;

        public virtual void Save()
        {
            if (_isSerializing || Application.isPlaying)
                return;

            _isSerializing = true;
            _objectReferences = new List<UnityEngine.Object>(); // Has to be new list, ref type -> Clear will clear it inside the serializer
            _serializedJsonString = JsonSerializer.Serialize(GetType(), this, _objectReferences);
//            QuestLogger.LogVerbose("Saved scriptable object - Object references count: " + _objectReferences.Count);

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif

            _isSerializing = false;
        }

        public virtual void Load()
        {
            if (_isSerializing)
                return;

            _isSerializing = true;

            JsonSerializer.DeserializeTo(this, GetType(), _serializedJsonString ?? "{}", _objectReferences);
//            QuestLogger.LogVerbose("Loaded scriptable object - Object references count: " + _objectReferences.Count);

            _isSerializing = false;
        }

        public void OnBeforeSerialize()
        {
            Save();
        }

        public void OnAfterDeserialize()
        {
            Load();
        }
    }
}
