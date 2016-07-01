using System;
using UnityEngine;

namespace Devdog.QuestSystemPro.Dialogue
{
    public abstract class ActionNodeBase : NodeBase
    {
        [NonSerialized]
        public new bool useAutoFocus = false;

        [NonSerialized]
        public new DialogueOwnerType ownerType;

        [NonSerialized]
        public new string message;

        protected ActionNodeBase()
        { }
    }
}