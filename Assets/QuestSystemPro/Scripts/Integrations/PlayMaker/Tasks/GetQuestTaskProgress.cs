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
    [HutongGames.PlayMaker.Tooltip("Get a quest tasks' progress.")]
    public class GetQuestTaskProgress : FsmStateAction
    {
        [RequiredField]
        public Quest quest;

        [RequiredField]
        public FsmString taskName;

        public FsmFloat result;

        public override void OnEnter()
        {
            result.Value = quest.GetTask(taskName.Value).progress;
            Finish();
        }
    }
}

#endif