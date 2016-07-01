using UnityEngine;
using System.Collections;
using System;

namespace Devdog.QuestSystemPro.Demo
{
    public class SetQuestProgressOnDestroy : MonoBehaviour
    {
        public enum Type
        {
            Add,
            Set
        }

        public Quest quest;
        public string taskName;

        public Type type = Type.Add;
        public float progress;


        private bool _isQuitting;
        protected void OnApplicationQuit()
        {
            _isQuitting = true;
        }

        protected virtual void OnDestroy()
        {
            if (_isQuitting)
            {
                return;
            }

            switch (type)
            {
                case Type.Add:
                    quest.ChangeTaskProgress(taskName, progress);
                    break;
                case Type.Set:
                    quest.SetTaskProgress(taskName, progress);
                    break;
                default:
                    throw new Exception();
            }
        }
    }
}