#if SALSA

using System;
using System.Collections.Generic;
using CrazyMinnow.SALSA;
using CrazyMinnow.SALSA.Examples;
using Devdog.QuestSystemPro.Dialogue;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.QuestSystemPro.Integration.SALSA
{
    public class DialogueOwnerSalsaBridge : MonoBehaviour
    {
        [SerializeField]
        [Required]
        private Salsa3D _salsa;

        private IDialogueOwner _owner;
        protected virtual void Awake()
        {
            _owner = GetComponent<IDialogueOwner>();
            Assert.IsNotNull(_owner, "No IDialogueOwner found!");
        }

        protected virtual void Start()
        {
            _owner.dialogue.OnCurrentNodeChanged += DialogueOnCurrentNodeChanged;
        }

        protected void DialogueOnCurrentNodeChanged(NodeBase before, NodeBase after)
        {
            if (before.ownerType == DialogueOwnerType.DialogueOwner && _salsa.isTalking)
            {
                // Stop previous node's talking.
                _salsa.Stop();
            }

            if (after.ownerType == DialogueOwnerType.DialogueOwner)
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