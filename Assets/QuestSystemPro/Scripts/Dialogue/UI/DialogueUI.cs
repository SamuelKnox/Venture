using System;
using UnityEngine;
using Devdog.QuestSystemPro.UI;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Devdog.QuestSystemPro.Dialogue.UI
{
    public class DialogueUI : MonoBehaviour
    {
        [Header("Settings")]
        public bool hideDialogueOwnerIconOnPlayerNode = true;

        [Header("Audio & Visuals")]
        public Image dialogueOwnerImage;
        public Text dialogueOwnerName;

        [Header("Misc")]
        public RectTransform nodeUIContainer;

        public Dialogue currentDialogue
        {
            get { return DialogueManager.instance.currentDialogue; }
        }

        public UIWindow window { get; protected set; }
        protected NodeUIBase currentNodeUI;


        protected virtual void Awake()
        {
            window = GetComponent<UIWindow>();
        }

        protected virtual void Start()
        {
            window.OnHide += WindowOnHide;
            DialogueManager.instance.OnCurrentDialogueStatusChanged += OnCurrentDialogueStatusChanged;
            DialogueManager.instance.OnCurrentDialogueNodeChanged += OnCurrentDialogueNodeChanged;
        }

        protected virtual void WindowOnHide()
        {
            if(currentDialogue != null)
            {
                currentDialogue.Stop();
            }
        }

        protected virtual void OnCurrentDialogueNodeChanged(NodeBase before, NodeBase after)
        {
            if (currentNodeUI != null)
            {
                Destroy(currentNodeUI.gameObject);
            }

            if (after.uiPrefab == null)
            {
                return;
            }

            if (after.ownerType != DialogueOwnerType.DialogueOwner)
            {
                SetDialogueOwnerIcon(hideDialogueOwnerIconOnPlayerNode ? null : QuestManager.instance.settingsDatabase.playerDialogueIcon);
            }
            else
            {
                if (DialogueManager.instance.currentDialogueOwner != null)
                {
                    SetDialogueOwnerIcon(QuestManager.instance.settingsDatabase.playerDialogueIcon);
                    
                    if (dialogueOwnerName != null)
                    {
                        dialogueOwnerName.text = DialogueManager.instance.currentDialogueOwner.ownerName;
                    }
                }
            }

            currentNodeUI = UnityEngine.Object.Instantiate<NodeUIBase>(after.uiPrefab);
            currentNodeUI.transform.SetParent(nodeUIContainer);
            UIUtility.ResetTransform(currentNodeUI.transform);
            UIUtility.InheritParentSize(currentNodeUI.transform);

            currentNodeUI.Repaint(after);
        }

        protected void SetDialogueOwnerIcon(Sprite sprite)
        {
            if (dialogueOwnerImage != null)
            {
                if (sprite == null)
                {
                    dialogueOwnerImage.gameObject.SetActive(false);
                }
                else
                {
                    dialogueOwnerImage.gameObject.SetActive(true);
                    dialogueOwnerImage.sprite = sprite;
                }
            }
        }

        protected virtual void OnCurrentDialogueStatusChanged(DialogueStatus before, DialogueStatus after, Dialogue self, IDialogueOwner owner)
        {
            switch (after)
            {
                case DialogueStatus.InActive:
                    window.Hide();
                    break;
                case DialogueStatus.Active:
                    window.Show();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("after", after, null);
            }
        }
    }
}