using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Devdog.QuestSystemPro
{
    [RequireComponent(typeof(SphereCollider))]
    public class QuestProgressTrigger : MonoBehaviour
    {
//        [Required]
        public Quest quest;
        public string task = "Main";
        public float addProgress = 1f;

        public AudioClipInfo triggerEnterAudio = new AudioClipInfo();

        private bool _rewarded = false;
        protected virtual void Awake()
        {
            var col = GetComponent<SphereCollider>();
            col.isTrigger = true;
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (_rewarded)
            {
                return;
            }

            if (other.gameObject.GetComponentInParent<QuestSystemPlayer>())
            {
                AudioManager.AudioPlayOneShot(triggerEnterAudio);
                quest.ChangeTaskProgress(task, addProgress);
                _rewarded = true;
            }
        }
    }
}
