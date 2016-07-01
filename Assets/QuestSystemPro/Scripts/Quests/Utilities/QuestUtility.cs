using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Devdog.QuestSystemPro
{
    public static class QuestUtility
    {
        private const string PlayerPrefsCheckedQuestKeyName = "QuestSystemPro_CheckedQuest_";


        public static string GetQuestCheckedSaveKey(Quest quest)
        {
            return PlayerPrefsCheckedQuestKeyName + quest.ID;
        }
    }
}
