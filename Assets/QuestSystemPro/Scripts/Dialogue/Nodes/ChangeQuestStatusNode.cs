using UnityEngine;
using System.Collections;
using System;
using System.Linq;

namespace Devdog.QuestSystemPro.Dialogue
{
    [System.Serializable]
    [Category("Devdog/Quests")]
    public class ChangeQuestStatusNode : ActionNodeBase
    {
        [ShowInNode]
        [Required]
        public Asset<Quest>[] quests;

        [ShowInNode]
        public QuestStatus status;

        protected ChangeQuestStatusNode()
        {
            useAutoFocus = false;
        }

        public override void OnExecute()
        {
            foreach (var quest in quests)
            {
                switch (status)
                {
                    case QuestStatus.InActive:
                    case QuestStatus.Cancelled:
                        quest.val.Cancel();
                        break;
                    case QuestStatus.Active:
                        quest.val.Activate();
                        break;
                    case QuestStatus.Completed:
                        quest.val.CompleteAndGiveRewards();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            Finish(true);
        }

        public override ValidationInfo Validate()
        {
            if (quests.Any(o => o == null || o.val == null))
            {
                return new ValidationInfo(ValidationType.Error, "One of the quest fields is empty!");
            }

            return base.Validate();
        }
    }
}