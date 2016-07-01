using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.QuestSystemPro.UI;
using UnityEngine;

namespace Devdog.QuestSystemPro
{
    public class QuestGiver : MonoBehaviour, IQuestGiver
    {
        public Quest[] quests = new Quest[0];

        [Header("UI")]
        public SpriteRenderer spriteRenderer;
        public Sprite availableQuest;
        public Sprite completableQuest;

        protected virtual void Start()
        {
            foreach (var quest in quests)
            {
                if (quest == null)
                {
                    QuestLogger.LogWarning("Empty quest object in questgiver", this);
                    continue;
                }

                quest.OnTaskProgressChanged += OnQuestTaskProgressChanged;
                quest.OnStatusChanged += OnQuestStatusChanged;

                OnQuestChanged(quest); // Invoke first time on start.
            }
        }

        protected virtual void OnQuestStatusChanged(Quest quest)
        {
            OnQuestChanged(quest);
        }

        protected virtual void OnQuestTaskProgressChanged(float before, Task task, Quest quest)
        {
            OnQuestChanged(quest);
        }

        protected virtual void OnQuestChanged(Quest quest)
        {
            if (quests.Any(o => o.CanComplete().status))
            {
                Show(completableQuest);
            }
            else if (quests.Any(o => o.CanActivate().status && o.status != QuestStatus.Active))
            {
                Show(availableQuest);
            }
            else
            {
                Show(null);
            }
        }

        public virtual void Show(Sprite sprite)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = sprite;
            }
        }

        protected void OnMouseDown()
        {
            var q = quests.FirstOrDefault(o => o != null && o.status != QuestStatus.Completed);
            if (q != null)
            {
                QuestManager.instance.questWindowUI.Repaint(q);
            }
        }
    }
}
