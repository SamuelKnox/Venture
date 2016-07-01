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
    [HutongGames.PlayMaker.Tooltip("Complete a quest.")]
    public class CompleteQuest : FsmStateAction
    {
        [RequiredField]
        public Quest quest;

        [RequiredField]
        public FsmBool force = false;

        public override void OnEnter()
        {
            quest.CompleteAndGiveRewards(force.Value);
            Finish();
        }
    }
}

#endif