using System;
using UnityEngine;
using UnityEngine.UI;

namespace Devdog.QuestSystemPro.Dialogue.UI
{
    public class DefaultTimedNodeUI : DefaultNodeUI
    {
        [Header("Options")]
        public float waitTimePerLetter = 0.1f;
        public bool showDefaultPlayerDecisions = true;
        public bool stopAtLeafNode = true;

        protected override void SetText(string msg)
        {
            base.SetText(msg);

            var currentNodeTemp = currentNode;
            TimerUtility.GetTimer().StartTimer(msg.Length * waitTimePerLetter, () =>
            {
                if (currentNodeTemp.isLeafNode == false)
                {
                    currentNodeTemp.Finish(true);
                }
            });
        }

        protected override void SetDefaultPlayerDecision()
        {
            if (showDefaultPlayerDecisions)
            {
                base.SetDefaultPlayerDecision();
            }
        }
    }
}