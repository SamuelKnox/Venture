using System;
using UnityEngine;
using Devdog.QuestSystemPro.ThirdParty.FullSerializer;
using Devdog.QuestSystemPro.UI;

namespace Devdog.QuestSystemPro.Dialogue
{
    [System.Serializable]
    [Summary("Opens or closes the quest window.")]
    [Category("Devdog/Quests")]
    public class ShowQuestWindowNode : NodeBase
    {
        public enum ShowType
        {
            Show,
            Hide,
            Toggle
        }

        [ShowInNode]
        public ShowType showType = ShowType.Show;

        [fsIgnore]
        public QuestWindowUI questWindow { get; set; }

        public override void OnExecute()
        {
            switch (showType)
            {
                case ShowType.Show:
                    questWindow.window.Show();
                    break;
                case ShowType.Hide:
                    questWindow.window.Hide();
                    break;
                case ShowType.Toggle:
                    questWindow.window.Toggle();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Finish(true);
        }
    }
}