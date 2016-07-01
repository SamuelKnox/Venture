using UnityEngine;
using System.Collections;

namespace Devdog.QuestSystemPro.Dialogue
{
    public interface IDialogueOwner
    {
        string ownerName { get; }
        Sprite ownerIcon { get; }
        Transform transform { get; }

        DialogueCamera dialogueCamera { get; }
        Dialogue dialogue { get; set; }

        void Use();

    }
}