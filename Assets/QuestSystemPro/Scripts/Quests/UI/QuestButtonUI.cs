using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Devdog.QuestSystemPro.UI
{
    public class QuestButtonUI : MonoBehaviour
    {
        [Header("UI Elements")]
        public Toggle toggle;

        [Required]
        public Button button;
        public Text questName;
        public Text questDescription;
        public Text questStatus;

        [Header("Visuals & Audio")]
        public AudioClipInfo clickAudioClip;

        protected Quest currentQuest;

        protected virtual void Start()
        {
            if (button != null)
            {
                button.onClick.AddListener(OnClicked);
            }
        }

        public virtual void Repaint(Quest quest)
        {
            currentQuest = quest;
            Assert.IsNotNull(currentQuest, "Given quest for repaint is null! This is not allowed. - If you want to hide it just disable the gameObject.");

            UIUtility.RepaintQuestUIRepaintableChildren(transform, currentQuest);

            if (questName != null)
            {
                questName.text = currentQuest.name;
            }

            if (questDescription != null)
            {
                questDescription.text = currentQuest.description;
            }

            if (questStatus != null)
            {
                questStatus.text = currentQuest.status.ToString();
            }

            if (toggle != null)
            {
                toggle.isOn = PlayerPrefs.HasKey(QuestUtility.GetQuestCheckedSaveKey(quest));
            }
        }

        protected virtual void OnClicked()
        {
            if (clickAudioClip != null)
            {
                AudioManager.AudioPlayOneShot(clickAudioClip);
            }
        }
    }
}