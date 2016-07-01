using System;
using System.Collections;
using System.Collections.Generic;
using Devdog.QuestSystemPro.Dialogue;
using UnityEngine;

namespace Devdog.QuestSystemPro
{
    public class PlayerTriggerHandler : MonoBehaviour, IPlayerTriggerHandler
    {
        [NonSerialized]
        private readonly List<ObjectTriggerBase> _triggerersInRange = new List<ObjectTriggerBase>();

        public QuestSystemPlayer player { get; set; }
        public SphereCollider sphereCollider { get; set; }
        public new Rigidbody rigidbody { get; set; }


        protected void Awake()
        {
            rigidbody = GetOrAddComponent<Rigidbody>();
            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;

            sphereCollider = GetOrAddComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
        }

        private T GetOrAddComponent<T>() where T : Component
        {
            var a = gameObject.GetComponent<T>();
            if (a != null)
            {
                return a;
            }

            return gameObject.AddComponent<T>();
        }

        public bool IsInRangeOfTrigger(ObjectTriggerBase trigger)
        {
            return _triggerersInRange.Contains(trigger);
        }

        protected void OnTriggerEnter(Collider other)
        {
            var c = other.GetComponentInChildren<ObjectTriggerBase>();
            if (c != null)
            {
                c.NotifyCameInRange(player);
                _triggerersInRange.Add(c);
            }
        }

        protected void OnTriggerExit(Collider other)
        {
            var c = other.GetComponentInChildren<ObjectTriggerBase>();
            if (c != null)
            {
                c.NotifyWentOutOfRange(player);
                _triggerersInRange.Remove(c);
            }
        }
    }
}