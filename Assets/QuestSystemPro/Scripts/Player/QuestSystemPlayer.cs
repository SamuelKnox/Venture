using System;
using System.Collections;
using System.Collections.Generic;
using Devdog.QuestSystemPro.Dialogue;
using UnityEngine;

namespace Devdog.QuestSystemPro
{
    public class QuestSystemPlayer : MonoBehaviour
    {
        public DialogueCamera dialogueCamera;
        public IPlayerTriggerHandler triggerHandler { get; set; }


        protected virtual void Awake()
        {
            SetTriggerHandler();
            QuestManager.instance.currentPlayer = this;
        }

        protected virtual void Start()
        {
            
        }

        protected virtual void SetTriggerHandler()
        {
            var obj = new GameObject("_TriggerHandler");
            obj.transform.SetParent(transform);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;
            obj.gameObject.layer = 2; // Ignore raycasts

            var handler = obj.AddComponent<PlayerTriggerHandler>();
            handler.player = this;
            handler.sphereCollider.radius = QuestManager.instance.settingsDatabase.objectUseDistance;

            triggerHandler = handler;
        }
    }
}