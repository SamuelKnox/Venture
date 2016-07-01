#if PLAYMAKER

using System;
using System.Linq;
using System.Runtime.Remoting.Channels;
using Devdog.QuestSystemPro.Dialogue;
using UnityEngine;
using Devdog.QuestSystemPro.Dialogue.UI;
using UnityEngine.Assertions;

namespace Devdog.QuestSystemPro.Integration.PlayMaker
{
    [System.Serializable]
    [Summary("Call a playmaker event.")]
    [Category("PlayMaker")]
    public class InvokePlayMakerEventNode : NodeBase
    {
        public override NodeUIBase uiPrefab
        {
            get { return null; }
        }

        [ShowInNode]
        public string eventName;

        [NonSerialized]
        public new string message;

        protected InvokePlayMakerEventNode()
            : base()
        {

        }

        public override void OnExecute()
        {
            if (DialogueManager.instance.currentDialogueOwner != null)
            {
                var fsm = DialogueManager.instance.currentDialogueOwner.transform.GetComponent<PlayMakerFSM>();
                if (fsm != null)
                {
                    fsm.SendEvent(eventName);
                }
                else
                {
                    QuestLogger.LogWarning("No FSM found on dialogue owner. Event not invoked.");
                }
            }
            else
            {
                QuestLogger.LogWarning("Dialogue not opened through a dialogue owner. No object is set and no FSM can therefore be found.");
            }

            Finish(true);
        }
    }
}

#endif