using System;
using UnityEngine;

namespace Devdog.QuestSystemPro.Dialogue
{
    [System.Serializable]
    [Summary("The split node can be used to run multiple outputs. All children (outgoing edges) will be executed.")]
    [Category("Devdog/Flow control")]
    public class SplitNode : ActionNodeBase
    {

        public SplitNode()
        {
            useAutoFocus = false;
        }

        public override void OnExecute()
        {
            Finish(true);
        }
    }
}