using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.QuestSystemPro.Dialogue
{
    public class DialoguePlayerAutoFocus : AutoFocusBase
    {
        protected override void Awake()
        {
            base.Awake();
            this.type = DialogueOwnerType.Player;
        }

        protected override void SetDialogueCamera()
        {
            var player = GetComponent<QuestSystemPlayer>();
            Assert.IsNotNull(player, "No IDialogueOwner found on DialogueOwnerAutoFocus component.");
            Assert.IsNotNull(player.dialogueCamera, "DialoguePlayerAutoFocus - IDialogueOwner found, but it has no camera, can't auto focus.");

            dialogueCamera = player.dialogueCamera;
        }

        protected override void RegisterEvent()
        {
            DialogueManager.instance.OnCurrentDialogueNodeChanged += OnNodeChanged;
        }
    }
}
