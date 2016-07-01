using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Devdog.QuestSystemPro.Dialogue.UI;
using UnityEngine.Assertions;
using UnityEngine.Networking;


namespace Devdog.QuestSystemPro
{
    public partial class QuestServerManager : QuestManager
    {
        protected override void Awake()
        {
            base.Awake();
        }


        protected ILocalIdentifier GetLocalIdentifierFromNetworkIdentity(NetworkIdentity identity)
        {
            foreach (var q in questStates)
            {
                if (q.Key.identity == identity)
                {
                    return q.Key;
                }
            }

            QuestLogger.LogWarning("Local identifier not found for identity with controller ID: " + identity.playerControllerId);
            return null;
        }


        public override bool HasCompletedQuest(Quest quest)
        {
            QuestLogger.LogVerbose("Checking server ... ");
            CmdHasCompletedQuest(quest);

            bool completed = questStates[quest.localIdentifier].completedQuests.Contains(quest);
            return completed;
        }

        // TODO: Move commands to a different script that inherits from NetworkBehaviour

        protected void CmdHasCompletedQuest(Quest quest)
        {
            bool completed = questStates[quest.localIdentifier].completedQuests.Contains(quest);
            QuestLogger.LogVerbose("(Server) Has " + quest.localIdentifier.identity.playerControllerId + " completed quest: " + completed);
        }

        public override void NotifyQuestTaskStatusChanged(TaskStatus before, TaskStatus after, Task task, Quest quest)
        {
            _NotifyQuestTaskStatusChanged(before, after, task, quest);
        }

        protected void _NotifyQuestTaskStatusChanged(TaskStatus before, TaskStatus after, Task task, Quest quest)
        {
            QuestLogger.LogVerbose("(Server) Quest task " + task.key + " status changed to " + task.isCompleted);
        }

        public override void NotifyQuestTaskProgressChanged(float before, Task task, Quest quest)
        {
            _NotifyQuestTaskProgressChanged(before, task, quest);
        }

        protected void _NotifyQuestTaskProgressChanged(float before, Task task, Quest quest)
        {
            QuestLogger.LogVerbose("(Server) Quest task " + task.key + " progress changed to " + task.progress);
        }

        public override void NotifyQuestStatusChanged(Quest quest)
        {
            _NotifyQuestStatusChanged(quest);
        }

        protected void _NotifyQuestStatusChanged(Quest quest)
        {
            QuestLogger.LogVerbose("(Server) Quest " + quest.name + " status changed to " + quest.status);

            if (quest.status == QuestStatus.Completed)
            {
                questStates[quest.localIdentifier].completedQuests.Add(quest);
                QuestLogger.LogVerbose("(Server) Completed quest " + quest.name);
            }
        }

        public override void NotifyAchievementTaskStatusChanged(TaskStatus before, TaskStatus after, Task task, Achievement achievement)
        {
            _NotifyAchievementTaskStatusChanged(before, after, task, achievement);
        }

        protected void _NotifyAchievementTaskStatusChanged(TaskStatus before, TaskStatus after, Task task, Achievement achievement)
        {
            QuestLogger.LogVerbose("(Server) achievement task " + task.key + " status changed to " + task.isCompleted);
        }

        public override void NotifyAchievementTaskProgressChanged(float before, Task task, Achievement achievement)
        {
            _NotifyAchievementTaskProgressChanged(before, task, achievement);
        }

        protected void _NotifyAchievementTaskProgressChanged(float before, Task task, Achievement achievement)
        {
            QuestLogger.LogVerbose("(Server) achievement task " + task.key + " progress changed to " + task.progress);
        }

        public override void NotifyAchievementStatusChanged(Achievement achievement)
        {
            _NotifyAchievementStatusChanged(achievement);
        }

        protected void _NotifyAchievementStatusChanged(Achievement achievement)
        {
            QuestLogger.LogVerbose("(Server) achievement " + achievement.name + " status changed to " + achievement.status);
        }


#if UNITY_EDITOR
        public override void Reset()
        {
            Awake();

            var manager = FindObjectOfType<NetworkManager>();
            if (manager.isNetworkActive == false)
            {
                manager.StartHost();
            }

//            manager.StopHost();
        }
#endif
    }
}