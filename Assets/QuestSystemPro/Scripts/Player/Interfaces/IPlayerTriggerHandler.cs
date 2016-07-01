using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devdog.QuestSystemPro
{
    public interface IPlayerTriggerHandler
    {
        bool IsInRangeOfTrigger(ObjectTriggerBase trigger);
    }
}
