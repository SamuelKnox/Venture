using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Devdog.QuestSystemPro
{
    public partial class QuestsContainer 
    {


        public HashSet<Quest> completedQuests = new HashSet<Quest>();
        public HashSet<Quest> activeQuests = new HashSet<Quest>();

    }
}