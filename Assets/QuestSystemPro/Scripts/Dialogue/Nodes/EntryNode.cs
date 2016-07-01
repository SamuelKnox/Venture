using UnityEngine;
using System.Collections;

namespace Devdog.QuestSystemPro.Dialogue
{
    [System.Serializable]
    [HideInCreationWindow]
    public class EntryNode : ActionNodeBase
    {


        public EntryNode()
        {
            useAutoFocus = false;
        }

        public override void OnExecute()
        {
            Finish(true);
        }
    }
}