#if PLAYMAKER

using System;
using Devdog.QuestSystemPro.Dialogue;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.QuestSystemPro.Integration.PlayMaker
{
    [ActionCategory(QuestSystemPro.ProductName)]
    [HutongGames.PlayMaker.Tooltip("Set a quest's status.")]
    public class SetQuestStatus : FsmStateAction
    {
        [RequiredField]
        public Quest quest;

        public QuestStatus status;

        public override void OnEnter()
        {
            switch (status)
            {
                case QuestStatus.InActive:
                case QuestStatus.Cancelled:
                    quest.Cancel();
                    break;
                case QuestStatus.Active:
                    quest.Activate();
                    break;
                case QuestStatus.Completed:
                    quest.CompleteAndGiveRewards();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Finish();
        }
    }
}

#endif