using System;
using System.Collections.Generic;
using UnityEngine;

namespace Devdog.QuestSystemPro
{
    public abstract class ObjectTriggerBase : MonoBehaviour
    {
        #region Events

        public delegate void TriggerAction(QuestSystemPlayer player);

        public event TriggerAction OnTriggerUsed;
        public event TriggerAction OnTriggerUnUsed;

        public event TriggerAction OnPlayerCameIntoRange;
        public event TriggerAction OnPlayerWentOutOfRange;

        #endregion

        public abstract float useDistance { get; }

        /// <summary>
        /// Is this triggerer currently active (used)
        /// </summary>
        public bool isInUse { get; protected set; }

        public virtual bool inRange
        {
            get
            {
                if(QuestManager.instance.currentPlayer == null)
                {
                    QuestLogger.Log("Player not found, can't determine distance to trigger. - Considered always in range when no player is present.");
                    return true;
                }

                return QuestManager.instance.currentPlayer.triggerHandler.IsInRangeOfTrigger(this);
            }
        }

        protected virtual void Start()
        {
            enabled = true;
        }

        public virtual bool Toggle(bool fireEvents = true)
        {
            return Toggle(QuestManager.instance.currentPlayer, fireEvents);
        }

        public virtual bool Toggle(QuestSystemPlayer player, bool fireEvents = true)
        {
            if (isInUse)
            {
                return UnUse(player, fireEvents);
            }

            return Use(player, fireEvents);
        }

        public abstract bool Use(bool fireEvents = true);
        public abstract bool Use(QuestSystemPlayer player, bool fireEvents = true);
        public abstract bool UnUse(bool fireEvents = true);
        public abstract bool UnUse(QuestSystemPlayer player, bool fireEvents = true);

        public virtual void NotifyCameInRange(QuestSystemPlayer player)
        {
            if (OnPlayerCameIntoRange != null)
                OnPlayerCameIntoRange(player);
        }

        public virtual void NotifyWentOutOfRange(QuestSystemPlayer player)
        {
            if (OnPlayerWentOutOfRange != null)
                OnPlayerWentOutOfRange(player);
        }

        protected virtual void NotifyTriggerUsed(QuestSystemPlayer player)
        {
            if (OnTriggerUsed != null)
                OnTriggerUsed(player);
        }

        protected virtual void NotifyTriggerUnUsed(QuestSystemPlayer player)
        {
            if (OnTriggerUnUsed != null)
                OnTriggerUnUsed(player);
        }
    }
}
