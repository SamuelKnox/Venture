#if SALSA

using System;
using System.Collections.Generic;
using CrazyMinnow.SALSA;
using Devdog.QuestSystemPro.Dialogue;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.QuestSystemPro.Integration.SALSA
{
    public class QuestSystemPlayerSalsaBridge : MonoBehaviour
    {
        [SerializeField]
        [Required]
        private Salsa3D _salsa;

        private QuestSystemPlayer _player;
        protected virtual void Awake()
        {
            _player = GetComponent<QuestSystemPlayer>();
            Assert.IsNotNull(_player, "No IDialogueOwner found!");
        }

        protected virtual void Start()
        {
            DialogueManager.instance.OnCurrentDialogueNodeChanged += DialogueOnCurrentNodeChanged;
        }

        protected void DialogueOnCurrentNodeChanged(NodeBase before, NodeBase after)
        {
            if (before.ownerType == DialogueOwnerType.Player && _salsa.isTalking)
            {
                // Stop previous node's talking.
                _salsa.Stop();
            }

            if (after.ownerType == DialogueOwnerType.Player)
            {
                if (after.audioClip.audioClip != null)
                {
                    SalsaPlayAudioClip(after.audioClip);
                }
            }
        }

        public virtual void SalsaPlayAudioClip(AudioClipInfo audioClip)
        {
            _salsa.SetAudioClip(audioClip.audioClip);
            _salsa.Play();
        }
    }
}

#endif