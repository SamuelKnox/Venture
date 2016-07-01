using System;
using UnityEngine;
using UnityEngine.UI;

namespace Devdog.QuestSystemPro.UI
{
    [System.Serializable]
    public class QuestRowModelUI
    {
        public Image icon;

        public Text ownerName;
        public Text description;
        public Text statusMessage;

        public UIShowValue progress;


        public virtual void Repaint(Quest quest)
        {
            if (icon != null)
            {
                icon.sprite = quest.icon;
            }

            SetText(ownerName, quest.name);
            SetText(description, quest.description);
            SetText(statusMessage, string.Empty);
        }

        public virtual void Repaint(Task task)
        {
            if (icon != null)
            {
                icon.sprite = task.owner.icon;
            }

            SetText(ownerName, task.owner.name);
            SetText(description, task.description);
            SetText(statusMessage, task.GetStatusMessage());
        }

        protected void SetText(Text text, string msg)
        {
            if (text != null)
            {
                text.text = msg;
            }
        }
    }
}
