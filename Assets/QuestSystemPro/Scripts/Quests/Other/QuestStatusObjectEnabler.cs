using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Devdog.QuestSystemPro
{
    public class QuestStatusObjectEnabler : MonoBehaviour
    {
        public Quest quest;
        public QuestStatus status;

        protected virtual void Start()
        {
            quest.OnStatusChanged += QuestOnStatusChanged;
            QuestOnStatusChanged(quest); // Initial.
        }

        protected virtual void QuestOnStatusChanged(Quest q)
        {
            gameObject.SetActive(q.status == QuestStatus.Active);
        }
    }
}
