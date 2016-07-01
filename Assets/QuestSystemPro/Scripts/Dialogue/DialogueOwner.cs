using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using Devdog.QuestSystemPro.Dialogue.UI;
using UnityEngine.Assertions;

namespace Devdog.QuestSystemPro.Dialogue
{
    public class DialogueOwner : MonoBehaviour, IDialogueOwner
    {
        [SerializeField]
        [Required]
        private Dialogue _dialogue;
        public Dialogue dialogue
        {
            get { return _dialogue; }
            set { _dialogue = value; }
        }

        [Header("Owner details")]
        [SerializeField]
        private string _ownerName;
        public string ownerName
        {
            get { return _ownerName; }
            set { _ownerName = value; }
        }

        [SerializeField]
        private Sprite _ownerIcon;
        public Sprite ownerIcon
        {
            get { return _ownerIcon; }
            set { _ownerIcon = value; }
        }

        [SerializeField]
        private DialogueCamera _dialogueCamera;
        public DialogueCamera dialogueCamera
        {
            get { return _dialogueCamera; }
            set { _dialogueCamera = value; }
        }


        public bool playAudio = true;
        public bool playAnimations = true;


        private ObjectTriggerBase _trigger;
        private AudioSource _audioSource;
        private Animator _animator;

        protected virtual void Awake()
        {
            _trigger = GetComponent<ObjectTriggerBase>();
            _audioSource = GetComponent<AudioSource>();
            _animator = GetComponent<Animator>();
        }

        protected virtual void Start()
        {
            dialogue.OnStatusChanged += DialogueOnStatusChanged;
            dialogue.OnCurrentNodeChanged += DialogueOnCurrentNodeChanged;

            if (_trigger != null)
            {
                _trigger.OnTriggerUsed += OnTriggerUsed;
                _trigger.OnTriggerUnUsed += OnTriggerUnUsed;
            }
        }

        protected virtual void DialogueOnStatusChanged(DialogueStatus before, DialogueStatus after, Dialogue self, IDialogueOwner owner)
        {
            if (after == DialogueStatus.InActive && _trigger != null)
            {
                _trigger.UnUse();
            }
        }

        protected virtual void DialogueOnCurrentNodeChanged(NodeBase before, NodeBase after)
        {
            if (after.ownerType == DialogueOwnerType.DialogueOwner)
            {
                if (playAudio && _audioSource != null && after.audioClip.audioClip != null)
                {
                    _audioSource.clip = after.audioClip.audioClip;
                    _audioSource.volume = after.audioClip.volume;
                    _audioSource.pitch = after.audioClip.pitch;
                    _audioSource.loop = after.audioClip.loop;
                    _audioSource.Play();
                }

                if (playAnimations && _animator != null && after.animationClip != null)
                {
                    _animator.Play(after.animationClip.name);
                }
            }
        }

        protected virtual void OnMouseDown()
        {
            // Manual activation if there's no trigger.
            if (_trigger == null)
            {
                if (UIUtility.isHoveringUIElement == false)
                {
                    Use();
                }
            }
        }

        private void OnTriggerUsed(QuestSystemPlayer player)
        {
            Use();
        }

        private void OnTriggerUnUsed(QuestSystemPlayer player)
        {
            UnUse();
        }

        public virtual void Use()
        {
            if (dialogue != null)
            {
                dialogue.StartDialogue(this);
            }
        }

        public virtual void UnUse()
        {
            if (dialogue != null)
            {
                dialogue.Stop();
            }
        }
    }
}