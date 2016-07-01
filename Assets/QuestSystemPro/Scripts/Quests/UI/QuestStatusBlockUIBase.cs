using UnityEngine;
using System.Collections;
using System.Linq;

namespace Devdog.QuestSystemPro.UI
{
    public abstract class QuestStatusBlockUIBase<T> : MonoBehaviour where T : Quest
    {
        public QuestStatus[] status = new QuestStatus[0];

        public bool onlyShowWhenCompletable;
        public bool onlyShowWhenNotCompletable;

        protected virtual void Awake()
        {
            gameObject.SetActive(false); // Disable by default and wait for Repaint() callback.
        }

        public virtual void Repaint(T quest)
        {
            if (status.Contains(quest.status))
            {
                if (onlyShowWhenCompletable)
                {
                    if (quest.CanComplete() == false)
                    {
                        gameObject.SetActive(false);
                        return;
                    }   
                }
                else if (onlyShowWhenNotCompletable)
                {
                    if (quest.CanComplete().status)
                    {
                        gameObject.SetActive(false);
                        return;
                    }
                }

                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}