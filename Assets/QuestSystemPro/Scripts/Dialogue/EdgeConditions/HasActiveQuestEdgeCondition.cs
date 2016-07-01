using System;
using System.Collections.Generic;
using System.Linq;

namespace Devdog.QuestSystemPro.Dialogue
{
    public class HasActiveQuestEdgeCondition : SimpleQuestEdgeConditionBase
    {
        public override bool CanUse(Dialogue dialogue)
        {
            return quests.All(quest => quest != null && QuestManager.instance.HasActiveQuest(quest.val));
        }
    }
}
